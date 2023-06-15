using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace LRUCache
{
    class Program
    {

        /*
         * LRUCache(int capacity) Initialize the LRU cache with positive size capacity.
        int get(int key) Return the value of the key if the key exists, otherwise return -1.
        void put(int key, int value) Update the value of the key if the key exists. Otherwise,
        add the key-value pair to the cache. If the number of keys exceeds the capacity from this operation, evict the least recently used key.
        The functions get and put must each run in O(1) average time complexity.

         * **/
        public class LRUCache
        {
            //Set the constrains first
            const short CAPACITY_MAX = 3000;    

            /// <summary>
            /// Represents the capacity of this cache
            /// </summary>
            private readonly int _capacity;

            /// <summary>
            /// Represents a Memory HashMap data-structure n(1) access
            /// </summary>
            private Dictionary<int, LinkedListNode<KeyValuePair<int, int>>> _dict;
            /// <summary>
            /// Represents a double linked list that contains the key, and the reference to the linked node
            /// </summary>
            private LinkedList<KeyValuePair<int, int>> _linkedList;



            public LRUCache(int capacity)
            {
                //Do validations for capacity constrain
                if (capacity >= CAPACITY_MAX)
                    throw new ArgumentException($"{nameof(capacity)} cannot be greater or equals than 3000");

                if (capacity < 1)
                    throw new ArgumentException($"{nameof(capacity)} cannot be less or equals than 1");

                //Initalize the capacity
                _capacity = capacity;
                //Initalize data-structures
                _dict = new Dictionary<int, LinkedListNode<KeyValuePair<int, int>>>();
                _linkedList = new LinkedList<KeyValuePair<int, int>>();
            }

            public int Get(int key)
            {              

                if (_dict.ContainsKey(key))
                {
                    //Find the node by key, remove it from the list, and put it on the first node of the list
                    var node = _dict[key];

                    if (node != null)
                    {
                        var value = node.Value.Value;
                        _linkedList.Remove(node);
                        _linkedList.AddFirst(node);                    

                        return value;
                    }
                }

                //by default return -1, if node is not found
                //set last key used as -1                
                return -1;

            }

            public void Put(int key, int value)
            {
                //Check if the linked-list is over capacity
                if (!_dict.ContainsKey(key))
                {
                    if (_linkedList.Count == _capacity)
                    {

                        //get the last index used and remove it from the linked list
                        var lastUsedKey = _linkedList.Last.Value.Key;
                        //remove last used node from linked list based on key
                        _linkedList.RemoveLast();
                        //remove the last used key from dictionary
                        _dict.Remove(lastUsedKey);                     

                        //then add the new value to the front
                        LinkedListNode<KeyValuePair<int, int>> node = CreateNode(key, value);
                        _dict.Add(key, node);
                    }
                    else
                    {
                        //then add the new value to the front
                        LinkedListNode<KeyValuePair<int, int>> node = CreateNode(key, value);
                        _dict.Add(key, node);
                    }
                }
                else
                {
                    UpdateNode(key, value);
                }            

            }

            /// <summary>
            /// Create a linked-list node with a key-value pair reference
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            private LinkedListNode<KeyValuePair<int, int>> CreateNode(int key, int value)
            {
                KeyValuePair<int, int> kvp = new KeyValuePair<int, int>(key, value);
                var node = new LinkedListNode<KeyValuePair<int, int>>(kvp);
                _linkedList.AddFirst(node);     
                return node;
            }

            /// <summary>
            /// Updates an exisiting node, leaving the key, and updating the value.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            private void UpdateNode(int key, int value)
            {
                var node = _dict[key];
                node.Value = new KeyValuePair<int, int>(key, value);                
                _linkedList.Remove(node);
                _linkedList.AddFirst(node);              
            }
        }
       
        /**
         * Your LRUCache object will be instantiated and called as such:
         * LRUCache obj = new LRUCache(capacity);
         * int param_1 = obj.Get(key);
         * obj.Put(key,value);
         */

        public static void Main(string[] args)
        {
            try
            {
                LRUCache lRUCache = new LRUCache(2);
                Assert.AreEqual(-1, lRUCache.Get(2));
                lRUCache.Put(2, 6);
                Assert.AreEqual(-1, lRUCache.Get(1));
                lRUCache.Put(1, 5);
                lRUCache.Put(1, 2);
                Assert.AreEqual(2, lRUCache.Get(1));
                Assert.AreEqual(6, lRUCache.Get(2));

                Console.WriteLine("¡All test passed!");
            }
            catch(InvalidOperationException inv)
            {
                Console.WriteLine(inv.Message, inv.StackTrace);
            }
            catch (AssertFailedException asf)
            {
                Console.WriteLine($"LRUCache didn't return the rigth value {asf.Message}");
            }
        }
    }
}
