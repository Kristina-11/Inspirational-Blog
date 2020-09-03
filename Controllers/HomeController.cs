using Blog.Data;
using Blog.Data.FileManager;
using Blog.Data.Repository;
using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private IRepository _repo;
        private IFileManager _fileManager;

        public HomeController(IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _fileManager = fileManager;
        }

        public IActionResult Index(string category)
        {
            var posts = string.IsNullOrEmpty(category) ? _repo.GetAllPosts() : _repo.GetAllPosts(category);
            return View(posts);
        }

        public IActionResult Post(int id)
        {
            var post = _repo.GetPost(id);
            return View(post);
        }

        [HttpGet("/Image/{image}")]
        [ResponseCache(CacheProfileName = "Monthly")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf(".") + 1);
            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }

        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel cvm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { id = cvm.PostId });
            }

            var post = _repo.GetPost(cvm.PostId);
            if (cvm.PostId > 0) 
            {
                post.Comments = post.Comments ?? new List<Comment>();
                post.Comments.Add(new Comment 
                { 
                    VisitorName = cvm.VisitorName,
                    Message = cvm.Message,
                    Created = DateTime.Now
                });

                if (string.IsNullOrEmpty(cvm.Message))
                {
                    Console.WriteLine("You can't leave message field empty.");
                }

                _repo.UpdatePost(post);
            }

            await _repo.SaveChangesAsync();

            return RedirectToAction("Post", new { id = cvm.PostId });
        }
    }
}
