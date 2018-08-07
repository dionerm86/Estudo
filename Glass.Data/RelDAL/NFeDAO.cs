using System;
using System.Text;
using Glass.Data.RelModel;
using System.Xml;
using Glass.Data.Helper;
using System.IO;
using Glass.Data.NFeUtils;
using System.Web;

namespace Glass.Data.RelDAL
{
    public sealed class NFeDAO : Glass.Pool.PoolableObject<NFeDAO>
    {
        private NFeDAO() { }

        internal XmlElement GetXmlInfNFe(HttpContext context, XmlDocument xmlNFe)
        {
            XmlElement nfe = xmlNFe["NFe"] != null ? xmlNFe["NFe"] : xmlNFe["nfeProc"]["NFe"];
            return nfe["infNFe"];
        }

        internal XmlElement GetXmlSignature(HttpContext context, XmlDocument xmlNFe)
        {
            XmlElement nfe = xmlNFe["NFe"] != null ? xmlNFe["NFe"] : xmlNFe["nfeProc"]["NFe"];
            return nfe["Signature"];
        }

        internal XmlElement GetXmlInfNFeSupl(HttpContext context, XmlDocument xmlNFe)
        {
            XmlElement nfe = xmlNFe["NFe"] != null ? xmlNFe["NFe"] : xmlNFe["nfeProc"]["NFe"];
            return nfe["infNFeSupl"];
        }

        /// <summary>
        /// Busca XML da NFe passada para imprimir o DANFE
        /// </summary>
        /// <param name="idNFe"></param>
        /// <returns></returns>
        public NFe GetForDanfe(string chaveAcesso)
        {
            return GetForDanfe(HttpContext.Current, chaveAcesso);
        }

        /// <summary>
        /// Busca XML da NFe passada para imprimir o DANFE
        /// </summary>
        /// <param name="idNFe"></param>
        /// <returns></returns>
        internal NFe GetForDanfe(HttpContext context, string chaveAcesso)
        {
            NFe nfe = new NFe();

            // Verifica se NFe existe
            if (!File.Exists(Utils.GetNfeXmlPathInternal(context) + chaveAcesso + "-nfe.xml"))
                throw new Exception("Arquivo da NF-e não encontrado.");

            // Busca arquivo XML da NFe
            XmlDocument xmlNFe = new XmlDocument();
            xmlNFe.Load(Utils.GetNfeXmlPathInternal(context) + chaveAcesso + "-nfe.xml");
            XmlElement xmlInfNFe = GetXmlInfNFe(context, xmlNFe);

            var versao = xmlNFe["nfeProc"] != null ?
                xmlNFe["nfeProc"]["NFe"]["infNFe"].GetAttribute("versao") :
                xmlNFe["NFe"]["infNFe"].GetAttribute("versao");

            #region Busca dados do XML da NF-e

            // Cabeçalho
            nfe.RazaoSocialEmit = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfNFe, "emit", "xNome"));
            nfe.NumeroNfe = Formatacoes.MascaraNroNFe(GetNodeValue(xmlInfNFe, "ide", "nNF"));
            nfe.SerieNfe = GetNodeValue(xmlInfNFe, "ide", "serie");
            nfe.ModeloNfe = GetNodeValue(xmlInfNFe, "ide", "mod");
            nfe.EnderecoEmit = Formatacoes.RestauraStringDocFiscal(GetEnderecoEmit(xmlInfNFe));
            nfe.TipoNfe = GetNodeValue(xmlInfNFe, "ide", "tpNF");
            nfe.TipoAmbiente = Glass.Conversoes.StrParaInt(GetNodeValue(xmlInfNFe, "ide", "tpAmb"));
            nfe.ChaveAcesso = Formatacoes.MascaraChaveAcessoNFe(chaveAcesso);
            nfe.UfNfe = Glass.Conversoes.StrParaInt(GetNodeValue(xmlInfNFe, "ide", "cUF"));
            nfe.TipoEmissao = Glass.Conversoes.StrParaInt(GetNodeValue(xmlInfNFe, "ide", "tpEmis"));

            // Dados da NF-e
            nfe.NatOperacao = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfNFe, "ide", "natOp"));
            nfe.InscEstEmit = GetNodeValue(xmlInfNFe, "emit", "IE");
            nfe.InscEstStEmit = GetNodeValue(xmlInfNFe, "emit", "IEST");
            nfe.CnpjEmit = Formatacoes.MascaraCnpj(GetNodeValue(xmlInfNFe, "emit", "CNPJ"));

            if (xmlNFe["NFe"] != null && xmlNFe["NFe"]["infProt"] != null) // Sql montado no sistema
                nfe.ProtAutorizacao = GetNodeValue(xmlNFe["NFe"], "infProt", "nProt") + " " + GetNodeValue(xmlNFe["NFe"], "infProt", "dhRecbto").Replace("T", " ");
            else if (xmlNFe["nfeProc"] != null && xmlNFe["nfeProc"]["protNFe"] != null) // Sql montado na receita
                nfe.ProtAutorizacao = GetNodeValue(xmlNFe["nfeProc"]["protNFe"], "infProt", "nProt") + " " + GetNodeValue(xmlNFe["nfeProc"]["protNFe"], "infProt", "dhRecbto").Replace("T", " ");

            // Preencher a partir do banco até mudarmos para baixar o xml direto da receita com o protocolo, caso a nota fique sem após a autorização
            if (string.IsNullOrEmpty(nfe.ProtAutorizacao))
                nfe.ProtAutorizacao = DAL.NotaFiscalDAO.Instance.ObtemValorCampo<string>("NumProtocolo", "ChaveAcesso=?chaveAcesso", new GDA.GDAParameter("?chaveAcesso", chaveAcesso));

            // Esta verificação não pode ser feita ao pré-visualizar nota
            //if (String.IsNullOrEmpty(nfe.ProtAutorizacao))
            //    throw new Exception("Nota sem protocolo.");

            // Destinatário/Remetente
            nfe.RazaoSocialRemet = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfNFe, "dest", "xNome"));
            nfe.CpfCnpjRemet = Formatacoes.MascaraCpfCnpj(GetNodeValue(xmlInfNFe, "dest", "CNPJ") + GetNodeValue(xmlInfNFe, "dest", "CPF"));
            nfe.EnderecoRemet = Formatacoes.RestauraStringDocFiscal((GetNodeValue(xmlInfNFe, "dest/enderDest", "xLgr") + ", " + GetNodeValue(xmlInfNFe, "dest/enderDest", "nro") + " " + GetNodeValue(xmlInfNFe, "dest/enderDest", "xCpl")));
            nfe.BairroRemet = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfNFe, "dest/enderDest", "xBairro"));
            nfe.CepRemet = GetNodeValue(xmlInfNFe, "dest/enderDest", "CEP");
            nfe.MunicipioRemet = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfNFe, "dest/enderDest", "xMun"));
            nfe.FoneRemet = GetNodeValue(xmlInfNFe, "dest/enderDest", "fone");
            nfe.UfRemet = GetNodeValue(xmlInfNFe, "dest/enderDest", "UF");
            nfe.InscEstRemet = GetNodeValue(xmlInfNFe, "dest", "IE");
            try
            {
                nfe.DataEmissao = DateTime.Parse(GetNodeValue(xmlInfNFe, "ide", "dhEmi")).ToString("dd/MM/yyyy HH:mm:ss");
                nfe.DataEmissaoOriginal = GetNodeValue(xmlInfNFe, "ide", "dhEmi");
            }
            catch { }
            try
            {
                nfe.DataEntradaSaida = DateTime.Parse(GetNodeValue(xmlInfNFe, "ide", "dhSaiEnt")).ToString("dd/MM/yyyy HH:mm");
            }
            catch { }

            // Fatura/Duplicatas
            if (DAL.NotaFiscalDAO.Instance.ObtemFormaPagto(chaveAcesso) == Model.NotaFiscal.FormaPagtoEnum.AVista) // Pagamento à vista
            {
                string nFat = GetNodeValue(xmlInfNFe, "cobr/fat", "nFat");
                string vOrig = GetNodeValue(xmlInfNFe, "cobr/fat", "vOrig");
                string vLiq = GetNodeValue(xmlInfNFe, "cobr/fat", "vLiq");

                nfe.Fatura = "Pagamento à vista / Num.: " + nFat + " / V. Orig.: " + vOrig.Replace('.', ',') + " / V. Liq.: " + vLiq.Replace('.', ',');
            }
            else if (xmlInfNFe["cobr"] != null) // Pagamento à prazo/outros
            {
                // Busca tags com duplicatas
                XmlNodeList xmlListDup = xmlInfNFe["cobr"].GetElementsByTagName("dup");

                string nFat = xmlInfNFe["cobr"]["fat"] != null ? GetNodeValue(xmlInfNFe, "cobr/fat", "nFat") : string.Empty;

                foreach (XmlElement xmlDup in xmlListDup)
                    nfe.Fatura +=
                        "Num.: " + nFat + "-" + xmlDup["nDup"].InnerXml +
                        " Venc.: " + DateTime.Parse(xmlDup["dVenc"].InnerXml).ToString("dd/MM/yyyy") +
                        " Valor: " + xmlDup["vDup"].InnerXml.Replace('.', ',') + "   /   ";

                if (nfe.Fatura != null)
                    nfe.Fatura = nfe.Fatura.TrimEnd(' ').TrimEnd('/');
            }

            // Cálculo do Imposto
            nfe.BcIcms = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ICMSTot", "vBC"), 2);
            nfe.VlrIcms = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ICMSTot", "vICMS"), 2);
            nfe.BcIcmsSt = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ICMSTot", "vBCST"), 2);
            nfe.VlrIcmsSt = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ICMSTot", "vST"), 2);
            nfe.VlrFrete = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ICMSTot", "vFrete"), 2);
            nfe.VlrSeguro = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ICMSTot", "vSeg"), 2);
            nfe.Desconto = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ICMSTot", "vDesc"), 2);
            nfe.OutrasDespesas = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ICMSTot", "vOutro"), 2);
            nfe.VlrIpi = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ICMSTot", "vIPI"), 2);
            nfe.VlrTotalProd = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ICMSTot", "vProd"), 2);
            nfe.VlrTotalNota = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ICMSTot", "vNF"), 2);

            // Transportador/Volumes transportados
            string modFrete = GetNodeValue(xmlInfNFe, "transp", "modFrete");
            nfe.RazaoSocialTransp = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfNFe, "transp/transporta", "xNome"));
            nfe.CpfCnpjTransp = Formatacoes.MascaraCpfCnpj(GetNodeValue(xmlInfNFe, "transp/transporta", "CNPJ") + GetNodeValue(xmlInfNFe, "transp/transporta", "CPF"));
            nfe.FretePorConta = modFrete == "0" ? "0 - Emitente" : modFrete == "1" ? "1 - Dest/Rem" : modFrete == "2" ? "2 - Terceiros" : modFrete == "3" ? "3 - Proprio/Rem" : "9 - Sem Frete";
            nfe.CodAntt = GetNodeValue(xmlInfNFe, "transp/veicTransp", "RNTC");
            nfe.PlacaVeiculo = GetNodeValue(xmlInfNFe, "transp/veicTransp", "placa");
            nfe.UfVeiculo = GetNodeValue(xmlInfNFe, "transp/veicTransp", "UF");
            nfe.EnderecoTransp = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfNFe, "transp/transporta", "xEnder"));
            nfe.MunicipioTransp = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfNFe, "transp/transporta", "xMun"));
            nfe.UfTransp = GetNodeValue(xmlInfNFe, "transp/transporta", "UF");
            nfe.InscEstTransp = GetNodeValue(xmlInfNFe, "transp/transporta", "IE");
            nfe.QtdTransp = GetNodeValue(xmlInfNFe, "transp/vol", "qVol");
            nfe.EspecieTransp = GetNodeValue(xmlInfNFe, "transp/vol", "esp");
            nfe.MarcaTransp = GetNodeValue(xmlInfNFe, "transp/vol", "marca");
            nfe.NumeracaoTransp = GetNodeValue(xmlInfNFe, "transp/vol", "nVol");
            nfe.PesoLiquido = GetNodeValue(xmlInfNFe, "transp/vol", "pesoL");
            nfe.PesoBruto = GetNodeValue(xmlInfNFe, "transp/vol", "pesoB");

            // Cálculo do ISSQN
            nfe.InscMunicIssqn = GetNodeValue(xmlInfNFe, "emit/enderEmit", "IM");
            nfe.VlrTotalServicosIssqn = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ISSQNtot", "vServ"), 2);
            nfe.BcIssqn = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ISSQNtot", "vBC"), 2);
            nfe.VlrIssqn = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfNFe, "total/ISSQNtot", "vISS"), 2);

            // Dados adicionais
            nfe.InformacoesCompl = (!string.IsNullOrEmpty(GetNodeValue(xmlInfNFe, "infAdic", "infAdFisco")) ? GetNodeValue(xmlInfNFe, "infAdic", "infAdFisco") + "\r\n" : "") +
                (!string.IsNullOrEmpty(GetNodeValue(xmlInfNFe, "infAdic", "infCpl")) ? Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfNFe, "infAdic", "infCpl")) + "\r\n" : "") +
                (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao || nfe.ModeloNfe == "65" ? string.Empty : "SEM VALOR FISCAL.");

            // Dados de controle
            nfe.Crt = GetNodeValue(xmlInfNFe, "emit", "CRT");

            //NFC-e
            if (nfe.ModeloNfe == "65")
            {
                var xmlSignature = GetXmlSignature(context, xmlNFe);
                if (xmlSignature != null)
                {
                    nfe.DigestValue = GetNodeValue(xmlSignature, "SignedInfo/Reference", "DigestValue");

                    nfe.LinkQrCode = GetXmlInfNFeSupl(context, xmlNFe)["qrCode"].InnerText;
                    nfe.UrlChave = GetXmlInfNFeSupl(context, xmlNFe)["urlChave"].InnerText;
                }
            }

            #endregion

            return nfe;
        }

        #region Endereço completo do emitente

        /// <summary>
        /// Retorna o endereço completo do emitente em apenas uma string
        /// </summary>
        /// <param name="xmlInfNfe"></param>
        /// <returns></returns>
        private string GetEnderecoEmit(XmlElement xmlInfNfe)
        {
            StringBuilder enderCompleto = new StringBuilder();
            enderCompleto.Append(GetNodeValue(xmlInfNfe, "emit/enderEmit", "xLgr") + ", ");
            enderCompleto.Append(GetNodeValue(xmlInfNfe, "emit/enderEmit", "nro"));

            if (!String.IsNullOrEmpty(GetNodeValue(xmlInfNfe, "emit/enderEmit", "xCpl")))
                enderCompleto.Append(" - " + GetNodeValue(xmlInfNfe, "emit/enderEmit", "xCpl"));

            enderCompleto.Append(" - " + GetNodeValue(xmlInfNfe, "emit/enderEmit", "xBairro") + ", ");
            enderCompleto.Append(GetNodeValue(xmlInfNfe, "emit/enderEmit", "xMun") + ", ");
            enderCompleto.Append(GetNodeValue(xmlInfNfe, "emit/enderEmit", "UF"));

            if (!String.IsNullOrEmpty(GetNodeValue(xmlInfNfe, "emit/enderEmit", "CEP")))
                enderCompleto.Append(" - CEP: " + GetNodeValue(xmlInfNfe, "emit/enderEmit", "CEP"));

            if (!String.IsNullOrEmpty(GetNodeValue(xmlInfNfe, "emit/enderEmit", "fone")))
                enderCompleto.Append(" - Fone: " + TrataTelefoneNFe(GetNodeValue(xmlInfNfe, "emit/enderEmit", "fone")));

            return Formatacoes.RestauraStringDocFiscal(enderCompleto.ToString());
        }

        /// <summary>
        /// Recebe um telefone no padrão da NFe para ser tratado, 10 dígitos sem símbolos (3133334444)
        /// </summary>
        /// <param name="telefone"></param>
        /// <returns></returns>
        private string TrataTelefoneNFe(string telefone)
        {
            return "(" + telefone.Substring(0, 2) + ") " + telefone.Substring(2, 4) + "-" + telefone.Substring(6);
        }

        #endregion

        #region Retorna o valor de um nodo do XML passado

        /// <summary>
        /// Retorna o valor de um nodo do XML passado
        /// </summary>
        /// <param name="xmlInfNfe">XmlElement a ser buscado o valor do nodo</param>
        /// <param name="parentsNodeName">Nome dos nodos pais do nodo desejado, quando houver mais de um
        /// nodo pai, pode ser informado da seguinte forma: "nodoPai1/nodoPai2/nodoPaiX"</param>
        /// <param name="nodeName">Nodo do xml que se deseja extrair conteúdo</param>
        /// <returns></returns>
        public string GetNodeValue(XmlElement xmlInfNfe, string parentsNodeName, string nodeName)
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

        #region Utilizado pelo ReportViewer

        public NFe[] GetAll()
        {
            return new NFe[0];
        }

        #endregion
    }
}
