using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CloudProject.Models
{
    public class CloudProjectContext: DbContext
    {
        public CloudProjectContext() : base("CloudProjectContext4") { }
        public DbSet<TaskClass> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
    }
}