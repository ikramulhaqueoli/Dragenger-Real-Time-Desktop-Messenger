using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLibrary
{
    public class Time
    {
        private int year, month, day, hour, minute, second;
        private static string[] months = new string[] { "n/a", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public Time(DateTime time)
        {
            this.year = time.Year;
            this.month = time.Month;
            this.day = time.Day;
            this.hour = time.Hour;
            this.minute = time.Minute;
            this.second = time.Second;
        }
        public Time(int year, int month, int day, int hour, int minute, int second)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }
        public Time(string timeValue) //format -> yyyymmddhhmiss
        {
            try
            {
                this.year = int.Parse(timeValue.Substring(0, 4));
                this.month = int.Parse(timeValue.Substring(4, 2));
                this.day = int.Parse(timeValue.Substring(6, 2));
                this.hour = int.Parse(timeValue.Substring(8, 2));
                this.minute = int.Parse(timeValue.Substring(10, 2));
                if (timeValue.Length > 12) this.second = int.Parse(timeValue.Substring(12, 2));
                else this.second = 0;
            }
            catch { }
        }

        public string TimeStampString    //format -> yyyymmddhhmmss
        {
            get
            {
                string stamp = "" + this.Year;
                if (this.Month < 10) stamp += '0';
                stamp += "" + this.Month;
                if (this.Day < 10) stamp += '0';
                stamp += "" + this.Day;
                if (this.Hour < 10) stamp += '0';
                stamp += "" + this.Hour;
                if (this.Minute < 10) stamp += '0';
                stamp += "" + this.Minute;
                if (this.Second < 10) stamp += '0';
                stamp += "" + this.Second;
                return stamp;
            }
        }

        public string DbFormat
        {
            get
            {
                //YYYY-MM-DD HH:MI:SS
                string dbformat = "" + this.Year;
                dbformat += '-';
                if (this.Month < 10) dbformat += '0';
                dbformat += "" + this.Month;
                dbformat += '-';
                if (this.Day < 10) dbformat += '0';
                dbformat += "" + this.Day;
                dbformat += ' ';
                if (this.Hour < 10) dbformat += '0';
                dbformat += "" + this.Hour;
                dbformat += ':';
                if (this.Minute < 10) dbformat += '0';
                dbformat += "" + this.Minute;
                dbformat += ':';
                if (this.Second < 10) dbformat += '0';
                dbformat += "" + this.Second;
                return dbformat;
            }
        }

        public int Day
        {
            get { return this.day; }
        }
        public int Month
        {
            get { return this.month; }
        }
        public int Year
        {
            get { return this.year; }
        }
        public string MMM
        {
            get { return Time.months[this.Month]; }
        }
        public int Hour
        {
            get { return this.hour; }
        }
        public int Minute
        {
            get { return this.minute; }
        }
        public int Second
        {
            get { return this.second; }
        }
        public string Meridiem
        {
            get
            {
                if (this.Hour <= 12) return "am";
                else return "pm";
            }
        }
        public int Hour12
        {
            get
            {
                if (this.Hour > 0 && this.hour <= 12) return this.Hour;
                else if (this.Hour > 12) return this.Hour - 12;
                else return 12;
            }
        }

        public string LongDate
        {
            get { return this.day + " " + Time.months[this.month] + " " + this.year; }
        }
        public string ShortDate
        {
            get
            {
                string shortDate = "";
                if (this.day < 10) shortDate += "0" + this.day;
                else shortDate += this.day;
                shortDate += '/';
                if (this.month < 10) shortDate += "0" + this.month;
                else shortDate += this.month;
                shortDate += '/';
                if (this.year < 10) shortDate += "0" + this.year;
                else shortDate += this.year;
                return shortDate;
            }
        }
        public string DateTimeShort
        {
            get { return this.ShortDate + " " + Time24; }
        }
        public string DateTimeLong
        {
            get { return this.LongDate + " " + Time12; }
        }
        public string Time12
        {
            get
            {
                string hString = this.Hour12.ToString();
                while (hString.Length < 2) hString = '0' + hString;
                string mString = this.minute.ToString();
                while (mString.Length < 2) mString = '0' + mString;
                string time12 = hString + ":" + mString + " " + this.Meridiem;
                return time12;
            }
        }

        public string Time24
        {
            get
            {
                string hString = this.Hour + "";
                while (hString.Length < 2) hString = '0' + hString;
                string mString = this.minute.ToString();
                while (mString.Length < 2) mString = '0' + mString;
                return hString + ":" + mString;
            }
        }

        public static long TimeDistanceInMinute(Time t1, Time t2)
        {
            double timeLong1 = t1.Year * 525600 + t1.Month * 43200 + t1.Day * 1440 + t1.Hour * 60 + t1.Minute + t1.Second / 60.0;
            double timeLong2 = t2.Year * 525600 + t2.Month * 43200 + t2.Day * 1440 + t2.Hour * 60 + t2.Minute + t2.Second / 60.0;
            return (long)Math.Round(Math.Abs(timeLong1 - timeLong2));                               //preceision mistake may exist
        }

        public static long TimeDistanceInSecond(Time t1, Time t2)
        {
            long timeLong1 = t1.Year * 31536000 + t1.Month * 2592000 + t1.Day * 86400 + t1.Hour * 3600 + t1.Minute * 60 + t1.Second;
            long timeLong2 = t2.Year * 31536000 + t2.Month * 2592000 + t2.Day * 86400 + t2.Hour * 3600 + t2.Minute * 60 + t2.Second;
            return Math.Abs(timeLong1 - timeLong2);
        }

        public static Time CurrentTime
        {
            get { return new Time(DateTime.Now); }
        }

        //demo_instance
        public static Time RandomTime
        {
            get
            {
                Random random = new Random();
                int year = (random.Next() % 100) + 2000;
                int month = (random.Next() % 12) + 1;
                int day = (random.Next() % 28) + 1;
                int hour = random.Next() % 24;
                int minute = (random.Next() % 60);
                int second = (random.Next() % 60);
                return new Time(year, month, day, hour, minute, second);
            }
        }
    }
}
