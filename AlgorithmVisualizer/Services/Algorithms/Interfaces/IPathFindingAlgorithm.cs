using AlgorithmVisualizer.Models;
using AlgorithmVisualizer.Models.GridModels;
using AlgorithmVisualizer.Models.SortingModels;
using AlgorithmVisualizer.Models.TreeModels;

namespace AlgorithmVisualizer.Services.Algorithms.Interfaces;

/// <summary>
/// Interface for pathfinding algorithms that operate on a grid
/// </summary>
public interface IPathFindingAlgorithm
{
    /// <summary>
    /// The name of the algorithm
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A description of how the algorithm works
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The time complexity of the algorithm in Big O notation
    /// </summary>
    string TimeComplexity { get; }

    /// <summary>
    /// The space complexity of the algorithm in Big O notation
    /// </summary>
    string SpaceComplexity { get; }

    /// <summary>
    /// Executes the algorithm and returns the final result
    /// </summary>
    Task<GridState> ExecuteAsync(GridState initialState, GridCell start, GridCell end);

    /// <summary>
    /// Executes the algorithm and returns each step for visualization
    /// </summary>
    Task<IEnumerable<AlgorithmStep<GridState>>> GetStepsAsync(GridState initialState, GridCell start, GridCell end);
}

/// <summary>
/// Interface for sorting algorithms
/// </summary>
public interface ISortingAlgorithm
{
    /// <summary>
    /// The name of the algorithm
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A description of how the algorithm works
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The time complexity of the algorithm in Big O notation
    /// </summary>
    string TimeComplexity { get; }

    /// <summary>
    /// The space complexity of the algorithm in Big O notation
    /// </summary>
    string SpaceComplexity { get; }

    /// <summary>
    /// Executes the algorithm and returns the sorted array
    /// </summary>
    Task<int[]> ExecuteAsync(int[] array);

    /// <summary>
    /// Executes the algorithm and returns each step for visualization
    /// </summary>
    Task<IEnumerable<AlgorithmStep<SortingState>>> GetStepsAsync(int[] array);
}

/// <summary>
/// Interface for tree algorithm implementations
/// </summary>
public interface ITreeAlgorithm
{
    /// <summary>
    /// Gets the name of the algorithm
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the algorithm
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the time complexity of the algorithm
    /// </summary>
    string TimeComplexity { get; }

    /// <summary>
    /// Gets the space complexity of the algorithm
    /// </summary>
    string SpaceComplexity { get; }

    /// <summary>
    /// Executes the specified tree operation
    /// </summary>
    /// <param name="initialState">The initial state of the tree</param>
    /// <param name="operation">The operation to perform</param>
    /// <param name="value">The value for the operation (if applicable)</param>
    /// <param name="parameters">Additional parameters for the operation (e.g., traversal type)</param>
    /// <returns>The resulting tree state after the operation</returns>
    Task<TreeState> ExecuteAsync(TreeState initialState, TreeOperationType operation, int? value = null,
        Dictionary<string, object> parameters = null);

    /// <summary>
    /// Gets detailed steps for tree operation visualization
    /// </summary>
    /// <param name="initialState">The initial state of the tree</param>
    /// <param name="operation">The operation to perform</param>
    /// <param name="value">The value for the operation (if applicable)</param>
    /// <param name="parameters">Additional parameters for the operation (e.g., traversal type)</param>
    /// <returns>A collection of algorithm steps</returns>
    Task<IEnumerable<AlgorithmStep<TreeState>>> GetStepsAsync(TreeState initialState, TreeOperationType operation,
        int? value = null, Dictionary<string, object> parameters = null);
}

/// <summary>
/// Defines the type of operation to perform on a tree
/// </summary>
public enum TreeOperationType
{
    Insert,
    Delete,
    Search,
    Traverse
}