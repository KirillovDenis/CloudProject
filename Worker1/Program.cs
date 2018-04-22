using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worker1
{
    class Program
    {
        static bool IsCanceled = false;
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
                for (int i = 2; i < 101; i += 2)
                {
                    if (!IsCanceled)
                    {
                        Thread.Sleep(1000);
                        Progress = i;
                        myHub.Invoke<int>("ExecuteTask1", i);
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
}