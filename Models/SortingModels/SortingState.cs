namespace AlgorithmVisualizer.Models.SortingModels;

/// <summary>
/// Represents the state of a sorting algorithm at a point in time
/// </summary>
public class SortingState
{
    /// <summary>
    /// Current state of the array being sorted
    /// </summary>
    public int[] Array { get; set; }

    /// <summary>
    /// Index of the first element being compared/swapped
    /// </summary>
    public int? CompareIndex1 { get; set; }

    /// <summary>
    /// Index of the second element being compared/swapped
    /// </summary>
    public int? CompareIndex2 { get; set; }

    /// <summary>
    /// Index of the pivot element (for partition-based algorithms)
    /// </summary>
    public int? PivotIndex { get; set; }

    /// <summary>
    /// Left boundary of the current sub-array being processed
    /// </summary>
    public int? SubArrayStart { get; set; }

    /// <summary>
    /// Right boundary of the current sub-array being processed
    /// </summary>
    public int? SubArrayEnd { get; set; }

    /// <summary>
    /// Indices of sorted elements
    /// </summary>
    public bool[] IsSorted { get; set; }

    /// <summary>
    /// Creates a deep copy of this sorting state
    /// </summary>
    public SortingState Clone()
    {
        return new SortingState
        {
            Array = (int[])Array.Clone(),
            CompareIndex1 = CompareIndex1,
            CompareIndex2 = CompareIndex2,
            PivotIndex = PivotIndex,
            SubArrayStart = SubArrayStart,
            SubArrayEnd = SubArrayEnd,
            IsSorted = IsSorted != null ? (bool[])IsSorted.Clone() : null
        };
    }
}