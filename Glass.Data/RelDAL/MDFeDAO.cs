using Glass.Data.Helper;
using Glass.Data.RelModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;

namespace Glass.Data.RelDAL
{
    public sealed class MDFeDAO : Glass.Pool.Singleton<MDFeDAO>
    {
        #region Utilizado pelo ReportViewer

        public static MDFe[] GetAll()
        {
            return new MDFe[0];
        }

        #endregion

        public static MDFe ObterParaDAMDFE(string chaveAcesso)
        {
            return ObterParaDAMDFE(HttpContext.Current, chaveAcesso);
        }

        internal static MDFe ObterParaDAMDFE(HttpContext context, string chaveAcesso)
        {
            MDFe mdfe = new MDFe();
            Glass.Data.Model.ManifestoEletronico _manifestoEletronico = Glass.Data.DAL.ManifestoEletronicoDAO.Instance.ObterPelaChaveAcesso(chaveAcesso);

            // Verifica se o MDFe existe
            if (!File.Exists(Utils.GetMDFeXmlPathInternal(context) + chaveAcesso + "-mdfe.xml"))
                throw new Exception("Arquivo do MDFe não encontrado.");

            // Carrega o arquivo XML
            XmlDocument xmlMDFe = new XmlDocument();
            xmlMDFe.Load(Utils.GetMDFeXmlPathInternal(context) + chaveAcesso + "-mdfe.xml");
            XmlElement xmlInfMDFe = ObterXmlInfMDFe(context, xmlMDFe);

            #region Busca dados do XML do MDFe

            #region IDENTIFICAÇÃO MDF-e

            mdfe.ChaveAcesso = Formatacoes.MascaraChaveAcessoNFe(chaveAcesso);
            mdfe.Modelo = GetNodeValue(xmlInfMDFe, "ide", "mod");
            mdfe.Serie = GetNodeValue(xmlInfMDFe, "ide", "serie");
            mdfe.TipoEmissao = GetNodeValue(xmlInfMDFe, "ide", "tpEmis") == "1" ? "Normal" : "Contingência";
            mdfe.NumeroManifestoEletronico = GetNodeValue(xmlInfMDFe, "ide", "nMDF");
            mdfe.TipoAmbiente = Conversoes.StrParaInt(GetNodeValue(xmlInfMDFe, "ide", "tpAmb"));
            mdfe.DataEmissao = DateTime.Parse(GetNodeValue(xmlInfMDFe, "ide", "dhEmi")).ToString("dd/MM/yyyy HH:mm");
            mdfe.UFInicio = GetNodeValue(xmlInfMDFe, "ide", "UFIni");
            mdfe.UFFim = GetNodeValue(xmlInfMDFe, "ide", "UFFim");

            //Protocolo de autorização
            if (xmlMDFe["MDFe"] != null && xmlMDFe["MDFe"]["infProt"] != null) // Sql montado no sistema
            {
                mdfe.ProtocoloAutorizacao = GetNodeValue(xmlMDFe["MDFe"], "infProt", "nProt") + " " +
                    DateTime.Parse(GetNodeValue(xmlMDFe["MDFe"], "infProt", "dhRecbto")).ToString("dd/MM/yyyy HH:mm");
            }
            else if (xmlMDFe["mdfeProc"] != null && xmlMDFe["mdfeProc"]["protMDFe"] != null) // Sql montado na receita
            {
                mdfe.ProtocoloAutorizacao = GetNodeValue(xmlMDFe["mdfeProc"]["protMDFe"], "infProt", "nProt") + " " +
                    DateTime.Parse(GetNodeValue(xmlMDFe["mdfeProc"]["protMDFe"], "infProt", "dhRecbto")).ToString("dd/MM/yyyy HH:mm");
            }

            if (mdfe.TipoEmissao == "Contingência" && string.IsNullOrWhiteSpace(mdfe.ProtocoloAutorizacao))
                mdfe.ProtocoloAutorizacao = "Impressão em contingência. Obrigatória a autorização em 168 horas após esta emissão. ("
                    + mdfe.DataEmissao + ")";

            // Emitente
            mdfe.RazaoSocialEmitente = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfMDFe, "emit", "xNome"));
            mdfe.EnderecoEmitente = Formatacoes.RestauraStringDocFiscal(ObterEnderecoEmitente(xmlInfMDFe));
            mdfe.CNPJEmitente = Formatacoes.MascaraCnpj(GetNodeValue(xmlInfMDFe, "emit", "CNPJ"));
            mdfe.InscEstEmitente = GetNodeValue(xmlInfMDFe, "emit", "IE");

            #endregion

            #region INFORMAÇÕES DO MODAL RODOVIARIO

            if (xmlInfMDFe["infModal"]["rodo"] != null)
            {
                var _RNTRC = string.Empty;

                XmlElement xmlRodo = xmlInfMDFe["infModal"]["rodo"];

                #region Grupo de informações para Agência Reguladora

                if (xmlRodo["infANTT"] != null)
                {
                    var _CIOTs = new List<string>();
                    var _ResponsaveisCIOT = new List<string>();
                    var _CNPJsResponsaveisPedagio = new List<string>();
                    var _CNPJsFornecedoresPedagio = new List<string>();
                    var _NumerosCompraPedagio = new List<string>();

                    _RNTRC = GetNodeValue(xmlRodo, "infANTT", "RNTRC");

                    var xmlListainfCIOT = xmlRodo["infANTT"].GetElementsByTagName("infCIOT");
                    foreach (XmlElement infCIOT in xmlListainfCIOT)
                    {
                        _CIOTs.Add(infCIOT["CIOT"].InnerXml);
                        _ResponsaveisCIOT.Add(Formatacoes.MascaraCnpj(infCIOT["CNPJ"].InnerXml));
                    }

                    if (xmlRodo["infANTT"]["valePed"] != null)
                    {
                        var xmlListaValePedagio = xmlRodo["infANTT"]["valePed"].GetElementsByTagName("disp");
                        foreach (XmlElement disp in xmlListaValePedagio)
                        {
                            _CNPJsResponsaveisPedagio.Add(disp["CNPJPg"] != null ? Formatacoes.MascaraCnpj(disp["CNPJPg"].InnerXml) : mdfe.CNPJEmitente);
                            _CNPJsFornecedoresPedagio.Add(Formatacoes.MascaraCnpj(disp["CNPJForn"].InnerXml));
                            _NumerosCompraPedagio.Add(disp["nCompra"].InnerXml);
                        }
                    }

                    mdfe.CIOTs = string.Join("\n", _CIOTs);
                    mdfe.ResposaveisCIOTs = string.Join("\n", _ResponsaveisCIOT);
                    mdfe.CNPJsResponsaveisPedagio = string.Join("\n", _CNPJsResponsaveisPedagio);
                    mdfe.CNPJsFornecedoresPedagio = string.Join("\n", _CNPJsFornecedoresPedagio);
                    mdfe.NumerosCompraPedagio = string.Join("\n", _NumerosCompraPedagio);
                }

                #endregion

                #region Veículos e Condutores

                var _placasVeiculos = new List<string>();
                var _RNTRCsVeiculos = new List<string>();
                var _CPFsCondutores = new List<string>();
                var _NomesCondutores = new List<string>();

                // Dados do Veículo Tração
                _placasVeiculos.Add(GetNodeValue(xmlRodo, "veicTracao", "placa"));
                if (xmlRodo["veicTracao"]["prop"] != null)
                {
                    _RNTRCsVeiculos.Add(GetNodeValue(xmlRodo, "veicTracao/prop", "RNTRC"));
                }
                else
                {
                    _RNTRCsVeiculos.Add(_RNTRC);
                }

                // Informações do(s) Condutor(s) do veículo
                var xmlListaCondutor = xmlRodo["veicTracao"].GetElementsByTagName("condutor");
                foreach (XmlElement condutor in xmlListaCondutor)
                {
                    _CPFsCondutores.Add(Formatacoes.MascaraCpf(condutor["CPF"].InnerXml));
                    _NomesCondutores.Add(condutor["xNome"].InnerXml);
                }

                // Dados do Veículo Reboque
                if (xmlRodo["veicReboque"] != null)
                {
                    var xmlListaVeicReboque = xmlRodo.GetElementsByTagName("veicReboque");
                    foreach (XmlElement veicReboque in xmlListaVeicReboque)
                    {
                        _placasVeiculos.Add(veicReboque["placa"].InnerXml);
                        if (xmlRodo["prop"] != null)
                        {
                            _RNTRCsVeiculos.Add(GetNodeValue(veicReboque, "prop", "RNTRC"));
                        }
                        else
                        {
                            _RNTRCsVeiculos.Add(_RNTRC);
                        }
                    }
                }

                mdfe.PlacasVeiculos = string.Join("\n", _placasVeiculos);
                mdfe.RNTRCsVeiculos = string.Join("\n", _RNTRCsVeiculos);
                mdfe.CPFsCondutores = string.Join("\n", _CPFsCondutores);
                mdfe.NomesCondutores = string.Join("\n", _NomesCondutores);

                #endregion
            }

            #endregion

            #region INFORMAÇÕES DOS DOCUMENTOS FISCAIS VINCULADOS

            if (mdfe.TipoEmissao == "Contingência" && xmlInfMDFe["infDoc"] != null)
            {
                var _documentosVinculados = new List<string>();
                var xmlListaInfMunDescarga = xmlInfMDFe["infDoc"].GetElementsByTagName("infMunDescarga");
                foreach (XmlElement infMunDescarga in xmlListaInfMunDescarga)
                {
                    // Se Tipo Emitente for Transportador de carga própria
                    if (GetNodeValue(xmlInfMDFe, "ide", "tpEmit") == "2")
                    {
                        var xmlListaInfNFe = infMunDescarga.GetElementsByTagName("infNFe");
                        foreach (XmlElement infNFe in xmlListaInfNFe)
                        {
                            _documentosVinculados.Add("NFe: " + infNFe["chNFe"].InnerXml);
                        }
                    }
                    else
                    {
                        var xmlListaInfCTe = infMunDescarga.GetElementsByTagName("infCTe");
                        foreach (XmlElement infCTe in xmlListaInfCTe)
                        {
                            _documentosVinculados.Add("CTe: " + infCTe["chCTe"].InnerXml);
                        }
                    }
                }

                mdfe.DocumentosFiscaisVinculados = string.Join("\n", _documentosVinculados);
            }

            #endregion

            #region TOTALIZADORES DA CARGA

            mdfe.QuantidadeCTe = string.IsNullOrWhiteSpace(GetNodeValue(xmlInfMDFe, "tot", "qCTe")) ? "0" : GetNodeValue(xmlInfMDFe, "tot", "qCTe");
            mdfe.QuantidadeNFe = string.IsNullOrWhiteSpace(GetNodeValue(xmlInfMDFe, "tot", "qNFe")) ? "0" : GetNodeValue(xmlInfMDFe, "tot", "qNFe");
            mdfe.ValorCarga = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfMDFe, "tot", "vCarga"), 2);
            mdfe.CodigoUnidade = GetNodeValue(xmlInfMDFe, "tot", "cUnid") == "1" ? "KG" : "TON";
            mdfe.PesoTotalCarga = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfMDFe, "tot", "qCarga"), 4);

            #endregion

            #region INFORMAÇÕES ADICIONAIS

            mdfe.InformacoesAdicionaisFisco = GetNodeValue(xmlInfMDFe, "infAdic", "infAdFisco");
            mdfe.InformacoesComplementares = GetNodeValue(xmlInfMDFe, "infAdic", "infCpl");

            #endregion

            #endregion

            return mdfe;
        }

        internal static XmlElement ObterXmlInfMDFe(HttpContext context, XmlDocument xmlMDFe)
        {
            XmlElement mdfe = xmlMDFe["MDFe"] != null ? xmlMDFe["MDFe"] : xmlMDFe["mdfeProc"]["MDFe"];
            return mdfe["infMDFe"];
        }

        #region Retorna o valor de um nodo do XML passado

        /// <summary>
        /// Retorna o valor de um nodo do XML passado
        /// </summary>
        /// <param name="xmlTemp">XmlElement a ser buscado o valor do nodo</param>
        /// <param name="parentsNodeName">Nome dos nodos pais do nodo desejado, quando houver mais de um
        /// nodo pai, pode ser informado da seguinte forma: "nodoPai1/nodoPai2/nodoPaiX"</param>
        /// <param name="nodeName">Nodo do xml que se deseja extrair conteúdo</param>
        /// <returns></returns>
        private static string GetNodeValue(XmlElement xmlTemp, string parentsNodeName, string nodeName)
        {
            if (xmlTemp == null)
                return string.Empty;

            string[] parentsNodes = parentsNodeName.Split('/');

            // Verifica se xml possui nodo pai passado, se possuir, vai entrando no XML,
            // deixando de lado níveis acima que não interessam
            foreach (string pNode in parentsNodes)
            {
                if (xmlTemp[pNode] != null)
                    xmlTemp = xmlTemp[pNode];
                else
                    return string.Empty;
            }

            if (xmlTemp[nodeName] != null)
                return xmlTemp[nodeName].InnerXml;
            else
                return string.Empty;
        }

        #endregion

        /// <summary>
        /// Retorna o endereço completo do emitente em apenas uma string
        /// </summary>
        /// <param name="xmlInfMDFe"></param>
        /// <returns></returns>
        private static string ObterEnderecoEmitente(XmlElement xmlInfMDFe)
        {
            StringBuilder enderCompleto = new StringBuilder();
            enderCompleto.Append(GetNodeValue(xmlInfMDFe, "emit/enderEmit", "xLgr") + ", ");
            enderCompleto.Append(GetNodeValue(xmlInfMDFe, "emit/enderEmit", "nro"));

            if (!string.IsNullOrEmpty(GetNodeValue(xmlInfMDFe, "emit/enderEmit", "xCpl")))
                enderCompleto.Append(" - " + GetNodeValue(xmlInfMDFe, "emit/enderEmit", "xCpl"));

            enderCompleto.Append(" - " + GetNodeValue(xmlInfMDFe, "emit/enderEmit", "xBairro") + ", ");
            enderCompleto.Append(GetNodeValue(xmlInfMDFe, "emit/enderEmit", "xMun") + ", ");
            enderCompleto.Append(GetNodeValue(xmlInfMDFe, "emit/enderEmit", "UF"));

            if (!string.IsNullOrEmpty(GetNodeValue(xmlInfMDFe, "emit/enderEmit", "CEP")))
                enderCompleto.Append(" - CEP: " + GetNodeValue(xmlInfMDFe, "emit/enderEmit", "CEP"));

            if (!string.IsNullOrEmpty(GetNodeValue(xmlInfMDFe, "emit/enderEmit", "fone")))
                enderCompleto.Append(" - Fone: " + TrataTelefoneMDFe(GetNodeValue(xmlInfMDFe, "emit/enderEmit", "fone")));

            return Formatacoes.RestauraStringDocFiscal(enderCompleto.ToString());
        }

        /// <summary>
        /// Recebe um telefone no padrão do MDFe para ser tratado, 10 dígitos sem símbolos (3133334444)
        /// </summary>
        /// <param name="telefone"></param>
        /// <returns></returns>
        private static string TrataTelefoneMDFe(string telefone)
        {
            return "(" + telefone.Substring(0, 2) + ") " + telefone.Substring(2, 4) + "-" + telefone.Substring(6);
        }
    }
}
