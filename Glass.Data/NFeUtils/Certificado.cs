using System;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using GDA;

namespace Glass.Data.NFeUtils
{
    public class Certificado
    {
        /// <summary>
        /// (Sobrecarga) Retorna o certificado cadastrado para a loja passada
        /// </summary>
        public static X509Certificate2 GetCertificado(uint idLoja)
        {
            return GetCertificado(null, idLoja);
        }

        /// <summary>
        /// Retorna o certificado cadastrado para a loja passada
        /// </summary>
        public static X509Certificate2 GetCertificado(GDASession session, uint idLoja)
        {
            return GetCertificado(session, idLoja, Utils.GetCertPath);
        }

        /// <summary>
        /// (Sobrecarga) Retorna o certificado cadastrado para a loja passada
        /// </summary>
        public static X509Certificate2 GetCertificado(uint idLoja, string certPath)
        {
            return GetCertificado(null, idLoja, Utils.GetCertPath);
        }

        /// <summary>
        /// Retorna o certificado cadastrado para a loja passada
        /// </summary>
        public static X509Certificate2 GetCertificado(GDASession session, uint idLoja, string certPath)
        {
            Loja loja = LojaDAO.Instance.GetElement(session, idLoja);

            // Verifica se o arquivo existe
            if (!File.Exists(certPath + "loja" + loja.IdLoja + ".pfx"))
                throw new Exception("Nenhum certificado foi cadastrado para esta loja.");

            X509Certificate2 cert = new X509Certificate2();

            byte[] rawData;

            // Cria certificado
            using (FileStream f = File.Open(certPath + "loja" + loja.IdLoja + ".pfx", FileMode.Open, FileAccess.Read))
            {
                f.Position = 0;
                rawData = new byte[f.Length];
                f.Read(rawData, 0, rawData.Length);
            }

            try
            {
                cert.Import(rawData, loja.SenhaCert, X509KeyStorageFlags.MachineKeySet);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("senha de rede"))
                    throw new Exception("A senha cadastrada para o certificado é inválida.");

                throw new Exception("Falha ao carregar pfx. " + ex.Message);
            }

            return cert;
        }
    }
}