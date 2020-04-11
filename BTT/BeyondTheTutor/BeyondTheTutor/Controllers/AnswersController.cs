﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BeyondTheTutor.DAL;
using BeyondTheTutor.Models.SurveyModels;
using Microsoft.AspNet.Identity;

namespace BeyondTheTutor.Controllers
{
    [Authorize]

    public class AnswersController : Controller
    {
        private BeyondTheTutorContext db = new BeyondTheTutorContext();

        // GET: Answers
        public ActionResult Index()
        {
            var answers = db.Answers.Include(a => a.Question).Include(a => a.Survey);
            return View(answers.ToList());
        }

        // GET: Answers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // GET: Answers/Create
        public ActionResult Create(int? qid, int? sid)
        {
            var userID = User.Identity.GetUserId();
            var currentUser = db.BTTUsers.Where(m => m.ASPNetIdentityID.Equals(userID)).FirstOrDefault().ID;

            var listOfQuestions = db.Surveys.Find(sid).Questions.ToList();
            var listOfAnswers = db.Surveys.Find(sid).Answers.Where(a => a.UserID == currentUser).ToList();


            ViewBag.question = db.Questions.Find(qid).AskingQuestion;
            ViewBag.name = db.Surveys.Find(sid).Name;
            ViewBag.description = db.Surveys.Find(sid).Description;
            ViewBag.QID = qid;
            ViewBag.SID = sid;
            ViewBag.UID = currentUser;

            ViewBag.QuestionsAnswered = listOfAnswers.Count() + " out of " + listOfQuestions.Count() + " questions answered.";

            if (listOfQuestions.Count() == listOfAnswers.Count() + 1)
            {
                ViewBag.ButtonText = "Submit Survey";
            }
            else
            {
                ViewBag.ButtonText = "Next Question";
            }

            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,SurveyID,QuestionID,UserAnswer")] Answer answer)
        {
            
            var userID = User.Identity.GetUserId();
            var currentUser = db.BTTUsers.Where(m => m.ASPNetIdentityID.Equals(userID)).FirstOrDefault().ID;

            var listOfQuestions = db.Surveys.Find(answer.SurveyID).Questions.ToList();
            var listOfAnswers = db.Surveys.Find(answer.SurveyID).Answers.Where(a => a.UserID == currentUser).ToList();

            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();

                //listOfQuestions.Where(m => m.ID == answer.Question
                //var j = listOfQuestions.FindIndex(m => m.ID == answer.QuestionID);
                //var nextQuestion = listOfQuestions.LastOrDefault().ID;
                int QID = 0;
                foreach (var q in listOfQuestions)
                {
                    if (q.Answers.Count() < 1)
                    {
                        //var nextQuestion = listOfQuestions.FindIndex(m => m.ID == q.QuestionID);
                        QID = q.ID;
                    }
                }

                if (listOfQuestions.Count() == listOfAnswers.Count() + 1)
                {
                    TempData["thankyou"] = answer.SurveyID;
                    return RedirectToAction("TutoringAppts", "student", null);
                }
                else
                {
                    return RedirectToAction("Create", new { qid = QID, sid = answer.SurveyID });
                }
            }

            ViewBag.QuestionID = new SelectList(db.Questions, "ID", "AskingQuestion", answer.QuestionID);
            ViewBag.SurveyID = new SelectList(db.Surveys, "ID", "Name", answer.SurveyID);
            return View(answer);
        }

        // GET: Answers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuestionID = new SelectList(db.Questions, "ID", "AskingQuestion", answer.QuestionID);
            ViewBag.SurveyID = new SelectList(db.Surveys, "ID", "Name", answer.SurveyID);
            return View(answer);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,SurveyID,QuestionID,UserAnswer")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QuestionID = new SelectList(db.Questions, "ID", "AskingQuestion", answer.QuestionID);
            ViewBag.SurveyID = new SelectList(db.Surveys, "ID", "Name", answer.SurveyID);
            return View(answer);
        }

        // GET: Answers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Answer answer = db.Answers.Find(id);
            db.Answers.Remove(answer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
