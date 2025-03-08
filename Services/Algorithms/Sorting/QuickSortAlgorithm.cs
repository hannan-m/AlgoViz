using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlgorithmVisualizer.Models;
using AlgorithmVisualizer.Models.SortingModels;
using AlgorithmVisualizer.Services.Algorithms.Interfaces;

namespace AlgorithmVisualizer.Services.Algorithms.Sorting
{
    /// <summary>
    /// Implementation of Quick Sort algorithm
    /// </summary>
    public class QuickSortAlgorithm : ISortingAlgorithm
    {
        public string Name => "Quick Sort";
        
        public string Description => "Quick Sort is a divide-and-conquer algorithm that works by selecting a 'pivot' element from the array " +
                                    "and partitioning the other elements into two sub-arrays according to whether they are less than or greater than the pivot. " +
                                    "The sub-arrays are then sorted recursively.";
        
        public string TimeComplexity => "O(n log n) average, O(n²) worst case";
        
        public string SpaceComplexity => "O(log n)";
        
        private List<AlgorithmStep<SortingState>> _steps;
        private int _stepNumber;
        private int[] _array;
        private bool[] _isSorted;
        
        /// <summary>
        /// Executes the Quick Sort algorithm and returns the sorted array
        /// </summary>
        public async Task<int[]> ExecuteAsync(int[] array)
        {
            // Create a copy of the array to avoid modifying the original
            int[] sortedArray = (int[])array.Clone();
            
            // Run the quicksort algorithm
            await QuickSortAsync(sortedArray, 0, sortedArray.Length - 1);
            
            return sortedArray;
        }
        
        /// <summary>
        /// Recursive implementation of Quick Sort
        /// </summary>
        private async Task QuickSortAsync(int[] array, int low, int high)
        {
            if (low < high)
            {
                // Partition the array and get the pivot index
                int pivotIndex = await PartitionAsync(array, low, high);
                
                // Recursively sort elements before and after the pivot
                await QuickSortAsync(array, low, pivotIndex - 1);
                await QuickSortAsync(array, pivotIndex + 1, high);
            }
        }
        
        /// <summary>
        /// Partitions the array and returns the pivot index
        /// </summary>
        private async Task<int> PartitionAsync(int[] array, int low, int high)
        {
            // Choose the rightmost element as the pivot
            int pivot = array[high];
            int i = low - 1;
            
            for (int j = low; j < high; j++)
            {
                if (array[j] <= pivot)
                {
                    i++;
                    
                    // Swap elements
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
                
                // Add small delay periodically to prevent UI locking
                if (j % 10 == 0)
                {
                    await Task.Delay(1);
                }
            }
            
            // Swap the pivot element with the element at i+1
            int temp2 = array[i + 1];
            array[i + 1] = array[high];
            array[high] = temp2;
            
            return i + 1;
        }
        
        /// <summary>
        /// Executes the Quick Sort algorithm and returns each step for visualization
        /// </summary>
        public async Task<IEnumerable<AlgorithmStep<SortingState>>> GetStepsAsync(int[] array)
        {
            _steps = new List<AlgorithmStep<SortingState>>();
            _stepNumber = 0;
            
            // Create a copy of the array to avoid modifying the original
            _array = (int[])array.Clone();
            _isSorted = new bool[_array.Length];
            
            // Initial state
            _steps.Add(new AlgorithmStep<SortingState>
            {
                State = new SortingState
                {
                    Array = (int[])_array.Clone(),
                    IsSorted = (bool[])_isSorted.Clone()
                },
                Description = "Initial array before Quick Sort.",
                StepNumber = _stepNumber++
            });
            
            // Run the quicksort algorithm
            await QuickSortWithStepsAsync(0, _array.Length - 1);
            
            // Final state
            for (int i = 0; i < _isSorted.Length; i++)
            {
                _isSorted[i] = true;
            }
            
            _steps.Add(new AlgorithmStep<SortingState>
            {
                State = new SortingState
                {
                    Array = _array,
                    IsSorted = _isSorted
                },
                Description = "Array is now fully sorted.",
                StepNumber = _stepNumber++
            });
            
            return _steps;
        }
        
        /// <summary>
        /// Recursive implementation of Quick Sort with step tracking
        /// </summary>
        private async Task QuickSortWithStepsAsync(int low, int high)
        {
            if (low < high)
            {
                // Add a step to show the current sub-array being sorted
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        SubArrayStart = low,
                        SubArrayEnd = high
                    },
                    Description = $"Sorting sub-array from index {low} to {high}.",
                    StepNumber = _stepNumber++
                });
                
                // Partition the array and get the pivot index
                int pivotIndex = await PartitionWithStepsAsync(low, high);
                
                // Mark the pivot as sorted
                _isSorted[pivotIndex] = true;
                
                // Add a step to show the pivot in its final position
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        SubArrayStart = low,
                        SubArrayEnd = high,
                        PivotIndex = pivotIndex
                    },
                    Description = $"Pivot element {_array[pivotIndex]} is now in its correct sorted position at index {pivotIndex}.",
                    StepNumber = _stepNumber++
                });
                
                // Recursively sort elements before and after the pivot
                await QuickSortWithStepsAsync(low, pivotIndex - 1);
                await QuickSortWithStepsAsync(pivotIndex + 1, high);
            }
            else if (low == high)
            {
                // Single element is always sorted
                _isSorted[low] = true;
                
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        SubArrayStart = low,
                        SubArrayEnd = high
                    },
                    Description = $"Single element sub-array at index {low} is already sorted.",
                    StepNumber = _stepNumber++
                });
            }
        }
        
        /// <summary>
        /// Partitions the array and returns the pivot index with step tracking
        /// </summary>
        private async Task<int> PartitionWithStepsAsync(int low, int high)
        {
            // Choose the rightmost element as the pivot
            int pivot = _array[high];
            
            _steps.Add(new AlgorithmStep<SortingState>
            {
                State = new SortingState
                {
                    Array = (int[])_array.Clone(),
                    IsSorted = (bool[])_isSorted.Clone(),
                    SubArrayStart = low,
                    SubArrayEnd = high,
                    PivotIndex = high
                },
                Description = $"Selected {pivot} as the pivot element.",
                StepNumber = _stepNumber++
            });
            
            int i = low - 1;
            
            for (int j = low; j < high; j++)
            {
                // Add a step for comparison
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        SubArrayStart = low,
                        SubArrayEnd = high,
                        PivotIndex = high,
                        CompareIndex1 = j,
                        CompareIndex2 = high
                    },
                    Description = $"Comparing element {_array[j]} at index {j} with pivot {pivot}.",
                    StepNumber = _stepNumber++
                });
                
                if (_array[j] <= pivot)
                {
                    i++;
                    
                    // Only swap if i and j are different
                    if (i != j)
                    {
                        // Swap elements
                        int temp = _array[i];
                        _array[i] = _array[j];
                        _array[j] = temp;
                        
                        // Add a step for the swap
                        _steps.Add(new AlgorithmStep<SortingState>
                        {
                            State = new SortingState
                            {
                                Array = (int[])_array.Clone(),
                                IsSorted = (bool[])_isSorted.Clone(),
                                SubArrayStart = low,
                                SubArrayEnd = high,
                                PivotIndex = high,
                                CompareIndex1 = i,
                                CompareIndex2 = j
                            },
                            Description = $"Swapped {_array[i]} and {_array[j]} to maintain elements <= pivot on the left side.",
                            StepNumber = _stepNumber++
                        });
                    }
                    else
                    {
                        // Add a step for when no swap is needed
                        _steps.Add(new AlgorithmStep<SortingState>
                        {
                            State = new SortingState
                            {
                                Array = (int[])_array.Clone(),
                                IsSorted = (bool[])_isSorted.Clone(),
                                SubArrayStart = low,
                                SubArrayEnd = high,
                                PivotIndex = high,
                                CompareIndex1 = i
                            },
                            Description = $"Element {_array[i]} at index {i} is already in the correct position (less than or equal to pivot).",
                            StepNumber = _stepNumber++
                        });
                    }
                }
                
                // Add small delay to prevent UI locking
                if (j % 5 == 0)
                {
                    await Task.Delay(1);
                }
            }
            
            // Swap the pivot element with the element at i+1
            int pivotFinalIndex = i + 1;
            
            // Only swap if pivot is not already at its final position
            if (pivotFinalIndex != high)
            {
                int temp = _array[pivotFinalIndex];
                _array[pivotFinalIndex] = _array[high];
                _array[high] = temp;
                
                // Add a step for the final pivot swap
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        SubArrayStart = low,
                        SubArrayEnd = high,
                        PivotIndex = pivotFinalIndex,
                        CompareIndex1 = pivotFinalIndex,
                        CompareIndex2 = high
                    },
                    Description = $"Placed pivot {_array[pivotFinalIndex]} at its correct position (index {pivotFinalIndex}).",
                    StepNumber = _stepNumber++
                });
            }
            
            return pivotFinalIndex;
        }
    }
}