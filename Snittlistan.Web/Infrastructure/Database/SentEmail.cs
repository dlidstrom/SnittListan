﻿#nullable enable

using Snittlistan.Queue;

namespace Snittlistan.Web.Infrastructure.Database;

public class SentEmail : HasVersion
{
    public SentEmail(
        string fromEmail,
        string toEmail,
        string bccEmail,
        string subject,
        object data)
    {
        FromEmail = fromEmail;
        ToEmail = toEmail;
        BccEmail = bccEmail;
        Subject = subject;
        Data = data.ToJson();
    }

    private SentEmail()
    {
    }

    public int SentEmailId { get; private set; }

    public string FromEmail { get; private set; } = null!;

    public string ToEmail { get; private set; } = null!;

    public string BccEmail { get; private set; } = null!;

    public string Subject { get; private set; } = null!;

    public string Data { get; private set; } = null!;
}
