using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace Milestone1DB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Business
        {
            public string name { get; set; }
            public string address { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            addStates();
            addCollumns2Grid();
            categoriesListBox.SelectionMode = SelectionMode.Multiple;
        }

        public void addCollumns2Grid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Business name";
            col1.Binding = new Binding("name");
            col1.Width = 255;
            businessesGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Address";
            col2.Binding = new Binding("address");
            col2.Width = 210;
            businessesGrid.Columns.Add(col2);
        }

        private string connectionStringBuilder()
        {
            return "Host=localhost; Username=postgres; Password=password; Database=Milestone2DB";
        }
        public void addStates()
        {
            using (var conn = new NpgsqlConnection(connectionStringBuilder()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT state FROM yelp_business_entity ORDER BY state;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            stateList.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        private void stateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityListBox.Items.Clear();
            zipcodeListBox.Items.Clear();
            categoriesListBox.Items.Clear();
            businessesGrid.Items.Clear();
            using (var conn = new NpgsqlConnection(connectionStringBuilder()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT city FROM  yelp_business_entity WHERE state = \'" + stateList.SelectedItem.ToString() + "\' ORDER BY city;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cityListBox.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        private void cityListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cityListBox.Items.Count != 0)
            {
                zipcodeListBox.Items.Clear();
                categoriesListBox.Items.Clear();
                businessesGrid.Items.Clear();
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT DISTINCT postal_code FROM  yelp_business_entity WHERE state = \'" + stateList.SelectedItem.ToString() + "\' AND city = \'" + cityListBox.SelectedItem.ToString() + "\' ORDER BY postal_code;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                zipcodeListBox.Items.Add(reader.GetString(0));
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }

        private void zipcodeListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (zipcodeListBox.Items.Count != 0)
            {
                categoriesListBox.Items.Clear();
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT DISTINCT categories FROM categories_entity, (SELECT DISTINCT business_id FROM yelp_business_entity WHERE state = \'" + stateList.SelectedItem.ToString() + "\' AND city = \'" + cityListBox.SelectedItem.ToString() + "\' AND postal_code = \'" + zipcodeListBox.SelectedItem.ToString() + "\' ORDER BY business_id) as c WHERE categories_entity.business_id = c.business_id ORDER BY categories;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categoriesListBox.Items.Add(reader.GetString(0));
                            }
                        }
                    }
                    conn.Close();
                }
            }
            if (zipcodeListBox.Items.Count != 0)
            {
                businessesGrid.Items.Clear();
                businessesGrid.Items.Add(new Business() { name = "Chosen tag:" });
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT name,address FROM yelp_business_entity WHERE state = \'" + stateList.SelectedItem.ToString() + "\' AND city = \'" + cityListBox.SelectedItem.ToString() + "\' AND postal_code = \'" + zipcodeListBox.SelectedItem.ToString() + "\' ORDER BY name";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1)});
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }

        private void categoriesListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoriesListBox.Items.Count != 0)
            {
                businessesGrid.Items.Clear();
                var catList = "%" + string.Join("%", categoriesListBox.SelectedItems.Cast<string>().ToList()) + "%";
                businessesGrid.Items.Add(new Business() { name = "Chosen tag:", address = string.Join(",\n", categoriesListBox.SelectedItems.Cast<string>().ToList())});
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT name,address FROM yelp_business_entity, (SELECT business_id, string_agg(categories, ',') AS cs FROM categories_entity GROUP BY business_id) as g WHERE yelp_business_entity.business_id = g.business_id AND g.cs LIKE " + "\'" + catList + "\' AND state = \'" + stateList.SelectedItem.ToString() + "\' AND city = \'" + cityListBox.SelectedItem.ToString() + "\' AND postal_code = \'" + zipcodeListBox.SelectedItem.ToString() + "\'" + " ORDER BY name";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1) });
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }
    }
}
