using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

namespace FFXI_MarbleChecker
{

    public class CheckedListItem<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isChecked;
        private T item;

        public CheckedListItem()
        { }

        public CheckedListItem(T item, bool isChecked = false)
        {
            this.item = item;
            this.isChecked = isChecked;
        }

        public T Item
        {
            get { return item; }
            set
            {
                item = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item"));
            }
        }


        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }
    }

/// <summary>
/// Interaction logic for marbleViewer.xaml
/// </summary>
public partial class marbleViewer 
    {
        public marbleViewer()
        {
            InitializeComponent();

            // LOAD XML FILE AND PROCESS FOUND MARBLES
            XDocument xdoc = XDocument.Load("data/CharacterData.xml");
            var lv1s = from lv1 in xdoc.Descendants("character")
                       select new
                       {
                           Header = lv1.Attribute("name").Value,
                           Children = lv1.Descendants("marble")
                       };
            foreach (var lv1 in lv1s)
            {
                CheckBox checkBox = new CheckBox()
                {
                    Content = lv1.Header,
                    Name = lv1.Header + "_DeleteAll"
                };
                enabledMarbles.Items.Add(checkBox);
               
                foreach (var lv2 in lv1.Children)
                {
                    enabledMarbles.Items.Add("          " + lv2.Value);
                }
            }

        }

        private void DeleteMarbles_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you wish to delete these Marbles?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                var checkbox = FindVisualChildren<CheckBox>(enabledMarbles).ToList();
                XDocument xdoc = XDocument.Load("data/CharacterData.xml");
                foreach (var c in checkbox)
                {
                    bool isChecked = c.IsChecked ?? false;
                    if (isChecked == true)
                    {
                        var unwanted = from ee in xdoc.Elements("Characters").Elements("character")
                                       where ee.Attribute("name").Value.Equals(c.Content)
                                       select ee;
                        unwanted.Remove();
                        xdoc.Save("data/CharacterData.xml");
                    }
                    MessageBox.Show("Deleted.");
                    Close();
                }
            }
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }
                else
                {
                    var childOfChild = FindVisualChildren<T>(child);
                    if (childOfChild != null)
                    {
                        foreach (var subchild in childOfChild)
                        {
                            yield return subchild;
                        }
                    }
                }
            }
        }
    }
}
