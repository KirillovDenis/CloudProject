using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CloudProject.Models;
using Microsoft.AspNet.SignalR;
using System.Web.Script.Serialization;

namespace Scheduler.Hubs
{
    public class TaskHub2 : Hub
    {
        static List<Client> Users = new List<Client>();
        static List<task> Queue1 = new List<task>();
        static List<task> Queue2 = new List<task>();
        static List<task> Tasks = new List<task>();

        public void Cancel(string taskId, int type)
        {

            var task = Tasks.Where(t => t.Id.ToString() == taskId).FirstOrDefault();
            if (task != null)
            {
                task.TaskResult = Result.Canceled;
            }
            if (type == 1)
            {
                var task2 = Queue1.Where(t => t.Id.ToString() == taskId).FirstOrDefault();
                if (task2 != null)
                    task2.TaskResult = Result.Canceled;
            }
            else
            {
                var task2 = Queue2.Where(t => t.Id.ToString() == taskId).FirstOrDefault();
                if (task2 != null)
                    task2.TaskResult = Result.Canceled;
            }

            Clients.Caller.cancel(taskId);
        }

        public void StartTask(int numTask, string userId, string taskId)
        {
            if (numTask == 1)
            {
                var task = new task { Type = 1, Owner = new Guid(userId), Id = new Guid(taskId), TaskResult = Result.Created, Percentage = 0, connIdUser = Context.ConnectionId };

                Queue1.Add(task);
                var us = Users.Where(u => u.UserId == userId).FirstOrDefault();
                us.Queue.Task1Count++;
            }
            else
            {
                var task = new task { Type = 2, Owner = new Guid(userId), Id = new Guid(taskId), TaskResult = Result.Created, Percentage = 0, connIdUser = Context.ConnectionId };

                Queue2.Add(task);
                var us = Users.Where(u => u.UserId == userId).FirstOrDefault();
                us.Queue.Task2Count++;
            }
        }


        public void readyExec1()
        {
            while (Queue1.Count == 0)
            {
                Thread.Sleep(1000);
            }

            Tasks.Add(Queue1.First());
            Clients.Client(Users.Where(u => u.UserId == "executer1").First().ConnectionId).startTask();
        }
        public void readyExec2()
        {
            while (Queue2.Count == 0)
            {
                Thread.Sleep(1000);
            }

            Tasks.Add(Queue2.First());
            Clients.Client(Users.Where(u => u.UserId == "executer2").First().ConnectionId).startTask();
        }

        public void ExecuteTask1(int perc)
        {
            if (Queue1.Count > 0)
            {
                var task = Queue1[0];
                string state = "";
                if (task.TaskResult != Result.Canceled)
                {
                    var task2 = Tasks.Where(t => t.Id.ToString() == task.Id.ToString()).FirstOrDefault();
                    var user = Users.Where(u => u.UserId == task.Owner.ToString()).FirstOrDefault();
                    if (task2 != null)
                    {
                        if (task2.Percentage == 0)
                        {
                            task2.TaskResult = Result.Executing;
                            state = "Executing";
                        }
                        if (perc != -1)
                        {
                            task2.Percentage = perc;

                            if (perc == 100)
                            {
                                task2.TaskResult = Result.Completed;
                                state = "Completed";
                                if (Users.Any(u => u.UserId == task2.Owner.ToString()))
                                    Tasks.Remove(Queue1[0]);
                                Queue1.RemoveAt(0);
                            }
                            if (user != null)
                                Clients.Client(user.ConnectionId).increase(task.Id.ToString(), perc, state);
                        }
                        else
                        {
                            task2.TaskResult = Result.Canceled;
                            state = "Canceled";
                            if (user != null)
                                Clients.Client(user.ConnectionId).cancel(task.Id.ToString());
                        }
                    }
                }
                else
                {
                    Clients.Client(Users.Where(u => u.UserId == "executer1").First().ConnectionId).cancel();
                    Tasks.Remove(Queue1[0]);
                    Queue1.RemoveAt(0);

                }
            }
        }

        public void ExecuteTask2(int perc)
        {
            if (Queue2.Count > 0)
            {
                var task = Queue2[0];
                string state = "";
                if (task.TaskResult != Result.Canceled)
                {
                    var task2 = Tasks.Where(t => t.Id.ToString() == task.Id.ToString()).FirstOrDefault();
                    var user = Users.Where(u => u.UserId == task.Owner.ToString()).FirstOrDefault();
                    if (task2 != null)
                    {
                        if (task2.Percentage == 0)
                        {
                            task2.TaskResult = Result.Executing;
                            state = "Executing";
                        }
                        if (perc != -1)
                        {
                            task2.Percentage = perc;

                            if (perc == 100)
                            {
                                task2.TaskResult = Result.Completed;
                                state = "Completed";
                                if (Users.Any(u => u.UserId == task2.Owner.ToString()))
                                    Tasks.Remove(Queue2[0]);
                                Queue2.RemoveAt(0);
                            }
                            if (user != null)
                                Clients.Client(user.ConnectionId).increase(task.Id.ToString(), perc, state);
                        }
                        else
                        {
                            task2.TaskResult = Result.Canceled;
                            state = "Canceled";
                            if (user != null)
                                Clients.Client(user.ConnectionId).cancel(task.Id.ToString());
                        }
                    }
                }
                else
                {
                    Clients.Client(Users.Where(u => u.UserId == "executer2").First().ConnectionId).cancel();
                    Tasks.Remove(Queue2[0]);
                    Queue2.RemoveAt(0);

                }
            }
        }

        
        public void Connect(string userId)
        {
            var id = Context.ConnectionId;

            if (!Users.Any(x => x.UserId == userId))
            {
                Users.Add(new Client { ConnectionId = id, UserId = userId, Queue = new Query { Task1Count = 0, Task2Count = 0 } });
            }

            foreach (var task in Tasks.Where(t => t.Owner.ToString() == userId))
            {
                Clients.Caller.increase(task.Id.ToString(), task.Percentage, task.TaskResult.ToString());
            }
            Tasks.RemoveAll(t => t.Owner.ToString() == userId && t.TaskResult == Result.Canceled || t.TaskResult == Result.Completed);
        }

        // Отключение пользователя
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var user = Users.FirstOrDefault(x => x.ConnectionId.ToString() == Context.ConnectionId);
            if (user != null)
            {
                Users.Remove(user);
                if (user.UserId == "executer1")
                {
                    foreach (var task in Tasks)
                    {
                        if (task.TaskResult != Result.Completed && task.Type == 1)
                        {
                            task.TaskResult = Result.Canceled;
                        }
                    }
                }
                else if (user.UserId == "executer2")
                {
                    foreach (var task in Tasks)
                    {
                        if (task.TaskResult != Result.Completed && task.Type == 2)
                        {
                            task.TaskResult = Result.Canceled;
                        }
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }
    }


    class Client
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public Query Queue { get; set; }
    }
    class Query
    {
        public int Task1Count { get; set; }
        public int Task2Count { get; set; }
    }

    class task : TaskClass
    {
        public string connIdUser;
    }

}