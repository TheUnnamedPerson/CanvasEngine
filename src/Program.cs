namespace CanvasEngine
{
    class Program
    {
        bool finished = false;
        const string AUTH_FILE = "./config/AUTH-INFO-HERE.txt";
        const string DATE_FILE = "./config/SEMESTER-START-DATE.txt";
        const string WE_PATH_FILE = "./config/WALLPAPER-ENGINE-PATH-HERE.txt";
        const string WE_ARGS = "wallpaper64.exe -control applyProperties -properties ";
        const string WE_JSON_START = "RAW~({\"assignmentslist\":\"";
        const string WE_JSON_END = "\"})~END";

        static async Task Main(string[] args)
        {
            //Read Data From Config Text Files
            Console.WriteLine("Reading From Files...\n");

            string[] Auth_Lines = System.IO.File.ReadAllLines(AUTH_FILE);
            string[] Path_lines = System.IO.File.ReadAllLines(WE_PATH_FILE);
            string[] Date_lines = System.IO.File.ReadAllLines(DATE_FILE);

            string URL = Auth_Lines[0];
            string AUTH_TOKEN = Auth_Lines[1];

            string WALLPAPER_ENGINE = Path_lines[0];
            int.TryParse(Date_lines[1], out int outputToWE);

            Date semesterStartDate = new Date(Date_lines[0]);
            int.TryParse(Date_lines[1], out int daysTillDue);

            //Find Courses From This Semester
            Console.WriteLine("\nGetting Courses...");

            Canvas canvas = new Canvas(URL, AUTH_TOKEN);
            List<Course> courses = await canvas.GetCourses();
            List<Course> activeCourses = new List<Course>();
            foreach (Course c in courses)
            {
                Date startDate = new Date(c.start_at);
                if (startDate - semesterStartDate >= 0)
                {
                    Console.WriteLine(c.name);
                    activeCourses.Add(c);
                }
            }
            Console.WriteLine("\nActive Courses = " + activeCourses.Count.ToString());

            //Find Assignments From Within Time Allocated
            Console.WriteLine("\nGetting Assignments...");
            string listToDisplay = "";
            foreach (Course c in activeCourses)
            {
                bool b = false;
                string s = "";
                Console.WriteLine(c.name);
                s += c.name += "\\n";
                List<Assignment> assignments = new List<Assignment>();
                assignments = await canvas.GetAssignments(c.id);
                foreach (Assignment a in assignments)
                {
                    Date dueDate = new Date(a.due_at);
                    if (0 <= (dueDate - Date.Today()) && (dueDate - Date.Today()) < daysTillDue)
                    {
                        b = true;
                        s += " - " + a.name + "\\n    - " + dueDate.ToString("USA") + "\\n";
                    }
                }
                if (b)
                {
                    listToDisplay += s + "\\n";
                }
            }

            Console.WriteLine("\n\n" + listToDisplay + "\n");

            if (outputToWE == 1)
            {
                //Feed To Wallpaper Engine
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                Console.WriteLine("\n\n/C cd " + WALLPAPER_ENGINE + " && " + WE_ARGS + WE_JSON_START + listToDisplay + WE_JSON_END + "\n");
                startInfo.Arguments = "/C cd " + WALLPAPER_ENGINE + " && " + WE_ARGS + WE_JSON_START + listToDisplay + WE_JSON_END;
                startInfo.FileName = "cmd.exe";
                process.StartInfo = startInfo;
                process.Start();
            }

            // Writing To Output File
            listToDisplay = listToDisplay.Replace("\\n", "\n");
            await File.WriteAllTextAsync("./Output.txt", listToDisplay);

            Console.WriteLine("\n\nFinished.");
        }
    }
}
