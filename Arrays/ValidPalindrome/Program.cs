/*
 Given a string s, determine if it is a palindrome, considering only alphanumeric characters and ignoring cases. 

Example 1:

Input: s = "A man, a plan, a canal: Panama"
Output: true
Explanation: "amanaplanacanalpanama" is a palindrome.
Example 2:

Input: s = "race a car"
Output: false
Explanation: "raceacar" is not a palindrome.
 

Constraints:

1 <= s.length <= 2 * 105
s consists only of printable ASCII characters.
 */

/*IMPLEMENTATION*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

try
{
    Solution solution = new();
    Assert.AreEqual(true, solution.IsPalindrome("A man, a plan, a canal -- Panama"));
    Assert.AreEqual(true, solution.IsPalindrome("madam"));
    Assert.AreEqual(true, solution.IsPalindrome("A man, a plan, a canal: Panama"));
    Assert.AreEqual(false, solution.IsPalindrome("race a car"));

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("All test passed!");
}
catch (AssertFailedException asf)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"    - Test failed: ${asf.Message}");
}

Console.ReadKey();

public class Solution
{
    public bool IsPalindrome(string s)
    {
        /*To validate if is palindrome we will have to
         * build an array of tuple with both indices, 
         * one from the beggining and the corresponding 
         * from the rigth side of the array
         * 
         * i.e: m a d a m
          *     0 1 2 3 4
          *             
          *             
          *  Validation array to be builded
          *  
          * Array= {(0,4),(1,3),(2,2),(3,1),(4,0)}
          *          m m   a a   d d   a a   m,m 
          * by getting the characters from the first
          * and secound should be the same
         */

        //First clean the string and put it in a array
        s = s.ToLower();
        //s = s.Replace("-","",)
        var sanitazedStr = Regex.Replace(s, @"[^a-zA-Z0-9]", "").ToArray();
        var tuples = new List<(int, int)>();

        //get the middle index of the string
        var middleIndex = sanitazedStr.Length / 2;

        //Create the validation tuple
        for (int i = 0; i < sanitazedStr.Length; i++)
        {
            if(middleIndex == i)
            {
                //At the middle point of the array
                tuples.Add((middleIndex, middleIndex));
                continue;
            }

            tuples.Add((i, (sanitazedStr.Length -1) - i));
        }

        //Do the palindrome validation
        foreach (var item in tuples)
        {
            if (sanitazedStr[item.Item1] != sanitazedStr[item.Item2])
                return false;
        }

        return true;
    }
}