using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlgorithmVisualizer.Models;
using AlgorithmVisualizer.Models.SortingModels;
using AlgorithmVisualizer.Services.Algorithms.Interfaces;

namespace AlgorithmVisualizer.Services.Algorithms.Sorting
{
    /// <summary>
    /// Implementation of Bubble Sort algorithm
    /// </summary>
    public class BubbleSortAlgorithm : ISortingAlgorithm
    {
        public string Name => "Bubble Sort";
        
        public string Description => "Bubble Sort is a simple comparison-based algorithm that repeatedly steps through the list, " +
                                    "compares adjacent elements, and swaps them if they are in the wrong order. " +
                                    "The pass through the list is repeated until the list is sorted.";
        
        public string TimeComplexity => "O(n²)";
        
        public string SpaceComplexity => "O(1)";
        
        /// <summary>
        /// Executes the Bubble Sort algorithm and returns the sorted array
        /// </summary>
        public async Task<int[]> ExecuteAsync(int[] array)
        {
            // Create a copy of the array to avoid modifying the original
            int[] sortedArray = (int[])array.Clone();
            int n = sortedArray.Length;
            
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (sortedArray[j] > sortedArray[j + 1])
                    {
                        // Swap elements
                        int temp = sortedArray[j];
                        sortedArray[j] = sortedArray[j + 1];
                        sortedArray[j + 1] = temp;
                    }
                }
                
                // Add small delay to prevent UI locking during long operations
                if (i % 10 == 0)
                {
                    await Task.Delay(1);
                }
            }
            
            return sortedArray;
        }
        
        /// <summary>
        /// Executes the Bubble Sort algorithm and returns each step for visualization
        /// </summary>
        public async Task<IEnumerable<AlgorithmStep<SortingState>>> GetStepsAsync(int[] array)
        {
            var steps = new List<AlgorithmStep<SortingState>>();
            int stepNumber = 0;
            
            // Create a copy of the array to avoid modifying the original
            int[] sortedArray = (int[])array.Clone();
            int n = sortedArray.Length;
            
            // Initialize the IsSorted array to track which elements are in their final sorted position
            bool[] isSorted = new bool[n];
            
            // Initial state
            steps.Add(new AlgorithmStep<SortingState>
            {
                State = new SortingState
                {
                    Array = (int[])sortedArray.Clone(),
                    IsSorted = (bool[])isSorted.Clone()
                },
                Description = "Initial array before sorting.",
                StepNumber = stepNumber++
            });
            
            bool swapped;
            for (int i = 0; i < n - 1; i++)
            {
                swapped = false;
                
                for (int j = 0; j < n - i - 1; j++)
                {
                    // Add a step for comparison
                    steps.Add(new AlgorithmStep<SortingState>
                    {
                        State = new SortingState
                        {
                            Array = (int[])sortedArray.Clone(),
                            CompareIndex1 = j,
                            CompareIndex2 = j + 1,
                            IsSorted = (bool[])isSorted.Clone()
                        },
                        Description = $"Comparing {sortedArray[j]} and {sortedArray[j + 1]}.",
                        StepNumber = stepNumber++
                    });
                    
                    if (sortedArray[j] > sortedArray[j + 1])
                    {
                        // Swap elements
                        int temp = sortedArray[j];
                        sortedArray[j] = sortedArray[j + 1];
                        sortedArray[j + 1] = temp;
                        swapped = true;
                        
                        // Add a step for the swap
                        steps.Add(new AlgorithmStep<SortingState>
                        {
                            State = new SortingState
                            {
                                Array = (int[])sortedArray.Clone(),
                                CompareIndex1 = j,
                                CompareIndex2 = j + 1,
                                IsSorted = (bool[])isSorted.Clone()
                            },
                            Description = $"Swapped {sortedArray[j + 1]} and {sortedArray[j]} because {sortedArray[j + 1]} < {sortedArray[j]}.",
                            StepNumber = stepNumber++
                        });
                    }
                }
                
                // Mark the last element of this iteration as sorted
                isSorted[n - i - 1] = true;
                
                // Add a step to show the sorted element
                steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])sortedArray.Clone(),
                        IsSorted = (bool[])isSorted.Clone()
                    },
                    Description = $"Element {sortedArray[n - i - 1]} is now in its correct sorted position.",
                    StepNumber = stepNumber++
                });
                
                // If no swaps occurred in this pass, the array is already sorted
                if (!swapped)
                {
                    // Mark all remaining elements as sorted
                    for (int k = 0; k < n - i - 1; k++)
                    {
                        isSorted[k] = true;
                    }
                    
                    steps.Add(new AlgorithmStep<SortingState>
                    {
                        State = new SortingState
                        {
                            Array = (int[])sortedArray.Clone(),
                            IsSorted = (bool[])isSorted.Clone()
                        },
                        Description = "No swaps needed in this pass. Array is sorted!",
                        StepNumber = stepNumber++
                    });
                    break;
                }
                
                // Add a small delay to prevent UI locking during long operations
                if (i % 5 == 0)
                {
                    await Task.Delay(1);
                }
            }
            
            // Final state
            steps.Add(new AlgorithmStep<SortingState>
            {
                State = new SortingState
                {
                    Array = sortedArray,
                    IsSorted = isSorted
                },
                Description = "Array is now fully sorted.",
                StepNumber = stepNumber++
            });
            
            return steps;
        }
    }
}