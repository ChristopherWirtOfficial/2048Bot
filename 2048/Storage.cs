using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<int> BestTileIndex; // An index to map the best tile value to count of hits, below
        public List<int> BestTileHits; // The actual count of hits for each best tile
        public double AvgTime;
        private DateTime start = DateTime.Now;
        public Storage() {
            count = 0;
            AvgTile = 0;
            AvgScore = 0;
            AvgMoves = 0;
            AvgRatio = 0;
            AvgTime = 0;
            BestScore = null;
            WorstScore = null;
            BestTileHits = new List<int>();
            BestTileIndex = new List<int>();
        }

        public bool AddScore(Score s) {
            if (!BestTileIndex.Contains(s.BestTile)) {
                BestTileIndex.Add(s.BestTile);
                BestTileHits.Add(1);
            } else {
                int index = BestTileIndex.IndexOf(s.BestTile);
                int newVal = BestTileHits.ElementAt(index) + 1;
                BestTileHits.RemoveAt(index);
                BestTileHits.Insert(index, newVal);
            }
            count++;
            AvgTile = ((AvgTile * (count - 1)) + s.BestTile) / count;
            AvgScore = ((AvgScore * (count - 1)) + s.TotalScore) / count;
            AvgMoves = ((AvgMoves * (count - 1)) + s.Moves) / count;
            AvgRatio = ((AvgRatio * (count - 1)) + (s.TotalScore / s.Moves)) / count;
            if (count % 10000 == 0) {
                AvgTime = ((AvgTime * (count - 1)) + (DateTime.Now.Subtract(start).TotalMilliseconds / 10000D)) / count;
                start = DateTime.Now;
            }
            if (WorstScore == null || WorstScore.Moves >= s.Moves) {
                WorstScore = s;
            }
            if (BestScore == null || (BestScore.BestTile < s.BestTile || (BestScore.BestTile == s.BestTile && BestScore.Moves > s.Moves))) {
                BestScore = s;
                return true;
            }
            return false;
        }

        public void Print() {
            Console.WriteLine("========Scores========");
            Console.WriteLine("For {0} scores", count);
            Console.WriteLine("Best Score:");
            Console.WriteLine("\tTile: {0}", BestScore.BestTile);
            Console.WriteLine("\tScore: {0}", BestScore.TotalScore);
            Console.WriteLine("\tMoves: {0}", BestScore.Moves);
            Console.WriteLine("Worst Score:");
            Console.WriteLine("\tTile: {0}", WorstScore.BestTile);
            Console.WriteLine("\tScore: {0}", WorstScore.TotalScore);
            Console.WriteLine("\tMoves: {0}", WorstScore.Moves);
            Console.WriteLine("Average BestTile: {0}", Math.Round(AvgTile, 3));
            Console.WriteLine("Average Score: {0}", Math.Round(AvgScore, 3));
            Console.WriteLine("Average Moves: {0}", Math.Round(AvgMoves, 3)); ;
            Console.WriteLine("Average Score/Move ratio: {0}", Math.Round(AvgRatio, 3));
            Console.WriteLine("Average Time per game: {0}ms", Math.Round(AvgTime, 5));
            Console.WriteLine();
            Console.WriteLine("====Best Tile Hits====");
            for (int i = 0; i < BestTileIndex.Count; i++) {
                int num = BestTileIndex.ElementAt(i);
                int val = BestTileHits.ElementAt(i);
                double percentage = Math.Round(100 * ((double)val / (double)count), 3);
                Console.WriteLine("\t{0}:\t{1} ({2}%)", num, val, percentage);
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
