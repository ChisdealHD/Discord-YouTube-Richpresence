using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace YouTube_Rich_Presence
{
    class YoutubeHandler
    {
        private System.Timers.Timer timer;
        private string currentTitle;
        private bool initialized = false;

        public YoutubeHandler(DiscordRpcClient client)
        {
            Initialize(client);
        }

        private void Initialize(DiscordRpcClient client)
        {
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += (sender, evt) => { client.Invoke(); Update(client); };
            timer.Start();

            secondUpd(client);
        }

        private void secondUpd(DiscordRpcClient client)
        {
            Process[] processes = Process.GetProcessesByName("chrome");
            if (processes.Length == 0)
            {
                setPresence(client, "Currently watching nothing.", "Taking a break..", false);
                Console.WriteLine("Chrome is not running");
                return;
            }
            foreach (var process in processes)
            {
                if(process.MainWindowHandle == IntPtr.Zero)
                {
                    continue; //Goto next element
                }
                AutomationElement root = AutomationElement.FromHandle(process.MainWindowHandle);
                Condition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window);
                var tabs = root.FindAll(TreeScope.Descendants, condition);
                Console.WriteLine(tabs.Count);
            }
        }
        private void Update(DiscordRpcClient client)
        {
            Process[] processes = Process.GetProcessesByName("chrome");
            if(processes.Length == 0)
            {
                setPresence(client, "Currently watching nothing.", "Taking a break..", false);
                return;
            }

            foreach (var process in processes)
            { 
                if(process.MainWindowTitle.Contains("YouTube"))
                {

                    string title = process.MainWindowTitle; //Max Length care!
                    title = title.Replace("- YouTube - Google Chrome", ""); //Remove Youtube and Chrome message

                    if (title.Length > 127)
                        title = title.Substring(0, 127);

                    //Check if Notifications are available
                    Regex titleRegex = new Regex(@"/^\s*?\(\d+\)/"); 
                    Match titleMatch = titleRegex.Match(title);

                    if (titleMatch.Success)
                        title = title.Substring(titleMatch.Length);

                    if (currentTitle == "") //Set Title after reset
                        currentTitle = title;

                    if (title != currentTitle) //Still in same video
                    {
                        currentTitle = title;
                        initialized = false;
                    }

                    //Title is the same
                    if (!initialized)
                    {
                        setPresence(client, "Currently watching a Video:", title, true);
                        initialized = true;
                    }
                    return;
                }
            }
            currentTitle = "";
            initialized = false;
            setPresence(client, "Currently watching nothing.", "Taking a break..", false); 
        }
        private void setPresence(DiscordRpcClient client, string details, string state, bool watching)
        {
            if (!watching)
            {
                client.SetPresence(new RichPresence()
                {
                    Details = details,
                    State = state,
                    Assets = new Assets()
                    {
                        LargeImageKey = "youtube-512"
                    }
                });
            }
            else
            {
                client.SetPresence(new RichPresence()
                {
                    Details = details,
                    State = state,
                    Assets = new Assets()
                    {
                        LargeImageKey = "youtube-512"
                    },
                    Timestamps = new Timestamps()
                    {
                        Start = DateTime.UtcNow
                    }
                });
            }
        }
        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
