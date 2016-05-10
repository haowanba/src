using System;
using System.Web.Mvc;
using Common.Define;
using SqlServer;
using Weixin.Api;

namespace HWB.Controllers
{
    public class WeixinController : Controller
    {
        private const string Title = "欢迎参加稻清上市会";
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
            ViewBag.Title = config.Title = Title;
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
                    item = tmp;
                }
            }
            return Json(new { id = item.Id });
            return RedirectToAction("Show", new { id = item.Id });
        }

        public ActionResult Show(string id)
        {
            ShakeItem item = ShakeItemDbHelper.GetById(id);
            if (item.Score > 0)
            {
                return RedirectToAction("Finish", new { id = item.Id });
            }
            WxConfig config = WeixinAccess.GetWxConfig(Request);
            ViewBag.Title = config.Title = Title;
            config.Desc = "稻清,稻瘟管理新标准！";
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
            return Content("");
            return RedirectToAction("Finish", new { id = item.Id });
        }

        public ActionResult Finish(string id)
        {
            ShakeItem item = ShakeItemDbHelper.GetById(id);
            WxConfig config = WeixinAccess.GetWxConfig(Request);
            config.Desc = String.Format("我是{1}{0}，2016我推广稻清{2}亩，为农户增产{3}公斤，小伙伴们为我点赞助力。", item.Name, item.Area, item.Score, item.Score * 50);
            config.ImgUrl = ImgUrl;
            ViewBag.Title = config.Title = Title;
            ViewBag.Config = config;
            return View(item);
        }
    }
}