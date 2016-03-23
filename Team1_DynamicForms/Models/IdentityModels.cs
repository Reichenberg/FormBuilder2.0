using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Team1_DynamicForms.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            //Should probly make this constant outside of class.
            await manager.AddToRoleAsync(this.Id, "User");
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

 //       public System.Data.Entity.DbSet<Team1_DynamicForms.Models.ApplicationUser> ApplicationUsers { get; set; }
    }

    public enum ValidRoles
    {
        [Display(Name = "User")]
        User,
        [Display(Name = "Admin")]
        Admin,
        [Display(Name = "Super Admin")]
        SuperAdmin
    };

    public class MyDbInitializer : DropCreateDatabaseAlways<Models.ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var UserManager = new UserManager<Models.ApplicationUser>(new UserStore<Models.ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));


            string userRoleName = ValidRoles.User.ToString();
            string adminRoleName = ValidRoles.Admin.ToString();
            string superAdminRoleName = ValidRoles.SuperAdmin.ToString();
            string emailAddress = "@test.com";


            //These should be removed used to generate sample accounts for testing purposes
            //TESTPASSWORDS
            string adminRoleDefPass = "123456";
            string userRoleDefPass = "123456";
            string superAdminRoleDefPass = "123456";

            //Create default roles if they do not exist
            if(!RoleManager.RoleExists(superAdminRoleName))
            {
                var roleResult = RoleManager.Create(new IdentityRole(superAdminRoleName));
            }
            if(!RoleManager.RoleExists(adminRoleName))
            {
                var roleResult = RoleManager.Create(new IdentityRole(adminRoleName));
            }
            if(!RoleManager.RoleExists(userRoleName))
            {
                var roleResult = RoleManager.Create(new IdentityRole(userRoleName));
            }

            //Make sample accounts for testing and add them to roles
            //TESTCODE
            var superAdmin = new Models.ApplicationUser();
            superAdmin.UserName = superAdminRoleName + emailAddress;
            superAdmin.Email = superAdminRoleName + emailAddress;
            var SuperAdminResult = UserManager.Create(superAdmin, superAdminRoleDefPass);
            if (SuperAdminResult.Succeeded)
            {
                var result = UserManager.AddToRole(superAdmin.Id, superAdminRoleName);
            }

            var admin = new Models.ApplicationUser();
            admin.UserName = adminRoleName + emailAddress;
            admin.Email = adminRoleName + emailAddress;
            var adminResult = UserManager.Create(admin, adminRoleDefPass);
            if(adminResult.Succeeded)
            {
                var result = UserManager.AddToRole(admin.Id, adminRoleName);
                result = UserManager.AddToRole(superAdmin.Id, adminRoleName);
            }

            var user = new Models.ApplicationUser();
            user.UserName = userRoleName + emailAddress;
            user.Email = userRoleName + emailAddress;
            var userResult = UserManager.Create(user, userRoleDefPass);
            if (userResult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, userRoleName);
                result = UserManager.AddToRole(admin.Id, userRoleName);
                result = UserManager.AddToRole(superAdmin.Id, userRoleName);
            }

            base.Seed(context);
        }
    }
}