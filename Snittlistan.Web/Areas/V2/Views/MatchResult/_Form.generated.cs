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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/V2/Views/MatchResult/_Form.cshtml")]
    public partial class _Areas_V2_Views_MatchResult__Form_cshtml : Snittlistan.Web.Infrastructure.BaseViewPage<PlayerFormViewModel[]>
    {
        public _Areas_V2_Views_MatchResult__Form_cshtml()
        {
        }
        public override void Execute()
        {

WriteLiteral("\r\n");


            
            #line 3 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
  
    var maxTotalScore = Model.Max(x => x.TotalScore);
    var maxAverageScore = Model.Max(x => x.ScoreAverage);


            
            #line default
            #line hidden
WriteLiteral(@"
<div class=""row"">
    <div class=""span12"">
        <p class=""lead"">
            <strong>Säsong</strong> innebär medel över säsongen.
            <span class=""badge badge-success"">5</span> innebär medel över de senaste 5 matcherna.
            <strong>Form</strong> är skillnaden mellan de två.
        </p>
    </div>
</div>

<div class=""row"">
    <div class=""span12"">
        <table class=""table table-condensed table-bordered"">
            <tr>
                <th rowspan=""2"">#</th>
                <th rowspan=""2"">Namn</th>
                <th>Säsong &#x25BC;</th>
                <th rowspan=""2"">Banp</th>
                <th rowspan=""2"">Form</th>
            </tr>
            <tr>
                <th><span class=""badge badge-success"">5</span></th>
            </tr>
");


            
            #line 31 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
              
                var index = 0;
            

            
            #line default
            #line hidden

            
            #line 34 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
             foreach (var item in Model)
            {

            
            #line default
            #line hidden
WriteLiteral("                <tr>\r\n                    <td rowspan=\"2\">");


            
            #line 37 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                                Write(++index);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                    <td rowspan=\"2\">");


            
            #line 38 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                               Write(item.Name);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                    <td>");


            
            #line 39 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                   Write(string.Format("{0} ({1})", item.FormattedSeasonAverage(), item.TotalSeries));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                    <td rowspan=\"2\">\r\n");


            
            #line 41 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                         if (item.TotalScore == maxTotalScore)
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <strong>\r\n                                ");


            
            #line 44 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                           Write(item.TotalScore);

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </strong>\r\n");


            
            #line 46 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                        }
                        else
                        {

            
            #line default
            #line hidden
WriteLiteral("                            ");

WriteLiteral("\r\n                                ");


            
            #line 50 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                           Write(item.TotalScore);

            
            #line default
            #line hidden
WriteLiteral("\r\n                            ");

WriteLiteral("\r\n");


            
            #line 52 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                        }

            
            #line default
            #line hidden

            
            #line 53 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                         if (Math.Abs(item.ScoreAverage - maxAverageScore) < 0.005)
                         {

            
            #line default
            #line hidden
WriteLiteral("                             <strong>\r\n                                 (");


            
            #line 56 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                             Write(item.FormattedScoreAverage());

            
            #line default
            #line hidden
WriteLiteral(")\r\n                             </strong>\r\n");


            
            #line 58 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                         }
                         else
                         {

            
            #line default
            #line hidden
WriteLiteral("                            ");

WriteLiteral("(");


            
            #line 61 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                          Write(item.FormattedScoreAverage());

            
            #line default
            #line hidden
WriteLiteral(")\r\n");


            
            #line 62 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                         }

            
            #line default
            #line hidden
WriteLiteral("                    </td>\r\n                    <td rowspan=\"2\" class=\"");


            
            #line 64 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                                      Write(item.Class());

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                        ");


            
            #line 65 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                   Write(item.FormattedDiff());

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n");



WriteLiteral("                <tr>\r\n                    <td>");


            
            #line 69 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
                   Write(item.FormattedLast5Average());

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n");


            
            #line 71 "..\..\Areas\V2\Views\MatchResult\_Form.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("        </table>\r\n    </div>\r\n</div>\r\n");


        }
    }
}
#pragma warning restore 1591
