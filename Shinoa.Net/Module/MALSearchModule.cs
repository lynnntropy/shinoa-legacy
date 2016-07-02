﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Text.RegularExpressions;
using System.Xml;
using RestSharp.Authenticators;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace Shinoa.Net.Module
{
    class MALSearchModule : IModule
    {
        static RestClient RestClient = new RestClient("http://myanimelist.net/api/");

        public string DetailedStats()
        {
            return null;
        }

        public void Init()
        {
            RestClient.Authenticator = new HttpBasicAuthenticator("omegavesko", "Tekagistreasure1;");
        }

        public void MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.User.Id != ShinoaNet.DiscordClient.CurrentUser.Id)
            {
                var regex = new Regex(@"^!anime (?<querytext>.*)");
                if (regex.IsMatch(e.Message.Text))
                {
                    var queryText = regex.Matches(e.Message.Text)[0].Groups["querytext"];

                    Logging.Log($"[{e.Server.Name} -> #{e.Channel.Name}] {e.User.Name} searched MAL for '{queryText}'.");

                    var request = new RestRequest($"anime/search.xml?q={queryText}");

                    IRestResponse response = null;
                    while (response == null)
                    {
                        response = RestClient.Execute(request);
                    }

                    try
                    {

                        XElement root = XElement.Parse(response.Content);

                        var firstResult = (from el in root.Descendants("entry") select el).First();
                        // Console.WriteLine(firstResult.ToString());

                        var responseMessage = "";
                        responseMessage += $"Title: **{firstResult.Descendants("title").First().Value}**\n";
                        responseMessage += $"English title: **{firstResult.Descendants("english").First().Value}**\n";
                        responseMessage += $"Synonyms: {firstResult.Descendants("synonyms").First().Value}\n\n";

                        responseMessage += $"Type: {firstResult.Descendants("type").First().Value}\n";
                        responseMessage += $"Status: {firstResult.Descendants("status").First().Value}\n";
                        responseMessage += $"Average score (0-10): {firstResult.Descendants("score").First().Value}\n";
                        responseMessage += $"Episode count: {firstResult.Descendants("episodes").First().Value}\n";

                        responseMessage += $"Aired: {firstResult.Descendants("start_date").First().Value} -> {firstResult.Descendants("end_date").First().Value}\n";

                        responseMessage += $"\nhttp://myanimelist.net/anime/{firstResult.Descendants("id").First().Value}";

                        e.Channel.SendMessage(responseMessage);

                        //WebClient webclient = new WebClient();
                        //string animeId = firstResult.Descendants("image").First().Value;

                        //webclient.DownloadFile($"{animeId}", $"{Path.GetTempPath()}anime_cover_{animeId}.jpg");
                        //e.Channel.SendFile($"{Path.GetTempPath()}anime_cover_{animeId}.jpg");
                    }
                    catch (Exception ex)
                    {
                        e.Channel.SendMessage("Anime not found.");
                        Logging.Log(ex.ToString());
                    }
                }
            }
        }
    }
}
