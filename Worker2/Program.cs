using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Worker2
{
    class Program
    {
        static bool IsCanceled = false;
        static int Progress { get; set; }
        
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
                    myHub.Invoke("Connect", "executer2");
                    myHub.Invoke("readyExec2");
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
                            string url = "https://habrahabr.ru/rss/interesting/";

                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            using (Stream stream = response.GetResponseStream())
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                html = reader.ReadToEnd();
                            }

                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(html);

                            XmlNode node = doc.DocumentElement.SelectSingleNode("/rss/channel/item");

                            myHub.Invoke<int>("ExecuteTask2", i, $"[{{\"title\":\"{node.ChildNodes[0].InnerText}\", \"url\": \"{node.ChildNodes[1].InnerText}\"}},{{\"title\":\"\", \"url\": \"\"}}]");
                        }
                        else
                        myHub.Invoke<int>("ExecuteTask2", i,"");
                    }
                    else
                        break;
                }
                IsCanceled = false;
                myHub.Invoke("readyExec2");
            });

            task.Start();

        }
    }
}