namespace BinaryTrees
{
    public class TreeNode
    {
        public int val;
        public TreeNode left;
        public TreeNode right;
        public TreeNode(int val = 0, TreeNode left = null, TreeNode right = null)
        {
            this.val = val;
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Create a tree based on a string of array i.e [1,null,2,3]
        /// where first index is root, even index is left node, and odd index is rigth node
        /// </summary>
        /// <param name="tree">A string representation of the tree</param>
        /// <returns>Return a well-formed tree with its values</returns>
        public static TreeNode CreateTree(string tree)
        {
            TreeNode root = new TreeNode();
            tree = tree.Replace("[", "");
            tree = tree.Replace("]", "");

            var mapTree = tree.Split(',').ToList();

            Queue<(string, int, int)> queue = new Queue<(string, int, int)>();

            int level = 0;
            short count = 0;

            for (int i = 0; i < mapTree.Count; i++)
            {
                var node = (node: mapTree[i], index: i, level: level);
                queue.Enqueue(node);


                if (count == 0)
                    level++;

                if (count >= 2)
                {
                    level++;
                    count = 0;
                }

                count++;

            }

            if (queue.Count >= 1)
            {
                var r = queue.Dequeue();
                //Parse
                var refVal = -1;
                int.TryParse(r.Item1, out refVal);

                if (refVal > -1)
                {
                    root.val = refVal;
                    root = CreateNode(root, queue, 1);
                }

            }

            return root;
        }

        /// <summary>
        /// Create a node passing its root, and childs as a queue
        /// </summary>
        /// <param name="root">The root node</param>
        /// <param name="childs">The childs that are going to be added, left or rigth</param>
        /// <returns>A node with its children added</returns>
        private static TreeNode CreateNode(TreeNode root, Queue<(string, int, int)> childs, int currentLevel)
        {
            if (childs.Count == 0)
                return root;

            (string, int, int) rNode = ("", -1, -1);
            (string, int, int) lNode = ("", -1, -1);

            //Deque rigth node
            if (childs.Count > 0)
                rNode = childs.Dequeue();

            //Deque left node
            if (childs.Count > 0)
                lNode = childs.Dequeue();


            //Add rigth node, and all its childs
            if (rNode.Item2 % 2 != 0 && rNode.Item3 == currentLevel)
            {

                int? nodeVal = GetValue(rNode);
                if (nodeVal.HasValue)
                    root.right = CreateNode(new TreeNode(nodeVal.Value), childs, ++rNode.Item3);
            }

            //Add left node, and all its childs
            if (lNode.Item2 % 2 == 0 && lNode.Item3 == currentLevel)
            {

                int? nodeVal = GetValue(lNode);
                if (nodeVal.HasValue)
                    root.left = CreateNode(new TreeNode(nodeVal.Value), childs, ++lNode.Item3);
            }

            return root;

        }

        private static int? GetValue((string, int, int) node)
        {
            int? val;
            if (node.Item1 == "null")
                val = null;
            else
                val = int.Parse(node.Item1);

            return val;
        }
    }
}
