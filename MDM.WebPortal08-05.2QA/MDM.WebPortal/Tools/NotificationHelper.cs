using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;

namespace MDM.WebPortal.Tools
{
    public class NotificationHelper
    {
        #region Constants

        private const string DEF_standardExSubject = "VOB notification";

        private const string HtmlEmailHeader = "";
        private const string HtmlEmailFooter = "";

        #endregion

        #region Fields

        private string _credMailFrom;
        private string _credPass;
        private List<string> _emailsToAdmin = new List<string>();
        private List<string> _emailsTo = new List<string>();
        private string _host;
        private int _port;

        private bool _isEmailsToValid = false;
        private bool _isEmailsToAdminValid = false;
        private bool _isCoreEmailSettingsValid = false;
        private List<string> _propertiesForMail = new List<string>()
            {
                "Message", 
                "InnerException", 
                "StackTrace"
            };

        #endregion

        #region Properties

        /// <summary>
        /// Include or exclude exception properties which will be sent.
        /// </summary>
        public List<string> Fields
        {
            get { return _propertiesForMail; }
            set { _propertiesForMail = value; }
        }

        #endregion

        #region Constructors

        public NotificationHelper()
        {
            _isEmailsToValid = InitEmailsToVOBCoreTeam();
            _isEmailsToAdminValid = InitEmailsToSystemAdmin();
            _isCoreEmailSettingsValid = InitEmailsFrom() && InitHost() && InitPort() && InitCredentials();

            _credMailFrom = "do_not_replay@medprosystems.net";
            _credPass = "MEDpro15!";
            _host = "smtp.office365.com";
            _port = 587;
        }

        #endregion

        public void SendEmailsAdvanced(List<string> emailsTo, string subject, string body)
        {
            //if (_isCoreEmailSettingsValid)
            //{
                using (SmtpClient smtpClient = new SmtpClient(_host, _port)
                {
                    Credentials = new NetworkCredential(_credMailFrom, _credPass),
                    EnableSsl = true
                })
                {
                    MailMessage message = new MailMessage()
                    {
                        From = new MailAddress(_credMailFrom),
                        Body = body,
                        Subject = subject,
                        IsBodyHtml = true
                    };
                    foreach (var email in emailsTo)
                    {
                        message.To.Add(email);
                    }

                    try
                    {
                        smtpClient.Send(message);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        List<string> emailsToSecondAttempt = new List<string>();

                        foreach (SmtpFailedRecipientException frsEx in ex.InnerExceptions)
                        {
                            SmtpStatusCode status = frsEx.StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable ||
                                status == SmtpStatusCode.TransactionFailed)
                            {
                                emailsToSecondAttempt.Add(frsEx.FailedRecipient);
                            }
                            else
                            {
                                LogDAL.Log.ErrorFormat("Failed to deliver message to {0}", frsEx.FailedRecipient);
                            }
                        }

                        Thread.Sleep(5000);
                        SendEmailsSecondAttempt(emailsToSecondAttempt, subject, body);
                    }
                    catch (SmtpFailedRecipientException smtpEx)
                    {
                        LogDAL.Log.Error("Email: " + smtpEx.FailedRecipient + " was not sent.");

                        LogExeption(smtpEx);

                        LogDAL.Log.Error("Send eamil failere with statusCode: " + smtpEx.StatusCode);

                        if (smtpEx.StatusCode == SmtpStatusCode.MailboxBusy ||
                            smtpEx.StatusCode == SmtpStatusCode.MailboxUnavailable ||
                            smtpEx.StatusCode == SmtpStatusCode.TransactionFailed)
                        {
                            //Log wait 5 seconds, try a second time
                            Thread.Sleep(5000);
                            SendEmailSecondAttempt(smtpEx.FailedRecipient, subject, body);
                        }
                        //else
                        //{
                        //    throw;
                        //}
                    }
                    catch (SmtpException smtpEx)
                    {
                        LogDAL.Log.Error("Send eamil failere with statusCode: " + smtpEx.StatusCode);
                        LogExeption(smtpEx);
                    }
                    finally
                    {
                        message.Dispose();
                    }
                }
            //}
            //else
            //{
            //    LogDAL.Log.Debug("Smtp Client could not be created because there were not all parameters specified.");
            //}
        }

        public void SendErrorMessageEmails(string body, string subject = DEF_standardExSubject)
        {
            if (_isEmailsToValid)
                SendEmailsLogic(_emailsTo, subject, body);
        }

        public void SendRequestsToAdmin(string body, string subject)
        {
            if (_isEmailsToAdminValid)
                SendEmailsLogic(_emailsToAdmin, subject, body);
        }

        public void SendEmails(string body, string subject, List<string> emails)
        {
            SendEmailsLogic(emails, subject, body);
        }

        public void SendEmail(string emailTo, string subject, string body)
        {
            SendEmailsLogic(new List<string>() { emailTo }, subject, body);
        }

        public void SendErrorMessage(Exception ex, string subject = DEF_standardExSubject)
        {
            string body = GetExceptionDetails(ex);
            if (_isEmailsToValid)
                SendEmailsLogic(_emailsTo, subject, body);
        }

        #region Helper Methods

        private void SendEmailsSecondAttempt(List<string> failedRecipientsEmails, string subject, string body)
        {
            try
            {
                LogDAL.Log.Debug("try send emails (second attempt)");
                SendEmailsLogic(failedRecipientsEmails, subject, body);
                LogDAL.Log.Debug("Emails (second attempt) sent successfully.");
            }
            catch (SmtpException smtpEx)
            {
                LogDAL.Log.Error("Send eamil failere with statusCode: " + smtpEx.StatusCode);
                LogExeption(smtpEx);
            }
            catch (Exception ex)
            {
                LogExeption(ex);
            }
        }

        private void SendEmailSecondAttempt(string failedRecipientEmail, string subject, string body)
        {
            try
            {
                LogDAL.Log.Debug("try send email (second attempt): " + failedRecipientEmail);
                SendEmail(failedRecipientEmail, subject, body);
                LogDAL.Log.Debug("Email " + failedRecipientEmail + " (second attempt) sent successfully.");
            }
            catch (SmtpException smtpEx)
            {
                LogDAL.Log.Error("Send eamil(second attempt) failere for email " + failedRecipientEmail);
                LogDAL.Log.Error("Send eamil failere with statusCode: " + smtpEx.StatusCode);
                LogExeption(smtpEx);
            }
            catch (Exception ex)
            {
                LogDAL.Log.Error("Send eamil(second attempt) failere for email " + failedRecipientEmail);
                LogExeption(ex);
            }
        }

        private void LogExeption(Exception ex)
        {
            LogDAL.Log.Error(ex.Message);

            if (ex.InnerException != null)
            {
                LogDAL.Log.Error("InnerException: " + ex.InnerException);
            }
        }

        //TODO: remove and use only SendEmailsAdvanced if needed
        private void SendEmailsLogic(List<string> emailsTo, string subject, string body)
        {
            if (_isCoreEmailSettingsValid)
            {
                using (SmtpClient smtpClient = new SmtpClient(_host, _port)
                {
                    Credentials = new NetworkCredential(_credMailFrom, _credPass),
                    EnableSsl = true
                })
                {
                    using (MailMessage msg = new MailMessage()
                    {
                        From = new MailAddress(_credMailFrom),
                        Body = body,
                        Subject = subject,
                        IsBodyHtml = true
                    })
                    {
                        foreach (var email in emailsTo)
                        {
                            msg.To.Add(email);
                        }

                        smtpClient.Send(msg);
                    }
                }
            }
            else
            {
                LogDAL.Log.Debug("Smtp Client could not be created because not all parameters specified.");
            }
        }

        private string GetExceptionDetails(Exception ex)
        {
            var properties = ex.GetType()
                                    .GetProperties();

            var fields = properties
                             .Select(property => new
                             {
                                 Name = property.Name,
                                 Value = property.GetValue(ex, null)
                             })
                             .Join(_propertiesForMail, o => o.Name, i => i, (a, b) => new { a.Name, a.Value })
                             .Where(f => f.Value != null)
                             .Select(x => String.Format("<p>{0} :</p><ul><li>{1}</li></ul>", x.Name, x.Value));

            return String.Join("", fields);
        }

        private bool IsValidEmail(string emailString)
        {
            return Regex.IsMatch(emailString, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        private bool InitEmailsToSystemAdmin()
        {
            List<string> allEmailsTo = CDAL.EnEmailsToSystemAdmin.Split(',').ToList();
            foreach (var email in allEmailsTo)
            {
                if (IsValidEmail(email))
                {
                    _emailsToAdmin.Add(email);
                }
            }

            if (_emailsToAdmin.Count == 0)
            {
                LogDAL.Log.Debug("EmailsTo wasn't found or incorrect");
                return false;
            }
            return true;
        }

        private bool InitEmailsToVOBCoreTeam()
        {
            List<string> allEmailsTo = CDAL.EnEmailsTo.Split(',').ToList();
            foreach (var email in allEmailsTo)
            {
                if (IsValidEmail(email))
                {
                    _emailsTo.Add(email);
                }
            }

            if (_emailsTo.Count == 0)
            {
                LogDAL.Log.Debug("EmailsTo wasn't found or incorrect");
                return false;
            }
            return true;
        }

        private bool InitEmailsFrom()
        {
            string mailFrom = CDAL.EnCredentialsMailFrom;
            if (IsValidEmail(mailFrom))
            {
                _credMailFrom = mailFrom;
            }
            else
            {
                LogDAL.Log.Debug("CredentialsMailFrom wasn't found or incorrect");
                return false;
            }
            return true;
        }

        private bool InitPort()
        {
            _port = CDAL.EnPort;
            if (_port == 0)
            {
                LogDAL.Log.Debug("Port wasn't found");
                return false;
            }
            return true;
        }

        private bool InitHost()
        {
            _host = CDAL.EnHost;
            if (string.IsNullOrEmpty(_host))
            {
                LogDAL.Log.Debug("Host wasn't found");
                return false;
            }
            return true;
        }

        private bool InitCredentials()
        {
            _credPass = CDAL.EnCredentialsPass;
            if (string.IsNullOrEmpty(_credPass))
            {
                LogDAL.Log.Debug("CredentialsPass wasn't found");
                return false;
            }
            return true;
        }

        #endregion
    }
}