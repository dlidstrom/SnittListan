﻿namespace Snittlistan.Web.ViewModels
{
    using System;

    public class TenantConfiguration
    {
        public TenantConfiguration(
            string name,
            string database,
            string connectionStringName,
            string favicon,
            string appleTouchIcon,
            string appleTouchIconSize,
            string webAppTitle,
            string fullTeamName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Database = database ?? throw new ArgumentNullException(nameof(database));
            ConnectionStringName = connectionStringName ?? throw new ArgumentNullException(nameof(connectionStringName));
            Favicon = favicon ?? throw new ArgumentNullException(nameof(favicon));
            AppleTouchIcon = appleTouchIcon ?? throw new ArgumentNullException(nameof(appleTouchIcon));
            AppleTouchIconSize = appleTouchIconSize ?? throw new ArgumentNullException(nameof(appleTouchIconSize));
            WebAppTitle = webAppTitle ?? throw new ArgumentNullException(nameof(webAppTitle));
            FullTeamName = fullTeamName ?? throw new ArgumentNullException(nameof(fullTeamName));
        }

        public string Name { get; }

        public string Database { get; }

        // TODO Remove
        public string ConnectionStringName { get; }

        public string Favicon { get; }

        public string AppleTouchIcon { get; }

        public string AppleTouchIconSize { get; }

        public string WebAppTitle { get; }

        public string FullTeamName { get; }
    }
}