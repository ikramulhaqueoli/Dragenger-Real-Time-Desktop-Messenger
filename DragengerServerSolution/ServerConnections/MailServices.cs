using Display;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using EntityLibrary;
using System.ComponentModel;

namespace ServerConnections
{
    public class MailServices
    {
        public static bool SendEmail(string receiverEmail, string mailSubject, string mailBody)
        {
            List<string> receiverEmailList = new List<string>();
            receiverEmailList.Add(receiverEmail);
            return SendEmail(receiverEmailList, mailSubject, mailBody);
        }
        public static bool SendEmail(List<string> receiverEmailList, string mailSubject, string mailBody)
        {
            try
            {
                string host = ConfigurationManager.AppSettings["SmtpHost"];
                string emailFrom = ConfigurationManager.AppSettings["emailFrom"];
                int port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                string credentialUsername = ConfigurationManager.AppSettings["SmtpUsername"];
                string credentialPassword = ConfigurationManager.AppSettings["SmtpPassword"];

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(host);

                mail.From = new MailAddress(emailFrom);
                foreach(string emailAddress in receiverEmailList)
                {
                    mail.To.Add(emailAddress);
                }
                
                mail.Subject = mailSubject;
                mail.IsBodyHtml = true;
                mail.Body = mailBody;

                SmtpServer.Port = port;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(credentialUsername, credentialPassword);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Output.ShowLog("Exception in SendEmail() => " + ex.Message);
                return false;
            }
        }

        internal static void SendVerificationCodeToEmail(Consumer consumer, string assignedCode, string purpose)
        {
            string mailBody = null, mailSubject = null;
            if (purpose == "email_verify")
            {
                mailBody = "<!DOCTYPE html> <html> <body style='font-family: sans-serif; -webkit-font-smoothing: antialiased; font-size: 14px; line-height: 1.4; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; margin: 0; padding: 0;' bgcolor='#f6f6f6'> <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;' bgcolor='#f6f6f6'> <tr> <td>&nbsp;</td> <td> <div style='box-sizing: border-box; display: block; max-width: 580px; margin: 0 auto; padding: 10px;'> <table role='presentation' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; border-radius: 3px;' bgcolor='#ffffff'> <tr> <td style='box-sizing: border-box; padding: 20px;'> <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;'> <tr> <td> <h2 style='color: #000000; font-family: sans-serif; font-weight: 400; line-height: 1.4; margin: 0 0 30px;'>Welcome to Dragenger, <b>" + consumer.Name + "</b> ! </h2> <p>Thanks for signing up for <b>Dragenger Account</b>. There is only one more step to go. <br> Enter the <b>verification code</b> below in <b>Dragenger App</b> to verify your email address, </p> <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; box-sizing: border-box;'> <tbody> <tr> <td align='center' style='padding-bottom: 15px;'> <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'> <tbody> <tr> <td> <a style='background-color: #FF3300; border-radius: 5px; box-sizing: border-box; color: #ffffff; cursor: pointer; display: inline-block; font-size: 18px; font-weight: bold; text-decoration: none; text-transform: capitalize; margin: 0; padding: 5px 15px; border: 1px solid #3498db;'>" + assignedCode + "</a> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> </td> </tr> </table> </td> </tr> </table> <div style='clear: both; margin-top: 10px; width: 100%;' align='center'> <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;'> <tr> <td style='padding-bottom: 10px; padding-top: 10px; color: #999999; font-size: 12px;' align='center'> <span style='color: #999999; font-size: 12px; text-align: center;'>Dragenger | A Realtime Messenger</span> <br> &copy; Copyright 2019-2020 Ikramul Haque Chowdhury Oli </td> </tr> <tr> <td style='padding-bottom: 10px; padding-top: 10px; color: #999999; font-size: 12px;' align='center'> Powered by Dragenger </td> </tr> </table> </div> </div> </td> <td>&nbsp;</td> </tr> </table> </body> </html>";
                mailSubject = "Dragenger Account | Email Verification Code";
            }
            else if (purpose == "password_reset")
            {
                mailBody = "<!DOCTYPE html> <html> <body style='font-family: sans-serif; -webkit-font-smoothing: antialiased; font-size: 14px; line-height: 1.4; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; margin: 0; padding: 0;' bgcolor='#f6f6f6'> <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;' bgcolor='#f6f6f6'> <tr> <td>&nbsp;</td> <td> <div style='box-sizing: border-box; display: block; max-width: 580px; margin: 0 auto; padding: 10px;'> <table role='presentation' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; border-radius: 3px;' bgcolor='#ffffff'> <tr> <td style='box-sizing: border-box; padding: 20px;'> <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;'> <tr> <td> <h2 style='color: #000000; font-family: sans-serif; font-weight: 400; line-height: 1.4; margin: 0 0 30px;'>Greetings from Dragenger, <b>" + consumer.Name + "</b> ! </h2> <p>Your have requested to reset your <b>Dragenger Account</b> password. There is only one more step to go. <br> Enter the <b>verification code</b> below in <b>Dragenger App</b> to <b>reset your password</b>, </p> <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; box-sizing: border-box;'> <tbody> <tr> <td align='center' style='padding-bottom: 15px;'> <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'> <tbody> <tr> <td> <a style='background-color: #FF3300; border-radius: 5px; box-sizing: border-box; color: #ffffff; cursor: pointer; display: inline-block; font-size: 18px; font-weight: bold; text-decoration: none; text-transform: capitalize; margin: 0; padding: 5px 15px; border: 1px solid #3498db;'>" + assignedCode + "</a> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> </td> </tr> </table> </td> </tr> </table> <div style='clear: both; margin-top: 10px; width: 100%;' align='center'> <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;'> <tr> <td style='padding-bottom: 10px; padding-top: 10px; color: #999999; font-size: 12px;' align='center'> <span style='color: #999999; font-size: 12px; text-align: center;'>Dragenger | A Realtime Messenger</span> <br> &copy; Copyright 2019-2020 Ikramul Haque Chowdhury Oli </td> </tr> <tr> <td style='padding-bottom: 10px; padding-top: 10px; color: #999999; font-size: 12px;' align='center'> Powered by Dragenger </td> </tr> </table> </div> </div> </td> <td>&nbsp;</td> </tr> </table> </body> </html>";
                mailSubject = "Dragenger Account | Password Reset Code";
            }
            SendEmail(consumer.Email, mailSubject, mailBody);
        }
    }
}
