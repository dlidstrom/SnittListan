﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Snittlistan.Web.Views
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using Snittlistan.Web.Areas.V2;
    using Snittlistan.Web.Areas.V2.ViewModels;
    using Snittlistan.Web.HtmlHelpers;
    using Snittlistan.Web.Infrastructure;
    using Snittlistan.Web.Infrastructure.Database;
    using Snittlistan.Web.ViewModels;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/V2/Views/MatchResult/Details4.cshtml")]
    public partial class _Areas_V2_Views_MatchResult_Details4_cshtml : Snittlistan.Web.Infrastructure.BaseViewPage<Result4ViewModel>
    {
        public _Areas_V2_Views_MatchResult_Details4_cshtml()
        {
        }
        public override void Execute()
        {

WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"span4\">\r\n        ");


            
            #line 5 "..\..\Areas\V2\Views\MatchResult\Details4.cshtml"
   Write(Html.DisplayFor(model => Model.HeaderViewModel, "Header"));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n<div class=\"row\">\r\n    <div class=\"span6\">\r\n        ");


            
            #line 10 "..\..\Areas\V2\Views\MatchResult\Details4.cshtml"
   Write(Html.DisplayFor(model => Model.ResultReadModel, "Result4"));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n");


        }
    }
}
#pragma warning restore 1591
