using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskMeAQuestion.ViewModels
{
    public class ClassListViewModel
    {
        public int CourseId { get; set; }
        public string StudentId { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string CourseDesignator { get; set; }
        public string CourseName { get; set; }
        public string UserId { get; set; }
    }
}