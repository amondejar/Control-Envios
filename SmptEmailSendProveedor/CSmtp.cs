using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace SmptEmailSendProveedor
{
    public class CSmtp
    {

        private string EmailNotificaciones = string.Empty;
        private string HostEmail = string.Empty;
        private string SmtpSalienteNotificaciones = string.Empty;
        private string Puerto = string.Empty;
        private string Password = string.Empty;

        private SmtpClient smtpCliente = new SmtpClient();

        public SmtpClient Smtp { get; private set; }

        


       public void CargaDatosGenealesEmail()
       {
           EmailNotificaciones = "notificaciones@derivadoscitricos.com";
           HostEmail = "smtp.derivadoscitricos.com";
           SmtpSalienteNotificaciones = "smtp.derivadoscitricos.com";
           Puerto = "25";
           Password = "ewO83)1d";

           this.CargaObjSmtp(); //Cargamos el objeto Smtp
       }


       private void CargaObjSmtp()
       {
           smtpCliente.Host = HostEmail;
           smtpCliente.Port = Convert.ToInt16(Puerto);
           smtpCliente.EnableSsl = false;
           smtpCliente.UseDefaultCredentials = false;
           smtpCliente.Timeout = 30000;
           smtpCliente.DeliveryMethod = SmtpDeliveryMethod.Network;
           smtpCliente.Credentials = new NetworkCredential(EmailNotificaciones, Password);

           this.Smtp = smtpCliente;
       }    
    }
}
