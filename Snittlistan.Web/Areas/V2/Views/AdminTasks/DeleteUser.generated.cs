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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/V2/Views/AdminTasks/DeleteUser.cshtml")]
    public partial class _Areas_V2_Views_AdminTasks_DeleteUser_cshtml : Snittlistan.Web.Infrastructure.BaseViewPage<UserViewModel>
    {
        public _Areas_V2_Views_AdminTasks_DeleteUser_cshtml()
        {
        }
        public override void Execute()
        {

WriteLiteral("\r\n");


            
            #line 3 "..\..\Areas\V2\Views\AdminTasks\DeleteUser.cshtml"
  
    ViewBag.Title = "Ta bort användare";


            
            #line default
            #line hidden
WriteLiteral("\r\n<h2>Ta bort</h2>\r\n<p>Är du säker på att du vill ta bort denna användare?</p>\r\n");


            
            #line 9 "..\..\Areas\V2\Views\AdminTasks\DeleteUser.cshtml"
 using (Html.BeginForm("DeleteUser", "AdminTasks", FormMethod.Post))
{

            
            #line default
            #line hidden
WriteLiteral("    <h3>");


            
            #line 11 "..\..\Areas\V2\Views\AdminTasks\DeleteUser.cshtml"
   Write(Model.Name);

            
            #line default
            #line hidden
WriteLiteral("</h3>\r\n");



WriteLiteral("    <button class=\"btn btn-danger btn-large\" type=\"submit\">Ta bort</button>\r\n");


            
            #line 13 "..\..\Areas\V2\Views\AdminTasks\DeleteUser.cshtml"
    
            
            #line default
            #line hidden
            
            #line 13 "..\..\Areas\V2\Views\AdminTasks\DeleteUser.cshtml"
Write(Html.ActionLink("Avbryt", "Users", null, new { @class = "btn btn-large" }));

            
            #line default
            #line hidden
            
            #line 13 "..\..\Areas\V2\Views\AdminTasks\DeleteUser.cshtml"
                                                                               
}
            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591
