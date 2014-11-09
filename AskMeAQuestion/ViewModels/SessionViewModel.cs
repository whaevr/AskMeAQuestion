using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskMeAQuestion.ViewModels
{
    public class SessionViewModel
    {
        public int SessionId { get; set; }
        public DateTime Date { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
    }
}