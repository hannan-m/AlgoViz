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
    /// Implementation of Depth-First Search (DFS) for pathfinding
    /// </summary>
    public class DepthFirstSearchAlgorithm : IPathFindingAlgorithm
    {
        public string Name => "Depth-First Search";
        
        public string Description => "Depth-First Search (DFS) is an algorithm that explores as far as possible along each branch before backtracking. " +
                                    "Unlike BFS, DFS does not guarantee the shortest path, but it may find a path more quickly in some cases.";
        
        public string TimeComplexity => "O(V + E)";
        
        public string SpaceComplexity => "O(V)";
        
        /// <summary>
        /// Executes DFS algorithm on the grid and returns the final state
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
            
            // Stack for DFS
            var stack = new Stack<GridCell>();
            stack.Push(startCell);
            startCell.IsVisited = true;
            
            while (stack.Count > 0)
            {
                var currentCell = stack.Pop();
                
                // If we reached the end, break
                if (currentCell.Row == endCell.Row && currentCell.Column == endCell.Column)
                {
                    break;
                }
                
                // Get all neighbors
                var neighbors = GridUtilities.GetNeighbors(state, currentCell);
                
                foreach (var neighbor in neighbors)
                {
                    // Skip if already visited
                    if (neighbor.IsVisited)
                    {
                        continue;
                    }
                    
                    // Mark as visited and set parent
                    neighbor.IsVisited = true;
                    neighbor.Parent = currentCell;
                    
                    // Add to stack
                    stack.Push(neighbor);
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
        /// Executes DFS algorithm and returns each step for visualization
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
            
            // Add initial step
            steps.Add(new AlgorithmStep<GridState>
            {
                State = state.Clone(),
                Description = "Initial state. Starting DFS from the start cell.",
                StepNumber = stepNumber++
            });
            
            // Stack for DFS
            var stack = new Stack<GridCell>();
            stack.Push(startCell);
            startCell.IsVisited = true;
            
            steps.Add(new AlgorithmStep<GridState>
            {
                State = state.Clone(),
                Description = "Added start cell to the stack and marked it as visited.",
                StepNumber = stepNumber++
            });
            
            // Pre-size for optimal performance
            steps.Capacity = Math.Min(state.Rows * state.Columns, 1000);
            
            while (stack.Count > 0)
            {
                var currentCell = stack.Pop();
                
                steps.Add(new AlgorithmStep<GridState>
                {
                    State = state.Clone(),
                    Description = $"Popped cell at ({currentCell.Row}, {currentCell.Column}) from the stack.",
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
                bool foundUnvisitedNeighbor = false;
                
                foreach (var neighbor in neighbors)
                {
                    // Skip if already visited
                    if (neighbor.IsVisited)
                    {
                        continue;
                    }
                    
                    // Mark as visited and set parent
                    neighbor.IsVisited = true;
                    neighbor.Parent = currentCell;
                    
                    // Add to stack
                    stack.Push(neighbor);
                    foundUnvisitedNeighbor = true;
                    
                    steps.Add(new AlgorithmStep<GridState>
                    {
                        State = state.Clone(),
                        Description = $"Discovered cell at ({neighbor.Row}, {neighbor.Column}). Marked as visited and added to stack.",
                        StepNumber = stepNumber++
                    });
                }
                
                if (!foundUnvisitedNeighbor)
                {
                    steps.Add(new AlgorithmStep<GridState>
                    {
                        State = state.Clone(),
                        Description = $"No unvisited neighbors found for cell at ({currentCell.Row}, {currentCell.Column}). Backtracking.",
                        StepNumber = stepNumber++
                    });
                }
            }
            
            // Reconstruct the path
            if (endCell.Parent != null || endCell == startCell)
            {
                var path = GridUtilities.ReconstructPath(endCell);
                GridUtilities.MarkPath(state, path);
                
                steps.Add(new AlgorithmStep<GridState>
                {
                    State = state.Clone(),
                    Description = $"Found path from start to end with length {path.Count - 1}. Note that DFS does not guarantee the shortest path.",
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
    }
}
