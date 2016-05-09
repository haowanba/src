using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Common.Define;
using SqlServer;
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
                        _imgUrl = requestUrl.Scheme + "://" + host + "/Content/images/logo.jpg";
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
            config.Desc = "稻清,稻瘟管理新标准！";
            config.ImgUrl = ImgUrl;
            ViewBag.Config = config;
            return View();
        }

        public ActionResult Add(ShakeItem item)
        {
            if (String.IsNullOrEmpty(item.Telephone))
            {
                ShakeItemDbHelper.Insert(item);
            }
            else
            {
                ShakeItem tmp = ShakeItemDbHelper.GetByTelephone(item.Telephone);
                if (tmp == null)
                {
                    ShakeItemDbHelper.Insert(item);
                }
                else
                {
                    tmp.Name = item.Name;
                    tmp.Area = item.Area;
                    tmp.Score = item.Score;
                    ShakeItemDbHelper.Update(tmp);
                }
            }
            return RedirectToAction("Show", new { id = item.Id });
        }

        public ActionResult Show(string id)
        {
            //ShakeItem item = ShakeItemDbHelper.GetById(id);
            WxConfig config = WeixinAccess.GetWxConfig(Request);
            ViewBag.Title = config.Title = "稻清产品宣传";
            config.Desc = "这是描述，啦啦啦";
            config.ImgUrl = ImgUrl;
            ViewBag.Config = config;
            ViewBag.Id = id;
            return View();
        }

        public ActionResult Update(string id, int result)
        {
            ShakeItem item = ShakeItemDbHelper.GetById(id);
            item.Score = result;
            ShakeItemDbHelper.Update(item);

            return RedirectToAction("Finish", new { id = item.Id });
        }

        public ActionResult Finish(string id)
        {
            ShakeItem item = ShakeItemDbHelper.GetById(id);
            WxConfig config = WeixinAccess.GetWxConfig(Request);
            config.Desc = String.Format("我是{0}，来自{1}，2016我推广稻清{2}亩。防治稻瘟，我推荐稻清。", item.Name, item.Area, item.Score);
            config.ImgUrl = ImgUrl;
            ViewBag.Title = config.Title = "稻清产品宣传";
            ViewBag.Config = config;
            return View(item);
        }
    }
}