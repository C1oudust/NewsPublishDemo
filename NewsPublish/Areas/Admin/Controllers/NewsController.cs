using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;
using NewsPublish.Service;

namespace NewsPublish.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewsController : Controller
    {
        private NewsService _newsService;

        private IHostingEnvironment _host;

        public NewsController(NewsService newsService,IHostingEnvironment host)
        {
            this._host = host;
            this._newsService = newsService;
        }

        public ActionResult Index()
        {
            var newsClassifies = _newsService.GetNewsClassifyList();
            return View(newsClassifies);
        }
        
        /// <summary>
        /// 使用ajax传递新闻文章的值s
        /// </summary>
        [HttpGet]
        public JsonResult GetNews(int pageIndex,int pageSize,int classifyId,string keyword)
        {
            List<Expression<Func<News,bool>>>  where = new List<Expression<Func<News, bool>>>();
            if (classifyId > 0)
            {
                where.Add(c => c.NewsClassifyId == classifyId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                where.Add(c => c.Title.Contains(keyword));
            }

            var newsList = _newsService.NewsPageQuery(pageSize, pageIndex, out var total, where);
            return Json(new
            {
                total, newsList.data
            });
        }

        public ActionResult NewsAdd()
        {
            var newsClassifies = _newsService.GetNewsClassifyList();
            return View(newsClassifies);
        }

        [HttpPost]
        public async Task<JsonResult> AddNews(AddNews news ,IFormCollection collection)
        {
            if (news.NewsClassifyId <= 0 || string.IsNullOrEmpty(news.Title) || string.IsNullOrEmpty(news.Contents))
            {
                return Json(new ResponseModel {code = 0, result = "parameter error!"});
            }

            var files = collection.Files;
            if (files.Count > 0)
            {
                string webRootPath = _host.WebRootPath;
                string relativeDirPath = "\\NewsPic";
                string absolutePath = webRootPath + relativeDirPath;
                string[] fileTypes = new[] {".gif", ".jpg", ".jpeg", ".png", ".bmp"};
                string extension = Path.GetExtension(files[0].FileName);
                if (fileTypes.Contains(extension))
                {
                    if (!Directory.Exists(absolutePath))
                    {
                        Directory.CreateDirectory(absolutePath);
                    }
                    var fileName = DateTime.Now.ToString("yyyyMMMMddHHms") + extension;
                    var filePath = absolutePath + "\\" + fileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await files[0].CopyToAsync(stream);
                    }

                    news.Image = "/NewsPic/" + fileName;
                    return Json(_newsService.AddNews(news));
                }
                return Json(new ResponseModel { code = 0, result = "image format error!" });
            }
            return Json(new ResponseModel { code = 0, result = "please upload image!" });
        }

        [HttpPost]
        public JsonResult DelNews(int id)
        {
            if (id < 0)
            {
                return Json(new ResponseModel { code = 0, result = "News not exist!" });
            }
            return Json(_newsService.DeleteOneNews(id));
        }
        #region 新闻类别

        public ActionResult NewsClassify()
        {
            var newsClassify = _newsService.GetNewsClassifyList();
            return View(newsClassify);
        }

        public ActionResult NewsClassifyAdd()
        {
            return View();
        }

        [HttpPost]
        public JsonResult NewsClassifyAdd(AddNewsClassify newsClassify)
        {
            if (string.IsNullOrEmpty(newsClassify.Name))
            {
                return Json(new ResponseModel{code = 0,result = "please input news classify!"});
            }

            return Json(_newsService.AddNewsClassify(newsClassify));
        }

        public ActionResult NewsClassifyEdit(int id)
        {
            return View(_newsService.GetOneNewsClassify(id));
        }

        [HttpPost]
        public JsonResult NewsClassifyEdit(EditNewsClassify newsClassify)
        {
            if (string.IsNullOrEmpty(newsClassify.Name))
            {
                return Json(new ResponseModel {code = 0, result = "please input news classify!"});
            }

            return Json(_newsService.EditNewsClassify(newsClassify));
        }
        #endregion
    }

}