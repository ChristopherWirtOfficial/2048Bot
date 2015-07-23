using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _2048 {
    class Program
    {
	    private static Random Rand = new Random();
        static void Main(string[] args) {
            string TestType = "RotateCW";

            if (args.Length > 0) {
                TestType = args[0];
            }
            int TestTypeNum = 0;
            switch (TestType) {
                case "RandomSimple":
                    TestTypeNum = 0;
                    break;
                case "RandomNoDouble":
                    TestTypeNum = 1;
                    break;
                case "RotateCW":
                    TestTypeNum = 2;
                    break;
                case "RotateCCW":
                    TestTypeNum = 3;
                    break;
                case "RotateBoth":
                    TestTypeNum = 4;
                    break;
                case "RotateBothRandom":
                    TestTypeNum = 5;
                    break;
            }
	        int progressCheck = 100000;
	        if (args.Length > 1)
	        {
		        int.TryParse(args[1], out progressCheck);
	        }
	        Console.WriteLine("Using progress check of {0}", progressCheck);
			
            string scoresFile = TestType + "scores.json";
            Storage storage;
            try {
                string scores = System.IO.File.ReadAllText(scoresFile);
                storage = JsonConvert.DeserializeObject<Storage>(scores);
	            storage.ScoresFile = scoresFile;
            } catch {
                storage = new Storage(scoresFile, TestType);
            }
	        storage.ProgressCheck = progressCheck;
            DateTime start = DateTime.Now;
            Console.Title = "2048: " + TestType;
            while (true)
            {
	            Parallel.For(0, 10000, i =>
	            {
					RunGameIteration(TestTypeNum, storage);
				});
            }
            try {
                System.IO.File.WriteAllText(scoresFile, JsonConvert.SerializeObject(storage));
            } catch (Exception e) {
                Console.Error.WriteLine(e.ToString());
            }
            storage.Print();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.Error.WriteLine("Done in {0} seconds!", DateTime.Now.Subtract(start).TotalSeconds);
            Console.WriteLine();
            Console.WriteLine();
            Console.ReadLine();
        }

	    private static void RunGameIteration(int testTypeNum, Storage storage)
	    {
			Game game = new Game(4);
			int?[,] startingBoard = new int?[game.size, game.size];
			for (int i = 0; i < game.size; i++)
			{
				for (int j = 0; j < game.size; j++)
				{
					startingBoard[i, j] = game.Board[i, j];
				}
			}

			//game.DisplayBoard();

			int move = 0;
			int temp = 0; // Means different things dependent on algorithm
			while (game.Running)
			{
				switch (testTypeNum)
				{
					case 0: // Random Simple
						move = Rand.Next(0, 4);
						break;
					case 1: // Random no doubles
						move = Rand.Next(0, 4);
						while (move == temp)
						{
							move = Rand.Next(0, 4); // Never do the same move twice in a row
						}
						temp = move;
						break;
					case 2: // Rotate Clockwise
						move = (move + 1) % 4;
						break;
					case 3: // Rotate Counter-Clockwise
						if (move == 0) move = 4;
						move = (move - 1) % 4;
						break;
					case 4: // Rotate both ways, switching at a constant interval
						if (temp >= 20)
							temp = 0;
						if (temp >= 10)
							move = (move - 1) % 4;
						else
							move = (move + 1) % 4;
						temp++;
						break;
					case 5: // Rotate both ways, switching at a random interval
						temp = Rand.Next(0, 20);
						if (temp >= 10)
						{
							if (move == 0) move = 4;
							move = (move - 1) % 4;
						}
						else
						{
							move = (move + 1) % 4;
						}
						break;
				}
				game.MakeMove(move);
			}
			//game.DisplayBoard();
			if (game.Loss)
			{
				//Console.WriteLine("You lost!");
			}
			else
			{
				Console.WriteLine("Something else happened..");
			}
			storage.AddScore(new Score { BestTile = game.CurrentMax, Moves = game.NumberOfMoves, TotalScore = game.TotalScore, Board = game.Board, StartingBoard = startingBoard });
		}
    }
}
