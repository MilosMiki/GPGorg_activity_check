using Microsoft.VisualBasic.ApplicationServices;

namespace GPGorg_activity_check
{
    public partial class Form1 : Form
    {
        private bool GPGSL_StartFound;
        //private bool GPGSL_EndFound;
        private int page;
        private List<User>? users;
        private List<User>? usersNotPosted;
        private List<Post>? posts;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = "dd/MM/yyyy HH:mm";
            dateTimePicker2.CustomFormat = "dd/MM/yyyy HH:mm";
            //GPGSL_EndFound = false;
            GPGSL_StartFound = false;
            page = 1;
            button1_Click(sender, e, true);
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
            checkBox1.Checked = b;
            posts = new List<Post>();
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

        private void button1_Click(object sender, EventArgs e, bool firstTime)
        {
            TextReader tr = new StreamReader("users.txt");
            List<User> uss = new List<User>();
            string? s;
            while ((s = tr.ReadLine()) != null)
            {
                uss.Add(new User(s, Convert.ToInt32(tr.ReadLine())));
            }
            this.users = new List<User>(uss);
            usersNotPosted = new List<User>(uss);
            refreshUsersNotPosted();
            if (!firstTime)
            {
                refreshPosts(true);
            }
            tr.Close();
            backupTextFile();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3_Click(sender, e, false);
        }

        private void button3_Click(object sender, EventArgs e, bool autoCall = false)
        {
            if (this.posts == null)
            {
                listBox3.Items.Add(new UserMessage("Failed: Null reference in posts"));
                return;
            }
            int cnt = this.posts.Count;
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
                Post p = new Post("#" + page + "/" + (this.posts.Count + 1 - cnt), user, ddate, body);
                this.posts.Add(p);
            }
            if (textBox1.Text.Length > 0)
            {
                listBox3.Items.Add(new UserMessage("Found " + (this.posts.Count - cnt) + " posts on this page"));
                if (autoCall)
                {
                    page++;
                }
                refreshPosts();
            }
            else
            {
                listBox3.Items.Add(new UserMessage("Finished"));
            }
        }

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
                listBox1.Items.Add(p);
                if (!GPGSL_StartFound)
                {
                    if (dateTimePicker1.Value > p.Date)
                    {
                        try
                        {
                            if (page - 1 == Convert.ToInt32(p.Id.Substring(1, p.Id.IndexOf("/") - 1)))
                                listBox3.Items.Add(new UserMessage("Found post before deadline: " + p.Id));
                        }
                        catch (Exception ex)
                        {
                            listBox3.Items.Add(new UserMessage(ex.ToString()));
                        }
                        continue;
                    }
                }
                if (p.Date > dateTimePicker2.Value)
                {
                    try
                    {
                        if (page - 1 == Convert.ToInt32(p.Id.Substring(1, p.Id.IndexOf("/") - 1)))
                            listBox3.Items.Add(new UserMessage("Found post after deadline: " + p.Id));
                    }
                    catch (Exception ex)
                    {
                        listBox3.Items.Add(new UserMessage(ex.ToString()));
                    }
                    continue;
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
                listBox1.Items.Add(p);
            }
        }

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
                if (found)
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
            if(this.page > 1)
            {
                checkBox1.Checked = false;
            }
            else {  checkBox1.Checked = true; }
        }
    }
}