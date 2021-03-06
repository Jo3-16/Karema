﻿using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Mvc4WebRole.Filters;
using Mvc4WebRole.Models;

namespace Mvc4WebRole.Controllers
{
    [EnhancedAuthorize(Roles = "Reader")]
    public class RecipeController : Controller
    {
        private readonly RecipeDomain recipeDomain;

        public RecipeController()
        {
            recipeDomain = new RecipeDomain();
            ViewBag.NoAjax = false;
        }

        public ActionResult Index()
        {
            try
            {
                var recipes = recipeDomain.RecipeInfos.OrderBy(t => t.Name);
                return View(recipes);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        public ActionResult Details(Guid id)
        {
            var recipemodel = recipeDomain.GetCompleteRecipe(id);
            if (recipemodel == null)
            {
                return HttpNotFound();
            }
            return View(recipemodel);
        }

        [EnhancedAuthorize(Roles = "Author")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RecipeModel recipemodel)
        {
            if (ModelState.IsValid)
            {
                recipeDomain.Create(recipemodel);

                SessionLogger.AddLog("Recipe " + recipemodel.Name + " with ID" + recipemodel.ID + " created");

                return RedirectToAction("AssignTags", "Tags", new { recipeId = recipemodel.ID });
            }

            return View(recipemodel);
        }

        public ActionResult Edit(Guid id)
        {
            if (!recipeDomain.CanChange(id))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            RecipeModel recipemodel = recipeDomain.GetCompleteRecipe(id);

            if (recipemodel == null)
            {
                return HttpNotFound();
            }
            return View(recipemodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RecipeModel recipemodel)
        {
            if (ModelState.IsValid)
            {
                // var recipeToUpdate = recipeDomain.GetCompleteRecipe(recipemodel.ID);
                // TryUpdateModel(recipeToUpdate, new[] { "Name", "Hint", "Ingredients", "DefaultPersonCount", "Author", "Description" });

                recipeDomain.EditRecipe(recipemodel);

                SessionLogger.AddLog("Recipe " + recipemodel.Name + " with ID" + recipemodel.ID + " modified");

                var redirectToRouteResult = RedirectToAction("Details", new { id = recipemodel.ID });
                return redirectToRouteResult;
            }
            return View(recipemodel);
        }

        public ActionResult Delete(Guid id)
        {
            if (!recipeDomain.CanChange(id))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            RecipeModel recipemodel = recipeDomain.GetRecipe(id);
            if (recipemodel == null)
            {
                return HttpNotFound();
            }

            SessionLogger.AddLog("Recipe " + recipemodel.Name + " with ID" + id + " is beeing deleted");

            return View(recipemodel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            recipeDomain.DeleteRecipe(id);
            SessionLogger.AddLog("Recipe with ID" + id + "has been deleted");

            return RedirectToAction("Index");
        }


        [ChildActionOnly]
        public PartialViewResult _Edit(RecipeModel recipemodel)
        {
            return PartialView(recipemodel);
        }

        [ChildActionOnly]
        public PartialViewResult _EditNew()
        {
            return PartialView("_Edit");
        }
    }
}