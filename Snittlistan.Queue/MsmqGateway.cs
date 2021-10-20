﻿#nullable enable

namespace Snittlistan.Queue
{
    using System;
    using System.Messaging;
    using System.Reflection;
    using log4net;
    using Snittlistan.Queue.Messages;

    public static class MsmqGateway
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static MessageQueue? messageQueue;

        public static void Initialize(string path)
        {
            messageQueue = new MessageQueue(path)
            {
                Formatter = new JsonMessageFormatter()
            };
        }

        public static MsmqTransactionScope AutoCommitScope()
        {
            return messageQueue == null ? throw new Exception("Initialize MsmqGateway") : new MsmqTransactionScope();
        }

        public class MsmqTransactionScope : IMsmqTransaction, IDisposable
        {
            private readonly MessageQueueTransaction transaction = new();

            public MsmqTransactionScope()
            {
                transaction.Begin();
            }

            public void PublishMessage(MessageEnvelope envelope)
            {
                Log.InfoFormat("Sending {0}", envelope);
                messageQueue!.Send(envelope, transaction);
            }

            public void Commit()
            {
                try
                {
                    transaction.Commit();
                }
                catch
                {
                    transaction.Abort();
                    throw;
                }
            }

            public void Dispose()
            {
                transaction.Dispose();
            }
        }
    }
}
