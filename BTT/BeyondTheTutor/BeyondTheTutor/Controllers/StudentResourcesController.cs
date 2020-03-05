﻿using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BeyondTheTutor.DAL;
using BeyondTheTutor.Models;
using Microsoft.AspNet.Identity;

namespace BeyondTheTutor.Controllers
{
    public class StudentResourcesController : Controller
    {
        private BeyondTheTutorContext db = new BeyondTheTutorContext();

        // GET: StudentResources
        public ActionResult Index()
        {
            ViewBag.Current = "StuResIndex";
            var studentResources = db.StudentResources.Include(s => s.BTTUser).OrderBy(s => s.Topic);
            return View(studentResources.ToList());
        }

        public ActionResult ManageResources()
        {
            ViewBag.Current = "StuResManage";
            var userID = User.Identity.GetUserId();
            var currentUserID = db.BTTUsers.Where(m => m.ASPNetIdentityID.Equals(userID)).FirstOrDefault().ID;
            ViewBag.currentUser = db.BTTUsers.Where(m => m.ASPNetIdentityID.Equals(userID)).FirstOrDefault().FirstName;
            var resourceList = db.StudentResources.Where(m => m.UserID == currentUserID).OrderBy(m => m.Topic);
            return View(resourceList.ToList());
        }

        public ActionResult ResourceSuccess()
        {
            var userID = User.Identity.GetUserId();
            var currentUserID = db.BTTUsers.Where(m => m.ASPNetIdentityID.Equals(userID)).FirstOrDefault().ID;
            var resourceList = db.StudentResources.Where(m => m.UserID == currentUserID).OrderBy(m => m.Topic);
            return View(resourceList.ToList());
        }

        // GET: StudentResources/Create
        public ActionResult Create()
        {
            ViewBag.Current = "StuResCreate";
            var userID = User.Identity.GetUserId();
            ViewBag.CurrentUserID = db.BTTUsers.Where(m => m.ASPNetIdentityID.Equals(userID)).FirstOrDefault().ID;
            return View();
        }

        // POST: StudentResources/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Topic,URL,DisplayText,UserID")] StudentResource studentResource)
        {
            if (ModelState.IsValid)
            {
                db.StudentResources.Add(studentResource);
                db.SaveChanges();
                return RedirectToAction("ResourceSuccess");
            }

            ViewBag.UserID = new SelectList(db.BTTUsers, "ID", "FirstName", studentResource.UserID);
            return View(studentResource);
        }

        // GET: StudentResources/Edit/5
        public ActionResult Edit(int? id)
        {
            var userID = User.Identity.GetUserId();
            ViewBag.CurrentUserID = db.BTTUsers.Where(m => m.ASPNetIdentityID.Equals(userID)).FirstOrDefault().ID;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentResource studentResource = db.StudentResources.Find(id);
            if (studentResource == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.BTTUsers, "ID", "FirstName", studentResource.UserID);
            return View(studentResource);
        }

        // POST: StudentResources/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Topic,URL,DisplayText,UserID")] StudentResource studentResource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentResource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManageResources");
            }
            ViewBag.UserID = new SelectList(db.BTTUsers, "ID", "FirstName", studentResource.UserID);
            return View(studentResource);
        }

        // GET: StudentResources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentResource studentResource = db.StudentResources.Find(id);
            if (studentResource == null)
            {
                return HttpNotFound();
            }
            return View(studentResource);
        }

        // POST: StudentResources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentResource studentResource = db.StudentResources.Find(id);
            db.StudentResources.Remove(studentResource);
            db.SaveChanges();
            return RedirectToAction("ManageResources");
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
