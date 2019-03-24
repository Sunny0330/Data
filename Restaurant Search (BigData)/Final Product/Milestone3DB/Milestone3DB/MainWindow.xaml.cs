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
using System.Device.Location;
using System.IO;

namespace Milestone3DB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Friends
        {
            public string name { get; set; }
            public string average_stars { get; set; }
            public string yelping_since { get; set; }
            public string user_id { get; set; }
        }

        public class postByFriends
        {
            public string friendName { get; set; }
            public string businessName { get; set; }
            public string city { get; set; }
            public string stars { get; set; }
            public string date { get; set; }
            public string text { get; set; }
            public string useful { get; set; }
            public string funny { get; set; }
            public string cool { get; set; }

        }

        private double currentUserLattitude = -999999;
        private double currentUserLongitude = -999999;
        private int currentCmd = 0;
        private string curCmd = "";
        private string beforeFilter = "";
        private string orderBy = "ORDER BY name";
        private Dictionary<string, Double> bidWithDistance = new Dictionary<string, Double>();


        public MainWindow()
        {
            InitializeComponent();
            addCollumns2userFriendsDataGrid();
            initInputUserSearch();
            addCollumns2latestReviewMadeByFriendsDataGrid();
            addStates();
            addCollumns2BusinessGrid();
            categoriesListBox.SelectionMode = SelectionMode.Multiple;
            performUpdates();
            addTriggers();
            addDayOfWeek();
            addTimes();
            addSortingOptions();
            addDistanceCollumn();
        }

        public class Business
        {
            public string name { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string distance { get; set; }
            public string stars { get; set; }
            public string numberOfReview { get; set; }
            public string avg_rating { get; set; }
            public string totalNumberCheckin { get; set; }
        }

        private string connectionStringBuilder()
        {
            return "Host=localhost; Username=postgres; Password=smiledlstjs2; Database=Milestone2DB";
        }

        /* Part A */
        private void addCollumns2userFriendsDataGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Name";
            col1.Binding = new Binding("name");
            col1.Width = 70;
            userFriendsDatagrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Avg Stars";
            col2.Binding = new Binding("average_stars");
            col2.Width = 60;
            userFriendsDatagrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Yelping Since";
            col3.Binding = new Binding("yelping_since");
            col3.Width = 100;
            userFriendsDatagrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "User_id";
            col4.Binding = new Binding("user_id");
            col4.Width = 100;
            userFriendsDatagrid.Columns.Add(col4);
        }

        private void addCollumns2latestReviewMadeByFriendsDataGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "User name";
            col1.Binding = new Binding("friendName");
            col1.Width = 100;
            latestReviewMadeByFriendsDataGrid.Columns.Add(col1);
     
            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Business";
            col2.Binding = new Binding("businessName");
            col2.Width = 100;
            latestReviewMadeByFriendsDataGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "city";
            col3.Binding = new Binding("city");
            col3.Width = 100;
            latestReviewMadeByFriendsDataGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Stars";
            col4.Binding = new Binding("stars");
            col4.Width = 50;
            latestReviewMadeByFriendsDataGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "Date";
            col5.Binding = new Binding("date");
            col5.Width = 100;
            latestReviewMadeByFriendsDataGrid.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Header = "text";
            col6.Binding = new Binding("text");
            col6.Width = 300;
            latestReviewMadeByFriendsDataGrid.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "funny";
            col7.Binding = new Binding("funny");
            col7.Width = 50;
            latestReviewMadeByFriendsDataGrid.Columns.Add(col7);

            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Header = "useful";
            col8.Binding = new Binding("useful");
            col8.Width = 50;
            latestReviewMadeByFriendsDataGrid.Columns.Add(col8);

            DataGridTextColumn col9 = new DataGridTextColumn();
            col9.Header = "cool";
            col9.Binding = new Binding("cool");
            col9.Width = 50;
            latestReviewMadeByFriendsDataGrid.Columns.Add(col9);
        }

        private void initInputUserSearch()
        {
            using (var conn = new NpgsqlConnection(connectionStringBuilder()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT user_id FROM yelp_user_entity ORDER BY user_id;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uidOfInputedUser.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        private void inputUserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            uidOfInputedUser.Items.Clear();
            userFriendsDatagrid.Items.Clear();
            latestReviewMadeByFriendsDataGrid.Items.Clear();

            using (var conn = new NpgsqlConnection(connectionStringBuilder()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    if (inputUserSearch.Text == "")
                    {
                        cmd.CommandText = "SELECT DISTINCT user_id FROM yelp_user_entity;";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT DISTINCT user_id FROM yelp_user_entity WHERE UPPER(yelp_user_entity.name) = \'" + inputUserSearch.Text.ToUpper() + "\';";
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uidOfInputedUser.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        private void userUIDSelected(object sender, SelectionChangedEventArgs e)
        {
            populateFieldsAndGrid();
        }

        private void populateFieldsAndGrid()
        {
            if (uidOfInputedUser.SelectedItem != null)
            {
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        string value = uidOfInputedUser.SelectedValue.ToString();
                        userFriendsDatagrid.Items.Clear();
                        latestReviewMadeByFriendsDataGrid.Items.Clear();
                        cmd.CommandText = "SELECT name FROM yelp_user_entity WHERE user_id = \'" + value + "\';";

                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            userName.Text = reader.GetString(0);
                        }
                        cmd.CommandText = "SELECT average_stars FROM yelp_user_entity WHERE user_id = \'" + value + "\';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            userStars.Text = reader.GetString(0);
                        }
                        cmd.CommandText = "SELECT fans FROM yelp_user_entity WHERE user_id = \'" + value + "\';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            userFans.Text = reader.GetString(0);
                        }
                        cmd.CommandText = "SELECT yelping_since FROM yelp_user_entity WHERE user_id = \'" + value + "\';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            userYelpingSince.Text = reader.GetDate(0).ToString();
                        }
                        cmd.CommandText = "SELECT funny FROM yelp_user_entity WHERE user_id = \'" + value + "\';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            userFunny.Text = reader.GetString(0);
                        }
                        cmd.CommandText = "SELECT cool FROM yelp_user_entity WHERE user_id = \'" + value + "\';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            userCool.Text = reader.GetString(0);
                        }
                        cmd.CommandText = "SELECT useful FROM yelp_user_entity WHERE user_id = \'" + value + "\';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            userUseful.Text = reader.GetString(0);
                        }

                        cmd.CommandText = "SELECT DISTINCT user_id, name, average_stars, yelping_since FROM yelp_user_entity WHERE user_id IN  (SELECT user_id_two FROM isfriend_relationship WHERE user_id_one = \'" + value + "\');";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userFriendsDatagrid.Items.Add(new Friends() { name = reader.GetString(1), average_stars = reader.GetString(2), yelping_since = reader.GetDate(3).ToString(), user_id = reader.GetString(0).ToString() });
                            }
                        }

                        cmd.CommandText = @"SELECT u.name as username, uJoin.name as business, uJoin.city, uJoin.review_stars, uJoin.publish_date, uJoin.text, uJoin.useful, uJoin.funny, uJoin.cool FROM yelp_user_entity u 
	                                            INNER JOIN (SELECT * FROM yelp_business_entity b 
    	                                            INNER JOIN (SELECT r.user_id, r.business_id, r.text, r.stars as review_stars, r.date as publish_date, r.useful, r.funny, r.cool from review_entity r 
        	                                            INNER JOIN (SELECT user_id, max(date) as latestPost FROM review_entity 
                        	                                            WHERE user_id 
                        	                                            IN  (SELECT user_id_two FROM isfriend_relationship WHERE user_id_one = '" + value + @"') 
                        	                                            GROUP BY user_id 
                        	                                            ORDER by user_id) maxD
    		                                            on r.user_id = maxD.user_id AND maxD.latestPost = r.date ORDER BY r.user_id) bJoin
    	                                            on b.business_id = bJoin.business_id) uJoin 
                                                on uJoin.user_id = u.user_id;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                latestReviewMadeByFriendsDataGrid.Items.Add(new postByFriends() { friendName = reader.GetString(0), businessName = reader.GetString(1), city = reader.GetString(2), stars = reader.GetString(3), date = reader.GetDate(4).ToString(), text = reader.GetString(5), useful = reader.GetString(6), funny = reader.GetString(7), cool = reader.GetString(8) });
                            }
                        }



                    }
                    conn.Close();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (uidOfInputedUser.SelectedItem != null && userFriendsDatagrid.SelectedItem != null)
            {
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "DELETE FROM isfriend_relationship WHERE user_id_one = \'" + uidOfInputedUser.SelectedItem.ToString() + "\' AND user_id_two = \'" + ((Friends)userFriendsDatagrid.SelectedItem).user_id + "\';";
                        cmd.ExecuteReader();
                    }
                    conn.Close();
                }

                populateFieldsAndGrid();
            }
        }

        /* Part B */
        public void addCollumns2BusinessGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Business name";
            col1.Binding = new Binding("name");
            col1.Width = 100;
            businessesGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Address";
            col2.Binding = new Binding("address");
            col2.Width = 120;
            businessesGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "City";
            col3.Binding = new Binding("city");
            col3.Width = 70;
            businessesGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "State";
            col4.Binding = new Binding("state");
            col4.Width = 70;
            businessesGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "Distances";
            col5.Binding = new Binding("distance");
            col5.Width = 70;
            businessesGrid.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Header = "Stars";
            col6.Binding = new Binding("stars");
            col6.Width = 70;
            businessesGrid.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "# of Reviews";
            col7.Binding = new Binding("numberOfReview");
            col7.Width = 70;
            businessesGrid.Columns.Add(col7);

            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Header = "Rating";
            col8.Binding = new Binding("avg_rating");
            col8.Width = 70;
            businessesGrid.Columns.Add(col8);

            DataGridTextColumn col9 = new DataGridTextColumn();
            col9.Header = "# Checkins";
            col9.Binding = new Binding("totalNumberCheckin");
            col9.Width = 70;
            businessesGrid.Columns.Add(col9);
        }

        private void SetLocationIsClicked(object sender, RoutedEventArgs e)
        {
            if (Lattitude.Text != "" && Longitude.Text != "")
            {
                setLocationError.Visibility = Visibility.Hidden;
                currentUserLattitude = Double.Parse(Lattitude.Text);
                currentUserLongitude = Double.Parse(Longitude.Text);
                businessesGrid.Items.Clear();
                categoriesListBox.Items.Clear();
                zipcodeListBox.Items.Clear();
                cityListBox.Items.Clear();
                stateList.Items.Clear();
                addStates();

                sortingOptions.IsEnabled = false;


            }
            else
            {
                setLocationError.Content = "Error: Missing Field(s)!";
                setLocationError.Visibility = Visibility.Visible;
            }
            
        }

        private void addStates()
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
                        while (reader.Read())
                        {
                            stateList.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        private void addSortingOptions()
        {
            sortingOptions.Items.Add("Business name (default)");
            sortingOptions.Items.Add("Highest Rating");
            sortingOptions.Items.Add("Most reviewed");
            sortingOptions.Items.Add("Best review rating");
            sortingOptions.Items.Add("Most-Checkins");
            sortingOptions.Items.Add("Nearest");
            sortingOptions.SelectedIndex = 0;
            sortingOptions.IsEnabled = false;
        }

        private void addDistanceCollumn()
        {
            using (var conn = new NpgsqlConnection(connectionStringBuilder()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "ALTER TABLE yelp_business_entity DROP COLUMN IF EXISTS distance ";
                    cmd.ExecuteReader();
                }
                conn.Close();
            }
            using (var conn = new NpgsqlConnection(connectionStringBuilder()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "ALTER TABLE yelp_business_entity ADD COLUMN distance FLOAT DEFAULT 0";
                    cmd.ExecuteReader();
                }
                conn.Close();
            }
        }

        private void performUpdates()
        {

            using (var conn = new NpgsqlConnection(connectionStringBuilder()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = File.ReadAllText(@"../../ByteMe_UPDATE.sql").Replace("'", "\'");
                    cmd.ExecuteReader();
                }
                conn.Close();
            }
        }

        private void addTriggers()
        {
            using (var conn = new NpgsqlConnection(connectionStringBuilder()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = File.ReadAllText(@"../../ByteMe_TRIGGER.sql").Replace("'","\'");
                    cmd.ExecuteReader();
                }
                conn.Close();
            }
        }

        private void stateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityListBox.Items.Clear();
            zipcodeListBox.Items.Clear();
            categoriesListBox.Items.Clear();
            if (stateList.Items.Count != 0)
            {
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
        }

        private void cityListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cityListBox.Items.Count != 0)
            {
                zipcodeListBox.Items.Clear();
                categoriesListBox.Items.Clear();
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

        private void zipcodeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        }


        private void searchBusinessButtonIsClicked(object sender, RoutedEventArgs e)
        {
            businessesGrid.Items.Clear();
            dayOfWeekListbox.Items.Clear();
            addDayOfWeek();
            dayOfWeekListbox.IsEnabled = true;
            beforeFilter = "";
            sortingOptions.IsEnabled = true;
            if (categoriesListBox.Items.Count != 0 && categoriesListBox.SelectedItems.Count != 0)
            {
                var catList = "%" + string.Join("%", categoriesListBox.SelectedItems.Cast<string>().ToList()) + "%";
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT name,address,city,state,latitude,longitude, stars, review_count, reviewrating, numcheckins, yelp_business_entity.business_id b FROM yelp_business_entity, (SELECT business_id, string_agg(categories, ',') AS cs FROM categories_entity GROUP BY business_id) as g WHERE yelp_business_entity.business_id = g.business_id AND g.cs LIKE " + "\'" + catList + "\' AND state = \'" + stateList.SelectedItem.ToString() + "\' AND city = \'" + cityListBox.SelectedItem.ToString() + "\' AND postal_code = \'" + zipcodeListBox.SelectedItem.ToString() + "\'" + " " + orderBy;
                        curCmd = "SELECT name,address,city,state,latitude,longitude, stars, review_count, reviewrating, numcheckins, yelp_business_entity.business_id b FROM yelp_business_entity, (SELECT business_id, string_agg(categories, ',') AS cs FROM categories_entity GROUP BY business_id) as g WHERE yelp_business_entity.business_id = g.business_id AND g.cs LIKE " + "\'" + catList + "\' AND state = \'" + stateList.SelectedItem.ToString() + "\' AND city = \'" + cityListBox.SelectedItem.ToString() + "\' AND postal_code = \'" + zipcodeListBox.SelectedItem.ToString() + "\'" + " " ;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (currentUserLattitude != -999999 && currentUserLattitude != -999999)
                                {
                                    Double dist = Math.Round((new GeoCoordinate(currentUserLattitude, currentUserLongitude).GetDistanceTo(new GeoCoordinate(Double.Parse(reader.GetFloat(4).ToString()), Double.Parse(reader.GetFloat(5).ToString()))) / 1609.344), 2);
                                    businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = dist.ToString(), stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString(), totalNumberCheckin = reader.GetInt16(9).ToString() });
                                    bidWithDistance.Add(reader.GetString(10), dist);
                                }
                                else
                                {
                                    businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = "0", stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString("#.##"), totalNumberCheckin = reader.GetInt16(9).ToString() });
                                }
                            }
                            if (bidWithDistance.Count != 0)
                            {
                                updateDistance();
                            }
                        }
                    }
                    conn.Close();
                }
            }
            else if (zipcodeListBox.Items.Count != 0 && zipcodeListBox.SelectedIndex != -1)
            {
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT name,address,city,state,latitude,longitude, stars, review_count, reviewrating, numcheckins, yelp_business_entity.business_id b FROM yelp_business_entity WHERE state = \'" + stateList.SelectedItem.ToString() + "\' AND city = \'" + cityListBox.SelectedItem.ToString() + "\' AND postal_code = \'" + zipcodeListBox.SelectedItem.ToString() + "\' " + orderBy;
                        curCmd = "SELECT name,address,city,state,latitude,longitude, stars, review_count, reviewrating, numcheckins, yelp_business_entity.business_id b FROM yelp_business_entity WHERE state = \'" + stateList.SelectedItem.ToString() + "\' AND city = \'" + cityListBox.SelectedItem.ToString() + "\' AND postal_code = \'" + zipcodeListBox.SelectedItem.ToString() + "\' ";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (currentUserLattitude != -999999 && currentUserLattitude != -999999)
                                {
                                    Double dist = Math.Round((new GeoCoordinate(currentUserLattitude, currentUserLongitude).GetDistanceTo(new GeoCoordinate(Double.Parse(reader.GetFloat(4).ToString()), Double.Parse(reader.GetFloat(5).ToString()))) / 1609.344), 2);
                                    businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = dist.ToString(), stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString(), totalNumberCheckin = reader.GetInt16(9).ToString() });
                                    bidWithDistance.Add(reader.GetString(10), dist);
                                }
                                else
                                {
                                    businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = "0", stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString("#.##"), totalNumberCheckin = reader.GetInt16(9).ToString() });
                                }
                            }
                            if (bidWithDistance.Count != 0)
                            {
                                updateDistance();
                            }
                        }
                    }
                    conn.Close();
                }
            }
            else if (cityListBox.Items.Count != 0 && cityListBox.SelectedIndex != -1)
            {
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT name,address,city,state,latitude,longitude, stars, review_count, reviewrating, numcheckins, yelp_business_entity.business_id b FROM yelp_business_entity WHERE state  = \'" + stateList.SelectedItem.ToString() + "\' AND city = \'" + cityListBox.SelectedItem.ToString() + "\' " + orderBy;
                        curCmd = "SELECT name,address,city,state,latitude,longitude, stars, review_count, reviewrating, numcheckins, yelp_business_entity.business_id b FROM yelp_business_entity WHERE state  = \'" + stateList.SelectedItem.ToString() + "\' AND city = \'" + cityListBox.SelectedItem.ToString() + "\' ";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (currentUserLattitude != -999999 && currentUserLattitude != -999999)
                                {
                                    Double dist = Math.Round((new GeoCoordinate(currentUserLattitude, currentUserLongitude).GetDistanceTo(new GeoCoordinate(Double.Parse(reader.GetFloat(4).ToString()), Double.Parse(reader.GetFloat(5).ToString()))) / 1609.344), 2);
                                    businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = dist.ToString(), stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString(), totalNumberCheckin = reader.GetInt16(9).ToString() });
                                    bidWithDistance.Add(reader.GetString(10), dist);
                                }
                                else
                                {
                                    businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = "0", stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString("#.##"), totalNumberCheckin = reader.GetInt16(9).ToString() });
                                }
                            }
                            if (bidWithDistance.Count != 0)
                            {
                                updateDistance();
                            }
                        }
                    }
                    conn.Close();
                }
            }
            else if(stateList.Items.Count != 0 && stateList.SelectedIndex != -1)
            {
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT name,address,city,state,latitude,longitude, stars, review_count, reviewrating, numcheckins, yelp_business_entity.business_id b FROM yelp_business_entity WHERE state = \'" + stateList.SelectedItem.ToString() + "\' " + orderBy;
                        curCmd = "SELECT name,address,city,state,latitude,longitude, stars, review_count, reviewrating, numcheckins, yelp_business_entity.business_id b FROM yelp_business_entity WHERE state = \'" + stateList.SelectedItem.ToString() + "\' ";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (currentUserLattitude != -999999 && currentUserLattitude != -999999)
                                {
                                    Double dist = Math.Round((new GeoCoordinate(currentUserLattitude, currentUserLongitude).GetDistanceTo(new GeoCoordinate(Double.Parse(reader.GetFloat(4).ToString()), Double.Parse(reader.GetFloat(5).ToString()))) / 1609.344), 2);
                                    businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = dist.ToString(), stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString(), totalNumberCheckin = reader.GetInt16(9).ToString() });
                                    bidWithDistance.Add(reader.GetString(10),dist);
                                }
                                else
                                {
                                    businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = "0", stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString("#.##"), totalNumberCheckin = reader.GetInt16(9).ToString() });
                                }
                            }
                            if (bidWithDistance.Count != 0)
                            {
                                updateDistance();
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }

        private void updateDistance()
        {
            foreach(KeyValuePair < string, Double > entry in bidWithDistance)
            {
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UPDATE yelp_business_entity SET distance = " + entry.Value + " WHERE yelp_business_entity.business_id = '" + entry.Key + "'";
                        cmd.ExecuteReader();
                      
                    }
                    conn.Close();
                }
            }
        }

        private void addDayOfWeek()
        {
            dayOfWeekListbox.Items.Add("Monday");
            dayOfWeekListbox.Items.Add("Tuesday");
            dayOfWeekListbox.Items.Add("Wednesday");
            dayOfWeekListbox.Items.Add("Thursday");
            dayOfWeekListbox.Items.Add("Friday");
            dayOfWeekListbox.Items.Add("Saturday");
            dayOfWeekListbox.Items.Add("Sunday");
            dayOfWeekListbox.IsEnabled = false;
        }
        private void addTimes()
        {
            fromTimeListBox.IsEnabled = false;
            toTimeListBox.IsEnabled = false;
            for(int c = 0; c < 24; c++)
            {
                fromTimeListBox.Items.Add(c.ToString() + ":00");
            }
        }

        private void dayOfWeekListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dayOfWeekListbox.SelectedIndex != -1)
            {
                fromTimeListBox.IsEnabled = true;
            }
            else
            {
                fromTimeListBox.IsEnabled = false;
                toTimeListBox.IsEnabled = false;
                fromTimeListBox.Items.Clear();
                toTimeListBox.Items.Clear();
                addTimes();
            }

        }

        private void filterTime()
        {
            using (var conn = new NpgsqlConnection(connectionStringBuilder()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    string joinClause = "SELECT yelp_business_entity.business_id bu FROM yelp_business_entity, (SELECT business_id, the_date, TO_TIMESTAMP(fromTime.value,'HH24:MI')::TIME f, TO_TIMESTAMP(toTime.value,'HH24:MI')::TIME t from hour_entity, regexp_split_to_table(hour_entity.the_time,'-') WITH ORDINALITY fromTime(value, r), regexp_split_to_table(hour_entity.the_time,'-') WITH ORDINALITY toTime(value, r) WHERE fromTime.r = 1 AND toTime.r = 2) as openHour WHERE yelp_business_entity.business_id = openHour.business_id AND openHour.the_date = '" + dayOfWeekListbox.SelectedItem.ToString() + "' AND ((TO_TIMESTAMP('" + fromTimeListBox.SelectedItem.ToString() + "', 'HH24:MI')::TIME >= openHour.f AND TO_TIMESTAMP('" + toTimeListBox.SelectedItem.ToString() + "','HH24:MI')::TIME <= openHour.t) OR (openHour.f = openHour.t)) And is_open = true";
                    cmd.CommandText = "SELECT name,address,city,state,latitude,longitude, stars, review_count, reviewrating, numcheckins, b FROM " + "(" + curCmd + orderBy + ") leftT " + "INNER JOIN " + "(" + joinClause + ") rightT " + "ON leftT.b = rightT.bu " + " " + orderBy;
                    curCmd = "SELECT name,address,city,state,latitude,longitude, stars, review_count, reviewrating, numcheckins, b FROM " + "(" + curCmd + ") leftT " + "INNER JOIN " + "(" + joinClause + ") rightT " + "ON leftT.b = rightT.bu ";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (currentUserLattitude != -999999 && currentUserLattitude != -999999)
                            {
                                businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = (new GeoCoordinate(currentUserLattitude, currentUserLongitude).GetDistanceTo(new GeoCoordinate(Double.Parse(reader.GetFloat(4).ToString()), Double.Parse(reader.GetFloat(5).ToString()))) / 1609.344).ToString("#.##"), stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString(), totalNumberCheckin = reader.GetInt16(9).ToString() });
                            }
                            else
                            {
                                businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = "0", stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString("#.##"), totalNumberCheckin = reader.GetInt16(9).ToString() });
                            }
                        }
                    }
                }
                conn.Close();
            }
        }

        private void fromTimeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            toTimeListBox.IsEnabled = true;
            toTimeListBox.Items.Clear();
            for (int c = fromTimeListBox.SelectedIndex + 1; c < 24; c++)
            {
                toTimeListBox.Items.Add(c.ToString() + ":00");
            }
        }

        private void toTimeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (toTimeListBox.SelectedIndex != -1)
            {
                businessesGrid.Items.Clear();
                if (beforeFilter != "")
                {
                    curCmd = beforeFilter;
                }
                filterTime();
                if (beforeFilter == "")
                {
                    beforeFilter = curCmd;
                }
            }
        }

        private void categoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            chosenCategories.Items.Clear();
            for (int i = 0; i < (categoriesListBox.SelectedItems.Cast<string>().ToList()).Count; i ++)
            {
                chosenCategories.Items.Add(categoriesListBox.SelectedItems.Cast<string>().ToList()[i]);
            }

                
        }

        private void sortingOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sortingOptions.SelectedIndex == 0)
            {
                orderBy = "ORDER BY name";
            }
            else if (sortingOptions.SelectedIndex == 1)
            {
                orderBy = "ORDER BY stars DESC";
            }
            else if (sortingOptions.SelectedIndex == 2)
            {
                orderBy = "ORDER BY review_count DESC";
            }
            else if (sortingOptions.SelectedIndex == 3)
            {
                orderBy = "ORDER BY reviewrating DESC";
            }
            else if (sortingOptions.SelectedIndex == 4)
            {
                orderBy = "ORDER BY numcheckins DESC";
            }
            else if (sortingOptions.SelectedIndex == 5)
            {
                orderBy = "ORDER BY distance";
            }
            if (curCmd != "")
            {
                businessesGrid.Items.Clear();
                using (var conn = new NpgsqlConnection(connectionStringBuilder()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = curCmd + orderBy;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (currentUserLattitude != -999999 && currentUserLattitude != -999999)
                                {
                                    businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = (new GeoCoordinate(currentUserLattitude, currentUserLongitude).GetDistanceTo(new GeoCoordinate(Double.Parse(reader.GetFloat(4).ToString()), Double.Parse(reader.GetFloat(5).ToString()))) / 1609.344).ToString("#.##"), stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString(), totalNumberCheckin = reader.GetInt16(9).ToString() });
                                }
                                else
                                {
                                    businessesGrid.Items.Add(new Business() { name = reader.GetString(0), address = reader.GetString(1), city = reader.GetString(2), state = reader.GetString(3), distance = "0", stars = reader.GetFloat(6).ToString(), numberOfReview = reader.GetInt32(7).ToString(), avg_rating = reader.GetDouble(8).ToString("#.##"), totalNumberCheckin = reader.GetInt16(9).ToString() });
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }
    }
}
