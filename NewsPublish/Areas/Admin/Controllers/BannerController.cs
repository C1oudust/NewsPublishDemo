using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;
using NewsPublish.Service;


namespace NewsPublish.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : Controller
    {
        private BannerService _bannerService;
        private IHostingEnvironment _host;

        public BannerController(BannerService bannerService, IHostingEnvironment host)
        {
            _bannerService = bannerService;
            _host = host;
        }

        // GET: Banner
        public ActionResult Index()
        {
            var banner = _bannerService.GetBannerList();
            return View(banner);
        }

        public ActionResult BannerAdd()
        {
            var banner = _bannerService.GetBannerList();
            return View(banner);
        }

        [HttpPost]
        public async Task<JsonResult> AddBanner(AddBanner banner, IFormCollection collection)
        {
            var files = collection.Files;
            if (files.Count > 0)
            {
                string webRootPath = _host.WebRootPath;
                string relativeDirPath = "\\BannerPic";
                string absolutePath = webRootPath + relativeDirPath;
                string[] fileType = new[] {".jpg", ".gif", ".jpeg", ".png", ".bmp"};
                string extension = Path.GetExtension(files[0].FileName);
                if (fileType.Contains(extension.ToLower()))
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

                    banner.Image = "/BannerPic/" + fileName;
                    return Json(_bannerService.AddBanner(banner));
                }
                return Json(new ResponseModel { code = 0, result = "Image format error!"});
            }
            return Json(new ResponseModel { code = 0, result = "please upload image!"});
        }

        [HttpPost]
        public JsonResult DelBanner(int id)
        {
            //TODO:需要添加删除数据库记录的同时删除文件夹里的图片文件功能
            if (id <= 0) return Json(new ResponseModel {code = 0, result = "parameter error"});
            return Json(_bannerService.DeleteBanner(id));
        }

    }
}