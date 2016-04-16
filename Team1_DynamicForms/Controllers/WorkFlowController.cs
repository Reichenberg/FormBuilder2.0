using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Team1_DynamicForms.Models;
using Team1_DynamicForms.DataProvider;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Net.Mail;

namespace Team1_DynamicForms.Controllers
{
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    public class WorkFlowController : Controller
    {
        private FormsDataProvider db = new FormsDataProvider();

        // GET: Workflows
        public ActionResult Index()
        {
            WorkFlowOverviewViewModel viewModel = new WorkFlowOverviewViewModel();


            //Add multiple to check for different types, might move logic around later
            var userId = User.Identity.GetUserId();
            WorkFlowGroupViewModel groupModel = db.GetWorkFlows(db.GetCurrentAccount());

            viewModel.Workflows.Add(groupModel);

            return View(viewModel);
        }

        // GET: Workflows/Details/5
        public ActionResult Details(int? id)
        {
            //This probly isn't needed, maybe use it later
            return View();
        }

        // Get: Workflows/CreateIndex
        public ActionResult CreateIndex()
        {
            return View(db.GetFormNamesAndIDs(db.GetCurrentAccount()));
        }

        // POST: Workflows/CreateIndex
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateIndex(WorkFlowCreateIndexViewModel workFlowCreateIndexViewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Create", new { formId = workFlowCreateIndexViewModel.SelectedIndex });
            }
            return View(workFlowCreateIndexViewModel);
        }

        // GET: Workflows/Create
        public ActionResult Create(int formId)
        {

            WorkFlowCreateViewModel viewModel = new WorkFlowCreateViewModel();
            viewModel.FormId = formId;
            viewModel.FormName = db.GetFormName(formId);
            viewModel.MemberEmails = new List<WorkFlowCreatePartialViewModel>();
            viewModel.MemberEmails.Add(new WorkFlowCreatePartialViewModel());
            return View(viewModel);
        }

        // POST: Workflows/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WorkFlowCreateViewModel workFlowCreateViewModel)
        {
            if (ModelState.IsValid)
            {

                var workFlowId = db.CreateAndAddWorkFlow(workFlowCreateViewModel.MemberEmails.Select(m => m.Email).ToList());
                if(workFlowId < 0)
                {
                    return View(workFlowCreateViewModel);
                }
                if (db.AddWorkFlowToForm(workFlowCreateViewModel.FormId, workFlowId))
                    return RedirectToAction("Index");
                else return View(workFlowCreateViewModel);
            }
            return View();
        }

        /// <summary>
        /// Creates and returns a new Empty user item for use in workflows
        /// </summary>
        public ActionResult AddNewMember()
        {
            var member = new WorkFlowCreatePartialViewModel();
            return PartialView("CreatePartial",member);
        }

    }
}
