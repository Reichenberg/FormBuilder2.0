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

            if(finished.ToLower() == "yes")
            {
                WholeForm form = db.WholeForms.Find(fp.WholeFormId);
                if(form.WorkFlowId > -1)
                {
                    var accWorkflows = db.AccountWorkflows.Where(aw => aw.WorkFlowId == form.WorkFlowId && aw.AccountId < 0);

                    foreach(AccountWorkflow adminWF in accWorkflows)
                    {
                        AccountWorkflow newAW = new AccountWorkflow();
                        int accountWorkFlowId = db.AccountWorkflows.Max(acwf => acwf.Id);
                        accountWorkFlowId = accountWorkFlowId < 0 ? 1 : accountWorkFlowId + 1;

                        newAW.Id = accountWorkFlowId;
                        newAW.Order = adminWF.Order;
                        newAW.Notified = adminWF.Notified;
                        newAW.AccountId = adminWF.AccountId;
                        newAW.WorkFlowId = adminWF.WorkFlowId;
                        newAW.SubmissionWholeId = newSubmissionWhole.Id;

                        db.AccountWorkflows.Add(newAW);
                        db.SaveChanges();
                    }
                }
            }


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
        public List<WholeForm> GetWholeForm()
        {

            var numberOfWholeForms = db.WholeForms.ToList();

            List<WholeForm> wholeFormsList = new List<WholeForm>();

            foreach(var unfilledForm in numberOfWholeForms)
            {
                wholeFormsList.Add(unfilledForm);
            }
                
            


            return wholeFormsList;
        }



        // method to retrieve a submissionwhole for user to view
        // is going to be used to generate link to the actual form page
        // submissionFormId keeps track of the form so that a link to it can be 
        // generated later
        // userId allows us to make sure that the user who is trying to access
        // this feature has actually saved their own form and aren't 
        // accessing someone else's
        public List<SubmissionWhole> GetSubmittedForm(string userName)
        {
            var getAccount = db.Accounts.Where(a => a.Name.Equals(userName)).ToList();

            int userId = 0;

            foreach (var account in getAccount)
            {
                userId = account.Id;
            }

            var gettingSubmittedForms = db.SubmissionWholes.Where(sw => sw.AccountId == userId).Where(sw => sw.Finished == "No");

            List<SubmissionWhole> submittedFormsList = new List<SubmissionWhole>();

            foreach (var submittedForm in gettingSubmittedForms)
            {
                submittedFormsList.Add(submittedForm);
            }

            return submittedFormsList;
        }






        /// <summary>
        /// Gets workflows from database based on user id
        /// TODO: add ability to grab workflows by type
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal WorkFlowGroupViewModel GetWorkFlows(Account user)
        {
            WorkFlowGroupViewModel output = new WorkFlowGroupViewModel();

            //Actually use type in future
            output.Type = "Standard";

            //Get account with matching user id

            //Loop through all attached workflows
            foreach (var accountFlow in user.AccountWorkflows)
            {
                WorkFlow curWorkflow = accountFlow.WorkFlow;
                //Ignore empty workflow for now, might just want to change db to fix this later, don't like it
                if (curWorkflow.Id != 1)
                    output.FormNames.AddRange(curWorkflow.WholeForms.Select(f => f.Name));
            }

            return output;
        }

        /// <summary>
        /// Creates a new WorkFlow and adds it to the database
        /// </summary>
        /// <param name="Type"></param>
        /// <returns>new workflow's id if successful, return negative value otherwise</returns>
        internal int CreateAndInsertWorkFlow(string Type)
        {
            int workFlowID = db.WorkFlows.Max(wf => wf.Id);

            workFlowID = workFlowID < 0 ? 1 : workFlowID + 1;
            if (workFlowID == 1)
                workFlowID = 2;

            WorkFlow newWorkflow = new WorkFlow();
            newWorkflow.Id = workFlowID;
            newWorkflow.Type = Type;

            db.WorkFlows.Add(newWorkflow);
            db.SaveChanges();

            return workFlowID;
        }

        /// <summary>
        /// Adds users to an existing workflow
        /// </summary>
        /// <param name="users">User ids</param>
        /// <param name="workFlowId">the id of the workflow to add users to</param>
        /// <returns></returns>
        internal bool AddUsersToWorkFlow(List<int> users, int workFlowId)
        {
            for (int i = 0; i < users.Count; i++)
            {
                AccountWorkflow newAccountWorkFlow = new AccountWorkflow();

                //Ensure new id is one higher than existing values
                int accountWorkFlowId = db.AccountWorkflows.Max(acwf => acwf.Id);
                accountWorkFlowId = accountWorkFlowId < 0 ? 1 : accountWorkFlowId + 1;

                newAccountWorkFlow.Id = accountWorkFlowId;
                newAccountWorkFlow.Notified = "false";
                newAccountWorkFlow.Order = i + 1;
                newAccountWorkFlow.WorkFlowId = workFlowId;
                newAccountWorkFlow.AccountId = users[i];

                db.AccountWorkflows.Add(newAccountWorkFlow);
                db.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// Adds a workFlow to an existing form
        /// </summary>
        /// <param name="formId">Id of the form being modified</param>
        /// <param name="workFlowId">Id of the workflow being added</param>
        internal void AddWorkFlowToForm(int formId, int workFlowId)
        {
            //Add workflow to form
            var form = db.WholeForms.Where(wf => wf.Id == formId).FirstOrDefault();
            form.WorkFlowId = workFlowId;
            db.WholeForms.Attach(form);
            var entry = db.Entry(form);
            entry.Property(e => e.WorkFlowId).IsModified = true;
            entry.Property(e => e.Id).IsModified = false;
            entry.Property(e => e.AccountId).IsModified = false;
            entry.Property(e => e.Name).IsModified = false;
            // other changed properties
            db.SaveChanges();
        }

        internal List<int> GetUsersByEmail(List<string> emails)
        {
            List<int> users = new List<int>();

            foreach (string email in emails)
            {
                users.AddRange(db.Accounts.Where(acc => acc.Name == email).Select(acc => acc.Id));
            }
            return users;
        }

        /// <summary>
        /// Gets an account based on the user current context's user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal Account GetCurrentAccount()
        {
            //Don't question this it just gets the current application user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            return db.Accounts.Where(u => u.UserId == user.Id).FirstOrDefault();
        }

        /// <summary>
        /// Gets a forms name based on its id
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        internal string GetFormName(int formId)
        {
            return db.WholeForms.Where(wf => wf.Id == formId).FirstOrDefault().Name;
        }
        /// <summary>
        /// Returns forms created by a given user in a dictionary pairing form id and form name
        /// </summary>
        /// <param name="user">User to look up forms created by</param>
        /// <returns>Dictionary pairing id and form name</returns>
        internal Dictionary<int, string> GetCreatedFormsDict(Account user)
        {
            return db.WholeForms.Where(wf => wf.AccountId == user.Id).ToDictionary(wf => wf.Id, wf => wf.Name);
        }












        public List<AccountWorkflow> GetSubmittedFormsForApproval(Account admin)
        {
            List<AccountWorkflow> FormsToReturn = new List<AccountWorkflow>();

            //Gets account forkflows the admin must complete at some point in time.
            var FormsToApprove =  db.AccountWorkflows.Where(aw => aw.AccountId == admin.Id && aw.Order != 0 && aw.SubmissionWholeId > -1);

            foreach (var aW in FormsToApprove)
            {
                bool add = true;
                //Gets all account workflows for each form submission to see if the admin is next in order
                var checkForOrder = db.AccountWorkflows.Where(aw => aw.SubmissionWholeId == aW.SubmissionWholeId);
                
                foreach (var checkOrder in checkForOrder)
                {
                    //If the admin is not next in line to approve the form, don't include the form to approve.
                    if(checkOrder.Order < aW.Order && checkOrder.Order > 0)
                    {
                        add = false;
                        break;
                    }
                }

                if(add == true)
                {
                    FormsToReturn.Add(aW);
                }
            }

            return FormsToReturn;
        }

        public string GetUserWhoFilledFormFromAccountWorkflow(int id)
        {
            AccountWorkflow form = db.AccountWorkflows.Find(id);
            SubmissionWhole formSubmission = db.SubmissionWholes.Find(form.SubmissionWholeId);
            Account user = db.Accounts.Find(formSubmission.AccountId);
            return user.Name;
        }

        public string GetNameOFFilledFormFromAccountWorkflow(int id)
        {
            AccountWorkflow form = db.AccountWorkflows.Find(id);
            SubmissionWhole formSubmission = db.SubmissionWholes.Find(form.SubmissionWholeId);
            FormSubmission wholeFormSub = db.FormSubmissions.Find(formSubmission.FormSubmissionId);
            WholeForm wholeForm = db.WholeForms.Find(wholeFormSub.WholeFormId);
            return wholeForm.Name;
        }









        public int ApproveForm(int accWorkflowId)
        {
            //Find the account workflow to approve
            AccountWorkflow approvedAW = db.AccountWorkflows.Find(accWorkflowId);
            approvedAW.Order = 0;
            db.SaveChanges();

            //Get all account workflows associated with the submitted form to see if the workflow has been completed
            var checkForLastApproval = db.AccountWorkflows.Where(aw => aw.SubmissionWholeId == approvedAW.SubmissionWholeId);

            foreach(var accW in checkForLastApproval)
            {
                //If there is still someone remaining that must check the form, return true.
                if(accW.Order != 0)
                {
                    return 1;
                }
            }

            //If all in the workflow have approved, set the status to Approved for the form
            SubmissionWhole completedForm = db.SubmissionWholes.Find(approvedAW.SubmissionWholeId);
            FormSubmission approvedForm = db.FormSubmissions.Find(completedForm.FormSubmissionId);
            approvedForm.ApprovalStatus = "Approved";

            return 1;
        }

        public int DenyForm(int accWorkflowId)
        {
            //Find the account workflow to deny
            AccountWorkflow deniedAW = db.AccountWorkflows.Find(accWorkflowId);
            deniedAW.Order = 0;
            db.SaveChanges();

            //Get all account workflows associated with the submitted form to see if the workflow has been completed
            var checkToClearRestOfWorkflow = db.AccountWorkflows.Where(aw => aw.SubmissionWholeId == deniedAW.SubmissionWholeId);

            foreach (var accW in checkToClearRestOfWorkflow)
            {
                if (accW.Order != 0)
                {
                    accW.Order = 0;
                    db.SaveChanges();
                }
            }

            //Set the entire form to Denied
            SubmissionWhole completedForm = db.SubmissionWholes.Find(deniedAW.SubmissionWholeId);
            FormSubmission deniedForm = db.FormSubmissions.Find(completedForm.FormSubmissionId);
            deniedForm.ApprovalStatus = "Denied";

            return 1;
        }













        public List<string> GetFilledFormData(int accWorkflowId)
        {
            List<string> formdata = new List<string>();
            AccountWorkflow aW = db.AccountWorkflows.Find(accWorkflowId);
            SubmissionPart filledForm = db.SubmissionParts.Find(aW.SubmissionWholeId);
            string htmlData = filledForm.HtmlCode;

            //int currentIndex = htmlData.IndexOf("<");


            //while (currentIndex < htmlData.Length)
            //{
            //    if (htmlData.Substring(currentIndex, "<h3>".Length) == "<h3>")
            //    {
            //        currentIndex = currentIndex + "<h3>".Length;
            //        formdata.Add("Header Label: ");
            //        formdata.Add(htmlData.Substring(currentIndex, htmlData.IndexOf("</h3>", currentIndex) - currentIndex));
            //        currentIndex += htmlData.IndexOf("</h3>") + "</h3>".Length;
            //    }


            //    else if (htmlData.Substring(currentIndex, "<label class=\"form-label\">".Length) == "<label class=\"form-label\">")
            //    {
            //        currentIndex = currentIndex + "<label class=\"form-label\">".Length;
            //        formdata.Add("Label: ");
            //        formdata.Add(htmlData.Substring(currentIndex, htmlData.IndexOf("</label>", currentIndex) - currentIndex));
            //        currentIndex += htmlData.IndexOf("</label>") + "</label>".Length;
            //    }


            //    else if (htmlData.Substring(currentIndex, "<input class=\"form-control\" type=\"text\" value=\"".Length) == "<input class=\"form-control\" type=\"text\" value=\"")
            //    {
            //        currentIndex = currentIndex + "<input class=\"form-control\" type=\"text\" value=\"".Length;
            //        formdata.Add("Test Field Label: ");
            //        formdata.Add(htmlData.Substring(currentIndex, htmlData.IndexOf("\" name=", currentIndex) - currentIndex));
            //        currentIndex += htmlData.IndexOf("\" name") + "\" name".Length;
            //    }


            //    else if (htmlData.Substring(currentIndex, "<input type=\"checkbox\" value=\"".Length) == "<input type=\"checkbox\" value=\"")
            //    {
            //        currentIndex = currentIndex + "<input type=\"checkbox\" value=\"".Length;
            //        formdata.Add("Label: ");
            //        formdata.Add(htmlData.Substring(htmlData.IndexOf(">", currentIndex) + 1, htmlData.IndexOf("</label>", currentIndex + 1) - currentIndex + 1));
            //        if (htmlData.IndexOf("checked", currentIndex) < htmlData.IndexOf(">", currentIndex) && (htmlData.IndexOf("checked", currentIndex) > 0))
            //        {
            //            formdata.Add(htmlData.Substring(currentIndex, htmlData.IndexOf("\" checked", currentIndex) - currentIndex));
            //            currentIndex += htmlData.IndexOf("\" checked") + "\" checked".Length;
            //        }
            //    }


            //    currentIndex += 1;
            //}

            int currentIndex = 0;
            int nextElement = 0;
            while (currentIndex < htmlData.Length )
            {
                nextElement = htmlData.IndexOf("type=\"text\"",currentIndex) + "type=\"text\"".Length;

                if (nextElement > currentIndex)
                {
                    htmlData = htmlData.Insert(nextElement, " readonly");
                    currentIndex = nextElement + 1;
                }
                else
                {
                    break;
                }
            }

            currentIndex = 0;
            nextElement = 0;
            while (currentIndex < htmlData.Length)
            {
                nextElement = htmlData.IndexOf("type=\"timepicker\"", currentIndex) + "type=\"timepicker\"".Length;

                if (nextElement > currentIndex)
                {
                    htmlData = htmlData.Insert(nextElement, " disabled");
                    currentIndex = nextElement + 1;
                }
                else
                {
                    break;
                }
            }

            currentIndex = 0;
            nextElement = 0;
            while (currentIndex < htmlData.Length)
            {
                nextElement = htmlData.IndexOf("type=\"datepicker\"", currentIndex) + "type=\"datepicker\"".Length;

                if (nextElement > currentIndex)
                {
                    htmlData = htmlData.Insert(nextElement, " disabled");
                    currentIndex = nextElement + 1;
                }
                else
                {
                    break;
                }
            }

            currentIndex = 0;
            nextElement = 0;
            while (currentIndex < htmlData.Length)
            {
                nextElement = htmlData.IndexOf("<select class=\"form-control\"", currentIndex) + "<select class=\"form-control\"".Length;

                if (nextElement > currentIndex)
                {
                    htmlData = htmlData.Insert(nextElement, " disabled");
                    currentIndex = nextElement + 1;
                }
                else
                {
                    break;
                }
            }

            currentIndex = 0;
            nextElement = 0;
            while (currentIndex < htmlData.Length)
            {
                nextElement = htmlData.IndexOf("type=\"checkbox\"", currentIndex) + "type=\"checkbox\"".Length;

                if (nextElement > currentIndex)
                {
                    htmlData = htmlData.Insert(nextElement, " disabled readonly");
                    currentIndex = nextElement + 1;
                }
                else
                {
                    break;
                }
            }

            currentIndex = 0;
            nextElement = 0;
            while (currentIndex < htmlData.Length)
            {
                nextElement = htmlData.IndexOf("type=\"radio\"", currentIndex) + "type=\"radio\"".Length;

                if (nextElement > currentIndex)
                {
                    htmlData = htmlData.Insert(nextElement, " disabled readonly");
                    currentIndex = nextElement + 1;
                }
                else
                {
                    break;
                }
            }

            return formdata;
        }


        public AccountWorkflow GetAccountForkflowFromId(int id)
        {
            return db.AccountWorkflows.Find(id);
        }


    }
}
