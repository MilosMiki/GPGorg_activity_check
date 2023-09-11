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
        private DateTime away;

        public User(string u = "", int i = 0, DateTime a = default(DateTime))
        {
            this.username = u;
            this.warnings = i;
            this.away = a;
        }

        public string Username { get => username; set => username = value; }
        public int Warnings { get => warnings; set => warnings = value; }
        public DateTime Away { get => away; set => away = value; }

        public override string? ToString()
        {
            return (away > default(DateTime) ? "// " : "") + username + " (" + warnings + ")" + (away > default(DateTime) ? " - "+away : "");
        }
    }
}
