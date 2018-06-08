﻿using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace GameProject.Models
{
    public static class HelperExtensions
    {
        public static MvcHtmlString ActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName = "Home", object htmlAttributes = null, string httpMethod = "GET", string onComplete = null)
        {
            if (controllerName != "Home") controllerName = "api/" + controllerName;
            var repID = Guid.NewGuid().ToString();
            var options = new AjaxOptions
            {
                HttpMethod = httpMethod,
                UpdateTargetId = "page-content",
                InsertionMode = InsertionMode.Replace
            };
            if (onComplete != null) options.OnComplete = onComplete;
            htmlAttributes = htmlAttributes ?? new
            {
                @class = "page-link"
            };
            var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, null, options, htmlAttributes);
            return MvcHtmlString.Create(lnk.ToString().Replace(repID, linkText));
        }

        public static MvcForm BeginForm(this AjaxHelper ajaxHelper, string actionName, string onComplete, string controllerName = "Account", string httpMethod = "POST")
        {
            controllerName = "api/" + controllerName;
            var options = new AjaxOptions
            {
                OnComplete = onComplete,
                HttpMethod = httpMethod
            };

            return ajaxHelper.BeginForm(actionName, controllerName, null, options, null);
        }


        public static string GetDisplayName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("DisplayName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}