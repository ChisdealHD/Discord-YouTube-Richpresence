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
        private string updateCache = "";

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
                var update = e.Presence.Details + " " + e.Presence.State;
                //Remember update to not spam update messages unnecessarily.
                if (updateCache == update) return;

                Console.WriteLine("Received Update! {0} {1}", e.Presence.Details, e.Presence.State);
                updateCache = update;
            };

            client.Initialize();

            youtubeHandler = new YoutubeHandler(client);

            Console.WriteLine("Press Enter to close Program");
            Console.ReadLine();
            //Program ends here, so dispose
            youtubeHandler.Dispose();
            client.Dispose();
            
        }
    }
}
