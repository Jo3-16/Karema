﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using KaReMa.Interfaces;
using Mvc4WebRole.Filters;
using Mvc4WebRole.Models;

namespace Mvc4WebRole.Controllers
{
    [EnhancedAuthorize(Roles = "Reader")]
    public class InfoController : Controller
    {
        private readonly RecipeDbContext repository = new RecipeDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Overview()
        {
            using (var ctx = new UsersContext())
            {
                var userList = ctx.UserProfiles.ToList();
                ViewBag.Users = userList.Count();
            }
            ViewBag.Recipes = repository.Recipes.Count();
            ViewBag.Tags = repository.Tags.Count();

            return View();
        }
        

        [HttpGet]
        [EnhancedAuthorize(Users = "Admin")]
        public virtual FileResult Download()
        {
            var fileStream = DataPersistance.SaveData(repository.Recipes.ToList(),repository.Tags.ToList());
           
            return File(fileStream, "application/json", "Recipes.json");
        }

        //
        // GET: /Recipe/
        public ActionResult ByAuthor()
        {
            IEnumerable<IGrouping<String, RecipeModel>> recipes = repository.Recipes.ToList().GroupBy(r => r.Author, StringComparer.InvariantCultureIgnoreCase);
            return View(recipes);
        }

        [AllowAnonymous]
        public ActionResult Impressum()
        {
            return View();
        }

        public ActionResult LastChanged()
        {
            ViewBag.Title = "Letzte Änderungen";
            IEnumerable<RecipeModel> recipes = repository.Recipes.ToList().OrderByDescending(r => r.LastTimeChanged);
            return View("ByDate", recipes);
        }

        public ActionResult LastCreated()
        {
            ViewBag.Title = "Neueste Rezepte";
            IEnumerable<RecipeModel> recipes = repository.Recipes.ToList().OrderByDescending(r => r.TimeCreated);
            return View("ByDate", recipes);
        }

        [EnhancedAuthorize(Users = "Admin")]
        public ActionResult Logs()
        {
            return View(SessionLogger.Logs);
        }

        [EnhancedAuthorize(Users = "Admin")]
        public ActionResult DeleteLogs()
        {
            SessionLogger.Logs.Clear();
            SessionLogger.AddLog("Logs deleted");
            return RedirectToAction("Logs");
        }

    }
}