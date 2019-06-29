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
        private CommentService _commentService;

        public NewsController(NewsService newsService,CommentService commentService)
        {
            _newsService = newsService;
            _commentService = commentService;
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
            if (classify.code == 0)
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

        public IActionResult Detail(int id)
        {
            ViewData["Title"] = "详情";
            if (id < 0)
                Response.Redirect("Home/Index");
            var news = _newsService.GetOneNews(id);
            var classify = _newsService.GetOneNewsClassify(id);
            ViewData["News"] = new ResponseModel();
            ViewData["RelevantNews"] = new ResponseModel();
            ViewData["Comments"] = new ResponseModel();
            if (news.code == 0)
            {
                Response.Redirect("Home/Index");
            }
            else
            {
                ViewData["Title"] = news.data.Title;
                ViewData["News"] = news;
                var relevantNews = _newsService.GetRelevantNews(id);
                ViewData["RelevantNews"] = relevantNews;
                var comments = _commentService.GetCommentList(c => c.NewsId == id);
                ViewData["Comments"] = comments;
            }

            return View(_newsService.GetNewsClassifyList());
        }
    }
}