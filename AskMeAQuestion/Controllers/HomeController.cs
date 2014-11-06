using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskMeAQuestion.ViewModels;
using AskMeAQuestion.Models;

namespace AskMeAQuestion.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(IndexViewModel loginInfo) 
        {

            if (ModelState.IsValid)
            {
                using (var database = new AskMeAQuestionEntities())
                {
                    var user = from d in database.Accounts
                               where d.UserId == loginInfo.UserName &&
                               d.Password == loginInfo.Password
                               select d.UserId;

                    if (user.Count() != 0)
                    {
                        return RedirectToAction("UserInterface", "Home", new { userId = user.First() });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Username and/or Password is incorrect";
                        return View(loginInfo);
                    }
                }
            }
            return View(loginInfo);
        }

        public ActionResult Register()
        {

            List<SelectListItem> accountTypeList = new List<SelectListItem>();
            accountTypeList.Add(new SelectListItem() { Text = "Student", Value = "1"});
            accountTypeList.Add(new SelectListItem() { Text = "Professor", Value = "2" });

            RegisterViewModel viewModel = new RegisterViewModel();
            viewModel.AccountTypeList = accountTypeList; 

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel registrationInfo)
        {

            List<SelectListItem> accountTypeList = new List<SelectListItem>();
            accountTypeList.Add(new SelectListItem() { Text = "Student", Value = "1" });
            accountTypeList.Add(new SelectListItem() { Text = "Professor", Value = "2" });

            registrationInfo.AccountTypeList = accountTypeList; 


            if (ModelState.IsValid)
            {
                using (var database = new AskMeAQuestionEntities())
                {
                    var user = from d in database.Accounts
                                where d.UserId == registrationInfo.UserName
                                select d;


                    if (user.Count() == 0)
                    {
                        Account registeredAccount = new Account();
                        registeredAccount.FirstName = registrationInfo.FirstName;
                        registeredAccount.LastName = registrationInfo.LastName;
                        registeredAccount.UserId = registrationInfo.UserName;
                        registeredAccount.Password = registrationInfo.Password;
                        registeredAccount.Role = registrationInfo.AccountType;
                        registeredAccount.Status = "pending";

                        database.Accounts.Add(registeredAccount);
                        database.SaveChanges();
                        return RedirectToAction("UserInterface", "Home", new { userId = registrationInfo.UserName });

                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Username already exists. Please choose another.";
                        return View(registrationInfo);
                    }


                }
            }


            return View(registrationInfo);
        }
        public ActionResult UserInterface(String userId)
        {
            using (var db = new AskMeAQuestionEntities())
            {

                UserInterfaceViewModel vm = new UserInterfaceViewModel();

                var user = (from u in db.Accounts
                            where u.UserId == userId
                            select u).First();

                vm.UserName = user.FirstName;
                vm.UserRole = user.Role;
                vm.UserId = user.UserId;

                List<int> courseIds = new List<int>();
                List<CourseViewModel> userClasses = new List<CourseViewModel>();

                if (user.ClassLists != null)
                {
                    foreach (var course in user.ClassLists) {

                        if(course.Status == "Active" || course.Status == "Professor") {
                            CourseViewModel addCourse = new CourseViewModel();
                            addCourse.SectionId = course.CourseId;
                            addCourse.CourseDesignator = course.Course.CourseDesignator;
                            addCourse.CourseName = course.Course.CourseName;
                            addCourse.ProfessorId = course.Course.ProfessorId;

                            var currentSession = from c in db.Sessions
                                                 where c.CourseId == course.CourseId &&
                                                 c.Date == DateTime.Today
                                                 select c;

                            if (currentSession.Count() != 0)
                            {
                                addCourse.OpenSession = true;

                            }
                            else
                            {
                                addCourse.OpenSession = false;

                            }

                            userClasses.Add(addCourse);
                        }

                    }

                    courseIds = user.ClassLists.Select(x => x.CourseId).ToList();
                }

                vm.UserCourseList = userClasses;


                var courses = from c in db.Courses
                              where c.Status == "Active"
                              select c;

                if (courses != null)
                {
                    List<SelectListItem> courseList = new List<SelectListItem>();
                    foreach (var course in courses)
                    {
                        if (courseIds == null)
                        {
                            courseList.Add(new SelectListItem { Text = course.CourseDesignator, Value = course.SectionId.ToString() });
                        }
                        else if (!courseIds.Contains(course.SectionId))
                        {
                            courseList.Add(new SelectListItem { Text = course.CourseDesignator, Value = course.SectionId.ToString() });
                        }
                    }

                    vm.Courses = courseList;
                }


                    var pendingCourses = from p in db.Courses
                                         where p.Status == "pending"
                                         select p;

                    List<CourseViewModel> pCourses = new List<CourseViewModel>();
                    if (pendingCourses != null && user.Role == "Admin")
                    {
                        foreach (var course in pendingCourses)
                        {
                            CourseViewModel c = new CourseViewModel();
                            c.SectionId = course.SectionId;
                            c.CourseDesignator = course.CourseDesignator;
                            c.CourseName = course.CourseName;
                            c.ProfessorId = course.ProfessorId;

                            pCourses.Add(c);
                        }

                       
                    }

                    vm.PendingCourses = pCourses;

                    var pendingStudents = from p in db.ClassLists
                                          where p.Status == "pending"
                                          select p;

                    if (user.Role == "2")
                    {
                        pendingStudents = pendingStudents.Where(x => x.Course.ProfessorId == user.UserId);
                    }

                    List<ClassListViewModel> pStudents = new List<ClassListViewModel>();
                    if (pendingStudents != null)
                    {
                        foreach (var student in pendingStudents)
                        {
                            ClassListViewModel c = new ClassListViewModel();
                            c.CourseDesignator = student.Course.CourseDesignator;
                            c.CourseId = student.CourseId;
                            c.CourseName = student.Course.CourseName;
                            c.Name = student.Account.FirstName;
                            c.Status = student.Status;
                            c.StudentId = student.StudentId;


                            pStudents.Add(c);
                        }
                    }
                    vm.PendingStudents = pStudents;

                    var myPendingCourses = from p in db.ClassLists
                                           where p.Status == "pending" &&
                                           p.StudentId == user.UserId
                                           select p;
                    List<CourseViewModel> mPendingStudents = new List<CourseViewModel>();
                    if (myPendingCourses != null)
                    {
                        foreach (var course in myPendingCourses)
                        {
                            CourseViewModel c = new CourseViewModel();
                            c.CourseDesignator = course.Course.CourseDesignator;
                            c.CourseName = course.Course.CourseName;
                            c.ProfessorId = course.Course.ProfessorId;

                            mPendingStudents.Add(c);
                        }

                    }
                    vm.MyPendingCourses = mPendingStudents;


                return View(vm);

            }
        }

        [HttpPost]
        public ActionResult UserInterface(UserInterfaceViewModel vm)
        {

            using (var db = new AskMeAQuestionEntities())
            {

                 var user = (from u in db.Accounts
                 where u.UserId == vm.UserId
                 select u).First();

                if (vm.CourseAdded != null)
                {

                    try
                    {
                        user.ClassLists.Add(new ClassList
                        {
                            CourseId = Int32.Parse(vm.CourseAdded),
                            StudentId = user.UserId,
                            Status = "pending"
                        });


                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }

                }

                vm.UserName = user.FirstName;
                vm.UserRole = user.Role;
                vm.UserId = user.UserId;

                List<int> courseIds = new List<int>();
                List<CourseViewModel> userClasses = new List<CourseViewModel>();

                if (user.ClassLists != null)
                {
                    foreach (var course in user.ClassLists)
                    {

                        if (course.Status == "Active" || course.Status == "Professor")
                        {
                            CourseViewModel addCourse = new CourseViewModel();
                            addCourse.SectionId = course.CourseId;
                            addCourse.CourseDesignator = course.Course.CourseDesignator;
                            addCourse.CourseName = course.Course.CourseName;
                            addCourse.ProfessorId = course.Course.ProfessorId;

                            var currentSession = from c in db.Sessions
                                                 where c.CourseId == course.CourseId &&
                                                 c.Date == DateTime.Today
                                                 select c;

                            if (currentSession != null)
                            {
                                addCourse.OpenSession = true;

                            }
                            else
                            {
                                addCourse.OpenSession = false;

                            }

                            userClasses.Add(addCourse);
                        }

                    }
                    courseIds = user.ClassLists.Select(x => x.CourseId).ToList();
                }

                vm.UserCourseList = userClasses;


                    var courses = from c in db.Courses
                                  where c.Status == "Active"
                                  select c;
                    if (courses != null)
                    {
                        List<SelectListItem> courseList = new List<SelectListItem>();
                        foreach (var course in courses)
                        {
                            if (courseIds == null)
                            {
                                courseList.Add(new SelectListItem { Text = course.CourseDesignator, Value = course.SectionId.ToString() });
                            }
                            else if (!courseIds.Contains(course.SectionId))
                            {
                                courseList.Add(new SelectListItem { Text = course.CourseDesignator, Value = course.SectionId.ToString() });
                            }
                        }

                        vm.Courses = courseList;
                    }


                    var pendingCourses = from p in db.Courses
                                         where p.Status == "pending"
                                         select p;
                    List<CourseViewModel> pCourses = new List<CourseViewModel>();
                    if (pendingCourses != null)
                    {
                        foreach (var course in pendingCourses)
                        {
                            CourseViewModel c = new CourseViewModel();
                            c.SectionId = course.SectionId;
                            c.CourseDesignator = course.CourseDesignator;
                            c.CourseName = course.CourseName;
                            c.ProfessorId = course.ProfessorId;

                            pCourses.Add(c);
                        }

                       
                    }

                    vm.PendingCourses = pCourses;

                    var pendingStudents = from p in db.ClassLists
                                          where p.Status == "pending"
                                          select p;

                    List<ClassListViewModel> pStudents = new List<ClassListViewModel>();
                    if (pendingStudents != null)
                    {
                        foreach (var student in pendingStudents)
                        {
                            ClassListViewModel c = new ClassListViewModel();
                            c.CourseDesignator = student.Course.CourseDesignator;
                            c.CourseId = student.CourseId;
                            c.CourseName = student.Course.CourseName;
                            c.Name = student.Account.FirstName;
                            c.Status = student.Status;
                            c.StudentId = student.StudentId;


                            pStudents.Add(c);
                        }
                    }
                    vm.PendingStudents = pStudents;

                    var myPendingCourses = from p in db.ClassLists
                                           where p.Status == "pending" &&
                                           p.StudentId == user.UserId
                                           select p;
                    List<CourseViewModel> mPendingStudents = new List<CourseViewModel>();
                    if (myPendingCourses != null)
                    {
                        foreach (var course in myPendingCourses)
                        {
                            CourseViewModel c = new CourseViewModel();
                            c.CourseDesignator = course.Course.CourseDesignator;
                            c.CourseName = course.Course.CourseName;
                            c.ProfessorId = course.Course.ProfessorId;

                            mPendingStudents.Add(c);
                        }

                    }
                    vm.MyPendingCourses = mPendingStudents;



            }

            return View(vm);
        }

        public ActionResult CreateCourse(String id)
        {
            CreateCourseViewModel vm = new CreateCourseViewModel();

            vm.ProfessorId = id;
            vm.UserId = id;

            return View(vm);
        }

        [HttpPost]
        public ActionResult CreateCourse(CreateCourseViewModel vm)
        {


            if (ModelState.IsValid)
            {
                using (var db = new AskMeAQuestionEntities())
                {

                    var professor = from p in db.Accounts
                                    where p.UserId == vm.ProfessorId
                                    && p.Role == "2"
                                    select p;

                    if (professor.Count() > 0)
                    {
                        Course newCourse = new Course();
                        newCourse.SectionId = vm.SectionId;
                        newCourse.ProfessorId = vm.ProfessorId;
                        newCourse.CourseName = vm.CourseName;
                        newCourse.CourseDesignator = vm.CourseDesignator;
                        newCourse.Status = "pending";

                        db.Courses.Add(newCourse);

                        ClassList addProf = new ClassList();
                        addProf.StudentId = vm.ProfessorId;
                        addProf.CourseId = vm.SectionId;
                        addProf.Status = "Professor";
                        db.ClassLists.Add(addProf);

                        db.SaveChanges();

                        
                        return RedirectToAction("UserInterface", "Home", new { userId = vm.UserId });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "User inputted is not a registered Professor.";
                        return View(vm);
                    }

                }
            }


            return View(vm);
        }

        public ActionResult LiveFeed(int id, string userId)
        {
            LiveFeedViewModel vm = new LiveFeedViewModel();

            using (var db = new AskMeAQuestionEntities())
            {
                var course = (from c in db.Courses
                              where c.SectionId == id
                              select c).First();

                var user = (from u in db.Accounts
                            where u.UserId == userId
                            select u).First();

                vm.CourseName = course.CourseName;
                vm.CourseDesignator = course.CourseDesignator;
                vm.UserId = user.UserId;
                
            }
            return View(vm);
        }

        public ActionResult AcceptCourse(int id, string userId)
        {

            using (var db = new AskMeAQuestionEntities()) {
                var acceptedCourse = (from a in db.Courses
                                     where a.SectionId == id
                                     select a).First();

                acceptedCourse.Status = "Active";

                db.SaveChanges();
            }
            return RedirectToAction("UserInterface", "Home", new { userId = userId });
        }

        public ActionResult DenyCourse(int id, string userId)
        {
            using (var db = new AskMeAQuestionEntities())
            {
                var acceptedCourse = (from a in db.Courses
                                      where a.SectionId == id
                                      select a).First();

                db.Courses.Remove(acceptedCourse);

                db.SaveChanges();
            }
            return RedirectToAction("UserInterface", "Home", new { userId = userId });
        }

        public ActionResult AcceptStudent(int id, string student, string userId)
        {
            using (var db = new AskMeAQuestionEntities())
            {
                var acceptedStudent = (from a in db.ClassLists
                                       where a.CourseId == id
                                       && a.StudentId == student
                                       select a).First();

                acceptedStudent.Status = "Active";

                db.SaveChanges();

            }
            return RedirectToAction("UserInterface", "Home", new { userId = userId });
        }

        public ActionResult DenyStudent(int id, string student, string userId)
        {
            using (var db = new AskMeAQuestionEntities())
            {
                var acceptedStudent = (from a in db.ClassLists
                                       where a.CourseId == id
                                       && a.StudentId == student
                                       select a).First();

                db.ClassLists.Remove(acceptedStudent);
                db.SaveChanges();

            }
            return RedirectToAction("UserInterface", "Home", new { userId = userId });
        }

        public ActionResult ClassList(int courseId)
        {
            List<ClassListViewModel> vm = new List<ClassListViewModel>();
            using (var db = new AskMeAQuestionEntities())
            {
                var classList = from c in db.ClassLists
                                where c.CourseId == courseId
                                select c;

                foreach (var student in classList)
                {
                    ClassListViewModel addStudent = new ClassListViewModel();
                    addStudent.CourseDesignator = student.Course.CourseDesignator;
                    addStudent.CourseId = student.CourseId;
                    addStudent.CourseName = student.Course.CourseName;
                    addStudent.Name = student.Account.FirstName;
                    addStudent.StudentId = student.StudentId;
                    addStudent.Status = student.Status;

                    vm.Add(addStudent);
                }


            }
            vm.OrderBy(x => x.Name);

            return View(vm);
        }

        [HttpGet]
        public JsonResult RemoveUser(int courseId, string studentId)
        {

            using (var db = new AskMeAQuestionEntities())
            {
                var deletedStudent = (from a in db.ClassLists
                                       where a.CourseId == courseId
                                       && a.StudentId == studentId
                                       select a).First();

                db.ClassLists.Remove(deletedStudent);
                db.SaveChanges();

            }
            return Json(new { success = true}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateSession(int id, string userId)
        {

            using (var db = new AskMeAQuestionEntities())
            {

                var lastSessionId = (from l in db.Sessions
                                     select l).OrderByDescending(x => x.SessionId).First();

                Models.Session newSession = new Session();

                newSession.SessionId = lastSessionId.SessionId + 1;
                newSession.CourseId = id;
                newSession.Date = DateTime.Today;

                db.Sessions.Add(newSession);
                db.SaveChanges();
            }
            return RedirectToAction("LiveFeed", "Home", new { id = id, userId = userId });
        }

        public ActionResult CourseHistory(int id)
        {

            CourseViewModel vm = new CourseViewModel();

            using (var db = new AskMeAQuestionEntities())
            {
                var course = (from c in db.Courses
                              where c.SectionId == id
                              select c).First();

                vm.CourseDesignator = course.CourseDesignator;
                vm.CourseName = course.CourseName;
                vm.ProfessorId = course.ProfessorId;

                List<SessionViewModel> sessions = new List<SessionViewModel>();
                foreach (var session in course.Sessions)
                {
                    SessionViewModel newS = new SessionViewModel();

                    newS.Date = session.Date;
                    newS.SessionId = session.SessionId;

                    List<string> questions = new List<string>();
                    foreach (var question in session.Questions)
                    {
                        questions.Add(question.Question1);
                    }

                    newS.Questions = questions;

                    sessions.Add(newS);
                }

                vm.Sessions = sessions;
            }
            return View(vm);
        }

    }

    
}
