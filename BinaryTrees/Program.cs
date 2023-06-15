
using BinaryTrees;
using BinaryTrees.Traverse;


Preorder preorder = new Preorder();
var result = preorder.PreorderTraversal(TreeNode.CreateTree("[3,1,2]"));

result.ToList().ForEach(n => Console.Write($"{n},"));
Console.ReadKey();



