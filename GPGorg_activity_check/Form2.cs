using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GPGorg_activity_check
{
    public partial class Form2 : Form
    {
        private string Id;
        private List<Post> posts;
        public Post P { get; set; }

        public Form2(List<Post> posts)
        {
            InitializeComponent();
            this.posts = posts;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Id = "#" + numericUpDown1.Value + "/" + numericUpDown2.Value;
            textBox1.Text = Id;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Id = "#" + numericUpDown1.Value + "/" + numericUpDown2.Value;
            textBox1.Text = Id;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Id = textBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool b = false;
            foreach (Post p1 in posts)
            {
                if (p1.Id == Id)
                {
                    P = p1;
                    b = true;
                    Close();
                }
            }
            if(!b)
                MessageBox.Show("Error: Post not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
