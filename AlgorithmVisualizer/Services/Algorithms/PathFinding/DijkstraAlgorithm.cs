using AlgorithmVisualizer.Models;
using AlgorithmVisualizer.Models.GridModels;
using AlgorithmVisualizer.Services.Algorithms.Interfaces;
using AlgorithmVisualizer.Utilities;

namespace AlgorithmVisualizer.Services.Algorithms.PathFinding;

/// <summary>
/// Implementation of Dijkstra's algorithm for pathfinding
/// </summary>
public class DijkstraAlgorithm : IPathFindingAlgorithm
{
    public string Name => "Dijkstra's Algorithm";

    public string Description =>
        "Dijkstra's algorithm is a weighted pathfinding algorithm that guarantees the shortest path. " +
        "It works by visiting the unvisited node with the smallest known distance, " +
        "then examining all its neighbors and updating their distances if a shorter path is found.";

    public string TimeComplexity => "O((V + E) log V)";

    public string SpaceComplexity => "O(V)";

    /// <summary>
    /// Executes Dijkstra's algorithm on the grid and returns the final state
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

        startCell.Distance = 0;

        // Priority queue for Dijkstra's algorithm
        // Use a list as a makeshift priority queue
        var unvisitedNodes = new List<GridCell>();

        // Add all cells to the unvisited set
        for (int i = 0; i < state.Rows; i++)
        {
            for (int j = 0; j < state.Columns; j++)
            {
                var cell = state.Grid[i, j];
                if (!cell.IsWall)
                {
                    unvisitedNodes.Add(cell);
                }
            }
        }

        while (unvisitedNodes.Count > 0)
        {
            // Sort the list to find the node with the smallest distance
            unvisitedNodes.Sort((a, b) => a.Distance.CompareTo(b.Distance));

            // Get the node with the smallest distance
            var currentCell = unvisitedNodes[0];
            unvisitedNodes.RemoveAt(0);

            // If we reached the end, break
            if (currentCell.Row == endCell.Row && currentCell.Column == endCell.Column)
            {
                break;
            }

            // If we're at infinity, there's no path
            if (currentCell.Distance == int.MaxValue)
            {
                break;
            }

            // Mark as visited
            currentCell.IsVisited = true;

            // Get all neighbors
            var neighbors = GridUtilities.GetNeighbors(state, currentCell);

            foreach (var neighbor in neighbors)
            {
                // Skip if already visited
                if (neighbor.IsVisited)
                {
                    continue;
                }

                // Calculate new distance
                int tentativeDistance = currentCell.Distance + neighbor.Weight;

                // If this path is better, update it
                if (tentativeDistance < neighbor.Distance)
                {
                    neighbor.Distance = tentativeDistance;
                    neighbor.Parent = currentCell;
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
    /// Executes Dijkstra's algorithm and returns each step for visualization
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

        startCell.Distance = 0;

        // Add initial step
        steps.Add(new AlgorithmStep<GridState>
        {
            State = state.Clone(),
            Description = "Initial state with start and end positions.",
            StepNumber = stepNumber++
        });

        // Priority queue for Dijkstra's algorithm
        var unvisitedNodes = new List<GridCell>();

        // Add all cells to the unvisited set
        for (int i = 0; i < state.Rows; i++)
        {
            for (int j = 0; j < state.Columns; j++)
            {
                var cell = state.Grid[i, j];
                if (!cell.IsWall)
                {
                    unvisitedNodes.Add(cell);
                }
            }
        }

        // Pre-size for optimal performance
        steps.Capacity = Math.Min(state.Rows * state.Columns, 1000);

        while (unvisitedNodes.Count > 0)
        {
            // Sort the list to find the node with the smallest distance
            unvisitedNodes.Sort((a, b) => a.Distance.CompareTo(b.Distance));

            // Get the node with the smallest distance
            var currentCell = unvisitedNodes[0];
            unvisitedNodes.RemoveAt(0);

            // If we're at infinity, there's no path
            if (currentCell.Distance == int.MaxValue)
            {
                steps.Add(new AlgorithmStep<GridState>
                {
                    State = state.Clone(),
                    Description = "No path exists to the target.",
                    StepNumber = stepNumber++
                });
                break;
            }

            // Mark as visited
            currentCell.IsVisited = true;

            // Add step for visiting a cell
            steps.Add(new AlgorithmStep<GridState>
            {
                State = state.Clone(),
                Description =
                    $"Visiting cell at ({currentCell.Row}, {currentCell.Column}) with distance {currentCell.Distance}.",
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

                // Calculate new distance
                int tentativeDistance = currentCell.Distance + neighbor.Weight;

                // Check if we found a better path
                bool foundBetterPath = tentativeDistance < neighbor.Distance;

                // If this path is better, update it
                if (foundBetterPath)
                {
                    neighbor.Distance = tentativeDistance;
                    neighbor.Parent = currentCell;

                    steps.Add(new AlgorithmStep<GridState>
                    {
                        State = state.Clone(),
                        Description =
                            $"Found shorter path to ({neighbor.Row}, {neighbor.Column}) with distance {neighbor.Distance}.",
                        StepNumber = stepNumber++
                    });
                }
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