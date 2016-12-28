using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleBlockSlider
{

    //Simple 2D point structure
    public class CInt2D
    {
        private int x;
        private int y;
        /// <summary>
        /// Get or Set the value of X
        /// </summary>
        public int X
        {
            get { return x; }
            set { x = value; }

        }

        /// <summary>
        /// Get or Set the value of Y
        /// </summary>
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public CInt2D(int iValueX, int iValueY)
        {
            x = iValueX;
            y = iValueY;
        }

        /// <summary>
        /// Compares input i2dCompare to the current structure and returns true if both x, y are equivelant
        /// </summary>
        /// <param name="i2dCompare"></param>
        /// <returns></returns>
        public bool Equals2D(CInt2D i2dCompare)
        {
            if ((i2dCompare.x == x) && (i2dCompare.y == y))
            {
                return true;
            }

            return false;
        }//end of Equal2D

        /// <summary>
        /// Compares input i2dCompare for only x value and returns true if input and this are equal
        /// </summary>
        /// <param name="i2dCompare"></param>
        /// <returns></returns>
        public bool EqualsX(CInt2D i2dCompare)
        {
            if (i2dCompare.x == x)
            {
                return true;
            }

            return false;
        }//end of EqualX(CInt2D i2dCompare)

        /// <summary>
        /// Compares input iCompare for only x value and returns true if input and this are equal
        /// </summary>
        /// <param name="iCompare"></param>
        /// <returns></returns>
        public bool EqualsX(int iCompare)
        {
            if (iCompare == x)
            {
                return true;
            }

            return false;
        }//end of EqualX(int iCompare)

        /// <summary>
        /// Compares input i2dCompare for only y value and returns true if input and this are equal
        /// </summary>
        /// <param name="i2dCompare"></param>
        /// <returns></returns>
        public bool EqualsY(CInt2D i2dCompare)
        {
            if (i2dCompare.y == y)
            {
                return true;
            }

            return false;
        }//end of EqualY(CInt2D i2dCompare)

        /// <summary>
        /// Compares input iCompare for only y value and returns true if input and this are equal
        /// </summary>
        /// <param name="iCompare"></param>
        /// <returns></returns>
        public bool EqualsY(int iCompare)
        {
            if (iCompare == y)
            {
                return true;
            }

            return false;
        }//end of EqualY(int iCompare)
    }; //end of CInt2D


    //Game block information (stores information about each block)
    public class CGameBlock
    {
        public int iContent;
        public CInt2D Position;

        public CGameBlock()
        {
            iContent = 0;
            Position = new CInt2D(0, 0);
        }

        public CGameBlock GetCopy()
        {
            return this;
        }

        public void CopyFrom(CGameBlock GameBlock)
        {
            if (GameBlock == null)
            {
                //GameBlock doesn't exist, do not copy
                return;
            }

            iContent = GameBlock.iContent;
            Position = GameBlock.Position;
        }

        public static bool Copy(ref CGameBlock DstGameBlock, ref CGameBlock SrcGameBlock)
        {
            DstGameBlock.iContent = SrcGameBlock.iContent;
            DstGameBlock.Position = SrcGameBlock.Position;

            return true;
        }
    }


    //Simple Block Movement Game
    public class CNumberBlockSequenceGame : System.Object
    {
        //Current Game Settings
        public struct sGameSettings
        {
            public int iCols;
            public int iRows;
            public int iBlocks;
            public CInt2D GameBoardSize;
            public CInt2D GameBlockSize;
            public CGameBlock[] aInitialGameBlocks;
            public CGameBlock[] aGameBlocks;
        }

        //game settings including game board & blocks
        protected sGameSettings GameSettings;


        //Get the number of Game Blocks that will be used
        public int NumberOfGameBlocks
        {
            get
            {
                return GameSettings.iBlocks;
            }
        }


        //Get a copy of the GameBlocks for layout purposes
        public CGameBlock[] GameBlocks
        {
            get
            {
                CGameBlock[] gbCopied = new CGameBlock[GameSettings.iBlocks];
                GameSettings.aGameBlocks.CopyTo(gbCopied, 0);
                return gbCopied;
            }
        }

        //Get a copy of the GameBlockSize information
        public CInt2D GameBlockSize
        {
            get
            {
                return new CInt2D(GameSettings.GameBlockSize.X, GameSettings.GameBlockSize.Y);
            }
        }

        //Are the numbers now sequenced
        public bool NumbersAreSequenced
        {
            get
            {
                return AreNumbersSequenced();
            }
        }

        public CNumberBlockSequenceGame() { }

        //Verify that 
        public bool IsInitialized()
        {
            //Check to see if objects have been allocated / assigned
            if (GameSettings.aInitialGameBlocks == null || GameSettings.aGameBlocks == null)
            {
                return false;
            }

            //Print out the result of the initialization
            //foreach (CGameBlock Block in GameSettings.aInitialGameBlocks)
            //{
            //    System.Diagnostics.Debug.WriteLine(Block.iContent);
            //}

            return true;
            
        }//end of Initialized

        /// <summary>
        /// Initializes the game with the specified Rows, Cols and tracks UI 
        /// elements using BoardWidthPx (in Pixels) and BoardHeightPx (in Pixels)
        /// </summary>
        /// <param name="iRows"></param>
        /// <param name="iCols"></param>
        /// <param name="iBoardWidthPx"></param>
        /// <param name="iBoardHeightPx"></param>
        /// <returns></returns>
        public bool InitializeGame(int iCols, int iRows, int iBoardWidthPx, int iBoardHeightPx)
        {
            //Validate input settings
            if ((iRows > 6 || iRows < 2) || 
                (iCols > 6 || iCols < 2) ||
                (iBoardWidthPx > 2048 || iBoardWidthPx < 100) ||
                (iBoardHeightPx > 2048 || iBoardHeightPx < 100 ))
            {
                //Won't initialize game with less than 2 rows or greater than 10 rows
                // also won't initialize game with a board UI greater than 9999 pixels or less than 100 pixels
                return false;
            }
            
            //Copy the desired game initialization settings for later use
            GameSettings.iCols = iCols;
            GameSettings.iRows = iRows;
            GameSettings.iBlocks = (iRows * iCols);
            GameSettings.GameBoardSize = new CInt2D(iBoardWidthPx,iBoardHeightPx);
            GameSettings.GameBlockSize = new CInt2D( (iBoardWidthPx / iCols), (iBoardHeightPx / iRows) );

            //Allocate game block list
            GameSettings.aInitialGameBlocks = new CGameBlock[GameSettings.iBlocks];
            GameSettings.aGameBlocks = new CGameBlock[GameSettings.iBlocks];

            //Validate allocation of game block list
            if (GameSettings.aInitialGameBlocks == null || GameSettings.aGameBlocks == null)
            {
                return false;
            }

            //Create random order of 1 to [iNumBlocks]
            if (InitializeGameBlocks() == false)
            {
                //Failed to create the random order
                return false;
            }

            //Copy the random order to the CurrentNumberSequence
            GameSettings.aGameBlocks.CopyTo(GameSettings.aInitialGameBlocks, 0);


            return true;
        }//end of InitializeGame

        /// <summary>
        /// Are the numbers sequenced from 1 to N (if so then the game should be concluded)
        /// </summary>
        /// <returns></returns>
        private bool AreNumbersSequenced()
        {
            //Cycle through values
            for (int iBlock = 0; iBlock < GameSettings.iBlocks; iBlock++)
            {
                //Validate 1,2,3,4,5,...,n
                if (GameSettings.aGameBlocks[iBlock].iContent != (iBlock + 1))
                {
                    return false;
                }
            }

            return true;
        }//end of AreNumbersSequenced


        //Only modify CurrentNumberSequence for moving pieces about
        public bool TryMovePiece(int iContent)
        {
            int iBlockIdx = -1;
            int iZeroIdx = -1;

            //Get current position
            for (int iBlock = 0; iBlock < GameSettings.iBlocks; iBlock++)
            {
                if (GameSettings.aGameBlocks[iBlock].iContent == iContent)
                {
                    iBlockIdx = iBlock;
                }

                if (GameSettings.aGameBlocks[iBlock].iContent == 0)
                {
                    iZeroIdx = iBlock;
                }
            }

            /*check to see if '0' block can be swapped with*/
            //Check to see if the piece that is attempting to move is in the same column as the empty spot
            if ( GameSettings.aGameBlocks[iBlockIdx].Position.EqualsX(GameSettings.aGameBlocks[iZeroIdx].Position) && (
                 GameSettings.aGameBlocks[iZeroIdx].Position.EqualsY(GameSettings.aGameBlocks[iBlockIdx].Position.Y - GameSettings.GameBlockSize.Y) ||
                 GameSettings.aGameBlocks[iZeroIdx].Position.EqualsY(GameSettings.aGameBlocks[iBlockIdx].Position.Y + GameSettings.GameBlockSize.Y) ) )
            {
                CGameBlock TempBlock = new CGameBlock();

                TempBlock.CopyFrom(GameSettings.aGameBlocks[iBlockIdx]);
                GameSettings.aGameBlocks[iBlockIdx].CopyFrom(GameSettings.aGameBlocks[iZeroIdx]);
                GameSettings.aGameBlocks[iZeroIdx].CopyFrom(TempBlock);

                AdjustGameBlock(iBlockIdx);
                AdjustGameBlock(iZeroIdx);

                TempBlock = null; //can GC TempBlock immediately
            }

            //Check to see if the piece that is attempting to move is in the same row as the empty spot
            if (GameSettings.aGameBlocks[iBlockIdx].Position.EqualsY(GameSettings.aGameBlocks[iZeroIdx].Position) && (
                 GameSettings.aGameBlocks[iZeroIdx].Position.EqualsX(GameSettings.aGameBlocks[iBlockIdx].Position.X - GameSettings.GameBlockSize.X) ||
                 GameSettings.aGameBlocks[iZeroIdx].Position.EqualsX(GameSettings.aGameBlocks[iBlockIdx].Position.X + GameSettings.GameBlockSize.X)))
            {
                CGameBlock TempBlock = new CGameBlock();

                TempBlock.CopyFrom(GameSettings.aGameBlocks[iBlockIdx]);
                GameSettings.aGameBlocks[iBlockIdx].CopyFrom(GameSettings.aGameBlocks[iZeroIdx]);
                GameSettings.aGameBlocks[iZeroIdx].CopyFrom(TempBlock);

                AdjustGameBlock(iBlockIdx);
                AdjustGameBlock(iZeroIdx);

                TempBlock = null; //can GC TempBlock immediately
            }

            return true;
        }//end of TryMovePiece

        /// <summary>
        /// Initializes the aGameBlocks array and sets default values for their settings
        /// </summary>
        /// <returns></returns>
        private bool InitializeGameBlocks()
        {
            //Validation of the number of blocks in unnecessary here 
            // due to earlier validation during the initialize step

            //Initialize random class
            Random rnd = new Random();
            int iValue = 0;
            bool bValueUsed = false;

            //Validate creation of object
            if (rnd == null)
            {
                return false;
            }

            //Allocate each of the GameBlocks
            for (int iBlock = 0; iBlock < GameSettings.iBlocks; iBlock++)
            {
                //Allocate individual block
                GameSettings.aGameBlocks[iBlock] = new CGameBlock();

                //Validate allocated block
                if (GameSettings.aGameBlocks[iBlock] == null)
                {
                    //Failed to allocate blocks
                    System.Diagnostics.Debug.WriteLine("Failed to allocate block");
                    return false;
                }

                //Set the initial content value to "-1" for allowing assignment of "0" as a random value
                GameSettings.aGameBlocks[iBlock].iContent = -1;
            }

            //Set random (serial random list) numbers for blocks
            for (int iBlock = 0; iBlock < GameSettings.iBlocks; iBlock++)
            {
                //Validate the current random value
                do
                {
                    //Reset the check flag
                    bValueUsed = false;

                    //Assign a random (serial) value
                    iValue = rnd.Next(GameSettings.iBlocks);

                    //Go through all the numbers
                    for (int iCheck = 0; iCheck < GameSettings.iBlocks; iCheck++)
                    {
                        //Validate if random value is already used
                        if (GameSettings.aGameBlocks[iCheck].iContent == iValue)
                        {
                            bValueUsed = true;
                        }
                    }
                } while (bValueUsed);

                //Set the [serial] random number for the game block
                GameSettings.aGameBlocks[iBlock].iContent = iValue;

                //Setup initial block size and position according to the game board
                //if (AdjustGameBlock(iBlock) == false)
                //{
                //    //Failed to adjust the game block, fail initialize
                //    return false;
                //}
            }

            return true;
        }//end of InitializeGameBlocks

        
        /// <summary>
        /// Adjust the number of columns for the board (N (Columns) x M (Rows) )
        ///  changing this value will reset the game
        /// </summary>
        /// <param name="iCols"></param>
        /// <returns></returns>
        public bool AdjustNumberOfCols(int iCols)
        {
            System.Diagnostics.Debug.WriteLine("AdjustNumberOfCols:" + iCols.ToString());

            return AdjustNumberOfColsAndRows(new CInt2D(iCols, GameSettings.iRows));
        }

        /// <summary>
        /// Adjust the number of rows for the board (N (Columns) x M (Rows) )
        ///   changing this value will reset the game
        /// </summary>
        /// <param name="iRows"></param>
        /// <returns></returns>
        public bool AdjustNumberOfRows(int iRows)
        {
            System.Diagnostics.Debug.WriteLine("AdjustNumberOfRows:" + iRows.ToString());

            return AdjustNumberOfColsAndRows(new CInt2D(GameSettings.iCols, iRows)); ;
        }

        /// <summary>
        /// Adjust the number of colrows for the board (N (Columns) x M (Rows) )
        ///   changing this value will reset the game
        /// </summary>
        /// <param name="i2dColsRows"></param>
        /// <returns></returns>
        public bool AdjustNumberOfColsAndRows(CInt2D i2dColsRows)
        {
            //Validate the input
            if (i2dColsRows.X < 2 || i2dColsRows.X > 6 || i2dColsRows.Y < 2 || i2dColsRows.Y > 6)
            {
                return false;
            }

            //Validate the settings have changed (if they haven't, still return true)
            if (i2dColsRows.X == GameSettings.iCols && i2dColsRows.Y == GameSettings.iRows)
            {
                return true;
            }


            System.Diagnostics.Debug.WriteLine("iColumns:" + i2dColsRows.X.ToString() +
                                                "\tiRows:" + i2dColsRows.Y.ToString() );

            return InitializeGame(i2dColsRows.X, 
                                  i2dColsRows.Y,
                                  GameSettings.GameBoardSize.X,
                                  GameSettings.GameBoardSize.Y);
        }


        /// <summary>
        /// Adjust the size of the Game Board this then calls in to AdjustGameBlockSize (debate the need for seperate functions...)
        /// </summary>
        /// <returns></returns>
        public bool AdjustGameBoardSize( CInt2D GameBoardSize )
        {
            //Validate input settings
            if ((GameBoardSize.X > 2048 || GameBoardSize.X < 100) ||
                (GameBoardSize.Y > 2048 || GameBoardSize.Y < 100))
            {
                //Won't adjust the game board with outrageous dimensions
                return false;
            }

            //Set the game board size to the input of GameBoardSize
            GameSettings.GameBoardSize = GameBoardSize;

            //Adjust the game block size based upon the game board size
            GameSettings.GameBlockSize = new CInt2D((GameBoardSize.X / GameSettings.iCols),
                                                    (GameBoardSize.Y / GameSettings.iRows));

            //Adjust all the GameBlocks
            if (AdjustGameBlocks() == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adjust the size of the Game Blocks (this is based upon the size of the Game Board)
        /// </summary>
        /// <returns></returns>
        public bool AdjustGameBlocks()
        {
            //Validate that the aGameBlocks array has been initialized
            if (GameSettings.aGameBlocks == null)
            {
                return false;
            }

            //Adjust the Game Blocks size based upon the board size
            for (int iBlock = 0; iBlock < GameSettings.iBlocks; iBlock++)
            {
                //Call in to AdjustGameBlock for the specific Block
                if (AdjustGameBlock(iBlock) == false)
                {
                    //Some kind of failure occured (probably should debug message this one if it occured)
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adjust the size and position of a single game block based upon the input
        /// </summary>
        /// <returns></returns>
        public bool AdjustGameBlock( int iGameBlock )
        {
            //Validate that the aGameBlocks array has been initialized
            if (GameSettings.aGameBlocks == null)
            {
                return false;
            }

            //Validate that iGameBlock is within the array
            if (iGameBlock < 0 || iGameBlock >= GameSettings.iBlocks)
            {
                return false;
            }

            //Adjust the Game Blocks size and location based upon the board size
            GameSettings.aGameBlocks[iGameBlock].Position.X = GameSettings.GameBlockSize.X * (iGameBlock % GameSettings.iCols);
            GameSettings.aGameBlocks[iGameBlock].Position.Y = GameSettings.GameBlockSize.Y * (iGameBlock / GameSettings.iCols);

           //System.Diagnostics.Debug.WriteLine("iGameBlock:" + iGameBlock.ToString() +
           //                                    "\tiContent:" + GameSettings.aGameBlocks[iGameBlock].iContent.ToString() +
           //                                    "\tX-Pos:" + (iGameBlock % GameSettings.iCols).ToString() +
           //                                    "\tY-Pos:" + (iGameBlock / GameSettings.iCols).ToString() + 
           //                                    "\tBlockSize(X,Y):" + GameSettings.GameBlockSize.X.ToString() + ", " + GameSettings.GameBlockSize.Y.ToString() +
           //                                    "\tBoardSize(X,Y):" + GameSettings.GameBoardSize.X.ToString() + ", " + GameSettings.GameBoardSize.Y.ToString() );

            return true;
        }
    }//end of CNumberBlockSequenceGame

}
