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
                                  select s).First();

                    var lastQ = (from q in db.Questions
                                 select q).OrderByDescending(x => x.QuestionId).First();

                    Question question = new Question();
                    question.SessionId = session.SessionId;
                    question.Question1 = message;
                    question.Submitter = name;
                    question.Time = DateTime.Now;
                    question.QuestionId = lastQ.QuestionId + 1;
                    question.Upvotes = 0;

                    db.Questions.Add(question);
                    db.SaveChanges();

                    string nameToSend; 
                    if (session.AnonOn == true)
                    {
                        nameToSend = "Anon";
                    }
                    else
                    {
                        nameToSend = name;
                    }

                    Clients.All.broadcastMessage(nameToSend, message, question.QuestionId, 0);

                }


            }
        }

        public void Upvote(int questionId, string userId)
        {
            using (var db = new AskMeAQuestionEntities())
            {

                if (!db.Upvotes.Where(x => x.QuestionId == questionId).Any(x => x.UserId.Equals(userId)))
                {
                    var upvoted = (from q in db.Questions
                                   where q.QuestionId == questionId
                                   select q).First();

                    upvoted.Upvotes += 1;

                    string name;
                    if (upvoted.Session.AnonOn == true)
                    {
                        name = "Anon";
                    }
                    else
                    {
                        name = upvoted.Account.UserId;
                    }

                    Upvote upvote = new Upvote()
                    {
                        UserId = userId,
                        QuestionId = questionId
                    };

                    db.Upvotes.Add(upvote);
                    db.SaveChanges();

                    Clients.All.upvoteQuestion(name, upvoted.Question1, questionId, upvoted.Upvotes);
                }
            }
        }

    }
}