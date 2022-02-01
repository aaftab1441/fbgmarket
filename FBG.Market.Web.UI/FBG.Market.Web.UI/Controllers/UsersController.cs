using DevExpress.Web.Mvc;
using FBG.Market.Web.Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace FBG.Market.Web.Identity.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        public const string EditResultKey = "EditResult";
        public const string EditErrorKey = "EditError";
        ApplicationDbContext context;
        FBGMarketEntities db = new FBGMarketEntities();
        Utils Utils = new Utils();

        public UsersController()
        {
            context = new ApplicationDbContext();

        }
        private Boolean isSuperAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "SuperAdmin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        // GET: Users
        private Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        [Authorize(Roles = "SuperAdmin")]
        public ActionResult GetRoles(string vendorID)
        {
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ValueField = "RoleId";
                p.TextField = "RoleName";
                //p.ValueType = typeof(int);
                if (String.IsNullOrEmpty(vendorID))
                    p.BindList(Utils.GetAllRoles());
                else
                {
                    //p.BindList(Utils.GetRoles(System.Convert.ToInt32(vendorID)));
                    p.BindList(Utils.GetAllRoles());
                }
            });
        }
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;
                ViewData[EditResultKey] = "No.";

                if (isSuperAdminUser())
                {
                    ViewData[EditResultKey] = "Yes. ";
                }
                return View();
            }
            else
            {
                ViewData[EditErrorKey] = "Not Logged IN";
            }


            return View();


        }
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Entitlement()
        {
            var usersWithRoles = (from user in context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in context.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()

                                  }).ToList().Select(p => new UserRoleViewModel()

                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      Email = p.Email,
                                      Role = string.Join(",", p.RoleNames)
                                  });

            var usersWithRolesNew = (from user in db.AspNetUsers
                                     select new
                                     {
                                         UserId = user.Id,
                                         Username = user.UserName,
                                         Email = user.Email,
                                         RoleNames = (from userRole in user.AspNetRoles
                                                      join role in db.AspNetRoles on userRole.Id
                                                      equals role.Id
                                                      select role.Name).ToList(),
                                         Vendor = (from vendor in db.Vendors
                                                   join role in db.AspNetUsers on vendor.VID
                                                   equals role.VID
                                                   select vendor.VendorName).FirstOrDefault()

                                     }).ToList().Select(p => new UserRoleViewModel()

                                     {
                                         UserId = p.UserId,
                                         Username = p.Username,
                                         Email = p.Email,
                                         Role = string.Join(",", p.RoleNames),
                                         Vendor = p.Vendor
                                     });
            return View(usersWithRolesNew);
        }

        FBG.Market.Web.Identity.Models.FBGMarketEntities db1 = new FBG.Market.Web.Identity.Models.FBGMarketEntities();

        /*[ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
			var model = from user in db1.AspNetUsers
						select new AspNetUsersLocal
						{
							VID=user.VID,
							PhoneNumber=user.PhoneNumber,
							UserName = user.UserName,
							RoleId = user.AspNetRoles.FirstOrDefault().Id,
							LockoutEnabled = user.LockoutEnabled,
							LockoutEndDateUtc = user.LockoutEndDateUtc,
							Email = user.Email,

						};
			var temp = model.ToList();
			return PartialView("~/Views/Users/_GridViewPartial.cshtml", temp);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] AspNetUsersLocal item)
        {
            var model = db1.AspNetUsers;
            if (ModelState.IsValid)
            {
                try
                {
					
                    db1.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("~/Views/Users/_GridViewPartial.cshtml", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] AspNetUsersLocal item)
        {
            var model = db1.AspNetUsers;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.Id == item.Id);
                    if (modelItem != null)
                    {
                        this.TryUpdateModel(modelItem);
                        db1.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("~/Views/Users/_GridViewPartial.cshtml", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialDelete(System.String Id)
        {
            var model = db1.AspNetUsers;
            if (Id != null)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.Id == Id);
                    if (item != null)
                        model.Remove(item);
                    db1.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("~/Views/Users/_GridViewPartial.cshtml", model.ToList());
        }*/

        [Authorize(Roles = "SuperAdmin")]
        [ValidateInput(false)]
        public ActionResult GridViewNewPartial()
        {
            var model = from user in db1.AspNetUsers
                        select new AspNetUsersLocal
                        {
                            Id = user.Id,
                            VID = user.VID,
                            PhoneNumber = user.PhoneNumber,
                            UserName = user.UserName,
                            RoleId = user.AspNetRoles.FirstOrDefault().Id,
                            LockoutEnabled = user.LockoutEnabled,
                            LockoutEndDateUtc = user.LockoutEndDateUtc,
                            Email = user.Email,
                            IsEnabled = user.IsEnabled

                        };
            var temp = model.ToList();
            return PartialView("~/Views/Users/_GridViewNewPartial.cshtml", temp);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ValidateInput(false)]
        //public ActionResult GridViewNewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] FBG.Market.Web.Identity.Models.AspNetUsersLocal user)
        public ActionResult GridViewNewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] FBG.Market.Web.Identity.Models.AspNetUsersLocal user)
        {
            var model = db1.AspNetUsers.FirstOrDefault();
            try
            {

                using (var db = new FBGMarketEntities())
                {
                    using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                    {
                        ApplicationDbContext context = new ApplicationDbContext();
                        var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                        if (string.IsNullOrEmpty(user.Password))
                            user.Password = "fbg123"; //default password
                        var passwordHash = UserManager.PasswordHasher.HashPassword(user.Password);
                        var SecurityStamp = Guid.NewGuid().ToString();

                        String query = @"INSERT INTO [dbo].[AspNetUsers]
                       ([Id]
                       ,[Email]
                       ,[EmailConfirmed]
                       ,[PasswordHash]
                       ,[SecurityStamp]
                        ,[PhoneNumberConfirmed]
                        ,[TwoFactorEnabled]
                        ,[LockoutEnabled]
      ,[AccessFailedCount]
                       ,[UserName]
                       ,[VID]
                       ,[IsEnabled])
     VALUES
           (@Id
           ,@Email
           ,@EmailConfirmed
           ,@PasswordHash
           ,@SecurityStamp
            ,@PhoneNumberConfirmed
            ,@TwoFactorEnabled
            ,@LockoutEnabled
      ,@AccessFailedCount
           ,@UserName
           ,@VID
           ,@IsEnabled)";
                        var userId = Guid.NewGuid().ToString();
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", userId);
                            command.Parameters.AddWithValue("@Email", user.Email);
                            command.Parameters.AddWithValue("@EmailConfirmed", 0);

                            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                            command.Parameters.AddWithValue("@SecurityStamp", SecurityStamp);
                            command.Parameters.AddWithValue("@PhoneNumberConfirmed", 0);
                            command.Parameters.AddWithValue("@TwoFactorEnabled", 0);
                            command.Parameters.AddWithValue("@LockoutEnabled", 1);

                            command.Parameters.AddWithValue("@AccessFailedCount", 0);

                            command.Parameters.AddWithValue("@UserName", user.UserName);
                            command.Parameters.AddWithValue("@VID", user.VID);
                            command.Parameters.AddWithValue("@IsEnabled", true);


                            connection.Open();
                            int result = command.ExecuteNonQuery();
                            // Check Error
                            if (result < 0)
                                Console.WriteLine("Error inserting data into Database!");
                        }
                        query = @"INSERT INTO [dbo].[AspNetUserRoles]
                                   ([UserId]
                                   ,[RoleId])
                             VALUES
                                   (@UserId
                                   ,@RoleId)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {

                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@RoleId", user.RoleId);
                            int result = command.ExecuteNonQuery();

                        }

                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewData["EditErrorKey"] = ex.Message;
            }
            var modelU = from user1 in db1.AspNetUsers
                         select new AspNetUsersLocal
                         {
                             Id = user1.Id,
                             VID = user1.VID,
                             PhoneNumber = user1.PhoneNumber,
                             UserName = user1.UserName,
                             Password = user1.PasswordHash,
                             SecurityStamp = user1.SecurityStamp,
                             RoleId = user1.AspNetRoles.FirstOrDefault().Id,
                             LockoutEnabled = user1.LockoutEnabled,
                             LockoutEndDateUtc = user1.LockoutEndDateUtc,
                             Email = user1.Email,
                             IsEnabled = user1.IsEnabled

                         };
            var temp = modelU.ToList();

            return PartialView("~/Views/Users/_GridViewNewPartial.cshtml", temp);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewNewPartialUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] FBG.Market.Web.Identity.Models.AspNetUsersLocal item)
        {
            
            var model = db1.AspNetUsers;
            if (ModelState.IsValid)
            {
                try
                {
                    //str = str.ToUpper();
                    var modelItem = model.FirstOrDefault(it => it.Id == item.Id);
                    if (modelItem != null)
                    {
                        modelItem.IsEnabled = item.IsEnabled.Value;
                        modelItem.Email = item.Email;
                        modelItem.UserName = item.UserName;
                        if (!string.IsNullOrEmpty(item.Password))
                        {
                            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                            var passwordHash = UserManager.PasswordHasher.HashPassword(item.Password);
                            var SecurityStamp = Guid.NewGuid().ToString();
                            modelItem.PasswordHash = passwordHash;
                            modelItem.SecurityStamp = SecurityStamp;
                        }
                        //this.TryUpdateModel(modelItem);
                        modelItem.VID = item.VID;
                        db1.SaveChanges();
                        ViewData[EditResultKey] = "User is updated successfully.";

                    }
                    using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                    {

                        string query = @"Update [dbo].[AspNetUserRoles] SET RoleId=@roleId Where UserId = @userId";
                        var userId = Guid.NewGuid().ToString();
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@roleId", item.RoleId);
                            command.Parameters.AddWithValue("@userId", item.Id);
                            connection.Open();
                            int result = command.ExecuteNonQuery();
                            // Check Error
                            if (result < 0)
                                ViewData[EditErrorKey] = "Error inserting data into Database!";
                        }

                    }

                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            else
                ViewData["EditErrorKey"] = "Please, correct all errors.";
            var modelU = from user in db1.AspNetUsers
                         select new AspNetUsersLocal
                         {
                             Id = user.Id,
                             VID = user.VID,
                             PhoneNumber = user.PhoneNumber,
                             UserName = user.UserName,
                             RoleId = user.AspNetRoles.FirstOrDefault().Id,
                             LockoutEnabled = user.LockoutEnabled,
                             LockoutEndDateUtc = user.LockoutEndDateUtc,
                             Email = user.Email,
                             IsEnabled = user.IsEnabled

                         };
            var temp = modelU.ToList();
            return PartialView("~/Views/Users/_GridViewNewPartial.cshtml", temp);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewNewPartialDelete(System.String Id)
        {
            var model = db1.AspNetUsers;
            if (Id != null)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.Id == Id);
                    if (item != null)
                        model.Remove(item);
                    db1.SaveChanges();
                    ViewData[EditResultKey] = "User is deleted successfully.";
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            var modelU = from user in db1.AspNetUsers
                         select new AspNetUsersLocal
                         {
                             Id = user.Id,
                             VID = user.VID,
                             PhoneNumber = user.PhoneNumber,
                             UserName = user.UserName,
                             RoleId = user.AspNetRoles.FirstOrDefault().Id,
                             LockoutEnabled = user.LockoutEnabled,
                             LockoutEndDateUtc = user.LockoutEndDateUtc,
                             Email = user.Email,
                             IsEnabled = user.IsEnabled

                         };
            var temp = modelU.ToList();
            return PartialView("~/Views/Users/_GridViewNewPartial.cshtml", temp.ToList());
        }
    }
}