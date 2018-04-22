using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Worker1
{
    class Program
    {
        static bool IsCanceled = false;
        static int Progress { get; set; }
        static news Result { get; set; }
        static void Main(string[] args)
        {
            var connection = new HubConnection("http://localhost/Scheduler/mysignalr", useDefaultUrl: false);
            //Make proxy to hub based on hub name on server
            var myHub = connection.CreateHubProxy("TaskHub2");
            //Start connection


            connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Connected");
                    myHub.Invoke("Connect", "executer1");
                    myHub.Invoke("readyExec1");
                }

            }).Wait();

            myHub.On("startTask", async () => await function(myHub));

            myHub.On("cancel", () =>
            {
                IsCanceled = true;
            });


            Console.Read();
            Console.Read();
            connection.Stop();



        }

        public async static Task function(IHubProxy myHub)
        {
            var task = new Task(() =>
            {
                for (int i = 0; i < 101; i += 10)
                {
                    if (!IsCanceled)
                    {
                        Thread.Sleep(1000);
                        Progress = i;
                        if (i == 100)
                        {
                            string html = string.Empty;
                            string url = "https://news.yandex.ru/ru/world5.utf8.js";

                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            using (Stream stream = response.GetResponseStream())
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                html = reader.ReadToEnd();
                            }

                            string pattern = Regex.Escape("[") + ".*]";
                            MatchCollection matches = Regex.Matches(html, pattern);
                            string json = matches[0].Value;
                            myHub.Invoke<int>("ExecuteTask1", i, $"[{json}]");
                        }

                        else
                            myHub.Invoke<int>("ExecuteTask1", i, "");
                    }
                    else
                        break;
                }
                IsCanceled = false;
                myHub.Invoke("readyExec1");
            });

            task.Start();

        }
    }

    class news
    {
        public string time;
        public string date;
        public string ts;
        public string urs;
        public string title;
        public string descr;
    }
}