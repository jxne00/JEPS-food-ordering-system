using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Mail;

public static class EmailUtl
{
   private static IConfiguration config =
      new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json")
         .Build()
         .GetSection("EmailSettings");

   private static string EMAIL_ID = config.GetValue<String>("LiveID");
   private static string EMAIL_PW = config.GetValue<String>("LivePW");

   // Using Microsoft's LIVE
   private static string HOST = "smtp.live.com";
   private static int PORT = 25;

   public static bool SendEmail(string recipient,
                                string subject, string msg,
                            out string error)
   {
      SmtpClient client = new SmtpClient(HOST, PORT);
      client.EnableSsl = true;
      client.Timeout = 100000;
      client.Credentials = new System.Net.NetworkCredential(EMAIL_ID, EMAIL_PW);

      MailMessage mm = new MailMessage(EMAIL_ID, recipient, subject, msg);
      mm.IsBodyHtml = true;
      bool success = true;
      error = "";
      try
      {
         client.Send(mm);
      }
      catch (Exception e)
      {
         error = e.Message;
         success = false;
      }
      return success;
   }

}
