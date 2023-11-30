using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Highlander_Logic
{
    public class Highlander
    {
        public string Name { get; set; }
        public int PowerLevel { get; set; }
        public int Age { get; set; }
        public int currentX { get; set; }
        public int currentY { get; set; }

        private Highlander[,] grid;
        private int gridSize;

        public Highlander(string name, int powerLevel, int age, Highlander[,] grid, int gridSize)
        {
            Name = name;
            PowerLevel = powerLevel;
            Age = age;
            this.grid = grid;
            this.gridSize = gridSize;
        }

        public virtual void Move()
        {
            // Implement Highlander movement logic
            Random random = new Random();
            int direction = random.Next(8); // 0 to 7, each representing a direction

            // Calculate new coordinates based on the chosen direction
            int[] newX = { -1, -1, 0, 1, 1, 1, 0, -1 };
            int[] newY = { 0, 1, 1, 1, 0, -1, -1, -1 };

            //int currentX = GetCurrentXCoordinate();
            //int currentY = GetCurrentYCoordinate();

            int newXCoordinate = WrapAround(currentX + newX[direction], gridSize);
            int newYCoordinate = WrapAround(currentY + newY[direction], gridSize);

            // Update grid positions
            grid[currentX, currentY] = null;
            currentX = newXCoordinate;
            currentY = newYCoordinate;

            grid[currentX, currentY] = this;
            Logger.logEvent(Name + " is now at position (" + currentX + ", " + currentY + ")");
        }

        private int WrapAround(int value, int maxValue)
        {
            Debug.WriteLine((value % maxValue + maxValue) % maxValue);
            Debug.WriteLine(currentX + " " + currentY);
            // Handle wrap-around for the given value and maximum value
            return (value % maxValue + maxValue) % maxValue;
        }


        private bool IsValidMove(int x, int y)
        {
            // Check if the new coordinates are within the grid boundaries
            return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
        }
    }

    public class GoodHighlander : Highlander
    {
        public GoodHighlander(string name, int powerLevel, int age, Highlander[,] grid, int gridSize) : base(name, powerLevel, age, grid, gridSize) { }
    }

    public class BadHighlander : Highlander
    {
        public BadHighlander(string name, int powerLevel, int age, Highlander[,] grid, int gridSize) : base(name, powerLevel, age, grid, gridSize) { }
    }

}
