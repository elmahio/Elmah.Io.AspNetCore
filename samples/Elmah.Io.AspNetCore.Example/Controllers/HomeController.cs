﻿using System;
using Microsoft.AspNetCore.Mvc;

namespace Elmah.Io.AspNetCore.Example.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            throw new Exception("some bug yo");
        }

        public IActionResult About()
        {
            try
            {
                var i = 0;
                var result = 42/i;
            }
            catch (DivideByZeroException e)
            {
                e.Ship(HttpContext);
            }

            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
