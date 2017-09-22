using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenPop.Pop3;
using AlertTrader.Classes;
using AlertTrader.Misc;
using OpenPop.Mime;

namespace AlertTrader
{
    public static class EmailChecker
    {
        public static string CheckEmailAlert()
        {
            string ret;
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect("pop-mail.outlook.com", 995, true);
                client.Authenticate(Properties.Settings.Default.email, Properties.Settings.Default.password, AuthenticationMethod.UsernameAndPassword);
                int count = client.GetMessageCount();

                if (count == 0)
                {
                    ret = string.Format("| No messages found on email {0}", Properties.Settings.Default.email);
                }
                else
                {
                    Message message = client.GetMessage(count);
                    StringBuilder builder = new StringBuilder();

                    if (message.Headers.From.Address == "noreply@tradingview.com" && message.Headers.Subject.StartsWith("TradingView Alert"))
                    {
                        string subject = message.Headers.Subject;
                        ret = subject;
                    }
                    else
                    {
                        ret = string.Format("| Found Non TradingView message: > {0}  > Deleting it !", message.Headers.Subject);
                    }
                                        
                    client.DeleteMessage(1);
                    client.Disconnect();

                    
                }
                return ret;
            }
        }

    }
}
