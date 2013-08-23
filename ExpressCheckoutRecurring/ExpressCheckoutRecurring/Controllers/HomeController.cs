using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ExpressCheckoutRecurring.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            ViewBag.Tx = HttpContext.Application["tx"];
            ViewBag.Tx1 = HttpContext.Application["tx1"];
            ViewBag.Request = HttpContext.Application["request"];

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Paypal(FormCollection form)
        {

            string val = "";
            foreach (string item in form.AllKeys)
            {
                val += string.Format(" {0} - {1} <br/>", item, form[item]);
            }


            if (!HttpContext.Application.AllKeys.Any(k => k.Equals("tx")))
            {
                HttpContext.Application.Add("tx", val);
            }
            else
            {
                HttpContext.Application["tx"] = val;
            }


            form.Add("cmd", "_notify-validate");
            string data = String.Join("&", form.Cast<string>()
       .Select(key => String.Format("{0}={1}", key, HttpUtility.UrlEncode(form[key]))));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr?");

            if (!HttpContext.Application.AllKeys.Any(k => k.Equals("request")))
            {
                HttpContext.Application.Add("request", data);
            }
            else
            {
                HttpContext.Application["request"] = data;
            }

            request.Method = "POST";
            request.ContentLength = data.Length;

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(data);
            }

            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                if (!HttpContext.Application.AllKeys.Any(k => k.Equals("tx1")))
                {
                    HttpContext.Application.Add("tx1", reader.ReadToEnd());
                }
                else
                {
                    HttpContext.Application["tx1"] = reader.ReadToEnd();
                }

            }

            // testing

            return View();
        }
    }
}
