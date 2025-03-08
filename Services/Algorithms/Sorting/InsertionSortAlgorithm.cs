using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlgorithmVisualizer.Models;
using AlgorithmVisualizer.Models.SortingModels;
using AlgorithmVisualizer.Services.Algorithms.Interfaces;

namespace AlgorithmVisualizer.Services.Algorithms.Sorting
{
    /// <summary>
    /// Implementation of Insertion Sort algorithm
    /// </summary>
    public class InsertionSortAlgorithm : ISortingAlgorithm
    {
        public string Name => "Insertion Sort";
        
        public string Description => "Insertion Sort is a simple sorting algorithm that builds the final sorted array one item at a time. " +
                                    "It is much less efficient on large lists than more advanced algorithms such as quicksort, heapsort, or merge sort " +
                                    "but can be efficient for small data sets, especially if they are already partially sorted.";
        
        public string TimeComplexity => "O(n²) worst case, O(n) best case";
        
        public string SpaceComplexity => "O(1)";
        
        /// <summary>
        /// Executes the Insertion Sort algorithm and returns the sorted array
        /// </summary>
        public async Task<int[]> ExecuteAsync(int[] array)
        {
            // Create a copy of the array to avoid modifying the original
            int[] sortedArray = (int[])array.Clone();
            int n = sortedArray.Length;
            
            for (int i = 1; i < n; i++)
            {
                int key = sortedArray[i];
                int j = i - 1;
                
                // Move elements of sortedArray[0..i-1] that are greater than key
                // to one position ahead of their current position
                while (j >= 0 && sortedArray[j] > key)
                {
                    sortedArray[j + 1] = sortedArray[j];
                    j = j - 1;
                }
                sortedArray[j + 1] = key;
                
                // Add a small delay periodically to prevent UI locking
                if (i % 10 == 0)
                {
                    await Task.Delay(1);
                }
            }
            
            return sortedArray;
        }
        
        /// <summary>
        /// Executes the Insertion Sort algorithm and returns each step for visualization
        /// </summary>
        public async Task<IEnumerable<AlgorithmStep<SortingState>>> GetStepsAsync(int[] array)
        {
            var steps = new List<AlgorithmStep<SortingState>>();
            int stepNumber = 0;
            
            // Create a copy of the array to avoid modifying the original
            int[] sortedArray = (int[])array.Clone();
            int n = sortedArray.Length;
            
            // Initialize the IsSorted array to track which elements are in their sorted position
            bool[] isSorted = new bool[n];
            
            // The first element is already "sorted" by itself
            isSorted[0] = true;
            
            // Initial state
            steps.Add(new AlgorithmStep<SortingState>
            {
                State = new SortingState
                {
                    Array = (int[])sortedArray.Clone(),
                    IsSorted = (bool[])isSorted.Clone()
                },
                Description = "Initial array. The first element is considered sorted by itself.",
                StepNumber = stepNumber++
            });
            
            for (int i = 1; i < n; i++)
            {
                int key = sortedArray[i];
                
                // Show the current element to be inserted
                steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])sortedArray.Clone(),
                        IsSorted = (bool[])isSorted.Clone(),
                        CompareIndex1 = i
                    },
                    Description = $"Inserting element {key} at index {i} into the sorted portion of the array.",
                    StepNumber = stepNumber++
                });
                
                int j = i - 1;
                
                // If the key is already in the right position (no shifting needed)
                if (j < 0 || sortedArray[j] <= key)
                {
                    steps.Add(new AlgorithmStep<SortingState>
                    {
                        State = new SortingState
                        {
                            Array = (int[])sortedArray.Clone(),
                            IsSorted = (bool[])isSorted.Clone(),
                            CompareIndex1 = j >= 0 ? j : null,
                            CompareIndex2 = i
                        },
                        Description = j >= 0 
                            ? $"Element {key} is already in the correct position (after {sortedArray[j]})."
                            : $"Element {key} is already in the correct position (at the beginning).",
                        StepNumber = stepNumber++
                    });
                }
                
                // Move elements of sortedArray[0..i-1] that are greater than key
                // to one position ahead of their current position
                while (j >= 0 && sortedArray[j] > key)
                {
                    // Show the comparison
                    steps.Add(new AlgorithmStep<SortingState>
                    {
                        State = new SortingState
                        {
                            Array = (int[])sortedArray.Clone(),
                            IsSorted = (bool[])isSorted.Clone(),
                            CompareIndex1 = j,
                            CompareIndex2 = j + 1
                        },
                        Description = $"Comparing {sortedArray[j]} with key {key}. Since {sortedArray[j]} > {key}, shifting {sortedArray[j]} to the right.",
                        StepNumber = stepNumber++
                    });
                    
                    sortedArray[j + 1] = sortedArray[j];
                    j = j - 1;
                    
                    // Show the shift
                    steps.Add(new AlgorithmStep<SortingState>
                    {
                        State = new SortingState
                        {
                            Array = (int[])sortedArray.Clone(),
                            IsSorted = (bool[])isSorted.Clone(),
                            CompareIndex1 = j >= 0 ? j : null,
                            CompareIndex2 = j + 1
                        },
                        Description = $"Shifted element to position {j + 1}. Looking at the next position to the left.",
                        StepNumber = stepNumber++
                    });
                }
                
                // Position found, place key
                sortedArray[j + 1] = key;
                
                // Show the insertion
                steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])sortedArray.Clone(),
                        IsSorted = (bool[])isSorted.Clone(),
                        CompareIndex1 = j + 1
                    },
                    Description = $"Inserted key {key} at position {j + 1}.",
                    StepNumber = stepNumber++
                });
                
                // Mark elements that are now in their final position
                for (int k = 0; k <= i; k++)
                {
                    isSorted[k] = true;
                }
                
                // Show the updated sorted portion
                steps.Add(new AlgorithmStep<SortingState>
                {
                    State = new SortingState
                    {
                        Array = (int[])sortedArray.Clone(),
                        IsSorted = (bool[])isSorted.Clone()
                    },
                    Description = $"Elements from index 0 to {i} are now sorted.",
                    StepNumber = stepNumber++
                });
                
                // Add a small delay periodically to prevent UI locking
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