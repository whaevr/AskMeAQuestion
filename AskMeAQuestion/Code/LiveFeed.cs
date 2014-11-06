using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using AskMeAQuestion.Models;


namespace AskMeAQuestion.Code
{
    public class LiveFeed : Hub
    {
        public static List<string> OnlineUsers = new List<string>();

        public void Send(string name, string message, string designator)
        {
            // Call the broadcastMessage method to update clients.

            if (message != "")
            {


                using (var db = new AskMeAQuestionEntities())
                {
                    var session = (from s in db.Sessions
                                  where s.Course.CourseDesignator == designator &&
                                  s.Date == DateTime.Today
                                  select s.SessionId).First();

                    var lastQ = (from q in db.Questions
                                 select q).OrderByDescending(x => x.QuestionId).First();

                    Question question = new Question();
                    question.SessionId = session;
                    question.Question1 = message;
                    question.Submitter = name;
                    question.Time = DateTime.Now;
                    question.QuestionId = lastQ.QuestionId + 1;
                    question.Upvotes = 0;

                    db.Questions.Add(question);
                    db.SaveChanges();

                    Clients.All.broadcastMessage(name, message, question.QuestionId, 0);

                }


            }
        }

        public void Upvote(int questionId)
        {
            using (var db = new AskMeAQuestionEntities())
            {
                var upvoted = (from q in db.Questions
                               where q.QuestionId == questionId 
                                select q).First();

                upvoted.Upvotes += 1;
                db.SaveChanges();

                Clients.All.upvoteQuestion(upvoted.Account.UserId, upvoted.Question1, questionId, upvoted.Upvotes);

            }
        }

    }
}