using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;


namespace CloudProject.Models
{
    public class TaskClass:IComparable
    {
        public Guid Id { get; set; }
        public Guid Owner { get; set; }
        public Result TaskResult { get; set; }
        public int Percentage { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Type { get; set; }

        public int CompareTo(object obj)
        {
            return this.StartDate.CompareTo((obj as TaskClass).StartDate);
        }
    }

    public enum Result
    {
        Completed = 0,
        Executing = 1,
        Canceled = 2,
        Created = 3,
    
    }

}