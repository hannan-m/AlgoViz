// Services/Algorithms/PathFinding/BreadthFirstSearchAlgorithm.cs
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
    /// Implementation of Breadth-First Search (BFS) for pathfinding
    /// </summary>
    public class BreadthFirstSearchAlgorithm : IPathFindingAlgorithm
    {
        public string Name => "Breadth-First Search";
        
        public string Description => "Breadth-First Search (BFS) is an unweighted pathfinding algorithm that guarantees the shortest path " +
                                    "in terms of the number of cells traveled. It works by exploring all neighbors at the present depth " +
                                    "before moving on to nodes at the next depth level.";
        
        public string TimeComplexity => "O(V + E)";
        
        public string SpaceComplexity => "O(V)";
        
        /// <summary>
        /// Executes BFS algorithm on the grid and returns the final state
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
            
            // Queue for BFS
            var queue = new Queue<GridCell>();
            queue.Enqueue(startCell);
            startCell.IsVisited = true;
            startCell.Distance = 0;
            
            while (queue.Count > 0)
            {
                var currentCell = queue.Dequeue();
                
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
                    neighbor.Distance = currentCell.Distance + 1;
                    
                    // Add to queue
                    queue.Enqueue(neighbor);
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
        /// Executes BFS algorithm and returns each step for visualization
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
                Description = "Initial state. Starting BFS from the start cell.",
                StepNumber = stepNumber++
            });
            
            // Queue for BFS
            var queue = new Queue<GridCell>();
            queue.Enqueue(startCell);
            startCell.IsVisited = true;
            startCell.Distance = 0;
            
            steps.Add(new AlgorithmStep<GridState>
            {
                State = state.Clone(),
                Description = "Added start cell to the queue and marked it as visited.",
                StepNumber = stepNumber++
            });
            
            // Pre-size for optimal performance
            steps.Capacity = Math.Min(state.Rows * state.Columns, 1000);
            
            while (queue.Count > 0)
            {
                var currentCell = queue.Dequeue();
                
                steps.Add(new AlgorithmStep<GridState>
                {
                    State = state.Clone(),
                    Description = $"Dequeued cell at ({currentCell.Row}, {currentCell.Column}) with distance {currentCell.Distance}.",
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
                    // Skip if already visited
                    if (neighbor.IsVisited)
                    {
                        continue;
                    }
                    
                    // Mark as visited and set parent
                    neighbor.IsVisited = true;
                    neighbor.Parent = currentCell;
                    neighbor.Distance = currentCell.Distance + 1;
                    
                    // Add to queue
                    queue.Enqueue(neighbor);
                    
                    steps.Add(new AlgorithmStep<GridState>
                    {
                        State = state.Clone(),
                        Description = $"Discovered cell at ({neighbor.Row}, {neighbor.Column}). Marked as visited and added to queue.",
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
    }
}