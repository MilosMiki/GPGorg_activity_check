﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPGorg_activity_check
{
    [Serializable]
    public class Post
    {
        private string id;
        private string user;
        private DateTime date;
        private string body;

        public Post()
        {
            id = "";
        }

        public Post(string i, string u="",string d = "",string b = "", bool autoLoad = false)
        {
            id = i;
            this.user = u;
            this.date = stringToDate(d,autoLoad);
            this.body = b;
        }

        public string Id { get => id; set => id = value; }
        public string User { get => user; set => user = value; }
        public string Body { get => body; set => body = value; }
        public DateTime Date { get => date; set => date = value; }

        public int getPage()
        {
            string ps = this.Id;
            int ips = ps.IndexOf("/");
            return Convert.ToInt32(ps.Substring(1, ips - 1));
        }

        public int getPost()
        {
            string ps = this.Id;
            int ips = ps.IndexOf("/") + 1;
            return Convert.ToInt32(ps.Substring(ips, ps.Length-ips));
        }

        public DateTime stringToDate(string d, bool autoLoad = false)
        {
            string ampm = d.Substring(d.Length - 2, 2);
            int min = Convert.ToInt32(d.Substring(d.Length - 4, 2));
            int hr = Convert.ToInt32(d.Substring(d.Length - 7, 2));
            if (hr == 12)
            {
                if (ampm == "AM")
                    hr = 0;
            }
            else
            {
                if (ampm == "PM")
                {
                    hr += 12;
                }
            }
            int yr = Convert.ToInt32(d.Substring(d.Length - 12, 4));
            int da = Convert.ToInt32(d.Substring(d.Length - 16, 2));
            string moStr = d.Substring(0, d.Length - 17);
            int mo = 0;
            switch (moStr)
            {
                case "January":
                    mo = 1;
                    break;
                case "February":
                    mo = 2;
                    break;
                case "March":
                    mo = 3;
                    break;
                case "April":
                    mo = 4;
                    break;
                case "May":
                    mo = 5;
                    break;
                case "June":
                    mo = 6;
                    break;
                case "July":
                    mo = 7;
                    break;
                case "August":
                    mo = 8;
                    break;
                case "September":
                    mo = 9;
                    break;
                case "October":
                    mo = 10;
                    break;
                case "November":
                    mo = 11;
                    break;
                case "December":
                    mo = 12;
                    break;
            }
            DateTime newDate = new DateTime(yr, mo, da, hr, min, 0);
            if (autoLoad)
            {
                return newDate.AddHours(-2);
            }
            return newDate;
        }

        public override string? ToString()
        {
            return id + " - " + user + " (" + date + ")";
        }
    }
}
