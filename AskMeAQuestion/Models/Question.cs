//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AskMeAQuestion.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Question
    {
        public int QuestionId { get; set; }
        public string Question1 { get; set; }
        public string Submitter { get; set; }
        public System.DateTime Time { get; set; }
        public int SessionId { get; set; }
        public string Status { get; set; }
        public Nullable<int> Upvotes { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual Session Session { get; set; }
    }
}
