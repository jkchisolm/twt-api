using System.Net.Mail;
using PostmarkDotNet;

namespace TwitterAPI.Helpers;

public class EmailHelper
{
    public bool SendEmail(string recipient, string subject, string body)
    {
        var message = new PostmarkMessage()
        {
            To = recipient,
            From = "fleeter@jkchisolm.com",
            TrackOpens = true,
            Subject = subject,
            TextBody = body,
        };

        return true;
        // var client = new PostmarkClient();
    }
}