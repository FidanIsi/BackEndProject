using System;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;

namespace WebApplication1.Views.Shared.Components.SearchResult
{
    public class SearchResultViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<Product> model)
        {
            return View(model);
        }
    }
}