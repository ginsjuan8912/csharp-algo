// See https://aka.ms/new-console-template for more information
/*
 Given an array of integers nums and an integer target, return indices of the two numbers such that they add up to target.
You may assume that each input would have exactly one solution, and you may not use the same element twice.
You can return the answer in any order.

 

Example 1:

Input: nums = [2,7,11,15], target = 9
Output: [0,1]
Output: Because nums[0] + nums[1] == 9, we return [0, 1].
Example 2:

Input: nums = [3,2,4], target = 6
Output: [1,2]
Example 3:

Input: nums = [3,3], target = 6
Output: [0,1]
 

Constraints:

2 <= nums.length <= 104
-109 <= nums[i] <= 109
-109 <= target <= 109
Only one valid answer exists. 
 */

/*Validation*/

using Microsoft.VisualStudio.TestTools.UnitTesting;

Solution solution = new();


var result = solution.TwoSum(new int[3] { 3, 2, 4 }, 6);
//Assert Validation
try
{
    var expected = new int[2] { 1, 2 };
    Assert.AreEqual(expected[0], result[0]);
    Assert.AreEqual(expected[1], result[1]);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("All test passed!");
}
catch (AssertFailedException asf)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"    - Test failed: ${asf.Message}");
}

Console.ReadKey();

/*Implementation*/

public class Solution
{
    public int[] TwoSum(int[] nums, int target)
    {
         Queue<int> result = new Queue<int>();

        /* 1- Get the target and substract it from  the first number in the array,
         * 2- Search the index of the substraction resutl
         *  if it finds a number, then return the two indexes
         *  if it doesn't find a number, continue with the next
          if there is not answer return empty
         */
        //Validate that there is more than two items in the array

        if (nums.Length >= 2)
        {           
            //Iterate through the numbers
            for (int i = 0; i < nums.Length; i++)
            {                
                for (int n = 0; n < nums.Length; n++)
                {
                    //Sum to numbers and see if they match target
                    var sum = nums[i] + nums[n];

                    if(sum == target && i != n)
                        result.Enqueue(i);                   
                }
            }

            if(result.Count > 1)
                return result.ToArray();
        }

        //return an empty list if no answer is found
        return new int[0];

    }
}




