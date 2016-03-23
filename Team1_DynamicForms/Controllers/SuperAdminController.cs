using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Team1_DynamicForms.Models;

namespace Team1_DynamicForms.Controllers
{

    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SuperAdmin
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("SuperAdmin"))
            {
                var users = new List<SuperAdminIndexViewModel>();
                foreach (var user in db.Users)
                {
                    var rolePerUser = db.Roles.Where(r => r.Users.Any(m => m.UserId == user.Id));
                    var curUser = new SuperAdminIndexViewModel(user, rolePerUser.FirstOrDefault());

                    //Check for other higher roles in the database
                    var curRoleVal = Enum.Parse(typeof(ValidRoles), curUser.Role);
                    foreach (var role in rolePerUser)
                    {
                        if ((ValidRoles)Enum.Parse(typeof(ValidRoles), role.Name) > (ValidRoles)curRoleVal)
                        {
                            curUser.Role = role.Name;
                            curRoleVal = (ValidRoles)Enum.Parse(typeof(ValidRoles), role.Name);
                        }
                    }

                    users.Add(curUser);
                }

                return View(users);
            }
            return RedirectToAction("../Account/Login");   
        }

        // GET: SuperAdmin/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Ensure user is valid
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            //Generate viewmodel, Linq statement is nonsense that looks up roles by user, if not broken don't touch
            SuperAdminDetailViewModel viewModel = new SuperAdminDetailViewModel(applicationUser, db.Roles.Where(r => r.Users.Select(u => u.UserId).Contains(applicationUser.Id)).ToList());

            return View(viewModel);
        }

        // GET: SuperAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SuperAdmin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SuperAdminCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = new ApplicationUser { UserName = viewModel.Email, Email = viewModel.Email };
                var result = await userManager.CreateAsync(user, viewModel.Password);
                if(result.Succeeded)
                {
                    //Ensure we have new user, might not actually be necessary
                    var newUser = await userManager.FindByEmailAsync(user.Email);
                    for(int i = 0; (i <= (int)viewModel.role ) && (i < Enum.GetNames(typeof(ValidRoles)).Length); ++i)
                    {
                        result = await userManager.AddToRoleAsync(newUser.Id, ((ValidRoles)i).ToString());
                    }


                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        // GET: SuperAdmin/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            //Generate viewmodel, Linq statement is nonsense that looks up roles by user, if not broken don't touch
            SuperAdminEditViewModel viewModel = new SuperAdminEditViewModel(applicationUser, db.Roles.Where(r => r.Users.Select(u => u.UserId).Contains(applicationUser.Id)).ToList());
            return View(viewModel);
        }

        // POST: SuperAdmin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SuperAdminEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                
                userManager.AddToRole(viewModel.Id, viewModel.newRole.ToString());
                if(viewModel.newRole == ValidRoles.SuperAdmin)
                {
                    userManager.AddToRole(viewModel.Id, ValidRoles.Admin.ToString());
                    userManager.AddToRole(viewModel.Id, ValidRoles.User.ToString());
                }
                else if (viewModel.newRole == ValidRoles.Admin)
                {
                    //If the user is being moved into Admin role add lower, but remove higher roles
                    if(userManager.IsInRole(viewModel.Id, ValidRoles.SuperAdmin.ToString()))
                    {
                        userManager.RemoveFromRole(viewModel.Id, ValidRoles.SuperAdmin.ToString());
                    }
                    userManager.AddToRole(viewModel.Id, "User");
                }
                else
                {
                    //If the user is being moved into user role remove them from higher roles
                    if (userManager.IsInRole(viewModel.Id, ValidRoles.SuperAdmin.ToString()))
                    {
                        userManager.RemoveFromRole(viewModel.Id, ValidRoles.SuperAdmin.ToString());
                    }
                    if (userManager.IsInRole(viewModel.Id, ValidRoles.Admin.ToString()))
                    {
                        userManager.RemoveFromRole(viewModel.Id, ValidRoles.Admin.ToString());
                    }
                }

//                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: SuperAdmin/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            SuperAdminDeleteViewModel viewModel = new SuperAdminDeleteViewModel(applicationUser);

            return View(viewModel);
        }

        // POST: SuperAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {            
            //Check valid state before doing any work
            if (ModelState.IsValid)
            {
                ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                
                //Get user
                var user = await userManager.FindByIdAsync(id);
                var logins = user.Logins;

                //Log user out of system
                foreach (var login in logins.ToList())
                {
                    await userManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                }

                //Removes user's roles
                var rolesForUser = await userManager.GetRolesAsync(id);

                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        // item should be the name of the role
                        var result = await userManager.RemoveFromRoleAsync(user.Id, item);
                    }
                }

                //Delete user
                await userManager.DeleteAsync(user);

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
