using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Common.Define;
using Weixin.Api;

namespace HWB.Controllers
{
    public class WeixinController : Controller
    {
        private string _imgUrl;
        private string ImgUrl
        {
            get
            {
                if (String.IsNullOrEmpty(_imgUrl))
                {
                    Uri requestUrl = Request.Url;
                    if (requestUrl != null)
                    {
                        string host = requestUrl.Host;
                        _imgUrl = requestUrl.Scheme + "://" + host + "/favicon.ico";
                    }
                    else
                    {
                        _imgUrl = String.Empty;
                    }
                }
                return _imgUrl;
            }
        }

        public ActionResult Access()
        {
            string s = WeixinAccess.CheckSignature(Request);
            return Content(s);
        }

        public ActionResult Play()
        {
            WxConfig config = WeixinAccess.GetWxConfig(Request);
            ViewBag.Title = config.Title = "稻清产品宣传";
            config.Desc = "这是描述，啦啦啦";
            config.ImgUrl = ImgUrl;
            ViewBag.Config = config;
            return View();
        }

        public ActionResult Add(ShakeItem item)
        {
            using (ShakeItemDbContext context = new ShakeItemDbContext())
            {
                context.Add(item);
            }

            return RedirectToAction("Show", new { id = item.Id });
        }

        public ActionResult Show(int id)
        {
            ShakeItem item;
            using (ShakeItemDbContext context = new ShakeItemDbContext())
            {
                item = context.Get(id);
            }
            WxConfig config = WeixinAccess.GetWxConfig(Request);
            ViewBag.Title = config.Title = "摇一摇，看一看";
            config.Desc = "这是描述，啦啦啦";
            config.ImgUrl = ImgUrl;
            ViewBag.Config = config;
            return View(item);
        }

        public ActionResult TryFindByTel(string tel)
        {
            using (ShakeItemDbContext context = new ShakeItemDbContext())
            {
                DbSet<ShakeItem> items = context.Get();
                var query = from d in items
                            where d.Telephone == tel
                            select d;
                query.Load();
                int id = context.Items.ToArray()[0].Id;
            }
            return Content("");
        }
    }
}