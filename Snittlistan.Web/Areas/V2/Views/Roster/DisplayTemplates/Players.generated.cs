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
    
    #line 1 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
    using Raven.Abstractions;
    
    #line default
    #line hidden
    using Snittlistan.Web.Areas.V2;
    using Snittlistan.Web.Areas.V2.ViewModels;
    using Snittlistan.Web.HtmlHelpers;
    using Snittlistan.Web.Infrastructure;
    using Snittlistan.Web.Infrastructure.Database;
    using Snittlistan.Web.ViewModels;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/V2/Views/Roster/DisplayTemplates/Players.cshtml")]
    public partial class _Areas_V2_Views_Roster_DisplayTemplates_Players_cshtml : Snittlistan.Web.Infrastructure.BaseViewPage<RosterViewModel>
    {
        public _Areas_V2_Views_Roster_DisplayTemplates_Players_cshtml()
        {
        }
        public override void Execute()
        {



            
            #line 3 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
  
    var hideControls = Convert.ToBoolean(ViewData["hideControls"]);


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"well well-small editable\" id=\"");


            
            #line 7 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                     Write(Model.Header.RosterId);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n");


            
            #line 8 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
     if (Model.Preliminary && User.IsInRole2(WebsiteRoles.Uk.UkTasks))
    {

            
            #line default
            #line hidden
WriteLiteral("        <div class=\"alert alert-info\">\r\n            <strong>Preliminär</strong> G" +
"ömd för vanliga användare\r\n        </div>\r\n");


            
            #line 13 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
    }

            
            #line default
            #line hidden

            
            #line 14 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
     if (Model.Preliminary && User.IsInRole2(WebsiteRoles.Uk.UkTasks) == false)
    {
        
            
            #line default
            #line hidden

            
            #line 16 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                                  
        Model.Players.Clear();
        Model.TeamLeader = null;
    }

            
            #line default
            #line hidden

            
            #line 20 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
     if (User.IsInRole2(WebsiteRoles.Uk.UkTasks)
        && hideControls == false
        && (Model.Header.Date.ToUniversalTime() > SystemTime.UtcNow || Request.IsAdmin(User)))
    {
        
            
            #line default
            #line hidden
            
            #line 24 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
   Write(Html.ActionLink(
             "Ändra laget",
             "EditPlayers",
             new { Model.Header.RosterId },
             new { @class = "btn btn-primary btn-block" }));

            
            #line default
            #line hidden
            
            #line 28 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                                          
    }

            
            #line default
            #line hidden

            
            #line 30 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
     if (Model.Preliminary == false
         && Model.Header.Date.ToUniversalTime() > SystemTime.UtcNow
         && hideControls == false)
    {
        if (User.CustomIdentity.PlayerId != null
            && Model.Players.Find(x => x.PlayerId == User.CustomIdentity.PlayerId)?.Accepted == false)
        {
            using (Html.BeginForm(
                "Accept",
                "RosterAccept",
                new
                {
                    Model.Header.RosterId,
                    User.CustomIdentity.PlayerId,
                    Model.Season,
                    Model.Turn
                }))
            {

            
            #line default
            #line hidden
WriteLiteral("                <button class=\"btn btn-success btn-block\" type=\"submit\">\r\n       " +
"             &#10004;\r\n                    Jag kommer\r\n                </button>" +
"\r\n");


            
            #line 52 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
            }
        }
        else if (Model.Players.Any()
                 && Model.Players.Any(x => x.Accepted == false)
                 && User.IsInRole2(WebsiteRoles.Uk.UkTasks))
        {
            
            
            #line default
            #line hidden
            
            #line 58 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
       Write(Html.ActionLink(
                "Ange spelare deltar",
                "AcceptPlayer",
                "AdminRosterAccept",
                new
                {
                    Model.Header.RosterId,
                    Model.Season,
                    Model.Turn
                },
                new
                {
                    @class = "btn btn-primary btn-block"
                }));

            
            #line default
            #line hidden
            
            #line 71 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                  
        }
    }

            
            #line default
            #line hidden

            
            #line 74 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
     if (User.IsInRole2(WebsiteRoles.Uk.UkTasks) && hideControls == false)
    {
        
            
            #line default
            #line hidden
            
            #line 76 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
   Write(Html.ActionLink(
            "Händelser",
            "Index",
            "AuditLogViewer",
            new
            {
                Id = Model.Header.RosterId
            },
            new
            {
                @class = "btn btn-info btn-block"
            }));

            
            #line default
            #line hidden
            
            #line 87 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
              
    }

            
            #line default
            #line hidden

            
            #line 89 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
     if (Model.IsFourPlayer)
    {

            
            #line default
            #line hidden
WriteLiteral("        <table class=\"table table-condensed\">\r\n            <tr>\r\n                " +
"<th></th>\r\n                <th colspan=\"2\">Namn</th>\r\n            </tr>\r\n");


            
            #line 96 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
             for (var i = 0; i < 4; i++)
            {

            
            #line default
            #line hidden
WriteLiteral("                <tr>\r\n                    <td class=\"table-id\">");


            
            #line 99 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                     Write(i + 1);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                    <td>\r\n");


            
            #line 101 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                          
                            var player = Model.Players.ElementAtOrDefault(i);
                        

            
            #line default
            #line hidden

            
            #line 104 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                         if (player != null)
                        {
                            
            
            #line default
            #line hidden
            
            #line 106 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                       Write(player.PlayerName);

            
            #line default
            #line hidden
            
            #line 106 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                              
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </td>\r\n                    <td>\r\n");


            
            #line 110 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                         if (player != null && player.Accepted)
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <span class=\"label label-success\">&#10004;</span>\r\n");


            
            #line 113 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </td>\r\n                </tr>\r\n");


            
            #line 116 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("            <tr>\r\n                <td class=\"table-id\">R</td>\r\n                <t" +
"d>\r\n");


            
            #line 120 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                      
                        var reserve = Model.Players.ElementAtOrDefault(4);
                    

            
            #line default
            #line hidden

            
            #line 123 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                     if (reserve != null)
                    {
                        
            
            #line default
            #line hidden
            
            #line 125 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                   Write(reserve.PlayerName);

            
            #line default
            #line hidden
            
            #line 125 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                           
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n                <td>\r\n");


            
            #line 129 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                     if (reserve != null && reserve.Accepted)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <span class=\"label label-success\">&#10004;</span>\r\n");


            
            #line 132 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n            </tr>\r\n            <tr>\r\n                <td c" +
"lass=\"table-id-medium\">\r\n                    Lagledare\r\n                </td>\r\n " +
"               <td>\r\n");


            
            #line 140 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                     if (Model.TeamLeader != null)
                    {
                        
            
            #line default
            #line hidden
            
            #line 142 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                   Write(Model.TeamLeader.PlayerName);

            
            #line default
            #line hidden
            
            #line 142 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                                    
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n                <td>\r\n");


            
            #line 146 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                     if (Model.TeamLeader != null && Model.TeamLeader.Accepted)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <span class=\"label label-success\">&#10004;</span>\r\n");


            
            #line 149 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n            </tr>\r\n        </table>\r\n");


            
            #line 153 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <table class=\"table table-condensed\">\r\n            <tr>\r\n                " +
"<th>Bord</th>\r\n                <th colspan=\"2\">Namn</th>\r\n            </tr>\r\n");


            
            #line 161 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
             for (var i = 0; i < 4; i++)
            {

            
            #line default
            #line hidden
WriteLiteral("                <tr>\r\n                    <td class=\"table-id\" rowspan=\"2\">");


            
            #line 164 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                                 Write(i + 1);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                    <td>\r\n");


            
            #line 166 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                          
                            var first = Model.Players.ElementAtOrDefault(2 * i);
                        

            
            #line default
            #line hidden

            
            #line 169 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                         if (first != null)
                        {
                            
            
            #line default
            #line hidden
            
            #line 171 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                       Write(first.PlayerName);

            
            #line default
            #line hidden
            
            #line 171 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                             
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </td>\r\n                    <td>\r\n");


            
            #line 175 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                         if (first != null && first.Accepted)
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <span class=\"label label-success\">&#10004;</span>\r\n");


            
            #line 178 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </td>\r\n                </tr>\r\n");



WriteLiteral("                <tr>\r\n                    <td>\r\n");


            
            #line 183 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                          
                            var second = Model.Players.ElementAtOrDefault(2 * i + 1);
                        

            
            #line default
            #line hidden

            
            #line 186 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                         if (second != null)
                        {
                            
            
            #line default
            #line hidden
            
            #line 188 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                       Write(second.PlayerName);

            
            #line default
            #line hidden
            
            #line 188 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                              
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </td>\r\n                    <td>\r\n");


            
            #line 192 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                         if (second != null && second.Accepted)
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <span class=\"label label-success\">&#10004;</span>\r\n");


            
            #line 195 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </td>\r\n                </tr>\r\n");


            
            #line 198 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("            <tr>\r\n                <td class=\"table-id\" rowspan=\"2\">R</td>\r\n      " +
"          <td>\r\n");


            
            #line 202 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                      
                        var reserve1 = Model.Players.ElementAtOrDefault(8);
                    

            
            #line default
            #line hidden

            
            #line 205 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                     if (reserve1 != null)
                    {
                        
            
            #line default
            #line hidden
            
            #line 207 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                   Write(reserve1.PlayerName);

            
            #line default
            #line hidden
            
            #line 207 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                            
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n                <td>\r\n");


            
            #line 211 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                     if (reserve1 != null && reserve1.Accepted)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <span class=\"label label-success\">&#10004;</span>\r\n");


            
            #line 214 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n            </tr>\r\n            <tr>\r\n                <td>\r" +
"\n");


            
            #line 219 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                      
                        var reserve2 = Model.Players.ElementAtOrDefault(9);
                    

            
            #line default
            #line hidden

            
            #line 222 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                     if (reserve2 != null)
                    {
                        
            
            #line default
            #line hidden
            
            #line 224 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                   Write(reserve2.PlayerName);

            
            #line default
            #line hidden
            
            #line 224 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                            
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n                <td>\r\n");


            
            #line 228 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                     if (reserve2 != null && reserve2.Accepted)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <span class=\"label label-success\">&#10004;</span>\r\n");


            
            #line 231 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n            </tr>\r\n            <tr>\r\n                <td c" +
"lass=\"table-id-medium\">\r\n                    Lagledare\r\n                </td>\r\n " +
"               <td>\r\n");


            
            #line 239 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                     if (Model.TeamLeader != null)
                    {
                        
            
            #line default
            #line hidden
            
            #line 241 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                   Write(Model.TeamLeader.PlayerName);

            
            #line default
            #line hidden
            
            #line 241 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                                                    
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n                <td>\r\n");


            
            #line 245 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                     if (Model.TeamLeader != null && Model.TeamLeader.Accepted)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <span class=\"label label-success\">&#10004;</span>\r\n");


            
            #line 248 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n            </tr>\r\n        </table>\r\n");


            
            #line 252 "..\..\Areas\V2\Views\Roster\DisplayTemplates\Players.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");


        }
    }
}
#pragma warning restore 1591
