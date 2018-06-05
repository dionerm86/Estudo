using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.NFeUtils;
using System.IO;
using System.Xml;
using Glass.Data.EFD;
using System.Linq;
using Glass.Configuracoes;
using Glass.Global;
using System.Text.RegularExpressions;

namespace Glass.Data.DAL
{
    public sealed class NotaFiscalDAO : BaseCadastroDAO<NotaFiscal, NotaFiscalDAO>
    {
        //private NotaFiscalDAO() { }

        #region Gera NF a partir de pedido

        public uint? ObtemCodValorFiscal(GDASession sessao, int tipoDocumento, uint idLoja, string cst)
        {
            bool simplesNacional = new List<int> { (int)CrtLoja.SimplesNacional, (int)CrtLoja.SimplesNacionalExcSub }.
                Contains(LojaDAO.Instance.BuscaCrtLoja(sessao, idLoja));

            return ObtemCodValorFiscal(tipoDocumento, cst, simplesNacional);
        }

        public uint? ObtemCodValorFiscal(int tipoDocumento, string cst, bool simplesNacional)
        {
            if (!simplesNacional)
            {
                switch (cst)
                {
                    case "00":
                    case "20":
                        return 1;
                    case "40":
                    case "41":
                        return 2;
                    case "30":
                    case "50":
                    case "51":
                    case "60":
                        return 3;
                    case "10":
                    case "70":
                    case "90":
                        return (uint)(tipoDocumento != 2 ? 3 : 1);
                }
            }
            else
                return 3;

            return null;
        }

        public uint GerarNf(string idsPedidos, string idsLiberarPedidos, uint? idNaturezaOperacao, uint idLoja,
            float percReducaoNfe, float percReducaoNfeRevenda, Dictionary<uint, uint> naturezasOperacao, uint idCli,
            bool transferencia, uint? idCarregamento, bool transferirNf, bool nfce, bool manterAgrupamentoDeProdutos)
        {
            FilaOperacoes.NotaFiscalInserir.AguardarVez();

            uint idNf = 0;

            try
            {
                /* Chamado 15932.
                 * Criamos esta verificação para impedir que sejam geradas notas fiscais duplicadas no sistema. */
                foreach (var idPedido in idsPedidos.Split(','))
                    foreach (var pedNotaFiscal in PedidosNotaFiscalDAO.Instance.GetByPedido(Conversoes.StrParaUint(idPedido)))
                        if (pedNotaFiscal.IdNf > 0 &&
                            NotaFiscalDAO.Instance.ObtemDataCad(pedNotaFiscal.IdNf) >= DateTime.Now.AddMinutes(-1))
                            throw new Exception("Foi gerada uma nota fiscal para o pedido " + idPedido +
                                " há poucos segundos. Aguarde um minuto e gere novamente");

                // Utilizado no decorrer do método para transferência de nota fiscal.
                var sessionTransf = FiscalConfig.NotaFiscalConfig.ExportarNotaFiscalOutroBD && transferirNf ?
                    new GDASession(GDA.GDASettings.GetProviderConfiguration("Notas")) : null;

                uint idNaturezaOperacaoDestino = 0, idNaturezaOperacaoDestinoProd = 0;
                var dicNaturezaOperacaoProdDestino = new Dictionary<uint, uint>();

                #region Recupera a cidade da loja

                var cidadeLoja = LojaDAO.Instance.ObtemValorCampo<uint?>("idCidade", "idLoja=" + idLoja);

                // Verifica se a cidade da loja foi informada.
                if (cidadeLoja == null)
                    throw new Exception("A cidade da loja que o pedido foi emitido não foi informada.");

                var nomeCidadeLoja = CidadeDAO.Instance.ObtemValorCampo<string>("nomeCidade", "idCidade=" + cidadeLoja.Value);

                #endregion

                if (idsPedidos.Length == 0)
                    idsPedidos = String.Join(",", Array.ConvertAll(PedidoDAO.Instance.GetIdsByLiberacoes(idsLiberarPedidos).ToArray(),
                        x => x.ToString()));

                Pedido[] peds = PedidoDAO.Instance.GetByString(null, idsPedidos);

                if (PedidoConfig.LiberarPedido && FiscalConfig.BloquearEmissaoNFeApenasPedidosLiberados)
                    foreach (var p in peds)
                        if (p.Situacao != Pedido.SituacaoPedido.Confirmado && p.Situacao != Pedido.SituacaoPedido.LiberadoParcialmente)
                            throw new Exception("O pedido " + p.IdPedido + " não está liberado ou liberado parcialmente. Emissão da NF-e não permitida.");

                bool liberacaoParcial = false;

                foreach (string id in idsLiberarPedidos.Split(','))
                {
                    if (String.IsNullOrEmpty(id))
                        continue;

                    liberacaoParcial = LiberarPedidoDAO.Instance.IsLiberacaoParcial(Glass.Conversoes.StrParaUint(id));

                    if (liberacaoParcial)
                        break;
                }

                ProdutosPedido[] lstProd = ProdutosPedidoDAO.Instance.GetByVariosPedidos(idsPedidos, idsLiberarPedidos,
                    FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe, false, (!liberacaoParcial && manterAgrupamentoDeProdutos));

                uint idCliente = 0;
                decimal desconto = 0;
                decimal totalNota = 0;
                decimal descontoLiberacao = LiberarPedidoDAO.Instance.GetDescontos(idsLiberarPedidos);
                decimal totalIpiPedido = 0;
                decimal totalIcmsPedido = 0;
                decimal totalFrete = 0;

                bool nfDeLiberacao = false;

                if (!String.IsNullOrEmpty(idsLiberarPedidos))
                {
                    // Se estiver gerando esta nf a partir de liberação, calcula o total da nota com base na mesma
                    foreach (string idLib in idsLiberarPedidos.Split(','))
                    {
                        if (String.IsNullOrEmpty(idLib) || !LiberarPedidoDAO.Instance.IsLiberacaoParcial(Glass.Conversoes.StrParaUint(idLib)))
                            continue;

                        nfDeLiberacao = true;
                        break;
                    }
                }

                #region Valida pedido

                /* Chamado 46917.
                 * O problema ocorreu porque dois pedidos foram liberados juntos, um com desconto e outro sem.
                 * Dessa forma, o desconto foi aplicado para os produtos de ambos pedidos, ao gerar a nota fiscal,
                 * como, além disso, ainda havia a situação de cálculo de IPI, em ambos pedidos, com alíquotas diferentes,
                 * o total da nota ficou maior que o total da liberação. Separando o cálculo do desconto por pedido,
                 * a situação foi resolvida. Obs.: o desconto por produto do pedido será aplicado caso a configuração
                 * AgruparProdutosGerarNFe esteja desabilitada. */
                var pedidosPercentualDesconto = new Dictionary<int, decimal>();
                /* Chamado 56412.
                 * Esse dicionário foi criado para aplicar o percentual de quantidade do produto nele próprio, ao invés de somá-lo ao desconto do pedido e aplicar para todos os produtos. */
                var produtosPedidoPercentualDescontoQtde = new Dictionary<int, decimal>();

                if(peds.Any(f => f.IdTransportador != peds.First().IdTransportador))
                    throw new Exception("Não é possível gerar nota fiscal com pedidos que possuem transportadoras diferentes.");

                // Verifica se pedido foi confirmado
                foreach (Pedido ped in peds)
                {
                    idCliente = ped.IdCli;

                    // Motivo retirada: Mesmo que seja nota de liberação normal ou parcial, deve considerar o desconto dado no pedido,
                    // caso contrário o mesmo não será rateado na nota.
                    //if (!nfDeLiberacao)
                    {
                        var usarEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(ped.IdPedido);
                        var usarImpPedEsp = PedidoConfig.LiberarPedido && PCPConfig.UsarConferenciaFluxo && usarEspelho;
                        var valorFrete = usarImpPedEsp ? PedidoEspelhoDAO.Instance.ObterValorEntrega(null, ped.IdPedido) : ped.ValorEntrega;
                        var percentualFastDelivery = (ped.TaxaFastDelivery > 0 ? 1 + ((decimal)ped.TaxaFastDelivery / 100) : 1);

                        // Caso o valor da conferência tenha que ser considerado ao gerar a nota fiscal o desconto da mesma deve ser
                        // recuperado. Ao definir a propriedade DescontoTotalPcp como True isto é feito automaticamente.
                        ped.DescontoTotalPcp = usarImpPedEsp;

                        if (!PedidoConfig.RatearDescontoProdutos)
                        {
                            var totalPedido = (ped.DescontoTotalPcp ? ped.TotalEspelho : ped.Total);
                            var valorIpi = usarImpPedEsp ? PedidoEspelhoDAO.Instance.ObtemValorCampo<decimal>("valorIpi", "idPedido=" + ped.IdPedido) : ped.ValorIpi;
                            var valorIcms = usarImpPedEsp ? PedidoEspelhoDAO.Instance.ObtemValorCampo<decimal>("valorIcms", "idPedido=" + ped.IdPedido) : ped.ValorIcms;
                            // Caso os produtos da nota não sejam agrupados, o desconto por quantidade deve ser aplicado nos produtos que possuem esse desconto.
                            var descontoQtdeProdutos = !FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe ? lstProd.Sum(f => f.ValorDescontoQtdeNf) : 0;
                            // Remove o desconto de quantidade dos produtos do desconto do pedido.
                            var descontoPedido = (ped.DescontoTotal - descontoQtdeProdutos) * percentualFastDelivery;

                            // Se o pedido tiver sido liberado parcialmente em um das liberações passadas e possuir desconto,
                            // é necessário somar somente o total liberado deste pedido e o desconto já rateado aplicado nesta liberação,
                            // para que gere a nota rateando corretamente o desconto aplicado no pedido.
                            if (!string.IsNullOrEmpty(idsLiberarPedidos) && LiberarPedidoDAO.Instance.IsPedidoLiberadoParcialmente(ped.IdPedido, idsLiberarPedidos))
                            {
                                var totalLiberado = (decimal)PedidoDAO.Instance.GetTotalLiberado(ped.IdPedido, idsLiberarPedidos);
                                descontoPedido = (totalLiberado / totalPedido) * descontoPedido;

                                // Faz um rateio "grosseiro" do ipi para liberações parciais, se o pedido possuir produtos
                                // com alíquota de ipi diferente, o cálculo não ficará correto
                                valorIpi = (totalLiberado / totalPedido) * valorIpi;
                                valorIcms = (totalLiberado / totalPedido) * valorIcms;
                                totalPedido = totalLiberado;
                            }

                            var totalPedidoCalculado = totalPedido + descontoPedido + descontoQtdeProdutos - valorIpi - valorIcms - valorFrete;
                            
                            if (!FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe)
                            {
                                // Salva, em um dicionário, o percentual de desconto do pedido.
                                if (descontoPedido > 0 && totalPedidoCalculado > 0)
                                    pedidosPercentualDesconto.Add((int)ped.IdPedido, descontoPedido / totalPedidoCalculado);

                                // Salva, em um dicionário, o percentual de desconto de quantidade de cada produto do pedido que possui esse tipo de desconto.
                                foreach (var prodPed in lstProd.Where(f => f.IdPedido == ped.IdPedido && f.ValorDescontoQtdeNf > 0 && f.IdProdPedParent.GetValueOrDefault() == 0))
                                    produtosPedidoPercentualDescontoQtde.Add((int)prodPed.IdProdPed, (decimal)prodPed.PercDescontoQtde);
                            }

                            desconto += descontoPedido + descontoQtdeProdutos;
                            totalNota += totalPedidoCalculado;
                            totalFrete += valorFrete;

                            totalIcmsPedido += valorIcms;
                            totalIpiPedido += valorIpi;
                        }
                        else
                        {
                            totalNota += (ped.DescontoTotalPcp ? ped.TotalEspelho : ped.Total) - ped.ValorIpi - ped.ValorIcms - valorFrete;
                            totalFrete += valorFrete;
                        }
                    }

                    if (ped.Situacao != Pedido.SituacaoPedido.Confirmado && ped.Situacao != Pedido.SituacaoPedido.ConfirmadoLiberacao &&
                        ped.Situacao != Pedido.SituacaoPedido.LiberadoParcialmente)
                    {
                        if (FiscalConfig.PermitirGerarNotaPedidoConferido)
                        {
                            if (ped.Situacao != Pedido.SituacaoPedido.Conferido)
                                throw new Exception("Este pedido ainda não foi conferido.");
                        }
                        else
                            throw new Exception("Este pedido ainda não foi confirmado.");
                    }

                    if (ped.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra && !FiscalConfig.NotaFiscalConfig.PermitirGerarNFPedMaoDeObra)
                        throw new Exception("Não é possível gerar nota fiscal de pedidos mão-de-obra. Pedido: " + ped.IdPedido);

                    if (ped.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial)
                        throw new Exception("Não é possível gerar nota fiscal de pedidos mão de obra especial. Pedido: " + ped.IdPedido);
                }

                #endregion

                #region Valida natureza operação

                var dicProdPedNaturezaOperacao = new Dictionary<uint, uint>();

                // Chamado 13506.
                // Esta validação estava sendo feita após a inserção da nota fiscal, caso a exceção fosse lançada a nota fiscal
                // continuava no sistema, porém, sem produtos.
                foreach (var prodPed in lstProd)
                {
                    uint idNatOp;
                    if (naturezasOperacao != null && naturezasOperacao.TryGetValue(prodPed.IdProd, out idNatOp))
                        dicProdPedNaturezaOperacao.Add(prodPed.IdProdPed, idNatOp);
                    else
                    {
                        // Busca a natureza de operação pela regra, se não houver, considera a natureza associada à nota fiscal.
                        idNatOp = RegraNaturezaOperacaoDAO.Instance.BuscaNaturezaOperacao(NotaFiscal.TipoDoc.Saída,
                            idLoja, idCli > 0 ? idCli : idCliente, (int?)prodPed.IdProd ?? (int?)idNaturezaOperacao).GetValueOrDefault();

                        // Chamado 17614: Caso não encontre uma natureza de operação para o produto, utiliza o da nota.
                        if (idNatOp == 0 && idNaturezaOperacao > 0)
                            idNatOp = idNaturezaOperacao.Value;

                        if (idNatOp == 0)
                            throw new Exception("O produto " + ProdutoDAO.Instance.GetDescrProduto((int)prodPed.IdProd) +
                                " não possui natureza de operação, selecione uma natureza de operação" +
                                " manualmente ou cadastre e preencha corretamente a regra de natureza de operação deste produto.");
                        else
                            dicProdPedNaturezaOperacao.Add(prodPed.IdProdPed, idNatOp);
                    }

                    if (nfce)
                    {
                        var cfop = CfopDAO.Instance.ObtemCodInterno(NaturezaOperacaoDAO.Instance.ObtemIdCfop(idNatOp));

                        if (!CfopDAO.Instance.IsCfopNFCe(cfop))
                            throw new Exception("O CFOP: " + cfop + " não pode ser utilizado para emissão de NFC-e");
                    }
                }

                #endregion

                #region Calcula o total da nota pela(s) liberação(ões)

                if (nfDeLiberacao)
                {
                    // Se estiver gerando esta nf a partir de liberação, calcula o total da nota com base na mesma
                    foreach (string idLib in idsLiberarPedidos.Split(','))
                    {
                        if (String.IsNullOrEmpty(idLib))
                            continue;

                        //desconto += LiberarPedidoDAO.Instance.ObtemValorCampo<decimal>("desconto", "idLiberarPedido=" + idLib);
                        //totalNota += LiberarPedidoDAO.Instance.ObtemValorCampo<decimal>("total", "idLiberarPedido=" + idLib);

                        decimal descLib = LiberarPedidoDAO.Instance.ObtemValorCampo<decimal>("desconto", "idLiberarPedido=" + idLib);
                        desconto += descLib;
                        totalNota += descLib;

                        // Motivo retirada: Mesmo que seja nota de liberação normal ou parcial, deve considerar o desconto dado no pedido,
                        // caso contrário o mesmo não será rateado na nota, logo, o total da nota nesta parte deve ser somado somente 
                        // um possível desconto na liberação
                        //totalNota += LiberarPedidoDAO.Instance.ObtemValorCampo<decimal>("total", "idLiberarPedido=" + idLib);
                    }
                }

                #endregion

                #region Transferência de nota fiscal

                // Desenvolvimento somente para a MS Vidros.
                if (FiscalConfig.NotaFiscalConfig.ExportarNotaFiscalOutroBD && transferirNf)
                {
                    // Valida os dados do cliente na base de destino.
                    var clienteOrigemCpfCnpj = ClienteDAO.Instance.ObtemCpfCnpj(idCli > 0 ? idCli : idCliente);
                    var clienteDestino = ClienteDAO.Instance.GetByCpfCnpj(sessionTransf, clienteOrigemCpfCnpj);

                    if (clienteDestino == null)
                        throw new Exception("O cliente não esta cadastrado no sistema para o qual a nota fiscal será transferida.");

                    // Recupera, da loja destino, a natureza de operação da nota fiscal.
                    if (idNaturezaOperacao > 0)
                    {
                        // Recupera os dados do CFOP e natureza de operação do sistema de origem.
                        var idCfopOrigem = NaturezaOperacaoDAO.Instance.ObtemIdCfop(idNaturezaOperacao.Value);
                        var codInternoCfopOrigem = CfopDAO.Instance.ObtemCodInterno(idCfopOrigem);
                        var codInternoNatOpOrigem = NaturezaOperacaoDAO.Instance.ObtemCodigoInterno(idNaturezaOperacao.Value);
                        // Recupera os dados de CFOP e natureza de operação do sistema de destino.
                        var idCfopDestino = CfopDAO.Instance.ObtemIdCfop(sessionTransf, codInternoCfopOrigem);
                        idNaturezaOperacaoDestino = NaturezaOperacaoDAO.Instance.ObtemIdNatOpPorCfopCodInterno(sessionTransf, idCfopDestino, codInternoNatOpOrigem);

                        #region Valida informações do CFOP e natureza de operação

                        // Recupera o código interno do CFOP e da natureza de operação de destino.
                        var codInternoCfopDestino = CfopDAO.Instance.ObtemCodInterno(sessionTransf, idCfopDestino);
                        var codInternoNatOpDestino = NaturezaOperacaoDAO.Instance.ObtemCodigoInterno(sessionTransf, idNaturezaOperacaoDestino);

                        // Verifica se as informações no sistema de origem e destino são iguais.
                        if ((codInternoCfopOrigem + " - " + codInternoNatOpOrigem).ToUpper() != (codInternoCfopDestino + " - " + codInternoNatOpDestino).ToUpper())
                            throw new Exception("A natureza de operação selecionada é inválida, pois, não existe no sistema para onde a nota será exportada.");

                        #endregion
                    }
                }

                #endregion

                #region Valida NFC-e

                if (nfce)
                {
                    if (totalNota > 200000)
                        throw new Exception("O valor do total excede o máximo da permitido para NFC-e (R$200.000,00)");

                    //Indicador do IE do destinatario
                    var cliente = ClienteDAO.Instance.GetElementByPrimaryKey(idCli > 0 ? idCli : idCliente);

                    if (cliente == null)
                        throw new Exception("O cliente não foi informado.");

                    if (cliente.IndicadorIEDestinatario != IndicadorIEDestinatario.NaoContribuinte)
                        throw new Exception("NFC-e só podera ser emitida para não contribuintes do ICMS");
                }

                #endregion

                decimal percDesc = 0, descontoDestacar = 0;
                if (FiscalConfig.NotaFiscalConfig.RatearDescontoProdutosNotaFiscal)
                    // Pega o percentual do desconto para aplicar nos produtos da nota
                    percDesc = totalNota > 0 ? desconto / totalNota : 0;
                else
                    descontoDestacar = desconto;

                var nf = new NotaFiscal();

                using (var transaction = new GDATransaction())
                {
                    // Se der algum erro ao buscar os produtos do pedido, ignora, para não perder o número da NF gerado
                    try
                    {
                        transaction.BeginTransaction();

                        #region Insere a nota fiscal

                        uint? idLiberacao = LiberarPedidoDAO.Instance.GetIdLiberacao(idsPedidos);
                        var tipoPagtoLiberacao = idLiberacao > 0 ? LiberarPedidoDAO.Instance.ObtemValorCampo<int?>("tipoPagto", "idLiberarPedido=" + idLiberacao) : null;

                        nf.IdCliente = idCli > 0 ? idCli : idCliente;
                        nf.IdLoja = idLoja;
                        nf.IdCidade = cidadeLoja.Value;
                        nf.IdTransportador = peds[0].IdTransportador > 0 ? (uint?)peds[0].IdTransportador : ClienteDAO.Instance.ObtemIdTransportador(nf.IdCliente.Value);
                        nf.Situacao = (int)NotaFiscal.SituacaoEnum.Aberta;
                        nf.TipoDocumento = (int)NotaFiscal.TipoDoc.Saída;
                        nf.DataSaidaEnt = nfce ? null : (DateTime?)DateTime.Now.AddMinutes(1);
                        nf.IdNaturezaOperacao = idNaturezaOperacao;
                        nf.DataEmissao = DateTime.Now;
                        nf.Desconto = descontoLiberacao + descontoDestacar;
                        nf.FormaPagto = FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber && idLiberacao > 0 && LiberarPedidoDAO.Instance.GetTotalLiberado(idLiberacao.ToString()) == 0 ? (int)NotaFiscal.FormaPagtoEnum.AVista :
                            idLiberacao > 0 && tipoPagtoLiberacao != null && tipoPagtoLiberacao <= 2 ? tipoPagtoLiberacao.Value :
                            peds.Length > 0 ? (peds[0].TipoVenda <= 3 ? peds[0].TipoVenda.Value : (int)NotaFiscal.FormaPagtoEnum.Outros) : (int)NotaFiscal.FormaPagtoEnum.Outros;
                        nf.ModalidadeFrete = ModalidadeFrete.SemTransporte; // Sem frete
                        if (!String.IsNullOrEmpty(nomeCidadeLoja)) nf.MunicOcor = nomeCidadeLoja;
                        nf.PeriodoApuracaoIpi = (int)FiscalConfig.NotaFiscalConfig.PeriodoApuracaoIpi;

                        nf.Consumidor = nfce;

                        if (FiscalConfig.NotaFiscalConfig.InformarOrcamentoNFe)
                        {
                            string idsOrcamentos = OrcamentoDAO.Instance.ObtemIdsOrcamento(idsPedidos);
                            if (!String.IsNullOrEmpty(idsOrcamentos)) nf.InfCompl = "Orçamento(s): " + idsOrcamentos + ". " + (nf.InfCompl != null ? nf.InfCompl : String.Empty);
                        }

                        // Busca a observação da NF-e do cadastro de clientes
                        nf.InfCompl = ClienteDAO.Instance.ObtemObsNfe(nf.IdCliente.Value);

                        if (FiscalConfig.NotaFiscalConfig.InformarPedidoClienteNFe)
                        {
                            var pedidos = idsPedidos.Split(',');
                            var inf = nf.InfCompl;

                            nf.InfCompl = null;

                            foreach (var idPedido in pedidos.Select(f => Glass.Conversoes.StrParaUint(f)))
                                nf.InfCompl += "SP: " + PedidoDAO.Instance.ObtemPedCli(idPedido) + "(NN: " + idPedido + "), ";

                            nf.InfCompl = nf.InfCompl.Trim().Trim(',') + ". " + (inf != null ? inf : String.Empty);
                        }
                        else if (FiscalConfig.NotaFiscalConfig.InformarPedidoNFe)
                        {
                            string textoPedidos = !FiscalConfig.NotaFiscalConfig.ExibirDescricaoPedidoInfCompl ? String.Empty : "Pedido(s): ";
                            nf.InfCompl = textoPedidos + idsPedidos + ". " + (nf.InfCompl != null ? nf.InfCompl : String.Empty);
                        }

                        bool gerarParcelas = false;

                        #region Informações de transporte

                        var tiposEntrega = idsPedidos.Split(',').Select(f => new
                        {
                            IdPedido = f.StrParaUint(),
                            TipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(f.StrParaUint())
                        })
                        .ToList();

                        //Se todos os pedidos forem do tipo entrega 'Balcão'
                        if (FiscalConfig.NotaFiscalConfig.PreencheTransporteSeBalcao && tiposEntrega.Where(f => f.TipoEntrega == (int)Pedido.TipoEntregaPedido.Balcao).Count() == tiposEntrega.Count)
                        {
                            nf.Especie = "VOLUMES";
                            nf.ModalidadeFrete = ModalidadeFrete.ContaDoDestinatario;
                            nf.QtdVol = Convert.ToInt32(PedidoDAO.Instance.GetPedidosForOC(idsPedidos, 0, false).Sum(f => f.QtdePecasVidro + f.QtdeVolume));
                        }

                        //Se todos os pedidos forem do tipo entrega 'Entrega'
                        if (FiscalConfig.NotaFiscalConfig.PreencheTransporteSeNaoForBalcao && tiposEntrega.Where(f => f.TipoEntrega != (int)Pedido.TipoEntregaPedido.Balcao).Count() == tiposEntrega.Count)
                        {
                            nf.Especie = "VOLUMES";
                            nf.ModalidadeFrete = ModalidadeFrete.ContaDoRemetente;
                            nf.QtdVol = Convert.ToInt32(PedidoDAO.Instance.GetPedidosForOC(idsPedidos, 0, false).Sum(f => f.QtdePecasVidro + f.QtdeVolume));
                            nf.VeicPlaca = transferencia && idCarregamento.GetValueOrDefault(0) > 0 ? CarregamentoDAO.Instance.ObtemPlaca(idCarregamento.Value) : "";
                            if (nf.IdTransportador.GetValueOrDefault() == 0)
                            {
                                var placa = idCarregamento.GetValueOrDefault(0) > 0 ? CarregamentoDAO.Instance.ObtemPlaca(idCarregamento.Value) : "";
                                nf.IdTransportador = !string.IsNullOrWhiteSpace(placa) ? (uint?)VeiculoDAO.Instance.ObtemValorCampo<int?>("IdTransportador", $"Placa='{placa}'") : null;
                            }
                        }

                        if (nf.IdTransportador.GetValueOrDefault(0) > 0)
                        {
                            if (LojaDAO.Instance.ObtemCnpj(nf.IdLoja.GetValueOrDefault(0)) == TransportadorDAO.Instance.ObtemValorCampo<string>("CpfCnpj", "IdTransportador = " + nf.IdTransportador.Value))
                                nf.ModalidadeFrete = ModalidadeFrete.ContaDoRemetente;
                            else
                                nf.ModalidadeFrete = ModalidadeFrete.ContaDoDestinatario;
                        }

                        if (FiscalConfig.NotaFiscalConfig.PreencheTransporteComVeiculoOC)
                        {
                            var veiculo = PedidoDAO.Instance.ObtemVeiculoCarregamento(idsPedidos);

                            if (!string.IsNullOrEmpty(veiculo.Key))
                            {
                                nf.Especie = FiscalConfig.NotaFiscalConfig.EspeciePadraoSeMesmoVeiculoOC;
                                nf.ModalidadeFrete = ModalidadeFrete.ContaDoRemetente;
                                nf.VeicPlaca = veiculo.Key;
                                nf.VeicUf = veiculo.Value;
                                nf.QtdVol = 1;
                            }
                        }

                        //Se tiver informado frete no pedido
                        if (totalFrete > 0)
                        {
                            nf.ModalidadeFrete = ModalidadeFrete.ContaDoDestinatario;//Destinatario
                            nf.ValorFrete = totalFrete;
                        }

                        #endregion

                        //Caso a informação complementar ultrapasse 1000 caracteres
                        if (nf.InfCompl != null && nf.InfCompl.Length > 1000)
                            nf.InfCompl = nf.InfCompl.Substring(0, 1000);

                        //Se a contigencia estiver habilitada cria a nota em modo de contigencia
                        if (FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.SCAN)
                        {
                            var uf = CidadeDAO.Instance.ObtemValorCampo<string>("NomeUf", "idCidade=" + nf.IdCidade);

                            nf.FormaEmissao = (int)ConfigNFe.ObtemTipoContingencia(uf);
                            nf.CodAleatorio = null;
                        }

                        idNf = Insert(transaction, nf);

                        #endregion

                        #region Associa liberações, pedidos e carregamento à nota

                        // Insere os pedidos
                        foreach (string idPedido in idsPedidos.Split(','))
                        {
                            if (String.IsNullOrEmpty(idPedido))
                                continue;

                            PedidosNotaFiscal pedNf = new PedidosNotaFiscal();
                            pedNf.IdNf = idNf;
                            pedNf.IdPedido = Glass.Conversoes.StrParaUint(idPedido);
                            PedidosNotaFiscalDAO.Instance.Insert(transaction, pedNf);
                        }

                        // Insere as liberações
                        foreach (string idLiberarPedido in idsLiberarPedidos.Split(','))
                        {
                            if (String.IsNullOrEmpty(idLiberarPedido))
                                continue;

                            PedidosNotaFiscal pedNf = new PedidosNotaFiscal();
                            pedNf.IdNf = idNf;
                            pedNf.IdLiberarPedido = Glass.Conversoes.StrParaUint(idLiberarPedido);
                            PedidosNotaFiscalDAO.Instance.Insert(transaction, pedNf);
                        }

                        if (transferencia && idCarregamento.GetValueOrDefault(0) > 0)
                        {
                            PedidosNotaFiscal pedNf = new PedidosNotaFiscal();
                            pedNf.IdNf = idNf;
                            pedNf.IdCarregamento = idCarregamento.Value;
                            PedidosNotaFiscalDAO.Instance.Insert(transaction, pedNf);
                        }

                        #endregion
                        
                        #region Busca o FCI caso necessário

                        foreach (var pp in lstProd)
                        {
                            //Verifica se utiliza FCI e não separa os produtos dos pedidos na nota
                            if (FiscalConfig.UtilizaFCI && !FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe && pp.IsChapaImportada)
                            {
                                // Caso o produto de pedido não tenha referência de produto de pedido espelho.
                                if (pp.IdProdPedEsp.GetValueOrDefault(0) == 0)
                                {

                                    string sql = @"
                                SELECT CAST(CONCAT(pnf.idProdNf, ';',pnf.cstOrig) as CHAR)
                                FROM produto_impressao pi 
                                    INNER JOIN produtos_nf pnf ON (pi.idProdNf = pnf.idProdNf)
                                    INNER JOIN nota_fiscal nf ON (pnf.idNf = nf.IdNf)
                                WHERE pi.cancelado = false AND nf.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.Saída + @"
                                    AND pi.idPedidoExpedicao=" + pp.IdPedido + @"
                                    AND pnf.idProd=" + pp.IdProd + @"
                                    AND cstOrig <> 0 AND cstOrig <> 4
                                GROUP BY pnf.idProdNf";

                                    var cstOrigFci = ExecuteMultipleScalar<string>(transaction, sql);

                                    //Verifica se a peça deu saída atraves de um produdo de nf
                                    if (cstOrigFci != null && cstOrigFci.Count > 0 && !string.IsNullOrEmpty(cstOrigFci[0]))
                                    {
                                        uint idProdNf = Glass.Conversoes.StrParaUint(cstOrigFci[0].Split(';')[0]);
                                        pp.CstOrig = Glass.Conversoes.StrParaInt(cstOrigFci[0].Split(';')[1]);

                                        var prodNf = ProdutosNfDAO.Instance.GetElement(idProdNf);
                                        if (prodNf != null && IsGuid(prodNf.NumControleFciStr))
                                            pp.NumControleFci = prodNf.NumControleFciStr;
                                    }
                                    else
                                    {
                                        //Calcula o conteudo de importação do produto
                                        var ci = ProdutosNfDAO.Instance.CalculaConteudoImportacao(pp.IdProd, DateTime.Now);

                                        if (ci > 0 && ci <= 40)
                                            pp.CstOrig = 5;
                                        else if (ci > 40 & ci <= 70)
                                            pp.CstOrig = 3;
                                        else if (ci > 70)
                                            pp.CstOrig = 8;

                                        var numControleFci = ProdutosArquivoFCIDAO.Instance.ObtemNumControleFci(pp.IdProd, ci).Split(';');
                                        if (numControleFci != null && numControleFci.Length > 0 && !string.IsNullOrEmpty(numControleFci[0]))
                                            pp.NumControleFci = numControleFci[0];
                                    }
                                }
                                else
                                {
                                    // Recupera todos os produtos de impressao associados ao produto de pedido espelho.
                                    string idsProdImpressao = ProdutoImpressaoDAO.Instance.ObtemValorCampo<string>(transaction, "Cast(Group_Concat(idProdImpressao) As Char)", "idProdPed=" + pp.IdProdPedEsp.Value);

                                    // Caso o produto de pedido não tenha referência de produto de impressão então passa para o próximo produto.
                                    if (String.IsNullOrEmpty(idsProdImpressao))
                                        continue;

                                    // Percorre toda a lista de produtos de impressão de pedido.
                                    foreach (string id in idsProdImpressao.Split(','))
                                    {
                                        // Recupera todos os produtos de impressao de chapa associados ao produto de impressão de pedido.
                                        string idsProdImpressaoChapa = ChapaCortePecaDAO.Instance.ObtemValorCampo<string>(transaction, "Cast(Group_Concat(idProdImpressaoChapa) As Char)", "idProdImpressaoPeca=" + id);

                                        // Caso o produto de pedido não tenha referência de produto de impressão de chapa então passa para o próximo produto, pois, este loop serve para identificar se o produto de chapa é nacional ou importado.
                                        if (String.IsNullOrEmpty(idsProdImpressaoChapa))
                                            continue;

                                        foreach (var idProdImpressaoChapa in idsProdImpressaoChapa.Split(','))
                                        {
                                            // Recupera o id do produto da nota fiscal associado ao produto de impressão de chapa.
                                            uint idProdNfChapa = ProdutoImpressaoDAO.Instance
                                                .ObtemValorCampo<uint>(transaction, "idProdNf", "idProdImpressao=" + idProdImpressaoChapa);

                                            var idPedido = ProdutoImpressaoDAO.Instance
                                                .ObtemValorCampo<uint>(transaction, "idPedido", "idProdImpressao=" + idProdImpressaoChapa);

                                            if (idProdNfChapa == 0 && idPedido != 0)
                                            {
                                                //Calcula o conteudo de importação do produto
                                                var ci = ProdutosNfDAO.Instance.CalculaConteudoImportacao(pp.IdProd, DateTime.Now);

                                                if (ci > 0 && ci <= 40)
                                                    pp.CstOrig = 5;
                                                else if (ci > 40 & ci <= 70)
                                                    pp.CstOrig = 3;
                                                else if (ci > 70)
                                                    pp.CstOrig = 8;

                                                var numControleFci = ProdutosArquivoFCIDAO.Instance.ObtemNumControleFci(pp.IdProd, ci).Split(';');
                                                if (numControleFci != null && numControleFci.Length > 0 && !string.IsNullOrEmpty(numControleFci[0]))
                                                    pp.NumControleFci = numControleFci[0];
                                            }
                                            else
                                            {
                                                var prodNfChapa = ProdutosNfDAO.Instance.GetElement(idProdNfChapa);
                                                pp.CstOrig = prodNfChapa.CstOrig;
                                                if (IsGuid(prodNfChapa.NumControleFciStr))
                                                    pp.NumControleFci = prodNfChapa.NumControleFciStr;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #region Cria o produto da nota

                        foreach (ProdutosPedido pp in lstProd)
                        {
                            uint? idProjetoModelo = !liberacaoParcial && FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe && pp.IdItemProjeto != null ? (uint?)ItemProjetoDAO.Instance.ObtemIdProjetoModelo(transaction, pp.IdItemProjeto.Value) : null;
                            uint? idProdParaNf = (idProjetoModelo != null && manterAgrupamentoDeProdutos) ? ProjetoModeloDAO.Instance.ObtemValorCampo<uint?>(transaction, "idProdParaNf", "idProjetoModelo=" + idProjetoModelo) : null;

                            if ((idProjetoModelo == null || idProdParaNf == null) && pp.Qtde <= 0)
                                continue;

                            // Caso o produto não tenha buscado a metragem quadrada correta do projeto, calcula novamente
                            if (idProdParaNf > 0 && pp.TotM < 1)
                            {
                                pp.TotM = UtilsProjeto.CalculaAreaVao(transaction, pp.IdItemProjeto.Value,
                                    ItemProjetoDAO.Instance.ObtemValorCampo<bool>("MedidaExata", "idItemProjeto=" + pp.IdItemProjeto));
                                pp.TotM2Calc = pp.TotM;
                            }

                            uint idProd = idProdParaNf.GetValueOrDefault(pp.IdProd);

                            Produto prod = ProdutoDAO.Instance.GetElement(transaction, idProd, idLoja, idCliente, null, true);

                            if (prod == null)
                                throw new Exception(string.Format("Um dos produtos dos pedidos não existe. IdProd: {0}", idProd));

                            if (prod.Ncm != null && prod.Ncm.Length > 8)
                                throw new Exception("O tamanho do campo NCM do produto " + prod.CodInterno + " - " + prod.Descricao + ", não pode ter mais que 8 caracteres.");

                            // Busca o CST origem padrão configurado, caso seja nota de saída
                            int cstOrig = FiscalConfig.NotaFiscalConfig.CstOrigPadraoNotaFiscalSaida;
                            if (cstOrig == 0)
                                cstOrig = prod.Descricao.ToLower().Contains("importado") ? 1 : 0;

                            ProdutosNf prodNf = new ProdutosNf();
                            prodNf.IdNaturezaOperacao = dicProdPedNaturezaOperacao[pp.IdProdPed];

                            var cstNatOp = prodNf.IdNaturezaOperacao > 0 ? NaturezaOperacaoDAO.Instance.ObtemCstIcms(transaction, prodNf.IdNaturezaOperacao.Value) : null;

                            // Foi solicitado pelo Tayson que o produto de pedido gerado a partir de chapa importada tenha o CST de origem "5".
                            // Foi removida a solicitação pois a orig agora vem da nota de importação
                            prodNf.CstOrig = !pp.IsChapaImportada ? cstOrig : pp.CstOrig;
                            prodNf.Cst = !string.IsNullOrEmpty(cstNatOp) ? cstNatOp : prod.Cst;

                            // Chamado 46155.
                            // O CST do produto e o IDCFOP devem ser informados para que ao chamar as propriedades AliqICMSInterna e AliqICMSInternaComIpiNoCalculo,
                            // da model Produto, o sistema consiga recuperar o percentual de carga tributária corretamente (cálculo diferenciado para o estado MT).
                            prod.ProdutoNfCst = prodNf.Cst;
                            prod.IdCfop = (int)NaturezaOperacaoDAO.Instance.ObtemIdCfop(transaction, prodNf.IdNaturezaOperacao.Value);

                            int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(transaction, prod.IdGrupoProd, prod.IdSubgrupoProd, true);

                            // Recalcula as medidas dos alumínios para que o tamanho cobrado seja exato e o valor na nota fique correto
                            if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 ||
                                tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6)
                            {
                                float alturaCalc = pp.Altura;
                                CalculosFluxo.ArredondaAluminio(tipoCalc, pp.Altura, ref alturaCalc);
                                pp.Altura = alturaCalc;
                            }

                            prodNf.IdNf = idNf;
                            prodNf.IdProd = idProd;
                            prodNf.Qtde = pp.Qtde;

                            prodNf.Altura =
                                idProdParaNf != null ?
                                    0 :
                                    pp.Altura > 0 ?
                                        pp.Altura :
                                        prod.TipoCalculo == (int)TipoCalculoGrupoProd.Qtd && prod.Altura > 0 ?
                                            prod.Altura.Value :
                                            0;

                            prodNf.Largura =
                                idProdParaNf != null ?
                                    0 :
                                    pp.Largura > 0 ?
                                        pp.Largura :
                                        prod.TipoCalculo == (int)TipoCalculoGrupoProd.Qtd && prod.Largura > 0 ?
                                            prod.Largura.Value :
                                            0;

                            prodNf.TotM = FiscalConfig.NotaFiscalConfig.ConsiderarM2CalcNotaFiscal ? pp.TotM2Calc : pp.TotM;
                            prodNf.NumControleFciStr = pp.NumControleFci;
                            prodNf.CstIpi = (int?)prod.CstIpi;

                            prodNf.Csosn = prod.Csosn;
                            prodNf.AliqIcms = IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(transaction, (uint)prod.IdProd, idLoja, null, idCliente);
                            prodNf.AliqIcmsSt = prod.AliqIcmsStInterna;
                            prodNf.AliqFcp = IcmsProdutoUfDAO.Instance.ObterFCPPorProduto(transaction, (uint)prod.IdProd, idLoja, null, idCliente);
                            prodNf.AliqFcpSt = IcmsProdutoUfDAO.Instance.ObterAliquotaFCPSTPorProduto(transaction, (uint)prod.IdProd, idLoja, null, idCliente);
                            prodNf.AliqIpi = prod.AliqIPI;
                            prodNf.IsChapaImportada = pp.IsChapaImportada;

                            #region CSTs por Natureza de Operação

                            if (prodNf.IdNaturezaOperacao > 0)
                            {
                                var nat = NaturezaOperacaoDAO.Instance.ObtemElemento(transaction, (int)prodNf.IdNaturezaOperacao.Value);

                                if (nat != null)
                                {
                                    prodNf.Csosn = nat.Csosn ?? prodNf.Csosn;
                                    prodNf.Cst = nat.CstIcms ?? prodNf.Cst;
                                    prodNf.PercRedBcIcms = nat.PercReducaoBcIcms;
                                    prodNf.PercDiferimento = nat.PercDiferimento;
                                    prodNf.CstIpi = (int?)(nat.CstIpi ?? ConfigNFe.CstIpi(idNf, nat.CodCfop));
                                    prodNf.CstPis = nat.CstPisCofins ?? ConfigNFe.CstPisCofins(idNf);
                                    prodNf.CstCofins = nat.CstPisCofins ?? ConfigNFe.CstPisCofins(idNf);
                                }
                            }

                            #endregion

                            // Coloca a primeira natureza de operação de produto, caso a natureza de operação
                            // da NF-e não seja informada pelo usuário
                            if (nf.IdNaturezaOperacao == null && prodNf.IdNaturezaOperacao > 0)
                            {
                                nf.IdNaturezaOperacao = prodNf.IdNaturezaOperacao;
                                objPersistence.ExecuteCommand(transaction, "update nota_fiscal set idNaturezaOperacao=?n where idNf=" + idNf,
                                    new GDAParameter("?n", nf.IdNaturezaOperacao.Value));

                                var obsCfop = CfopDAO.Instance.GetObs(transaction, nf.IdCfop.Value);

                                if (!String.IsNullOrEmpty(obsCfop))
                                    objPersistence.ExecuteCommand(transaction, "Update nota_fiscal set infCompl=?infCompl Where idNf=" + nf.IdNf,
                                        new GDAParameter("?infCompl", obsCfop + ". " + nf.InfCompl));
                            }

                            #region Transferência de nota fiscal

                            if (FiscalConfig.NotaFiscalConfig.ExportarNotaFiscalOutroBD && transferirNf)
                            {
                                // Recupera, da loja destino, a natureza de operação da nota fiscal.
                                if (prodNf.IdNaturezaOperacao > 0)
                                {
                                    // Recupera os dados do CFOP e natureza de operação do sistema de origem.
                                    var idCfopOrigem = NaturezaOperacaoDAO.Instance.ObtemIdCfop(transaction, prodNf.IdNaturezaOperacao.Value);
                                    var codInternoCfopOrigem = CfopDAO.Instance.ObtemCodInterno(transaction, idCfopOrigem);
                                    var codInternoNatOpOrigem = NaturezaOperacaoDAO.Instance.ObtemCodigoInterno(transaction, prodNf.IdNaturezaOperacao.Value);

                                    // Recupera os dados de CFOP e natureza de operação do sistema de destino.
                                    var idCfopDestino = CfopDAO.Instance.ObtemIdCfop(sessionTransf, codInternoCfopOrigem);
                                    idNaturezaOperacaoDestinoProd = NaturezaOperacaoDAO.Instance.ObtemIdNatOpPorCfopCodInterno(sessionTransf, idCfopDestino, codInternoNatOpOrigem);

                                    #region Valida informações do CFOP e natureza de operação

                                    // Recupera o código interno do CFOP e da natureza de operação de destino.
                                    var codInternoCfopDestino = CfopDAO.Instance.ObtemCodInterno(sessionTransf, idCfopDestino);
                                    var codInternoNatOpDestino = NaturezaOperacaoDAO.Instance.ObtemCodigoInterno(sessionTransf, idNaturezaOperacaoDestinoProd);

                                    // Verifica se as informações no sistema de origem e destino são iguais.
                                    if ((codInternoCfopOrigem + " - " + codInternoNatOpOrigem).ToUpper() != (codInternoCfopDestino + " - " + codInternoNatOpDestino).ToUpper())
                                        throw new Exception("A natureza de operação selecionada é inválida, pois, não existe no sistema para onde a nota será exportada.");

                                    #endregion
                                }

                                if (idNaturezaOperacaoDestino == 0)
                                    idNaturezaOperacaoDestino = idNaturezaOperacaoDestinoProd;
                            }

                            #endregion

                            var ncmNaturezaOp = prodNf.IdNaturezaOperacao > 0 ? NaturezaOperacaoDAO.Instance.ObtemNcm(transaction, prodNf.IdNaturezaOperacao.Value) : null;

                            prodNf.Ncm = !string.IsNullOrEmpty(ncmNaturezaOp) ? ncmNaturezaOp : prod.Ncm;
                            prodNf.Mva = MvaProdutoUfDAO.Instance.ObterMvaPorProduto(transaction, (int)prod.IdProd, idLoja, null, nf.IdCliente, true);
                            prodNf.IdContaContabil = (uint?)prod.IdContaContabil;

                            // Se for liberação (parcial), é necessário recalcular o total com base na qtd liberada.
                            // Foi acrescentado o !FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe para corrigir um problema de geração de nota (Dekor):
                            // valor trazido quando a nota fiscal é de liberação e utiliza o agrupamento de produtos já é o valor liberado
                            // Foi acrescentado o ControleSistema.GetSite() != ControleSistema.ClienteSistema.VidroCel pelo mesmo motivo da Dekor

                            // Soma o ValorDescontoQtde ao total devido ao rateio que é feito logo abaixo do desconto total do pedido 
                            // (Que inclui o desconto por qtd, o qual é rateado no total do produto)
                            prodNf.Total = !FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe && nfDeLiberacao && pp.QtdeOriginal > 0 && !Liberacao.DadosLiberacao.LiberarPedidoProdutos ?
                                (pp.Total + (!PedidoConfig.RatearDescontoProdutos ? pp.ValorDescontoQtdeNf : 0) + pp.ValorBenef) / (decimal)pp.QtdeOriginal * (decimal)pp.Qtde :
                                pp.Total + (!PedidoConfig.RatearDescontoProdutos ? pp.ValorDescontoQtdeNf : 0) + pp.ValorBenef;

                            // Informa a altura e largura do item nas observações dos produtos na nota
                            if (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd))
                            {
                                if (FiscalConfig.NotaFiscalConfig.ExibirLarguraEAlturaInfAdicProduto)
                                    prodNf.InfAdic = prodNf.Largura + "x" + prodNf.Altura;
                                else if (FiscalConfig.NotaFiscalConfig.ExibirQtdLarguraEAlturaInfAdicProduto && !FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe)
                                    prodNf.InfAdic = prodNf.Qtde + " - " + prodNf.Altura + "x" + prodNf.Largura;
                            }

                            //Informa o Ped. Cli do produto na informação complementar do produto da nota
                            if (FiscalConfig.NotaFiscalConfig.InformarPedidoClienteProdutoNFe && !string.IsNullOrEmpty(pp.PedCli))
                                prodNf.InfAdic += (string.IsNullOrEmpty(prodNf.InfAdic) ? "" : " ") + pp.PedCli;

                            // Verifica se o produto é calculado por ML e se a altura foi informada
                            if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML && prodNf.Altura == 0)
                            {
                                throw new Exception("O produto " + prod.Descricao + " é calculado por ML e tem seu comprimento no pedido " +
                                    "zerado. Corrija-o antes de prosseguir com a geração da nota fiscal");
                            }

                            // Aplica desconto do pedido no produto
                            if (!PedidoConfig.RatearDescontoProdutos && percDesc > 0)
                            {
                                // Calcula o percentual de desconto do produto, por pedido, somente se a configuração AgruparProdutosGerarNFe
                                // estiver desabilitada, dessa forma, é possível saber quais produtos deverão receber o desconto do pedido.
                                if (!FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe)
                                {
                                    // Aplica, primeiro, o percentual de desconto de quantidade do produto, pois, no pedido é feito dessa forma.
                                    if (produtosPedidoPercentualDescontoQtde.ContainsKey((int)pp.IdProdPed) && produtosPedidoPercentualDescontoQtde[(int)pp.IdProdPed] > 0)
                                        prodNf.Total = prodNf.Total - (prodNf.Total * (produtosPedidoPercentualDescontoQtde[(int)pp.IdProdPed] / 100));

                                    // Aplica o percentual de desconto do pedido no produto.
                                    if (pedidosPercentualDesconto.ContainsKey((int)pp.IdPedido) && pedidosPercentualDesconto[(int)pp.IdPedido] > 0)
                                        prodNf.Total = prodNf.Total - (prodNf.Total * pedidosPercentualDesconto[(int)pp.IdPedido]);
                                }
                                else
                                    prodNf.Total = prodNf.Total - (prodNf.Total * percDesc);
                            }

                            //Se for NF de oc de transferencia deve usar o valor de transferência do cadastro de produtos.
                            if (transferencia)
                            {
                                var valorProd = ProdutoDAO.Instance.ObtemValorTransferencia(transaction, prod.IdProd);
                                prodNf.Total = CalculosFluxo.CalcTotaisItemProdFast(tipoCalc, pp.Altura, pp.Largura,
                                    pp.Qtde, pp.TotM, valorProd);
                            }

                            // Verifica se o cliente possui redução de cálculo da Nfe
                            bool isVenda = ProdutoDAO.Instance.IsProdutoVenda(transaction, (int)prodNf.IdProd);

                            if (isVenda && percReducaoNfe > 0)
                                prodNf.Total = prodNf.Total - (prodNf.Total * ((decimal)percReducaoNfe / 100));
                            else if (!isVenda && percReducaoNfeRevenda > 0)
                                prodNf.Total = prodNf.Total - (prodNf.Total * ((decimal)percReducaoNfeRevenda / 100));

                            bool clienteCalcIpi = ClienteDAO.Instance.IsCobrarIpi(transaction, idCliente);
                            bool clienteCalcIcmsSt = ClienteDAO.Instance.IsCobrarIcmsSt(transaction, idCliente);
                            bool calcIpi = NaturezaOperacaoDAO.Instance.CalculaIpi(transaction, prodNf.IdNaturezaOperacao.Value);
                            bool calcIcmsSt = NaturezaOperacaoDAO.Instance.CalculaIcmsSt(transaction, prodNf.IdNaturezaOperacao.Value);
                            var idLojaPedido = PedidoDAO.Instance.ObtemIdLoja(pp.IdPedido);

                            // Retira a alíquota do IPI no total do produto (e do icms st para tempera e vidrometro)
                            if ((FiscalConfig.NotaFiscalConfig.RatearIpiNfPedido ||
                                FiscalConfig.NotaFiscalConfig.AliquotaIcmsStRatearIpiNfPedido != ConfigNFe.TipoCalculoIcmsStNf.NaoCalcular) &&
                                prod.AliqIPI > 0 && calcIpi)
                            {
                                decimal aliqIcms = calcIcmsSt ?
                                    (FiscalConfig.NotaFiscalConfig.AliquotaIcmsStRatearIpiNfPedido == ConfigNFe.TipoCalculoIcmsStNf.CalculoPadrao ? prod.AliqICMSInterna :
                                    FiscalConfig.NotaFiscalConfig.AliquotaIcmsStRatearIpiNfPedido == ConfigNFe.TipoCalculoIcmsStNf.AliquotaIcmsStComIpi ?
                                    prod.AliqICMSInternaComIpiNoCalculo : 0) : 0;

                                // Se o cliente estiver marcado para calcular icms st, zera a alíquota, pois não poderá rateá-la na nota
                                // Necessário para resolver o chamado 6806
                                if (LojaDAO.Instance.ObtemCalculaIcmsPedido(transaction, idLojaPedido) && clienteCalcIcmsSt)
                                    aliqIcms = 0;

                                if (!LojaDAO.Instance.ObtemCalculaIpiPedido(transaction, idLojaPedido) || !clienteCalcIpi)
                                {
                                    // Caso a alíquota de icms esteja zerada (automaticamente não considerando a diferença no cálculo do ICMS ST 
                                    // que a cobrança do IPI iria causar) e o cliente calcule ICMS ST no pedido considerando o IPI no cálculo
                                    // e o cliente esteja marcado para calcular ICMS no pedido mas não para calcular IPI no mesmo porém a nota 
                                    // calcule ICMS ST e IPI, é necessário recalcular este produto considerando a AliqICMSInterna junto com a 
                                    // alíquota do IPI e depois adicionando o valor da AliqICMSInterna somente
                                    bool recalcularIcmsStProd = aliqIcms == 0 && FiscalConfig.NotaFiscalConfig.CalculoAliquotaIcmsSt == ConfigNFe.TipoCalculoIcmsSt.ComIpiNoCalculo &&
                                        (!clienteCalcIpi || !LojaDAO.Instance.ObtemCalculaIpiPedido(transaction, idLojaPedido)) && clienteCalcIcmsSt && LojaDAO.Instance.ObtemCalculaIcmsPedido(transaction, idLojaPedido);

                                    if (recalcularIcmsStProd)
                                        aliqIcms = prod.AliqICMSInterna;

                                    // Rateia o valor dos impostos no total do produto
                                    prodNf.Total = prodNf.Total / (decimal)(1 + (((decimal)prod.AliqIPI + aliqIcms) / 100));

                                    if (recalcularIcmsStProd)
                                        prodNf.Total *= (decimal)(1 + (aliqIcms / 100));
                                }

                                // Recalcula os valores de ICMS e IPI se o pedido tiver calculado o IPI mas não o ICMS
                                else if ((!LojaDAO.Instance.ObtemCalculaIcmsPedido(transaction, idLojaPedido) || !clienteCalcIcmsSt) && calcIcmsSt &&
                                    FiscalConfig.NotaFiscalConfig.RatearIcmsStNfPedido)
                                {
                                    decimal aliqIcmsCalc = (decimal)IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(transaction, idProd, nf.IdLoja.Value, nf.IdFornec, nf.IdCliente);

                                    // Se tiver sido cobrado ipi no pedido e esteja tentando ratear o st na nota e  
                                    // a alíquota de icms seja diferente da alíquota de icms st, usa esta função
                                    if (pp.ValorIpiNf > 0 && (aliqIcmsCalc != (decimal)prod.AliqIcmsStInterna))
                                    {
                                        decimal mvaCalc = (decimal)MvaProdutoUfDAO.Instance.ObterMvaPorProduto(transaction, (int)idProd, nf.IdLoja.Value, (int?)nf.IdFornec, nf.IdCliente,
                                            (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída ||
                                            /* Chamado 32984 e 39660. */
                                            (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Entrada &&
                                            CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(transaction, prodNf.IdNaturezaOperacao.Value)))));

                                        var totalProd = pp.Total + pp.ValorDescontoQtdeNf + pp.ValorBenef;

                                        if (FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe)
                                            totalProd -= totalProd * percDesc;
                                        else
                                        {
                                            // Aplica, primeiro, o percentual de desconto de quantidade do produto, pois, no pedido é feito dessa forma.
                                            if (produtosPedidoPercentualDescontoQtde.ContainsKey((int)pp.IdProdPed) && produtosPedidoPercentualDescontoQtde[(int)pp.IdProdPed] > 0)
                                                totalProd -= totalProd * produtosPedidoPercentualDescontoQtde[(int)pp.IdProdPed];

                                            // Aplica o percentual de desconto do pedido no produto.
                                            if (pedidosPercentualDesconto.ContainsKey((int)pp.IdPedido) && pedidosPercentualDesconto[(int)pp.IdPedido] > 0)
                                                totalProd -= totalProd * pedidosPercentualDesconto[(int)pp.IdPedido];
                                        }

                                        // Calcula o icms st deste produto e depos divide pelo total do produto + ipi calculado no pedido,
                                        // para definir a alíquota que deverá ser somada ao produto da nota para que seja rateado corretamente o icms st
                                        var aliqIcmsStCalc =
                                            // Cálculo ICMS ST.
                                            ((totalProd + pp.ValorIpiNf) * (1 + (mvaCalc / 100)) * ((decimal)prod.AliqIcmsStInterna / 100) -

                                            // Cálculo ICMS.
                                            (totalProd * (aliqIcmsCalc / (decimal)100))) /

                                            (totalProd + pp.ValorIpiNf);

                                        prodNf.Total = prodNf.Total / (decimal)(1 + aliqIcmsStCalc);
                                    }
                                    else
                                    {
                                        prodNf.Total *= (decimal)(1 + (prod.AliqIPI / 100));
                                        prodNf.Total = prodNf.Total / (decimal)(1 + (((decimal)prod.AliqIPI + aliqIcms) / 100));
                                    }
                                }
                            }

                            // Retira o valor do icms st da nota, caso não tenha IPI mas tenha ICMS ST
                            else if ((!LojaDAO.Instance.ObtemCalculaIcmsPedido(transaction, idLojaPedido) || !clienteCalcIcmsSt) && calcIcmsSt &&
                                FiscalConfig.NotaFiscalConfig.RatearIcmsStNfPedido)
                            {
                                // Chamado 46155.
                                // O valor do IPI afeta o cálculo da propriedade AliqICMSInterna, portanto, caso o cálculo do IPI não deva ser feito
                                // é necessário que a alíquota seja zerada, senão, o cálculo é feito automaticamente.
                                // prod.AliqIPI = prodNf.AliqIpi;
                                prod.AliqIPI = calcIpi ? prodNf.AliqIpi : 0;
                                prodNf.AliqIpi = calcIpi ? prodNf.AliqIpi : 0;
                                prodNf.Total = prodNf.Total / (decimal)(1 + (prod.AliqICMSInterna / 100));
                            }

                            // Calcula o valor unitário
                            if (tipoCalc == (uint)Glass.Data.Model.TipoCalculoGrupoProd.Qtd || tipoCalc == (uint)Glass.Data.Model.TipoCalculoGrupoProd.QtdM2)
                                prodNf.ValorUnitario = prodNf.Total / (decimal)prodNf.Qtde;
                            else if (prodNf.TotM > 0)
                                prodNf.ValorUnitario = prodNf.Total / (decimal)prodNf.TotM;
                            else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                                prodNf.ValorUnitario = prodNf.Total / (decimal)(prodNf.Altura * prodNf.Qtde);
                            else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.Perimetro) // ML
                            {
                                var metroLinear = (prodNf.Altura * 2) + (prodNf.Largura * 2);
                                prodNf.ValorUnitario = prodNf.Total / (decimal)((metroLinear * prodNf.Qtde) / 1000F);
                            }
                            else if (prodNf.Altura > 0)
                                prodNf.ValorUnitario = (prodNf.Total * 6) / (decimal)(prodNf.Altura * prodNf.Qtde);
                            else
                                prodNf.ValorUnitario = prodNf.Total / (decimal)prodNf.Qtde;

                            // Rateia o valor do fast delivery na nota.
                            if (PedidoDAO.Instance.IsFastDelivery(transaction, pp.IdPedido))
                                prodNf.ValorUnitario *= (decimal)((PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery / 100) + 1);

                            if (prodNf.ValorUnitario.ToString().Contains("NaN"))
                                prodNf.ValorUnitario = 0;

                            // Calcula PIS e COFINS
                            prodNf.AliqPis = ConfigNFe.AliqPis(idLoja);
                            prodNf.BcPis = prodNf.Total;
                            prodNf.ValorPis = prodNf.BcPis * (decimal)prodNf.AliqPis / 100;

                            prodNf.AliqCofins = ConfigNFe.AliqCofins(idLoja);
                            prodNf.BcCofins = prodNf.Total;
                            prodNf.ValorCofins = prodNf.BcCofins * (decimal)prodNf.AliqCofins / 100;

                            // Salva o tipo de contribuição social e tipo de crédito (se necessário)
                            if (FiscalConfig.NotaFiscalConfig.TipoNotaBuscarContribuicaoSocialPadrao != DataSourcesEFD.TipoUsoCredCont.Entrada)
                                prodNf.CodCont = (int?)FiscalConfig.NotaFiscalConfig.TipoContribuicaoSocialPadrao;

                            if (FiscalConfig.NotaFiscalConfig.TipoNotaBuscarCreditoPadrao != DataSourcesEFD.TipoUsoCredCont.Entrada)
                                prodNf.CodCred = (int?)FiscalConfig.NotaFiscalConfig.TipoCreditoPadrao;

                            prodNf.CodValorFiscal = ObtemCodValorFiscal(transaction, nf.TipoDocumento, nf.IdLoja.GetValueOrDefault(), prodNf.Cst);

                            uint idProdNf = ProdutosNfDAO.Instance.Insert(transaction, prodNf);

                            //Se for transferir a nf de loja salva a natureza da operação da loja destino
                            dicNaturezaOperacaoProdDestino.Add(prodNf.IdProdNf, idNaturezaOperacaoDestinoProd);

                            // Insere os dados na tabela produto_nf_item_projeto
                            ProdutoNfItemProjetoDAO.Instance.DeleteByIdProdNf(idProdNf);
                            if (idProdParaNf > 0)
                                ProdutoNfItemProjetoDAO.Instance.Inserir(transaction, idProdNf, pp.IdsItemProjeto);

                            // Importa os beneficiamentos do produtoPedido para o produtoNf
                            ProdutoNfBenefDAO.Instance.ImportaProdPedBenef(transaction, pp.IdProdPed, idProdNf);
                        }

                        var totalNotaAjuste = ObtemTotal(transaction, idNf);
                        var totalPedidos = totalNota + totalIpiPedido + totalIcmsPedido - desconto;

                        var diferencaNotaPedidos = Math.Abs(totalNotaAjuste - totalPedidos);

                        // Chamado 16135: Recalcula valores da nota a fim do valor da mesma bater com a liberação
                        if (!transferencia && percReducaoNfe == 0 && percReducaoNfeRevenda == 0 && totalNotaAjuste != totalPedidos && diferencaNotaPedidos < 0.10m)
                        {
                            // Busca um produto qualquer na nota e modifica seu valor unitário a fim de aproximar ao valor que deve ficar
                            var lstProdNfAjuste = ProdutosNfDAO.Instance.GetByNf(transaction, idNf);

                            // Usado no ajuste do valor, quanto maior, mais iterações são necessárias para ajustar o valor.
                            var divisor = 50000;

                            var iteracoes = 10;

                            for (var j = 0; j < lstProdNfAjuste.Length; j++)
                            {
                                var i = 0;
                                for (i = 1; i <= iteracoes; i++)
                                {
                                    // Se o total já tiver ficado igual, não precisa ajustar mais
                                    if (Math.Round(totalNotaAjuste = ObtemTotal(transaction, idNf), 2) == Math.Round(totalPedidos, 2))
                                        break;

                                    var valorAjuste = lstProdNfAjuste[j].ValorUnitario / (divisor + (i * 5000));

                                    lstProdNfAjuste[j].ValorUnitario += (totalNotaAjuste > totalPedidos ? -valorAjuste : valorAjuste);
                                    ProdutosNfDAO.Instance.Update(transaction, lstProdNfAjuste[j]);
                                }

                                // Caso o i seja menor que 7, significa que o valor já está igual, portanto pode sair do for
                                // Caso já esteja no quinto produto, não tenta ajustar mais, para não travar muito o processo
                                if (i <= iteracoes || j >= 5)
                                    break;
                            }
                        }

                        #endregion

                        #region Informações de Pagamento

                        CriarPagtoNotaFiscalConsumidor(transaction, idNf, idsLiberarPedidos, nfce, totalNotaAjuste);

                        #endregion

                        #region Gera parcelas da nota

                        nf = GetElement(transaction, idNf);

                        // Se o idLiberacao buscado a partir dos pedidos estiver nulo ou vazio, verifica se foi passada liberação por parâmetro,
                        // para preencher corretamente as parcelas
                        if (PedidoConfig.LiberarPedido && (idLiberacao == null || idLiberacao == 0) && !String.IsNullOrEmpty(idsLiberarPedidos))
                            idLiberacao = Glass.Conversoes.StrParaUint(idsLiberarPedidos.Split(',')[0]);

                        if (PedidoConfig.LiberarPedido && idLiberacao > 0)
                        {
                            var plp = PagtoLiberarPedidoDAO.Instance.GetByLiberacao(transaction, idLiberacao.Value).ToArray();

                            if (nf.FormaPagto != (int)NotaFiscal.FormaPagtoEnum.AVista && plp.Length > 0)
                            {
                                if (LiberarPedidoDAO.Instance.ObtemValorCampo<int>(transaction, "tipoPagto", "idLiberarPedido=" + idLiberacao) == (int)LiberarPedido.TipoPagtoEnum.APrazo)
                                {
                                    nf.FormaPagto = 2;
                                    var cr = ContasReceberDAO.Instance.GetByLiberacaoPedido(transaction, idLiberacao.Value, true).ToArray();
                                    gerarParcelas = cr.Length > 0;

                                    nf.NumParc = cr.Length;
                                    nf.DatasParcelas = new DateTime[nf.NumParc.Value];
                                    nf.ValoresParcelas = new decimal[nf.NumParc.Value];

                                    if (nf.NumParc > 0)
                                    {
                                        decimal valorParc = nf.TotalNota / nf.NumParc.Value;

                                        for (int i = 0; i < nf.NumParc; i++)
                                        {
                                            nf.DatasParcelas[i] = cr[i].DataVec;
                                            nf.ValoresParcelas[i] = Decimal.Round(valorParc, 2);
                                        }

                                        if (nf.ValoresParcelas.Sum(f => f) > nf.TotalNota)
                                            nf.ValoresParcelas[0] = nf.ValoresParcelas[0] - (nf.ValoresParcelas.Sum(f => f) - nf.TotalNota);
                                        if (nf.ValoresParcelas.Sum(f => f) < nf.TotalNota)
                                            nf.ValoresParcelas[0] = nf.ValoresParcelas[0] + (nf.TotalNota - nf.ValoresParcelas.Sum(f => f));
                                    }
                                }
                            }
                        }
                        else if (idsPedidos.Split(',').Length == 1)
                        {
                            if (peds[0].TipoVenda == (int)Pedido.TipoVendaPedido.APrazo)
                            {
                                nf.FormaPagto = 2;
                                var cr = ContasReceberDAO.Instance.GetByPedido(transaction, Glass.Conversoes.StrParaUint(idsPedidos), false, true).ToArray();
                                gerarParcelas = cr.Length > 0;

                                // Não gerar parcelas na NF se o pedido for de data antiga
                                if (gerarParcelas)
                                {
                                    nf.NumParc = cr.Length;
                                    nf.DatasParcelas = new DateTime[nf.NumParc.Value];
                                    nf.ValoresParcelas = new decimal[nf.NumParc.Value];

                                    decimal valorParc = nf.TotalNota / (nf.NumParc > 0 ? nf.NumParc.Value : 1);

                                    for (int i = 0; i < nf.NumParc; i++)
                                    {
                                        nf.DatasParcelas[i] = cr[i].DataVec;
                                        nf.ValoresParcelas[i] = Decimal.Round(valorParc, 2);
                                    }

                                    if (nf.ValoresParcelas.Sum(f => f) > nf.TotalNota)
                                        nf.ValoresParcelas[0] = nf.ValoresParcelas[0] - (nf.ValoresParcelas.Sum(f => f) - nf.TotalNota);
                                    if (nf.ValoresParcelas.Sum(f => f) < nf.TotalNota)
                                        nf.ValoresParcelas[0] = nf.ValoresParcelas[0] + (nf.TotalNota - nf.ValoresParcelas.Sum(f => f));
                                }
                            }
                        }

                        // Se o número de parcelas da nota for 0, atualizar como 1
                        if (nf.NumParc == 0) nf.NumParc = 1;

                        Update(transaction, nf);

                        if (!gerarParcelas)
                            ParcelaNfDAO.Instance.DeleteFromNf(transaction, idNf);

                        #endregion

                        #region Transferência NF-e

                        // Chamado 12747. Caso ocorra algum problema na geração da nota fiscal a mesma não deve ser transferida, por isso,
                        // a transferência deve ser feita dentro do "try - catch".
                        //Se for ms vidros transfere a nf para outro banco e apaga a original.

                        if (FiscalConfig.NotaFiscalConfig.ExportarNotaFiscalOutroBD && transferirNf)
                            return TransfereNFeBanco(transaction, idNf, idNaturezaOperacaoDestino, dicNaturezaOperacaoProdDestino);

                        #endregion

                        if (nf.TotalNota != (decimal)LiberarPedidoDAO.Instance.GetTotalLiberado(idsLiberarPedidos) && FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe)
                        {
                            if(PedidoDAO.Instance.VerificarPedidoPossuiIcmsEDesconto(idsPedidos))
                            {
                                var descricao = @"A diferença entre os valores da nota e da(s) liberação(ões) se dá por um ou mais pedidos das liberações possuem Icms e Desconto além da configuração Agrupar produtos do(s) pedido(s) ao gerar NF-e estar marcada";

                                LogNfDAO.Instance.NewLog(transaction, nf.IdNf, "Geração", 0, descricao);
                            }
                        }

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException("GerarNotaFiscal NF: " + nf.NumeroNFe, ex);

                        throw;
                    }
                }
            }
            finally
            {
                FilaOperacoes.NotaFiscalInserir.ProximoFila();
            }

            return idNf;
        }

        /// <summary>
        /// Cria o PagtoNotaFiscal para a NFe ou NFCe.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNf"></param>
        /// <param name="idsLiberarPedidos"></param>
        /// <param name="nfce"></param>
        /// <param name="totalNotaAjuste"></param>
        private static void CriarPagtoNotaFiscalConsumidor(GDATransaction sessao, uint idNf, string idsLiberarPedidos, bool nfce, decimal totalNotaAjuste)
        {
            if (nfce && FiscalConfig.TelaCadastro.FormaPagtoPadraoNFCe.HasValue)
            {
                var pagamentoNotaFiscal = new PagtoNotaFiscal();
                pagamentoNotaFiscal.IdNf = (int)idNf;
                pagamentoNotaFiscal.FormaPagto = (int)FiscalConfig.TelaCadastro.FormaPagtoPadraoNFCe;
                pagamentoNotaFiscal.Valor = totalNotaAjuste;

                PagtoNotaFiscalDAO.Instance.Insert(sessao, pagamentoNotaFiscal);
            }
            // Define o valor recebido e a forma de pagamento da NFe ou NFCe de acordo com a forma de pagamento da liberação
            else if (!string.IsNullOrEmpty(idsLiberarPedidos))
            {
                decimal totalDinheiro = 0, totalCheque = 0, totalCreditoLoja = 0, totalBoleto = 0, totalOutros = 0;
                // Busca as formas de pagamento de todas as liberações
                foreach (string id in idsLiberarPedidos.Split(','))
                {
                    if (string.IsNullOrEmpty(id))
                        continue;

                    // Busca as formas de pagamento pelo id da liberação
                    var formaPgtoLiberacao = PagtoLiberarPedidoDAO.Instance.GetByLiberacao(sessao, Glass.Conversoes.StrParaUint(id));
                    foreach (var formaPgto in formaPgtoLiberacao)
                    {
                        switch (formaPgto.IdFormaPagto)
                        {
                            case (uint)Pagto.FormaPagto.Dinheiro:
                                totalDinheiro += formaPgto.ValorPagto;
                                break;
                            case (uint)Pagto.FormaPagto.ChequeProprio:
                            case (uint)Pagto.FormaPagto.ChequeTerceiro:
                                totalCheque += formaPgto.ValorPagto;
                                break;
                            case (uint)Pagto.FormaPagto.Credito:
                                totalCreditoLoja += formaPgto.ValorPagto;
                                break;
                            case (uint)Pagto.FormaPagto.Boleto:
                                totalBoleto += formaPgto.ValorPagto;
                                break;
                            // Se a liberação tiver sido paga com cartão
                            case (uint)Pagto.FormaPagto.Cartao:
                            case (uint)Pagto.FormaPagto.CartaoNaoIdentificado:
                                // Verifica se foi cartão de Debito e insere esse tipo na forma de pagamento
                                if (formaPgto.IdTipoCartao != null && TipoCartaoCreditoDAO.Instance.ObterTipoCartao(sessao, (int)formaPgto.IdTipoCartao) == TipoCartaoEnum.Debito)
                                    PagtoNotaFiscalDAO.Instance.Insert(sessao, new PagtoNotaFiscal
                                    {
                                        IdNf = (int)idNf,
                                        FormaPagto = (int)FormaPagtoNotaFiscalEnum.CartaoDebito,
                                        Valor = formaPgto.ValorPagto,
                                        NumAut = formaPgto.NumAutCartao
                                    });
                                // Verifica se foi cartão de Crédito e insere esse tipo na forma de pagamento
                                else if (formaPgto.IdTipoCartao != null && TipoCartaoCreditoDAO.Instance.ObterTipoCartao(sessao, (int)formaPgto.IdTipoCartao) == TipoCartaoEnum.Credito)
                                    PagtoNotaFiscalDAO.Instance.Insert(sessao, new PagtoNotaFiscal
                                    {
                                        IdNf = (int)idNf,
                                        FormaPagto = (int)FormaPagtoNotaFiscalEnum.CartaoCredito,
                                        Valor = formaPgto.ValorPagto,
                                        NumAut = formaPgto.NumAutCartao
                                    });
                                break;
                            // Se a forma de pagamento da liberação não estiver definida acima
                            // Insere como outros por padrão
                            default:
                                totalOutros += formaPgto.ValorPagto;
                                break;
                        }
                    }
                }
                // Insere o valor total somado por nota.
                if (totalDinheiro > 0)
                    PagtoNotaFiscalDAO.Instance.Insert(sessao, new PagtoNotaFiscal
                    {
                        IdNf = (int)idNf,
                        FormaPagto = (int)FormaPagtoNotaFiscalEnum.Dinheiro,
                        Valor = totalDinheiro
                    });
                if (totalCheque > 0)
                    PagtoNotaFiscalDAO.Instance.Insert(sessao, new PagtoNotaFiscal
                    {
                        IdNf = (int)idNf,
                        FormaPagto = (int)FormaPagtoNotaFiscalEnum.Cheque,
                        Valor = totalCheque
                    });
                if (totalCreditoLoja > 0)
                    PagtoNotaFiscalDAO.Instance.Insert(sessao, new PagtoNotaFiscal
                    {
                        IdNf = (int)idNf,
                        FormaPagto = (int)FormaPagtoNotaFiscalEnum.CreditoLoja,
                        Valor = totalCreditoLoja
                    });
                if (totalBoleto > 0)
                    PagtoNotaFiscalDAO.Instance.Insert(sessao, new PagtoNotaFiscal
                    {
                        IdNf = (int)idNf,
                        FormaPagto = (int)FormaPagtoNotaFiscalEnum.BoletoBancario,
                        Valor = totalBoleto
                    });
                if (totalOutros > 0)
                    PagtoNotaFiscalDAO.Instance.Insert(sessao, new PagtoNotaFiscal
                    {
                        IdNf = (int)idNf,
                        FormaPagto = (int)FormaPagtoNotaFiscalEnum.Outros,
                        Valor = totalOutros
                    });
            }
        }

        #endregion

        #region Gera NF a partir de compra

        /// <summary>
        /// Gera uma NF a partir de uma compra.
        /// </summary>
        public uint GerarNfCompra(GDASession session, uint idCompra, uint? idNaturezaOperacao, IEnumerable<ProdutosCompra> prodCompra)
        {
            var where = "idCompra=" + idCompra;
            var idFornec = CompraDAO.Instance.ObtemValorCampo<uint>(session, "idFornec", where);
            var idLoja = CompraDAO.Instance.ObtemValorCampo<uint>(session, "idLoja", where);
            var tipoCompra = CompraDAO.Instance.ObtemValorCampo<int>(session, "tipoCompra", where);
            var idConta = CompraDAO.Instance.ObtemValorCampo<uint>(session, "idConta", where);
            var nf = CompraDAO.Instance.ObtemValorCampo<uint>(session, "nf", where);
            var dataCadastro = CompraDAO.Instance.ObtemValorCampo<DateTime?>(session, "dataCad", where);

            return GerarNfCompra(session, new[] { idCompra }, idFornec, idNaturezaOperacao, null, idLoja, tipoCompra,
                idConta, nf, dataCadastro, prodCompra);
        }

        /// <summary>
        /// Gera uma NF a partir de uma compra.
        /// </summary>
        public uint GerarNfCompraComTransacao(uint idCompra, uint? idNaturezaOperacao, IEnumerable<ProdutosCompra> prodCompra)
        {
            FilaOperacoes.GerarNfCompra.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var where = "idCompra=" + idCompra;
                    var idFornec = CompraDAO.Instance.ObtemValorCampo<uint>(transaction, "idFornec", where);
                    var idLoja = CompraDAO.Instance.ObtemValorCampo<uint>(transaction, "idLoja", where);
                    var tipoCompra = CompraDAO.Instance.ObtemValorCampo<int>(transaction, "tipoCompra", where);
                    var idConta = CompraDAO.Instance.ObtemValorCampo<uint>(transaction, "idConta", where);
                    var nf = CompraDAO.Instance.ObtemValorCampo<uint>(transaction, "nf", where);
                    var dataCadastro = CompraDAO.Instance.ObtemValorCampo<DateTime?>(transaction, "dataCad", where);

                    var retorno = GerarNfCompra(transaction, new[] { idCompra }, idFornec, idNaturezaOperacao, null, idLoja, tipoCompra,
                        idConta, nf, dataCadastro, prodCompra);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
                finally
                {
                    FilaOperacoes.GerarNfCompra.ProximoFila();
                }
            }
        }

        /// <summary>
        /// Gera uma NF a partir de uma compra.
        /// </summary>
        public uint GerarNfCompraComTransacao(IEnumerable<uint> idsCompras, uint idFornec, uint? idNaturezaOperacao, Dictionary<uint, uint> naturezasOperacao,
            uint idLoja, int tipoCompra, uint idConta, uint numeroNFe, DateTime? dataCadastro, IEnumerable<ProdutosCompra> produtosCompra)
        {
            FilaOperacoes.GerarNfCompra.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();
                    
                    var retorno = GerarNfCompra(transaction, idsCompras, idFornec, idNaturezaOperacao, naturezasOperacao, idLoja,
                        tipoCompra, idConta, numeroNFe, dataCadastro, produtosCompra);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
                finally
                {
                    FilaOperacoes.GerarNfCompra.ProximoFila();
                }
            }
        }

        /// <summary>
        /// Gera uma NF a partir de uma compra.
        /// </summary>
        public uint GerarNfCompra(GDASession sessao, IEnumerable<uint> idsCompras, uint idFornec, uint? idNaturezaOperacao, Dictionary<uint, uint> naturezasOperacao,
            uint idLoja, int tipoCompra, uint idConta, uint numeroNFe, DateTime? dataCadastro, IEnumerable<ProdutosCompra> produtosCompra)
        {
            if (idsCompras == null || idsCompras.Count() == 0)
                throw new ArgumentException("Argumento vazio.", "idsCompras");

            var compras = CompraDAO.Instance.GetByString(sessao, string.Join(",", idsCompras.Select(x => x.ToString()).ToArray()));

            decimal desconto = 0;

            foreach (var comp in compras)
            {
                desconto += comp.Desconto;

                if (comp.Situacao != Compra.SituacaoEnum.Finalizada && comp.Situacao != Compra.SituacaoEnum.EmAndamento)
                    throw new Exception("Essa compra ainda não foi finalizada.");
            }
            
            uint? cidadeLoja = FornecedorDAO.Instance.ObtemValorCampo<uint?>(sessao, "idCidade", "idFornec=" + idFornec);

            // Verifica se a cidade do fornecedor foi informada
            if (cidadeLoja == null)
                throw new Exception("A cidade do fornecedor que a compra foi feita não foi informada.");

            string nomeCidadeLoja = CidadeDAO.Instance.ObtemValorCampo<string>(sessao, "nomeCidade", "idCidade=" + cidadeLoja.Value);

            dataCadastro = dataCadastro ?? DateTime.Now;

            NotaFiscal nf = new NotaFiscal();
            nf.IdFornec = idFornec;
            nf.IdLoja = idLoja;
            nf.IdCidade = cidadeLoja.Value;
            nf.IdConta = idConta;
            nf.NumeroNFe = numeroNFe;
            nf.Situacao = (int)NotaFiscal.SituacaoEnum.Aberta;
            nf.TipoDocumento = (int)NotaFiscal.TipoDoc.EntradaTerceiros;
            nf.DataSaidaEnt = dataCadastro.Value;
            nf.IdNaturezaOperacao = idNaturezaOperacao;
            nf.DataEmissao = dataCadastro.Value;
            nf.FormaPagto = tipoCompra;
            if (!String.IsNullOrEmpty(nomeCidadeLoja)) nf.MunicOcor = nomeCidadeLoja;
            nf.ModalidadeFrete = ModalidadeFrete.ContaDoDestinatario; // Por conta do destinatário
            nf.PeriodoApuracaoIpi = (int)FiscalConfig.NotaFiscalConfig.PeriodoApuracaoIpi;
            nf.OutrasDespesas = compras.Sum(x => x.OutrasDespesas);
            nf.GerarContasPagar = FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar;
            nf.Desconto = desconto;
            // Chamado 11778.
            nf.GerarEtiqueta = FiscalConfig.NotaFiscalConfig.GerarNotaFiscalCompraGerarEtiqueta;

            bool gerarParcelas = false;

            uint idNf = Insert(sessao, nf);

            #region Informações de Pagamento

            var idFormaPagto = compras[0].IdFormaPagto;
            var totalPago = compras.Sum(c => c.Total);

            switch (idFormaPagto)
            {
                case (uint)Pagto.FormaPagto.Dinheiro:
                    if (totalPago > 0)
                        PagtoNotaFiscalDAO.Instance.Insert(sessao, new PagtoNotaFiscal
                        {
                            IdNf = (int)idNf,
                            FormaPagto = (int)FormaPagtoNotaFiscalEnum.Dinheiro,
                            Valor = totalPago
                        });
                    break;
                case (uint)Pagto.FormaPagto.ChequeProprio:
                case (uint)Pagto.FormaPagto.ChequeTerceiro:
                    if (totalPago > 0)
                        PagtoNotaFiscalDAO.Instance.Insert(sessao, new PagtoNotaFiscal
                        {
                            IdNf = (int)idNf,
                            FormaPagto = (int)FormaPagtoNotaFiscalEnum.Cheque,
                            Valor = totalPago
                        });
                    break;
                case (uint)Pagto.FormaPagto.Boleto:
                    if (totalPago > 0)
                        PagtoNotaFiscalDAO.Instance.Insert(sessao, new PagtoNotaFiscal
                        {
                            IdNf = (int)idNf,
                            FormaPagto = (int)FormaPagtoNotaFiscalEnum.BoletoBancario,
                            Valor = totalPago
                        });
                    break;
                // Se a forma de pagamento não estiver definida acima
                // Insere como outros por padrão
                default:
                    if (totalPago > 0)
                        PagtoNotaFiscalDAO.Instance.Insert(sessao, new PagtoNotaFiscal
                        {
                            IdNf = (int)idNf,
                            FormaPagto = (int)FormaPagtoNotaFiscalEnum.Outros,
                            Valor = totalPago
                        });
                    break;
            }

            #endregion

            #region Insere as compras

            foreach (var id in idsCompras)
            {
                var cnf = new CompraNotaFiscal()
                {
                    IdNf = idNf,
                    IdCompra = id
                };

                CompraNotaFiscalDAO.Instance.Insert(sessao, cnf);
            }

            #endregion

            #region Cria o produto da nota

            foreach (ProdutosCompra pc in produtosCompra)
            {
                Produto prod = ProdutoDAO.Instance.GetElement(sessao, pc.IdProd, idLoja, null, idFornec, false);
                int tipoCalc = GrupoProdDAO.Instance.TipoCalculo(sessao, prod.IdGrupoProd, prod.IdSubgrupoProd, true);

                // Recalcula as medidas dos alumínios para que o tamanho cobrado seja exato e o valor na nota fique correto
                if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 ||
                    tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6)
                {
                    float alturaCalc = pc.Altura;
                    CalculosFluxo.ArredondaAluminio(tipoCalc, pc.Altura, ref alturaCalc);
                    pc.Altura = alturaCalc;
                }

                ProdutosNf prodNf = new ProdutosNf();
                prodNf.IdNf = idNf;
                prodNf.IdProd = pc.IdProd;
                prodNf.DescricaoItemGenerico = pc.DescricaoItemGenerico;
                prodNf.Qtde = pc.Qtde;
                prodNf.ValorUnitario = pc.Valor;
                prodNf.Altura = pc.Altura;
                prodNf.Largura = pc.Largura;
                prodNf.TotM = pc.TotM;
                prodNf.Csosn = prod.Csosn;
                prodNf.AliqIcms = IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(sessao, pc.IdProd, idLoja, idFornec, null);
                prodNf.AliqIcmsSt = prod.AliqIcmsStInterna;
                prodNf.AliqFcp = IcmsProdutoUfDAO.Instance.ObterFCPPorProduto(sessao, pc.IdProd, idLoja, idFornec, null);
                prodNf.AliqFcpSt = IcmsProdutoUfDAO.Instance.ObterAliquotaFCPSTPorProduto(sessao, pc.IdProd, idLoja, idFornec, null);
                prodNf.AliqIpi = prod.AliqIPI;

                uint dicNaturezaOperacao;
                if (naturezasOperacao != null && naturezasOperacao.TryGetValue(prodNf.IdProd, out dicNaturezaOperacao))
                    prodNf.IdNaturezaOperacao = dicNaturezaOperacao;
                else
                {
                    // Busca a natureza de operação pela regra, se houver
                    prodNf.IdNaturezaOperacao = idNaturezaOperacao ?? RegraNaturezaOperacaoDAO.Instance.BuscaNaturezaOperacao(sessao, nf.IdNf, nf.IdLoja, nf.IdCliente, (int)prodNf.IdProd) ?? nf.IdNaturezaOperacao;
                }

                // Coloca a primeira natureza de operação de produto, caso a natureza de operação
                // da NF-e não seja informada pelo usuário
                if (nf.IdNaturezaOperacao == null && prodNf.IdNaturezaOperacao > 0)
                {
                    nf.IdNaturezaOperacao = prodNf.IdNaturezaOperacao;
                    objPersistence.ExecuteCommand(sessao, "update nota_fiscal set idNaturezaOperacao=?n where idNf=" + idNf,
                        new GDAParameter("?n", nf.IdNaturezaOperacao.Value));
                }

                string cstNatOp = prodNf.IdNaturezaOperacao > 0 ? NaturezaOperacaoDAO.Instance.ObtemCstIcms(sessao, prodNf.IdNaturezaOperacao.Value) : null;
                float percRedBcIcms = prodNf.IdNaturezaOperacao > 0 ? NaturezaOperacaoDAO.Instance.ObtemPercReducaoBcIcms(sessao, prodNf.IdNaturezaOperacao.Value) : 0;
                var ncmNaturezaOp = prodNf.IdNaturezaOperacao > 0 ? NaturezaOperacaoDAO.Instance.ObtemNcm(sessao, prodNf.IdNaturezaOperacao.Value) : null;

                prodNf.Ncm = !string.IsNullOrEmpty(ncmNaturezaOp) ? ncmNaturezaOp : prod.Ncm;
                prodNf.Cst = !String.IsNullOrEmpty(cstNatOp) ? cstNatOp : prod.Cst;
                prodNf.PercRedBcIcms = percRedBcIcms;
                prodNf.Mva = MvaProdutoUfDAO.Instance.ObterMvaPorProduto(sessao, (int)pc.IdProd, idLoja, (int?)idFornec, null, false);
                prodNf.CstIpi = (int?)prod.CstIpi;
                prodNf.IdContaContabil = (uint?)prod.IdContaContabil;

                #region CSTs por Natureza de Operação

                if (prodNf.IdNaturezaOperacao > 0)
                {
                    var nat = NaturezaOperacaoDAO.Instance.ObtemElemento(sessao, (int)prodNf.IdNaturezaOperacao.Value);

                    if (nat != null)
                    {
                        prodNf.Csosn = nat.Csosn ?? prodNf.Csosn;
                        prodNf.Cst = nat.CstIcms ?? prodNf.Cst;
                        prodNf.PercRedBcIcms = nat.PercReducaoBcIcms;
                        prodNf.PercDiferimento = nat.PercDiferimento;
                        prodNf.CstIpi = (int?)(nat.CstIpi ?? ConfigNFe.CstIpi(idNf, nat.CodCfop));
                        prodNf.CstPis = nat.CstPisCofins ?? ConfigNFe.CstPisCofins(idNf);
                        prodNf.CstCofins = nat.CstPisCofins ?? ConfigNFe.CstPisCofins(idNf);
                    }
                }

                #endregion

                // Soma o ValorDescontoQtde ao total devido ao rateio que é feito logo abaixo do desconto total do pedido 
                // (Que inclui o desconto por qtd, o qual é rateado no total do produto)
                prodNf.Total =
                    !FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFeTerceiros && pc.QtdeOriginal > 0 ?
                    (pc.Total + pc.ValorBenef) / (decimal)pc.QtdeOriginal * (decimal)pc.Qtde :
                    pc.Total + pc.ValorBenef;

                // Verifica se o produto é calculado por ML e se a altura foi informada
                if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML && prodNf.Altura == 0)
                    throw new Exception("O comprimento do produto " + prod.Descricao + " está zerado.");
                
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

                // Calcula PIS e COFINS
                prodNf.CstPis = ConfigNFe.CstPisCofins(idNf);
                prodNf.AliqPis = ConfigNFe.AliqPis(idLoja);
                prodNf.BcPis = prodNf.Total;
                prodNf.ValorPis = prodNf.BcPis * (decimal)prodNf.AliqPis / 100;
                prodNf.CstCofins = ConfigNFe.CstPisCofins(idNf);
                prodNf.AliqCofins = ConfigNFe.AliqCofins(idLoja);
                prodNf.BcCofins = prodNf.Total;
                prodNf.ValorCofins = prodNf.BcCofins * (decimal)prodNf.AliqCofins / 100;

                // Salva o tipo de contribuição social e tipo de crédito (se necessário)
                if (FiscalConfig.NotaFiscalConfig.TipoNotaBuscarContribuicaoSocialPadrao != DataSourcesEFD.TipoUsoCredCont.Entrada)
                    prodNf.CodCont = (int?)FiscalConfig.NotaFiscalConfig.TipoContribuicaoSocialPadrao;

                if (FiscalConfig.NotaFiscalConfig.TipoNotaBuscarCreditoPadrao != DataSourcesEFD.TipoUsoCredCont.Entrada)
                    prodNf.CodCred = (int?)FiscalConfig.NotaFiscalConfig.TipoCreditoPadrao;

                prodNf.CodValorFiscal = ObtemCodValorFiscal(sessao, nf.TipoDocumento, nf.IdLoja.GetValueOrDefault(), prodNf.Cst);

                uint idProdNf = ProdutosNfDAO.Instance.Insert(sessao, prodNf);

                // Importa os beneficiamentos do produtoPedido para o produtoNf
                ProdutoNfBenefDAO.Instance.ImportaProdCompraBenef(sessao, pc.IdProdCompra, idProdNf);
            }

            #endregion

            #region Gera parcelas da nota

            if (idsCompras.Count() == 1)
            {
                if (tipoCompra == (int)Compra.TipoCompraEnum.APrazo)
                {
                    var parc = ParcelasCompraDAO.Instance.GetByCompra(sessao, idsCompras.First()).ToArray();
                    gerarParcelas = parc.Length > 0;

                    var datas = new DateTime[parc.Length];
                    var valores = new decimal[parc.Length];

                    decimal totalCompra = CompraDAO.Instance.ObtemValorCampo<decimal>(sessao, "total", "idCompra=" + idsCompras.First());
                    decimal valorTributado = CompraDAO.Instance.ObtemValorCampo<decimal>(sessao, "valorTributado", "idCompra=" + idsCompras.First());
                    decimal valorParc = 0;

                    for (int i = 0; i < parc.Length; i++)
                    {
                        decimal v = Math.Round(parc[i].Valor / (valorTributado > 0 && totalCompra - valorTributado > 0 ? totalCompra - valorTributado : 1) * (valorTributado > 0 ? valorTributado : 1), 2);
                        datas[i] = parc[i].Data != null ? parc[i].Data.Value : new DateTime();
                        valores[i] = i < (parc.Length - 1) ? v : (valorTributado > 0 ? valorTributado : totalCompra) - valorParc;
                        valorParc += v;
                    }

                    nf = GetElementByPrimaryKey(sessao, idNf);

                    // Se o número de parcelas da compra for 0, atualizar como 1
                    nf.NumParc = compras[0].NumParc > 0 ? compras[0].NumParc : 1;
                    nf.DatasParcelas = datas;
                    nf.ValoresParcelas = valores;
                    nf.DataBaseVenc = CompraDAO.Instance.ObtemValorCampo<DateTime?>(sessao, "dataBaseVenc", "idCompra=" + idsCompras.First());
                    nf.ValorParc = CompraDAO.Instance.ObtemValorCampo<decimal>(sessao, "valorParc", "idCompra=" + idsCompras.First());
                }
            }

            // Se o número de parcelas da nota for 0, atualizar como 1
            if (nf.NumParc == 0) nf.NumParc = 1;

            Update(sessao, nf);

            if (!gerarParcelas)
                ParcelaNfDAO.Instance.DeleteFromNf(sessao, idNf);

            #endregion

            return idNf;
        }

        #endregion

        #region Gera Nota Fiscal Complementar

        /// <summary>
        /// Gera Nota Fiscal Complementar
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns>Id da nota fiscal complementar gerada</returns>
        public uint GeraNFeComplementar(uint idNf)
        {
            NotaFiscal nf = GetElementByPrimaryKey(idNf);
            nf.IdsNfRef = idNf.ToString();
            nf.IdNf = 0;
            nf.IdNaturezaOperacao = null;
            nf.ChaveAcesso = null;
            nf.CodAleatorio = null;
            nf.Situacao = (int)NotaFiscal.SituacaoEnum.Aberta;
            nf.DataEmissao = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy"));
            nf.DataSaidaEnt = null;
            nf.FormaPagto = 1;
            nf.FinalidadeEmissao = (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar;
            nf.BcIcms = 0;
            nf.Valoricms = 0;
            nf.BcIcmsSt = 0;
            nf.ValorIcmsSt = 0;
            nf.ValorIpi = 0;
            nf.ValorIpiDevolvido = 0;
            nf.TotalProd = 0;
            nf.ValorFrete = 0;
            nf.VeicPlaca = null;
            nf.VeicRntc = null;
            nf.VeicUf = null;
            nf.QtdVol = 0;
            nf.Especie = null;
            nf.ValorSeguro = 0;
            nf.OutrasDespesas = 0;
            nf.PesoBruto = 0;
            nf.PesoLiq = 0;
            nf.Desconto = 0;
            nf.NumParc = 1;
            nf.TotalNota = 0;
            nf.NumLote = null;
            nf.NumRecibo = null;
            nf.NumProtocolo = null;
            nf.InfCompl = null;
            nf.ObsSefaz = null;

            if (FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.NaoUtilizar)
                nf.FormaEmissao = (int)NotaFiscal.TipoEmissao.Normal;
            else if (FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.SCAN)
                nf.FormaEmissao = (int)NotaFiscal.TipoEmissao.ContingenciaComSCAN;
            else if (FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.FSDA)
                nf.FormaEmissao = (int)NotaFiscal.TipoEmissao.ContingenciaFSDA;

            return Insert(nf);
        }

        #endregion

        #region Gerar XML da NF-e para emissão

        private ProdutosNf[] GetSubstProdutosProjeto(IEnumerable<ProdutosNf> prodNf, bool manterProdutoDaNf)
        {
            return GetSubstProdutosProjeto(null, prodNf, manterProdutoDaNf);
        }

        private ProdutosNf[] GetSubstProdutosProjeto(GDASession session, IEnumerable<ProdutosNf> prodNf, bool manterProdutoDaNf)
        {
            List<ProdutosNf> retorno = new List<ProdutosNf>();
            foreach (ProdutosNf p in prodNf)
                retorno.Add(MetodosExtensao.Clonar(p));

            // Troca os produtos contábeis de projeto pelos materiais
            for (int i = retorno.Count - 1; i >= 0; i--)
                if (ProdutoNfItemProjetoDAO.Instance.ProdNfCadastrado(session, retorno[i].IdProdNf))
                {
                    retorno.AddRange(ProdutoNfItemProjetoDAO.Instance.GetAsProdNf(session, retorno[i].IdProdNf));
                    if (!manterProdutoDaNf)
                        retorno.RemoveAt(i);
                }

            return retorno.ToArray();
        }

        /// <summary>
        /// Gera o XML da NF-e.
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="preVisualizar"></param>
        /// <returns></returns>
        private XmlDocument GerarXmlNf(uint idNf, bool preVisualizar)
        {
            NotaFiscal nf = GetElement(idNf);

            // Verifica se NFe pode ser emitida
            if (nf.Situacao != (int)NotaFiscal.SituacaoEnum.Aberta && nf.Situacao == (int)NotaFiscal.SituacaoEnum.NaoEmitida &&
                nf.Situacao == (int)NotaFiscal.SituacaoEnum.FalhaEmitir)
                throw new Exception("Apenas Notas Fiscais nas situações: Aberta, Não Emitida e Falha ao emitir podem ser emitidas.");

            // Verifica se o cliente foi selecionado
            if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída && nf.IdCliente == null && nf.IdFornec == null)
                throw new Exception("Informe o destinatário/remetente.");

            // Verifica se o fornecedor foi selecionado
            if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Entrada && nf.IdCliente == null && nf.IdFornec == null)
                throw new Exception("Informe o destinatário/remetente.");

            // Verifica se algum produto da nota possui o CFOP da própria nota
            if (objPersistence.ExecuteSqlQueryCount(@"Select Count(*) From produtos_nf pnf
                inner join natureza_operacao no on (pnf.idNaturezaOperacao=no.idNaturezaOperacao)
                Where pnf.idNf=" + idNf + @" And no.idCfop=(Select no1.idCfop From nota_fiscal nf 
                inner join natureza_operacao no1 on (nf.idNaturezaOperacao=no1.idNaturezaOperacao) Where nf.idNf=" + idNf + ")") == 0)
                throw new Exception("Nenhum produto desta nota fiscal possui o CFOP informado na própria nota.");

            if (!preVisualizar && FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber && !PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)idNf).Any())
                throw new Exception("Informe a forma de pagamento.");

            if (!preVisualizar && FiscalConfig.NotaFiscalConfig.BloquearEmissaoNotaSePedidoPossuirContaRecebida)
            {
                var idsLiberacoes = PedidosNotaFiscalDAO.Instance.GetByNf(idNf)
                    .Where(f => f.IdLiberarPedido != null).Select(f => f.IdLiberarPedido.Value).ToList();

                foreach (var idLib in idsLiberacoes)
                    if (ContasReceberDAO.Instance.GetRecebidasByLiberacao(idLib).Count > 0)
                        throw new Exception("Não é possível emitir esta nota fiscal, pois existe uma conta recebida para a liberacao " + idLib + ".");
            }

            if (!preVisualizar && !nf.Consumidor && Configuracoes.FiscalConfig.NotaFiscalConfig.GerarEFD &&
                (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída || nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Entrada) &&
                !nf.DataSaidaEnt.HasValue)
            {
                throw new Exception("Não é possível emitir esta nota fiscal, pois a data de Saída/Entrada não foi informada e a configuração para utilizar SPED Físcal esta ativa.");
            }

            Cliente cliente = nf.IdCliente > 0 ? ClienteDAO.Instance.GetElement(nf.IdCliente.Value) : null;
            Fornecedor fornec = nf.IdFornec > 0 ? FornecedorDAO.Instance.GetElement(nf.IdFornec.Value) : null;

            Transportador transportador = null;
            Loja loja = LojaDAO.Instance.GetElementByPrimaryKey(nf.IdLoja.Value);
            Cfop cfop;
            Cidade cidEmitente;

            List<ProdutosNf> lstProdNf = null;
            ParcelaNf[] lstParcNf = null;

            bool isImportacao = IsNotaFiscalImportacao(idNf);
            bool isExportacao = IsNotaFiscalExportacao(null, idNf);

            #region Busca os produtos

            try
            {
                // Busca os produtos da nota
                lstProdNf = ProdutosNfDAO.Instance.GetByNfExtended(idNf).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao buscar produtos da nota.", ex);
            }

            #endregion

            #region Busca as duplicatas

            try
            {
                // Busca as duplicatas da nota
                lstParcNf =
                    !nf.Consumidor ?
                        ParcelaNfDAO.Instance.GetByNf(idNf).ToArray() :
                        new ParcelaNf[] { new ParcelaNf() };
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao buscar duplicatas da nota.", ex);
            }

            #endregion

            #region Calcula o ICMS ST e o Total da Nota

            try
            {
                // Calcula o ICMS ST dos produtos da NF e da própria NF
                ProdutosNfDAO.Instance.CalcImposto(null, ref lstProdNf, false, false);

                if (nf.TotalManual > 0)
                    objPersistence.ExecuteCommand("Update nota_fiscal Set totalNota=totalManual Where idNf=" + idNf);

                // Recarrega a NF, para buscar os totais corretos
                nf = GetElement(idNf);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao calcular impostos.", ex);
            }

            #endregion

            // Busca o total dos produtos desta NF
            decimal totalProd = nf.TotalProd;
            float totalProdSN = Glass.Conversoes.StrParaFloat(ProdutosNfDAO.Instance.ObtemTotalCalcSN(idNf));

            // Alíquota/Valor do ICMS para aproveitamento de crédito (SIMPLES NACIONAL)
            Single aliqICMSSN = FiscalConfig.NotaFiscalConfig.AliqICMSSN / 100F;
            Single valorICMSSN = aliqICMSSN * totalProdSN;

            //Indicador do IE do destinatario
            Cidade cidadeFornec;
            var indIeDest = ObterIndicadorIE(nf, null, cliente, fornec, out cidadeFornec);

            #region Verificações da NFe

            #region Geral

            // Verifica se o CFOP foi informado
            if (nf.IdNaturezaOperacao == null || nf.IdNaturezaOperacao == 0)
                throw new Exception("Informe a natureza da operação.");

            cfop = CfopDAO.Instance.GetElementByPrimaryKey(nf.IdCfop.Value);

            // Se for cfop de devolução, obriga a informar o tipo do mesmo.
            if (CfopDAO.Instance.IsCfopDevolucao(cfop.CodInterno) && cfop.IdTipoCfop.GetValueOrDefault() == 0)
                throw new Exception("É necessário informar o tipo de CFOP para todo CFOP de devolução.");

            // Verifica se a data de saída é >= que a data de emissão
            if (nf.DataSaidaEnt != null && nf.DataSaidaEnt.Value < nf.DataEmissao)
                throw new Exception("A data de saída/entrada não pode ser menor que a data de emissão.");

            // Verifica se o tipo de documento foi informado
            if (nf.TipoDocumento == 0)
                throw new Exception("Informe o tipo do documento.");

            // Verifica se o CFOP selecionado é de nota fiscal de saída
            if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída)
            {
                if (CfopDAO.Instance.IsCfopEntrada(nf.IdCfop.Value))
                    throw new Exception("O CFOP informado na nota não é uma CFOP de saída.");

                foreach (ProdutosNf pnf in lstProdNf)
                    if (pnf.IdNaturezaOperacao > 0 && CfopDAO.Instance.IsCfopEntrada(pnf.IdCfop.Value))
                        throw new Exception("O CFOP informado no produto " + pnf.DescrProduto + " não é uma CFOP de saída.");
            }

            // Verifica se o CFOP selecionado é de nota fiscal de entrada
            if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Entrada)
            {
                if (!CfopDAO.Instance.IsCfopEntrada(nf.IdCfop.Value))
                    throw new Exception("O CFOP informado na nota não é uma CFOP de entrada.");

                foreach (ProdutosNf pnf in lstProdNf)
                    if (pnf.IdCfop > 0 && !CfopDAO.Instance.IsCfopEntrada(pnf.IdCfop.Value))
                        throw new Exception("O CFOP informado no produto " + pnf.DescrProduto + " não é uma CFOP de entrada.");
            }

            // Se o transportador tiver sido informado, verifica se seus dados estão corretos
            if (nf.IdTransportador > 0)
            {
                transportador = TransportadorDAO.Instance.GetElement(nf.IdTransportador.Value);

                if ((transportador.TipoPessoa == 1 && !Glass.Validacoes.ValidaCpf(transportador.CpfCnpj)) ||
                    (transportador.TipoPessoa == 2 && !Glass.Validacoes.ValidaCnpj(transportador.CpfCnpj)))
                    throw new Exception("O CPF/CNPJ do Transportador é inválido. Altere no cadastro de Transportador.");

                if (transportador.TipoPessoa == 2 && !Glass.Validacoes.ValidaIE(transportador.Uf, transportador.InscEst))
                    throw new Exception("A inscrição estadual do transportador é inválida para a UF cadastrada (" + transportador.Uf + "). Altere no cadastro de Transportador.");

                // Verifica se a quantidade de volumes foi informada
                if (nf.QtdVol == 0)
                    throw new Exception("Informe a qtd. de volumes que serão transportados.");

                // Verifica se a espécie foi informada
                if (String.IsNullOrEmpty(nf.Especie))
                    throw new Exception("Informe a espécie dos volumes que serão transportados.");
            }
            else
            {
                if (!String.IsNullOrEmpty(nf.VeicPlaca))
                    throw new Exception("Para que a placa do veículo seja informada na nota, é necessário informar o transportador.");
            }

            #endregion

            #region Parcelas

            if (!nf.Consumidor && nf.FormaPagto == (int)NotaFiscal.FormaPagtoEnum.AVista && 
                PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)idNf).Any(p => p.FormaPagto == (int)FormaPagtoNotaFiscalEnum.BoletoBancario))
                throw new Exception("A nota fiscal não pode ser à vista e ter forma de pagamento boleto.");

            if (!nf.Consumidor && nf.FormaPagto == 2 && lstParcNf.Length == 0)
                throw new Exception("Informe os valores e os vencimentos das duplicatas.");

            // Se a nota for à prazo, verifica se o valor das duplicatas está de acordo com o total da nota
            if (!nf.Consumidor && (nf.FormaPagto == 2 || nf.FormaPagto == 3))
            {
                var parc = ParcelaNfDAO.Instance.GetByNf(idNf);

                decimal totalParc = parc.Sum(x => x.Valor);

                if (Math.Round(totalParc, 2) != Math.Round(nf.TotalNota, 2) && Math.Round(totalParc, 2) != Math.Round(nf.TotalManual, 2))
                    throw new Exception("O valor das duplicatas está diferente do valor da nota. Valor das duplicatas: " +
                        totalParc.ToString("C") + " Valor da nota: " + nf.TotalNota.ToString("C"));

                var datasParc = new List<DateTime>();

                foreach (ParcelaNf p in ParcelaNfDAO.Instance.GetByNf(idNf))
                {
                    if (p.Data == null || p.Data.Value.Year == 1)
                        throw new Exception("Informe a data de vencimento das duplicatas.");
                    else if (DateTime.Parse(p.Data.Value.ToString("dd/MM/yyyy")) < DateTime.Parse(nf.DataEmissao.ToString("dd/MM/yyyy")))
                        throw new Exception("A data de vencimento das duplicatas não pode ser inferior à data de emissão da nota.");

                    if (datasParc.Contains(p.Data.Value))
                        throw new Exception("A data de vencimento " + p.Data.Value.ToString("dd/MM/yyyy") +
                            " foi definida para mais de uma duplicata.");

                    datasParc.Add(p.Data.Value);
                }
            }

            #endregion

            #region Certificado

            if (!preVisualizar && !File.Exists(Utils.GetCertPath + "loja" + loja.IdLoja + ".pfx"))
                throw new Exception("O certificado digital da loja " + loja.NomeFantasia + " não foi cadastrado.");

            #endregion

            #region Emitente

            // Verifica se a loja (emitente) possui cidade selecionada
            if (loja.IdCidade == 0 || loja.IdCidade == null)
                throw new Exception("A cidade do Emitente não foi informada, informar no cadastro de lojas.");

            // Valida CNPJ da loja (emitente)
            if (!Glass.Validacoes.ValidaCnpj(loja.Cnpj))
                throw new Exception("CNPJ do Emitente inválido, alterar no cadastro de lojas.");

            // Verifica se foi informado o CRT (Código de Regime Tributário) da loja
            if (loja.Crt == null || loja.Crt == 0)
                throw new Exception("Informe o CRT do Emitente no cadastro de lojas.");

            // Verifica se foi informado o tipo da loja
            if (loja.Tipo == null || loja.Tipo == 0)
                throw new Exception("Informe se o Emitente é indústria ou comércio no cadastro de lojas.");

            // Busca a cidade do emitente
            cidEmitente = CidadeDAO.Instance.GetElementByPrimaryKey((uint)loja.IdCidade.Value);

            // Verifica se a inscrição estadual do emitente é válida
            if (!Glass.Validacoes.ValidaIE(cidEmitente.NomeUf, loja.InscEst))
                throw new Exception("Inscrição Estadual do Emitente inválida para a UF cadastrada (" + cidEmitente.NomeUf + "), alterar no cadastro de lojas.");

            if (String.IsNullOrEmpty(loja.Cep))
                throw new Exception("O CEP do emitente deve ser informado.");

            if (String.IsNullOrEmpty(loja.Endereco))
                throw new Exception("Informe o Endereço do Emitente no cadastro de lojas.");

            if (String.IsNullOrEmpty(loja.Numero))
                throw new Exception("Informe o Número do endereço do Emitente no cadastro de lojas.");

            if (!Glass.Validacoes.ApenasLetrasNumeros(loja.Numero))
                throw new Exception("O número do endereço do Emitente deve ter apenas números e letras.");

            if (String.IsNullOrEmpty(loja.Bairro))
                throw new Exception("Informe o Bairro do Emitente no cadastro de lojas.");

            /* Chamado 44653. */
            /*if (Formatacoes.TrataStringDocFiscal(loja.Telefone).Length <= 8)
                throw new Exception("O telefone do Emitente deve ser informado com o DDD.");*/

            string cepEmit = Formatacoes.TrataStringDocFiscal(loja.Cep);
            if (cepEmit.Length != 0 && cepEmit.Length != 8)
                throw new Exception("O CEP do Emitente é inválido.");

            #endregion

            #region Destinatário

            if (cliente != null && !(nf.Consumidor && ClienteDAO.Instance.IsConsumidorFinal((uint)cliente.IdCli)))
            {
                if (cliente.TipoPessoa.ToUpper() == "J")
                {
                    // Verifica se o CNPJ é válido
                    if (!Glass.Validacoes.ValidaCnpj(cliente.CpfCnpj) && !isImportacao && !isExportacao)
                        throw new Exception("O CNPJ do Cliente é inválido. Altere no cadastro de clientes.");

                    if (String.IsNullOrEmpty(cliente.Uf))
                        throw new Exception("A cidade do cliente não foi selecionada, selecione-a no cadastro de clientes.");

                    // Verifica se a inscrição estadual do cliene é válida TODO: Remover o comentário após corrigir a inscrição estadual "4810002208" do RS
                    //if (!Glass.Validacoes.ValidaIE(cliente.Uf, cliente.RgEscinst))
                    //    throw new Exception("Inscrição Estadual do Cliente inválida para a UF cadastrada (" + cliente.Uf + "), altere no cadastro de clientes.");
                }
                else
                {
                    // Verifica se o CPF é válido
                    if (!Glass.Validacoes.ValidaCpf(cliente.CpfCnpj))
                        throw new Exception("O CPF do Cliente é inválido. Altere no cadastro de clientes.");
                }

                // Se possuir email, verifica se é válido
                if (!string.IsNullOrEmpty(cliente.EmailFiscal) && !Glass.Validacoes.ValidaEmail(cliente.EmailFiscal))
                    throw new Exception("O email fiscal do Cliente é inválido, altere no cadastro de clientes");

                // Verifica se o nome do cliente está preenchido
                if (String.IsNullOrEmpty(cliente.Nome))
                    throw new Exception("O nome do Cliente deve ser informado.");

                // Verifica se a cidade do cliente foi informada
                if (cliente.IdCidade == null || cliente.IdCidade == 0)
                    throw new Exception("A cidade do Cliente não foi informada, informe no cadastro de clientes.");

                if (String.IsNullOrEmpty(cliente.Endereco))
                    throw new Exception("Informe o Endereço do Cliente no cadastro de clientes.");

                if (String.IsNullOrEmpty(cliente.Numero))
                    throw new Exception("Informe o Número do Endereço do Cliente no cadastro de clientes.");

                if (!Glass.Validacoes.ApenasLetrasNumeros(cliente.Numero))
                    throw new Exception("O número do endereço do Cliente deve ter apenas números e letras.");

                if (String.IsNullOrEmpty(cliente.Bairro))
                    throw new Exception("Informe o Bairro do Cliente no cadastro de clientes.");

                if (Formatacoes.TrataStringDocFiscal(cliente.Telefone).Length <= 8)
                    throw new Exception("O telefone do Cliente deve ser informado com o DDD.");

                string cepDest = Formatacoes.TrataStringDocFiscal(cliente.Cep);
                if (cepDest.Length != 0 && cepDest.Length != 8)
                    throw new Exception("O CEP do Cliente é inválido.");

                if (FiscalConfig.NotaFiscalConfig.UsarNomeFantasiaNotaFiscal &&
                    cliente.TipoFiscal.GetValueOrDefault(0) == 0)
                    throw new Exception("Informe o tipo fiscal deste cliente no cadastro de clientes.");
            }
            else if (fornec != null)
            {
                if (fornec.TipoPessoa.ToUpper() == "J")
                {
                    // Verifica se o CNPJ é válido
                    if (!Glass.Validacoes.ValidaCnpj(fornec.CpfCnpj) && !isImportacao)
                        throw new Exception("O CNPJ do Fornecedor é inválido. Altere no cadastro de fornecedores.");

                    // Verifica se a inscrição estadual do fornecedor é válida
                    if (!Glass.Validacoes.ValidaIE(fornec.Uf, fornec.RgInscEst) && !isImportacao)
                        throw new Exception("Inscrição Estadual do Fornecedor inválida para a UF cadastrada (" + fornec.Uf + "). Altere no cadastro de fornecedores.");
                }
                else
                {
                    // Verifica se o CPF é válido
                    if (!Glass.Validacoes.ValidaCpf(fornec.CpfCnpj))
                        throw new Exception("O CPF do Fornecedor é inválido. Altere no cadastro de fornecedores.");
                }

                // Verifica se o nome do fornecedor está preenchido
                if (String.IsNullOrEmpty(fornec.Razaosocial))
                    throw new Exception("O nome do Fornecedor deve ser informado.");

                // Verifica se a cidade do fornecedor foi informada
                if (fornec.IdCidade == null || fornec.IdCidade == 0)
                    throw new Exception("A cidade do Fornecedor não foi informada, informe no cadastro de fornecedores.");

                if (String.IsNullOrEmpty(fornec.Endereco))
                    throw new Exception("Informe o Endereço do Fornecedor no cadastro de fornecedores.");

                if (String.IsNullOrEmpty(fornec.Numero))
                    throw new Exception("Informe o Número do Endereço do Fornecedor no cadastro de fornecedores.");

                if (!Glass.Validacoes.ApenasLetrasNumeros(fornec.Numero))
                    throw new Exception("O número do endereço do Fornecedor deve ter apenas números e letras.");

                if (String.IsNullOrEmpty(fornec.Bairro))
                    throw new Exception("Informe o Bairro do Fornecedor no cadastro de fornecedores.");

                if (Formatacoes.TrataStringDocFiscal(fornec.Telcont).Length <= 8)
                    throw new Exception("O telefone do Fornecedor deve ser informado com o DDD.");

                string cepDest = Formatacoes.TrataStringDocFiscal(fornec.Cep);
                if (cepDest.Length != 0 && cepDest.Length != 8)
                    throw new Exception("O CEP do Fornecedor é inválido.");
            }

            #endregion

            #region Produtos

            // Para cada produto da Nota Fiscal
            ProdutosNf[] lstProdNfValida = GetSubstProdutosProjeto(lstProdNf, true);
            foreach (ProdutosNf pnf in lstProdNfValida)
            {
                bool produtoDeProjeto = ProdutoNfItemProjetoDAO.Instance.ProdNfCadastrado(pnf.IdProdNf);
                bool produtoOriginal = Array.Exists(lstProdNf.ToArray(), x => x.IdProdNf == pnf.IdProdNf && x.IdProd == pnf.IdProd);

                if (!produtoDeProjeto || produtoOriginal)
                {
                    // Verifica se o campo NCM do produto foi informado
                    if (String.IsNullOrEmpty(pnf.Ncm))
                        throw new Exception("O campo NCM do produto " + pnf.DescrProduto.Replace("'", "") + " deve ser informado.");

                    // Verifica se o campo NCM do produto têm 2 ou 8 dígitos
                    if (pnf.Ncm.Length != 2 && pnf.Ncm.Length != 8)
                        throw new Exception("O campo NCM do produto " + pnf.DescrProduto.Replace("'", "") + " deve ter 2 ou 8 dígitos.");

                    // Verifica se o valor é > 0
                    if (pnf.ValorUnitario <= 0 && nf.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Normal && cfop.CodInterno != "5949")
                        throw new Exception("O valor do produto " + pnf.DescrProduto.Replace("'", "") + " não pode ser menor ou igual a 0.");

                    // Verifica se a unidade foi informada
                    if (pnf.Unidade == null || Formatacoes.TrataStringDocFiscal(pnf.Unidade).Trim() == String.Empty)
                        throw new Exception("A unidade do produto " + pnf.DescrProduto.Replace("'", "") + " não foi informada ou é inválida. Informe no cadastro de produtos.");

                    // Verifica se a unidade trib foi informada
                    if (pnf.UnidadeTrib == null || Formatacoes.TrataStringDocFiscal(pnf.UnidadeTrib).Trim() == String.Empty)
                        throw new Exception("A unidade trib. do produto " + pnf.DescrProduto.Replace("'", "") + " não foi informada ou é inválida. Informe no cadastro de produtos.");

                    // Se for vidro, verifica se a espessura foi informada
                    if (pnf.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro && pnf.Espessura <= 0)
                        throw new Exception("A espessura do produto " + pnf.DescrProduto.Replace("'", "") + " deve ser informada.");

                    // Verifica se o peso foi informado
                    if (pnf.Peso <= 0 && nf.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Normal)
                    {
                        string dadosProduto = pnf.DescrProduto.Replace("'", "") + " (altura {0}, largura {1}, qtde {2}{3})";
                        dadosProduto = String.Format(dadosProduto, pnf.Altura, pnf.Largura, pnf.Qtde, "{0}");
                        dadosProduto = String.Format(dadosProduto, pnf.TipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 && pnf.TipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto ? "" :
                            ", tot. m² " + pnf.TotM.ToString("0.##"));

                        throw new Exception("O peso do produto " + dadosProduto + " deve ser informado. Produtos calculados por m² devem ter o campo altura e largura informados. Informe no cadastro de produtos.");
                    }

                    if (isImportacao)
                    {
                        if (string.IsNullOrEmpty(pnf.NumDocImp))
                            throw new Exception("Informe o número do DI de cada produto.");

                        if (string.IsNullOrEmpty(pnf.CodExportador))
                            throw new Exception("Informe o código do exportador de cada produto.");

                        if (pnf.DataDesembaraco == null || pnf.DataRegDocImp == null)
                            throw new Exception("Informe a data de desembaraço e a data de registro do DI.");

                        if (string.IsNullOrEmpty(pnf.LocalDesembaraco))
                            throw new Exception("Informe o local do desembaraço de cada produto.");

                        if (ProdutoNfAdicaoDAO.Instance.GetByProdNf(pnf.IdProdNf).Length == 0)
                            throw new Exception("Informe as Adições de cada produto.");
                    }

                    //if (FiscalConfig.UtilizaFCI && (pnf.CstOrig == 3 || pnf.CstOrig == 5 || pnf.CstOrig == 8))
                    //{
                    //    if (string.IsNullOrEmpty(pnf.NumControleFciStr))
                    //        throw new Exception("Número de controle da FCI do produto " + pnf.CodInterno + " - " + pnf.DescrProduto + " não foi encontrado.");
                    //}
                }

                // Verifica se o produto existe no estoque fiscal/estoque de terceiros, se estiver em modo de produção e for nota de saída
                // (não verifica os produtos de projeto)
                if ((!produtoDeProjeto || !produtoOriginal) &&
                    ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao && nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída)
                    VerificaEstoque(pnf, lstProdNfValida, nf.IdLoja.Value);
            }

            #endregion

            #endregion

            #region Código aleatório e chave de acesso

            try
            {
                // Código aleatório da NFe
                if (String.IsNullOrEmpty(nf.CodAleatorio))
                    nf.CodAleatorio = (nf.NumeroNFe + (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.Normal ? 10203040 : 9020304)).ToString();

                // Gera chave de acesso para a NFe
                nf.ChaveAcesso = ChaveDeAcesso(cidEmitente.CodIbgeUf, nf.DataEmissao.ToString("yyMM"), loja.Cnpj, nf.Modelo,
                    nf.Serie.ToString().PadLeft(3, '0'), nf.NumeroNFe.ToString(), nf.FormaEmissao.ToString(), nf.CodAleatorio);

                // Atualiza dados da NF
                if (!preVisualizar)
                    Update(nf);
                else
                    objPersistence.ExecuteCommand("Update nota_fiscal Set codAleatorio=?codAl, chaveAcesso=?chAc Where idNf=" + nf.IdNf,
                        new GDAParameter("?codAl", nf.CodAleatorio), new GDAParameter("?chAc", nf.ChaveAcesso));
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao gerar chave de acesso.", ex);
            }

            #endregion

            #region Transparência ("De olho no imposto")

            string valorTotalTributosIbpt = null;

            // CFOP's de simples faturamento/venda futura não devem destacar os impostos, assim como os CFOPS 5923 e 6923
            var exibirImpostoCfop = cfop.CodInterno != "5922" && cfop.CodInterno != "6922" && cfop.CodInterno != "5923" && cfop.CodInterno != "6923";

            try
            {
                if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída && exibirImpostoCfop && cliente != null)
                {
                    var retorno = ImpostoNcmUFDAO.Instance.ObtemDadosImpostos(lstProdNf);

                    valorTotalTributosIbpt = String.Format("Trib aprox R$ {0:c} Federal e ({1:c}) Estadual Fonte: {2}",
                        retorno.ValorImpostoNacional, retorno.ValorImpostoEstadual, retorno.Fonte);
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("BuscarTributos", ex);

                throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao buscar tributos aproximados.", ex));
            }

            #endregion

            #region NFC-e

            if (nf.Consumidor)
            {
                if (nf.TotalNota > 10000 && ClienteDAO.Instance.IsConsumidorFinal((uint)cliente.IdCli))
                    throw new Exception("O valor do total excede o máximo da permitido para NFC-e sem informar um consumidor (R$10.000,00)");

                if (nf.TotalNota > 200000)
                    throw new Exception("O valor do total excede o máximo da permitido para NFC-e (R$200.000,00)");

                if (cliente != null && indIeDest != IndicadorIEDestinatario.NaoContribuinte)
                    throw new Exception("NFC-e só podera ser emitida para não contribuintes do ICMS");

                if (!CfopDAO.Instance.IsCfopNFCe(cfop.CodInterno))
                    throw new Exception("O CFOP: " + cfop.CodInterno + " não pode ser utilizado para emissão de NFC-e");

                var pagamentos = PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)idNf);

                if (pagamentos == null || !pagamentos.Any())
                    throw new Exception("Nenhuma forma de pagto. foi infomada para esta NFC-e");

                if (pagamentos.Sum(f => f.Valor) != nf.TotalNota)
                    throw new Exception("O valor das formas de pagto. não confere com o total da NFC-e");

                nf.DataEmissao = DateTime.Now;
                objPersistence.ExecuteCommand("UPDATE nota_fiscal SET dataEmissao = ?dt WHERE idNF = " + idNf, new GDAParameter("?dt", nf.DataEmissao));
            }


            #endregion

            #region Gera XML

            XmlDocument doc = new XmlDocument();
            //XmlNode declarationNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            //doc.AppendChild(declarationNode);

            XmlElement NFe = doc.CreateElement("NFe");
            NFe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
            doc.AppendChild(NFe);

            XmlElement infNFe = doc.CreateElement("infNFe");
            infNFe.SetAttribute("versao", ConfigNFe.VersaoNFe);
            infNFe.SetAttribute("Id", "NFe" + nf.ChaveAcesso);
            NFe.AppendChild(infNFe);

            #region Identificação da NFe

            try
            {
                // Identificação da NFe
                XmlElement ide = doc.CreateElement("ide");
                ManipulacaoXml.SetNode(doc, ide, "cUF", cidEmitente.CodIbgeUf);
                ManipulacaoXml.SetNode(doc, ide, "cNF", nf.CodAleatorio.PadLeft(8, '0'));
                ManipulacaoXml.SetNode(doc, ide, "natOp", Formatacoes.TrataStringDocFiscal(cfop.CodInterno + "-" + cfop.Descricao));
                ManipulacaoXml.SetNode(doc, ide, "mod", nf.Modelo);
                ManipulacaoXml.SetNode(doc, ide, "serie", nf.Serie.ToString());
                ManipulacaoXml.SetNode(doc, ide, "nNF", nf.NumeroNFe.ToString());
                ManipulacaoXml.SetNode(doc, ide, "dhEmi", nf.DataEmissao.ToString("yyyy-MM-ddTHH:mm:sszzz"));

                //NFC-e não deve ser informado data de Entrada/Saída
                if (nf.DataSaidaEnt != null && !nf.Consumidor)
                    ManipulacaoXml.SetNode(doc, ide, "dhSaiEnt", nf.DataSaidaEnt.Value.ToString("yyyy-MM-ddTHH:mm:sszzz"));

                ManipulacaoXml.SetNode(doc, ide, "tpNF", (nf.TipoDocumento - 1).ToString());
                
                var idDest = 1;

                if (cfop.CodInterno[0] == '2' || cfop.CodInterno[0] == '6')
                    idDest = 2;
                else if (cfop.CodInterno[0] == '3' || cfop.CodInterno[0] == '7')
                    idDest = 3;

                ManipulacaoXml.SetNode(doc, ide, "idDest", idDest.ToString());

                ManipulacaoXml.SetNode(doc, ide, "cMunFG", CidadeDAO.Instance.GetElementByPrimaryKey(nf.IdCidade).CodUfMunicipio);

                ManipulacaoXml.SetNode(doc, ide, "tpImp", nf.TipoImpressao.ToString());
                ManipulacaoXml.SetNode(doc, ide, "tpEmis", nf.FormaEmissao.ToString());
                ManipulacaoXml.SetNode(doc, ide, "cDV", nf.ChaveAcesso[43].ToString());
                ManipulacaoXml.SetNode(doc, ide, "tpAmb", ((int)ConfigNFe.TipoAmbiente).ToString()); // 1-Produção 2-Homologação
                ManipulacaoXml.SetNode(doc, ide, "finNFe", nf.FinalidadeEmissao.ToString());
                ManipulacaoXml.SetNode(doc, ide, "indFinal",
                    (cliente != null && cliente.TipoFiscal.GetValueOrDefault() == 1) ||
                    (fornec != null && indIeDest == IndicadorIEDestinatario.NaoContribuinte) ? "1" : "0"); //Indica operação com Consumidor final (0 - não / 1 - consumidor final
                ManipulacaoXml.SetNode(doc, ide, "indPres",
                    nf.Consumidor ?
                        ((int)NotaFiscal.IndicadorPresencaComprador.OperacaoPresencial).ToString() :
                        ((int)NotaFiscal.IndicadorPresencaComprador.OperacaoNaoPresencialOutros).ToString());
                ManipulacaoXml.SetNode(doc, ide, "procEmi", "0"); // Emissão de NF-e com aplicativo do contribuinte
                ManipulacaoXml.SetNode(doc, ide, "verProc", "3.1.0");

                if (nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.Normal)
                {
                    ManipulacaoXml.SetNode(doc, ide, "dhCont", DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"));
                    ManipulacaoXml.SetNode(doc, ide, "xJust", Formatacoes.TrataStringDocFiscal(FiscalConfig.NotaFiscalConfig.JustificativaContingenciaNFE));
                }

                if (!string.IsNullOrEmpty(nf.IdsNfRef) &&
                    ((nf.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar ||
                    nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Entrada)
                    || CfopDAO.Instance.IsCfopDevolucaoNFeRefereciada(nf.IdCfop.GetValueOrDefault()) || nf.CodCfop == "5929"))
                {
                    foreach (var i in nf.IdsNfRef.Split(','))
                    {
                        XmlElement nfRef = doc.CreateElement("NFref");
                        ManipulacaoXml.SetNode(doc, nfRef, "refNFe", NotaFiscalDAO.Instance.ObtemChaveAcesso(Conversoes.StrParaUint(i)));
                        ide.AppendChild(nfRef);
                    }
                }

                infNFe.AppendChild(ide);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir identificação da NFe no XML.", ex);
            }

            #endregion

            #region Emitente

            try
            {
                // Se estiver emitindo nota em modo de contingência, deve-se retirar os zeros à esquerda do CNPJ e da IE
                if (nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.Normal)
                {
                    //loja.Cnpj = loja.Cnpj.TrimStart('0');
                    //loja.InscEst = loja.InscEst.TrimStart('0');
                }

                // Emitente
                XmlElement emit = doc.CreateElement("emit");
                ManipulacaoXml.SetNode(doc, emit, "CNPJ", Formatacoes.TrataStringDocFiscal(loja.Cnpj).PadLeft(14, '0'));
                ManipulacaoXml.SetNode(doc, emit, "xNome", Formatacoes.TrataStringDocFiscal(loja.RazaoSocial));
                //ManipulacaoXml.SetNode(doc, emit, "xFant", Formatacoes.TrataStringDocFiscal(loja.NomeFantasia));
                XmlElement enderEmit = doc.CreateElement("enderEmit");
                ManipulacaoXml.SetNode(doc, enderEmit, "xLgr", Formatacoes.TrataStringDocFiscal(loja.Endereco));
                ManipulacaoXml.SetNode(doc, enderEmit, "nro", Formatacoes.TrataStringDocFiscal(Glass.Formatacoes.RetiraCaracteresEspeciais(loja.Numero)));

                if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(loja.Compl)))
                    ManipulacaoXml.SetNode(doc, enderEmit, "xCpl", Formatacoes.TrataStringDocFiscal(loja.Compl));

                ManipulacaoXml.SetNode(doc, enderEmit, "xBairro", Formatacoes.TrataStringDocFiscal(loja.Bairro));
                ManipulacaoXml.SetNode(doc, enderEmit, "cMun", cidEmitente.CodUfMunicipio);
                ManipulacaoXml.SetNode(doc, enderEmit, "xMun", Formatacoes.TrataStringDocFiscal(cidEmitente.NomeCidade));
                ManipulacaoXml.SetNode(doc, enderEmit, "UF", cidEmitente.NomeUf);
                ManipulacaoXml.SetNode(doc, enderEmit, "CEP", Formatacoes.TrataStringDocFiscal(loja.Cep));

                ManipulacaoXml.SetNode(doc, enderEmit, "cPais", "1058");
                ManipulacaoXml.SetNode(doc, enderEmit, "xPais", "Brasil");

                /* Chamado 45303. */
                if (!string.IsNullOrEmpty(loja.Telefone))
                    ManipulacaoXml.SetNode(doc, enderEmit, "fone", Formatacoes.TrataStringDocFiscal(loja.Telefone, true));

                emit.AppendChild(enderEmit);

                ManipulacaoXml.SetNode(doc, emit, "IE", Formatacoes.TrataStringDocFiscal(loja.InscEst.ToUpper()));

                // Recupera o IEST com base na loja que está emitindo a nota e no UF do destinatário.
                int idCidade = 0;
                if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída)
                {
                    var cfopDevolucao = CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(nf.IdNaturezaOperacao.Value));
                    idCidade = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.IdCidade.Value : fornec.IdCidade.Value;
                }
                else
                    idCidade = cliente != null ? cliente.IdCidade.Value : fornec.IdCidade.Value;

                var nomeUF = CidadeDAO.Instance.GetNomeUf(idCidade);
                var iest = IestUfLojaDAO.Instance.ObterIestUfLoja((uint)loja.IdLoja, nomeUF);
                if (!string.IsNullOrEmpty(iest))
                    ManipulacaoXml.SetNode(doc, emit, "IEST", Formatacoes.TrataStringDocFiscal(iest.ToUpper()));

                ManipulacaoXml.SetNode(doc, emit, "CRT", Formatacoes.TrataStringDocFiscal(loja.Crt <= 3 ? loja.Crt.ToString() : "3"));
                infNFe.AppendChild(emit);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir dados do emitente no XML.", ex);
            }

            #endregion

            #region Destinatário

            try
            {
                // Destinatário
                if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída) // Saída = dados do cliente ou fornecedor
                {
                    if (!(nf.Consumidor && ClienteDAO.Instance.IsConsumidorFinal((uint)cliente.IdCli)))
                    {

                        var cfopDevolucao =
                            CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(nf.IdNaturezaOperacao.Value));
                        int idCidade = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.IdCidade.Value : fornec.IdCidade.Value;
                        bool pj = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.TipoPessoa.ToUpper() == "J" : fornec.TipoPessoa.ToUpper() == "J";
                        bool produtorRural = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.ProdutorRural : fornec.ProdutorRural;
                        string cpfCnpj = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.CpfCnpj : fornec.CpfCnpj;
                        var idEstrangeiro = cliente != null ? cliente.NumEstrangeiro : string.Empty;
                        var suframa = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.Suframa : fornec.Suframa;

                        // A Vidro Metro e a Ponto por algum motivo inverteu a razão social e o nome do cliente no cadastro, por esse motivo foi necessário
                        // fazer a modificação abaixo, para que seja exibida a razão social do cliente na nota
                        string nome = cliente != null && (!cfopDevolucao || fornec == null) ? (FiscalConfig.NotaFiscalConfig.UsarNomeFantasiaNotaFiscal ? cliente.NomeFantasia : cliente.Nome) : fornec.Razaosocial;

                        string endereco = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.Endereco : fornec.Endereco;
                        string numero = cliente != null && (!cfopDevolucao || fornec == null) ? Glass.Formatacoes.RetiraCaracteresEspeciais(cliente.Numero) : Glass.Formatacoes.RetiraCaracteresEspeciais(fornec.Numero);
                        string complemento = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.Compl : String.Empty;
                        string bairro = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.Bairro : fornec.Bairro;
                        string cep = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.Cep : fornec.Cep;
                        string inscEstadual = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.RgEscinst : fornec.RgInscEst;
                        string email = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.EmailFiscal : fornec.Email;
                        string telefone = cliente != null && (!cfopDevolucao || fornec == null) ? cliente.Telefone : fornec.Telcont;

                        Cidade cidadeCli = CidadeDAO.Instance.GetElementByPrimaryKey((uint)idCidade);
                        Pais paisFornec = cliente != null && (!cfopDevolucao || fornec == null) ? PaisDAO.Instance.GetElementByPrimaryKey((uint)cliente.IdPais) : PaisDAO.Instance.GetElementByPrimaryKey((uint)fornec.IdPais);

                        XmlElement dest = doc.CreateElement("dest");

                        if (isExportacao)
                            ManipulacaoXml.SetNode(doc, dest, "idEstrangeiro", Formatacoes.TrataStringDocFiscal(idEstrangeiro));
                        else
                            ManipulacaoXml.SetNode(doc, dest, pj ? "CNPJ" : "CPF", Formatacoes.TrataStringDocFiscal(cpfCnpj));

                        ManipulacaoXml.SetNode(doc, dest, "xNome",
                        nf.TipoAmbiente == 2 ? "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL" : Formatacoes.TrataStringDocFiscal(nome));
                        XmlElement enderDest = doc.CreateElement("enderDest");
                        ManipulacaoXml.SetNode(doc, enderDest, "xLgr", Formatacoes.TrataStringDocFiscal(endereco));
                        ManipulacaoXml.SetNode(doc, enderDest, "nro", Formatacoes.TrataStringDocFiscal(numero));

                        if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(complemento)))
                            ManipulacaoXml.SetNode(doc, enderDest, "xCpl", Formatacoes.TrataStringDocFiscal(complemento));

                        ManipulacaoXml.SetNode(doc, enderDest, "xBairro", Formatacoes.TrataStringDocFiscal(bairro));
                        ManipulacaoXml.SetNode(doc, enderDest, "cMun", cidadeCli.CodUfMunicipio);
                        ManipulacaoXml.SetNode(doc, enderDest, "xMun", Formatacoes.TrataStringDocFiscal(cidadeCli.NomeCidade));
                        ManipulacaoXml.SetNode(doc, enderDest, "UF", cidadeCli.NomeUf);

                        if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(cep)))
                            ManipulacaoXml.SetNode(doc, enderDest, "CEP", Formatacoes.TrataStringDocFiscal(cep));

                        if (paisFornec != null)
                        {
                            ManipulacaoXml.SetNode(doc, enderDest, "cPais", paisFornec.CodPais);
                            ManipulacaoXml.SetNode(doc, enderDest, "xPais", Formatacoes.TrataStringDocFiscal(paisFornec.NomePais));
                        }
                        else
                        {
                            ManipulacaoXml.SetNode(doc, enderDest, "cPais", "1058");
                            ManipulacaoXml.SetNode(doc, enderDest, "xPais", "Brasil");
                        }

                        ManipulacaoXml.SetNode(doc, enderDest, "fone", Formatacoes.TrataStringDocFiscal(telefone, true));
                        dest.AppendChild(enderDest);

                        ManipulacaoXml.SetNode(doc, dest, "indIEDest", ((int)indIeDest).ToString());

                        if (fornec != null && cidadeFornec.CodIbgeUf == "99")
                        {
                        }
                        else
                        {
                            if (indIeDest == IndicadorIEDestinatario.ContribuinteICMS)
                            {
                                if (pj || produtorRural)
                                {
                                    if (String.IsNullOrEmpty(inscEstadual))
                                        throw new Exception("Informe a inscrição estadual do cliente.");

                                    ManipulacaoXml.SetNode(doc, dest, "IE", Formatacoes.TrataStringDocFiscal(inscEstadual.ToUpper()));
                                }
                                else
                                    ManipulacaoXml.SetNode(doc, dest, "IE", "");
                            }
                            else if (indIeDest == IndicadorIEDestinatario.ContribuinteIsento && inscEstadual != null && inscEstadual.ToLower() == "isento" && (pj || produtorRural))
                                ManipulacaoXml.SetNode(doc, dest, "IE", string.Empty);
                            else if (indIeDest == IndicadorIEDestinatario.NaoContribuinte && !string.IsNullOrEmpty(inscEstadual) && (pj || produtorRural))
                                ManipulacaoXml.SetNode(doc, dest, "IE", Formatacoes.TrataStringDocFiscal(inscEstadual.ToUpper()));
                        }

                        if (!nf.Consumidor && !string.IsNullOrEmpty(suframa) && lstProdNf != null &&
                            lstProdNf.Any(f => f.ValorIcmsDesonerado > 0 && f.MotivoDesoneracao == 7))
                            ManipulacaoXml.SetNode(doc, dest, "ISUF", suframa);

                        if (!String.IsNullOrEmpty(email))
                            ManipulacaoXml.SetNode(doc, dest, "email", Formatacoes.TrataStringDocFiscal(email));

                        infNFe.AppendChild(dest);
                    }
                    else if(nf.Consumidor && !string.IsNullOrEmpty(nf.Cpf) && ClienteDAO.Instance.IsConsumidorFinal((uint)cliente.IdCli))
                    {
                        XmlElement dest = doc.CreateElement("dest");
                        ManipulacaoXml.SetNode(doc, dest, "CPF", Formatacoes.TrataStringDocFiscal(nf.Cpf));
                        ManipulacaoXml.SetNode(doc, dest, "indIEDest", ((int)indIeDest).ToString());

                        infNFe.AppendChild(dest);
                    }
                }
                else if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Entrada) // Entrada = dados do cliente ou fornecedor
                {
                    int idCidade = cliente != null ? cliente.IdCidade.Value : fornec.IdCidade.Value;
                    bool pj = cliente != null ? cliente.TipoPessoa.ToUpper() == "J" : fornec.TipoPessoa.ToUpper() == "J";
                    bool produtorRural = cliente != null ? cliente.ProdutorRural : fornec.ProdutorRural;
                    string cpfCnpj = cliente != null ? cliente.CpfCnpj : fornec.CpfCnpj;
                    var suframa = cliente != null ? cliente.Suframa : fornec.Suframa;
                    string nome = cliente != null ? (FiscalConfig.NotaFiscalConfig.UsarNomeFantasiaNotaFiscal ? cliente.NomeFantasia : cliente.Nome) : fornec.Razaosocial;
                    string endereco = cliente != null ? cliente.Endereco : fornec.Endereco;
                    string numero = cliente != null ? Glass.Formatacoes.RetiraCaracteresEspeciais(cliente.Numero) : Glass.Formatacoes.RetiraCaracteresEspeciais(fornec.Numero);
                    string complemento = cliente != null ? cliente.Compl : String.Empty;
                    string bairro = cliente != null ? cliente.Bairro : fornec.Bairro;
                    string cep = cliente != null ? cliente.Cep : fornec.Cep;
                    string inscEstadual = cliente != null ? cliente.RgEscinst : fornec.RgInscEst;
                    string email = cliente != null ? cliente.EmailFiscal : fornec.Email;
                    string telefone = cliente != null ? cliente.Telefone : fornec.Telcont;

                    cidadeFornec = CidadeDAO.Instance.GetElementByPrimaryKey((uint)idCidade);
                    Pais paisFornec = fornec != null ? PaisDAO.Instance.GetElementByPrimaryKey((uint)fornec.IdPais) : null;

                    XmlElement dest = doc.CreateElement("dest");
                    if (fornec != null && cidadeFornec.CodIbgeUf == "99")
                    {
                        if (string.IsNullOrEmpty(fornec.PassaporteDoc))
                            throw new Exception("Campo Documento Estrangeiro, no cadastro do fornecedor, deve ser preenchido.");
                        ManipulacaoXml.SetNode(doc, dest, "idEstrangeiro", fornec.PassaporteDoc);
                    }
                    else
                        ManipulacaoXml.SetNode(doc, dest, pj ? "CNPJ" : "CPF", Formatacoes.TrataStringDocFiscal(cpfCnpj));
                    ManipulacaoXml.SetNode(doc, dest, "xNome",
                    nf.TipoAmbiente == 2 ? "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL" : Formatacoes.TrataStringDocFiscal(nome));
                    XmlElement enderDest = doc.CreateElement("enderDest");
                    ManipulacaoXml.SetNode(doc, enderDest, "xLgr", Formatacoes.TrataStringDocFiscal(endereco));
                    ManipulacaoXml.SetNode(doc, enderDest, "nro", Formatacoes.TrataStringDocFiscal(numero));
                    //ManipulacaoXml.SetNode(doc, enderDest, "xCpl", "");
                    ManipulacaoXml.SetNode(doc, enderDest, "xBairro", Formatacoes.TrataStringDocFiscal(bairro));
                    ManipulacaoXml.SetNode(doc, enderDest, "cMun", cidadeFornec.CodUfMunicipio);
                    ManipulacaoXml.SetNode(doc, enderDest, "xMun", Formatacoes.TrataStringDocFiscal(cidadeFornec.NomeCidade));
                    ManipulacaoXml.SetNode(doc, enderDest, "UF", cidadeFornec.NomeUf);

                    if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(cep)))
                        ManipulacaoXml.SetNode(doc, enderDest, "CEP", Formatacoes.TrataStringDocFiscal(cep));

                    if (paisFornec != null)
                    {
                        ManipulacaoXml.SetNode(doc, enderDest, "cPais", paisFornec.CodPais);
                        ManipulacaoXml.SetNode(doc, enderDest, "xPais", Formatacoes.TrataStringDocFiscal(paisFornec.NomePais));
                    }
                    else
                    {
                        ManipulacaoXml.SetNode(doc, enderDest, "cPais", "1058");
                        ManipulacaoXml.SetNode(doc, enderDest, "xPais", "Brasil");
                    }

                    ManipulacaoXml.SetNode(doc, enderDest, "fone", Formatacoes.TrataStringDocFiscal(telefone, true));
                    dest.AppendChild(enderDest);

                    ManipulacaoXml.SetNode(doc, dest, "indIEDest", ((int)indIeDest).ToString());
                    
                    if (indIeDest == IndicadorIEDestinatario.ContribuinteICMS)
                    {
                        if (inscEstadual != null && (pj || produtorRural))
                            ManipulacaoXml.SetNode(doc, dest, "IE", Formatacoes.TrataStringDocFiscal(inscEstadual.ToUpper()));
                        else
                            ManipulacaoXml.SetNode(doc, dest, "IE", "");
                    }

                    if (!nf.Consumidor && !string.IsNullOrEmpty(suframa) && lstProdNf != null &&
                        lstProdNf.Any(f => f.ValorIcmsDesonerado > 0 && f.MotivoDesoneracao == 7))
                        ManipulacaoXml.SetNode(doc, dest, "ISUF", suframa);

                    if (fornec != null && !String.IsNullOrEmpty(email))
                        ManipulacaoXml.SetNode(doc, dest, "email", Formatacoes.TrataStringDocFiscal(email));

                    infNFe.AppendChild(dest);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir identificação da NFe no XML.", ex);
            }

            // Preenche o CPF/CNPJ do contabilista
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CnpjContabilista"]))
            {
                XmlElement autXML = doc.CreateElement("autXML");
                ManipulacaoXml.SetNode(doc, autXML, "CNPJ", System.Configuration.ConfigurationManager.AppSettings["CnpjContabilista"]);
                infNFe.AppendChild(autXML);
            }
            else if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CpfContabilista"]))
            {
                XmlElement autXML = doc.CreateElement("autXML");
                ManipulacaoXml.SetNode(doc, autXML, "CPF", System.Configuration.ConfigurationManager.AppSettings["CpfContabilista"]);
                infNFe.AppendChild(autXML);
            }

            #endregion

            #region Produtos

            int countProd = 1;

            // Campos usados no totalizador da nota, após somar os valores do DIFAL
            decimal totalIcmsUFDestino = 0;
            decimal totalIcmsUFRemetente = 0;
            decimal totalOutrasDespesasAplicado = 0;
            decimal totalFreteAplicado = 0;
            var contadorPnf = 0;
            var totalIcmsFCP = 0m;

            try
            {
                // Produtos
                foreach (ProdutosNf pnf in lstProdNf)
                {
                    contadorPnf++;
                    float mva = pnf.Mva > 0 ? pnf.Mva :
                        MvaProdutoUfDAO.Instance.ObterMvaPorProduto(null, (int)pnf.IdProd, nf.IdLoja.GetValueOrDefault(), (int?)nf.IdFornec,
                        nf.IdCliente, (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída ||
                        /* Chamado 32984 e 39660. */
                        (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Entrada &&
                        CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(pnf.IdNaturezaOperacao.Value)))));

                    XmlElement det = doc.CreateElement("det");
                    det.SetAttribute("nItem", countProd++.ToString());
                    infNFe.AppendChild(det);

                    // Se o CFOP do produto tiver sido informado, utiliza-o ao invés de utilizar o da NF
                    if (pnf.IdNaturezaOperacao > 0) cfop = CfopDAO.Instance.GetElementByPrimaryKey(pnf.IdCfop.Value);

                    // Verifica se deverá buscar os complementos de impostos da nfe
                    bool nfeAjuste = nf.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste && lstProdNf.Count == 1;
                    bool nfeComplAjuste = (nf.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar ||
                        nf.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste) && lstProdNf.Count == 1;

                    // Recupera a quantidade que deverá ser mostrada na NF
                    decimal qtdPnf =
                        !nfeComplAjuste ?
                            (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(pnf) :
                            /* Chamado 32888. */
                            nf.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar ?
                                (decimal)pnf.Qtde : 1;

                    if (qtdPnf == 0 && nf.FinalidadeEmissao != (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar)
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("O produto " + pnf.CodInterno + " está com quantidade zerada, verifique o tipo de cálculo do mesmo e se todas informações necessárias foram inseridas.", null));

                    // Define se será utilizada qtd tributária diferente da qtd comercial
                    decimal qtdPnfTrib = pnf.QtdeTrib > 0 ? (decimal)pnf.QtdeTrib : qtdPnf;
                    decimal valorUnitTrib =
                        qtdPnfTrib == 0 ?
                            0 : pnf.Total / (decimal)qtdPnfTrib;

                    decimal valorUnitCom =
                        qtdPnf == 0 ?
                            0 : pnf.Total / qtdPnf;

                    // Verifica se será preenchido o valor unitário real
                    if (System.Configuration.ConfigurationManager.AppSettings["ValorUnitRealNota"] == "true")
                    {
                        valorUnitTrib = pnf.ValorUnitarioTrib == 0 ? pnf.ValorUnitario : pnf.ValorUnitarioTrib;
                        valorUnitCom = pnf.ValorUnitario;
                    }

                    // Copia o valor de outras despesas para o produto caso seja nota fiscal complementar
                    if (nf.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar)
                        pnf.ValorOutrasDespesas = nf.OutrasDespesas;

                    var produtoOriginal = ProdutoDAO.Instance.GetElement(pnf.IdProd);

                    XmlElement prod = doc.CreateElement("prod");

                    /* Chamado 38114. */
                    var codInterno =
                        string.Format("{0}{1}", pnf.CodInterno,
                            cliente != null && (cliente.PercReducaoNFe > 0 || cliente.PercReducaoNFeRevenda > 0) ?
                                FiscalConfig.NotaFiscalConfig.CaractereConcatenarCodigoProdutoClienteComReducao :
                                string.Empty);

                    /* Chamado 38113. */
                    if (FiscalConfig.NotaFiscalConfig.ConcatenarPrimeiraLetraBeneficiamentoCodigoProduto)
                    {
                        var caracteresBeneficiamento = new List<char>();
                        
                        // Percorre todos os beneficiamentos dos produtos.
                        foreach (var produtoNfBenef in ProdutoNfBenefDAO.Instance.ObterPeloProdutoNf((int)pnf.IdProdNf))
                            // Caso a descrição do beneficiamento possua alguma informação e o caractere não tenha sido adicionado
                            // à lista de caracteres, então adiciona o primeiro caractere da descrição.
                            if (!string.IsNullOrEmpty(produtoNfBenef.DescrBenef) &&
                                !caracteresBeneficiamento.Contains(produtoNfBenef.DescrBenef.ToUpper().ElementAt(0)))
                                caracteresBeneficiamento.Add(produtoNfBenef.DescrBenef.ToUpper().ElementAt(0));


                        // Concatena os caracteres dos beneficiamentos ao código interno do produto.
                        if (caracteresBeneficiamento.Count > 0)
                            codInterno += string.Join("", caracteresBeneficiamento);
                    }

                    ManipulacaoXml.SetNode(doc, prod, "cProd", Formatacoes.TrataTextoDocFiscal(codInterno));
                    ManipulacaoXml.SetNode(doc, prod, "cEAN", Formatacoes.TrataStringDocFiscal(ProdutoDAO.Instance.ObtemValorCampo<string>("GTINPRODUTO", "idProd=" + pnf.IdProd)));
                    ManipulacaoXml.SetNode(doc, prod, "xProd", nf.TipoAmbiente == 2 ? "NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL" : Formatacoes.TrataTextoDocFiscal(pnf.DescrProduto));
                    ManipulacaoXml.SetNode(doc, prod, "NCM", pnf.Ncm);

                    //Verifica se a loja trabalha no regime normal de tributação
                    var lojaRegimeNormal = loja.Crt == (int)CrtLoja.LucroPresumido || loja.Crt == (int)CrtLoja.LucroReal;

                    //Verifica se o produto da nota, no caso de regime normal de tributação, é de substituição.
                    var produtoRegimeNormalST = pnf.Cst == "10" || pnf.Cst == "30" || pnf.Cst == "60" ||
                        pnf.Cst == "70" || (pnf.Cst == "90" && pnf.ValorIcmsSt > 0);

                    //Verifica se o produto da nota, no caso de regime simples de tributação, é de substituição.
                    var produtoRegimeSimplesST = pnf.Csosn == "201" || pnf.Csosn == "202" || pnf.Csosn == "203" ||
                        pnf.Csosn == "500" || (pnf.Csosn == "900" && pnf.ValorIcmsSt > 0);

                    if (nf.Consumidor &&
                        !string.IsNullOrEmpty(pnf.Csosn) &&
                        pnf.Csosn != "102" &&
                        pnf.Csosn != "103" &&
                        pnf.Csosn != "300" &&
                        pnf.Csosn != "400" &&
                        pnf.Csosn != "500" &&
                        pnf.Csosn != "900")
                        throw new Exception(string.Format("NFC-e não pode ser emitida com o CSOSN {0} associado a algum produto.", pnf.Csosn));

                    if (((lojaRegimeNormal && produtoRegimeNormalST) ||
                        (!lojaRegimeNormal && produtoRegimeSimplesST)) &&
                        produtoOriginal.IdCest > 0)
                    {
                        var cest = CestDAO.Instance.GetElementByPrimaryKey(produtoOriginal.IdCest.Value);

                        if (cest == null)
                            throw new Exception(string.Format("CEST não encontrado. Verifique o Produto {0} - {1}", produtoOriginal.CodInterno, produtoOriginal.Descricao));
                        
                        ManipulacaoXml.SetNode(doc, prod, "CEST", cest.Codigo);
                    }

                    ManipulacaoXml.SetNode(doc, prod, "CFOP", cfop.CodInterno);
                    ManipulacaoXml.SetNode(doc, prod, "uCom", Formatacoes.TrataStringDocFiscal(pnf.Unidade));
                    ManipulacaoXml.SetNode(doc, prod, "qCom", Formatacoes.TrataValorDecimal(qtdPnf, 4));
                    ManipulacaoXml.SetNode(doc, prod, "vUnCom", Formatacoes.TrataValorDecimal(valorUnitCom, 10));
                    ManipulacaoXml.SetNode(doc, prod, "vProd", Formatacoes.TrataValorDecimal(pnf.Total, 2));
                    ManipulacaoXml.SetNode(doc, prod, "cEANTrib", Formatacoes.TrataStringDocFiscal(ProdutoDAO.Instance.ObtemValorCampo<string>("GTINUNIDTRIB", "idProd=" + pnf.IdProd)));
                    ManipulacaoXml.SetNode(doc, prod, "uTrib", Formatacoes.TrataStringDocFiscal(pnf.UnidadeTrib));
                    ManipulacaoXml.SetNode(doc, prod, "qTrib", Formatacoes.TrataValorDecimal(qtdPnfTrib, 4));
                    ManipulacaoXml.SetNode(doc, prod, "vUnTrib", Formatacoes.TrataValorDecimal(valorUnitTrib, 10));

                    // Trata o valor de frete do produto, no XML, para que não ocorra diferença entre o somatório de frete dos produtos com o total de frete da nota.
                    if (nf.ValorFrete > 0 && Formatacoes.TrataValorDecimal(pnf.ValorFrete, 2) != "0.00")
                    {
                        /* Chamado 63752. */
                        var valorFrete = Math.Round(pnf.ValorFrete, 2);
                        totalFreteAplicado += valorFrete;

                        if (contadorPnf == lstProdNf.Count() && Math.Abs(nf.ValorFrete - totalFreteAplicado) <= (decimal)0.3)
                            valorFrete += (nf.ValorFrete - totalFreteAplicado);

                        ManipulacaoXml.SetNode(doc, prod, "vFrete", Formatacoes.TrataValorDecimal(valorFrete, 2));
                    }

                    if (nf.ValorSeguro > 0) ManipulacaoXml.SetNode(doc, prod, "vSeg", Formatacoes.TrataValorDecimal(pnf.ValorSeguro, 2));
                    if (Formatacoes.TrataValorDecimal(pnf.ValorDesconto, 2) != "0.00") ManipulacaoXml.SetNode(doc, prod, "vDesc", Formatacoes.TrataValorDecimal(pnf.ValorDesconto, 2));

                    // Trata o valor de outras despesas do produto, no XML, para que não ocorra diferença entre o somatório de outras despesas dos produtos com o total de outras despesas da nota.
                    if (nf.OutrasDespesas > 0 && Formatacoes.TrataValorDecimal(pnf.ValorOutrasDespesas, 2) != "0.00")
                    {
                        /* Chamado 49684. */
                        var valorOutrasDespesas = Math.Round(pnf.ValorOutrasDespesas, 2);
                        totalOutrasDespesasAplicado += valorOutrasDespesas;
                        if (contadorPnf == lstProdNf.Count() && Math.Abs(nf.OutrasDespesas - totalOutrasDespesasAplicado) <= (decimal)0.3)
                            valorOutrasDespesas += (nf.OutrasDespesas - totalOutrasDespesasAplicado);

                        ManipulacaoXml.SetNode(doc, prod, "vOutro", Formatacoes.TrataValorDecimal(valorOutrasDespesas, 2));
                    }

                    ManipulacaoXml.SetNode(doc, prod, "indTot", nf.TotalProd == 0 && nfeComplAjuste ? "0" : "1"); // Indica se o valor do item compões a NF, 0-Não Compõe, 1-Compõe
                    det.AppendChild(prod);

                    #region DI

                    if (isImportacao)
                    {
                        // Cria tag da Declaração de Importação
                        XmlElement di = doc.CreateElement("DI");
                        ManipulacaoXml.SetNode(doc, di, "nDI", Formatacoes.TrataStringDocFiscal(pnf.NumDocImp));
                        ManipulacaoXml.SetNode(doc, di, "dDI", pnf.DataRegDocImp.Value.ToString("yyyy-MM-dd"));
                        ManipulacaoXml.SetNode(doc, di, "xLocDesemb", Formatacoes.TrataStringDocFiscal(pnf.LocalDesembaraco));
                        ManipulacaoXml.SetNode(doc, di, "UFDesemb", pnf.UfDesembaraco);
                        ManipulacaoXml.SetNode(doc, di, "dDesemb", pnf.DataDesembaraco.Value.ToString("yyyy-MM-dd"));
                        ManipulacaoXml.SetNode(doc, di, "tpViaTransp", ((int)pnf.TpViaTransp).ToString());
                        if (pnf.TpViaTransp == ViaTransporteInternacional.Maritima)
                        {
                            //if (pnf.VAFRMM == 0)
                            //    throw new Exception("Para via de transporte marítima, vafrmm deve ser preenchido");
                            ManipulacaoXml.SetNode(doc, di, "vAFRMM", Formatacoes.TrataValorDecimal(pnf.VAFRMM, 2));
                        }
                        if (pnf.TpIntermedio != 0)
                            ManipulacaoXml.SetNode(doc, di, "tpIntermedio", ((int)pnf.TpIntermedio).ToString());
                        if (pnf.TpIntermedio == TpIntermedio.ImportacaoContaOrdem || pnf.TpIntermedio == TpIntermedio.ImportacaoEncomenda)
                        {
                            if (string.IsNullOrEmpty(pnf.CnpjAdquirenteEncomendante) || string.IsNullOrEmpty(pnf.UfTerceiro))
                                throw new Exception("Para TpIntemedio 'ImportacaoContaOrdem' ou 'ImportacaoEncomenda', deve ser preenchido cnpj do adquirente encomendante e uf terceiro.");
                            ManipulacaoXml.SetNode(doc, di, "CNPJ", pnf.CnpjAdquirenteEncomendante);
                            ManipulacaoXml.SetNode(doc, di, "UFTerceiro", pnf.UfTerceiro);
                        }
                        ManipulacaoXml.SetNode(doc, di, "cExportador", Formatacoes.TrataStringDocFiscal(pnf.CodExportador));
                        prod.AppendChild(di);

                        // Cria tags de Adição
                        foreach (ProdutoNfAdicao pnfa in ProdutoNfAdicaoDAO.Instance.GetByProdNf(pnf.IdProdNf))
                        {
                            XmlElement adi = doc.CreateElement("adi");
                            ManipulacaoXml.SetNode(doc, adi, "nAdicao", pnfa.NumAdicao.ToString());
                            ManipulacaoXml.SetNode(doc, adi, "nSeqAdic", pnfa.NumSeqAdicao.ToString());
                            ManipulacaoXml.SetNode(doc, adi, "cFabricante", Formatacoes.TrataStringDocFiscal(pnfa.CodFabricante));

                            if (pnfa.DescAdicao > 0)
                                ManipulacaoXml.SetNode(doc, adi, "vDescDI", Formatacoes.TrataValorDouble(pnfa.DescAdicao, 2));

                            di.AppendChild(adi);
                        }
                    }

                    #endregion

                    #region Exportação

                    if (isExportacao && !string.IsNullOrWhiteSpace(pnf.NumACDrawback))
                    {
                        // Cria tag da Declaração de Importação
                        XmlElement detExport = doc.CreateElement("detExport");
                        ManipulacaoXml.SetNode(doc, detExport, "nDraw", Formatacoes.TrataStringDocFiscal(pnf.NumACDrawback));

                        if(!string.IsNullOrWhiteSpace(pnf.NumRegExportacao) || !string.IsNullOrWhiteSpace(pnf.ChaveAcessoExportacao) || pnf.QtdeExportada > 0)
                        {
                            XmlElement exportInd = doc.CreateElement("exportInd");

                            ManipulacaoXml.SetNode(doc, exportInd, "nRE", Formatacoes.TrataStringDocFiscal(pnf.NumRegExportacao));
                            ManipulacaoXml.SetNode(doc, exportInd, "chNFe", Formatacoes.TrataStringDocFiscal(pnf.ChaveAcessoExportacao));
                            ManipulacaoXml.SetNode(doc, exportInd, "qExport", Formatacoes.TrataValorDecimal(pnf.QtdeExportada, 4));

                            detExport.AppendChild(exportInd);
                        }

                        prod.AppendChild(detExport);
                    }

                    #endregion

                    XmlElement imposto = doc.CreateElement("imposto");
                    det.AppendChild(imposto);

                    // Insere o total de tributos no XML, conforme lei da transparência
                    if (pnf.ValorTotalTrib > 0)
                        ManipulacaoXml.SetNode(doc, imposto, "vTotTrib", Formatacoes.TrataValorDecimal(pnf.ValorTotalTrib, 2));
                    
                    #region ICMS

                    XmlElement icms = doc.CreateElement("ICMS");
                    imposto.AppendChild(icms);

                    // Busca os valores referentes ao icms e icmsst, de acordo com a finalidade da nota
                    decimal bcIcms = !nfeComplAjuste ? pnf.BcIcms : nf.BcIcms;
                    decimal aliqIcms = (decimal)pnf.AliqIcms;
                    decimal valorIcms = !nfeComplAjuste ? pnf.ValorIcms : nf.Valoricms;
                    decimal bcIcmsSt = !nfeComplAjuste ? pnf.BcIcmsSt : nf.BcIcmsSt;
                    decimal aliqIcmsSt = !nfeComplAjuste ? (decimal)pnf.AliqIcmsSt : (nf.ValorIcmsSt > 0 && nf.BcIcmsSt > 0 ? nf.ValorIcmsSt / nf.BcIcmsSt : 0);
                    decimal valorIcmsSt = !nfeComplAjuste ? pnf.ValorIcmsSt : nf.ValorIcmsSt;

                    // Busca os valores referente ao FCP.
                    decimal bcFcp = !nfeComplAjuste ? pnf.BcFcp : nf.BcFcp;
                    decimal aliqFcp = (decimal)pnf.AliqFcp;
                    decimal valorFcp = !nfeComplAjuste ? pnf.ValorFcp : nf.ValorFcp;
                    decimal bcFcpSt = !nfeComplAjuste ? pnf.BcFcpSt : nf.BcFcpSt;
                    decimal aliqFcpSt = !nfeComplAjuste ? (decimal)pnf.AliqFcpSt : (nf.ValorFcpSt > 0 && nf.BcFcpSt > 0 ? nf.ValorFcpSt / nf.BcFcpSt : 0);
                    decimal valorFcpSt = !nfeComplAjuste ? pnf.ValorFcpSt : nf.ValorFcpSt;
                    decimal bcFcpStRet = 0;
                    decimal aliqFcpStRet = 0;
                    decimal valorFcpStRet = 0;

                    // Busca os valores referentes ao ipi, de acordo com a finalidade da nota
                    decimal bcIpi = !nfeComplAjuste ? pnf.Total : nf.TotalNota;
                    decimal aliqIpi = (decimal)pnf.AliqIpi;
                    decimal valorIpi = !nfeComplAjuste ? pnf.ValorIpi : nf.ValorIpi;

                    /* Chamado 22263. */
                    if (!nfeComplAjuste && valorIpi == 0)
                        bcIpi = 0;

                    // Se for NFe Complementar, atualiza valores de imposto no produto
                    if (nfeComplAjuste && lstProdNf.Count == 1 && !isImportacao)
                    {
                        objPersistence.ExecuteCommand("Update produtos_nf Set bcIcms=" + bcIcms.ToString().Replace(",", ".") +
                            ", aliqIcms=" + aliqIcms.ToString().Replace(",", ".") + ", valorIcms=" + valorIcms.ToString().Replace(",", ".") +
                            ", bcIcmsSt=" + bcIcmsSt.ToString().Replace(",", ".") + ", aliqIcmsSt=" +
                            aliqIcmsSt.ToString().Replace(",", ".") + ", valorIcmsSt=" + valorIcmsSt.ToString().Replace(",", ".") +
                            ", aliqIpi=" + aliqIpi.ToString().Replace(",", ".") + ", valorIpi=" + valorIpi.ToString().Replace(",", ".") +
                            " Where idProdNf=" + pnf.IdProdNf);
                    }

                    // Se for regime normal ou nota de entrada de terceiros
                    if (loja.Crt == 3 || loja.Crt == 4 || nf.TipoDocumento == 3)
                    {
                        switch (pnf.Cst)
                        {
                            case "00":
                                XmlElement icms00 = doc.CreateElement("ICMS00");
                                ManipulacaoXml.SetNode(doc, icms00, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icms00, "CST", "00");
                                ManipulacaoXml.SetNode(doc, icms00, "modBC", "3"); // 0-MVA (%), 1-Pauta (Valor), 2-Preço Tabelado Máx. (Valor), 3-Valor da Operação
                                ManipulacaoXml.SetNode(doc, icms00, "vBC", Formatacoes.TrataValorDecimal(bcIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms00, "pICMS", Formatacoes.TrataValorDecimal(aliqIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms00, "vICMS", Formatacoes.TrataValorDecimal(valorIcms, 2));
                                if (aliqFcp > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms00, "pFCP", Formatacoes.TrataValorDecimal(aliqFcp, 2));
                                    ManipulacaoXml.SetNode(doc, icms00, "vFCP", Formatacoes.TrataValorDecimal(valorFcp, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP {0}, Valor FCP {1} ({2}%);",
                                        bcFcp.ToString("C"), valorFcp.ToString("C"), aliqFcp);
                                }
                                icms.AppendChild(icms00);
                                break;
                            case "10":
                                XmlElement icms10 = doc.CreateElement("ICMS10");
                                ManipulacaoXml.SetNode(doc, icms10, "orig", pnf.CstOrig.ToString());
                                ManipulacaoXml.SetNode(doc, icms10, "CST", "10");
                                ManipulacaoXml.SetNode(doc, icms10, "modBC", "3"); // 0-MVA (%), 1-Pauta (Valor), 2-Preço Tabelado Máx. (Valor), 3-Valor da Operação
                                ManipulacaoXml.SetNode(doc, icms10, "vBC", Formatacoes.TrataValorDecimal(bcIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms10, "pICMS", Formatacoes.TrataValorDecimal(aliqIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms10, "vICMS", Formatacoes.TrataValorDecimal(valorIcms, 2));
                                if (aliqFcp > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms10, "vBCFCP", Formatacoes.TrataValorDecimal(bcFcp, 2));
                                    ManipulacaoXml.SetNode(doc, icms10, "pFCP", Formatacoes.TrataValorDecimal(aliqFcp, 2));
                                    ManipulacaoXml.SetNode(doc, icms10, "vFCP", Formatacoes.TrataValorDecimal(valorFcp, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP {0}, Valor FCP {1} ({2}%);",
                                        bcFcp.ToString("C"), valorFcp.ToString("C"), aliqFcp);
                                }
                                ManipulacaoXml.SetNode(doc, icms10, "modBCST", "4"); // 0-Preço tabelado ou máximo sugerido, 1-Lista negativa, 2-Lista positiva, 3-Lista Neutra, 4-Margem valor agregado, 5-Pauta
                                if (!nfeComplAjuste) ManipulacaoXml.SetNode(doc, icms10, "pMVAST", Formatacoes.TrataValorDecimal((decimal)mva, 2));
                                ManipulacaoXml.SetNode(doc, icms10, "vBCST", Formatacoes.TrataValorDecimal(bcIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icms10, "pICMSST", Formatacoes.TrataValorDecimal(aliqIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icms10, "vICMSST", Formatacoes.TrataValorDecimal(valorIcmsSt, 2));
                                if (aliqFcpSt > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms10, "vBCFCPST", Formatacoes.TrataValorDecimal(bcFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icms10, "pFCPST", Formatacoes.TrataValorDecimal(aliqFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icms10, "vFCPST", Formatacoes.TrataValorDecimal(valorFcpSt, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP ST {0}, Valor FCP ST {1} ({2}%);",
                                        bcFcpSt.ToString("C"), valorFcpSt.ToString("C"), aliqFcpSt);
                                }
                                icms.AppendChild(icms10);
                                break;
                            case "20":
                                XmlElement icms20 = doc.CreateElement("ICMS20");
                                ManipulacaoXml.SetNode(doc, icms20, "orig", pnf.CstOrig.ToString());
                                ManipulacaoXml.SetNode(doc, icms20, "CST", "20");
                                ManipulacaoXml.SetNode(doc, icms20, "modBC", "3"); // 0-MVA (%), 1-Pauta (Valor), 2-Preço Tabelado Máx. (Valor), 3-Valor da Operação
                                ManipulacaoXml.SetNode(doc, icms20, "pRedBC", Formatacoes.TrataValorDouble(pnf.PercRedBcIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms20, "vBC", Formatacoes.TrataValorDecimal(pnf.BcIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms20, "pICMS", Formatacoes.TrataValorDouble(pnf.AliqIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms20, "vICMS", Formatacoes.TrataValorDecimal(pnf.ValorIcms, 2));
                                if (aliqFcp > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms20, "vBCFCP", Formatacoes.TrataValorDecimal(bcFcp, 2));
                                    ManipulacaoXml.SetNode(doc, icms20, "pFCP", Formatacoes.TrataValorDecimal(aliqFcp, 2));
                                    ManipulacaoXml.SetNode(doc, icms20, "vFCP", Formatacoes.TrataValorDecimal(valorFcp, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP {0}, Valor FCP {1} ({2}%);",
                                        bcFcp.ToString("C"), valorFcp.ToString("C"), aliqFcp);
                                }
                                if (pnf.ValorIcmsDesonerado > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms20, "vICMSDeson", Formatacoes.TrataValorDecimal(pnf.ValorIcmsDesonerado, 2));
                                    ManipulacaoXml.SetNode(doc, icms20, "motDesICMS", ((int)pnf.MotivoDesoneracao).ToString());
                                }
                                icms.AppendChild(icms20);
                                break;
                            case "30":
                                XmlElement icms30 = doc.CreateElement("ICMS30");
                                ManipulacaoXml.SetNode(doc, icms30, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icms30, "CST", "30");
                                ManipulacaoXml.SetNode(doc, icms30, "modBCST", "3"); // 0-MVA (%), 1-Pauta (Valor), 2-Preço Tabelado Máx. (Valor), 3-Valor da Operação
                                if (!nfeComplAjuste) ManipulacaoXml.SetNode(doc, icms30, "pMVAST", Formatacoes.TrataValorDecimal((decimal)pnf.Mva, 2));
                                ManipulacaoXml.SetNode(doc, icms30, "vBCST", Formatacoes.TrataValorDecimal(bcIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icms30, "pICMSST", Formatacoes.TrataValorDecimal(aliqIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icms30, "vICMSST", Formatacoes.TrataValorDecimal(valorIcmsSt, 2));
                                if (aliqFcpSt > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms30, "vBCFCPST", Formatacoes.TrataValorDecimal(bcFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icms30, "pFCPST", Formatacoes.TrataValorDecimal(aliqFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icms30, "vFCPST", Formatacoes.TrataValorDecimal(valorFcpSt, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP ST {0}, Valor FCP ST {1} ({2}%);",
                                        bcFcpSt.ToString("C"), valorFcpSt.ToString("C"), aliqFcpSt);
                                }
                                if (pnf.ValorIcmsDesonerado > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms30, "vICMSDeson", Formatacoes.TrataValorDecimal(pnf.ValorIcmsDesonerado, 2));
                                    ManipulacaoXml.SetNode(doc, icms30, "motDesICMS", ((int)pnf.MotivoDesoneracao).ToString());
                                }
                                icms.AppendChild(icms30);
                                break;
                            case "40":
                            case "41":
                            case "50":
                                XmlElement icms40 = doc.CreateElement("ICMS40");
                                ManipulacaoXml.SetNode(doc, icms40, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icms40, "CST", pnf.Cst);
                                if (pnf.ValorIcmsDesonerado > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms40, "vICMSDeson", Formatacoes.TrataValorDecimal(pnf.ValorIcmsDesonerado, 2));
                                    ManipulacaoXml.SetNode(doc, icms40, "motDesICMS", ((int)pnf.MotivoDesoneracao).ToString());
                                }
                                icms.AppendChild(icms40);
                                break;
                            case "51":
                                // Calcula o valor do ICMS sem diferimento, o valor diferido, e o valor final (o primeiro menos o segundo)
                                var valorIcmsSemDiferimento = Math.Round(pnf.BcIcms * (decimal)(pnf.AliqIcms / 100), 2, MidpointRounding.AwayFromZero);
                                var valorIcmsDiferido = Math.Round(valorIcmsSemDiferimento * ((decimal)pnf.PercDiferimento / 100), 2, MidpointRounding.AwayFromZero);
                                valorIcms = valorIcmsSemDiferimento - valorIcmsDiferido;

                                XmlElement icms51 = doc.CreateElement("ICMS51");
                                ManipulacaoXml.SetNode(doc, icms51, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icms51, "CST", "51");
                                ManipulacaoXml.SetNode(doc, icms51, "modBC", "3"); // 0-MVA (%), 1-Pauta (Valor), 2-Preço Tabelado Máx. (Valor), 3-Valor da Operação
                                ManipulacaoXml.SetNode(doc, icms51, "pRedBC", Formatacoes.TrataValorDouble(pnf.PercRedBcIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms51, "vBC", Formatacoes.TrataValorDecimal(pnf.BcIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms51, "pICMS", Formatacoes.TrataValorDouble(pnf.AliqIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms51, "vICMSOp", Formatacoes.TrataValorDecimal(valorIcmsSemDiferimento, 2));
                                ManipulacaoXml.SetNode(doc, icms51, "pDif", Formatacoes.TrataValorDouble(pnf.PercDiferimento, 2));
                                ManipulacaoXml.SetNode(doc, icms51, "vICMSDif", Formatacoes.TrataValorDecimal(valorIcmsDiferido, 2));
                                ManipulacaoXml.SetNode(doc, icms51, "vICMS", Formatacoes.TrataValorDecimal(valorIcms, 2));
                                icms.AppendChild(icms51);
                                break;
                            case "60":
                                XmlElement icms60 = doc.CreateElement("ICMS60");
                                ManipulacaoXml.SetNode(doc, icms60, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icms60, "CST", "60");
                                ManipulacaoXml.SetNode(doc, icms60, "vBCSTRet", Formatacoes.TrataValorDecimal(0, 2)); // TODO: Como obter este campo?
                                // Alíquota suportada pelo Consumidor Final - alíquota do cálculo do ICMS-ST, já incluso o FCP
                                ManipulacaoXml.SetNode(doc, icms60, "pST", Formatacoes.TrataValorDecimal(0, 2));
                                ManipulacaoXml.SetNode(doc, icms60, "vICMSSTRet", Formatacoes.TrataValorDecimal(0, 2));  // TODO: Como obter este campo?
                                if (aliqFcpStRet > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms60, "vBCFCPSTRet", Formatacoes.TrataValorDecimal(0, 2));
                                    ManipulacaoXml.SetNode(doc, icms60, "pFCPSTRet", Formatacoes.TrataValorDecimal(0, 2));
                                    ManipulacaoXml.SetNode(doc, icms60, "vFCPSTRet", Formatacoes.TrataValorDecimal(0, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP ST Ret {0}, Valor FCP ST Ret {1} ({2}%);",
                                        bcFcpStRet.ToString("C"), valorFcpStRet.ToString("C"), aliqFcpStRet);
                                }
                                icms.AppendChild(icms60);
                                break;
                            case "70":
                                XmlElement icms70 = doc.CreateElement("ICMS70");
                                ManipulacaoXml.SetNode(doc, icms70, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icms70, "CST", "70");
                                ManipulacaoXml.SetNode(doc, icms70, "modBC", "3"); // 0-MVA (%), 1-Pauta (Valor), 2-Preço Tabelado Máx. (Valor), 3-Valor da Operação
                                ManipulacaoXml.SetNode(doc, icms70, "pRedBC", Formatacoes.TrataValorDouble(pnf.PercRedBcIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms70, "vBC", Formatacoes.TrataValorDecimal(bcIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms70, "pICMS", Formatacoes.TrataValorDecimal(aliqIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms70, "vICMS", Formatacoes.TrataValorDecimal(valorIcms, 2));
                                if (aliqFcp > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms70, "vBCFCP", Formatacoes.TrataValorDecimal(bcFcp, 2));
                                    ManipulacaoXml.SetNode(doc, icms70, "pFCP", Formatacoes.TrataValorDecimal(aliqFcp, 2));
                                    ManipulacaoXml.SetNode(doc, icms70, "vFCP", Formatacoes.TrataValorDecimal(valorFcp, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP {0}, Valor FCP {1} ({2}%);",
                                        bcFcp.ToString("C"), valorFcp.ToString("C"), aliqFcp);
                                }
                                ManipulacaoXml.SetNode(doc, icms70, "modBCST", "4"); // 0-Preço tabelado ou máximo sugerido, 1-Lista negativa, 2-Lista positiva, 3-Lista Neutra, 4-Margem valor agregado, 5-Pauta
                                if (pnf.Mva > 0)
                                    ManipulacaoXml.SetNode(doc, icms70, "pMVAST", Formatacoes.TrataValorDouble(pnf.Mva, 2));
                                if (pnf.PercRedBcIcmsSt > 0) ManipulacaoXml.SetNode(doc, icms70, "pRedBCST", Formatacoes.TrataValorDouble(pnf.PercRedBcIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icms70, "vBCST", Formatacoes.TrataValorDecimal(bcIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icms70, "pICMSST", Formatacoes.TrataValorDecimal(aliqIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icms70, "vICMSST", Formatacoes.TrataValorDecimal(valorIcmsSt, 2));
                                if (aliqFcpSt > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms70, "vBCFCPST", Formatacoes.TrataValorDecimal(bcFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icms70, "pFCPST", Formatacoes.TrataValorDecimal(aliqFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icms70, "vFCPST", Formatacoes.TrataValorDecimal(valorFcpSt, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP ST {0}, Valor FCP ST {1} ({2}%);",
                                        bcFcpSt.ToString("C"), valorFcpSt.ToString("C"), aliqFcpSt);
                                }
                                if (pnf.ValorIcmsDesonerado > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms70, "vICMSDeson", Formatacoes.TrataValorDecimal(pnf.ValorIcmsDesonerado, 2));
                                    ManipulacaoXml.SetNode(doc, icms70, "motDesICMS", ((int)pnf.MotivoDesoneracao).ToString());
                                }
                                icms.AppendChild(icms70);
                                break;
                            case "90":
                                XmlElement icms90 = doc.CreateElement("ICMS90");
                                ManipulacaoXml.SetNode(doc, icms90, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icms90, "CST", "90");
                                ManipulacaoXml.SetNode(doc, icms90, "modBC", "3"); // 0-MVA (%), 1-Pauta (Valor), 2-Preço Tabelado Máx. (Valor), 3-Valor da Operação
                                ManipulacaoXml.SetNode(doc, icms90, "vBC", Formatacoes.TrataValorDecimal(bcIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms90, "pICMS", Formatacoes.TrataValorDecimal(aliqIcms, 2));
                                ManipulacaoXml.SetNode(doc, icms90, "vICMS", Formatacoes.TrataValorDecimal(valorIcms, 2));
                                if (aliqFcp > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms90, "vBCFCP", Formatacoes.TrataValorDecimal(bcFcp, 2));
                                    ManipulacaoXml.SetNode(doc, icms90, "pFCP", Formatacoes.TrataValorDecimal(aliqFcp, 2));
                                    ManipulacaoXml.SetNode(doc, icms90, "vFCP", Formatacoes.TrataValorDecimal(valorFcp, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP {0}, Valor FCP {1} ({2}%);",
                                        bcFcp.ToString("C"), valorFcp.ToString("C"), aliqFcp);
                                }
                                ManipulacaoXml.SetNode(doc, icms90, "modBCST", "4"); // 0-Preço tabelado ou máximo sugerido, 1-Lista negativa, 2-Lista positiva, 3-Lista Neutra, 4-Margem valor agregado, 5-Pauta
                                ManipulacaoXml.SetNode(doc, icms90, "vBCST", Formatacoes.TrataValorDecimal(bcIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icms90, "pICMSST", Formatacoes.TrataValorDecimal(aliqIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icms90, "vICMSST", Formatacoes.TrataValorDecimal(valorIcmsSt, 2));
                                if (aliqFcpSt > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms90, "vBCFCPST", Formatacoes.TrataValorDecimal(bcFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icms90, "pFCPST", Formatacoes.TrataValorDecimal(aliqFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icms90, "vFCPST", Formatacoes.TrataValorDecimal(valorFcpSt, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP ST {0}, Valor FCP ST {1} ({2}%);",
                                        bcFcpSt.ToString("C"), valorFcpSt.ToString("C"), aliqFcpSt);
                                }
                                if (pnf.ValorIcmsDesonerado > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icms90, "vICMSDeson", Formatacoes.TrataValorDecimal(pnf.ValorIcmsDesonerado, 2));
                                    ManipulacaoXml.SetNode(doc, icms90, "motDesICMS", ((int)pnf.MotivoDesoneracao).ToString());
                                }
                                icms.AppendChild(icms90);
                                break;
                            case "":
                                throw new Exception("Informe o CST de todos os produtos.");
                            default:
                                throw new Exception("CST informada no produto não existe.");
                        }
                    }
                    else // Simples Nacional
                    {
                        switch (pnf.Csosn)
                        {
                            case "101":
                                XmlElement icmsSn101 = doc.CreateElement("ICMSSN101");
                                ManipulacaoXml.SetNode(doc, icmsSn101, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icmsSn101, "CSOSN", pnf.Csosn);
                                ManipulacaoXml.SetNode(doc, icmsSn101, "pCredSN", Formatacoes.TrataValorDouble(aliqICMSSN, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn101, "vCredICMSSN", Formatacoes.TrataValorDecimal(pnf.Total * ((decimal)aliqICMSSN / 100), 2));
                                icms.AppendChild(icmsSn101);
                                break;
                            case "102":
                            case "103":
                            case "300":
                            case "400":
                                XmlElement icmsSn102 = doc.CreateElement("ICMSSN102");
                                ManipulacaoXml.SetNode(doc, icmsSn102, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icmsSn102, "CSOSN", pnf.Csosn);
                                icms.AppendChild(icmsSn102);
                                break;
                            case "201":
                                XmlElement icmsSn201 = doc.CreateElement("ICMSSN201");
                                ManipulacaoXml.SetNode(doc, icmsSn201, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icmsSn201, "CSOSN", pnf.Csosn);
                                ManipulacaoXml.SetNode(doc, icmsSn201, "modBCST", "4"); // 4-MVA
                                if (mva > 0) ManipulacaoXml.SetNode(doc, icmsSn201, "pMVAST", Formatacoes.TrataValorDouble(mva, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn201, "vBCST", Formatacoes.TrataValorDecimal(bcIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn201, "pICMSST", Formatacoes.TrataValorDecimal(aliqIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn201, "vICMSST", Formatacoes.TrataValorDecimal(valorIcmsSt, 2));
                                if (aliqFcpSt > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icmsSn201, "vBCFCPST", Formatacoes.TrataValorDecimal(bcFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icmsSn201, "pFCPST", Formatacoes.TrataValorDecimal(aliqFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icmsSn201, "vFCPST", Formatacoes.TrataValorDecimal(valorFcpSt, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP ST {0}, Valor FCP ST {1} ({2}%);",
                                        bcFcpSt.ToString("C"), valorFcpSt.ToString("C"), aliqFcpSt);
                                }
                                ManipulacaoXml.SetNode(doc, icmsSn201, "pCredSN", Formatacoes.TrataValorDouble(aliqICMSSN, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn201, "vCredICMSSN", Formatacoes.TrataValorDecimal(pnf.Total * ((decimal)aliqICMSSN / 100), 2));
                                icms.AppendChild(icmsSn201);
                                break;
                            case "202":
                            case "203":
                                XmlElement icmsSn202 = doc.CreateElement("ICMSSN202");
                                ManipulacaoXml.SetNode(doc, icmsSn202, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icmsSn202, "CSOSN", pnf.Csosn);
                                ManipulacaoXml.SetNode(doc, icmsSn202, "modBCST", "4"); // 4-MVA
                                if (mva > 0) ManipulacaoXml.SetNode(doc, icmsSn202, "pMVAST", Formatacoes.TrataValorDouble(mva, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn202, "vBCST", Formatacoes.TrataValorDecimal(bcIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn202, "pICMSST", Formatacoes.TrataValorDecimal(aliqIcmsSt, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn202, "vICMSST", Formatacoes.TrataValorDecimal(valorIcmsSt, 2));
                                if (aliqFcpSt > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icmsSn202, "vBCFCPST", Formatacoes.TrataValorDecimal(bcFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icmsSn202, "pFCPST", Formatacoes.TrataValorDecimal(aliqFcpSt, 2));
                                    ManipulacaoXml.SetNode(doc, icmsSn202, "vFCPST", Formatacoes.TrataValorDecimal(valorFcpSt, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP ST {0}, Valor FCP ST {1} ({2}%);",
                                        bcFcpSt.ToString("C"), valorFcpSt.ToString("C"), aliqFcpSt);
                                }
                                icms.AppendChild(icmsSn202);
                                break;
                            case "500":
                                XmlElement icmsSn500 = doc.CreateElement("ICMSSN500");
                                ManipulacaoXml.SetNode(doc, icmsSn500, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icmsSn500, "CSOSN", pnf.Csosn);
                                ManipulacaoXml.SetNode(doc, icmsSn500, "vBCSTRet", Formatacoes.TrataValorDecimal(ProdutosNfDAO.Instance.GetLastBcIcmsSt(pnf.IdProdNf), 2)); // TODO: Verificar se o preenchimento deste campo está correto

                                // Alíquota suportada pelo Consumidor Final - alíquota do cálculo do ICMS-ST, já incluso o FCP
                                ManipulacaoXml.SetNode(doc, icmsSn500, "pST", Formatacoes.TrataValorDecimal(0, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn500, "vICMSSTRet", Formatacoes.TrataValorDecimal(ProdutosNfDAO.Instance.GetLastIcmsSt(pnf.IdProdNf), 2)); // TODO: Verificar se o preenchimento deste campo está correto
                                if (aliqFcpStRet > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icmsSn500, "vBCFCPSTRet", Formatacoes.TrataValorDecimal(0, 2));
                                    ManipulacaoXml.SetNode(doc, icmsSn500, "pFCPSTRet", Formatacoes.TrataValorDecimal(0, 2));
                                    ManipulacaoXml.SetNode(doc, icmsSn500, "vFCPSTRet", Formatacoes.TrataValorDecimal(0, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP ST Ret {0}, Valor FCP ST Ret {1} ({2}%);",
                                        bcFcpStRet.ToString("C"), valorFcpStRet.ToString("C"), aliqFcpStRet);
                                }
                                icms.AppendChild(icmsSn500);
                                break;
                            case "900":
                                bool calcIcms = NaturezaOperacaoDAO.Instance.CalculaIcms(null, pnf.IdNaturezaOperacao.Value);
                                bool calcIcmsSt = NaturezaOperacaoDAO.Instance.CalculaIcmsSt(null, pnf.IdNaturezaOperacao.Value);

                                XmlElement icmsSn900 = doc.CreateElement("ICMSSN900");
                                ManipulacaoXml.SetNode(doc, icmsSn900, "orig", pnf.CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(doc, icmsSn900, "CSOSN", pnf.Csosn);

                                // Calcula ICMS
                                ManipulacaoXml.SetNode(doc, icmsSn900, "modBC", "0"); // 0-MVA
                                ManipulacaoXml.SetNode(doc, icmsSn900, "vBC", Formatacoes.TrataValorDecimal(calcIcms ? bcIcms : 0, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn900, "pICMS", Formatacoes.TrataValorDecimal(calcIcms ? aliqIcms : 0, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn900, "vICMS", Formatacoes.TrataValorDecimal(calcIcms ? valorIcms : 0, 2));

                                // Calcula ICMS ST
                                ManipulacaoXml.SetNode(doc, icmsSn900, "modBCST", "4"); // 4-MVA
                                if (mva > 0) ManipulacaoXml.SetNode(doc, icmsSn900, "pMVAST", Formatacoes.TrataValorDouble(mva, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn900, "vBCST", Formatacoes.TrataValorDecimal(calcIcmsSt ? bcIcmsSt : 0, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn900, "pICMSST", Formatacoes.TrataValorDecimal(calcIcmsSt ? aliqIcmsSt : 0, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn900, "vICMSST", Formatacoes.TrataValorDecimal(calcIcmsSt ? valorIcmsSt : 0, 2));
                                if (calcIcmsSt && aliqFcpSt > 0)
                                {
                                    ManipulacaoXml.SetNode(doc, icmsSn900, "vBCFCPST", Formatacoes.TrataValorDecimal(calcIcmsSt ? bcFcpSt : 0, 2));
                                    ManipulacaoXml.SetNode(doc, icmsSn900, "pFCPST", Formatacoes.TrataValorDecimal(calcIcmsSt ? aliqFcpSt : 0, 2));
                                    ManipulacaoXml.SetNode(doc, icmsSn900, "vFCPST", Formatacoes.TrataValorDecimal(calcIcmsSt ? valorFcpSt : 0, 2));

                                    pnf.InfAdic += string.Format(" Base cálculo FCP ST {0}, Valor FCP ST {1} ({2}%);",
                                        bcFcpSt.ToString("C"), valorFcpSt.ToString("C"), aliqFcpSt);
                                }
                                // Calcula alíquota do simples
                                //ManipulacaoXml.SetNode(doc, icmsSn900, "vBCSTRet", Formatacoes.TrataValorDecimal(ProdutosNfDAO.Instance.GetLastBcIcmsSt(pnf.IdProdNf), 2));
                                //ManipulacaoXml.SetNode(doc, icmsSn900, "vICMSSTRet", Formatacoes.TrataValorDecimal(ProdutosNfDAO.Instance.GetLastIcmsSt(pnf.IdProdNf), 2));
                                ManipulacaoXml.SetNode(doc, icmsSn900, "pCredSN", Formatacoes.TrataValorDouble(calcIcms ? aliqICMSSN : 0, 2));
                                ManipulacaoXml.SetNode(doc, icmsSn900, "vCredICMSSN", Formatacoes.TrataValorDecimal(calcIcms ? pnf.Total * ((decimal)aliqICMSSN / 100) : 0, 2));

                                icms.AppendChild(icmsSn900);
                                break;
                            case "":
                            case null:
                                throw new Exception("Informe o CSOSN de todos os produtos da nota.");
                            default:
                                throw new Exception("CSOSN não implantada.");
                        }
                    }

                    #endregion

                    #region IPI

                    /* Chamado 67620.
                     * A TAG IPI deverá ser criada caso o CST IPI tenha sido informado ou caso a alíquota do IPI seja maior que zero.
                     * Caso a alíquota seja maior que zero e o CST IPI esteja vazio, o sistema deve preencher essa informação com o valor padrão da config CstIpi.
                     * Porém, caso a alíquota seja 0 e o CST IPI esteja preenchido, a TAG deverá ser criada e o CST IPI será recuperado
                     *  de acordo com a informação inserida no produto da nota fiscal. */
                    if ((pnf.CstIpi.HasValue || pnf.AliqIpi > 0) && (!nf.Consumidor || UserInfo.GetUserInfo.UfLoja == "RN"))
                    {
                        var cstIpi = !pnf.CstIpi.HasValue ? pnf.CstIpi.GetValueOrDefault((int)ConfigNFe.CstIpi(pnf.IdProdNf)) : pnf.CstIpi.Value;
                        var codEnqIpi = NaturezaOperacaoDAO.Instance.ObtemValorCampo<string>("CodEnqIpi", "idNaturezaOperacao=" + pnf.IdNaturezaOperacao.GetValueOrDefault(nf.IdNaturezaOperacao.GetValueOrDefault()));

                        XmlElement ipi = doc.CreateElement("IPI");
                        imposto.AppendChild(ipi);

                        // Se o CSTIPI do produto da nota estiver diferente da configuração, atualiza o do produto.
                        ManipulacaoXml.SetNode(doc, ipi, "cEnq", !string.IsNullOrEmpty(codEnqIpi) ? codEnqIpi : "999");

                        if (!pnf.CstIpi.HasValue)
                        {
                            objPersistence.ExecuteCommand(string.Format("Update produtos_nf Set CstIpi={0} Where idProdNf={1}", cstIpi, pnf.IdProdNf));
                            pnf.CstIpi = cstIpi;
                        }

                        switch (pnf.CstIpi.Value)
                        {
                            case 0:
                            case 49:
                            case 50:
                            case 99:
                                XmlElement ipiTrib = doc.CreateElement("IPITrib");
                                ManipulacaoXml.SetNode(doc, ipiTrib, "CST", pnf.CstIpi.Value.ToString("0#")); // 00-Entrada com recuperação de crédito 49-Outras entradas 50-Saída tributada 99-Outras saídas
                                                                                                              /* Chamado 23331. */
                                                                                                              //ManipulacaoXml.SetNode(doc, ipiTrib, "vBC", Formatacoes.TrataValorDecimal(bcIpi, 2));
                                ManipulacaoXml.SetNode(doc, ipiTrib, "vBC", aliqIpi > 0 ? Formatacoes.TrataValorDecimal(bcIpi, 2) : Formatacoes.TrataValorDecimal(0, 2));
                                ManipulacaoXml.SetNode(doc, ipiTrib, "pIPI", Formatacoes.TrataValorDecimal(aliqIpi, 2));
                                ManipulacaoXml.SetNode(doc, ipiTrib, "vIPI", Formatacoes.TrataValorDecimal(valorIpi, 2));
                                ipi.AppendChild(ipiTrib);
                                break;
                            default: // 01, 02, 03, 04, 51, 52, 53, 54 e 55
                                XmlElement ipiNT = doc.CreateElement("IPINT");
                                ManipulacaoXml.SetNode(doc, ipiNT, "CST", pnf.CstIpi.Value.ToString().PadLeft(2, '0'));
                                ipi.AppendChild(ipiNT);
                                break;
                        }
                    }

                    #endregion

                    #region II

                    if (IsNotaFiscalImportacao(idNf))
                    {
                        XmlElement ii = doc.CreateElement("II");
                        ManipulacaoXml.SetNode(doc, ii, "vBC", Formatacoes.TrataValorDecimal(pnf.BcIi, 2));
                        ManipulacaoXml.SetNode(doc, ii, "vDespAdu", Formatacoes.TrataValorDecimal(pnf.DespAduaneira, 2));
                        ManipulacaoXml.SetNode(doc, ii, "vII", Formatacoes.TrataValorDecimal(pnf.ValorIi, 2));
                        ManipulacaoXml.SetNode(doc, ii, "vIOF", Formatacoes.TrataValorDecimal(pnf.ValorIof, 2));
                        imposto.AppendChild(ii);
                    }

                    #endregion

                    #region PIS

                    XmlElement pis = doc.CreateElement("PIS");
                    imposto.AppendChild(pis);

                    // Regime normal (Destaca PIS)
                    if (loja.Crt == (int)CrtLoja.LucroPresumido || loja.Crt == (int)CrtLoja.LucroReal)
                    {
                        bool calcPis = NaturezaOperacaoDAO.Instance.CalculaPis(pnf.IdNaturezaOperacao.Value);

                        if (pnf.CstPis == null || pnf.CstPis == 0)
                        {
                            var cstPis = ConfigNFe.CstPisCofins(pnf.IdNf);

                            // Atualiza o cst do pis conforme o que o sistema buscar por padrão
                            objPersistence.ExecuteCommand(String.Format("Update produtos_nf Set CstPis={0} Where idProdNf={1}", cstPis, pnf.IdProdNf));

                            XmlElement pisAliq = doc.CreateElement("PISAliq");
                            ManipulacaoXml.SetNode(doc, pisAliq, "CST", cstPis.ToString("0#"));
                            ManipulacaoXml.SetNode(doc, pisAliq, "vBC", Formatacoes.TrataValorDouble(!nfeAjuste && calcPis ? (double)pnf.BcPis : 0, 2));
                            ManipulacaoXml.SetNode(doc, pisAliq, "pPIS", Formatacoes.TrataValorDouble(!nfeAjuste && calcPis ? (pnf.AliqPis > 0 ? pnf.AliqPis : ConfigNFe.AliqPis((uint)loja.IdLoja)) : 0, 2));
                            ManipulacaoXml.SetNode(doc, pisAliq, "vPIS", Formatacoes.TrataValorDouble(!nfeAjuste && calcPis ? (pnf.ValorPis > 0 ? (double)pnf.ValorPis : (double)pnf.BcPis * (ConfigNFe.AliqPis((uint)loja.IdLoja) / 100)) : 0, 2));
                            pis.AppendChild(pisAliq);
                        }
                        else
                        {
                            switch (pnf.CstPis.Value)
                            {
                                case 1:
                                case 2:
                                    XmlElement pisAliq = doc.CreateElement("PISAliq");
                                    ManipulacaoXml.SetNode(doc, pisAliq, "CST", pnf.CstPis.Value.ToString().PadLeft(2, '0'));
                                    ManipulacaoXml.SetNode(doc, pisAliq, "vBC", Formatacoes.TrataValorDouble(!nfeAjuste && calcPis ? (double)pnf.BcPis : 0, 2));
                                    ManipulacaoXml.SetNode(doc, pisAliq, "pPIS", Formatacoes.TrataValorDouble(!nfeAjuste && calcPis ? (pnf.AliqPis > 0 ? pnf.AliqPis : ConfigNFe.AliqPis((uint)loja.IdLoja)) : 0, 2));
                                    ManipulacaoXml.SetNode(doc, pisAliq, "vPIS", Formatacoes.TrataValorDouble(!nfeAjuste && calcPis ? (pnf.ValorPis > 0 ? (double)pnf.ValorPis : (double)pnf.BcPis * (ConfigNFe.AliqPis((uint)loja.IdLoja) / 100)) : 0, 2));
                                    pis.AppendChild(pisAliq);
                                    break;
                                case 3:
                                    throw new Exception("PIS CST 03 não implementado.");
                                case 4:
                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                    XmlElement pisNT = doc.CreateElement("PISNT");
                                    ManipulacaoXml.SetNode(doc, pisNT, "CST", pnf.CstPis.Value.ToString().PadLeft(2, '0'));
                                    pis.AppendChild(pisNT);
                                    break;
                                default:
                                    XmlElement pisOutr = doc.CreateElement("PISOutr");
                                    ManipulacaoXml.SetNode(doc, pisOutr, "CST", pnf.CstPis.Value.ToString().PadLeft(2, '0'));
                                    ManipulacaoXml.SetNode(doc, pisOutr, "vBC", Formatacoes.TrataValorDouble(!nfeAjuste && calcPis ? (double)pnf.BcPis : 0, 2));
                                    ManipulacaoXml.SetNode(doc, pisOutr, "pPIS", Formatacoes.TrataValorDouble(!nfeAjuste && calcPis ? (pnf.AliqPis > 0 ? pnf.AliqPis : ConfigNFe.AliqPis((uint)loja.IdLoja)) : 0, 2));

                                    // Devem ser informados os campos vBC e pPIS ou os campos qBCProd e vAliqProd
                                    //ManipulacaoXml.SetNode(doc, pisOutr, "qBCProd", Formatacoes.TrataValorDecimal(0, 4));
                                    //ManipulacaoXml.SetNode(doc, pisOutr, "vAliqProd", Formatacoes.TrataValorDecimal(0, 2));

                                    ManipulacaoXml.SetNode(doc, pisOutr, "vPIS", Formatacoes.TrataValorDouble(!nfeAjuste && calcPis ? (pnf.ValorPis > 0 ? (double)pnf.ValorPis : (double)pnf.BcPis * (ConfigNFe.AliqPis((uint)loja.IdLoja) / 100)) : 0, 2));
                                    pis.AppendChild(pisOutr);
                                    break;
                            }
                        }
                    }
                    else // Simples Nacional (Não destaca PIS)
                    {
                        XmlElement pisnt = doc.CreateElement("PISNT");
                        ManipulacaoXml.SetNode(doc, pisnt, "CST", ConfigNFe.CstPisCofins(pnf.IdNf).ToString("0#")); // Operação sem incidência da contribuição
                        pis.AppendChild(pisnt);
                    }

                    #endregion

                    #region COFINS

                    XmlElement cofins = doc.CreateElement("COFINS");
                    imposto.AppendChild(cofins);

                    // Regime normal (Destaca COFINS)
                    if (loja.Crt == (int)CrtLoja.LucroPresumido || loja.Crt == (int)CrtLoja.LucroReal)
                    {
                        bool calcCofins = NaturezaOperacaoDAO.Instance.CalculaCofins(pnf.IdNaturezaOperacao.Value);

                        if (pnf.CstCofins == null || pnf.CstCofins == 0)
                        {
                            var cstCofins = ConfigNFe.CstPisCofins(pnf.IdNf);

                            // Atualiza o cst do pis conforme o que o sistema buscar por padrão
                            objPersistence.ExecuteCommand(String.Format("Update produtos_nf Set CstCofins={0} Where idProdNf={1}", cstCofins, pnf.IdProdNf));

                            XmlElement cofinsAliq = doc.CreateElement("COFINSAliq");
                            ManipulacaoXml.SetNode(doc, cofinsAliq, "CST", cstCofins.ToString("0#"));
                            ManipulacaoXml.SetNode(doc, cofinsAliq, "vBC", Formatacoes.TrataValorDouble(!nfeAjuste && calcCofins ? (double)pnf.BcCofins : 0, 2));
                            ManipulacaoXml.SetNode(doc, cofinsAliq, "pCOFINS", Formatacoes.TrataValorDouble(!nfeAjuste && calcCofins ? (pnf.AliqCofins > 0 ? pnf.AliqCofins : ConfigNFe.AliqCofins((uint)loja.IdLoja)) : 0, 2));
                            ManipulacaoXml.SetNode(doc, cofinsAliq, "vCOFINS", Formatacoes.TrataValorDouble(!nfeAjuste && calcCofins ? (pnf.ValorCofins > 0 ? (double)pnf.ValorCofins : (double)pnf.BcCofins * (ConfigNFe.AliqCofins((uint)loja.IdLoja) / 100)) : 0, 2));
                            cofins.AppendChild(cofinsAliq);
                        }
                        else
                        {
                            switch (pnf.CstCofins.Value)
                            {
                                case 1:
                                case 2:
                                    XmlElement cofinsAliq = doc.CreateElement("COFINSAliq");
                                    ManipulacaoXml.SetNode(doc, cofinsAliq, "CST", pnf.CstCofins.Value.ToString().PadLeft(2, '0'));
                                    ManipulacaoXml.SetNode(doc, cofinsAliq, "vBC", Formatacoes.TrataValorDouble(!nfeAjuste && calcCofins ? (double)pnf.BcCofins : 0, 2));
                                    ManipulacaoXml.SetNode(doc, cofinsAliq, "pCOFINS", Formatacoes.TrataValorDouble(!nfeAjuste && calcCofins ? (pnf.AliqCofins > 0 ? pnf.AliqCofins : ConfigNFe.AliqCofins((uint)loja.IdLoja)) : 0, 2));
                                    ManipulacaoXml.SetNode(doc, cofinsAliq, "vCOFINS", Formatacoes.TrataValorDouble(!nfeAjuste && calcCofins ? (pnf.ValorCofins > 0 ? (double)pnf.ValorCofins : (double)pnf.BcCofins * (ConfigNFe.AliqCofins((uint)loja.IdLoja) / 100)) : 0, 2));
                                    cofins.AppendChild(cofinsAliq);
                                    break;
                                case 3:
                                    throw new Exception("COFINS CST 03 não implementado.");
                                case 4:
                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                    XmlElement cofinsNT = doc.CreateElement("COFINSNT");
                                    ManipulacaoXml.SetNode(doc, cofinsNT, "CST", pnf.CstCofins.Value.ToString().PadLeft(2, '0'));
                                    cofins.AppendChild(cofinsNT);
                                    break;
                                default:
                                    XmlElement cofinsOutr = doc.CreateElement("COFINSOutr");
                                    ManipulacaoXml.SetNode(doc, cofinsOutr, "CST", pnf.CstCofins.Value.ToString().PadLeft(2, '0'));
                                    ManipulacaoXml.SetNode(doc, cofinsOutr, "vBC", Formatacoes.TrataValorDouble(!nfeAjuste && calcCofins ? (double)pnf.BcCofins : 0, 2));
                                    ManipulacaoXml.SetNode(doc, cofinsOutr, "pCOFINS", Formatacoes.TrataValorDouble(!nfeAjuste && calcCofins ? (pnf.AliqCofins > 0 ? pnf.AliqCofins : ConfigNFe.AliqCofins((uint)loja.IdLoja)) : 0, 2));

                                    // Devem ser informados os campos vBC e pCOFINS ou os campos qBCProd e vAliqProd
                                    //ManipulacaoXml.SetNode(doc, cofinsOutr, "qBCProd", Formatacoes.TrataValorDecimal(0, 4));
                                    //ManipulacaoXml.SetNode(doc, cofinsOutr, "vAliqProd", Formatacoes.TrataValorDecimal(0, 2));

                                    ManipulacaoXml.SetNode(doc, cofinsOutr, "vCOFINS", Formatacoes.TrataValorDouble(!nfeAjuste && calcCofins ? (pnf.ValorCofins > 0 ? (double)pnf.ValorCofins : (double)pnf.BcCofins * (ConfigNFe.AliqCofins((uint)loja.IdLoja) / 100)) : 0, 2));
                                    cofins.AppendChild(cofinsOutr);
                                    break;
                            }
                        }
                    }
                    else // Simples Nacional (Não destaca COFINS)
                    {
                        XmlElement cofinsnt = doc.CreateElement("COFINSNT");
                        ManipulacaoXml.SetNode(doc, cofinsnt, "CST", ConfigNFe.CstPisCofins(pnf.IdNf).ToString("0#")); // Operação sem incidência da contribuição
                        cofins.AppendChild(cofinsnt);
                    }

                    #endregion

                    #region DIFAL

                    if (pnf.IdNaturezaOperacao > 0 &&
                        NaturezaOperacaoDAO.Instance.ObtemValorCampo<bool?>("CalcularDifal",
                            string.Format("IdNaturezaOperacao={0}", pnf.IdNaturezaOperacao)).GetValueOrDefault())
                    {
                        var codigoCfop =
                            CfopDAO.Instance.ObtemCodInterno(NaturezaOperacaoDAO.Instance.ObtemIdCfop(pnf.IdNaturezaOperacao.Value)).Substring(0, 1);

                        // Salva as tags do DIFAL somente se o CFOP for interestadual.
                        if (codigoCfop == "2" || codigoCfop == "6")
                        {
                            var notaFiscal = GetElementByPrimaryKey(pnf.IdNf);

                            if (notaFiscal.IdCliente > 0)
                            {
                                var idCidadeCliente =
                                    ClienteDAO.Instance.ObtemValorCampo<int?>("IdCidade",
                                        string.Format("Id_Cli={0}", notaFiscal.IdCliente));
                                var nomeUfDestino =
                                    CidadeDAO.Instance.ObtemValorCampo<string>("NomeUF",
                                        string.Format("IdCidade={0}", idCidadeCliente));

                                var idCidadeLoja = LojaDAO.Instance.ObtemIdCidade(notaFiscal.IdLoja.Value);
                                var nomeUfOrigem =
                                    CidadeDAO.Instance.ObtemValorCampo<string>("NomeUF",
                                        string.Format("IdCidade={0}", idCidadeLoja));

                                var idTipoCliente = ClienteDAO.Instance.ObtemIdTipoCliente(notaFiscal.IdCliente.Value);

                                var dadosIcms = IcmsProdutoUfDAO.Instance.ObtemPorProduto(null, pnf.IdProd, nomeUfOrigem, nomeUfDestino, idTipoCliente);

                                if (dadosIcms == null)
                                    throw new Exception(
                                        string.Format("A natureza de operação associada ao produto {0} está marcada para calcular DIFAL, portanto, informe as alíquotas de ICMS no cadastro do produto.",
                                            ProdutoDAO.Instance.GetCodInterno((int)pnf.IdProd)));

                                var origemSulSudesteExcetoES =
                                    nomeUfOrigem.ToUpper().Contains("MG") ||
                                    nomeUfOrigem.ToUpper().Contains("PR") ||
                                    nomeUfOrigem.ToUpper().Contains("RJ") ||
                                    nomeUfOrigem.ToUpper().Contains("RS") ||
                                    nomeUfOrigem.ToUpper().Contains("SC") ||
                                    nomeUfOrigem.ToUpper().Contains("SP");

                                var destinoNorteNordesteCentroOesteES =
                                    nomeUfDestino.ToUpper().Contains("AC") ||
                                    nomeUfDestino.ToUpper().Contains("AL") ||
                                    nomeUfDestino.ToUpper().Contains("AM") ||
                                    nomeUfDestino.ToUpper().Contains("AP") ||
                                    nomeUfDestino.ToUpper().Contains("BA") ||
                                    nomeUfDestino.ToUpper().Contains("CE") ||
                                    nomeUfDestino.ToUpper().Contains("DF") ||
                                    nomeUfDestino.ToUpper().Contains("ES") ||
                                    nomeUfDestino.ToUpper().Contains("GO") ||
                                    nomeUfDestino.ToUpper().Contains("MA") ||
                                    nomeUfDestino.ToUpper().Contains("MS") ||
                                    nomeUfDestino.ToUpper().Contains("MT") ||
                                    nomeUfDestino.ToUpper().Contains("PA") ||
                                    nomeUfDestino.ToUpper().Contains("PB") ||
                                    nomeUfDestino.ToUpper().Contains("PE") ||
                                    nomeUfDestino.ToUpper().Contains("PI") ||
                                    nomeUfDestino.ToUpper().Contains("RN") ||
                                    nomeUfDestino.ToUpper().Contains("RO") ||
                                    nomeUfDestino.ToUpper().Contains("RR") ||
                                    nomeUfDestino.ToUpper().Contains("SE") ||
                                    nomeUfDestino.ToUpper().Contains("TO");

                                var percentualIcmsInterestadual =
                                    pnf.CstOrig == 1 ? 4 :
                                        origemSulSudesteExcetoES && destinoNorteNordesteCentroOesteES ? 7 : 12;

                                var valorDifal =
                                    (pnf.BcIcms * ((decimal)dadosIcms.AliquotaInternaDestinatario / 100)) -
                                    (pnf.BcIcms * ((decimal)percentualIcmsInterestadual / 100));

                                var percentualIcmsUFDestino =
                                    DateTime.Now.Year == 2016 ? (decimal)0.4 :
                                    DateTime.Now.Year == 2017 ? (decimal)0.6 :
                                    DateTime.Now.Year == 2018 ? (decimal)0.8 : 100;

                                var percentualIcmsUFRemetente =
                                    DateTime.Now.Year == 2016 ? (decimal)0.6 :
                                    DateTime.Now.Year == 2017 ? (decimal)0.4 :
                                    DateTime.Now.Year == 2018 ? (decimal)0.2 : 0;

                                var valorIcmsUFDestino = Math.Round(valorDifal * percentualIcmsUFDestino, 2);
                                var valorIcmsUFRemetente = Math.Round(valorDifal * percentualIcmsUFRemetente, 2);
                                var valorIcmsFCP = Math.Round(pnf.BcIcms * (FiscalConfig.PercentualFundoPobreza / 100), 2);

                                totalIcmsUFDestino += valorIcmsUFDestino;
                                totalIcmsUFRemetente += valorIcmsUFRemetente;
                                totalIcmsFCP += valorIcmsFCP;

                                XmlElement icmsUfDest = doc.CreateElement("ICMSUFDest");

                                ManipulacaoXml.SetNode(doc, icmsUfDest, "vBCUFDest", Formatacoes.TrataValorDecimal(pnf.BcIcms, 2));
                                ManipulacaoXml.SetNode(doc, icmsUfDest, "vBCFCPUFDest", Formatacoes.TrataValorDecimal(pnf.BcIcms, 2));// Valor da Base de Cálculo do FCP na UF de destino.
                                ManipulacaoXml.SetNode(doc, icmsUfDest, "pFCPUFDest", Formatacoes.TrataValorDecimal(FiscalConfig.PercentualFundoPobreza, 2));
                                ManipulacaoXml.SetNode(doc, icmsUfDest, "pICMSUFDest", Formatacoes.TrataValorDecimal((decimal)dadosIcms.AliquotaInternaDestinatario, 2));
                                ManipulacaoXml.SetNode(doc, icmsUfDest, "pICMSInter", Formatacoes.TrataValorDecimal(percentualIcmsInterestadual, 2));
                                ManipulacaoXml.SetNode(doc, icmsUfDest, "pICMSInterPart",
                                    DateTime.Now.Year == 2016 ?
                                        Formatacoes.TrataValorDecimal(40, 2) :
                                        DateTime.Now.Year == 2017 ?
                                            Formatacoes.TrataValorDecimal(60, 2) :
                                            DateTime.Now.Year == 2018 ?
                                                Formatacoes.TrataValorDecimal(80, 2) :
                                                Formatacoes.TrataValorDecimal(100, 2));
                                ManipulacaoXml.SetNode(doc, icmsUfDest, "vFCPUFDest", Formatacoes.TrataValorDecimal(valorIcmsFCP, 2));
                                ManipulacaoXml.SetNode(doc, icmsUfDest, "vICMSUFDest", Formatacoes.TrataValorDecimal(valorIcmsUFDestino, 2));
                                ManipulacaoXml.SetNode(doc, icmsUfDest, "vICMSUFRemet", Formatacoes.TrataValorDecimal(valorIcmsUFRemetente, 2));

                                imposto.AppendChild(icmsUfDest);
                            }
                        }
                    }

                    #endregion

                    // Preenche o campo informação adicional do produto
                    if (!String.IsNullOrEmpty(pnf.InfAdic))
                        ManipulacaoXml.SetNode(doc, det, "infAdProd", Formatacoes.TrataTextoDocFiscal(pnf.InfAdic));

                    #region FCI

                    if (FiscalConfig.UtilizaFCI && (pnf.CstOrig == 3 || pnf.CstOrig == 5 || pnf.CstOrig == 8))
                    {
                        //if(string.IsNullOrEmpty(pnf.NumControleFciStr))
                        //throw new Exception("Número de controle da FCI do produto " + pnf.CodInterno + " - " + pnf.DescrProduto + " não foi encontrado.");

                        if (!string.IsNullOrEmpty(pnf.NumControleFciStr))
                            ManipulacaoXml.SetNode(doc, prod, "nFCI", pnf.NumControleFciStr);
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir produtos no XML.", ex);
            }

            #endregion

            #region Total

            try
            {
                // Total
                XmlElement total = doc.CreateElement("total");
                infNFe.AppendChild(total);
                XmlElement icmsTot = doc.CreateElement("ICMSTot");
                ManipulacaoXml.SetNode(doc, icmsTot, "vBC", Formatacoes.TrataValorDecimal(nf.BcIcms, 2)); // Verificar
                ManipulacaoXml.SetNode(doc, icmsTot, "vICMS", Formatacoes.TrataValorDecimal(nf.Valoricms, 2)); // Verificar
                ManipulacaoXml.SetNode(doc, icmsTot, "vICMSDeson", Formatacoes.TrataValorDecimal(lstProdNf.Sum(f => f.ValorIcmsDesonerado), 2));

                if (totalIcmsUFDestino > 0 || totalIcmsUFRemetente > 0)
                {
                    ManipulacaoXml.SetNode(doc, icmsTot, "vFCPUFDest", Formatacoes.TrataValorDecimal(totalIcmsFCP, 2));
                    ManipulacaoXml.SetNode(doc, icmsTot, "vICMSUFDest", Formatacoes.TrataValorDecimal(totalIcmsUFDestino, 2));
                    ManipulacaoXml.SetNode(doc, icmsTot, "vICMSUFRemet", Formatacoes.TrataValorDecimal(totalIcmsUFRemetente, 2));
                }
                
                // FCP
                ManipulacaoXml.SetNode(doc, icmsTot, "vFCP", Formatacoes.TrataValorDecimal(nf.ValorFcp, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vBCST", Formatacoes.TrataValorDecimal(nf.BcIcmsSt, 2)); // Verificar
                ManipulacaoXml.SetNode(doc, icmsTot, "vST", Formatacoes.TrataValorDecimal(nf.ValorIcmsSt, 2)); // Verificar
                ManipulacaoXml.SetNode(doc, icmsTot, "vFCPST", Formatacoes.TrataValorDecimal(nf.ValorFcpSt, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vFCPSTRet", Formatacoes.TrataValorDecimal(0, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vProd", Formatacoes.TrataValorDecimal(totalProd, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vFrete", Formatacoes.TrataValorDecimal(nf.ValorFrete, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vSeg", Formatacoes.TrataValorDecimal(nf.ValorSeguro, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vDesc", Formatacoes.TrataValorDecimal(nf.Desconto, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vII", Formatacoes.TrataValorDecimal(ProdutosNfDAO.Instance.ObtemTotalII(idNf), 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vIPI", Formatacoes.TrataValorDecimal(nf.ValorIpi, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vIPIDevol", Formatacoes.TrataValorDecimal(nf.ValorIpiDevolvido, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vPIS", loja.Crt == (int)CrtLoja.LucroPresumido || loja.Crt == (int)CrtLoja.LucroReal ? Formatacoes.TrataValorDecimal(nf.ValorPis, 2) : Formatacoes.TrataValorDecimal(0, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vCOFINS", loja.Crt == (int)CrtLoja.LucroPresumido || loja.Crt == (int)CrtLoja.LucroReal ? Formatacoes.TrataValorDecimal(nf.ValorCofins, 2) : Formatacoes.TrataValorDecimal(0, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vOutro", Formatacoes.TrataValorDecimal(nf.OutrasDespesas, 2));
                ManipulacaoXml.SetNode(doc, icmsTot, "vNF", Formatacoes.TrataValorDecimal(nf.TotalNota, 2));
                if (nf.ValorTotalTrib > 0) ManipulacaoXml.SetNode(doc, icmsTot, "vTotTrib", Formatacoes.TrataValorDecimal(nf.ValorTotalTrib, 2));
                total.AppendChild(icmsTot);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir totais da NFe no XML.", ex);
            }

            #endregion

            #region Transportador

            try
            {
                // Transportador
                XmlElement transp = doc.CreateElement("transp");
                ManipulacaoXml.SetNode(doc, transp, "modFrete", ((int)nf.ModalidadeFrete).ToString());

                // Se houver Transportadora, inclui na NFe
                if (transportador != null)
                {
                    XmlElement transporta = doc.CreateElement("transporta");
                    if (transportador.TipoPessoa == 1) ManipulacaoXml.SetNode(doc, transporta, "CPF", Formatacoes.TrataStringDocFiscal(transportador.CpfCnpj));
                    if (transportador.TipoPessoa == 2) ManipulacaoXml.SetNode(doc, transporta, "CNPJ", Formatacoes.TrataStringDocFiscal(transportador.CpfCnpj));
                    ManipulacaoXml.SetNode(doc, transporta, "xNome", Formatacoes.TrataStringDocFiscal(transportador.Nome));

                    var cidadeTransportador = CidadeDAO.Instance.GetElementByPrimaryKey(transportador.IdCidade.GetValueOrDefault());

                    if ((transportador.TipoPessoa == 2) ||
                        (transportador.TipoPessoa == 1 && cidadeTransportador != null && Validacoes.ValidaIE(cidadeTransportador.NomeUf, transportador.InscEst)))
                        ManipulacaoXml.SetNode(doc, transporta, "IE", Formatacoes.TrataStringDocFiscal(transportador.InscEst).ToUpper());

                    ManipulacaoXml.SetNode(doc, transporta, "xEnder", Formatacoes.TrataTextoDocFiscal(transportador.EnderecoNfe));
                    ManipulacaoXml.SetNode(doc, transporta, "xMun", Formatacoes.TrataStringDocFiscal(transportador.Cidade));
                    ManipulacaoXml.SetNode(doc, transporta, "UF", Formatacoes.TrataStringDocFiscal(transportador.Uf));
                    transp.AppendChild(transporta);

                    // Se os dados do veículo tiverem sido informados, acrescenta na NFe
                    if (!String.IsNullOrEmpty(nf.VeicPlaca) && !String.IsNullOrEmpty(nf.VeicUf))
                    {
                        XmlElement veicTransp = doc.CreateElement("veicTransp");
                        ManipulacaoXml.SetNode(doc, veicTransp, "placa", Formatacoes.TrataStringDocFiscal(nf.VeicPlaca));
                        ManipulacaoXml.SetNode(doc, veicTransp, "UF", Formatacoes.TrataStringDocFiscal(nf.VeicUf));

                        if (!String.IsNullOrEmpty(nf.VeicRntc))
                            ManipulacaoXml.SetNode(doc, veicTransp, "RNTC", Formatacoes.TrataStringDocFiscal(nf.VeicRntc));

                        transp.AppendChild(veicTransp);
                    }
                }

                XmlElement vol = doc.CreateElement("vol");
                if (nf.QtdVol > 0) ManipulacaoXml.SetNode(doc, vol, "qVol", nf.QtdVol.ToString());
                if (!String.IsNullOrEmpty(nf.Especie)) ManipulacaoXml.SetNode(doc, vol, "esp", Formatacoes.TrataStringDocFiscal(nf.Especie));
                if (!String.IsNullOrEmpty(nf.MarcaVol)) ManipulacaoXml.SetNode(doc, vol, "marca", Formatacoes.TrataStringDocFiscal(nf.MarcaVol));
                if (!String.IsNullOrEmpty(nf.NumeracaoVol)) ManipulacaoXml.SetNode(doc, vol, "nVol", Formatacoes.TrataStringDocFiscal(nf.NumeracaoVol));
                if (nf.PesoLiq > 0) ManipulacaoXml.SetNode(doc, vol, "pesoL", Formatacoes.TrataValorDecimal(nf.PesoLiq, 3));
                if (nf.PesoBruto > 0) ManipulacaoXml.SetNode(doc, vol, "pesoB", Formatacoes.TrataValorDecimal(nf.PesoBruto, 3));
                transp.AppendChild(vol);

                infNFe.AppendChild(transp);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir dados do transportador no XML.", ex);
            }

            #endregion

            #region Fatura

            try
            {
                if (lstParcNf.Length > 0 && lstParcNf[0].Valor > 0)
                {
                    XmlElement cobr = doc.CreateElement("cobr");
                    infNFe.AppendChild(cobr);

                    XmlElement fat = doc.CreateElement("fat");
                    ManipulacaoXml.SetNode(doc, fat, "nFat", nf.NumeroNFe.ToString());
                    ManipulacaoXml.SetNode(doc, fat, "vOrig", Formatacoes.TrataValorDecimal(nf.TotalNota, 2));
                    //if (false) ManipulacaoXml.SetNode(doc, fat, "vDesc", Formatacoes.TrataValorDecimal(0, 2));
                    ManipulacaoXml.SetNode(doc, fat, "vLiq", Formatacoes.TrataValorDecimal(nf.TotalNota, 2));
                    cobr.AppendChild(fat);

                    // Busca as parcelas da nota fiscal
                    if (nf.FormaPagto != (int)NotaFiscal.FormaPagtoEnum.AVista)
                    {
                        for (int i = 0; i < lstParcNf.Length; i++)
                        {
                            XmlElement dup = doc.CreateElement("dup");
                            ManipulacaoXml.SetNode(doc, dup, "nDup", nf.NumeroNFe.ToString() + "-" + (i + 1));
                            ManipulacaoXml.SetNode(doc, dup, "dVenc", lstParcNf[i].Data.Value.ToString("yyyy-MM-dd"));
                            ManipulacaoXml.SetNode(doc, dup, "vDup", Formatacoes.TrataValorDecimal(lstParcNf[i].Valor, 2));
                            cobr.AppendChild(dup);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir dados da fatura da NFe no XML.", ex);
            }

            #endregion

            #region Informações de Pagamento

            XmlElement pag = doc.CreateElement("pag");
            infNFe.AppendChild(pag);

            var pagamentosNfe = PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)idNf);

            foreach (var p in pagamentosNfe)
            {
                XmlElement detPag = doc.CreateElement("detPag");
                pag.AppendChild(detPag);

                ManipulacaoXml.SetNode(doc, detPag, "tPag", Formatacoes.FormataNumero(p.FormaPagto.ToString(), null, 2));
                ManipulacaoXml.SetNode(doc, detPag, "vPag", Formatacoes.TrataValorDecimal(p.Valor, 2));

                if (p.FormaPagto == (int)FormaPagtoNotaFiscalEnum.CartaoDebito || p.FormaPagto == (int)FormaPagtoNotaFiscalEnum.CartaoCredito)
                {
                    XmlElement card = doc.CreateElement("card");
                    detPag.AppendChild(card);

                    ManipulacaoXml.SetNode(doc, card, "tpIntegra", ((int)TipoIntegracaoEnum.NaoIntegrado).ToString());

                    //ManipulacaoXml.SetNode(doc, card, "CNPJ", Formatacoes.TrataStringDocFiscal(p.CnpjCredenciadora));
                    //ManipulacaoXml.SetNode(doc, card, "tBand", Formatacoes.FormataNumero(p.Bandeira.ToString(), null, 2));
                    //ManipulacaoXml.SetNode(doc, card, "cAut", Formatacoes.TrataStringDocFiscal(p.NumAut));
                }
            }

            if(pagamentosNfe != null && pagamentosNfe.Any() && pagamentosNfe.Sum(f => f.Valor) > nf.TotalNota)
                ManipulacaoXml.SetNode(doc, pag, "vTroco", Formatacoes.TrataValorDecimal(pagamentosNfe.Sum(f => f.Valor) - nf.TotalNota, 2));

            #endregion

            #region Informações adicionais

            try
            {
                // Substitui valores dos campos #bcicms, #vicmsdest e #vicmsremet
                if (nf.InfCompl != null)
                    nf.InfCompl = nf.InfCompl.Replace("#bcicms", nf.BcIcms.ToString("C"))
                        .Replace("#vicmsdest", totalIcmsUFDestino.ToString("C"))
                        .Replace("#vicmsremet", totalIcmsUFRemetente.ToString("C"));

                // Acrescenta o total de tributos, conforme lei da transparência, nas informações complementares
                if (!String.IsNullOrEmpty(valorTotalTributosIbpt) && !nf.Consumidor)
                    nf.InfCompl = (!String.IsNullOrEmpty(nf.InfCompl) ? nf.InfCompl + " " : "") + valorTotalTributosIbpt;

                else if (nf.ValorTotalTrib > 0 && !nf.Consumidor)
                    nf.InfCompl += (!String.IsNullOrEmpty(nf.InfCompl) ? " " : "") + "Valor aproximado dos tributos: " + nf.ValorTotalTrib.ToString("C");
                
                var mensagemNaturezasOperacao = nf.MensagemNaturezasOperacao;
                if (!String.IsNullOrEmpty(mensagemNaturezasOperacao))
                    nf.InfCompl += (!String.IsNullOrEmpty(nf.InfCompl) ? " " : "") + mensagemNaturezasOperacao;

                // Informa na observação da NFe, a alíquota/valor para aproveitamento de crédito do Simples Nacional
                if (nf.InfCompl != null)
                    nf.InfCompl = nf.InfCompl
                        .Replace("#aliqicmssn", aliqICMSSN.ToString("P"))
                        .Replace("#valoricmssn", valorICMSSN.ToString("C"))
                        .Replace("#bcicmssn", totalProdSN.ToString("C"));

                if (FiscalConfig.NotaFiscalConfig.InformarFormaPagtoNFe && PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)idNf).Any() && !nf.Consumidor)
                    nf.InfCompl += nf.FormaPagtoString + " ";

                var textoPadraoInfComplNota = FiscalConfig.NotaFiscalConfig.TextoPadraoInfComplNota;
                if (!string.IsNullOrEmpty(textoPadraoInfComplNota) && nf.TipoDocumento != (int)NotaFiscal.TipoDoc.Entrada)
                    nf.InfCompl = textoPadraoInfComplNota + nf.InfCompl;

                if (FiscalConfig.NotaFiscalConfig.ExibirEnderecoEntregaInfCompl && nf.IdCliente.GetValueOrDefault() > 0)
                {
                    var enderecoPadrao = ClienteDAO.Instance.ObtemEnderecoCompleto(nf.IdCliente.GetValueOrDefault());
                    var enderecoEntrega = ClienteDAO.Instance.ObtemEnderecoEntregaCompleto(nf.IdCliente.GetValueOrDefault());

                    if (!String.IsNullOrEmpty(enderecoEntrega))
                        nf.InfCompl += enderecoEntrega;
                    else
                        nf.InfCompl += enderecoPadrao;
                }

                if (FiscalConfig.NotaFiscalConfig.ExibirFormaPagamentoLiberacaoInfCompl)
                    nf.InfCompl = (!string.IsNullOrEmpty(nf.InfCompl) ? nf.InfCompl + " " : "") + "F. Pagto. Lib.:" + PagtoLiberarPedidoDAO.Instance.ObtemFormaPagtoParaNf(nf.IdNf);

                if (!string.IsNullOrEmpty(nf.InfCompl) && !string.IsNullOrWhiteSpace(nf.InfCompl) && nf.InfCompl.Trim(' ') != ".")
                {
                    XmlElement infAdic = doc.CreateElement("infAdic");
                    ManipulacaoXml.SetNode(doc, infAdic, "infAdFisco", Formatacoes.TrataTextoDocFiscal(
                        string.Format("Valor total do FCP {0}; Valor total do FCP retido por ST {1}; Valor total do FCP retido anteriormente por ST {2}.",
                        nf.ValorFcp.ToString("C"), nf.ValorFcpSt.ToString("C"), (0).ToString("C"))));
                    ManipulacaoXml.SetNode(doc, infAdic, "infCpl", Formatacoes.TrataTextoDocFiscal(nf.InfCompl));
                    infNFe.AppendChild(infAdic);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir informações complementares da NFe no XML.", ex);
            }

            #endregion

            #region Exportação

            if (isExportacao)
            {
                XmlElement exporta = doc.CreateElement("exporta");

                ManipulacaoXml.SetNode(doc, exporta, "UFSaidaPais", Formatacoes.TrataTextoDocFiscal(nf.UfEmbarque));
                ManipulacaoXml.SetNode(doc, exporta, "xLocExporta", Formatacoes.TrataTextoDocFiscal(nf.LocalEmbarque));
                ManipulacaoXml.SetNode(doc, exporta, "xLocDespacho", Formatacoes.TrataTextoDocFiscal(nf.LocalDespacho));

                infNFe.AppendChild(exporta);
            }

            #endregion

            #endregion

            #region Assina NFe

            try
            {
                if (!preVisualizar)
                {
                    MemoryStream stream = new MemoryStream();
                    doc.Save(stream);

                    using (stream)
                    {
                        // Classe responsável por assinar o xml da NFe
                        AssinaturaDigital AD = new AssinaturaDigital();

                        System.Security.Cryptography.X509Certificates.X509Certificate2 cert = Certificado.GetCertificado((uint)loja.IdLoja);

                        if (DateTime.Now > cert.NotAfter)
                            throw new Exception("O certificado digital cadastrado está vencido, insira um novo certificado para emitir esta nota. Data Venc.: " + cert.GetExpirationDateString());

                        int resultado = AD.Assinar(ref doc, "infNFe", cert);

                        if (resultado > 0)
                            throw new Exception(AD.mensagemResultado);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao assinar NFe." + ex.Message);
            }

            #endregion

            #region QR Code NFC-e

            if (nf.Consumidor && !preVisualizar)
            {
                XmlElement infNFeSupl = doc.CreateElement("infNFeSupl");

                var xmlSignature = doc["NFe"]["Signature"];
                var xmlInf = doc["NFe"]["infNFe"];

                var digestValue = RelDAL.NFeDAO.Instance.GetNodeValue(xmlSignature, "SignedInfo/Reference", "DigestValue");

                var link = ObtemLinkQrCodeNfce(nf, digestValue);
                var urlChave = NFeUtils.GetWebService.UrlConsultaPorChaveAcesso(cidEmitente.NomeUf, ConfigNFe.TipoAmbiente);

                ManipulacaoXml.SetNode(doc, infNFeSupl, "qrCode", link);
                ManipulacaoXml.SetNode(doc, infNFeSupl, "urlChave", urlChave);

                doc["NFe"].InnerXml = xmlInf.OuterXml + infNFeSupl.OuterXml + xmlSignature.OuterXml;
            }

            #endregion

            #region Valida XML

            try
            {
                if (!preVisualizar)
                    ValidaXML.Validar(doc, ValidaXML.TipoArquivoXml.NFe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            #region Salva arquivo XML da NFe

            try
            {
                string fileName = Utils.GetNfeXmlPath + doc["NFe"]["infNFe"].GetAttribute("Id").Remove(0, 3) + "-nfe.xml";

                if (File.Exists(fileName))
                    File.Delete(fileName);

                doc.Save(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao salvar arquivo xml da NFe. " + ex.Message);
            }

            #endregion

            return doc;
        }

        public void VerificaEstoque(ProdutosNf pnf, ProdutosNf[] lstProdNfValida, uint idLoja)
        {
            VerificaEstoque(null, pnf, lstProdNfValida, idLoja);
        }

        public void VerificaEstoque(GDASession session, ProdutosNf pnf, ProdutosNf[] lstProdNfValida, uint idLoja)
        {
            uint idNaturezaOperacao = pnf.IdNaturezaOperacao > 0 ? pnf.IdNaturezaOperacao.Value : NotaFiscalDAO.Instance.ObtemIdNaturezaOperacao(session, pnf.IdNf);
            bool alteraEstoqueFiscal = NaturezaOperacaoDAO.Instance.AlterarEstoqueFiscal(session, idNaturezaOperacao);
            bool alteraEstoqueTerceiros = CfopDAO.Instance.AlterarEstoqueTerceiros(session, NaturezaOperacaoDAO.Instance.ObtemIdCfop(session, idNaturezaOperacao));

            // Ao verificar o estoque, deve ser verificado cada produto para baixa configurados para este produto nf
            foreach (ProdutoBaixaEstoqueFiscal pBaixa in ProdutoBaixaEstoqueFiscalDAO.Instance.GetByProd(session, pnf.IdProd))
            {
                float qtdEstoque = 0;

                if (alteraEstoqueFiscal || alteraEstoqueTerceiros)
                {
                    // Valida o estoque deste produto somando a quantidade de todos os produtos iguais à este 
                    // (ou o produto em si ou o produto que ele dá baixa) inseridos nesta nota
                    foreach (ProdutosNf pVal in lstProdNfValida)
                    {
                        foreach (ProdutoBaixaEstoqueFiscal pBaixaComparacao in ProdutoBaixaEstoqueFiscalDAO.Instance.GetByProd(session, pVal.IdProd))
                            if (pBaixa.IdProdBaixa == pBaixaComparacao.IdProdBaixa)
                                qtdEstoque += (float)Math.Round(ProdutosNfDAO.Instance.ObtemQtdDanfe(session, pVal, true) * pBaixaComparacao.Qtde, 2);
                    }

                    // O valor é arredondado porque a quantidade do produto na nota fiscal considera 4 casas decimais,
                    // já a quantidade do produto_loja considera 2 casas decimais. Chamado 8299.
                    qtdEstoque = (float)Math.Round(qtdEstoque, 2);
                }

                if (alteraEstoqueFiscal && ProdutoLojaDAO.Instance.GetEstoqueFiscal(session, (uint)pBaixa.IdProdBaixa, idLoja) < (float)Math.Round(qtdEstoque, 2))
                    throw new Exception("A quantidade do produto " + pnf.DescrProduto.Replace("'", "") +
                        " (Baixa: " + ProdutoDAO.Instance.GetCodInterno(session, pBaixa.IdProdBaixa) + ")" +
                        " ou um de seus produtos de baixa é maior do que a quantidade no estoque fiscal.");

                if (alteraEstoqueTerceiros && ProdutoLojaDAO.Instance.ObtemEstoqueTerceiros(session, (uint)pBaixa.IdProdBaixa, idLoja) < (float)Math.Round(qtdEstoque, 2))
                    throw new Exception("A quantidade do produto " + pnf.DescrProduto.Replace("'", "") +
                        " (Baixa: " + ProdutoDAO.Instance.GetCodInterno(session, pBaixa.IdProdBaixa) + ")" +
                        " ou um de seus produtos de baixa é maior do que a quantidade no estoque em posse de terceiros.");
            }
        }

        /// <summary>
        /// Cria XML da NF para ser assinada e enviada para o webservice da receita
        /// </summary>
        public string EmitirNf(uint idNf, bool preVisualizar, bool offline)
        {
            return EmitirNf(idNf, preVisualizar, offline, true);
        }

        /// <summary>
        /// Cria XML da NF para ser assinada e enviada para o webservice da receita
        /// </summary>
        public string EmitirNf(uint idNf, bool preVisualizar, bool offline, bool usarFilaOperacoes)
        {
            if (usarFilaOperacoes)
                FilaOperacoes.NotaFiscalEmitir.AguardarVez();

            try
            {
                var loja = LojaDAO.Instance.GetElement(ObtemIdLoja(idNf));

                #region Verificações para emissão assíncrona

                if (loja.Uf.ToUpper() == "BA" || loja.Uf.ToUpper() == "SP")
                {
                    #region Permite o envio do lote mais de uma vez somente em um intervalo maior que 2 minutos

                    /* Chamado 35334. */
                    if (ExecuteScalar<bool>(string.Format(
                        @"SELECT COUNT(*)>0 FROM log_nf ln
                        WHERE ln.IdNf={0} AND
                            ln.Codigo=103 AND
                            ln.DataHora>=DATE_ADD(NOW(), INTERVAL - 2 MINUTE)", idNf)))
                        return "Lote em processamento.";

                    #endregion

                    #region Bloqueia a emissão da nota caso já esteja autorizada

                    /* Chamado 41975. */
                    if (ExecuteScalar<bool>(string.Format(
                        @"SELECT COUNT(*)>0 FROM nota_fiscal nf
                        WHERE nf.IdNf={0} AND nf.Situacao={1}",
                            idNf, (int)NotaFiscal.SituacaoEnum.Autorizada)))
                        return "A nota fiscal já está autorizada.";

                    #endregion

                    #region Consulta a situação da nota caso o lote tenha sido enviado há mais de 2 minutos em um intervalo de 10 minutos

                    /* Chamado 35334. */
                    if (ExecuteScalar<bool>(string.Format(
                        @"SELECT COUNT(*)>0 FROM log_nf ln
                        WHERE ln.IdNf={0} AND
                            ln.Codigo=103 AND
                            ln.DataHora>=DATE_ADD(NOW(), INTERVAL - 10 MINUTE)", idNf)))
                        return ConsultaSituacao.ConsultaSitNFe(idNf);

                    #endregion
                }

                #endregion

                #region Bloqueia a emissão para nota fiscal com valor maior que as liberações/pedidos

                if (FiscalConfig.NotaFiscalConfig.BloquearEmissaoNotaFiscalDePedidoMaiorQueOsPedidos)
                {
                    var ped = PedidosNotaFiscalDAO.Instance.GetByNf(idNf);

                    if (ped.Count() > 0 && ped.Count(x => x.IdCarregamento.HasValue) == 0)
                    {
                        var ids = new List<uint>(ped.Where(x => x.IdPedido.HasValue).Select(x => x.IdPedido.Value));

                        if (ids.Count > 0)
                        {
                            var total = PedidoDAO.Instance.ExecuteScalar<decimal>(@"
                                Select sum(Coalesce(pe.total, p.total)) 
                                From pedido p
                                    Left Join pedido_espelho pe On (p.idPedido=pe.idPedido)
                                Where p.idPedido In (" + String.Join(",", ids) + ")");

                            if (ObtemValorCampo<decimal>("totalNota", "idNf=" + idNf) - (decimal)0.05 > total)
                                throw new Exception("Não é possível emitir essa nota fiscal: o valor da nota fiscal " +
                                    "é maior que o valor do(s) pedido(s) que a compõem.");
                        }
                    }
                }

                #endregion

                #region Atualiza campo tipoAmbiente da NF-e

                // Atualiza campo tipoAmbiente da NFe
                objPersistence.ExecuteCommand("Update nota_fiscal set tipoAmbiente=" + (int)ConfigNFe.TipoAmbiente + " where idNf=" + idNf);

                #endregion

                #region Emissão de NFC-e em modo de contingência offline

                if (offline)
                {
                    var nf = GetElement(idNf);

                    if (!nf.Consumidor)
                        throw new Exception("Apenas notas fiscais de consumidor (NFC-e) podem ser emitidas em contingência offline.");
                    
                    var idCfop = NaturezaOperacaoDAO.Instance.ObtemIdCfop(nf.IdNaturezaOperacao.GetValueOrDefault(0));
                    var cfop = CfopDAO.Instance.ObtemCodInterno(idCfop);

                    if (cfop[0] == '2' || cfop[0] == '3' || cfop[0] == '6' || cfop[0] == '7')
                        throw new Exception("A contingência offline só pode ser emitida para operações internas.");

                    if (nf.ModalidadeFrete != ModalidadeFrete.SemTransporte)
                        throw new Exception("A contingência offline só pode ser emitida em operações presenciais.");

                    objPersistence.ExecuteCommand("Update nota_fiscal set formaEmissao=" + (int)NotaFiscal.TipoEmissao.ContingenciaNFCe + " where idNf=" + idNf);

                    GerarXmlNf(idNf, preVisualizar);

                    // Altera situação para contingência offline
                    AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.ContingenciaOffline);

                    return null;
                }

                #endregion

                XmlDocument doc = GerarXmlNf(idNf, preVisualizar);

                #region Envia NFe para SEFAZ e atualiza tipo de ambiente da NFe

                if (!preVisualizar)
                {
                    // Verifica se há uma outra nota com a mesma chave de acesso
                    if (ExecuteScalar<bool>("Select Count(*)>1 From nota_fiscal where chaveAcesso=(Select chaveAcesso From nota_fiscal Where idNf=" + idNf + ")"))
                        throw new Exception("Existe mais de uma nota com a mesma chave de acesso desta nota, acerte esta situação antes de emitir esta nota.");

                    try
                    {
                        // Altera situação para processo de emissão
                        AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.ProcessoEmissao);

                        // Envia NFe para SEFAZ
                        var retorno = EnviaXML.EnviaNFe(doc, idNf);

                        //Altera a situação do faturamento
                        var idsPedidoNf = GetIdsPedidoNotaFiscal(null, idNf);
                        if (idsPedidoNf.Any())
                            CarregamentoDAO.Instance.AlterarSituacaoFaturamentoCarregamentos(null, idsPedidoNf);

                        return retorno;
                    }
                    catch (Exception ex)
                    {
                        // Altera situação para falha ao emitir
                        AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                        throw ex;
                    }
                }

                #endregion

                return null;
            }
            finally
            {
                if (usarFilaOperacoes)
                    FilaOperacoes.NotaFiscalEmitir.ProximoFila();
            }
        }

        static volatile object _emitirNfcOfflineLock = new object();

        /// <summary>
        /// Autoriza a NFC-e emitida em contigencia offline anteriormente
        /// </summary>
        public string EmitirNfcOffline(uint idNf)
        {
            lock (_emitirNfcOfflineLock)
            {
                #region Envia NFe para SEFAZ e atualiza tipo de ambiente da NFe

                // Verifica se há uma outra nota com a mesma chave de acesso
                if (ExecuteScalar<bool>("Select Count(*)>1 From nota_fiscal where chaveAcesso=(Select chaveAcesso From nota_fiscal Where idNf=" + idNf + ")"))
                    throw new Exception("Existe mais de uma nota com a mesma chave de acesso desta nota, acerte esta situação antes de emitir esta nota.");

                var chaveAcesso = ObtemChaveAcesso(idNf);
                var context = System.Web.HttpContext.Current;

                // Verifica se NFe existe
                if (!File.Exists(Utils.GetNfeXmlPathInternal(context) + chaveAcesso + "-nfe.xml"))
                    throw new Exception("Arquivo da NF-e não encontrado.");

                // Busca arquivo XML da NFe
                var xmlNFe = new XmlDocument();
                xmlNFe.Load(Utils.GetNfeXmlPathInternal(context) + chaveAcesso + "-nfe.xml");

                // Envia NFe para SEFAZ
                return EnviaXML.EnviaNFe(xmlNFe, idNf);

                #endregion
            }
        }

        #endregion

        #region Gerar XML da NF-e para cancelamento

        /// <summary>
        /// Cancelamento de NFe
        /// </summary>
        public XmlDocument CancelarNFeXmlEvt(GDASession session, string justificativa, NotaFiscal nf)
        {
            if (nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada)
                throw new Exception("Esta nota já foi cancelada.");

            // Apenas notas autorizadas com falha no cancelamento ou em processo de cancelamento podem ser canceladas
            if (nf.Situacao != (int)NotaFiscal.SituacaoEnum.Autorizada && nf.Situacao != (int)NotaFiscal.SituacaoEnum.FalhaCancelar
                && nf.Situacao != (int)NotaFiscal.SituacaoEnum.ProcessoCancelamento)
                throw new Exception("Apenas Notas Fiscais autorizadas ou com falha no cancelamento podem ser canceladas.");

            // Se a nota estiver autorizada mas não possuir protocolo de autorização, não pode ser cancelada
            if (nf.Situacao == (int)NotaFiscal.SituacaoEnum.Autorizada && String.IsNullOrEmpty(nf.NumProtocolo))
                throw new Exception("Esta NFe não pode ser cancelada por não possuir protocolo de autorização.");

            // Verifica se há uma outra nota com a mesma chave de acesso
            if (ExecuteScalar<bool>(session, "Select Count(*)>1 From nota_fiscal where chaveAcesso=?chaveAcesso", new GDAParameter("?chaveAcesso", nf.ChaveAcesso)))
                throw new Exception("Existe mais de uma nota com a mesma chave de acesso desta nota, acerte esta situação antes de emitir esta nota.");

            #region Monta XML

            XmlDocument xmlCanc = new XmlDocument();
            XmlNode declarationNode = xmlCanc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlCanc.AppendChild(declarationNode);

            XmlElement evento = xmlCanc.CreateElement("evento");

            evento.SetAttribute("versao", ConfigNFe.VersaoCancelamento);
            evento.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
            xmlCanc.AppendChild(evento);

            XmlElement infEvento = xmlCanc.CreateElement("infEvento");

            //Identificador da TAG a ser assinada, a regra de formação 
            //do Id é: 
            //“ID” + tpEvento +  chave da NF-e + nSeqEvento 
            infEvento.SetAttribute("Id", "ID110111" + nf.ChaveAcesso + "1".PadLeft(2, '0'));
            evento.AppendChild(infEvento);

            // Código do órgão de recepção do Evento. Utilizar a Tabela 
            // do IBGE, utilizar 90 para identificar o Ambiente Nacional. 
            XmlElement cOrgao = xmlCanc.CreateElement("cOrgao");
            uint idCidade = LojaDAO.Instance.ObtemValorCampo<uint>(session, "idCidade", "idLoja=" + NotaFiscalDAO.Instance.ObtemIdLoja(session, nf.IdNf));
            string codIbgeUf = CidadeDAO.Instance.ObtemValorCampo<string>(session, "codIbgeUf", "idCidade=" + idCidade);
            cOrgao.InnerText = codIbgeUf;
            infEvento.AppendChild(cOrgao);

            // Identificação do Amb
            // 1 - Produção 
            // 2 – Homologação 
            XmlElement tpAmb = xmlCanc.CreateElement("tpAmb");
            tpAmb.InnerText = nf.TipoAmbiente.ToString();
            infEvento.AppendChild(tpAmb);

            //Autor do evento
            XmlElement CNPJ = xmlCanc.CreateElement("CNPJ");
            CNPJ.InnerText = LojaDAO.Instance.ObtemValorCampo<string>
                (session, "cnpj", "idLoja=" + NotaFiscalDAO.Instance.ObtemIdLoja(session, nf.IdNf))
                .Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty); ;
            infEvento.AppendChild(CNPJ);

            XmlElement chNFe = xmlCanc.CreateElement("chNFe");
            chNFe.InnerText = nf.ChaveAcesso;
            infEvento.AppendChild(chNFe);

            XmlElement dhEvento = xmlCanc.CreateElement("dhEvento");
            dhEvento.InnerText = DateTime.Now.AddMinutes(-2).ToString("yyyy-MM-ddTHH:mm:sszzz");
            infEvento.AppendChild(dhEvento);

            //Código do de evento = 110111
            XmlElement tpEvento = xmlCanc.CreateElement("tpEvento");
            tpEvento.InnerText = "110111";
            infEvento.AppendChild(tpEvento);

            XmlElement nSeqEvento = xmlCanc.CreateElement("nSeqEvento");
            nSeqEvento.InnerText = "1";
            infEvento.AppendChild(nSeqEvento);

            XmlElement verEvento = xmlCanc.CreateElement("verEvento");
            verEvento.InnerText = "1.00";
            infEvento.AppendChild(verEvento);

            XmlElement detEvento = xmlCanc.CreateElement("detEvento");
            detEvento.SetAttribute("versao", "1.00");

            ManipulacaoXml.SetNode(xmlCanc, detEvento, "descEvento", "Cancelamento");
            ManipulacaoXml.SetNode(xmlCanc, detEvento, "nProt", nf.NumProtocolo);
            ManipulacaoXml.SetNode(xmlCanc, detEvento, "xJust", Formatacoes.TrataStringDocFiscal(justificativa));

            infEvento.AppendChild(detEvento);

            #endregion

            #region Assina XML

            try
            {
                // Classe responsável por assinar o xml da NFe
                AssinaturaDigital AD = new AssinaturaDigital();

                int resultado = AD.Assinar(ref xmlCanc, "infEvento", Certificado.GetCertificado(session, nf.IdLoja.Value));

                if (resultado > 0)
                    throw new Exception(AD.mensagemResultado);
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Cancelamento", 1, "Falha ao cancelar NFe. " + ex.Message);

                throw new Exception("Falha ao assinar pedido de cancelamento." + ex.Message);
            }

            #endregion

            #region Valida XML

            try
            {
                ValidaXML.Validar(xmlCanc, ValidaXML.TipoArquivoXml.EvtCancNfe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            AlteraSituacao(session, nf.IdNf, NotaFiscal.SituacaoEnum.ProcessoCancelamento);

            // Atualiza nf informando o motivo do cancelamento
            objPersistence.ExecuteCommand(session, "Update nota_fiscal set motivoCanc=?motivo Where idNf=" + nf.IdNf,
                new GDAParameter("?motivo", justificativa));

            return xmlCanc;
        }

        #endregion

        #region Gerar XML da NF-e para Inutilização

        /// <summary>
        /// Inutilização de NFe
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="justificativa"></param>
        /// <returns></returns>
        public XmlDocument InutilizarNFeXml(uint idNf, string justificativa)
        {
            NotaFiscal nf = GetElement(idNf);

            if (nf.Situacao == (int)NotaFiscal.SituacaoEnum.Inutilizada)
                throw new Exception("A numeração desta nota já foi inutilizada.");

            // Apenas notas abertas, não emitidas, com falha na inutilização ou em processo de inutilização podem ser inutilizadas
            if (nf.Situacao != (int)NotaFiscal.SituacaoEnum.Aberta && nf.Situacao != (int)NotaFiscal.SituacaoEnum.FalhaInutilizar &&
                nf.Situacao != (int)NotaFiscal.SituacaoEnum.FalhaEmitir && nf.Situacao != (int)NotaFiscal.SituacaoEnum.NaoEmitida &&
                nf.Situacao != (int)NotaFiscal.SituacaoEnum.ProcessoInutilizacao)
                throw new Exception("Apenas Notas Fiscais abertas, não emitidas ou com falha ao inutilizar podem ser inutilizadas.");

            if (nf.IdLoja == null)
                throw new Exception("A nota precisa ter um emitente para ser inutilizada.");

            #region Monta XML

            // Monta o atributo ID, necessário para identificar esse pedido de inutilização
            Loja emitente = LojaDAO.Instance.GetElement(nf.IdLoja.Value);
            string codUf = emitente.CodUf;
            string idInut = Formatacoes.TrataStringDocFiscal("ID" + codUf + DateTime.Now.ToString("yy") + emitente.Cnpj + nf.Modelo.PadLeft(2, '0') +
                nf.Serie.ToString().PadLeft(3, '0') + nf.NumeroNFe.ToString().PadLeft(9, '0') + nf.NumeroNFe.ToString().PadLeft(9, '0'));

            XmlDocument xmlInut = new XmlDocument();
            XmlNode declarationNode = xmlInut.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlInut.AppendChild(declarationNode);

            XmlElement inutNFe = xmlInut.CreateElement("inutNFe");
            inutNFe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
            inutNFe.SetAttribute("versao", ConfigNFe.VersaoInutilizacao);
            xmlInut.AppendChild(inutNFe);

            XmlElement infInut = xmlInut.CreateElement("infInut");
            infInut.SetAttribute("Id", idInut);
            inutNFe.AppendChild(infInut);
            ManipulacaoXml.SetNode(xmlInut, infInut, "tpAmb", ((int)ConfigNFe.TipoAmbiente).ToString());
            ManipulacaoXml.SetNode(xmlInut, infInut, "xServ", "INUTILIZAR");
            ManipulacaoXml.SetNode(xmlInut, infInut, "cUF", codUf.ToString());
            ManipulacaoXml.SetNode(xmlInut, infInut, "ano", DateTime.Now.ToString("yy"));
            ManipulacaoXml.SetNode(xmlInut, infInut, "CNPJ", Formatacoes.TrataStringDocFiscal(emitente.Cnpj));
            ManipulacaoXml.SetNode(xmlInut, infInut, "mod", nf.Modelo);
            ManipulacaoXml.SetNode(xmlInut, infInut, "serie", nf.Serie.ToString());
            ManipulacaoXml.SetNode(xmlInut, infInut, "nNFIni", nf.NumeroNFe.ToString());
            ManipulacaoXml.SetNode(xmlInut, infInut, "nNFFin", nf.NumeroNFe.ToString());
            ManipulacaoXml.SetNode(xmlInut, infInut, "xJust", Formatacoes.TrataStringDocFiscal(justificativa));

            #endregion

            #region Assina XML

            try
            {
                // Classe responsável por assinar o xml da NFe
                AssinaturaDigital AD = new AssinaturaDigital();

                int resultado = AD.Assinar(ref xmlInut, "infInut", Certificado.GetCertificado(nf.IdLoja.Value));

                if (resultado > 0)
                    throw new Exception(AD.mensagemResultado);
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(idNf, "Inutilização", 1, "Falha ao inutilizar NFe. " + ex.Message);

                throw new Exception("Falha ao assinar pedido de inutilização." + ex.Message);
            }

            #endregion

            #region Valida XML

            try
            {
                ValidaXML.Validar(xmlInut, ValidaXML.TipoArquivoXml.InutNFe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.ProcessoInutilizacao);

            // Atualiza nf informando o motivo da inutilização
            objPersistence.ExecuteCommand("Update nota_fiscal set motivoInut=?motivo Where idNf=" + idNf,
                new GDAParameter("?motivo", justificativa));

            return xmlInut;
        }

        #endregion

        #region Inclusão de protocolo de recebimento da NF-e

        /// <summary>
        /// Inclui o protocolo de recebimento na NF-e.
        /// </summary>
        /// <param name="path">O caminho do arquivo no servidor.</param>
        /// <param name="xmlProt">O XML que será adicionado ao fim da NF-e.</param>
        public void IncluiProtocoloXML(string path, XmlNode xmlProt)
        {
            if (!File.Exists(path))
                throw new Exception("Não foi possível anexar o protocolo de autorização, xml da nota não encontrado.");

            //XmlDocument xml = new XmlDocument();

            //string infProtTeste = "<infProt Id=\"ID231110000128921\" xmlns=\"http://www.portalfiscal.inf.br/nfe\"><tpAmb>1</tpAmb>" +
            //    "<verAplic>SCAN_4.01</verAplic><chNFe>31110403979137000128559000000000013090203051</chNFe>" +
            //    "<dhRecbto>2011-04-08T08:36:35</dhRecbto><nProt>231110000128921</nProt><digVal>sDc8J+R/oKYwLJ+EhcF/bPved3s=</digVal>" +
            //    "<cStat>100</cStat><xMotivo>Autorizado o uso da NF-e</xMotivo></infProt>";

            //if (xmlProt == null)
            //{
            //    xml.LoadXml(infProtTeste);
            //    xmlProt = xml.FirstChild;
            //}

            // Carrega o conteúdo do arquivo XML da NF-e
            string conteudoArquivoNFe = "";
            using (FileStream arquivoNFe = File.OpenRead(path))
            using (StreamReader textoArquivoNFe = new StreamReader(arquivoNFe))
                conteudoArquivoNFe = textoArquivoNFe.ReadToEnd();

            /* Chamado 66669. */
            if (conteudoArquivoNFe.IndexOf("<Signature") == -1)
                throw new Exception("Não foi possível identificar a assinatura digital da NFe no XML. Reabra a nota fiscal e a emita novamente.");

            // Salva o texto do arquivo XML junto com o texto da autorização da NF-e
            conteudoArquivoNFe = conteudoArquivoNFe.Insert(conteudoArquivoNFe.IndexOf("<Signature"), xmlProt.InnerXml);
            using (FileStream arquivoNFe = File.OpenWrite(path))
            using (StreamWriter salvaArquivoNFe = new StreamWriter(arquivoNFe))
            {
                salvaArquivoNFe.Write(conteudoArquivoNFe);
                salvaArquivoNFe.Flush();
            }
        }

        #endregion

        #region Importar XML

        #endregion

        #region Retorno da emissão da NF-e

        private void SeparaValoresAReceber(NotaFiscal nf)
        {
            // Em caso de exceção salva no banco de dados,
            // mas não impede a execução da autorização da nota

            try
            {
                //Chamado 17246 - Não pode fazer a separação de notas que não tenham cliente
                if (nf.FormaPagto != (int)NotaFiscal.FormaPagtoEnum.AVista && nf.IdCliente.GetValueOrDefault() > 0)
                {
                    new SeparacaoValoresFiscaisEReaisContasReceber().SepararComTransacao(nf.IdNf);
                }
            }
            catch (Exception ex)
            {
                var erro = string.Format("Falha na separação de valores da NFe {0}. {1}{2}", nf.IdNf,
                    ex.Message != null ? string.Format("{0}. ", ex.Message) : string.Empty,
                    ex.InnerException != null && ex.InnerException.Message != null ? ex.InnerException.Message : string.Empty);

                LogNfDAO.Instance.NewLog(nf.IdNf, "Falha na Separacao", 0, erro);
            }
        }

        /// <summary>
        /// Chamado 17870
        /// Referencia a NF-e nas contas recebidas de pedidos que foram pagos antecipadamente
        /// ou que receberam sinal. Apenas quando usar comissão de contas recebidas.
        /// </summary>
        public void ReferenciaPedidosAntecipados(GDASession session, NotaFiscal nf)
        {
            try
            {
                if (nf == null || nf.IdNf == 0)
                    return;

                /* Chamado 38752. */
                if (!Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas ||
                    !FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber ||
                    nf.TipoDocumento != (int)NotaFiscal.TipoDoc.Saída)
                    return;

                var pedidosNf = PedidosNotaFiscalDAO.Instance.GetByNf(session, nf.IdNf);

                if (pedidosNf == null || pedidosNf.Length == 0)
                    return;

                var idsPedido = string.Join(",",
                    pedidosNf.
                        Where(f => f.IdPedido.GetValueOrDefault(0) > 0).
                        Select(f => f.IdPedido.Value.ToString()));

                if (string.IsNullOrWhiteSpace(idsPedido))
                    return;

                var sql = string.Format(@"SELECT cr.IdContaR 
                FROM contas_receber cr
                    INNER JOIN pedido p ON (cr.IdSinal = COALESCE(p.IdPagamentoAntecipado, p.IdSinal))
                WHERE (p.IdPagamentoAntecipado IS NOT NULL OR p.IdSinal IS NOT NULL) AND p.IdPedido IN ({0})", idsPedido);

                var idsContasReceber = ContasReceberDAO.Instance.GetValoresCampo(session, sql, "IdContaR");

                // Indica o IdNf nas contas fiscais
                if (string.IsNullOrWhiteSpace(idsContasReceber))
                    return;

                var idsContasR = string.Join(",", idsContasReceber.Split(',').Select(f => f.ToString()).ToArray());

                if (string.IsNullOrWhiteSpace(idsContasR))
                    return;

                ContasReceberDAO.Instance.ExecuteScalar<int>(session, string.Format("UPDATE contas_receber SET TipoConta=?tipoConta, IdNf={0} WHERE IdContaR IN ({1})",
                    nf.IdNf, idsContasR), new GDAParameter("?tipoConta", (byte)ContasReceber.TipoContaEnum.Contabil));
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Falha ao alterar a situação da nota fiscal.", 0, ex.Message);
            }
        }

        /// <summary>
        /// Remova a referencia da NF-e nas contas recebidas de pedidos que foram pagos antecipadamente
        /// ou que receberam sinal. Apenas quando usar comissão de contas recebidas.
        /// </summary>
        public void DesvinculaReferenciaPedidosAntecipados(int nf)
        {
            DesvinculaReferenciaPedidosAntecipados(null, nf);
        }

        /// <summary>
        /// Remova a referencia da NF-e nas contas recebidas de pedidos que foram pagos antecipadamente
        /// ou que receberam sinal. Apenas quando usar comissão de contas recebidas.
        /// </summary>
        public void DesvinculaReferenciaPedidosAntecipados(GDASession session, int nf)
        {
            /* Chamado 38752. */
            if (!Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas ||
                !FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber ||
                ObtemValorCampo<int>("TipoDocumento", string.Format("IdNf={0}", nf)) != (int)NotaFiscal.TipoDoc.Saída)
                return;

            var pedidosNf = PedidosNotaFiscalDAO.Instance.GetByNf(session, (uint)nf);
            var idsPedido = pedidosNf.Where(f => f.IdPedido.GetValueOrDefault(0) > 0).Select(f => f.IdPedido.Value).ToArray();

            var sql = string.Format(@"
                        SELECT IdContaR 
                        FROM contas_receber cr
                            INNER JOIN pedido p ON (cr.IdSinal = COALESCE(p.IdPagamentoAntecipado, p.IdSinal))
                        WHERE (p.IdPagamentoAntecipado IS NOT NULL OR  p.IdSinal IS NOT NULL)
                            AND p.IdPedido IN ({0})", string.Join(",", idsPedido.Select(f => f.ToString()).ToArray()));

            var idsContasReceber = ContasReceberDAO.Instance.GetValoresCampo(session, sql, "IdContaR");

            // Indica o IdNf nas contas fiscais
            string idsContasR = String.Join(",", idsContasReceber.Split(',').Select(f => f.ToString()).ToArray());

            if (string.IsNullOrEmpty(idsContasR))
                return;

            ContasReceberDAO.Instance.ExecuteScalar<int>(session, @"update contas_receber set TipoConta=?tipoConta, idNf=null
                 where idContaR in (" + idsContasR + ")", new GDAParameter("?tipoConta", (byte)ContasReceber.TipoContaEnum.NaoContabil));
        }

        /// <summary>
        /// Lote enviado com sucesso (salva número de recibo na nota fiscal)
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="numRecibo"></param>
        public void RetornoEnvioLote(uint idNf, string numRecibo)
        {
            // Adiciona na Nota Fiscal, o número do recibo do lote da mesma, para consultá-lo posteriormente
            if (!String.IsNullOrEmpty(numRecibo))
                objPersistence.ExecuteCommand("Update nota_fiscal set numRecibo='" + numRecibo + "' Where idNf=" + idNf);

            AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.ProcessoEmissao);
        }

        /// <summary>
        /// (Sobrecarga) Retorno da consulta de emissão da NFe
        /// </summary>
        public void RetornoEmissaoNFe(string chaveAcesso, XmlNode xmlProt)
        {
            RetornoEmissaoNFe(chaveAcesso, xmlProt, Utils.GetNfeXmlPath);
        }

        static volatile object _retornoEmissaoLock = new object();

        /// <summary>
        /// Retorno da consulta de emissão da NFe
        /// </summary>
        public void RetornoEmissaoNFe(string chaveAcesso, XmlNode xmlProt, string nfePath)
        {
            lock (_retornoEmissaoLock)
            {
                NotaFiscal nf = null;

                try
                {
                    nf = GetByChaveAcesso(null, chaveAcesso);

                    // Se a Nota Fiscal já tiver sido autorizada, não faz nada
                    if (nf.Situacao == (int)NotaFiscal.SituacaoEnum.Autorizada)
                        return;

                    if (xmlProt == null || xmlProt["infProt"] == null)
                        throw new Exception("Não foi possível recuperar os dados do retorno da emissão da nota fiscal.");

                    if (xmlProt["infProt"]["cStat"] == null)
                        throw new Exception("Não foi possível recuperar a situação da nota fiscal.");

                    string cStat = xmlProt["infProt"]["cStat"].InnerXml;

                    if (xmlProt["infProt"]["xMotivo"] == null)
                        throw new Exception("Não foi possível recuperar o motivo da rejeição da nota fiscal.");

                    // Gera log do ocorrido
                    LogNfDAO.Instance.NewLog(nf.IdNf, "Emissão", Glass.Conversoes.StrParaInt(cStat),
                        ConsultaSituacao.CustomizaMensagemRejeicao(nf.IdNf, xmlProt["infProt"]["xMotivo"].InnerXml));

                    // Atualiza número do protocolo de uso da NFe
                    if (cStat == "100" || cStat == "150")
                    {
                        // Anexa protocolo de autorização
                        string path = nfePath + nf.ChaveAcesso + "-nfe.xml";
                        IncluiProtocoloXML(path, xmlProt);

                        AutorizaNotaFiscal(nf, xmlProt);
                    }
                    // NFe denegada
                    else if (cStat == "301" || cStat == "302" || cStat == "303" || cStat == "110" || cStat == "205")
                    {
                        // Salva protocolo de denegação de uso
                        if (xmlProt["infProt"]["nProt"] != null)
                            objPersistence.ExecuteCommand("Update nota_fiscal set numProtocolo=?numProt Where idNf=" + nf.IdNf,
                                new GDAParameter[] { new GDAParameter("?numProt", xmlProt["infProt"]["nProt"].InnerXml) });
                        
                        // Altera situação da NFe para denegada
                        AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.Denegada);
                    }
                    // Se o código de retorno da emissão for > 105, algum erro ocorreu, altera situação da NF para Falha ao Emitir
                    else if (Convert.ToInt32(cStat) > 105)
                        AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                    BaixaCreditaEstoqueFiscalReal(cStat, nf);
                }
                catch (Exception ex)
                {
                    ErroDAO.Instance.InserirFromException(string.Format("Retorno Emissao NF. Chave acesso: {0}.", chaveAcesso), ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Realiza operações necessárias ao autorizar NF-e
        /// </summary>
        private void AutorizaNotaFiscal(NotaFiscal nf, XmlNode xmlProt)
        {
            var numProtocolo = string.Empty;

            // Tenta recuperar o protocolo de autorização
            if (xmlProt != null && xmlProt["infProt"] != null && xmlProt["infProt"]["nProt"] != null)
                numProtocolo = xmlProt["infProt"]["nProt"].InnerXml;

            // Tenta recuperar o protocolo de autorização se o de cima não der certo
            if (string.IsNullOrWhiteSpace(numProtocolo) && xmlProt != null && xmlProt["protNFe"] != null && xmlProt["protNFe"]["infProt"] != null && xmlProt["protNFe"]["infProt"]["nProt"] != null)
                numProtocolo = xmlProt["protNFe"]["infProt"]["nProt"].InnerXml;

            // Salva protocolo de autorização
            if (!string.IsNullOrWhiteSpace(numProtocolo))
                objPersistence.ExecuteCommand("Update nota_fiscal set numProtocolo=?numProt Where idNf=" + nf.IdNf,
                    new GDAParameter[] { new GDAParameter("?numProt", numProtocolo) });

            //Referencia a NF-e nas contas recebidas de pedidos que foram pagos antecipadamente ou que receberam sinal
            ReferenciaPedidosAntecipados(null, nf);

            // Altera situação da NFe para autorizada
            AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.Autorizada);
                                    
            // Separa os valores
            SeparaValoresAReceber(nf);

            // Envia email para o cliente com o XML
            EnviarEmailXml(nf);
        }

        #endregion

        #region Retorno da consulta do lote da NFe

        public void RetornoConsSitNFe(uint idNf, XmlNode xmlRetConsSit)
        {
            NotaFiscal nf = GetElement(idNf);

            // Se a Nota Fiscal já tiver sido autorizada, não faz nada
            if (nf.Situacao == (int)NotaFiscal.SituacaoEnum.Autorizada)
                return;

            string cStat = xmlRetConsSit?["cStat"]?.InnerXml ?? xmlRetConsSit?["protNFe"]?["infProt"]?["cStat"]?.InnerXml; ;

            // Gera log do ocorrido
            LogNfDAO.Instance.NewLog(nf.IdNf, "Consulta", cStat.StrParaInt(), xmlRetConsSit?["protNFe"]?["infProt"]?["xMotivo"].InnerXml);

            // Atualiza número do protocolo de uso da NFe
            if (cStat == "100" || cStat == "150")
            {
                // Anexa protocolo de autorizaçã
                string path = Utils.GetNfeXmlPath + nf.ChaveAcesso + "-nfe.xml";
                IncluiProtocoloXML(path, xmlRetConsSit?["protNFe"]);

                AutorizaNotaFiscal(nf, xmlRetConsSit);
            }
            else if (cStat == "206" || cStat == "256") // NF-e já está inutilizada
                AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.Inutilizada);
            else if (cStat == "218" || cStat == "420" || cStat == "101" || cStat == "151") // NF-e já está cancelada
                AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.Cancelada);
            else if (cStat == "220") // NF-e já está autorizada há mais de 7 dias
                AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.Autorizada);
            // NFe denegada
            else if (cStat == "301" || cStat == "302" || cStat == "110" || cStat == "205")
            {
                // Salva protocolo de denegação de uso
                if (xmlRetConsSit["protNFe"] != null)
                    objPersistence.ExecuteCommand("Update nota_fiscal set numProtocolo=?numProt Where idNf=" + nf.IdNf,
                        new GDAParameter[] { new GDAParameter("?numProt", xmlRetConsSit["protNFe"]["infProt"]["nProt"].InnerXml) });

                // Altera situação da NFe para denegada
                AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.Denegada);
            }
            // Se o código de retorno da emissão for > 105, algum erro ocorreu, altera situação da NF para Falha ao Emitir
            else if (Convert.ToInt32(cStat) > 105)
                AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

            BaixaCreditaEstoqueFiscalReal(cStat, nf);
        }

        #endregion

        #region Retorno de cancelamento da NF-e

        /// <summary>
        /// Retorno do cancelamento da NF-e, grava log e altera situação da NF-e
        /// </summary>
        public string RetornoEvtCancelamentoNFe(GDASession session, uint idNf, string justificativa, XmlNode xmlRetCanc,
            bool cancelarSeparacaoValores)
        {
            // Se o xml de retorno for nulo, ocorreu alguma falha no processo
            if (xmlRetCanc == null)
            {
                LogNfDAO.Instance.NewLog(idNf, "Cancelamento", 1, "Falha ao cancelar NFe. Sem retorno. ");

                NotaFiscalDAO.Instance.AlteraSituacao(session, idNf, NotaFiscal.SituacaoEnum.FalhaCancelar);

                throw new Exception("Servidor da SEFAZ não respondeu em tempo hábil, tente novamente.");
            }

            try
            {
                // Lê Xml de retorno do envio do lote
                string status = xmlRetCanc["cStat"].InnerText;
                string resposta = xmlRetCanc["xMotivo"].InnerText;
                int statusProcessamento = Glass.Conversoes.StrParaInt(xmlRetCanc["retEvento"]["infEvento"]["cStat"].InnerText);
                string respostaProcessamento = xmlRetCanc["retEvento"]["infEvento"]["xMotivo"].InnerText;

                // Insere o log de cancelamento desta NF
                LogNfDAO.Instance.NewLog(idNf, "Cancelamento", statusProcessamento, respostaProcessamento);

                // Se o código de retorno for 135 ou 136-Cancelamento de NF-e homologado, altera situação para cancelada
                // e estorna produtos no estoque fiscal
                if (statusProcessamento == 135 || statusProcessamento == 136)
                {
                    BaixaCreditaEstoqueFiscalReal(session, statusProcessamento.ToString(), GetElement(idNf));

                    // Insere protocolo de cancelamento na NFe
                    objPersistence.ExecuteCommand(session, "Update nota_fiscal set numProtocoloCanc=?numProt Where idNf=" + idNf,
                        new GDAParameter[] { new GDAParameter("?numProt", xmlRetCanc["retEvento"]["infEvento"]["nProt"].InnerText) });

                    AlteraSituacao(session, idNf, NotaFiscal.SituacaoEnum.Cancelada);

                    if (cancelarSeparacaoValores)
                    {
                        new SeparacaoValoresFiscaisEReaisContasReceber().Cancelar(session, idNf);
                    }

                    DesvinculaReferenciaPedidosAntecipados(session, (int)idNf);
                }
                else if (statusProcessamento == 206 || statusProcessamento == 256) // NF-e já está inutilizada
                    AlteraSituacao(session, idNf, NotaFiscal.SituacaoEnum.Inutilizada);
                else if (statusProcessamento == 218 || statusProcessamento == 420) // NF-e já está cancelada
                    AlteraSituacao(session, idNf, NotaFiscal.SituacaoEnum.Cancelada);
                else if (statusProcessamento == 220) // NF-e já está autorizada há mais de 7 dias
                    AlteraSituacao(session, idNf, NotaFiscal.SituacaoEnum.Autorizada);
                else if (statusProcessamento == 110 || statusProcessamento == 301 || statusProcessamento == 302 || statusProcessamento == 205)
                    AlteraSituacao(session, idNf, NotaFiscal.SituacaoEnum.Denegada);
                // Altera situação da NF para Falha ao Cancelar
                else
                    AlteraSituacao(session, idNf, NotaFiscal.SituacaoEnum.FalhaCancelar);

                LogMovimentacaoNotaFiscalDAO.Instance.DeleteFromNf(session, idNf);

                if (statusProcessamento == 135 || statusProcessamento == 136)
                    return "Cancelamento efetuado.";
                else
                    return "Falha ao cancelar NFe. " + respostaProcessamento;

            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(idNf, "Cancelamento", 1, "Falha ao cancelar NFe. Falha ao processar retorno. ");
                ErroDAO.Instance.InserirFromException("Falha ao processar retorno do cancelamento - IdNF " + idNf, ex);

                NotaFiscalDAO.Instance.AlteraSituacao(session, idNf, NotaFiscal.SituacaoEnum.FalhaCancelar);

                throw new Exception("Falha ao processar retorno, tente novamente.");
            }
        }

        #endregion

        #region Baixa/Credita Estoque Fiscal/Real

        private void BaixaCreditaEstoqueFiscalReal(string cStat, NotaFiscal nf)
        {
            BaixaCreditaEstoqueFiscalReal(null, cStat, nf);
        }

        private void BaixaCreditaEstoqueFiscalReal(GDASession session, string cStat, NotaFiscal nf)
        {
            // Busca produtos da nota
            ProdutosNf[] lstProd = ProdutosNfDAO.Instance.GetByNf(session, nf.IdNf);

            // 100 - Autorizada para Uso
            if (cStat == "100" || cStat == "150")
            {
                #region Baixa/Credita estoque fiscal/real

                // Se for entrada e ainda não tiver dado entrada no estoque, credita estoque fiscal
                if (nf.TipoDocumento == 1)
                {
                    if (nf.EntrouEstoque == false)
                    {
                        var mensagemLog = string.Empty;

                        if (!nf.GerarEstoqueReal)
                            mensagemLog += "Nota não está configurada para geração de estoque Real. ";
                        if (EstoqueConfig.EntradaEstoqueManual)
                            mensagemLog += "Nota não gerou Estoque real pois a Configuração (Entrada de Estoque Manual) Está marcada. ";

                        if (!string.IsNullOrEmpty(mensagemLog))
                        {
                            var logMovNotaFiscal = new LogMovimentacaoNotaFiscal();
                            logMovNotaFiscal.IdNf = nf.IdNf;
                            logMovNotaFiscal.MensagemLog = mensagemLog;
                            logMovNotaFiscal.DataCad = DateTime.Now;
                            logMovNotaFiscal.Usucad = UserInfo.GetUserInfo.CodUser;
                            LogMovimentacaoNotaFiscalDAO.Instance.Insert(session, logMovNotaFiscal);
                        }

                        foreach (ProdutosNf p in lstProd)
                        {
                            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd((int)p.IdProd);
                            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)p.IdProd);

                            mensagemLog = string.Empty;

                            // Altera o estoque somente se estiver marcado para alterar no cadastro de subgrupo, no cadastro de CFOP e 
                            // se o tipo de ambiente da NFe estiver em produção
                            if (Glass.Data.DAL.GrupoProdDAO.Instance.NaoAlterarEstoqueFiscal(idGrupoProd, idSubgrupoProd))
                                mensagemLog += "Grupo/Subgrupo do produto está configurado para não gerar estoque fiscal. ";
                            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
                                mensagemLog += "O tipo de ambiente da nota está configurado como Homologação, o que impede a geração estoque fiscal. ";
                            if ((nf.IdNaturezaOperacao != null && !NaturezaOperacaoDAO.Instance.AlterarEstoqueFiscal(nf.IdNaturezaOperacao.Value)))
                                mensagemLog += "A natureza de operação da nota está configurada para não gerar estoque fiscal. ";

                            if (!string.IsNullOrEmpty(mensagemLog))
                            {
                                var logMovNotaFiscal = new LogMovimentacaoNotaFiscal();
                                logMovNotaFiscal.IdNf = nf.IdNf;
                                logMovNotaFiscal.IdProdNf = p.IdProdNf;
                                logMovNotaFiscal.MensagemLog = mensagemLog;
                                logMovNotaFiscal.DataCad = DateTime.Now;
                                logMovNotaFiscal.Usucad = UserInfo.GetUserInfo.CodUser;
                                LogMovimentacaoNotaFiscalDAO.Instance.Insert(session, logMovNotaFiscal);
                            }

                            MovEstoqueFiscalDAO.Instance.CreditaEstoqueNotaFiscal(session, p.IdProd, nf.IdLoja.Value,
                                p.IdNaturezaOperacao > 0 ? p.IdNaturezaOperacao.Value : nf.IdNaturezaOperacao.Value, p.IdNf, p.IdProdNf,
                                (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(session, p, true), false, false);

                            // Altera o estoque real dos produtos
                            if (nf.GerarEstoqueReal && !EstoqueConfig.EntradaEstoqueManual)
                            {
                                if (!MovEstoqueDAO.Instance.AlteraEstoque(session, p.IdProd))
                                {
                                    var logMovNotaFiscal = new LogMovimentacaoNotaFiscal();
                                    logMovNotaFiscal.IdNf = nf.IdNf;
                                    logMovNotaFiscal.IdProdNf = p.IdProdNf;
                                    logMovNotaFiscal.MensagemLog = "Grupo/Subgrupo do produto está configurado para não gerar estoque real. ";
                                    logMovNotaFiscal.DataCad = DateTime.Now;
                                    logMovNotaFiscal.Usucad = UserInfo.GetUserInfo.CodUser;
                                    LogMovimentacaoNotaFiscalDAO.Instance.Insert(session, logMovNotaFiscal);
                                }

                                bool m2 = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                                    Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                                MovEstoqueDAO.Instance.CreditaEstoqueNotaFiscal(session, p.IdProd, nf.IdLoja.Value, nf.IdNf, p.IdProdNf,
                                    (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(session, p.IdProd, p.TotM, p.Qtde, p.Altura, p.Largura, false, false));

                                objPersistence.ExecuteCommand(session, "Update produtos_nf Set qtdeEntrada=" + ProdutosNfDAO.Instance.ObtemQtdDanfe(session, p).ToString().Replace(",", ".") +
                                    " Where idProdNf=" + p.IdProdNf);
                            }

                            //Altera o estoque da materia-prima
                            MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaNotaFiscal(session, (int)p.IdProd, (int)p.IdNf, (int)p.IdProdNf, (decimal)p.TotM, MovEstoque.TipoMovEnum.Entrada);
                        }

                        objPersistence.ExecuteCommand(session, "Update nota_fiscal Set entrouEstoque=true Where idNf=" + nf.IdNf);
                    }
                }
                // Se for saída e ainda não tiver dado saída no estoque, baixa estoque fiscal
                else if (nf.TipoDocumento == 2)
                {
                    if (nf.SaiuEstoque == false)
                    {
                        if (!nf.GerarEstoqueReal)
                        {
                            var logMovNotaFiscal = new LogMovimentacaoNotaFiscal();
                            logMovNotaFiscal.IdNf = nf.IdNf;
                            logMovNotaFiscal.MensagemLog = "Nota não está configurada para geração de estoque Real. ";
                            logMovNotaFiscal.DataCad = DateTime.Now;
                            logMovNotaFiscal.Usucad = UserInfo.GetUserInfo.CodUser;
                            LogMovimentacaoNotaFiscalDAO.Instance.Insert(session, logMovNotaFiscal);
                        }

                        foreach (ProdutosNf p in lstProd)
                        {
                            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd((int)p.IdProd);
                            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)p.IdProd);

                            var mensagemLog = string.Empty;

                            // Altera o estoque somente se estiver marcado para alterar no cadastro de subgrupo, no cadastro de CFOP e 
                            // se o tipo de ambiente da NFe estiver em produção
                            if (Glass.Data.DAL.GrupoProdDAO.Instance.NaoAlterarEstoqueFiscal(idGrupoProd, idSubgrupoProd))
                                mensagemLog += "Grupo/Subgrupo do produto está configurado para não gerar estoque fiscal. ";
                            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
                                mensagemLog += "O tipo de ambiente da nota está configurado como Homologação, o que impede a geração estoque fiscal. ";
                            if ((nf.IdNaturezaOperacao != null && !NaturezaOperacaoDAO.Instance.AlterarEstoqueFiscal(nf.IdNaturezaOperacao.Value)))
                                mensagemLog += "A natureza de operação da nota está configurada para não gerar estoque fiscal. ";

                            if (!string.IsNullOrEmpty(mensagemLog))
                            {
                                var logMovNotaFiscal = new LogMovimentacaoNotaFiscal();
                                logMovNotaFiscal.IdNf = nf.IdNf;
                                logMovNotaFiscal.IdProdNf = p.IdProdNf;
                                logMovNotaFiscal.MensagemLog = mensagemLog;
                                logMovNotaFiscal.DataCad = DateTime.Now;
                                logMovNotaFiscal.Usucad = UserInfo.GetUserInfo.CodUser;
                                LogMovimentacaoNotaFiscalDAO.Instance.Insert(session, logMovNotaFiscal);
                            }

                            MovEstoqueFiscalDAO.Instance.BaixaEstoqueNotaFiscal(session, p.IdProd, nf.IdLoja.Value,
                                p.IdNaturezaOperacao > 0 ? p.IdNaturezaOperacao.Value : nf.IdNaturezaOperacao.Value, p.IdNf, p.IdProdNf,
                                (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(session, p, true), false);

                            // Altera o estoque real dos produtos
                            if (nf.GerarEstoqueReal)
                            {
                                if (!MovEstoqueDAO.Instance.AlteraEstoque(session, p.IdProd))
                                {
                                    var logMovNotaFiscal = new LogMovimentacaoNotaFiscal();
                                    logMovNotaFiscal.IdNf = nf.IdNf;
                                    logMovNotaFiscal.IdProdNf = p.IdProdNf;
                                    logMovNotaFiscal.MensagemLog = "Grupo/Subgrupo do produto está configurado para não gerar estoque real. ";
                                    logMovNotaFiscal.DataCad = DateTime.Now;
                                    logMovNotaFiscal.Usucad = UserInfo.GetUserInfo.CodUser;
                                    LogMovimentacaoNotaFiscalDAO.Instance.Insert(session, logMovNotaFiscal);
                                }

                                bool m2 = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                                    Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                                MovEstoqueDAO.Instance.BaixaEstoqueNotaFiscal(session, p.IdProd, nf.IdLoja.Value, nf.IdNf, p.IdProdNf,
                                    (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(session, p.IdProd, p.TotM, p.Qtde, p.Altura, p.Largura, false, false));

                                objPersistence.ExecuteCommand(session, "Update produtos_nf Set qtdeSaida=" + ProdutosNfDAO.Instance.ObtemQtdDanfe(p).ToString().Replace(",", ".") +
                                    " Where idProdNf=" + p.IdProdNf);
                            }
                        }

                        objPersistence.ExecuteCommand(session, "Update nota_fiscal Set saiuEstoque=true Where idNf=" + nf.IdNf);
                    }
                }

                #endregion
            }
            // 101 - Cancelamento de NF-e homologado
            else if (cStat == "101" || cStat == "151" ||
                cStat == "135" || cStat == "136")
            {
                #region Estorna estoque fiscal/real

                // Se for entrada, estorna produtos baixando estoque fiscal
                if (nf.TipoDocumento == 1)
                {
                    if (nf.SaiuEstoque == false)
                    {
                        foreach (ProdutosNf p in lstProd)
                        {
                            MovEstoqueFiscalDAO.Instance.BaixaEstoqueNotaFiscal(session, p.IdProd, nf.IdLoja.Value,
                                p.IdNaturezaOperacao > 0 ? p.IdNaturezaOperacao.Value : nf.IdNaturezaOperacao.Value, p.IdNf, p.IdProdNf,
                                (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(session, p, true), false);

                            if (nf.GerarEstoqueReal)
                            {
                                MovEstoqueDAO.Instance.BaixaEstoqueNotaFiscal(session, p.IdProd, nf.IdLoja.Value, nf.IdNf, p.IdProdNf,
                                    (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(session, p.IdProd, p.TotM, p.Qtde, p.Altura, p.Largura, false, false));
                            }

                            //Altera o estoque da materia-prima
                            MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaNotaFiscal(session, (int)p.IdProd, (int)p.IdNf, (int)p.IdProdNf, (decimal)p.TotM, MovEstoque.TipoMovEnum.Saida);
                        }

                        objPersistence.ExecuteCommand(session, "Update nota_fiscal Set saiuEstoque=true Where idNf=" + nf.IdNf);
                    }
                }
                // Se for saída, estorna produtos creditando estoque fiscal
                else if (nf.TipoDocumento == 2)
                {
                    if (nf.EntrouEstoque == false)
                    {
                        foreach (ProdutosNf p in lstProd)
                        {
                            MovEstoqueFiscalDAO.Instance.CreditaEstoqueNotaFiscal(session, p.IdProd, nf.IdLoja.Value,
                                p.IdNaturezaOperacao > 0 ? p.IdNaturezaOperacao.Value : nf.IdNaturezaOperacao.Value, nf.IdNf, p.IdProdNf,
                                (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(session, p, true), false, true);

                            // Credita o estoque real
                            if (nf.GerarEstoqueReal)
                            {
                                MovEstoqueDAO.Instance.CreditaEstoqueNotaFiscal(session, p.IdProd, nf.IdLoja.Value, nf.IdNf, p.IdProdNf,
                                    (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(session, p.IdProd, p.TotM, p.Qtde, p.Altura, p.Largura, false, false));

                                objPersistence.ExecuteCommand(session, "Update produtos_nf Set qtdeSaida=0 Where idProdNf=" + p.IdProdNf);
                            }
                        }

                        objPersistence.ExecuteCommand(session, "Update nota_fiscal Set entrouEstoque=true Where idNf=" + nf.IdNf);
                    }
                }

                #endregion
            }
        }

        public void BaixaCreditaSobraProducao(bool baixar, uint idNf, uint idLoja, uint idProdNf, uint idProd, uint idNaturezaOperacao,
            ref decimal qtdMov)
        {
            BaixaCreditaSobraProducao(null, baixar, idNf, idLoja, idProdNf, idProd, idNaturezaOperacao, ref qtdMov);
        }

        /// Baixa/Credita sobras de produção
        /// </summary>
        /// <param name="baixar">True=Baixar, False=Creditar</param>
        /// <param name="idNf"></param>
        /// <param name="idLoja"></param>
        /// <param name="idProdNf"></param>
        /// <param name="idProd"></param>
        /// <param name="idNaturezaOperacao"></param>
        /// <param name="qtdMov">Quantidade original do produto do qual vai ser gerada a sobra, já retorna o valor que deve ser dado baixa</param>
        public void BaixaCreditaSobraProducao(GDASession sessao, bool baixar, uint idNf, uint idLoja, uint idProdNf, uint idProd, uint idNaturezaOperacao, ref decimal qtdMov)
        {
            // Se não estiver configurado um percentual de geração de sobra ou se o produto da nota já for o produto de sobra, não faz nada.
            if (FiscalConfig.NotaFiscalConfig.PercentualAGerarDeSobraDeProducao <= 0 ||
                ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, (int)idProd) == (uint)Utils.SubgrupoProduto.SobrasDeVidro ||
                ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)idProd) != (uint)Glass.Data.Model.NomeGrupoProd.Vidro)
                return;

            // Verifica se o item da nota altera estoque.
            if (!MovEstoqueFiscalDAO.Instance.AlteraEstoqueFiscal(idProd, idNaturezaOperacao))
                return;

            try
            {
                // Verifica se o produto possui cor e a espessura 
                float espessura = ProdutoDAO.Instance.ObtemEspessura(sessao, (int)idProd);
                var idCorVidro = ProdutoDAO.Instance.ObtemIdCorVidro((int)idProd);

                if (espessura == 0 || idCorVidro.GetValueOrDefault(0) == 0)
                {
                    LogNfDAO.Instance.NewLog(idNf, "Sobra de estoque", 1, "Falha ao " + (baixar ? "baixar" : "creditar") +
                        " sobra de estoque, a espessura ou a cor do vidro não estão preenchidas no cadastro de produtos.");

                    return;
                }

                // Verifica se existe este produto de sobra com a espessura e cor do produto da nota
                //uint idProdSobra = ProdutoDAO.Instance.ObtemIdProdSobra(espessura, (uint)idCorVidro.Value);
                // Chamado 26945: Mudança para creditar a sobra em apenas um produto (incolor de 8mm).
                uint idProdSobra = ProdutoDAO.Instance.ObtemIdProdSobra(8, 2);

                if (idProdSobra == 0)
                {
                    LogNfDAO.Instance.NewLog(idNf, "Sobra de estoque", 1, "Falha ao " + (baixar ? "baixar" : "creditar") +
                        " sobra de estoque, não existe produto de sobra com a cor e espessura deste produto.");

                    return;
                }

                decimal qtdSobra = qtdMov * (FiscalConfig.NotaFiscalConfig.PercentualAGerarDeSobraDeProducao / (decimal)100);

                if (baixar)
                    MovEstoqueFiscalDAO.Instance.BaixaEstoqueNotaFiscal(sessao, idProdSobra, idLoja, idNaturezaOperacao, idNf, idProdNf, qtdSobra, true);
                else
                    MovEstoqueFiscalDAO.Instance.CreditaEstoqueNotaFiscal(sessao, idProdSobra, idLoja, idNaturezaOperacao, idNf, idProdNf, qtdSobra, true, false);

                // A quantidade que será baixada/creditada do produto original deve ser acrescida da sobra.
                qtdMov += qtdSobra;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(idNf, "Sobra de estoque", 1, Glass.MensagemAlerta.FormatErrorMsg("Falha ao " + (baixar ? "baixar" : "creditar") +
                    " sobra de estoque.", ex));

                return;
            }
        }

        #endregion

        #region Retorno de inutilização de numeração da NF-e

        /// <summary>
        /// Retorno de inutilização de numeração da NF-e, grava log e altera situação da NF-e
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="justificativa"></param>
        /// <param name="xmlRetInut"></param>
        public void RetornoInutilizacaoNFe(uint idNf, string justificativa, XmlNode xmlRetInut)
        {
            // Se o xml de retorno for nulo, ocorreu alguma falha no processo
            if (xmlRetInut == null)
            {
                LogNfDAO.Instance.NewLog(idNf, "Inutilização", 1, "Falha ao inutilizar NFe. ");

                NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaInutilizar);

                throw new Exception("Servidor da SEFAZ não respondeu em tempo hábil, tente novamente.");
            }

            int cod = Glass.Conversoes.StrParaInt(xmlRetInut["infInut"]["cStat"].InnerXml);

            // Insere o log de cancelamento desta NF
            LogNfDAO.Instance.NewLog(idNf, "Inutilização", cod, xmlRetInut["infInut"]["xMotivo"].InnerXml);

            // Se o código de retorno for 102-Inutilização de número homologado, altera situação para inutilizada
            if (cod == 102)
            {
                // Insere protocolo de cancelamento na NFe
                objPersistence.ExecuteCommand("Update nota_fiscal set numProtocolo=?numProt Where idNf=" + idNf,
                    new GDAParameter[] { new GDAParameter("?numProt", xmlRetInut["infInut"]["nProt"].InnerXml) });

                AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.Inutilizada);
            }
            else if (cod == 206 || cod == 256) // NF-e já está inutilizada
                AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.Inutilizada);
            else if (cod == 218 || cod == 420) // NF-e já está cancelada
                AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.Cancelada);
            else if (cod == 220) // NF-e já está autorizada há mais de 7 dias
                AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.Autorizada);
            else if (cod == 110 || cod == 301 || cod == 302 || cod == 205)
                AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.Denegada);
            // Se o código de retorno for > 105, algum erro ocorreu, altera situação da NF para Falha ao Inutilizar
            else if (cod > 105)
                AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaInutilizar);
        }

        #endregion

        #region Busca de NF por situação

        private string SqlPorSituacao(uint idNf, uint numeroNFe, uint idPedido, string modelo, uint idLoja, uint idCliente, string nomeCliente,
            int tipoFiscal, uint idFornec, string nomeFornec, string codRota, string situacao, int tipoDoc, string dataIni, string dataFim,
            string idsCfop, string idsTiposCfop, string dataEntSaiIni, string dataEntSaiFim, uint formaPagto, string idsFormaPagtoNotaFiscal, int tipoNf,
            int finalidade, int formaEmissao, string infCompl, string codInternoProd, string descrProd, string valorInicial, string valorFinal,
            string cnpjFornecedor, int ordenar, bool acessoExterno, bool sintegra, string lote, bool selecionar)
        {
            string campos = @"n.*, cf.codInterno as codCfop, cf.descricao as DescrCfop, mun.NomeCidade as municOcor, 
                " + SqlCampoEmitente(TipoCampo.Nome, "l", "c", "f", "n", "tc") + @" as nomeEmitente, (mbai.idNf is not null) as temMovimentacaoBemAtivo, '$$$' as Criterio, 
                " + SqlCampoDestinatario(TipoCampo.Nome, "l", "c", "f", "n", "tc") + @" as nomeDestRem, concat(g.Descricao, concat(' - ', pc.Descricao)) as DescrPlanoContas, 
                t.Nome as nomeTransportador, " + SqlCampoDestinatario(TipoCampo.CpfCnpj, "l", "c", "f", "n", "tc") + @" as cpfCnpjDestRem,
                " + SqlCampoEmitente(TipoCampo.CpfCnpj, "l", "c", "f", "n", "tc") + @" as cnpjEmitente, (select tipoEntrega from pedido ped inner join 
                pedidos_nota_fiscal pnf on (ped.idPedido=pnf.idPedido) where pnf.idNf=n.idNf limit 1) as tipoEntrega, 
                (select cast(group_concat(ped.idPedido) as char) from pedido ped inner join 
                pedidos_nota_fiscal pnf on (ped.idPedido=pnf.idPedido) where pnf.idNf=n.idNf group by pnf.idNf) as idsPedido, 
                coalesce(no.codInterno, cf.codInterno) as CodNaturezaOperacao, func.nome as DescrUsuCad, (" + SqlIsImportacao("n.idNf") + ") as isNfImportacao";

            string sql = "";

            if (selecionar)
                sql = "Select " + campos + @" From nota_fiscal n 
                    Left Join natureza_operacao no On (n.idNaturezaOperacao=no.idNaturezaOperacao)
                    Left Join cfop cf On (no.idCfop=cf.idCfop) 
                    Left Join tipo_cfop tc On (cf.idTipoCfop=tc.idTipoCfop) 
                    Left Join cidade mun On (n.idCidade=mun.idCidade) 
                    Left Join loja l On (n.idLoja=l.idLoja) 
                    Left Join fornecedor f On (n.idFornec=f.idFornec) 
                    Left Join cliente c On (n.idCliente=c.id_Cli) 
                    Left Join transportador t On (n.idTransportador=t.idTransportador) 
                    Left Join funcionario func On (n.usuCad=func.idFunc)
                    Left Join plano_contas pc on (n.idConta=pc.idConta)
                    Left Join grupo_conta g On (pc.IdGrupo=g.IdGrupo) 
                    Left Join (
                        select pnf.idNf
                        from movimentacao_bem_ativo_imob mbai 
                            left join produtos_nf pnf on (mbai.idProdNf=pnf.idProdNf)
                        group by pnf.idNf
                    ) as mbai on (n.idNf=mbai.idNf)
                    Where 1";
            else
                sql = @"Select Count(*) From (Select Distinct n.idNf From nota_fiscal n 
                    Left Join natureza_operacao no On (n.idNaturezaOperacao=no.idNaturezaOperacao)
                    Left Join cfop cf On (no.idCfop=cf.idCfop) 
                    Left Join tipo_cfop tc On (cf.idTipoCfop=tc.idTipoCfop) 
                    Left Join cidade mun On (n.idCidade=mun.idCidade) 
                    Left Join loja l On (n.idLoja=l.idLoja) 
                    Left Join fornecedor f On (n.idFornec=f.idFornec) 
                    Left Join cliente c On (n.idCliente=c.id_Cli) 
                    Left Join transportador t On (n.idTransportador=t.idTransportador) 
                    Left Join funcionario func On (n.usuCad=func.idFunc)
                    Left Join plano_contas pc on (n.idConta=pc.idConta)
                    Left Join grupo_conta g On (pc.IdGrupo=g.IdGrupo) 
                    Where 1";

            NotaFiscal temp = new NotaFiscal();
            string criterio = "";

            if (idNf > 0)
            {
                sql += " And n.idNf=" + idNf;
                criterio += "Nota fiscal: " + idNf + "    ";
            }

            if (numeroNFe > 0)
            {
                sql += " And n.numeroNFe=" + numeroNFe;
                criterio += "Número NFe: " + numeroNFe + "    ";
            }

            if (idPedido > 0)
            {
                sql += " And n.idNf in (select idNf from pedidos_nota_fiscal where idPedido=" + idPedido + ")";
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (!string.IsNullOrEmpty(modelo))
            {
                sql += " AND n.Modelo=?modelo";
                criterio += string.Format("Modelo: {0}    ", modelo);
            }

            if (idLoja > 0)
            {
                sql += " And n.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetElement(idLoja).NomeFantasia + "    ";
            }

            if (idCliente > 0)
            {
                sql += " And n.idCliente=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (tipoFiscal > 0)
            {
                sql += " And c.tipoFiscal=" + tipoFiscal;

                switch (tipoFiscal)
                {
                    case (int)TipoFiscalCliente.ConsumidorFinal: criterio += "Tipo Fiscal: Consumidor Final    "; break;
                    case (int)TipoFiscalCliente.Revenda: criterio += "Tipo Fiscal: Revenda    "; break;
                }
            }

            if (idFornec > 0)
            {
                sql += " And n.idFornec=" + idFornec;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(idFornec) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                sql += " And (f.RazaoSocial LIKE ?nomeFornec or f.NomeFantasia LIKE ?nomeFornec)";
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }

            if (!string.IsNullOrEmpty(cnpjFornecedor))
            {
                sql += " AND f.cpfCnpj = ?cpfCnpj";
                criterio += "CPF/CNPJ Fornecedor: " + Formatacoes.FormataCpfCnpj(cnpjFornecedor) + "    ";
            }

            if (!String.IsNullOrEmpty(codRota))
            {
                sql += " And n.idCliente In (Select idCliente From rota_cliente Where idRota In " +
                    "(Select idRota From rota where codInterno like ?codRota))";
                criterio += "Rota: " + codRota + "    ";
            }

            if (!String.IsNullOrEmpty(situacao) && tipoNf != (int)NotaFiscal.TipoDoc.EntradaTerceiros)
            {
                sql += " And n.situacao in(" + situacao + ")";
                criterio += "Situação: ";

                string[] s = situacao.Split(',');

                foreach (string item in s)
                {
                    criterio += Enum.Parse(typeof(NotaFiscal.SituacaoEnum), item) + ", ";
                }

                criterio.Remove(criterio.LastIndexOf(','));
            }

            if (tipoDoc > 0)
            {
                sql += " And n.tipoDocumento=" + tipoDoc;
                temp.TipoDocumento = tipoDoc;
                criterio += "Tipo documento: " + temp.TipoDocumentoString + "    ";
            }

            if (!sintegra)
            {
                if (idPedido == 0 && numeroNFe == 0)
                {
                    if (!String.IsNullOrEmpty(dataIni))
                    {
                        sql += " And n.DataEmissao>=?dataIni";
                        criterio += "Período Emissão: " + dataIni + "    ";
                    }

                    if (!String.IsNullOrEmpty(dataFim))
                    {
                        sql += " And n.DataEmissao<=?dataFim";

                        if (!String.IsNullOrEmpty(dataIni))
                            criterio += " até " + dataFim + "    ";
                        else
                            criterio += "Período Emissão: até " + dataFim + "    ";
                    }
                }
            }
            else
            {
                sql += " And coalesce(if(n.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.Saída + @", 
                    n.dataSaidaEnt, null), n.dataEmissao) Between ?dataIni And ?dataFim";
            }

            if (!String.IsNullOrEmpty(dataEntSaiIni))
            {
                sql += " And n.DataSaidaEnt>=?dataEntSaiIni";
                criterio += "Período Entrada/Saída: " + dataEntSaiIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataEntSaiFim))
            {
                sql += " And n.DataSaidaEnt<=?dataEntSaiFim";

                if (!String.IsNullOrEmpty(dataEntSaiIni))
                    criterio += " até " + dataEntSaiFim + "    ";
                else
                    criterio += "Período Entrada/Saída: até " + dataEntSaiFim + "    ";
            }

            if (formaPagto > 0)
            {
                sql += " and n.formaPagto=" + formaPagto;
                criterio += "Forma Pagto.: " + (formaPagto == 1 ? "À Vista" : formaPagto == 2 ? "À Prazo " : "Outros") + "    ";
            }

            if (!string.IsNullOrWhiteSpace(idsFormaPagtoNotaFiscal))
            {
                // Recupera os IdNf que tenha a forma de pagamento filtrada
                var idsNF = string.Join(",", ExecuteMultipleScalar<string>(
                    string.Format("SELECT IdNf FROM pagto_nota_fiscal WHERE FormaPagto IN ({0})", idsFormaPagtoNotaFiscal)));

                sql += string.Format(" AND n.IdNf IN ({0})", idsNF);
            }

            switch (tipoNf)
            {
                case 1:
                    sql += " and n.tipoDocumento=1";
                    criterio += "Tipo NF: Entrada    ";
                    break;
                case 2:
                    sql += " and n.tipoDocumento=2";
                    criterio += "Tipo NF: Saída    ";
                    break;
                case 3:
                    sql += " and n.tipoDocumento=3 and n.situacao=" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros;
                    criterio += "Tipo NF: Entrada (terceiros)    ";
                    break;
                case 4:
                    sql += " and n.tipoDocumento=4";
                    criterio += "Tipo NF: Nota Fiscal de Cliente    ";
                    break;
                case 5:
                    sql += " and n.tipoDocumento=3 and n.transporte=1";
                    criterio += "Tipo NF: Transporte    ";
                    break;
                case 6:
                    sql += " and n.tipoDocumento in (3,4)";
                    criterio += "Tipo NF: Entrada (terceiros), Nota Fiscal de Cliente    ";
                    break;
            }

            switch (finalidade)
            {
                case 1:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Normal;
                    criterio += "Finalidade: NF-e Normal    ";
                    break;
                case 2:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar;
                    criterio += "Finalidade: NF-e Complementar    ";
                    break;
                case 3:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste;
                    criterio += "Finalidade: NF-e de Ajuste    ";
                    break;
                case 4:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Devolucao;
                    criterio += "Finalidade: NF-e de Devolução/Retorno    ";
                    break;
            }

            switch (formaEmissao)
            {
                case 1:
                    sql += " and n.formaEmissao=" + (int)NotaFiscal.TipoEmissao.Normal;
                    criterio += "Forma de emissão: Normal    ";
                    break;
                case 3:
                    sql += " and n.formaEmissao=" + (int)NotaFiscal.TipoEmissao.ContingenciaComSCAN;
                    criterio += "Forma de emissão: Contingência    ";
                    break;
            }

            if (!String.IsNullOrEmpty(idsCfop))
            {
                sql += " and (no.idCfop IN(" + idsCfop + @") or EXISTS (select * from produtos_nf pnf 
                    inner join natureza_operacao no1 on (pnf.idNaturezaOperacao=no1.idNaturezaOperacao) where idNf=n.idNf AND no1.idCfop IN (" + idsCfop + ")))";

                criterio += "CFOP: " + CfopDAO.Instance.GetDescricoes(idsCfop) + "    ";
            }

            if (!String.IsNullOrEmpty(idsTiposCfop))
            {
                sql += string.Format(@" 
                    And (cf.idTipoCfop IN (" + idsTiposCfop + @") or EXISTS
                    (
                        select * from produtos_nf p 
                            inner join natureza_operacao no1 on (p.idNaturezaOperacao=no1.idNaturezaOperacao)
                            inner join cfop c On (no1.idCfop=c.idCfop) 
                        Where idNf=n.idNf And c.idTipoCfop IN (" + idsTiposCfop + @") {0}
                    ))", !String.IsNullOrEmpty(idsCfop) ? "AND c.idCfop IN (" + idsCfop + @")" : "");

                criterio += "Tipo(s) CFOP: " + TipoCfopDAO.Instance.GetDescrByString(idsTiposCfop) + "    ";
            }

            if (!String.IsNullOrEmpty(infCompl))
            {
                sql += " And n.infCompl like ?infCompl";
                criterio += "Informação complementar: " + infCompl + "    ";
            }

            // Se for acesso externo, permite visualização apenas de notas autorizadas e canceladas
            if (acessoExterno)
                sql += " And n.situacao in (" + (int)NotaFiscal.SituacaoEnum.Autorizada + "," + (int)NotaFiscal.SituacaoEnum.Cancelada + ")";

            if (!String.IsNullOrEmpty(codInternoProd))
            {
                string idsProd = ProdutoDAO.Instance.ObtemIds(codInternoProd, null);
                sql += @" 
                    And n.idNf In (
                        Select p.idNf from produtos_nf p 
                        Where p.idProd In (" + idsProd + @")
                    )";

                criterio += "Produto: " + ProdutoDAO.Instance.GetDescrProduto(codInternoProd) + "    ";
            }
            else if (!String.IsNullOrEmpty(descrProd))
            {
                string idsProd = ProdutoDAO.Instance.ObtemIds(null, descrProd);
                sql += @" 
                    And n.idNf In (
                        Select p.idNf from produtos_nf p 
                        Where p.idProd In (" + idsProd + @")
                    )";

                criterio += "Produto: " + descrProd + "    ";
            }

            if (!string.IsNullOrEmpty(lote))
            {
                sql += @" 
                    And n.idNf In (
                        Select p.idNf from produtos_nf p 
                        Where p.Lote Like ?lote
                    )";

                criterio += "Lote: " + lote + "    ";
            }

            if (!String.IsNullOrEmpty(valorInicial))
            {
                sql += " And n.TotalNota >= " + valorInicial.Replace(",", ".");
            }

            if (!String.IsNullOrEmpty(valorFinal))
            {
                sql += " And n.TotalNota <= " + valorFinal.Replace(",", ".");
            }

            switch (ordenar)
            {
                case 1:
                    sql += " order by n.NumeroNFE desc";
                    criterio += "Ordenar por: Número NF-e (descresc.)    ";
                    break;
                case 2:
                    sql += " order by n.NumeroNFE asc";
                    criterio += "Ordenar por: Número NF-e (cresc.)    ";
                    break;
                case 3:
                    sql += " order by n.DataEmissao desc";
                    criterio += "Ordenar por: Data de emissão (descresc.)    ";
                    break;
                case 4:
                    sql += " order by n.DataEmissao asc";
                    criterio += "Ordenar por: Data de emissão (cresc.)    ";
                    break;
                case 5:
                    sql += " order by n.DataSaidaEnt desc";
                    criterio += "Ordenar por: Data de entrada/saída (descresc.)    ";
                    break;
                case 6:
                    sql += " order by n.DataSaidaEnt asc";
                    criterio += "Ordenar por: Data de entrada/saída (cresc.)    ";
                    break;
                case 7:
                    sql += " order by n.TotalNota asc";
                    criterio += "Ordenar por: Valor Total (cresc.)    ";
                    break;
                case 8:
                    sql += " order by n.TotalNota desc";
                    criterio += "Ordenar por: Valor Total (descresc.)    ";
                    break;
            }

            if (!selecionar)
                sql += ") as Contador";

            return sql.Replace("$$$", criterio);
        }


        public IList<NotaFiscal> GetListPorSituacao(uint numeroNFe, uint idPedido, string modelo, uint idLoja, uint idCliente, string nomeCliente, int tipoFiscal, uint idFornec,
            string nomeFornec, string codRota, int tipoDoc, string situacao, string dataIni, string dataFim, string idsCfop, string idsTiposCfop,
            string dataEntSaiIni, string dataEntSaiFim, uint formaPagto, string idsFormaPagtoNotaFiscal, int tipoNf, int finalidade, int formaEmissao, string infCompl,
            string codInternoProd, string descrProd, string valorInicial, string valorFinal, string cnpjFornecedor, string lote,
            int ordenar, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "n.idNf desc" : sortExpression;

            return LoadDataWithSortExpression(SqlPorSituacao(0, numeroNFe, idPedido, modelo, idLoja, idCliente, nomeCliente, tipoFiscal, idFornec, nomeFornec, codRota,
                situacao, tipoDoc, dataIni, dataFim, idsCfop, idsTiposCfop, dataEntSaiIni, dataEntSaiFim, formaPagto, idsFormaPagtoNotaFiscal, tipoNf, finalidade, formaEmissao,
                infCompl, codInternoProd, descrProd, valorInicial, valorFinal, cnpjFornecedor, ordenar, false, false, lote, true), sortExpression, startRow, pageSize,
                GetParams(modelo, codRota, nomeCliente, nomeFornec, dataIni, dataFim, dataEntSaiIni, dataEntSaiFim, infCompl, codInternoProd, descrProd,
                valorInicial, valorFinal, cnpjFornecedor, lote));
        }

        public int GetCountPorSituacao(uint numeroNFe, uint idPedido, string modelo, uint idLoja, uint idCliente, string nomeCliente, int tipoFiscal, uint idFornec,
            string nomeFornec, string codRota, int tipoDoc, string situacao, string dataIni, string dataFim, string idsCfop, string idsTiposCfop,
            string dataEntSaiIni, string dataEntSaiFim, uint formaPagto, string idsFormaPagtoNotaFiscal, int tipoNf, int finalidade, int formaEmissao, string infCompl,
            string codInternoProd, string descrProd, string valorInicial, string valorFinal, string cnpjFornecedor, string lote, int ordenar)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlPorSituacao(0, numeroNFe, idPedido, modelo, idLoja, idCliente, nomeCliente, tipoFiscal, idFornec, nomeFornec,
                codRota, situacao, tipoDoc, dataIni, dataFim, idsCfop, idsTiposCfop, dataEntSaiIni, dataEntSaiFim, formaPagto, idsFormaPagtoNotaFiscal, tipoNf, finalidade,
                formaEmissao, infCompl, codInternoProd, descrProd, valorInicial, valorFinal, cnpjFornecedor, ordenar, false, false, lote, false),
                GetParams(modelo, codRota, nomeCliente, nomeFornec, dataIni, dataFim, dataEntSaiIni, dataEntSaiFim, infCompl, codInternoProd,
                descrProd, valorInicial, valorFinal, cnpjFornecedor, lote));
        }

        public List<uint> GetListPorSituacaoAjax(uint numeroNFe, uint idPedido, string modelo, uint idLoja, uint idCliente, string nomeCliente, int tipoFiscal, uint idFornec,
            string nomeFornec, string codRota, int tipoDoc, string situacao, string dataIni, string dataFim, string idsCfop, string idsTiposCfop,
            string dataEntSaiIni, string dataEntSaiFim, uint formaPagto, string idsFormaPagtoNotaFiscal, int tipoNf, int finalidade, int formaEmissao, string infCompl,
            string codInternoProd, string descrProd, string valorInicial, string valorFinal)
        {
            return objPersistence.LoadResult(SqlPorSituacao(0, numeroNFe, idPedido, modelo, idLoja, idCliente, nomeCliente, tipoFiscal, idFornec, nomeFornec, codRota,
                situacao, tipoDoc, dataIni, dataFim, idsCfop, idsTiposCfop, dataEntSaiIni, dataEntSaiFim, formaPagto, idsFormaPagtoNotaFiscal, tipoNf, finalidade, formaEmissao,
                infCompl, codInternoProd, descrProd, valorInicial, valorFinal, null, 0, false, false, null, true),
                GetParams(modelo, codRota, nomeCliente, nomeFornec, dataIni, dataFim, dataEntSaiIni, dataEntSaiFim, infCompl, codInternoProd,
                descrProd, valorInicial, valorFinal, null, null))
                .Select(f => f.GetUInt32(0))
                       .ToList();
        }

        #endregion

        #region Busca notas para tela padrão NFE > Notas Fiscais

        private string SqlListaPadrao(uint idNf, uint numeroNFe, uint idPedido, string modelo, uint idLoja, uint idCliente, string nomeCliente,
            int tipoFiscal, uint idFornec, string nomeFornec, string codRota, string situacao, int tipoDoc, string dataIni, string dataFim,
            string idsCfop, string idsTiposCfop, string dataEntSaiIni, string dataEntSaiFim, uint formaPagto, string idsFormaPagtoNotaFiscal, int tipoNf,
            int finalidade, int formaEmissao, string infCompl, string codInternoProd, string descrProd, string valorInicial, string valorFinal,
            string cnpjFornecedor, int ordenar, bool acessoExterno, bool sintegra, bool selecionar)
        {
            string campos = @"n.*, cf.codInterno as codCfop, cf.descricao as DescrCfop, mun.NomeCidade as municOcor, 
                " + SqlCampoEmitente(TipoCampo.Nome, "l", "c", "f", "n", "tc") + @" as nomeEmitente, '$$$' as Criterio, 
                " + SqlCampoDestinatario(TipoCampo.Nome, "l", "c", "f", "n", "tc", "transp") + @" as nomeDestRem, 
                " + SqlCampoDestinatario(TipoCampo.CpfCnpj, "l", "c", "f", "n", "tc", "transp") + @" as cpfCnpjDestRem,
                " + SqlCampoEmitente(TipoCampo.CpfCnpj, "l", "c", "f", "n", "tc") + @" as cnpjEmitente, 
                (select cast(group_concat(ped.idPedido) as char) from pedido ped inner join 
                pedidos_nota_fiscal pnf on (ped.idPedido=pnf.idPedido) where pnf.idNf=n.idNf group by pnf.idNf) as idsPedido, 
                coalesce(no.codInterno, cf.codInterno) as CodNaturezaOperacao, func.nome as DescrUsuCad, (" + SqlIsImportacao("n.idNf") + ") as isNfImportacao";

            string sql = "";

            if (selecionar)
                sql = "Select " + campos + @" From nota_fiscal n 
                    Left Join natureza_operacao no On (n.idNaturezaOperacao=no.idNaturezaOperacao)
                    Left Join cfop cf On (no.idCfop=cf.idCfop) 
                    Left Join tipo_cfop tc On (cf.idTipoCfop=tc.idTipoCfop) 
                    Left Join cidade mun On (n.idCidade=mun.idCidade) 
                    Left Join loja l On (n.idLoja=l.idLoja) 
                    Left Join fornecedor f On (n.idFornec=f.idFornec)
                    LEFT JOIN transportador transp ON (n.IdTransportador=transp.IdTransportador)
                    Left Join cliente c On (n.idCliente=c.id_Cli) 
                    Left Join funcionario func On (n.usuCad=func.idFunc)
                    Where 1";
            else
                sql = @"Select Count(*) From (Select Distinct n.idNf From nota_fiscal n 
                    Left Join natureza_operacao no On (n.idNaturezaOperacao=no.idNaturezaOperacao)
                    Left Join cfop cf On (no.idCfop=cf.idCfop) 
                    Left Join tipo_cfop tc On (cf.idTipoCfop=tc.idTipoCfop) 
                    Left Join cidade mun On (n.idCidade=mun.idCidade) 
                    Left Join loja l On (n.idLoja=l.idLoja) 
                    Left Join fornecedor f On (n.idFornec=f.idFornec) 
                    Left Join cliente c On (n.idCliente=c.id_Cli) 
                    Left Join funcionario func On (n.usuCad=func.idFunc)
                    Where 1";

            NotaFiscal temp = new NotaFiscal();
            string criterio = "";

            if (idNf > 0)
            {
                sql += " And n.idNf=" + idNf;
                criterio += "Nota fiscal: " + idNf + "    ";
            }

            if (numeroNFe > 0)
            {
                sql += " And n.numeroNFe=" + numeroNFe;
                criterio += "N??o NFe: " + numeroNFe + "    ";
            }

            if (idPedido > 0)
            {
                sql += " And n.idNf in (select idNf from pedidos_nota_fiscal where idPedido=" + idPedido + ")";
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (!string.IsNullOrEmpty(modelo))
            {
                sql += " AND n.Modelo=?modelo";
                criterio += string.Format("Modelo: {0}    ", modelo);
            }

            if (idLoja > 0)
            {
                sql += " And n.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetElement(idLoja).NomeFantasia + "    ";
            }

            if (idCliente > 0)
            {
                sql += " And n.idCliente=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (tipoFiscal > 0)
            {
                sql += " And c.tipoFiscal=" + tipoFiscal;

                switch (tipoFiscal)
                {
                    case (int)TipoFiscalCliente.ConsumidorFinal: criterio += "Tipo Fiscal: Consumidor Final    "; break;
                    case (int)TipoFiscalCliente.Revenda: criterio += "Tipo Fiscal: Revenda    "; break;
                }
            }

            if (idFornec > 0)
            {
                sql += " And n.idFornec=" + idFornec;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(idFornec) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                sql += " And (f.RazaoSocial LIKE ?nomeFornec or f.NomeFantasia LIKE ?nomeFornec)";
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }

            if (!string.IsNullOrEmpty(cnpjFornecedor))
            {
                sql += " AND f.cpfCnpj = ?cpfCnpj";
                criterio += "CPF/CNPJ Fornecedor: " + Formatacoes.FormataCpfCnpj(cnpjFornecedor) + "    ";
            }

            if (!String.IsNullOrEmpty(codRota))
            {
                sql += " And n.idCliente In (Select idCliente From rota_cliente Where idRota In " +
                    "(Select idRota From rota where codInterno like ?codRota))";
                criterio += "Rota: " + codRota + "    ";
            }

            if (!String.IsNullOrEmpty(situacao) && tipoNf != (int)NotaFiscal.TipoDoc.EntradaTerceiros)
            {
                sql += " And n.situacao in(" + situacao + ")";
                criterio += "Situação: ";

                string[] s = situacao.Split(',');

                foreach (string item in s)
                {
                    criterio += Enum.Parse(typeof(NotaFiscal.SituacaoEnum), item) + ", ";
                }

                criterio.Remove(criterio.LastIndexOf(','));
            }
            if (tipoDoc > 0)
            {
                sql += " And n.tipoDocumento=" + tipoDoc;
                temp.TipoDocumento = tipoDoc;
                criterio += "Tipo documento: " + temp.TipoDocumentoString + "    ";
            }

            if (idPedido == 0 && numeroNFe == 0)
            {
                if (!sintegra)
                {
                    if (!String.IsNullOrEmpty(dataIni))
                    {
                        sql += " And n.DataEmissao>=?dataIni";
                        criterio += "Período Emissão " + dataIni + "    ";
                    }

                    if (!String.IsNullOrEmpty(dataFim))
                    {
                        sql += " And n.DataEmissao<=?dataFim";

                        if (!String.IsNullOrEmpty(dataIni))
                            criterio += " até " + dataFim + "    ";
                        else
                            criterio += "Período Emissão até " + dataFim + "    ";
                    }
                }
                else
                {
                    sql += " And coalesce(if(n.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.Saída + @", 
                    n.dataSaidaEnt, null), n.dataEmissao) Between ?dataIni And ?dataFim";
                }

                if (!String.IsNullOrEmpty(dataEntSaiIni))
                {
                    sql += " And n.DataSaidaEnt>=?dataEntSaiIni";
                    criterio += "Período Entrada/Saída: " + dataEntSaiIni + "    ";
                }

                if (!String.IsNullOrEmpty(dataEntSaiFim))
                {
                    sql += " And n.DataSaidaEnt<=?dataEntSaiFim";

                    if (!String.IsNullOrEmpty(dataEntSaiIni))
                        criterio += " até " + dataEntSaiFim + "    ";
                    else
                        criterio += "Período Entrada/Saída: até " + dataEntSaiFim + "    ";
                }
            }

            if (formaPagto > 0)
            {
                sql += " and n.formaPagto=" + formaPagto;
                criterio += "Forma Pagto.: " + (formaPagto == 1 ? " Vista" : formaPagto == 2 ? " Prazo " : "Outros") + "    ";
            }

            if (!string.IsNullOrWhiteSpace(idsFormaPagtoNotaFiscal))
            {
                // Recupera os IdNf que tenha a forma de pagamento filtrada
                var idsNF = string.Join(",", ExecuteMultipleScalar<string>(
                    string.Format("SELECT IdNf FROM pagto_nota_fiscal WHERE FormaPagto IN ({0})", idsFormaPagtoNotaFiscal)));

                // Caso não for encontrado IdNf com a forma de pagamento, não exibe nenhuma nota
                sql += string.Format(" AND n.IdNf IN ({0})", string.IsNullOrWhiteSpace(idsNF) ? "0" : idsNF);
            }

            switch (tipoNf)
            {
                case 1:
                    sql += " and n.tipoDocumento=1";
                    criterio += "Tipo NF: Entrada    ";
                    break;
                case 2:
                    sql += " and n.tipoDocumento=2";
                    criterio += "Tipo NF: Saída    ";
                    break;
                case 3:
                    sql += " and n.tipoDocumento=3 and n.situacao=" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros;
                    criterio += "Tipo NF: Entrada (terceiros)    ";
                    break;
                case 4:
                    sql += " and n.tipoDocumento=4";
                    criterio += "Tipo NF: Nota Fiscal de Cliente    ";
                    break;
                case 5:
                    sql += " and n.tipoDocumento=3 and n.transporte=1";
                    criterio += "Tipo NF: Transporte    ";
                    break;
                case 6:
                    sql += " and n.tipoDocumento in (3,4)";
                    criterio += "Tipo NF: Entrada (terceiros), Nota Fiscal de Cliente    ";
                    break;
            }

            switch (finalidade)
            {
                case 1:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Normal;
                    criterio += "Finalidade: NF-e Normal    ";
                    break;
                case 2:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar;
                    criterio += "Finalidade: NF-e Complementar    ";
                    break;
                case 3:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste;
                    criterio += "Finalidade: NF-e de Ajuste    ";
                    break;
                case 4:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Devolucao;
                    criterio += "Finalidade: NF-e de Devolução/Retorno    ";
                    break;
            }

            switch (formaEmissao)
            {
                case 1:
                    sql += " and n.formaEmissao=" + (int)NotaFiscal.TipoEmissao.Normal;
                    criterio += "Forma de emissão Normal    ";
                    break;
                case 3:
                    sql += " and n.formaEmissao=" + (int)NotaFiscal.TipoEmissao.ContingenciaComSCAN;
                    criterio += "Forma de emissão Contingência    ";
                    break;
            }

            if (!String.IsNullOrEmpty(idsCfop))
            {
                sql += " and (no.idCfop IN(" + idsCfop + @") or EXISTS (select * from produtos_nf pnf 
                    inner join natureza_operacao no1 on (pnf.idNaturezaOperacao=no1.idNaturezaOperacao) where idNf=n.idNf AND no1.idCfop IN (" + idsCfop + ")))";

                criterio += "CFOP: " + CfopDAO.Instance.GetDescricoes(idsCfop) + "    ";
            }

            if (!String.IsNullOrEmpty(idsTiposCfop))
            {
                sql += string.Format(@" 
                    And (cf.idTipoCfop IN (" + idsTiposCfop + @") or EXISTS
                    (
                        select * from produtos_nf p 
                            inner join natureza_operacao no1 on (p.idNaturezaOperacao=no1.idNaturezaOperacao)
                            inner join cfop c On (no1.idCfop=c.idCfop) 
                        Where idNf=n.idNf And c.idTipoCfop IN (" + idsTiposCfop + @") {0}
                    ))", !String.IsNullOrEmpty(idsCfop) ? "AND c.idCfop IN (" + idsCfop + @")" : "");

                criterio += "Tipo(s) CFOP: " + TipoCfopDAO.Instance.GetDescrByString(idsTiposCfop) + "    ";
            }

            if (!String.IsNullOrEmpty(infCompl))
            {
                sql += " And n.infCompl like ?infCompl";
                criterio += "Informação complementar: " + infCompl + "    ";
            }

            // Se for acesso externo, permite visualização apenas de notas autorizadas e canceladas
            if (acessoExterno)
                sql += " And n.situacao in (" + (int)NotaFiscal.SituacaoEnum.Autorizada + "," + (int)NotaFiscal.SituacaoEnum.Cancelada + ")";

            if (!String.IsNullOrEmpty(codInternoProd))
            {
                string idsProd = ProdutoDAO.Instance.ObtemIds(codInternoProd, null);
                sql += @" 
                    And n.idNf In (
                        Select p.idNf from produtos_nf p 
                        Where p.idProd In (" + idsProd + @")
                    )";

                criterio += "Produto: " + ProdutoDAO.Instance.GetDescrProduto(codInternoProd) + "    ";
            }
            else if (!String.IsNullOrEmpty(descrProd))
            {
                string idsProd = ProdutoDAO.Instance.ObtemIds(null, descrProd);
                sql += @" 
                    And n.idNf In (
                        Select p.idNf from produtos_nf p 
                        Where p.idProd In (" + idsProd + @")
                    )";

                criterio += "Produto: " + descrProd + "    ";
            }

            if (!String.IsNullOrEmpty(valorInicial))
            {
                sql += " And n.TotalNota >= " + valorInicial.Replace(",", ".");
            }

            if (!String.IsNullOrEmpty(valorFinal))
            {
                sql += " And n.TotalNota <= " + valorFinal.Replace(",", ".");
            }

            switch (ordenar)
            {
                case 1:
                    sql += " order by n.NumeroNFE desc";
                    break;
                case 3:
                    sql += " order by n.DataEmissao desc";
                    criterio += "Ordenar por: Data de emissão (descresc.)    ";
                    break;
                case 4:
                    sql += " order by n.DataEmissao asc";
                    criterio += "Ordenar por: Data de emissão (cresc.)    ";
                    break;
                case 5:
                    sql += " order by n.DataSaidaEnt desc";
                    criterio += "Ordenar por: Data de entrada/saída (descresc.)    ";
                    break;
                case 6:
                    sql += " order by n.DataSaidaEnt asc";
                    criterio += "Ordenar por: Data de entrada/saída (cresc.)    ";
                    break;
            }

            if (!selecionar)
                sql += ") as Contador";

            return sql.Replace("$$$", criterio);
        }

        public IList<NotaFiscal> GetListaPadrao(uint numeroNFe, uint idPedido, string modelo, uint idLoja, uint idCliente, string nomeCliente, int tipoFiscal, uint idFornec,
            string nomeFornec, string codRota, int tipoDoc, string situacao, string dataIni, string dataFim, string idsCfop, string idsTiposCfop,
            string dataEntSaiIni, string dataEntSaiFim, uint formaPagto, string idsFormaPagtoNotaFiscal, int tipoNf, int finalidade, int formaEmissao, string infCompl,
            string codInternoProd, string descrProd, string valorInicial, string valorFinal, string cnpjFornecedor,
            int ordenar, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "n.idNf desc" : sortExpression;

            return LoadDataWithSortExpression(SqlListaPadrao(0, numeroNFe, idPedido, modelo, idLoja, idCliente, nomeCliente, tipoFiscal, idFornec, nomeFornec, codRota,
                situacao, tipoDoc, dataIni, dataFim, idsCfop, idsTiposCfop, dataEntSaiIni, dataEntSaiFim, formaPagto, idsFormaPagtoNotaFiscal, tipoNf, finalidade, formaEmissao,
                infCompl, codInternoProd, descrProd, valorInicial, valorFinal, cnpjFornecedor, ordenar, false, false, true), sortExpression, startRow, pageSize,
                GetParams(modelo, codRota, nomeCliente, nomeFornec, dataIni, dataFim, dataEntSaiIni, dataEntSaiFim, infCompl, codInternoProd, descrProd,
                valorInicial, valorFinal, cnpjFornecedor, null));
        }

        public int GetCountListaPadrao(uint numeroNFe, uint idPedido, string modelo, uint idLoja, uint idCliente, string nomeCliente, int tipoFiscal, uint idFornec,
            string nomeFornec, string codRota, int tipoDoc, string situacao, string dataIni, string dataFim, string idsCfop, string idsTiposCfop,
            string dataEntSaiIni, string dataEntSaiFim, uint formaPagto, string idsFormaPagtoNotaFiscal, int tipoNf, int finalidade, int formaEmissao, string infCompl,
            string codInternoProd, string descrProd, string valorInicial, string valorFinal, string cnpjFornecedor, int ordenar)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlListaPadrao(0, numeroNFe, idPedido, modelo, idLoja, idCliente, nomeCliente, tipoFiscal, idFornec, nomeFornec,
                codRota, situacao, tipoDoc, dataIni, dataFim, idsCfop, idsTiposCfop, dataEntSaiIni, dataEntSaiFim, formaPagto, idsFormaPagtoNotaFiscal, tipoNf, finalidade,
                formaEmissao, infCompl, codInternoProd, descrProd, valorInicial, valorFinal, cnpjFornecedor, ordenar, false, false, false),
                GetParams(modelo, codRota, nomeCliente, nomeFornec, dataIni, dataFim, dataEntSaiIni, dataEntSaiFim, infCompl, codInternoProd,
                descrProd, valorInicial, valorFinal, cnpjFornecedor, null));
        }

        #endregion

        #region Busca padrão de NF

        private enum TipoCampo
        {
            Nome,
            CpfCnpj
        }

        private string SqlCampoEmitente(TipoCampo tipoCampo, string aliasLoja, string aliasCliente, string aliasFornecedor, string aliasNotaFiscal, string aliasTipoCfop)
        {
            string baseEmitente = String.Format("if({0}.tipodocumento not in (3,4), {2}, if(!coalesce({1}.devolucao,false) || {0}.tipoDocumento=4, {3}, {4}))",
                aliasNotaFiscal, aliasTipoCfop, "{0}", "{1}", "{2}");

            string campoNomeCliente = FiscalConfig.NotaFiscalConfig.UsarNomeFantasiaNotaFiscal ? "nomeFantasia" : "nome";

            string campoLoja = String.Format("{0}.{1}", aliasLoja, tipoCampo == TipoCampo.Nome ? "razaoSocial" : "cnpj");
            string campoCliente = String.Format("{0}.{1}", aliasCliente, tipoCampo == TipoCampo.Nome ? campoNomeCliente : "cpf_cnpj");
            string campoFornec = String.Format("{0}.{1}", aliasFornecedor, tipoCampo == TipoCampo.Nome ? "razaoSocial" : "cpfCnpj");

            return String.Format(baseEmitente, campoLoja, campoFornec, campoCliente);
        }

        private string SqlCampoDestinatario(TipoCampo tipoCampo, string aliasLoja, string aliasCliente,
            string aliasFornecedor, string aliasNotaFiscal, string aliasTipoCfop)
        {
            return SqlCampoDestinatario(tipoCampo, aliasLoja, aliasCliente, aliasFornecedor, aliasNotaFiscal, aliasTipoCfop, null);
        }

        private string SqlCampoDestinatario(TipoCampo tipoCampo, string aliasLoja, string aliasCliente, string aliasFornecedor,
            string aliasNotaFiscal, string aliasTipoCfop, string aliasTransportador)
        {
            var baseEmitente = string.Format(@"if(({0}.tipoDocumento=1 and !coalesce({1}.devolucao,false)) or ({0}.tipoDocumento=2 and {1}.devolucao), Coalesce({3},{2}), 
                if(({0}.tipoDocumento=1 and {1}.devolucao) or ({0}.tipodocumento in (2,4) and !coalesce({1}.devolucao, false)), Coalesce({3},{2}), {4}))",
                aliasNotaFiscal, aliasTipoCfop, "{0}", "{1}", "{2}");

            var campoNomeCliente = FiscalConfig.NotaFiscalConfig.UsarNomeFantasiaNotaFiscal ? "nomeFantasia" : "nome";

            var campoLoja = string.Format("{0}.{1}", aliasLoja, tipoCampo == TipoCampo.Nome ? "razaoSocial" : "cnpj");
            var campoCliente = string.Format("COALESCE({0}, {1}.{2})", tipoCampo == TipoCampo.Nome ? aliasCliente + "." + campoNomeCliente : aliasNotaFiscal + ".cpf", aliasCliente, tipoCampo == TipoCampo.Nome ? campoNomeCliente : "cpf_cnpj");
            var campoFornec = string.Format("{0}.{1}", aliasFornecedor, tipoCampo == TipoCampo.Nome ? "razaoSocial" : "cpfCnpj");
            var campoTransportador = string.Format("{0}.{1}", aliasTransportador, tipoCampo == TipoCampo.Nome ? "Nome" : "CpfCnpj");
            var campoFornecTransportador = string.Format("COALESCE({0},{1})", campoFornec, campoTransportador);

            return string.Format(baseEmitente, string.IsNullOrEmpty(aliasTransportador) ? campoFornec : campoFornecTransportador, campoCliente, campoLoja);
        }

        private string Sql(uint idNf, uint numeroNFe, uint idPedido, uint idLoja, uint idCliente, string nomeCliente, uint idFornec,
            string nomeFornec, string codRota, int situacao, string situacoes, int tipoDoc, string dataIni, string dataFim, uint idCfop,
            string idsTiposCfop, string dataEntSaiIni, string dataEntSaiFim, string idsFormaPagtoNotaFiscal, int tipoNf, int finalidade,
            int formaEmissao, string infCompl, string codInternoProd, string descrProd, string valorInicial, string valorFinal,
            int ordenar, bool acessoExterno, bool sintegra, bool selecionar)
        {
            return Sql(null, idNf, numeroNFe, idPedido, idLoja, idCliente, nomeCliente, idFornec, nomeFornec, codRota, situacao, situacoes,
                tipoDoc, dataIni, dataFim, idCfop, idsTiposCfop, dataEntSaiIni, dataEntSaiFim, idsFormaPagtoNotaFiscal, tipoNf, finalidade,
                formaEmissao, infCompl, codInternoProd, descrProd, valorInicial, valorFinal, ordenar, acessoExterno, sintegra, selecionar);
        }

        private string Sql(GDASession session, uint idNf, uint numeroNFe, uint idPedido, uint idLoja, uint idCliente, string nomeCliente,
            uint idFornec, string nomeFornec, string codRota, int situacao, string situacoes, int tipoDoc, string dataIni, string dataFim,
            uint idCfop, string idsTiposCfop, string dataEntSaiIni, string dataEntSaiFim, string idsFormaPagtoNotaFiscal, int tipoNf, int finalidade,
            int formaEmissao, string infCompl, string codInternoProd, string descrProd, string valorInicial, string valorFinal,
            int ordenar, bool acessoExterno, bool sintegra, bool selecionar)
        {
            string campos = selecionar ? @"n.*, cf.codInterno as codCfop, cf.descricao as DescrCfop, mun.NomeCidade as municOcor, concat(mun.codIbgeUf, mun.codIbgeCidade) as codMunicipio, 
                " + SqlCampoEmitente(TipoCampo.Nome, "l", "c", "f", "n", "tc") + @" as nomeEmitente, (mbai.idNf is not null) as temMovimentacaoBemAtivo, '$$$' as Criterio, 
                " + SqlCampoDestinatario(TipoCampo.Nome, "l", "c", "f", "n", "tc") + @" as nomeDestRem, c.suframa as suframaCliente, concat(g.Descricao, concat(' - ', pc.Descricao)) as DescrPlanoContas, 
                t.Nome as nomeTransportador, " + SqlCampoDestinatario(TipoCampo.CpfCnpj, "l", "c", "f", "n", "tc") + @" as cpfCnpjDestRem,
                " + SqlCampoEmitente(TipoCampo.CpfCnpj, "l", "c", "f", "n", "tc") + @" as cnpjEmitente, (select tipoEntrega from pedido ped inner join 
                pedidos_nota_fiscal pnf on (ped.idPedido=pnf.idPedido) where pnf.idNf=n.idNf limit 1) as tipoEntrega, 
                coalesce(no.codInterno, cf.codInterno) as CodNaturezaOperacao, func.nome as DescrUsuCad, (" + SqlIsImportacao("n.idNf") + ") as isNfImportacao" : "Count(*)";

            string sql = "Select Distinct " + campos + @" From nota_fiscal n 
                Left Join natureza_operacao no On (n.idNaturezaOperacao=no.idNaturezaOperacao)
                Left Join cfop cf On (no.idCfop=cf.idCfop) 
                Left Join tipo_cfop tc On (cf.idTipoCfop=tc.idTipoCfop) 
                Left Join cidade mun On (n.idCidade=mun.idCidade) 
                Left Join loja l On (n.idLoja=l.idLoja) 
                Left Join fornecedor f On (n.idFornec=f.idFornec) 
                Left Join cliente c On (n.idCliente=c.id_Cli) 
                Left Join transportador t On (n.idTransportador=t.idTransportador) 
                Left Join funcionario func On (n.usuCad=func.idFunc)
                Left Join plano_contas pc on (n.idConta=pc.idConta)
                Left Join grupo_conta g On (pc.IdGrupo=g.IdGrupo) 
                Left Join (
                    select pnf.idNf
                    from movimentacao_bem_ativo_imob mbai 
                        left join produtos_nf pnf on (mbai.idProdNf=pnf.idProdNf)
                    group by pnf.idNf
                ) as mbai on (n.idNf=mbai.idNf)
                Where 1";

            NotaFiscal temp = new NotaFiscal();
            string criterio = "";

            if (idNf > 0)
            {
                sql += " And n.idNf=" + idNf;
                criterio += "Nota fiscal: " + idNf + "    ";
            }

            if (numeroNFe > 0)
            {
                sql += " And n.numeroNFe=" + numeroNFe;
                criterio += "Número NFe: " + numeroNFe + "    ";
            }

            if (idPedido > 0)
            {
                sql += " And n.idNf in (select idNf from pedidos_nota_fiscal where idPedido=" + idPedido + ")";
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (idLoja > 0)
            {
                sql += " And n.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetElement(session, idLoja).NomeFantasia + "    ";
            }

            if (idCliente > 0)
            {
                sql += " And n.idCliente=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(session, idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(session, null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (idFornec > 0)
            {
                sql += " And n.idFornec=" + idFornec;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(session, idFornec) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                sql += " And (f.RazaoSocial LIKE ?nomeFornec or f.NomeFantasia LIKE ?nomeFornec)";
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }

            if (!String.IsNullOrEmpty(codRota))
            {
                sql += " And n.idCliente In (Select idCliente From rota_cliente Where idRota In " +
                    "(Select idRota From rota where codInterno like ?codRota))";
                criterio += "Rota: " + codRota + "    ";
            }

            if (situacao > 0)
            {
                sql += " And n.situacao=" + situacao;
                temp.Situacao = situacao;
                criterio += "Situação: " + temp.SituacaoString + "    ";
            }
            else if (!string.IsNullOrEmpty(situacoes))
            {
                sql += " AND n.situacao IN(" + situacoes + ")";
            }

            if (tipoDoc > 0)
            {
                sql += " And n.tipoDocumento=" + tipoDoc;
                temp.TipoDocumento = tipoDoc;
                criterio += "Tipo documento: " + temp.TipoDocumentoString + "    ";
            }

            if (!sintegra)
            {
                if (!String.IsNullOrEmpty(dataIni))
                {
                    sql += " And n.DataEmissao>=?dataIni";
                    criterio += "Período Emissão: " + dataIni + "    ";
                }

                if (!String.IsNullOrEmpty(dataFim))
                {
                    sql += " And n.DataEmissao<=?dataFim";

                    if (!String.IsNullOrEmpty(dataIni))
                        criterio += " até " + dataFim + "    ";
                    else
                        criterio += "Período Emissão: até " + dataFim + "    ";
                }
            }
            else
            {
                sql += " And coalesce(if(n.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.Saída + @", n.dataSaidaEnt, null),
                    n.dataEmissao) Between ?dataIni And ?dataFim";
            }

            if (!String.IsNullOrEmpty(dataEntSaiIni))
            {
                sql += " And n.DataSaidaEnt>=?dataEntSaiIni";
                criterio += "Período Entrada/Saída: " + dataEntSaiIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataEntSaiFim))
            {
                sql += " And n.DataSaidaEnt<=?dataEntSaiFim";

                if (!String.IsNullOrEmpty(dataEntSaiIni))
                    criterio += " até " + dataEntSaiFim + "    ";
                else
                    criterio += "Período Entrada/Saída: até " + dataEntSaiFim + "    ";
            }

            if (!string.IsNullOrWhiteSpace(idsFormaPagtoNotaFiscal))
            {
                // Recupera os IdNf que tenha a forma de pagamento filtrada
                var idsNF = string.Join(",", ExecuteMultipleScalar<string>(
                    string.Format("SELECT IdNf FROM pagto_nota_fiscal WHERE FormaPagto IN ({0})", idsFormaPagtoNotaFiscal)));

                sql += string.Format(" AND n.IdNf IN ({0})", idsNF);
            }

            switch (tipoNf)
            {
                case 1:
                    sql += " and n.tipoDocumento=1";
                    criterio += "Tipo NF: Entrada    ";
                    break;
                case 2:
                    sql += " and n.tipoDocumento=2";
                    criterio += "Tipo NF: Saída    ";
                    break;
                case 3:
                    sql += " and n.tipoDocumento=3";
                    criterio += "Tipo NF: Entrada (terceiros)    ";
                    break;
                case 4:
                    sql += " and n.tipoDocumento=4";
                    criterio += "Tipo NF: Nota Fiscal de Cliente    ";
                    break;
                case 5:
                    sql += " and n.tipoDocumento=3 and n.transporte=1";
                    criterio += "Tipo NF: Transporte    ";
                    break;
            }

            switch (finalidade)
            {
                case 1:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Normal;
                    criterio += "Finalidade: NF-e Normal    ";
                    break;
                case 2:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar;
                    criterio += "Finalidade: NF-e Complementar    ";
                    break;
                case 3:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste;
                    criterio += "Finalidade: NF-e de Ajuste    ";
                    break;
                case 4:
                    sql += " and n.finalidadeEmissao=" + (int)NotaFiscal.FinalidadeEmissaoEnum.Devolucao;
                    criterio += "Finalidade: NF-e de Devolução/Retorno    ";
                    break;
            }

            switch (formaEmissao)
            {
                case 1:
                    sql += " and n.formaEmissao=" + (int)NotaFiscal.TipoEmissao.Normal;
                    criterio += "Forma de emissão: Normal    ";
                    break;
                case 3:
                    sql += " and n.formaEmissao=" + (int)NotaFiscal.TipoEmissao.ContingenciaComSCAN;
                    criterio += "Forma de emissão: Contingência    ";
                    break;
            }

            if (idCfop > 0)
            {
                sql += " and (no.idCfop=" + idCfop + " or " + idCfop + @" in (select no1.idCfop from produtos_nf pnf 
                    inner join natureza_operacao no1 on (pnf.idNaturezaOperacao=no1.idNaturezaOperacao) where idNf=n.idNf))";

                criterio += "CFOP: " + CfopDAO.Instance.ObtemValorCampo<string>(session, "codInterno", "idCfop=" + idCfop) + "    ";
            }

            if (!String.IsNullOrEmpty(idsTiposCfop))
            {
                sql += @" 
                    And (cf.idTipoCfop IN (" + idsTiposCfop + ") or " + idCfop + @" in 
                    (
                        select c.idCfop from produtos_nf p 
                            inner join natureza_operacao no1 On (p.idNaturezaOperacao=no1.idNaturezaOperacao)
                            inner join cfop c On (no1.idCfop=c.idCfop) 
                        Where idNf=n.idNf And c.idTipoCfop IN (" + idsTiposCfop + @")
                    ))";

                criterio += "Tipo(s) CFOP: " + TipoCfopDAO.Instance.GetDescrByString(session, idsTiposCfop) + "    ";
            }

            if (!String.IsNullOrEmpty(infCompl))
            {
                sql += " And n.infCompl like ?infCompl";
                criterio += "Informação complementar: " + infCompl + "    ";
            }

            // Se for acesso externo, permite visualização apenas de notas autorizadas e canceladas
            if (acessoExterno)
                sql += " And n.situacao in (" + (int)NotaFiscal.SituacaoEnum.Autorizada + "," + (int)NotaFiscal.SituacaoEnum.Cancelada + ")";

            if (!String.IsNullOrEmpty(codInternoProd))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(session, codInternoProd, null);
                sql += " and n.idnf in (select idnf from produtos_nf where idProd in (" + ids + "))";
                criterio += "Produto: " + ProdutoDAO.Instance.GetDescrProduto(session, codInternoProd) + "    ";
            }
            else if (!String.IsNullOrEmpty(descrProd))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(session, null, descrProd);
                sql += " and n.idnf in (select idnf from produtos_nf where idProd in (" + ids + "))";
                criterio += "Produto: " + descrProd + "    ";
            }

            if (!String.IsNullOrEmpty(valorInicial))
            {
                sql += " And n.TotalNota >= " + valorInicial.Replace(",", ".");
            }

            if (!String.IsNullOrEmpty(valorFinal))
            {
                sql += " And n.TotalNota <= " + valorFinal.Replace(",", ".");
            }

            switch (ordenar)
            {
                case 1:
                    sql += " order by n.NumeroNFE desc";
                    criterio += "Ordenar por: Número NF-e (descresc.)    ";
                    break;
                case 2:
                    sql += " order by n.NumeroNFE asc";
                    criterio += "Ordenar por: Número NF-e (cresc.)    ";
                    break;
                case 3:
                    sql += " order by n.DataEmissao desc";
                    criterio += "Ordenar por: Data de emissão (descresc.)    ";
                    break;
                case 4:
                    sql += " order by n.DataEmissao asc";
                    criterio += "Ordenar por: Data de emissão (cresc.)    ";
                    break;
                case 5:
                    sql += " order by n.DataSaidaEnt desc";
                    criterio += "Ordenar por: Data de entrada/saída (descresc.)    ";
                    break;
                case 6:
                    sql += " order by n.DataSaidaEnt asc";
                    criterio += "Ordenar por: Data de entrada/saída (cresc.)    ";
                    break;
            }

            return sql.Replace("$$$", criterio);
        }

        public NotaFiscal GetElement(uint idNf)
        {
            return GetElement(null, idNf);
        }

        public NotaFiscal GetElement(GDASession sessao, uint idNf)
        {
            NotaFiscal notaFiscal = objPersistence.LoadOneData(sessao, Sql(idNf, 0, 0, 0, 0, null, 0, null, null, 0, null, 0, null, null, 0, null, null, null, null, 0,
                0, 0, null, null, null, null, null, 0, false, false, true));

            if (notaFiscal == null)
                return null;

            #region Busca as parcelas da nota fiscal

            var lstParc = ParcelaNfDAO.Instance.GetByNf(sessao, idNf).ToArray();

            notaFiscal.ValoresParcelas = new decimal[lstParc.Length];
            notaFiscal.DatasParcelas = new DateTime[lstParc.Length];
            notaFiscal.BoletosParcelas = new string[lstParc.Length];
            string parcelas = lstParc.Length + " vez(es): ";

            for (int i = 0; i < lstParc.Length; i++)
            {
                notaFiscal.ValoresParcelas[i] = lstParc[i].Valor;
                notaFiscal.DatasParcelas[i] = lstParc[i].Data != null ? lstParc[i].Data.Value : new DateTime();
                notaFiscal.BoletosParcelas[i] = lstParc[i].NumBoleto;
                parcelas += lstParc[i].Valor.ToString("c") + "-" + (lstParc[i].Data != null ? lstParc[i].Data.Value.ToString("d") : "") + ",  ";
            }

            if (lstParc.Length > 0 && notaFiscal.FormaPagto != 1)
                notaFiscal.DescrParcelas = parcelas.TrimEnd(' ').TrimEnd(',');

            #endregion

            #region Forma Pgto

            if (notaFiscal.IdAntecipFornec.HasValue &&
                notaFiscal.FormaPagto == (int)NotaFiscal.FormaPagtoEnum.Antecipacao)
                notaFiscal.DescrFormaPagto = "Antecip. Fornecedor";

            #endregion

            return notaFiscal;
        }

        public NotaFiscal[] GetByNumeroNFe(uint numeroNFe, int tipoDocumento)
        {
            return GetByNumeroNFe(null, numeroNFe, tipoDocumento);
        }

        public NotaFiscal[] GetByNumeroNFe(GDASession session, uint numeroNFe, int tipoDocumento)
        {
            return objPersistence.LoadData(session, Sql(session, 0, numeroNFe, 0, 0, 0, null, 0, null, null, 0, null, tipoDocumento, null, null, 0, null, null,
                null, null, 0, 0, 0, null, null, null, null, null, 0, false, false, true)).ToArray();
        }

        public IList<NotaFiscal> GetList(uint numeroNFe, uint idPedido, uint idLoja, uint idCliente, string nomeCliente, uint idFornec,
            string nomeFornec, string codRota, int tipoDoc, int situacao, string dataIni, string dataFim, uint idCfop, string idsTiposCfop,
            string dataEntSaiIni, string dataEntSaiFim, string idsFormaPagtoNotaFiscal, int tipoNf, int finalidade, int formaEmissao, string infCompl,
            string codInternoProd, string descrProd, string valorInicial, string valorFinal, int ordenar, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "n.idNf desc" : sortExpression;

            return LoadDataWithSortExpression(Sql(0, numeroNFe, idPedido, idLoja, idCliente, nomeCliente, idFornec, nomeFornec, codRota,
                situacao, null, tipoDoc, dataIni, dataFim, idCfop, idsTiposCfop, dataEntSaiIni, dataEntSaiFim, idsFormaPagtoNotaFiscal, tipoNf, finalidade, formaEmissao,
                infCompl, codInternoProd, descrProd, valorInicial, valorFinal, ordenar, false, false, true), sortExpression, startRow, pageSize,
                GetParams(null, codRota, nomeCliente, nomeFornec, dataIni, dataFim, dataEntSaiIni, dataEntSaiFim, infCompl, codInternoProd, descrProd,
                valorInicial, valorFinal, null, null));
        }

        public int GetCount(uint numeroNFe, uint idPedido, uint idLoja, uint idCliente, string nomeCliente, uint idFornec,
            string nomeFornec, string codRota, int tipoDoc, int situacao, string dataIni, string dataFim, uint idCfop, string idsTiposCfop,
            string dataEntSaiIni, string dataEntSaiFim, string idsFormaPagtoNotaFiscal, int tipoNf, int finalidade, int formaEmissao, string infCompl,
            string codInternoProd, string descrProd, string valorInicial, string valorFinal, int ordenar)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, numeroNFe, idPedido, idLoja, idCliente, nomeCliente, idFornec, nomeFornec,
                codRota, situacao, null, tipoDoc, dataIni, dataFim, idCfop, idsTiposCfop, dataEntSaiIni, dataEntSaiFim, idsFormaPagtoNotaFiscal, tipoNf, finalidade,
                formaEmissao, infCompl, codInternoProd, descrProd, valorInicial, valorFinal, ordenar, false, false, false),
                GetParams(null, codRota, nomeCliente, nomeFornec, dataIni, dataFim, dataEntSaiIni, dataEntSaiFim, infCompl, codInternoProd,
                descrProd, valorInicial, valorFinal, null, null));
        }

        /// <summary>
        /// Retorna notas fiscais para acesso externo
        /// </summary>
        public IList<NotaFiscal> GetListAcessoExterno(uint numeroNFe, string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            uint? idCliente = UserInfo.GetUserInfo.IdCliente;

            if (idCliente == null || idCliente == 0)
                return null;

            sortExpression =
                string.IsNullOrEmpty(sortExpression) ?
                    "DataEmissao DESC" :
                    sortExpression;

            return LoadDataWithSortExpression(Sql(0, numeroNFe, 0, 0, idCliente.Value, null, 0, null, null, 0, null, 0, dataIni, dataFim, 0, null, null,
                null, null, 0, 0, 0, null, null, null, null, null, 0, true, false, true), sortExpression, startRow, pageSize,
                GetParams(null, null, null, null, dataIni, dataFim, null, null, null, null, null, null, null, null, null));
        }

        public int GetAcessoExternoCount(uint numeroNFe, string dataIni, string dataFim)
        {
            uint? idCliente = UserInfo.GetUserInfo.IdCliente;

            if (idCliente == null || idCliente == 0)
                return 0;

            return objPersistence.ExecuteSqlQueryCount(Sql(0, numeroNFe, 0, 0, idCliente.Value, null, 0, null, null, 0, null, 0, dataIni, dataFim,
                0, null, null, null, null, 0, 0, 0, null, null, null, null, null, 0, true, false, false), GetParams(null, null, null, null, dataIni, dataFim,
                null, null, null, null, null, null, null, null, null));
        }

        private void ColocaMensagemNaturezasOperacaoInfAdic(ref List<NotaFiscal> notas)
        {
            notas.ForEach(nf =>
            {
                string mensagemNaturezasOperacao = nf.MensagemNaturezasOperacao;
                if (!String.IsNullOrEmpty(mensagemNaturezasOperacao))
                    nf.InfCompl = (!String.IsNullOrEmpty(nf.InfCompl) ? nf.InfCompl + " " : "") + mensagemNaturezasOperacao;
            });
        }

        public List<NotaFiscal> GetForSintegra(uint idLoja, string dataIni, string dataFim, bool canceladas)
        {
            string sql = Sql(0, 0, 0, 0, 0, null, 0, null, null, 0, null, 0, dataIni, dataFim, 0, null, null, null, null, 0, 0, 0, null, null, null, null, null, 0, false, true, true);

            // Filtra pela loja selecionada
            sql += " And n.idLoja=" + idLoja + " And n.situacao In (" + (int)NotaFiscal.SituacaoEnum.Autorizada + ", " +
                (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros + (canceladas ? "," + (int)NotaFiscal.SituacaoEnum.Cancelada : "") + @")
                and n.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.NotaCliente;

            var notas = objPersistence.LoadData(sql, GetParams(null, null, null, null, dataIni, dataFim, null, null, null, null, null, null, null, null, null)).ToList();

            ColocaMensagemNaturezasOperacaoInfAdic(ref notas);
            return notas;
        }

        public NotaFiscal[] GetForEFD(string idsLojas, DateTime inicio, DateTime fim)
        {
            return GetForEFD(idsLojas, inicio, fim, true, 0, null);
        }

        public NotaFiscal[] GetForEFD(string idsLojas, DateTime inicio, DateTime fim, bool atribuirMensagensNaturezaOperacao, int tipoDoc, string situacoesNf)
        {
            string dataIni = inicio.ToString("dd/MM/yyyy");
            string dataFim = fim.ToString("dd/MM/yyyy");

            string sql = Sql(0, 0, 0, 0, 0, null, 0, null, null, 0, situacoesNf, tipoDoc, dataIni, dataFim, 0, null, null, null, null, 0, 0, 0, null, null, null, null, null, 0, false, true, true);

            // Filtra pela loja selecionada
            if (!String.IsNullOrEmpty(idsLojas) && idsLojas != "0")
                sql += " And n.idLoja in (" + idsLojas + ")";

            sql += " And n.situacao In (" + (int)NotaFiscal.SituacaoEnum.Autorizada + ", " + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros + ", " +
                (int)NotaFiscal.SituacaoEnum.Cancelada + ", " + (int)NotaFiscal.SituacaoEnum.Denegada + ", " + (int)NotaFiscal.SituacaoEnum.Inutilizada + @")
                and n.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.NotaCliente;

            var notas = objPersistence.LoadData(sql, GetParams(null, null, null, null, dataIni, dataFim, null, null, null, null, null, null, null, null, null)).ToList();

            if (atribuirMensagensNaturezaOperacao)
                ColocaMensagemNaturezasOperacaoInfAdic(ref notas);

            return notas.ToArray();
        }

        private GDAParameter[] GetParams(string modelo, string codRota, string nomeCliente, string nomeFornec, string dataIni, string dataFim,
            string dataEntSaiIni, string dataEntSaiFim, string infCompl, string codInternoProd, string descrProd,
            string valorInicial, string valorFinal, string cnpjFornecedor, string lote)
        {
            List<GDAParameter> retorno = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(modelo))
                retorno.Add(new GDAParameter("?modelo", modelo));

            if (!String.IsNullOrEmpty(codRota))
                retorno.Add(new GDAParameter("?codRota", codRota));

            if (!String.IsNullOrEmpty(nomeCliente))
                retorno.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(nomeFornec))
                retorno.Add(new GDAParameter("?nomeFornec", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                retorno.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                retorno.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(dataEntSaiIni))
                retorno.Add(new GDAParameter("?dataEntSaiIni", DateTime.Parse(dataEntSaiIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataEntSaiFim))
                retorno.Add(new GDAParameter("?dataEntSaiFim", DateTime.Parse(dataEntSaiFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(infCompl))
                retorno.Add(new GDAParameter("?infCompl", "%" + infCompl + "%"));

            if (!String.IsNullOrEmpty(codInternoProd))
                retorno.Add(new GDAParameter("?codInterno", codInternoProd));

            if (!String.IsNullOrEmpty(descrProd))
                retorno.Add(new GDAParameter("?descrProd", "%" + descrProd + "%"));

            //if (!String.IsNullOrEmpty(valorInicial))
            //    retorno.Add(new GDAParameter("?valorInicial", valorInicial));

            //if (!String.IsNullOrEmpty(valorFinal))
            //    retorno.Add(new GDAParameter("?valorFinal", valorFinal));

            if (!string.IsNullOrEmpty(cnpjFornecedor))
                retorno.Add(new GDAParameter("?cpfCnpj", Formatacoes.FormataCpfCnpj(cnpjFornecedor)));

            if (!String.IsNullOrEmpty(lote))
                retorno.Add(new GDAParameter("?lote", "%" + lote + "%"));

            return retorno.ToArray();
        }

        public IList<NotaFiscal> GetByString(string idsNf)
        {
            if (idsNf == null || String.IsNullOrEmpty(idsNf.Trim(',')))
                return new List<NotaFiscal>();

            string sql = Sql(0, 0, 0, 0, 0, null, 0, null, null, 0, null, 0, null, null, 0, null, null, null, null, 0,
                0, 0, null, null, null, null, null, 0, false, false, true);

            sql += " and n.idNf in (" + idsNf.Trim(',') + ")";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca de NF para cancelamento

        private string SqlCanc(uint idNf, uint numeroNFe, uint idPedido, uint idLoja, int situacao, int tipoDoc, bool selecionar)
        {
            string campos = selecionar ? @"n.*, cf.codInterno as codCfop, mun.NomeCidade as municOcor, 
                " + SqlCampoEmitente(TipoCampo.Nome, "l", "c", "f", "n", "tc") + @" as nomeEmitente, 
                " + SqlCampoDestinatario(TipoCampo.Nome, "l", "c", "f", "n", "tc") + @" as nomeDestRem, 
                " + SqlCampoDestinatario(TipoCampo.CpfCnpj, "l", "c", "f", "n", "tc") + @" as cpfCnpjDestRem, 
                coalesce(no.codInterno, cf.codInterno) as CodNaturezaOperacao, t.Nome as nomeTransportador, " +
                SqlCampoEmitente(TipoCampo.CpfCnpj, "l", "c", "f", "n", "tc") + @" as cnpjEmitente" : "Count(*)";

            string sql = "Select " + campos + @" From nota_fiscal n 
                Left Join natureza_operacao no On (n.idNaturezaOperacao=no.idNaturezaOperacao)
                Left Join cfop cf On (no.idCfop=cf.idCfop) 
                Left Join tipo_cfop tc On (cf.idTipoCfop=tc.idTipoCfop) 
                Left Join cidade mun On (n.idCidade=mun.idCidade) 
                Left Join loja l On (n.idLoja=l.idLoja) 
                Left Join fornecedor f On (n.idFornec=f.idFornec) 
                Left Join cliente c On (n.idCliente=c.id_Cli) 
                Left Join transportador t On (n.idTransportador=t.idTransportador) 
                Where tipoDocumento not in (" + (int)NotaFiscal.TipoDoc.EntradaTerceiros + "," + (int)NotaFiscal.TipoDoc.NotaCliente + @")
                And n.situacao in (" + (int)NotaFiscal.SituacaoEnum.Autorizada + "," +
                    (int)NotaFiscal.SituacaoEnum.FalhaCancelar + "," + (int)NotaFiscal.SituacaoEnum.ProcessoCancelamento + ")";

            if (idNf > 0)
                sql += " And n.idNf=" + idNf;

            if (numeroNFe > 0)
                sql += " And n.numeroNFe=" + numeroNFe;

            if (idPedido > 0)
                sql += " And n.idNf In (Select idNf From pedidos_nota_fiscal Where idPedido=" + idPedido + ")";

            if (idLoja > 0)
                sql += " And n.IdLoja=" + idLoja;

            if (situacao > 0)
                sql += " And n.situacao=" + situacao;

            if (tipoDoc > 0)
                sql += " And n.tipoDocumento=" + tipoDoc;

            return sql;
        }

        public IList<NotaFiscal> GetListCanc(uint numeroNFe, uint idPedido, uint idLoja, int tipoDoc, int situacao, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "n.idNf desc" : sortExpression;

            return LoadDataWithSortExpression(SqlCanc(0, numeroNFe, idPedido, idLoja, situacao, tipoDoc, true), sort, startRow, pageSize, null);
        }

        public int GetCountCanc(uint numeroNFe, uint idPedido, uint idLoja, int tipoDoc, int situacao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlCanc(0, numeroNFe, idPedido, idLoja, situacao, tipoDoc, false), null);
        }

        #endregion

        #region Busca de NF para inutilização

        private string SqlInut(uint idNf, uint numeroNFe, uint idPedido, uint idLoja, int situacao, int tipoDoc, bool selecionar)
        {
            string campos = selecionar ? @"n.*, cf.codInterno as codCfop, mun.NomeCidade as municOcor, 
                " + SqlCampoEmitente(TipoCampo.Nome, "l", "c", "f", "n", "tc") + @" as nomeEmitente, 
                " + SqlCampoDestinatario(TipoCampo.Nome, "l", "c", "f", "n", "tc") + @" as nomeDestRem, 
                " + SqlCampoDestinatario(TipoCampo.CpfCnpj, "l", "c", "f", "n", "tc") + @" as cpfCnpjDestRem, 
                coalesce(no.codInterno, cf.codInterno) as CodNaturezaOperacao, t.Nome as nomeTransportador,
                " + SqlCampoEmitente(TipoCampo.CpfCnpj, "l", "c", "f", "n", "tc") + @" as cnpjEmitente" : "Count(*)";

            string sql =
                "Select " + campos + @" From nota_fiscal n 
                    Left Join natureza_operacao no On (n.idNaturezaOperacao=no.idNaturezaOperacao)
                    Left Join cfop cf On (no.idCfop=cf.idCfop) 
                    Left Join tipo_cfop tc On (cf.idTipoCfop=tc.idTipoCfop) 
                    Left Join cidade mun On (n.idCidade=mun.idCidade) 
                    Left Join loja l On (n.idLoja=l.idLoja) 
                    Left Join fornecedor f On (n.idFornec=f.idFornec) 
                    Left Join cliente c On (n.idCliente=c.id_Cli) 
                    Left Join transportador t On (n.idTransportador=t.idTransportador) 
                Where tipoDocumento<>" + (int)NotaFiscal.TipoDoc.EntradaTerceiros +
                " And n.situacao in (" + (int)NotaFiscal.SituacaoEnum.Aberta + "," +
                    (int)NotaFiscal.SituacaoEnum.FalhaInutilizar + "," +
                    (int)NotaFiscal.SituacaoEnum.FalhaEmitir + "," +
                    (int)NotaFiscal.SituacaoEnum.FalhaCancelar + "," +
                    (int)NotaFiscal.SituacaoEnum.NaoEmitida + "," +
                    (int)NotaFiscal.SituacaoEnum.ProcessoCancelamento + "," +
                    (int)NotaFiscal.SituacaoEnum.ProcessoEmissao + "," +
                    (int)NotaFiscal.SituacaoEnum.ProcessoInutilizacao + ")";

            if (idNf > 0)
                sql += " And n.idNf=" + idNf;

            if (numeroNFe > 0)
                sql += " And n.numeroNFe=" + numeroNFe;

            if (idPedido > 0)
                sql += " And n.idNf In (Select idNf From pedidos_nota_fiscal Where idPedido=" + idPedido + ")";

            if (idLoja > 0)
                sql += " And n.idLoja=" + idLoja;

            if (situacao > 0)
                sql += " And n.situacao=" + situacao;

            if (tipoDoc > 0)
                sql += " And n.tipoDocumento=" + tipoDoc;

            return sql;
        }

        public IList<NotaFiscal> GetListInut(uint numeroNFe, uint idPedido, uint idLoja, int tipoDoc, int situacao, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "n.idNf desc" : sortExpression;
            return LoadDataWithSortExpression(SqlInut(0, numeroNFe, idPedido, idLoja, situacao, tipoDoc, true), sort, startRow, pageSize, null);
        }

        public int GetCountInut(uint numeroNFe, uint idPedido, uint idLoja, int tipoDoc, int situacao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlInut(0, numeroNFe, idPedido, idLoja, situacao, tipoDoc, false), null);
        }

        #endregion

        #region Obtém NF pela chave de acesso

        /// <summary>
        /// Obtém NF pela chave de acesso
        /// </summary>
        public NotaFiscal GetByChaveAcesso(GDASession session, string chaveAcesso)
        {
            string sql = @"
                SELECT n.*, " + SqlCampoEmitente(TipoCampo.CpfCnpj, "l", "c", "f", "n", "tc") + @" as cnpjEmitente
                FROM nota_fiscal n
                    Left Join natureza_operacao no On (n.idNaturezaOperacao=no.idNaturezaOperacao)
                    Left Join cfop cf On (no.idCfop=cf.idCfop) 
                    Left Join tipo_cfop tc On (cf.idTipoCfop=tc.idTipoCfop)
                    Left Join loja l On (n.idLoja=l.idLoja) 
                    Left Join fornecedor f On (n.idFornec=f.idFornec) 
                    Left Join cliente c On (n.idCliente=c.id_Cli) 
                WHERE n.chaveAcesso=?chAcesso";

            List<NotaFiscal> lstNf = objPersistence.LoadData(session, sql,
                new GDAParameter[] { new GDAParameter("?chAcesso", chaveAcesso.Replace("NFe", "")) }).ToList();

            if (lstNf.Count == 0)
                throw new Exception("Não há nenhuma NF-e cadastrada com a chave de acesso informada.");
            else if (lstNf.Count > 1)
                throw new Exception("Há mais de uma NF-e cadastrada com a chave de acesso informada.");

            return lstNf[0];
        }

        #endregion

        #region Verifica a existência de uma nota de entrada de terceiros

        /// <summary>
        /// Verifica se existe nota de entrada de terceiros para o fornecedor com o número e série informados.
        /// </summary>
        public bool VerificarExistenciaNotaEntradaTerceiros(string idFornecedor, string serie, string numeroNfe)
        {
            var sql =
                string.Format("SELECT COUNT(*) FROM nota_fiscal WHERE IdFornec=?idFornec AND Serie=?serie AND NumeroNfe=?numeroNfe AND TipoDocumento={0}",
                    (int)NotaFiscal.TipoDoc.EntradaTerceiros);

            return objPersistence.ExecuteSqlQueryCount(sql,
                new GDAParameter("?idFornec", idFornecedor), new GDAParameter("?serie", serie), new GDAParameter("?numeroNfe", numeroNfe)) > 0;
        }

        #endregion

        #region Chave de Acesso NF-e

        /// <summary>
        /// Retorna chave de acesso da NFe
        /// </summary>
        /// <param name="cUf">Código da UF do emitente do Documento Fiscal</param>
        /// <param name="aamm">Ano e Mês de emissão da NF-e</param>
        /// <param name="cnpj">CNPJ do emitente</param>
        /// <param name="mod">Modelo do Documento Fiscal</param>
        /// <param name="serie">Série do Documento Fiscal</param>
        /// <param name="nNF">Número do Documento Fiscal</param>
        /// <param name="cNf">Código Numérico que compõe a Chave de Acesso</param>
        public string ChaveDeAcesso(string cUf, string aamm, string cnpj, string mod, string serie, string nNF, string tpEmis, string cNf)
        {
            if (!Glass.Validacoes.ValidaCnpj(cnpj))
                throw new Exception("CNPJ do emitente é inválido.");

            string chave = cUf + aamm + cnpj.Replace(".", "").Replace("/", "").Replace("-", "") + mod.PadLeft(2, '0') +
                serie.PadLeft(3, '0') + nNF.PadLeft(9, '0') + tpEmis + cNf.PadLeft(8, '0');

            if (chave.Length != 43)
                throw new Exception("Parâmetros da chave de acesso incorretos.");

            return chave + CalculaDV(chave, 4);
        }

        /// <summary>
        /// Calcula o dígito verificador para uma sequência numérica.
        /// </summary>
        /// <param name="textoCalcular"></param>
        /// <param name="pesoInicial"></param>
        /// <returns></returns>
        internal int CalculaDV(string textoCalcular, int pesoInicial)
        {
            int peso = pesoInicial, ponderacao = 0;

            for (int i = 0; i < textoCalcular.Length; i++)
            {
                ponderacao += Glass.Conversoes.StrParaInt(textoCalcular[i].ToString()) * peso--;

                if (peso == 1)
                    peso = 9;
            }

            // Calcula o resto da divisão da ponderação por 11
            int restoDiv = (ponderacao % 11);

            // Se o restoDiv for 0 ou 1, o dígito deverá ser 0
            return 11 - (restoDiv == 0 || restoDiv == 1 ? 11 : restoDiv);
        }

        #endregion

        #region Altera situação

        public int AlteraSituacao(uint idNf, NotaFiscal.SituacaoEnum situacao)
        {
            return AlteraSituacao(null, idNf, situacao);
        }

        /// <summary>
        /// Altera situação da NF
        /// </summary>
        public int AlteraSituacao(GDASession sessao, uint idNf, NotaFiscal.SituacaoEnum situacao)
        {
            try
            {
                string sql = "Update nota_fiscal set situacao=" + (int)situacao + " Where idNf=" + idNf;

                if (situacao == NotaFiscal.SituacaoEnum.Cancelada)
                    EnviarEmailXml(sessao, GetElement(sessao, idNf), true);

                return objPersistence.ExecuteCommand(sessao, sql);
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(idNf, "Falha ao alterar a situação da nota fiscal.", 0, ex.Message);
                return 0;
            }
        }

        #endregion

        #region Insere informação complementar

        /// <summary>
        /// Insere informação complementar
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="situacao"></param>
        public int InsereInfCompl(GDASession sessao, uint idNf, string infCompl)
        {
            string sql = "Update nota_fiscal set infCompl=Concat(Coalesce(infCompl, ''), ?infCompl) Where idNf=" + idNf;

            return objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?infCompl", infCompl));
        }

        #endregion

        #region Gera um número de lote

        /// <summary>
        /// Atualiza a nota fiscal passada com um número de lote ainda não utilizado, retornado-o
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public int GetNewNumLote(uint idNf)
        {
            // Verifica se essa NFe já possui um número de lote
            string sql = "Select Coalesce(numLote, 0) From nota_fiscal Where idNf=" + idNf;

            int numLote = Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString());

            if (numLote > 0)
                return numLote;

            // Se esta NFe não possuir um número de lote, cria um novo número para a mesma
            sql = "Select Coalesce(Max(numLote)+1, 1) From nota_fiscal Where idNf<>" + idNf;

            numLote = Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString());

            objPersistence.ExecuteCommand("Update nota_fiscal Set numLote=" + numLote + " Where idNf=" + idNf);

            return numLote;
        }

        #endregion

        #region Retorna próxima numeração de NFe

        /// <summary>
        /// Retorna o próximo número de NFe a ser utilizado
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="serie"></param>
        private uint ProxNumeroNFe(uint idLoja, int serie)
        {
            return ProxNumeroNFe(null, idLoja, serie);
        }

        /// <summary>
        /// Retorna o próximo número de NFe a ser utilizado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idLoja"></param>
        /// <param name="serie"></param>
        private uint ProxNumeroNFe(GDASession sessao, uint idLoja, int serie)
        {
            string sql = null;

            sql = "Select Coalesce(Max(numeroNFe) + 1, 1) From nota_fiscal Where idLoja=" + idLoja + " And serie=" + serie +
                " and TipoDocumento not in (" + (int)NotaFiscal.TipoDoc.EntradaTerceiros + "," + (int)NotaFiscal.TipoDoc.NotaCliente + ")";

            return Glass.Conversoes.StrParaUint(objPersistence.ExecuteScalar(sessao, sql).ToString());
        }

        /// <summary>
        /// Verifica se já existe uma nota com o número passado
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="serie"></param>
        /// <returns></returns>
        private bool ExisteNumeroNFe(int numeroNFe, uint idLoja, int serie)
        {
            return ExecuteScalar<bool>("Select Count(*) > 0 From nota_fiscal Where numeroNFe=" + numeroNFe + " And idLoja=" + idLoja + " And serie=" + serie +
                " and TipoDocumento not in (" + (int)NotaFiscal.TipoDoc.EntradaTerceiros + "," + (int)NotaFiscal.TipoDoc.NotaCliente + ")");
        }

        #endregion

        #region Cria nota fiscal em contingência

        /// <summary>
        /// Cria nota fiscal em contingência a partir de outra nota fiscal
        /// </summary>
        /// <param name="idNf"></param>
        public uint CriaNFeContingencia(NotaFiscal nf)
        {
            uint idNfEmissaoNormal = nf.IdNf;
            uint idNf = 0;

            if (FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.SCAN && nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.Normal)
            {
                var uf = CidadeDAO.Instance.ObtemValorCampo<string>("NomeUf", "idCidade=" + nf.IdCidade);

                nf.FormaEmissao = (int)ConfigNFe.ObtemTipoContingencia(uf);
                nf.CodAleatorio = null;
                nf.NumeroNFe = ProxNumeroNFe(nf.IdLoja.Value, Conversoes.StrParaInt(nf.Serie));

                idNf = NotaFiscalDAO.Instance.Insert(nf);

                // Insere os produtos da nota
                foreach (ProdutosNf pnf in ProdutosNfDAO.Instance.GetByNf(idNfEmissaoNormal))
                {
                    pnf.IdNf = idNf;
                    ProdutosNfDAO.Instance.InsertBase(pnf);
                }

                // Insere as parcelas da nota
                foreach (ParcelaNf parc in ParcelaNfDAO.Instance.GetByNf(idNfEmissaoNormal))
                {
                    parc.IdNf = idNf;
                    ParcelaNfDAO.Instance.Insert(parc);
                }

                // Insere os pedidos associados à esta nota
                foreach (PedidosNotaFiscal ped in PedidosNotaFiscalDAO.Instance.GetByNf(idNfEmissaoNormal))
                {
                    ped.IdNf = idNf;
                    PedidosNotaFiscalDAO.Instance.Insert(ped);
                }
            }

            return idNf;
        }

        #endregion

        #region Retorna idCfop da NFe

        public uint GetIdNaturezaOperacao(GDASession sessao, uint idNf)
        {
            return ObtemValorCampo<uint>(sessao, "idNaturezaOperacao", "idNf=" + idNf);
        }

        /// <summary>
        /// Retorna o idCfop da nota
        /// </summary>
        public uint GetIdCfop(GDASession sessao, uint idNf)
        {
            uint idNaturezaOperacao = GetIdNaturezaOperacao(sessao, idNf);
            return NaturezaOperacaoDAO.Instance.ObtemIdCfop(sessao, idNaturezaOperacao);
        }

        #endregion

        #region Retorna tipo documento da NFe

        public int GetTipoDocumento(uint idNf)
        {
            return GetTipoDocumento(null, idNf);
        }

        /// <summary>
        /// Retorna o tipo da nota
        /// 1-Entrada, 2-Saída, 3-Entrada(Terceiros)
        /// </summary>
        /// <param name="idNf"></param>
        public int GetTipoDocumento(GDASession sessao, uint idNf)
        {
            return ObtemValorCampo<int>(sessao, "tipoDocumento", "idNf=" + idNf);
        }

        #endregion

        #region Retorna números de NFe's a partir de idsNf

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna números de NFe's a partir de idsNf
        /// </summary>
        /// <param name="idsNf"></param>
        /// <returns></returns>
        public string ObtemNumerosNFePeloIdNf(string idsNf)
        {
            return ObtemNumerosNFePeloIdNf(null, idsNf);
        }

        /// <summary>
        /// Retorna números de NFe's a partir de idsNf
        /// </summary>
        /// <param name="idsNf"></param>
        /// <returns></returns>
        public string ObtemNumerosNFePeloIdNf(GDASession sessao, string idsNf)
        {
            if (String.IsNullOrEmpty(idsNf))
                return String.Empty;

            string sql = "Select Cast(group_concat(distinct numeroNfe) as char) From nota_fiscal Where idNf In (" + idsNf.Trim().TrimEnd(',') + ")";

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return String.Empty;

            return obj.ToString();
        }

        #endregion

        #region Retorna dados da NFe

        public uint? ObtemIdCliente(uint idNf)
        {
            return ObtemIdCliente(null, idNf);
        }

        public uint? ObtemIdCliente(GDASession sessao, uint idNf)
        {
            return ObtemValorCampo<uint?>(sessao, "idCliente", "idNf=" + idNf);
        }

        public DateTime? ObtemDataCad(uint idNf)
        {
            return ObtemValorCampo<DateTime?>("dataCad", "idNf=" + idNf);
        }

        public DateTime? ObtemDataEmissao(uint idNf)
        {
            return ObtemValorCampo<DateTime?>("dataEmissao", "idNf=" + idNf);
        }

        public DateTime? ObtemDataEntradaSaida(uint idNf)
        {
            return ObtemValorCampo<DateTime?>("dataSaidaEnt", "idNf=" + idNf);
        }

        /// <summary>
        /// Retorna números de NFe's a partir de idsNf
        /// </summary>
        /// <param name="idsNf"></param>
        /// <returns></returns>
        public string ObtemChaveAcesso(uint idNf)
        {
            string sql = "Select chaveAcesso From nota_fiscal Where idNf=" + idNf;

            object obj = objPersistence.ExecuteScalar(sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return String.Empty;

            return obj.ToString();
        }

        /// <summary>
        /// Retorna finalidade da NFe
        /// 1 - Normal
        /// 2 - Complementar
        /// 3 - Ajuste
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public int ObtemFinalidade(uint idNf)
        {
            return ObtemFinalidade(null, idNf);
        }

        /// <summary>
        /// Retorna finalidade da NFe
        /// 1 - Normal
        /// 2 - Complementar
        /// 3 - Ajuste
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public int ObtemFinalidade(GDASession sessao, uint idNf)
        {
            string sql = "Select finalidadeEmissao From nota_fiscal Where idNf=" + idNf;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return 1;

            return Glass.Conversoes.StrParaInt(obj.ToString());
        }

        /// <summary>
        /// Returna o número da nota passando o seu identificador
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public uint ObtemNumeroNf(GDASession session, uint idNf)
        {
            return ObtemValorCampo<uint>(session, "numeroNFE", "idnf=" + idNf);
        }

        public int ObtemSituacao(uint idNf)
        {
            return ObtemSituacao(null, idNf);
        }

        /// <summary>
        /// Retorna a situação da NFe
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public int ObtemSituacao(GDASession sessao, uint idNf)
        {
            string sql = "Select situacao From nota_fiscal Where idNf=" + idNf;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return 1;

            return Glass.Conversoes.StrParaInt(obj.ToString());
        }

        /// <summary>
        /// Retorna forma de emissão da NFe
        /// 1-Normal
        /// 2-Contingência
        /// 3-Contingência com SCAN
        /// 4-Contingência via DPEC
        /// 5-Contingência FS-DA
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public int ObtemFormaEmissao(uint idNf)
        {
            string sql = "Select formaEmissao From nota_fiscal Where idNf=" + idNf;

            object obj = objPersistence.ExecuteScalar(sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()) || obj.ToString() == "0")
                return 1;

            return Glass.Conversoes.StrParaInt(obj.ToString());
        }

        /// <summary>
        /// Retorna forma de emissão da NFe
        /// </summary>
        public NotaFiscal.FormaPagtoEnum ObtemFormaPagto(int idNf)
        {
            var sql = string.Format("SELECT FormaPagto FROM nota_fiscal WHERE IdNf={0}", idNf);

            var retorno = objPersistence.ExecuteScalar(sql);

            if (retorno == null)
                return (NotaFiscal.FormaPagtoEnum)0;

            return (NotaFiscal.FormaPagtoEnum)retorno.ToString().StrParaInt();
        }

        /// <summary>
        /// Retorna forma de emissão da NFe
        /// </summary>
        public NotaFiscal.FormaPagtoEnum ObtemFormaPagto(string chaveAcesso)
        {
            return ObtemValorCampo<NotaFiscal.FormaPagtoEnum>("FormaPagto", "ChaveAcesso=?chaveAcesso", new GDA.GDAParameter("?chaveAcesso", chaveAcesso));
        }

        public uint ObtemIdNaturezaOperacao(uint idNf)
        {
            return ObtemIdNaturezaOperacao(null, idNf);
        }

        public uint ObtemIdNaturezaOperacao(GDASession sessao, uint idNf)
        {
            return ObtemValorCampo<uint>(sessao, "idNaturezaOperacao", "idNf=" + idNf);
        }

        /// <summary>
        /// Retorna o IdCfop da nota
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        /* public uint ObtemIdCfop(uint idNf)
        {
            string sql = "Select idCfop From nota_fiscal Where idNf=" + idNf;

            object obj = objPersistence.ExecuteScalar(sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return 1;

            return Glass.Conversoes.StrParaUint(obj.ToString());
        } */

        public uint ObtemIdLoja(uint idNf)
        {
            return ObtemIdLoja(null, idNf);
        }

        /// <summary>
        /// Retorn o IdLoja da nota
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public uint ObtemIdLoja(GDASession sessao, uint idNf)
        {
            string sql = "Select idLoja From nota_fiscal Where idNf=" + idNf;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return 1;

            return Glass.Conversoes.StrParaUint(obj.ToString());
        }

        /// <summary>
        /// Retorna o modelo da nota.
        /// </summary>
        public string ObterModelo(GDASession sessao, uint idNf)
        {
            string sql = "SELECT Modelo FROM nota_fiscal WHERE IdNf=" + idNf;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return string.Empty;

            return obj.ToString();
        }

        /// <summary>
        /// Obtém o percentual de desconto da nota
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public float ObtemPercDesconto(uint idNf)
        {
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From nota_fiscal Where totalNota>0 And idNf=" + idNf) == 0)
                return 0;

            string sql = "Select desconto/if(totalManual>0, totalManual, totalNota) From nota_fiscal Where idNf=" + idNf;

            object obj = objPersistence.ExecuteScalar(sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return 0;

            return float.Parse(obj.ToString());
        }

        /// <summary>
        /// Obtém o percentual de desconto da nota considerando apenas os produtos
        /// (Usado para calcular a BCICMS de empresas do Simples Nacional)
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public float ObtemPercDescontoProd(uint idNf)
        {
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From nota_fiscal Where totalNota>0 And idNf=" + idNf) == 0)
                return 0;

            string sql = "Select desconto/if(totalManual>0, totalManual, totalProd) From nota_fiscal Where idNf=" + idNf;

            object obj = objPersistence.ExecuteScalar(sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return 0;

            return float.Parse(obj.ToString());
        }

        /// <summary>
        /// Obtém o valor total da nota
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public decimal ObtemTotal(uint idNf)
        {
            return ObtemTotal(null, idNf);
        }

        /// <summary>
        /// Obtém o valor total da nota
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public decimal ObtemTotal(GDASession sessao, uint idNf)
        {
            return ExecuteScalar<decimal>(sessao, "Select totalNota From nota_fiscal Where idNf=" + idNf);
        }

        /// <summary>
        /// Obtem o total manual da nota.
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public decimal ObtemTotalManual(uint idNf)
        {
            return ObtemTotalManual(null, idNf);
        }

        /// <summary>
        /// Obtem o total manual da nota.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public decimal ObtemTotalManual(GDASession sessao, uint idNf)
        {
            return ObtemValorCampo<decimal>(sessao, "totalManual", "idNf=" + idNf);
        }

        public string ObtemIdsNfRef(uint idNf)
        {
            return ObtemIdsNfRef(null, idNf);
        }

        public string ObtemIdsNfRef(GDASession session, uint idNf)
        {
            return ObtemValorCampo<string>(session, "idsNfRef", "idNf=" + idNf);
        }

        public string ObterIdsNf(GDASession session, int numeroNfe)
        {
            var idsNf = ExecuteMultipleScalar<int>(session, string.Format("SELECT IdNf FROM nota_fiscal WHERE NumeroNfe={0}", numeroNfe));

            return idsNf != null && idsNf.Count > 0 ? string.Join(",", idsNf) : string.Empty;
        }

        public string ObtemMotivoCanc(uint idNf)
        {
            return ObtemValorCampo<string>("motivoCanc", "idNf=" + idNf);
        }

        public string ObtemMotivoInut(uint idNf)
        {
            return ObtemValorCampo<string>("motivoInut", "idNf=" + idNf);
        }
        public uint? ObtemIdFornec(uint idNf)
        {
            return ObtemIdFornec(null, idNf);
        }

        public uint? ObtemIdFornec(GDASession session, uint idNf)
        {
            return ObtemValorCampo<uint?>(session, "idFornec", "idNf=" + idNf);
        }

        public IList<uint> GetIdsPedidoNotaFiscal(GDASession session, uint idNf)
        {
            var sql = string.Format(@"SELECT pnf.IdPedido FROM pedidos_nota_fiscal pnf WHERE pnf.IdNf={0}", idNf);
            return ExecuteMultipleScalar<uint>(session, sql);
        }

        public string ObtemNumNfePedidoLiberacao(uint? idLiberacao, uint? idPedido, bool apenasAutorizadas)
        {
            string sql = @"
                Select GROUP_CONCAT(nf.numeroNfe)
                From nota_fiscal nf
                    Inner Join pedidos_nota_fiscal pnf On (nf.IDNF = pnf.IDNF)
                Where " + (apenasAutorizadas ? "nf.Situacao = 2 And " : "") +
                (idLiberacao.HasValue && idLiberacao.Value > 0 ? " pnf.idLiberarPedido = " + idLiberacao.Value :
                (idPedido.HasValue && idPedido.Value > 0 ? " pnf.idPedido = " + idPedido.Value : ""));

            object obj = objPersistence.ExecuteScalar(sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return "";

            return obj.ToString();
        }

        public IList<uint> ObtemIdNfByContaR(uint idContaR, bool apenasAutorizadas)
        {
            string sql = @"
                SELECT nf.idNf
                FROM nota_fiscal nf
                    INNER JOIN pedidos_nota_fiscal pnf ON (nf.idNf = pnf.idNf)
                WHERE " + (apenasAutorizadas ? "nf.Situacao = " + (int)NotaFiscal.SituacaoEnum.Autorizada : "") +
                " AND pnf.idLiberarPedido = (SELECT COALESCE(idLiberarPedido, 0) FROM contas_receber WHERE idContaR =" + idContaR + ")";

            return ExecuteMultipleScalar<uint>(sql);
        }

        public int ObtemIdConta(GDASession session, int idNf)
        {
            return ObtemValorCampo<int>(session, "IdConta", "idNf=" + idNf);
        }

        public IList<uint> ObtemIdsNfReferenciadas(uint idNf)
        {
            var sql = @"
                SELECT idNF
                FROM nota_fiscal
                WHERE idsNfRef is not null
                    AND CONCAT(',',idsNfRef,',') LIKE ?idNf";

            return ExecuteMultipleScalar<uint>(sql, new GDAParameter("idNf", "%," + idNf + ",%"));
        }

        public decimal ObterValoresPagosAntecipadamente(int idNf)
        {
            return ExecuteScalar<decimal>(@"select sum(p.valorentrada + ValorPagamentoAntecipado) as valorPagamentoAntecipado
                                                from pedidos_nota_fiscal pnf 
                                                inner join pedido p on (p.idPedido = pnf.idPedido)
                                                where idnf =" + idNf);
        }

        #endregion

        #region Verifica se a nota pode ser editada

        /// <summary>
        /// Verifica se a nota fiscal pode ser editada
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool PodeSerEditada(uint idNf)
        {
            object obj = objPersistence.ExecuteScalar("Select situacao from nota_fiscal Where idNF=" + idNf);

            if (obj == null)
                return true;

            int situacao = Glass.Conversoes.StrParaInt(obj.ToString());

            NotaFiscal temp = new NotaFiscal();
            temp.Situacao = situacao;
            return temp.EditVisible;
        }

        #endregion

        #region Verifica se a nota é de transporte

        /// <summary>
        /// Verifica se a nota fiscal é de transporte
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool IsTransporte(uint idNf)
        {
            if (idNf == 0)
                return false;

            return objPersistence.ExecuteSqlQueryCount("Select Count(*) from nota_fiscal Where transporte=true And idNF=" + idNf) > 0;
        }

        #endregion

        #region Verifica se a nota é complementar

        /// <summary>
        /// Verifica se a nota fiscal é complementar
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool IsComplementar(uint idNf)
        {
            if (idNf == 0)
                return false;

            return ObtemFinalidade(idNf) == (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar;
        }

        #endregion

        #region Verifica se a nota é de série U

        /// <summary>
        /// Verifica se a nota fiscal é de transporte
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool IsSerieU(uint idNf)
        {
            if (idNf == 0)
                return false;

            return objPersistence.ExecuteSqlQueryCount("Select Count(*) from nota_fiscal Where serie='U' And idNF=" + idNf) > 0;
        }

        #endregion

        #region Recupera todas as notas de um dia, modelo e série

        /// <summary>
        /// Recupera todas as notas de um dia, modelo e série.
        /// </summary>
        /// <param name="dia"></param>
        /// <param name="modelo"></param>
        /// <param name="serie"></param>
        /// <returns></returns>
        public NotaFiscal[] GetByDiaModeloSerie(DateTime dia, string modelo, string serie)
        {
            string data = dia.ToString("dd/MM/yyyy");
            string sql = Sql(0, 0, 0, 0, 0, null, 0, null, null, 0, null, 0, data, data, 0, null, null, null, null, 0, 0, 0, null, null, null, null, null, 0, false, false, true);
            sql += " and Modelo='" + modelo + "' and Serie='" + serie + "' order by DataEmissao asc";
            return objPersistence.LoadData(sql, GetParams(null, null, null, null, data, data, null, null, null, null, null, null, null, null, null)).ToArray();
        }

        #endregion

        #region Calcula os totais da NF

        /// <summary>
        /// Calcula os totais da NF, somando os totais dos produtos relacionados a ela
        /// </summary>
        public void UpdateTotalNf(GDASession sessao, uint idNf)
        {
            bool isImportacao = IsNotaFiscalImportacao(sessao, idNf);
            string where = " Where (n.tipoDocumento not in (" + (int)NotaFiscal.TipoDoc.EntradaTerceiros + "," + (int)NotaFiscal.TipoDoc.NotaCliente +
                ") or n.serie<>'U' or n.serie is null) and n.idNf=" + idNf;

            string sql = "update nota_fiscal n set ";
              
            // Calcula o valor total dos produtos, o Round deve ficar dentro da função Sum, para que não ocorram problema de R$0,01
            // do somatório dos produtos da nota com o que é exibido no DANFE
            sql += "TotalProd=Round((Select Sum(Round(pn.Total, 2)) From produtos_nf pn Where pn.idNf=n.IdNf), 2), ";

            // Calcula o IPI da nota
            sql += "ValorIpi=Round((Select Sum(Round(pn.valorIpi, 2)) From produtos_nf pn Where pn.idNf=n.IdNf), 2), ";

            /* Chamado 14370.
             * O valor da base de cálculo de ICMS estava ficando diferente em 2 centavos por causa do desconto aplicado na nota fiscal.
             * Nós retiramos o Round, no sql abaixo, e o colocamos no cálculo do produto da nota fiscal, dessa forma,
             * com e sem desconto o valor da base de cálculo do ICMS ficou correto. */
            // Calcula a BC do ICMS e valor do ICMS (o round do pn.valorIcms foi alterado de 4 para 2, para resolver o chamado 10338)
            // sql += "BcIcms=Round((Select Sum(Round(pn.BcIcms, 2)) From produtos_nf pn Where pn.valorIcms>0 And pn.idNf=n.IdNf), 2), " +
            sql += "BcIcms=(Select Sum(pn.BcIcms) From produtos_nf pn Where pn.valorIcms>0 And pn.idNf=n.IdNf), " +
                "ValorIcms=Round((Select Sum(Round(pn.valorIcms, 2)) From produtos_nf pn Where pn.idNf=n.IdNf), 2), ";

            // Calcula a BC do FCP e valor do FCP
            sql += "BcFcp=(Select Sum(pn.BcFcp) From produtos_nf pn Where pn.valorFcp>0 And pn.idNf=n.IdNf), " +
                "ValorFcp=Round((Select Sum(Round(pn.valorFcp, 2)) From produtos_nf pn Where pn.idNf=n.IdNf), 2), ";

            // Calcula o valor da BC do ICMS ST e valor do ICMS ST (o round do pn.ValorIcmsSt foi alterado de 4 para 2, para resolver o chamado 10338)
            sql += "BcIcmsSt=Round((Select Sum(Round(pn.BcIcmsSt, 2)) From produtos_nf pn Where pn.BcIcmsSt>0 And pn.idNf=n.IdNf), 2), " +
                "ValorIcmsSt=Round((Select Sum(Round(pn.ValorIcmsSt, 2)) From produtos_nf pn Where pn.idNf=n.IdNf), 2), ";

            // Calcula o valor da BC do FCP ST e valor do FCP ST
            sql += "BcFcpSt=Round((Select Sum(Round(pn.BcFcpSt, 2)) From produtos_nf pn Where pn.BcFcpSt>0 And pn.idNf=n.IdNf), 2), " +
                "ValorFcpSt=Round((Select Sum(Round(pn.ValorFcpSt, 2)) From produtos_nf pn Where pn.idNf=n.IdNf), 2), ";

            // Calcula o valores de PIS/Cofins
            sql += @"ValorPis=Round((select sum(Round(valorPis, 2)) from produtos_nf where valorPis>0 and idNf=n.idNf), 2),
                AliqPis=ValorPis/(select sum(Round(bcPis, 2)) from produtos_nf where valorPis>0 and idNf=n.idNf)*100, 
                ValorCofins=Round((select sum(Round(valorCofins, 2)) from produtos_nf where valorCofins>0 and idNf=n.idNf), 2),
                AliqCofins=ValorCofins/(select sum(Round(bcCofins, 2)) from produtos_nf where valorCofins>0 and idNf=n.idNf)*100, ";

            // Calcula valor da Nota, somando o FCP ST, frete, outras despesas, seguro, IPI, IPIDevolvido, ICMS ST e subtraindo o desconto
            sql += "TotalNota=Round((totalProd + valorFcpSt + valorFrete + outrasDespesas + valorSeguro + valorIcmsSt + valorIpiDevolvido + valorIpi " +
                (isImportacao ? " + valorIcms" : "") + ") - desconto, 2), ";

            // Calcula o total dos tributos conforme lei da transparência
            sql += "ValorTotalTrib=(Select Sum(valorTotalTrib) From produtos_nf pnf Where idNf=n.idNf), ";

            // Calcula o peso bruto/líquido
            sql += @"PesoBruto=COALESCE(PesoConteiner, 0) + ROUND((SELECT SUM(pn.Peso) FROM produtos_nf pn WHERE pn.IdNf=n.IdNf), 2), 
                PesoLiq=Round((Select Sum(pn.Peso) From produtos_nf pn Where pn.idNf=n.IdNf), 2) ";

            sql += where;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Finaliza Notas Fiscais de Entrada (terceiros)

        private void VerificaChapasVidro(GDASession session)
        {
            // Recupera os produtos que serão verificados
            string idsProd = GetValoresCampo(session, @"
                select idProd from produto p
                where situacao=" + (int)Situacao.Ativo + @" and exists (
                    select idSubgrupoProd from subgrupo_prod
                    where idSubgrupoProd=p.idSubgrupoProd 
                        and tipoSubgrupo IN (" + (int)TipoSubgrupoProd.ChapasVidro + "," + (int)TipoSubgrupoProd.ChapasVidroLaminado + @")
                )", "idProd");

            // Sai do método se não houver chapas de vidro
            if (String.IsNullOrEmpty(idsProd))
                return;

            // Lista de itens por tipo
            var lista = new Dictionary<string, byte>();

            // Adiciona os produtos à lista (se ainda não estiverem cadastrados)
            var adicionar = new Action<string, byte>((where, tipo) =>
            {
                // Recupera os códigos dos produtos
                var itens = ExecuteMultipleScalar<string>(session, String.Format(
                    "select codInterno from produto where idProd in ({0}) and {1}",
                    idsProd, where));

                // Adiciona à lista, se ainda não estiver cadastrado
                foreach (var item in itens)
                    if (!lista.ContainsKey(item))
                        lista.Add(item, tipo);
            });

            // Busca todos os produtos por tipo
            adicionar("coalesce(altura, 0)=0 and coalesce(largura, 0)=0 and coalesce(idProdBase, 0)=0", 0);
            adicionar("coalesce(altura, 0)=0 and coalesce(largura, 0)=0", 1);
            adicionar("coalesce(altura, 0)=0", 2);
            adicionar("coalesce(largura, 0)=0", 3);
            adicionar("coalesce(idProdBase, 0)=0", 4);

            // Se não houver produtos desconfigurados
            if (lista.Count == 0)
                return;

            // Lista de mensagens de erro
            var listaErros = new List<string>();

            // Recupera os produtos para a montagem das mensagens de erro
            var mensagem = new Action<string, byte>((msg, tipo) =>
            {
                // Recupera os códigos dos produtos com erro do tipo indicado
                var codigos = lista.Where(x => x.Value == tipo).Select(x => x.Key).ToArray();

                // Se não houver códigos, retorna a mensagem vazia
                if (codigos.Length == 0)
                    return;

                // Retorna a mensagem formatada
                listaErros.Add(String.Format("• {0}: {1}", msg, String.Join(", ", codigos)));
            });

            // Monta as mensagens
            mensagem("Produtos sem altura, largura e produto base configurados", 0);
            mensagem("Produtos sem altura e largura configuradas", 1);
            mensagem("Produtos sem alura configurada", 2);
            mensagem("Produtos sem largura configurada", 3);
            mensagem("Produtos sem produto base configurado", 4);

            // Exibe a exceção
            throw new Exception("Há produtos cujo cadastro das chapas está incorreto. Erros:\n" +
                String.Join("\n", listaErros.ToArray()));
        }

        static volatile object locker = new object();

        /// <summary>
        /// Finaliza uma nota fiscal de entrada (terceiros).
        /// </summary>
        /// <param name="idNf"></param>
        public void FinalizarNotaEntradaTerceiros(uint idNf)
        {
            lock (locker)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        NotaFiscal nf = NotaFiscalDAO.Instance.GetElement(transaction, idNf);
                        ProdutosNf[] lstProdNf = ProdutosNfDAO.Instance.GetByNf(transaction, idNf);

                        #region Valida dados da Nota Fiscal

                        // TODO: Fazer validações do tipo: CNPJ/Insc Est do transportador, 
                        // codigo NCM preenchido automaticamente entre outros.

                        if (nf.NumeroNFe == 0)
                            throw new Exception("Informe o número da nota fiscal.");

                        if (String.IsNullOrEmpty(nf.Modelo))
                            throw new Exception("Informe o modelo da nota fiscal.");

                        if (nf.IdNaturezaOperacao == 0)
                            throw new Exception("Selecione a natureza de operação da nota fiscal.");

                        if (nf.IdFornec.GetValueOrDefault(0) == 0 && nf.IdCliente.GetValueOrDefault() == 0)
                            throw new Exception("Informe o emitente.");

                        if (nf.IdCidade == 0)
                            throw new Exception("Informe o município de ocorrência da nota fiscal.");

                        if (nf.IdNaturezaOperacao == null)
                            throw new Exception("Informe a natureza de operação.");

                        /* Chamado 24454. */
                        if (nf.Situacao == (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros)
                            throw new Exception("Esta nota já foi finalizada.");

                        /* Chamado 40463. */
                        if (lstProdNf.Any(f => f.IdNaturezaOperacao.GetValueOrDefault() == 0))
                            throw new Exception("Informe a natureza de operação de todos os produtos da nota fiscal.");

                        #region Impede a finalização da NFe caso o CSOSN ou CST não tenham sido informados
                        
                        var crt = LojaDAO.Instance.BuscaCrtLoja(transaction, nf.IdLoja.GetValueOrDefault());
                        var cst = false;
                        var csosn = false;

                        var lojaSimplesNacional = crt == (int)CrtLoja.SimplesNacional || crt == (int)CrtLoja.SimplesNacionalExcSub;

                        // Verifica se os produtos devem possuir CST ou CSOSN.
                        if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.EntradaTerceiros &&
                            FornecedorDAO.Instance.BuscaRegimeFornec(transaction, idNf) == (int)RegimeFornecedor.SimplesNacional)
                            //csosn = true;
                            if (lojaSimplesNacional)
                                /* Chamado 49020 */
                                csosn = true;
                            else
                                /* Chamado 43993 */
                                cst = true;
                        else if ((nf.TipoDocumento != (int)NotaFiscal.TipoDoc.EntradaTerceiros && (crt == (int)CrtLoja.LucroPresumido || crt == (int)CrtLoja.LucroReal)) ||
                            nf.TipoDocumento == (int)NotaFiscal.TipoDoc.EntradaTerceiros)
                            cst = true;
                        else
                            csosn = true;

                        /* Chamado 43437. */
                        if (lstProdNf.Any(f => (csosn && string.IsNullOrEmpty(f.Csosn)) || (cst && string.IsNullOrEmpty(f.Cst))))
                            throw new Exception(string.Format("Informe o {0} de todos os produtos da nota fiscal.", cst ? "CST" : "CSOSN"));

                        #endregion

                        // Só valida o CFOP se for nota de entrada (terceiros)
                        if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.EntradaTerceiros)
                        {
                            // Verifica se o destinatário foi informado.
                            if (nf.IdLoja.GetValueOrDefault() == 0)
                                throw new Exception("Informe o destinatário da nota fiscal.");

                            // Verifica se o CFOP selecionado é de nota fiscal de saída
                            if (!CfopDAO.Instance.IsCfopEntrada(transaction, (int)nf.IdCfop.Value))
                                throw new Exception("O CFOP informado na nota não é um CFOP de entrada.");

                            foreach (ProdutosNf pnf in lstProdNf)
                                if (pnf.IdNaturezaOperacao > 0 && !CfopDAO.Instance.IsCfopEntrada(transaction, (int)pnf.IdCfop.Value))
                                    throw new Exception("O CFOP informado no produto " + pnf.DescrProduto + " não é um CFOP de entrada.");

                            //Verifica se o cfop selecionado corresponde a uf do emitente e destinatario
                            var codCfop = Glass.Conversoes.StrParaInt(CfopDAO.Instance.ObtemCodInterno(transaction, nf.IdCfop.Value)[0].ToString());
                            var ufFornec = CidadeDAO.Instance.GetNomeUf(transaction, nf.IdFornec > 0 ? FornecedorDAO.Instance.ObtemIdCidade(transaction, (int)nf.IdFornec.Value) :
                                ClienteDAO.Instance.ObtemIdCidade(transaction, nf.IdCliente.Value));
                            var ufLoja = CidadeDAO.Instance.GetNomeUf(transaction, LojaDAO.Instance.ObtemValorCampo<uint>(transaction, "idCidade", "idLoja=" + nf.IdLoja));

                            if (ufFornec.ToLower() == ufLoja.ToLower() && codCfop != 1)
                                throw new Exception("O CFOP informado não corresponde a um CFOP de entrada dentro do estado.");
                            else if (ufFornec.ToLower() != ufLoja.ToLower() && codCfop != 2)
                                throw new Exception("O CFOP informado não corresponde a um CFOP de entrada fora do estado.");
                            else if (ufFornec.ToLower() == "ex" && codCfop != 3)
                                throw new Exception("O CFOP informado não corresponde a um CFOP de entrada fora do país.");

                            if (nf.IdFornec.GetValueOrDefault() == 0 && nf.GerarContasPagar)
                                throw new Exception("Não é possível gerar contas a pagar para cliente.");

                            if (CompraNotaFiscalDAO.Instance.PossuiCompra(transaction, (int)idNf) && nf.GerarEstoqueReal)
                                throw new Exception("Esta nota está marcada para gerar estoque real, no entanto ela veio de uma ou mais compras que já geraram o estoque das peças, desmarque esta opção para conseguir finalizá-la.");
                        }

                        // Verifica se o cliente controla estoque
                        else if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.NotaCliente)
                        {
                            if (!CfopDAO.Instance.ObtemValorCampo<bool>(transaction, "alterarEstoqueCliente", "idCfop=" + nf.IdCfop))
                                throw new Exception("O CFOP informado na nota não altera estoque de clientes.");

                            foreach (ProdutosNf pnf in lstProdNf)
                                if (pnf.IdNaturezaOperacao > 0 && !CfopDAO.Instance.ObtemValorCampo<bool>(transaction, "alterarEstoqueCliente", "idCfop=" + pnf.IdCfop.Value))
                                    throw new Exception("O CFOP informado no produto " + pnf.DescrProduto + " não altera estoque de clientes.");

                            if (!ClienteDAO.Instance.ObtemValorCampo<bool>(transaction, "controlarEstoqueVidros", "id_Cli=" + nf.IdCliente))
                                throw new Exception("O cliente desta nota fiscal deve controlar estoque de vidros.");

                            if (nf.IdLoja == null)
                                throw new Exception("A loja onde deve ser creditado o estoque deve ser informada.");
                            else if (LojaDAO.Instance.ObtemValorCampo<int>(transaction, "situacao", "idLoja=" + nf.IdLoja) == (int)Situacao.Inativo)
                                throw new Exception("A loja desta nota fiscal está inativa.");
                        }

                        // Foi necessário desabilitar este controle de validação do CNPJ com chave de acesso pois várias empresas não podiam ter esse bloqueio
                        if (!string.IsNullOrEmpty(nf.ChaveAcesso))
                        {
                            if (nf.ChaveAcesso.Length != 44)
                                throw new Exception("A chave de acesso deve conter 44 caracteres.");

                            var cnpjChaveAcesso = nf.ChaveAcesso.Substring(6, 14);

                            if (nf.CnpjEmitente == null)
                                throw new Exception("O CNPJ do Emitente está vazio.");

                            /*if (Formatacoes.LimpaCpfCnpj(nf.CnpjEmitente) != cnpjChaveAcesso)
                                throw new Exception("O CNPJ do Emitente não é o mesmo informado na chave de acesso."*/
                        }

                        // Verifica se a data de saída/entrada foi informada
                        if (nf.DataSaidaEnt == null)
                            throw new Exception("Informe a data de entrada desta nota fiscal.");

                        // Verifica se já foi inserida uma nota com a mesma numeração e fornecedor desta
                        if (objPersistence.ExecuteSqlQueryCount(transaction,
                            string.Format("SELECT COUNT(*) FROM nota_fiscal WHERE {0} AND NumeroNfe={1} AND Serie=?serie AND TipoDocumento={2} AND Situacao <> {3}",
                            nf.IdFornec > 0 ? "IdFornec=" + nf.IdFornec : "IdCliente=" + nf.IdCliente, nf.NumeroNFe, (int)NotaFiscal.TipoDoc.EntradaTerceiros,
                            /* Chamado 49937. */
                            (int)NotaFiscal.SituacaoEnum.Cancelada), new GDAParameter("?serie", nf.Serie)) > 1)
                            throw new Exception("Já existe uma nota de entrada para este fornecedor com esta numeração e série.");

                        if (nf.FormaPagto == 12 && !nf.GerarContasPagar)
                            throw new Exception("Para usar a forma de pagamento antecipação o campo 'Gerar contas a pagar' deve estar marcado.");

                        // Se as parcelas tiverem sido configuradas manualmente
                        // Se a forma de pagamento for à Prazo, ou se for outros e não houver pagamento em dinheiro, e sem pagamento.
                        if (nf.FormaPagto == 2 || (nf.FormaPagto == 3 && 
                            !PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)idNf).Any(p => p.FormaPagto == (int)FormaPagtoNotaFiscalEnum.Dinheiro || p.FormaPagto == (int)FormaPagtoNotaFiscalEnum.SemPagamento)))
                        {
                            if(nf.DatasParcelas.Distinct().Count() < nf.NumParc)
                                throw new Exception("As parcelas precisam ter datas distintas.");

                            if (nf.NumParc <= FiscalConfig.NotaFiscalConfig.NumeroParcelasNFe)
                            {
                                // Verifica se as parcelas já estão definidas
                                decimal valorParcelas = 0;
                                for (int i = 0; i < nf.NumParc.Value; i++)
                                {
                                    if (nf.DatasParcelas.Length <= i)
                                        throw new Exception("Informe as parcelas para continuar.");

                                    if (nf.DatasParcelas[i].Ticks == 0)
                                        throw new Exception("Data das parcelas inválida.");

                                    if (nf.ValoresParcelas[i] == 0)
                                        throw new Exception("Valor das parcelas inválido.");

                                    valorParcelas += nf.ValoresParcelas[i];
                                }

                                if ((nf.TotalManual == 0 && Math.Round(valorParcelas, 2) != Math.Round(nf.TotalNota, 2)) ||
                                    (nf.TotalManual > 0 && Math.Round(valorParcelas, 2) != Math.Round(nf.TotalManual, 2)))
                                    throw new Exception("Valor das parcelas não confere com o valor da nota.");
                            }
                            else // Se as parcelas tiverem sido configuradas automaticamente
                            {
                                if (nf.NumParc == null)
                                    throw new Exception("Informe o número de parcelas.");

                                if (nf.DataBaseVenc == null)
                                    throw new Exception("Informe a data base de vencimento das parcelas.");

                                if (nf.ValorParc == 0)
                                    throw new Exception("Informe o valor das parcelas.");

                                if (Math.Round((nf.ValorParc * nf.NumParc.Value), 2) != Math.Round(nf.TotalNota, 2) &&
                                    Math.Round((nf.ValorParc * nf.NumParc.Value), 2) != Math.Round(nf.TotalManual, 2))
                                    throw new Exception("Valor das parcelas não confere com o valor da nota.");
                            }
                        }

                        #endregion

                        // Padroniza a NF-e de cliente
                        if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.NotaCliente)
                        {
                            nf.GerarContasPagar = false;
                            nf.GerarEstoqueReal = true;
                        }

                        bool separouValores = false;

                        // Verifica se há alguma chapa de vidro que não esteja configurada
                        VerificaChapasVidro(transaction);

                        // Separaçao de valores
                        // Chamado 12770. Separa os valores fiscais e reais da nota fiscal somente se a mesma tenha sido gerado a partir de uma compra.
                        if (nf.GerarContasPagar && FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar && !String.IsNullOrEmpty(nf.IdCompras))
                        {
                            if (nf.IdConta == null)
                                throw new Exception("Selecione o plano de contas.");

                            if (ExecuteScalar<int>(transaction, "Select Count(*) From contas_pagar Where idNf=" + nf.IdNf) > 0)
                                throw new Exception("Já foram geradas contas a pagar para esta nota fiscal.");

                            try
                            {
                                separouValores = new SeparacaoValoresFiscaisEReaisContasPagar().Separar(transaction, idNf);
                            }
                            catch (Exception ex)
                            {
                                ErroDAO.Instance.InserirFromException("Separação de valores da NFe " + nf.IdNf + ": pagamento", ex);

                                // Caso esteja tudo ok para fazer a separação, o erro que ocorrer será erro de programação e portanto o processo terá que terminar neste ponto
                                if (FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar && CompraNotaFiscalDAO.Instance.PodeSepararContasPagarFiscaisEReais(transaction, (int)idNf))
                                    throw ex;
                            }
                        }

                        // Gera as contas à pagar
                        if (!separouValores && nf.GerarContasPagar && !nf.ExibirComprasVisible)
                        {
                            if (nf.IdConta == null)
                                throw new Exception("Selecione o plano de contas.");

                            if (ExecuteScalar<int>(transaction, string.Format("Select Count(*) From contas_pagar Where idNf={0} AND Paga IS NOT NULL AND Paga=1", nf.IdNf)) > 0)
                                throw new Exception("Já foram geradas contas a pagar para esta nota fiscal.");
                            /* Chamado 32846. */
                            else
                                objPersistence.ExecuteCommand(transaction, string.Format("DELETE FROM contas_pagar WHERE IdNf={0}", nf.IdNf));

                            //Pagamento á vista
                            if (nf.FormaPagto == 1)
                            {
                                ContasPagar nova = new ContasPagar();
                                nova.IdNf = idNf;
                                nova.IdConta = nf.IdConta.Value;
                                nova.IdFornec = nf.IdFornec.Value;
                                nova.IdLoja = nf.IdLoja.Value;
                                nova.DataVenc = DateTime.Now;
                                nova.ValorVenc = nf.TotalNota;
                                nova.Contabil = true;
                                // Busca o IdFormaPagto corresponde à primeira forma de pagamento da nota.
                                nova.IdFormaPagto = (uint)PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)nf.IdNf).First().IdFormaPagtoCorrespondente;

                                ContasPagarDAO.Instance.Insert(transaction, nova);
                            }
                            //Pagamento á prazo e outros
                            else if (nf.FormaPagto == 2 || nf.FormaPagto == 3)
                            {
                                // Se as parcelas tiverem sido configuradas manualmente
                                if (nf.NumParc <= FiscalConfig.NotaFiscalConfig.NumeroParcelasNFe)
                                {
                                    // Calcula o valor das parcelas
                                    decimal valorParcelas = 0;
                                    for (int i = 0; i < nf.NumParc.Value; i++)
                                        valorParcelas += nf.ValoresParcelas[i];

                                    for (int i = 0; i < nf.NumParc.Value; i++)
                                    {
                                        ContasPagar nova = new ContasPagar();
                                        nova.IdNf = idNf;
                                        nova.IdConta = nf.IdConta.Value;
                                        nova.IdFornec = nf.IdFornec.Value;
                                        nova.IdLoja = nf.IdLoja.Value;
                                        nova.DataVenc = nf.DatasParcelas[i];
                                        nova.ValorVenc = nf.ValoresParcelas[i];
                                        nova.NumBoleto = nf.BoletosParcelas[i];
                                        nova.Contabil = true;
                                        // Busca o IdFormaPagto corresponde à primeira forma de pagamento da nota.
                                        nova.IdFormaPagto = (uint)PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)nf.IdNf).First().IdFormaPagtoCorrespondente;

                                        ContasPagarDAO.Instance.Insert(transaction, nova);
                                    }
                                }
                                else // Se as parcelas tiverem sido configuradas automaticamente
                                {
                                    DateTime dataBase = nf.DataBaseVenc.Value;

                                    for (int i = 0; i < nf.NumParc.Value; i++)
                                    {
                                        ContasPagar nova = new ContasPagar();
                                        nova.IdNf = idNf;
                                        nova.IdConta = nf.IdConta.Value;
                                        nova.IdFornec = nf.IdFornec.Value;
                                        nova.IdLoja = nf.IdLoja.Value;
                                        nova.DataVenc = dataBase;
                                        nova.ValorVenc = nf.ValorParc;
                                        nova.NumBoleto = null;
                                        nova.Contabil = true;
                                        // Busca o IdFormaPagto corresponde à primeira forma de pagamento da nota.
                                        nova.IdFormaPagto = (uint)PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)nf.IdNf).First().IdFormaPagtoCorrespondente;

                                        ContasPagarDAO.Instance.Insert(transaction, nova);

                                        dataBase = dataBase.AddMonths(1);
                                    }
                                }
                            }
                            //pagamento antecipação
                            else
                            {
                                if (!nf.IdAntecipFornec.HasValue)
                                    throw new Exception("Informe o número da antecipação.");

                                uint idFornec = AntecipacaoFornecedorDAO.Instance.GetIdFornec(transaction, (int)nf.IdAntecipFornec.GetValueOrDefault());

                                if (nf.IdFornec != idFornec)
                                    throw new Exception("O fornecedor da antecipação deve ser o mesmo da nota fiscal.");

                                decimal saldoAntecip = AntecipacaoFornecedorDAO.Instance.GetSaldo(transaction, nf.IdAntecipFornec.GetValueOrDefault());

                                decimal valorRestante = nf.TotalNota - saldoAntecip;

                                //Se não houver saldo sufuciente na antecipação gera conta a pagar
                                if (valorRestante > 0)
                                {
                                    ContasPagar nova = new ContasPagar();
                                    nova.IdNf = idNf;
                                    nova.IdConta = nf.IdConta.Value;
                                    nova.IdFornec = nf.IdFornec.Value;
                                    nova.IdLoja = nf.IdLoja.Value;
                                    nova.DataVenc = DateTime.Now;
                                    nova.ValorVenc = valorRestante;
                                    nova.Contabil = true;
                                    // Busca o IdFormaPagto corresponde à primeira forma de pagamento da nota.
                                    nova.IdFormaPagto = (uint)PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)nf.IdNf).First().IdFormaPagtoCorrespondente;

                                    ContasPagarDAO.Instance.Insert(transaction, nova);
                                }

                                // Altera a situação da nota
                                NotaFiscalDAO.Instance.AlteraSituacao(transaction, idNf, NotaFiscal.SituacaoEnum.FinalizadaTerceiros);

                                //Atualiza o saldo da antecipação
                                AntecipacaoFornecedorDAO.Instance.AtualizaSaldo(transaction, nf.IdAntecipFornec.GetValueOrDefault());
                            }
                        }

                        // Atualiza os números das parcelas da NF
                        ContasPagarDAO.Instance.AtualizaNumParcNf(transaction, idNf);

                        var mensagemLog = string.Empty;

                        if (!nf.GerarEstoqueReal)
                            mensagemLog += "Nota não está configurada para geração de estoque Real. ";
                        if (EstoqueConfig.EntradaEstoqueManual)
                            mensagemLog += "Nota não gerou Estoque real pois a Configuração(Entrada de Estoque Manual) Está marcada. ";

                        if (!string.IsNullOrEmpty(mensagemLog))
                        {
                            var logMovNotaFiscal = new LogMovimentacaoNotaFiscal();
                            logMovNotaFiscal.IdNf = nf.IdNf;
                            logMovNotaFiscal.MensagemLog = mensagemLog;
                            logMovNotaFiscal.DataCad = DateTime.Now;
                            logMovNotaFiscal.Usucad = UserInfo.GetUserInfo.CodUser;
                            LogMovimentacaoNotaFiscalDAO.Instance.Insert(transaction, logMovNotaFiscal);
                        }

                        foreach (ProdutosNf p in lstProdNf)
                        {
                            if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.EntradaTerceiros)
                            {

                                var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd((int)p.IdProd);
                                var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)p.IdProd);

                                mensagemLog = string.Empty;

                                // Altera o estoque somente se estiver marcado para alterar no cadastro de subgrupo, no cadastro de CFOP e 
                                // se o tipo de ambiente da NFe estiver em produção
                                if (Glass.Data.DAL.GrupoProdDAO.Instance.NaoAlterarEstoqueFiscal(idGrupoProd, idSubgrupoProd))
                                    mensagemLog += "Grupo/Subgrupo do produto está configurado para não gerar estoque fiscal. ";
                                if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
                                    mensagemLog += "O tipo de ambiente da nota está configurado como Homologação, o que impede a geração estoque fiscal. ";
                                if ((nf.IdNaturezaOperacao != null && !NaturezaOperacaoDAO.Instance.AlterarEstoqueFiscal(nf.IdNaturezaOperacao.Value)))
                                    mensagemLog += "A natureza de operação da nota está configurada para não gerar estoque fiscal. ";

                                if (!string.IsNullOrEmpty(mensagemLog))
                                {
                                    var logMovNotaFiscal = new LogMovimentacaoNotaFiscal();
                                    logMovNotaFiscal.IdNf = nf.IdNf;
                                    logMovNotaFiscal.IdProdNf = p.IdProdNf;
                                    logMovNotaFiscal.MensagemLog = mensagemLog;
                                    logMovNotaFiscal.DataCad = DateTime.Now;
                                    logMovNotaFiscal.Usucad = UserInfo.GetUserInfo.CodUser;
                                    LogMovimentacaoNotaFiscalDAO.Instance.Insert(transaction, logMovNotaFiscal);
                                }

                                // Credita o estoque fiscal
                                MovEstoqueFiscalDAO.Instance.CreditaEstoqueNotaFiscal(transaction, p.IdProd, nf.IdLoja.Value,
                                    p.IdNaturezaOperacao > 0 ? p.IdNaturezaOperacao.Value : nf.IdNaturezaOperacao.Value, p.IdNf, p.IdProdNf,
                                    (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(transaction, p, true), false, false);
                            }
                            else if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.NotaCliente)
                            {
                                if(!EstoqueConfig.ControlarEstoqueVidrosClientes)
                                {
                                    var logMovNotaFiscal = new LogMovimentacaoNotaFiscal();
                                    logMovNotaFiscal.IdNf = nf.IdNf;
                                    logMovNotaFiscal.IdProdNf = p.IdProdNf;
                                    logMovNotaFiscal.MensagemLog = "O sistema não está configurado para controlar estoque de clientes.";
                                    logMovNotaFiscal.DataCad = DateTime.Now;
                                    logMovNotaFiscal.Usucad = UserInfo.GetUserInfo.CodUser;
                                    LogMovimentacaoNotaFiscalDAO.Instance.Insert(transaction, logMovNotaFiscal);
                                }

                                // Credita o estoque do cliente
                                MovEstoqueClienteDAO.Instance.CreditaEstoqueNotaFiscal(transaction, nf.IdCliente.Value, p.IdProd,
                                    nf.IdLoja.Value, p.IdNaturezaOperacao > 0 ? p.IdNaturezaOperacao.Value : nf.IdNaturezaOperacao.Value,
                                    p.IdNf, p.IdProdNf, (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(transaction, p, true));
                            }

                            // Credita o estoque real
                            if (nf.GerarEstoqueReal && !EstoqueConfig.EntradaEstoqueManual)
                            {
                                if(!MovEstoqueDAO.Instance.AlteraEstoque(transaction, p.IdProd))
                                {
                                    var logMovNotaFiscal = new LogMovimentacaoNotaFiscal();
                                    logMovNotaFiscal.IdNf = nf.IdNf;
                                    logMovNotaFiscal.IdProdNf = p.IdProdNf;
                                    logMovNotaFiscal.MensagemLog = "Grupo/Subgrupo do produto está configurado para não gerar estoque real. ";
                                    logMovNotaFiscal.DataCad = DateTime.Now;
                                    logMovNotaFiscal.Usucad = UserInfo.GetUserInfo.CodUser;
                                    LogMovimentacaoNotaFiscalDAO.Instance.Insert(transaction, logMovNotaFiscal);
                                }

                                bool m2 = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(transaction, (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                                    Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(transaction, (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                                MovEstoqueDAO.Instance.CreditaEstoqueNotaFiscal(transaction, p.IdProd, nf.IdLoja.Value, idNf, p.IdProdNf,
                                    (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(transaction, p.IdProd, p.TotM, p.Qtde, p.Altura, p.Largura, false, false));

                                objPersistence.ExecuteCommand(transaction, "update produtos_nf set qtdeEntrada=" + ProdutosNfDAO.Instance.ObtemQtdDanfe(transaction, p).ToString().Replace(",", ".") +
                                    " where idProdNf=" + p.IdProdNf);
                            }

                            //Altera o estoque da materia-prima
                            MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaNotaFiscal(transaction, (int)p.IdProd, (int)p.IdNf, (int)p.IdProdNf, (decimal)p.TotM, MovEstoque.TipoMovEnum.Entrada);
                        }

                        // Altera a situação da nota
                        AlteraSituacao(transaction, idNf, NotaFiscal.SituacaoEnum.FinalizadaTerceiros);

                        LogNfDAO.Instance.NewLog(nf.IdNf, "Nota Finalizada", 0, "Nota Finalizada");

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException("FinalizarNF", ex);

                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Reabre uma nota fiscal de entrada (terceiros).
        /// </summary>
        public void ReabrirNotaEntradaTerceiros(uint idNf)
        {
            lock (locker)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        // Verifica se a impressoes de etiqueta
                        if (ProdutoImpressaoDAO.Instance.NfPossuiPecaImpressa(transaction, (int)idNf))
                            throw new Exception("Existe pelo menos uma impressão de etiqueta gerada por essa nota fiscal.");

                        /* Chamado 64491. */
                        if (EntradaEstoqueDAO.Instance.VerificarNotaFiscalPossuiEntradaEstoqueAtiva(transaction, (int)idNf))
                            throw new Exception("Não é possível reabrir essa NFe, pois ela possui um ou mais entradas de estoque manuais. Cancele as entradas de estoque para reabrir a nota.");

                        // Verifica se ao reabrir esta nota o estoque ficará negativo
                        ProdutosNf[] lstProdNf = ProdutosNfDAO.Instance.GetByNfExtended(transaction, idNf);
                        ProdutosNf[] lstProdNfValida = GetSubstProdutosProjeto(transaction, lstProdNf, true);
                        foreach (ProdutosNf pnf in lstProdNfValida)
                            VerificaEstoque(transaction, pnf, lstProdNfValida, ObtemIdLoja(transaction, idNf));

                        var separarValores = false;

                        /* Chamado 42645. */
                        try
                        {
                            // Verifica se o cancelamento de valores pode ser feito
                            new SeparacaoValoresFiscaisEReaisContasPagar().ValidaCancelamento(transaction, idNf);
                            separarValores = true;
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.Contains("Não existem parcelas a serem restauradas."))
                                throw ex;
                        }

                        // Só verifica o pagamento das contas a pagar se não houver cancelamento
                        // de separação de valores, uma vez que essa operação envolve a restauração
                        // das contas a pagar originais e só é feita se não houver contas pagas
                        if (!ParcelaNaoFiscalOriginalDAO.Instance.NfTemParcela(transaction, idNf))
                        {
                            // Verifica se há contas pagas
                            if (ContasPagarDAO.Instance.ExistePagasNf(transaction, idNf))
                                throw new Exception("Já existe pelo menos uma conta paga gerada por essa nota fiscal.");

                            // Exclui as contas a pagar (sessão não funciona neste método chamado 23674)
                            ContasPagarDAO.Instance.DeleteByNf(transaction, idNf);
                        }
                        var tipoDoc = GetTipoDocumento(transaction, idNf);

                        // Exclui as movimentações de estoque
                        if (tipoDoc == (int)NotaFiscal.TipoDoc.EntradaTerceiros)
                        {
                            var idNaturezaOperacao = NotaFiscalDAO.Instance.ObtemIdNaturezaOperacao(transaction, idNf);
                            var idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(transaction, idNf);

                            // Chamado 23947. A quantidade de estoque em posse de terceiros não estava sendo estornada ao reabrir a nota fiscal.
                            foreach (ProdutosNf pnf in lstProdNfValida)
                                if (idNaturezaOperacao > 0 &&
                                    CfopDAO.Instance.AlterarEstoqueTerceiros(transaction, NaturezaOperacaoDAO.Instance.ObtemIdCfop(transaction, idNaturezaOperacao)))
                                {
                                    MovEstoqueFiscalDAO.Instance.BaixaEstoqueNotaFiscal(transaction, pnf.IdProd, idLoja, idNaturezaOperacao, pnf.IdNf, pnf.IdProdNf,
                                        (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(transaction, pnf, true), false);
                                }

                            MovEstoqueFiscalDAO.Instance.DeleteByNf(transaction, idNf);
                        }
                        else if (tipoDoc == (int)NotaFiscal.TipoDoc.NotaCliente)
                            MovEstoqueClienteDAO.Instance.DeleteByNf(transaction, idNf);

                        if (ObtemValorCampo<bool>(transaction, "gerarEstoqueReal", "idNf=" + idNf) || !EstoqueConfig.EntradaEstoqueManual)
                        {
                            var movsEstoque = MovEstoqueDAO.Instance.GetListByNf(transaction, idNf);

                            foreach (var m in movsEstoque)
                            {
                                LogCancelamentoDAO.Instance.LogMovEstoque(transaction, m, "Reabertura de NF-e", false);
                            }

                            MovEstoqueDAO.Instance.DeleteByNf(transaction, idNf);
                        }

                        foreach (var p in ProdutosNfDAO.Instance.GetByNf(idNf))
                        {
                            //Altera o estoque da materia-prima
                            MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaNotaFiscal(transaction, (int)p.IdProd, (int)idNf, (int)p.IdProdNf, (decimal)p.TotM, MovEstoque.TipoMovEnum.Saida);
                        }

                        // Altera a situação da nota para Aberta
                        NotaFiscalDAO.Instance.AlteraSituacao(transaction, idNf, NotaFiscal.SituacaoEnum.Aberta);

                        int formaPagto = ObtemValorCampo<int>(transaction, "formaPagto", "idNf=" + idNf);

                        // Se foi paga com antecipação, atualiza o saldo
                        if (formaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.AntecipFornec)
                        {
                            uint idAntecipFornec = ObtemValorCampo<uint>(transaction, "idAntecipFornec", "idNf=" + idNf);
                            AntecipacaoFornecedorDAO.Instance.AtualizaSaldo(transaction, idAntecipFornec);
                        }

                        if (separarValores)
                        {
                            // Faz o cancelamento da separação de valores.
                            new SeparacaoValoresFiscaisEReaisContasPagar().Cancelar(transaction, idNf);
                        }

                        LogMovimentacaoNotaFiscalDAO.Instance.DeleteFromNf(transaction, idNf);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Verifica se a nota informada deve gerar etiqueta
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool GerarEtiqueta(uint idNf)
        {
            return ObtemValorCampo<bool>("gerarEtiqueta", "idnf = " + idNf);
        }

        #endregion

        #region Notas Fiscais FS-DA

        /// <summary>
        /// Finaliza uma NF usando formulário de segurança FS-DA.
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public XmlDocument FinalizarNfFS(uint idNf)
        {
            if (ObtemValorCampo<uint>("numDocFsda", "idNf=" + idNf) == 0)
                throw new Exception("Preencha o campo com o número do documento do FS-DA.");

            objPersistence.ExecuteCommand("update nota_fiscal set formaEmissao=" +
                (int)NotaFiscal.TipoEmissao.ContingenciaFSDA + " where idNf=" + idNf);

            XmlDocument doc = GerarXmlNf(idNf, false);

            AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.NaoEmitida);

            return doc;
        }

        /// <summary>
        /// Envia para a SEFAZ uma NF emitida usando um formulário de segurança FS-DA.
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public string EmitirNfFS(uint idNf)
        {
            if (ObtemFormaEmissao(idNf) != (int)NotaFiscal.TipoEmissao.ContingenciaFSDA ||
                ObtemSituacao(idNf) != (int)NotaFiscal.SituacaoEnum.NaoEmitida)
                throw new Exception("NF-e inválida para emissão.");

            string fileName = Utils.GetNfeXmlPath + ObtemChaveAcesso(idNf) + "-nfe.xml";

            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            #region Envia NFe para SEFAZ e atualiza tipo de ambiente da NFe

            try
            {
                // Altera situação para processo de emissão
                AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.ProcessoEmissao);

                // Atualiza campo tipoAmbiente da NFe
                objPersistence.ExecuteCommand("Update nota_fiscal set tipoAmbiente=" + (int)ConfigNFe.TipoAmbiente + " where idNf=" + idNf);

                // Envia NFe para SEFAZ
                return EnviaXML.EnviaNFe(doc, idNf);
            }
            catch (Exception ex)
            {
                // Altera situação para falha ao emitir
                AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                throw ex;
            }

            #endregion
        }

        #endregion

        #region Verifica se a nota fiscal existe para um ou mais pedido

        public bool ExistsByPedido(GDASession session, uint idPedido)
        {
            return ExistsByPedidos(session, idPedido.ToString());
        }

        public bool ExistsByPedidos(GDASession session, string idsPedidos)
        {
            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(session, "Select Count(*) From pedidos_nota_fiscal pnf " +
                "left join nota_fiscal nf on (pnf.idNf=nf.idNf) Where pnf.idPedido in (" + idsPedidos + ") " +
                "and nf.situacao in (" + (int)NotaFiscal.SituacaoEnum.Aberta + "," + (int)NotaFiscal.SituacaoEnum.Autorizada + "," +
                (int)NotaFiscal.SituacaoEnum.ProcessoEmissao + ")").ToString()) > 0;
        }

        #endregion

        #region Recupera o número de uma nota fiscal de entrada (terceiros) ou importação

        /// <summary>
        /// Recupera o número de uma nota fiscal de entrada (terceiros) ou importação.
        /// </summary>
        public uint GetIdByNumeroEntradaTerc(uint numeroNFe, uint? idFornec)
        {
            // Chamado 49781 - O Order By foi adicionado para sempre buscar a nota fiscal mais recente caso exista duas notas com série diferente.
            var idNf = ObtemValorCampo<uint>("idNf", string.Format("NumeroNFe={0} AND TipoDocumento IN ({1},{2},{3}) ORDER BY IdNf DESC", numeroNFe, (int)NotaFiscal.TipoDoc.EntradaTerceiros,
                (int)NotaFiscal.TipoDoc.NotaCliente, (int)NotaFiscal.TipoDoc.Entrada, idFornec > 0 ? string.Format(" AND IdFornec={0}", idFornec) : string.Empty));

            if (idFornec.GetValueOrDefault() == 0 && !IsNotaFiscalImportacao(idNf))
                idNf = 0;

            return idNf;
        }

        #endregion

        #region Verifica se a nota fiscal está autorizada

        /// <summary>
        /// Verifica se a nota fiscal está autorizada.
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool IsAutorizada(uint idNf)
        {
            string sql = "select count(*) from nota_fiscal where idNf=" + idNf + " and situacao=" + (int)NotaFiscal.SituacaoEnum.Autorizada;
            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Verifica se a nota fiscal está finalizada

        /// <summary>
        /// Verifica se a nota fiscal está finalizada.
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool IsFinalizada(GDASession sessao, uint idNf)
        {
            string sql = "select count(*) from nota_fiscal where idNf=" + idNf + " and situacao=" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros;
            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Verifica se existe mais de uma nota fiscal de saída no sistema

        /// <summary>
        /// Verifica se existe alguma nota fiscal de saída no sistema que não seja a passada por parâmetro
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool ExisteNotaSaida(GDASession sessao, uint idNf)
        {
            string sql = "Select Count(*) From nota_fiscal Where idNf<>" + idNf + " And tipoDocumento=" + (int)NotaFiscal.TipoDoc.Saída;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Verifica se existe uma carta de correção registrada para a nota fiscal informada

        /// <summary>
        /// Verifica se existe uma carta de correção registrada para a nota fiscal informada
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool ExisteCartaCorrecaoRegistrada(GDASession sessao, uint idNf)
        {
            string sql = "SELECT COUNT(*) FROM carta_correcao WHERE situacao=" + (int)CartaCorrecao.SituacaoEnum.Registrada
                + " AND idNf=" + idNf;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Verifica se a nota fiscal é de importação

        private string SqlIsImportacao(string idNf)
        {
            return "select count(*) from nota_fiscal nf where tipoDocumento=" + (int)NotaFiscal.TipoDoc.Entrada + @"
                and (select count(*) from fornecedor f inner join cidade c on (f.idCidade=c.idCidade) where c.codIbgeCidade='99999' 
                and f.idFornec=nf.idFornec)>0 and idNf=" + idNf;
        }

        public bool IsNotaFiscalImportacao(uint idNf)
        {
            return IsNotaFiscalImportacao(null, idNf);
        }

        /// <summary>
        /// Verifica se a nota fiscal é de importação.
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool IsNotaFiscalImportacao(GDASession sessao, uint idNf)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, SqlIsImportacao(idNf.ToString())) > 0;
        }

        #endregion

        #region Verifica se a nota fiscal é de exportação

        /// <summary>
        /// Verifica se a nota fiscal é de importação.
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool IsNotaFiscalExportacao(GDASession sessao, uint idNf)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM nota_fiscal nf
                    INNER JOIN cliente c ON (c.Id_cli = nf.IdCliente)
                    INNER JOIN cidade cid ON (c.IdCidade = cid.IdCidade)
                WHERE nf.TipoDocumento = {0} AND nf.IdNf = {1} AND cid.CodIbgeCidade = 99999";

            return objPersistence.ExecuteSqlQueryCount(sessao, string.Format(sql, (int)NotaFiscal.TipoDoc.Saída, idNf)) > 0;
        }

        #endregion

        #region Verifica se a nota pode ser editada

        /// <summary>
        /// Verifica se a nota fiscal pode ser editada
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool PodeEditar(uint idNf)
        {
            int situacao = ObtemSituacao(idNf);
            LoginUsuario login = UserInfo.GetUserInfo;
            int ultCodEvento = LogNfDAO.Instance.ObtemUltimoCodigo(idNf);

            // Códigos de retorno da receita que não pode permitir edição de NFe, pela possibilidade da mesma já estar autorizada
            List<int> lstCodNaoEditavel = new List<int>() { 100, 101, 102, 103, 104, 105, 108, 110, 204, 205, 206, 218, 219, 220, 221, 256, 420, 563 };

            bool flagSituacao = situacao == (int)NotaFiscal.SituacaoEnum.Aberta ||
                (situacao == (int)NotaFiscal.SituacaoEnum.FalhaEmitir && !lstCodNaoEditavel.Contains(ultCodEvento));

            bool flagManual = (situacao == (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros ||
                (situacao == (int)NotaFiscal.SituacaoEnum.Autorizada && NotaFiscalDAO.Instance.ExisteCartaCorrecaoRegistrada(null, idNf))) &&
                Config.PossuiPermissao(Config.FuncaoMenuFiscal.AlteracaoManualNFe);

            return (flagSituacao || flagManual);
        }

        #endregion

        #region Retorna o número de NF a emitir (FS-DA)

        /// <summary>
        /// Retorna o número de NF a emitir (FS-DA).
        /// </summary>
        /// <returns></returns>
        public int GetCountEmitirFs()
        {
            string sql = "select count(*) from nota_fiscal where situacao=" + (int)NotaFiscal.SituacaoEnum.NaoEmitida +
                " and formaEmissao=" + (int)NotaFiscal.TipoEmissao.ContingenciaFSDA;

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        #endregion

        #region Retorna o valor total das notas canceladas (Registro C380 EFD Cont.)

        /// <summary>
        /// Retorna o valor total das notas canceladas (Registro C380 EFD Cont.).
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fim"></param>
        /// <returns></returns>
        internal decimal GetTotalCancC380(DateTime inicio, DateTime fim)
        {
            string sql = @"select sum(totalNota) from nota_fiscal where dataEmissao>=?ini and dataEmissao<=?fim
                and situacao=" + (int)NotaFiscal.SituacaoEnum.Cancelada;

            return ExecuteScalar<decimal>(sql, new GDAParameter("?ini", inicio.Date),
                new GDAParameter("?fim", fim.Date.AddDays(1).AddSeconds(-1)));
        }

        #endregion

        #region Obtém as notas fiscais abertas

        /// <summary>
        /// Obtém as notas fiscais abertas.
        /// </summary>
        public string ObtemAbertas(GDASession session, string idsNf)
        {
            string sql = "select idNf from nota_fiscal where idNf in (" + idsNf + @")
                and situacao=" + (int)NotaFiscal.SituacaoEnum.Aberta;

            return GetValoresCampo(session, sql, "idNf");
        }

        #endregion

        #region Remove o cliente ou fornecedor da nota fiscal

        /// <summary>
        /// Remove o cliente ou o fornecedor da nota fiscal, caso ambos tenham sido informados.
        /// A nota fiscal com o tipo 4 "Nota Fiscal de Cliente" não é verificada nesse método porque o fornecedor e o cliente devem ser informados nela.
        /// </summary>
        private void LimparEmitenteDestinatario(GDASession session, NotaFiscal notaFiscal)
        {
            if (notaFiscal.IdCfop > 0 && notaFiscal.IdCliente > 0 && notaFiscal.IdFornec > 0 && notaFiscal.TipoDocumento != 4)
            {
                // Verifica se o CFOP é de devolução.
                var cfopDevolucao = CfopDAO.Instance.IsCfopDevolucao(session, notaFiscal.IdCfop.Value);

                // Caso o cliente deva ser informado na nota fiscal, remove o fornecedor.
                if ((notaFiscal.TipoDocumento == 2 && !cfopDevolucao) ||
                    (notaFiscal.TipoDocumento == 1 && cfopDevolucao))
                    notaFiscal.IdFornec = null;
                // Caso o fornecedor deva ser informado na nota fiscal, remove o cliente.
                else if ((notaFiscal.TipoDocumento == 1 && !cfopDevolucao) ||
                    (notaFiscal.TipoDocumento == 2 && cfopDevolucao))
                    notaFiscal.IdCliente = null;
                // Os casos acima atendem condições para notas em que o fornecedor ou o cliente são destinatários.
                // Nos demais casos, o sistema irá exibir uma mensagem informando que estes campos não podem ser informados concomitantemente.
                else
                    throw new Exception("A nota fiscal não pode ser salva com informações do cliente e fornecedor. Somente um dos dois deve ser informado.");
            }
        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Insere a nota fiscal.
        /// </summary>
        public uint InserirComTransacao(NotaFiscal objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Verifica se foi emitida uma nota fiscal com os mesmos dados há menos de 30 segundos.
                    var idNfGerada = ObtemValorCampo<int>(transaction, "IdNf", string.Format("DataCad>=?data{0}{1}{2}",
                        objInsert.IdCliente > 0 ? string.Format(" AND IdCliente={0}", objInsert.IdCliente) : string.Empty,
                        objInsert.IdFornec > 0 ? string.Format(" AND IdFornec={0}", objInsert.IdFornec) : string.Empty,
                        objInsert.IdLoja > 0 ? string.Format(" AND IdLoja={0}", objInsert.IdLoja) : string.Empty),
                        new GDAParameter("?data", DateTime.Now.AddSeconds(-30)));

                    /* Chamado 63518. */
                    if (idNfGerada > 0)
                        throw new Exception("Foi gerada uma nota fiscal com os mesmos dados há poucos segundos. Aguarde um minuto e tente novamente.");

                    var retorno = Insert(transaction, objInsert);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        private static object syncRoot = new object();

        public uint InsertComTransacao(NotaFiscal objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Insert(transaction, objInsert);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override uint Insert(NotaFiscal objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession sessao, NotaFiscal objInsert)
        {
            lock (syncRoot)
            {
                LimparEmitenteDestinatario(sessao, objInsert);

                // Não permite inserir notas para clientes inativos
                if (!Configuracoes.FiscalConfig.NotaFiscalConfig.PermitirEmitirNotaParaClienteBloqueadoOuInativo &&
                    objInsert.IdCliente > 0 && ClienteDAO.Instance.GetSituacao(sessao, objInsert.IdCliente.Value) != (int)SituacaoCliente.Ativo)
                    throw new Exception("O cliente selecionado está inativado.");

                // Não permite inserir notas para fornecedores inativos
                if (objInsert.IdFornec > 0 && FornecedorDAO.Instance.GetSituacao(sessao, objInsert.IdFornec.Value) != (int)SituacaoFornecedor.Ativo)
                    throw new Exception("O fornecedor selecionado está inativado.");

                // Não permite inserir nota fiscal se a loja informada não existir
                if (objInsert.IdLoja.GetValueOrDefault() == 0 || (objInsert.IdLoja > 0 && !LojaDAO.Instance.Exists(sessao, objInsert.IdLoja)))
                    throw new Exception("A loja informada não existe.");

                /* Chamado 49733. */
                if (!Config.PossuiPermissao(Config.FuncaoMenuFiscal.InserirNotaFiscalParaQualquerLoja) &&
                    UserInfo.GetUserInfo.IdLoja != objInsert.IdLoja)
                    throw new Exception("Você não possui a permissão Cadastrar nota fiscal para qualquer loja, portanto, " +
                        "é permitido inserir nota fiscal somente para a loja associada ao seu cadastro.");

                objInsert.Situacao = (int)NotaFiscal.SituacaoEnum.Aberta;

                //Se for NFC-e(DANFE NFC-e) o tipo de impressão é diferente da NF(Retrato)
                objInsert.TipoImpressao = objInsert.Consumidor ? 4 : 1;

                // Se a forma de emissão não tiver sido especificada
                if (objInsert.FormaEmissao == 0)
                    objInsert.FormaEmissao = (int)NotaFiscal.TipoEmissao.Normal;

                // Se a finalidade de emissão não tiver sido especificada
                if (objInsert.FinalidadeEmissao == 0)
                {
                    var cfop = NaturezaOperacaoDAO.Instance.ObtemIdCfop(sessao, objInsert.IdNaturezaOperacao.GetValueOrDefault());
                    if (CfopDAO.Instance.IsCfopDevolucao(cfop))
                        objInsert.FinalidadeEmissao = (int)NotaFiscal.FinalidadeEmissaoEnum.Devolucao;
                    else
                        objInsert.FinalidadeEmissao = (int)NotaFiscal.FinalidadeEmissaoEnum.Normal;
                }

                // Caso o município de ocorrência não tenha sido informado busca direto do emitente da nota
                if (objInsert.IdCidade == 0)
                {
                    if (objInsert.TipoDocumento == (int)NotaFiscal.TipoDoc.Entrada || objInsert.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída)
                        objInsert.IdCidade = LojaDAO.Instance.ObtemValorCampo<uint>(sessao, "idCidade", "idLoja=" + objInsert.IdLoja);
                    else if (objInsert.TipoDocumento == (int)NotaFiscal.TipoDoc.EntradaTerceiros || objInsert.TipoDocumento == (int)NotaFiscal.TipoDoc.NotaCliente)
                    {
                        if (CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(sessao, objInsert.IdNaturezaOperacao.GetValueOrDefault())))
                            objInsert.IdCidade = ClienteDAO.Instance.ObtemValorCampo<uint>(sessao, "idCidade", "id_Cli=" + objInsert.IdCliente.GetValueOrDefault());
                        else
                            objInsert.IdCidade = FornecedorDAO.Instance.ObtemValorCampo<uint>(sessao, "idCidade", "idFornec=" + objInsert.IdFornec.GetValueOrDefault());
                    }
                }

                // Busca um número para esta NFe que ainda não foi utilizado
                if (objInsert.TipoDocumento != (int)NotaFiscal.TipoDoc.EntradaTerceiros && objInsert.TipoDocumento != (int)NotaFiscal.TipoDoc.NotaCliente)
                {
                    if (String.IsNullOrEmpty(objInsert.Serie) || objInsert.Serie == "0" || objInsert.Serie == "1")
                    {
                        var codCfop = CfopDAO.Instance.ObtemCodInterno(sessao, NaturezaOperacaoDAO.Instance.ObtemIdCfop(sessao, objInsert.IdNaturezaOperacao.GetValueOrDefault()));
                        var inscEstLoja = LojaDAO.Instance.ObtemValorCampo<string>(sessao, "inscEst", "idLoja=" + objInsert.IdLoja);

                        // Ao transferir a nota para outro BD, essa configuração deve ser igual nas duas empresas
                        objInsert.Serie = FiscalConfig.NotaFiscalConfig.SeriePadraoNFe(codCfop, inscEstLoja, objInsert.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste).ToString();
                    }

                    objInsert.Modelo = ConfigNFe.Modelo(objInsert.Consumidor);
                }

                // A observação da natureza de operação é buscada dinamicamente ao imprimir o DANFE e na tela de cadastro da nota fiscal,
                // ou seja, a observação não pode ser salva nas informações complementares da nota fiscal.
                // Busca observação da CFOP e salva nas informações complementares da nota
                if (objInsert.IdCfop != null)
                {
                    string obsCfop = CfopDAO.Instance.GetObs(sessao, objInsert.IdCfop.Value);
                    if (!String.IsNullOrEmpty(obsCfop))
                        objInsert.InfCompl = obsCfop + ". " + objInsert.InfCompl;
                }

                if (objInsert.NumParc == null || objInsert.NumParc == 0)
                    objInsert.NumParc = 1;

                // Verifica se a data de saída é menor que a data de emissão da nota
                if (objInsert.DataSaidaEnt != null && objInsert.DataSaidaEnt.Value == objInsert.DataEmissao)
                    objInsert.DataSaidaEnt = objInsert.DataSaidaEnt.Value.AddSeconds(10);
                else if (objInsert.DataSaidaEnt != null && objInsert.DataSaidaEnt.Value < objInsert.DataEmissao)
                    throw new Exception("A data de saída/entrada não pode ser inferior à data de emissão.");

                //Verifica se o cfop pode ser utilizado na nota fiscal
                if (!NaturezaOperacaoDAO.Instance.ValidarCfop((int)objInsert.IdNaturezaOperacao.GetValueOrDefault(0), objInsert.TipoDocumento))
                    throw new Exception("A Natureza de operação selecionada não pode ser utilizada em notas desse tipo.");

                objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
                objInsert.DataCad = DateTime.Now;

                objInsert.InfCompl =
                    !string.IsNullOrEmpty(objInsert.InfCompl) ?
                    Formatacoes.TrataTextoDocFiscal(objInsert.InfCompl).Replace("$", "S") : string.Empty;

                if (objInsert.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída && objInsert.Modelo == ConfigNFe.Modelo(true)
                    && ClienteDAO.Instance.IsConsumidorFinal(sessao, objInsert.IdCliente.GetValueOrDefault(0)) && !string.IsNullOrEmpty(objInsert.CpfCnpjDestRem))
                    objInsert.Cpf = objInsert.CpfCnpjDestRem;

                var idNotaFiscal =  base.Insert(sessao, objInsert);

                #region Informações de Pagamento

                if (objInsert.PagamentoNfce != null || objInsert.PagamentoNfce.Any())
                {
                    foreach (var p in objInsert.PagamentoNfce)
                    {
                        p.IdNf = (int)idNotaFiscal;
                        PagtoNotaFiscalDAO.Instance.Insert(sessao, p);
                    }
                }

                #endregion

                return idNotaFiscal;
            }
        }

        public int UpdateComTransacao(NotaFiscal objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Update(transaction, objUpdate);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override int Update(NotaFiscal objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession session, NotaFiscal objUpdate)
        {
            NotaFiscal old = GetElementByPrimaryKey(session, objUpdate.IdNf);

            objUpdate.TipoDocumento = old.TipoDocumento;

            LimparEmitenteDestinatario(session, objUpdate);

            // Não permite inserir nota fiscal se a loja informada não existir
            if (objUpdate.IdLoja.GetValueOrDefault() == 0 || (objUpdate.IdLoja > 0 && !LojaDAO.Instance.Exists(session, objUpdate.IdLoja)))
                throw new Exception("A loja informada não existe.");

            if ((old.TipoDocumento == (int)NotaFiscal.TipoDoc.Entrada || old.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída) && objUpdate.IdLoja != old.IdLoja)
                throw new Exception("A loja da nota não pode ser alterada depois da mesma já ter sido inserida");
            
            // Não permite inserir notas para clientes inativos
            if (!Configuracoes.FiscalConfig.NotaFiscalConfig.PermitirEmitirNotaParaClienteBloqueadoOuInativo &&
                objUpdate.IdCliente > 0 && ClienteDAO.Instance.GetSituacao(objUpdate.IdCliente.Value) != (int)SituacaoCliente.Ativo)
                throw new Exception("O cliente selecionado está inativo.");

            // Verifica se a data de saída é menor que a data de emissão da nota
            if (objUpdate.DataSaidaEnt != null && objUpdate.DataSaidaEnt.Value == objUpdate.DataEmissao)
                objUpdate.DataSaidaEnt.Value.AddSeconds(10);
            else if (objUpdate.DataSaidaEnt != null && objUpdate.DataSaidaEnt.Value < objUpdate.DataEmissao)
                throw new Exception("A data de saída/entrada não pode ser inferior à data de emissão.");

            // Comentado pois a forma de pagamento da nota é diferente da forma do cliente
            // Chamado 13382.
            // Se o cliente for alterado, verifica se a forma de pagamento da nota existe em seu cadastro
            //if (objUpdate.IdCliente.GetValueOrDefault() > 0 && PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)objUpdate.IdNf).Any() &&
            //    old.IdCliente.GetValueOrDefault() != objUpdate.IdCliente.GetValueOrDefault())
            //{
            //    var formaPagtoValida = false;

            //    foreach (var formaPagto in FormaPagtoDAO.Instance.GetByCliente(objUpdate.IdCliente.Value))
            //        if (formaPagto.IdFormaPagto == objUpdate.IdFormaPagto.Value)
            //            formaPagtoValida = true;

            //    if (!formaPagtoValida)
            //        throw new Exception("O cliente não possui, em seu cadastro, a forma de pagamento selecionada na nota fiscal.");
            //}

            // Chamado 13429.
            if (objUpdate.DatasParcelas != null && objUpdate.DatasParcelas.Length > 0)
            {
                var lstDataParcelas = new List<DateTime>();

                foreach (var dataParcela in objUpdate.DatasParcelas)
                {
                    if (!lstDataParcelas.Contains(dataParcela))
                        lstDataParcelas.Add(dataParcela);
                    else if (dataParcela.Date != new DateTime().Date)
                    {
                        var parcelaNova = dataParcela;
                        while (lstDataParcelas.Contains(parcelaNova))
                            parcelaNova = parcelaNova.AddDays(1);

                        lstDataParcelas.Add(parcelaNova);
                    }
                }
            }

            if (!NaturezaOperacaoDAO.Instance.ValidarCfop((int)objUpdate.IdNaturezaOperacao.GetValueOrDefault(0), objUpdate.TipoDocumento))
                throw new Exception("A Natureza de operação selecionada não pode ser utilizada em notas desse tipo.");

            // Se a nota não puder ser editada, não atualiza
            if (!old.EditVisible)
                return 1;

            objUpdate.Situacao = old.Situacao;
            objUpdate.TipoDocumento = old.TipoDocumento;
            objUpdate.TipoImpressao = old.TipoImpressao;
            objUpdate.FormaEmissao = old.FormaEmissao;

            var cfop = NaturezaOperacaoDAO.Instance.ObtemIdCfop(objUpdate.IdNaturezaOperacao.GetValueOrDefault());

            objUpdate.NumLote = old.NumLote;
            objUpdate.NumProtocolo = old.NumProtocolo;
            objUpdate.NumRecibo = old.NumRecibo;
            objUpdate.ObsSefaz = old.ObsSefaz;
            objUpdate.TipoAmbiente = old.TipoAmbiente;
            objUpdate.Consumidor = old.Consumidor;

            /* Chamado 22266.
            * A nota fiscal complementar deve ter a mesma natureza de operação da nota complementada, por isso,
            * mesmo que o CFOP seja de devolução, a finalidade de emissão deve ser mantida. */
            if (old.FinalidadeEmissao != (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar &&
                old.FinalidadeEmissao != (int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste)
            {
                if (CfopDAO.Instance.IsCfopDevolucao(cfop))
                    objUpdate.FinalidadeEmissao = (int)Glass.Data.Model.NotaFiscal.FinalidadeEmissaoEnum.Devolucao;
                else if (old.FinalidadeEmissao == (int)Glass.Data.Model.NotaFiscal.FinalidadeEmissaoEnum.Devolucao)
                    objUpdate.FinalidadeEmissao = (int)Glass.Data.Model.NotaFiscal.FinalidadeEmissaoEnum.Normal;
                else
                    objUpdate.FinalidadeEmissao = old.FinalidadeEmissao;
            }
            else
                objUpdate.FinalidadeEmissao = old.FinalidadeEmissao;

            if (objUpdate.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar)
                objUpdate.IdsNfRef = old.IdsNfRef;

            if (objUpdate.TipoDocumento != (int)NotaFiscal.TipoDoc.EntradaTerceiros && objUpdate.TipoDocumento != (int)NotaFiscal.TipoDoc.NotaCliente)
            {
                objUpdate.Serie = old.Serie;
                objUpdate.Modelo = old.Modelo;

                if (objUpdate.NumeroNFe == 0 || NotaFiscalDAO.Instance.ExisteNotaSaida(session, objUpdate.IdNf))
                    objUpdate.NumeroNFe = old.NumeroNFe;
            }

            /* Chamado 39222. */
            if (!string.IsNullOrEmpty(objUpdate.Modelo))
                objUpdate.Modelo = objUpdate.Modelo.Trim();

            #region Parcelas

            if (objUpdate.NumParc.GetValueOrDefault() <= FiscalConfig.NotaFiscalConfig.NumeroParcelasNFe)
            {
                ParcelaNf parcela = new ParcelaNf();

                parcela.IdNf = objUpdate.IdNf;

                // Se for venda à vista ou se o exclui as parcelas
                if (objUpdate.FormaPagto == 1 || objUpdate.NumParc.GetValueOrDefault() == 0)
                    ParcelaNfDAO.Instance.DeleteFromNf(session, objUpdate.IdNf);
                // Se for venda à prazo, salva as parcelas
                else if (objUpdate.ValoresParcelas != null && objUpdate.ValoresParcelas.Length > 0 && objUpdate.ValoresParcelas[0] > 0)
                {
                    ParcelaNfDAO.Instance.DeleteFromNf(session, objUpdate.IdNf);

                    for (int i = 0; i < objUpdate.NumParc; i++)
                    {
                        parcela.Valor = objUpdate.ValoresParcelas[i];
                        parcela.Data = objUpdate.DatasParcelas[i];
                        parcela.NumBoleto = objUpdate.BoletosParcelas != null && objUpdate.BoletosParcelas.Length > 0 ? objUpdate.BoletosParcelas[i] : null;
                        ParcelaNfDAO.Instance.Insert(session, parcela);
                    }
                }
            }

            #endregion

            #region Informações de Pagamento

            if (old.PagamentoNfce == null || !old.PagamentoNfce.Any())
            {
                foreach (var p in objUpdate.PagamentoNfce)
                {
                    p.IdNf = (int)objUpdate.IdNf;
                    PagtoNotaFiscalDAO.Instance.Insert(session, p);
                }
            }
            else
            {
                for (int i = 0; i < old.PagamentoNfce.Count; i++)
                {
                    var pOld = old.PagamentoNfce[i];

                    var p = objUpdate.PagamentoNfce
                        .FirstOrDefault(f => f.FormaPagto == pOld.FormaPagto &&
                        f.Valor == pOld.Valor && f.Bandeira == pOld.Bandeira &&
                        f.CnpjCredenciadora == pOld.CnpjCredenciadora && f.NumAut == pOld.NumAut);

                    if (p == null)
                        PagtoNotaFiscalDAO.Instance.DeleteByPrimaryKey(session, pOld.IdPagtoNf);
                    else
                        objUpdate.PagamentoNfce.Remove(p);

                }

                foreach (var p in objUpdate.PagamentoNfce)
                {
                    p.IdNf = (int)objUpdate.IdNf;
                    PagtoNotaFiscalDAO.Instance.Insert(session, p);
                }
            }

            #endregion

            #region Atualiza a observação do CFOP nas informações complementares da nota.

            /* Chamado 36403. */
            objUpdate.InfCompl = Formatacoes.TrataTextoDocFiscal(objUpdate.InfCompl).Replace("$", "S");

            /* Chamado 35147. */
            if (old.IdCfop > 0 && objUpdate.IdCfop > 0 && old.IdCfop != objUpdate.IdCfop)
            {
                var obsCfopNovo = CfopDAO.Instance.GetObs(session, objUpdate.IdCfop.Value);

                var podeInserirObs = VerificarInserirInformacaoComplementar(objUpdate.InfCompl, obsCfopNovo);

                if (podeInserirObs && !string.IsNullOrEmpty(obsCfopNovo))
                {
                    obsCfopNovo =
                        !string.IsNullOrEmpty(obsCfopNovo) ?
                            string.Format("{0}.", Formatacoes.TrataTextoDocFiscal(obsCfopNovo).Replace("$", "S")) :
                            string.Empty;

                    objUpdate.InfCompl =
                        string.Format("{0} {1}",
                            obsCfopNovo,
                            !string.IsNullOrEmpty(objUpdate.InfCompl) ?
                                Formatacoes.TrataTextoDocFiscal(objUpdate.InfCompl).Replace("$", "S") :
                                string.Empty).TrimEnd(' ');
                }
            }

            #endregion

            if (objUpdate.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída && objUpdate.Modelo == ConfigNFe.Modelo(true)
                    && ClienteDAO.Instance.IsConsumidorFinal(objUpdate.IdCliente.GetValueOrDefault(0)) && !string.IsNullOrEmpty(objUpdate.CpfCnpjDestRem))
                objUpdate.Cpf = objUpdate.CpfCnpjDestRem;

            LogAlteracaoDAO.Instance.LogNotaFiscal(session, objUpdate);
            int retorno = base.Update(session, objUpdate);

            /* Chamado 14947.
             * É necessário que a nota de devolução do tipo EntradaTerceiros atualize o valor total da nota. */
            // Calcula impostos/totais apenas se não for nota de transporte ou complementar ou de importação
            if (!objUpdate.Transporte && !objUpdate.Complementar &&
                (objUpdate.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Normal ||
                objUpdate.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Devolucao) &&
                !IsFinalizada(session, objUpdate.IdNf) && !IsNotaFiscalImportacao(session, objUpdate.IdNf))
            {
                // Recupera valor antigo do IPI da nota
                objUpdate.ValorIpi = old.ValorIpi;

                UpdateTotalNf(session, objUpdate.IdNf);

                var lstProdNf = ProdutosNfDAO.Instance.GetByNf(session, objUpdate.IdNf).ToList();
                ProdutosNfDAO.Instance.CalcImposto(session, ref lstProdNf, true, false);

                UpdateTotalNf(session, objUpdate.IdNf);
            }
            else if (objUpdate.Transporte)
            {
                // Apaga produtos da nota se for transporte
                ProdutosNfDAO.Instance.DeleteByNotaFiscal(session, objUpdate.IdNf);
            }

            return retorno;
        }

        public override int Delete(NotaFiscal objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdNf);
        }

        public int DeleteNotaFiscal(GDASession sessao, uint idNf)
        {
            // Verifica se esta nota possui peças impressas
            if (ProdutoImpressaoDAO.Instance.NotaFiscalPossuiPecaProducao(sessao, idNf))
                throw new Exception("Esta nota fiscal não pode ser excluída pois existem etiquetas para a mesma.");

            // Verifica se esta nota possui contas a pagar/pagas
            if (ContasPagarDAO.Instance.ExecuteScalar<bool>(sessao, "Select Count(*)>0 From contas_pagar Where idNf=" + idNf))
                throw new Exception("Esta nota fiscal não pode ser excluída pois existem contas a pagar/pagas para a mesma.");

            var nfReferencia = NotaFiscalDAO.Instance.ExecuteScalar<string>(sessao, string.Format("SELECT numeronfe FROM nota_fiscal WHERE idsnfref LIKE '%{0}%'", idNf));

            if(!string.IsNullOrEmpty(nfReferencia))
                throw new Exception("Esta nota fiscal não pode ser excluída pois existem notas fiscais referenciando a mesma.\nNotas:" + nfReferencia);

            try
            {
                var prodsNF = ProdutosNfDAO.Instance.GetByNf(sessao, idNf);
                foreach (ProdutosNf pnf in prodsNF)
                    ProdutosNfDAO.Instance.DeleteProdutoNf(sessao, pnf.IdProdNf);

                var parcNF = ParcelaNfDAO.Instance.GetByNf(sessao, idNf).ToArray();
                foreach (ParcelaNf pnf in parcNF)
                    ParcelaNfDAO.Instance.Delete(sessao, pnf);

                CompraNotaFiscalDAO.Instance.ApagarPelaNFe(sessao, idNf);

                PagtoNotaFiscalDAO.Instance.RemovePagamentos(sessao, (int)idNf);
            }
            catch (Exception)
            {
                return 0;
            }

            LogAlteracaoDAO.Instance.ApagaLogNotaFiscal(idNf);
            return GDAOperations.Delete(sessao, new NotaFiscal { IdNf = idNf });
        }

        public override int DeleteByPrimaryKey(uint idNf)
        {
            return DeleteNotaFiscal(null, idNf);
        }

        private bool VerificarInserirInformacaoComplementar(string infCompl, string obsNova)
        {
            var informacaoCompl = Formatacoes.TrataTextoDocFiscal(infCompl).TrimEnd('.');

            var observacaoNova = Formatacoes.TrataTextoDocFiscal(obsNova);

            if (informacaoCompl.Contains(observacaoNova))
                return false;

           return true;
        }

        #endregion

        #region Recupera autorizadas e finalizadas

        public List<NotaFiscal> ObtemAutorizadasFinalizadas()
        {
            string sql = SqlPorSituacao(0, 0, 0, null, 0, 0, null, 0, 0, null, null, "2,13", 0, null, null, null, null, null, null, 0, null, 0, 0, 0,
                null, null, null, null, null, null, 0, false, false, null, true);
            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Recupera as notas fiscais para envio do FCI.

        /// <summary>
        /// Recupera as notas fiscais que contenham produtos com os campos
        /// PercentualImportacaoAtual e AliquotaInterEstadual diferentes da tabela
        /// produto_percentual_importacao on inexistentes na mesma
        /// </summary>
        /// <returns></returns>
        public List<NotaFiscal> ObterListaNotasEnvioFCI(uint idNf)
        {
            string sql = @"SELECT DISTINCT nf.*, func.nome as DescrUsuCad,
                " + SqlCampoDestinatario(TipoCampo.Nome, "l", "c", "f", "nf", "tc") + @" as NomeDestRem
                    FROM nota_fiscal nf 
                    INNER JOIN produtos_nf pnf ON(pnf.IDNF=nf.IDNF)
                    LEFT JOIN natureza_operacao no ON (nf.idNaturezaOperacao=no.idNaturezaOperacao)
                    LEFT JOIN cfop cf ON (no.idCfop=cf.idCfop)
                    LEFT JOIN tipo_cfop tc ON (cf.idTipoCfop=tc.idTipoCfop) 
                    LEFT JOIN funcionario func ON (nf.usuCad=func.idFunc)
                    LEFT JOIN cliente c ON (nf.idCliente=c.id_Cli) 
                    LEFT JOIN loja l ON (nf.idLoja=l.idLoja) 
                    LEFT JOIN fornecedor f ON (nf.idFornec=f.idFornec)
                WHERE nf.situacao = " + (int)NotaFiscal.SituacaoEnum.Aberta + @"
                    AND nf.tipoDocumento = " + (int)NotaFiscal.TipoDoc.Saída + @"
                    AND pnf.CstOrig IN (3, 5, 8)";

            if (idNf > 0)
                sql += " AND nf.IdNf=" + idNf;

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Relatório de Antecipação

        /// <summary>
        /// Busca notas relacionados à Antecipação passada
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public NotaFiscal[] GetForRptAntecipFornec(uint idAntecipFornec)
        {
            string sql = @"
                           SELECT n.*, cf.codInterno as codCfop, CONCAT(f.IdFornec,' - ',f.nomeFantasia) as nomeEmitente, l.nomeFantasia as nomeDestRem,
                                 coalesce(no.codInterno, cf.codInterno) as CodNaturezaOperacao, func.nome as DescrUsuCad 
                           FROM nota_fiscal n 
                           LEFT JOIN natureza_operacao no ON (n.idNaturezaOperacao=no.idNaturezaOperacao)
                           LEFT JOIN cfop cf ON (no.idCfop=cf.idCfop) 
                           LEFT JOIN loja l ON (n.idLoja=l.idLoja) 
                           Left JOIN fornecedor f ON (n.idFornec=f.idFornec) 
                           Left JOIN funcionario func ON (n.usuCad=func.idFunc)
                           WHERE n.idAntecipFornec=" + idAntecipFornec + " AND n.situacao=" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros;

            return objPersistence.LoadData(sql).ToArray();
        }

        #endregion

        #region Recupera as mensagens das naturezas de operação da NF-e

        /// <summary>
        /// Recupera as mensagens das naturezas de operação da NF-e.
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public string ObtemMensagemNaturezasOperacao(uint idNf)
        {
            string ids = GetValoresCampo(@"
                select idNaturezaOperacao from produtos_nf where idNf=?id and idNaturezaOperacao is not null
                union select idNaturezaOperacao from nota_fiscal where idNf=?id and idNaturezaOperacao is not null",
                "idNaturezaOperacao", new GDAParameter("?id", idNf));

            if (String.IsNullOrEmpty(ids))
                return null;

            var mensagens = ExecuteMultipleScalar<string>(@"
                select mensagem from natureza_operacao
                where mensagem is not null and idNaturezaOperacao in (" + ids + ")");

            return String.Join(", ", mensagens.ToArray());
        }

        #endregion

        #region Valida um GUID

        public bool IsGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return false;

            var isGuid =
                new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
                    RegexOptions.Compiled);

            if (isGuid.IsMatch(guid))
                return true;

            return false;
        }

        #endregion

        #region Envia o e-mail do XML para o cliente

        /// <summary>
        /// Envia o e-mail do XML para o cliente
        /// </summary>
        public void EnviarEmailXml(uint idNf)
        {
            EnviarEmailXml(idNf, false);
        }

        /// <summary>
        /// Envia o e-mail do XML para o cliente
        /// </summary>
        public void EnviarEmailXml(uint idNf, bool cancelamento)
        {
            var nf = NotaFiscalDAO.Instance.GetElement(idNf);
            EnviarEmailXml(nf, cancelamento);
        }

        /// <summary>
        /// Envia o e-mail do XML para o cliente
        /// </summary>
        public void EnviarEmailXml(NotaFiscal nf)
        {
            EnviarEmailXml(null, nf);
        }

        /// <summary>
        /// Envia o e-mail do XML para o cliente
        /// </summary>
        public void EnviarEmailXml(GDASession session, NotaFiscal nf)
        {
            EnviarEmailXml(session, nf, false);
        }

        /// <summary>
        /// Envia o e-mail do XML para o cliente
        /// </summary>
        public void EnviarEmailXml(NotaFiscal nf, bool cancelamento)
        {
            EnviarEmailXml(null, nf, cancelamento);
        }

        /// <summary>
        /// Envia o e-mail do XML para o cliente
        /// </summary>
        public void EnviarEmailXml(GDASession session, NotaFiscal nf, bool cancelamento)
        {
            if (nf == null || nf.IdNf == 0 || (nf.IdLoja.GetValueOrDefault() == 0 && nf.IdCliente.GetValueOrDefault() == 0))
                return;

            if (nf.IdCliente > 0)
            {
                try
                {
                    var cliente = ClienteDAO.Instance.GetElementByPrimaryKey(session, nf.IdCliente.Value);

                    if (cliente != null && !cliente.NaoReceberEmailFiscal)
                    {
                        var loja = LojaDAO.Instance.GetElementByPrimaryKey(session, nf.IdLoja.Value);

                        if ((loja?.IdLoja).GetValueOrDefault() == 0)
                        {
                            throw new Exception("Não foi possível recuperar a loja da nota fiscal ao salvar o e-mail a ser enviado.");
                        }

                        if (!cancelamento)
                        {
                            Email.EnviaEmailAsync((uint)loja.IdLoja, cliente.EmailFiscal, "Emissão de NF-e", "Prezados(as),\n\nSegue em anexo XML de " +
                                "nota fiscal emitida pela " + loja.RazaoSocial + ". A NF-e também pode ser consultada diretamente pelo " +
                                "portal nacional da NF-e: www.nfe.fazenda.gov.br/portal informando a seguinte chave de acesso: " +
                                nf.ChaveAcesso + ".", Email.EmailEnvio.Fiscal,
                                new AnexoEmail("~/Handlers/NotaXml.ashx?idNf=" + nf.IdNf, "NotaFiscal.xml"),
                                new AnexoEmail("~/Handlers/Danfe.ashx?idNf=" + nf.IdNf, "DANFE.pdf"));

                            LogNfDAO.Instance.NewLog(nf.IdNf, "Envio de Email", 0, "Email com XML da NF-e enviado ao cliente.");
                        }
                        else
                        {
                            var assunto = "Cancelamento de NF-e";
                            var dataCancelamento = LogNfDAO.Instance.ObtemDataCancelamento(session, (int)nf.IdNf);
                            var mensagem =
                                string.Format("Prezados(as),\n\nSegue em anexo XML da nota fiscal cancelada pela {0}. A NF-e também pode ser consultada diretamente pelo " +
                                    "portal nacional da NF-e: www.nfe.fazenda.gov.br/portal informando a seguinte chave de acesso: {1}. " +
                                    "\nProtocolo de cancelamento: {2}.{3}", loja.RazaoSocial, nf.ChaveAcesso, nf.NumProtocoloCanc,
                                    dataCancelamento != null ? string.Format("\nData do cancelamento: {0}.", dataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss")) : "");

                            Email.EnviaEmailAsync((uint)loja.IdLoja, cliente.EmailFiscal, assunto, mensagem, Email.EmailEnvio.Fiscal,
                                new AnexoEmail("~/Handlers/NotaXml.ashx?idNf=" + nf.IdNf, "NotaFiscal.xml"),
                                new AnexoEmail("~/Handlers/Danfe.ashx?idNf=" + nf.IdNf, "DANFE.pdf"));

                            LogNfDAO.Instance.NewLog(nf.IdNf, "Envio de Email NF-e cancelada", 0, "Email com XML, da NF-e cancelada, enviado ao cliente.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogNfDAO.Instance.NewLog(nf.IdNf, "Envio de Email", 0, Glass.MensagemAlerta.FormatErrorMsg("Falha ao enviar email com XML da NF-e.", ex));
                }
            }
        }

        #endregion

        #region Transfere a NF-e de um banco para outro

        /// <summary>
        /// Transfere a NF-e de um banco para outro.
        /// </summary>
        private uint TransfereNFeBanco(GDASession session, uint idNf, uint idNaturezaOperacaoDestino, Dictionary<uint, uint> dicNaturezaOperacaoProdDestino)
        {
            /* Chamado 52139. */
            using (var transactionTransferencia = new GDATransaction(GDASettings.GetProviderConfiguration("Notas")))
            {
                try
                {
                    transactionTransferencia.BeginTransaction();

                    var nf = GetElement(session, idNf);
                    var prods = ProdutosNfDAO.Instance.GetByNf(session, idNf);
                    var parcelas = ParcelaNfDAO.Instance.GetByNf(session, idNf);
                    var idLoja = LojaDAO.Instance.GetLojaByCNPJIE(transactionTransferencia, "06.915.743/0001-78", null, false).StrParaUint();

                    if (idLoja == 0)
                        throw new Exception("A loja para exportação não foi encontrada.");

                    nf.IdLoja = idLoja;
                    if (nf.IdCliente > 0)
                        nf.IdCliente = ClienteDAO.Instance.ObterIdPorCpfCnpj(transactionTransferencia, nf.CpfCnpjDestRem);
                    if (nf.IdFornec > 0)
                        nf.IdFornec = FornecedorDAO.Instance.ObterIdPorCpfCnpj(transactionTransferencia, nf.CpfCnpjDestRem);
                    nf.NumeroNFe = ProxNumeroNFe(transactionTransferencia, nf.IdLoja.GetValueOrDefault(), nf.Serie.StrParaInt());

                    //Caso não tenha selecionado a natureza da operação
                    if (idNaturezaOperacaoDestino == 0)
                    {
                        // Recupera os dados do CFOP e natureza de operação do sistema de origem.
                        var codInternoCfopOrigem = CfopDAO.Instance.ObtemCodInterno(nf.IdCfop.GetValueOrDefault(0));
                        var codInternoNatOpOrigem = NaturezaOperacaoDAO.Instance.ObtemCodigoInterno(nf.IdNaturezaOperacao.GetValueOrDefault(0));

                        // Recupera os dados de CFOP e natureza de operação do sistema de destino.
                        var idCfopDestino = CfopDAO.Instance.ObtemIdCfop(transactionTransferencia, codInternoCfopOrigem);
                        idNaturezaOperacaoDestino = NaturezaOperacaoDAO.Instance.ObtemIdNatOpPorCfopCodInterno(transactionTransferencia, idCfopDestino, codInternoNatOpOrigem);
                    }

                    nf.IdNaturezaOperacao = idNaturezaOperacaoDestino;

                    /* Chamado 52139. */
                    if (prods == null || prods.Count() == 0)
                        throw new Exception("Não é possível efetuar uma transferência de nota fiscal caso a nota fiscal original não possua produtos.");

                    var novoIdNf = Insert(transactionTransferencia, nf);

                    /* Chamado 52139. */
                    if (novoIdNf == 0)
                        throw new Exception("Não foi possível transferir a nota fiscal. Foi inserida uma nota com o mesmo cliente e " +
                            "loja há menos de 60 segundos. Aguarde alguns segundos e tente novamente.");

                    foreach (var prod in prods)
                    {
                        prod.IdNf = novoIdNf;
                        prod.IdProd = ProdutoDAO.Instance.ObterIdPorCodInterno(transactionTransferencia, prod.CodInterno);
                        prod.IdNaturezaOperacao = dicNaturezaOperacaoProdDestino[prod.IdProdNf];
                        // Necessário zerar para não atualizar um produtoNF antigo no método calcular imposto executado antes de Insert.
                        prod.IdProdNf = 0;
                        ProdutosNfDAO.Instance.Insert(transactionTransferencia, prod);
                    }

                    foreach (var parc in parcelas)
                    {
                        parc.IdNf = novoIdNf;
                        ParcelaNfDAO.Instance.Insert(transactionTransferencia, parc);
                    }

                    /* Chamado 52139. */
                    LogNfDAO.Instance.NewLog(transactionTransferencia, nf.IdNf, "Transferência NF-e", 0, "Transferência efetuada com sucesso.");

                    ProdutosNfDAO.Instance.DeleteByNotaFiscal(session, idNf);
                    PedidosNotaFiscalDAO.Instance.DeleteByNotaFiscal(session, idNf);
                    ParcelaNfDAO.Instance.DeleteFromNf(session, idNf);

                    DeleteNotaFiscal(session, idNf);

                    transactionTransferencia.Commit();
                    transactionTransferencia.Close();

                    return novoIdNf;
                }
                catch (Exception ex)
                {
                    transactionTransferencia.Rollback();
                    transactionTransferencia.Close();

                    throw new Exception("Falha ao exportar NF-e", ex);
                }
            }
        }

        #endregion

        #region Obtem o indicador da IE do Destinatário

        /// <summary>
        ///  Obtem o indicador da IE do Destinatário
        /// </summary>
        public IndicadorIEDestinatario? ObterIndicadorIE(NotaFiscal notaFiscal, Model.Cte.ConhecimentoTransporte conhecimentoTransporte, Cliente cliente, Fornecedor fornec, out Cidade cidadeFornec)
        {
            IndicadorIEDestinatario? indIeDest = null;

            cidadeFornec = fornec != null ? CidadeDAO.Instance.GetElementByPrimaryKey(fornec.IdCidade.GetValueOrDefault()) : null;

            if ((notaFiscal != null && (notaFiscal.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída || notaFiscal.TipoDocumento == (int)NotaFiscal.TipoDoc.NotaCliente)) ||
                (conhecimentoTransporte != null && conhecimentoTransporte.TipoDocumentoCte == (int)Model.Cte.ConhecimentoTransporte.TipoDocumentoCteEnum.Saida))
            {
                if (cliente != null)
                    indIeDest = cliente.IndicadorIEDestinatario;
                else if (fornec != null)
                {
                    if ((string.IsNullOrEmpty(fornec.RgInscEst) || fornec.TipoPessoa.ToUpper() == "F") && !fornec.ProdutorRural)
                        indIeDest = IndicadorIEDestinatario.NaoContribuinte;
                    else if (string.IsNullOrEmpty(fornec.RgInscEst) || fornec.RgInscEst.ToLower().Contains("isento"))
                        indIeDest = IndicadorIEDestinatario.ContribuinteIsento;
                    else if (Validacoes.ValidaIE(cidadeFornec.NomeUf, fornec.RgInscEst))
                        indIeDest = IndicadorIEDestinatario.ContribuinteICMS;
                    else
                        throw new Exception("A IE do fornecedor é inválida. Altere esta informação no cadastro do fornecedor e tente emitir a NFe novamente.");
                }
            }
            else
            {
                if (fornec != null)
                {
                    if ((string.IsNullOrEmpty(fornec.RgInscEst) || fornec.TipoPessoa.ToUpper() == "F") && !fornec.ProdutorRural)
                        indIeDest = IndicadorIEDestinatario.NaoContribuinte;
                    else if (string.IsNullOrEmpty(fornec.RgInscEst) || fornec.RgInscEst.ToLower().Contains("isento"))
                        indIeDest = IndicadorIEDestinatario.ContribuinteIsento;
                    else if (Validacoes.ValidaIE(cidadeFornec.NomeUf, fornec.RgInscEst))
                        indIeDest = IndicadorIEDestinatario.ContribuinteICMS;
                    else
                        throw new Exception("A IE do fornecedor é inválida. Altere esta informação no cadastro do fornecedor e tente emitir a NFe novamente.");
                }
                else if (cliente != null)
                    indIeDest = cliente.IndicadorIEDestinatario;
            }

            return indIeDest;
        }

        #endregion

        #region NFC-e

        /// <summary>
        /// Verifica se a nota fiscal é de consumidor (NFC-e)
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool IsNotaFiscalConsumidor(uint idNf)
        {
            if (idNf == 0)
                return false;

            return objPersistence.ExecuteSqlQueryCount("Select Count(*) from nota_fiscal Where Consumidor And idNF=" + idNf) > 0;
        }

        /// <summary>
        /// Obtem o link do QR Code da NFC-e
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public string ObtemLinkQrCodeNfce(NotaFiscal nfe, string digestValue)
        {
            if (nfe == null)
                throw new Exception("Erro ao gerar link do QRCode. Não foi possível recuperar a nota fiscal.");

            var chaveAcesso = nfe.ChaveAcesso;

            if (nfe.IdLoja.GetValueOrDefault() == 0)
                throw new Exception("Erro ao gerar link do QRCode. Informe a loja da nota fiscal.");

            var loja = LojaDAO.Instance.GetElement(nfe.IdLoja.Value);

            if (loja == null)
                throw new Exception("Erro ao gerar link do QRCode. Loja da nota fiscal não encontrada.");

            var dtEmissHex = nfe.DataEmissao.ToString("yyyy-MM-ddTHH:mm:sszzz").StrParaHex();
            var digValHex = digestValue.StrParaHex();

            var idCsc = loja.IdCsc;
            var csc = loja.Csc;

            if (string.IsNullOrEmpty(idCsc) || string.IsNullOrEmpty(csc))
                throw new Exception("Código de Segurança do Contribuinte (CSC) não foi informado no cadastro da loja.");

            var link = "";

            //Chave de Acesso
            link += @"chNFe=" + nfe.ChaveAcesso.Replace(" ", "");

            //Versão Qr Code
            link += @"&nVersao=100";

            //Tipo Ambiente
            link += "&tpAmb=" + nfe.TipoAmbiente;

            //Identificação do Destinatario
            if (!ClienteDAO.Instance.IsConsumidorFinal(nfe.IdCliente.GetValueOrDefault(0)) || !string.IsNullOrEmpty(nfe.Cpf))
                link += @"&cDest=" + nfe.CpfCnpjDestRem.Replace(".", "").Replace("/", "").Replace("-", "");

            //Data de Emissão
            link += @"&dhEmi=" + dtEmissHex;

            //Valor total da NFC-e
            link += @"&vNF=" + Math.Round(nfe.TotalNota, 2).ToString().Replace(",", ".");

            //Valor de ICMS
            link += @"&vICMS=" + Math.Round(nfe.Valoricms, 2).ToString().Replace(",", ".");

            //Digest Value da NFC-e
            link += @"&digVal=" + digValHex.PadLeft(56, '0');

            //Identificador do token
            link += @"&cIdToken=" + idCsc.PadLeft(6, '0');

            //Hash da primeira parte do link
            var linkHash = (link + csc).CodificaParaSHA1();

            //Hash
            link += @"&cHashQRCode=" + linkHash;

            link = GetWebService.UrlQrCode(loja.Uf, (ConfigNFe.TipoAmbienteNfe)nfe.TipoAmbiente) + link;

            return link;
        }

        ///// <summary>
        ///// Obtem o modelo da nota
        ///// </summary>
        ///// <param name="idNf"></param>
        ///// <returns></returns>
        //public int ObtemModelo(uint idNf)
        //{
        //    return ExecuteScalar<int>("SELECT modelo FROM nota_fiscal where IdNf = " + idNf);
        //}

        ///// <summary>
        ///// Marca que houve um erro de conexão ao tentar emitir uma NFC-e, para poder posteriormente ser possivel 
        ///// emitir em modo offline
        ///// </summary>
        ///// <param name="idNf"></param>
        //public void MarcarFalhaEmissao(uint idNf)
        //{
        //    objPersistence.ExecuteCommand("UPDATE TABLE nota_fiscal set FalhaEmitir = 1 WHERE idNf = " + idNf);
        //}

        ///// <summary>
        ///// Verifica se houve falha de conexão ao tentar emitir a NFC-e
        ///// </summary>
        ///// <param name="idNf"></param>
        ///// <returns></returns>
        //public bool ObtemFalhaEmissao(uint idNf)
        //{
        //    if (idNf == 0)
        //        return false;

        //    return objPersistence.ExecuteSqlQueryCount("Select Count(*) from nota_fiscal Where FalhaEmitir And idNF=" + idNf) > 0;
        //}

        #endregion

        #region Pode enviar pedido inutilização

        /// <summary>
        /// Verifica o pedido de inutilização da nota fiscal pode ser enviado à receita.
        /// </summary>
        public bool PodeEnviarPedidoInutilizacao(GDASession sessao, uint idNf)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao,
                string.Format(
                    @"SELECT COUNT(*) FROM log_nf ln
                        WHERE ln.IdNf={0} AND
                        ln.Codigo=102 AND
                        ln.DataHora>=DATE_ADD(NOW(), INTERVAL - 2 MINUTE)", idNf)) == 0;
        }

        #endregion
    }
}