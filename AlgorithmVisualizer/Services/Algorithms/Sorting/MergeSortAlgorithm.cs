using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlgorithmVisualizer.Models;
using AlgorithmVisualizer.Models.SortingModels;
using AlgorithmVisualizer.Services.Algorithms.Interfaces;

namespace AlgorithmVisualizer.Services.Algorithms.Sorting
{
    /// <summary>
    /// Implementation of Merge Sort algorithm
    /// </summary>
    public class MergeSortAlgorithm : ISortingAlgorithm
    {
        public string Name => "Merge Sort";
        
        public string Description => "Merge Sort is a divide-and-conquer algorithm that divides the input array into two halves, " +
                                    "recursively sorts them, and then merges the sorted halves to produce the final sorted array.";
        
        public string TimeComplexity => "O(n log n)";
        
        public string SpaceComplexity => "O(n)";
        
        private List<AlgorithmStep<SortingState>> _steps;
        private int _stepNumber;
        private int[] _array;
        private bool[] _isSorted;
        
        /// <summary>
        /// Executes the Merge Sort algorithm and returns the sorted array
        /// </summary>
        public async Task<int[]> ExecuteAsync(int[] array)
        {
            // Create a copy of the array to avoid modifying the original
            int[] sortedArray = (int[])array.Clone();
            
            // Run the mergesort algorithm
            await MergeSortAsync(sortedArray, 0, sortedArray.Length - 1);
            
            return sortedArray;
        }
        
        /// <summary>
        /// Recursive implementation of Merge Sort
        /// </summary>
        private async Task MergeSortAsync(int[] array, int left, int right)
        {
            if (left < right)
            {
                // Find the middle point
                int middle = left + (right - left) / 2;
                
                // Sort first and second halves
                await MergeSortAsync(array, left, middle);
                await MergeSortAsync(array, middle + 1, right);
                
                // Merge the sorted halves
                await MergeAsync(array, left, middle, right);
            }
        }
        
        /// <summary>
        /// Merges two sorted subarrays
        /// </summary>
        private async Task MergeAsync(int[] array, int left, int middle, int right)
        {
            // Calculate sizes of the two subarrays to be merged
            int n1 = middle - left + 1;
            int n2 = right - middle;
            
            // Create temporary arrays
            int[] leftArray = new int[n1];
            int[] rightArray = new int[n2];
            
            // Copy data to temporary arrays
            for (int i = 0; i < n1; i++)
                leftArray[i] = array[left + i];
            for (int j = 0; j < n2; j++)
                rightArray[j] = array[middle + 1 + j];
            
            // Merge the temporary arrays back into the original array
            int i1 = 0, i2 = 0;
            int k = left;
            
            while (i1 < n1 && i2 < n2)
            {
                if (leftArray[i1] <= rightArray[i2])
                {
                    array[k] = leftArray[i1];
                    i1++;
                }
                else
                {
                    array[k] = rightArray[i2];
                    i2++;
                }
                k++;
                
                // Add small delay periodically to prevent UI locking
                if (k % 10 == 0)
                {
                    await Task.Delay(1);
                }
            }
            
            // Copy remaining elements of leftArray[] if any
            while (i1 < n1)
            {
                array[k] = leftArray[i1];
                i1++;
                k++;
            }
            
            // Copy remaining elements of rightArray[] if any
            while (i2 < n2)
            {
                array[k] = rightArray[i2];
                i2++;
                k++;
            }
        }
        
        /// <summary>
        /// Executes the Merge Sort algorithm and returns each step for visualization
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
                Description = "Initial array before Merge Sort.",
                StepNumber = _stepNumber++
            });
            
            // Run the mergesort algorithm
            await MergeSortWithStepsAsync(0, _array.Length - 1);
            
            // Final state - mark all elements as sorted
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
        /// Recursive implementation of Merge Sort with step tracking
        /// </summary>
        private async Task MergeSortWithStepsAsync(int left, int right)
        {
            if (left < right)
            {
                // Add a step to show the current sub-array being sorted
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        SubArrayStart = left,
                        SubArrayEnd = right
                    },
                    Description = $"Splitting array from index {left} to {right}.",
                    StepNumber = _stepNumber++
                });
                
                // Find the middle point
                int middle = left + (right - left) / 2;
                
                // Sort first and second halves
                await MergeSortWithStepsAsync(left, middle);
                await MergeSortWithStepsAsync(middle + 1, right);
                
                // Merge the sorted halves
                await MergeWithStepsAsync(left, middle, right);
            }
            else if (left == right)
            {
                // Single element sub-array is already sorted
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        SubArrayStart = left,
                        SubArrayEnd = right
                    },
                    Description = $"Single element sub-array at index {left} is already sorted.",
                    StepNumber = _stepNumber++
                });
            }
        }
        
        /// <summary>
        /// Merges two sorted subarrays with step tracking
        /// </summary>
        private async Task MergeWithStepsAsync(int left, int middle, int right)
        {
            _steps.Add(new AlgorithmStep<SortingState>
            {
                State = new SortingState
                {
                    Array = (int[])_array.Clone(),
                    IsSorted = (bool[])_isSorted.Clone(),
                    SubArrayStart = left,
                    SubArrayEnd = right
                },
                Description = $"Merging sub-arrays: [{left}...{middle}] and [{middle+1}...{right}].",
                StepNumber = _stepNumber++
            });
            
            // Calculate sizes of the two subarrays to be merged
            int n1 = middle - left + 1;
            int n2 = right - middle;
            
            // Create temporary arrays
            int[] leftArray = new int[n1];
            int[] rightArray = new int[n2];
            
            // Copy data to temporary arrays
            for (int i = 0; i < n1; i++)
                leftArray[i] = _array[left + i];
            for (int j = 0; j < n2; j++)
                rightArray[j] = _array[middle + 1 + j];
            
            // Add step to show the split arrays
            var tempArray = (int[])_array.Clone();
            _steps.Add(new AlgorithmStep<SortingState>
            {
                State = new SortingState
                {
                    Array = tempArray,
                    IsSorted = (bool[])_isSorted.Clone(),
                    SubArrayStart = left,
                    SubArrayEnd = right
                },
                Description = $"Split into two sub-arrays: Left[{string.Join(", ", leftArray)}] and Right[{string.Join(", ", rightArray)}].",
                StepNumber = _stepNumber++
            });
            
            // Merge the temporary arrays back into the original array
            int i1 = 0, i2 = 0;
            int k = left;
            
            while (i1 < n1 && i2 < n2)
            {
                // Add step for comparison
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        SubArrayStart = left,
                        SubArrayEnd = right,
                        CompareIndex1 = left + i1,
                        CompareIndex2 = middle + 1 + i2
                    },
                    Description = $"Comparing {leftArray[i1]} and {rightArray[i2]}.",
                    StepNumber = _stepNumber++
                });
                
                if (leftArray[i1] <= rightArray[i2])
                {
                    _array[k] = leftArray[i1];
                    i1++;
                    
                    // Add step for placing element from left array
                    _steps.Add(new AlgorithmStep<SortingState>
                    {
                        State = new SortingState
                        {
                            Array = (int[])_array.Clone(),
                            IsSorted = (bool[])_isSorted.Clone(),
                            SubArrayStart = left,
                            SubArrayEnd = right
                        },
                        Description = $"Placed {_array[k]} at index {k} from left sub-array.",
                        StepNumber = _stepNumber++
                    });
                }
                else
                {
                    _array[k] = rightArray[i2];
                    i2++;
                    
                    // Add step for placing element from right array
                    _steps.Add(new AlgorithmStep<SortingState>
                    {
                        State = new SortingState
                        {
                            Array = (int[])_array.Clone(),
                            IsSorted = (bool[])_isSorted.Clone(),
                            SubArrayStart = left,
                            SubArrayEnd = right
                        },
                        Description = $"Placed {_array[k]} at index {k} from right sub-array.",
                        StepNumber = _stepNumber++
                    });
                }
                k++;
                
                // Add small delay periodically to prevent UI locking
                if (k % 5 == 0)
                {
                    await Task.Delay(1);
                }
            }
            
            // Copy remaining elements of leftArray[] if any
            while (i1 < n1)
            {
                _array[k] = leftArray[i1];
                i1++;
                
                // Add step for placing remaining element from left array
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        SubArrayStart = left,
                        SubArrayEnd = right
                    },
                    Description = $"Placed remaining element {_array[k]} at index {k} from left sub-array.",
                    StepNumber = _stepNumber++
                });
                
                k++;
            }
            
            // Copy remaining elements of rightArray[] if any
            while (i2 < n2)
            {
                _array[k] = rightArray[i2];
                i2++;
                
                // Add step for placing remaining element from right array
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        SubArrayStart = left,
                        SubArrayEnd = right
                    },
                    Description = $"Placed remaining element {_array[k]} at index {k} from right sub-array.",
                    StepNumber = _stepNumber++
                });
                
                k++;
            }
            
            // Add step for completed merge
            _steps.Add(new AlgorithmStep<SortingState>
            {
                State = new SortingState
                {
                    Array = (int[])_array.Clone(),
                    IsSorted = (bool[])_isSorted.Clone(),
                    SubArrayStart = left,
                    SubArrayEnd = right
                },
                Description = $"Completed merging sub-arrays from index {left} to {right}.",
                StepNumber = _stepNumber++
            });
            
            // If this is a complete section, mark these elements as sorted
            if (right - left + 1 == _array.Length)
            {
                for (int i = left; i <= right; i++)
                {
                    _isSorted[i] = true;
                }
            }
        }
    }
}