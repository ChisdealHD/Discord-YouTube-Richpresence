using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTube_Rich_Presence
{
    class RichPresenceClient
    {
        private DiscordRpcClient client;
        string clientID = "530364837688246273";
        private YoutubeHandler youtubeHandler;

        public RichPresenceClient()
        {
            client = new DiscordRpcClient(clientID);

            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            client.Initialize();

            client.SetPresence(new RichPresence()
            {
                Details = "Currently playing nothing.",
                State = "On a break!",
                Assets = new Assets()
                {
                    LargeImageKey = "youtube-512"
                }
            });

            youtubeHandler = new YoutubeHandler(client);

            Console.ReadKey();
            //Program ends here, so dispose
            youtubeHandler.Dispose();
            client.Dispose();
            
        }
    }
}
