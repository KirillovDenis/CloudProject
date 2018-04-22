using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using CloudProject.Models;
using Microsoft.AspNet.SignalR;
using System.Web.Script.Serialization;
using System.Threading.Tasks;

namespace CloudProject.Hubs
{
    public class TaskHub : Hub
    {
        CloudProjectContext db = new CloudProjectContext();
        static List<User> Users = new List<User>();
        

        public void Cancel(string taskId)
        {
            var task = db.Tasks.Where(t => t.Id.ToString() == taskId).FirstOrDefault();
            if (task != null) {
                task.TaskResult = Result.Canceled;
                task.EndDate = DateTime.Now.ToString();
                db.SaveChanges();

                Clients.Caller.endDate(task.EndDate, taskId);
                Clients.Caller.disable("delete", false, taskId);
                Clients.Caller.disable("cancel", true, taskId);
            }
        }
        public void AddTask(string userId, int type)
        {
            var user = db.Users.Where(u => u.Id == new Guid(userId)).FirstOrDefault();
            if (user != null)
            {
                var task = new TaskClass() { Id = Guid.NewGuid(), Type = type, StartDate = DateTime.Now.ToString(), Owner = new Guid(userId), Percentage = 0, TaskResult = Result.Created };

                var json = new JavaScriptSerializer().Serialize(task);

                Clients.Caller.addTask(json);

                db.Tasks.Add(task);
                db.SaveChanges();
            }
        }

        public void DeleteTask(string id)
        {
            db.Tasks.Remove(db.Tasks.Where(t => t.Id.ToString() == id).First());
            db.SaveChanges();
            Clients.Caller.deleteTask(id);
        }

        public void UpdateTask(string id, int perc)
        {
            var task = db.Tasks.Where(t => t.Id == new Guid(id)).First();
            if (task.Percentage == 0)
            {
                task.TaskResult = Result.Executing;
            }
            else if (perc == 100)
            {
                task.TaskResult = Result.Completed;
                task.EndDate = DateTime.Now.ToString();
                Clients.Caller.endDate(task.EndDate, id);
                Clients.Caller.disable("delete", false, id);
                Clients.Caller.disable("cancel", true, id);
            }
            else if (perc == -1)
            {
                task.TaskResult = Result.Canceled;
                task.EndDate = DateTime.Now.ToString();
                Clients.Caller.endDate(task.EndDate, id);
                Clients.Caller.disable("delete", false, id);
                Clients.Caller.disable("cancel", true, id);
            }
            task.Percentage = perc;
            db.SaveChanges();
        }
    }
}