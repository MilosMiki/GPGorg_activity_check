using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPGorg_activity_check
{
    public class User
    {
        private string username;
        private int warnings;

        public User(string u = "", int i = 0)
        {
            this.username = u;
            this.warnings = i;
        }

        public string Username { get => username; set => username = value; }
        public int Warnings { get => warnings; set => warnings = value; }

        public override string? ToString()
        {
            return username + " (" + warnings + ")";
        }
    }
}
