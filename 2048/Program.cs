using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _2048
{
    class Program
    {
        static void Main(string[] args)
        {
            Random Rand = new Random();
            string TestType = "RotateCW";
            if (args.Length > 0)
            {
                TestType = args[0];
            }
            string scoresFile = @"C:\Users\Chris\Documents\Workspace\2048\2048\" + TestType + "scores.json";
            Storage storage = null;
            try
            {
                string scores = System.IO.File.ReadAllText(scoresFile);
                storage = JsonConvert.DeserializeObject<Storage>(scores);
            }
            catch
            {
                storage = new Storage();
            }
            Console.WriteLine("Starting {0} ({1})", TestType, scoresFile);
            DateTime start = DateTime.Now;
            while (true)
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
                while (game.Running)
                {
                    move = (move + 1) % 4;
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
                if (storage.AddScore(new Score { BestTile = game.CurrentMax, Moves = game.NumberOfMoves, TotalScore = game.TotalScore, Board = game.Board, StartingBoard = startingBoard }))
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Error.WriteLine("New High Score!");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Number of valid moves made: {0}", game.NumberOfMoves);
                    Console.WriteLine("Final Score: {0}!", game.TotalScore);
                    Console.WriteLine("Largest tile: {0}!", game.CurrentMax);
                    try
                    {
                        System.IO.File.WriteAllText(scoresFile, JsonConvert.SerializeObject(storage));
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.ToString());
                    }
                }
                int check = 10000;
                if (storage.count % check == 0)
                {
                    try
                    {
                        System.IO.File.WriteAllText(scoresFile, JsonConvert.SerializeObject(storage));
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.ToString());
                    }
                    storage.Print();
                }
            }
            try
            {
                System.IO.File.WriteAllText(scoresFile, JsonConvert.SerializeObject(storage));
            }
            catch (Exception e)
            {
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
    }
}
