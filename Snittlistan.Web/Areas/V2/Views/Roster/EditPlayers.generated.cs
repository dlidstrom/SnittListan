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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/V2/Views/Roster/EditPlayers.cshtml")]
    public partial class _Areas_V2_Views_Roster_EditPlayers_cshtml : Snittlistan.Web.Infrastructure.BaseViewPage<EditRosterPlayersViewModel>
    {
        public _Areas_V2_Views_Roster_EditPlayers_cshtml()
        {
        }
        public override void Execute()
        {

WriteLiteral("\r\n<h4>Omgång ");


            
            #line 3 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
      Write(Model.RosterViewModel.Turn);

            
            #line default
            #line hidden
WriteLiteral("</h4>\r\n<div class=\"row\">\r\n    <div class=\"span6\">\r\n        ");


            
            #line 6 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
   Write(Html.DisplayFor(model => model.RosterViewModel, "RosterViewModel", new { hideControls = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n<div class=\"row\">\r\n    <div class=\"span8\">\r\n");


            
            #line 11 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
         using (Html.BeginForm(
            "EditPlayers",
            "Roster",
            new { Model.RosterViewModel.Header.RosterId },
            FormMethod.Post,
            new { @class = "form-horizontal" }))
        {
            
            
            #line default
            #line hidden
            
            #line 18 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
       Write(Html.Hidden("IsFourPlayer", Model.RosterViewModel.IsFourPlayer));

            
            #line default
            #line hidden
            
            #line 18 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                            
            
            
            #line default
            #line hidden
            
            #line 19 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
       Write(Html.DisplayFor(model => Html.ViewData.ModelState, "ValidationSummary"));

            
            #line default
            #line hidden
            
            #line 19 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                                    
            if (Model.RosterViewModel.IsFourPlayer)
            {

            
            #line default
            #line hidden
WriteLiteral("                <table class=\"table table-condensed\">\r\n                    <tr>\r\n" +
"                        <th></th>\r\n                        <th>Namn</th>\r\n      " +
"              </tr>\r\n");


            
            #line 27 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                     for (var i = 0; i < 4; i++)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <tr>\r\n                            <td class=\"table-id\">");


            
            #line 30 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                             Write(i+1);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                            <td>\r\n                                <select " +
"name=\"");


            
            #line 32 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                         Write(string.Format("Player{0}", i + 1));

            
            #line default
            #line hidden
WriteLiteral("\" required>\r\n                                    <option value=\"\">Välj spelare</o" +
"ption>\r\n");


            
            #line 34 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                     foreach (var player in Model.AvailablePlayers)
                                    {
                                        var elementAtOrDefault = Model.RosterViewModel.Players.ElementAtOrDefault(i);
                                        if (elementAtOrDefault != null && player.Id == elementAtOrDefault.PlayerId)
                                        {

            
            #line default
            #line hidden
WriteLiteral("                                            <option value=\"");


            
            #line 39 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                      Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\" selected=\"selected\">");


            
            #line 39 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                                      Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 40 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                        }
                                        else
                                        {

            
            #line default
            #line hidden
WriteLiteral("                                            <option value=\"");


            
            #line 43 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                      Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 43 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                  Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 44 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                        }
                                    }

            
            #line default
            #line hidden
WriteLiteral("                                </select>\r\n                            </td>\r\n   " +
"                     </tr>\r\n");


            
            #line 49 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                    <tr>\r\n                        <td class=\"table-id\">R</td>\r\n  " +
"                      <td>\r\n                            <select name=\"reserve1\">" +
"\r\n                                <option value=\"\">Välj reserv</option>\r\n");


            
            #line 55 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                 foreach (var player in Model.AvailablePlayers)
                                {
                                    var elementAtOrDefault = Model.RosterViewModel.Players.ElementAtOrDefault(4);
                                    if (elementAtOrDefault != null && player.Id == elementAtOrDefault.PlayerId)
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <option value=\"");


            
            #line 60 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                  Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\" selected=\"selected\">");


            
            #line 60 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                                  Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 61 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                    }
                                    else
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <option value=\"");


            
            #line 64 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                  Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 64 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                              Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 65 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                    }
                                }

            
            #line default
            #line hidden
WriteLiteral(@"                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td class=""table-id-medium"">Lagledare</td>
                        <td>
                            <select name=""teamleader"">
                                <option value="""">Välj lagledare</option>
");


            
            #line 75 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                 foreach (var player in Model.AvailablePlayers)
                                {
                                    if (Model.RosterViewModel.TeamLeader != null && player.Id == Model.RosterViewModel.TeamLeader.PlayerId)
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <option value=\"");


            
            #line 79 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                  Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\" selected=\"selected\">");


            
            #line 79 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                                  Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 80 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                    }
                                    else
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <option value=\"");


            
            #line 83 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                  Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 83 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                              Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 84 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                    }
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </select>\r\n                        </td>\r\n           " +
"         </tr>\r\n                </table>\r\n");


            
            #line 90 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
            }
            else
            {

            
            #line default
            #line hidden
WriteLiteral("                <table class=\"table table-condensed\">\r\n                    <tr>\r\n" +
"                        <th>Bord</th>\r\n                        <th>Namn</th>\r\n  " +
"                  </tr>\r\n");


            
            #line 98 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                     for (var i = 0; i < 4; i++)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <tr>\r\n                            <td class=\"table-id\" ro" +
"wspan=\"2\">");


            
            #line 101 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                         Write(i+1);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                            <td>\r\n                                <select " +
"name=\"");


            
            #line 103 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                         Write(string.Format("Table{0}Player1", i + 1));

            
            #line default
            #line hidden
WriteLiteral("\" required>\r\n                                    <option value=\"\">Välj spelare</o" +
"ption>\r\n");


            
            #line 105 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                     foreach (var player in Model.AvailablePlayers)
                                    {
                                        var elementAtOrDefault = Model.RosterViewModel.Players.ElementAtOrDefault(2 * i);
                                        if (elementAtOrDefault != null && player.Id == elementAtOrDefault.PlayerId)
                                        {

            
            #line default
            #line hidden
WriteLiteral("                                            <option value=\"");


            
            #line 110 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                      Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\" selected=\"selected\">");


            
            #line 110 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                                      Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 111 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                        }
                                        else
                                        {

            
            #line default
            #line hidden
WriteLiteral("                                            <option value=\"");


            
            #line 114 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                      Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 114 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                  Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 115 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                        }
                                    }

            
            #line default
            #line hidden
WriteLiteral("                                </select>\r\n                            </td>\r\n   " +
"                     </tr>\r\n");



WriteLiteral("                        <tr>\r\n                            <td>\r\n                 " +
"               <select name=\"");


            
            #line 122 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                         Write(string.Format("Table{0}Player2", i + 1));

            
            #line default
            #line hidden
WriteLiteral("\" required>\r\n                                    <option value=\"\">Välj spelare</o" +
"ption>\r\n");


            
            #line 124 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                     foreach (var player in Model.AvailablePlayers)
                                    {
                                        var elementAtOrDefault = Model.RosterViewModel.Players.ElementAtOrDefault(2 * i + 1);
                                        if (elementAtOrDefault != null && player.Id == elementAtOrDefault.PlayerId)
                                        {

            
            #line default
            #line hidden
WriteLiteral("                                            <option value=\"");


            
            #line 129 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                      Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\" selected=\"selected\">");


            
            #line 129 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                                      Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 130 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                        }
                                        else
                                        {

            
            #line default
            #line hidden
WriteLiteral("                                            <option value=\"");


            
            #line 133 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                      Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 133 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                  Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 134 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                        }
                                    }

            
            #line default
            #line hidden
WriteLiteral("                                </select>\r\n                            </td>\r\n   " +
"                     </tr>\r\n");


            
            #line 139 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                    <tr>\r\n                        <td class=\"table-id\" rowspan=\"2" +
"\">R</td>\r\n                        <td>\r\n                            <select name" +
"=\"reserve1\">\r\n                                <option value=\"\">Välj reserv 1</op" +
"tion>\r\n");


            
            #line 145 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                 foreach (var player in Model.AvailablePlayers)
                                {
                                    var elementAtOrDefault = Model.RosterViewModel.Players.ElementAtOrDefault(8);
                                    if (elementAtOrDefault != null && player.Id == elementAtOrDefault.PlayerId)
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <option value=\"");


            
            #line 150 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                  Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\" selected=\"selected\">");


            
            #line 150 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                                  Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 151 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                    }
                                    else
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <option value=\"");


            
            #line 154 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                  Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 154 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                              Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 155 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                    }
                                }

            
            #line default
            #line hidden
WriteLiteral(@"                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <select name=""reserve2"">
                                <option value="""">Välj reserv 2</option>
");


            
            #line 164 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                 foreach (var player in Model.AvailablePlayers)
                                {
                                    var elementAtOrDefault = Model.RosterViewModel.Players.ElementAtOrDefault(9);
                                    if (elementAtOrDefault != null && player.Id == elementAtOrDefault.PlayerId)
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <option value=\"");


            
            #line 169 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                  Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\" selected=\"selected\">");


            
            #line 169 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                                  Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 170 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                    }
                                    else
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <option value=\"");


            
            #line 173 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                  Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 173 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                              Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 174 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                    }
                                }

            
            #line default
            #line hidden
WriteLiteral(@"                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td class=""table-id-medium"">Lagledare</td>
                        <td>
                            <select name=""teamleader"">
                                <option value="""">Välj lagledare</option>
");


            
            #line 184 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                 foreach (var player in Model.AvailablePlayers)
                                {
                                    if (Model.RosterViewModel.TeamLeader != null && player.Id == Model.RosterViewModel.TeamLeader.PlayerId)
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <option value=\"");


            
            #line 188 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                  Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\" selected=\"selected\">");


            
            #line 188 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                                                  Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 189 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                    }
                                    else
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <option value=\"");


            
            #line 192 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                  Write(player.Id);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 192 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                                              Write(player.Name);

            
            #line default
            #line hidden
WriteLiteral("</option>\r\n");


            
            #line 193 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                                    }
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </select>\r\n                        </td>\r\n           " +
"         </tr>\r\n                </table>\r\n");


            
            #line 199 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"control-group\">\r\n                ");


            
            #line 201 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
           Write(Html.Label(
                    "Preliminary",
                    "Preliminär",
                    new { @class = "control-label" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                <div class=\"controls\">\r\n                    ");


            
            #line 206 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
               Write(Html.CheckBox("Preliminary"));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    <span class=\"help-block\">\r\n                        Visas in" +
"te för vanliga spelare\r\n                    </span>\r\n                </div>\r\n   " +
"         </div>\r\n");


            
            #line 212 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
            if (Model.RosterMailEnabled)
            {

            
            #line default
            #line hidden
WriteLiteral("                <div class=\"control-group\">\r\n                    <div class=\"cont" +
"rols\">\r\n                        <label class=\"checkbox\">\r\n                      " +
"      ");


            
            #line 217 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
                       Write(Html.CheckBox("sendUpdateMail", false));

            
            #line default
            #line hidden
WriteLiteral(@" Skicka mail till spelarna <strong style=""background-color: yellow; padding: 5px; border: 1px dashed red;"">Nyhet!</strong>
                        </label>
                        <span class=""help-block"">
                            Ett mail med uttagningen och uppmaning om att
                            acceptera kommer skickas till alla uttagna spelare.
                        </span>
                    </div>
                </div>
");


            
            #line 225 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"form-actions\">\r\n                <button class=\"btn btn-pr" +
"imary btn-large\" type=\"submit\">Spara</button>\r\n                ");


            
            #line 228 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
           Write(Html.ActionLink("Avbryt", "View", new { season = Model.RosterViewModel.Season, turn = Model.RosterViewModel.Turn }, new { @class = "btn btn-large" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");


            
            #line 230 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>\r\n");


            
            #line 233 "..\..\Areas\V2\Views\Roster\EditPlayers.cshtml"
Write(Html.Action("PlayerStatus", new
{
    turn = Model.RosterViewModel.Turn,
    season = Model.RosterViewModel.Season
}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");


        }
    }
}
#pragma warning restore 1591