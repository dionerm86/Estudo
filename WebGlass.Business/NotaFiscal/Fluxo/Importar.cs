using System;
using System.Xml;
using Glass.Data.DAL;
using WebGlass.Business.NotaFiscal.Entidade;
using System.Globalization;
using Glass.Data.Model;
using Glass.Configuracoes;
using Glass.Data.NFeUtils;

namespace WebGlass.Business.NotaFiscal.Fluxo
{
    public sealed class Importar : BaseFluxo<Importar>
    {
        private Importar() { }

        public DadosVerificarNFe VerificarNFe(XmlDocument nfeFile, string fileName)
        {
            XmlElement nfeRoot = null;

            if (nfeFile["nfeProc"] != null && nfeFile["nfeProc"]["NFe"] != null)
                nfeRoot = nfeFile["nfeProc"]["NFe"];
            else if (nfeFile["NFe"] != null)
                nfeRoot = nfeFile["NFe"];
            else if (nfeFile["enviNFe"] != null && nfeFile["enviNFe"]["NFe"] != null)
                nfeRoot = nfeFile["enviNFe"]["NFe"];
            else
                throw new Exception("A tag raiz é inválida");

            //Verifica se fornecedor tem CPF ou CNPJ cadastrado no sistema
            XmlElement nfeIde = nfeRoot["infNFe"]["emit"];
            string idFornecedor = null;

            if (nfeIde.GetElementsByTagName("CNPJ").Count == 1)
                idFornecedor = FornecedorDAO.Instance.GetFornecedorByCPFCNPJ(nfeIde["CNPJ"].InnerText);

            else if (nfeIde.GetElementsByTagName("CPF").Count == 1)
                idFornecedor = FornecedorDAO.Instance.GetFornecedorByCPFCNPJ(nfeIde["CPF"].InnerText);

            if (idFornecedor == null)
                throw new Exception("Fornecedor não cadastrado no sistema.");

            try
            {
                string destCNPJ = nfeRoot["infNFe"]["dest"]["CNPJ"].InnerText;
            }
            catch (NullReferenceException)
            {
                throw new Exception("NFE não é de entrada.");
            }

            //Variáveis para verificar se o xml se trata de uma NFe versão 3.10 válida
            XmlNode nfeInfNFe = nfeFile.GetElementsByTagName("infNFe")[0];
            string nfeVersao = string.Empty;
            string nfeID = string.Empty;

            foreach (XmlAttribute nfeAtt in nfeInfNFe.Attributes)
            {
                if (nfeAtt.Name == "versao")
                    nfeVersao = nfeAtt.Value;

                //Checa se o ID começa com 'NFe', de acordo com a especificação da receita
                if (nfeAtt.Name == "Id" && nfeAtt.Value.StartsWith("NFe"))
                    nfeID = nfeAtt.Value;
            }

            //Checa se o namespace, a versão e o ID estão corretos, de acordo com a especificação da Receita
            if (nfeInfNFe.NamespaceURI.Equals("http://www.portalfiscal.inf.br/nfe"))
            {
                if (nfeVersao.Equals("3.10") || nfeVersao.Equals("4.00"))
                {
                    if (!nfeID.Equals(string.Empty))
                    {
                        DadosVerificarNFe dados = new DadosVerificarNFe()
                        {
                            InfoNFe = "<br />Nota Fiscal Eletrônica" +
                                "<br />Arquivo: " + fileName +
                                "<br />Versao: " + nfeVersao +
                                "<br />ID: " + nfeID + "<br />",

                            ChaveAcesso = nfeID.Remove(0, 3)
                        };

                        return dados;
                    }
                    else
                    {
                        throw new Exception("NFe não possui chave identificadora.");
                    }
                }
                else
                {
                    throw new Exception("Versão da Nota Fiscal não suportada.<br/>O sistema suporta apenas notas fiscais de versão 2.00 ou 3.10");
                }
            }
            else
            {
                throw new Exception("Namespace da Nota Fiscal não confere.");
            }
        }

        public void ImportarNFe(XmlDocument loadedNFE, DadosImportarNFe dadosImportar)
        {
            // Controla a importação de PIS e Cofins na NFe
            const bool IMPORTAR_PIS_COFINS = true;

            var versao = string.Empty;

            XmlElement nfeRoot;
            try
            {
                nfeRoot = loadedNFE["nfeProc"]["NFe"];
                versao = loadedNFE["nfeProc"]["NFe"]["infNFe"] != null ? loadedNFE["nfeProc"]["NFe"]["infNFe"].GetAttribute("versao") : "";
            }
            catch
            {
                try
                {
                    nfeRoot = loadedNFE["NFe"];
                    versao = loadedNFE["NFe"]["infNFe"].GetAttribute("versao");
                }
                catch (Exception ex2)
                {
                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Erro na tag raíz da NF-e.", ex2));
                }
            }

            #region Validações da compra

            if (dadosImportar.IdCompra.GetValueOrDefault(0) > 0)
            {
                if (CompraDAO.Instance.ObtemSituacao(null, dadosImportar.IdCompra.Value) != (int)Glass.Data.Model.Compra.SituacaoEnum.Finalizada)
                    throw new Exception("A compra " + dadosImportar.IdCompra.Value + " não esta finalizada.");

                if (CompraNotaFiscalDAO.Instance.PossuiNFe(dadosImportar.IdCompra.Value))
                    throw new Exception("A compra " + dadosImportar.IdCompra.Value + " já possui uma nota fiscal.");
            }

            #endregion

            #region verifica se a NFE já existe no banco
            
            try
            {
                XmlElement nfeIde = nfeRoot["infNFe"]["emit"];
                string verifCnpjCpf = string.Empty;

                string verifNumNFE = nfeRoot["infNFe"]["ide"]["nNF"].InnerText;
                string verifSerie = nfeRoot["infNFe"]["ide"]["serie"].InnerText;                

                //Verifica no XML se o emitente usa CPF ou CNPJ
                if (nfeIde.GetElementsByTagName("CNPJ").Count == 1)
                    verifCnpjCpf = nfeIde["CNPJ"].InnerText;

                else if (nfeIde.GetElementsByTagName("CPF").Count == 1)
                    verifCnpjCpf = nfeIde["CPF"].InnerText;
                
                string verifFornec = FornecedorDAO.Instance.GetFornecedorByCPFCNPJ(verifCnpjCpf);

                if (verifFornec.Contains(","))
                    throw new Exception("Existe mais de um fornecedor com o CNPJ informado na nota fiscal.");

                if (NotaFiscalDAO.Instance.VerificarExistenciaNotaEntradaTerceiros(verifFornec, verifSerie, verifNumNFE))
                    throw new Exception("Nota Fiscal já existente no sistema.");
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao verificar se nota já existe no banco.", ex));
            }

            #endregion

            #region Inserção na tabela nota_fiscal

            var nfe = new Glass.Data.Model.NotaFiscal();

            try
            {
                nfe.ChaveAcesso = loadedNFE["nfeProc"]["protNFe"]["infProt"]["chNFe"].InnerText;
            }
            catch (NullReferenceException)
            {
                nfe.ChaveAcesso = dadosImportar.ChaveAcesso;
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao retornar chave de acesso", ex));
            }

            uint idNotaFiscalInserida;

            #region Dados com verificação
            //IDLOJA
            try
            {
                XmlElement nfeDest = nfeRoot["infNFe"]["dest"];
                string destCnpj = nfeDest["CNPJ"].InnerText;
                string destIE = nfeDest["IE"] != null ? nfeDest["IE"].InnerText : null;

                string idLoja = LojaDAO.Instance.GetLojaByCNPJIE(null, destCnpj, destIE, false);

                if (string.IsNullOrEmpty(idLoja))
                    throw new Exception();

                nfe.IdLoja = Glass.Conversoes.StrParaUint(idLoja);
            }
            catch (Exception)
            {
                throw new Exception("Destinatário da NFe não cadastrado no sistema, verifique o CNPJ e a inscrição estadual.");
            }

            //IDCIDADE
            try
            {
                XmlElement nfeIde = nfeRoot["infNFe"]["ide"];
                string estado = nfeIde["cMunFG"].InnerText.Substring(0, 2);
                string cidade = nfeIde["cMunFG"].InnerText.Substring(2, 5);

                string idCidade = CidadeDAO.Instance.GetCidadeByCodIBGE(cidade, estado);

                if (string.IsNullOrEmpty(idCidade))
                    throw new Exception();

                nfe.IdCidade = Glass.Conversoes.StrParaUint(idCidade);
            }
            catch
            {
                throw new Exception("Informação da cidade da Nota Fiscal incorreta.");
            }

            // Busca o emitente da nota (Fornecedor ou Cliente se for CFOP de devolução)
            try
            {
                XmlElement nfeEmit = nfeRoot["infNFe"]["emit"];
                string destCnpjCpf = string.Empty;

                if (nfeEmit.GetElementsByTagName("CNPJ").Count == 1)
                    destCnpjCpf = nfeEmit["CNPJ"].InnerText;

                else if (nfeEmit.GetElementsByTagName("CPF").Count == 1)
                    destCnpjCpf = nfeEmit["CPF"].InnerText;

                // Se for cfop de devolução, o emitente deve ser o cliente
                if (CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(dadosImportar.IdNaturezaOperacao)))
                {
                    nfe.IdCliente = (uint)ClienteDAO.Instance.GetByCpfCnpj(destCnpjCpf).IdCli;
                    if (nfe.IdCliente == 0)
                        throw new Exception("Cliente não encontrado.");
                }
                else
                {
                    nfe.IdFornec = Glass.Conversoes.StrParaUint(FornecedorDAO.Instance.GetFornecedorByCPFCNPJ(destCnpjCpf));
                    if (nfe.IdFornec == 0)
                        throw new Exception("Fornecedor não encontrado.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Emitente da NFe não cadastrado no sistema.", ex));
            }

            #endregion

            #region dados opcionais

            //IDTRANSPORTADOR
            try
            {
                XmlElement nfeTransp = nfeRoot["infNFe"]["transp"];
                if (nfeTransp["transporta"] != null) nfeTransp = nfeTransp["transporta"];

                string transpCnpj =
                    nfeTransp["CNPJ"].InnerText.Substring(0, 2) + "." +
                    nfeTransp["CNPJ"].InnerText.Substring(2, 3) + "." +
                    nfeTransp["CNPJ"].InnerText.Substring(5, 3) + "/" +
                    nfeTransp["CNPJ"].InnerText.Substring(8, 4) + "-" +
                    nfeTransp["CNPJ"].InnerText.Substring(12, 2);

                string idTransportador = TransportadorDAO.Instance.GetIDByCNPJ(transpCnpj);

                nfe.IdTransportador = Glass.Conversoes.StrParaUintNullable(idTransportador);
            }
            catch (NullReferenceException)
            {
                nfe.IdTransportador = null;
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar dados do transportador.", ex));
            }

            //MODALIDADE FRETE
            try
            {
                XmlElement nfeTransp = nfeRoot["infNFe"]["transp"];
                var modalidadeFrete = Glass.Conversoes.StrParaInt(nfeTransp["modFrete"].InnerText);

                nfe.ModalidadeFrete = (Glass.Data.Model.ModalidadeFrete)modalidadeFrete;
            }
            catch (NullReferenceException ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar modalidade do frete.", ex));
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar modalidade do frete.", ex));
            }

            ///VEICPLACA, VEICRNTC, VEICUF
            try
            {
                XmlElement nfeTransp = nfeRoot["infNFe"]["transp"]["veicTransp"];
                if (nfeTransp != null)
                {
                    nfe.VeicPlaca = nfeTransp["placa"].InnerText;
                    nfe.VeicRntc = nfeTransp["RNTC"].InnerText;
                    nfe.VeicUf = nfeTransp["UF"].InnerText;
                }
            }
            catch (NullReferenceException)
            {
                nfe.VeicPlaca = null;
                nfe.VeicRntc = null;
                nfe.VeicUf = null;
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar dados do veículo de transporte.", ex));
            }

            //QTDVOL, ESPECIE
            try
            {
                XmlElement nfeTransp = nfeRoot["infNFe"]["transp"]["vol"];
                string qtdVol = nfeTransp["qVol"].InnerText;
                nfe.Especie = nfeTransp["esp"].InnerText;

                nfe.QtdVol = Glass.Conversoes.StrParaInt(qtdVol);
            }
            catch (NullReferenceException)
            {
                nfe.QtdVol = 0;
                nfe.Especie = null;
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar quantidades tranportadas.", ex));
            }

            //NUMFATURA
            try
            {
                XmlElement nfeCobr = nfeRoot["infNFe"]["cobr"];
                nfe.NumFatura = nfeCobr["nFat"] != null ? nfeCobr["nFat"].InnerText : null;
            }
            catch (NullReferenceException)
            {
                nfe.NumFatura = null;
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar dados da fatura.", ex));
            }

            #endregion

            #region Dados sem verificação

            try
            {
                nfe.Situacao = 2;
                nfe.Modelo = nfeRoot["infNFe"]["ide"]["mod"].InnerText;
                nfe.Serie = nfeRoot["infNFe"]["ide"]["serie"].InnerText;
                nfe.TipoDocumento = 3;
                try { nfe.TipoImpressao = Glass.Conversoes.StrParaInt(nfeRoot["infNFe"]["ide"]["tpImp"].InnerText); }
                catch (Exception) { throw new Exception("Erro na propriedade TipoImpressao"); }

                try
                {
                    nfe.DataEmissao = new DateTime(Int32.Parse(nfeRoot["infNFe"]["ide"]["dhEmi"].InnerText.Substring(0, 4)),
                        Int32.Parse(nfeRoot["infNFe"]["ide"]["dhEmi"].InnerText.Substring(5, 2)),
                        Int32.Parse(nfeRoot["infNFe"]["ide"]["dhEmi"].InnerText.Substring(8, 2)),
                        Int32.Parse(nfeRoot["infNFe"]["ide"]["dhEmi"].InnerText.Substring(11, 2)),
                        Int32.Parse(nfeRoot["infNFe"]["ide"]["dhEmi"].InnerText.Substring(14, 2)),
                        Int32.Parse(nfeRoot["infNFe"]["ide"]["dhEmi"].InnerText.Substring(17, 2)));
                }
                catch (Exception) { throw new Exception("Erro na propriedade DataEmissao"); }

                try
                {
                    var dataSaidaEnt = nfeRoot["infNFe"]["ide"]["dhSaiEnt"] != null ? new DateTime(Int32.Parse(nfeRoot["infNFe"]["ide"]["dhSaiEnt"].InnerText.Substring(0, 4)),
                        Int32.Parse(nfeRoot["infNFe"]["ide"]["dhSaiEnt"].InnerText.Substring(5, 2)),
                        Int32.Parse(nfeRoot["infNFe"]["ide"]["dhSaiEnt"].InnerText.Substring(8, 2)),
                        Int32.Parse(nfeRoot["infNFe"]["ide"]["dhSaiEnt"].InnerText.Substring(11, 2)),
                        Int32.Parse(nfeRoot["infNFe"]["ide"]["dhSaiEnt"].InnerText.Substring(14, 2)),
                        Int32.Parse(nfeRoot["infNFe"]["ide"]["dhSaiEnt"].InnerText.Substring(17, 2))) : DateTime.Now;

                    nfe.DataSaidaEnt = dataSaidaEnt;
                }
                catch (NullReferenceException) { }

                //Números da forma de pagamento na model são "+1" em relação à especificação da receita
                try
                {
                    if (versao == "3.10")
                    {
                        int formaPagto = Glass.Conversoes.StrParaInt(nfeRoot["infNFe"]["ide"]["indPag"].InnerText);
                        formaPagto = formaPagto + 1; //+1 para se adequar à model
                        nfe.FormaPagto = formaPagto;
                    }
                    else
                    {
                        nfe.FormaPagto = (int)Glass.Data.Model.NotaFiscal.FormaPagtoEnum.Outros;
                    }
                }
                catch (Exception) { throw new Exception("Erro na propriedade FormaPagto"); }
                
                nfe.FormaEmissao = (int)Glass.Data.Model.NotaFiscal.TipoEmissao.Normal;

                try { nfe.FinalidadeEmissao = Glass.Conversoes.StrParaInt(nfeRoot["infNFe"]["ide"]["finNFe"].InnerText); }
                catch (Exception) { throw new Exception("Erro na propriedade TipoImpressao"); }

                try { nfe.BcIcms = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vBC"].InnerText, CultureInfo.InvariantCulture); }
                catch (Exception) { throw new Exception("Erro na propriedade BcIcms"); }

                try { nfe.Valoricms = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vICMS"].InnerText, CultureInfo.InvariantCulture); }
                catch (Exception) { throw new Exception("Erro na propriedade Valoricms"); }

                if (versao == "4.00")
                {
                    try
                    {
                        nfe.ValorFcp = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vFCP"].InnerText, CultureInfo.InvariantCulture);
                        nfe.BcFcp = nfe.BcIcms;
                    }
                    catch (Exception) { throw new Exception("Erro na propriedade ValorFCP"); }
                }

                try { nfe.BcIcmsSt = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vBCST"].InnerText, CultureInfo.InvariantCulture); }
                catch (Exception) { throw new Exception("Erro na propriedade BcIcmsSt"); }

                try { nfe.ValorIcmsSt = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vST"].InnerText, CultureInfo.InvariantCulture); }
                catch (Exception) { throw new Exception("Erro na propriedade ValorIcmsSt"); }

                if (versao == "4.00")
                {
                    try
                    {
                        nfe.ValorFcpSt = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vFCPST"].InnerText, CultureInfo.InvariantCulture);
                        nfe.BcFcpSt = nfe.BcIcmsSt;
                    }
                    catch (Exception) { throw new Exception("Erro na propriedade ValorFCPST"); }
                }

                try { nfe.TotalProd = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vProd"].InnerText, CultureInfo.InvariantCulture); }
                catch (Exception) { throw new Exception("Erro na propriedade TotalProd"); }

                try { nfe.ValorFrete = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vFrete"].InnerText, CultureInfo.InvariantCulture); }
                catch (Exception) { throw new Exception("Erro na propriedade ValorFrete"); }

                try { nfe.ValorSeguro = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vSeg"].InnerText, CultureInfo.InvariantCulture); }
                catch (Exception) { throw new Exception("Erro na propriedade ValorSeguro"); }

                try { nfe.Desconto = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vDesc"].InnerText, CultureInfo.InvariantCulture); }
                catch (Exception) { throw new Exception("Erro na propriedade Desconto"); }

                try { nfe.ValorIpi = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vIPI"].InnerText, CultureInfo.InvariantCulture); }
                catch (Exception) { throw new Exception("Erro na propriedade ValorIpi"); }

                if (versao == "4.00")
                {
                    try { nfe.ValorIpiDevolvido = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vIPIDevol"].InnerText, CultureInfo.InvariantCulture); }
                    catch (Exception) { throw new Exception("Erro na propriedade ValorIPIdevolvido"); }
                }

                try { nfe.OutrasDespesas = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vOutro"].InnerText, CultureInfo.InvariantCulture); }
                catch (Exception) { throw new Exception("Erro na propriedade OutrasDespesas"); }

                try { nfe.TotalNota = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vNF"].InnerText, CultureInfo.InvariantCulture); }
                catch (Exception) { throw new Exception("Erro na propriedade TotalNota"); }

                try { nfe.PesoBruto = decimal.Parse(nfeRoot["infNFe"]["transp"]["vol"]["pesoB"].InnerText, CultureInfo.InvariantCulture); }
                catch (NullReferenceException) { }

                try { nfe.PesoLiq = decimal.Parse(nfeRoot["infNFe"]["transp"]["vol"]["pesoL"].InnerText, CultureInfo.InvariantCulture); }
                catch (NullReferenceException) { }

                try { nfe.NumProtocolo = loadedNFE["nfeProc"] != null ? loadedNFE["nfeProc"]["protNFe"]["infProt"]["nProt"].InnerText : loadedNFE["NFe"]["infProt"]["nProt"].InnerText; }
                catch (NullReferenceException) { }

                try
                {
                    nfe.InfCompl = nfeRoot["infNFe"]["infAdic"]["infCpl"].InnerText;
                    if (nfe.InfCompl.Length > 500)
                    {
                        nfe.InfCompl = nfe.InfCompl.Substring(0, 500);
                    }
                }
                catch (NullReferenceException) { }

                try
                {
                    nfe.TipoAmbiente = Glass.Conversoes.StrParaInt(loadedNFE["nfeProc"]["protNFe"]["infProt"]["tpAmb"].InnerText);
                }
                catch (Exception)
                {
                    try { nfe.TipoAmbiente = Glass.Conversoes.StrParaInt(nfeRoot["infNFe"]["ide"]["tpAmb"].InnerText); }
                    catch (Exception)
                    {
                        throw new Exception("Erro na propriedade TipoAmbiente");
                    }
                }

                try { nfe.NumeroNFe = Glass.Conversoes.StrParaUint(nfeRoot["infNFe"]["ide"]["nNF"].InnerText); }
                catch (Exception) { throw new Exception("Erro na propriedade NumeroNFe"); }

                try { nfe.IdConta = dadosImportar.IdPlanoConta; }
                catch (Exception) { throw new Exception("Erro na propriedade IdConta"); }

                try { nfe.IdNaturezaOperacao = dadosImportar.IdNaturezaOperacao; }
                catch (Exception) { throw new Exception("Erro na propriedade IdNaturezaOperacao"); }

                if (dadosImportar.IdCompra > 0 && !CompraDAO.Instance.Exists(dadosImportar.IdCompra.Value))
                    throw new Exception("Número de compra informado não existe.");

                nfe.GerarContasPagar = dadosImportar.IdCompra > 0 && FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar;
                nfe.Transporte = false;
                nfe.GerarEstoqueReal = true;

                try
                {
                    if (IMPORTAR_PIS_COFINS)
                    {
                        // Valor de PIS e COFINS não devem ser inseridos nas notas de entrada de terceiros
                        nfe.ValorPis = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vPIS"].InnerText, CultureInfo.InvariantCulture);
                        nfe.ValorCofins = decimal.Parse(nfeRoot["infNFe"]["total"]["ICMSTot"]["vCOFINS"].InnerText, CultureInfo.InvariantCulture);

                        // Chamado 14733: A alíquota do PIS/COFINS não deve ser calculada dessa forma, caso seja necessária, a alíquota deverá considerar a alíquota somente
                        // dos produtos que calculem PIS/COFINS
                        //nfe.AliqPis = (float)(nfe.ValorPis != 0 ? nfe.TotalNota / nfe.ValorPis : 0);
                        //nfe.AliqCofins = (float)(nfe.ValorCofins != 0 ? nfe.TotalNota / nfe.ValorCofins : 0);
                    }
                }
                catch (NullReferenceException) { throw new Exception("Erro ao calcular os valores de imposto da nota."); }
                catch (Exception ex) { throw ex; }
            }
            catch (Exception ex)
            {
                throw new Exception("Confira se os dados do arquivo xml estão corretos e tente novamente.<br />" + ex.Message);
            }

            #endregion

            try
            {
                idNotaFiscalInserida = NotaFiscalDAO.Instance.Insert(nfe);

                if (idNotaFiscalInserida == 0)
                    throw new Exception("Erro ao importar a NFe.");
            }
            catch (Exception ex)
            {
                throw new Exception("Confira se os dados do arquivo xml estão corretos e tente novamente.<br/>Mensagem de erro: " + ex.Message);
            }

            if (dadosImportar.IdCompra > 0)
            {
                var cnf = new CompraNotaFiscal()
                {
                    IdNf = idNotaFiscalInserida,
                    IdCompra = dadosImportar.IdCompra.Value
                };

                CompraNotaFiscalDAO.Instance.Insert(cnf);
            }

            #endregion

            #region Inserção na tabela pagto_nota_fiscal

            // Informações de Pagamento
            try
            {
                var count = 1;
                foreach (var pagto in dadosImportar.Pagamentos)
                {
                    var pagtoNotaFiscal = new Glass.Data.Model.PagtoNotaFiscal();

                    pagtoNotaFiscal.IdNf = (int)idNotaFiscalInserida;
                    pagtoNotaFiscal.FormaPagto = pagto.FormaPagto;
                    pagtoNotaFiscal.Valor = pagto.Valor;

                    try
                    {
                        PagtoNotaFiscalDAO.Instance.Insert(pagtoNotaFiscal);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Erro ao inserir os pagamentos da NFe.<br />Erro no pagamento de número " + (count), ex));
                    }

                    count++;
                }
            }
            catch (Exception ex) { throw ex; }

            #endregion

            #region Inserção na tabela parcela_nf

            // Informações de Pagamento
            try
            {
                var count = 1;
                foreach (var parc in dadosImportar.Parcelas)
                {
                    var parcelaNotaFiscal = new Glass.Data.Model.ParcelaNf();

                    parcelaNotaFiscal.IdNf = idNotaFiscalInserida;
                    parcelaNotaFiscal.Data = parc.DataVenc;
                    parcelaNotaFiscal.Valor = parc.Valor;
                    parcelaNotaFiscal.NumBoleto = parc.NumBoleto;

                    try
                    {
                        ParcelaNfDAO.Instance.Insert(parcelaNotaFiscal);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Erro ao inserir as parcelas da NFe.<br />Erro na parcela de número " + (count), ex));
                    }

                    count++;
                }
            }
            catch (Exception ex) { throw ex; }

            #endregion

            #region Inserção na tabela produtos_nf

            try
            {
                XmlNodeList xlist = loadedNFE.GetElementsByTagName("det");
                for (int count = 0; count < loadedNFE.GetElementsByTagName("det").Count; count++)
                {
                    XmlElement xel = (XmlElement)xlist[count];

                    if (nfe.IdFornec.GetValueOrDefault(0) == 0)
                    {
                        XmlElement nfeEmit = nfeRoot["infNFe"]["emit"];
                        string destCnpj = nfeEmit["CNPJ"].InnerText;

                        nfe.IdFornec = Glass.Conversoes.StrParaUint(FornecedorDAO.Instance.GetFornecedorByCPFCNPJ(destCnpj));
                        if (nfe.IdFornec == 0)
                            throw new Exception("Fornecedor (" + Glass.Formatacoes.FormataCpfCnpj(destCnpj) + ") não encontrado.");
                    }

                    uint idProd = ProdutoFornecedorDAO.Instance.GetIdProdByIdFornecCodFornec(nfe.IdFornec.Value, xel["prod"]["cProd"].InnerText);

                    if (idProd == 0)
                        throw new Exception("Alguns produtos da nota não foram associados.");

                    var produtoAssociado = ProdutoDAO.Instance.GetElementByPrimaryKey(idProd);

                    var produtoNF = new Glass.Data.Model.ProdutosNf();
                    produtoNF.IdNf = idNotaFiscalInserida;
                    produtoNF.IdProd = (uint)produtoAssociado.IdProd;
                    produtoNF.Altura = produtoAssociado.Altura.GetValueOrDefault();
                    produtoNF.Largura = produtoAssociado.Largura.GetValueOrDefault();

                    int tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)produtoNF.IdProd, true);
                    bool calcM2 = tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                    try { produtoNF.Qtde = calcM2 ? 1 : float.Parse(xel["prod"]["qCom"].InnerText, CultureInfo.InvariantCulture); }
                    catch { throw new Exception("Erro na propriedade Qtde do produto " + count); }

                    try { produtoNF.TotM = calcM2 || produtoAssociado.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro ? float.Parse(xel["prod"]["qCom"].InnerText, CultureInfo.InvariantCulture) : 0; }
                    catch { throw new Exception("Erro na propriedade TotM do produto " + count); }

                    try { produtoNF.Ncm = xel["prod"]["NCM"].InnerText; }
                    catch { throw new Exception("Erro na propriedade Ncm do produto " + count); }

                    try { produtoNF.TipoMercadoria = (int?)produtoAssociado.TipoMercadoria; }
                    catch { throw new Exception("Erro na propriedade TipoMercadoria do produto " + count); }

                    try { produtoNF.IdNaturezaOperacao = Glass.Conversoes.StrParaUint((string)dadosImportar.NaturezaOperacaoProd["prodNatOp" + count.ToString()]); }
                    catch { throw new Exception("O CFOP do produto " + produtoAssociado.Descricao + " não foi informado."); }

                    try { produtoNF.ValorUnitario = decimal.Parse(xel["prod"]["vUnCom"].InnerText, CultureInfo.InvariantCulture); }
                    catch { throw new Exception("Erro na propriedade Valor Unitário do produto " + produtoAssociado.Descricao); }

                    if (xel["prod"]["vProd"] != null)
                        produtoNF.Total = decimal.Parse(xel["prod"]["vProd"].InnerText, CultureInfo.InvariantCulture);

                    if (xel["prod"]["qTrib"] != null)
                        produtoNF.QtdeTrib = Glass.Conversoes.StrParaFloat(xel["prod"]["qTrib"].InnerText);

                    if (xel["prod"]["vUnTrib"] != null)
                        produtoNF.ValorUnitarioTrib = Glass.Conversoes.StrParaDecimal(xel["prod"]["vUnTrib"].InnerText);

                    if (xel["prod"]["nFCI"] != null)
                        produtoNF.NumControleFciStr = xel["prod"]["nFCI"].InnerText;

                    #region CSTs por Natureza de Operação

                    if (produtoNF.IdNaturezaOperacao > 0)
                    {
                        var nat = NaturezaOperacaoDAO.Instance.ObtemElemento((int)produtoNF.IdNaturezaOperacao.Value);

                        if (nat != null)
                        {
                            produtoNF.Cst = nat.CstIcms ?? produtoNF.Cst;
                            produtoNF.PercRedBcIcms = nat.PercReducaoBcIcms;
                            produtoNF.PercDiferimento = nat.PercDiferimento;
                            produtoNF.CstIpi = ((int?)nat.CstIpi) ?? (int?)ConfigNFe.CstIpi(idNotaFiscalInserida, nat.CodCfop);
                            produtoNF.CstPis = nat.CstPisCofins ?? ConfigNFe.CstPisCofins(idNotaFiscalInserida);
                            produtoNF.CstCofins = nat.CstPisCofins ?? ConfigNFe.CstPisCofins(idNotaFiscalInserida);
                        }
                    }

                    #endregion

                    #region ICMS

                    XmlNode xelICMS;

                    try
                    {
                        try
                        {
                            xelICMS = xel["imposto"]["ICMS"] != null ? xel["imposto"]["ICMS"].ChildNodes[0] : null;

                            if (xelICMS != null && xelICMS.Name.Equals("ICMS20"))
                            {
                                produtoNF.PercRedBcIcms = float.Parse(xelICMS["pRedBC"].InnerText, CultureInfo.InvariantCulture);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Erro nos dados de ICMS dos produtos que constam na NFe.", ex));
                        }

                        if (xelICMS != null)
                        {
                            //CSTORIG
                            if (xelICMS["orig"] != null)
                                produtoNF.CstOrig = Int32.Parse(xelICMS["orig"].InnerText);

                            //VALOR ICMS
                            if (xelICMS["vICMS"] != null)
                                produtoNF.ValorIcms = decimal.Parse(xelICMS["vICMS"].InnerText,
                                    CultureInfo.InvariantCulture);

                            //CST ICMS
                            if (String.IsNullOrEmpty(produtoNF.Cst) && xelICMS["CST"] != null)
                                produtoNF.Cst = xelICMS["CST"].InnerText;

                            //ALIQUOTA ICMS
                            if (xelICMS["pICMS"] != null)
                                produtoNF.AliqIcms = float.Parse(xelICMS["pICMS"].InnerText,
                                    CultureInfo.InvariantCulture);

                            //BC ICMS
                            if (xelICMS["vBC"] != null)
                                produtoNF.BcIcms = decimal.Parse(xelICMS["vBC"].InnerText, CultureInfo.InvariantCulture);

                            // VALOR FCP
                            if (xelICMS["vFCP"] != null)
                                produtoNF.ValorFcp = decimal.Parse(xelICMS["vFCP"].InnerText, CultureInfo.InvariantCulture);

                            // ALIQUOTA FCP
                            if (xelICMS["pFCP"] != null)
                                produtoNF.AliqFcp = float.Parse(xelICMS["pFCP"].InnerText, CultureInfo.InvariantCulture);

                            // BC FCP
                            if (xelICMS["vBCFCP"] != null)
                                produtoNF.BcFcp = decimal.Parse(xelICMS["vBCFCP"].InnerText, CultureInfo.InvariantCulture);

                            //VALOR ICMS ST
                            if (xelICMS["vICMSST"] != null)
                                produtoNF.ValorIcmsSt = decimal.Parse(xelICMS["vICMSST"].InnerText,
                                    CultureInfo.InvariantCulture);

                            //ALIQUOTA ICMS ST
                            if (xelICMS["pICMSST"] != null)
                                produtoNF.AliqIcmsSt = float.Parse(xelICMS["pICMSST"].InnerText,
                                    CultureInfo.InvariantCulture);
                            
                            if (xelICMS["pMVAST"] != null)
                                produtoNF.Mva = float.Parse(xelICMS["pMVAST"].InnerText,
                                    CultureInfo.InvariantCulture);

                            //BC ICMS ST
                            if (xelICMS["vBCST"] != null)
                                produtoNF.BcIcmsSt = decimal.Parse(xelICMS["vBCST"].InnerText,
                                    CultureInfo.InvariantCulture);

                            // VALOR FCP ST
                            if (xelICMS["vFCPST"] != null)
                                produtoNF.ValorFcpSt = decimal.Parse(xelICMS["vFCPST"].InnerText, CultureInfo.InvariantCulture);

                            // ALIQUOTA FCP ST
                            if (xelICMS["pFCPST"] != null)
                                produtoNF.AliqFcpSt = float.Parse(xelICMS["pFCPST"].InnerText, CultureInfo.InvariantCulture);

                            // BC FCP ST
                            if (xelICMS["vBCFCPST"] != null)
                                produtoNF.BcFcpSt = decimal.Parse(xelICMS["vBCFCPST"].InnerText, CultureInfo.InvariantCulture);

                            //CSOSN
                            if (xelICMS["CSOSN"] != null)
                                produtoNF.Csosn = xelICMS["CSOSN"].InnerText;

                            //Busca o codigo do valor fiscal ICMS
                            produtoNF.CodValorFiscal = Glass.Data.DAL.NotaFiscalDAO.Instance.ObtemCodValorFiscal(null,
                                nfe.TipoDocumento, nfe.IdLoja.GetValueOrDefault(), produtoNF.Cst);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha nos dados de ICMS dos produtos que constam na NFe.", ex));
                    }

                    #endregion

                    #region IPI

                    try
                    {
                        XmlNode xelIPI = xel["imposto"]["IPI"]["IPITrib"];
                        try { produtoNF.AliqIpi = float.Parse(xelIPI["pIPI"].InnerText, CultureInfo.InvariantCulture); }
                        catch (NullReferenceException) { }
                        try { produtoNF.ValorIpi = decimal.Parse(xelIPI["vIPI"].InnerText, CultureInfo.InvariantCulture); }
                        catch (NullReferenceException) { }
                        try { produtoNF.CstIpi = produtoNF.CstIpi ?? Int32.Parse(xelIPI["CST"].InnerText); }
                        catch (NullReferenceException) { }
                    }
                    catch (NullReferenceException)
                    {
                        try
                        {
                            XmlNode xelIPI = xel["imposto"]["IPI"]["IPINT"];
                            try { produtoNF.AliqIpi = float.Parse(xelIPI["pIPI"].InnerText, CultureInfo.InvariantCulture); }
                            catch (NullReferenceException) { }
                            try { produtoNF.ValorIpi = decimal.Parse(xelIPI["vIPI"].InnerText, CultureInfo.InvariantCulture); }
                            catch (NullReferenceException) { }
                            try { produtoNF.CstIpi = produtoNF.CstIpi ?? Int32.Parse(xelIPI["CST"].InnerText); }
                            catch (NullReferenceException) { }
                        }
                        catch (NullReferenceException)
                        {

                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Erro nos dados de IPI dos produtos que constam na NFe (1).", ex));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Erro nos dados de IPI dos produtos que constam na NFe (2).", ex));
                    }

                    #endregion

                    #region PIS/COFINS

                    if (IMPORTAR_PIS_COFINS)
                    {
                        // Valor de PIS e COFINS não devem ser inseridos nas notas de entrada de terceiros
                        if (xel["imposto"]["PIS"] != null)
                        {
                            if (xel["imposto"]["PIS"]["PISAliq"] != null)
                            {
                                produtoNF.CstPis = produtoNF.CstPis ?? Glass.Conversoes.StrParaInt(xel["imposto"]["PIS"]["PISAliq"]["CST"].InnerText);
                                produtoNF.BcPis = Glass.Conversoes.StrParaInt(xel["imposto"]["PIS"]["PISAliq"]["vBC"].InnerText);
                                produtoNF.AliqPis = Glass.Conversoes.StrParaInt(xel["imposto"]["PIS"]["PISAliq"]["pPIS"].InnerText);
                                produtoNF.ValorPis = Glass.Conversoes.StrParaInt(xel["imposto"]["PIS"]["PISAliq"]["vPIS"].InnerText);
                            }
                        }

                        if (xel["imposto"]["COFINS"] != null)
                        {
                            if (xel["imposto"]["COFINS"]["COFINSAliq"] != null)
                            {
                                produtoNF.CstCofins = produtoNF.CstCofins ?? Glass.Conversoes.StrParaInt(xel["imposto"]["COFINS"]["COFINSAliq"]["CST"].InnerText);
                                produtoNF.BcCofins = Glass.Conversoes.StrParaInt(xel["imposto"]["COFINS"]["COFINSAliq"]["vBC"].InnerText);
                                produtoNF.AliqCofins = Glass.Conversoes.StrParaInt(xel["imposto"]["COFINS"]["COFINSAliq"]["pCOFINS"].InnerText);
                                produtoNF.ValorCofins = Glass.Conversoes.StrParaInt(xel["imposto"]["COFINS"]["COFINSAliq"]["vCOFINS"].InnerText);
                            }
                        }
                    }

                    #endregion

                    try
                    {
                        ProdutosNfDAO.Instance.InsertBase(produtoNF);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Erro ao inserir os produtos da NFe.<br />Erro no produto de número " + (count + 1), ex));
                    }
                }
            }
            catch (Exception ex)
            {
                //Caso dê erro: apagar a nota fiscal que foi inserida antes
                NotaFiscalDAO.Instance.DeleteByPrimaryKey(idNotaFiscalInserida);

                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao importar itens da nota fiscal.", ex));
            }

            #endregion
        }
    }
}
