using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCSharpService.Controllers
{
    public class ContactMVCController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }

        public string[] Get()
        {
            return new string[]
            {
                "hello",
                "world"
            };
        }
    }
}