using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048 {
    public class Game {

        public int size = 4;
        public int CurrentMax = 2;
        public int?[,] Board;
        public bool[,] CanCombine; // A bitmap of spaces that can be combined for a given move
        public int NumberOfMoves = 0;
        public int TotalScore = 0;
        public int Rotation = 0; // Indicates the number of times the board has been rotated clock-wise
        public bool Running = true;
        public bool Loss = false;
        public int[] scores = new int[2049]; // Keep track of the number of each thing we've encountered (Not the number currently on the board)
        public Random Rand;
        public bool dirty = false;
        public Game(int size) {
            Rand = new Random();
            Board = new int?[size, size];
            CanCombine = new bool[size, size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    Board[i, j] = null; // Explicitly initialize to null
                    CanCombine[i, j] = true; // Everything can combine at first
                }
            }
            AddPiece();
            AddPiece();
        }

        public void DisplayBoard() {
            int max = 4; // Max characters inside a block (2048)
            Console.WriteLine("Score: {0} [Max-{1} Moves-{2}] [", TotalScore, CurrentMax, NumberOfMoves);

            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    string inner = Board[i, j] != null ? Board[i, j].ToString() : " ";
                    StringBuilder sb = new StringBuilder();
                    sb.Append(inner);
                    for (int c = 0; c < (max - inner.Length) / 2; c++) {
                        sb.Insert(0, ' ');
                        sb.Append(' ');
                    }
                    string toWrite = "[" + sb.ToString() + "]";
                    Console.Write(toWrite);
                }
                for (int j = 0; j < max; j++) {
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }

        public void RotateBoard() {
            int?[,] temp = new int?[size, size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    temp[i, j] = Board[size - j - 1, i];
                }
            }
            Board = temp;
        }

        public bool HasLost() {
            bool full = true;
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    if (Board[i, j] == null) {
                        full = false;
                        break;
                    }
                    if (!full) {
                        break;
                    }
                }
            }
            if (!full) {
                return false;
            }
            // Only check the board for loss if it's full
            for (int y = 1; y < size; y++) { // X and Y both start at 1 so we can always check up and to the left
                for (int x = 1; x < size; x++) {
                    if (Board[y, x] == Board[y - 1, x] || Board[y, x] == Board[y, x - 1]) {
                        return false;
                    }
                }
            }
            return true; // If we made it this far, then they have indeed lost!
        }

        /// <summary>
        /// Makes the move by first rotating the board, then making a universal left move, then rotating back.
        /// </summary>
        /// <param name="ToRotate">
        /// Amount to rotate the board (clock-wise) to make the move
        /// Left: 0
        /// Down: 1
        /// Right: 2
        /// Up: 3
        /// </param>
        public void MakeMove(int ToRotate) {
            // Rotate back by doing 4 - ToRotate % 4... Or implement a counter-clockwise rotator. But that's work!
            ToRotate = ToRotate % 4;
            /*for (int i = 0; i < ToRotate; i++) {
                //Console.WriteLine("Rotating[" + i + "]");
                RotateBoard();
                //DisplayBoard();
            }*/
            dirty = false; // The board starts out clean, no changes.
            switch (ToRotate) {
                case 0:
                    MoveLeft();
                    break;
                case 1:
                    MoveDown();
                    break;
                case 2:
                    MoveRight();
                    break;
                case 3:
                    MoveUp();
                    break;
            }
            
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    CanCombine[i, j] = true; // Everything can combine again
                }
            }
            if (dirty) {
                NumberOfMoves++;
                AddPiece(); // Only add a piece if they just made a real move. If nothing happened, then don't do anyting.
            }
            if (HasLost()) {
                Running = false;
                Loss = true;
            }
        }

        public void MoveLeft() {
            for (int y = 0; y < size; y++) {
                for (int x = 1; x < size; x++) { // Start at x=1, because we'll never move x=0
                    int? p = Board[y, x]; // The piece at x,y
                    if (p != null) {
                        // There's a piece here!
                        int tempX = x - 1;
                        while (true) {
                            int? temp = null;
                            if (tempX > 0) {
                                temp = Board[y, tempX]; // Piece we're checking against right now
                                if (temp == null) {
                                    tempX--;
                                    continue;
                                }
                            }
                            temp = Board[y, tempX]; // Piece we're checking against right now
                            // At this point, we're either at either x=0 or a non-empty space
                            if (temp == null) { // The farthest left thing is null, replace it
                                dirty = true;
                                Board[y, tempX] = p;
                                Board[y, x] = null;
                            } else if (!CanCombine[y, tempX] || temp != p) { // Either way, we need to just put it to the right of tempX
                                if (tempX + 1 != x) {
                                    dirty = true;
                                    Board[y, tempX + 1] = p;
                                    Board[y, x] = null;
                                    // Only do things if it's actually a different spot..
                                }

                            } else if (temp == p) {
                                dirty = true;
                                int val = (int)(2 * p);
                                if (val > CurrentMax) {
                                    CurrentMax = val;
                                }
                                Board[y, tempX] = val;
                                scores[val]++; // We just saw one more of 2*p
                                TotalScore += val;
                                Board[y, x] = null;
                                CanCombine[y, tempX] = false; // We just combined, so now we can't anymore.
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void MoveDown() {
            for (int x = 0; x < size; x++) {
                for (int y = 0; y < size-1; y++) { // Start at x=1, because we'll never move x=0
                    int? p = Board[y, x]; // The piece at x,y
                    if (p != null) {
                        // There's a piece here!
                        int tempY = y + 1;
                        while (true) {
                            int? temp = null;
                            if (tempY < size - 1) {
                                temp = Board[tempY, x]; // Piece we're checking against right now
                                if (temp == null) {
                                    tempY++;
                                    continue;
                                }
                            }
                            temp = Board[tempY,x]; // Piece we're checking against right now
                            // At this point, we're either at either y=size-1 or a non-empty space
                            if (temp == null) { // The farthest down thing is null, replace it
                                dirty = true;
                                Board[tempY, x] = p;
                                Board[y, x] = null;
                            } else if (!CanCombine[tempY, x] || temp != p) { // Either way, we need to just put it to the right of tempX
                                if (tempY - 1 != y) {
                                    dirty = true;
                                    Board[tempY - 1, x] = p;
                                    Board[y, x] = null;
                                    // Only do things if it's actually a different spot..
                                }

                            } else if (temp == p) {
                                dirty = true;
                                int val = (int)(2 * p);
                                if (val > CurrentMax) {
                                    CurrentMax = val;
                                }
                                Board[tempY, x] = val;
                                scores[val]++; // We just saw one more of 2*p
                                TotalScore += val;
                                Board[y, x] = null;
                                CanCombine[tempY, x] = false; // We just combined, so now we can't anymore.
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void MoveRight() {
            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size - 1; x++) {
                    int? p = Board[y, x]; // The piece at x,y
                    if (p != null) {
                        // There's a piece here!
                        int tempX = x + 1;
                        while (true) {
                            int? temp = null;
                            if (tempX < size-1) {
                                temp = Board[y, tempX]; // Piece we're checking against right now
                                if (temp == null) {
                                    tempX++;
                                    continue;
                                }
                            }
                            temp = Board[y, tempX]; // Piece we're checking against right now
                            // At this point, we're either at either x=0 or a non-empty space
                            if (temp == null) { // The farthest left thing is null, replace it
                                dirty = true;
                                Board[y, tempX] = p;
                                Board[y, x] = null;
                            } else if (!CanCombine[y, tempX] || temp != p) { 
                                if (tempX - 1 != x) {
                                    dirty = true;
                                    Board[y, tempX - 1] = p;
                                    Board[y, x] = null;
                                    // Only do things if it's actually a different spot..
                                }

                            } else if (temp == p) {
                                dirty = true;
                                int val = (int)(2 * p);
                                if (val > CurrentMax) {
                                    CurrentMax = val;
                                }
                                Board[y, tempX] = val;
                                scores[val]++; // We just saw one more of 2*p
                                TotalScore += val;
                                Board[y, x] = null;
                                CanCombine[y, tempX] = false; // We just combined, so now we can't anymore.
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void MoveUp() {
            for (int x = 0; x < size; x++) {
                for (int y = 1; y < size; y++) { // Start at x=1, because we'll never move x=0
                    int? p = Board[y, x]; // The piece at x,y
                    if (p != null) {
                        // There's a piece here!
                        int tempY = y - 1;
                        while (true) {
                            int? temp = null;
                            if (tempY > 0) {
                                temp = Board[tempY, x]; // Piece we're checking against right now
                                if (temp == null) {
                                    tempY--;
                                    continue;
                                }
                            }
                            temp = Board[tempY, x]; // Piece we're checking against right now
                            // At this point, we're either at either y=size-1 or a non-empty space
                            if (temp == null) { // The farthest down thing is null, replace it
                                dirty = true;
                                Board[tempY, x] = p;
                                Board[y, x] = null;
                            } else if (!CanCombine[tempY, x] || temp != p) { // Either way, we need to just put it to the right of tempX
                                if (tempY + 1 != y) {
                                    dirty = true;
                                    Board[tempY + 1, x] = p;
                                    Board[y, x] = null;
                                    // Only do things if it's actually a different spot..
                                }

                            } else if (temp == p) {
                                dirty = true;
                                int val = (int)(2 * p);
                                if (val > CurrentMax) {
                                    CurrentMax = val;
                                }
                                Board[tempY, x] = val;
                                scores[val]++; // We just saw one more of 2*p
                                TotalScore += val;
                                Board[y, x] = null;
                                CanCombine[tempY, x] = false; // We just combined, so now we can't anymore.
                            }
                            break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Adds a piece to a random location on the board.
        /// </summary>
        public void AddPiece() {
            int p = Rand.Next(0, 10) == 0 ? 4 : 2;

            TotalScore += p;
            XYPair[] openSpots = new XYPair[size * size]; // Largest possible
            int n=0;
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    if (Board[i, j] == null) {
                        openSpots[n].x = j;
                        openSpots[n++].y = i;
                    }
                }
            }
            while (n > 1) {
                n--;
                int k = Rand.Next(n + 1);
                XYPair value = openSpots[k];
                openSpots[k] = openSpots[n];
                openSpots[n] = value;
            }
            // We've now shuffled the list and can take the first one!
            XYPair pair = openSpots[0];
            Board[pair.y, pair.x] = p;
        }
    }

    public struct XYPair {
        public int x, y;

        public XYPair(int p1, int p2) {
            x = p1;
            y = p2;
        }
    }
}
