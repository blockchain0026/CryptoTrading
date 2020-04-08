using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Services
{
    public class EmailService : IEmailService
    {
        public async Task Send(string title, string message, string fromMailAddress = null, string fromName = null, List<string> to = null)
        {

            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            if (to == null)
            {
                msg.To.Add("blockchain0026@gmail.com");
                msg.To.Add("pet973926@gmail.com");
                msg.To.Add("martin973926@gmail.com");

                //msg.To.Add("b@b.com");可以發送給多人
                //msg.CC.Add("c@c.com");
                //msg.CC.Add("c@c.com");可以抄送副本給多人 
            }
            else
            {
                foreach (var targetMail in to)
                {
                    msg.To.Add(targetMail);
                }
            }

            msg.From = new MailAddress(fromMailAddress ?? "blockchain0026emailservice@gmail.com",
                fromName ?? "歐樂炒幣中心", System.Text.Encoding.UTF8);
            /* 上面3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼*/


            msg.Subject = title;//郵件標題


            msg.SubjectEncoding = System.Text.Encoding.UTF8;//郵件標題編碼
            msg.Body = message; //郵件內容

            msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
                                                         //msg.Attachments.Add(new Attachment(@"D:\test2.docx"));  //附件
                                                         //msg.IsBodyHtml = true;//是否是HTML郵件 
                                                         //msg.Priority = MailPriority.High;//郵件優先級 

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("blockchain0026emailservice@gmail.com", "blockchain123465"); //這裡要填正確的帳號跟密碼
            client.Host = "smtp.gmail.com"; //設定smtp Server
            client.Port = 25; //設定Port
            client.EnableSsl = true; //gmail預設開啟驗證

            client.Send(msg); //寄出信件

            client.Dispose();
            msg.Dispose();
        }
    }
}
