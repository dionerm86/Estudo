using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Glass.UI.Web.Utils
{
    public partial class AnexarXMLNFeEntradaTerceiros : System.Web.UI.Page
    {
        private uint _idNfTer;

        protected void Page_Load(object sender, EventArgs e)
        {
            _idNfTer = Glass.Conversoes.StrParaUint(Request["idNfTer"]);
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            XmlDocument xmlNFeTer = new XmlDocument();

            if (fupXMLNFeTer.HasFile)
            {
                try
                {
                    xmlNFeTer.Load(fupXMLNFeTer.FileContent);
                }
                catch (Exception ex)
                {
                    ValidaXml(fupXMLNFeTer.FileContent);
                    throw ex;
                }
            }

            if (VerificarNFE(xmlNFeTer, fupXMLNFeTer.FileName))
            {
                try
                {
                    var chaveAcesso = string.Empty;
                    
                    if (xmlNFeTer["nfeProc"] != null)
                        chaveAcesso = xmlNFeTer["nfeProc"]["NFe"]["infNFe"].GetAttribute("Id").Remove(0, 3);
                    else
                        chaveAcesso = xmlNFeTer["NFe"]["infNFe"].GetAttribute("Id").Remove(0, 3);

                    if (NotaFiscalDAO.Instance.ObtemChaveAcesso(_idNfTer) != chaveAcesso)
                        throw new Exception("Chave de acesso da nota importada não corresponde com a chave de acesso da nota cadastrada. Verifique a chave de acesso informada.");

                    string fileName = Data.Helper.Utils.GetNfeXmlPath + chaveAcesso + "-ter.xml";

                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    xmlNFeTer.Save(fileName);
                }
                catch (Exception ex)
                {
                    throw new Exception("Falha ao salvar arquivo xml da NFe. " + ex.Message);
                }
            }
        }

        #region Valida o arquivo XML

        private void ValidaXml(Stream conteudoArquivo)
        {
            string arquivo;

            using (StreamReader f = new StreamReader(conteudoArquivo))
            {
                arquivo = f.ReadToEnd();
            }

            if (arquivo.IndexOf("<xEvento>Carta de Corre", StringComparison.Ordinal) > 0)
                throw new Exception(
                    "Este XML é referente a uma carta de correção, é possível importar somente XML de notas fiscais.");
        }

        /// <summary>
        /// Verifica se o arquivo xml inserido é uma NFe válida
        /// </summary>
        protected bool VerificarNFE(XmlDocument nfeFile, string fileName)
        {
            try
            {
                var dadosVerificar = WebGlass.Business.NotaFiscal.Fluxo.Importar.Instance.VerificarNFe(nfeFile, fileName);
                lblInfoNFE.Text = dadosVerificar.InfoNFe;
                //hdfChaveAcesso.Value = dadosVerificar.ChaveAcesso;

                return true;
            }
            catch (Exception ex)
            {
                lblInfoNFE.Text = "Erro de verificação:<br/>" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
                return false;
            }
        }

        #endregion
    }
}