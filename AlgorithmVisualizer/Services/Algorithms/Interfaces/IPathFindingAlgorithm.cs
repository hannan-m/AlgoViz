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

/// <summary>s
/// Interface for tree algorithms
/// </summary>
public interface ITreeAlgorithm
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
    /// Executes the algorithm on the tree
    /// </summary>
    System.Threading.Tasks.Task<TreeState> ExecuteAsync(TreeState initialState, TreeOperationType operation,
        int? value = null);

    /// <summary>
    /// Executes the algorithm and returns each step for visualization
    /// </summary>
    System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Models.AlgorithmStep<TreeState>>> GetStepsAsync(
        TreeState initialState, TreeOperationType operation, int? value = null);
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