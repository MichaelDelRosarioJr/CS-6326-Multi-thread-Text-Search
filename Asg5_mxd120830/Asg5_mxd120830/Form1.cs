/******************************************************************************
 * Assignment 5 - Multithreaded Text Search
 *
 * This program opens Dante's Inferno text file, if file path exits, and starts a
 * search thread to search for the string input by the user in the file.
 * The search is insensitive with a 1 millisecond delay for every line read.
 * The user can cancel search at any time.
 *
 * Written by Michael Del Rosario (mxd120830)
 * Assignment 5, starting November 8, 2017.
 ******************************************************************************/


using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Asg5_mxd120830
{
    public partial class MultiThreaded_Text_Search : Form
    {
        public MultiThreaded_Text_Search()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        /*
         * Browse button opens an additional window that allows user to insert file path automatically.
         */
        private void browseButton_Click(object sender, EventArgs e)
        {
            //Create a dialogue box to search file
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();

            //Write file path to file path text box
            if (result == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                fileNametextBox.Text = file;
            }
        }
        
        //Create a global thread that stores the search thread
        private Thread searchThread;
        private ThreadStart searchThreadStart;

        /*
         * SearchButton_Click starts the searchStringThread and cancels thread if thread is currently running
         */ 
        private void searchButton_Click(object sender, EventArgs e)
        {
            //Check to see what the button text is
            if (searchButton.Text.Contains("Search"))//If search has not started, start search thread and change button
            {
                //Initialize thread
                searchThreadStart = new ThreadStart(searchStringThread);
                searchThread = new Thread(searchThreadStart);

                //Clear searchListView for new Search
                searchListView.Items.Clear();

                //Start search Thread
                searchThread.Start();

                
                
            }
            if(searchButton.Text.Contains("Cancel"))//If search is in progress, cancel search thread
            {
                //Cancel search Thread
                searchThread.Abort();              
            }

            //Switch Button Text when search starts
            if(searchButton.Text.Contains("Search"))
                searchButton.Text = "Cancel";
        }

        /*
         * searchStringThread opens text file if file exists, or end thread otherwise.
         * Funtion reads file line by line, and print the line number and line everytime the string
         * from the searchTextBox is contained within the line. The search is insensitive and finds any
         * insance of the substring of searchTextBox. Thread records current state while running and end state before closing.
         */ 
        private void searchStringThread()
        {
           try
            {
                //Check to see if file exists
                string path = fileNametextBox.Text;
                
                //If file is not found set, end search
                if (!File.Exists(path))
                { 
                    statusStrip.Text = "Error! File does not exist or File path is incorrect. Search ended.";
                    
                }
                else//If file exits
                {
                    //Create file with filepath
                    FileInfo searchFile = new FileInfo(path);
                    statusStrip.Text = "File Found. Starting thread.";

                    //Store search string
                    string searchText = searchtextBox.Text;
                    int lineNum = 1; //Stores the current line number.

                    

                    using (StreamReader r = searchFile.OpenText())
                    {
                        
                        

                        string currentLine = "";//Stores the current line
                        StringComparison compare = StringComparison.OrdinalIgnoreCase;//For insensitive searching

                        //Read file line by line and print any line that contains searchText
                        while ((currentLine = r.ReadLine()) != null)
                        {
                            //Give current status information
                            statusStrip.Text = lineNum + "/" + (File.ReadAllLines(path).Length + 1) + " lines remaining. " + "Searching Text...     ";

                            //If the current line contains the search text, print line number and line on listView.
                            if (currentLine.IndexOf(searchText, compare)>=0)
                            {
                                //Create new item row for listView via string array
                                string[] row = { lineNum.ToString(), currentLine };
                                ListViewItem insrt = new ListViewItem(row);
                                
                                //Allow thread to add to listView
                                if (searchListView.InvokeRequired)
                                {
                                    searchListView.BeginInvoke(new MethodInvoker(() => searchListView.Items.Add(insrt)));
                                }
                                else
                                    searchListView.Items.Add(insrt);


                                //TEST: Write line to console
                                //Console.WriteLine(lineNum.ToString() + " " + t);
                            }

                            lineNum++;//Increment lineNum
                            Thread.Sleep(1);//Wait for a millisecond

                            //TEST: Put Thread to Extended Sleep to test Cancel Button
                            //Thread.Sleep(1000);
                        }
                    }

                    

                    //Inform user that the search has concluded
                    statusStrip.Text = "Search complete. Search ended";
                }

                

                
            }
            catch (ThreadAbortException e) //Tell User That Thread was cancelled
            {
                statusStrip.Text = "Search cancelled. Search ended";
            }
            finally
            {
                //Allow thread to change cancel button back to search
                if (searchButton.InvokeRequired)
                    searchListView.BeginInvoke(new MethodInvoker(() => searchButton.Text = "Search"));
                else
                    searchButton.Text = "Search";
            }
         }

        private void statusStrip_Click(object sender, EventArgs e)
        {

        }
    }
}


