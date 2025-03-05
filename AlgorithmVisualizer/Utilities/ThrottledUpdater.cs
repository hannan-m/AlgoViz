using System.Diagnostics;
using AlgorithmVisualizer.Models;
using AlgorithmVisualizer.Models.GridModels;

namespace AlgorithmVisualizer.Utilities;

/// <summary>
/// Throttles UI updates to optimize performance during algorithm visualization
/// </summary>
public class ThrottledUpdater
{
    private readonly TimeSpan _minUpdateInterval;
    private readonly Stopwatch _stopwatch = new Stopwatch();
    private TaskCompletionSource<bool> _pendingUpdate;
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Creates a new throttled updater with the specified minimum update interval
    /// </summary>
    /// <param name="minUpdateIntervalMs">Minimum time between updates in milliseconds</param>
    public ThrottledUpdater(int minUpdateIntervalMs = 16) // Default to ~60fps
    {
        _minUpdateInterval = TimeSpan.FromMilliseconds(minUpdateIntervalMs);
        _stopwatch.Start();
    }

    /// <summary>
    /// Throttles an update based on the specified minimum update interval
    /// </summary>
    /// <returns>Task that completes when the update should be processed</returns>
    public async Task ThrottleAsync()
    {
        // If we haven't waited long enough since the last update, delay
        if (_stopwatch.Elapsed < _minUpdateInterval)
        {
            // Calculate how long to wait
            var delayMs = _minUpdateInterval - _stopwatch.Elapsed;

            // Cancel any previous delay
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Delay(delayMs, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                // Task was canceled because a new update came in
            }
        }

        // Reset the stopwatch for the next update
        _stopwatch.Restart();
    }

    /// <summary>
    /// Schedules an update for the specified action
    /// </summary>
    /// <param name="updateAction">Action to execute when update is due</param>
    public async Task ScheduleUpdateAsync(Func<Task> updateAction)
    {
        await ThrottleAsync();
        await updateAction();
    }

    /// <summary>
    /// Schedules an update with adaptive timing based on complexity
    /// </summary>
    /// <param name="updateAction">Action to execute when update is due</param>
    /// <param name="complexity">Estimated complexity of the update (higher values = less frequent updates)</param>
    public async Task AdaptiveUpdateAsync(Func<Task> updateAction, int complexity)
    {
        // Adjust update interval based on complexity
        // For complex operations, we update less frequently
        var adaptiveInterval = Math.Min(1000, _minUpdateInterval.TotalMilliseconds * (1 + (complexity / 100.0)));

        if (_stopwatch.ElapsedMilliseconds < adaptiveInterval)
        {
            var delayMs = TimeSpan.FromMilliseconds(adaptiveInterval - _stopwatch.ElapsedMilliseconds);

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Delay(delayMs, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                // Task was canceled
            }
        }

        _stopwatch.Restart();
        await updateAction();
    }
}

/// <summary>
/// Tracks performance metrics during algorithm execution
/// </summary>
public class PerformanceTracker : IDisposable
{
    private readonly Stopwatch _stopwatch = new Stopwatch();
    private readonly AlgorithmStatistics _statistics = new AlgorithmStatistics();

    /// <summary>
    /// Starts tracking performance
    /// </summary>
    public void Start()
    {
        _stopwatch.Start();
    }

    /// <summary>
    /// Records a step in the algorithm execution
    /// </summary>
    public void RecordStep()
    {
        _statistics.StepCount++;
    }

    /// <summary>
    /// Records an element being processed
    /// </summary>
    /// <param name="count">Number of elements processed</param>
    public void RecordElementsProcessed(int count = 1)
    {
        _statistics.ElementsProcessed += count;
    }

    /// <summary>
    /// Sets additional algorithm-specific metrics
    /// </summary>
    /// <param name="metrics">Description of additional metrics</param>
    public void SetAdditionalMetrics(string metrics)
    {
        _statistics.AdditionalMetrics = metrics;
    }

    /// <summary>
    /// Stops tracking and returns the final statistics
    /// </summary>
    /// <returns>Performance statistics</returns>
    public AlgorithmStatistics Stop()
    {
        _stopwatch.Stop();
        _statistics.ExecutionTimeMs = _stopwatch.Elapsed.TotalMilliseconds;
        return _statistics;
    }

    /// <summary>
    /// Disposes resources
    /// </summary>
    public void Dispose()
    {
        _stopwatch.Stop();
    }
}

/// <summary>
/// Helps optimize rendering by detecting changes between states
/// </summary>
/// <typeparam name="T">Type of state to compare</typeparam>
public class ChangeDetector<T>
{
    private T _lastState;
    private readonly Func<T, T, bool> _comparer;

    /// <summary>
    /// Creates a new change detector with the specified comparison function
    /// </summary>
    /// <param name="comparer">Function to compare states</param>
    public ChangeDetector(Func<T, T, bool> comparer)
    {
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    /// <summary>
    /// Detects if the state has changed since the last check
    /// </summary>
    /// <param name="newState">New state to compare</param>
    /// <returns>True if the state has changed, false otherwise</returns>
    public bool HasChanged(T newState)
    {
        if (_lastState == null)
        {
            _lastState = newState;
            return true;
        }

        bool hasChanged = !_comparer(_lastState, newState);
        _lastState = newState;
        return hasChanged;
    }

    /// <summary>
    /// Resets the change detector
    /// </summary>
    public void Reset()
    {
        _lastState = default;
    }
}

/// <summary>
/// Provides utility methods for grid operations
/// </summary>
public static class GridUtilities
{
    /// <summary>
    /// Creates a new empty grid with the specified dimensions
    /// </summary>
    public static GridState CreateEmptyGrid(int rows, int columns)
    {
        var grid = new GridCell[rows, columns];

        // Initialize all cells
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                grid[i, j] = new GridCell
                {
                    Row = i,
                    Column = j,
                    Weight = 1,
                    Distance = int.MaxValue
                };
            }
        }

        return new GridState
        {
            Grid = grid
        };
    }

    /// <summary>
    /// Gets the neighbors of a cell in the grid
    /// </summary>
    public static List<GridCell> GetNeighbors(GridState state, GridCell cell, bool allowDiagonal = false)
    {
        var neighbors = new List<GridCell>(allowDiagonal ? 8 : 4);
        int rows = state.Rows;
        int cols = state.Columns;

        // Check the 4 cardinal directions
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { -1, 0, 1, 0 };

        for (int i = 0; i < 4; i++)
        {
            int newRow = cell.Row + dy[i];
            int newCol = cell.Column + dx[i];

            // Check if the neighbor is within bounds
            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
            {
                // Add non-wall neighbors
                var neighbor = state.Grid[newRow, newCol];
                if (!neighbor.IsWall)
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        // If diagonal movement is allowed, check the 4 diagonal directions
        if (allowDiagonal)
        {
            int[] dxDiag = { -1, 1, 1, -1 };
            int[] dyDiag = { -1, -1, 1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int newRow = cell.Row + dyDiag[i];
                int newCol = cell.Column + dxDiag[i];

                // Check if the neighbor is within bounds
                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                {
                    // Add non-wall neighbors
                    var neighbor = state.Grid[newRow, newCol];
                    if (!neighbor.IsWall)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Calculates the Manhattan distance between two cells
    /// </summary>
    public static int ManhattanDistance(GridCell a, GridCell b)
    {
        return Math.Abs(a.Row - b.Row) + Math.Abs(a.Column - b.Column);
    }

    /// <summary>
    /// Calculates the Euclidean distance between two cells
    /// </summary>
    public static double EuclideanDistance(GridCell a, GridCell b)
    {
        int dx = a.Column - b.Column;
        int dy = a.Row - b.Row;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Reconstructs the path from start to end using cell parents
    /// </summary>
    public static List<GridCell> ReconstructPath(GridCell endCell)
    {
        var path = new List<GridCell>();
        var current = endCell;

        while (current != null)
        {
            path.Add(current);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// Marks cells as part of the final path
    /// </summary>
    public static void MarkPath(GridState state, List<GridCell> path)
    {
        foreach (var cell in path)
        {
            // Skip marking start and end cells
            if (!cell.IsStart && !cell.IsEnd)
            {
                state.Grid[cell.Row, cell.Column].IsPath = true;
            }
        }
    }

    /// <summary>
    /// Resets all cells' visited and path flags
    /// </summary>
    public static void ResetVisitedAndPath(GridState state)
    {
        for (int i = 0; i < state.Rows; i++)
        {
            for (int j = 0; j < state.Columns; j++)
            {
                state.Grid[i, j].IsVisited = false;
                state.Grid[i, j].IsPath = false;
                state.Grid[i, j].Distance = int.MaxValue;
                state.Grid[i, j].Parent = null;
            }
        }

        // Reset distance for start cell
        if (state.StartCell != null)
        {
            state.StartCell.Distance = 0;
        }
    }
}