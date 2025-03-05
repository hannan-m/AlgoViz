using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlgorithmVisualizer.Models;
using AlgorithmVisualizer.Models.GridModels;
using AlgorithmVisualizer.Services.Algorithms.Interfaces;
using AlgorithmVisualizer.Utilities;

namespace AlgorithmVisualizer.Services.Algorithms.PathFinding
{
    /// <summary>
    /// Implementation of Greedy Best-First Search for pathfinding
    /// </summary>
    public class GreedyBestFirstSearchAlgorithm : IPathFindingAlgorithm
    {
        public string Name => "Greedy Best-First Search";

        public string Description =>
            "Greedy Best-First Search is an informed search algorithm that uses a heuristic to always expand the node " +
            "that is closest to the goal. Unlike A*, it only considers the heuristic cost (estimated distance to goal) " +
            "and not the path cost so far, making it faster but not guaranteed to find the shortest path.";

        public string TimeComplexity => "O(E log V)";

        public string SpaceComplexity => "O(V)";

        /// <summary>
        /// Executes Greedy Best-First Search on the grid and returns the final state
        /// </summary>
        public async Task<GridState> ExecuteAsync(GridState initialState, GridCell start, GridCell end)
        {
            var state = initialState.Clone();

            // If start or end is not defined, return immediately
            if (start == null || end == null)
            {
                return state;
            }

            // Reset visited states and distances
            GridUtilities.ResetVisitedAndPath(state);

            // Use the actual start and end cells from the cloned state
            var startCell = state.Grid[start.Row, start.Column];
            var endCell = state.Grid[end.Row, end.Column];

            // Set heuristic for start cell
            startCell.Heuristic = CalculateHeuristic(startCell, endCell);

            // Priority queue for Greedy Best-First Search
            // Using a List as a makeshift priority queue
            var openSet = new List<GridCell> { startCell };
            var closedSet = new HashSet<GridCell>();

            while (openSet.Count > 0)
            {
                // Sort by heuristic (distance to goal)
                openSet.Sort((a, b) => a.Heuristic.CompareTo(b.Heuristic));

                // Get the node with the smallest heuristic
                var currentCell = openSet[0];
                openSet.RemoveAt(0);

                // Add to closed set
                closedSet.Add(currentCell);

                // Mark as visited
                currentCell.IsVisited = true;

                // If we reached the end, break
                if (currentCell.Row == endCell.Row && currentCell.Column == endCell.Column)
                {
                    break;
                }

                // Get all neighbors
                var neighbors = GridUtilities.GetNeighbors(state, currentCell);

                foreach (var neighbor in neighbors)
                {
                    // Skip if in closed set
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    // If not in open set, add it
                    if (!openSet.Contains(neighbor))
                    {
                        neighbor.Parent = currentCell;
                        neighbor.Heuristic = CalculateHeuristic(neighbor, endCell);
                        openSet.Add(neighbor);
                    }
                }

                // Add a small delay to prevent UI freezing in real-time visualization
                await Task.Delay(1);
            }

            // Reconstruct the path
            if (endCell.Parent != null || endCell == startCell)
            {
                var path = GridUtilities.ReconstructPath(endCell);
                GridUtilities.MarkPath(state, path);
            }

            return state;
        }

        /// <summary>
        /// Executes Greedy Best-First Search and returns each step for visualization
        /// </summary>
        public async Task<IEnumerable<AlgorithmStep<GridState>>> GetStepsAsync(GridState initialState, GridCell start,
            GridCell end)
        {
            var state = initialState.Clone();
            var steps = new List<AlgorithmStep<GridState>>();
            int stepNumber = 0;

            // If start or end is not defined, return empty list
            if (start == null || end == null)
            {
                return steps;
            }

            // Reset visited states and distances
            GridUtilities.ResetVisitedAndPath(state);

            // Use the actual start and end cells from the cloned state
            var startCell = state.Grid[start.Row, start.Column];
            var endCell = state.Grid[end.Row, end.Column];

            // Set heuristic for start cell
            startCell.Heuristic = CalculateHeuristic(startCell, endCell);

            // Add initial step
            steps.Add(new AlgorithmStep<GridState>
            {
                State = state.Clone(),
                Description = "Initial state. Starting Greedy Best-First Search using Manhattan distance heuristic.",
                StepNumber = stepNumber++
            });

            // Priority queue for Greedy Best-First Search
            var openSet = new List<GridCell> { startCell };
            var closedSet = new HashSet<GridCell>();

            steps.Add(new AlgorithmStep<GridState>
            {
                State = state.Clone(),
                Description = $"Added start cell to the open set with heuristic {startCell.Heuristic}.",
                StepNumber = stepNumber++
            });

            // Pre-size for optimal performance
            steps.Capacity = Math.Min(state.Rows * state.Columns, 1000);

            while (openSet.Count > 0)
            {
                // Sort by heuristic (distance to goal)
                openSet.Sort((a, b) => a.Heuristic.CompareTo(b.Heuristic));

                // Get the node with the smallest heuristic
                var currentCell = openSet[0];
                openSet.RemoveAt(0);

                // Add to closed set
                closedSet.Add(currentCell);

                // Mark as visited
                currentCell.IsVisited = true;

                steps.Add(new AlgorithmStep<GridState>
                {
                    State = state.Clone(),
                    Description =
                        $"Visiting cell at ({currentCell.Row}, {currentCell.Column}) with heuristic {currentCell.Heuristic}.",
                    StepNumber = stepNumber++
                });

                // If we reached the end, break
                if (currentCell.Row == endCell.Row && currentCell.Column == endCell.Column)
                {
                    steps.Add(new AlgorithmStep<GridState>
                    {
                        State = state.Clone(),
                        Description = "Reached the target cell!",
                        StepNumber = stepNumber++
                    });
                    break;
                }

                // Get all neighbors
                var neighbors = GridUtilities.GetNeighbors(state, currentCell);

                foreach (var neighbor in neighbors)
                {
                    // Skip if in closed set
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    bool isNewCell = !openSet.Contains(neighbor);

                    // If not in open set, add it
                    if (isNewCell)
                    {
                        neighbor.Parent = currentCell;
                        neighbor.Heuristic = CalculateHeuristic(neighbor, endCell);
                        openSet.Add(neighbor);

                        steps.Add(new AlgorithmStep<GridState>
                        {
                            State = state.Clone(),
                            Description =
                                $"Discovered cell at ({neighbor.Row}, {neighbor.Column}). Added to open set with heuristic {neighbor.Heuristic}.",
                            StepNumber = stepNumber++
                        });
                    }
                }

                // Update state to show current open set
                state.OpenSet = new List<GridCell>(openSet);
            }

            // Reconstruct the path
            if (endCell.Parent != null || endCell == startCell)
            {
                var path = GridUtilities.ReconstructPath(endCell);
                GridUtilities.MarkPath(state, path);

                steps.Add(new AlgorithmStep<GridState>
                {
                    State = state.Clone(),
                    Description =
                        $"Found path from start to end with length {path.Count - 1}. Note that this might not be the shortest path since Greedy Best-First Search is not guaranteed to find the optimal solution.",
                    StepNumber = stepNumber++
                });
            }
            else
            {
                steps.Add(new AlgorithmStep<GridState>
                {
                    State = state.Clone(),
                    Description = "No path found from start to end.",
                    StepNumber = stepNumber++
                });
            }

            return steps;
        }

        /// <summary>
        /// Calculates the Manhattan distance heuristic between two cells
        /// </summary>
        private int CalculateHeuristic(GridCell a, GridCell b)
        {
            return GridUtilities.ManhattanDistance(a, b);
        }
    }
}