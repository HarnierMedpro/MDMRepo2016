using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MDM.WebPortal.Models.Identity
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public bool Active { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    //Agregando una definicion para la clase Rol, sin embargo hay que actualizar el fichero IdentityConfig.cs de
    //la carpeta APP-Start
    //especificamente ApplicationRoleManager y InitializeDatabaseForEF y los metodos del controlador RolesAdmin como Create()
    //tambien hay que actualizar el fichero AdminViewModels.cs
    /*Adding a Definition for Role Entity */
    public class ApplicationRole : IdentityRole
    {
        public bool Active { get; set; }
        public int Priority { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }

        public ApplicationRole() : base() { }
        public ApplicationRole(string name, bool Active, int Priority) : base(name)
        {
            this.Active = Active;
            this.Priority = Priority;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Permission> Permissions { get; set; }

        public DbSet<ControllerSystem> Controllers { get; set; }

        public DbSet<ActionSystem> Actions { get; set; }

        public DbSet<AreaSystem> Areas { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public ApplicationDbContext()
            : base("MDMPermissionsIdentitiesv1", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

       
    }
}