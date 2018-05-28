using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Web;
using Glass.Data.Exceptions;
using System.Linq;
using Glass.Data.RelDAL;
using Glass.Configuracoes;
using Glass.Global;
using Colosoft;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.DAL
{
    public sealed partial class PedidoDAO : BaseCadastroDAO<Pedido, PedidoDAO>
    {
        #region Data de entrega mínima do pedido

        /// <summary>
        /// Verifica se a data de entrega de um pedido deve ser bloqueada.
        /// </summary>
        public bool BloquearDataEntregaMinima(uint? idPedido)
        {
            return BloquearDataEntregaMinima(null, idPedido);
        }

        /// <summary>
        /// Verifica se a data de entrega de um pedido deve ser bloqueada.
        /// </summary>
        public bool BloquearDataEntregaMinima(GDASession session, uint? idPedido)
        {
            var tipoPedido = idPedido > 0 ? GetTipoPedido(session, idPedido.Value) : Pedido.TipoPedidoEnum.Producao;

            // Comentado: não bloqueia a data de entrega mínima se o pedido for de revenda
            //int configDias = tipoPedido == Pedido.TipoPedidoEnum.Venda ? PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido :
            //    tipoPedido == Pedido.TipoPedidoEnum.Revenda ? PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedidoRevenda : 0;

            var configDias = tipoPedido == Pedido.TipoPedidoEnum.Venda ? PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido : 0;

            int? tipoEntrega = idPedido > 0 ? (int?)ObtemTipoEntrega(session, idPedido.Value) : null;
            if (tipoEntrega > 0 && PedidoConfig.DiasMinimosEntregaTipo.Keys.Contains((Pedido.TipoEntregaPedido)tipoEntrega.Value))
                configDias = Math.Max(configDias, PedidoConfig.DiasMinimosEntregaTipo[(Pedido.TipoEntregaPedido)tipoEntrega.Value]);

            return configDias > 0 &&
                !Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega) &&
                (!(idPedido > 0) || !IsFastDelivery(idPedido.Value));
        }

        /// <summary>
        /// Altera a data de entrega do pedido para a data mínima.
        /// </summary>
        public void SetDataEntregaMinima(GDASession session, uint idPedido)
        {
            DateTime dataEntrega, dataFastDelivery;

            if (GetDataEntregaMinima(session, ObtemIdCliente(session, idPedido), idPedido, (int)GetTipoPedido(session, idPedido), ObtemTipoEntrega(session, idPedido),
                out dataEntrega, out dataFastDelivery))
                objPersistence.ExecuteCommand(session, "update pedido set dataEntrega=?data where idPedido=" + idPedido,
                    new GDAParameter("?data", !IsFastDelivery(session, idPedido) ? dataEntrega : dataFastDelivery));
        }

        internal DateTime GetDataEntregaMinimaFinal(uint idPedido, DateTime dataEntregaAtual, bool fastDelivery = false)
        {
            return GetDataEntregaMinimaFinal(null, idPedido, dataEntregaAtual, fastDelivery);
        }

        internal DateTime GetDataEntregaMinimaFinal(GDASession session, uint idPedido, DateTime dataEntregaAtual, bool fastDelivery = false)
        {
            DateTime data1, data2;
            bool desabilitar;

            uint idCliente = PedidoDAO.Instance.ObtemIdCliente(session, idPedido);
            int? tipoPedido = (int?)GetTipoPedido(session, idPedido);
            int? tipoEntrega = (int?)ObtemTipoEntrega(session, idPedido);
            DateTime dataPedido = GetDataPedido(session, idPedido);

            var dataComparar = !PedidoDAO.Instance.GetDataEntregaMinima(session, idCliente, idPedido, tipoPedido, tipoEntrega,
                dataPedido, out data1, out data2, out desabilitar, 0, fastDelivery) ? dataEntregaAtual : !fastDelivery ? data1 : data2;

            return dataEntregaAtual.Date >= dataComparar.Date ? dataEntregaAtual : dataComparar;
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(uint idCli, uint? idPedido, out DateTime dataEntregaMinima,
            out DateTime dataFastDelivery)
        {
            return GetDataEntregaMinima(null, idCli, idPedido, out dataEntregaMinima, out dataFastDelivery);
        }

        public bool GetDataEntregaMinima(GDASession session, uint idCli, uint? idPedido, out DateTime dataEntregaMinima, out DateTime dataFastDelivery)
        {
            int? tipoPedido = idPedido > 0 ? (int?)GetTipoPedido(session, idPedido.Value) : null;
            int? tipoEntrega = idPedido > 0 ? (int?)ObtemTipoEntrega(session, idPedido.Value) : null;
            return GetDataEntregaMinima(session, idCli, idPedido, tipoPedido, tipoEntrega, out dataEntregaMinima, out dataFastDelivery);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, out DateTime dataEntregaMinima, out DateTime dataFastDelivery)
        {
            return GetDataEntregaMinima((GDASession)null, idCli, idPedido, tipoPedido, tipoEntrega, out dataEntregaMinima, out dataFastDelivery);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(GDASession session, uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, out DateTime dataEntregaMinima, out DateTime dataFastDelivery)
        {
            return GetDataEntregaMinima(session, idCli, idPedido, tipoPedido, tipoEntrega, out dataEntregaMinima, out dataFastDelivery, 0);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, out DateTime dataEntregaMinima, out DateTime dataFastDelivery, int numeroDiasUteisMinimoNaoConfig)
        {
            return GetDataEntregaMinima(null, idCli, idPedido, tipoPedido, tipoEntrega, out dataEntregaMinima, out dataFastDelivery, numeroDiasUteisMinimoNaoConfig);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(GDASession session, uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, out DateTime dataEntregaMinima, out DateTime dataFastDelivery, int numeroDiasUteisMinimoNaoConfig)
        {
            bool temp;
            DateTime? dataBase = idPedido > 0 ? (DateTime?)GetDataPedido(session, idPedido.Value) : null;

            if (dataBase != null && dataBase.Value.Date < DateTime.Now.Date)
                dataBase = DateTime.Now;

            return GetDataEntregaMinima(session, idCli, idPedido, tipoPedido, tipoEntrega, dataBase, out dataEntregaMinima, out dataFastDelivery, out temp, numeroDiasUteisMinimoNaoConfig);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, DateTime? dataBase, out DateTime dataEntregaMinima, out DateTime dataFastDelivery, out bool desabilitarCampo)
        {
            return GetDataEntregaMinima(null, idCli, idPedido, tipoPedido, tipoEntrega, dataBase, out dataEntregaMinima, out dataFastDelivery, out desabilitarCampo);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(GDASession session, uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, DateTime? dataBase, out DateTime dataEntregaMinima, out DateTime dataFastDelivery, out bool desabilitarCampo)
        {
            return GetDataEntregaMinima(session, idCli, idPedido, tipoPedido, tipoEntrega, dataBase, out dataEntregaMinima, out dataFastDelivery, out desabilitarCampo, 0);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega,
            DateTime? dataBase, out DateTime dataEntregaMinima, out DateTime dataFastDelivery, out bool desabilitarCampo,
            int numeroDiasUteisMinimoNaoConfig, bool fastDelivery = false)
        {
            return GetDataEntregaMinima(null, idCli, idPedido, tipoPedido, tipoEntrega, dataBase, out dataEntregaMinima,
                out dataFastDelivery, out desabilitarCampo, numeroDiasUteisMinimoNaoConfig, fastDelivery);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(GDASession session, uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, DateTime? dataBase,
            out DateTime dataEntregaMinima, out DateTime dataFastDelivery, out bool desabilitarCampo, int numeroDiasUteisMinimoNaoConfig, bool fastDelivery = false)
        {
            try
            {
                /* Chamado 47744. */
                if (UserInfo.GetUserInfo == null || (UserInfo.GetUserInfo.CodUser == 0 && UserInfo.GetUserInfo.IdCliente == 0))
                    throw new Exception("Não foi possível recuperar o login do usuário.");

                if (dataBase == null || dataBase.Value.Ticks == 0)
                    dataBase = DateTime.Now;

                DateTime? dataRota = RotaDAO.Instance.GetDataRota(session, idCli, dataBase.Value.Date);

                dataEntregaMinima = dataBase.Value.Date;
                dataFastDelivery = dataBase.Value.Date;
                desabilitarCampo = PedidoConfig.DataEntrega.BloquearDataEntregaPedidoVendedor && !Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega)
                    && !UserInfo.GetUserInfo.IsAdministrador;

                tipoPedido = tipoPedido != null ? tipoPedido : idPedido > 0 ? (int?)GetTipoPedido(session, idPedido.Value) : null;
                int numeroDiasUteisMinimoConfig = tipoPedido == (int)Pedido.TipoPedidoEnum.Revenda ? PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedidoRevenda :
                    tipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra ? PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedidoMaoDeObra :
                    PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido;

                if (tipoEntrega != null)
                {
                    var existeTipo = PedidoConfig.DiasMinimosEntregaTipo.ContainsKey((Pedido.TipoEntregaPedido)tipoEntrega.Value);

                    numeroDiasUteisMinimoConfig = Math.Max(numeroDiasUteisMinimoConfig,
                        existeTipo ? PedidoConfig.DiasMinimosEntregaTipo[(Pedido.TipoEntregaPedido)tipoEntrega.Value] : 0);
                }

                int numeroDiasSomar = numeroDiasUteisMinimoConfig > 0 ? numeroDiasUteisMinimoConfig : numeroDiasUteisMinimoNaoConfig;
                var considerouDiasUteisSubgrupo = false;
                int? diaSemanaEntrega = null;

                #region Busca a data de entrega mínima de acordo com os produtos do pedido

                if (idPedido > 0)
                {
                    var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(session, idPedido.Value);

                    // Etiqueta Processo
                    var diasDataEntregaProcesso = 0;
                    foreach (var pp in produtosPedido.Where(f => f.IdProcesso > 0).ToList())
                        diasDataEntregaProcesso = Math.Max(diasDataEntregaProcesso, EtiquetaProcessoDAO.Instance.ObterNumeroDiasUteisDataEntrega(session, pp.IdProcesso.Value));
                    // Considera a data maior entre a data das configurações e da data do processo.
                    numeroDiasSomar = Math.Max(numeroDiasSomar, diasDataEntregaProcesso);

                    // Subgrupo produto.
                    Dictionary<uint, KeyValuePair<int?, int?>> subgrupos = new Dictionary<uint, KeyValuePair<int?, int?>>();
                    foreach (ProdutosPedido pp in produtosPedido)
                        if (pp.IdSubgrupoProd > 0 && !subgrupos.ContainsKey(pp.IdSubgrupoProd))
                        {
                            subgrupos.Add(pp.IdSubgrupoProd, new KeyValuePair<int?, int?>(
                                SubgrupoProdDAO.Instance.ObtemValorCampo<int?>(session, "numeroDiasMinimoEntrega", "idSubgrupoProd=" + pp.IdSubgrupoProd),
                                SubgrupoProdDAO.Instance.ObtemValorCampo<int?>(session, "diaSemanaEntrega", "idSubgrupoProd=" + pp.IdSubgrupoProd)
                            ));
                        }

                    uint idSubgrupoMaiorPrazo = 0;
                    foreach (uint key in subgrupos.Keys)
                        if (subgrupos[key].Key > 0 || subgrupos[key].Value != null)
                            if (idSubgrupoMaiorPrazo == 0 || subgrupos[key].Key > subgrupos[idSubgrupoMaiorPrazo].Key)
                                idSubgrupoMaiorPrazo = key;

                    if (idSubgrupoMaiorPrazo > 0)
                    {
                        /* Chamado 54042. */
                        considerouDiasUteisSubgrupo = subgrupos[idSubgrupoMaiorPrazo].Key.GetValueOrDefault() > numeroDiasSomar;

                        numeroDiasSomar = Math.Max(numeroDiasSomar, subgrupos[idSubgrupoMaiorPrazo].Key.GetValueOrDefault());
                        diaSemanaEntrega = subgrupos[idSubgrupoMaiorPrazo].Value;
                        desabilitarCampo = desabilitarCampo && tipoPedido == (int)Pedido.TipoPedidoEnum.Venda;
                    }
                }

                #endregion

                // Se tiver permissão de ignorar bloqueios na data de entrega, não desabilita o campo
                desabilitarCampo = desabilitarCampo && !Config.PossuiPermissao<Config.FuncaoMenuPedido>(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega);

                // Calcula a data do fast delivery
                int j = 0;
                while (j < PedidoConfig.Pedido_FastDelivery.PrazoEntregaFastDelivery)
                {
                    dataFastDelivery = dataFastDelivery.AddDays(1);
                    while (!dataFastDelivery.DiaUtil())
                        dataFastDelivery = dataFastDelivery.AddDays(1);

                    j++;
                }

                if (PedidoConfig.Pedido_FastDelivery.ConsiderarTurnoFastDelivery && DateTime.Now.Hour >= 12 && DateTime.Now.Minute >= 1)
                {
                    dataFastDelivery = dataFastDelivery.AddDays(1);
                    while (!dataFastDelivery.DiaUtil())
                        dataFastDelivery = dataFastDelivery.AddDays(1);
                }

                var m2Pedido = idPedido > 0 ? ProdutosPedidoDAO.Instance.GetTotalM2ByPedido(session, idPedido.Value) : 0;

                // Calcula o fast delivery somente se o pedido for fast delivery (10651)
                if (PedidoConfig.Pedido_FastDelivery.FastDelivery && idPedido > 0 && (fastDelivery == true || IsFastDelivery(session, idPedido.Value)))
                {
                    // Se a metragem do pedido for maior que o total diário do fast delivery e se o pedido for importado, apenas não calcula a data do fast delivery
                    // chamado (67439)
                    if (m2Pedido > PedidoConfig.Pedido_FastDelivery.M2MaximoFastDelivery && !IsPedidoImportado(session, idPedido.Value))
                        dataFastDelivery = ProdutosPedidoDAO.Instance.GetFastDeliveryDay(session, idPedido.Value, dataFastDelivery, m2Pedido, false).GetValueOrDefault(dataFastDelivery);
                }

                if (numeroDiasSomar > 0)
                {
                    int i = 0;

                    while (i < numeroDiasSomar)
                    {
                        dataEntregaMinima = dataEntregaMinima.AddDays(1);
                        while (!dataEntregaMinima.DiaUtil())
                            dataEntregaMinima = dataEntregaMinima.AddDays(1);

                        i++;
                    }

                    if (diaSemanaEntrega != null)
                    {
                        while ((int)dataEntregaMinima.DayOfWeek != diaSemanaEntrega.Value)
                            dataEntregaMinima = dataEntregaMinima.AddDays(1);

                        if (diaSemanaEntrega != (int)DayOfWeek.Saturday && diaSemanaEntrega != (int)DayOfWeek.Sunday)
                            while (!dataEntregaMinima.DiaUtil())
                                dataEntregaMinima = dataEntregaMinima.AddDays(7);
                    }
                }

                bool valido = false;

                while (!valido)
                {
                    valido = true;

                    // Recalcula a data de entrega para que caia em um dia da rota
                    if (dataRota != null && ((idPedido == null && tipoEntrega.GetValueOrDefault(0) != (uint)Pedido.TipoEntregaPedido.Balcao) ||
                        (idPedido > 0 && (tipoEntrega ?? ObtemTipoEntrega(session, idPedido.Value)) != (int)Pedido.TipoEntregaPedido.Balcao)))
                    {
                        if (dataRota < dataEntregaMinima)
                            dataRota = RotaDAO.Instance.GetDataRota(session, idCli, dataEntregaMinima, !considerouDiasUteisSubgrupo);

                        valido = dataRota.Value.Date == dataEntregaMinima.Date;

                        dataEntregaMinima = dataRota.Value.Date;
                    }
                }

                return numeroDiasSomar > 0 || (dataRota != null && dataRota.Value.Date == dataEntregaMinima.Date);
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("CalculoDataMinima", ex);
                desabilitarCampo = false;
                dataEntregaMinima = dataBase ?? DateTime.Now;
                dataFastDelivery = dataEntregaMinima;
                return false;
            }
        }

        /// <summary>
        /// Recalcula a data de entrega do pedido baseando-se na data passada e atualiza o pedido
        /// </summary>
        public void RecalcularEAtualizarDataEntregaPedido(GDASession session, uint idPedido, DateTime? dataBase, out bool enviarMensagem)
        {
            uint idCliente = PedidoDAO.Instance.ObtemIdCliente(session, idPedido);
            int? tipoPedido = (int?)GetTipoPedido(session, idPedido);
            int? tipoEntrega = (int?)ObtemTipoEntrega(session, idPedido);
            DateTime dataEntregaMinima, dataFastDelivery;
            bool desabilitarCampo;
            var fastDelivey = IsFastDelivery(session, idPedido);
            enviarMensagem = false;

            GetDataEntregaMinima(session, idCliente, idPedido, tipoPedido, tipoEntrega, dataBase, out dataEntregaMinima,
                out dataFastDelivery, out desabilitarCampo, 0, fastDelivey);

            if (dataFastDelivery != DateTime.MinValue && dataEntregaMinima != DateTime.MinValue)
            {
                var dataEntregaAtual = PedidoDAO.Instance.ObtemDataEntrega(session, idPedido);

                if (fastDelivey)
                {
                    if (dataFastDelivery != dataEntregaAtual)
                    {
                        AtualizarDataEntrega(session, (int)idPedido, dataFastDelivery);
                        enviarMensagem = true;
                    }
                }
                else
                {
                    if (dataEntregaMinima != dataEntregaAtual)
                    {
                        AtualizarDataEntrega(session, (int)idPedido, dataEntregaMinima);
                        enviarMensagem = true;
                    }
                }
            }
        }
        #endregion

        #region Altera o desconto do pedido

        /// <summary>
        /// Atualiza os dados do pedido através da notinha verde.
        /// </summary>
        public void UpdateDescontoComTransacao(Pedido objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    UpdateDesconto(transaction, objUpdate, true);

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

        /// <summary>
        /// Atualiza os dados do pedido através da notinha verde.
        /// </summary>
        internal void UpdateDesconto(GDASession session, Pedido objUpdate, bool atualizarPedido)
        {
            FilaOperacoes.AtualizarPedido.AguardarVez();

            try
            {
                #region Declaração de variáveis

                var ped = GetElementByPrimaryKey(session, objUpdate.IdPedido);
                objUpdate = CarregarPedidoComDadosAtualizacao(session, objUpdate);

                objUpdate.DataEntrega = !PedidoConfig.LiberarPedido && !objUpdate.DataEntrega.HasValue ? ped.DataEntrega : objUpdate.DataEntrega;
                // Pega a data de entrega da fábrica.
                DateTime dataFabrica = PedidoEspelhoDAO.Instance.CalculaDataFabrica(objUpdate.DataEntrega.Value);
                var existeEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(session, objUpdate.IdPedido);
                var atualizarSomenteDataEntrega = !Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoConfirmadoListaPedidos) &&
                    !Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoCofirmadoListaPedidosVendedor) &&
                    Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarDataEntregaPedidoListaPedidos);
                var aplicarDesconto = ((!atualizarPedido && objUpdate.Desconto > 0) ||
                    (atualizarPedido && (ped.TipoDesconto != objUpdate.TipoDesconto || ped.Desconto != objUpdate.Desconto))) && PedidoConfig.LiberarPedido;
                var aplicarAcrescimo = ((!atualizarPedido && objUpdate.Acrescimo > 0) ||
                    (atualizarPedido && (ped.TipoAcrescimo != objUpdate.TipoAcrescimo || ped.Acrescimo != objUpdate.Acrescimo))) && PedidoConfig.LiberarPedido;
                // Recupera a obra ainda com as informações do pedido, para que o log dela seja criado corretamente.
                var obraAtual = ped.IdObra > 0 ? ObraDAO.Instance.GetElement(session, ped.IdObra.Value) : null;
                var obraNova = objUpdate.IdObra > 0 ? ObraDAO.Instance.GetElement(session, objUpdate.IdObra.Value) : null;
                var isPedidoProducao = IsProducao(session, objUpdate.IdPedido);
                var idPedidoRevenda = ObterIdPedidoRevenda(session, (int)objUpdate.IdPedido);
                var pedidoPossuiOrdemCarga = PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, objUpdate.IdPedido);

                #endregion

                #region Validações do pedido

                if (PedidoConfig.LiberarPedido && ped.Situacao == Pedido.SituacaoPedido.Confirmado)
                {
                    throw new Exception("Esse pedido já foi liberado.");
                }

                if (ped.IdLoja != objUpdate.IdLoja)
                {
                    var idsLojaSubgrupoProd = ProdutosPedidoDAO.Instance.ObterIdsLojaSubgrupoProdPeloPedido(session, (int)objUpdate.IdPedido);
                    var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(session, objUpdate.IdPedido);

                    if (idsLojaSubgrupoProd.Count > 0 && !idsLojaSubgrupoProd.Contains((int)objUpdate.IdLoja))
                    {
                        throw new Exception("Não é possível alterar a loja deste pedido, a loja cadastrada para o subgrupo de um ou mais produtos é diferente da loja selecionada para o pedido.");
                    }

                    foreach (var prodPed in produtosPedido)
                    {
                        if (GrupoProdDAO.Instance.BloquearEstoque(session, (int)prodPed.IdGrupoProd, (int)prodPed.IdSubgrupoProd))
                        {
                            var estoque = ProdutoLojaDAO.Instance.GetEstoque(session, objUpdate.IdLoja, prodPed.IdProd, null, isPedidoProducao, false, true);

                            if (estoque < produtosPedido.Where(f => f.IdProd == prodPed.IdProd).Sum(f => f.Qtde))
                            {
                                throw new Exception(string.Format("O produto {0} possui apenas {1} em estoque na loja selecionada.", prodPed.DescrProduto, estoque));
                            }
                        }
                    }
                }

                if (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo && objUpdate.IdFormaPagto.GetValueOrDefault() == 0)
                {
                    throw new Exception("Selecione a forma de pagamento do pedido");
                }

                /* Chamado 65135. */
                if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto && objUpdate.IdFormaPagto.GetValueOrDefault() == 0 &&
                    (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.AVista || objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo))
                {
                    throw new Exception("Não é possível atualizar os dados do pedido, pois a forma de pagamento não foi selecionada.");
                }

                // Verifica se o desconto que já exista no pedido pode ser mantido pelo usuário que está atualizando o pedido, 
                // tendo em vista que o mesmo possa ter sido lançado por um administrador.
                if (objUpdate.Desconto == ped.Desconto && ped.Desconto > 0 && !DescontoPermitido(session, objUpdate))
                {
                    throw new Exception("O desconto lançado anteriormente está acima do permitido para este login.");
                }

                /* Chamado 43090. */
                if (objUpdate.ValorEntrada < 0)
                {
                    throw new Exception("O valor de entrada do pedido não pode ser negativo. Edite o pedido, atualize os valores e tente novamente.");
                }

                #endregion

                #region Validações de cliente

                // Verifica se este pedido pode ter desconto.
                if (PedidoConfig.Desconto.ImpedirDescontoSomativo && UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador && objUpdate.Desconto > 0)
                {
                    var clientePossuiDesconto = DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(session, ped.IdCli, 0, null, objUpdate.IdPedido, null);

                    if (clientePossuiDesconto)
                    {
                        throw new Exception("O cliente já possui desconto por grupo/subgrupo, não é permitido lançar outro desconto.");
                    }
                }

                #endregion

                #region Validações de obra

                if (objUpdate.IdObra > 0 && (Obra.SituacaoObra)obraNova.Situacao != Obra.SituacaoObra.Confirmada)
                {
                    throw new Exception("A obra informada não está confirmada.");
                }

                if (Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas && ped.TipoVenda == (int)Pedido.TipoVendaPedido.Obra && ped.IdObra.GetValueOrDefault() > 0)
                {
                    var idFunc = ObraDAO.Instance.ObtemIdFunc(session, ped.IdObra.Value);
                    var idLojaFunc = FuncionarioDAO.Instance.ObtemIdLoja(session, idFunc);
                    var idCliente = ObraDAO.Instance.ObtemIdCliente(session, (int)ped.IdObra.Value);
                    var idLojaCliente = ClienteDAO.Instance.ObtemIdLoja(session, (uint)idCliente);

                    if (Geral.ConsiderarLojaClientePedidoFluxoSistema && idLojaCliente > 0)
                    {
                        if (idLojaCliente != ped.IdLoja)
                        {
                            throw new Exception("A loja da obra informada é diferente da loja do pedido.");
                        }
                    }
                    else if (idLojaFunc != ped.IdLoja)
                    {
                        throw new Exception("A loja da obra informada é diferente da loja do pedido.");
                    }
                }

                if (PedidoConfig.DadosPedido.UsarControleNovoObra)
                {
                    // Não pode modificar o tipo de venda para obra, pois além de não inserir o valor do pagamento antecipado do pedido
                    // teria que fazer todas as validações que faz quando o pedido é de obra ao inserir produtos no pedido.
                    if (ped.TipoVenda != (int)Pedido.TipoVendaPedido.Obra && objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                    {
                        throw new Exception("Não é possível alterar o tipo de venda para obra, é necessário definir o tipo de venda obra antes de inserir produtos no pedido.");
                    }

                    // Chamado 12792. Não pode modificar o tipo de obra para outro tipo.
                    if (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Obra && objUpdate.TipoVenda != (int)Pedido.TipoVendaPedido.Obra)
                    {
                        throw new Exception("Não é possível alterar o tipo de venda, como o tipo de venda é Obra o mesmo pode ser alterado somente se o pedido estiver sem produtos.");
                    }

                    if (objUpdate.IdObra > 0 && objUpdate.Acrescimo > 0)
                    {
                        throw new Exception("Não é permitido lançar acréscimo em pedidos de obra, pois o valor do m² já foi definido com o cliente.");
                    }

                    if (objUpdate.IdObra > 0 && objUpdate.Desconto > 0)
                    {
                        throw new Exception("Não é permitido lançar desconto em pedidos de obra, pois o valor do m² já foi definido com o cliente.");
                    }
                }

                #endregion

                #region Validações de ordem de carga

                if (OrdemCargaConfig.UsarControleOrdemCarga)
                {
                    // Valida a alteração no tipo de entrega.
                    if (ped.TipoEntrega != objUpdate.TipoEntrega && ped.TipoEntrega == DataSources.Instance.GetTipoEntregaEntrega())
                    {
                        var pedidoPossuiVolume = TemVolume(session, objUpdate.IdPedido);

                        if (pedidoPossuiVolume || pedidoPossuiOrdemCarga)
                        {
                            throw new Exception("Não é possível alterar o tipo de entrega desse pedido pois ele já possui volume ou OC gerados. Cancele-os antes de prosseguir.");
                        }
                    }

                    if (pedidoPossuiOrdemCarga && ped.DeveTransferir != objUpdate.DeveTransferir)
                    {
                        throw new Exception("Não é possível alterar a opção Deve Transferir desse pedido pois ele já esta vinculado a uma OC.");
                    }

                    if (pedidoPossuiOrdemCarga && ped.IdLoja != objUpdate.IdLoja)
                    {
                        throw new Exception("Não é possível alterar a loja desse pedido pois ele já esta vinculado a uma OC.");
                    }
                }

                #endregion

                #region Atualização dos pedidos de produção associados

                // Valida a alteração no tipo de entrega
                if (ped.TipoEntrega != objUpdate.TipoEntrega)
                {
                    /* Chamado 48370. */
                    if (ObterIdsPedidoProducaoPeloIdPedidoRevenda(session, (int)objUpdate.IdPedido).Count > 0)
                    {
                        objPersistence.ExecuteCommand(session, "UPDATE pedido SET TipoEntrega=?tipoEntrega WHERE IdPedidoRevenda=?idPedidoRevenda",
                            new GDAParameter("?tipoEntrega", objUpdate.TipoEntrega), new GDAParameter("?idPedidoRevenda", objUpdate.IdPedido));
                    }
                }

                #endregion

                #region Atualização dos pedidos de produção associados

                // Caso o tipo de entrega tenha sido alterado e o pedido seja de produção vinculado a um de revenda atualiza o tipo entrega
                // do pedido de revenda tambem, caso o mesmo não esteja liberado.
                if (ped.TipoEntrega != objUpdate.TipoEntrega && isPedidoProducao && idPedidoRevenda > 0)
                {
                    var situacaoPedidoRevenda = ObtemSituacao(session, (uint)idPedidoRevenda.Value);

                    if (situacaoPedidoRevenda == Pedido.SituacaoPedido.Confirmado || situacaoPedidoRevenda == Pedido.SituacaoPedido.LiberadoParcialmente)
                    {
                        throw new Exception("Não é possível alterar o tipo de entrega desse pedido pois o pedido de revenda já está liberado");
                    }

                    // Atualiza o tipo de entrega do pedido de revenda.
                    objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET TipoEntrega={0} WHERE IdPedido={1}", objUpdate.TipoEntrega, idPedidoRevenda.Value));
                }

                #endregion

                #region Atualização da data de entrega

                // Altera a data de entrega e data de fábrica da conferência
                if (objUpdate.DataEntrega != null && objUpdate.DataEntrega != ped.DataEntrega)
                {
                    if (!existeEspelho)
                    {
                        VerificaCapacidadeProducaoSetor(session, objUpdate.IdPedido, objUpdate.DataEntrega.Value, 0, 0);
                    }
                    else
                    {
                        PedidoEspelhoDAO.Instance.VerificaCapacidadeProducaoSetor(session, objUpdate.IdPedido, dataFabrica, 0, 0);
                    }

                    if (DateTime.Now > objUpdate.DataEntrega)
                        throw new Exception("A data selecionada não pode ser inferior a " + DateTime.Now.ToShortDateString());

                    // Atualiza a data de entrega do pedido.
                    objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET DataEntrega=?dataEntrega WHERE IdPedido={0}", objUpdate.IdPedido),
                        new GDAParameter("?dataEntrega", objUpdate.DataEntrega.Value));

                    // Salva os dados do Log
                    /* Chamado 53869.
                     * Caso o pedido não seja atualizado, o log não é gerado.
                     * Caso somente a data de entrega seja atualizada, o método é parado ao final dessa region. */
                    if (atualizarSomenteDataEntrega || !atualizarPedido)
                    {
                        LogAlteracaoDAO.Instance.LogPedido(session, ped, GetElementByPrimaryKey(session, ped.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
                    }

                    // Atualiza a data de entrega da fábrica.
                    objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido_espelho SET DataFabrica=?dataFabrica WHERE IdPedido={0}", objUpdate.IdPedido),
                        new GDAParameter("?dataFabrica", dataFabrica));

                    if (existeEspelho)
                    {
                        // Atualiza a data de entrega da fábrica.
                        objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido_espelho SET DataFabrica=?dataFabrica WHERE IdPedido={0}", objUpdate.IdPedido),
                            new GDAParameter("?dataFabrica", dataFabrica));
                        LogAlteracaoDAO.Instance.LogPedidoEspelho(session, PedidoEspelhoDAO.Instance.GetElementByPrimaryKey(session, ped.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Novo);
                    }
                }

                // Altera somente a data de entrega.
                if (atualizarSomenteDataEntrega)
                {
                    return;
                }

                #endregion

                #region Atualização dos dados do pedido

                // Atualiza o valor do frete.
                objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET ValorEntrega=?valorEntrega WHERE IdPedido={0}", objUpdate.IdPedido),
                    new GDAParameter("?valorEntrega", objUpdate.ValorEntrega));

                if (ped.IdTransportador != objUpdate.IdTransportador)
                {
                    objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET IdTransportador={0} WHERE IdPedido={1}", objUpdate.IdTransportador, objUpdate.IdPedido));
                }

                objUpdate.ObsLiberacao = string.IsNullOrEmpty(objUpdate.ObsLiberacao) ? null :
                    objUpdate.ObsLiberacao.Length > 300 ? string.Format("{0}...", objUpdate.ObsLiberacao.Substring(0, 297)) : objUpdate.ObsLiberacao;

                // Verifica se o código do pedido do cliente já foi cadastrado.
                if (objUpdate.TipoVenda < 3)
                {
                    CodigoClienteUsado(session, objUpdate.IdPedido, objUpdate.IdPedido, objUpdate.CodCliente, false);
                }

                if (atualizarPedido)
                {
                    #region Declaração de variáveis

                    // Estas duas primeiras condições vão de acordo com a detail view selecionada para ser alterada no popup de desconto de pedido,
                    // alterando-as deve alterar nesta tela também (Utils/DescontoPedido.aspx.cs).
                    var alterarSomenteObsDesconto = (UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor &&
                        !Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoCofirmadoListaPedidosVendedor)) ||
                        (PedidoConfig.DescontoPedidoVendedorUmProduto && UserInfo.GetUserInfo.CodUser == objUpdate.IdFunc &&
                        UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor &&
                        !Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoConfirmadoListaPedidos) &&
                        !Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoCofirmadoListaPedidosVendedor));
                    var alterarVendedor = Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarVendedorPedido);
                    var sqlAtualizacaoDadosPedido = string.Empty;

                    #endregion

                    #region Atualização do acréscimo e desconto do pedido

                    var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedidoLite(session, objUpdate.IdPedido, false, true);

                    if (aplicarDesconto)
                    {
                        RemoverDesconto(session, objUpdate, produtosPedido);
                        AplicarDesconto(session, objUpdate, objUpdate.TipoDesconto, objUpdate.Desconto, produtosPedido);
                    }

                    if (aplicarAcrescimo)
                    {
                        RemoverAcrescimo(session, objUpdate, produtosPedido);
                        AplicarAcrescimo(session, objUpdate, objUpdate.TipoAcrescimo, objUpdate.Acrescimo, produtosPedido);
                    }

                    if (objUpdate.Desconto != ped.Desconto && PedidoConfig.Desconto.GetDescontoMaximoPedido(session, UserInfo.GetUserInfo.CodUser, (int)objUpdate.TipoVenda, (int)objUpdate.IdParcela) != 100)
                    {
                        objUpdate.IdFuncDesc = null;
                        objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET idFuncDesc=NULL WHERE IdPedido={0}", objUpdate.IdPedido));
                    }

                    bool descontoRemovido = !DescontoPermitido(session, objUpdate);
                    if (descontoRemovido)
                    {
                        RemoverDesconto(session, objUpdate, produtosPedido);
                        objUpdate.Desconto = 0;
                    }

                    FinalizarAplicacaoComissaoAcrescimoDesconto(session, objUpdate, produtosPedido,
                        aplicarDesconto || aplicarAcrescimo || descontoRemovido);

                    #endregion

                    #region Atualização dos dados do pedido

                    #region Tipo Venda

                    if (!alterarSomenteObsDesconto && objUpdate.TipoVenda.GetValueOrDefault() == 0)
                    {
                        // Se o tipo de venda for nulo então o tipo de venda não estava disponível para ser selecionado na tela, sendo assim o tipo de venda deve ser mantido.
                        if (objUpdate.TipoVenda == null)
                        {
                            objUpdate.TipoVenda = ObtemTipoVenda(session, objUpdate.IdPedido);

                            //Chamado 37640 : se o pedido tiver aberto em uma segunda tela de alterações o sistema estava mantendo o idobra e atualizando em cima de outra atualização. essa alteração impede que isso ocorra
                            if (objUpdate.TipoVenda != (int)Pedido.TipoVendaPedido.Obra)
                            {
                                objUpdate.IdObra = null;
                                objUpdate.ValorPagamentoAntecipado = 0;
                            }
                        }

                        if (objUpdate.TipoVenda.GetValueOrDefault() == 0)
                        {
                            throw new Exception("Informe o tipo de venda");
                        }
                    }

                    #endregion

                    #region Obra

                    //Se o pagamento era obra e nao é mais remove as referencias da obra.
                    if (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Obra && objUpdate.TipoVenda != (int)Pedido.TipoVendaPedido.Obra)
                    {
                        objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET IdObra=NULL, ValorPagamentoAntecipado=NULL WHERE IdPedido={0}", objUpdate.IdPedido));

                        /* Chamado 51738.
                         * O ID da obra estava sendo removido do pedido através do SQL acima, mas no SQL abaixo ele era adicionado novamente. */
                        objUpdate.IdObra = null;
                        objUpdate.ValorPagamentoAntecipado = 0;

                        if (ped.IdObra > 0)
                        {
                            ObraDAO.Instance.AtualizaSaldo(session, obraAtual, obraAtual.IdObra, false, false);
                        }
                    }

                    #endregion

                    #region Fast delivery

                    if (!objUpdate.FastDelivery)
                    {
                        objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET TaxaFastDelivery=0 WHERE IdPedido={0}", objUpdate.IdPedido));
                    }

                    #endregion

                    if (!PedidoConfig.LiberarPedido)
                    {
                        sqlAtualizacaoDadosPedido = string.Format(@"UPDATE pedido SET
                                Obs=?obs,
                                PercentualComissao=?percentualComissao
                            WHERE IdPedido={0}", objUpdate.IdPedido);
                    }
                    else if (!alterarSomenteObsDesconto)
                    {
                        sqlAtualizacaoDadosPedido = string.Format(@"UPDATE pedido SET
                                Acrescimo=?acrescimo,
                                CodCliente=?codCliente,
                                Desconto=?desconto,
                                DeveTransferir=?deveTransferir,
                                FastDelivery=?fastDelivery,
                                IdFormaPagto=?idFormaPagto,
                                IdFunc=?idFunc,
                                IdFuncVenda=?idFuncVenda,
                                IdLoja=?idLoja,
                                IdObra=?idObra,
                                IdParcela=?idParcela,
                                IdTipoCartao=?idTipoCartao,
                                NumParc=?numParc,
                                Obs=?obs,
                                ObsLiberacao=?obsLiberacao,
                                OrdemCargaParcial=?ordemCargaParcial,
                                PercentualComissao=?percentualComissao,
                                TipoAcrescimo=?tipoAcrescimo,
                                TipoDesconto=?tipoDesconto,
                                TipoEntrega=?tipoEntrega,
                                TipoVenda=?tipoVenda,
                                ValorEntrada=?valorEntrada
                            WHERE IdPedido={0}", objUpdate.IdPedido);
                    }
                    else if (alterarVendedor)
                    {
                        sqlAtualizacaoDadosPedido = string.Format(@"UPDATE pedido SET
                                Desconto=?desconto,
                                IdFunc=?idFunc,
                                Obs=?obs,
                                ObsLiberacao=?obsLiberacao,
                                TipoDesconto=?tipoDesconto
                            WHERE IdPedido={0}", objUpdate.IdPedido);
                    }
                    else
                    {
                        sqlAtualizacaoDadosPedido = string.Format(@"UPDATE pedido SET
                                Desconto=?desconto,
                                Obs=?obs,
                                ObsLiberacao=?obsLiberacao,
                                TipoDesconto=?tipoDesconto
                            WHERE IdPedido={0}", objUpdate.IdPedido);
                    }

                    // Atualiza os dados do pedido.
                    objPersistence.ExecuteCommand(session, sqlAtualizacaoDadosPedido,
                        new GDAParameter("?acrescimo", objUpdate.Acrescimo),
                        new GDAParameter("?codCliente", objUpdate.CodCliente),
                        new GDAParameter("?desconto", objUpdate.Desconto),
                        new GDAParameter("?deveTransferir", objUpdate.DeveTransferir),
                        new GDAParameter("?fastDelivery", objUpdate.FastDelivery),
                        new GDAParameter("?idFormaPagto", objUpdate.IdFormaPagto),
                        new GDAParameter("?idFunc", objUpdate.IdFunc),
                        new GDAParameter("?idFuncVenda", objUpdate.IdFuncVenda),
                        new GDAParameter("?idLoja", objUpdate.IdLoja),
                        new GDAParameter("?idObra", objUpdate.IdObra),
                        new GDAParameter("?idParcela", objUpdate.IdParcela),
                        new GDAParameter("?idTipoCartao", objUpdate.IdTipoCartao),
                        new GDAParameter("?numParc", objUpdate.NumParc),
                        new GDAParameter("?obsLiberacao", objUpdate.ObsLiberacao),
                        new GDAParameter("?obs", objUpdate.Obs),
                        new GDAParameter("?ordemCargaParcial", objUpdate.OrdemCargaParcial),
                        new GDAParameter("?percentualComissao", objUpdate.PercentualComissao),
                        new GDAParameter("?tipoAcrescimo", objUpdate.TipoAcrescimo),
                        new GDAParameter("?tipoDesconto", objUpdate.TipoDesconto),
                        new GDAParameter("?tipoEntrega", objUpdate.TipoEntrega),
                        new GDAParameter("?tipoVenda", objUpdate.TipoVenda),
                        new GDAParameter("?valorEntrada", objUpdate.ValorEntrada));

                    #endregion

                    #region Atualização de estoque reserva/liberação

                    if (ped.IdLoja != objUpdate.IdLoja)
                    {
                        if (!ped.Producao)
                        {
                            var idsProdQtde = new Dictionary<int, float>();

                            foreach (var prodPed in ProdutosPedidoDAO.Instance.GetByPedido(session, ped.IdPedido))
                            {
                                var tipoCalc = GrupoProdDAO.Instance.TipoCalculo(session, (int)prodPed.IdProd);
                                var m2 = tipoCalc == (int)TipoCalculoGrupoProd.M2 || tipoCalc == (int)TipoCalculoGrupoProd.M2Direto;
                                var qtdEstornoEstoque = prodPed.Qtde;

                                if (tipoCalc == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL05 ||
                                    tipoCalc == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL6)
                                {
                                    qtdEstornoEstoque = prodPed.Qtde * prodPed.Altura;
                                }

                                // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque

                                if (!idsProdQtde.ContainsKey((int)prodPed.IdProd))
                                {
                                    idsProdQtde.Add((int)prodPed.IdProd, m2 ? prodPed.TotM : qtdEstornoEstoque);
                                }
                                else
                                {
                                    idsProdQtde[(int)prodPed.IdProd] += m2 ? prodPed.TotM : qtdEstornoEstoque;
                                }

                            }

                            ProdutoLojaDAO.Instance.ColocarReserva(session, (int)objUpdate.IdLoja, idsProdQtde, null, null, null, null, (int)ped.IdPedido, null, null, "PedidoDAO - UpdateDesconto");
                            ProdutoLojaDAO.Instance.TirarReserva(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null, (int)ped.IdPedido, null, null, "PedidoDAO - UpdateDesconto");
                        }
                    }

                    #endregion

                    // Salva os dados do Log.
                    LogAlteracaoDAO.Instance.LogPedido(session, ped, GetElementByPrimaryKey(session, ped.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
                }

                #endregion

                #region Atualização dos totais do pedido espelho

                // Atualiza os dados do pedido espelho.
                if (existeEspelho)
                {
                    var pedidoEspelho = PedidoEspelhoDAO.Instance.GetElement(session, objUpdate.IdPedido);
                    var produtosPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, pedidoEspelho.IdPedido, false, false, true);

                    if (aplicarDesconto)
                    {
                        PedidoEspelhoDAO.Instance.RemoverDesconto(session, pedidoEspelho, produtosPedidoEspelho);
                        PedidoEspelhoDAO.Instance.AplicarDesconto(session, pedidoEspelho, objUpdate.TipoDesconto, objUpdate.Desconto, produtosPedidoEspelho);
                    }

                    if (aplicarAcrescimo)
                    {
                        PedidoEspelhoDAO.Instance.RemoverAcrescimo(session, pedidoEspelho, produtosPedidoEspelho);

                        // Aplica acréscimo nos clones, desde que existam (MUITO IMPORTANTE APLICAR ANTES DE APLICAR NO ESPELHO LOGO ABAIXO, 
                        // porque o "BENEFICIAMENTOS" da model de pedidos está buscando os beneficiamentos do produtos_pedido_espelho, que da forma 
                        // como está agora ainda não tem acréscimo aplicado).
                        if (ExecuteScalar<bool>(session, string.Format("SELECT COUNT(*)>0 FROM produtos_pedido WHERE IdPedido={0} AND (InvisivelFluxo=1 OR InvisivelPedido=1)", objUpdate.IdPedido)))
                        {
                            var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedidoLite(session, objUpdate.IdPedido, true, true);
                            bool aplicado = AplicarAcrescimo(session, objUpdate, objUpdate.TipoAcrescimo, objUpdate.Acrescimo, produtosPedido);
                            FinalizarAplicacaoComissaoAcrescimoDesconto(session, objUpdate, produtosPedido, aplicado);
                        }

                        PedidoEspelhoDAO.Instance.AplicarAcrescimo(session, pedidoEspelho, objUpdate.TipoAcrescimo, objUpdate.Acrescimo, produtosPedidoEspelho);
                    }

                    PedidoEspelhoDAO.Instance.FinalizarAplicacaoComissaoAcrescimoDesconto(session, pedidoEspelho,
                        produtosPedidoEspelho, aplicarDesconto || aplicarAcrescimo);

                    objPersistence.ExecuteCommand(session, string.Format(@"UPDATE pedido_espelho SET
                            Acrescimo=?acrescimo,
                            Desconto=?desconto,
                            TipoAcrescimo=?tipoAcrescimo,
                            TipoDesconto=?tipoDesconto
                        WHERE IdPedido={0}", objUpdate.IdPedido), new GDAParameter("?acrescimo", objUpdate.Acrescimo), new GDAParameter("?desconto", objUpdate.Desconto),
                        new GDAParameter("?tipoAcrescimo", objUpdate.TipoAcrescimo), new GDAParameter("?tipoDesconto", objUpdate.TipoDesconto));

                    // Atualiza o valor do frete.
                    objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido_espelho SET ValorEntrega=?valorEntrega WHERE IdPedido={0}", objUpdate.IdPedido),
                        new GDAParameter("?valorEntrega", objUpdate.ValorEntrega));

                    pedidoEspelho.Acrescimo = objUpdate.Acrescimo;
                    pedidoEspelho.TipoAcrescimo = objUpdate.TipoAcrescimo;
                    pedidoEspelho.Desconto = objUpdate.Desconto;
                    pedidoEspelho.TipoDesconto = objUpdate.TipoDesconto;
                    pedidoEspelho.ValorEntrega = objUpdate.ValorEntrega;

                    PedidoEspelhoDAO.Instance.UpdateTotalPedido(session, pedidoEspelho);
                }

                #endregion

                #region Atualiza preço dos produtos do pedido e dos produtos do pedido espelho

                // Primeiramente, a empresa não pode trabalhar com produtos na obra ou o pedido não pode estar associado a uma obra, o inverso dessa condição indica que o pedido está associado a uma obra
                // e que os produtos dele possuem um preço definido previamente no cadastro da mesma.
                // Atualiza os valores dos produtos caso o tipo de entrega ou o cliente sejam alterados.
                // Atualiza os valores dos produtos caso a empresa utilize tabela de desconto/acréscimo e uma das condições a seguir sejam verdadeiras:
                // Tipo de venda alterado, forma de pagamento alterada ou parcela alterada.
                if ((objUpdate.IdObra.GetValueOrDefault() == 0 || !PedidoConfig.DadosPedido.UsarControleNovoObra) &&
                    ((ped.TipoEntrega != objUpdate.TipoEntrega || ped.IdCli != objUpdate.IdCli) ||
                    (PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista && (ped.TipoVenda != objUpdate.TipoVenda || objUpdate.IdFormaPagto != ped.IdFormaPagto || objUpdate.IdParcela != ped.IdParcela))))
                {
                    var situacaoPedidoEspelho = existeEspelho ? PedidoEspelhoDAO.Instance.ObtemSituacao(session, objUpdate.IdPedido) : (PedidoEspelho.SituacaoPedido?)null;
                    var podeAtualizar = situacaoPedidoEspelho == null ||
                        situacaoPedidoEspelho == PedidoEspelho.SituacaoPedido.Processando ||
                        situacaoPedidoEspelho == PedidoEspelho.SituacaoPedido.Aberto ||
                        situacaoPedidoEspelho == PedidoEspelho.SituacaoPedido.ImpressoComum;

                    if (podeAtualizar)
                    {
                        AtualizarValorTabelaProdutosPedido(session, aplicarDesconto, ped, objUpdate);

                        if (existeEspelho)
                        {
                            PedidoEspelhoDAO.Instance.AtualizarValorTabelaProdutosPedidoEspelho(session, ped, objUpdate);
                        }
                    }
                }

                #endregion

                #region Atualização do total do pedido

                UpdateTotalPedido(session, objUpdate, false, false, aplicarDesconto, true);

                #endregion

                #region Atualização das parcelas do pedido

                if (objUpdate.ValoresParcelas != null)
                {
                    RecalculaParcelas(session, ref objUpdate, TipoCalculoParcelas.Valor);
                    SalvarParcelas(session, objUpdate);
                }

                #endregion                    

                #region Atualização da obra

                // O saldo da obra sempre deve ser atualizado, pois, caso seja aplicado um desconto no pedido o valor total será recalculado
                // e, da mesma forma, o valor do pagamento antecipado também deve ser recalculado.
                // Se o pedido passou a ser de obra, atualiza os dados do mesmo.
                // Verifica se o tipo de venda do pedido foi alterado para obra nesta atualização.
                if (ped.TipoVenda != (int)Pedido.TipoVendaPedido.Obra && objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                {
                    AtualizaSaldoObra(session, objUpdate.IdPedido, obraNova, obraNova.IdObra, objUpdate.Total, 0, false);
                }
                else if (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                {
                    /* Chamado 51738.
                     * Verifica se a obra associada ao pedido foi alterada. */
                    if (ped.IdObra > 0 && objUpdate.IdObra > 0 && ped.IdObra != objUpdate.IdObra)
                    {
                        // Atualiza o saldo da obra atual para que os valores fiquem corretos.
                        ObraDAO.Instance.AtualizaSaldo(session, obraAtual, obraAtual.IdObra, false, false);
                        // Atualiza o saldo da obra nova para que os valores fiquem corretos.
                        AtualizaSaldoObra(session, objUpdate.IdPedido, obraNova, obraNova.IdObra, objUpdate.Total, 0, false);
                    }
                    else
                    {
                        /* Chamado 63233. */
                        AtualizaSaldoObra(session, objUpdate.IdPedido, obraNova, obraNova.IdObra, objUpdate.Total, ped.Total, true);
                    }
                }

                #endregion
            }
            finally
            {
                FilaOperacoes.AtualizarPedido.ProximoFila();
            }
        }

        private Pedido CarregarPedidoComDadosAtualizacao(GDASession sessao, Pedido pedido)
        {
            var pedidoNovo = GetElementByPrimaryKey(sessao, pedido.IdPedido);

            pedidoNovo.Acrescimo = pedido.Acrescimo;
            pedidoNovo.CodCliente = pedido.CodCliente;
            pedidoNovo.Desconto = pedido.Desconto;
            pedidoNovo.DeveTransferir = pedido.DeveTransferir;
            pedidoNovo.FastDelivery = pedido.FastDelivery;
            pedidoNovo.IdFormaPagto = pedido.IdFormaPagto;
            pedidoNovo.IdFunc = pedido.IdFunc;
            pedidoNovo.IdFuncVenda = pedido.IdFuncVenda;
            pedidoNovo.IdLoja = pedido.IdLoja;
            pedidoNovo.IdObra = pedido.IdObra;
            pedidoNovo.IdParcela = pedido.IdParcela;
            pedidoNovo.IdTipoCartao = pedido.IdTipoCartao;
            pedidoNovo.NumParc = pedido.NumParc;
            pedidoNovo.ObsLiberacao = pedido.ObsLiberacao;
            pedidoNovo.Obs = pedido.Obs;
            pedidoNovo.OrdemCargaParcial = pedido.OrdemCargaParcial;
            pedidoNovo.PercentualComissao = pedido.PercentualComissao;
            pedidoNovo.TipoAcrescimo = pedido.TipoAcrescimo;
            pedidoNovo.TipoDesconto = pedido.TipoDesconto;
            pedidoNovo.TipoEntrega = pedido.TipoEntrega;
            pedidoNovo.TipoVenda = pedido.TipoVenda;
            pedidoNovo.ValorEntrada = pedido.ValorEntrada;
            pedidoNovo.IdComissionado = pedido.IdComissionado;
            pedidoNovo.PercComissao = pedido.PercComissao;
            pedidoNovo.DataEntrega = pedido.DataEntrega;

            return pedidoNovo;
        }

        #endregion

        #region Recupera o ID da obra de um pedido

        public uint? GetIdObra(uint idPedido)
        {
            return GetIdObra(null, idPedido);
        }

        /// <summary>
        /// Recupera o ID da obra de um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint? GetIdObra(GDASession sessao, uint idPedido)
        {
            object retorno = objPersistence.ExecuteScalar(sessao, "select idObra from pedido where idPedido=" + idPedido);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (uint?)Glass.Conversoes.StrParaUint(retorno.ToString()) : null;
        }

        #endregion

        #region Recupera o ID do cliente de um pedido

        /// <summary>
        /// Recupera o ID do cliente de um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint GetIdCliente(uint idPedido)
        {
            return GetIdCliente(null, idPedido);
        }

        /// <summary>
        /// Recupera o ID do cliente de um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint GetIdCliente(GDASession sessao, uint idPedido)
        {
            object retorno = objPersistence.ExecuteScalar(sessao, "select coalesce(idCli,0) from pedido where idPedido=" + idPedido);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? Glass.Conversoes.StrParaUint(retorno.ToString()) : 0;
        }

        #endregion

        #region Altera os dados do pedido de um parceiro

        /// <summary>
        /// Altera os dados do pedido de um parceiro.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="codPedCli"></param>
        /// <param name="valorEntrada"></param>
        /// <param name="obs"></param>
        public void UpdateParceiro(uint idPedido, string codPedCli, string valorEntrada, string obs, string obsLib, int? idTransportador)
        {
            UpdateParceiro(null, idPedido, codPedCli, valorEntrada, obs, obsLib, idTransportador);
        }

        /// <summary>
        /// Altera os dados do pedido de um parceiro.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <param name="codPedCli"></param>
        /// <param name="valorEntrada"></param>
        /// <param name="obs"></param>
        public void UpdateParceiro(GDASession sessao, uint idPedido, string codPedCli, string valorEntrada, string obs, string obsLib, int? idTransportador)
        {
            string sql = "update pedido set codCliente=?codPedCli, obs=?obs, ObsLiberacao=?obsLib{0}, IdTransportador=?idTransp where idPedido=" + idPedido;

            var lstParam = new List<GDAParameter>();
            lstParam.Add(new GDAParameter("?codPedCli", codPedCli));
            lstParam.Add(new GDAParameter("?obs", obs));
            lstParam.Add(new GDAParameter("?obsLib", obsLib));
            lstParam.Add(new GDAParameter("?idTransp", idTransportador));

            if (!String.IsNullOrEmpty(valorEntrada))
            {
                sql = String.Format(sql, ", valorEntrada=?valorEntrada");
                lstParam.Add(new GDAParameter("?valorEntrada", valorEntrada));
            }
            else
                sql = String.Format(sql, String.Empty);

            objPersistence.ExecuteCommand(sessao, sql, lstParam.ToArray());
        }

        /// <summary>
        /// Gera parcelas padrão do cliente parceiro
        /// </summary>
        /// <param name="ped"></param>
        public void GeraParcelaParceiro(ref Pedido ped)
        {
            GeraParcelaParceiro(null, ref ped);
        }

        /// <summary>
        /// Gera parcelas padrão do cliente parceiro
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="ped"></param>
        public void GeraParcelaParceiro(GDASession sessao, ref Pedido ped)
        {
            Parcelas parc = ParcelasDAO.Instance.GetPadraoCliente(sessao, ped.IdCli);

            if (parc != null && parc.NumParcelas > 0)
            {
                ped.NumParc = parc.NumParcelas;
                ped.ValoresParcelas = new decimal[parc.NumParcelas];
                ped.DatasParcelas = new DateTime[parc.NumParcelas];

                decimal valorParc = Math.Round((ped.Total - ped.ValorEntrada) / parc.NumParcelas, 2);
                decimal soma = 0;
                for (int i = 0; i < parc.NumParcelas; i++)
                {
                    ped.ValoresParcelas[i] = i < parc.NumParcelas - 1 ? valorParc : (ped.Total - ped.ValorEntrada) - soma;
                    ped.DatasParcelas[i] = DateTime.Now.AddDays(parc.NumeroDias[i]);
                    soma += valorParc;
                }

                ParcelasPedidoDAO.Instance.DeleteFromPedido(sessao, ped.IdPedido);

                if (ped.ValoresParcelas.Length > 0 && ped.ValoresParcelas[0] > 0)
                    for (int i = 0; i < ped.NumParc; i++)
                    {
                        ParcelasPedido parcela = new ParcelasPedido();
                        parcela.IdPedido = ped.IdPedido;
                        parcela.Valor = ped.ValoresParcelas[i];
                        parcela.Data = ped.DatasParcelas[i];
                        ParcelasPedidoDAO.Instance.Insert(sessao, parcela);
                    }
            }
        }

        #endregion

        #region Verifica Código Cliente

        /// <summary>
        /// Verifica se o código do cliente passado já foi usado em outro pedido que não esteja cancelado
        /// </summary>
        /// <returns></returns>
        public bool CodigoClienteUsado(GDASession sessao, uint idPedido, uint idCliente, string codCliente, bool verificarSempre)
        {
            Pedido.TipoPedidoEnum tipoPedido = GetTipoPedido(sessao, idPedido);
            if (tipoPedido == Pedido.TipoPedidoEnum.Producao)
                return false;

            string sqlVerifica = "Select Count(*) From pedido Where codCliente=?codCliente And situacao<>" +
                (int)Pedido.SituacaoPedido.Cancelado + " And idCli=" + idCliente + (idPedido > 0 ? " And idPedido<>" + idPedido : "");

            string sqlBuscaPedido = "Select idPedido From pedido Where codCliente=?codCliente And idCli=" + idCliente + " limit 1";

            if ((verificarSempre || PedidoConfig.CodigoClienteUsado) && !String.IsNullOrEmpty(codCliente) &&
                Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sqlVerifica, new GDAParameter("?codCliente", codCliente)).ToString()) > 0)
            {
                if (!verificarSempre)
                {
                    uint idPedidoUsado = Glass.Conversoes.StrParaUint(objPersistence.ExecuteScalar(sessao, sqlBuscaPedido,
                        new GDAParameter("?codCliente", codCliente)).ToString());

                    throw new Exception("O código: " + codCliente + " de pedido do cliente já foi utilizado no pedido " + idPedidoUsado + ".");
                }
                else
                    return true;
            }

            return false;
        }

        #endregion

        #region Recupera o tipo de venda do pedido

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Recupera o tipo de venda do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int? GetTipoVenda(uint idPedido)
        {
            return GetTipoVenda(null, idPedido);
        }

        /// <summary>
        /// Recupera o tipo de venda do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int? GetTipoVenda(GDASession sessao, uint idPedido)
        {
            string sql = "select tipoVenda from pedido where idPedido=" + idPedido;
            object retorno = objPersistence.ExecuteScalar(sessao, sql);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (int?)Glass.Conversoes.StrParaInt(retorno.ToString()) : null;
        }

        #endregion

        #region Verifica se o pedido tem sinal/pagamento antecipado a receber

        /// <summary>
        /// Verifica se o pedido tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(Pedido ped, out string mensagemErro)
        {
            return VerificaSinalPagamentoReceber(null, ped, out mensagemErro);
        }

        /// <summary>
        /// Verifica se o pedido tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="ped"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(GDASession sessao, Pedido ped, out string mensagemErro)
        {
            var totalPedido = PedidoEspelhoDAO.Instance.ObtemTotal(sessao, ped.IdPedido);
            mensagemErro = string.Empty;

            if (totalPedido == 0)
                totalPedido = ped.Total;

            // Se for reposição ou garantia, não deve verificar se possui sinal ou pagto antecipado pendente.
            if (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição || ped.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia ||
                // Verifica se o pagamento antes da produção deverá ser efetuado de acordo com o tipo do pedido e configuração interna.
                PedidoConfig.NaoObrigarPagamentoAntesProducaoParaPedidoRevenda((Pedido.TipoPedidoEnum)ped.TipoPedido))
                return true;

            /* Chamado 48262. */
            if (ped.TipoPedido == (int)Pedido.TipoPedidoEnum.Producao)
                return true;

            // Verifica se o pedido tem sinal a receber
            if (TemSinalReceber(sessao, ped.IdPedido))
            {
                mensagemErro = "O pedido " + ped.IdPedido + " tem um sinal de " + ped.ValorEntrada.ToString("C") + " a receber.";
                return false;
            }
            // Verifica se o pedido tem pagamento antecipado a receber, desde que já não tenha recebido sinal 
            // (Alterado para ser um ou outro, conforme era na versão 4.1, a pedido da Dekor)
            else if (!ped.RecebeuSinal && TemPagamentoAntecipadoReceber(sessao, ped.IdPedido))
            {
                mensagemErro = "O pedido " + ped.IdPedido + " deve ser pago antecipadamente.";
                return false;
            }
            // Chamado 15570: Verifica se o pedido foi totalmente pago antecipado, caso o cliente esteja obrigado a pagar
            else if (ped.IdPagamentoAntecipado > 0 && ClienteDAO.Instance.IsPagamentoAntesProducao(ped.IdCli) &&
                totalPedido > ped.ValorPagamentoAntecipado + (ped.IdSinal > 0 ? ped.ValorEntrada : 0))
            {
                mensagemErro = "O pedido " + ped.IdPedido + " deve ser totalmente pago antecipadamente, apenas uma parte do mesmo foi paga.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="idsPedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(string idsPedidos, out string mensagemErro)
        {
            return VerificaSinalPagamentoReceber(null, idsPedidos, out mensagemErro);
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idsPedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(GDASession sessao, string idsPedidos, out string mensagemErro)
        {
            return VerificaSinalPagamentoReceber(sessao, GetByString(null, idsPedidos), out mensagemErro);
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="pedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(IEnumerable<Pedido> pedidos, out string mensagemErro)
        {
            return VerificaSinalPagamentoReceber(null, pedidos, out mensagemErro);
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="pedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(GDASession sessao, IEnumerable<Pedido> pedidos, out string mensagemErro)
        {
            List<uint> temp1 = new List<uint>(), temp2 = new List<uint>();
            return VerificaSinalPagamentoReceber(sessao, pedidos, out mensagemErro, out temp1, out temp2);
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="pedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <param name="idsPedidosOk"></param>
        /// <param name="idsPedidosErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(IEnumerable<Pedido> pedidos, out string mensagemErro,
            out List<uint> idsPedidosOk, out List<uint> idsPedidosErro)
        {
            return VerificaSinalPagamentoReceber(null, pedidos, out mensagemErro, out idsPedidosOk, out idsPedidosErro);
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="pedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <param name="idsPedidosOk"></param>
        /// <param name="idsPedidosErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(GDASession sessao, IEnumerable<Pedido> pedidos, out string mensagemErro,
            out List<uint> idsPedidosOk, out List<uint> idsPedidosErro)
        {
            string idsSinal = "";
            string idsPagtoAntecipado = "";
            idsPedidosOk = new List<uint>();
            idsPedidosErro = new List<uint>();

            // Verifica, em cada pedido, se há sinal/pagamento antecipado a receber
            foreach (Pedido p in pedidos)
            {
                string erro = "";
                if (!VerificaSinalPagamentoReceber(sessao, p, out erro))
                {
                    if (erro.IndexOf("sinal") > -1)
                        idsSinal += p.IdPedido + ", ";
                    else
                        idsPagtoAntecipado += p.IdPedido + ", ";

                    idsPedidosErro.Add(p.IdPedido);
                }
                else
                    idsPedidosOk.Add(p.IdPedido);
            }

            idsSinal = idsSinal.TrimEnd(',', ' ');
            idsPagtoAntecipado = idsPagtoAntecipado.TrimEnd(',', ' ');

            mensagemErro = "";
            if (!String.IsNullOrEmpty(idsSinal))
            {
                bool plural = idsSinal.IndexOf(',') > -1;
                mensagemErro += String.Format("O{1} pedido{1} {0} tem sinal a receber.\n",
                    idsSinal, plural ? "s" : "");
            }

            if (!String.IsNullOrEmpty(idsPagtoAntecipado))
            {
                bool plural = idsPagtoAntecipado.IndexOf(',') > -1;
                mensagemErro += String.Format("O{1} pedido{1} {0} deve{2} ser pago{1} antecipadamente.\n",
                    idsPagtoAntecipado, plural ? "s" : "", plural ? "m" : "");
            }

            mensagemErro = mensagemErro.TrimEnd('\n');
            return String.IsNullOrEmpty(mensagemErro);
        }

        #endregion

        #region Verifica o número de pedidos atrasados (bloqueio de emissão de pedido)

        private string SqlBloqueioEmissao(uint idCliente, uint idPedido, bool getClientes, bool selecionar)
        {
            string campos = getClientes ? "ped.idCli" : selecionar ? "ped.idPedido" : "count(distinct ped.idPedido)";

            string sql = "select " + campos + @"
                from pedido ped
                    " + (getClientes ? "inner join cliente c on (ped.idCli=c.id_Cli)" : "") + @"
                where date(ped.dataEntrega)<date(now())
                    and ped.situacao not in (" + (int)Pedido.SituacaoPedido.Cancelado + "," + (int)Pedido.SituacaoPedido.Confirmado + @") 
                    and ped.tipoPedido not in (" + (int)Pedido.TipoPedidoEnum.Producao + @")
                    and ped.dataPronto<date_sub(now(), interval " + PedidoConfig.NumeroDiasPedidoProntoAtrasado + @" day)";

            if (!getClientes)
            {
                if (idCliente > 0)
                {
                    // Não retorna nenhum pedido para bloqueio se o pedido possuir pagamento antecipado já pago
                    sql += " and ped.idSinal is null and coalesce(ped.valorPagamentoAntecipado,0)=0 and ped.idCli=" + idCliente;
                }

                if (idPedido > 0)
                    sql += " and ped.idPedido=" + idPedido;
            }
            else
                sql += " and lower(c.nome)<>'consumidor final' and c.situacao=" + (int)SituacaoCliente.Ativo;

            return sql;
        }

        public int GetCountBloqueioEmissao(uint idCliente)
        {
            return GetCountBloqueioEmissao(null, idCliente);
        }

        public int GetCountBloqueioEmissao(GDASession session, uint idCliente)
        {
            return PedidoConfig.NumeroDiasPedidoProntoAtrasado == 0 ||
                ClienteDAO.Instance.IgnorarBloqueioPedidoPronto(session, idCliente) ? 0 :
                objPersistence.ExecuteSqlQueryCount(session, SqlBloqueioEmissao(idCliente, 0, false, false));
        }

        public string GetIdsBloqueioEmissao(uint idCliente)
        {
            return GetIdsBloqueioEmissao(null, idCliente);
        }

        public string GetIdsBloqueioEmissao(GDASession session, uint idCliente)
        {
            return PedidoConfig.NumeroDiasPedidoProntoAtrasado == 0 ||
                ClienteDAO.Instance.IgnorarBloqueioPedidoPronto(session, idCliente) ? "" :
                GetValoresCampo(session, SqlBloqueioEmissao(idCliente, 0, false, true), "idPedido");
        }

        public uint[] GetClientesAtrasados()
        {
            if (PedidoConfig.NumeroDiasPedidoProntoAtrasado == 0)
                return new uint[0];

            string temp = GetValoresCampo(SqlBloqueioEmissao(0, 0, true, true), "idCli");

            if (String.IsNullOrEmpty(temp))
                return new uint[0];

            List<uint> retorno = new List<uint>();
            foreach (string s in temp.Split(','))
            {
                uint i;
                if (uint.TryParse(s, out i) && !ClienteDAO.Instance.IgnorarBloqueioPedidoPronto(i))
                    retorno.Add(i);
            }

            return retorno.ToArray();
        }

        public Pedido[] GetPedidosBloqueioEmissaoByCliente(uint idCliente)
        {
            if (idCliente > 0)
            {
                string idsPedidos = GetIdsBloqueioEmissao(idCliente);

                if (string.IsNullOrEmpty(idsPedidos))
                    idsPedidos = "0";

                bool temFiltro;
                string filtroAdicional;

                return objPersistence.LoadData(Sql(0, 0, idsPedidos, null, 0, idCliente, null, 0, null, 0, null, null, null, null, null,
                    null, null, null, null, null, null, null, null, null, 0, false, false, 0, 0, 0, 0, 0, null, 0, 0, 0, "", true, out filtroAdicional,
                    out temFiltro).Replace("?filtroAdicional?", filtroAdicional)).ToArray();
            }
            else
            {
                return new Pedido[0];
            }
        }

        #endregion

        #region Verifica se o pedido está atrasado pela data de entrega

        /// <summary>
        /// Verifica se o pedido está atrasado pela data de entrega.
        /// </summary>
        public bool IsPedidoAtrasado(uint idPedido, bool forLiberacao)
        {
            return IsPedidoAtrasado(null, idPedido, forLiberacao);
        }

        /// <summary>
        /// Verifica se o pedido está atrasado pela data de entrega.
        /// </summary>
        public bool IsPedidoAtrasado(GDASession session, uint idPedido, bool forLiberacao)
        {
            if (forLiberacao && !Liberacao.DadosLiberacao.LiberarPedidoAtrasadoParcialmente)
                return false;

            string sql = "select count(*) from pedido where dataEntrega<=date(now()) and idPedido=" + idPedido;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Verifica se o pedido possui parcela cadastrada

        /// <summary>
        /// Verifica se o pedido possui parcela cadastrada
        /// </summary>
        /// <param name="idsPedido"></param>
        /// <returns></returns>
        public bool PossuiParcela(uint idPedido)
        {
            string sql = "Select Count(*) From parcelas_pedido Where idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Recupera os pedidos de um sinal

        /// <summary>
        /// Recupera os pedidos de um sinal.
        /// </summary>
        /// <param name="idSinal"></param>
        /// <param name="pagtoAntecipado"></param>
        /// <returns></returns>
        public Pedido[] GetBySinal(uint idSinal, bool pagtoAntecipado)
        {
            return GetBySinal(null, idSinal, pagtoAntecipado);
        }

        /// <summary>
        /// Recupera os pedidos de um sinal.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idSinal"></param>
        /// <param name="pagtoAntecipado"></param>
        /// <returns></returns>
        public Pedido[] GetBySinal(GDASession session, uint idSinal, bool pagtoAntecipado)
        {
            return GetBySinal(session, idSinal, pagtoAntecipado, false);
        }

        /// <summary>
        /// Recupera os pedidos de um sinal.
        /// </summary>
        /// <param name="idSinal"></param>
        /// <param name="pagtoAntecipado"></param>
        /// <param name="retificacao"></param>
        /// <returns></returns>
        public Pedido[] GetBySinal(uint idSinal, bool pagtoAntecipado, bool retificacao)
        {
            return GetBySinal(null, idSinal, pagtoAntecipado, retificacao);
        }

        /// <summary>
        /// Recupera os pedidos de um sinal.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idSinal"></param>
        /// <param name="pagtoAntecipado"></param>
        /// <param name="retificacao"></param>
        /// <returns></returns>
        public Pedido[] GetBySinal(GDASession session, uint idSinal, bool pagtoAntecipado, bool retificacao)
        {
            bool buscarReais = !SinalDAO.Instance.Exists(session, idSinal) ||
                SinalDAO.Instance.ObtemValorCampo<Sinal.SituacaoEnum>(session, "situacao", "idSinal=" + idSinal) != Sinal.SituacaoEnum.Cancelado;

            string sql = @"
                select p.*, pe.total as totalEspelho, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as nomeCliente, s.dataCad as dataEntrada, s.usuCad as usuEntrada, 
                    cast(s.valorCreditoAoCriar as decimal(12,2)) as valorCreditoAoReceberSinal, cast(s.creditoGeradoCriar as decimal(12,2)) as creditoGeradoReceberSinal, 
                    cast(s.creditoUtilizadoCriar as decimal(12,2)) as creditoUtilizadoReceberSinal, s.isPagtoAntecipado as pagamentoAntecipado,
                    f.nome as nomeFunc, fc.nome as nomeFuncCliente
                from pedido p
                    left join pedido_espelho pe on (p.idPedido=pe.idPedido)
                    left join cliente c on (p.idCli=c.id_Cli)
                    left join sinal s on (p.idSinal=s.idSinal)
                    left join funcionario f on (p.idFunc=f.idFunc)
                    left join funcionario fc on (c.idFunc=fc.idFunc)
                where 1";

            if (retificacao)
                sql += " And p.situacao<>" + (int)Pedido.SituacaoPedido.Confirmado;

            if (buscarReais)
                sql += (pagtoAntecipado ? " And p.idPagamentoAntecipado=" : " And p.idSinal=") + idSinal;
            else
            {
                string idsPedidosR = SinalDAO.Instance.ObtemValorCampo<string>(session, "idsPedidosR", "idSinal=" + idSinal);
                sql += " and p.idPedido in (" + (!String.IsNullOrEmpty(idsPedidosR) ? idsPedidosR : "0") + ")";
            }

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Obtém os Valores Totais

        public string GetTotais(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, string endereco,
            string bairro, string situacao, string situacaoProd, string byVend, string maoObra, string maoObraEspecial, string producao,
            uint idOrcamento, float altura, int largura, string nomeColunaTotal)
        {
            bool temFiltro;
            string filtroAdicional;

            string sqlTotal = "select sum(temp." + nomeColunaTotal + ") from (" + Sql(idPedido, 0, null, null, idLoja, idCli, nomeCli, 0,
                codCliente, 0, endereco, bairro, situacao, situacaoProd, byVend, null, maoObra, maoObraEspecial, producao, null, null,
                null, null, null, idOrcamento, false, false, altura, largura, 0, 0, 0, null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro).
                Replace("?filtroAdicional?", filtroAdicional) + " ) as temp";

            return ExecuteScalar<string>(sqlTotal, GetParam(nomeCli, codCliente, endereco, bairro, situacao, situacaoProd, null, null, null, null, ""));
        }

        #endregion

        #region Obtém o total dos pedidos de um período - Gráfico

        //public string GetTotalPedidos(uint idLoja, uint idFunc, string dataIni, string dataFim)
        //{
        //    string dataInicial = DateTime.Parse(dataIni).ToString("yyyy/MM/dd");
        //    string dataFinal = DateTime.Parse(dataFim).ToString("yyyy/MM/dd");

        //    string sqltotal = "select sum(temp.Total) from (" + Sql(0, 0, null, null, idLoja, 0, null, idFunc, null,
        //        null, null, null, null, null, null, null, null, null, null, null, 0, false, false, 0,
        //        0, true) + " And p.Datacad between '" + dataInicial + "' And '" + dataFinal + "') as temp";

        //    Object retorno = objPersistence.ExecuteScalar(sqltotal);

        //    return retorno.ToString();
        //}

        public string GetTotalPedidos(uint idLoja, int tipoFunc, uint idVendedor, string dataIni, string dataFim)
        {
            string data = PedidoConfig.LiberarPedido ? "DataLiberacao" : "DataConf";

            string campos = @"p.idLoja, p.idFunc" + (tipoFunc == 0 ? "" : "Cli") + @" as idFunc, cast(Sum(p.Total) as decimal(12,2)) as TotalVenda, NomeFunc" + (tipoFunc == 0 ? "" : "Cli") +
                @" as NomeVendedor, NomeLoja, (Right(Concat('0', Cast(Month(p." + data + ") as char), '/', Cast(Year(p." + data + @") as char)), 7)) as DataVenda, 
                '$$$' as Criterio";

            string criterio = String.Empty;

            bool temFiltro;
            string filtroAdicional;

            string sql = @"
                Select " + campos + @" 
                From (" + PedidoDAO.Instance.SqlLucr("0", "0", (int)Pedido.SituacaoPedido.Confirmado, dataIni, dataFim, 0, 0, true,
                    out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional) + @") as p
                Where 1";

            if (idLoja > 0)
            {
                sql += " And p.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idVendedor > 0)
            {
                sql += " And p.idFunc" + (tipoFunc == 0 ? "" : "Cli") + "=" + idVendedor;
                criterio += (tipoFunc == 0 ? "Emissor" : "Vendedor") + ": " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(idVendedor)) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
                criterio += "Data Início: " + dataIni + "    ";

            if (!String.IsNullOrEmpty(dataFim))
                criterio += "Data Fim: " + dataFim + "    ";

            sql = "Select sum(temp.TotalVenda) from (" + sql + ") as temp";

            Object retorno = objPersistence.ExecuteScalar(sql.Replace("$$$", criterio), GetParams(dataIni, dataFim));

            return retorno.ToString();
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dtIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dtFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParams.ToArray();
        }

        #endregion

        #region Verifica se o pedido pode ser exportado

        /// <summary>
        /// Verifica se o pedido pode ser exportado.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PodeExportar(uint idPedido)
        {
            var pcpFinalizado = GetTipoPedido(idPedido) == Pedido.TipoPedidoEnum.Revenda ? true :
                PedidoEspelhoDAO.Instance.ObtemSituacao(idPedido) == PedidoEspelho.SituacaoPedido.Finalizado ||
                PedidoEspelhoDAO.Instance.ObtemSituacao(idPedido) == PedidoEspelho.SituacaoPedido.Impresso;

            bool pode = PedidoDAO.Instance.IsPedidoConfirmadoLiberado(idPedido) && pcpFinalizado &&
                 PedidoExportacaoDAO.Instance.PodeExportar(idPedido);

            return pode;
        }

        #endregion

        #region Atualiza os campos TotM do Pedido e Pedido_Espelho

        /// <summary>
        /// Atualiza os campos TotM do Pedido e Pedido_Espelho.
        /// </summary>
        public void AtualizaTotM(uint idPedido)
        {
            AtualizaTotM(idPedido, null);
        }

        /// <summary>
        /// Atualiza os campos TotM do Pedido e Pedido_Espelho.
        /// </summary>
        public void AtualizaTotM(uint idPedido, bool? pcp)
        {
            AtualizaTotM(null, idPedido, pcp);
        }

        /// <summary>
        /// Atualiza os campos TotM do Pedido e Pedido_Espelho.
        /// </summary>
        /// <param name="pcp">NULL: Ambos; true: PCP; false: Pedido</param>
        internal void AtualizaTotM(GDASession sessao, uint idPedido, bool? pcp)
        {
            string campo = "Round(pp.totM" + (PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ? "2Calc" :
                "*if(p1.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + " and ap.idAmbientePedido is not null, ap.qtde, 1)") + ", 2)";

            string sql = "";

            // Usa left join para calcular o totM, porque caso não tenha nenhum produto associado à este pedido o valor seja preenchido com 0 (zero)
            if (!pcp.GetValueOrDefault(false))
                sql += @"
                    update pedido p
	                    left join (
    	                    select pp.idPedido, sum(coalesce({0},0)) as totM
                            from produtos_pedido pp
                                left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                                inner join pedido p1 on (pp.idPedido=p1.idPedido)
    	                    where coalesce(pp.invisivelPedido, false)=false And coalesce(pp.IdProdPedParent, 0) = 0 {2}
                            group by pp.idPedido
                        ) pp on (p.idPedido=pp.idPedido)
                    set p.totM=pp.totM
                    where 1 {1};";

            // Usa left join para calcular o totM, porque caso não tenha nenhum produto associado à este pedido o valor seja preenchido com 0 (zero)
            if (pcp.GetValueOrDefault(true))
                sql += @"
                    update pedido_espelho p
	                    left join (
    	                    select pp.idPedido, sum(coalesce({0},0)) as totM
                            from produtos_pedido_espelho pp
                                left join ambiente_pedido_espelho ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                                inner join pedido p1 on (pp.idPedido=p1.idPedido)
    	                    where coalesce(pp.invisivelFluxo, false)=false And coalesce(pp.IdProdPedParent, 0) = 0 {2}
                            group by pp.idPedido
                        ) pp on (p.idPedido=pp.idPedido)
                    set p.totM=pp.totM
                    where 1 {1}";

            string where = "", whereInt = "";

            if (idPedido > 0)
            {
                where += " and p.idPedido=" + idPedido;
                whereInt += " and pp.idPedido=" + idPedido;
            }

            objPersistence.ExecuteCommand(sessao, String.Format(sql, campo, where, whereInt));
        }

        #endregion

        #region Recupera a data do último recebimento dos pedidos

        /// <summary>
        /// Recupera a data da última alteração, data de recebimento de sinal ou pagamento antecipado dos pedidos.
        /// </summary>
        public DateTime? ObterDataUltimaAlteracaoPedidoRecebimentoSinalouPagamentoAntecipado(GDASession session, string idsPedidos)
        {
            // Recupera a data da última alteração feita nos pedidos.
            var dataUltimaAlteracao = ObterDataUltimaAlteracaoPedido(session, idsPedidos);
            // Recupera a data do último recebimento de sinal ou pagamento antecipado dos pedidos.
            var dataUltimoRecebimentoSinalOuPagamentoAntecipado = ObterDataRecebimentoSinalOuPagamentoAntecipado(session, idsPedidos);

            // Caso nenhuma data seja retornada, sai do método retornando um valor nulo.
            if (!dataUltimaAlteracao.HasValue && !dataUltimoRecebimentoSinalOuPagamentoAntecipado.HasValue)
                return null;

            // Seta um valor mínimo nas variáveis que estiverem nulas.
            var primeiraData = dataUltimaAlteracao.GetValueOrDefault(DateTime.MinValue);
            var segundaData = dataUltimoRecebimentoSinalOuPagamentoAntecipado.GetValueOrDefault(DateTime.MinValue);

            // Verifica se as datas são iguais.
            if (DateTime.Compare(primeiraData, segundaData) == 0)
                return primeiraData;

            // Retorna a maior data dentre as 2 recuperadas.
            return DateTime.Compare(primeiraData, segundaData) > 0 ? primeiraData : DateTime.Compare(segundaData, primeiraData) > 0 ? segundaData : (DateTime?)null;
        }

        /// <summary>
        /// Recupera a data da última alteração dos pedidos.
        /// </summary>
        public DateTime? ObterDataUltimaAlteracaoPedido(GDASession session, string idsPedidos)
        {
            if (string.IsNullOrWhiteSpace(idsPedidos))
                return null;

            var sql = string.Format("SELECT MAX(DataAlt) AS Data FROM log_alteracao WHERE IdRegistroAlt IN ({0}) AND Tabela={1}", idsPedidos, (int)LogAlteracao.TabelaAlteracao.Pedido);

            return ExecuteScalar<DateTime?>(session, sql);
        }

        /// <summary>
        /// Recupera a data de recebimento dos sinais e dos pagamentos antecipados dos pedidos.
        /// </summary>
        public DateTime? ObterDataRecebimentoSinalOuPagamentoAntecipado(GDASession session, string idsPedidos)
        {
            if (string.IsNullOrWhiteSpace(idsPedidos))
                return null;

            var sql = string.Format(@"SELECT MAX(Data) FROM (
                    SELECT MAX(s.DataCad) AS Data FROM sinal s
                        INNER JOIN pedido p ON (s.IdSinal=p.IdSinal)
                    WHERE p.IdPedido IN ({0})
                    
                    UNION SELECT MAX(s.DataCad) AS Data FROM sinal s
                        INNER JOIN pedido p ON (s.IdSinal=p.IdPagamentoAntecipado)
                    WHERE p.IdPedido IN ({0})
                ) AS temp", idsPedidos);

            return ExecuteScalar<DateTime?>(session, sql);
        }

        /// <summary>
        /// Recupera a data de cancelamento dos sinais e pagamentos antecipados informados.
        /// </summary>
        public DateTime? ObterDataCancelamentoSinalOuPagamentoAntecipado(GDASession session, string idsSinais, string idsPagtoAntecip)
        {
            if (string.IsNullOrWhiteSpace(idsSinais))
                return null;

            if (!string.IsNullOrWhiteSpace(idsPagtoAntecip))
                idsSinais += string.Format(",{0}", idsPagtoAntecip);

            var sql = string.Format("SELECT MAX(DataCanc) AS Data FROM log_cancelamento WHERE IdRegistroCanc IN ({0}) AND Tabela={1}", idsSinais, (int)LogCancelamento.TabelaCancelamento.Sinal);

            return ExecuteScalar<DateTime?>(session, sql);
        }

        #endregion

        #region Exibir Nota Promissória?

        /// <summary>
        /// Exibir Nota Promissória?
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExibirNotaPromissoria(uint idPedido)
        {
            int tipoVenda = ObtemTipoVenda(idPedido);
            Pedido.SituacaoPedido situacao = ObtemSituacao(idPedido);
            return ExibirNotaPromissoria(tipoVenda, situacao);
        }

        internal bool ExibirNotaPromissoria(int tipoVenda, Pedido.SituacaoPedido situacao)
        {
            return !PedidoConfig.LiberarPedido && FinanceiroConfig.DadosLiberacao.NumeroViasNotaPromissoria > 0 && tipoVenda == (int)Pedido.TipoVendaPedido.APrazo &&
                situacao == Pedido.SituacaoPedido.Confirmado && Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
        }

        #endregion

        #region Remove e aplica comissão, desconto e acréscimo

        #region Remover

        /// <summary>
        /// Remove comissão, desconto e acréscimo.
        /// </summary>
        internal void RemoveComissaoDescontoAcrescimo(GDASession sessao, Pedido pedido,
            IEnumerable<ProdutosPedido> produtosPedido = null)
        {
            var ambientesPedido = (pedido as IContainerCalculo).Ambientes.Obter()
                .Cast<AmbientePedido>()
                .Where(f => f.Acrescimo > 0)
                .ToList();

            if (produtosPedido == null)
                produtosPedido = ProdutosPedidoDAO.Instance.GetByPedidoLite(sessao, pedido.IdPedido, false, true);

            var removidos = new List<uint>();

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
            {
                var produtosAmbiente = produtosPedido.Where(p => p.IdAmbientePedido == ambientePedido.IdAmbientePedido);
                if (AmbientePedidoDAO.Instance.RemoverAcrescimo(sessao, pedido, ambientePedido.IdAmbientePedido, produtosAmbiente))
                    removidos.AddRange(produtosAmbiente.Select(p => p.IdProdPed));
            }

            if (RemoverComissao(sessao, pedido, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            if (RemoverAcrescimo(sessao, pedido, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            if (RemoverDesconto(sessao, pedido, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            var produtosAtualizar = produtosPedido
                .Where(p => removidos.Contains(p.IdProdPed))
                .ToList();

            FinalizarAplicacaoComissaoAcrescimoDesconto(sessao, pedido, produtosAtualizar, true);
            UpdateTotalPedido(sessao, pedido);

            objPersistence.ExecuteCommand(sessao, @"update pedido set percComissao=0, desconto=0,
                acrescimo=0 where idPedido=" + pedido.IdPedido);
        }

        /// <summary>
        /// Remove comissão, desconto e acréscimo.
        /// </summary>
        private void RemoveComissaoDescontoAcrescimo(GDASession sessao, Pedido antigo, Pedido novo,
            IEnumerable<ProdutosPedido> produtosPedido)
        {
            var ambientesPedido = (novo as IContainerCalculo).Ambientes.Obter()
                .Cast<AmbientePedido>()
                .Where(f => f.Acrescimo > 0)
                .ToList();

            var removidos = new List<uint>();

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
            {
                var produtosAmbiente = produtosPedido.Where(p => p.IdAmbientePedido == ambientePedido.IdAmbientePedido);

                if (AmbientePedidoDAO.Instance.RemoverAcrescimo(sessao, novo, ambientePedido.IdAmbientePedido, produtosAmbiente))
                    removidos.AddRange(produtosAmbiente.Select(p => p.IdProdPed));
            }

            // Remove o valor da comissão nos produtos e no pedido
            if (RemoverComissao(sessao, novo, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            // Remove o acréscimo do pedido
            if (RemoverAcrescimo(sessao, novo, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            // Remove o desconto do pedido
            if (RemoverDesconto(sessao, novo, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            var produtosAtualizar = produtosPedido
                .Where(p => removidos.Contains(p.IdProdPed))
                .ToList();

            FinalizarAplicacaoComissaoAcrescimoDesconto(sessao, novo, produtosAtualizar, true);
            UpdateTotalPedido(sessao, novo);
        }

        #endregion

        #region Aplicar

        public void AplicaComissaoDescontoAcrescimo(GDASession sessao, Pedido pedido, bool manterFuncDesc,
            IEnumerable<ProdutosPedido> produtosPedido = null)
        {
            var ambientesPedido = (pedido as IContainerCalculo).Ambientes.Obter()
                .Cast<AmbientePedido>()
                .Where(f => f.Acrescimo > 0)
                .ToList();

            if (produtosPedido == null)
                produtosPedido = ProdutosPedidoDAO.Instance.GetByPedidoLite(sessao, pedido.IdPedido, false, true);

            if (pedido.IdComissionado > 0)
                objPersistence.ExecuteCommand(sessao, "update pedido set idComissionado=" + pedido.IdComissionado +
                    " where idPedido=" + pedido.IdPedido);

            var removidos = new List<uint>();

            if (AplicarAcrescimo(sessao, pedido, pedido.TipoAcrescimo, pedido.Acrescimo, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            if (AplicarDesconto(sessao, pedido, pedido.TipoDesconto, pedido.Desconto, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            if (AplicarComissao(sessao, pedido, pedido.PercComissao, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            objPersistence.ExecuteCommand(sessao, @"update pedido set percComissao=?pc, tipoDesconto=?td, desconto=?d,
                tipoAcrescimo=?ta, acrescimo=?a where idPedido=" + pedido.IdPedido, new GDAParameter("?pc", pedido.PercComissao),
                new GDAParameter("?td", pedido.TipoDesconto), new GDAParameter("?d", pedido.Desconto),
                new GDAParameter("?ta", pedido.TipoAcrescimo), new GDAParameter("?a", pedido.Acrescimo));

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
            {
                var produtosAmbiente = produtosPedido.Where(p => p.IdAmbientePedido == ambientePedido.IdAmbientePedido);

                bool atualizar = AmbientePedidoDAO.Instance.AplicarAcrescimo(
                    sessao,
                    pedido,
                    ambientePedido.IdAmbientePedido,
                    ambientePedido.TipoAcrescimo,
                    ambientePedido.Acrescimo,
                    produtosAmbiente
                );

                if (atualizar)
                    removidos.AddRange(produtosAmbiente.Select(p => p.IdProdPed));
            }

            var produtosAtualizar = produtosPedido
                .Where(p => removidos.Contains(p.IdProdPed))
                .ToList();

            FinalizarAplicacaoComissaoAcrescimoDesconto(sessao, pedido, produtosAtualizar, true, manterFuncDesc);
            UpdateTotalPedido(sessao, pedido);
        }

        /// <summary>
        /// Aplica comissão, desconto e acréscimo em uma ordem pré-estabelecida.
        /// </summary>
        internal void AplicaComissaoDescontoAcrescimo(GDASession sessao, Pedido antigo, Pedido novo,
            IEnumerable<ProdutosPedido> produtosPedido = null)
        {
            var ambientesPedido = (novo as IContainerCalculo).Ambientes.Obter()
                .Cast<AmbientePedido>()
                .Where(f => f.Acrescimo > 0)
                .ToList();

            if (produtosPedido == null)
                produtosPedido = ProdutosPedidoDAO.Instance.GetByPedidoLite(sessao, novo.IdPedido, false, true);

            var removidos = new List<uint>();

            // Remove o acréscimo do pedido
            if (AplicarAcrescimo(sessao, novo, novo.TipoAcrescimo, novo.Acrescimo, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            // Remove o desconto do pedido
            if (AplicarDesconto(sessao, novo, novo.TipoDesconto, novo.Desconto, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            // Remove o valor da comissão nos produtos e no pedido
            if (AplicarComissao(sessao, novo, novo.PercComissao, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
            {
                var produtosAmbiente = produtosPedido.Where(p => p.IdAmbientePedido == ambientePedido.IdAmbientePedido);

                bool atualizar = AmbientePedidoDAO.Instance.AplicarAcrescimo(
                    sessao,
                    novo,
                    ambientePedido.IdAmbientePedido,
                    ambientePedido.TipoAcrescimo,
                    ambientePedido.Acrescimo,
                    produtosAmbiente
                );

                if (atualizar)
                    removidos.AddRange(produtosAmbiente.Select(p => p.IdProdPed));
            }

            var produtosAtualizar = produtosPedido
                .Where(p => removidos.Contains(p.IdProdPed))
                .ToList();

            FinalizarAplicacaoComissaoAcrescimoDesconto(sessao, novo, produtosAtualizar, true, false);
            UpdateTotalPedido(sessao, novo);
        }

        #endregion

        #endregion

        #region Verifica se o pedido foi gerado por um parceiro

        /// <summary>
        /// Verifica se o pedido foi gerado por um parceiro.
        /// </summary>
        public bool IsGeradoParceiro(uint idPedido)
        {
            return IsGeradoParceiro(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido foi gerado por um parceiro.
        /// </summary>
        public bool IsGeradoParceiro(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<bool>(session, "geradoParceiro", "idPedido=" + idPedido);
        }

        #endregion

        #region Parcelas do pedido

        public enum TipoCalculoParcelas
        {
            Ambas,
            Data,
            Valor
        }

        /// <summary>
        /// Recalcula as parcelas do pedido.
        /// </summary>
        /// <param name="objPedido"></param>
        public void RecalculaParcelas(GDASession sessao, ref Pedido objPedido, TipoCalculoParcelas tipoCalculo)
        {
            uint tipoPagtoCli = ClienteDAO.Instance.ObtemValorCampo<uint>(sessao, "tipoPagto", "id_Cli=" + objPedido.IdCli);
            uint idParc = objPedido.IdParcela.GetValueOrDefault(tipoPagtoCli);

            if (idParc == 0)
                return;

            int numParcelas = ParcelasDAO.Instance.ObtemValorCampo<int>(sessao, "numParcelas", "idParcela=" + idParc);

            if (numParcelas == 0)
                return;

            string[] numDias = ParcelasDAO.Instance.ObtemValorCampo<string>(sessao, "dias", "idParcela=" + idParc).Split(',');

            objPedido.IdParcela = idParc;
            objPedido.NumParc = numParcelas;

            decimal totalParc = objPedido.Total - (objPedido.ValorEntrada + objPedido.ValorPagamentoAntecipado);
            decimal valorParc = Math.Round(totalParc / objPedido.NumParc, 2);

            decimal[] valoresParc = new decimal[objPedido.NumParc];
            DateTime[] datasParc = new DateTime[objPedido.NumParc];

            decimal somaParc = 0;
            for (int i = 0; i < objPedido.NumParc; i++)
            {
                if (i < (objPedido.NumParc - 1))
                {
                    valoresParc[i] = valorParc;
                    somaParc += valorParc;
                }
                else
                    valoresParc[i] = Math.Round(totalParc - somaParc, 2);

                datasParc[i] = DateTime.Now.AddDays(Glass.Conversoes.StrParaInt(numDias[i]));
            }

            if (tipoCalculo != TipoCalculoParcelas.Data || objPedido.ValoresParcelas.Length < valoresParc.Length)
                objPedido.ValoresParcelas = valoresParc;

            if (tipoCalculo != TipoCalculoParcelas.Valor || objPedido.DatasParcelas.Length < datasParc.Length)
                objPedido.DatasParcelas = datasParc;
        }

        /// <summary>
        /// Salva as parcelas do pedido.
        /// </summary>
        private void SalvarParcelas(GDASession session, Pedido objPedido)
        {
            // Se for venda à vista exclui as parcelas
            if (objPedido.TipoVenda == 1)
            {
                ParcelasPedidoDAO.Instance.DeleteFromPedido(session, objPedido.IdPedido);
            }
            // Se for venda à prazo, salva as parcelas
            else if (objPedido.TipoVenda == 2)
            {
                ParcelasPedidoDAO.Instance.DeleteFromPedido(session, objPedido.IdPedido);

                ParcelasPedido parcela = new ParcelasPedido();
                parcela.IdPedido = objPedido.IdPedido;

                if (objPedido.ValoresParcelas == null)
                {
                    throw new Exception("Informe as parcelas do pedido");
                }

                if (objPedido.ValoresParcelas.Length > 0 && objPedido.ValoresParcelas[0] > 0)
                    for (int i = 0; i < objPedido.NumParc; i++)
                    {
                        // Chamado 35806. Caso o índice seja maior que a quantidade de itens dentro das variáveis "ValoresParcelas" ou
                        // "DatasParcelas", o loop deve ser finalizado.
                        if (objPedido.ValoresParcelas.Count() == i || objPedido.DatasParcelas.Count() == i)
                        {
                            break;
                        }

                        parcela.Valor = objPedido.ValoresParcelas[i];
                        parcela.Data = objPedido.DatasParcelas[i];
                        ParcelasPedidoDAO.Instance.Insert(session, parcela);
                    }
            }
        }

        #endregion

        #region Rentabilidade

        /// <summary>
        /// Atualiza a rentabilidade do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="percentualRentabilidade">Percentual da rentabilidade.</param>
        /// <param name="rentabilidadeFinanceira">Rentabilidade financeira.</param>
        public void AtualizarRentabilidade(GDA.GDASession sessao,
            uint idPedido, decimal percentualRentabilidade, decimal rentabilidadeFinanceira)
        {
            objPersistence.ExecuteCommand(sessao, "UPDATE pedido SET PercentualRentabilidade=?percentual, RentabilidadeFinanceira=?rentabilidade WHERE IdPedido=?idPedido",
                new GDA.GDAParameter("?percentual", percentualRentabilidade),
                new GDA.GDAParameter("?rentabilidade", rentabilidadeFinanceira),
                new GDA.GDAParameter("?idPedido", idPedido));
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(Pedido objInsert)
        {
            lock (_inserirPedidoLock)
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
        }

        public override uint Insert(GDASession session, Pedido objInsert)
        {
            uint idPedido = 0;

            if (objInsert.TipoPedido == 0)
                throw new Exception("Não foi informado o Tipo do pedido.");

            if (UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.Vendedor &&
            !Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao))
            {
                if (objInsert.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição &&
                    !Config.PossuiPermissao(Config.FuncaoMenuPedido.GerarReposicao))
                    throw new Exception("Apenas o gerente pode emitir pedido de reposição.");
                else if (objInsert.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia)
                    throw new Exception("Apenas o gerente pode emitir pedido de garantia.");
            }

            if (objInsert.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial)
            {
                if (!ClienteDAO.Instance.ObtemValorCampo<bool>(session, "controlarEstoqueVidros", "id_Cli=" + objInsert.IdCli))
                    throw new Exception("O cliente deve controlar estoque para ser utilizado em um pedido de mão-de-obra especial.");
            }

            if (objInsert.IdObra.GetValueOrDefault(0) > 0 && ObraDAO.Instance.ObtemSituacao(session, objInsert.IdObra.Value) != Obra.SituacaoObra.Confirmada)
                throw new Exception("A obra informada não esta confirmada.");

            // Caso o usuário não tenha permissão de alterar o vendedor no pedido, força a associação do vendedor associado ao cliente neste pedido
            uint? idFunc = ClienteDAO.Instance.ObtemIdFunc(session, objInsert.IdCli);
            if ((objInsert.Importado || !objInsert.SelVendEnabled) && idFunc > 0 && PedidoConfig.DadosPedido.BuscarVendedorEmitirPedido)
                objInsert.IdFunc = idFunc.Value;

            if (objInsert.IdOrcamento == 0)
                objInsert.IdOrcamento = null;

            if (!PedidoConfig.Comissao.AlterarPercComissionado)
            {
                if (objInsert.IdComissionado > 0)
                    objInsert.PercComissao = ComissionadoDAO.Instance.ObtemValorCampo<float>(session, "percentual", "idComissionado=" + objInsert.IdComissionado.Value);
                else
                    objInsert.PercComissao = 0;
            }

            // Verifica se o id do orcamento informado existe
            if (objInsert.IdOrcamento != null)
                if (OrcamentoDAO.Instance.GetCount(session, objInsert.IdOrcamento.Value) < 1)
                    throw new Exception("O Orcamento informado no pedido não existe.");

            if (objInsert.IdFormaPagto == 0)
                objInsert.IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio;

            if (objInsert.TipoVenda == null || objInsert.TipoVenda == 0)
                objInsert.TipoVenda = (int)Pedido.TipoVendaPedido.AVista;

            // Se o pedido for à vista, não é necessário informar a forma de pagamento
            if (objInsert.TipoVenda == (int)Pedido.TipoVendaPedido.AVista && !Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
                objInsert.IdFormaPagto = null;

            //Verifica se o cliente possui contas a receber vencidas se nao for garantia
            if (!FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro && (ClienteDAO.Instance.ObtemValorCampo<bool>(session, "bloquearPedidoContaVencida", "id_Cli=" + objInsert.IdCli)) &&
                ContasReceberDAO.Instance.ClientePossuiContasVencidas(session, objInsert.IdCli) && objInsert.TipoVenda != (int)Pedido.TipoVendaPedido.Garantia)
                throw new Exception("Cliente bloqueado. Motivo: Contas a receber em atraso.");

            // Verifica se o código do pedido do cliente já foi cadastrado
            if (objInsert.TipoVenda < 3)
                CodigoClienteUsado(session, objInsert.IdPedido, objInsert.IdCli, objInsert.CodCliente, false);

            // Obtém a data de atraso deste funcionário, se houver
            if (FuncionarioDAO.Instance.PossuiDiasAtraso(session, objInsert.Usucad))
                objInsert.DataPedido = FuncionarioDAO.Instance.ObtemDataAtraso(session, objInsert.Usucad);

            decimal descontoPadraoAVista = PedidoConfig.Desconto.DescontoPadraoPedidoAVista;
            if (descontoPadraoAVista > 0)
            {
                uint? tipoPagto = ClienteDAO.Instance.ObtemValorCampo<uint?>(session, "tipoPagto", "id_Cli=" + objInsert.IdCli);

                bool podeTerDesconto = !(PedidoConfig.Desconto.ImpedirDescontoSomativo &&
                    UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador &&
                        DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(session, objInsert.IdCli, 0, null, 0, null));

                if (podeTerDesconto && (objInsert.TipoVenda == (int)Pedido.TipoVendaPedido.AVista || (tipoPagto > 0 &&
                        ParcelasDAO.Instance.ObtemValorCampo<string>(session, "descricao", "idParcela=" + tipoPagto).ToLower().Contains("na entrega"))))
                {
                    objInsert.TipoDesconto = 1;
                    objInsert.Desconto = descontoPadraoAVista;
                }
            }

            idPedido = InsertBase(session, objInsert);

            #region Data de entrega

            AtualizarDataEntregaCalculada(session, objInsert, idPedido);

            #endregion

            // Se o pedido não for mão de obra e se a empresa trabalha com ambiente no pedido, insere um ambiente balcão de uma vez, se o tipo de entrega for balcão
            if (objInsert.TipoPedido != (int)Pedido.TipoPedidoEnum.MaoDeObra &&
                PedidoConfig.DadosPedido.AmbientePedido && objInsert.TipoEntrega == (int)Pedido.TipoEntregaPedido.Balcao &&
                objInsert.IdProjeto == null && !objInsert.FromOrcamentoRapido && objInsert.IdPedidoAnterior == null)
            {
                AmbientePedido ambiente = new AmbientePedido();
                ambiente.Ambiente = "Balcão";
                ambiente.Descricao = "Balcão";
                ambiente.IdPedido = idPedido;
                ambiente.Qtde = 1;
                AmbientePedidoDAO.Instance.Insert(session, ambiente);
            }

            return idPedido;
        }

        public override int Update(Pedido objUpdate)
        {
            FilaOperacoes.AtualizarPedido.AguardarVez();

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
                finally
                {
                    FilaOperacoes.AtualizarPedido.ProximoFila();
                }
            }
        }

        public override int Update(GDASession session, Pedido objUpdate)
        {
            #region Declaração de variáveis

            var ped = GetElementByPrimaryKey(session, objUpdate.IdPedido);
            var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(session, objUpdate.IdPedido);
            var idLojaOriginal = ObtemIdLoja(session, objUpdate.IdPedido);
            var clienteRevenda = ClienteDAO.Instance.IsRevenda(session, objUpdate.IdCli);
            var idFuncionarioCliente = ClienteDAO.Instance.ObtemIdFunc(session, objUpdate.IdCli);
            var pedidoPossuiOrdemCarga = PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, objUpdate.IdPedido);

            #endregion

            #region Validações de permissão

            if (UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.Vendedor && !Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao))
            {
                if (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição && !Config.PossuiPermissao(Config.FuncaoMenuPedido.GerarReposicao))
                {
                    throw new Exception("Apenas o gerente pode emitir pedido de reposição.");
                }
                else if (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia)
                {
                    throw new Exception("Apenas o gerente pode emitir pedido de garantia.");
                }
            }

            #endregion

            #region Validações de cliente

            // Verifica se o cliente possui contas a receber vencidas se nao for garantia.
            if (ped.IdCli != objUpdate.IdCli && !FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro && objUpdate.TipoVenda != (int)Pedido.TipoVendaPedido.Garantia)
            {
                var clienteBloqueiaPedidoContaVencida = ClienteDAO.Instance.ObtemValorCampo<bool>(session, "BloquearPedidoContaVencida", string.Format("Id_Cli={0}", objUpdate.IdCli));
                var clientePossuiContaVencida = ContasReceberDAO.Instance.ClientePossuiContasVencidas(session, objUpdate.IdCli);

                if (clienteBloqueiaPedidoContaVencida && clientePossuiContaVencida)
                {
                    LancarExceptionValidacaoPedidoFinanceiro("Cliente bloqueado. Motivo: Contas a receber em atraso.", objUpdate.IdPedido, true, null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Finalizacao);
                }
            }

            // Verifica se este orçamento pode ter desconto
            if (PedidoConfig.Desconto.ImpedirDescontoSomativo && UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador && objUpdate.Desconto > 0)
            {
                var clientePossuiDesconto = DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(session, objUpdate.IdCli, 0, null, objUpdate.IdPedido, null);

                if (clientePossuiDesconto)
                {
                    throw new Exception("O cliente já possui desconto por grupo/subgrupo, não é permitido lançar outro desconto.");
                }
            }

            #endregion

            #region Validações de orçamento

            // Verifica se o id do orcamento informado existe.
            if (objUpdate.IdOrcamento > 0 && !OrcamentoDAO.Instance.Exists(session, objUpdate.IdOrcamento.Value))
            {
                throw new Exception("O Orcamento informado no pedido não existe.");
            }

            #endregion

            #region Validações do pedido

            #region Loja

            if (idLojaOriginal > 0 && idLojaOriginal != objUpdate.IdLoja)
            {
                /* Chamado 53901. */
                if (!PedidoConfig.AlterarLojaPedido)
                {
                    throw new Exception("Não é permitido alterar a loja do pedido.");
                }

                foreach (var prodPed in produtosPedido)
                {
                    // Esse produto não pode ser utilizado, pois a loja do seu subgrupo é diferente da loja do pedido.
                    var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLojaPeloProduto(session, (int)prodPed.IdProd);

                    if (!idsLojaSubgrupoProd.Any(f => f == objUpdate.IdLoja))
                    {
                        throw new Exception("Não é possível alterar a loja deste pedido, as lojas cadastradas para o subgrupo de um ou mais produtos é diferente da loja selecionada para o pedido.");
                    }

                    if (GrupoProdDAO.Instance.BloquearEstoque(session, (int)prodPed.IdGrupoProd, (int)prodPed.IdSubgrupoProd))
                    {
                        var estoque = ProdutoLojaDAO.Instance.GetEstoque(session, objUpdate.IdLoja, prodPed.IdProd, null, IsProducao(session, objUpdate.IdPedido), false, true);

                        if (estoque < produtosPedido.Where(f => f.IdProd == prodPed.IdProd).Sum(f => f.Qtde))
                        {
                            throw new Exception(string.Format("O produto {0} possui apenas {1} em estoque na loja selecionada.", prodPed.DescrProduto, estoque));
                        }
                    }
                }
            }

            #endregion

            #region Tipo pedido

            if (objUpdate.TipoPedido == 0)
            {
                throw new Exception("Não foi informado o Tipo do pedido.");
            }

            if (objUpdate.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial)
            {
                var clienteControlaEstoqueVidro = ClienteDAO.Instance.ObtemValorCampo<bool>(session, "ControlarEstoqueVidros", string.Format("Id_Cli={0}", objUpdate.IdCli));

                if (!clienteControlaEstoqueVidro)
                {
                    throw new Exception("O cliente deve controlar estoque para ser utilizado em um pedido de mão-de-obra especial.");
                }
            }

            #endregion

            #region Tipo venda

            // Verifica se o código do pedido do cliente já foi cadastrado.
            if (objUpdate.TipoVenda < 3)
            {
                CodigoClienteUsado(session, objUpdate.IdPedido, objUpdate.IdCli, objUpdate.CodCliente, false);
            }

            #endregion

            #region Cliente

            // Caso o pedido tenha pagamento antecipado e o usuário esteja tentando alterar o cliente, não permite
            if (objUpdate.IdCli != ped.IdCli && TemPagamentoAntecipadoRecebido(session, objUpdate.IdPedido))
            {
                throw new Exception("O cliente deste pedido não pode ser alterado pois já existe um pagamento antecipado para este pedido.");
            }

            #endregion

            #region Fast delivery

            // Se o pedido tiver marcado com fast delivery e se tiver valida as aplicações dos produtos.
            if (objUpdate.FastDelivery)
            {
                foreach (var produtoPedido in produtosPedido.Where(f => f.IdAplicacao > 0))
                {
                    if (EtiquetaAplicacaoDAO.Instance.NaoPermitirFastDelivery(session, produtoPedido.IdAplicacao.Value))
                    {
                        var codInternoAplicacao = EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(session, produtoPedido.IdAplicacao.Value);

                        throw new Exception(string.Format("Erro|O produto {0} tem a aplicacao {1} e esta aplicacao não permite fast delivery.", produtoPedido.DescrProduto, codInternoAplicacao));
                    }
                }
            }

            #endregion

            #region Desconto

            /* Chamado 22806. */
            if (objUpdate.Desconto != ped.Desconto && ped.Situacao != Pedido.SituacaoPedido.Ativo && ped.Situacao != Pedido.SituacaoPedido.AtivoConferencia)
            {
                throw new Exception("Não é possível alterar o desconto deste pedido, ele não está ativo.");
            }

            /* Chamado 28243. */
            if ((objUpdate.Desconto > 0 || objUpdate.Acrescimo > 0) && (produtosPedido?.Count()).GetValueOrDefault() == 0)
            {
                throw new Exception("Não é possível definir o percentual/valor de desconto/acréscimo no pedido caso o mesmo não possua produtos.");
            }

            // Verifica se o desconto que já exista no pedido pode ser mantido pelo usuário que está atualizando o pedido, 
            // tendo em vista que o mesmo possa ter sido lançado por um administrador
            if (objUpdate.Desconto == ped.Desconto && ped.Desconto > 0 && !DescontoPermitido(session, objUpdate))
            {
                throw new Exception("O desconto lançado anteriormente está acima do permitido para este login.");
            }

            #endregion

            #region Situação

            if (objUpdate.Situacao == Pedido.SituacaoPedido.Ativo)
            {
                if (ped.Situacao == Pedido.SituacaoPedido.ConfirmadoLiberacao)
                {
                    throw new Exception("Não é possível mudar a situação do pedido de confirmado para ativo, feche a tela e tente realizar a operação novamente.");
                }
                else if (ped.Situacao == Pedido.SituacaoPedido.Conferido)
                {
                    throw new Exception("Não é possível mudar a situação do pedido de conferido para ativo, feche a tela e tente realizar a operação novamente.");
                }
            }

            // Não permite atualizar o pedido se já estiver confirmado/liberado.
            if (ped.Situacao == Pedido.SituacaoPedido.Confirmado)
            {
                throw new Exception(string.Format("Não é possível alterar dados do pedido depois de {0}.", PedidoConfig.LiberarPedido ? "liberado" : "confirmado"));
            }

            #endregion

            #region Sinal/pagamento antecipado

            /* Chamado 43090. */
            if (objUpdate.ValorEntrada < 0)
            {
                throw new Exception("O valor de entrada do pedido não pode ser negativo. Edite o pedido, atualize os valores e tente novamente.");
            }

            // Não permite que pedido de garantia e reposição tenham sinal
            if (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia || objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição)
            {
                if (ped.RecebeuSinal)
                {
                    throw new Exception("Não é possivel alterar o tipo de venda para Garantia ou Reposição pois este pedido já possui sinal recebido.");
                }

                objUpdate.ValorEntrada = 0;
                objUpdate.ValorPagamentoAntecipado = 0;
            }

            if (objUpdate.IdCli != ped.IdCli && (ped.IdSinal > 0 || ped.IdPagamentoAntecipado > 0))
            {
                throw new Exception("Não é possível alterar o cliente deste pedido pois o mesmo já possui sinal/pagamento antecipado recebido.");
            }

            if (!PedidoConfig.LiberarPedido && objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.AVista && (ped.IdSinal > 0 || ped.IdPagamentoAntecipado > 0))
            {
                throw new Exception(string.Format("Este pedido teve um {0} recebido, não é possivel alterá-lo para à vista.", ped.IdSinal > 0 ? "sinal" : "pagto. antecipado"));
            }

            if (ped.ValorEntrada != objUpdate.ValorEntrada && ped.ValorEntrada > 0 && ped.RecebeuSinal)
            {
                throw new Exception("O sinal deste pedido já foi recebido, não é possível alterar o valor de entrada.");
            }

            #endregion

            #endregion

            #region Validações de obra

            if (objUpdate.IdObra > 0 && ObraDAO.Instance.ObtemSituacao(session, objUpdate.IdObra.Value) != Obra.SituacaoObra.Confirmada)
            {
                throw new Exception("A obra informada não esta confirmada.");
            }

            if (PedidoConfig.DadosPedido.UsarControleNovoObra)
            {
                if (objUpdate.IdObra > 0 && objUpdate.Desconto > 0)
                {
                    throw new Exception("Não é permitido lançar desconto em pedidos de obra, pois o valor do m² já foi definido com o cliente.");
                }

                if (objUpdate.IdObra > 0 && objUpdate.Acrescimo > 0)
                {
                    throw new Exception("Não é permitido lançar acréscimo em pedidos de obra, pois o valor do m² já foi definido com o cliente.");
                }
            }

            #endregion

            #region Validações de ordem de carga

            // Se o pedido ja tiver oc gerada não pode alterar a data de entrega.
            if (pedidoPossuiOrdemCarga && objUpdate.DataEntrega != ObtemDataEntrega(session, objUpdate.IdPedido))
            {
                throw new Exception("O pedido já possui OC gerada, não é possível alterar a data de entrega.");
            }

            #endregion

            #region Atualização dos dados do pedido

            if ((objUpdate.Importado || !objUpdate.SelVendEnabled) && idFuncionarioCliente > 0 && PedidoConfig.DadosPedido.BuscarVendedorEmitirPedido)
            {
                objUpdate.IdFunc = idFuncionarioCliente.Value;
            }

            if (objUpdate.IdOrcamento == 0)
            {
                objUpdate.IdOrcamento = null;
            }

            if (!PedidoConfig.Comissao.AlterarPercComissionado)
            {
                objUpdate.PercComissao = objUpdate.IdComissionado > 0 ? ComissionadoDAO.Instance.ObtemValorCampo<float>(session, "Percentual",
                    string.Format("IdComissionado={0}", objUpdate.IdComissionado.Value)) : 0;
            }

            // Se tiver sido lançado 100% de desconto no pedido, altera o desconto para reais, porque o desconto ficando 100%,
            // a propriedade DescontoTotal fica incorreta e na liberação também
            if (objUpdate.Desconto == 100 && objUpdate.TipoDesconto == 1)
            {
                objUpdate.Desconto = objUpdate.TotalSemDesconto;
                objUpdate.TipoDesconto = 2;
            }

            /* Chamado 28687. */
            objUpdate.Importado = ped.Importado;
            // Marca recebeu sinal e situação de acordo com o que está no banco.
            objUpdate.IdPagamentoAntecipado = ped.IdPagamentoAntecipado;
            objUpdate.IdSinal = ped.IdSinal;
            objUpdate.ValorPagamentoAntecipado = ped.ValorPagamentoAntecipado;
            objUpdate.Situacao = ped.Situacao;
            objUpdate.TipoVenda = objUpdate.TipoVenda.GetValueOrDefault((int)Pedido.TipoVendaPedido.AVista);
            objUpdate.IdProjeto = ped.IdProjeto;

            if (ped.Situacao == Pedido.SituacaoPedido.Confirmado)
            {
                objUpdate.Situacao = Pedido.SituacaoPedido.Confirmado;
            }

            if (ped.BloquearDescontoAcrescimoAtualizar)
            {
                objUpdate.Acrescimo = ped.Acrescimo;
                objUpdate.TipoAcrescimo = ped.TipoAcrescimo;
                objUpdate.Desconto = ped.Desconto;
                objUpdate.TipoDesconto = ped.TipoDesconto;
            }

            // Se o controle de desconto por forma de pagamento estiver desabilitado e o pedido for à vista, não é necessário informar a forma de pagamento.
            if (!FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto && objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.AVista)
            {
                objUpdate.IdFormaPagto = null;
            }

            // Se o comissionado não tiver sido informado, zera percComissao e valorComissao.
            if (objUpdate.IdComissionado == null)
            {
                objUpdate.PercComissao = 0;
                objUpdate.ValorComissao = 0;
            }

            // Recupera data e hora de cadastro da DataPedido.
            if (objUpdate.DataPedido.Hour == 0)
            {
                objUpdate.DataPedido = objUpdate.DataPedido.AddHours(ped.DataCad.Hour).AddMinutes(ped.DataCad.Minute).AddSeconds(ped.DataCad.Second);
            }

            // Remove o idObra caso não seja mais obra o tipo de venda.
            if (objUpdate.TipoVenda != (int)Pedido.TipoVendaPedido.Obra)
            {
                objUpdate.IdObra = null;
                objUpdate.ValorPagamentoAntecipado = 0;
            }

            // Atualiza o tipo de venda e a parcela do pedido, para que o desconto à vista da tabela do cliente seja recuperado corretamente.
            objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET TipoVenda={0}{1} WHERE IdPedido={2}", objUpdate.TipoVenda,
                objUpdate.IdParcela > 0 ? string.Format(", IdParcela={0}", objUpdate.IdParcela) : string.Empty, objUpdate.IdPedido));

            var retorno = UpdateBase(session, objUpdate);

            #endregion

            #region Atualização da parcela do pedido

            if (ped.IdCli != objUpdate.IdCli)
            {
                var parcelasCliente = ParcelasDAO.Instance.GetByCliente(session, objUpdate.IdCli, ParcelasDAO.TipoConsulta.Prazo);

                if (parcelasCliente != null && parcelasCliente.Count > 0 && parcelasCliente.All(f => f.IdParcela != (int)objUpdate.IdParcela.GetValueOrDefault()))
                {
                    objUpdate.IdParcela = (uint)parcelasCliente[0].IdParcela;
                }
                else
                {
                    /* Chamado 54857. */
                    objUpdate.IdParcela = null;
                }
            }

            #endregion

            #region Atualiza preço dos produtos do pedido

            // Primeiramente, a empresa não pode trabalhar com produtos na obra ou o pedido não pode estar associado a uma obra, o inverso dessa condição indica que o pedido está associado a uma obra
            // e que os produtos dele possuem um preço definido previamente no cadastro da mesma.
            // Atualiza os valores dos produtos caso o tipo de entrega ou o cliente sejam alterados.
            // Atualiza os valores dos produtos caso a empresa utilize tabela de desconto/acréscimo e uma das condições a seguir sejam verdadeiras:
            // Tipo de venda alterado, forma de pagamento alterada ou parcela alterada.
            if ((objUpdate.IdObra.GetValueOrDefault() == 0 || !PedidoConfig.DadosPedido.UsarControleNovoObra) &&
                ((ped.TipoEntrega != objUpdate.TipoEntrega || ped.IdCli != objUpdate.IdCli) ||
                (PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista && (ped.TipoVenda != objUpdate.TipoVenda || objUpdate.IdFormaPagto != ped.IdFormaPagto || objUpdate.IdParcela != ped.IdParcela))))
            {
                AtualizarValorTabelaProdutosPedido(session, ped.Desconto != objUpdate.Desconto || ped.TipoDesconto != objUpdate.TipoDesconto, ped, objUpdate);
            }

            #endregion

            #region Atualização de valores do pedido

            // Remove e aplica comissão, desconto e acréscimo.
            RemoveComissaoDescontoAcrescimo(session, ped, objUpdate, produtosPedido);
            AplicaComissaoDescontoAcrescimo(session, ped, objUpdate, produtosPedido);

            UpdateTotalPedido(session, objUpdate, false, true, ped.Desconto != objUpdate.Desconto || ped.TipoDesconto != objUpdate.TipoDesconto, true);

            objUpdate.Total = GetTotal(session, objUpdate.IdPedido);

            // Atualiza a tabela com o valor da comissão.
            if (objUpdate.Total > 0)
            {
                PedidoComissaoDAO.Instance.Create(session, objUpdate);
            }

            #endregion

            #region Atualização das parcelas do pedido

            if ((objUpdate.FastDelivery != ped.FastDelivery && objUpdate.Total != ParcelasPedidoDAO.Instance.ObtemTotalPorPedido(session, ped.IdPedido)) || ped.IdCli != objUpdate.IdCli)
            {
                RecalculaParcelas(session, ref objUpdate, TipoCalculoParcelas.Valor);
            }

            // Salva as parcelas do pedido.
            SalvarParcelas(session, objUpdate);

            #endregion

            #region Atualização da obra

            // Se o pagamento era obra e nao é mais atualiza o saldo da mesma.
            if (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Obra && objUpdate.TipoVenda != (int)Pedido.TipoVendaPedido.Obra)
            {
                ObraDAO.Instance.AtualizaSaldo(session, ped.IdObra.Value, false, false);
            }

            #endregion

            return retorno;
        }

        /// <summary>
        /// Método utilizado apenas para gerar pedido pelo projeto
        /// </summary>
        public uint InsertBase(Pedido objInsert)
        {
            return InsertBase(null, objInsert);
        }

        /// <summary>
        /// Método utilizado apenas para gerar pedido pelo projeto
        /// </summary>
        /// <param name="objInsert"></param>
        /// <returns></returns>
        public uint InsertBase(GDASession sessao, Pedido objInsert)
        {
            objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
            objInsert.DataCad = DateTime.Now;

            if (objInsert.DataPedido.Hour == 0)
                objInsert.DataPedido = objInsert.DataPedido.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute).AddSeconds(DateTime.Now.Second);

            objInsert.IdPedido = base.Insert(sessao, objInsert);

            // Associa textos de pedidos padrões
            TextoPedidoDAO.Instance.AssociaTextoPedidoPadrao(sessao, objInsert.IdPedido);

            return objInsert.IdPedido;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Método utilizado apenas para confirmar e cancelar pedido
        /// </summary>
        /// <param name="objUpdate"></param>
        public int UpdateBase(Pedido objUpdate)
        {
            return UpdateBase(null, objUpdate);
        }

        /// <summary>
        /// Método utilizado apenas para confirmar e cancelar pedido
        /// </summary>
        /// <param name="objUpdate"></param>
        public int UpdateBase(GDASession sessao, Pedido objUpdate)
        {
            /* Chamado 43090. */
            if (objUpdate.ValorEntrada < 0)
                throw new Exception("O valor de entrada do pedido não pode ser negativo. Edite o pedido, atualize os valores e tente novamente.");

            Pedido ped = GetElement(sessao, objUpdate.IdPedido);

            // Chamado 18844: Recupera o valor no banco do pagto antecipado deste pedido
            objUpdate.ValorPagamentoAntecipado = ped.ValorPagamentoAntecipado;

            // Chamado 24273: Ao remover a obra, o pedido continuava com o valorpagamentoantecipado pree
            if (objUpdate.IdObra.GetValueOrDefault() == 0 && objUpdate.IdPagamentoAntecipado.GetValueOrDefault() == 0)
                objUpdate.ValorPagamentoAntecipado = 0;

            int retorno = base.Update(sessao, objUpdate);

            LogAlteracaoDAO.Instance.LogPedido(sessao, ped, GetElement(objUpdate.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);

            //UpdateTotalPedido(objUpdate.IdPedido, objUpdate.Desconto);
            //PedidoComissaoDAO.Instance.Create(objUpdate);

            return retorno;
        }

        #endregion
    }
}
