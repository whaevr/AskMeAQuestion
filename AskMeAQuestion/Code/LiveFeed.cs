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

                    Question question = new Question();
                    question.SessionId = session.SessionId;
                    question.Question1 = message;
                    question.Submitter = name;
                    question.Time = DateTime.Now;
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

                    string time = question.Time.ToShortTimeString();

                    Clients.All.broadcastMessage(nameToSend, message, time, question.QuestionId, 0);

                }


            }
        }

        public void SendReply(string name, string message, string designator, int questionId)
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

                    Response r = new Response()
                    {
                        QuestionId = questionId,
                        Response1 = message,
                        Submitter = name,
                        Time = DateTime.Now
                    };

                    db.Responses.Add(r);
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

                    string time = r.Time.ToShortTimeString();

                    Clients.All.submitReply(nameToSend, message, time, questionId);

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

                    Upvote upvote = new Upvote()
                    {
                        UserId = userId,
                        QuestionId = questionId
                    };

                    db.Upvotes.Add(upvote);
                    db.SaveChanges();

                    Clients.All.upvoteQuestion(questionId, upvoted.Upvotes);
                }
            }
        }

    }
}