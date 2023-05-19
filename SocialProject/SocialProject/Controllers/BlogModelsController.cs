using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SocialProject.Data;
using SocialProject.Models;

namespace SocialProject.Controllers
{
    public class BlogModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PostModelsController> _logger;
        private readonly IWebHostEnvironment _environment;
        public BlogModelsController(ApplicationDbContext context, ILogger<PostModelsController> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        // GET: BlogModels
        public async Task<IActionResult> Index()
        {

			var fullname = HttpContext.Session.Get("FullName");

			if (fullname == null)
				return RedirectToAction("Login", "User");
			return _context.Blogs != null ? 
                          View(await _context.Blogs.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Blogs'  is null.");
        }
        //public static string Truncate(string value, int numWords)
        //{
        //    if (string.IsNullOrEmpty(value)) return value;

        //    string[] words = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //    if (words.Length <= numWords)
        //    {
        //        return value;
        //    }
        //    else
        //    {
        //        return string.Join(" ", words.Take(numWords)) + "...";
        //    }
        //}

        public async Task<IActionResult> ActiveTopic()
        {
            return _context.Blogs != null ?
                          View(await _context.Blogs.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Blogs'  is null.");
           // return View();
        }
        // GET: BlogModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var fullname = HttpContext.Session.Get("FullName");

            if (fullname == null)
                return RedirectToAction("Login", "User");
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blogModel = await _context.Blogs
                 .Include(b => b.UserComment)
                     .ThenInclude(c => c.User)
                 .FirstOrDefaultAsync(m => m.BlogId == id);
            if (blogModel == null)
            {
                return NotFound();
            }

            return View(blogModel);
        }



        // GET: BlogModels/Create
        public IActionResult Create()
        {
            var username = HttpContext.Session.Get("AdminUserName");

            if (username == null)
                return RedirectToAction("Login", "Admin");
            return View();
        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Title,Description,CreateBy")] BlogModel blogPost, IList<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                blogPost.CreateBy = HttpContext.Session.GetString("AdminUserName");
				blogPost.CreateDate = DateTime.Now;
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

                            if (blogPost.Attachment == "" || blogPost.Attachment == null)
                            {
                                blogPost.Attachment = fileName;
                            }
                            else
                            {

                                blogPost.Attachment = blogPost.Attachment + "," + fileName;
                            }
                        }
                    }

                }
                _context.Add(blogPost);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(ActiveTopic));
			}
			return View(blogPost);
		}

		// GET: BlogModels/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blogModel = await _context.Blogs.FindAsync(id);
            if (blogModel == null)
            {
                return NotFound();
            }
            return View(blogModel);
        }

        // POST: BlogModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int BlogId, [Bind("BlogId,Title,Description,Location,Attachment,CreateBy,CreateDate,UpdateBy,UpdateDate,Status,UserID,FullName")] BlogModel blogModel)
        {
            if (BlogId != blogModel.BlogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogModelExists(blogModel.BlogId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ActiveTopic));
            }
            return View(blogModel);
        }

        // GET: BlogModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blogModel = await _context.Blogs
                .FirstOrDefaultAsync(m => m.BlogId == id);
            if (blogModel == null)
            {
                return NotFound();
            }

            return View(blogModel);
        }

        // POST: BlogModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int BlogId)
        {
            if (_context.Blogs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Blogs' is null.");
            }

            // Check if there are any related comments
            var comments = await _context.UserComments.Where(c => c.BlogId == BlogId).ToListAsync();

            if (comments.Count > 0)
            {
                // Delete the related comments
                _context.UserComments.RemoveRange(comments);
            }

            var blogModel = await _context.Blogs.FindAsync(BlogId);
            if (blogModel != null)
            {
                _context.Blogs.Remove(blogModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ActiveTopic));
        }


        private bool BlogModelExists(int id)
        {
          return (_context.Blogs?.Any(e => e.BlogId == id)).GetValueOrDefault();
    
        }

        [HttpGet]
        public IActionResult AddComment(int blogId)
        {
            var fullname = HttpContext.Session.Get("FullName");

            if (fullname == null)
                return RedirectToAction("Login", "User");
            int? userId = HttpContext.Session.GetInt32("UserId");
           // UserId=userId;
            var comment = new UserCommentModel
            {
                UserId = userId,
            BlogId = blogId
            };
            return View(comment);
        }
         
       

        [HttpPost]
        public IActionResult AddComment(UserCommentModel Comment, int blogId, string body)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var blog = _context.Blogs.FirstOrDefault(b => b.BlogId == blogId);
            var user = _context.UserModels.FirstOrDefault(u => u.UserId == userId);

            if (blog == null || user == null)
            {
                return NotFound();
            }

            Comment.UserId = userId;
            Comment.User = user;
            Comment.BlogId = blogId;
            Comment.FullName=user.FullName;
            Comment.Body = body;
            Comment.CreateDate = DateTime.Now;

            _context.UserComments.Add(Comment);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new { blogId });
        }




    }

}
