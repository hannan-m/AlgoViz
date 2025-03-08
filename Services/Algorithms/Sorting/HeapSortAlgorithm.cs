using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlgorithmVisualizer.Models;
using AlgorithmVisualizer.Models.SortingModels;
using AlgorithmVisualizer.Services.Algorithms.Interfaces;

namespace AlgorithmVisualizer.Services.Algorithms.Sorting
{
    /// <summary>
    /// Implementation of Heap Sort algorithm
    /// </summary>
    public class HeapSortAlgorithm : ISortingAlgorithm
    {
        public string Name => "Heap Sort";
        
        public string Description => "Heap Sort is a comparison-based sorting algorithm that uses a binary heap data structure. " +
                                    "It divides the input into a sorted and an unsorted region, and iteratively shrinks the unsorted region " +
                                    "by extracting the largest element and moving it to the sorted region.";
        
        public string TimeComplexity => "O(n log n)";
        
        public string SpaceComplexity => "O(1)";
        
        private List<AlgorithmStep<SortingState>> _steps;
        private int _stepNumber;
        private int[] _array;
        private bool[] _isSorted;
        
        /// <summary>
        /// Executes the Heap Sort algorithm and returns the sorted array
        /// </summary>
        public async Task<int[]> ExecuteAsync(int[] array)
        {
            // Create a copy of the array to avoid modifying the original
            int[] sortedArray = (int[])array.Clone();
            int n = sortedArray.Length;
            
            // Build heap (rearrange array)
            for (int i = n / 2 - 1; i >= 0; i--)
                await Heapify(sortedArray, n, i);
            
            // One by one extract an element from heap
            for (int i = n - 1; i > 0; i--)
            {
                // Move current root to end
                int temp = sortedArray[0];
                sortedArray[0] = sortedArray[i];
                sortedArray[i] = temp;
                
                // Call max heapify on the reduced heap
                await Heapify(sortedArray, i, 0);
                
                // Add a small delay periodically to prevent UI locking
                if (i % 10 == 0)
                {
                    await Task.Delay(1);
                }
            }
            
            return sortedArray;
        }
        
        /// <summary>
        /// To heapify a subtree rooted with node i which is an index in array[]
        /// </summary>
        private async Task Heapify(int[] array, int n, int i)
        {
            int largest = i; // Initialize largest as root
            int left = 2 * i + 1; // left = 2*i + 1
            int right = 2 * i + 2; // right = 2*i + 2
            
            // If left child is larger than root
            if (left < n && array[left] > array[largest])
                largest = left;
            
            // If right child is larger than largest so far
            if (right < n && array[right] > array[largest])
                largest = right;
            
            // If largest is not root
            if (largest != i)
            {
                int swap = array[i];
                array[i] = array[largest];
                array[largest] = swap;
                
                // Recursively heapify the affected sub-tree
                await Heapify(array, n, largest);
            }
        }
        
        /// <summary>
        /// Executes the Heap Sort algorithm and returns each step for visualization
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
                Description = "Initial array before Heap Sort.",
                StepNumber = _stepNumber++
            });
            
            int n = _array.Length;
            
            // Add description step for heap building phase
            _steps.Add(new AlgorithmStep<SortingState>
            {
                State = new SortingState
                {
                    Array = (int[])_array.Clone(),
                    IsSorted = (bool[])_isSorted.Clone()
                },
                Description = "Phase 1: Building a max heap from the array.",
                StepNumber = _stepNumber++
            });
            
            // Build heap (rearrange array)
            for (int i = n / 2 - 1; i >= 0; i--)
            {
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        CompareIndex1 = i
                    },
                    Description = $"Heapifying subtree with root at index {i}.",
                    StepNumber = _stepNumber++
                });
                
                await HeapifyWithSteps(n, i);
            }
            
            // Add description step for extraction phase
            _steps.Add(new AlgorithmStep<SortingState>
            {
                State = new SortingState
                {
                    Array = (int[])_array.Clone(),
                    IsSorted = (bool[])_isSorted.Clone()
                },
                Description = "Phase 2: Extracting elements from the heap one by one.",
                StepNumber = _stepNumber++
            });
            
            // One by one extract an element from heap
            for (int i = n - 1; i > 0; i--)
            {
                // Move current root to end
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        CompareIndex1 = 0,
                        CompareIndex2 = i
                    },
                    Description = $"Swapping root element {_array[0]} with last element {_array[i]} of the heap.",
                    StepNumber = _stepNumber++
                });
                
                int temp = _array[0];
                _array[0] = _array[i];
                _array[i] = temp;
                
                // Mark this position as sorted
                _isSorted[i] = true;
                
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone()
                    },
                    Description = $"Element {_array[i]} is now in its correct sorted position at index {i}.",
                    StepNumber = _stepNumber++
                });
                
                // Call max heapify on the reduced heap
                await HeapifyWithSteps(i, 0);
                
                // Add a small delay periodically to prevent UI locking
                if (i % 5 == 0)
                {
                    await Task.Delay(1);
                }
            }
            
            // Mark the first element as sorted too
            _isSorted[0] = true;
            
            // Final state
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
        /// To heapify a subtree with step tracking
        /// </summary>
        private async Task HeapifyWithSteps(int n, int i)
        {
            int largest = i; // Initialize largest as root
            int left = 2 * i + 1; // left = 2*i + 1
            int right = 2 * i + 2; // right = 2*i + 2
            
            // Check if left child exists and compare
            if (left < n)
            {
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        CompareIndex1 = i,
                        CompareIndex2 = left
                    },
                    Description = $"Comparing parent {_array[i]} at index {i} with left child {_array[left]} at index {left}.",
                    StepNumber = _stepNumber++
                });
                
                // If left child is larger than root
                if (_array[left] > _array[largest])
                {
                    largest = left;
                    
                    _steps.Add(new AlgorithmStep<SortingState>
                    {
                        State = new SortingState
                        {
                            Array = (int[])_array.Clone(),
                            IsSorted = (bool[])_isSorted.Clone(),
                            CompareIndex1 = largest
                        },
                        Description = $"Left child {_array[left]} is larger than parent {_array[i]}. Updating largest index to {largest}.",
                        StepNumber = _stepNumber++
                    });
                }
            }
            
            // Check if right child exists and compare
            if (right < n)
            {
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        CompareIndex1 = largest,
                        CompareIndex2 = right
                    },
                    Description = $"Comparing current largest {_array[largest]} at index {largest} with right child {_array[right]} at index {right}.",
                    StepNumber = _stepNumber++
                });
                
                // If right child is larger than largest so far
                if (_array[right] > _array[largest])
                {
                    largest = right;
                    
                    _steps.Add(new AlgorithmStep<SortingState>
                    {
                        State = new SortingState
                        {
                            Array = (int[])_array.Clone(),
                            IsSorted = (bool[])_isSorted.Clone(),
                            CompareIndex1 = largest
                        },
                        Description = $"Right child {_array[right]} is larger than current largest. Updating largest index to {largest}.",
                        StepNumber = _stepNumber++
                    });
                }
            }
            
            // If largest is not root
            if (largest != i)
            {
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        CompareIndex1 = i,
                        CompareIndex2 = largest
                    },
                    Description = $"Swapping {_array[i]} at index {i} with {_array[largest]} at index {largest} to maintain heap property.",
                    StepNumber = _stepNumber++
                });
                
                // Swap elements
                int swap = _array[i];
                _array[i] = _array[largest];
                _array[largest] = swap;
                
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone()
                    },
                    Description = $"After swap: {_array[i]} is now at index {i} and {_array[largest]} is at index {largest}.",
                    StepNumber = _stepNumber++
                });
                
                // Recursively heapify the affected sub-tree
                await HeapifyWithSteps(n, largest);
            }
            else
            {
                _steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])_array.Clone(),
                        IsSorted = (bool[])_isSorted.Clone(),
                        CompareIndex1 = i
                    },
                    Description = $"Heap property satisfied at index {i}, no swap needed.",
                    StepNumber = _stepNumber++
                });
            }
        }
    }
}