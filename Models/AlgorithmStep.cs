namespace AlgorithmVisualizer.Models;

/// <summary>
/// Represents a single step in an algorithm execution for visualization
/// </summary>
/// <typeparam name="T">The type of state being visualized</typeparam>
public class AlgorithmStep<T>
{
    /// <summary>
    /// Current state of the algorithm
    /// </summary>
    public T State { get; set; }
        
    /// <summary>
    /// Description of what happened in this step
    /// </summary>
    public string Description { get; set; }
        
    /// <summary>
    /// Step number in the execution sequence
    /// </summary>
    public int StepNumber { get; set; }
        
    /// <summary>
    /// Timestamp when this step occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

/// <summary>
/// Stores performance statistics for algorithm execution
/// </summary>
public class AlgorithmStatistics
{
    /// <summary>
    /// Time taken for algorithm execution in milliseconds
    /// </summary>
    public double ExecutionTimeMs { get; set; }
        
    /// <summary>
    /// Number of steps executed
    /// </summary>
    public int StepCount { get; set; }
        
    /// <summary>
    /// Number of elements or nodes processed
    /// </summary>
    public int ElementsProcessed { get; set; }
        
    /// <summary>
    /// Additional algorithm-specific metrics (e.g., path length, comparisons)
    /// </summary>
    public string AdditionalMetrics { get; set; }
}