using Blog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Repository
{
    public class Repository : IRepository
    {
        private AppDbContext _repo;

        public Repository(AppDbContext repo)
        {
            _repo = repo;
        }
        public void AddPost(Post post)
        {
            _repo.Posts.Add(post);
        }

        public List<Post> GetAllPosts()
        {
            return _repo.Posts.ToList();
        }

        public List<Post> GetAllPosts(string category)
        {
            return _repo.Posts
                .Where(post => post.Category.ToLower().Equals(category.ToLower()))
                .ToList();
        }

        public Post GetPost(int id)
        {
            return _repo.Posts
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == id);
        }

        public void RemovePost(int id)
        {
            _repo.Posts.Remove(GetPost(id));
        }

        public void UpdatePost(Post post)
        {
            _repo.Posts.Update(post);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if(await _repo.SaveChangesAsync() > 0)
            {
                return true;
            }

            return false;
        }
    }
}
