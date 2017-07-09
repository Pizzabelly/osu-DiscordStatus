using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Discord;
using System.Xml;
namespace osuDiscordStatus
{
    class Program
    {
        DiscordClient _client;
        String discordGame;
        static Process osuProcess;
        String previousGame = null;  
        static void Main(string[] args) => new Program().Start();

        static string getNowPlaying()
        {   
            Process[] process = Process.GetProcessesByName("osu!");
            if (process.Length > 0)
            {
                osuProcess = process[0];
                string windowTitle = osuProcess.MainWindowTitle;

                if (windowTitle.IndexOf("-") == -1)
                    return ("Selecting song...");
                else
                    return (windowTitle.Substring(windowTitle.IndexOf('-') + 2));
            }
            else
            {
                Console.WriteLine("osu! process not found plz open osu!");
                return (null);
            }
            
        }
        public void Start()
        {
            XmlDocument settingsXml = new XmlDocument();
            settingsXml.Load("settings.xml");
            XmlNodeList clientEmail = settingsXml.GetElementsByTagName("email");
            XmlNodeList clientPassword = settingsXml.GetElementsByTagName("password");
            XmlNodeList updateRate = settingsXml.GetElementsByTagName("update");

            string email = clientEmail[0].InnerText.Trim();
            string password = clientPassword[0].InnerText.Trim();

            _client = new DiscordClient();
            if (email.IndexOf("@") > -1)
            {
                _client.ExecuteAndWait(async () =>
                {
                    await _client.Connect(email, password);
                    while (true)
                    {
                        Thread.Sleep(Convert.ToInt32(updateRate[0].InnerText.Trim()));
                        discordGame = getNowPlaying();
                        if (discordGame != previousGame)
                        {
                            _client.SetGame(discordGame);
                            Console.WriteLine(discordGame);
                        }
                        previousGame = discordGame;
                    }
                });
            }
            else
            {
                Console.WriteLine("Email Not Found");
                Console.ReadKey();
            }
        }
    }
}