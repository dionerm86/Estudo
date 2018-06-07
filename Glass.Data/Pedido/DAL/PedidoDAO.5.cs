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
        #region Busca pedidos para exportação

        public Pedido[] GetForPedidoExportar(uint idPedido, uint idCli, string nomeCli, string codCliente, string dataIni, string dataFim)
        {
            bool temFiltro;
            string filtroAdicional;

            // Situações de pedidos que são permitidas para exportação
            string situacoes = (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + "," + (int)Pedido.SituacaoPedido.Confirmado;

            string sql = Sql(idPedido, 0, null, null, 0, idCli, nomeCli, 0, codCliente, 0, null, null, null, null,
                null, null, null, null, null, dataIni, dataFim, null, null, null, 0, false, false, 0, 0, 0,
                0, 0, null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro);

            // Busca apenas pedidos nas situações confirmado e confirmado liberação (usando GDAParameter não funciona)
            filtroAdicional += " And p.situacao In (" + situacoes + ")";

            // Só busca pedidos que foram gerados PCP e que não esteja em aberto
            if (Geral.ControlePCP)
            {
                filtroAdicional += " And IF(pe.IdPedido is not null, pe.Situacao in (" + (int)PedidoEspelho.SituacaoPedido.Finalizado + "," +
                    (int)PedidoEspelho.SituacaoPedido.Impresso + "," + (int)PedidoEspelho.SituacaoPedido.ImpressoComum + "), 0)";
            }

            // Só busca pedidos não exportados
            filtroAdicional += String.Format(" and coalesce((" + PedidoExportacaoDAO.Instance.SqlSituacaoExportacao("p.idPedido") + "),{0})={0}",
                (int)PedidoExportacao.SituacaoExportacaoEnum.Cancelado);

            /* Chamado 55675. */
            filtroAdicional += string.Format(" AND IF((p.TipoPedido={0} AND p.TipoEntrega={1}) OR p.TipoPedido={2}, 1, 0)",
                (int)Pedido.TipoPedidoEnum.Revenda, (int)Pedido.TipoEntregaPedido.Entrega, (int)Pedido.TipoPedidoEnum.Venda);

            Pedido[] model = objPersistence.LoadData(sql.Replace(FILTRO_ADICIONAL, filtroAdicional) + " order by p.idPedido",
                GetParam(nomeCli, codCliente, null, null, null, null, dataIni, dataFim, null, null, "")).ToArray();

            return model;
        }

        #endregion

        #region Busca pedidos para finalização/confirmação Financeiro

        private string SqlFinalizarFinanc(uint idPedido, string codCliente, uint idCliente, string nomeCliente,
            uint idOrcamento, string endereco, string bairro, string dataPedidoIni, string dataPedidoFim,
            uint idLoja, int situacao, float alturaProd, int larguraProd, bool selecionar, out bool temFiltro,
            out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select ");
            var criterio = new StringBuilder();

            sql.Append(selecionar ? @"p.*, coalesce(l.nomeFantasia, l.razaoSocial) as NomeLoja,
                f.Nome as NomeFunc, (select cast(group_concat(distinct idItemProjeto) as char)
                from produtos_pedido where idPedido=p.idPedido) as idItensProjeto, tmp.motivoErroFinalizarFinanc, tmp.motivoErroConfirmarFinanc,
                '$$$' as criterio" : "count(*)");

            if (selecionar)
            {
                sql.Append(FinanceiroConfig.TelaFinalizacaoFinanceiro.ExibirRazaoSocial ?
                    ", COALESCE(cli.nome, cli.NomeFantasia) as NomeCliente" : ", COALESCE(cli.NomeFantasia, cli.nome) as NomeCliente");
            }

            sql.AppendFormat(@"
                from pedido p
                INNER JOIN cliente cli ON (p.idCli = cli.id_cli)
                LEFT JOIN loja l ON (p.idLoja = l.idLoja)
                LEFT JOIN funcionario f ON (p.idFunc = f.idFunc)
                LEFT JOIN (
                    SELECT * FROM (
                        SELECT idPedido, motivoErroFinalizarFinanc, motivoErroConfirmarFinanc
                        FROM observacao_finalizacao_financeiro
                        WHERE motivo=" + (int)ObservacaoFinalizacaoFinanceiro.MotivoEnum.Aberto + @" 
                        ORDER BY idObsFinanc DESC
                    ) as o
                    GROUP BY idPedido
                ) as tmp ON (p.idPedido = tmp.idPedido)
                where 1 {0}", FILTRO_ADICIONAL);

            temFiltro = false;
            StringBuilder fa = new StringBuilder(" and p.situacao in (" +
                (int)Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro + "," +
                (int)Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro + ")");

            if (idPedido > 0)
            {
                fa.AppendFormat(" and p.idPedido={0}", idPedido);
                criterio.Append("Pedido: " + idPedido + "     ");
            }

            if (!String.IsNullOrEmpty(codCliente))
                fa.Append(" and p.codCliente like ?codCliente");

            if (idCliente > 0)
            {
                fa.AppendFormat(" and p.idCli={0}", idCliente);
                criterio.Append("Cód Cliente: " + idCliente + "     ");
            }
            else if (!String.IsNullOrEmpty(nomeCliente) || !String.IsNullOrEmpty(endereco) || !String.IsNullOrEmpty(bairro))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, endereco, bairro, null, null, 0);
                fa.AppendFormat(" and p.idCli in ({0})", ids);
                criterio.Append("Cliente: " + nomeCliente + "     ");
            }

            if (idOrcamento > 0)
            {
                fa.AppendFormat(" and p.idOrcamento={0}", idOrcamento);
                criterio.Append("Orçamento: " + idOrcamento + "     ");
            }

            if (!String.IsNullOrEmpty(dataPedidoIni))
            {
                fa.Append(" and p.dataPedido>=?dataPedidoIni");
                criterio.Append("Data Cad. Ini: " + dataPedidoIni + "     ");
            }

            if (!String.IsNullOrEmpty(dataPedidoFim))
            {
                fa.Append(" and p.dataPedido<=?dataPedidoFim");
                criterio.Append("Data Cad. Fim: " + dataPedidoFim + "     ");
            }

            if (idLoja > 0)
            {
                fa.AppendFormat(" and p.idLoja={0}", idLoja);
                criterio.Append("Loja: " + LojaDAO.Instance.GetNome(idLoja) + "     ");
            }

            if (situacao > 0)
            {
                fa.AppendFormat(" and p.situacao={0}", situacao);
                var p = new Pedido() { Situacao = (Pedido.SituacaoPedido)situacao };
                criterio.Append("Situacao: " + p.DescrSituacaoPedido + "     ");
            }

            if (alturaProd > 0)
            {
                fa.Append(" and p.idPedido in (select * from (select idPedido from produtos_pedido where altura=?alturaProd) as temp)");
                criterio.Append("Altura Prod.: " + alturaProd + "     ");
            }

            if (larguraProd > 0)
            {
                fa.AppendFormat(" and p.idPedido in (select * from (select idPedido from produtos_pedido where largura={0}) as temp)", larguraProd);
                criterio.Append("Largura Prod.: " + larguraProd + "     ");
            }

            filtroAdicional = fa.ToString();
            return sql.ToString().Replace("$$$", criterio.ToString());
        }

        private GDAParameter[] GetParamFinalizarFinanc(string codCliente, string dataPedidoIni,
            string dataPedidoFim, float alturaProd)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codCliente))
                lst.Add(new GDAParameter("?codCliente", "%" + codCliente + "%"));

            if (!String.IsNullOrEmpty(dataPedidoIni))
                lst.Add(new GDAParameter("?dataPedidoIni", DateTime.Parse(dataPedidoIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataPedidoFim))
                lst.Add(new GDAParameter("?dataPedidoFim", DateTime.Parse(dataPedidoFim + " 23:59:59")));

            if (alturaProd > 0)
                lst.Add(new GDAParameter("?alturaProd", alturaProd));

            return lst.ToArray();
        }

        public IList<Pedido> ObtemItensFinalizarFinanceiro(uint idPedido, string codCliente, uint idCliente, string nomeCliente,
            uint idOrcamento, string endereco, string bairro, string dataPedidoIni, string dataPedidoFim,
            uint idLoja, int situacao, float alturaProd, int larguraProd, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "p.idPedido desc";

            bool temFiltro;
            string filtroAdicional, sql = SqlFinalizarFinanc(idPedido, codCliente, idCliente, nomeCliente, idOrcamento,
                endereco, bairro, dataPedidoIni, dataPedidoFim, idLoja, situacao, alturaProd, larguraProd, true,
                out temFiltro, out filtroAdicional);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetParamFinalizarFinanc(codCliente, dataPedidoIni, dataPedidoFim, alturaProd));
        }

        public int ObtemNumeroItensFinalizarFinanceiro(uint idPedido, string codCliente, uint idCliente, string nomeCliente,
            uint idOrcamento, string endereco, string bairro, string dataPedidoIni, string dataPedidoFim,
            uint idLoja, int situacao, float alturaProd, int larguraProd)
        {
            bool temFiltro;
            string filtroAdicional, sql = SqlFinalizarFinanc(idPedido, codCliente, idCliente, nomeCliente, idOrcamento,
                endereco, bairro, dataPedidoIni, dataPedidoFim, idLoja, situacao, alturaProd, larguraProd, true,
                out temFiltro, out filtroAdicional);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamFinalizarFinanc(codCliente,
                dataPedidoIni, dataPedidoFim, alturaProd));
        }

        public IList<Pedido> ObtemItensFinalizarFinanceiroRpt(uint idPedido, string codCliente, uint idCliente, string nomeCliente,
            uint idOrcamento, string endereco, string bairro, string dataPedidoIni, string dataPedidoFim,
            uint idLoja, int situacao, float alturaProd, int larguraProd)
        {
            bool temFiltro;
            string filtroAdicional, sql = SqlFinalizarFinanc(idPedido, codCliente, idCliente, nomeCliente, idOrcamento,
                endereco, bairro, dataPedidoIni, dataPedidoFim, idLoja, situacao, alturaProd, larguraProd, true,
                out temFiltro, out filtroAdicional);

            return objPersistence.LoadData(sql.Replace(FILTRO_ADICIONAL, filtroAdicional),
                GetParamFinalizarFinanc(codCliente, dataPedidoIni, dataPedidoFim, alturaProd)).OrderByDescending(f => f.IdPedido).ToList();
        }


        #endregion

        #region Pesquisa de pedidos para etiquetas

        public Pedido[] GetForEtiquetas(string idsProdPed, string idsAmbPed)
        {
            string sql = "select * from pedido where false";

            if (!String.IsNullOrEmpty(idsProdPed) && idsProdPed != "0")
                sql += " or idPedido in (select distinct idPedido from produtos_pedido_espelho where idProdPed in (" + idsProdPed + "))";

            if (!String.IsNullOrEmpty(idsAmbPed) && idsAmbPed != "0")
                sql += " or idPedido in (select distinct idPedido from ambiente_pedido_espelho where idAmbientePedido in (" + idsAmbPed + "))";

            return objPersistence.LoadData(sql).ToArray();
        }

        #endregion

        #region Gerar Pedido

        /// <summary>
        /// Gera um pedido a partir do orçamento passado
        /// </summary>
        public uint GerarPedido(uint idOrcamento)
        {
            var login = UserInfo.GetUserInfo;

            lock (_gerarPedidoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        #region Declaração de variáveis

                        // Busca o orçamento.
                        var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(transaction, idOrcamento);
                        // Produtos do orçamento.
                        var produtosOrcamento = ProdutosOrcamentoDAO.Instance.GetByOrcamento(transaction, idOrcamento, true);
                        var ambientesOrcamento = produtosOrcamento.Where(p => !p.IdProdParent.HasValue).ToList();
                        // Verifica se existe algum pedido, gerado através do orçamento atual, que não esteja cancelado, nesse caso, o orçamento não pode gerar um novo pedido.
                        var idPedidoNaoCanceladoAssociadoOrcamento = ExecuteScalar<int?>(transaction, string.Format("SELECT IdPedido FROM pedido WHERE Situacao<>{0} AND IdOrcamento={1}",
                            (int)Pedido.SituacaoPedido.Cancelado, orcamento.IdOrcamento));
                        // Recupera a medição mais recente do orçamento.
                        var idMedicaoMaisRecente = !string.IsNullOrWhiteSpace(orcamento.IdsMedicao) ?
                            orcamento.IdsMedicao.Split(',').Select(f => f.StrParaInt()).Where(f => f > 0).OrderByDescending(f => f).First() : 0;
                        // Verifica se o cliente possui desconto.
                        var clientePossuiDesconto = DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(transaction, orcamento.IdCliente.Value, idOrcamento, null, 0, null);
                        // Verifica se o cliente poossui contas vencidas.
                        var clientePossuiContasVencidas = ContasReceberDAO.Instance.ClientePossuiContasVencidas(transaction, orcamento.IdCliente.Value);
                        // Recupera a aituação atual do cliente.
                        var situacaoCliente = ClienteDAO.Instance.GetSituacao(transaction, orcamento.IdCliente.Value);
                        // Verifica se o cliente deve ser bloqueado caso existam contas vencidas.
                        var clienteBloquearContaVencida = ClienteDAO.Instance.ObtemValorCampo<bool>(transaction, "bloquearPedidoContaVencida", string.Format("Id_Cli={0}", orcamento.IdCliente));
                        // Recupera o funcionario associado ao cliente.
                        var idVendCliente = ClienteDAO.Instance.ObtemIdFunc(transaction, orcamento.IdCliente.Value);
                        // Recupera a parcela padrão do cliente.
                        var tipoPagto = ClienteDAO.Instance.ObtemTipoPagto(transaction, orcamento.IdCliente.Value);

                        DateTime dataEntrega, dataFastDelivery;
                        uint idPedido = 0;
                        uint idProdPed = 0;

                        #endregion

                        #region Validações

                        // Verifica se ao menos um produto do orçamento foi marcado para gerar pedido (Negociar?).
                        if (OrcamentoConfig.NegociarParcialmente && !ambientesOrcamento.Any(f => f.IdProdPed.GetValueOrDefault() == 0 && f.Negociar))
                            throw new Exception("Selecione pelo menos 1 produto para ser negociado.");

                        if (orcamento.TipoVenda == null)
                            throw new Exception("Selecione tipo de venda para este orçamento antes de gerar pedido.");

                        // Verifica se o vendedor do orçamento foi selecionado.
                        if (orcamento.IdFuncionario.GetValueOrDefault() == 0)
                            throw new Exception("Selecione um vendedor para este orçamento antes de gerar pedido.");

                        // Verifica se o tipo do orçamento foi selecionado.
                        if (orcamento.TipoOrcamento.GetValueOrDefault() == 0)
                            throw new Exception("Selecione o tipo do orçamento.");

                        // Verifica se o cliente foi informado.
                        if (orcamento.IdCliente == null || orcamento.IdCliente == 0)
                            throw new Exception("Cadastre o cliente informado no orçamento antes de gerar pedido.");

                        // Impede a geração do pedido caso o cliente não esteja ativo.
                        if (situacaoCliente != (int)SituacaoCliente.Ativo)
                            throw new Exception("O cliente não está ativo.");

                        // Verifica se o cliente possui contas a receber vencidas se nao for garantia.
                        if (!FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro && clienteBloquearContaVencida && clientePossuiContasVencidas)
                            throw new Exception("Cliente bloqueado. Motivo: Contas a receber em atraso.");

                        // Verifica se este orçamento pode ter desconto.
                        if (PedidoConfig.Desconto.ImpedirDescontoSomativo && clientePossuiDesconto && orcamento.Desconto > 0 && !login.IsAdministrador)
                            throw new Exception("O cliente já possui desconto por grupo/subgrupo, não é permitido lançar outro desconto.");

                        // Verifica se já foi gerado um pedido para este orçamento.
                        if (orcamento.Situacao != (int)Orcamento.SituacaoOrcamento.NegociadoParcialmente && idPedidoNaoCanceladoAssociadoOrcamento.GetValueOrDefault() > 0)
                            throw new Exception(string.Format("Já foi gerado um pedido para este orçamento. Número do pedido: {0}.", idPedidoNaoCanceladoAssociadoOrcamento));

                        // Verifica se existem produtos no orçamento.
                        if (ExecuteScalar<bool>(transaction, string.Format("SELECT COUNT(*)=0 FROM produtos_orcamento WHERE IdOrcamento={0}", idOrcamento)))
                            throw new Exception("Insira pelo menos um item neste orçamento antes de gerar pedido.");

                        /* Chamado 56301. */
                        if (ambientesOrcamento.Any(f => f.IdProduto > 0 && f.IdSubgrupoProd.GetValueOrDefault() == 0))
                            throw new Exception(string.Format("Informe o subgrupo dos produtos {0} antes de gerar o pedido.",
                                string.Join(", ", ambientesOrcamento.Where(f => f.IdProduto > 0 && f.IdSubgrupoProd == 0).Select(f => f.CodInterno).Distinct().ToList())));

                        #endregion

                        #region Bloqueio itens tipo pedido

                        if (orcamento.TipoOrcamento != null && PedidoConfig.DadosPedido.BloquearItensTipoPedido)
                        {
                            var idProdutoComparar = 0;
                            var idCorVidroProdutoComparar = 0;
                            float espessuraProdutoComparar = 0;
                            var lojaBloqueaItensCorEspessura = LojaDAO.Instance.GetIgnorarBloquearItensCorEspessura(transaction, orcamento.IdLoja.GetValueOrDefault());
                            var materiaisVidroMesmaCorEspessura = MaterialItemProjetoDAO.Instance.VidrosMesmaCorEspessura(transaction, idOrcamento);

                            // Impede que o pedido seja gerado com produtos de cor e espessura diferentes. (Materiais de projeto)
                            if ((PedidoConfig.DadosPedido.BloquearItensCorEspessura && !lojaBloqueaItensCorEspessura) && orcamento.TipoOrcamento == (int)Orcamento.TipoOrcamentoEnum.Venda &&
                                !materiaisVidroMesmaCorEspessura)
                                throw new Exception("Não é possível incluir produtos de cor e espessura diferentes.");

                            foreach (var po in ambientesOrcamento.Where(f => f.IdProduto > 0))
                            {
                                // Não negocia os produtos já negociados ou que não serão negociados
                                if (OrcamentoConfig.NegociarParcialmente && (po.IdProdPed != null || !po.Negociar))
                                    continue;

                                if (idProdutoComparar == 0)
                                {
                                    idProdutoComparar = (int)po.IdProduto.Value;
                                    idCorVidroProdutoComparar = ProdutoDAO.Instance.ObtemIdCorVidro(transaction, idProdutoComparar).GetValueOrDefault();
                                    espessuraProdutoComparar = ProdutoDAO.Instance.ObtemEspessura(transaction, idProdutoComparar);
                                }

                                var idCorVidro = ProdutoDAO.Instance.ObtemIdCorVidro(transaction, (int)po.IdProduto.Value);
                                var espessura = ProdutoDAO.Instance.ObtemEspessura(transaction, (int)po.IdProduto.Value);
                                var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(transaction, (int)po.IdProduto.Value);
                                var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(transaction, (int)po.IdProduto.Value);
                                var subgrupoProducao = SubgrupoProdDAO.Instance.IsSubgrupoProducao(transaction, idGrupoProd, idSubgrupoProd);

                                if (orcamento.TipoOrcamento == (int)Orcamento.TipoOrcamentoEnum.Venda && idGrupoProd != (uint)NomeGrupoProd.MaoDeObra &&
                                    (idGrupoProd != (uint)NomeGrupoProd.Vidro || (idGrupoProd == (uint)NomeGrupoProd.Vidro && subgrupoProducao)))
                                {
                                    throw new Exception("Não é possível incluir produtos de revenda em um pedido de venda.");
                                }
                                if (orcamento.TipoOrcamento == (int)Orcamento.TipoOrcamentoEnum.Revenda &&
                                    ((idGrupoProd == (uint)NomeGrupoProd.Vidro && !subgrupoProducao) || idGrupoProd == (uint)NomeGrupoProd.MaoDeObra))
                                {
                                    throw new Exception("Não é possível incluir produtos de venda em um pedido de revenda.");
                                }
                                // Impede que o pedido seja gerado com produtos de cor e espessura diferentes.
                                else if ((PedidoConfig.DadosPedido.BloquearItensCorEspessura && !lojaBloqueaItensCorEspessura) && orcamento.TipoOrcamento == (int)Orcamento.TipoOrcamentoEnum.Venda &&
                                    idGrupoProd == (uint)NomeGrupoProd.Vidro && (idCorVidro != idCorVidroProdutoComparar || espessura != espessuraProdutoComparar))
                                {
                                    throw new Exception("Não é possível incluir produtos de cor e espessura diferentes.");
                                }
                            }
                        }

                        #endregion

                        #region Insere o pedido

                        var pedido = new Pedido
                        {
                            IdLoja = orcamento.IdLoja > 0 ? orcamento.IdLoja.Value : login.IdLoja,
                            IdFunc = PedidoConfig.DadosPedido.BuscarVendedorEmitirPedido && idVendCliente > 0 ? idVendCliente.Value : login.CodUser,
                            IdCli = orcamento.IdCliente.Value,
                            IdOrcamento = idOrcamento,
                            IdProjeto = orcamento.IdProjeto,
                            TipoEntrega = orcamento.TipoEntrega,
                            TipoVenda = orcamento.TipoVenda,
                            Situacao = Pedido.SituacaoPedido.Ativo,
                            DataPedido = DateTime.Now,
                            FromOrcamentoRapido = true,
                            CustoPedido = orcamento.Custo,
                            Total = orcamento.Total,
                            EnderecoObra = orcamento.EnderecoObra,
                            BairroObra = orcamento.BairroObra,
                            CidadeObra = orcamento.CidadeObra,
                            CepObra = orcamento.CepObra,
                            Obs = orcamento.Obs,
                            GerarPedidoProducaoCorte = false,
                            TipoPedido = orcamento.TipoOrcamento.GetValueOrDefault((int)Pedido.TipoPedidoEnum.Venda),
                            ValorEntrega = orcamento.ValorEntrega,
                            NumParc = orcamento.NumParc,
                            IdParcela = orcamento.IdParcela,
                            PrazoEntrega = orcamento.PrazoEntrega,
                            DataEntrega = (GetDataEntregaMinima(transaction, orcamento.IdCliente.Value, null, orcamento.TipoOrcamento.GetValueOrDefault((int)Pedido.TipoPedidoEnum.Venda), orcamento.TipoEntrega,
                                out dataEntrega, out dataFastDelivery) ?
                                dataEntrega : RotaDAO.Instance.GetDataRota(transaction, orcamento.IdCliente.Value, orcamento.DataEntrega != null ? orcamento.DataEntrega.Value : DateTime.Now)) ?? orcamento.DataEntrega,
                            IdMedidor = idMedicaoMaisRecente > 0 ? MedicaoDAO.Instance.GetMedidor(transaction, (uint)idMedicaoMaisRecente) : null,
                            PercentualComissao = PedidoConfig.Comissao.PerComissaoPedido ? ClienteDAO.Instance.ObtemPercentualComissao(transaction, orcamento.IdCliente.Value) : 0,

                            // Chamado 68242: Insere o acréscimo no pedido, para que ao removê-lo e adicioná-lo novamente logo abaixo 
                            // nos métodos "RemoveComissaoDescontoAcrescimo" e "AplicaComissaoDescontoAcrescimo", não seja aplicado novamente sem que seja removido
                            // (que é o que acontece se não preenhcer o acréscimo e tipo acréscimo neste momento)
                            Acrescimo = orcamento.Acrescimo,
                            TipoAcrescimo = orcamento.TipoAcrescimo,
                            Desconto = orcamento.Desconto,
                            TipoDesconto = orcamento.TipoDesconto,
                            IdComissionado = orcamento.IdComissionado,
                            PercComissao = orcamento.PercComissao
                        };

                        if (tipoPagto > 0)
                        {
                            var parcelaPadrao = ParcelasDAO.Instance.GetElementByPrimaryKey(transaction, tipoPagto.Value);

                            if (parcelaPadrao != null && parcelaPadrao.NumParcelas > 0)
                                pedido.TipoVenda = (int)Pedido.TipoVendaPedido.APrazo;
                        }

                        idPedido = InsertBase(transaction, pedido);

                        if (idPedido == 0)
                            throw new Exception("Inserção do pedido retornou 0.");

                        // Insere o id do pedido no campo idPedidoGerado do orçamento
                        objPersistence.ExecuteCommand(transaction, string.Format("UPDATE orcamento SET IdPedidoGerado={0} WHERE IdOrcamento={1}", idPedido, idOrcamento));

                        #endregion

                        // Se a empresa não trabalha com venda de vidro, a forma de gerar pedido é diferente
                        if (Geral.NaoVendeVidro())
                        {
                            #region Inserção de produtos para empresas que NÃO VENDEM vidro

                            foreach (var po in ambientesOrcamento)
                            {
                                // Não negocia os produtos já negociados ou que não serão negociados
                                if (OrcamentoConfig.NegociarParcialmente && (po.IdProdPed != null || !po.Negociar))
                                    continue;

                                var prodPed = new ProdutosPedido
                                {
                                    IdPedido = idPedido,
                                    IdProd = po.IdProduto.Value,
                                    Qtde = po.Qtde.Value,
                                    TotM = po.TotM,
                                    TotM2Calc = po.TotMCalc,
                                    Altura = po.AlturaCalc,
                                    AlturaReal = po.Altura,
                                    Largura = po.Largura,
                                    CustoProd = po.Custo,
                                    AliqIcms = po.AliquotaIcms,
                                    ValorIcms = po.ValorIcms,
                                    AliqIpi = po.AliquotaIpi,
                                    ValorIpi = po.ValorIpi,
                                    Redondo = po.Redondo,
                                    ValorTabelaOrcamento = po.ValorTabela,
                                    ValorTabelaPedido = ProdutoDAO.Instance.GetValorTabela(transaction, (int)po.IdProduto.Value, pedido.TipoEntrega, pedido.IdCli, false, false, po.PercDescontoQtde,
                                        (int?)idPedido, null, null),
                                    TipoCalculoUsadoOrcamento = po.TipoCalculoUsado,
                                    TipoCalculoUsadoPedido = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)po.IdProduto.Value),
                                    PercDescontoQtde = po.PercDescontoQtde,
                                    ValorDescontoQtde = po.ValorDescontoQtde,
                                    ValorDescontoCliente = po.ValorDescontoCliente,
                                    ValorAcrescimoCliente = po.ValorAcrescimoCliente,
                                    ValorUnitarioBruto = po.ValorUnitarioBruto,
                                    TotalBruto = po.TotalBruto,
                                    IdProcesso = po.IdProcesso,
                                    IdAplicacao = po.IdAplicacao,
                                    ValorVendido = po.ValorProd ?? 0
                                };

                                ValorTotal.Instance.Calcular(
                                    transaction,
                                    pedido,
                                    prodPed,
                                    Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                                    true,
                                    po.Beneficiamentos.CountAreaMinimaSession(transaction)
                                );

                                // O valor vendido e o total devem ser preenchidos, assim como os outros campos abaixo, 
                                // caso contrário o valor deste produto ficaria zerado ou incorreto no pedido, antes,
                                // todos os campos abaixo estavam sendo preenchidos apenas se a opção PedidoConfig.DadosPedido.AlterarValorUnitarioProduto fosse true
                                prodPed.Total = po.Total.Value;
                                prodPed.ValorAcrescimo = po.ValorAcrescimo + (PedidoConfig.DadosPedido.AmbientePedido ? 0 : po.ValorAcrescimoProd);
                                prodPed.ValorDesconto = po.ValorDesconto + (PedidoConfig.DadosPedido.AmbientePedido ? 0 : po.ValorDescontoProd);
                                prodPed.ValorAcrescimoProd = !PedidoConfig.DadosPedido.AmbientePedido ? 0 : po.ValorAcrescimoProd;
                                prodPed.ValorDescontoProd = !PedidoConfig.DadosPedido.AmbientePedido ? 0 : po.ValorDescontoProd;
                                prodPed.ValorComissao = PedidoConfig.Comissao.ComissaoPedido ? po.ValorComissao : 0;
                                idProdPed = ProdutosPedidoDAO.Instance.InsertBase(transaction, prodPed, pedido);

                                if (idProdPed == 0)
                                    throw new Exception("Inserção do produto do pedido retornou 0.");

                                // Atualiza o produto, indicando o produto do pedido que foi gerado
                                if (idProdPed > 0)
                                    objPersistence.ExecuteCommand(transaction, string.Format("UPDATE produtos_orcamento SET IdProdPed={0} WHERE IdProd={1}", idProdPed, po.IdProd));
                            }

                            #endregion
                        }
                        else
                        {
                            #region Inserção de produtos para empresas que VENDEM vidro

                            var pedidoReposicaoGarantia = pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição || pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia;
                            var pedidoMaoObraEspecial = pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial;

                            foreach (var po in ambientesOrcamento)
                            {
                                // Não negocia os produtos já negociados ou que não serão negociados
                                if (OrcamentoConfig.NegociarParcialmente && (po.IdProdPed != null || !po.Negociar))
                                    continue;

                                uint? idAmbiente = null; // Ambiente do pedido
                                var itensProjetoId = new Dictionary<uint, uint>();

                                // Cria um ambiente se a empresa trabalha com ambiente no pedido ou 
                                // se o produto do orçamento for um cálculo de projeto
                                if (PedidoConfig.DadosPedido.AmbientePedido || po.IdItemProjeto > 0)
                                {
                                    // Se o produto for um cálculo de projeto, faz uma cópia para o pedido
                                    if (po.IdItemProjeto > 0 && !itensProjetoId.ContainsKey(po.IdItemProjeto.Value))
                                    {
                                        var idItemProjeto = ClonaItemProjeto(transaction, po.IdItemProjeto.Value, idPedido);
                                        itensProjetoId.Add(po.IdItemProjeto.Value, idItemProjeto);
                                    }

                                    var ambiente = new AmbientePedido
                                    {
                                        IdPedido = idPedido,
                                        Ambiente = po.Ambiente,
                                        Descricao = po.Descricao,
                                        IdItemProjeto = po.IdItemProjeto != null ? (uint?)itensProjetoId[po.IdItemProjeto.Value] : null
                                    };

                                    // Na Center Box/Mega Temper, a impressão do pedido é igual do orçamento, portanto, 
                                    // precisa mostrar a quantidade na impressão do pedido 
                                    if (!PedidoConfig.RelatorioPedido.ExibirItensProdutosPedido)
                                        ambiente.Qtde = (int)po.Qtde;

                                    idAmbiente = AmbientePedidoDAO.Instance.Insert(transaction, ambiente);

                                    // Correção Mega Temper
                                    // Insere os produtos de projeto através do método específico
                                    if (ambiente.IdItemProjeto > 0)
                                    {
                                        var itemProjeto = ItemProjetoDAO.Instance.GetElement(transaction, ambiente.IdItemProjeto.Value);
                                        ProdutosPedidoDAO.Instance.InsereAtualizaProdProj(transaction, pedido, idAmbiente, itemProjeto, true, false);
                                    }

                                    // Atualiza os dados de desconto/acréscimo do ambiente.
                                    objPersistence.ExecuteCommand(transaction, string.Format(@"UPDATE ambiente_pedido SET TipoDesconto=?tipoDesconto, Desconto=?desconto, TipoAcrescimo=?tipoAcrescimo,
                                        Acrescimo=?acrescimo, Descricao=?descricao WHERE IdAmbientePedido={0}", idAmbiente), new GDAParameter("?tipoDesconto", po.TipoDesconto),
                                        new GDAParameter("?desconto", po.Desconto), new GDAParameter("?tipoAcrescimo", po.TipoAcrescimo), new GDAParameter("?acrescimo", po.Acrescimo),
                                        new GDAParameter("?descricao", po.Descricao));

                                    idProdPed = idAmbiente.Value;
                                }

                                // Adiciona os itens internos como os produtos do pedido
                                if (po.TemItensProdutoSession(transaction))
                                {
                                    foreach (var poChild in produtosOrcamento.Where(p => p.IdProdParent == po.IdProd))
                                    {
                                        // O custo do produto de orçamento é atualizado somente se o cliente estiver inserido no orçamento, 
                                        // para certificar que o custo inserido no pedido será o valor correto é necessário atualizar novamente
                                        decimal valorProd = poChild.ValorProd != null ? po.ValorProd.Value : 0;

                                        var prodPed = new ProdutosPedido
                                        {
                                            IdPedido = idPedido,
                                            IdAmbientePedido = idAmbiente,
                                            IdItemProjeto = poChild.IdItemProjeto != null ? (uint?)itensProjetoId[poChild.IdItemProjeto.Value] : null,
                                            IdProd = poChild.IdProduto != null ? poChild.IdProduto.Value : 0,
                                            Qtde = poChild.Qtde != null ? poChild.Qtde.Value : 0,
                                            TotM = poChild.TotM,
                                            TotM2Calc = poChild.TotMCalc,
                                            Altura = poChild.AlturaCalc,
                                            AlturaReal = poChild.Altura,
                                            Largura = poChild.Largura,
                                            Espessura = poChild.Espessura > 0 ? poChild.Espessura : poChild.IdProduto > 0 ?
                                            ProdutoDAO.Instance.ObtemEspessura(transaction, (int)poChild.IdProduto.Value) : 0,
                                            ValorVendido = poChild.ValorProd ?? 0
                                        };

                                        ValorTotal.Instance.Calcular(
                                            transaction,
                                            pedido,
                                            prodPed,
                                            Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                                            true,
                                            poChild.Beneficiamentos.CountAreaMinimaSession(transaction)
                                        );

                                        prodPed.AliqIcms = poChild.AliquotaIcms;
                                        prodPed.ValorIcms = poChild.ValorIcms;
                                        prodPed.AliqIpi = poChild.AliquotaIpi;
                                        prodPed.ValorIpi = poChild.ValorIpi;
                                        prodPed.Redondo = poChild.Redondo;
                                        prodPed.ValorTabelaOrcamento = poChild.ValorTabela;
                                        prodPed.ValorTabelaPedido = ProdutoDAO.Instance.GetValorTabela(transaction, (int)prodPed.IdProd, pedido.TipoEntrega, pedido.IdCli, false, false,
                                            poChild.PercDescontoQtde, (int)prodPed.IdPedido, null, null);
                                        prodPed.TipoCalculoUsadoOrcamento = poChild.TipoCalculoUsado;
                                        prodPed.TipoCalculoUsadoPedido = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)prodPed.IdProd);
                                        prodPed.PercDescontoQtde = poChild.PercDescontoQtde;
                                        prodPed.ValorDescontoQtde = poChild.ValorDescontoQtde;
                                        prodPed.ValorDescontoCliente = poChild.ValorDescontoCliente;
                                        prodPed.ValorAcrescimoCliente = poChild.ValorAcrescimoCliente;
                                        prodPed.Beneficiamentos = poChild.Beneficiamentos;
                                        prodPed.ValorUnitarioBruto = poChild.ValorUnitarioBruto;
                                        prodPed.TotalBruto = poChild.TotalBruto;
                                        prodPed.CustoProd = poChild.Custo;
                                        prodPed.IdProcesso = poChild.IdProcesso;
                                        prodPed.IdAplicacao = poChild.IdAplicacao;
                                        // O valor vendido e o total devem ser preenchidos, assim como os outros campos abaixo, 
                                        // caso contrário o valor deste produto ficaria zerado ou incorreto no pedido, antes,
                                        // todos os campos abaixo estavam sendo preenchidos apenas se a opção PedidoConfig.DadosPedido.AlterarValorUnitarioProduto fosse true
                                        prodPed.Total = poChild.Total != null ? poChild.Total.Value : 0;
                                        prodPed.ValorAcrescimo = poChild.ValorAcrescimo + (PedidoConfig.DadosPedido.AmbientePedido ? 0 : poChild.ValorAcrescimoProd);
                                        prodPed.ValorDesconto = poChild.ValorDesconto + (PedidoConfig.DadosPedido.AmbientePedido ? 0 : poChild.ValorDescontoProd);
                                        prodPed.ValorAcrescimoProd = !PedidoConfig.DadosPedido.AmbientePedido ? 0 : poChild.ValorAcrescimoProd;
                                        prodPed.ValorDescontoProd = !PedidoConfig.DadosPedido.AmbientePedido ? 0 : poChild.ValorDescontoProd;
                                        prodPed.ValorComissao = PedidoConfig.Comissao.ComissaoPedido ? poChild.ValorComissao : 0;

                                        // Verifica se o valor unitário do produto foi informado, pois pode acontecer do usuário inserir produtos zerados
                                        // o que não é permitido em pedidos que não são de produção, reposição ou garantia.
                                        if (!pedidoReposicaoGarantia && prodPed.ValorVendido == 0)
                                            throw new Exception(string.Format("O produto {0} não pode ter valor zerado.", ProdutoDAO.Instance.ObtemDescricao(transaction, (int)prodPed.IdProd)));

                                        idProdPed = ProdutosPedidoDAO.Instance.InsertBase(transaction, prodPed, pedido);

                                        if (idProdPed == 0)
                                            throw new Exception("Inserção do produto do pedido retornou 0.");

                                        //Caso o produto seja do subgrupo de tipo laminado, insere os filhos
                                        var tipoSubgrupoProd = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(transaction, (int)prodPed.IdProd);

                                        if (tipoSubgrupoProd == TipoSubgrupoProd.VidroLaminado || tipoSubgrupoProd == TipoSubgrupoProd.VidroDuplo)
                                        {
                                            var tipoEntrega = ObtemTipoEntrega(transaction, prodPed.IdPedido);

                                            foreach (var p in ProdutoBaixaEstoqueDAO.Instance.GetByProd(transaction, prodPed.IdProd, false))
                                            {
                                                var idProdPedFilho = ProdutosPedidoDAO.Instance.Insert(transaction, new ProdutosPedido()
                                                {
                                                    IdProdPedParent = prodPed.IdProdPed,
                                                    IdProd = (uint)p.IdProdBaixa,
                                                    IdProcesso = (uint)p.IdProcesso,
                                                    IdAplicacao = (uint)p.IdAplicacao,
                                                    IdPedido = prodPed.IdPedido,
                                                    IdAmbientePedido = prodPed.IdAmbientePedido,
                                                    Qtde = p.Qtde,
                                                    Beneficiamentos = p.Beneficiamentos,
                                                    Altura = p.Altura > 0 ? p.Altura : prodPed.Altura,
                                                    Largura = p.Largura > 0 ? p.Largura : prodPed.Largura,
                                                    ValorVendido = ProdutoDAO.Instance.GetValorTabela(transaction, p.IdProdBaixa, tipoEntrega, prodPed.IdCliente, false, false, 0, (int)prodPed.IdPedido, null, null),
                                                }, false, true);

                                                var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.IProdutoBaixaEstoqueRepositorioImagens>();

                                                var stream = new System.IO.MemoryStream();

                                                //Verifica se a matéria prima possui imagem
                                                var possuiImagem = repositorio.ObtemImagem(p.IdProdBaixaEst, stream);

                                                if (possuiImagem)
                                                    ProdutosPedidoDAO.Instance.SalvarImagemProdutoPedido(transaction, idProdPedFilho, stream);
                                            }
                                        }

                                    }
                                }

                                // Atualiza o produto, indicando o produto do pedido que foi gerado
                                if (idProdPed > 0)
                                    objPersistence.ExecuteCommand(transaction, string.Format("UPDATE produtos_orcamento SET IdProdPed={0} WHERE IdProd={1}", idProdPed, po.IdProd));
                            }

                            #endregion
                        }

                        // Finaliza o projeto
                        if (orcamento.IdProjeto != null)
                            ProjetoDAO.Instance.Finaliza(transaction, orcamento.IdProjeto.Value);

                        if (OrcamentoConfig.NegociarParcialmente)
                        {
                            var situacao = OrcamentoDAO.Instance.IsNegociadoParcialmente(transaction, idOrcamento) ?
                                (int)Orcamento.SituacaoOrcamento.NegociadoParcialmente : (int)Orcamento.SituacaoOrcamento.Negociado;

                            objPersistence.ExecuteCommand(transaction, string.Format("UPDATE orcamento SET Situacao={0} WHERE IdOrcamento={1}", situacao, idOrcamento));
                        }

                        if (!PedidoConfig.DadosPedido.AlterarValorUnitarioProduto)
                        {
                            // Atualiza o pedido, recalculando os valores dos produtos.
                            pedido = GetElementByPrimaryKey(transaction, idPedido);
                            pedido.TipoEntrega = orcamento.TipoEntrega;
                            pedido.ValoresParcelas = new decimal[] { pedido.Total };
                            pedido.DatasParcelas = new DateTime[] { DateTime.Now };

                            Update(transaction, pedido);

                            // Marca novamente os projetos como conferido.
                            foreach (var item in ItemProjetoDAO.Instance.GetByPedido(transaction, idPedido))
                                ItemProjetoDAO.Instance.CalculoConferido(transaction, item.IdItemProjeto);
                        }

                        #region Comissão/Desconto/Acréscimo

                        // Remove o percentual de comissão
                        objPersistence.ExecuteCommand(transaction, string.Format("UPDATE pedido SET PercComissao=0 WHERE IdPedido={0}", idPedido));

                        // Salva no pedido o funcionário que aplicou o desconto no orçamento.
                        if (orcamento.Desconto > 0)
                        {
                            var idFuncDesc = OrcamentoDAO.Instance.ObtemIdFuncDesc(transaction, orcamento.IdOrcamento);

                            /* Chamado 29245. */
                            if (idFuncDesc.GetValueOrDefault() == 0)
                                idFuncDesc = UserInfo.GetUserInfo.CodUser;

                            objPersistence.ExecuteCommand(transaction, string.Format("UPDATE pedido SET IdFuncDesc={0} WHERE IdPedido={1}", idFuncDesc, idPedido));
                        }

                        var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedidoLite(transaction, pedido.IdPedido, false, true);

                        RemoveComissaoDescontoAcrescimo(transaction, pedido, produtosPedido);
                        AplicaComissaoDescontoAcrescimo(transaction, pedido, Geral.ManterDescontoAdministrador, produtosPedido);

                        foreach (var a in (pedido as IContainerCalculo).Ambientes.Obter().Cast<AmbientePedido>())
                        {
                            if (a.Acrescimo == 0 && a.Desconto == 0)
                                continue;

                            var produtosAmbiente = produtosPedido.Where(p => p.IdAmbientePedido == a.IdAmbientePedido);

                            AmbientePedidoDAO.Instance.RemoverAcrescimo(transaction, pedido, a.IdAmbientePedido, produtosAmbiente);
                            AmbientePedidoDAO.Instance.RemoverDesconto(transaction, pedido, a.IdAmbientePedido, produtosAmbiente);

                            if (a.Acrescimo > 0)
                            {
                                AmbientePedidoDAO.Instance.AplicarAcrescimo(
                                    transaction,
                                    pedido,
                                    a.IdAmbientePedido,
                                    a.TipoAcrescimo,
                                    a.Acrescimo,
                                    produtosAmbiente
                                );
                            }

                            if (a.Desconto > 0)
                            {
                                AmbientePedidoDAO.Instance.AplicarDesconto(
                                    transaction,
                                    pedido,
                                    a.IdAmbientePedido,
                                    a.TipoDesconto,
                                    a.Desconto,
                                    produtosAmbiente
                                );
                            }

                            AmbientePedidoDAO.Instance.FinalizarAplicacaoAcrescimoDesconto(transaction, pedido, produtosAmbiente, true);
                        }

                        objPersistence.ExecuteCommand(transaction, string.Format(@"UPDATE pedido SET TipoDesconto=?tipoDesconto, Desconto=?desconto, TipoAcrescimo=?tipoAcrescimo, Acrescimo=?acrescimo
                            WHERE IdPedido={0}", idPedido), new GDAParameter("?tipoDesconto", orcamento.TipoDesconto), new GDAParameter("?desconto", orcamento.Desconto),
                            new GDAParameter("?tipoAcrescimo", orcamento.TipoAcrescimo), new GDAParameter("?acrescimo", orcamento.Acrescimo));

                        pedido.TipoDesconto = orcamento.TipoDesconto;
                        pedido.Desconto = orcamento.Desconto;
                        pedido.TipoAcrescimo = orcamento.TipoAcrescimo;
                        pedido.Acrescimo = orcamento.Acrescimo;

                        UpdateTotalPedido(transaction, idPedido);

                        #endregion

                        /* Cancela o pedido se o total do mesmo não coincidir com o total do orçamento (Margem de erro de R$0,50)
                         * Teve que ser retirado para confirmação porque na vidrália aconteceu do pedido 162677 ter sido gerado PCP com um valor diferente
                         * Teve que ser retirado da tempera de Vespasiano porque lá pedido original tem dois valores, à vista e à prazo, porém na conferência
                         * só o à vista (taxa à prazo). */
                        var totalPedido = GetTotal(transaction, idPedido);
                        var totalOrcamento = OrcamentoDAO.Instance.GetTotal(transaction, idOrcamento);

                        if ((!OrcamentoConfig.NegociarParcialmente || !OrcamentoDAO.Instance.PossuiPedidoGerado(idOrcamento)) &&
                            PedidoConfig.LiberarPedido && (totalPedido > totalOrcamento + (decimal)0.5 || totalPedido < totalOrcamento - (decimal)0.5))
                            throw new Exception("O pedido não poderá ser gerado, houve alguma modificação nos valores dos produtos ou no cadastro do cliente, recalcule o orçamento e tente gerar o pedido novamente.");

                        transaction.Commit();
                        transaction.Close();

                        return idPedido;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        #region Salva um log de alteração no orçamento com a falha

                        /* Chamado 48759. */
                        var logOrcamento = new LogAlteracao();
                        logOrcamento.Campo = "Geração pedido";
                        logOrcamento.DataAlt = DateTime.Now;
                        logOrcamento.IdFuncAlt = login.CodUser;
                        logOrcamento.IdRegistroAlt = (int)idOrcamento;
                        logOrcamento.NumEvento = 1;
                        logOrcamento.Referencia = "Orçamento: " + idOrcamento;
                        logOrcamento.Tabela = (int)LogAlteracao.TabelaAlteracao.Orcamento;
                        logOrcamento.ValorAtual = ex.Message != null ? ex.Message :
                            ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message :
                            "Não foi possível recuperar o motivo da falha.";

                        LogAlteracaoDAO.Instance.Insert(logOrcamento);

                        #endregion

                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Buscar pedidos prontos e não entregues

        public bool ExistemPedidosProntosNaoEntreguesPeriodo(int qtdeDias)
        {
            string sql = "SELECT * FROM pedido WHERE SituacaoProducao = 3 AND NOW() > DATE_ADD(DATAPRONTO, INTERVAL " + qtdeDias + " DAY) ";

            return objPersistence.LoadData(sql).Count() > 0;
        }

        #endregion

        #region Verifica se todos os produtos do pedido podem ser liberados sem estarem prontos na produção

        /// <summary>
        /// Verifica se todos os produtos do pedido podem ser liberados sem estarem prontos na produção
        /// </summary>
        public bool ProdutosPodemLiberarProducaoPendente(GDASession session, uint idPedido)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM produtos_pedido_espelho ppe
                    INNER JOIN produto prod ON (ppe.IdProd = prod.IdProd)
                    INNER JOIN subgrupo_prod sgp ON (prod.IdSubgrupoProd = sgp.IdSubGrupoProd)
                WHERE COALESCE(ppe.InvisivelFluxo, 0) = 0 
                    AND COALESCE(sgp.LiberarPendenteProducao, 0) = 0
                    AND ppe.IdPedido = " + idPedido;

            return objPersistence.ExecuteSqlQueryCount(session, sql) == 0;
        }

        #endregion

        #region API

        /// <summary>
        /// Busca os dados de vendas por pedidos para API
        /// </summary>
        /// <returns>1 - Tipo de Pedido, 2 - Data, 3 - Valor</returns>
        public List<Tuple<int, string, DateTime, decimal>> PesquisarVendasPedido()
        {
            var ini = DateTime.Now.AddDays(-15);
            var fim = DateTime.Now;

            //var ini = new DateTime(2015, 10, 6);
            //var fim = new DateTime(2015, 10, 19);

            var sql = @"
                SELECT CONCAT(TipoPedido, ';', Date(DataCad), ';', cast(SUM(Total) as char))
                FROM pedido
                WHERE
                    Situacao <> " + (int)Pedido.SituacaoPedido.Cancelado + @"
		                AND DATE(dataCad) >= ?dtIni
		                AND DATE(dataCad) <= ?dtFim
                        AND TipoPedido <> " + (int)Pedido.TipoPedidoEnum.Producao + @"
                        AND TipoVenda NOT IN (" + (int)Pedido.TipoVendaPedido.Garantia + @", " + (int)Pedido.TipoVendaPedido.Reposição + @")
                GROUP BY TipoPedido, DATE(DataCad)
                HAVING SUM(Total) > 0
                ORDER BY DataCad Desc";

            var dados = ExecuteMultipleScalar<string>(sql, new GDAParameter("dtIni", ini), new GDAParameter("dtFim", fim));

            var retorno = new List<Tuple<int, string, DateTime, decimal>>();

            foreach (var d in dados)
            {
                var str = d.Split(';');

                var tipoPedidoStr = Colosoft.Translator.Translate(((Pedido.TipoPedidoEnum)str[0].StrParaInt())).Format();

                retorno.Add(new Tuple<int, string, DateTime, decimal>(str[0].StrParaInt(), tipoPedidoStr, DateTime.Parse(str[1]), str[2].StrParaDecimal()));
            }

            return retorno;
        }

        /// <summary>
        /// Verifica se houve cadastro de pedido apos a data informada
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="dataInicial"></param>
        /// <returns></returns>
        public bool TeveCadPosterior(GDASession sessao, DateTime dataInicial)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM pedido
                WHERE DataCad > ?dtIni";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?dtIni", dataInicial)) > 0;
        }

        #endregion

        #region Valida Pedido para Liberação

        public string ValidaPedidoLiberacao(GDASession session, uint idPedido, int? tipoVenda, int? idFormaPagto, bool cxDiario, List<uint> idsPedido)
        {
            try
            {
                // Verifica se o pedido existe
                if (!PedidoExists(session, idPedido))
                    return "false|Não existe pedido com esse número.";

                var idCliente = ObtemIdCliente(session, idPedido);
                var tipoEntrega = ObtemTipoEntrega(session, idPedido);
                var isReposicaoGarantia = IsPedidoGarantia(session, idPedido.ToString()) || IsPedidoReposicao(session, idPedido.ToString());
                var existeEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido);
                var idLoja = ObtemIdLoja(session, idPedido);
                var naoIgnorar = !LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(session, idLoja);
                var idObra = GetIdObra(session, idPedido);

                // Verifica se o cliente está ativo
                if (ClienteDAO.Instance.GetSituacao(session, idCliente) != (int)SituacaoCliente.Ativo)
                    return "false|O cliente desse pedido está inativo.";

                // Verifica se o pedido possui funcionário
                if (string.IsNullOrEmpty(ObtemNomeFuncResp(session, idPedido)))
                    return "false|Este pedido não possui nenhum funcionário associado ao mesmo.";

                #region Validações da situação do pedido

                // Verifica se o pedido já foi liberado
                if (IsPedidoLiberado(session, idPedido))
                {
                    var idLiberacao = LiberarPedidoDAO.Instance.GetIdLiberacao(session, idPedido.ToString());
                    return string.Format("false|Este pedido já foi liberado.{0}", idLiberacao > 0 ? string.Format(" Número da liberação: {0}.", idLiberacao.Value) : string.Empty);
                }

                if (!IsPedidoConfirmado(session, idPedido))
                {
                    var retorno = "false|Esse pedido ainda não foi confirmado.";

                    if (!isReposicaoGarantia && TemSinalReceber(session, idPedido))
                        retorno += " Pedido possui sinal a receber.";
                    else if (!isReposicaoGarantia && TemPagamentoAntecipadoReceber(session, idPedido))
                        retorno += " Pedido possui pagamento antecipado a receber.";

                    return retorno;
                }

                #endregion

                #region Validações do pedido espelho

                if (existeEspelho)
                {
                    if (PedidoEspelhoDAO.Instance.ObtemSituacao(session, idPedido) == PedidoEspelho.SituacaoPedido.Aberto)
                        return "false|A conferência deste pedido deve estar finalizada ou impressa para poder liberá-lo.";

                    if (!PCPConfig.ControlarProducao && PedidoEspelhoDAO.Instance.ObtemSituacao(session, idPedido) != PedidoEspelho.SituacaoPedido.Finalizado)
                        return "false|Este pedido deve estar finalizado no PCP para ser liberado.";

                    // Verifica se o desconto lançado no pedido é o mesmo no pedido original e no PCP
                    if (ObterDesconto(session, (int)idPedido) != PedidoEspelhoDAO.Instance.ObterDesconto(session, (int)idPedido))
                        return "false|O desconto lançado no pedido original está diferente do desconto lançado no pedido espelho, retire o mesmo e lançe-o novamente no ícone ($) na lista de pedidos.";

                    // Verifica se o desconto lançado no pedido é o mesmo no pedido original e no PCP
                    if (ObterAcrescimo(session, (int)idPedido) != PedidoEspelhoDAO.Instance.ObterAcrescimo(session, (int)idPedido))
                        return "false|O acréscimo lançado no pedido original está diferente do acréscimo lançado no pedido espelho, retire o mesmo e lançe-o novamente no ícone ($) na lista de pedidos.";
                }
                else
                {
                    if (PCPConfig.ControlarProducao && PCPConfig.ImpedirLiberacaoPedidoSemPCP && (PossuiVidrosProducao(session, idPedido) || IsMaoDeObra(session, idPedido)))
                        return "false|Este pedido deve passar no PCP antes de ser liberado.";
                }

                #endregion

                #region Validações do pagamento antecipado

                // Verifica se o pagto antecipado do pedido é válido
                if (ObtemIdPagamentoAntecipado(session, idPedido) > 0 && ObtemValorPagtoAntecipado(session, idPedido) == 0)
                    return "false|O pedido possui pagamento antecipado mas o valor recebido está zerado, será necessário receber o valor novamente.";

                #endregion

                #region Validações da produção do pedido

                if ((Liberacao.DadosLiberacao.LiberarProdutosProntos && naoIgnorar) && !Liberacao.DadosLiberacao.LiberarPedidoProdutos && PossuiVidrosProducao(session, idPedido) &&
                    !IsPedidoAtrasado(session, idPedido, true) && !ProdutosPodemLiberarProducaoPendente(session, idPedido))
                {
                    var situacaoProducao = ObtemSituacaoProducao(session, idPedido);

                    if ((situacaoProducao == (int)Pedido.SituacaoProducaoEnum.Pendente || situacaoProducao == (int)Pedido.SituacaoProducaoEnum.NaoEntregue) &&
                        (!Liberacao.DadosLiberacao.LiberarClienteRota || !RotaDAO.Instance.ClientePossuiRota(session, idCliente)))
                        return "false|Algumas peças deste pedido ainda não estão prontas.";
                }

                #endregion

                #region Validações da Ordem de Carga

                if (OrdemCargaConfig.UsarControleOrdemCarga && tipoEntrega == DataSources.Instance.GetTipoEntregaEntrega())
                {
                    if ((ObtemDeveTransferir(session, idPedido) && !PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, OrdemCarga.TipoOCEnum.Venda, idPedido)) ||
                        !PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, idPedido) && !GerarPedidoProducaoCorte(session, idPedido) &&
                        (PossuiVidrosProducao(session, idPedido) || PossuiVidrosEstoque(session, idPedido) || PossuiVolume(session, idPedido)))
                        return "false|Este pedido deve ter uma OC venda gerada para ser liberado.";
                }

                #endregion

                #region Validações das informações de forma de pagamento do pedido

                if (!isReposicaoGarantia)
                {
                    if (Liberacao.DadosLiberacao.BloquearLiberacaoDadosPedido && tipoVenda.GetValueOrDefault() > 0 && GetTipoVenda(session, idPedido) != tipoVenda)
                        return string.Format("false|Este pedido não foi vendido como '{0}'.", Pedido.GetDescrTipoVenda(tipoVenda));

                    /* Chamado 65135. */
                    if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto && idFormaPagto.GetValueOrDefault() > 0 && ObtemFormaPagto(session, idPedido) != idFormaPagto)
                        throw new Exception("Como o controle de desconto por forma de pagamento e dados do produto está habilitado, não é possível liberar pedidos com formas de pagamento diferentes.");
                }

                #region Parcelas

                // Se a empresa não libera pedidos com parcelas diferentes então as parcelas devem ser validadas.
                if (Liberacao.BloquearLiberacaoParcelasDiferentes && idsPedido != null && idsPedido.Count > 0)
                {
                    var lstIdsParcela = new List<uint>();

                    foreach (var id in idsPedido)
                    {
                        var idParcela = ObtemIdParcela(session, id).GetValueOrDefault();

                        if (lstIdsParcela.Count == 0)
                            lstIdsParcela.Add(idParcela);

                        if (lstIdsParcela.Count > 0 && !lstIdsParcela.Contains(idParcela))
                            return "false|Somente pedidos com a mesma parcela podem ser liberados.";
                    }
                }

                #endregion

                #endregion

                // Se for pedido de obra, recalcula o saldo da mesma, em alguns casos o saldo já havia debitado o valor do pedido antes de ser liberado.
                if (idObra > 0)
                    ObraDAO.Instance.AtualizaSaldo(session, null, idObra.Value, cxDiario, false);

                return string.Format("true|{0}", idCliente);
            }
            catch (Exception ex)
            {
                return MensagemAlerta.FormatErrorMsg("false|", ex);
            }
        }

        #endregion

        #region App

        /// <summary>
        /// Finaliza o projeto do aplicativo criando o pedido e finalizando-o em seguida
        /// </summary>
        /// <param name="projeto">Dados do projeto que será finalizado.</param>
        /// <param name="imagens">Imagens do projeto.</param>
        public void FinalizarProjetoGerarPedidoApp(IProjeto projeto, IEnumerable<IImagemProjeto> imagens)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var rota = RotaDAO.Instance.GetByCliente(transaction, UserInfo.GetUserInfo.IdCliente.GetValueOrDefault());

                    var pedido = new Pedido();
                    pedido.IdCli = UserInfo.GetUserInfo.IdCliente.GetValueOrDefault();
                    pedido.TipoEntrega = rota != null && rota.EntregaBalcao ? (int)Pedido.TipoEntregaPedido.Balcao : projeto.IdTipoEntrega;
                    pedido.IdFunc = ClienteDAO.Instance.ObtemIdFunc(transaction, pedido.IdCli).GetValueOrDefault(0);
                    pedido.Situacao = Pedido.SituacaoPedido.Ativo;
                    pedido.DataPedido = DateTime.Now;
                    pedido.AliquotaIcms = 0.12f;
                    pedido.CustoPedido = projeto.CustoTotal;
                    pedido.CodCliente = projeto.Pedido;
                    pedido.Obs = projeto.Obs;
                    pedido.TipoPedido = (int)Pedido.TipoPedidoEnum.Venda;
                    pedido.TipoVenda = (int)Pedido.TipoVendaPedido.AVista;
                    pedido.IdLoja = UserInfo.GetUserInfo.IdLoja;
                    pedido.GeradoParceiro = true;

                    #region Define as informações de pagamento do pedido

                    // Recupera a parcela padrão do cliente.
                    var tipoPagto = ClienteDAO.Instance.ObtemTipoPagto(transaction, pedido.IdCli);

                    if (tipoPagto > 0)
                    {
                        var parcelaPadrao = ParcelasDAO.Instance.GetElementByPrimaryKey(transaction, tipoPagto.Value);

                        // Caso a parcela padrão seja uma parcela à prazo, altera o tipo de venda do pedido para À Prazo.
                        if (parcelaPadrao != null && parcelaPadrao.NumParcelas > 0)
                            pedido.TipoVenda = (int)Pedido.TipoVendaPedido.APrazo;
                    }

                    // Recupera a forma de pagamento padrão do cliente.
                    var idFormaPagtoCliente = ClienteDAO.Instance.ObtemIdFormaPagto(transaction, pedido.IdCli);

                    if (idFormaPagtoCliente > 0)
                    {
                        // Recupera as formas de pagamento disponíveis, para o cliente, de acordo com o tipo de venda do pedido.
                        var formasPagamento = FormaPagtoDAO.Instance.GetForPedido(transaction, (int)pedido.IdCli, pedido.TipoVenda.GetValueOrDefault());

                        // Caso a forma de pagamento, padrão do cliente, esteja presente nas opções de forma de pagamento do pedido, seleciona ela por padrão.
                        if (formasPagamento != null && formasPagamento.Count > 0 && formasPagamento.Select(f => f.IdFormaPagto).Contains(idFormaPagtoCliente))
                            pedido.IdFormaPagto = idFormaPagtoCliente;
                    }

                    #endregion

                    var idPedido = Insert(transaction, pedido);
                    if (idPedido == 0)
                        throw new Exception("Inserção do pedido retornou 0.");

                    var idObra = PedidoConfig.DadosPedido.UsarControleNovoObra ? GetIdObra(idPedido) : null;

                    var retornoValidacao = string.Empty;

                    foreach (var ip in projeto.Itens)
                    {
                        var projetoModelo = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(transaction, ip.IdProjetoModelo);

                        #region Cria projetos 

                        var itemProjeto = ItemProjetoDAO.Instance.NovoItemProjetoVazio(transaction, null,
                            null, null, idPedido, null, null, null, ip.IdProjetoModelo, ip.EspessuraVidro,
                            ip.IdCorVidro, 0, 0, true, ip.MedidaExata, true);

                        //// Salva as medidas da área de instalação, com a referência do item de projeto.
                        //var medidasItemProjeto = SalvarMedidasAreaInstalacao;

                        var medidasItemProjeto = SalvarMedidasAreaInstalacaoApp(transaction, projetoModelo, itemProjeto, ip.Medidas);
                        itemProjeto.Qtde = medidasItemProjeto.Where(f => f.IdMedidaProjeto == 1).FirstOrDefault().Valor;

                        // Busca as peças deste item, que serão utilizadas nas expressões
                        var pecasItemProjeto = PecaItemProjetoDAO.Instance.GetByItemProjeto(transaction, itemProjeto.IdItemProjeto, itemProjeto.IdProjetoModelo);

                        var pecasProjetoModelo = ObterPecasProjetoModeloApi(transaction, ip, pecasItemProjeto);

                        // Calcula as medidas das peças do projeto.
                        var lstPecaModelo = UtilsProjeto.CalcularMedidasPecas(transaction, itemProjeto, projetoModelo, pecasProjetoModelo, medidasItemProjeto, true, false,
                            out retornoValidacao);

                        // Insere Peças na tabela peca_item_projeto
                        PecaItemProjetoDAO.Instance.InsertFromPecaModelo(transaction, itemProjeto, ref lstPecaModelo);

                        // Insere as peças de vidro apenas se todas as Peça Projeto Modelo tiver idProd
                        var inserirPecasVidro = !lstPecaModelo.Any(f => f.IdProd == 0);
                        if (inserirPecasVidro)
                            // Insere Peças na tabela material_item_projeto
                            MaterialItemProjetoDAO.Instance.InserePecasVidro(transaction, idObra, UserInfo.GetUserInfo.IdCliente,
                                projeto.IdTipoEntrega, itemProjeto, projetoModelo, lstPecaModelo);


                        // Atualiza qtds dos materiais apenas
                        MaterialItemProjetoDAO.Instance.AtualizaQtd(transaction, idObra, UserInfo.GetUserInfo.IdCliente,
                            projeto.IdTipoEntrega, itemProjeto, projetoModelo);

                        ItemProjetoDAO.Instance.UpdateTotalItemProjeto(transaction, itemProjeto.IdItemProjeto);

                        uint? idProjeto = ItemProjetoDAO.Instance.GetIdProjeto(transaction, itemProjeto.IdItemProjeto);
                        uint? idOrcamento = ItemProjetoDAO.Instance.GetIdOrcamento(transaction, itemProjeto.IdItemProjeto);

                        if (idProjeto > 0)
                            ProjetoDAO.Instance.UpdateTotalProjeto(transaction, idProjeto.Value);
                        else if (idOrcamento > 0)
                        {
                            uint idProd = ProdutosOrcamentoDAO.Instance.ObtemIdProdutoPorIdItemProjeto(transaction, itemProjeto.IdItemProjeto);
                            if (idProd > 0)
                                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(transaction, idProd);

                            OrcamentoDAO.Instance.UpdateTotaisOrcamento(transaction, idOrcamento.Value);
                        }

                        #endregion

                        #region Finaliza projeto

                        itemProjeto = ItemProjetoDAO.Instance.GetElementByPrimaryKey(itemProjeto.IdItemProjeto);

                        uint idAmbienteNovo;

                        retornoValidacao = string.Empty;

                        /* Chamado 48676. */
                        if (itemProjeto == null)
                            throw new Exception("Não foi possível recuperar o projeto. Atualize a tela e confirme o projeto novamente.");


                        // Atualiza o campo ambiente no itemProjeto
                        ItemProjetoDAO.Instance.AtualizaAmbiente(transaction, itemProjeto.IdItemProjeto, ip.Ambiente);
                        itemProjeto.Ambiente = ip.Ambiente;

                        var lstPecas = PecaItemProjetoDAO.Instance.GetByItemProjeto(transaction, itemProjeto.IdItemProjeto, itemProjeto.IdProjetoModelo);

                        var idGrupoModelo = ProjetoModeloDAO.Instance.ObtemGrupoModelo(transaction, itemProjeto.IdProjetoModelo);
                        var codigoGrupoModelo = GrupoModeloDAO.Instance.ObtemDescricao(transaction, idGrupoModelo);

                        if (lstPecas.Count == 0 &&
                            ProjetoModeloDAO.Instance.ObtemCodigo(transaction, itemProjeto.IdProjetoModelo) != "OTR01" &&
                            /* Chamado 22588. */
                            !codigoGrupoModelo.ToLower().Contains("esquadria"))
                            throw new Exception("Informe as peças de vidro.");

                        // Verifica se as peças do item projeto estão de acordo com os materiais do mesmo. Chamado 9673.
                        foreach (var peca in lstPecas)
                        {
                            /* Chamado 63058. */
                            if (peca.Qtde == 0 || peca.IdProd == 0)
                                continue;

                            var material = MaterialItemProjetoDAO.Instance.GetMaterialByPeca(transaction, peca.IdPecaItemProj);

                            // Se a Peça Item Projeto não tiver IdProd, é porque não foi calculado os vidros.
                            if (peca.IdProd > 0 && (material == null || material.Qtde != peca.Qtde))
                            {
                                var ex = new Exception("Calcule as medidas novamente. Os materias do projeto não conferem com as peças do mesmo.");
                                ErroDAO.Instance.InserirFromException("CadProjetoAvulso.aspx", ex);
                                throw ex;
                            }
                            else if (peca.Altura == 0 || peca.Largura == 0)
                                throw new Exception(
                                    string.Format("A {0} da peça {1} está zerada. Informe o valor desta medida e confirme o projeto novamente.",
                                        peca.Altura == 0 ? "Altura" : "Largura", peca.CodInterno));

                            /* Chamado 24308. */
                            objPersistence.ExecuteCommand(transaction, string.Format("UPDATE peca_item_projeto SET ImagemEditada=0 WHERE IdPecaItemProj={0};", peca.IdPecaItemProj));
                        }

                        if (idPedido > 0)
                        {
                            /* Chamado 52637.
                             * Remove e aplica acréscimo/desconto/comissão no pedido somente uma vez.
                             * Antes essa atualização estava demorando muito porque era feita para cada ambiente. */
                            #region Remove acréscimo/desconto/comissão do pedido

                            var idsAmbientePedido = new List<uint>();
                            pedido = GetElementByPrimaryKey(transaction, idPedido);

                            // Remove acréscimo, desconto e comissão.
                            RemoveComissaoDescontoAcrescimo(transaction, pedido);

                            #endregion

                            var ambiente = AmbientePedidoDAO.Instance.GetIdByItemProjeto(itemProjeto.IdItemProjeto);

                            idAmbienteNovo = ProdutosPedidoDAO.Instance.InsereAtualizaProdProj(transaction, pedido, ambiente, itemProjeto, false, false, true);

                            #region Aplica acréscimo/desconto/comissão do pedido

                            var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedidoLite(transaction, pedido.IdPedido, false, true);

                            // Aplica acréscimo, desconto e comissão.
                            AplicaComissaoDescontoAcrescimo(transaction, pedido, Geral.ManterDescontoAdministrador, produtosPedido);

                            // Aplica acréscimo e desconto no ambiente.
                            if (OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento && idsAmbientePedido.Count > 0)
                                foreach (var idAmbPed in idsAmbientePedido)
                                {
                                    var produtosAmbiente = produtosPedido.Where(p => p.IdAmbientePedido == idAmbPed);

                                    var acrescimoAmbiente = AmbientePedidoDAO.Instance.ObterAcrescimo(transaction, idAmbPed);
                                    var descontoAmbiente = AmbientePedidoDAO.Instance.ObterAcrescimo(transaction, idAmbPed);

                                    if (acrescimoAmbiente > 0)
                                    {
                                        AmbientePedidoDAO.Instance.AplicarAcrescimo(
                                            transaction,
                                            pedido,
                                            idAmbPed,
                                            AmbientePedidoDAO.Instance.ObterTipoAcrescimo(transaction, idAmbPed),
                                            acrescimoAmbiente,
                                            produtosAmbiente
                                        );
                                    }

                                    if (descontoAmbiente > 0)
                                    {
                                        AmbientePedidoDAO.Instance.AplicarDesconto(
                                            transaction,
                                            pedido,
                                            idAmbPed,
                                            AmbientePedidoDAO.Instance.ObterTipoDesconto(transaction, idAmbPed),
                                            descontoAmbiente,
                                            produtosAmbiente
                                        );
                                    }
                                }

                            // Atualiza o total do pedido.
                            UpdateTotalPedido(transaction, pedido);

                            #endregion

                            // Verifica se todas as medidas de instalação foram inseridas
                            if (!itemProjeto.MedidaExata && itemProjeto.IdCorVidro > 0 && MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(transaction, itemProjeto.IdProjetoModelo, false).Count >
                                MedidaItemProjetoDAO.Instance.GetListByItemProjeto(transaction, itemProjeto.IdItemProjeto).Count && ProjetoModeloDAO.Instance.ObtemCodigo(transaction, itemProjeto.IdProjetoModelo) != "OTR01")
                                throw new Exception("Falha ao inserir medidas, confirme o projeto novamente.");

                            ItemProjetoDAO.Instance.CalculoConferido(transaction, itemProjeto.IdItemProjeto);
                        }

                        #endregion
                    }

                    UpdateTotalPedido(transaction, pedido);

                    var totalPedido = GetTotal(transaction, idPedido);

                    if (Math.Round(projeto.Total, 2) != Math.Round(totalPedido, 2))
                        throw new Exception(
                            string.Format(Globalizacao.Cultura.CulturaSistema, "Erro ao gerar pedido. Valor do pedido difere do valor do projeto. (Pedido: {0:C}, Projeto: {1:C}",
                                totalPedido, projeto.Total));

                    var emConferencia = false;

                    if (!imagens.Any())
                    {
                        FinalizarPedido(transaction, idPedido, ref emConferencia, false);

                        //PedidoEspelhoDAO.Instance.GeraEspelho(transaction, idPedido);
                        //PedidoEspelhoDAO.Instance.FinalizarPedido(transaction, idPedido);
                    }

                    foreach (var imagem in imagens)
                    {
                        using (var stream = imagem.Abrir())
                        {
                            var content = new byte[stream.Length];
                            stream.Read(content, 0, content.Length);

                            Anexo.InserirAnexo(IFoto.TipoFoto.Pedido, idPedido, content, imagem.Nome, imagem.Descricao);
                        }
                    }

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


        public IEnumerable<IMedidaItemProjeto> SalvarMedidasAreaInstalacaoApp(GDASession session, ProjetoModelo projetoModelo,
            ItemProjeto itemProjeto, IEnumerable<IMedidaItemProjeto> medidasItemProjeto)
        {
            // Insere a quantidade
            var medidaQtd = new MedidaItemProjeto();
            medidaQtd.IdItemProjeto = itemProjeto.IdItemProjeto;
            medidaQtd.IdMedidaProjeto = 1;
            medidaQtd.Valor = medidasItemProjeto.Where(f => f.IdMedidaProjeto == 1).FirstOrDefault().Valor;
            MedidaItemProjetoDAO.Instance.Insert(session, medidaQtd);

            #region Salva as medidas da área de instalação

            foreach (MedidaProjetoModelo mpm in MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(session, itemProjeto.IdProjetoModelo, false))
            {
                var med = medidasItemProjeto.FirstOrDefault(f => f.IdMedidaProjeto == (uint)mpm.IdMedidaProjeto);
                var alturaBox = itemProjeto.EspessuraVidro == 6 ? ProjetoConfig.AlturaPadraoProjetoBox6mm : ProjetoConfig.AlturaPadraoProjetoBoxAcima6mm;

                // Não insere a medida QTD, pois já foi inserida no código acima
                if (mpm.IdMedidaProjeto != 1)
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, mpm.IdMedidaProjeto, med.Valor);
            }

            #endregion

            return MedidaItemProjetoDAO.Instance.GetListByItemProjeto(itemProjeto.IdItemProjeto);
        }

        /// <summary>
        /// Retorna Lista de PecasModeloProjeto a partir da tbPecasModelo
        /// </summary>
        public static List<PecaProjetoModelo> ObterPecasProjetoModeloApi(GDASession session, IItemProjeto itemProjeto, IEnumerable<PecaItemProjeto> pecasItemProjeto)
        {
            // Busca as peças deste item, que serão utilizadas nas expressões

            var pecasProjetoModelo = new List<PecaProjetoModelo>();

            foreach (var pecaApp in itemProjeto.Pecas)
            {
                var peca = pecasItemProjeto.First(f => f.IdPecaProjMod == pecaApp.IdPecaProjMod);
                var pecaModelo = PecaProjetoModeloDAO.Instance.GetElementByPrimaryKey(session, pecaApp.IdPecaProjMod);

                pecasProjetoModelo.Add(new PecaProjetoModelo
                {
                    IdPecaProjMod = pecaApp.IdPecaProjMod,
                    IdPecaItemProj = peca.IdPecaItemProj,
                    IdProd = pecaApp.IdProd.GetValueOrDefault(),
                    Qtde = pecaApp.Qtde,
                    Obs = pecaApp.Obs,
                    Largura = pecaApp.Largura,
                    Altura = pecaApp.Altura,
                    CalculoAltura = pecaModelo.CalculoAltura,
                    CalculoLargura = pecaModelo.CalculoLargura,
                    CalculoQtde = pecaModelo.CalculoQtde,
                    Item = pecaApp.Item,
                    Tipo = pecaApp.Tipo
                });
            }

            return pecasProjetoModelo;
        }

        #endregion

        #region Atualização do valor de tabela dos produtos do pedido

        /// <summary>
        /// Atualiza o valor de tabela dos produtos do pedido.
        /// </summary>
        private void AtualizarValorTabelaProdutosPedido(GDASession session, bool alterouDesconto, Pedido antigo, Pedido novo)
        {
            #region Declaração de variáveis

            var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(session, novo.IdPedido);
            var itensProjeto = ItemProjetoDAO.Instance.GetByPedido(session, novo.IdPedido);

            #endregion

            #region Remoção do acréscimo, comissão e desconto

            RemoveComissaoDescontoAcrescimo(session, novo, produtosPedido);

            #endregion

            #region Atualização dos itens de projeto

            // Marca os projetos como não conferido, pois é mais complicado recalcular os projetos.
            foreach (var itemProjeto in itensProjeto)
            {
                var idAmbientePedido = AmbientePedidoDAO.Instance.GetIdByItemProjeto(session, itemProjeto.IdItemProjeto);
                var itemProjetoConferido = itemProjeto.Conferido;

                ProdutosPedidoDAO.Instance.InsereAtualizaProdProj(session, novo, idAmbientePedido, itemProjeto, false, false, false, true);

                // Este método é chamado através da atualização do pedido pela notinha verde. Dentro do método InsereAtualizaProdProj, o item de projeto é marcado como não conferido,
                // porém ele deve-se manter como conferido, pois não foi feita alteração no projeto, diretamente.
                if (itemProjetoConferido)
                {
                    ItemProjetoDAO.Instance.CalculoConferido(session, itemProjeto.IdItemProjeto);
                }
            }

            #endregion

            #region Atualização dos totais dos produtos do pedido

            // Percorre cada produto, do pedido, e recalcula seu valor unitário, com base no valor de tabela e no desconto/acréscimo do cliente.
            foreach (var produtoPedido in produtosPedido)
            {
                if (ProdutoDAO.Instance.VerificarAtualizarValorTabelaProduto(session, antigo, novo, produtoPedido))
                {
                    ProdutosPedidoDAO.Instance.RecalcularValores(session, produtoPedido, novo, false);
                    ProdutosPedidoDAO.Instance.UpdateBase(session, produtoPedido, novo, false);
                }
            }

            #endregion

            #region Atualização dos totais do pedido espelho

            AplicaComissaoDescontoAcrescimo(session, novo, Geral.ManterDescontoAdministrador, produtosPedido);
            UpdateTotalPedido(session, novo, false, false, alterouDesconto, true);

            #endregion
        }

        #endregion
    }
}
