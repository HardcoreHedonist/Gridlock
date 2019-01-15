using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace Gridlock2
{
    // This class is where the reall cool shit happens.
    public class Player
    {
        #region Variables
        int liberties;      // Stores how much freedom a square has.
        public int captured;    // Stores how many squares were captured.
        SquareState number;     // The player's id for their free squares.
        SquareState compromised;    // The player's id for their compromised squares.
        #endregion

        #region __init__
        public Player(SquareState playerNumber, SquareState playerCompromised)
        {
            number = playerNumber;
            compromised = playerCompromised;
        }
        #endregion

        // Counts liberties and finds compromised squares through recursion.
        void FloodCount(int row, int col)
        {
            try
            {
                // Checks if a square is free
                if (MainWindow.CurrentBoard[row,col] == SquareState.Free)
                {
                    // If there's a free square add a liberty and stop searching.
                    this.liberties += 1;
                    return;
                }

                if (MainWindow.CurrentBoard[row,col] != this.number)
                {
                    // If it's an enemy stop searching.
                    return;
                }
                else
                {
                    // If it's not free and not an enemy, it is a compromised square.
                    MainWindow.CurrentBoard[row, col] = this.compromised;
                }
                // Repeat for surrounding squares.
                this.FloodCount(row - 1, col);
                this.FloodCount(row + 1, col);
                this.FloodCount(row, col - 1);
                this.FloodCount(row, col + 1);
            }
            // Stop the program from searching outside the grid.
            catch { IndexOutOfRangeException i; return; }
        }

        // Finds and destroys compromised squares through recursion.
        void FlooDestroy(int row, int col)
        {
            try
            {
                if (MainWindow.CurrentBoard[row,col] != this.compromised)
                {
                    return;
                }

                if (this.liberties <= 0)
                {
                    MainWindow.CurrentBoard[row,col] = SquareState.Free;
                    this.captured++;
                }
                else
                {
                    MainWindow.CurrentBoard[row, col] = this.number;
                }
                this.FlooDestroy(row - 1, col);
                this.FlooDestroy(row + 1, col);
                this.FlooDestroy(row, col - 1);
                this.FlooDestroy(row, col + 1);
            }
            catch { IndexOutOfRangeException i; return; }
        }

        // Method that runs count and destroy on all squares of the grid.
        public void CaptureCount()
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++) // Loops through board.
                {
                    if (MainWindow.CurrentBoard[row,col] == this.number)    //Finds player squares and runs count and destroy methods.
                    {   
                        // Resets liberites for the next iteration.
                        this.liberties = 0;
                        this.FloodCount(row, col);
                        this.FlooDestroy(row, col);
                    }
                }
            }
        }
    }
}
