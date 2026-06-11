using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Configuration;

namespace SmptEmailSendProveedor
{

    public class CMensage
    {

        private MailMessage email = new MailMessage();
        public MailMessage EmailMensage { get; private set; }

        public CMensage(string EmailEnviador,string Asunto,string Cuerpo)
        {
            email.To.Add(new MailAddress(EmailEnviador));
            email.From = new MailAddress("notificaciones@derivadoscitricos.com");
            email.Subject = Asunto;
            email.Body = Cuerpo;
            email.BodyEncoding = UTF8Encoding.UTF8;
            email.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            this.EmailMensage = email;
        }

       

    }
}
