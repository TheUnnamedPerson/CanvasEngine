using System.Net;
using Newtonsoft.Json;

namespace CanvasEngine
{
    static class webMethods
    {
        public static string GET(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using(Stream stream = response.GetResponseStream())
            using(StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static async Task<string> GET_Async(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using(HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using(Stream stream = response.GetResponseStream())
            using(StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }

    class Canvas
    {
        const string AUTH_CODE = "?access_token=";
        string url = "";
        string token = "";

        public Canvas(string _url, string _token)
        {
            url = _url;
            token = _token;
        }

        public async Task<List<Course>> GetCourses ()
        {
            string s = await webMethods.GET_Async(url + "/api/v1/courses" + AUTH_CODE + token + "&per_page=100");

            List<Course> _c = JsonConvert.DeserializeObject<List<Course>>(s);

            return _c;
        }

        public async Task<List<Assignment>> GetAssignments (int courseID)
        {
            //List<string> assignments = new List<string>();

            string s = await webMethods.GET_Async(url + "/api/v1/courses/" + courseID + "/assignments" + AUTH_CODE + token + "&per_page=100");

            //Console.WriteLine(s);

            List<Assignment> _c = JsonConvert.DeserializeObject<List<Assignment>>(s);

            return _c;
        }

        public async Task<string> GET (string s)
        {
            string r = await webMethods.GET_Async(url + s + AUTH_CODE + token);
            return r;
        }
    }

    class Date
    {
        public int year = 2000;
        public int month = 1;
        public int day = 1;

        public Date ()
        {
            year = 2000;
            month = 1;
            day = 1;
        }

        public Date (string s)
        {
            if (s == "") return;
            //Console.WriteLine("\t\tConverting Date, s = " + s);
            try
            {
                string y = "" + s[0] + s[1] + s[2] + s[3];
                int.TryParse(y, out year);
                string m = "" + s[5] + s[6];
                int.TryParse(m, out month);
                string d = "" + s[8] + s[9];
                int.TryParse(d, out day);
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Exception: Wrong Date Format! (First 10 Characters Should Be YYYY-MM-DD)");
            }
            catch
            {

            }
            
        }

        public Date (int y, int m, int d)
        {
            year = y;
            month = m;
            day = d;
        }

        public override string ToString ()
        {
            return year.ToString() + "-" + month.ToString() + "-" + day.ToString();
        }

        public string DayOfTheWeek ()
        {
            int h = (int)((year % 1000) * 1.25f) + day;
            switch (month)
            {
                case 1:
                    if((year % 4 == 0) && ((year % 100 != 0) || ((year % 100 == 0) && (year % 400 == 0))))
                        h += 1;
                    else
                        h += 0;
                    break;
                case 2:
                    if((year % 4 == 0) && ((year % 100 != 0) || ((year % 100 == 0) && (year % 400 == 0))))
                        h += 4;
                    else
                        h += 3;
                    break;
                case 3:
                case 11:
                    h += 4;
                    break;
                case 4:
                case 7:
                    h += 0;
                    break;
                case 5:
                    h += 2;
                    break;
                case 6:
                    h += 5;
                    break;
                case 8:
                    h += 3;
                    break;
                case 9:
                case 12:
                    h += 6;
                    break;
                case 10:
                    h += 1;
                    break;
            }

            h = h % 7;

            string _out = "";
            switch (h)
            {
                case 0:
                    _out = "Sunday";
                    break;
                case 1:
                    _out = "Monday";
                    break;
                case 2:
                    _out = "Tuesday";
                    break;
                case 3:
                    _out = "Wednesday";
                    break;
                case 4:
                    _out = "Thursday";
                    break;
                case 5:
                    _out = "Friday";
                    break;
                case 6:
                    _out = "Saturday";
                    break;
            }

            return _out;
        }

        public string ToString (string format, bool dayOfWeek = false)
        {
            string s = "";
            if (dayOfWeek)
            {
                s += DayOfTheWeek() + ", ";
            }
            switch (format)
            {
                case "USA":
                    s += month.ToString() + "/" + day.ToString() + "/" + year.ToString();
                    break;
                case "computer":
                default:
                    s += year.ToString() + "-" + month.ToString() + "-" + day.ToString();
                    break;
            }
            return s;
        }

        public static int DaysApart (Date x, Date y)
        {
            return Math.Abs(x - y);
        }

        public int DayOfYear ()
        {
            int d = 0;
            for (int i = 1; i < month; i++)
            {
                switch (i)
                {
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                    case 12:
                        d += 31;
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        d += 30;
                        break;
                    case 2:
                        d += (((year % 4 == 0) && ((year % 100 != 0) || ((year % 100 == 0) && (year % 400 == 0)))) ? 29 : 28);
                        break;
                }
            }
            return d + day;
        }

        public static int operator- (Date x, Date y)
        {
            int d = 0;
            bool n = false;
            Date a = x;
            Date b = y;
            if (x.year < y.year)
            {
                a = y;
                b = x;
                n = true;
            }
            d += (a.year - b.year) * 365 + ((int)(((b.year % 4) + (a.year - b.year)) / 4));
            d += (a.DayOfYear() - b.DayOfYear());

            if (n) d *= -1;

            return d;
        }

        public static Date operator- (Date x, int y)
        {
            if (y < 0)
            {
                return x + (y * -1);
            }

            int h = x.day - y;
            int m = x.month;
            int k = x.year;
            
            while (h < 1)
            {
                m -= 1;
                while (m < 1)
                {
                    k -= 1;
                    m += 12;
                }
                switch (m)
                {
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                    case 12:
                        h += 31;
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        h += 30;
                        break;
                    case 2:
                        h += (((k % 4 == 0) && ((k % 100 != 0) || ((k % 100 == 0) && (k % 400 == 0)))) ? 29 : 28);
                        break;
                }
            }

            return new Date(k, m, h);
        }

        public static Date operator+ (Date x, int y)
        {
            if (y < 0)
            {
                return x - (y * -1);
            }

            int h = x.day + y;
            int m = x.month;
            int k = x.year;

            int t = 0;
            switch (m)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    t = 31;
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    t = 30;
                    break;
                case 2:
                    t = (((k % 4 == 0) && ((k % 100 != 0) || ((k % 100 == 0) && (k % 400 == 0)))) ? 29 : 28);
                    break;
            }
            
            while (h < t)
            {
                switch (m)
                {
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                    case 12:
                        h -= 31;
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        h -= 30;
                        break;
                    case 2:
                        h -= (((k % 4 == 0) && ((k % 100 != 0) || ((k % 100 == 0) && (k % 400 == 0)))) ? 29 : 28);
                        break;
                }
                m += 1;
                while (m > 12)
                {
                    k += 1;
                    m -= 12;
                }
                switch (m)
                {
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                    case 12:
                        t = 31;
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        t = 30;
                        break;
                    case 2:
                        t = (((k % 4 == 0) && ((k % 100 != 0) || ((k % 100 == 0) && (k % 400 == 0)))) ? 29 : 28);
                        break;
                }
            }

            return new Date(k, m, h);
        }
    
        public static Date Today ()
        {
            return new Date(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        }
    
    }

    class Course
    {
        public int id = 0;
        public string name = "";
        public int account_id = 0;
        public string uuid = "";
        public string start_at = "";
        public string courseCode = "";
        public object grading_standard_id = null;
        public Nullable<bool> is_public = false;
        public string created_at = "";
        public string course_code = "";
        public string default_view = "";
        public int root_account_id = 0;
        public int enrollment_term_id = 0;
        public string license = "";
        public object grade_passback_setting = null;
        public object end_at = null;
        public bool public_syllabus = false;
        public bool public_syllabus_to_auth = false;
        public int storage_quota_mb = 0;
        public bool is_public_to_auth_users = false;
        public bool homeroom_course = false;
        public object course_color = null;
        public string friendly_name = null;
        public bool apply_assignment_group_weights = false;
        public Calendar calendar = new Calendar();
        public string time_zone = "";
        public bool blueprint = false;
        public bool template = false;
        public List<Enrollment> enrollments = new List<Enrollment>();
        public bool hide_final_grades = false;
        public string workflow_state = "";
        public bool restrict_enrollments_to_course_dates = false;
        public string overridden_course_visibility = "";
    }

    class Assignment
    {
        public int ID = 0;
        public string name = "";
        public string due_at = "";
    }

    class Calendar
    {
        public string ics = "";
    }

    class Enrollment
    {
        public string type = "";
        public string role = "";
        public int role_id = 0;
        public int user_id = 0;
        public string enrollment_state = "";
        public bool limit_privileges_to_course_section = false;
            
        
    }
}