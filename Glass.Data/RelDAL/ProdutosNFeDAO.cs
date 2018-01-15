using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using System.Xml;
using System.IO;
using Glass.Data.Helper;
using System.Web;

namespace Glass.Data.RelDAL
{
    public sealed class ProdutosNFeDAO : Glass.Pool.PoolableObject<ProdutosNFeDAO>
    {
        private ProdutosNFeDAO() { }

        /// <summary>
        /// Busca XML da NFe passada para imprimir o DANFE
        /// </summary>
        /// <param name="idNFe"></param>
        /// <returns></returns>
        public ProdutosNFe[] GetForDanfe(string chaveAcesso)
        {
            return GetForDanfe(HttpContext.Current, chaveAcesso);
        }

        /// <summary>
        /// Busca XML da NFe passada para imprimir o DANFE
        /// </summary>
        /// <param name="idNFe"></param>
        /// <returns></returns>
        internal ProdutosNFe[] GetForDanfe(HttpContext context, string chaveAcesso)
        {
            List<ProdutosNFe> lstProdNFe = new List<ProdutosNFe>();
            
            // Verifica se NFe existe
            if (!File.Exists(Utils.GetNfeXmlPathInternal(context) + chaveAcesso + "-nfe.xml"))
                throw new Exception("Arquivo da NF-e não encontrado.");

            // Busca arquivo XML da NFe
            XmlDocument xmlNFe = new XmlDocument();
            xmlNFe.Load(Utils.GetNfeXmlPathInternal(context) + chaveAcesso + "-nfe.xml");

            #region Busca produtos da NF-e contidos no XML

            // Busca tags com produtos
            XmlNodeList xmlListProdsNFe = NFeDAO.Instance.GetXmlInfNFe(context, xmlNFe).GetElementsByTagName("det");

            foreach (XmlElement xmlProd in xmlListProdsNFe)
            {
                ProdutosNFe prod = new ProdutosNFe();
                prod.Codigo = GetNodeValue(xmlProd, "prod", "cProd");
                prod.Descricao = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlProd, "prod", "xProd"));
                prod.NcmSh = GetNodeValue(xmlProd, "prod", "NCM");
                prod.Cfop = GetNodeValue(xmlProd, "prod", "CFOP");
                prod.Unidade = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlProd, "prod", "uCom"));
                prod.Qtd = Formatacoes.FormataValorDecimal(GetNodeValue(xmlProd, "prod", "qCom"), 4);
                prod.VlrUnit = Formatacoes.FormataValorDecimal(GetNodeValue(xmlProd, "prod", "vUnCom"), 2);
                prod.VlrTotal = Formatacoes.FormataValorDecimal(GetNodeValue(xmlProd, "prod", "vProd"), 2);
                prod.Cst = GetICMSNodeValue(xmlProd, "imposto/ICMS", "orig") + GetICMSNodeValue(xmlProd, "imposto/ICMS", "CST");
                prod.Csosn = GetICMSNodeValue(xmlProd, "imposto/ICMS", "orig") + GetICMSNodeValue(xmlProd, "imposto/ICMS", "CSOSN");
                prod.BcIcms = Formatacoes.FormataValorDecimal(GetICMSNodeValue(xmlProd, "imposto/ICMS", "vBC"), 2);
                prod.AliqIcms = Formatacoes.FormataValorDecimal(GetICMSNodeValue(xmlProd, "imposto/ICMS", "pICMS"), 2);
                prod.VlrIcms = Formatacoes.FormataValorDecimal(GetICMSNodeValue(xmlProd, "imposto/ICMS", "vICMS"), 2);
                prod.BcIcmsSt = Formatacoes.FormataValorDecimal(GetICMSNodeValue(xmlProd, "imposto/ICMS", "vBCST"), 2);
                prod.VlrIcmsSt = Formatacoes.FormataValorDecimal(GetICMSNodeValue(xmlProd, "imposto/ICMS", "vICMSST"), 2);
                prod.AliqIpi = Formatacoes.FormataValorDecimal(GetNodeValue(xmlProd, "imposto/IPI/IPITrib", "pIPI"), 2);
                prod.VlrIpi = Formatacoes.FormataValorDecimal(GetNodeValue(xmlProd, "imposto/IPI/IPITrib", "vIPI"), 2);

                lstProdNFe.Add(prod);

                if (xmlProd["infAdProd"] != null)
                {
                    ProdutosNFe pNf = new ProdutosNFe();
                    pNf.InfAdic = true;
                    pNf.Descricao = Formatacoes.RestauraStringDocFiscal(xmlProd["infAdProd"].InnerText);
                    lstProdNFe.Add(pNf);
                }
            }

            #endregion

            return lstProdNFe.ToArray();
        }

        #region Retorna o valor de um nodo do XML passado

        /// <summary>
        /// Retorna dados do icms de um produto da NFe, independete da tag que estiver (ICMS00, ICMS60, etc)
        /// </summary>
        /// <param name="xmlInfNfe"></param>
        /// <param name="parentsNodeName"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private string GetICMSNodeValue(XmlElement xmlProdNfe, string parentsNodeName, string nodeName)
        {
            if (xmlProdNfe == null)
                return String.Empty;

            string[] parentsNodes = parentsNodeName.Split('/');

            // Verifica se xml possui nodo pai passado, se possuir, vai entrando no XML, 
            // deixando de lado níveis acima que não interessam
            foreach (string pNode in parentsNodes)
            {
                if (xmlProdNfe[pNode] != null)
                    xmlProdNfe = xmlProdNfe[pNode];
                else
                    return String.Empty;
            }

            // Pega a tag do icms (icms00, icms60, etc)
            if (xmlProdNfe.ChildNodes[0][nodeName] != null)
                return xmlProdNfe.ChildNodes[0][nodeName].InnerXml;
            else
                return String.Empty;
        }

        /// <summary>
        /// Retorna o valor de um nodo do XML passado
        /// </summary>
        /// <param name="xmlInfNfe">XmlElement a ser buscado o valor do nodo</param>
        /// <param name="parentsNodeName">Nome dos nodos pais do nodo desejado, quando houver mais de um
        /// nodo pai, pode ser informado da seguinte forma: "nodoPai1/nodoPai2/nodoPaiX"</param>
        /// <param name="nodeName">Nodo do xml que se deseja extrair conteúdo</param>
        /// <returns></returns>
        private string GetNodeValue(XmlElement xmlInfNfe, string parentsNodeName, string nodeName)
        {
            if (xmlInfNfe == null)
                return String.Empty;

            string[] parentsNodes = parentsNodeName.Split('/');

            // Verifica se xml possui nodo pai passado, se possuir, vai entrando no XML, 
            // deixando de lado níveis acima que não interessam
            foreach (string pNode in parentsNodes)
            {
                if (xmlInfNfe[pNode] != null)
                    xmlInfNfe = xmlInfNfe[pNode];
                else
                    return String.Empty;
            }

            if (xmlInfNfe[nodeName] != null)
                return xmlInfNfe[nodeName].InnerXml;
            else
                return String.Empty;
        }

        #endregion
    }
}
