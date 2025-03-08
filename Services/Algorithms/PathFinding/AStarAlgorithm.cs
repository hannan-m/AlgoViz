// Services/Algorithms/PathFinding/AStarAlgorithm.cs
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
    /// Implementation of A* pathfinding algorithm
    /// </summary>
    public class AStarAlgorithm : IPathFindingAlgorithm
    {
        public string Name => "A* Algorithm";
        
        public string Description => "A* is an informed search algorithm that uses a heuristic to guide its search. " +
                                    "It combines Dijkstra's algorithm (which gives the shortest path) with a heuristic " +
                                    "(which provides an estimated distance to the goal) to efficiently find paths.";
        
        public string TimeComplexity => "O(E log V)";
        
        public string SpaceComplexity => "O(V)";
        
        /// <summary>
        /// Executes A* algorithm on the grid and returns the final state
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
            
            // Set initial values for start cell
            startCell.Distance = 0;
            startCell.Heuristic = CalculateHeuristic(startCell, endCell);
            
            // Open set for A* algorithm
            var openSet = new List<GridCell> { startCell };
            var closedSet = new HashSet<GridCell>();
            
            while (openSet.Count > 0)
            {
                // Sort the open set to find the cell with the lowest f-score (distance + heuristic)
                openSet.Sort((a, b) => a.TotalCost.CompareTo(b.TotalCost));
                
                // Get the cell with the lowest f-score
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
                    
                    // Calculate tentative g-score (distance from start)
                    int tentativeDistance = currentCell.Distance + neighbor.Weight;
                    
                    // If neighbor is not in open set, add it
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    // If neighbor is already in open set with a better g-score, skip
                    else if (tentativeDistance >= neighbor.Distance)
                    {
                        continue;
                    }
                    
                    // This path is better, update it
                    neighbor.Parent = currentCell;
                    neighbor.Distance = tentativeDistance;
                    neighbor.Heuristic = CalculateHeuristic(neighbor, endCell);
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
        /// Executes A* algorithm and returns each step for visualization
        /// </summary>
        public async Task<IEnumerable<AlgorithmStep<GridState>>> GetStepsAsync(GridState initialState, GridCell start, GridCell end)
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
            
            // Set initial values for start cell
            startCell.Distance = 0;
            startCell.Heuristic = CalculateHeuristic(startCell, endCell);
            
            // Add initial step
            steps.Add(new AlgorithmStep<GridState>
            {
                State = state.Clone(),
                Description = "Initial state. Start position has been set. Using Manhattan distance heuristic.",
                StepNumber = stepNumber++
            });
            
            // Open set for A* algorithm
            var openSet = new List<GridCell> { startCell };
            var closedSet = new HashSet<GridCell>();
            
            // Pre-size for optimal performance
            steps.Capacity = Math.Min(state.Rows * state.Columns, 1000);
            
            while (openSet.Count > 0)
            {
                // Sort the open set to find the cell with the lowest f-score (distance + heuristic)
                openSet.Sort((a, b) => a.TotalCost.CompareTo(b.TotalCost));
                
                // Get the cell with the lowest f-score
                var currentCell = openSet[0];
                openSet.RemoveAt(0);
                
                // Add to closed set and mark as visited
                closedSet.Add(currentCell);
                currentCell.IsVisited = true;
                
                steps.Add(new AlgorithmStep<GridState>
                {
                    State = state.Clone(),
                    Description = $"Visiting cell at ({currentCell.Row}, {currentCell.Column}) with f-score {currentCell.TotalCost} (g={currentCell.Distance}, h={currentCell.Heuristic}).",
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
                    
                    // Calculate tentative g-score (distance from start)
                    int tentativeDistance = currentCell.Distance + neighbor.Weight;
                    
                    // Check if we need to add to open set or if we found a better path
                    bool needToAddToOpenSet = !openSet.Contains(neighbor);
                    bool foundBetterPath = tentativeDistance < neighbor.Distance;
                    
                    // If neighbor is not in open set or we found a better path
                    if (needToAddToOpenSet || foundBetterPath)
                    {
                        // Update neighbor
                        neighbor.Parent = currentCell;
                        neighbor.Distance = tentativeDistance;
                        neighbor.Heuristic = CalculateHeuristic(neighbor, endCell);
                        
                        if (needToAddToOpenSet)
                        {
                            openSet.Add(neighbor);
                            
                            steps.Add(new AlgorithmStep<GridState>
                            {
                                State = state.Clone(),
                                Description = $"Added cell ({neighbor.Row}, {neighbor.Column}) to the open set with f-score {neighbor.TotalCost} (g={neighbor.Distance}, h={neighbor.Heuristic}).",
                                StepNumber = stepNumber++
                            });
                        }
                        else if (foundBetterPath)
                        {
                            steps.Add(new AlgorithmStep<GridState>
                            {
                                State = state.Clone(),
                                Description = $"Found a better path to ({neighbor.Row}, {neighbor.Column}) with f-score {neighbor.TotalCost} (g={neighbor.Distance}, h={neighbor.Heuristic}).",
                                StepNumber = stepNumber++
                            });
                        }
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
                    Description = $"Found path from start to end with length {path.Count - 1}.",
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