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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/V2/Views/AdminTasks/SendMail.cshtml")]
    public partial class _Areas_V2_Views_AdminTasks_SendMail_cshtml : Snittlistan.Web.Infrastructure.BaseViewPage<SendMailViewModel>
    {
        public _Areas_V2_Views_AdminTasks_SendMail_cshtml()
        {
        }
        public override void Execute()
        {

WriteLiteral("\r\n");


            
            #line 3 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
  
    ViewBag.Title = "Skicka ett mail";


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"span8\">\r\n        <h3>");


            
            #line 9 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
       Write(ViewBag.Title);

            
            #line default
            #line hidden
WriteLiteral("</h3>\r\n");


            
            #line 10 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
         using (Html.BeginForm("SendMail", "AdminTasks", FormMethod.Post, new { @class = "form-horizontal" }))
        {
            
            
            #line default
            #line hidden
            
            #line 12 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
       Write(Html.DisplayFor(model => Html.ViewData.ModelState, "ValidationSummary"));

            
            #line default
            #line hidden
            
            #line 12 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
                                                                                    

            
            #line default
            #line hidden
WriteLiteral(@"            <div class=""control-group"">
                <label class=""control-label"" for=""recipient"">Mottagare:</label>
                <div class=""controls"">
                    <input type=""email""
                           name=""recipient""
                           placeholder=""""
                           autocorrect=""off""
                           autocomplete=""off""
                           value=""");


            
            #line 21 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
                             Write(Model.Recipient);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n                           autofocus=\"autofocus\"\r\n                           r" +
"equired />\r\n                </div>\r\n            </div>\r\n");



WriteLiteral(@"            <div class=""control-group"">
                <label class=""control-label"" for=""recipient"">Svara till:</label>
                <div class=""controls"">
                    <input type=""email""
                           name=""replyTo""
                           placeholder=""""
                           autocorrect=""off""
                           autocomplete=""off""
                           value=""");


            
            #line 34 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
                             Write(Model.ReplyTo);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n                           required />\r\n                </div>\r\n            </" +
"div>\r\n");



WriteLiteral(@"            <div class=""control-group"">
                <label class=""control-label"" for=""subject"">Ämnesrad:</label>
                <div class=""controls"">
                    <input type=""text""
                           name=""subject""
                           maxlength=""128""
                           placeholder=""""
                           value=""");


            
            #line 45 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
                             Write(Model.Subject);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n                           required />\r\n                </div>\r\n            </" +
"div>\r\n");



WriteLiteral("            <div class=\"control-group\">\r\n                <label class=\"control-la" +
"bel\" for=\"content\">Innehåll:</label>\r\n                <div class=\"controls\">\r\n  " +
"                  <textarea name=\"content\" maxlength=\"1024\" placeholder=\"\" rows=" +
"\"4\" required>");


            
            #line 52 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
                                                                                          Write(Model.Content);

            
            #line default
            #line hidden
WriteLiteral("</textarea>\r\n                </div>\r\n            </div>\r\n");



WriteLiteral("            <div class=\"control-group\">\r\n                <label class=\"control-la" +
"bel\" for=\"rate\">Rate:</label>\r\n                <div class=\"controls\">\r\n         " +
"           ");


            
            #line 58 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
               Write(Html.EnumDropDownListFor(
                        x => x.RateSetting,
                        "Välj rate",
                        new { required = "required", id = "rate" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n            </div>\r\n");



WriteLiteral("            <div class=\"control-group\">\r\n                <label class=\"control-la" +
"bel\" for=\"count\">Antal:</label>\r\n                <div class=\"controls\">\r\n       " +
"             ");


            
            #line 67 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
               Write(Html.EnumDropDownListFor(
                        x => x.MailCount,
                        "Välj antal mail",
                        new { required = "required", id = "count" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n            </div>\r\n");



WriteLiteral("            <div class=\"form-actions\">\r\n                <button class=\"btn btn-pr" +
"imary btn-large\" type=\"submit\">Skicka</button>\r\n                ");


            
            #line 75 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
           Write(Html.ActionLink("Avbryt", "Index", null, new { @class = "btn btn-large" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");


            
            #line 77 "..\..\Areas\V2\Views\AdminTasks\SendMail.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>\r\n");


        }
    }
}
#pragma warning restore 1591