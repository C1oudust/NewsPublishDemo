using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;

namespace NewsPublish.Service
{
    public class CommentService
    {
        private Db _db;
        private NewsService _newsService;

        public CommentService(Db db, NewsService newsService)
        {
            this._db = db;
            this._newsService = newsService;
        }

        public ResponseModel AddComment(AddComment comment)
        {
            var news = _newsService.GetOneNews(comment.NewsId);
            if (news.code == 0)
            {
                return new ResponseModel { code = 0, result = "News inexistence!" };
            }

            var com = new NewsComment
            { AddTime = DateTime.Now.ToString(), NewsId = comment.NewsId, Contents = comment.Contents };
            _db.NewsComment.Add(com);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel
                {
                    code = 200,
                    result = "Comment add success!",
                    data = new //便于前端调用
                    {
                        contents = comment.Contents,
                        floor = "#" + news.data.CommentCount + 1,
                        addTime = DateTime.Now.ToString()
                    }
                };
            }
            return new ResponseModel { code = 0, result = "Comment add failed!" };

        }

        public ResponseModel DeleteComment(int id)
        {
            var comment = _db.NewsComment.Find(id);
            if (comment == null)
            {
                return new ResponseModel
                {
                    code = 0,
                    result = "Comment not exist!"
                };
            }

            _db.NewsComment.Remove(comment);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel { code = 200, result = "Comment  delete success!" };
            }
            return new ResponseModel { code = 0, result = "Comment delete failed!" };
        }
        /// <summary>
        /// 获取评论集合
        /// </summary>
        public ResponseModel GetCommentList(Expression<Func<NewsComment,bool>> where)
        {
            var comments = _db.NewsComment
                .Include("News")
                .Where(where)
                .OrderBy(c => c.AddTime)
                .ToList();
            var response = new ResponseModel();
            response.code = 200;
            response.result = "Comment get success!";
            response.data = new List<CommentModel>();
            int floor = 1;
            foreach (var comment in comments)
            {
                response.data.Add(new CommentModel
                {
                    Id = comment.Id,
                    AddTime = comment.AddTime,
                    NewsName = comment.News.Title,
                    Contents = comment.Contents,
                    Remark = comment.Remark,
                    Floor = "#" + floor
                });  
                floor++;
            }
            response.data.Reverse();
            return response;
        }
    }
}
