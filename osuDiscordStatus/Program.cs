﻿using System;
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
        XmlDocument settingsXml = new XmlDocument();
        public int update = 1000;
        static String windowTitle = null;
        static String nowPlaying = null;
        private DiscordClient _client;
        private String discordGame = null;
        private String previousGame = null;
        private String email = null;
        private String password = null;
        static Process osuProcess = null;
        static void Main(string[] args) => new Program().Start();

        static string getNowPlaying()
        {   
            Process[] process = Process.GetProcessesByName("osu!");
            if (process.Length > 0)
            {
                osuProcess = process[0];

                windowTitle = osuProcess.MainWindowTitle;

                if (windowTitle.IndexOf("-") == -1)
                {
                    return ("Selecting song...");
                }
                else
                {
                    nowPlaying = windowTitle.Substring(windowTitle.IndexOf('-') + 2);
                    return (nowPlaying);
                }
            }
            else
            {
                Console.WriteLine("osu! process not found plz open osu!");
                return (null);
            }
            
        }
        public void Start()
        {
            settingsXml.Load("settings.xml");
            XmlNodeList clientEmail = settingsXml.GetElementsByTagName("email");
            XmlNodeList clientPassword = settingsXml.GetElementsByTagName("password");
            XmlNodeList updateRate = settingsXml.GetElementsByTagName("update");
            email = clientEmail[0].InnerText.Trim();
            password = clientPassword[0].InnerText.Trim();
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
                Console.WriteLine("email not found");
                Console.ReadKey();
            }

        }
    }
}