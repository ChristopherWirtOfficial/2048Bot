using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048 {
    class TestProgram {
        static void Main(string[] args) {
            Game game = new Game(4);
            game.DisplayBoard();
            while (game.Running) {
                try {
                    game.MakeMove(int.Parse(Console.ReadLine()));
                    game.DisplayBoard();
                } catch {
                    Console.WriteLine("Nope..");
                }
            }
            Console.WriteLine("Number of valid moves made: {0}", game.NumberOfMoves);
            Console.WriteLine("Final Score: {0}!", game.TotalScore);
            Console.WriteLine("Largest tile: {0}!", game.CurrentMax);
            Console.ReadLine();
        }
    }
}
