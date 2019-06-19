using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;

namespace NewsPublish.Service
{
    public class BannerService
    {
        private Db _db;

        public BannerService(Db db)
        {
            this._db = db;
        }
        public ResponseModel AddBanner(AddBanner banner)
        {
            var ba = new Banner {AddTime = DateTime.Now.ToString(),Image = banner.Image,Url = banner.Url,Remark = banner.Remark};
            _db.Banner.Add(ba);
            int i = _db.SaveChanges();
            if (i> 0)
            {
                
                return new ResponseModel{code = 200,result = "Banner add success!"};
            }
            return new ResponseModel{code = 0,result = "Banner add failed!"};
        }

        public ResponseModel GetBannerList()
        {
            var banners = _db.Banner.ToList().OrderByDescending(c => c.AddTime);
            var response = new ResponseModel
            {
                code = 200,
                result = "Get Banner List success!",
                data = new List<BannerModel>()
            };
            foreach (var banner in banners)
            {
                response.data.Add(new BannerModel
                {
                    Id = banner.Id,
                    Image = banner.Image,
                    Remark = banner.Remark
                });
            }
            return response;
        }

        public ResponseModel DeleteBanner(int bannerId)
        {
            var banner = _db.Banner.Find(bannerId);
            if (banner == null)
            {
                return new ResponseModel
                {
                    code = 0,
                    result = "This banner has not existed!"
                };
            }

            _db.Banner.Remove(banner);
            int i = _db.SaveChanges();
            if (i > 0)
            {

                return new ResponseModel { code = 200, result = "Banner delete success!" };
            }
            return new ResponseModel { code = 0, result = "Banner delete failed!" };
        }
    }
}
