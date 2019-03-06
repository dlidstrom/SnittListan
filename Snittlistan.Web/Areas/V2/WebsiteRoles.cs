﻿namespace Snittlistan.Web.Areas.V2
{
    using System.Collections.Generic;
    using System.Linq;

    public static class WebsiteRoles
    {
        // roles
        private static readonly List<WebsiteRole> Roles = new List<WebsiteRole>();

        public static void Initialize()
        {
            var absenceView =
                new WebsiteRole(Absence.View, "Visa frånvaro", RoleLevel.Player);

            var editAllRole =
                new WebsiteRole(Absence.EditAll, "Frånvaro", RoleLevel.User);

            var editMedalsRole =
                new WebsiteRole(EliteMedals.EditMedals, "Elitmedaljer", RoleLevel.User);

            var addManualResultRole =
                new WebsiteRole(MatchResult.AddManualResult, "Manuella matchresultat", RoleLevel.User);

            var editPlayerRole =
                new WebsiteRole(Player.EditPlayer, "Medlemmar", RoleLevel.User);

            var showEmailAddressesRole =
                new WebsiteRole(Player.ShowEmailAddresses, "Visa e-postadresser", RoleLevel.Player);

            var manageRostersRole =
                new WebsiteRole(Roster.ManageRosters, "Laguttagningar (UK)", RoleLevel.User);
        }

        public static class Absence
        {
            public const string View = "Absence.View";

            public const string EditAll = "Absence.EditAll";
        }

        public static class EliteMedals
        {
            public const string EditMedals = "EliteMedals.EditMedals";
        }

        public static class MatchResult
        {
            public const string AddManualResult = "MatchResult.AddManualResult";
        }

        public static class Player
        {
            public const string EditPlayer = "Player.EditPlayer";

            public const string ShowEmailAddresses = "Player.ShowEmailAddresses";
        }

        public static class Roster
        {
            public const string ManageRosters = "Roster.ManageRosters";
        }

        // groups
        public static WebsiteRole[] PlayerGroup()
        {
            return Roles.Where(x => x.RoleLevel <= RoleLevel.Player).ToArray();
        }

        public static WebsiteRole[] UserGroup()
        {
            return Roles.Where(x => x.RoleLevel <= RoleLevel.User).ToArray();
        }

        public static WebsiteRole[] AdminGroup()
        {
            return Roles.Where(x => x.RoleLevel <= RoleLevel.Admin).ToArray();
        }

        public enum RoleLevel
        {
            Player,
            User,
            Admin
        }

        public class WebsiteRole
        {
            public WebsiteRole(string name, string description, RoleLevel roleLevel)
            {
                Name = name;
                Description = description;
                RoleLevel = roleLevel;
                Roles.Add(this);
            }

            public string Name { get; }

            public string Description { get; }

            public RoleLevel RoleLevel { get; }
        }
    }
}
