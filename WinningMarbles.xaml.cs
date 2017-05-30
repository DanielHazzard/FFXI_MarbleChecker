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
using System.Windows.Threading;

namespace WinningMarbles
{
    public class Marble : List<Marble>
    {
        public string CharacterName { get; set; }
        public string MarbleNumber { get; set; }
        public int Prize { get; set; }
    }
}

namespace FFXI_MarbleChecker
{



    public partial class winningMarbles
    {
        public List<Marble> PrizesWon = new List<Marble>();
        MainWindow MW = (MainWindow)Application.Current.MainWindow;

        public winningMarbles()
        {
            InitializeComponent();

            XDocument xdoc = XDocument.Load("data/CharacterData.xml");

            var lv1s = from lv1 in xdoc.Descendants("character")
                       select new
                       {
                           Header = lv1.Attribute("name").Value,
                           Children = lv1.Descendants("marble")
                       };
            foreach (var lv1 in lv1s)
            {

                // lv1.Header - NAME

                foreach (var lv2 in lv1.Children)
                {
                    // lv2.Value - NUMBER
                    string marbleNumber_gen = "0";

                    // Check that the marble number matches 5 characters, if not add 0 in the front until it does.
                    int total = lv2.Value.Length;

                    // Add the required zeroes
                    if (total != 5)
                    {
                        if (total == 4)
                            marbleNumber_gen = "0" + lv2.Value;
                        else if (total == 3)
                            marbleNumber_gen = "00" + lv2.Value;
                        else if (total == 2)
                            marbleNumber_gen = "000" + lv2.Value;
                        else if (total == 1)
                            marbleNumber_gen = "0000" + lv2.Value;
                    }
                    else { marbleNumber_gen = lv2.Value; }

                    // Item retrieved or generated so check against known winners.
                    if (marbleNumber_gen == MW.rank1.Text)
                        PrizesWon.Add(new Marble { CharacterName = lv1.Header, MarbleNumber = marbleNumber_gen, Prize = 1 });
                    else if (marbleNumber_gen.Substring(marbleNumber_gen.Length - 4) == MW.rank2.Text)
                        PrizesWon.Add(new Marble { CharacterName = lv1.Header, MarbleNumber = marbleNumber_gen, Prize = 2 });
                    else if (marbleNumber_gen.Substring(marbleNumber_gen.Length - 3) == MW.rank3.Text)
                        PrizesWon.Add(new Marble { CharacterName = lv1.Header, MarbleNumber = marbleNumber_gen, Prize = 3 });
                    else if (marbleNumber_gen.Substring(marbleNumber_gen.Length - 2) == MW.rank4.Text)
                        PrizesWon.Add(new Marble { CharacterName = lv1.Header, MarbleNumber = marbleNumber_gen, Prize = 4 });
                    else if (marbleNumber_gen.Substring(marbleNumber_gen.Length - 1) == MW.rank5.Text)
                        PrizesWon.Add(new Marble { CharacterName = lv1.Header, MarbleNumber = marbleNumber_gen, Prize = 5 });
                }
            }

            // FIRST GENERATE FIRST PLACE PRIZE MARBLES
            var Rank1_Marbles = PrizesWon.Where(x => x.Prize == 1);
            if (Rank1_Marbles.Count() != 0)
            {
                winningMarbleNumbers.Items.Add("Congratulations, Rank 1 prizes won on the following Character/s.\n");
                foreach (var data in Rank1_Marbles)
                {
                    winningMarbleNumbers.Items.Add("Character Name: " + data.CharacterName + "\n" + "Marble Number: " + data.MarbleNumber + "\n");
                }
            }
            else
                winningMarbleNumbers.Items.Add("Sorry, No rank 1 prizes won.\n\n");
            // NOW GENERATE SECOND PLACE PRIZE WINNERS
            var Rank2_Marbles = PrizesWon.Where(x => x.Prize == 2);
            if (Rank2_Marbles.Count() != 0)
            {
                winningMarbleNumbers.Items.Add("Congratulations, Rank 2 prizes won on the following Character/s.\n");
                foreach (var data in Rank2_Marbles)
                {
                    winningMarbleNumbers.Items.Add("Character Name: " + data.CharacterName + "\n" + "Marble Number: " + data.MarbleNumber + "\n");
                }

            }
            else
                winningMarbleNumbers.Items.Add("Sorry, No rank 2 prizes won.\n\n");
            // NOW GENERATE THIRD PLACE PRIZE WINNERS
            var Rank3_Marbles = PrizesWon.Where(x => x.Prize == 3);
            if (Rank3_Marbles.Count() != 0)
            {
                winningMarbleNumbers.Items.Add("Congratulations, Rank 3 prizes won on the following Character/s.\n");
                foreach (var data in Rank3_Marbles)
                {
                    winningMarbleNumbers.Items.Add("Character Name: "+data.CharacterName + "\n"+"Marble Number: " + data.MarbleNumber + "\n");
                }
            }
            else
                winningMarbleNumbers.Items.Add("Sorry, No rank 3 prizes won.\n\n");
            // NOW GENERATE FORTH PLACE PRIZE WINNERS
            var Rank4_Marbles = PrizesWon.Where(x => x.Prize == 4);
            if (Rank4_Marbles.Count() != 0)
            {
                winningMarbleNumbers.Items.Add("Congratulations, Rank 4 prizes won on the following Character/s.\n");
                foreach (var data in Rank4_Marbles)
                {
                    winningMarbleNumbers.Items.Add("Character Name: " + data.CharacterName + "\n" + "Marble Number: " + data.MarbleNumber + "\n");
                }
            }
            else
                winningMarbleNumbers.Items.Add("Sorry, No rank 4 prizes won.\n\n");
            // NOW GENERATE FIFTH PLACE PRIZE WINNERS
            var Rank5_Marbles = PrizesWon.Where(x => x.Prize == 5);
                if (Rank5_Marbles.Count() != 0)
                {
                winningMarbleNumbers.Items.Add("Congratulations, Rank 5 prizes won on the following Character/s.\n");

                foreach (var data in Rank5_Marbles)
                    {
                    winningMarbleNumbers.Items.Add("Character Name: " + data.CharacterName + "\n" + "Marble Number: " + data.MarbleNumber + "\n");
                }
            }
            else
                winningMarbleNumbers.Items.Add("Sorry, No rank 5 prizes won.");
        }
    }
}
