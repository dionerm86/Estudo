using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.NFeUtils;
using Glass.Data.CFeUtils;
using Glass.Configuracoes;
using Glass.Global;

namespace Glass.Data.DAL
{
    public sealed class CupomFiscalDAO : BaseCadastroDAO<CupomFiscal, CupomFiscalDAO>
    {
        //private CupomFiscalDAO() { }

        #region Metodos Publicos
       
        public void cancelarCupom(uint sequencialCupom)
        {
            try
            {
                CupomFiscal objCupom = GetElementByPrimaryKey(sequencialCupom);
                objCupom.Cancelado = "S";
                int result = objPersistence.Update(objCupom);

                if (result < 1)
                    throw new Exception("Não foi possível salvar os dados de cancelamento do Cupom.");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public String ObterDadosCancelarVenda(string codAtivacao , string numeroCaixa, uint idLoja, out string chaveSAT, out uint sequencialCupom)
        {
            CupomFiscal cupom = getUltimoCupomEmitido();
            chaveSAT = "";
            sequencialCupom = 0;

            try
            {
                if (cupom != null)
                {
                    if (cupom.Cancelado == "S")
                        throw new Exception("Este cupom já foi cancelado.");

                    if (cupom.DataEmissao.AddMinutes(30) < DateTime.Now)
                        throw new Exception("Este cupom foi emitido a mais de 30 minutos e não pode ser cancelado.");

                    Loja loja = LojaDAO.Instance.GetElement(idLoja);
                    Cliente cliente = ClienteDAO.Instance.GetElement(cupom.IdCliente);
                    chaveSAT = cupom.ChaveCupomSat;
                    sequencialCupom = cupom.IdCupomFiscal;

                    string dadosCanc = FormataXML.MontarXmlCancelamento(cupom.ChaveCupomSat, numeroCaixa, cupom.DataEmissao, cliente, loja);

                    // Formato Mensagem Retorno (Em caso de Sucesso)
                    // numeroSessao|EEEEE|CCCC|mensagem|cod|mensagemSEFAZ|arquivoCFeBase64|timeS tamp|chaveConsulta|valorTotalCFe|CPFCNPJValue|assinaturaQRCODE
                    // Se der erro no processamento pelo SAT, o retorno terá o seguinte formato:
                    // numeroSessao|EEEEE|CCCC|mensagem|cod|mensagemSEFAZ
                    //retornoSAT = "";// SAT.SAT.CancelarUltimaVenda(numSessao, codAtivacao, cupom.ChaveCupomSat, dadosCanc);
                    //camposRetorno = retornoSAT.Split('|');

                    // Código 07000 indica sucesso no cancelamento do CFe.
                    //if (camposRetorno == null || camposRetorno.Length <= 6 || camposRetorno[1].Trim() != "07000")
                    //    throw new Exception(camposRetorno[3]);

                    //cupom.Cancelado = "S";
                    //objPersistence.Update(cupom);

                    //return camposRetorno[6];
                    return dadosCanc;
                }
                else
                    throw new Exception("Cupom fiscal não encontrado");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public uint inserirCupom(CupomFiscal cupomFiscal)
        {
            return objPersistence.Insert(cupomFiscal);
        }
        
        public ProdutosNf[] obterProdutosPedido(string idsPedidos, uint idLoja, uint idNaturezaOperacao, out uint idCliente)
        {
            ProdutosPedido[] lstProd = ProdutosPedidoDAO.Instance.GetByVariosPedidos(idsPedidos, "", FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe, false);
            Pedido[] peds = PedidoDAO.Instance.GetByString(null, idsPedidos);
            List<ProdutosNf> lstProdNF = new List<ProdutosNf>();
            decimal desconto = 0;
            decimal totalNota = 0;
            idCliente = 0;

            try
            {
                foreach (Pedido ped in peds)
                {
                    idCliente = ped.IdCli;

                    // Motivo retirada: Mesmo que seja nota de liberação normal ou parcial, deve considerar o desconto dado no pedido,
                    // caso contrário o mesmo não será rateado na nota.
                    if (!PedidoConfig.RatearDescontoProdutos)
                    {
                        desconto += ped.DescontoTotal;
                        totalNota += ped.Total + ped.DescontoTotal;
                    }
                    else
                        totalNota += ped.Total;

                    if (ped.Situacao != Pedido.SituacaoPedido.Confirmado && ped.Situacao != Pedido.SituacaoPedido.ConfirmadoLiberacao && ped.Situacao != Pedido.SituacaoPedido.LiberadoParcialmente)
                        throw new Exception("Este pedido ainda não foi confirmado.");

                    if (ped.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra)
                        throw new Exception("Não é possível gerar cupom fiscal de pedidos mão-de-obra. Pedido: " + ped.IdPedido);
                }

                decimal percDesc = desconto / totalNota;

                foreach (ProdutosPedido pp in lstProd)
                {
                    if (pp.Qtde <= 0)
                        continue;

                    Produto prod = ProdutoDAO.Instance.GetElement(null, pp.IdProd, idLoja, idCliente, null, true);

                    int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(prod.IdGrupoProd, prod.IdSubgrupoProd, true);

                    // Recalcula as medidas dos alumínios para que o tamanho cobrado seja exato e o valor na nota fique correto
                    if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 ||
                        tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6)
                    {
                        float alturaCalc = pp.Altura;
                        CalculosFluxo.ArredondaAluminio(tipoCalc, pp.Altura, ref alturaCalc);
                        pp.Altura = alturaCalc;
                    }

                    if (prod.Ncm != null && prod.Ncm.Length > 8)
                        throw new Exception("O tamanho do campo NCM do produto " + prod.CodInterno + " - " + prod.Descricao + ", não pode ter mais que 8 caracteres.");

                    ProdutosNf prodNf = new ProdutosNf();
                    prodNf.DescrProduto = pp.DescrProduto;
                    prodNf.CodInterno = pp.CodInterno;
                    prodNf.IdProd = pp.IdProd;
                    prodNf.Qtde = pp.Qtde;
                    prodNf.Altura = pp.Altura;
                    prodNf.Largura = pp.Largura;
                    prodNf.TotM = FiscalConfig.NotaFiscalConfig.ConsiderarM2CalcNotaFiscal ? pp.TotM2Calc : pp.TotM;
                    prodNf.Cst = prod.Cst;
                    prodNf.CstOrig = prod.Descricao.ToLower().Contains("importado") ? 1 : 0;
                    prodNf.Csosn = prod.Csosn;
                    prodNf.AliqIcms = IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(null, pp.IdProd, idLoja, null, idCliente);
                    prodNf.AliqIcmsSt = prod.AliqIcmsStInterna;
                    prodNf.AliqIpi = prod.AliqIPI;
                    prodNf.IdNaturezaOperacao = idNaturezaOperacao;
                    var ncmNaturezaOp = prodNf.IdNaturezaOperacao > 0 ? NaturezaOperacaoDAO.Instance.ObtemNcm(null, prodNf.IdNaturezaOperacao.Value) : null;
                    prodNf.Ncm = !string.IsNullOrEmpty(ncmNaturezaOp) ? ncmNaturezaOp : prod.Ncm;
                    prodNf.Mva = MvaProdutoUfDAO.Instance.ObterMvaPorProduto(null, (int)pp.IdProd, idLoja, null, idCliente, true);
                    prodNf.CstIpi = (int?)prod.CstIpi;
                    prodNf.IdContaContabil = (uint?)prod.IdContaContabil;
                    // prodNf.CstIpi = ConfigNFe.CstIpi(idNf, CfopDAO.Instance.ObtemCodInterno(idCfop));
                    prodNf.GTINProduto = (prod.GTINProduto != null) ? prod.GTINProduto : "";
                    prodNf.Unidade = prod.Unidade;

                    prodNf.Total = pp.QtdeOriginal > 0 ?
                        (pp.Total + pp.ValorBenef) / (decimal)pp.QtdeOriginal * (decimal)pp.Qtde : pp.Total + pp.ValorBenef;

                    if (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd))
                        prodNf.InfAdic = prodNf.Largura + "x" + prodNf.Altura;

                    // Verifica se o produto é calculado por ML e se a altura foi informada
                    if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML && prodNf.Altura == 0)
                    {
                        throw new Exception("O produto " + prod.Descricao + " é calculado por ML e tem seu comprimento no pedido " +
                            "zerado. Corrija-o antes de prosseguir com a emissão do cupom fiscal");
                    }

                    // Aplica desconto do pedido no produto
                    if (!PedidoConfig.RatearDescontoProdutos && percDesc > 0)
                        prodNf.Total = prodNf.Total - (prodNf.Total * percDesc);

                    // Verifica se o cliente possui redução de cálculo da Nfe
                    float percReducaoNfe = ClienteDAO.Instance.GetPercReducaoNFe(idCliente);
                    if (percReducaoNfe > 0)
                        prodNf.Total = prodNf.Total - (prodNf.Total * ((decimal)percReducaoNfe / 100));

                    bool clienteCalcIpi = ClienteDAO.Instance.IsCobrarIpi(null, idCliente);
                    bool clienteCalcIcmsSt = ClienteDAO.Instance.IsCobrarIcmsSt(idCliente);
                    bool cfopCalcIpi = NaturezaOperacaoDAO.Instance.CalculaIpi(null, idNaturezaOperacao);
                    bool cfopCalcIcmsSt = NaturezaOperacaoDAO.Instance.CalculaIcmsSt(null, idNaturezaOperacao);

                    // Retira a alíquota do IPI no total do produto (e do icms st para tempera e vidrometro)
                    if ((FiscalConfig.NotaFiscalConfig.RatearIpiNfPedido || 
                        FiscalConfig.NotaFiscalConfig.AliquotaIcmsStRatearIpiNfPedido != ConfigNFe.TipoCalculoIcmsStNf.NaoCalcular) &&
                        prod.AliqIPI > 0 && cfopCalcIpi)
                    {
                        if (!PedidoConfig.Impostos.CalcularIpiPedido || !clienteCalcIpi)
                        {
                            decimal aliqIcms = cfopCalcIcmsSt ?
                                (FiscalConfig.NotaFiscalConfig.AliquotaIcmsStRatearIpiNfPedido == ConfigNFe.TipoCalculoIcmsStNf.CalculoPadrao ? prod.AliqICMSInterna :
                                FiscalConfig.NotaFiscalConfig.AliquotaIcmsStRatearIpiNfPedido == ConfigNFe.TipoCalculoIcmsStNf.AliquotaIcmsStComIpi ?
                                prod.AliqICMSInternaComIpiNoCalculo : 0) : 0;

                            // Caso a alíquota de icms esteja zerada (automaticamente não considerando a diferença no cálculo do ICMS ST 
                            // que a cobrança do IPI iria causar) e o cliente calcule ICMS ST no pedido considerando o IPI no cálculo
                            // e o cliente esteja marcado para calcular ICMS no pedido mas não para calcular IPI no mesmo porém a nota 
                            // calcule ICMS ST e IPI, é necessário recalcular este produto considerando a AliqICMSInterna junto com a 
                            // alíquota do IPI e depois adicionando o valor da AliqICMSInterna somente
                            bool recalcularIcmsStProd = aliqIcms == 0 && FiscalConfig.NotaFiscalConfig.CalculoAliquotaIcmsSt == ConfigNFe.TipoCalculoIcmsSt.ComIpiNoCalculo &&
                                (!clienteCalcIpi || !PedidoConfig.Impostos.CalcularIpiPedido) && clienteCalcIcmsSt && PedidoConfig.Impostos.CalcularIcmsPedido;

                            if (recalcularIcmsStProd)
                                aliqIcms = prod.AliqICMSInterna;

                            // Rateia o valor dos impostos no total do produto
                            prodNf.Total = (prodNf.Total / (decimal)(1 + (((decimal)prod.AliqIPI + aliqIcms) / 100)));

                            if (recalcularIcmsStProd)
                                prodNf.Total *= (decimal)(1 + (aliqIcms / 100));
                        }
                    }
                    // Retira o valor do icms st do cupom, caso não tenha IPI mas tenha ICMS ST
                    else if (!PedidoConfig.Impostos.CalcularIcmsPedido && cfopCalcIcmsSt && 
                        FiscalConfig.NotaFiscalConfig.RatearIcmsStNfPedido)
                        prodNf.Total = prodNf.Total / (decimal)(1 + (prod.AliqICMSInterna / 100));

                    // Calcula o valor unitário
                    if (tipoCalc == (uint)Glass.Data.Model.TipoCalculoGrupoProd.Qtd || tipoCalc == (uint)Glass.Data.Model.TipoCalculoGrupoProd.QtdM2)
                        prodNf.ValorUnitario = prodNf.Total / (decimal)prodNf.Qtde;
                    else if (prodNf.TotM > 0)
                        prodNf.ValorUnitario = prodNf.Total / (decimal)prodNf.TotM;
                    else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                        prodNf.ValorUnitario = prodNf.Total / (decimal)(prodNf.Altura * prodNf.Qtde);
                    else if (prodNf.Altura > 0)
                        prodNf.ValorUnitario = (prodNf.Total * 6) / (decimal)(prodNf.Altura * prodNf.Qtde);
                    else
                        prodNf.ValorUnitario = prodNf.Total / (decimal)prodNf.Qtde;

                    // Rateia o valor do fast delivery na nota.
                    if (PedidoDAO.Instance.IsFastDelivery(pp.IdPedido))
                        prodNf.ValorUnitario *= (decimal)((PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery / 100) + 1);

                    if (prodNf.ValorUnitario.ToString().Contains("NaN"))
                        prodNf.ValorUnitario = 0;

                    // Calcula PIS e COFINS
                    prodNf.CstPis = ConfigCFe.CstPisCofins(idLoja);
                    prodNf.AliqPis = ConfigNFe.AliqPis(idLoja);
                    prodNf.BcPis = prodNf.Total;
                    prodNf.ValorPis = prodNf.BcPis * (decimal)prodNf.AliqPis / 100;
                    prodNf.CstCofins = ConfigCFe.CstPisCofins(idLoja);
                    prodNf.AliqCofins = ConfigNFe.AliqCofins(idLoja);
                    prodNf.BcCofins = prodNf.Total;
                    prodNf.ValorCofins = prodNf.BcCofins * (decimal)prodNf.AliqCofins / 100;

                    // Importa os beneficiamentos do produtoPedido para o produtoNf
                    //ProdutoNfBenefDAO.Instance.ImportaProdPedBenef(pp.IdProdPed, idProdNf);

                    lstProdNF.Add(prodNf);
                }

                return lstProdNF.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CuponsFiscaisGerados(uint idPedido)
        {
            return PedidosCupomFiscalDAO.Instance.CuponsFiscaisGerados(idPedido);
        }

        public int GerarNumeroSessao()
        {
            return new Random().Next(0, 999999);
        }

        public CupomFiscal getCupomByChaveCFe(string chaveCFe)
        {
            List<CupomFiscal> lstRetorno;
            
            string sql = "select * from cupom_fiscal_cfe where chaveCupomSat='" + chaveCFe + "'";
            lstRetorno = objPersistence.LoadData(sql);

            return (lstRetorno != null && lstRetorno.Count > 0) ? lstRetorno[0] : null;
        }

        public CupomFiscal getUltimoCupomEmitido()
        {
            List<CupomFiscal> lstRetorno;

            string sql = "select * from cupom_fiscal_cfe where dataEmissao = (select max(dataEmissao) from cupom_fiscal_cfe)";
            lstRetorno = objPersistence.LoadData(sql);

            return (lstRetorno != null && lstRetorno.Count > 0) ? lstRetorno[0] : null;
        }
        
        #endregion

        #region Metodos Privados

        public StatusOperacionalSAT MontarStatusOperacional(string[] dadosStatus)
        {
            StatusOperacionalSAT objStatus = new StatusOperacionalSAT();

            objStatus.NumeroSerieSAT = dadosStatus[5];
            objStatus.TipoLAN = dadosStatus[6];
            objStatus.IpAparelho = dadosStatus[7];
            objStatus.MacAdress = dadosStatus[8];
            objStatus.MascaraSubRede = dadosStatus[9];
            objStatus.Gateway = dadosStatus[10];
            objStatus.DNS1 = dadosStatus[11];
            objStatus.DNS2 = dadosStatus[12];
            objStatus.StatusRede = dadosStatus[13];
            objStatus.NivelBateria = dadosStatus[14];
            objStatus.MemoriaTotal = dadosStatus[15];
            objStatus.MemoriaUsada = dadosStatus[16];
            objStatus.DataAtualAparelho = ConvertFromTimeStamp(dadosStatus[17]);
            objStatus.VersaoSoftwareBasico = dadosStatus[18];
            objStatus.VersaoLayoutTabInf = dadosStatus[19];
            objStatus.UltimoCFeEmitido = dadosStatus[20];
            objStatus.PrimeiroCFeArmazenado = dadosStatus[21];
            objStatus.UltimoCFeArmazenado = dadosStatus[22];
            objStatus.UltimaTransmissaoSEFAZ = ConvertFromTimeStamp(dadosStatus[23]);
            objStatus.UltimaComunicacaoSEFAZ = ConvertFromTimeStamp(dadosStatus[24]);
            objStatus.EmissaoCertDigital = ConvertFromTimeStamp(dadosStatus[25]);
            objStatus.VencimentoCertDigital = ConvertFromTimeStamp(dadosStatus[26]);
            objStatus.StatusOperacional = (StatusOperacional)Enum.Parse(typeof(StatusOperacional), dadosStatus[27]);

            return objStatus;
        }

        private DateTime ConvertFromTimeStamp(string timeStamp)
        {
            DateTime data = DateTime.Now;

            if (!String.IsNullOrEmpty(timeStamp) && timeStamp.Length == 14)
            {
                data = new DateTime(Convert.ToInt32(timeStamp.Substring(0, 4)), Convert.ToInt32(timeStamp.Substring(4, 2)),
                    Convert.ToInt32(timeStamp.Substring(6, 2)), Convert.ToInt32(timeStamp.Substring(8, 2)),
                    Convert.ToInt32(timeStamp.Substring(10, 2)), Convert.ToInt32(timeStamp.Substring(12, 2)));
            }
            else if (!String.IsNullOrEmpty(timeStamp) && timeStamp.Length == 8)
            {
                data = new DateTime(Convert.ToInt32(timeStamp.Substring(0, 4)), Convert.ToInt32(timeStamp.Substring(4, 2)),
                    Convert.ToInt32(timeStamp.Substring(6, 2)));
            }

            return data;
        }

        private CupomFiscal montarCupomFiscal(uint numeroSessao, DateTime dataEmissao, string chaveCupomSAT, uint idLoja,
            uint idPedido, bool cfeCancelamento, double totalCupom, double valorPago, uint idCliente)
        {
            CupomFiscal objCupom = new CupomFiscal();

            objCupom.DataCad = DateTime.Now;
            objCupom.DataEmissao = dataEmissao;
            objCupom.ChaveCupomSat = chaveCupomSAT;
            objCupom.IdLoja = idLoja;
            objCupom.NumeroSessao = numeroSessao;
            //objCupom.IdPedido = idPedido;
            objCupom.Usucad = UserInfo.GetUserInfo.CodUser;
            objCupom.DescrUsuCad = UserInfo.GetUserInfo.Nome;
            objCupom.Cancelado = (cfeCancelamento) ? "S" : "N";
            objCupom.TotalCupom = totalCupom;
            objCupom.ValorPago = valorPago;
            objCupom.IdCliente = idCliente;

            return objCupom;
        }

        #endregion
    }
}