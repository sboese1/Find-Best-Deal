using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// This file holds all of the event handling of the GUI. One of the events is the recursive
/// search with backtracking. Basically, this does all of the backend involved in the
/// project.
/// </summary>

namespace Project1_starter
{
    public partial class UserInterface : Form
    {
        int NumItems;
        List<Deal> _deals;
        List<List<Deal>> minDeal = new List<List<Deal>>();
        int[] order_amounts;

        public UserInterface()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is called everytime the menu strip item (file) is called.
        /// When this happens, an OpenFileDialog is created which allows the user to
        /// select a file. Using that file, the deals are displayed in the upper text box
        /// using some loops.
        /// </summary>
        /// <param name="sender">Dealing with handling the event</param>
        /// <param name="e">Dealing with handling the event</param>
        private void openMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            StreamReader reader = new StreamReader(openFileDialog.FileName);
            string line = "";
            int count = 0;
            List<string> pieces = new List<string>();

            textBox1.Text = "";
            secondTextBox.Text = "";

            while (line != null)
            {
                if (count == 0)
                {
                    string temp = reader.ReadLine();
                    pieces = temp.Split(' ').ToList();

                    NumItems = pieces.Count();
                    _deals = new List<Deal>(NumItems);

                    if (pieces.Count < 5)
                    {
                        if (pieces.Count == 4)
                        {
                            label5.Visible = false;
                            textBox6.Visible = false;
                        }
                        else if (pieces.Count == 3)
                        {
                            label4.Visible = false;
                            textBox5.Visible = false;
                            label5.Visible = false;
                            textBox6.Visible = false;
                        }
                        else if (pieces.Count == 2)
                        {
                            label3.Visible = false;
                            textBox4.Visible = false;
                            label4.Visible = false;
                            textBox5.Visible = false;
                            label5.Visible = false;
                            textBox6.Visible = false;
                        }
                    }
                    count++;
                }
                else if (count == 1)
                {
                    count++;
                }
                else
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        double cost = 0;
                        string[] temp = line.Split(' ');
                        string[] temp2 = new string[temp.Length-1];
                        for (int i = 0; i < temp.Length; i++)
                        {
                            if (temp[i] != null)
                            {
                                if (i == temp.Length - 1)
                                {
                                    try
                                    {
                                        cost = double.Parse(temp[i]);
                                    }
                                    catch(Exception ex) { }
                                }
                                else
                                {
                                    temp2[i] = temp[i];
                                }
                            }
                        }
                        int[] quantity = new int[temp2.Length];

                        try
                        {
                            quantity = Array.ConvertAll(temp2, s => int.Parse(s));
                        }
                        catch(Exception ex) { }

                        Deal deal = new Deal(pieces.ToArray(), quantity, cost);
                        if (deal.ToString() != null && deal.ToString() != "")
                        {
                            _deals.Add(deal);
                        }
                        order_amounts = new int[deal.ItemAmounts.Length];

                        if (deal.ToString() != null && deal.ToString() != "")
                        {
                            textBox1.AppendText(deal.ToString());
                            textBox1.AppendText(Environment.NewLine);
                        }

                        label1.Text = pieces.ToArray()[0];
                        label2.Text = pieces.ToArray()[1];
                        label3.Text = pieces.ToArray()[2];
                        try { label4.Text = pieces.ToArray()[3]; }
                        catch (Exception ex) { }
                        try { label5.Text = pieces.ToArray()[4]; }
                        catch (Exception ex) { }
                    }
                }
            }
            reader.Close();

            button1.Enabled = true;
        }

        /// <summary>
        /// This is the recursive method which finds the best deal. We start
        /// with a base case which returns if itemPos is equal to the length of orderAmounts.
        /// Recursion, with the help of backtracking, is then used to go through all of the possible combinations.
        /// The best combination of deals is then returned.
        /// </summary>
        /// <param name="curOrder">The current list of deals we have</param>
        /// <param name="itemPos">The current position to be (possibly) fulfilled</param>
        /// <param name="orderAmounts">The current order amounts of the desired items</param>
        /// <returns>A list of the best possible combination of deals</returns>
        public List<Deal> FindBestDeal(List<Deal> curOrder, int itemPos, int[] orderAmounts)
        {
            while (itemPos < orderAmounts.Length)
            {
                if (orderAmounts[itemPos] > 0) // If the amount wanted to order is greater is than 0
                {
                    break;
                }
                itemPos++;
            }

            // Base case
            if (itemPos == orderAmounts.Length) // If it's at the end of the loop
            {
                return new List<Deal>(curOrder); // Return the order
            }

            List<Deal> bestDeal = null;
            double minCost = double.MaxValue;

            // Recursive case
            for (int i = 0; i < _deals.Count; i++)
            {
                if (_deals[i].ItemAmounts[itemPos] > 0) // If the deal at position i has at least one of the desired item
                {
                    for (int j = 0; j < orderAmounts.Length; j++)
                    {
                        orderAmounts[j] -= _deals[i].ItemAmounts[j]; // Subtracting the items in the deal from the remaining desired items
                    }
                    curOrder.Add(_deals[i]); // Add the deal that was just applied to the order

                    List<Deal> deal = FindBestDeal(curOrder, itemPos, orderAmounts); // Recursive call

                    double cost = 0;

                    if (deal != null) // If deal is not null
                    {
                        for (int j = 0; j < deal.Count; j++)
                        {
                            cost += deal[j].Cost; // Gets the total cost of all the deals combined
                        }
                    }
                    else
                    {
                        cost = double.MaxValue; // Sets cost to the max value possible
                    }

                    if (cost < minCost && deal != null) // If the current cost of the deals is less than minCost and deal is not null
                    {
                        minCost = cost; // minCost is set to cost
                        bestDeal = deal; // bestDeal is set to deal
                    }

                    for (int j = 0; j < orderAmounts.Length; j++)
                    {
                        orderAmounts[j] += _deals[i].ItemAmounts[j]; // Sets the order amounts back the original amount
                    }
                    curOrder.RemoveAt(curOrder.Count - 1); // Remove the last item in the list
                }
            }

            return bestDeal;
        }

        /// <summary>
        /// This method handles the button click event. It first calls the recursive method,
        /// then tallies its cost. It is then outputted to the bottom text box.
        /// </summary>
        /// <param name="sender">Dealing with handling the event</param>
        /// <param name="e">Dealing with handling the event</param>
        private void button1_Click(object sender, EventArgs e)
        {
            secondTextBox.Text = "";
            setOrderAmounts();

            int itemPos = 0;
            List<Deal> order = new List<Deal>();

            List<Deal> d = FindBestDeal(order, itemPos, order_amounts);

            double cost = 0;
            if (d != null)
            {
                for (int i = 0; i < d.Count; i++)
                {
                    cost += d[i].Cost;
                }
            }

            if (d == null)
            {
                secondTextBox.AppendText("No such order is possible");
            }
            else {
                string currency = String.Format("{0:C}", cost);
                secondTextBox.AppendText("Best order, with cost of " + currency + ":");
                secondTextBox.AppendText(Environment.NewLine);
                for (int j = 0; j < d.Count; j++)
                {
                    secondTextBox.AppendText(d[j].ToString());
                    secondTextBox.AppendText(Environment.NewLine);
                }
            }
        }

        /// <summary>
        /// This method deals with making sure all of the order amounts are set to
        /// the correct amount.
        /// </summary>
        public void setOrderAmounts()
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "0";
            }
            string itemOne = textBox2.Text;
            order_amounts[0] = Convert.ToInt32(itemOne);
            if (textBox3.Text == "")
            {
                textBox3.Text = "0";
            }
            string itemTwo = textBox3.Text;
            order_amounts[1] = Convert.ToInt32(itemTwo);
            try
            {
                if (textBox4.Text == "")
                {
                    textBox4.Text = "0";
                }
                string itemThree = textBox4.Text;
                order_amounts[2] = Convert.ToInt32(itemThree);
            }
            catch (Exception ex) { }
            try
            {
                if (textBox5.Text == "")
                {
                    textBox5.Text = "0";
                }
                string itemFour = textBox5.Text;
                order_amounts[3] = Convert.ToInt32(itemFour);
            }
            catch (Exception ex) { }
            try
            {
                if (textBox6.Text == "")
                {
                    textBox6.Text = "0";
                }
                string itemFive = textBox5.Text;
                order_amounts[4] = Convert.ToInt32(itemFive);
            }
            catch (Exception ex) { }
        }

        private void UserInterface_Load(object sender, EventArgs e)
        {

        }
    }
}
