using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;

namespace _2048 {
    public class Storage {
        //public List<Score> Scores;
        public Score BestScore; // Judged by BestTile
        public Score WorstScore; // Judged by Number of Moves (also interesting)
        public long count;
        public double AvgTile;
        public double AvgScore;
        public double AvgMoves;
        public double AvgRatio; // Ratio of Score/Moves
        public List<BestTileHit> BestTileHits; // The actual count of hits for each best tile
        public double AvgTime;
        private DateTime start = DateTime.Now;
	    public string TestType;
	    public int ProgressCheck = 10000;
		[JsonIgnore]
	    public string ScoresFile;
		[JsonIgnore]
	    private Thread QueueTimer;

	    public Storage()
	    {
			count = 0;
			AvgTile = 0;
			AvgScore = 0;
			AvgMoves = 0;
			AvgRatio = 0;
			AvgTime = 0;
			BestScore = null;
			WorstScore = null;
			BestTileHits = new List<BestTileHit>();
			QueueTimer = new Thread(QueueChecker);
			//QueueTimer.Priority = ThreadPriority.AboveNormal;
			QueueTimer.Start();
	    }

        public Storage(string scoresFile, string testType) : this() {
	        ScoresFile = scoresFile;
	        TestType = testType;
        }
		private ConcurrentQueue<Score> scoreList = new ConcurrentQueue<Score>();

        public void AddScore(Score s) {
			scoreList.Enqueue(s);
        }

	    private void QueueChecker()
	    {
		    while (true)
		    {
				while (!scoreList.IsEmpty)
				{ 
					Score s;
					if (scoreList.TryDequeue(out s))
					{
						bool found = false;
						foreach (BestTileHit h in BestTileHits)
						{
							if (h.Tile == s.BestTile)
							{
								h.Number++;
								found = true;
								break;
							}
						}
						if (!found)
						{
							BestTileHits.Add(new BestTileHit { Tile = s.BestTile, Number = 1 });
							BestTileHits.Sort();
						}
						count++;
						AvgTile = ((AvgTile * (count - 1)) + s.BestTile) / count;
						AvgScore = ((AvgScore * (count - 1)) + s.TotalScore) / count;
						AvgMoves = ((AvgMoves * (count - 1)) + s.Moves) / count;
						AvgRatio = ((AvgRatio * (count - 1)) + ((double)s.TotalScore / s.Moves)) / count;
						if (count % 10000 == 0)
						{
							AvgTime = ((AvgTime * (count - 1)) + (DateTime.Now.Subtract(start).TotalMilliseconds / 10000D)) / count;
							start = DateTime.Now;
						}
						if (WorstScore == null || WorstScore.Moves >= s.Moves)
						{
							WorstScore = s;
						}
						if (BestScore == null || (BestScore.BestTile < s.BestTile || (BestScore.BestTile == s.BestTile && BestScore.Moves > s.Moves)))
						{
							BestScore = s;
							OnNewBestScore();
						}

						if (count % ProgressCheck == 0)
						{
							try
							{
								System.IO.File.WriteAllText(ScoresFile, JsonConvert.SerializeObject(this));
							}
							catch (Exception e)
							{
								Console.Error.WriteLine(e.ToString());
							}
							Print();
						}
					}
				}
				Thread.Sleep(25);
			}
	    }

	    private void OnNewBestScore()
	    {
			Console.Title = TestType + ": " + BestScore.BestTile + " (" + BestScore.Moves + " moves)";
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			Console.Error.WriteLine("New High Score!");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Number of valid moves made: {0}", BestScore.Moves);
			Console.WriteLine("Final Score: {0}!", BestScore.TotalScore);
			Console.WriteLine("Largest tile: {0}!", BestScore.BestTile);
			try
			{
				System.IO.File.WriteAllText(ScoresFile, JsonConvert.SerializeObject(this));
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.ToString());
			}
		}

        public void Print() {
            Console.WriteLine("========Scores========");
            Console.WriteLine("For {0:n0} scores", count);
            Console.WriteLine("Best Score:");
            Console.WriteLine("\tTile: {0}", BestScore.BestTile);
            Console.WriteLine("\tScore: {0:n0}", BestScore.TotalScore);
            Console.WriteLine("\tMoves: {0:n0}", BestScore.Moves);
            Console.WriteLine("Worst Score:");
            Console.WriteLine("\tTile: {0}", WorstScore.BestTile);
            Console.WriteLine("\tScore: {0:n0}", WorstScore.TotalScore);
            Console.WriteLine("\tMoves: {0:n0}", WorstScore.Moves);
            Console.WriteLine("Average BestTile: {0:n}", Math.Round(AvgTile, 3));
            Console.WriteLine("Average Score: {0:n}", Math.Round(AvgScore, 3));
            Console.WriteLine("Average Moves: {0:n}", Math.Round(AvgMoves, 3)); ;
            Console.WriteLine("Average Score/Move ratio: {0:n}", Math.Round(AvgRatio, 3));
            Console.WriteLine("Average Time per game: {0:n}ms", Math.Round(AvgTime, 5));
            Console.WriteLine();
            Console.WriteLine("====Best Tile Hits====");
            foreach(BestTileHit h in BestTileHits) {
                int val = h.Number;
                double percentage = Math.Round(100 * ((double)val / (double)count), 3);
                Console.WriteLine("\t{0}:\t{1:n0} ({2}%)", h.Tile, val, percentage);
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }

    public class BestTileHit : IComparable<BestTileHit> {
        public int Tile;
        public int Number;

        public int CompareTo(BestTileHit other) {
            return this.Tile - other.Tile;
        }
    }
}
