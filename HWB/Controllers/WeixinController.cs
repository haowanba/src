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
                        //string host = ConfigReader.GetAppSettingsValue(WeixinConstName.HomeUrl);
                        //_imgUrl = requestUrl.Scheme + "://" + host + "/Content/images/logo.jpg?a=2";
                        _imgUrl = "http://hwb.blob.core.chinacloudapi.cn/images/logo.jpg";
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

        /// <summary>
        /// 录入信息页面
        /// </summary>
        /// <returns></returns>
        public ActionResult DataInput()
        {
            WxConfig config = WeixinAccess.GetWxConfig(Request);
            ViewBag.Title = config.Title = Title;
            config.Desc = "稻清,稻瘟管理新标准！";
            config.ImgUrl = ImgUrl;
            ViewBag.Config = config;
            return View();
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ActionResult CheckIn(ShakeItem item)
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
        }

        /// <summary>
        /// 摇一摇页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[WeixinFilter]
        public ActionResult Shake(string id)
        {
            try
            {
                ShakeItem item = ShakeItemDbHelper.GetById(id);
                if (item.Score > 0)
                {
                    return RedirectToAction("Finish", new { id = item.Id });
                }
                if (String.IsNullOrEmpty(item.Name))
                {
                    return RedirectToAction("DataInput");
                }
                WxConfig config = WeixinAccess.GetWxConfig(Request);
                ViewBag.Title = config.Title = Title;
                config.Desc = "稻清,稻瘟管理新标准！";
                config.ImgUrl = ImgUrl;
                ViewBag.Config = config;
                ViewBag.Id = id;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("DataInput");
            }
        }

        /// <summary>
        /// 摇一摇完了更新得分
        /// </summary>
        /// <param name="id"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public ActionResult Update(string id, int result)
        {
            try
            {
                ShakeItem item = ShakeItemDbHelper.GetById(id);
                item.Score = result;
                ShakeItemDbHelper.Update(item);

                string desc = String.Format("我是{1}-{0}，我将推广稻清{2}亩，水稻增产{3}公斤，请为我助力点赞。", item.Name, item.Area, item.Score, item.Score * 50);
                return Json(new { desc, ImgUrl, Title });
            }
            catch (Exception)
            {
                return RedirectToAction("DataInput");
            }
        }

        /// <summary>
        /// 分享结果后跳转到结果页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Finish(string id)
        {
            try
            {
                ShakeItem item = ShakeItemDbHelper.GetById(id);
                WxConfig config = WeixinAccess.GetWxConfig(Request);
                config.Desc = String.Format("我是{1}-{0}，我将推广稻清{2}亩，水稻增产{3}公斤，请为我助力点赞。", item.Name, item.Area, item.Score, item.Score * 50);
                config.ImgUrl = ImgUrl;
                ViewBag.Title = config.Title = Title;
                ViewBag.Config = config;
                return View(item);
            }
            catch (Exception)
            {
                return RedirectToAction("DataInput");
            }
        }
    }
}
