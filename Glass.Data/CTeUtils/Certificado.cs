using System;
using System.Security.Cryptography.X509Certificates;
using Glass.Data.DAL;
using System.IO;
using Glass.Data.Helper;

namespace Glass.Data.CTeUtils
{
    public class Certificado
    {
        /// <summary>
        /// (Sobrecarga) Retorna o certificado cadastrado para a loja passada
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificado(uint idLoja)
        {
            return GetCertificado(idLoja, Utils.GetCertPath);
        }
        #region certificado teste A3

        public static X509Certificate2 BuscaNome(string Nome, bool usaWService)
        {
            X509Certificate2 _X509Cert = new X509Certificate2();
            try
            {
                //X509Certificate2Collection collection2 = CollectCertificates(usaWService);
                X509Certificate2Collection collection2 = CollectCertificates();

                //if (Nome == "")
                //{
                //    X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(collection2, "Certificado(s) Digital(is) disponível(is)", "Selecione o Certificado Digital para uso no aplicativo", X509SelectionFlag.SingleSelection);
                //    if (scollection.Count == 0)
                //    {
                //        _X509Cert.Reset();
                //    }
                //    else
                //    {
                //        _X509Cert = scollection[0];
                //    }
                //}
                //else
                //{
                //    X509Certificate2Collection scollection = (X509Certificate2Collection)collection2.Find(X509FindType.FindBySubjectDistinguishedName, Nome, false);
                //    if (scollection.Count == 0)
                //    {
                //        _X509Cert.Reset();
                //    }
                //    else
                //    {
                //        _X509Cert = scollection[0];
                //    }
                //}


                return collection2[0];
            }
            catch //(System.Exception ex)
            {
                return _X509Cert;
            }
        }

        public static X509Certificate2Collection CollectCertificates()
        {
            StoreLocation StLocation = StoreLocation.CurrentUser;

            //if (usaWService)
            //    StLocation = StoreLocation.LocalMachine;

            X509Store store = new X509Store("MY", StLocation);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
            X509Certificate2Collection collection1 = (X509Certificate2Collection)collection.Find(X509FindType.FindByTimeValid, DateTime.Now, true);
            X509Certificate2Collection collection2 = (X509Certificate2Collection)collection1.Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, true);

            store.Close();

            return collection2;
        }

        #endregion

        /// <summary>
        /// Retorna o certificado cadastrado para a loja passada
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificado(uint idLoja, string certPath)
        {
            Glass.Data.Model.Loja loja = LojaDAO.Instance.GetElement(idLoja);

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
