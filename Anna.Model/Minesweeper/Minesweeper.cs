// Original source: https://github.com/jozsefsallai/discord.js-minesweeper
//
// Copyright 2019 József Sallai
//  
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using System;

namespace Anna.Model.MineField
{
    /// <summary>
    /// <para>Used for creating random games of minesweeper for Discord.</para>
    /// <para>Specify your own arguments in the constructor for a custom the game.</para>
    /// </summary>
    public class Minesweeper
    {
        #region Constructor
        /// <summary>
        /// <para>Creates a minefield with the specified arguments.</para>
        /// </summary>
        public Minesweeper(byte rows = 9, byte columns = 9, byte mines = 10, string mineEmote = "boom")
        {
            Rows = rows;
            Columns = columns;
            Mines = mines;
            MineEmote = mineEmote;
            Field = new string[rows, columns];
        }
        #endregion


        #region Properties
        /// <summary>
        /// <para>The rows in the <see cref="Field"/>.</para>
        /// <para>Default: 9.</para>
        /// </summary>
        public virtual byte Rows { get; set; }

        /// <summary>
        /// <para>The columns in the <see cref="Field"/>.</para>
        /// <para>Default: 9.</para>
        /// </summary>
        public virtual byte Columns { get; set; }

        /// <summary>
        /// <para>The amount of mines in the <see cref="Field"/>.</para>
        /// <para>Default: 10.</para>
        /// </summary>
        public virtual byte Mines { get; set; }

        /// <summary>
        /// <para>The emote used as mine in the <see cref="Field"/>.</para
        /// <para>Default: "boom".</para>
        /// </summary>
        public virtual string MineEmote { get; set; }

        /// <summary>
        /// <para>The minefield which contains the game.</para>
        /// </summary>
        public virtual string[,] Field { get; set; }

        /// <summary>
        /// <para>The emotes used as counters for nearby mines.</para>
        /// </summary>
        public virtual string[] FieldNumbers { get; init; }
            = new string[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight" };
        #endregion


        #region Methods

        #region Create
        /// <summary>
        /// <para>Generates, and returns the minefield.</para>
        /// </summary>
        /// <returns></returns>
        public virtual string Create()
        {
            // Fills the field with :zero: emotes
            GenerateEmptyField();

            // Place mines
            PlaceRandomMines();

            // Check mines
            CheckForMines();

            // Spoilerize the array
            string[,] spoiledArray = Spoilerize(Field);

            // Return the field as a string
            return string.Join("\r",
                   spoiledArray.OfType<string>()
                   .Select((str, idx) => new { index = idx, value = str })
                   .GroupBy(a => a.index / (spoiledArray.GetUpperBound(0) + 1))
                   .Select(gr => gr.Select(n => n.value).ToArray())
                   .Select(a => string.Join("", a.SelectMany(x => x)))
                   .ToArray());
        }
        #endregion

        #region GenerateEmptyField
        /// <summary>
        /// <para>Fills the <see cref="Field"/> with zero emotes.</para>
        /// </summary>
        public virtual void GenerateEmptyField()
        {
            // The X axis of the 2D array.
            for(int x = 0; x < Columns; x++)
            {
                // The Y axis of the 2D array.
                for(int y = 0; y < Rows; y++)
                {
                    // Fill the position with :zero: emotes.
                    Field[x, y] = FieldNumbers[0];
                }
            }
        }
        #endregion

        #region PlaceRandomMines
        /// <summary>
        /// <para>Places mines randomly in the minefield.</para>
        /// </summary>
        public virtual void PlaceRandomMines()
        {
            // Iterate through the amount of mines.
            for(int m = 0; m < Mines; m++)
            {
                // Create random position.
                int x = new Random().Next(0, Rows);
                int y = new Random().Next(0, Columns);

                // Check if position already is a mine.
                if(Field[x, y] == MineEmote)
                {
                    m--;
                }
                else
                {
                    Field[x, y] = MineEmote;
                }
            }
        }
        #endregion

        #region CheckForMines
        /// <summary>
        /// <para>Check all positions for nearby mines, and set the position to the amount of mines.</para>
        /// </summary>
        public virtual void CheckForMines()
        {
            // The X axis of the 2D array.
            for(int x = 0; x < Columns; x++)
            {
                // The Y axis of the 2D array.
                for(int y = 0; y < Rows; y++)
                {
                    // Count nearby mines
                    int mines = CountMines(x, y);

                    // Only set if the position is not a mine
                    if(mines != -1)
                    {
                        // Set the position value to the number of mines
                        Field[x, y] = FieldNumbers[mines];
                    }
                }
            }
        }
        #endregion

        #region CountMines
        /// <summary>
        /// <para>Checks surrounding fields for mines, and counts them.</para>
        /// <para>Returns -1 if the position is a mine.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CountMines(int x, int y)
        {
            int mines = 0;

            // Check if the position is a mine
            if(Field[x, y] == MineEmote)
            {
                return -1;
            }

            // Position checks
            bool hasTop = y > 0;
            bool hasLeft = x > 0;
            bool hasRight = x < Columns - 1;
            bool hasBottom = y < Rows - 1;

            // Top left
            if(hasTop && hasLeft && Field[x - 1, y - 1] == MineEmote)
            {
                mines++;
            }

            // Top
            if(hasTop && Field[x, y - 1] == MineEmote)
            {
                mines++;
            }

            // Top Right
            if(hasTop && hasRight && Field[x + 1, y - 1] == MineEmote)
            {
                mines++;
            }

            // Left
            if(hasLeft && Field[x - 1, y] == MineEmote)
            {
                mines++;
            }

            // Right
            if(hasRight && Field[x + 1, y] == MineEmote)
            {
                mines++;
            }

            // Bottom left
            if(hasBottom && hasLeft && Field[x - 1, y + 1] == MineEmote)
            {
                mines++;
            }

            // Bottom
            if(hasBottom && Field[x, y + 1] == MineEmote)
            {
                mines++;
            }

            // Bottom Right
            if(hasBottom && hasRight && Field[x + 1, y + 1] == MineEmote)
            {
                mines++;
            }

            return mines;
        }
        #endregion

        #region Spoilerize
        /// <summary>
        /// <para>Places spoilers around all positions in a 2D array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public virtual string[,] Spoilerize(string[,] array)
        {
            // The X axis of the 2D array.
            for(int x = 0; x < Columns; x++)
            {
                // The Y axis of the 2D array.
                for(int y = 0; y < Rows; y++)
                {
                    // Spoilerize the position
                    array[x, y] = $"||:{array[x, y]}:||";
                }
            }

            // Return the array
            return array;
        }
        #endregion

        #endregion
    }
}

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit { }
}