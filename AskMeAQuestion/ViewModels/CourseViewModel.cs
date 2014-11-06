using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskMeAQuestion.ViewModels
{
    public class CourseViewModel
    {
        public int SectionId { get; set; }
        public string CourseDesignator { get; set; }
        public string CourseName { get; set; }
        public string ProfessorId { get; set; }
        public string Status { get; set; }
        public bool OpenSession { get; set; }

        public List<SessionViewModel> Sessions { get; set; }
    }
}