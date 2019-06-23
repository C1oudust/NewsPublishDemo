using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Model.Response;
using NewsPublish.Service;

namespace NewsPublish.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CommentController : Controller
    {
        private CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }
        public IActionResult Index()
        {
            var commentList = _commentService.GetCommentList(c => true);
            return View(commentList);
        }

        [HttpPost]
        public JsonResult DelComment(int id)
        {
            if (id < 0)
            {
                return Json(new ResponseModel {code = 0, result = "Comment get error!"});
            }

            return Json(_commentService.DeleteComment(id));
        }
    }
}