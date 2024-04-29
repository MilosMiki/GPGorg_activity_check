using Microsoft.VisualBasic.ApplicationServices;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

namespace GPGorg_activity_check
{
    public partial class Form1 : Form
    {
        public const int NUM_POSTS_PAGE = 20;

        private bool GPGSL_StartFound;
        //private bool GPGSL_EndFound;
        private int page;
        private List<User>? users;
        private List<User>? usersNotPosted;
        private List<Post>? posts;
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_Closing;
        }

        public static DateTime startDate = default(DateTime);
        private void Form1_Load(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = "dd/MM/yyyy HH:mm";
            dateTimePicker2.CustomFormat = "dd/MM/yyyy HH:mm";
            //GPGSL_EndFound = false;
            GPGSL_StartFound = false;
            //Find when previous boost post was read
            try
            {
                TextReader tr = new StreamReader("boost.txt");
                page = Convert.ToInt32(tr.ReadLine());
                tr.Close();
            }
            catch
            {
                listBox3.Items.Add(new UserMessage("No boost.txt, setting to default"));
                page = 1;
            }
            numericUpDown2.Value = page;
            numericUpDown1.Value = page;

            //first load of page # - required
            if (this.page > 1)
            {
                checkBox1.Checked = false;
            }
            else { checkBox1.Checked = true; }

            button1_Click(sender, e, true);
            /*
            bool b = true;
            if (users != null)
            {
                foreach (User u in users)
                {
                    if (u.Warnings > 0)
                    {
                        b = false;
                        break;
                    }
                }
            }
            checkBox1.Checked = b;*/
            posts = new List<Post>();

            //Check if saved data exists
            try
            {
                TextReader tr = new StreamReader("SaveData\\data.xml");
                //Load saved data, if exists
                if (Convert.ToBoolean(tr.ReadLine()))
                {
                    numericUpDown1.Value = Convert.ToDecimal(tr.ReadLine());
                    checkBox1.Checked = Convert.ToBoolean(tr.ReadLine());
                    this.posts = Loadd<Post>("SaveData\\posts.xml");
                    List<DateTime> l = Loadd<DateTime>("SaveData\\date.xml");
                    dateTimePicker1.Value = l.FirstOrDefault();
                    refreshPosts();
                }
                tr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Save<T>(string fileName, List<T> list)
        {
            // Gain code access to the file that we are going
            // to write to
            try
            {
                // Create a FileStream that will write data to file.
                using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, list);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static List<T> Loadd<T>(string fileName)
        {
            var list = new List<T>();
            // Check if we had previously Save information of our friends
            // previously
            if (File.Exists(fileName))
            {

                try
                {
                    // Create a FileStream will gain read access to the
                    // data file.
                    using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        var formatter = new BinaryFormatter();
                        list = (List<T>)
                            formatter.Deserialize(stream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return list;
        }

        private void backupTextFile()
        {
            listBox3.Items.Add(new UserMessage("Executing backup"));
            if (this.users == null)
            {
                listBox3.Items.Add(new UserMessage("Failed: Null reference in users"));
                return;
            }
            TextWriter tw = new StreamWriter("users_backup.txt");
            foreach (User u in this.users)
            {
                tw.WriteLine(u.Username);
                tw.WriteLine(u.Warnings);
            }
            tw.Close();
        }

        private void refreshUsersNotPosted()
        {
            listBox3.Items.Add(new UserMessage("Refreshing users not posted list"));
            if (this.usersNotPosted == null)
            {
                listBox3.Items.Add(new UserMessage("Failed: Null reference in usersNotPosted"));
                return;
            }
            listBox2.Items.Clear();
            foreach (User u in usersNotPosted)
            {
                listBox2.Items.Add(u);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e, false);
        }

        //Find all users away/on holiday, place them in map
        private List<KeyValuePair<string, DateTime>> userAway()
        {
            List<KeyValuePair<string, DateTime>> sol = new List<KeyValuePair<string, DateTime>>();
            TextReader tr = new StreamReader("away.txt");
            string? s;
            int line = 0;
            string username = "";
            while ((s = tr.ReadLine()) != null)
            {
                if (s == "" || s.Substring(0, 2) == "//") continue;
                if (line == 1) //ReadLine is on DateTime
                {
                    int day = Convert.ToInt32(s.Substring(0, 2));
                    int mon = Convert.ToInt32(s.Substring(3, 2));
                    int y = Convert.ToInt32(s.Substring(6, 4));
                    DateTime d = new DateTime(y, mon, day, 23, 59, 59);
                    sol.Add(new KeyValuePair<string, DateTime>(username, d));
                    line = 0;
                }
                else //ReadLine is on username
                {
                    username = s; //needs to be inputed without typos
                    line++;
                }
            }
            tr.Close();
            return sol;
        }

        //Loads all users from text file
        //Set up for field "users"
        private void button1_Click(object sender, EventArgs e, bool firstTime, bool backup = true)
        {
            List<KeyValuePair<string, DateTime>> usersAway = userAway();
            TextReader tr = new StreamReader("users.txt");
            List<User> uss = new List<User>();
            string? s;
            while ((s = tr.ReadLine()) != null)
            {
                var v = usersAway.FirstOrDefault(x => x.Key == s);
                DateTime val = v.Value;
                uss.Add(new User(s, Convert.ToInt32(tr.ReadLine()), val));
            }
            this.users = new List<User>(uss);
            usersNotPosted = new List<User>(uss);
            refreshUsersNotPosted();
            if (!firstTime)
            {
                refreshPosts(true);
            }
            tr.Close();
            if (backup)
            {
                backupTextFile();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3_Click(sender, e, false);
        }

        //Update this.posts field
        private void button3_Click(object sender, EventArgs e, bool autoCall = false)
        {
            if (this.posts == null)
            {
                listBox3.Items.Add(new UserMessage("Failed: Null reference in posts"));
                return;
            }
            int cnt = this.posts.Count - (this.posts.Count % NUM_POSTS_PAGE);
            //listBox3.Items.Add((int)cnt);
            //this way numbering always begins from a multiple of NUM_POSTS_PAGE

            if (textBox1.Text.Length > 0)
                listBox3.Items.Add(new UserMessage("Reading HTML"));
            string s = textBox1.Text;
            while (s.Length > 0)
            {
                //Find a post (message)
                int i1 = s.IndexOf("<div class=\"message\">");
                s = s.Substring(i1 + 1);
                //Find the next post
                int i2 = s.IndexOf("<div class=\"message\">");
                //Extract the previous post
                if (i1 < 0) break;
                string post = "";
                if (i2 < 0)
                {
                    post = s.Substring(0, s.Length - 1);
                }
                else
                {
                    post = s.Substring(0, i2);
                }


                //Find the date
                int date1 = post.IndexOf("Date:");
                post = post.Substring(date1 + 6);
                int date2 = post.IndexOf("<br>");
                string ddate = post.Substring(0, date2);
                post = post.Substring(date2 + 1);

                //Find the user
                int user1 = post.IndexOf("Posted by:");
                post = post.Substring(user1 + 1);
                int user2 = post.IndexOf("www.grandprixgames.org/profile.php");
                post = post.Substring(user2 + 1);
                int user3 = post.IndexOf(">");
                post = post.Substring(user3);
                int user4 = post.IndexOf("</a>");
                string user = post.Substring(1, user4 - 1);

                //Message body
                int body1 = post.IndexOf("<div class=\"message-body\">");
                post = post.Substring(body1 + 26);
                int body2 = post.IndexOf("<div class=\"message-options\">");
                if (body2 < 0) continue;
                string body = post.Substring(0, body2);

                //Add to posts
                Post lastPost = this.posts.Last();
                int thisPostId = (this.posts.Count + 1 - cnt);
                int lastPostId = lastPost.getPost();

                Post p = new Post("#" + page + "/" + thisPostId, user, ddate, body);
                if (lastPostId != NUM_POSTS_PAGE)
                {
                    if (lastPost.Date >= p.Date) continue;
                }
                //listBox3.Items.Add("LP:" + lastPostId + "; TP: " + thisPostId);
                this.posts.Add(p);

            }
            //Found all posts on page, tasks after
            if (textBox1.Text.Length > 0)
            {
                listBox3.Items.Add(new UserMessage("Found " + (this.posts.Count - cnt) + " posts on this page"));
                if (autoCall && this.posts.Last().getPost() == NUM_POSTS_PAGE)
                {
                    page++;
                    //autoCall = false;
                    numericUpDown1.Value = page;
                }
                refreshPosts();
            }
            else
            {
                listBox3.Items.Add(new UserMessage("Finished"));
            }
            //Save this.posts
            checkBox2_CheckedChanged(sender, e);
        }

        //Update the listBox
        private void refreshPosts(bool firstTime = false)
        {
            listBox3.Items.Add(new UserMessage("Refreshing posts list"));
            if (this.posts == null)
            {
                listBox3.Items.Add(new UserMessage("Failed: Null reference in posts"));
                return;
            }
            if (this.usersNotPosted == null)
            {
                listBox3.Items.Add(new UserMessage("Failed: Null reference in usersNotPosted"));
                return;
            }
            listBox1.Items.Clear();
            if (firstTime)
            {
                listBox4.Items.Clear();
            }
            //GPGSL_EndFound = false;
            GPGSL_StartFound = checkBox1.Checked;
            listBox3.Items.Add(new UserMessage("Refreshing users posted list"));
            foreach (Post p in this.posts)
            {
                //If I want to overwrite posts in the selected page
                /*
                int psPage = p.getPage();
                if (psPage >= this.page) continue;*/

                listBox1.Items.Add(p);
                //listBox3.Items.Add(new UserMessage(checkBox1.Checked.ToString()));

                //Action if post is found before deadline
                if (!GPGSL_StartFound)
                {
                    if (dateTimePicker1.Value > p.Date)
                    {
                        try
                        {
                            if (page - 1 == Convert.ToInt32(p.Id.Substring(1, p.Id.IndexOf("/") - 1)))
                                listBox3.Items.Add(new UserMessage("Found post before deadline: " + p.Id));
                        }
                        catch
                        {
                            listBox3.Items.Add(new UserMessage("Invalid post ID"));
                        }
                        continue;
                    }
                }
                //Action if post is found after deadline
                if (p.Date > dateTimePicker2.Value)
                {
                    checkBox2.Checked = false;
                    try
                    {
                        if (page - 1 == Convert.ToInt32(p.Id.Substring(1, p.Id.IndexOf("/") - 1)))
                            listBox3.Items.Add(new UserMessage("Found post after deadline: " + p.Id));
                    }
                    catch
                    {
                        listBox3.Items.Add(new UserMessage("Invalid post ID"));
                    }
                    continue;
                }
                //Saves the numericUpDown "boost post", to make it easier to look up the beginning of the deadline
                if (p.User == "GPGSL" && p.Body.IndexOf("Boost Announcement", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    listBox3.Items.Add(new UserMessage("Found boost post: " + p.Id));
                    numericUpDown2.Value = this.page - 1;

                    bool setEndDate = true;
                    for(int i = listBox1.Items.Count + 1; i < posts.Count; i++)
                    {
                        Post curPost = posts[i];
                        if (p.User == "GPGSL" && p.Body.IndexOf("Activity check", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            //trying to see if we found an activity check post AFTER the boost post.
                            listBox3.Items.Add(new UserMessage("Found activity post: " + curPost.Id));
                            setEndDate = false;
                            break;
                        }
                    }

                    //if we found no new GPGSL posts, I can attempt to read the end date from the boost announcement post
                    if (setEndDate)
                    {
                        //try to find the year first as it will usually be either in the current or next year
                        int tryYear = this.posts.Last().Date.Year; //no need to check for null as posts HAS to be filled to be here
                        int cntYear = 0;
                        int yearId = -1;
                        while(cntYear < 2 && yearId == -1)
                        {
                            yearId = p.Body.IndexOf(tryYear.ToString());
                            cntYear++;
                            tryYear++;
                        }
                        if(yearId > 0)
                        {
                            tryYear--;
                            //listBox3.Items.Add(tryYear);
                            //now try to find the month
                            int roundId = p.Body.IndexOf("Round", StringComparison.OrdinalIgnoreCase);
                            if(roundId >= 0)
                            {
                                string looking = p.Body.Substring(roundId, yearId - roundId - 1);
                                string[] bitsOfString = looking.Split(' ');
                                int mo = 0;
                                switch (bitsOfString.Last().ToLower())
                                {
                                    case "january":
                                        mo = 1;
                                        break;
                                    case "february":
                                        mo = 2;
                                        break;
                                    case "march":
                                        mo = 3;
                                        break;
                                    case "april":
                                        mo = 4;
                                        break;
                                    case "may":
                                        mo = 5;
                                        break;
                                    case "june":
                                        mo = 6;
                                        break;
                                    case "july":
                                        mo = 7;
                                        break;
                                    case "august":
                                        mo = 8;
                                        break;
                                    case "september":
                                        mo = 9;
                                        break;
                                    case "october":
                                        mo = 10;
                                        break;
                                    case "november":
                                        mo = 11;
                                        break;
                                    case "december":
                                        mo = 12;
                                        break;
                                }
                                if(mo > 0) //if we found a month
                                {
                                    //listBox3.Items.Add(mo);
                                    //now extract the date
                                    int ll = bitsOfString.Length;
                                    looking = bitsOfString[ll - 2];
                                    int dateIndex = looking.IndexOf("th");
                                    if (dateIndex < 0) dateIndex = looking.IndexOf("st");
                                    if (dateIndex < 0) dateIndex = looking.IndexOf("nd");
                                    if (dateIndex < 0) dateIndex = looking.IndexOf("rd");

                                    if(dateIndex > 0)
                                    {
                                        int dd = Convert.ToInt32(looking.Substring(0, dateIndex));
                                        //listBox3.Items.Add(dd);
                                        //now let's find the time
                                        int gmtIndex = -1;
                                        for(int j = ll-1; j >= 0; j--)
                                        {
                                            if (bitsOfString[j].IndexOf("GMT", StringComparison.OrdinalIgnoreCase) >= 0)
                                            {
                                                gmtIndex = j-1;
                                                break;
                                            }
                                        }
                                        if(gmtIndex >= 0)
                                        {
                                            looking = bitsOfString[gmtIndex];
                                            int timeIndex = looking.IndexOf("am");
                                            bool add12hr = false;
                                            bool is24hr = false;
                                            if (timeIndex < 0)
                                            {
                                                timeIndex = looking.IndexOf("pm");
                                                if (timeIndex < 0)
                                                {
                                                    is24hr = true;
                                                    timeIndex = looking.Length + 1;
                                                }
                                                else add12hr = true;
                                            }
                                            int time = Convert.ToInt32(looking.Substring(0,dateIndex - 1)) + (add12hr?12:0);
                                            if (!is24hr)
                                            {
                                                if(time % 12 == 0)
                                                {
                                                    time = (time + 12) % 24;
                                                }
                                            }
                                            //now we found everything to be able to build a date
                                            dateTimePicker2.Value = new DateTime(tryYear, mo, dd, time, 0, 0);
                                        }
                                        else
                                        {
                                            listBox3.Items.Add(new UserMessage("Time not found"));
                                        }
                                    }
                                    else
                                    {
                                        listBox3.Items.Add(new UserMessage("Date not found"));
                                    }
                                }
                                else
                                {
                                    listBox3.Items.Add(new UserMessage("Month not found"));
                                }
                            }
                        }
                        else
                        {
                            listBox3.Items.Add(new UserMessage("Year not found"));
                        }
                    }
                }
                /*
                if (!GPGSL_StartFound)
                {
                    if (p.User != "GPGSL") continue;
                    if (p.Body.IndexOf("Boost Announcement") < 0) continue;
                    GPGSL_StartFound = true;
                    listBox3.Items.Add(new UserMessage("Found starting boost post: " + p.Id));
                }
                else if (p.User == "GPGSL" && p.Body.IndexOf("Boost Announcement") >= 0)
                {
                    GPGSL_EndFound = true;
                    listBox3.Items.Add(new UserMessage("Found ending boost post: " + p.Id));
                }
                if (GPGSL_EndFound)
                {
                    continue;
                }*/
                //Remove user from NotPosted list, if found
                foreach (User u in this.usersNotPosted)
                {
                    if (u.Username == p.User)
                    {
                        listBox4.Items.Add(p.User + " - " + p.Id);
                        this.usersNotPosted.Remove(u);
                        break;
                    }
                }
            }
            //Change last updated time
            try
            {
                label11.Text = this.posts.Last().Date.ToString();
            }
            catch (InvalidOperationException) //occurs when clicking "refresh users not posted" without any posts
            {
                label11.Text = "refreshed";
                listBox3.Items.Add(new UserMessage("thrown InvalidOperationException"));
            }
            //Change last updated page
            try
            {
                int ps = this.posts.Last().getPage();
                label12.Text = "(last updated page #" + ps + ")";
            }
            catch (InvalidOperationException) //occurs when clicking "refresh users not posted" without any posts
            {
                label12.Text = "refreshed";
                listBox3.Items.Add(new UserMessage("thrown InvalidOperationException"));
            }

            refreshUsersNotPosted();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Tutorial tutorial = new Tutorial();
            tutorial.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button3_Click(sender, e, true);
            textBox1.Text = string.Empty;
        }

        //refresh POSTS
        private void button5_Click(object sender, EventArgs e)
        {
            if (this.posts == null)
            {
                listBox3.Items.Add(new UserMessage("Failed: Null reference in posts"));
                return;
            }
            listBox1.Items.Clear();
            foreach (Post p in this.posts)
            {
                //if (p.getPage() >= this.page) continue;

                listBox1.Items.Add(p);
            }
        }

        //Save new warnings to file
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.users == null)
            {
                listBox3.Items.Add(new UserMessage("Failed: Null reference in users"));
                return;
            }
            if (this.usersNotPosted == null)
            {
                listBox3.Items.Add(new UserMessage("Failed: Null reference in usersNotPosted"));
                return;
            }
            TextWriter tw = new StreamWriter("users.txt");
            foreach (User u in this.users)
            {
                tw.WriteLine(u.Username);
                bool found = false;
                foreach (User u2 in this.usersNotPosted)
                {
                    if (u2.Username == u.Username)
                    {
                        found = true;
                        break;
                    }
                }
                if (found && u.Away < dateTimePicker2.Value)
                {
                    tw.WriteLine(u.Warnings + 1);
                }
                else
                {
                    tw.WriteLine(u.Warnings);
                }
            }
            tw.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            GPGSL_StartFound = checkBox1.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.page = (int)numericUpDown1.Value;
            //refreshPosts();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            listBox3.Items.Add(new UserMessage("Updating boost.txt"));
            TextWriter tw = new StreamWriter("boost.txt");
            tw.WriteLine(numericUpDown2.Value);
            tw.Close();
        }

        //write to SaveData/data.xml
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            TextWriter tw = new StreamWriter("SaveData\\data.xml");
            if (checkBox2.Checked)
            {
                if (this.posts == null)
                {
                    listBox3.Items.Add(new UserMessage("Failed: Null reference in posts"));
                    return;
                }
                Save<Post>("SaveData\\posts.xml", this.posts);
                List<DateTime> l = new List<DateTime>();
                l.Add(dateTimePicker1.Value);
                Save<DateTime>("SaveData\\date.xml", l);
                tw.WriteLine(true);
                tw.WriteLine(numericUpDown1.Value);
                tw.WriteLine(checkBox1.Checked);
                tw.Close();
            }
            else
            {
                tw.WriteLine(false);
                tw.Close();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            startDate = dateTimePicker1.Value;
            button1_Click(sender, e, false, false);
        }

        //execute when the current form is closed
        private void Form1_Closing(object? sender, FormClosingEventArgs e)
        {
            //save everything in SaveData
            checkBox2_CheckedChanged(sender, e);
        }

        //start post manually
        private void button6_Click(object sender, EventArgs e)
        {
            changeDateManually(sender, e, dateTimePicker1);
        }

        //end post manually
        private void button7_Click(object sender, EventArgs e)
        {
            changeDateManually(sender, e, dateTimePicker2);
        }

        private void changeDateManually(object sender, EventArgs e, DateTimePicker dp)
        {
            if (posts == null)
            {
                listBox3.Items.Add(new UserMessage("Failed: No posts in List<Post> posts"));
                return;
            }
            //Post p;
            Form2 f2 = new Form2(posts);
            f2.ShowDialog();

            //execute when Form 2 is closed
            Post p = f2.P;
            if (p == null)
            {
                listBox3.Items.Add(new UserMessage("Failed: Post is null"));
                return;
            }
            if (!p.Id.Equals(""))
            {
                dp.Value = p.Date;
                button1_Click(sender, e, false, false);
                listBox3.Items.Add(new UserMessage("Finished"));
            }
            else
            {
                listBox3.Items.Add(new UserMessage("Failed: Error loading date from post"));
                return;
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            //this can be optimized to not double-check posts, but because it does it very fast I don't think it's necessary
            button1_Click(sender, e, false, false);
        }
    }
}