﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Memberships.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString GlyphLink(this HtmlHelper htmlHelper, string controller, string action, string text,
            string glyphicon, string cssClases="", string id = "")
        {
            //declare a span for the glyphicon
            var glyph = string.Format("<span class='glyphicon glyphicon-{0}'></span>", glyphicon);

            //declare the anchor tag
            var anchor = new TagBuilder("a");
            anchor.MergeAttribute("href", string.Format("/{0}/{1}/",
                controller, action));
            anchor.InnerHtml = string.Format("{0} {1}", glyph, text);
            anchor.AddCssClass(cssClases);
            anchor.GenerateId(id);

            return MvcHtmlString.Create(anchor.ToString(TagRenderMode.Normal));
        }
    }
}