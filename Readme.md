
# 🧮 Algorithm Visualizer

![License](https://img.shields.io/badge/license-MIT-blue) ![Version](https://img.shields.io/badge/version-1.0.0-green) ![Build Status](https://img.shields.io/badge/build-passing-brightgreen)

> An interactive educational tool for visualizing and understanding algorithms through step-by-step animations

[✨ Live Demo](https://algoviz.azurewebsites.net/)

<p align="center"> <img src="https://github.com/hannan-m/AlgoViz/blob/main/wwwroot/preview-main.png" alt="Algorithm Visualizer Screenshot" width="800"> </p>
<p align="center"> <img src="https://github.com/hannan-m/AlgoViz/blob/main/wwwroot/preview-pathfinding.png" alt="Algorithm Visualizer Screenshot" width="800"> </p>
<p align="center"> <img src="https://github.com/hannan-m/AlgoViz/blob/main/wwwroot/preview-tree.png" alt="Algorithm Visualizer Screenshot" width="800"> </p>
<p align="center"> <img src="https://github.com/hannan-m/AlgoViz/blob/main/wwwroot/preview-sorting.png" alt="Algorithm Visualizer Screenshot" width="800"> </p>

## ✨ Features

### 🔍 Interactive Visualization

Watch algorithms execute one step at a time with clear explanations of what's happening. Pause, rewind, and fast-forward through the execution process.

### 📊 Performance Metrics

Compare execution time and operation counts to understand algorithm efficiency across different inputs and scenarios.

### 🛠️ Interactive Editor

Create custom inputs and modify parameters in real-time to see how algorithms respond to different conditions.

### 📚 Educational Insights

Learn about algorithm complexity, strategies, and real-world applications with integrated explanations and resources.

## 🧩 Supported Algorithm Categories

### 🧭 Pathfinding Algorithms

Visualize how algorithms like A*, Dijkstra, BFS, and DFS find paths in a grid.

-   Compare informed vs uninformed search strategies
-   Understand heuristic functions in A*
-   Create custom maps with obstacles and weights

### 📋 Sorting Algorithms

Watch how algorithms like Quick Sort, Merge Sort, and Heap Sort rearrange arrays.

-   Compare time and space complexity
-   Understand divide and conquer approaches
-   See how initial conditions affect performance

### 🌳 Tree Algorithms

Explore operations on binary search trees like insertion, deletion, and traversal.

-   Understand tree-based data structures
-   Learn different traversal techniques
-   Visualize tree balancing operations

## 🚀 Getting Started

### Prerequisites

-   .NET 9.0 or later
-   A modern web browser

### Installation

1.  Clone the repository

```bash
git clone https://github.com/yourusername/algorithm-visualizer.git

```

2.  Navigate to the project directory

```bash
cd algorithm-visualizer

```

3.  Restore dependencies

```bash
dotnet restore

```

4.  Build the project

```bash
dotnet build

```

5.  Run the application

```bash
dotnet run

```

6.  Open your browser and navigate to `https://localhost:5001`

## 💻 Usage

1.  Select an algorithm category from the homepage
2.  Choose a specific algorithm to visualize
3.  Configure input parameters or use the default examples
4.  Click "Visualize" to start the step-by-step animation
5.  Use the playback controls to navigate through the algorithm execution
6.  View performance metrics and educational explanations alongside the visualization


## 🏗️ Project Structure

```
algorithm-visualizer/
├── Components/           # Blazor components
│   ├── Trees/            # Tree visualization components
│   ├── Pathfinding/      # Pathfinding visualization components
│   └── Sorting/          # Sorting visualization components
├── Models/               # Data models
├── Services/             # Backend services
│   └── Algorithms/       # Algorithm implementations
├── Shared/               # Shared components and layouts
├── wwwroot/              # Static web assets
└── Program.cs            # Application entry point

```


## 📝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1.  Fork the repository
2.  Create your feature branch (`git checkout -b feature/amazing-feature`)
3.  Commit your changes (`git commit -m 'Add some amazing feature'`)
4.  Push to the branch (`git push origin feature/amazing-feature`)
5.  Open a Pull Request

## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👏 Acknowledgments

-   Inspired by various algorithm visualization tools
-   Thanks to all contributors and educators in the computer science field
-   Built with .NET and Blazor WebAssembly

----------

<p align="center"> Made with ❤️ by <a href="https://github.com/hannan-m">Muhammad Hannan</a> </p>