using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SocialProject.Data;
using SocialProject.Models;
using static System.Net.WebRequestMethods;

namespace SocialProject.Controllers
{
	public class PostModelsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<PostModelsController> _logger;
		private readonly IWebHostEnvironment _environment;
		public PostModelsController(ApplicationDbContext context, ILogger<PostModelsController> logger, IWebHostEnvironment environment)
		{
			_context = context;
			_logger = logger;
			_environment = environment;
		}
        public async Task<IActionResult> IndexAsync()
        {
            var username = HttpContext.Session.Get("AdminUserName");
            if (username == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            var activatePosts = await _context.PostModel.Where(p => p.Status == "activate").ToListAsync();

            if (activatePosts == null) // Check if activatePosts is null
            {
                activatePosts = new List<PostModel>(); // Initialize activatePosts to an empty list
            }

            return View(activatePosts);
        }


        //          return _context.PostModel != null ?
        //				View(await _context.PostModel.ToListAsync()) :
        //				Problem("Entity set 'ApplicationDbContext.PostModel'  is null.");

        //}
        //	public async Task<IActionResult> IndexAsync(int postId)
        //	{
        //		var username = HttpContext.Session.Get("AdminUserName");

        //		if (username == null)
        //			return RedirectToAction("Login", "Admin");
        //		// Get approved posts
        //		var activatePosts =  await _context.PostModel.Include(p => p.Likes).Where(p => p.Status == "activate").FirstOrDefaultAsync(p => p.PostId == postId);

        //		return View(activatePosts);
        //	}
        // GET: PostModels/Details/5
        public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.PostModel == null)
			{
				return NotFound();
			}

			var postModel = await _context.PostModel
				.FirstOrDefaultAsync(m => m.PostId == id);
			if (postModel == null)
			{
				return NotFound();
			}

			return View(postModel);
		}

		// GET: PostModels/Create
		public IActionResult Create(int postId)
		{
			var fullname = HttpContext.Session.Get("FullName");

			if (fullname == null)
				return RedirectToAction("Login", "User");
			var activatePosts = _context.PostModel.Where(p => p.Status == "activate").ToList();

			ViewBag.activatePosts = activatePosts;
			//var post = _context.PostModel.Include(p => p.Comments).FirstOrDefault(p => p.PostId == postId);

			ViewBag.comments = _context.Comments.ToList();
			

			return View(new PostModel());
		}

		// POST: PostModels/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(PostModel postModel,int postId, IList<IFormFile> files)
		{
			int? userId = HttpContext.Session.GetInt32("UserId");
			string? fullName = HttpContext.Session.GetString("FullName");
			string? createdby = HttpContext.Session.GetString("Attachment");

			if (userId.HasValue && !string.IsNullOrEmpty(fullName))
			{
				postModel.UserId = userId.Value;
				postModel.FullName = fullName; // add this line to assign the value to the postModel
				postModel.CreateBy = createdby;
                postModel.CreateDate = DateTime.Now;

                foreach (var file in files)
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

							if (postModel.Attachment == "" || postModel.Attachment == null)
							{
								postModel.Attachment = fileName;
							}
							else
							{

								postModel.Attachment = postModel.Attachment + "," + fileName;
							}
						}
					}

				}
			}


			//postModel.LikesCount = 0;


			_context.Add(postModel);
			await _context.SaveChangesAsync();
			{
				TempData["message"] = "Need admin approval: Your post has been recorded successfully";
			}
			return RedirectToAction(nameof(Create));



		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Activate(int id)
		{
            int? userId = HttpContext.Session.GetInt32("AdminId");
			// string?  Attachment = HttpContext.Session.GetString("Attachment");
			String? adminpic = "/assets/images/icon/admin.png";
		   string ? AdminUserName = HttpContext.Session.GetString("AdminUserName");
			var post = await _context.PostModel.FindAsync(id);
			if (post != null)
			{
				post.Status = "Activate";
				_context.Update(post);
				await _context.SaveChangesAsync();
			}
			if (post.UserId != userId)
			{
				var notification = new NotificationModel
                {
                    Message = $"Admin Accept your post",
                    Type = NotificationType.PostAccepted,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    SenderId = userId,
                    ReceiverId = post.UserId,
                   // SenderName = AdminUserName,
                    ReceiverName = post.FullName,
                    PostId = id,
                    Senderpic = adminpic,
                };
                await _context.Notifications.AddAsync(notification);
            }
			await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var post = await _context.PostModel.FindAsync(id);

			if (post == null)
			{
				return NotFound();
			}

			var postViewModel = new PostModel
			{
				PostId = post.PostId,
				Title = post.Title,
				Description = post.Description,
				Location = post.Location,
				Activity = post.Activity,
				Attachment = post.Attachment
			};

			return View(postViewModel);
		}

		// POST: Posts/Edit/5

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int postId, [Bind("PostId,Title,Description,Location,Activity,Attachment,CreateBy,CreateDate,UpdateBy,UpdateDate,Status,UserId")] PostModel postModel)
		{
			if (postId != postModel.PostId)
			{
				return NotFound();
			}
			var existingPost = await _context.PostModel.FindAsync(postId);

			existingPost.Title = postModel.Title;
			existingPost.Description = postModel.Description;
			existingPost.Location = postModel.Location;
			existingPost.Activity = postModel.Activity;
			existingPost.Attachment = postModel.Attachment;
			existingPost.UpdateBy = postModel.UpdateBy;
			existingPost.UpdateDate = postModel.UpdateDate;
			existingPost.Status = postModel.Status;

			if (ModelState.IsValid)
			{
				_context.Update(existingPost);
				await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(postModel);
		}

		private bool PostExists(int id)
		{
			return _context.PostModel.Any(e => e.PostId == id);
		}


		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.PostModel == null)
			{
				return NotFound();
			}

			var postModel = await _context.PostModel
				.FirstOrDefaultAsync(m => m.PostId == id);
			if (postModel == null)
			{
				return NotFound();
			}

			return View(postModel);
		}

		// POST: PostModels/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int postId)
        {
            // Get the post and its associated comments
            var post = await _context.PostModel.Include(p => p.Comments).FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
            {
                TempData["error"] = "Post not found.";
                return RedirectToAction(nameof(Index));
            }

            // Delete the likes associated with the post
            var likes = _context.Likes.Where(l => l.PostId == postId);
            _context.Likes.RemoveRange(likes);

            // Delete the comments
            foreach (var comment in post.Comments)
            {
                _context.Comments.Remove(comment);
            }

            // Delete the post
            _context.PostModel.Remove(post);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            TempData["success"] = "Post deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AdminDashboard()
		{
			var username = HttpContext.Session.Get("AdminUserName");

			if (username == null)
				return RedirectToAction("Login", "Admin");

			// Get all pending posts
			var pendingPosts = await _context.PostModel
				.Where(p => p.Status == "Inactive")
				.ToListAsync();

			// Pass the pending posts to the view
			return View(pendingPosts);
		}

		private bool PostModelExists(int id)
		{
			return (_context.PostModel?.Any(e => e.PostId == id)).GetValueOrDefault();
		}



		public IActionResult PIndex()
		{
			var posts = _context.PostModel.ToList();
			ViewBag.comments = _context.Comments.ToList();
			return View(new PostCommentModel());
		}

		//public IActionResult GetPostComments(int postId)
		//{
		//	var post = _context.PostModel.Include(p => p.Comments).FirstOrDefault(p => p.PostId == postId);

		//	//var activatePosts = _context.PostModel.Where(p => p.Status == "activate").ToList();

		//	//ViewBag.activatePosts = activatePosts;
		//	var comments = post.Comments.Select(c => new
		//	{
		//		PostCommentId = c.PostCommentId,
		//		Body = c.Body,
		//		PostId = c.PostId,
		//		FullName = c.FullName,
		//              Attachment=c.Attachment

		//	});

		//	var Comments = _context.PostModel.Include(p => p.Comments).FirstOrDefault(p => p.PostId == postId);
		//	//ViewBag.comments = _context.Comments.ToList();

		//	return View();
		//}
		[HttpGet]
		public IActionResult AddComment()
		{
			return View();
		}

		[HttpPost]
		public IActionResult AddComment(int postId, string body, string? FullName, string? Attachment)
		{
            int? userId = HttpContext.Session.GetInt32("UserId");
            Attachment = HttpContext.Session.GetString("Attachment");
			FullName = HttpContext.Session.GetString("FullName");
			if (string.IsNullOrWhiteSpace(body))
			{
				ModelState.AddModelError("body", "Comment body is required.");
				return View(postId);
			}


			var post = _context.PostModel.FirstOrDefault(p => p.PostId == postId);
			if (post == null)
			{
				return NotFound();
			}

			var comment = new PostCommentModel
			{
				Body = body,
				PostId = postId,
				FullName = FullName,
				Attachment = Attachment

			};

			_context.Comments.Add(comment);
            if (post.UserId != userId)
            {
                var notification = new NotificationModel
                {
                    Message = $"{FullName} Comment on your post",
                    Type = NotificationType.Comment,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    SenderId = userId,
                    ReceiverId = post.UserId,
                    SenderName = FullName,
                    ReceiverName = post.FullName,
                    PostId = postId,
                    Senderpic = Attachment,
                };
                 _context.Notifications.AddAsync(notification);
            }

            _context.SaveChanges();

			return RedirectToAction("Create", new { postId = postId });

		}

		public IActionResult AddCommentForm(int postId)
		{
			return View(postId);
		}

		 

		[HttpPost]
		public async Task<IActionResult> LikePost(int postId)
		{
			// Get the authenticated user
			int? userId = HttpContext.Session.GetInt32("UserId");
			string? fullname = HttpContext.Session.GetString("FullName");
            string? pic = HttpContext.Session.GetString("Attachment");
            // Check if the post exists
            var post = await _context.PostModel.FindAsync(postId);
			if (post == null)
			{
				TempData["error"] = "Post not found.";
				return RedirectToAction(nameof(Index));
			}

			// Check if the user has already liked the post
			var like = await _context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

			if (like == null)
			{
				// Create a new like
				like = new LikeModel { PostId = postId, UserId = userId };
				await _context.Likes.AddAsync(like);

				// Increment the LikesCount property of the PostModel entity
				if (post.LikesCount == null)
				{
					post.LikesCount = 1;
				}
				else
				{
					post.LikesCount++;
				}

				if (post.UserId != userId)
				{
					var notification = new NotificationModel
					{
						Message = $"{userId} liked your post",
						Type = NotificationType.Like,
						CreatedAt = DateTime.UtcNow,
						IsRead = false,
						SenderId = userId,
						ReceiverId = post.UserId,
						SenderName = fullname,
						ReceiverName = post.FullName,
						PostId = postId,
						Senderpic = pic,
					};
					await _context.Notifications.AddAsync(notification);
				}
				// Save the changes to the database
				await _context.SaveChangesAsync();

				TempData["success"] = "You liked this post!";

				//if (post.UserId != userId)
				//{
				//	var notification = new NotificationModel
				//	{
				//		Message = $"{userId} liked your post",
				//		Type = NotificationType.Like,
				//		CreatedAt = DateTime.UtcNow,
				//		IsRead = false,
				//		SenderId = userId,
				//		ReceiverId = post.UserId
				//	};
				//	await _context.Notifications.AddAsync(notification);
				//}
			}

			else
			{
				// Remove the existing like
				_context.Likes.Remove(like);

				// Decrement the LikesCount property of the PostModel entity
				post.LikesCount--;

				// Save the changes to the database
				await _context.SaveChangesAsync();

				TempData["success"] = "You unliked this post.";
			}

			//return RedirectToAction(nameof(Create));
			return Json(new { success = true, likesCount = post.LikesCount });
		}






	}
}
