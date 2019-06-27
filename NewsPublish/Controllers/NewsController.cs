using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Service;

namespace NewsPublish.Controllers
{
    public class NewsController : Controller
    {   
        private NewsService _newsService;

        public NewsController(NewsService newsService)
        {
            _newsService = newsService;
        }
        public IActionResult Classify(int id)
        {   
            if(id < 0)
                Response.Redirect("Home/Index");
            var classify = _newsService.GetOneNewsClassify(id);
            if (classify.code <= 0)
            {
                Response.Redirect("Home/Index");
            }

            ViewData["ClassifyName"] = classify.data.Name;

            var newsList = _newsService.GetNewsList(c => c.NewsClassifyId == id, 1);
            ViewData["NewsList"] = newsList;

            var newCommentNewsList = _newsService.GetNewCommentNewsList(c => c.NewsClassifyId == id, 1);
            ViewData["NewCommentNews"] = newCommentNewsList;

            ViewData["Title"] = "首页";
            return View(_newsService.GetNewsClassifyList());
        }
    }
}