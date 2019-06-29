using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Model.Response;
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
            ViewData["ClassifyName"] = "首页";
            ViewData["NewsList"] = new ResponseModel();
            ViewData["NewCommentNews"] = new ResponseModel();
            ViewData["Title"] = "首页";
            if (classify.code <= 0)
            {
                
                Response.Redirect("Home/Index");
            }
            else
            {
                ViewData["ClassifyName"] = classify.data.Name;
                int count = _newsService.GetNewsCount(c => c.NewsClassifyId == id).data;
                var newsList = _newsService.GetNewsList(c => c.NewsClassifyId == id, count);
                ViewData["NewsList"] = newsList;
                var newCommentNewsList = _newsService.GetNewCommentNewsList(c => c.NewsClassifyId == id);
                ViewData["NewCommentNews"] = newCommentNewsList;

                ViewData["Title"] = classify.data.Name;
            }
            
            return View(_newsService.GetNewsClassifyList());
        }
    }
}