using SocialProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SocialProject.Data;


namespace SocialProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }
		public IActionResult Setting()
		{
			return View();
		}
        public async Task<IActionResult> Blogs()
        {
            return _context.Blogs != null ?
                        View(await _context.Blogs.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Blogs'  is null.");
        }
        public IActionResult BlogsDetail()
		{
			return View();
		}
		public IActionResult Events()
		{
			return View();
		}
		public async Task<IActionResult> Notifications()
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			// Retrieve the notifications for the current user
			if (userId != null)
			{
				var notifications = await _context.Notifications.Where(n => n.ReceiverId == userId && !n.IsRead).ToListAsync();

				// Mark the notifications as read
				foreach (var notification in notifications)
				{
					notification.IsRead = true;
				}
				await _context.SaveChangesAsync();

				ViewBag.NotificationsCount = notifications.Count; // Pass the count of notifications to ViewBag

				// Pass the notifications to the partial view
				return PartialView("_Notifications", notifications);
			}

			ViewBag.NotificationsCount = 0; // If there are no notifications, set the count to 0
			return PartialView("_Notifications", new List<NotificationModel>());
		}

		[HttpGet]
		public async Task<IActionResult> NotificationsData()
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId != null)
			{
				var notifications = await _context.Notifications.Where(n => n.ReceiverId == userId && !n.IsRead).ToListAsync();
				return Json(notifications);
			}

			return Json(new List<NotificationModel>());
		}

		//public async Task<IActionResult> Notifications()
		//{
		//	int? userId = HttpContext.Session.GetInt32("UserId");

		//	// Retrieve the notifications for the current user
		//	if (userId != null)
		//	{
		//		var notifications = await _context.Notifications.Where(n => n.ReceiverId == userId && n.IsRead).ToListAsync();

		//		// Mark the notifications as read
		//		foreach (var notification in notifications)
		//		{
		//			notification.IsRead = true;
		//		}
		//		await _context.SaveChangesAsync();

		//		// Pass the notifications to the view
		//		ViewBag.Notifications = notifications;

		//		return View();
		//	}
		//	return View();
		//}

		public IActionResult MyProfile(int? userId)

		{
			string? Email=HttpContext.Session.GetString("Email");
			string? Address = HttpContext.Session.GetString("Address");

			userId = HttpContext.Session.GetInt32("UserId");
			var userPosts = _context.PostModel.Where(up => up.UserId == userId).ToList();
			ViewBag.userPosts = userPosts;
			ViewBag.comments = _context.Comments.ToList();
			return View(new PostModel());


		}


		//public IActionResult MyProfile(int? userId)
		//{
		//	userId = HttpContext.Session.GetInt32("UserId");
		//	var userPosts = from u in _context.UserModels
		//					join p in _context.PostModel on u.UserId equals p.UserId
		//					where u.UserId == userId
		//					select new { u, p };

		//	List<UserPost> userTimeline = new List<UserPost>();

		//	foreach (var up in userPosts)
		//	{
		//		userTimeline.Add(new UserPost
		//		{
		//			UserId = up.u.UserId,
		//			FirstName = up.u.FirstName,
		//			LastName = up.u.LastName,
		//			Email = up.u.Email,
		//			Password = up.u.Password,
		//			Content = up.p.Content,
		//			CreatedAt = up.p.CreatedAt
		//		});
		//	}

		//	return View(userTimeline);
		//}

		public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult FAQs()
        {

              return View();
        }
        [HttpGet]
        public async Task<IActionResult> Setting(int? id)
        {
			id = HttpContext.Session.GetInt32("UserId");
			if (id == null)
            {
                return NotFound();
            }

            var user = await _context.UserModels.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
           // ViewData["UserId"] = user.UserId;
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setting(int? id, [Bind("UserId,UserName,FullName,Email,Password,Attachment,Gender,UserRole,Address,CreateBy,CreateDate,UpdateBy,UpdateDate")] UserModel user)
        {
            id = HttpContext.Session.GetInt32("UserId");

            if (id == null || id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing user entity from the context
                    var existingUser = await _context.UserModels.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Update the properties of the existing user entity
                   // existingUser.UserName = user.UserName;
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.Password = user.Password;
                   // existingUser.Gender = user.Gender;
                    existingUser.UserRole = user.UserRole;
                    existingUser.Address = user.Address;
                  //  existingUser.UpdateBy = user.UpdateBy;
                   // existingUser.UpdateDate = user.UpdateDate;

                    // If a new attachment was selected, update the Attachment property
                    if (user.Attachment != null)
                    {
                        existingUser.Attachment = user.Attachment;
                    }

                    // Update the context with the changes
                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserModelExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("MyProfile", "Home");
            }

            return View(user);
        }

        private bool UserModelExists(int id)
        {
            return _context.UserModels.Any(e => e.UserId == id);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}