using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using AskMeAQuestion.Models;

namespace AskMeAQuestion.ViewModels
{
    public class UserInterfaceViewModel
    {

        public String UserName { get; set; }
        public String UserRole { get; set; }
        public String UserId { get; set; }
        public String DisplayMessage { get; set; }
        public int QuestionCount { get; set; }


        public List<CourseViewModel> UserCourseList { get; set; }
        public List<CourseViewModel> PendingCourses { get; set; }
        public List<ClassListViewModel> PendingStudents { get; set; }
        public List<CourseViewModel> MyPendingCourses { get; set; }


        public List<SelectListItem> Courses { get; set; }

        public String CourseAdded { get; set; }

    }
}