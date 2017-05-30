using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using EliteMMO.API;
using System.Windows.Media;
using System;
using System.ComponentModel;
using System.Threading;
using System.Runtime.InteropServices;
using WinningMarbles;
using System.Collections.Generic;

namespace FFXI_MarbleChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private static BackgroundWorker backgroundWorker;


        public int characterSelected = 0;
        public int firstSelect = 0;

        public bool automaticCheck = false;
        public ListBox processids = new ListBox();


        public static EliteAPI api;

        public MainWindow()
        {
            InitializeComponent();














            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            if (File.Exists("eliteapi.dll") && File.Exists("elitemmo.api.dll"))
            {
                automaticCheck = true;
                var pol = Process.GetProcessesByName("pol");

                if (pol.Length < 1)
                {
                    MessageBox.Show("FFXI not found");
                }
                else
                {

                    for (var i = 0; i < pol.Length; i++)
                    {
                        this.POLID.Items.Add(pol[i].MainWindowTitle);
                        this.processids.Items.Add(pol[i].Id);
                    }
                    this.POLID.SelectedIndex = 0;
                    this.processids.SelectedIndex = 0;

                }
            } 

            if (automaticCheck == false)
            {
                automaticCheck_enabled.IsEnabled = false;
            }


                XDocument xdoc = XDocument.Load("data/WinningNumbers.xml");
            var lv1s = from lv1 in xdoc.Descendants("WinningNumbers")
                       select new
                       {
                           Children = lv1.Descendants("rank")
                       };

            //Loop through results
            foreach (var lv1 in lv1s)
            {
                foreach (var lv2 in lv1.Children)
                {
                    if (lv2.Attribute("name").Value == "1")
                    {
                        this.rank1.Text = lv2.Value;
                    } else if (lv2.Attribute("name").Value == "2")
                    {
                        this.rank2.Text = lv2.Value;
                    }
                    else if (lv2.Attribute("name").Value == "3")
                    {
                        this.rank3.Text = lv2.Value;
                    }
                    else if (lv2.Attribute("name").Value == "4")
                    {
                        this.rank4.Text = lv2.Value;
                    }
                    else if (lv2.Attribute("name").Value == "5")
                    {
                        this.rank5.Text = lv2.Value;
                    }
                }
            }








        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BeginCheck_Click(object sender, RoutedEventArgs e)
        {
            bool isWindowOpen = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is winningMarbles)
                {
                    isWindowOpen = true;
                    w.Activate();
                }
            }
            if (!isWindowOpen)
            {
                winningMarbles subWindow = new winningMarbles();
                subWindow.Show();
            }
    }

        private void AddMarble_Click(object sender, RoutedEventArgs e)
        {
            // characterName
            // MarbleNumber
            XDocument xdoc = XDocument.Load("data/CharacterData.xml");

            int foundCharacter = 0;

            // First check you don't have more than 10 marbles on this character
            var lv1s = from lv1 in xdoc.Descendants("character")
                       select new
                       {
                           Header = lv1.Attribute("name").Value,
                           Children = lv1.Descendants("marble")
                       };
            foreach (var lv1 in lv1s)
            {
                if (lv1.Header == characterName.Text)
                {
                    foundCharacter = 1;

                    if (lv1.Children.Count() >= 10)
                    {
                        MessageBox.Show("This characters count is at "+lv1.Children.Count()+"/10 and can not hold anymore. Cancelling!");
                    }
                    else
                    {
                        // Reached a line that doesn't seem to exist so create it.
                        xdoc.Element("Characters").Elements("character")
                            .First(c => (string)c.Attribute("name") == characterName.Text).Add
                                     (
                                         new XElement
                                             (
                                                 "marble", MarbleNumber.Text
                                             )
                                      );
                        xdoc.Save("data/CharacterData.xml");
                        MessageBox.Show("Added marble: " + MarbleNumber.Text + " to character: " + characterName.Text);
                    }
                }
            }

            if (foundCharacter != 1)
            {

                xdoc.Element("Characters").Add(new XElement("character", new XAttribute("name", characterName.Text)));
                xdoc.Save("data/CharacterData.xml");

                // Reached a line that doesn't seem to exist so create it.
                xdoc.Element("Characters").Elements("character")
                    .First(c => (string)c.Attribute("name") == characterName.Text).Add
                             (
                                 new XElement
                                     (
                                         "marble", MarbleNumber.Text
                                     )
                              );
                xdoc.Save("data/CharacterData.xml");
                MessageBox.Show("Added marble: " + MarbleNumber.Text + " to character: " + characterName.Text);


            }


        }


        private void EditMarbles_Click(object sender, RoutedEventArgs e)
        {
            bool isWindowOpen = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is marbleViewer)
                {
                    isWindowOpen = true;
                    w.Activate();
                }
            }
            if (!isWindowOpen)
            {
                marbleViewer subWindow = new marbleViewer();
                subWindow.Show();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // LOAD XML FILE AND PROCESS WINNING NUMBERS
            //Load xml
            XDocument xdoc = XDocument.Load("data/WinningNumbers.xml");
            var lv1s = from lv1 in xdoc.Descendants("WinningNumbers")
                       select new
                       {
                           Children = lv1.Descendants("rank")
                       };
            foreach (var lv1 in lv1s)
            {
                foreach (var lv2 in lv1.Children)
                {
                    if (lv2.Attribute("name").Value == "1")
                    {
                        this.rank1.Text = lv2.Value;
                    }
                    else if (lv2.Attribute("name").Value == "2")
                    {
                        this.rank2.Text = lv2.Value;
                    }
                    else if (lv2.Attribute("name").Value == "3")
                    {
                        this.rank3.Text = lv2.Value;
                    }
                    else if (lv2.Attribute("name").Value == "4")
                    {
                        this.rank4.Text = lv2.Value;
                    }
                    else if (lv2.Attribute("name").Value == "5")
                    {
                        this.rank5.Text = lv2.Value;
                    }
                }
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void SelectPOLIDButton_Click(object sender, RoutedEventArgs e)
        {
            this.processids.SelectedIndex = this.POLID.SelectedIndex;
            api = new EliteAPI((int)this.processids.SelectedItem);
            this.SelectPOLID.Content = "SELECTED";
            this.SelectPOLID.Background = Brushes.LightGreen;

            EliteAPI.ChatEntry cl = api.Chat.GetNextChatLine();
            while (cl != null) cl = api.Chat.GetNextChatLine();

            if (firstSelect == 0)
            {
                backgroundWorker.RunWorkerAsync();
            }





        }

        public string numberFound = "0";

        public void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {


            EliteAPI.ChatEntry cl = api.Chat.GetNextChatLine();

            while (cl != null)
            {

                //Trace.WriteLine(string.Format("type:{0} idx1:{1} idx2:{2} {3}", cl.ChatType, cl.Index1, cl.Index2, cl.Text));

                // First appearance will be the selected number.
                Match m = Regex.Match(cl.Text, @"Bonanza Moogle : (?<name>\d+).");
                if (m.Success)
                {
                    // Number found store this until confirmed.
                    numberFound = m.Groups["name"].Value;
                }
                // Confirmation verified, add to the XML
                if (cl.Text == "Bonanza Moogle : Thank you, and good luck, kupo!")
                {
                    Add_Marble(api.Player.Name, numberFound);
                }

                cl = api.Chat.GetNextChatLine();
            }
            Thread.Sleep(TimeSpan.FromSeconds(0.1));
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           // Thread.Sleep(500);
            backgroundWorker.RunWorkerAsync();
        }


        private void Add_Marble(string character_name, string marble_number)
        {
            XDocument xdoc = XDocument.Load("data/CharacterData.xml");
            int foundCharacter = 0;
            // First check you don't have more than 10 marbles on this character
            var lv1s = from lv1 in xdoc.Descendants("character")
                       select new
                       {
                           Header = lv1.Attribute("name").Value,
                           Children = lv1.Descendants("marble")
                       };
            foreach (var lv1 in lv1s)
            {
                if (lv1.Header == character_name)
                {
                    foundCharacter = 1;

                        // Reached a line that doesn't seem to exist so create it.
                        xdoc.Element("Characters").Elements("character")
                            .First(c => (string)c.Attribute("name") == character_name).Add
                                     (
                                         new XElement
                                             (
                                                 "marble", marble_number
                                             )
                                      );
                        xdoc.Save("data/CharacterData.xml");
                }
            }

            if (foundCharacter != 1)
            {

                xdoc.Element("Characters").Add(new XElement("character", new XAttribute("name", character_name)));
                xdoc.Save("data/CharacterData.xml");

                // Reached a line that doesn't seem to exist so create it.
                xdoc.Element("Characters").Elements("character")
                    .First(c => (string)c.Attribute("name") == character_name).Add
                             (
                                 new XElement
                                     (
                                         "marble", marble_number
                                     )
                              );
                xdoc.Save("data/CharacterData.xml");


            }

        }






    }
} 






 