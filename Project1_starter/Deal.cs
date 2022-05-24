using System;
using System.Collections.Generic;

/// <summary>
/// This file is used to create a deal object which holds a few
/// variables and is able to output its info with the ToString method
/// </summary>

namespace Project1_starter
{
    public class Deal
    {
        public Deal(string[] itemName, int[] itemAmount, double cost)
        {
            ItemNames = itemName;
            ItemAmounts = itemAmount;
            Cost = cost;
        }

        public static string[] ItemNames { get; set; }
        public int[] ItemAmounts { get; set; }
        public double Cost { get; private set; }
        public int[] OrderAmounts { get; set; }

        /// <summary>
        /// This method outputs the info it holds in a clean format
        /// </summary>
        /// <returns>Returns the cleaned up string</returns>
        public override string ToString()
        {
            List<string> list = new List<string>();

            for (int i = 0; i < ItemAmounts.Length; i++)
            {
                list.Add(ItemNames[i] + ": " + ItemAmounts[i] + ", ");
            }
            if (Cost != 0)
            {
                string currency = String.Format("{0:C}", Cost);
                list.Add(currency);
            }

            string text = string.Join("\n", list);
            return text;
        }
    }
}
