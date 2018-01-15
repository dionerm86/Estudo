using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.RelModel;
using System.Xml;
using System.IO;
using Glass.Data.Helper;
using Glass.Data.Model.Cte;
using System.Web;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class CTeDAO : Glass.Pool.PoolableObject<CTeDAO>
    {
        private CTeDAO() { }

        internal static XmlElement GetXmlInfCTe(HttpContext context, XmlDocument xmlCTe)
        {
            XmlElement cte = xmlCTe["CTe"] != null ? xmlCTe["CTe"] : xmlCTe["cteProc"]["CTe"];
            return cte["infCte"];
        }

        /// <summary>
        /// Busca XML do CTe passado para imprimir o DACTE
        /// </summary>
        /// <param name="chaveAcesso"></param>
        /// <returns></returns>
        public static CTe GetForDacte(string chaveAcesso)
        {
            return GetForDacte(HttpContext.Current, chaveAcesso);
        }

        /// <summary>
        /// Busca XML do CTe passado para imprimir o DACTE
        /// </summary>
        /// <param name="context"></param>
        /// <param name="chaveAcesso"></param>
        /// <returns></returns>
        internal static CTe GetForDacte(HttpContext context, string chaveAcesso)
        {
            CTe cte = new CTe();
            Glass.Data.Model.Cte.ConhecimentoTransporte _cte = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.GetByChaveAcesso(chaveAcesso);

            // Verifica se CTe existe
            if (!File.Exists(Utils.GetCteXmlPathInternal(context) + chaveAcesso + "-cte.xml"))
                throw new Exception("Arquivo do CT-e não encontrado.");

            // Busca arquivo XML do CTe
            XmlDocument xmlCTe = new XmlDocument();
            xmlCTe.Load(Utils.GetCteXmlPathInternal(context) + chaveAcesso + "-cte.xml");
            XmlElement xmlInfCTe = GetXmlInfCTe(context, xmlCTe);

            #region Busca dados do XML do CT-e

            // Identificação do Emitente
            cte.RazaoSocialEmit = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfCTe, "emit", "xNome"));
            cte.EnderecoEmit = Formatacoes.RestauraStringDocFiscal(GetEnderecoEmit(xmlInfCTe));
            cte.CnpjCpfEmitente = Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "emit", "CNPJ"));
            cte.InscEstEmitente = GetNodeValue(xmlInfCTe, "emit", "IE");

            //Modal
            cte.Modal = "RODOVIÁRIO";

            //Modelo
            cte.Modelo = GetNodeValue(xmlInfCTe, "ide", "mod");

            //Série
            cte.SerieCte = GetNodeValue(xmlInfCTe, "ide", "serie");

            //Número
            cte.NumeroCte = Formatacoes.MascaraNroCTe(GetNodeValue(xmlInfCTe, "ide", "nCT"));

            //Tipo Ambiente
            cte.TipoAmbiente = Glass.Conversoes.StrParaInt(GetNodeValue(xmlInfCTe, "ide", "tpAmb"));
            
            //Data e hora de emissão            
            cte.DHEmissao = GetNodeValue(xmlInfCTe, "ide", "dhEmi");

            cte.FormaEmissao = GetNodeValue(xmlInfCTe, "ide", "tpEmis");

            //Inscrição Suframa Destinatário
            cte.InscSuframa = GetNodeValue(xmlInfCTe, "dest", "ISUF");

            //Chave de acesso
            cte.ChaveAcesso = Formatacoes.MascaraChaveAcessoCTe(chaveAcesso);            

            //Protocolo de autorização
            if (xmlCTe["CTe"] != null && xmlCTe["CTe"]["infProt"] != null) // Sql montado no sistema
                cte.ProtocoloAutorizacao = GetNodeValue(xmlCTe["CTe"], "infProt", "nProt") + " " + GetNodeValue(xmlCTe["CTe"], "infProt", "dhRecbto").Replace("T", " ");
            else if (xmlCTe["cteProc"] != null && xmlCTe["cteProc"]["protCte"] != null) // Sql montado na receita
                cte.ProtocoloAutorizacao = GetNodeValue(xmlCTe["cteProc"]["protCte"], "infProt", "nProt") + " " + GetNodeValue(xmlCTe["cteProc"]["protCte"], "infProt", "dhRecbto").Replace("T", " ");

            //Tipo Cte
            var tipoCte = GetNodeValue(xmlInfCTe, "ide", "tpCTe");
            cte.TipoCte = tipoCte == "0" ? "Normal" : tipoCte == "1" ? "Complemento Valores" : tipoCte == "2" ? "Anulação Valores" : "Substituto" ;

            //Tipo Servico
            var tipoServico = GetNodeValue(xmlInfCTe, "ide", "tpServ");
            cte.TipoServico = tipoServico == "0" ? "Normal" : tipoServico == "1" ? "Subcontratação" : tipoServico == "2" ? "Redespacho" : "Redespacho Intermediário" ;
            
            //CFOP - natureza operação
            cte.NatOperacao = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfCTe, "ide", "natOp"));

            var codInternoCfop = GetNodeValue(xmlInfCTe, "ide", "CFOP");

            var cfop = new GDA.Sql.Query("CodInterno=?codInterno")
                .Add("?codInterno", codInternoCfop)
                .First<Glass.Data.Model.Cfop>();

            //Origem prestação
            cte.OrigemPrestacao = GetNodeValue(xmlInfCTe, "ide", "UFIni") + "-" + GetNodeValue(xmlInfCTe, "ide", "xMunIni");

            //Destino prestação
            cte.DestinoPrestacao = GetNodeValue(xmlInfCTe, "ide", "UFFim") + "-" + GetNodeValue(xmlInfCTe, "ide", "xMunFim");

            //Campo de uso livre do contribuinte 
            cte.InformacoesAdicionais = GetNodeValue(xmlInfCTe, "compl", "xObs") + "\n" + cfop.Obs;
            
            //Remetente
            cte.Remetente = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfCTe, "rem", "xNome"));
            cte.EnderecoRem = GetEnderecoRem(xmlInfCTe);
            cte.MunicipioRem = GetNodeValue(xmlInfCTe, "rem/enderReme", "xMun");
            cte.CepRem = GetNodeValue(xmlInfCTe, "rem/enderReme", "CEP");
            cte.CnpjCpfRem = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "rem", "CPF")) ? Formatacoes.MascaraCpf(GetNodeValue(xmlInfCTe, "rem", "CPF"))
                : Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "rem", "CNPJ"));
            cte.InscEstRem = GetNodeValue(xmlInfCTe, "rem", "IE");
            cte.UFRem = GetNodeValue(xmlInfCTe, "rem/enderReme", "UF");
            cte.PaisRem = GetNodeValue(xmlInfCTe, "rem/enderReme", "xPais");
            cte.FoneRem = GetNodeValue(xmlInfCTe, "rem", "fone");

            //Destinatário
            cte.Destinatario = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfCTe, "dest", "xNome"));
            cte.EnderecoDest = GetEnderecoDest(xmlInfCTe);
            cte.MunicipioDest = GetNodeValue(xmlInfCTe, "dest/enderDest", "xMun");
            cte.CepDest = GetNodeValue(xmlInfCTe, "dest/enderDest", "CEP");
            cte.CnpjCpfDest = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "dest", "CPF")) ? Formatacoes.MascaraCpf(GetNodeValue(xmlInfCTe, "dest", "CPF"))
                : Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "dest", "CNPJ"));
            cte.InscEstDest = GetNodeValue(xmlInfCTe, "dest", "IE");
            cte.UFDest = GetNodeValue(xmlInfCTe, "dest/enderDest", "UF");
            cte.PaisDest = GetNodeValue(xmlInfCTe, "dest/enderDest", "xPais");
            cte.FoneDest = GetNodeValue(xmlInfCTe, "dest", "fone");

            //Expedidor
            cte.Expedidor = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfCTe, "exped", "xNome"));
            cte.EnderecoExped = GetEnderecoExped(xmlInfCTe);
            cte.MunicipioExpd = GetNodeValue(xmlInfCTe, "exped/enderExped", "xMun");
            cte.CepExped = GetNodeValue(xmlInfCTe, "exped/enderExped", "CEP");
            cte.CnpjCpfExped = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "exped", "CPF")) ? Formatacoes.MascaraCpf(GetNodeValue(xmlInfCTe, "exped", "CPF"))
                : Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "exped", "CNPJ"));
            cte.InscEstExped = GetNodeValue(xmlInfCTe, "exped", "IE");
            cte.UFExped = GetNodeValue(xmlInfCTe, "exped/enderExped", "UF");
            cte.PaisExped = GetNodeValue(xmlInfCTe, "exped/enderExped", "xPais");
            cte.FoneExped = GetNodeValue(xmlInfCTe, "exped", "fone");

            //Recebedor
            cte.Recebedor = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfCTe, "receb", "xNome"));
            cte.EnderecoReceb = GetEnderecoReceb(xmlInfCTe);
            cte.MunicipioReceb = GetNodeValue(xmlInfCTe, "receb/enderReceb", "xMun");
            cte.CepReceb = GetNodeValue(xmlInfCTe, "receb/enderReceb", "CEP");
            cte.CnpjCpfReceb = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "receb", "CPF")) ? Formatacoes.MascaraCpf(GetNodeValue(xmlInfCTe, "receb", "CPF"))
                : Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "receb", "CNPJ"));
            cte.InscEstReceb = GetNodeValue(xmlInfCTe, "receb", "IE");
            cte.UFReceb = GetNodeValue(xmlInfCTe, "receb/enderReceb", "UF");
            cte.PaisReceb = GetNodeValue(xmlInfCTe, "receb/enderReceb", "xPais");
            cte.FoneReceb = GetNodeValue(xmlInfCTe, "receb", "fone");

            //Tomador            

            var tipoTomador = GetNodeValue(xmlInfCTe, "ide/toma3", "toma");
            cte.TipoTomador = tipoTomador;
            //se tomador for remetente (0)
            if (tipoTomador == "0")
            {
                cte.Tomador = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfCTe, "rem", "xNome"));
                cte.EnderecoToma = GetEnderecoRem(xmlInfCTe);
                cte.MunicipioToma = GetNodeValue(xmlInfCTe, "rem/enderReme", "xMun");
                cte.CepToma = GetNodeValue(xmlInfCTe, "rem/enderReme", "CEP");
                cte.CnpjCpfToma = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "rem", "CPF")) ? Formatacoes.MascaraCpf(GetNodeValue(xmlInfCTe, "rem", "CPF"))
                    : Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "rem", "CNPJ"));
                cte.InscEstToma = GetNodeValue(xmlInfCTe, "rem", "IE");
                cte.UFToma = GetNodeValue(xmlInfCTe, "rem/enderReme", "UF");
                cte.PaisToma = GetNodeValue(xmlInfCTe, "rem/enderReme", "xPais");
                cte.FoneToma = GetNodeValue(xmlInfCTe, "rem", "fone");
            }
            //se tomador for expedidor (1)
            else if (tipoTomador == "1")
            {
                cte.Tomador = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfCTe, "exped", "xNome"));
                cte.EnderecoToma = GetEnderecoExped(xmlInfCTe);
                cte.MunicipioToma = GetNodeValue(xmlInfCTe, "exped/enderExped", "xMun");
                cte.CepToma = GetNodeValue(xmlInfCTe, "exped/enderExped", "CEP");
                cte.CnpjCpfToma = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "exped", "CPF")) ? Formatacoes.MascaraCpf(GetNodeValue(xmlInfCTe, "exped", "CPF"))
                    : Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "exped", "CNPJ"));
                cte.InscEstToma = GetNodeValue(xmlInfCTe, "exped", "IE");
                cte.UFToma = GetNodeValue(xmlInfCTe, "exped/enderExped", "UF");
                cte.PaisToma = GetNodeValue(xmlInfCTe, "exped/enderExped", "xPais");
                cte.FoneToma = GetNodeValue(xmlInfCTe, "exped", "fone");
            }
            //se tomador for recebedor (2)
            else if (tipoTomador == "2")
            {
                cte.Tomador = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfCTe, "receb", "xNome"));
                cte.EnderecoToma = GetEnderecoReceb(xmlInfCTe);
                cte.MunicipioToma = GetNodeValue(xmlInfCTe, "receb/enderReceb", "xMun");
                cte.CepToma = GetNodeValue(xmlInfCTe, "receb/enderReceb", "CEP");
                cte.CnpjCpfToma = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "receb", "CPF")) ? Formatacoes.MascaraCpf(GetNodeValue(xmlInfCTe, "receb", "CPF"))
                    : Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "receb", "CNPJ"));
                cte.InscEstToma = GetNodeValue(xmlInfCTe, "receb", "IE");
                cte.UFToma = GetNodeValue(xmlInfCTe, "receb/enderReceb", "UF");
                cte.PaisToma = GetNodeValue(xmlInfCTe, "receb/enderReceb", "xPais");
                cte.FoneToma = GetNodeValue(xmlInfCTe, "receb", "fone");
            }
            //se tomador for destinatário (3)
            else if (tipoTomador == "3")
            {
                cte.Tomador = Formatacoes.RestauraStringDocFiscal(GetNodeValue(xmlInfCTe, "dest", "xNome"));
                cte.EnderecoToma = GetEnderecoDest(xmlInfCTe);
                cte.MunicipioToma = GetNodeValue(xmlInfCTe, "dest/enderDest", "xMun");
                cte.CepToma = GetNodeValue(xmlInfCTe, "dest/enderDest", "CEP");
                cte.CnpjCpfToma = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "dest", "CPF")) ? Formatacoes.MascaraCpf(GetNodeValue(xmlInfCTe, "dest", "CPF"))
                    : Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "dest", "CNPJ"));
                cte.InscEstToma = GetNodeValue(xmlInfCTe, "dest", "IE");
                cte.UFToma = GetNodeValue(xmlInfCTe, "dest/enderDest", "UF");
                cte.PaisToma = GetNodeValue(xmlInfCTe, "dest/enderDest", "xPais");
                cte.FoneToma = GetNodeValue(xmlInfCTe, "dest", "fone");
            }
           
            // Produto Predominante
            cte.ProdutoPredominante = GetNodeValue(xmlInfCTe, "infCTeNorm/infCarga", "proPred");

            // Outras caracteristicas da carga
            if(GetNodeValue(xmlInfCTe, "infCTeNorm/infCarga", "xOutCat") != null)
            cte.OutCarctCarga = GetNodeValue(xmlInfCTe, "infCTeNorm/infCarga", "xOutCat");

            cte.ValorTotalMercadoria = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "infCTeNorm/infCarga", "vCarga"), 2);

            // Produto Predominante
            cte.PesoBruto = GetNodeValue(xmlInfCTe, "infCTeNorm/infCarga", "proPred");

            //infCarga
            //var infCarga = xmlInfCTe["infCTeNorm"].GetElementsByTagName("infCarga");

            XmlNodeList xmlListInfCarga = xmlInfCTe["infCTeNorm"].GetElementsByTagName("infQ");
            if (xmlListInfCarga.Count > 0)
            {
                var listaInfCarga = new List<InfoCargaCte>();

                foreach (XmlElement xmlInfo in xmlListInfCarga)
                {
                    var infoCarga = new InfoCargaCte
                    {
                        Quantidade = Glass.Conversoes.StrParaFloat(xmlInfo.GetElementsByTagName("qCarga")[0].InnerText),
                        TipoMedida = xmlInfo.GetElementsByTagName("tpMed")[0].InnerText,
                        TipoUnidade = Glass.Conversoes.StrParaInt(xmlInfo.GetElementsByTagName("cUnid")[0].InnerText)
                    };
                    
                    listaInfCarga.Add(infoCarga);
                }
                cte.ListaInfoCargaCte = listaInfCarga;                
            }
                       
            //Seguradoras            
            var respSeg = GetNodeValue(xmlInfCTe, "infCTeNorm/seg", "respSeg");
            cte.ResponsavelSeguro = respSeg == "0" ? "Remetente" : respSeg == "1" ? "Expedidor" : respSeg == "2" ? "Recebedor"
                : respSeg == "3" ? "Destinatário" : respSeg == "4" ? "Emitente" : "Tomador";
            cte.NomeSeguradora = GetNodeValue(xmlInfCTe, "infCTeNorm/seg", "xSeg");
            cte.NumApolice = GetNodeValue(xmlInfCTe, "infCTeNorm/seg", "nApol");
            cte.NumAverbacao = GetNodeValue(xmlInfCTe, "infCTeNorm/seg", "nAver");            
                    
            //Componentes valor
            XmlNodeList xmlListaComponenteValor = xmlInfCTe["vPrest"].GetElementsByTagName("Comp");
            if (xmlListaComponenteValor.Count > 0)
            {
                var listaComponentes = new List<ComponenteValorCte>();

                foreach (XmlElement xmlComp in xmlListaComponenteValor)
                {
                    var componente = new ComponenteValorCte
                    {
                        NomeComponente = xmlComp.GetElementsByTagName("xNome")[0].InnerText,
                        ValorComponente = Glass.Conversoes.StrParaDecimal(xmlComp.GetElementsByTagName("vComp")[0].InnerText)
                    };
                    listaComponentes.Add(componente);                    
                }
                cte.ListaComponentes = listaComponentes;
            }

            cte.ValorTotalServicoComponente = Glass.Conversoes.StrParaDecimal(GetNodeValue(xmlInfCTe, "vPrest", "vTPrest"));
            cte.ValorReceberComponente = Glass.Conversoes.StrParaDecimal(GetNodeValue(xmlInfCTe, "vPrest", "vRec"));

            //Informações relativas ao imposto
            if(!string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "imp/ICMS", "ICMS00")))
            {
                cte.SubstituicaoTributaria = "00 - tributação normal ICMS";
                cte.BaseCalculo = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS00", "vBC"), 2);
                cte.AliquotaIcms = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS00", "pICMS"), 2);
                cte.ValorIcms = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS00", "vICMS"), 2);
            }
            else if (!string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "imp/ICMS", "ICMS20")))
            {
                cte.SubstituicaoTributaria = "20 - tributação com BC reduzida do ICMS";
                cte.BaseCalculo = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS20", "vBC"), 2);
                cte.AliquotaIcms = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS20", "pICMS"), 2);
                cte.ValorIcms = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS20", "vICMS"), 2);
                cte.ReducaoBaseCalculo = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS20", "pRedBC"), 2);
            }            
            else if (!string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "imp/ICMS", "ICMS60")))
            {
                cte.SubstituicaoTributaria = "60 - ICMS cobrado anteriormente por substituição tributária";
                cte.IcmsST = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS60", "vICMSSTRet"), 2);
            }
            else if (!string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "imp/ICMS", "ICMS90")))
            {
                cte.SubstituicaoTributaria = "90 - ICMS outros ";
                cte.BaseCalculo = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS90", "vBC"), 2);
                cte.AliquotaIcms = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS90", "pICMS"), 2);
                cte.ValorIcms = Formatacoes.FormataValorDecimal(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS90", "vICMS"), 2);
            }
            else if (!string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "imp/ICMS", "ICMS45")))
            {
                switch(GetNodeValue(xmlInfCTe, "imp/ICMS/ICMS45", "CST"))
                {
                    case "40":
                        cte.SubstituicaoTributaria = "40 - ICMS isenção";
                        break;
                    case "41":
                        cte.SubstituicaoTributaria = "41 - ICMS não tributada";
                        break;
                    case "51":
                        cte.SubstituicaoTributaria = "51 - ICMS diferido";
                        break;
                }
            }

            if (xmlInfCTe["infCTeNorm"]["infDoc"] != null)
            {
                //Documentos Originários
                if (xmlInfCTe["infCTeNorm"]["infDoc"]["infNF"] != null)
                {
                    var xmlListaDocumentosOriginarios = xmlInfCTe["infCTeNorm"]["infDoc"].GetElementsByTagName("infNF");

                    if (xmlListaDocumentosOriginarios.Count > 0)
                    {
                        var cpfCnpjDocsOriginarios = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "rem", "CPF")) ?
                            Formatacoes.MascaraCpf(GetNodeValue(xmlInfCTe, "rem", "CPF")) : Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "rem", "CNPJ"));

                        var listaDocsOriginarios = new List<NotaFiscalCte>();
                        int cont = 0;

                        foreach (XmlElement xmlDocOriginario in xmlListaDocumentosOriginarios)
                        {
                            var docsOriginarios = new NotaFiscalCte
                            {
                                TipoDoc = "NF",
                                DocEmitenteNf = cpfCnpjDocsOriginarios,
                                Serie = xmlDocOriginario.GetElementsByTagName("serie")[cont].InnerText,
                                NumeroDoc = xmlDocOriginario.GetElementsByTagName("nDoc")[cont].InnerText
                            };

                            cont++;

                            listaDocsOriginarios.Add(docsOriginarios);
                        }
                        cte.ListaDocumentosOriginarios = listaDocsOriginarios;
                    }
                }

                //Documentos Originários
                else if (xmlInfCTe["infCTeNorm"]["infDoc"]["infNFe"] != null &&
                    xmlInfCTe["infCTeNorm"]["infDoc"]["infNFe"]["chave"] != null)
                {
                    var xmlListaDocumentosOriginariosNfe = xmlInfCTe["infCTeNorm"]["infDoc"].GetElementsByTagName("infNFe");
                    if (xmlListaDocumentosOriginariosNfe.Count > 0)
                    {
                        var cpfCnpjDocsOriginarios = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "rem", "CPF")) ?
                            Formatacoes.MascaraCpf(GetNodeValue(xmlInfCTe, "rem", "CPF")) : Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "rem", "CNPJ"));

                        var listaDocsOriginarios = new List<NotaFiscalCte>();

                        foreach (XmlElement xmlDocOriginarioNfe in xmlListaDocumentosOriginariosNfe)
                        {
                            // var nf = Glass.Data.DAL.NotaFiscalDAO.Instance.GetByChaveAcesso(xmlDocOriginarioNfe.InnerText);

                            var docsOriginarios = new NotaFiscalCte
                            {
                                TipoDoc = "NFe",
                                DocEmitenteNf = xmlDocOriginarioNfe.InnerText
                                //DocEmitenteNf = nf.CnpjEmitente,
                                //Serie = nf.Serie,
                                //NumeroDoc = nf.NumeroNFe.ToString()
                            };

                            listaDocsOriginarios.Add(docsOriginarios);
                        }
                        cte.ListaDocumentosOriginarios = listaDocsOriginarios;
                    }
                }
            }
            //else
            //{
            //    XmlNodeList xmlListaDocumentosOriginarios = xmlInfCTe["rem"].GetElementsByTagName("infNFe");
            //    if (xmlListaDocumentosOriginarios.Count > 0)
            //    {
            //        var cpfCnpjDocsOriginarios = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "rem", "CPF")) ?
            //            Formatacoes.MascaraCpf(GetNodeValue(xmlInfCTe, "rem", "CPF")) : Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "rem", "CNPJ"));

            //        var listaDocsOriginarios = new List<NotaFiscalCte>();

            //        foreach (XmlElement xmlDocOriginario in xmlListaDocumentosOriginarios)
            //        {
            //            var docsOriginarios = new NotaFiscalCte
            //            {
            //                TipoDoc = xmlDocOriginario.GetElementsByTagName("mod")[0].InnerText,
            //                DocEmitenteNf = cpfCnpjDocsOriginarios,
            //                SerieNumDoc = xmlDocOriginario.GetElementsByTagName("serie")[0].InnerText
            //            };
            //            listaDocsOriginarios.Add(docsOriginarios);
            //        }
            //        cte.ListaDocumentosOriginarios = listaDocsOriginarios;
            //    }
            //}


            //Informações específicas do modal rodoviário lotação

            /*cte.TipoVeiculo = GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo/veic", "tpVeic") == "0" ? "Tração" : "Reboque";
            cte.Placa = GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo/veic", "placa");
            cte.UFVeiculo = GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo/veic", "UF");*/

            var veiculosCte = DAL.CTe.VeiculoCteDAO.Instance.GetVeiculosCteByIdCte(_cte.IdCte);

            /* Chamado 44905. */
            if (veiculosCte != null)
            {
                var veiculosTipo = new List<string>();
                var veiculosPlaca = new List<string>();
                var veiculosUf = new List<string>();
                var proprietariosRntrc = new List<string>();
                var participantes = DAL.CTe.ParticipanteCteDAO.Instance.GetParticipanteByIdCte(_cte.IdCte);
                var participanteEmitente = participantes.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Emitente).First();
                var loja = DAL.LojaDAO.Instance.GetElement(participanteEmitente.IdLoja.Value);

                foreach (var veiculoCte in veiculosCte)
                {
                    var listaProprietarios = DAL.CTe.ProprietarioVeiculo_VeiculoDAO.Instance.GetList(veiculoCte.Placa, 0);
                    var proprietario = new ProprietarioVeiculo();

                    foreach (var prop in listaProprietarios)
                    {
                        if (Formatacoes.LimpaCpfCnpj(DAL.CTe.ProprietarioVeiculoDAO.Instance.GetElement(prop.IdPropVeic).Cnpj) == Formatacoes.LimpaCpfCnpj(loja.Cnpj))
                        {
                            var rntrc = DAL.CTe.ProprietarioVeiculoDAO.Instance.ObtemValorCampo<string>("RNTRC", "IdPropVeic=" + prop.IdPropVeic);

                            if (!string.IsNullOrEmpty(rntrc))
                                proprietariosRntrc.Add(rntrc);

                            break;
                        }
                    }

                    var veiculo = DAL.VeiculoDAO.Instance.GetElement(veiculoCte.Placa);

                    veiculosTipo.Add(veiculo.TipoVeiculo == 0 ? "Tração" : "Reboque");
                    veiculosPlaca.Add(veiculoCte.Placa);
                    veiculosUf.Add(veiculo.UfLicenc);
                }
                
                cte.TipoVeiculo = string.Join("\n", veiculosTipo);
                cte.Placa = string.Join("\n", veiculosPlaca);
                cte.UFVeiculo = string.Join("\n", veiculosUf);
                cte.RNTRCProprietario = string.Join("\n", proprietariosRntrc);
            }
            else
            {
                cte.TipoVeiculo = GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo/veic", "tpVeic") == "0" ? "Tração" : "Reboque";
                cte.Placa = GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo/veic", "placa");
                cte.UFVeiculo = GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo/veic", "UF");
                cte.RNTRCProprietario = GetNodeValue(xmlInfCTe, "prop", "RNTRC");
            }

            cte.RespValePedCnpj = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "valePed", "CNPJPg")) ?
                Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "valePed", "CNPJPg")) : "";
            cte.FornValePedagioCnpj = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "valePed", "CNPJForn")) ?
                Formatacoes.MascaraCnpj(GetNodeValue(xmlInfCTe, "valePed", "CNPJForn")) : "";
            cte.NumeroComprovante = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "valePed", "nCompra")) ? GetNodeValue(xmlInfCTe, "valePed", "nCompra") : "";

            cte.RNTRCRodo = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo", "RNTRC")) ? GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo", "RNTRC") : "";
            cte.Lotacao = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo", "lota")) ? GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo", "lota") : "";
            cte.CIOT = !string.IsNullOrEmpty(GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo", "CIOT")) ? GetNodeValue(xmlInfCTe, "infCTeNorm/infModal/rodo", "CIOT") : "";

            XmlNodeList xmlListaLacre = xmlInfCTe["infCTeNorm"].GetElementsByTagName("lacRodo");
            if (xmlListaLacre.Count > 0)
            {                
                var listaNumLacre = new List<LacreCteRod>();

                foreach (XmlElement xmlLacre in xmlListaLacre)
                {                    
                    var lacre = new LacreCteRod
                    {
                        NumeroLacre = xmlLacre.GetElementsByTagName("nLacre")[0].InnerText
                    };
                    listaNumLacre.Add(lacre);
                }
                cte.ListaNumeroLacre = listaNumLacre;
            }            
            
            #endregion

            return cte;
        }

        #region Endereço completo do emitente

        /// <summary>
        /// Retorna o endereço completo do emitente em apenas uma string
        /// </summary>
        /// <param name="xmlInfNfe"></param>
        /// <returns></returns>
        private static string GetEnderecoEmit(XmlElement xmlInfCte)
        {
            StringBuilder enderCompleto = new StringBuilder();
            enderCompleto.Append(GetNodeValue(xmlInfCte, "emit/enderEmit", "xLgr") + ", ");
            enderCompleto.Append(GetNodeValue(xmlInfCte, "emit/enderEmit", "nro"));

            if (!String.IsNullOrEmpty(GetNodeValue(xmlInfCte, "emit/enderEmit", "xCpl")))
                enderCompleto.Append(" - " + GetNodeValue(xmlInfCte, "emit/enderEmit", "xCpl"));

            enderCompleto.Append(" - " + GetNodeValue(xmlInfCte, "emit/enderEmit", "xBairro") + ", ");
            enderCompleto.Append(GetNodeValue(xmlInfCte, "emit/enderEmit", "xMun") + ", ");
            enderCompleto.Append(GetNodeValue(xmlInfCte, "emit/enderEmit", "UF"));

            if (!String.IsNullOrEmpty(GetNodeValue(xmlInfCte, "emit/enderEmit", "CEP")))
                enderCompleto.Append(" - CEP: " + GetNodeValue(xmlInfCte, "emit/enderEmit", "CEP"));

            if (!String.IsNullOrEmpty(GetNodeValue(xmlInfCte, "emit/enderEmit", "fone")))
                enderCompleto.Append(" - Fone: " + TrataTelefoneCte(GetNodeValue(xmlInfCte, "emit/enderEmit", "fone")));

            return Formatacoes.RestauraStringDocFiscal(enderCompleto.ToString());
        }

        /// <summary>
        /// Retorna o endereço do remetente em apenas uma string
        /// </summary>
        /// <param name="xmlInfCte"></param>
        /// <returns></returns>
        private static string GetEnderecoRem(XmlElement xmlInfCte)
        {
            StringBuilder enderRemetente = new StringBuilder();
            enderRemetente.Append(GetNodeValue(xmlInfCte, "rem/enderReme", "xLgr") + ", ");
            enderRemetente.Append(GetNodeValue(xmlInfCte, "rem/enderReme", "nro"));

            if (!String.IsNullOrEmpty(GetNodeValue(xmlInfCte, "rem/enderReme", "xCpl")))
                enderRemetente.Append(" - " + GetNodeValue(xmlInfCte, "rem/enderReme", "xCpl"));

            enderRemetente.Append(" - " + GetNodeValue(xmlInfCte, "rem/enderReme", "xBairro"));            

            return Formatacoes.RestauraStringDocFiscal(enderRemetente.ToString());
        }

        /// <summary>
        /// Retorna o endereço do destinatário em apenas uma string
        /// </summary>
        /// <param name="xmlInfCte"></param>
        /// <returns></returns>
        private static string GetEnderecoDest(XmlElement xmlInfCte)
        {
            StringBuilder enderDestinatario = new StringBuilder();
            enderDestinatario.Append(GetNodeValue(xmlInfCte, "dest/enderDest", "xLgr") + ", ");
            enderDestinatario.Append(GetNodeValue(xmlInfCte, "dest/enderDest", "nro"));

            if (!String.IsNullOrEmpty(GetNodeValue(xmlInfCte, "dest/enderDest", "xCpl")))
                enderDestinatario.Append(" - " + GetNodeValue(xmlInfCte, "dest/enderDest", "xCpl"));

            enderDestinatario.Append(" - " + GetNodeValue(xmlInfCte, "dest/enderDest", "xBairro"));

            return Formatacoes.RestauraStringDocFiscal(enderDestinatario.ToString());
        }

        /// <summary>
        /// Retorna o endereço do expedidor em apenas uma string
        /// </summary>
        /// <param name="xmlInfCte"></param>
        /// <returns></returns>
        private static string GetEnderecoExped(XmlElement xmlInfCte)
        {
            StringBuilder enderExpedidor = new StringBuilder();
            enderExpedidor.Append(GetNodeValue(xmlInfCte, "exped/enderExped", "xLgr") + ", ");
            enderExpedidor.Append(GetNodeValue(xmlInfCte, "exped/enderExped", "nro"));

            if (!String.IsNullOrEmpty(GetNodeValue(xmlInfCte, "exped/enderExped", "xCpl")))
                enderExpedidor.Append(" - " + GetNodeValue(xmlInfCte, "exped/enderExped", "xCpl"));

            enderExpedidor.Append(" - " + GetNodeValue(xmlInfCte, "exped/enderExped", "xBairro"));

            return Formatacoes.RestauraStringDocFiscal(enderExpedidor.ToString());
        }

        /// <summary>
        /// Retorna o endereço do recebedor em apenas uma string
        /// </summary>
        /// <param name="xmlInfCte"></param>
        /// <returns></returns>
        private static string GetEnderecoReceb(XmlElement xmlInfCte)
        {
            StringBuilder enderExpedidor = new StringBuilder();
            enderExpedidor.Append(GetNodeValue(xmlInfCte, "receb/enderReceb", "xLgr") + ", ");
            enderExpedidor.Append(GetNodeValue(xmlInfCte, "receb/enderReceb", "nro"));

            if (!String.IsNullOrEmpty(GetNodeValue(xmlInfCte, "receb/enderReceb", "xCpl")))
                enderExpedidor.Append(" - " + GetNodeValue(xmlInfCte, "receb/enderReceb", "xCpl"));

            enderExpedidor.Append(" - " + GetNodeValue(xmlInfCte, "receb/enderReceb", "xBairro"));

            return Formatacoes.RestauraStringDocFiscal(enderExpedidor.ToString());
        }

        /// <summary>
        /// Recebe um telefone no padrão do CTe para ser tratado, 10 dígitos sem símbolos (3133334444)
        /// </summary>
        /// <param name="telefone"></param>
        /// <returns></returns>
        private static string TrataTelefoneCte(string telefone)
        {
            return "(" + telefone.Substring(0, 2) + ") " + telefone.Substring(2, 4) + "-" + telefone.Substring(6);
        }

        #endregion

        #region Retorna o valor de um nodo do XML passado

        /// <summary>
        /// Retorna o valor de um nodo do XML passado
        /// </summary>
        /// <param name="xmlInfCte">XmlElement a ser buscado o valor do nodo</param>
        /// <param name="parentsNodeName">Nome dos nodos pais do nodo desejado, quando houver mais de um
        /// nodo pai, pode ser informado da seguinte forma: "nodoPai1/nodoPai2/nodoPaiX"</param>
        /// <param name="nodeName">Nodo do xml que se deseja extrair conteúdo</param>
        /// <returns></returns>
        private static string GetNodeValue(XmlElement xmlInfCte, string parentsNodeName, string nodeName)
        {
            if (xmlInfCte == null)
                return String.Empty;

            string[] parentsNodes = parentsNodeName.Split('/');

            // Verifica se xml possui nodo pai passado, se possuir, vai entrando no XML, 
            // deixando de lado níveis acima que não interessam
            foreach (string pNode in parentsNodes)
            {
                if (xmlInfCte[pNode] != null)
                    xmlInfCte = xmlInfCte[pNode];
                else
                    return String.Empty;
            }

            if (xmlInfCte[nodeName] != null)
                return xmlInfCte[nodeName].InnerXml;
            else
                return String.Empty;
        }

        #endregion

        #region Utilizado pelo ReportViewer

        public static CTe[] GetAll()
        {
            return new CTe[0];
        }

        #endregion
    }
}

