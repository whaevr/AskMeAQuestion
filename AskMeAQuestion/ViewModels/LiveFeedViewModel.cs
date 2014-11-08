using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskMeAQuestion.Models;

namespace AskMeAQuestion.ViewModels
{
    public class LiveFeedViewModel
    {
        public String UserId { get; set; }
        public String CourseName { get; set; }
        public String CourseDesignator { get; set; }
        public SessionViewModel CurrentSession { get; set; }
        public bool AnonOn { get; set; }
    }
}