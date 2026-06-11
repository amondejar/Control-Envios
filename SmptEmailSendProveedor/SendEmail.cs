using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmptEmailSendProveedor
{
     
    public class SendEmail
    {
        private string _EmailProveedor = string.Empty;
        private string _Asunto = string.Empty;
        private string _Cuerpo = string.Empty;

        private CSmtp smtp = new CSmtp();
        private CMensage msg;
       public SendEmail(string EmailProveedor,string Asunto,string Cuerpo)
        {

           
            _EmailProveedor = EmailProveedor;
            _Asunto = Asunto;
            _Cuerpo = Cuerpo;
            

        }

       public void PrepararEnvioMail()
       {
           smtp.CargaDatosGenealesEmail();
           msg = new CMensage(_EmailProveedor, _Asunto, _Cuerpo);

       }

       public bool EnviarEmail(ref string MsgError)
       {
           bool resultado = true;
           try
           {
               smtp.Smtp.Send(msg.EmailMensage);
           }
           catch (Exception exc)
           {
               MsgError = exc.Message;
               resultado = false;
           }
           return resultado;
            
       }


    }
}
