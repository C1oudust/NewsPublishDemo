﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Models;
using NewsPublish.Service;
using Newtonsoft.Json.Schema;

namespace NewsPublish.Controllers
{
    public class HomeController : Controller
    {
        private NewsService _newsService;
        private BannerService _bannerService;

        public HomeController(NewsService newsService, BannerService bannerService)
        {
            _newsService = newsService;
            _bannerService = bannerService;
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "首页";
            return View(_newsService.GetNewsClassifyList());
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpGet]
        public JsonResult GetBanner()
        {
            var bannerList = _bannerService.GetBannerList();
            return Json(bannerList);
        }

        [HttpGet]
        public JsonResult GetTotalNews()
        {
            return Json(_newsService.GetNewsCount(c => true));
        }

        [HttpGet]
        public JsonResult GetHomeNews()
        {
            return Json(_newsService.GetNewsList(c => true, 4));
        }

        [HttpGet]
        public JsonResult GetNewCommentNews()
        {
            return Json(_newsService.GetNewCommentNewsList(c => true, 5));
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

    }
}
