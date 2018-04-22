
using CloudProject.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CloudProject.Controllers
{
    public class CloudProjectController : Controller
    {
        CloudProjectContext db = new CloudProjectContext();


        [HttpGet]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("id"))
            {
                var id = (Guid)TempData["id"];
                List<TaskClass> tasks = new List<TaskClass>(db.Tasks.Where(t => t.Owner == id && t.Type == 1));
                List<string> nameBar = new List<string>(tasks.Count);
                tasks.Sort();
                foreach (var item in tasks)
                {
                    if (item.TaskResult == Result.Canceled)
                        nameBar.Add("danger");
                    else
                        nameBar.Add("success");
                }
                ViewBag.Tasks = tasks;
                ViewBag.NameBars = nameBar;

                List<TaskClass> tasks2 = new List<TaskClass>(db.Tasks.Where(t => t.Owner == id && t.Type == 2));
                List<string> nameBar2 = new List<string>(tasks2.Count);
                tasks2.Sort();
                foreach (var item in tasks2)
                {
                    if (item.TaskResult == Result.Canceled)
                        nameBar2.Add("danger");
                    else
                        nameBar2.Add("success");
                }
                ViewBag.Tasks2 = tasks2;
                ViewBag.NameBars2 = nameBar2;

                ViewBag.UserId = id;

                return View();
            }
            else
            {
                return RedirectToAction("Login","Account");
            }
        }


    }
}