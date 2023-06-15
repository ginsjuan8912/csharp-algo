using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryTrees.Traverse
{

    public class Preorder
    {
        public IList<int> PreorderTraversal(TreeNode root)
        {
            var values = new List<int>();

            //Check if node is empty
            if(root == null)
                return new List<int>();

            //Check if is a root node
            values.Add(root.val);        

          
            //Check the left node, then the rigth node
            if(root.left != null)
            {             
                values.AddRange(PreorderTraversal(root.left));
            }
           

            if(root.right != null)
            {                
                values.AddRange(PreorderTraversal(root.right));
            }

            return values;
        }

    }
}
