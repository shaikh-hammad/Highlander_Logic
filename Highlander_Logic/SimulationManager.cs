using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlander_Logic
{
    public class SimulationManager
    {

        private List<Highlander> highlanders;
        private int gridSize;
        private Highlander[,] grid;
        private int iterations = 0;
        private Random random = new Random();

        public SimulationManager(int gridSize, int initialGoodHighlanders, int initialBadHighlanders)
        {
            this.gridSize = gridSize;
            InitializeGrid();
            InitializeHighlanders(initialGoodHighlanders, initialBadHighlanders);
        }

        private void InitializeGrid()
        {
            grid = new Highlander[gridSize, gridSize];
            Debug.WriteLine(grid);
        }

        private void InitializeHighlanders(int initialGoodHighlanders, int initialBadHighlanders)
        {
            highlanders = new List<Highlander>();

            // Add good Highlanders
            for (int i = 0; i < initialGoodHighlanders; i++)
            {
                GoodHighlander goodHighlander = new GoodHighlander($"Good_Highlander_{i + 1}", 100, 25, grid, gridSize);
                PlaceHighlanderOnGrid(goodHighlander);
                highlanders.Add(goodHighlander);
            }

            // Add bad Highlanders
            for (int i = 0; i < initialBadHighlanders; i++)
            {
                BadHighlander badHighlander = new BadHighlander($"Bad_Highlander_{i + 1}", 150, 30, grid, gridSize);
                PlaceHighlanderOnGrid(badHighlander);
                highlanders.Add(badHighlander);
            }
        }

        private void PlaceHighlanderOnGrid(Highlander highlander)
        {
            int x, y;

            do
            {
                // Randomly place Highlander on the grid
                x = random.Next(0, gridSize);
                y = random.Next(0, gridSize);
            } while (grid[x, y] != null);

            grid[x, y] = highlander;
            highlander.currentX = x;
            highlander.currentY = y;
        }

        public void StartSimulation()
        {
            iterations = 0;
            List<Highlander> highlandersCopy;
            while (!IsSimulationEndConditionMet() && iterations < 1000)
            {
                // Implement Highlander movement logic
                foreach (Highlander highlander in highlanders)
                {
                    highlander.Move();
                }
                highlandersCopy = new List<Highlander>(highlanders);
                foreach (Highlander highlander in highlandersCopy)
                {
                    CheckInteractions(grid[highlander.currentX, highlander.currentY], highlander.currentX, highlander.currentY);
                }

                iterations++;
            }
        }

        private void CheckInteractions(Highlander currentHighlander, int x, int y)
        {
            // Check interactions with neighboring Highlanders
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < gridSize && j >= 0 && j < gridSize && grid[i, j] != null && (i != x || j != y))
                    {
                        // Interaction logic based on Highlander types
                        if (currentHighlander is GoodHighlander && grid[i, j] is BadHighlander)
                        {
                            HandleGoodVsBadInteraction((GoodHighlander)currentHighlander, (BadHighlander)grid[i, j]);
                        }
                        else if (currentHighlander is BadHighlander && grid[i, j] is BadHighlander)
                        {
                            HandleBadVsBadInteraction((BadHighlander)currentHighlander, (BadHighlander)grid[i, j]);
                        }
                        // Add more conditions as needed
                    }
                }
            }
        }

        private void HandleGoodVsBadInteraction(GoodHighlander goodHighlander, BadHighlander badHighlander)
        {
            // Good Highlander tries to escape
            bool escaped = TryEscape(goodHighlander);

            if (!escaped)
            {
                // Good Highlander fights back
                Fight(goodHighlander, badHighlander);
            }
            else
            {
                // Log escape
                Logger.logEvent($"{goodHighlander.Name} escaped from {badHighlander.Name}!");
            }
        }


        private void HandleBadVsBadInteraction(BadHighlander badHighlander1, BadHighlander badHighlander2)
        {
            // Bad Highlanders fight
            Fight(badHighlander1, badHighlander2);
        }

        private bool TryEscape(GoodHighlander goodHighlander)
        {
            // Implement logic for Highlander escaping (return true if successful)
            return random.Next(0, 2) == 1; // 50% chance of escaping
        }
        private void pickAttacker(ref Highlander attacker, ref Highlander target)
        {
            int damage = random.Next(10, 21);
            if (random.Next(0, 2) == 1)
                target.PowerLevel -= damage;
            else
                attacker.PowerLevel -= damage;
        }

        private void Fight(Highlander attacker, Highlander target)
        {
            int originalTargetPower = target.PowerLevel;
            int originalAttackerPower = attacker.PowerLevel;
            while (attacker.PowerLevel > 0 && target.PowerLevel > 0)
            {
                // Implement logic for Highlander fight
                pickAttacker(ref attacker, ref target);

                if (target.PowerLevel <= 0)
                {
                    // Highlander is defeated
                    Logger.logEvent($"{attacker.Name} killed {target.Name} and absorbed {originalTargetPower} power!");
                    attacker.PowerLevel += originalTargetPower; // Absorb power
                    RemoveHighlander(target);
                }
                else if (attacker.PowerLevel <= 0)
                {
                    Logger.logEvent($"{target.Name} killed {attacker.Name} and absorbed {originalAttackerPower} power!");
                    target.PowerLevel += originalAttackerPower; // Absorb power
                    RemoveHighlander(attacker);
                    // Log the fight
                    //Logger.Logger.logEvent(Directory.GetCurrentDirectory(), $"{attacker.Name} attacked {target.Name} and dealt {damage} damage!");
                }
            }
        }

        private void RemoveHighlander(Highlander highlander)
        {
            // Remove Highlander from the grid and the list of Highlanders
            grid[highlander.currentX, highlander.currentY] = null;
            highlanders.Remove(highlander);
        }



        private bool IsSimulationEndConditionMet()
        {
            // Check end conditions: Only 1 Highlander left or maximum iterations reached
            return highlanders.Count == 1 || AllHighlandersAreDead();
        }

        private bool AllHighlandersAreDead()
        {
            // Check if all Highlanders are dead
            foreach (Highlander highlander in highlanders)
            {
                if (highlander != null)
                {
                    return false; // At least one Highlander is still alive
                }
            }
            return true; // All Highlanders are dead
        }
        public void StopSimulation()
        {
            iterations = int.MaxValue;
        }
        public string GetSimulationResult()
        {
            // Generate and return simulation results as a string
            if (highlanders.Count == 1)
            {
                // Option 1: Only 1 Highlander left
                Highlander winner = highlanders[0];
                return $"Winner: {winner.Name}\nPower absorbed: {winner.PowerLevel}\nIterations: {iterations}";
            }
            else
            {
                // Option 2: A certain number of simulation runs occur
                StringBuilder resultBuilder = new StringBuilder();
                resultBuilder.AppendLine("Simulation results:");
                resultBuilder.AppendLine($"Iterations: {iterations}");

                foreach (Highlander highlander in highlanders)
                {
                    resultBuilder.AppendLine($"{highlander.Name} - Power: {highlander.PowerLevel}");
                }

                return resultBuilder.ToString();
            }
        }
    }
}
