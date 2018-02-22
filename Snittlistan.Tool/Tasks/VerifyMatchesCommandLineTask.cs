﻿namespace Snittlistan.Tool.Tasks
{
    using Queue;
    using Queue.Messages;

    public class VerifyMatchesCommandLineTask : ICommandLineTask
    {
        public void Run(string[] args)
        {
            using (var scope = MsmqGateway.AutoCommitScope())
            {
                foreach (var apiUrl in CommandLineTaskHelper.AllApiUrls())
                {
                    scope.PublishMessage(new MessageEnvelope(new VerifyMatchesMessage(), apiUrl));
                }

                scope.Commit();
            }
        }

        public string HelpText => "Verifies registered matches";
    }
}