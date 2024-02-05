using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Migrations;
using MyBlog.Models;

namespace MyBlog.Controllers
{
	[Authorize]
    public class PostController : Controller
    {
        private readonly AppDbContext _context;

		public PostController(AppDbContext  c)
		{
			_context=c;
		}

		public IActionResult Index(int page = 1)
        {
			ViewBag.PrevPage = (page > 1) ? page - 1 : 1; 
			ViewBag.NextPage = page + 1;
            int dataPerpage = 10;
            int skip = dataPerpage * page - dataPerpage;
			List<Post> data = _context.Posts.ToList();
            List<Post> filtreddata = data
                .Skip(skip)
                .Take(dataPerpage)
                .OrderBy(post =>post.Id)
                .ToList();
            return View(filtreddata);
        }

		public IActionResult Detail(int id)
		{
			Post data = _context.Posts.Where(Post=>Post.Id == id).FirstOrDefault();
			return View(data);
		}

		public IActionResult Create() 
		{
			return View();
		}
		[HttpPost]
        public IActionResult Create([FromForm] Post data)
        {
			data.CreatedDate = DateTime.Now;
			_context.Posts.Add(data);
			_context.SaveChanges();
            return RedirectToAction("Index");
        }
		public IActionResult Edit(int id) {
			var postData = _context.Posts.FirstOrDefault(x => x.Id == id);

			return View(postData);
		}

		[HttpPost]
		public IActionResult Edit([FromForm] Post data)
		{
			var dataFromDb = _context.Posts.FirstOrDefault(x => x.Id == data.Id);

			if(dataFromDb != null)
			{
                dataFromDb.Title = data.Title;
                dataFromDb.Content = data.Content;
				dataFromDb.Likes = data.Likes;

                _context.Posts.Update(dataFromDb);
                _context.SaveChanges();
            }
			
			return RedirectToAction("Index");
		}
		public IActionResult Delete(int id)
		{
			var dataFormDb = _context.Posts.FirstOrDefault(x => x.Id==id);
			if(dataFormDb != null)
			{
				_context.Posts.Remove(dataFormDb);
				_context.SaveChanges();
			}
			return RedirectToAction("Index");
		}

        public IActionResult TopLikedPosts()
        {
            List<Post> topLikedPosts = _context.Posts.OrderByDescending(post => post.Likes).Take(5).ToList();
            ViewBag.Posts = topLikedPosts; 
            return View();
        }

    }
}