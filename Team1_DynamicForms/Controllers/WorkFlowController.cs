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
            WorkFlowGroupViewModel groupModel = db.GetWorkFlows(db.GetCurrentAccount(userId));

            viewModel.Workflows.Add(groupModel);

            return View(viewModel);
        }

        // GET: Workflows/Details/5
        public ActionResult Details(int? id)
        {
            //This probly isn't needed, maybe use it later
            return View();
        }

        // GET: Workflows/Create
        //   public ActionResult Create(int formId)
        public ActionResult Create()
        {

            //Will need to later modifiy this to actually take in formId, for now just default to 1
            int formId = 1;

            WorkFlowCreateViewModel viewModel = new WorkFlowCreateViewModel();
            viewModel.partialViewModel.FormId = formId;
            viewModel.FormName = db.GetFormName(formId);
            //viewModel.UserEmails = new List<string>();
            return View(viewModel);
        }

        // POST: Workflows/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WorkFlowCreatePartialViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string userEmails = new string(viewModel.UserEmails.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());

                var workFlowId = db.CreateAndAddWorkFlow(userEmails.Split(',').ToList());
                db.AddWorkFlowToForm(1/*viewModel.FormId*/, workFlowId);
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}
