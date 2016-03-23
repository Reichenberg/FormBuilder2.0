using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Team1_DynamicForms.Models
{
    public class SuperAdminIndexViewModel
    {
        public SuperAdminIndexViewModel(ApplicationUser user, IdentityRole role)
        {
            this.Email = user.Email;
            this.EmailConfirmed = user.EmailConfirmed;
            this.UserName = user.UserName;
            this.Role = role.Name;
            this.Id = user.Id;
        }

        public SuperAdminIndexViewModel()
        {

        }

        [Required]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }

        public string UserName { get; set; }
        [Required]
        public string Role{ get; set; }
        public string Id { get; set; }
    }
    
    //View model for Super Admin's account detail page
    public class SuperAdminDetailViewModel
    {
        public SuperAdminDetailViewModel(ApplicationUser user, IEnumerable<IdentityRole> roles)
        {
            this.Email = user.Email;
            this.EmailConfirmed = user.EmailConfirmed;
            this.UserName = user.UserName;
            this.Id = user.Id;
            this.LockoutEnabled = user.LockoutEnabled;
            this.Roles = roles.Select(r => r.Name);
        }

        public SuperAdminDetailViewModel()
        {

        }

        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
        [Required]
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool EmailConfirmed { get; set; }
    }

    public class SuperAdminEditViewModel
    {
        public SuperAdminEditViewModel(ApplicationUser user, IEnumerable<IdentityRole> currentRoles)
        {
            this.Email = user.Email;
            this.Id = user.Id;
            this.currentRoles = currentRoles.Select(r => r.Name);
        }

        public SuperAdminEditViewModel()
        {

        }

        public string Email { get; set; }
        public IEnumerable<string> currentRoles { get; set; }
        [Required]
        public string Id { get; set; }

        //Assumes when user is assigned a role they are automatically assigned all lower roles
        public ValidRoles newRole { get; set; }
    }

    //View model for Super Admin's account detail page
    public class SuperAdminDeleteViewModel
    {
        public SuperAdminDeleteViewModel(ApplicationUser user)
        {
            this.Email = user.Email;
            this.UserName = user.UserName;
            this.Id = user.Id;
        }

        public SuperAdminDeleteViewModel()
        {

        }

        public string Email { get; set; }
        [Required]
        public string Id { get; set; }
        public string UserName { get; set; }
    }

    //View model for Super Admin's account creation page
    public class SuperAdminCreateViewModel
    {
        public SuperAdminCreateViewModel()
        {

        }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Choose Role")]
        public ValidRoles role { get; set; }
    }
}