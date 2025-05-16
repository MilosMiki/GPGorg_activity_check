using Microsoft.VisualBasic.ApplicationServices;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json;
using FirebaseAdmin;
using DotNetEnv;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System.Diagnostics;
using System.Text;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;
using System.IO;
using Google.Cloud.Firestore.V1;

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
        private List<string>? postedUsers;
        static private FirestoreDb db;
        private bool failedDb = false;
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_Closing;

            // Create a ContextMenuStrip
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            // Add a "Copy" item to the context menu
            ToolStripMenuItem copyItem = new ToolStripMenuItem("Copy");
            copyItem.Click += CopyItem_Click; // Attach event handler
            contextMenu.Items.Add(copyItem);

            // Attach the context menu to the ListBox
            listBox3.ContextMenuStrip = contextMenu;
        }
        private void CopyItem_Click(object sender, EventArgs e)
        {
            // Copy selected items to the clipboard
            if (listBox3.SelectedItems.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in listBox3.SelectedItems)
                {
                    sb.AppendLine(item.ToString());
                }
                Clipboard.SetText(sb.ToString());
            }
        }
        private void listBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C) // Check for Ctrl+C
            {
                // Copy selected items to the clipboard
                if (listBox3.SelectedItems.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in listBox3.SelectedItems)
                    {
                        sb.AppendLine(item.ToString());
                    }
                    Clipboard.SetText(sb.ToString());
                }
            }
        }

        public static DateTime startDate = default(DateTime);

        static public FirestoreDb Db { get => db; set => db = value; }
        private void listBox3_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            string text = listBox3.Items[e.Index].ToString();
            SizeF textSize = e.Graphics.MeasureString(text, listBox3.Font, listBox3.Width);
            e.ItemHeight = (int)textSize.Height;
        }
        private void listBox3_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index >= 0)
            {
                string text = listBox3.Items[e.Index].ToString();
                using (Brush brush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
                }
            }

            e.DrawFocusRectangle();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load .env file
            Env.Load();

            // Log the generated JSON string for debugging
            // string credentialsJson = Firestore.GetFirebaseCredentialsJson();
            //listBox3.Items.Add(new UserMessage("Generated Credentials JSON:"));
            //listBox3.Items.Add(new UserMessage(credentialsJson));

            try
            {
                // Load Firebase credentials from JSON file
                string credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase.json");
                GoogleCredential credential;
                using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream);
                }

                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });

                listBox3.Items.Add(new UserMessage("Firebase initialized successfully!"));

                var projectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
                listBox3.Items.Add(new UserMessage($"Firestore Project ID: {projectId}"));

                if (string.IsNullOrEmpty(projectId))
                {
                    listBox3.Items.Add(new UserMessage("Error: FIREBASE_PROJECT_ID is not set!"));
                    failedDb = true;
                }
                else
                {
                    db = FirestoreDb.Create(projectId, new FirestoreClientBuilder
                    {
                        Credential = credential
                    }.Build());
                    Console.WriteLine("Connected to Firestore!");
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine($"Firebase Init Error: {ex.Message}");
                listBox3.Items.Add(new UserMessage($"Firebase Init Error: {ex.Message}"));
                failedDb = true;
            }

            dateTimePicker1.CustomFormat = "dd/MM/yyyy HH:mm";
            dateTimePicker2.CustomFormat = "dd/MM/yyyy HH:mm";
            //GPGSL_EndFound = false;
            GPGSL_StartFound = false;
            //Find when previous boost post was read (atm used only to read current page, not very reliable)
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
            try
            {
                TextReader tw = new StreamReader("SaveData\\timer.xml");
                numericUpDown3.Value = Convert.ToInt32(tw.ReadLine());
                tw.Close();
            }
            catch
            {
                listBox3.Items.Add(new UserMessage("No saved data for timer duration."));
                timer1.Interval = 60 * 60 * 1000;
            }
            numericUpDown2.Value = page;
            numericUpDown1.Value = page;

            //Find target API from file
            try
            {
                TextReader tr = new StreamReader("SaveData\\path.xml");
                textBox2.Text = tr.ReadLine();
                tr.Close();
            }
            catch
            {
                listBox3.Items.Add(new UserMessage("No target url saved."));
            }

            //Find image path from file
            try
            {
                TextReader tr = new StreamReader("SaveData\\path.xml");
                tr.ReadLine();
                textBox3.Text = tr.ReadLine();
                tr.Close();
            }
            catch
            {
                listBox3.Items.Add(new UserMessage("No image path saved."));
            }

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
            postedUsers = new List<string>();

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

            if (!failedDb)
                PushUsersNotPostedToFirestore();
        }
        private void PushUsersNotPostedToFirestore()
        {
            // Convert usersNotPosted to JSON
            string json = JsonConvert.SerializeObject(usersNotPosted, Formatting.Indented);

            // Upload the JSON to Firestore under the UID "notPosted"
            if (db != null)
            {
                DocumentReference docRef = db.Collection("warnings").Document("notPosted");
                docRef.SetAsync(new { Data = json }).Wait();
                // Log success
                listBox3.Items.Add(new UserMessage("Success: usersNotPosted pushed to Firestore"));
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

                //try
                //{
                //Find the date
                int date1 = post.IndexOf("Date:");
                post = post.Substring(date1 + 6);
                //listBox3.Items.Add("Post failed to add.");
                int date2 = post.IndexOf("<br>");
                bool autoLoad = false;
                if (date2 < 0)
                {
                    date2 = post.IndexOf("<br />"); //fix for automatic loading of posts
                    autoLoad = true;
                }
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
                Post lastPost = new Post();
                int thisPostId;
                int lastPostId;
                if (this.posts.Count > 0)
                {
                    lastPost = this.posts.Last();
                    thisPostId = (this.posts.Count + 1 - cnt);
                    lastPostId = lastPost.getPost();
                }
                else
                {
                    thisPostId = (0 + 1 - cnt);
                    lastPostId = NUM_POSTS_PAGE;
                }

                Post p = new Post("#" + page + "/" + thisPostId, user, ddate, body, autoLoad);
                if (lastPostId != NUM_POSTS_PAGE)
                {
                    if (lastPost.Date >= p.Date) continue;
                }
                //listBox3.Items.Add("LP:" + lastPostId + "; TP: " + thisPostId);
                this.posts.Add(p);
                /*}
                catch(Exception ex)
                {
                    listBox3.Items.Add("Post failed to add.");
                    listBox3.Items.Add(ex.Message);
                }*/

            }
            //Found all posts on page, tasks after
            int numPosts = (this.posts.Count - cnt);
            if (textBox1.Text.Length > 0)
            {
                if (numPosts <= 0)
                {
                    listBox3.Items.Add(new UserMessage("Found no posts, check HTML input"));
                }
                else
                {
                    listBox3.Items.Add(new UserMessage("Found " + numPosts + " posts on this page"));
                    if (autoCall && this.posts.Last().getPost() == NUM_POSTS_PAGE)
                    {
                        page++;
                        //autoCall = false;
                        numericUpDown1.Value = page;
                    }
                    refreshPosts();
                }
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
                /*
                //Saves the numericUpDown "boost post", to make it easier to look up the beginning of the deadline
                if (p.User == "GPGSL" && p.Body.IndexOf("Boost Announcement", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    listBox3.Items.Add(new UserMessage("Found boost post: " + p.Id));
                    numericUpDown2.Value = this.page - 1;

                    bool setEndDate = true;
                    for (int i = listBox1.Items.Count + 1; i < posts.Count; i++)
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
                        while (cntYear < 2 && yearId == -1)
                        {
                            yearId = p.Body.IndexOf(tryYear.ToString());
                            cntYear++;
                            tryYear++;
                        }
                        if (yearId > 0)
                        {
                            tryYear--;
                            //listBox3.Items.Add(tryYear);
                            //now try to find the month
                            int roundId = p.Body.IndexOf("Round", StringComparison.OrdinalIgnoreCase);
                            if (roundId >= 0)
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
                                if (mo > 0) //if we found a month
                                {
                                    //listBox3.Items.Add(mo);
                                    //now extract the date
                                    int ll = bitsOfString.Length;
                                    looking = bitsOfString[ll - 2];
                                    int dateIndex = looking.IndexOf("th");
                                    if (dateIndex < 0) dateIndex = looking.IndexOf("st");
                                    if (dateIndex < 0) dateIndex = looking.IndexOf("nd");
                                    if (dateIndex < 0) dateIndex = looking.IndexOf("rd");

                                    if (dateIndex > 0)
                                    {
                                        int dd = Convert.ToInt32(looking.Substring(0, dateIndex));
                                        //listBox3.Items.Add(dd);
                                        //now let's find the time
                                        int gmtIndex = -1;
                                        for (int j = ll - 1; j >= 0; j--)
                                        {
                                            if (bitsOfString[j].IndexOf("GMT", StringComparison.OrdinalIgnoreCase) >= 0)
                                            {
                                                gmtIndex = j - 1;
                                                break;
                                            }
                                        }
                                        if (gmtIndex >= 0)
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
                                            int time = Convert.ToInt32(looking.Substring(0, dateIndex - 1)) + (add12hr ? 12 : 0);
                                            if (!is24hr)
                                            {
                                                if (time % 12 == 0)
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
                }*/
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
                bool unknownUser = (p.User != "GPGSL") && (p.User != "GPGTV.org/farked");
                foreach (User u in this.usersNotPosted)
                {
                    if (u.Username == p.User)
                    {
                        if (postedUsers != null) this.postedUsers.Add(p.User);
                        listBox4.Items.Add(p.User + " - " + p.Id);
                        this.usersNotPosted.Remove(u);
                        unknownUser = false;
                        break;
                    }
                }
                if (unknownUser)
                {
                    if (postedUsers == null)
                    {
                        listBox3.Items.Add(new UserMessage("Unknown user at " + p.Id));
                        listBox1.Items.Add("^^ unknown user");
                        continue; //the original foreach (Posts p.. )
                    }
                    else
                    {
                        foreach (string s in this.postedUsers)
                        {
                            if (s == p.User)
                            {
                                unknownUser = false;
                                break;
                            }

                        }
                        if (unknownUser)
                        {
                            listBox3.Items.Add(new UserMessage("Unknown user at " + p.Id));
                            listBox1.Items.Add("^^ unknown user");
                            continue; //the original foreach (Posts p.. )
                        }
                    }
                }
                //if we know the user, check for potential announced absense/holiday
                int absense = 1;
                absense *= -Math.Min(0, p.Body.IndexOf("away")); //if returns -1, then -(-1)=1 and result will stay 1
                absense *= -Math.Min(0, p.Body.IndexOf("holiday")); //if found, return 0 so the product will stay 0
                absense *= -Math.Min(0, p.Body.IndexOf("absen")); //catches absence, absense or absent

                if (absense == 0) //if we FOUND a holiday, this MUST be 0
                {
                    listBox3.Items.Add(new UserMessage("Possible absence at " + p.Id));
                    listBox1.Items.Add("^^ possible absence !!!"); //alert me so I can manually check for a false positive
                }

            }
            try
            {
                //Change last updated time
                label11.Text = this.posts.Last().Date.ToString();

                //Change last updated page
                int ps = this.posts.Last().getPage();
                label12.Text = "(last updated page #" + ps + ")";

                // Create a JSON object with the last updated time and page
                var lastUpdatedData = new
                {
                    LastUpdatedTime = label11.Text,
                    LastUpdatedPage = label12.Text
                };

                // Serialize the object to JSON
                string json = JsonConvert.SerializeObject(lastUpdatedData, Formatting.Indented);

                // Upload the JSON to Firestore under the UID "notPosted"
                DocumentReference docRef = db.Collection("warnings").Document("lastUpdated");
                docRef.SetAsync(new { Data = json }).Wait();
            }
            catch (InvalidOperationException)
            { //occurs when clicking "refresh users not posted" without any posts, or when loading an empty save file

                label11.Text = "refreshed";
                label12.Text = "refreshed";
                //listBox3.Items.Add(new UserMessage("thrown InvalidOperationException"));
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
        private void saveWarningsToFile(object sender, EventArgs e, bool reset = false)
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

            // List to store structured data for JSON
            List<UserWarning> userWarnings = new List<UserWarning>();

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
                int warnings = 0;
                if (!reset)
                {
                    warnings = found && u.Away < dateTimePicker2.Value ? u.Warnings + 1 : u.Warnings;
                }
                tw.WriteLine(warnings);

                // Add to structured data for JSON
                userWarnings.Add(new UserWarning { Username = u.Username, Warnings = warnings });
            }
            listBox3.Items.Add(new UserMessage("Saved new warnings to file."));
            tw.Close();

            // Convert structured data to JSON
            string json = JsonConvert.SerializeObject(userWarnings, Formatting.Indented);

            // Post JSON to Firebase Firestore
            try
            {
                Firestore.UploadToFirestore(json);
                listBox3.Items.Add(new UserMessage("Pushed new warnings to Firestore."));
            }
            catch (Exception ex)
            {
                listBox3.Items.Add(new UserMessage("Error: Could not save to Firestore." + ex.Message));
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            saveWarningsToFile(sender, e, false);
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

        private void button8_Click(object sender, EventArgs e)
        {
            string api = textBox2.Text + ",page=" + numericUpDown1.Value;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
            try
            {
                using HttpResponseMessage resp = client.GetAsync(api).Result;

                string httpResponse = resp.Content.ReadAsStringAsync().Result;

                listBox3.Items.Add(new UserMessage("Loaded posts automatically for page " + numericUpDown1.Value));

                textBox1.Text = httpResponse;
                listBox3.Items.Add(new UserMessage("Loaded posts automatically for page " + numericUpDown1.Value));
            }
            catch
            {
                listBox3.Items.Add(new UserMessage("Fetch failed: check URL."));
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            savePath();
        }

        private void savePath()
        {
            try
            {
                TextWriter tw = new StreamWriter("SaveData\\path.xml");
                tw.WriteLine(textBox2.Text);
                tw.WriteLine(textBox3.Text);
                listBox3.Items.Add(new UserMessage("Saved new target url to disk."));
                tw.Close();
            }
            catch
            {
                listBox3.Items.Add(new UserMessage("Failed to save new url to disk."));
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            savePath();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = 1000 * 60 * Convert.ToInt32(numericUpDown3.Value
                );
            TextWriter tw = new StreamWriter("SaveData\\timer.xml");
            tw.WriteLine(numericUpDown3.Value.ToString());
            tw.Close();
            listBox3.Items.Add(new UserMessage("Changed timer interval."));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            listBox3.Items.Add(new UserMessage("Timer tick"));
            button8_Click(sender, e); //load posts from API call
            button9_Click(sender, e); //save screenshot to disk
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox3.Checked;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = Form.ActiveForm;
                using (var bmp = new Bitmap(frm.Width, frm.Height))
                {
                    //NOTE: magic values for location of rectangle, if screenshot is bad, edit here
                    Rectangle cropRect = new Rectangle(630, 40, 1090 - 630, 810);
                    frm.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    using (Bitmap bmp2 = bmp.Clone(cropRect, bmp.PixelFormat))
                    {
                        string path = textBox3.Text + "\\gpgsl.png";
                        bmp2.Save(@path);
                    }
                }
                listBox3.Items.Add(new UserMessage("Saved image to disk."));
            }
            catch (Exception ex)
            {
                listBox3.Items.Add(new UserMessage("Save to disk failed."));
                listBox3.Items.Add(new UserMessage(ex.Message));
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            saveWarningsToFile(sender, e, true);
        }
    }
    public class UserWarning
    {
        public string Username { get; set; }
        public int Warnings { get; set; }
    }
}