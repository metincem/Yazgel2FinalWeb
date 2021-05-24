using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using YazGelFinal.WebUI.EFCore;
using YazGelFinal.WebUI.Models;

namespace YazGelFinal.WebUI.Controllers
{
    public class HomeController : Controller
    {
        EFCoreContext context;
        public HomeController()
        {
            context = new EFCoreContext();
        }

        public IActionResult Index()
        {
            var cards = context.Cards.Select(x => new { x.Image }).ToList();

            return View();
        }

        [Route("/OyunBasladi")]
        public IActionResult OyunBasladi()
        {
            return View();
        }
        [Route("/Routing/{connectionId}")]
        public IActionResult Routing(string connectionId)
        {
            int score = 0;
            List<GameProccess> proccesses = context.GameProccesses.Where(x => x.Completed && x.ConnectionId.Equals(connectionId)).ToList();
            if (proccesses.Count == 54)
            {
                HttpContext.Session.SetInt32("AllCompleted", 1);
                HttpContext.Session.SetInt32("Score", 870);
            }
            else
            {
                int lvl1Score = proccesses.Where(x => x.CardLevel == 1).Count() / 2;
                int lvl2Score = proccesses.Where(x => x.CardLevel == 2).Count() / 2;
                int lvl3Score = proccesses.Where(x => x.CardLevel == 3).Count() / 2;
                score = (lvl1Score * 20) + (lvl2Score * 30) + (lvl3Score * 40);
                HttpContext.Session.SetInt32("Score", score);
            }
            return RedirectToAction("Sonuc");
        }
        [Route("/Sonuc")]
        public IActionResult Sonuc()
        {
            if (HttpContext.Session.GetInt32("Score") == null)
                return NotFound();

            int[] model = new int[2];
            model[0] = HttpContext.Session.GetInt32("AllCompleted") ?? 0;
            model[1] = HttpContext.Session.GetInt32("Score") ?? 0;
            HttpContext.Session.Clear();
            return View(model);
        }
    }
}
