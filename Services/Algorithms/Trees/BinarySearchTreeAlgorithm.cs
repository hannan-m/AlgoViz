using AlgorithmVisualizer.Models;
using AlgorithmVisualizer.Models.TreeModels;
using AlgorithmVisualizer.Services.Algorithms.Interfaces;

namespace AlgorithmVisualizer.Services.Algorithms.Trees
{
    /// <summary>
    /// Implementation of binary search tree operations with enhanced visualization support
    /// </summary>
    public class BinarySearchTreeAlgorithm : ITreeAlgorithm
    {
        public string Name => "Binary Search Tree";

        public string Description =>
            "A Binary Search Tree (BST) is a node-based binary tree data structure that has the following properties: " +
            "The left subtree of a node contains only nodes with keys less than the node's key. " +
            "The right subtree of a node contains only nodes with keys greater than the node's key. " +
            "Both the left and right subtrees are also binary search trees.";

        public string TimeComplexity =>
            "O(h) for search, insert, delete where h is height of the tree. In balanced trees, h = log(n)";

        public string SpaceComplexity => "O(n) for the tree, O(h) for recursive operations";

        private List<AlgorithmStep<TreeState>> _steps;
        private int _stepNumber;

        /// <summary>
        /// Executes the specified tree operation
        /// </summary>
        public async Task<TreeState> ExecuteAsync(TreeState initialState, TreeOperationType operation,
            int? value = null, Dictionary<string, object> parameters = null)
        {
            // Clone the initial state to avoid modifying it
            TreeState resultState = initialState.Clone();

            // Execute the specified operation
            switch (operation)
            {
                case TreeOperationType.Insert:
                    if (value.HasValue)
                        resultState.Root = Insert(resultState.Root, value.Value);
                    break;

                case TreeOperationType.Delete:
                    if (value.HasValue)
                        resultState.Root = Delete(resultState.Root, value.Value);
                    break;

                case TreeOperationType.Search:
                    if (value.HasValue)
                        Search(resultState.Root, value.Value);
                    break;

                case TreeOperationType.Traverse:
                    // Get traversal type from parameters
                    string traversalType = "InOrder"; // Default
                    if (parameters != null && parameters.ContainsKey("traversalType"))
                    {
                        traversalType = parameters["traversalType"].ToString();
                    }

                    PerformTraversal(resultState.Root, traversalType);
                    break;
            }

            // Add a small delay for UI responsiveness
            await Task.Delay(1);

            // Calculate node positions for visualization
            CalculateNodePositions(resultState.Root, 0, 0, 1);

            return resultState;
        }

        /// <summary>
        /// Performs the specified traversal type
        /// </summary>
        private void PerformTraversal(TreeNode root, string traversalType)
        {
            switch (traversalType)
            {
                case "InOrder":
                    InOrderTraverse(root);
                    break;
                case "PreOrder":
                    PreOrderTraverse(root);
                    break;
                case "PostOrder":
                    PostOrderTraverse(root);
                    break;
                case "LevelOrder":
                    LevelOrderTraverse(root);
                    break;
                default:
                    InOrderTraverse(root); // Default to in-order
                    break;
            }
        }

        /// <summary>
        /// In-order traversal (Left -> Root -> Right)
        /// </summary>
        private void InOrderTraverse(TreeNode root)
        {
            if (root == null)
                return;

            InOrderTraverse(root.Left);
            root.IsResult = true;
            InOrderTraverse(root.Right);
        }

        /// <summary>
        /// Pre-order traversal (Root -> Left -> Right)
        /// </summary>
        private void PreOrderTraverse(TreeNode root)
        {
            if (root == null)
                return;

            root.IsResult = true;
            PreOrderTraverse(root.Left);
            PreOrderTraverse(root.Right);
        }

        /// <summary>
        /// Post-order traversal (Left -> Right -> Root)
        /// </summary>
        private void PostOrderTraverse(TreeNode root)
        {
            if (root == null)
                return;

            PostOrderTraverse(root.Left);
            PostOrderTraverse(root.Right);
            root.IsResult = true;
        }

        /// <summary>
        /// Level-order traversal (Breadth-First)
        /// </summary>
        private void LevelOrderTraverse(TreeNode root)
        {
            if (root == null)
                return;

            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                TreeNode node = queue.Dequeue();
                node.IsResult = true;

                if (node.Left != null)
                    queue.Enqueue(node.Left);

                if (node.Right != null)
                    queue.Enqueue(node.Right);
            }
        }

        /// <summary>
        /// Gets detailed steps for tree operation visualization
        /// </summary>
        public async Task<IEnumerable<AlgorithmStep<TreeState>>> GetStepsAsync(
            TreeState initialState,
            TreeOperationType operation,
            int? value = null,
            Dictionary<string, object> parameters = null)
        {
            _steps = new List<AlgorithmStep<TreeState>>();
            _stepNumber = 0;

            // Clone the initial state to avoid modifying it
            TreeState currentState = initialState.Clone();

            // Set the target value for the operation
            currentState.TargetValue = value;
            CalculateNodePositions(currentState.Root, 0, 0, 5);

            // Add initial step
            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = currentState.Clone(),
                Description = GetOperationDescription(operation, value, parameters),
                StepNumber = _stepNumber++
            });

            // Execute the specified operation with step tracking
            switch (operation)
            {
                case TreeOperationType.Insert:
                    if (value.HasValue)
                    {
                        currentState.Root = await InsertWithStepsAsync(currentState.Root, value.Value, currentState);

                        // Final step after insertion
                        var insertedState = currentState.Clone();

                        // Calculate node positions for visualization
                        CalculateNodePositions(insertedState.Root, 0, 0, 5);

                        _steps.Add(new AlgorithmStep<TreeState>
                        {
                            State = insertedState,
                            Description = $"Node with value {value} has been inserted.",
                            StepNumber = _stepNumber++
                        });
                    }

                    break;

                case TreeOperationType.Delete:
                    if (value.HasValue)
                    {
                        currentState.Root = await DeleteWithStepsAsync(currentState.Root, value.Value, currentState);

                        // Final step after deletion
                        var deletedState = currentState.Clone();

                        // Calculate node positions for visualization
                        CalculateNodePositions(deletedState.Root, 0, 0, 5);

                        _steps.Add(new AlgorithmStep<TreeState>
                        {
                            State = deletedState,
                            Description = $"Node with value {value} has been deleted.",
                            StepNumber = _stepNumber++
                        });
                    }

                    break;

                case TreeOperationType.Search:
                    if (value.HasValue)
                    {
                        bool found = await SearchWithStepsAsync(currentState.Root, value.Value, currentState);

                        // Final step after search
                        var searchState = currentState.Clone();

                        // Calculate node positions for visualization
                        CalculateNodePositions(searchState.Root, 0, 0, 5);

                        _steps.Add(new AlgorithmStep<TreeState>
                        {
                            State = searchState,
                            Description =
                                found
                                    ? $"Node with value {value} has been found."
                                    : $"Node with value {value} is not in the tree.",
                            StepNumber = _stepNumber++
                        });
                    }

                    break;

                case TreeOperationType.Traverse:
                    // Get traversal type from parameters
                    string traversalType = "InOrder"; // Default
                    if (parameters != null && parameters.ContainsKey("traversalType"))
                    {
                        traversalType = parameters["traversalType"].ToString();
                    }

                    await TraverseWithStepsAsync(currentState.Root, currentState, traversalType);

                    // Final step after traversal
                    var traversedState = currentState.Clone();

                    // Calculate node positions for visualization
                    CalculateNodePositions(traversedState.Root, 0, 0, 5);

                    _steps.Add(new AlgorithmStep<TreeState>
                    {
                        State = traversedState,
                        Description = $"{traversalType} traversal complete.",
                        StepNumber = _stepNumber++
                    });
                    break;
            }

            // Ensure we calculate positions for all steps
            foreach (var step in _steps)
            {
                CalculateNodePositions(step.State.Root, 0, 0, 5);
            }

            return _steps;
        }

        /// <summary>
        /// Gets a description of the operation
        /// </summary>
        private string GetOperationDescription(TreeOperationType operation, int? value,
            Dictionary<string, object> parameters = null)
        {
            if (operation == TreeOperationType.Traverse && parameters != null &&
                parameters.ContainsKey("traversalType"))
            {
                string traversalType = parameters["traversalType"].ToString();
                return $"Performing {traversalType} traversal of the tree.";
            }

            return operation switch
            {
                TreeOperationType.Insert => $"Inserting node with value {value} into the tree.",
                TreeOperationType.Delete => $"Deleting node with value {value} from the tree.",
                TreeOperationType.Search => $"Searching for node with value {value} in the tree.",
                TreeOperationType.Traverse => "Performing traversal of the tree.",
                _ => "Unknown operation."
            };
        }

        /// <summary>
        /// Inserts a new node into the tree
        /// </summary>
        private TreeNode Insert(TreeNode root, int value)
        {
            // If the tree is empty, create a new root node
            if (root == null)
            {
                return new TreeNode { Value = value };
            }

            // Recursively insert into the appropriate subtree
            if (value < root.Value)
            {
                root.Left = Insert(root.Left, value);
            }
            else if (value > root.Value)
            {
                root.Right = Insert(root.Right, value);
            }
            // Equal values are not allowed in this BST implementation

            return root;
        }

        /// <summary>
        /// Inserts a new node with detailed steps
        /// </summary>
        private async Task<TreeNode> InsertWithStepsAsync(TreeNode root, int value, TreeState currentState)
        {
            // Clone the current state for this step
            var stepState = currentState.Clone();

            if (root != null)
            {
                // Mark the current node as active
                root.IsActive = true;

                // Add step for node comparison
                CalculateNodePositions(stepState.Root, 0, 0, 5);
                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = stepState,
                    Description = root != null
                        ? $"Comparing {value} with node {root.Value}."
                        : "Tree is empty, creating root node.",
                    StepNumber = _stepNumber++
                });

                // Short delay for visualization
                await Task.Delay(1);

                // Reset active flag
                root.IsActive = false;
            }

            // If the tree is empty, create a new root node
            if (root == null)
            {
                TreeNode newNode = new TreeNode { Value = value, IsHighlighted = true };

                // Add step for new node creation
                var newNodeState = currentState.Clone();
                newNodeState.Root = newNode;

                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = newNodeState,
                    Description = "Created new node as the root of the tree.",
                    StepNumber = _stepNumber++
                });

                return newNode;
            }

            // Recursively insert into the appropriate subtree
            if (value < root.Value)
            {
                // Mark the path we're taking
                root.IsHighlighted = true;

                // Add step for going to left subtree
                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = stepState,
                    Description = $"{value} < {root.Value}, going to left subtree.",
                    StepNumber = _stepNumber++
                });

                // Update the actual node in the current state
                var currentNode = FindNode(currentState.Root, root.Value);
                if (currentNode != null)
                {
                    currentNode.IsHighlighted = true;
                    currentNode.Left = await InsertWithStepsAsync(root.Left?.Clone(), value, currentState);
                }
            }
            else if (value > root.Value)
            {
                // Mark the path we're taking
                root.IsHighlighted = true;

                // Add step for going to right subtree
                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = stepState,
                    Description = $"{value} > {root.Value}, going to right subtree.",
                    StepNumber = _stepNumber++
                });

                // Update the actual node in the current state
                var currentNode = FindNode(currentState.Root, root.Value);
                if (currentNode != null)
                {
                    currentNode.IsHighlighted = true;
                    currentNode.Right = await InsertWithStepsAsync(root.Right?.Clone(), value, currentState);
                }
            }
            else // Equal values
            {
                // Mark the duplicate node
                root.IsHighlighted = true;

                // Add step for duplicate value
                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = stepState,
                    Description = $"{value} already exists in the tree. BST doesn't allow duplicates.",
                    StepNumber = _stepNumber++
                });
            }

            return root;
        }

        /// <summary>
        /// Searches for a node in the tree
        /// </summary>
        private bool Search(TreeNode root, int value)
        {
            // Base case: not found or found
            if (root == null)
                return false;

            if (root.Value == value)
            {
                root.IsResult = true;
                return true;
            }

            // Recursively search in the appropriate subtree
            if (value < root.Value)
                return Search(root.Left, value);
            else
                return Search(root.Right, value);
        }

        /// <summary>
        /// Searches for a node with detailed steps
        /// </summary>
        private async Task<bool> SearchWithStepsAsync(TreeNode root, int value, TreeState currentState)
        {
            // Clone the current state for this step
            var stepState = currentState.Clone();

            // Base case: not found
            if (root == null)
            {
                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = stepState,
                    Description = $"Reached a null node. Value {value} is not in the tree.",
                    StepNumber = _stepNumber++
                });

                return false;
            }

            // Mark the current node as active
            var currentNode = FindNode(currentState.Root, root.Value);
            if (currentNode != null)
            {
                currentNode.IsActive = true;

                // Add this node to the current path
                currentState.CurrentPath.Add(currentNode);
            }

            // Add step for node comparison
            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = stepState,
                Description = $"Comparing {value} with node {root.Value}.",
                StepNumber = _stepNumber++
            });

            // Short delay for visualization
            await Task.Delay(1);

            // Reset active flag but keep the node highlighted to show the search path
            if (currentNode != null)
            {
                currentNode.IsActive = false;
                currentNode.IsHighlighted = true;
            }

            // Check if we found the node
            if (root.Value == value)
            {
                // Mark the node as the result
                if (currentNode != null)
                {
                    currentNode.IsResult = true;
                }

                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = currentState.Clone(),
                    Description = $"Found node with value {value}!",
                    StepNumber = _stepNumber++
                });

                return true;
            }

            // Recursively search in the appropriate subtree
            if (value < root.Value)
            {
                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = currentState.Clone(),
                    Description = $"{value} < {root.Value}, going to left subtree.",
                    StepNumber = _stepNumber++
                });

                return await SearchWithStepsAsync(root.Left, value, currentState);
            }
            else
            {
                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = currentState.Clone(),
                    Description = $"{value} > {root.Value}, going to right subtree.",
                    StepNumber = _stepNumber++
                });

                return await SearchWithStepsAsync(root.Right, value, currentState);
            }
        }

        /// <summary>
        /// Deletes a node from the tree
        /// </summary>
        private TreeNode Delete(TreeNode root, int value)
        {
            // Base case: empty tree or not found
            if (root == null)
                return null;

            // Recursively search for the node to delete
            if (value < root.Value)
            {
                root.Left = Delete(root.Left, value);
            }
            else if (value > root.Value)
            {
                root.Right = Delete(root.Right, value);
            }
            else
            {
                // Node found, handle different cases

                // Case 1: Leaf node (no children)
                if (root.Left == null && root.Right == null)
                    return null;

                // Case 2: Node with only one child
                if (root.Left == null)
                    return root.Right;
                if (root.Right == null)
                    return root.Left;

                // Case 3: Node with two children
                // Find the inorder successor (smallest node in right subtree)
                TreeNode successor = FindMin(root.Right);

                // Copy the successor value to this node
                root.Value = successor.Value;

                // Delete the successor
                root.Right = Delete(root.Right, successor.Value);
            }

            return root;
        }

        /// <summary>
        /// Deletes a node with detailed steps
        /// </summary>
        private async Task<TreeNode> DeleteWithStepsAsync(TreeNode root, int value, TreeState currentState)
        {
            // Clone the current state for this step
            var stepState = currentState.Clone();

            // Base case: empty tree or not found
            if (root == null)
            {
                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = stepState,
                    Description = $"Reached a null node. Value {value} is not in the tree.",
                    StepNumber = _stepNumber++
                });

                return null;
            }

            // Mark the current node as active
            var currentNode = FindNode(currentState.Root, root.Value);
            if (currentNode != null)
            {
                currentNode.IsActive = true;
            }

            // Add step for node comparison
            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = stepState,
                Description = $"Comparing {value} with node {root.Value}.",
                StepNumber = _stepNumber++
            });

            // Short delay for visualization
            await Task.Delay(1);

            // Reset active flag but keep highlighted to show path
            if (currentNode != null)
            {
                currentNode.IsActive = false;
                currentNode.IsHighlighted = true;
            }

            // Recursively search for the node to delete
            if (value < root.Value)
            {
                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = currentState.Clone(),
                    Description = $"{value} < {root.Value}, going to left subtree.",
                    StepNumber = _stepNumber++
                });

                if (currentNode != null)
                {
                    currentNode.Left = await DeleteWithStepsAsync(root.Left, value, currentState);
                }
            }
            else if (value > root.Value)
            {
                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = currentState.Clone(),
                    Description = $"{value} > {root.Value}, going to right subtree.",
                    StepNumber = _stepNumber++
                });

                if (currentNode != null)
                {
                    currentNode.Right = await DeleteWithStepsAsync(root.Right, value, currentState);
                }
            }
            else
            {
                // Node found
                if (currentNode != null)
                {
                    // Mark the node to be deleted
                    currentNode.IsActive = true;
                    currentNode.IsHighlighted = false;
                }

                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = currentState.Clone(),
                    Description = $"Found node with value {value} to delete.",
                    StepNumber = _stepNumber++
                });

                // Identify parent node for updates
                TreeNode parentNode = FindParent(currentState.Root, root.Value);

                // Case 1: Leaf node (no children)
                if (root.Left == null && root.Right == null)
                {
                    _steps.Add(new AlgorithmStep<TreeState>
                    {
                        State = currentState.Clone(),
                        Description = $"Node {value} is a leaf node. Simply removing it.",
                        StepNumber = _stepNumber++
                    });

                    // Remove the node from the parent
                    if (parentNode != null)
                    {
                        if (parentNode.Left?.Value == value)
                            parentNode.Left = null;
                        else
                            parentNode.Right = null;
                    }
                    else
                    {
                        // It's the root node
                        currentState.Root = null;
                    }

                    _steps.Add(new AlgorithmStep<TreeState>
                    {
                        State = currentState.Clone(),
                        Description = $"Node {value} has been removed.",
                        StepNumber = _stepNumber++
                    });

                    return null;
                }

                // Case 2: Node with only one child
                if (root.Left == null)
                {
                    // Highlight the child that will replace it
                    if (currentNode?.Right != null)
                    {
                        currentNode.Right.IsHighlighted = true;
                    }

                    _steps.Add(new AlgorithmStep<TreeState>
                    {
                        State = currentState.Clone(),
                        Description = $"Node {value} has only a right child. Replacing it with its right child.",
                        StepNumber = _stepNumber++
                    });

                    // Update the parent to point to the right child
                    if (parentNode != null)
                    {
                        if (parentNode.Left?.Value == value)
                            parentNode.Left = currentNode?.Right;
                        else
                            parentNode.Right = currentNode?.Right;
                    }
                    else
                    {
                        // It's the root node
                        currentState.Root = currentState.Root?.Right;
                    }

                    _steps.Add(new AlgorithmStep<TreeState>
                    {
                        State = currentState.Clone(),
                        Description = $"Node {value} has been replaced with its right child.",
                        StepNumber = _stepNumber++
                    });

                    return root.Right;
                }

                if (root.Right == null)
                {
                    // Highlight the child that will replace it
                    if (currentNode?.Left != null)
                    {
                        currentNode.Left.IsHighlighted = true;
                    }

                    _steps.Add(new AlgorithmStep<TreeState>
                    {
                        State = currentState.Clone(),
                        Description = $"Node {value} has only a left child. Replacing it with its left child.",
                        StepNumber = _stepNumber++
                    });

                    // Update the parent to point to the left child
                    if (parentNode != null)
                    {
                        if (parentNode.Left?.Value == value)
                            parentNode.Left = currentNode?.Left;
                        else
                            parentNode.Right = currentNode?.Left;
                    }
                    else
                    {
                        // It's the root node
                        currentState.Root = currentState.Root?.Left;
                    }

                    _steps.Add(new AlgorithmStep<TreeState>
                    {
                        State = currentState.Clone(),
                        Description = $"Node {value} has been replaced with its left child.",
                        StepNumber = _stepNumber++
                    });

                    return root.Left;
                }

                // Case 3: Node with two children
                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = currentState.Clone(),
                    Description =
                        $"Node {value} has two children. Need to find the inorder successor (smallest node in right subtree).",
                    StepNumber = _stepNumber++
                });

                // Find the inorder successor
                TreeNode successor = FindMin(root.Right);

                // Highlight the successor
                TreeNode successorNode = FindNode(currentState.Root, successor.Value);
                if (successorNode != null)
                {
                    successorNode.IsHighlighted = true;
                }

                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = currentState.Clone(),
                    Description = $"Found inorder successor with value {successor.Value}.",
                    StepNumber = _stepNumber++
                });

                // Copy the successor value to this node
                int oldValue = root.Value;
                root.Value = successor.Value;
                if (currentNode != null)
                {
                    currentNode.Value = successor.Value;
                }

                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = currentState.Clone(),
                    Description = $"Replaced value {oldValue} with successor value {successor.Value}.",
                    StepNumber = _stepNumber++
                });

                // Now delete the successor
                if (successorNode != null)
                {
                    successorNode.IsHighlighted = false;
                }

                _steps.Add(new AlgorithmStep<TreeState>
                {
                    State = currentState.Clone(),
                    Description =
                        $"Now need to delete the successor node with value {successor.Value} from the right subtree.",
                    StepNumber = _stepNumber++
                });

                // Delete the successor from the right subtree
                if (currentNode != null)
                {
                    currentNode.Right = await DeleteWithStepsAsync(root.Right, successor.Value, currentState);
                }
            }

            return root;
        }

        /// <summary>
        /// Finds the node with the minimum value in the tree
        /// </summary>
        private TreeNode FindMin(TreeNode root)
        {
            if (root == null)
                return null;

            TreeNode current = root;

            // Leftmost node has the minimum value
            while (current.Left != null)
            {
                current = current.Left;
            }

            return current;
        }

        /// <summary>
        /// Performs traversal with detailed steps
        /// </summary>
        private async Task TraverseWithStepsAsync(TreeNode root, TreeState currentState, string traversalType)
        {
            switch (traversalType)
            {
                case "InOrder":
                    await InOrderTraverseWithStepsAsync(root, currentState);
                    break;
                case "PreOrder":
                    await PreOrderTraverseWithStepsAsync(root, currentState);
                    break;
                case "PostOrder":
                    await PostOrderTraverseWithStepsAsync(root, currentState);
                    break;
                case "LevelOrder":
                    await LevelOrderTraverseWithStepsAsync(root, currentState);
                    break;
                default:
                    await InOrderTraverseWithStepsAsync(root, currentState); // Default to in-order
                    break;
            }
        }

        /// <summary>
        /// In-order traversal with detailed steps (Left -> Root -> Right)
        /// </summary>
        private async Task InOrderTraverseWithStepsAsync(TreeNode root, TreeState currentState)
        {
            if (root == null)
                return;

            // Clone the current state for the step
            var stepState = currentState.Clone();

            // Find the actual node in the current state
            var currentNode = FindNode(currentState.Root, root.Value);

            // Visit left subtree
            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = stepState,
                Description = currentNode?.Left != null
                    ? $"At node {root.Value}, going to left subtree first (InOrder: Left->Root->Right)."
                    : $"At node {root.Value}, there is no left subtree.",
                StepNumber = _stepNumber++
            });

            if (currentNode?.Left != null)
            {
                await InOrderTraverseWithStepsAsync(root.Left, currentState);
            }

            // Visit root node
            if (currentNode != null)
            {
                currentNode.IsActive = true;
                await Task.Delay(1);
                currentNode.IsActive = false;
                currentNode.IsResult = true;
            }

            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = currentState.Clone(),
                Description = $"Visiting node {root.Value} (InOrder).",
                StepNumber = _stepNumber++
            });

            // Visit right subtree
            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = currentState.Clone(),
                Description = currentNode?.Right != null
                    ? $"At node {root.Value}, going to right subtree now."
                    : $"At node {root.Value}, there is no right subtree.",
                StepNumber = _stepNumber++
            });

            if (currentNode?.Right != null)
            {
                await InOrderTraverseWithStepsAsync(root.Right, currentState);
            }
        }

        /// <summary>
        /// Pre-order traversal with detailed steps (Root -> Left -> Right)
        /// </summary>
        private async Task PreOrderTraverseWithStepsAsync(TreeNode root, TreeState currentState)
        {
            if (root == null)
                return;

            // Clone the current state for the step
            var stepState = currentState.Clone();

            // Find the actual node in the current state
            var currentNode = FindNode(currentState.Root, root.Value);

            // Visit root node first
            if (currentNode != null)
            {
                currentNode.IsActive = true;
                await Task.Delay(1);
                currentNode.IsActive = false;
                currentNode.IsResult = true;
            }

            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = currentState.Clone(),
                Description = $"Visiting node {root.Value} (PreOrder: Root->Left->Right).",
                StepNumber = _stepNumber++
            });

            // Visit left subtree
            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = currentState.Clone(),
                Description = currentNode?.Left != null
                    ? $"At node {root.Value}, going to left subtree now."
                    : $"At node {root.Value}, there is no left subtree.",
                StepNumber = _stepNumber++
            });

            if (currentNode?.Left != null)
            {
                await PreOrderTraverseWithStepsAsync(root.Left, currentState);
            }

            // Visit right subtree
            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = currentState.Clone(),
                Description = currentNode?.Right != null
                    ? $"At node {root.Value}, going to right subtree now."
                    : $"At node {root.Value}, there is no right subtree.",
                StepNumber = _stepNumber++
            });

            if (currentNode?.Right != null)
            {
                await PreOrderTraverseWithStepsAsync(root.Right, currentState);
            }
        }

        /// <summary>
        /// Post-order traversal with detailed steps (Left -> Right -> Root)
        /// </summary>
        private async Task PostOrderTraverseWithStepsAsync(TreeNode root, TreeState currentState)
        {
            if (root == null)
                return;

            // Clone the current state for the step
            var stepState = currentState.Clone();

            // Find the actual node in the current state
            var currentNode = FindNode(currentState.Root, root.Value);

            // Visit left subtree first
            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = stepState,
                Description = currentNode?.Left != null
                    ? $"At node {root.Value}, going to left subtree first (PostOrder: Left->Right->Root)."
                    : $"At node {root.Value}, there is no left subtree.",
                StepNumber = _stepNumber++
            });

            if (currentNode?.Left != null)
            {
                await PostOrderTraverseWithStepsAsync(root.Left, currentState);
            }

            // Visit right subtree
            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = currentState.Clone(),
                Description = currentNode?.Right != null
                    ? $"At node {root.Value}, going to right subtree now."
                    : $"At node {root.Value}, there is no right subtree.",
                StepNumber = _stepNumber++
            });

            if (currentNode?.Right != null)
            {
                await PostOrderTraverseWithStepsAsync(root.Right, currentState);
            }

            // Visit root node last
            if (currentNode != null)
            {
                currentNode.IsActive = true;
                await Task.Delay(1);
                currentNode.IsActive = false;
                currentNode.IsResult = true;
            }

            _steps.Add(new AlgorithmStep<TreeState>
            {
                State = currentState.Clone(),
                Description = $"Visiting node {root.Value} (PostOrder).",
                StepNumber = _stepNumber++
            });
        }

        /// <summary>
        /// Level-order traversal with detailed steps (Breadth-First)
        /// </summary>
        private async Task LevelOrderTraverseWithStepsAsync(TreeNode root, TreeState currentState)
        {
            if (root == null)
                return;

            // Create a queue for level-order traversal
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(root);

            int level = 1;
            int nodesInCurrentLevel = 1;
            int nodesInNextLevel = 0;

            while (queue.Count > 0)
            {
                TreeNode node = queue.Dequeue();
                nodesInCurrentLevel--;

                // Find the actual node in the current state
                var currentNode = FindNode(currentState.Root, node.Value);

                if (currentNode != null)
                {
                    currentNode.IsActive = true;

                    _steps.Add(new AlgorithmStep<TreeState>
                    {
                        State = currentState.Clone(),
                        Description = $"Visiting node {node.Value} at level {level} (LevelOrder).",
                        StepNumber = _stepNumber++
                    });

                    await Task.Delay(1);

                    currentNode.IsActive = false;
                    currentNode.IsResult = true;

                    // Enqueue children
                    if (node.Left != null)
                    {
                        queue.Enqueue(node.Left);
                        nodesInNextLevel++;
                    }

                    if (node.Right != null)
                    {
                        queue.Enqueue(node.Right);
                        nodesInNextLevel++;
                    }

                    // Check if we're moving to the next level
                    if (nodesInCurrentLevel == 0)
                    {
                        if (nodesInNextLevel > 0)
                        {
                            _steps.Add(new AlgorithmStep<TreeState>
                            {
                                State = currentState.Clone(),
                                Description = $"Moving to level {level + 1}.",
                                StepNumber = _stepNumber++
                            });
                        }

                        level++;
                        nodesInCurrentLevel = nodesInNextLevel;
                        nodesInNextLevel = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the visual positions for tree nodes
        /// </summary>
        private void CalculateNodePositions(TreeNode node, double x, double y, double horizontalSpacing)
        {
            if (node == null)
                return;

            // Set the position of the current node
            node.X = x;
            node.Y = y;

            // Calculate positions for children
            double nextY = y + 1.5; // Vertical distance

            // Adjust horizontal spacing based on tree depth
            double depth = GetNodeDepth(node);
            double adjustedHorizontalSpacing = horizontalSpacing / Math.Max(1, Math.Log(depth + 1));

            double leftX = x - adjustedHorizontalSpacing; // Left child x-position
            double rightX = x + adjustedHorizontalSpacing; // Right child x-position

            // Process left subtree
            if (node.Left != null)
            {
                CalculateNodePositions(node.Left, leftX, nextY, adjustedHorizontalSpacing);
            }

            // Process right subtree
            if (node.Right != null)
            {
                CalculateNodePositions(node.Right, rightX, nextY, adjustedHorizontalSpacing);
            }
        }

        /// <summary>
        /// Gets the depth of a node in the tree
        /// </summary>
        private int GetNodeDepth(TreeNode node)
        {
            if (node == null)
                return 0;

            int leftDepth = GetNodeDepth(node.Left);
            int rightDepth = GetNodeDepth(node.Right);

            return Math.Max(leftDepth, rightDepth) + 1;
        }

        /// <summary>
        /// Finds a node with the specified value in the tree
        /// </summary>
        private TreeNode FindNode(TreeNode root, int value)
        {
            if (root == null)
                return null;

            if (root.Value == value)
                return root;

            TreeNode leftResult = FindNode(root.Left, value);
            if (leftResult != null)
                return leftResult;

            return FindNode(root.Right, value);
        }

        /// <summary>
        /// Finds the parent of a node with the specified value
        /// </summary>
        private TreeNode FindParent(TreeNode root, int value)
        {
            if (root == null)
                return null;

            // Check if this node is the parent of the target
            if ((root.Left != null && root.Left.Value == value) ||
                (root.Right != null && root.Right.Value == value))
            {
                return root;
            }

            // Recursively search in the appropriate subtree
            if (value < root.Value)
            {
                return FindParent(root.Left, value);
            }
            else if (value > root.Value)
            {
                return FindParent(root.Right, value);
            }

            // Value is the root, so it has no parent
            return null;
        }
    }
}