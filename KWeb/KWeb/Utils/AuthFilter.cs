using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KWeb.Utils
{
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var excludedControllers = new[] { "Auth" };
            var excludedActions = new[] { "Login", "Register" };

            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = filterContext.ActionDescriptor.ActionName;

            if (excludedControllers.Contains(controllerName) || excludedActions.Contains(actionName))
            {
                return;
            }

            var userSession = filterContext.HttpContext.Session["user"] as string;
            if (string.IsNullOrEmpty(userSession))
            {    
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                    { "controller", "Auth" },
                    { "action", "Login" }
                    });
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}