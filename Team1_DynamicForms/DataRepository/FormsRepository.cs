//----------------------------------------------------
//File Name: 			        FormsRepository.cc
//Project Name: 		        Team1_DynamicForms
//----------------------------------------------------
//Creator's Name and Email: 	Chance Reichenberg	Reichenberg@goldmail.etsu.edu
//Creation Date:		        2/4/2016
//----------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Team1_DynamicForms.Models;

/// <summary>
/// Handles communication with database.
/// </summary>
namespace Team1_DynamicForms.DataRepository
{
    public class FormsRepository
    {
        //Connection to database
        private FormsDbAzureConnection db = new FormsDbAzureConnection();
        private bool disposed = false;


        /// <summary>
        /// Dispose Method for Database
        /// </summary>
        /// <param name="disposing">Confirm Disposing of database</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Dispose Method for Garbage Collector to remove Database connection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool AddAccountToDb(string name, string type, string userId)
        {
            int maxAccountId = db.Accounts.Max(a => a.Id);
            if (maxAccountId < 1)
            {
                maxAccountId = 1;
            }
            else
            {
                maxAccountId++;
            }

            Account newAccount = new Account();

            newAccount.Id = maxAccountId;
            newAccount.Name = name;
            newAccount.Type = type;
            newAccount.UserId = userId;

            db.Accounts.Add(newAccount);
            db.SaveChanges();

            return true;
        }

        /// <summary>
        /// Adds a new form to the database
        /// </summary>
        /// <param name="formName">Name of the form to add</param>
        /// <param name="formHtml">Form Html</param>
        /// <param name="workflow">Specified workflow to be attached to the form</param>
        /// <returns>Json response indicating failure or success</returns>
        public bool InsertFormToDb(String formName, String formHtml, int workflow, string creatorAccount)
        {
            //Following code adds the whole form
            int maxWholeFormId = db.WholeForms.Max(w => w.Id);
            if (maxWholeFormId < 1)
            {
                maxWholeFormId = 1; 
            }
            else
            {
                maxWholeFormId++;
            }

            WholeForm newWholeForm = new WholeForm();

            var getAccount = db.Accounts.Where(a => a.Name.Equals(creatorAccount)).ToList();

            foreach (var account in getAccount)
            {
                newWholeForm.AccountId = account.Id;
            }

            newWholeForm.Id = maxWholeFormId;
            newWholeForm.Name = formName;
            newWholeForm.WorkFlowId = workflow;

            try
            {
                db.WholeForms.Add(newWholeForm);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
            //-------------------------------------------------------------------

            //Following code adds the Form page
            int maxFormPageId = db.FormPages.Max(p => p.Id);
            if (maxFormPageId < 1)
            {
                maxFormPageId = 1;
            }
            else
            {
                maxFormPageId++;
            }

            int formPageNumber = 1;

            FormPage newFormPage = new FormPage();

            newFormPage.Id = maxFormPageId;
            newFormPage.PageNumber = formPageNumber;
            newFormPage.HtmlCode = formHtml;
            newFormPage.WholeFormId = maxWholeFormId;

            try
            {
                db.FormPages.Add(newFormPage);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
            //----------------------------------------------------------------------
            //Return true if no errors
            return true;
        }

        /// <summary>
        /// Gets the formpage for a user to fill out
        /// </summary>
        /// <param name="wholeFormId">Id of form to fill out</param>
        /// <returns></returns>
        public FormPage GetFormToFill(int wholeFormId)
        {
            FormPage formPageToFill = null;

            WholeForm formToFill = db.WholeForms.Find(wholeFormId);

            var GettingFormPageToFill = db.FormPages.Where(fp => fp.WholeFormId == wholeFormId);

            foreach(var page in GettingFormPageToFill)
            {
                formPageToFill = page;
            }

            return formPageToFill;
        }

        /// <summary>
        /// Returns a formpage based on the id
        /// </summary>
        /// <param name="formPageId">id of page to retreve</param>
        /// <returns></returns>
        public FormPage GetPage(int formPageId)
        {
            return db.FormPages.Find(formPageId);
        }

        /// <summary>
        /// Adds a form submission to the database
        /// </summary>
        /// <param name="pageFormId">id of page filled</param>
        /// <param name="userId">Id of user who filled form</param>
        /// <param name="htmlCode">Html to be saved</param>
        /// <param name="finished">If form is completed, or part saved</param>
        /// <returns></returns>
        public bool AddFormSubmission(int pageFormId, string userId, string htmlCode, string finished)
        {
            int maxFormSubmittionId = db.FormSubmissions.Max(fs => fs.Id);
            if (maxFormSubmittionId < 1)
            {
                maxFormSubmittionId = 1;
            }
            else
            {
                maxFormSubmittionId++;
            }

            FormPage fp = db.FormPages.Find(pageFormId);

            FormSubmission newFormSubmission = new FormSubmission();
            newFormSubmission.Id = maxFormSubmittionId;
            newFormSubmission.ApprovalStatus = "Pending";
            newFormSubmission.WholeFormId = fp.WholeFormId;

            db.FormSubmissions.Add(newFormSubmission);
            db.SaveChanges();

            int maxSubmittionWholeId = db.SubmissionWholes.Max(sw => sw.Id);
            if (maxSubmittionWholeId < 1)
            {
                maxSubmittionWholeId = 1;
            }
            else
            {
                maxSubmittionWholeId++;
            }

            SubmissionWhole newSubmissionWhole = new SubmissionWhole();

            var getAccount = db.Accounts.Where(a => a.Name.Equals(userId)).ToList();

            foreach (var account in getAccount)
            {
                newSubmissionWhole.AccountId = account.Id;
            }


            newSubmissionWhole.Id = maxSubmittionWholeId;
            newSubmissionWhole.DateModified = DateTime.Now;
            newSubmissionWhole.Finished = finished;
            newSubmissionWhole.FormSubmissionId = maxFormSubmittionId;

            db.SubmissionWholes.Add(newSubmissionWhole);
            db.SaveChanges();

            int maxSubmissionPartId = db.SubmissionParts.Max(sp => sp.Id);
            if (maxSubmissionPartId < 1)
            {
                maxSubmissionPartId = 1;
            }
            else
            {
                maxSubmissionPartId++;
            }

            SubmissionPart newSubmissionPart = new SubmissionPart();
            newSubmissionPart.Id = maxSubmissionPartId;
            newSubmissionPart.PageNumber = 1;
            newSubmissionPart.HtmlCode = htmlCode;
            newSubmissionPart.SubmissionWholeId = maxSubmittionWholeId;

            db.SubmissionParts.Add(newSubmissionPart);
            db.SaveChanges();

            return true;
        }

        public SubmissionPart getSubmissionPart(int submissionWholeId)
        {
            var GettingSavedFormToFill = db.SubmissionParts.Where(sp => sp.SubmissionWholeId == submissionWholeId);

            SubmissionPart savedFormToFill = null;

            foreach (var page in GettingSavedFormToFill)
            {
                savedFormToFill = page;
            }

            return savedFormToFill;
        }

        public bool UpdateSubmissionWhole(int id, string html, string finished)
        {
            SubmissionWhole updatedSubmissionWhole = db.SubmissionWholes.Find(id);
            updatedSubmissionWhole.DateModified = DateTime.Now;
            updatedSubmissionWhole.Finished = finished;

            db.SaveChanges();

            SubmissionPart updatedSubPart = getSubmissionPart(id);
            updatedSubPart.HtmlCode = html;

            db.SaveChanges();

            return true;
        }

        public FormPage GetPageFromSubmissionWhole(int submissionWholeId)
        {
            SubmissionWhole subWhole = db.SubmissionWholes.Find(submissionWholeId);
            FormSubmission formSub = db.FormSubmissions.Find(subWhole.FormSubmissionId);
            WholeForm wholeForm = db.WholeForms.Find(formSub.WholeFormId);

            FormPage formPage = GetFormToFill(wholeForm.Id);
            return formPage;
        }

        // method to retrieve a wholeform for a user to view
        // is going to be used to generate a link to the actual form page
        // cheers and hope that it works
        // wholeFormId used is the id of the form to be retrieved
        public WholeForm GetWholeForm(int wholeFormId)
        {
            WholeForm wholeFormToGet = db.WholeForms.Find(wholeFormId);

            var RetrievingWholeForm = db.WholeForms.Where(w => w.Id == wholeFormId);

            foreach (var itemId in RetrievingWholeForm)
            {
                wholeFormToGet = itemId;
            }

            return wholeFormToGet;
        }

        // method to retrieve a submissionwhole for user to view
        // is going to be used to generate link to the actual form page
        // submissionFormId keeps track of the form so that a link to it can be 
        // generated later
        // userId allows us to make sure that the user who is trying to access
        // this feature has actually saved their own form and aren't 
        // accessing someone else's
        public SubmissionWhole GetSubmittedForm(int submissionFormId, int userId)
        {
            SubmissionWhole submittedFormToGet = db.SubmissionWholes.Find(submissionFormId);

            var RetrievingSubmittedForm = db.SubmissionWholes.Where(s => (s.FormSubmissionId == submissionFormId))
                                                                .Where(s => (s.Finished == "No"))
                                                                .Where(s => (s.AccountId == userId));

            foreach (var itemId in RetrievingSubmittedForm)
            {
                submittedFormToGet = itemId;
            }

            return submittedFormToGet;
        }
    }
}
