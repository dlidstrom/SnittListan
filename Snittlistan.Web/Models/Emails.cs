﻿#nullable enable

namespace Snittlistan.Web.Models
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using JetBrains.Annotations;
    using Postal;
    using Snittlistan.Web.Areas.V2.Domain;
    using Snittlistan.Web.Areas.V2.ReadModels;

    public static class Emails
    {
        private static EmailService? service;

        public static async Task SendUpdateMail(
            string recipient,
            FormattedAuditLog formattedAuditLog)
        {
            await service!.SendAsync(
                new UpdateMailViewModel(recipient, ConfigurationManager.AppSettings["OwnerEmail"], formattedAuditLog));
        }

        public class UpdateMailViewModel : Email
        {
            public UpdateMailViewModel(string to, string bcc, FormattedAuditLog formattedAuditLog)
                : base("UpdateMail")
            {
                To = to;
                Bcc = bcc;
                FormattedAuditLog = formattedAuditLog;
            }

            public string Bcc { get; }
            public string From { get; } = ConfigurationManager.AppSettings["OwnerEmail"];
            public string Subject { get; } = "Uttagning har uppdaterats";
            public string To { get; }
            public FormattedAuditLog FormattedAuditLog { get; }
        }

        public static async Task SendOneTimePassword(
            string recipient,
            string subject,
            string oneTimePassword)
        {
            await Send(
                "OneTimePassword",
                recipient,
                subject,
                o =>
                {
                    o.OneTimePassword = oneTimePassword;
                });
        }

        public static async Task InviteUser(string recipient, string subject, string activationUri)
        {
            await Send(
                "InviteUser",
                recipient,
                subject,
                o =>
                {
                    o.ActivationUri = activationUri;
                });
        }

        public static async Task UserRegistered(string recipient, string subject, string id, string activationKey)
        {
            await Send(
                "UserRegistered",
                recipient,
                subject,
                o =>
                {
                    o.Id = id;
                    o.ActivationKey = activationKey;
                });
        }

        public static async Task MatchRegistered(
            string team,
            string opponent,
            int score,
            int opponentScore,
            ResultSeriesReadModel resultSeriesReadModel,
            ResultHeaderReadModel resultHeaderReadModel)
        {
            string subject = $"{team} mot {opponent}: {score} - {opponentScore}";
            await Send(
                "MatchRegistered",
                ConfigurationManager.AppSettings["OwnerEmail"],
                subject,
                o =>
                {
                    o.Subject = subject;
                    o.Team = team;
                    o.Opponent = opponent;
                    o.Score = score;
                    o.OpponentScore = opponentScore;
                    o.ResultSeriesReadModel = resultSeriesReadModel;
                    o.ResultHeaderReadModel = resultHeaderReadModel;
                });
        }

        public static async Task SendAdminMail(string subject, string content)
        {
            await Send(
                "Mail",
                ConfigurationManager.AppSettings["OwnerEmail"],
                subject,
                o => o.Content = content);
        }

        public static async Task SendMail(string email, string subject, string content)
        {
            await Send(
                "Mail",
                email,
                subject,
                o => o.Content = content);
        }

        public static void Initialize(string viewsPath)
        {
            ViewEngineCollection engines = new()
            {
                new FileSystemRazorViewEngine(viewsPath)
            };
            service = new EmailService(engines);
        }

        private static async Task Send(
            [AspMvcView] string view,
            string recipient,
            string subject,
            Action<dynamic> action)
        {
            dynamic email = new Email(view);
            email.To = recipient;
            email.From = ConfigurationManager.AppSettings["OwnerEmail"];
            email.Subject = subject;

            // add moderators
            string moderatorEmails = string.Join(", ", ConfigurationManager.AppSettings["OwnerEmail"].Split(';'));
            email.Bcc = moderatorEmails;
            action.Invoke(email);

            await service!.SendAsync(email);
        }
    }
}
