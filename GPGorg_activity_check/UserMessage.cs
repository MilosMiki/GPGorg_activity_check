using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPGorg_activity_check
{
    public class UserMessage
    {
        private string message;
        private TimeOnly time;

        public UserMessage(string m)
        {
            this.message = m;
            this.time = TimeOnly.FromDateTime(DateTime.Now);
        }
        public string Message { get => message; set => message = value; }

        public override string? ToString()
        {
            TimeSpan ts = new TimeSpan(time.Hour,time.Minute,time.Second);
            return ts + ": " + message;
        }
    }
}
