using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AskMeAQuestion.ViewModels
{
    public class CreateCourseViewModel
    {
        [Key]
        [Range(0, Int32.MaxValue, ErrorMessage = "*")]
        public int SectionId { get; set; }
        [Display(Name = "Professor Id")]
        [Required(ErrorMessage = "*")]
        public String ProfessorId { get; set; }
        [Display(Name = "Course Designator")]
        [Required(ErrorMessage = "*")]
        public String CourseDesignator { get; set; }
        [Display(Name = "Course Name")]
        [Required(ErrorMessage = "*")]
        public String CourseName { get; set; }

        public String UserId { get; set; }
    }
}