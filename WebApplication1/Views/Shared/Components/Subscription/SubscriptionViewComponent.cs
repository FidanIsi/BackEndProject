using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Views.Shared.Components.Subscription
{
    public class SubscriptionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new SubscriptionVM(); 
            return View(model);
        }
    }
}
