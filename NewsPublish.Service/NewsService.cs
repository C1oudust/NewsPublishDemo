using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;

namespace NewsPublish.Service
{
    public class NewsService
    {
        private Db _db;

        public NewsService(Db db)
        {
            this._db = db;
        }

        public ResponseModel AddNewsClassify(AddNewsClassify newsClassify)
        {
            var exist = _db.NewsClassify.FirstOrDefault(c => c.Name == newsClassify.Name) != null;
            if (exist)
            {
                return new ResponseModel {code = 0,result = "This Classify has existed!" };
            }

            var nc = new  NewsClassify{Name = newsClassify.Name,Remark = newsClassify.Remark,Sort = newsClassify.Sort};
            _db.NewsClassify.Add(nc);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel { code = 200, result = "NewsClassify add success!" };
            }
            return new ResponseModel { code = 0, result = "NewsClassify add failed!" };
        }

        public ResponseModel GetOneNewsClassify(int Id)
        {
            var classify = _db.NewsClassify.Find(Id);
            if (classify == null)
            {
                return new ResponseModel{code = 0,result = "This classify has not existed!" };
            }
            return  new ResponseModel
            {
                code = 200,
                result = "Get Classify success!",
                data = new NewsClassifyModel
                {
                    Id = classify.Id,
                    Name = classify.Name,
                    Remark = classify.Remark,
                    Sort = classify.Sort
                }

            };
        }
        /// <summary>
        /// get a newsClassify by expression
        /// </summary>
        private NewsClassify GetOneNewsClassify(Expression<Func<NewsClassify, bool>> where)
        {
            return _db.NewsClassify.FirstOrDefault(where);
        }
        public ResponseModel EditNewsClassify(EditNewsClassify newsClassify)
        {
            var classify = GetOneNewsClassify(c => c.Id == newsClassify.Id);
            if (classify == null)
            {
                return new ResponseModel
                {
                    code = 0,
                    result = "This classify has not existed!"
                };
            }

            classify.Name = newsClassify.Name;
            classify.Remark = newsClassify.Remark;
            classify.Sort = newsClassify.Sort;
            _db.NewsClassify.Update(classify);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel { code = 200, result = "NewsClassify edit success!" };
            }
            return new ResponseModel { code = 0, result = "NewsClassify edit failed!" };
        }

        public ResponseModel GetNewsClassifyList()
        {
            var classifies = _db.NewsClassify.OrderByDescending(c => c.Sort).ToList();
            var response = new ResponseModel
            {
                code = 200,
                result = "Get NewsClassifyList success!"
            };
            response.data = new List<NewsClassifyModel>();
            foreach (var classify in classifies)
            {
                response.data.Add(new NewsClassifyModel
                {
                    Id = classify.Id,
                    Name = classify.Name,
                    Remark = classify.Remark,
                    Sort = classify.Sort
                });
            }
            return response;
        }

        public ResponseModel AddNews(AddNews news)
        {
            var classify = this.GetOneNewsClassify(c => c.Id == news.NewsClassifyId);
            if (classify == null)
            {
                return new ResponseModel{code = 0,result = "This classify has not existed!" };
            }
            var n = new News
            {
                NewsClassifyId = news.NewsClassifyId,
                PublishDate = DateTime.Now.ToString(),
                Title = news.Title,
                Contents = news.Contents,
                Image = news.Image,
                Remark = news.Remark
            };
            _db.News.Add(n);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel { code = 200, result = "News add success!" };
            }
            return new ResponseModel { code = 0, result = "News add failed!" };
        }

        public ResponseModel GetOneNews(int id)
        {
            var news = _db.News
                .Include("NewsClassify")
                .Include("NewsComment")
                .FirstOrDefault(c=>c.Id == id);
            if (news == null)
            {
                return new ResponseModel { code = 0, result = "This news has not existed!" };
            }
            return new ResponseModel
            {
                code = 200,
                result = "Get news success!",
                data = new NewsModel
                {
                    Id = news.Id,
                    PublishDate =Convert.ToDateTime(news.PublishDate).ToString("yyyy-MM-dd"),
                    Title = news.Title,
                    Contents = news.Contents,
                    Image = news.Image,
                    Remark = news.Remark,
                    ClassifyName = news.NewsClassify.Name,
                    CommentCount = news.NewsComment.Count()
                }

            };
        }

        public ResponseModel DeleteOneNews(int id)
        {
            var news = _db.News.FirstOrDefault(c => c.Id == id);
            if (news == null)
            {
                return new ResponseModel { code = 0, result = "This news has not existed!" };
            }

            _db.News.Remove(news);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel { code = 200, result = "News delete success!" };
            }
            return new ResponseModel { code = 0, result = "News delete failed!" };
        }

        /// <summary>
        /// 分页查询新闻
        /// </summary>
        public ResponseModel NewsPageQuery(int pageSize, int pageIndex, out int total,
            List<Expression<Func<News, bool>>> where)
        {
            var list = _db.News
                .Include("NewsClassify")
                .Include("NewsComment");
            foreach (var item in where)
            {
                list = list.Where(item);
            }

            total = list.Count();
            var data = list.OrderByDescending(c => c.PublishDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize)
                .ToList();
            var response = new ResponseModel
            {
                code = 200,
                result = "Get NewsPageQuery success!",
                data = new List<NewsModel>()
            };
            foreach (var news in data)
            {
                response.data.Add(new NewsModel
                {
                    Id = news.Id,
                    PublishDate = Convert.ToDateTime(news.PublishDate).ToString("yyyy-MM-dd"),
                    Title = news.Title,
                    Contents = news.Contents.Length > 50?news.Contents.Substring(0,50) + "...":news.Contents,
                    Image = news.Image,
                    Remark = news.Remark,
                    ClassifyName = news.NewsClassify.Name,
                    CommentCount = news.NewsComment.Count()

                });
            }

            return response;
        }
        /// <summary>
        /// 获取最新发布的新闻
        /// </summary>

        public ResponseModel GetNewsList(Expression<Func<News, bool>> where, int topCount)
        {
            var list = _db.News
                .Include("NewsClassify")
                .Include("NewsComment")
                .Where(where)
                .OrderByDescending(c => c.PublishDate)
                .Take(topCount);
            var response = new ResponseModel
            {
                code = 200,
                result = "Get News list success!",
                data = new List<NewsModel>()
            };
            foreach (var news in list)
            {
                response.data.Add(new NewsModel
                {
                    Id = news.Id,
                    PublishDate = Convert.ToDateTime(news.PublishDate).ToString("yyyy-MM-dd"),
                    Title = news.Title,
                    Contents = news.Contents.Length>50?news.Contents.Substring(0,50)+"...":news.Contents,
                    Image = news.Image,
                    Remark = news.Remark,
                    ClassifyName = news.NewsClassify.Name,
                    CommentCount = news.NewsComment.Count()

                });
            }

            return response;
        }

        /// <summary>
        /// 获取最新评论的新闻集合
        /// </summary>
        public ResponseModel GetNewCommentNewsList(Expression<Func<News, bool>> where, int topCount)
        {
            var newsId = _db.NewsComment
                .OrderByDescending(c => c.AddTime)
                .GroupBy(c => c.NewsId)
                .Select(c => c.Key)
                .Take(topCount);
            var list = _db.News
                .Include("NewsClassify")
                .Include("NewsComment")
                .Where(c => newsId.Contains(c.Id))
                .OrderByDescending(c => c.PublishDate);
            var response = new ResponseModel
            {
                code = 200,
                result = "Get News Comment success!",
                data = new List<NewsModel>()
            };
            foreach (var news in list)
            {
                response.data.Add(new NewsModel
                {
                    Id = news.Id,
                    PublishDate = Convert.ToDateTime(news.PublishDate).ToString("yyyy-MM-dd"),
                    Title = news.Title,
                    Contents = news.Contents.Length > 50 ? news.Contents.Substring(0, 50) : news.Contents,
                    Image = news.Image,
                    Remark = news.Remark,
                    ClassifyName = news.NewsClassify.Name,
                    CommentCount = news.NewsComment.Count()

                });
            }

            return response;
        }

        public ResponseModel SearchOneNews(Expression<Func<News, bool>> where)
        {
            var news = _db.News
                .Where(where)
                .FirstOrDefault();
            if (news == null)
            {
                return new ResponseModel { code = 0, result = "search failed!" };
            }
            return new ResponseModel
            {
                code = 200,
                result = "search success!",
                data = news.Id
            };
        }

        public ResponseModel GetNewsCount(Expression<Func<News, bool>> where)
        {
            var count = _db.News.Where(where).Count();
            return new ResponseModel
            {
                code = 200,
                result = "Get news count success!",
                data = count
            };

        }

        /// <summary>
        /// 根据当然文章类型获取相关类型的文章以作推荐
        /// </summary>
        public ResponseModel GetRelevantNews(int id)
        {
            var news = _db.News.FirstOrDefault(c => c.Id == id);
            if (news == null)
            {
                return new ResponseModel
                {
                    code = 0,
                    result = "relevant inexistence!"
                };
            }

            var newsList = _db.News
                .Include("NewsComment")
                .Where(c => c.NewsClassifyId == news.NewsClassifyId && c.Id != id)
                .OrderByDescending(c => c.PublishDate)
                .ThenByDescending(c => c.NewsComment.Count)
                .Take(6)
                .ToList();
            var response = new ResponseModel
            {
                code = 200,
                result = "Get News Comment success!",
                data = new List<NewsModel>()
            };
            foreach (var n in newsList)
            {
                response.data.Add(new NewsModel
                {
                    Id = n.Id,
                    PublishDate = Convert.ToDateTime(n.PublishDate).ToString("yyyy-MM-dd"),
                    Title = n.Title,
                    Contents = n.Contents.Length > 50 ? n.Contents.Substring(0, 50) : n.Contents,
                    Image = n.Image,
                    Remark = n.Remark,
                    ClassifyName = n.NewsClassify.Name,
                    CommentCount = n.NewsComment.Count()

                });
            }

            return response;
        }
    }
}
