namespace AlgorithmVisualizer.Models.GridModels;

/// <summary>
/// Represents a single cell in the pathfinding grid
/// </summary>
public class GridCell
{
    /// <summary>
    /// Row index (y-coordinate)
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Column index (x-coordinate)
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Cost to travel through this cell (for weighted algorithms)
    /// </summary>
    public int Weight { get; set; } = 1;

    /// <summary>
    /// Whether this cell is a wall/obstacle
    /// </summary>
    public bool IsWall { get; set; }

    /// <summary>
    /// Whether this cell is the start point
    /// </summary>
    public bool IsStart { get; set; }

    /// <summary>
    /// Whether this cell is the end/target point
    /// </summary>
    public bool IsEnd { get; set; }

    /// <summary>
    /// Whether this cell has been visited during algorithm execution
    /// </summary>
    public bool IsVisited { get; set; }

    /// <summary>
    /// Whether this cell is part of the final path
    /// </summary>
    public bool IsPath { get; set; }

    /// <summary>
    /// Current distance from start (for pathfinding algorithms)
    /// </summary>
    public int Distance { get; set; } = int.MaxValue;

    /// <summary>
    /// Estimated distance to end (for A* algorithm)
    /// </summary>
    public int Heuristic { get; set; }

    /// <summary>
    /// Total cost (distance + heuristic)
    /// </summary>
    public int TotalCost => Distance + Heuristic;

    /// <summary>
    /// Reference to the parent cell (for path reconstruction)
    /// </summary>
    public GridCell Parent { get; set; }

    /// <summary>
    /// Creates a deep copy of this cell
    /// </summary>
    public GridCell Clone()
    {
        return new GridCell
        {
            Row = this.Row,
            Column = this.Column,
            Weight = this.Weight,
            IsWall = this.IsWall,
            IsStart = this.IsStart,
            IsEnd = this.IsEnd,
            IsVisited = this.IsVisited,
            IsPath = this.IsPath,
            Distance = this.Distance,
            Heuristic = this.Heuristic,
            // Note: Parent reference is not cloned to avoid circular references
        };
    }
}

/// <summary>
/// Represents the entire state of the pathfinding grid
/// </summary>
public class GridState
{
    /// <summary>
    /// The 2D grid of cells
    /// </summary>
    public GridCell[,] Grid { get; set; }

    /// <summary>
    /// Number of rows in the grid
    /// </summary>
    public int Rows => Grid.GetLength(0);

    /// <summary>
    /// Number of columns in the grid
    /// </summary>
    public int Columns => Grid.GetLength(1);

    /// <summary>
    /// Reference to the starting cell
    /// </summary>
    public GridCell StartCell { get; set; }

    /// <summary>
    /// Reference to the end/target cell
    /// </summary>
    public GridCell EndCell { get; set; }

    /// <summary>
    /// List of cells currently being considered
    /// </summary>
    public List<GridCell> OpenSet { get; set; } = new List<GridCell>();

    /// <summary>
    /// Creates a deep copy of the grid state
    /// </summary>
    public GridState Clone()
    {
        var newGrid = new GridCell[Rows, Columns];

        // Clone each cell
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                newGrid[i, j] = Grid[i, j].Clone();

                // Restore parent references within the new grid
                if (Grid[i, j].Parent != null)
                {
                    int parentRow = Grid[i, j].Parent.Row;
                    int parentCol = Grid[i, j].Parent.Column;
                    newGrid[i, j].Parent = newGrid[parentRow, parentCol];
                }
            }
        }

        // Create new state
        var newState = new GridState
        {
            Grid = newGrid,
        };

        // Set start and end cell references in the new grid
        if (StartCell != null)
        {
            newState.StartCell = newGrid[StartCell.Row, StartCell.Column];
        }

        if (EndCell != null)
        {
            newState.EndCell = newGrid[EndCell.Row, EndCell.Column];
        }

        // Clone open set with references to new grid cells
        if (OpenSet != null)
        {
            newState.OpenSet = new List<GridCell>();
            foreach (var cell in OpenSet)
            {
                newState.OpenSet.Add(newGrid[cell.Row, cell.Column]);
            }
        }

        return newState;
    }
}