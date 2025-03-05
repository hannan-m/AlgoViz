namespace AlgorithmVisualizer.Models.TreeModels;

/// <summary>
/// Represents a node in a binary tree
/// </summary>
public class TreeNode
{
    /// <summary>
    /// Value stored in the node
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Reference to the left child
    /// </summary>
    public TreeNode Left { get; set; }

    /// <summary>
    /// Reference to the right child
    /// </summary>
    public TreeNode Right { get; set; }

    /// <summary>
    /// Whether this node is being highlighted during visualization
    /// </summary>
    public bool IsHighlighted { get; set; }

    /// <summary>
    /// Whether this node is currently being processed
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether this node is part of the result (e.g., search path)
    /// </summary>
    public bool IsResult { get; set; }

    /// <summary>
    /// Display position (x-coordinate) for visualization
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Display position (y-coordinate) for visualization
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Creates a deep copy of this node and its children
    /// </summary>
    public TreeNode Clone()
    {
        var newNode = new TreeNode
        {
            Value = this.Value,
            IsHighlighted = this.IsHighlighted,
            IsActive = this.IsActive,
            IsResult = this.IsResult,
            X = this.X,
            Y = this.Y
        };

        if (Left != null)
        {
            newNode.Left = Left.Clone();
        }

        if (Right != null)
        {
            newNode.Right = Right.Clone();
        }

        return newNode;
    }
}

/// <summary>
/// Represents the state of a tree algorithm visualization
/// </summary>
public class TreeState
{
    /// <summary>
    /// Root node of the tree
    /// </summary>
    public TreeNode Root { get; set; }

    /// <summary>
    /// Value being searched, inserted, or deleted
    /// </summary>
    public int? TargetValue { get; set; }

    /// <summary>
    /// List of nodes in the current search/traversal path
    /// </summary>
    public List<TreeNode> CurrentPath { get; set; } = new List<TreeNode>();

    /// <summary>
    /// Creates a deep copy of this tree state
    /// </summary>
    public TreeState Clone()
    {
        var clonedState = new TreeState
        {
            Root = Root?.Clone(),
            TargetValue = TargetValue,
            CurrentPath = new List<TreeNode>()
        };

        // We need to rebuild the current path with references to the cloned nodes
        if (Root != null && CurrentPath.Count > 0)
        {
            foreach (var node in CurrentPath)
            {
                var clonedNode = FindNode(clonedState.Root, node.Value);
                if (clonedNode != null)
                {
                    clonedState.CurrentPath.Add(clonedNode);
                }
            }
        }

        return clonedState;
    }

    /// <summary>
    /// Helper method to find a node in the cloned tree
    /// </summary>
    private TreeNode FindNode(TreeNode root, int value)
    {
        if (root == null)
            return null;

        if (root.Value == value)
            return root;

        var leftResult = FindNode(root.Left, value);
        if (leftResult != null)
            return leftResult;

        return FindNode(root.Right, value);
    }
}