using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048 {
    public class Score {
        public int TotalScore;
        public int Moves;
        public int BestTile;
        public int?[,] Board;
        public int?[,] StartingBoard;
    }
}
