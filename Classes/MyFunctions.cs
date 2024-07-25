using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;

namespace ColecaoNumismatica.Classes
{
    public class MyFunctions
    {

        /// <summary>
        /// Função para envio de E-mails
        /// </summary>
        /// <param name="email"></param>
        /// <param name="body"></param>
        public static void SendEmail(string email, string body, string subject)
        {
            //Tratamento de exceção.... Try_Catch
            //Try_Catch para a messagem para prevenir que a mensagem está correta
            try
            {
                SmtpClient servidor = new SmtpClient();//Servidor SMTP - pass, user, porto

                MailMessage emails = new MailMessage(); //Email
                emails.Subject = subject; //Nome de quem é o email (tb_mail)
                emails.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_MailUser"]); //Sender
                emails.To.Add(new MailAddress(email)); //Destinatário

                emails.Body = body;
                emails.IsBodyHtml = true; //Reconhece a formatação do HTML, tags, p.e., links,<b>,<p>,etc. Poderemos usar plain-text IsBodyHtml= false.

                servidor.Host = ConfigurationManager.AppSettings["SMTP_URL"]; //SMTP URL configurada no WebConfig
                servidor.Port = int.Parse(ConfigurationManager.AppSettings["SMTP_Port"]); //SMTP Port configurada no WebConfig

                string user = ConfigurationManager.AppSettings["SMTP_MailUser"]; //SMTP User Mail configurado no WebConfig
                string pw = ConfigurationManager.AppSettings["SMTP_Pass"]; //SMTP Pass Mail configurado no WebConfig

                servidor.Credentials = new NetworkCredential(user, pw); //Indicar as credenciais do utilizador

                servidor.EnableSsl = true; //Habilitar o SSL - o SMTP Client usa o SSL para criptografar a ligação

                servidor.Send(emails); //O objeto servidor envia o mail

            }
            catch (Exception ex)
            {
                /*ex.Message = ""*/;//Mensagem de insucesso conforme a exceção encontrada.
            }

        }

        /// <summary>
        /// Função de Encriptação de Dados MD5
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string EncryptString(string Message)
        {
            string Passphrase = "Patrícia Rocha";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string

            string enc = Convert.ToBase64String(Results);
            enc = enc.Replace("+", "KKK");
            enc = enc.Replace("/", "JJJ");
            enc = enc.Replace("\\", "III");
            return enc;
        }

        /// <summary>
        /// Função de Desencritação de Dados MD5
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string DecryptString(string Message)
        {
            string Passphrase = "Patrícia Rocha";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]

            Message = Message.Replace("KKK", "+");
            Message = Message.Replace("JJJ", "/");
            Message = Message.Replace("III", "\\");


            byte[] DataToDecrypt = Convert.FromBase64String(Message);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }

        /// <summary>
        /// Listar Elementos Money
        /// </summary>
        /// <param name="query"></param>
        public static List<Money> Listar(string query)
        {
            List<Money> LstMoney = new List<Money>();
            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString);
            SqlCommand myCommand = new SqlCommand(query, myCon);
            myCon.Open();

            SqlDataReader dr = myCommand.ExecuteReader();

            while (dr.Read())
            {
                Money record = new Money();
                record.cod = Convert.ToInt32(dr["CodMN"]);
                record.titulo = dr["Titulo"].ToString();
                record.estado = dr["Estado"].ToString();
                record.valorCunho = dr["ValorCunho"].ToString();
                record.valorAtual = Convert.ToDecimal(dr["ValorAtual"]);
                record.imagem = "data:image/jpeg;base64," + Convert.ToBase64String((byte[])dr["Imagem"]);
                record.imagemC = Convert.ToBase64String((byte[])dr["Imagem"]);

                LstMoney.Add(record);
            }

            

            myCon.Close();

            return LstMoney;
        }

    }
}