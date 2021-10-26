﻿#nullable enable

namespace Snittlistan.Queue.WindowsServiceHost
{
    using System;
    using System.Configuration;
    using System.ServiceProcess;
    using NLog;
    using Snittlistan.Queue.Config;

    public partial class QueueService : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Application application;

        public QueueService()
        {
            InitializeComponent();
            application = new(
                (MessagingConfigSection)ConfigurationManager.GetSection("messaging"),
                ConfigurationManager.AppSettings["UrlScheme"]);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Logger.Info("Starting queue service");
                application.Start();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                ExitCode = 1;
            }
        }

        protected override void OnStop()
        {
            Logger.Info("Stopping queue service");
            application.Stop();
        }
    }
}
