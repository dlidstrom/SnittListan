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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/V2/Views/AdminTasks/ResetIndexes.cshtml")]
    public partial class _Areas_V2_Views_AdminTasks_ResetIndexes_cshtml : Snittlistan.Web.Infrastructure.BaseViewPage<dynamic>
    {
        public _Areas_V2_Views_AdminTasks_ResetIndexes_cshtml()
        {
        }
        public override void Execute()
        {

            
            #line 1 "..\..\Areas\V2\Views\AdminTasks\ResetIndexes.cshtml"
  
    ViewBag.Title = "Administrera RavenDB";


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"span8\">\r\n        <h3>");


            
            #line 7 "..\..\Areas\V2\Views\AdminTasks\ResetIndexes.cshtml"
       Write(ViewBag.Title);

            
            #line default
            #line hidden
WriteLiteral("</h3>\r\n");


            
            #line 8 "..\..\Areas\V2\Views\AdminTasks\ResetIndexes.cshtml"
         using (Html.BeginForm("ResetIndexes", "AdminTasks", FormMethod.Post, new { @class = "form-horizontal" }))
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"alert alert-danger\">Är du säker på att du vill nollställa" +
" alla index? Det innebär\r\n                omräkning av <strong>alla</strong> ind" +
"ex. Använd vid behov t.ex. efter uppgradering av RavenDB.\r\n            </div>\r\n");



WriteLiteral("            <div class=\"form-actions\">\r\n                <button class=\"btn btn-pr" +
"imary btn-large\" type=\"submit\">Ja, utför</button>\r\n            </div>\r\n");


            
            #line 16 "..\..\Areas\V2\Views\AdminTasks\ResetIndexes.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>\r\n");


        }
    }
}
#pragma warning restore 1591