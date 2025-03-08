// Services/Algorithms/AlgorithmService.cs
using System.Collections.Generic;
using AlgorithmVisualizer.Services.Algorithms.Interfaces;
using AlgorithmVisualizer.Services.Algorithms.PathFinding;
using AlgorithmVisualizer.Services.Algorithms.Sorting;
using AlgorithmVisualizer.Services.Algorithms.Trees;
using Microsoft.Extensions.DependencyInjection;

namespace AlgorithmVisualizer.Services.Algorithms
{
    /// <summary>
    /// Service to register and provide access to all algorithms
    /// </summary>
    public static class AlgorithmService
    {
        /// <summary>
        /// Registers all algorithm services with the dependency injection container
        /// </summary>
        public static void RegisterAlgorithms(IServiceCollection services)
        {
            // Register pathfinding algorithms
            services.AddTransient<IPathFindingAlgorithm, DijkstraAlgorithm>();
            services.AddTransient<IPathFindingAlgorithm, AStarAlgorithm>();
            services.AddTransient<IPathFindingAlgorithm, BreadthFirstSearchAlgorithm>();
            services.AddTransient<IPathFindingAlgorithm, DepthFirstSearchAlgorithm>();
            services.AddTransient<IPathFindingAlgorithm, GreedyBestFirstSearchAlgorithm>();
            
            // Register sorting algorithms
            services.AddTransient<ISortingAlgorithm, BubbleSortAlgorithm>();
            services.AddTransient<ISortingAlgorithm, QuickSortAlgorithm>();
            services.AddTransient<ISortingAlgorithm, MergeSortAlgorithm>();
            services.AddTransient<ISortingAlgorithm, HeapSortAlgorithm>();
            services.AddTransient<ISortingAlgorithm, InsertionSortAlgorithm>();
            
            // Register tree algorithms
            services.AddTransient<ITreeAlgorithm, BinarySearchTreeAlgorithm>();
        }
        
        /// <summary>
        /// Gets all registered pathfinding algorithms
        /// </summary>
        public static List<IPathFindingAlgorithm> GetPathFindingAlgorithms(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetServices<IPathFindingAlgorithm>().ToList();
        }
        
        /// <summary>
        /// Gets all registered sorting algorithms
        /// </summary>
        public static List<ISortingAlgorithm> GetSortingAlgorithms(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetServices<ISortingAlgorithm>().ToList();
        }
        
        /// <summary>
        /// Gets all registered tree algorithms
        /// </summary>
        public static List<ITreeAlgorithm> GetTreeAlgorithms(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetServices<ITreeAlgorithm>().ToList();
        }
    }
}