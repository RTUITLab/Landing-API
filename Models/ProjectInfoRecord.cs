using System;
using System.Collections.Generic;
using System.Text;

namespace Landing.API.Models
{
    public class ProjectInfoRecord
    {
        public int Id { get; set; }
        public string Repo { get; set; }
        public string Commit { get; set; }
        public DateTimeOffset CommitDate { get; set; }
        public bool IsPublic { get; set; }
        public ProjectInfo Info { get; set; } // Save as JSON
    }
}
