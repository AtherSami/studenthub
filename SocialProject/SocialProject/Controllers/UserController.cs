using SocialProject.Data;
using SocialProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using symphony_services.Helpers;
using System.Text;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace SocialProject.Controllers
{
    public class UserController : Controller
    { 
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _applicationDb;
		private readonly IWebHostEnvironment _environment;

		public UserController(ILogger<UserController> logger, IWebHostEnvironment environment, ApplicationDbContext applicationDb)
    {
        _logger = logger;
        _applicationDb = applicationDb;
			_environment = environment;
		}
    
        public IActionResult Index()
        {
            return View();
        }
       
        public IActionResult Add()
        {
            


                return View();
            }
            [HttpPost]
		public async Task<IActionResult> Add(UserModel usr, IFormFile file)
            {
			
            if (file != null && file.Length > 0)
			{
				// save file to the server
				var uploads = Path.Combine(_environment.WebRootPath, "uploads");
				if (!Directory.Exists(uploads))
				{
					Directory.CreateDirectory(uploads);
				}

				var fileName = Path.GetFileName(file.FileName);
				using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
				{
					await file.CopyToAsync(fileStream);
					usr.Attachment =  fileName;
				}
			}
            var email=_applicationDb.UserModels.FirstOrDefault(x => x.Email == usr.Email);
            if (email == null) {
				_applicationDb.Add(usr);
				_applicationDb.SaveChanges();


				return RedirectToActionPermanent("login", "User");

			}
            
			if (usr.Email== email.Email && email.Email !=null)
            {
				TempData["message"] = "This email already taken.";

				return RedirectToActionPermanent("Add", "User");
			}
			//_applicationDb.Add(usr);
   //             _applicationDb.SaveChanges();


            return RedirectToActionPermanent("login", "User");
      }


        [HttpGet]
        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult login(UserModel usr)
        {


            
            var Found = _applicationDb.UserModels.Where(m => m.Email == usr.Email && m.Password == usr.Password).FirstOrDefault();


            
            if (Found == null)
            {
                ViewBag.LoginStatus = 0;
            }
            else
            {

                HttpContext.Session.SetString("FullName", Found.FullName);
                HttpContext.Session.SetString("Password", Found.Password);
                HttpContext.Session.SetString("Attachment", "/Uploads/"+Found.Attachment);
				HttpContext.Session.SetInt32("UserId", Found.UserId);
				HttpContext.Session.SetString("Email", Found.Email);
				HttpContext.Session.SetString("Address", Found.Address);
				//HttpContext.Session.SetString("Joined", Found.CreateDate.ToString());






				return RedirectToActionPermanent("Create", "PostModels");
            }
            return View(usr);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
		[HttpGet]
		public async Task<IActionResult> GetNotifications()
		{
			// Get the authenticated user
			int? userId = HttpContext.Session.GetInt32("UserId");

			// Get the notifications for the user
			var notifications = await _applicationDb.Notifications
				.Where(n => n.ReceiverId == userId && !n.IsRead)
				.OrderByDescending(n => n.CreatedAt)
				.ToListAsync();

			// Mark the notifications as read
			foreach (var notification in notifications)
			{
				notification.IsRead = true;
			}
			await _applicationDb.SaveChangesAsync();
            ViewBag.Notifications = notifications;

            // Return the notifications as JSON
            return Json(notifications);
		}
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //     public async Task<IActionResult> ForgotPassword(string email)
        //     {
        //         // Find the user with the specified email address
        //         var user = await _applicationDb.UserModels.FirstOrDefaultAsync(u => u.Email == email);
        //         if (user == null)
        //         {
        //             // Don't reveal that the user does not exist
        //             return View("ForgotPasswordConfirmation");
        //         }

        //// var token = Guid.NewGuid().ToString();
        //string EncruptedString = user.Email + "&&$" + DateTime.Now + "&&$" + user.UserId;
        //var Encrupted = Crypto.Encrypt(EncruptedString);
        //var callbackUrl = Url.Action("ResetPassword", "User", new { userId = user.UserId, token = Encrupted }, Request.Scheme);


        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine("Dear!");
        //sb.AppendLine("Member your forget password request for ITCP!");
        //sb.AppendLine("If you want to reset your account then click on blew link.");
        //sb.AppendLine(callbackUrl);
        //sb.AppendLine("Regards");
        //sb.AppendLine("ITCP Admin");
        ////user.ResetPasswordToken = token;
        //user.ResetPasswordExpiration = DateTime.UtcNow.AddHours(24);
        //         await _applicationDb.SaveChangesAsync();



        //         return View("ForgotPasswordConfirmation");
        //     }
        //public class EmailSender
        //{
        //	public string SmtpServer { get; set; }
        //	public int SmtpPort { get; set; }
        //	public string SmtpUsername { get; set; }
        //	public string SmtpPassword { get; set; }
        //	public bool SmtpEnableSsl { get; set; }

        //	public async Task SendEmailAsync(string to, string subject, string body)
        //	{
        //		using (var message = new MailMessage())
        //		{
        //			message.To.Add(to);
        //			message.Subject = subject;
        //			message.Body = body;
        //			message.IsBodyHtml = true;

        //			using (var client = new SmtpClient(SmtpServer, SmtpPort))
        //			{
        //				client.UseDefaultCredentials = false;
        //				client.Credentials = new NetworkCredential(SmtpUsername, SmtpPassword);
        //				client.EnableSsl = SmtpEnableSsl;

        //				await client.SendMailAsync(message);
        //			}
        //		}
        //	}
        //}

        public bool SendEmail(EmailSetting setting)
        {
            string username = "info@cloudhawktech.com";
            string password = "*?mD3NuO(8@8";
            ICredentialsByHost credentials = new NetworkCredential(username, password);

            SmtpClient smtpClient = new SmtpClient()
            {
                Host = "mail.cloudhawktech.com",
                Port = 25,
                EnableSsl = false,
                Credentials = credentials
            };

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(username);
            mail.To.Add(setting.ToEmail);
            mail.Subject = setting.EmailBody;
            mail.Body = setting.EmailString;

            smtpClient.Send(mail)
;
            return true;
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
		{

			var user = await _applicationDb.UserModels.FirstOrDefaultAsync(u => u.Email == email);
			var message = "";
			if (user != null)
            {
                string EncruptedString = user.Email + "&&$" + DateTime.Now + "&&$" + user.UserId;
                var Encrupted = Crypto.Encrypt(EncruptedString);
                var en = Encrupted.Replace("+", "mdmd");
                string APIsString = "https://localhost:7182/User/ForgotPassword?Token=" + en;
                
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Dear!");
                sb.AppendLine("Member your forget password request for ITCP!");
                sb.AppendLine("If you want to reset your account then click on blew link.");
                sb.AppendLine(APIsString);
                sb.AppendLine("Regards");
                sb.AppendLine("ITCP Admin");
                EmailSetting setting = new EmailSetting()
                {
                    ToEmail = user.Email,
                    EmailString = sb.ToString(),
                    EmailBody = "Forget Password Request of ITCP."
                };
                SendEmail(setting);
                message = "Email are send successfully.";
            }
            else
            {
                message = "Problem occure while sending email.";
            }
            return Ok(message);
        }
  //      [HttpPost]
		//public async Task<IActionResult> ForgotPassword(string email)
		//{
			
		//	var user = await _applicationDb.UserModels.FirstOrDefaultAsync(u => u.Email == email);
		//	if (user == null)
		//	{
				
		//		return View("ForgotPasswordConfirmation");
		//	}

		//	string encryptedString = user.Email + "&&$" + DateTime.Now + "&&$" + user.UserId;
		//	var encryptedData = Crypto.Encrypt(encryptedString);

		//	var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.UserId, token = encryptedData }, Request.Scheme);

			
		//	var message = new StringBuilder();
		//	message.AppendLine("Dear!");
		//	message.AppendLine("Member your forget password request for ITCP!");
		//	message.AppendLine("If you want to reset your account then click on below link.");
		//	message.AppendLine(callbackUrl);
		//	message.AppendLine("Regards");
		//	message.AppendLine("ITCP Admin");
  //          EmailSetting setting= new EmailSetting();

  //          await SendEmail(setting.Emailbody);

		//	// Set the password reset expiration time and save changes
		//	user.ResetPasswordExpiration = DateTime.UtcNow.AddHours(24);
		//	await _applicationDb.SaveChangesAsync();

		//	return View("ForgotPasswordConfirmation");
		//}

		[HttpGet]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		


	}
}
