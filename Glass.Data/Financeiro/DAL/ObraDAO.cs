using System;
using System.Collections.Generic;
using System.Linq;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ObraDAO : BaseDAO<Obra, ObraDAO>
    {
        //private ObraDAO() { }
        
        private static readonly object _receberObraLock = new object();

        #region Métodos de retorno de itens

        public enum TipoRetorno : long
        {
            IdPedido,
            TotalPedido
        }
        
        internal string SqlPedidos(string idObra, TipoRetorno tipoRetorno)
        {
            var campos = tipoRetorno == TipoRetorno.IdPedido ? "GROUP_CONCAT(IdPedido SEPARATOR ', ')" :
                tipoRetorno == TipoRetorno.TotalPedido ? "SUM(COALESCE(Total, 0))" : "*";
 
            var sql = string.Format("SELECT {0} FROM pedido WHERE IdObra={1}", campos, idObra);
            return sql;
        }
        
        internal string SqlPedidosAbertos(string idObra, string idsPedidosIgnorar, TipoRetorno tipoRetorno)
        {
            var campos = tipoRetorno == TipoRetorno.IdPedido ? "GROUP_CONCAT(IdPedido SEPARATOR ', ')" :
                tipoRetorno == TipoRetorno.TotalPedido ? "SUM(COALESCE(Total, 0))" : "*";

            var sql = string.Format("SELECT {0} FROM pedido WHERE Situacao IN ({1},{2},{3}) AND IdObra={4}", campos,
                (int)Pedido.SituacaoPedido.Ativo, (int)Pedido.SituacaoPedido.AtivoConferencia, (int)Pedido.SituacaoPedido.Conferido, idObra);

            if (!string.IsNullOrEmpty(idsPedidosIgnorar))
                sql += " AND IdPedido NOT IN (" + idsPedidosIgnorar + ")";

            return sql;
        }
 
        internal string SqlPedidosConferidos(string idObra, string idsPedidosIgnorar, TipoRetorno tipoRetorno)
        {
            var campos = tipoRetorno == TipoRetorno.IdPedido ? "GROUP_CONCAT(IdPedido SEPARATOR ', ')" :
                tipoRetorno == TipoRetorno.TotalPedido ? "SUM(COALESCE(Total, 0))" : "*";

            var sql = string.Format("SELECT {0} FROM pedido WHERE Situacao IN ({1}) AND IdObra={2}",
                campos, (int)Pedido.SituacaoPedido.Conferido, idObra);

            if (!string.IsNullOrEmpty(idsPedidosIgnorar))
                sql += " AND IdPedido NOT IN (" + idsPedidosIgnorar + ")";

            return sql;
        }

        private string  Sql(uint idObra, uint? idCliente, string nomeCliente, uint idFunc, uint idFuncCad, uint idFormaPagto, int situacao, DateTime? dtIni, DateTime? dtFim,
            DateTime? dataFinIni, DateTime? dataFinFim, bool? gerarCredito, string idsPedidosIgnorar, uint idPedido, string descricao, bool selecionar)
        {
            /* Chamado 51738 este comando é necessário para que as informações dos pedidos associados à obra sejam buscadas. */
            var sql = selecionar ? "SET SESSION group_concat_max_len = 1000000;" : string.Empty;
            
            var campos = selecionar ? string.Format(@"o.*, c.Nome AS NomeCliente, c.Credito AS CreditoCliente, f.Nome as NomeFunc, fCad.Nome AS NomeFuncCad,
                ({0}) AS IdsPedidos,
                ({1}) AS TotalPedidos,
                ({2}) AS IdsPedidosAbertos,
                ({3}) AS TotalPedidosAbertos,
                ({4}) AS IdsPedidosConferidos,
                ({5}) AS TotalPedidosConferidos, '$$$' AS Criterio",
                SqlPedidos("o.IdObra", TipoRetorno.IdPedido),
                SqlPedidos("o.IdObra", TipoRetorno.TotalPedido),
                SqlPedidosAbertos("o.IdObra", idsPedidosIgnorar, TipoRetorno.IdPedido),
                SqlPedidosAbertos("o.IdObra", idsPedidosIgnorar, TipoRetorno.TotalPedido),
                SqlPedidosConferidos("o.IdObra", idsPedidosIgnorar, TipoRetorno.IdPedido),
                SqlPedidosConferidos("o.IdObra", idsPedidosIgnorar, TipoRetorno.TotalPedido)) : "COUNT(*)";

            sql = "select " + campos + @" from obra o 
                left join cliente c on (o.IdCliente=c.id_Cli) 
                left join funcionario f on (o.IdFunc=f.IdFunc)
                LEFT JOIN funcionario fCad ON (o.Usucad=fCad.IdFunc) where 1";

            string criterio = string.Empty;

            if (idObra > 0)
            {
                sql += " And o.idObra=" + idObra;
                criterio += "Num: " + idObra + "     ";
            }

            if (idCliente > 0)
            {
                sql += " and (o.idCliente=" + idCliente.Value + " or o.idCliente in (select idCliente from cliente_vinculo where idClienteVinculo=" + idCliente.Value + "))";
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome((uint)idCliente) + "     ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And (o.idCliente in (" + ids + ") or o.idCliente in (select idCliente from cliente_vinculo where idClienteVinculo in (" + ids + ")))";
                criterio += "Cliente: " + nomeCliente + "     ";
            }

            if (idFunc > 0)
            {
                sql += " AND o.IdFunc=" + idFunc;
                criterio += "Funcionário Obra: " + FuncionarioDAO.Instance.GetNome(idFunc) + "     ";
            }
 
            if (idFuncCad > 0)
            {
                sql += " AND o.Usucad=" + idFuncCad;
                criterio += "Funcionário Cad.: " + FuncionarioDAO.Instance.GetNome(idFuncCad) + "     ";
            }

            if (idFormaPagto > 0)
            {
                sql += " and o.idObra In (Select idObra From pagto_obra Where idFormaPagto=" + idFormaPagto + ")";
                criterio += "Forma Pagto.: " + FormaPagtoDAO.Instance.GetDescricao(idFormaPagto) + "     ";
            }

            if (situacao > 0)
            {
                Obra o = new Obra {Situacao = situacao};

                sql += " and o.Situacao=" + situacao;
                criterio += "Situação: " + o.DescrSituacao + "     ";
            }

            if (dtIni != null)
            {
                sql += " and o.DataCad>='" + dtIni.Value.ToString("yyyy-MM-dd 00:00") + "'";
                criterio += "A partir de " + dtIni.Value.ToString("yyyy-MM-dd") + "     ";
            }

            if (dtFim != null)
            {
                sql += " and o.DataCad<='" + dtFim.Value.ToString("yyyy-MM-dd 23:59") + "'";
                criterio += "Até " + dtFim.Value.ToString("yyyy-MM-dd") + "     ";
            }

            if (dataFinIni != null)
            {
                sql += " AND o.DataFin>='" + dataFinIni.Value.ToString("yyyy-MM-dd 00:00") + "'";
                criterio += "A partir de " + dataFinIni.Value.ToString("yyyy-MM-dd") + "     ";
            }

            if (dataFinFim != null)
            {
                sql += " AND o.DataFin<='" + dataFinFim.Value.ToString("yyyy-MM-dd 23:59") + "'";
                criterio += "Até " + dataFinFim.Value.ToString("yyyy-MM-dd") + "     ";
            }

            if (gerarCredito != null)
                sql += " and gerarCredito=" + gerarCredito.ToString().ToLower();

            if (idPedido > 0)
            {
                sql += " AND EXISTS (SELECT * FROM pedido WHERE idObra=o.idObra AND idPedido=" + idPedido + ")";
                criterio += " Pedido: " + idPedido + "   ";
            }

            if(!string.IsNullOrEmpty(descricao))
            {
                sql += " AND descricao LIKE ?descricao";
                criterio += "Descrição: " + descricao + "   ";
            }

            return sql.Replace("$$$",criterio);
        }

        public Obra GetElement(uint idObra)
        {
            return GetElement(null, idObra);
        }
 
        public Obra GetElement(GDASession session, uint idObra)
        {
            return objPersistence.LoadOneData(session, Sql(idObra, null, null, 0, 0, 0, 0, null, null, null, null, null, null, 0, null, true));
        }

        public Obra[] GetListRpt(uint? idCliente, string nomeCliente, uint idFunc, uint idFuncCad, uint idFormaPagto, int situacao, string dtIni,
            string dtFim, string dataFinIni, string dataFinFim, bool gerarCredito, string idsPedidosIgnorar, uint idObra, uint idPedido, string descricao)
        {
            return objPersistence.LoadData(Sql(idObra, idCliente, nomeCliente, idFunc, idFuncCad, idFormaPagto, situacao, dtIni.StrParaDate(), dtFim.StrParaDate(),
                dataFinIni.StrParaDate(), dataFinFim.StrParaDate(), gerarCredito, idsPedidosIgnorar, idPedido, descricao, true), Params(descricao)).ToArray();
        }

        public IList<Obra> GetList(uint? idCliente, string nomeCliente, uint idFunc, uint idFuncCad, uint idFormaPagto, int situacao, DateTime? dtIni, DateTime? dtFim,
            DateTime? dataFinIni, DateTime? dataFinFim, bool? gerarCredito, string idsPedidosIgnorar, uint idObra, uint idPedido, string descricao,
            string sortExpression, int startRow, int pageSize)
        {
            string order = String.IsNullOrEmpty(sortExpression) ? "o.idObra desc" : sortExpression;
            return LoadDataWithSortExpression(Sql(idObra, idCliente, nomeCliente, idFunc, idFuncCad, idFormaPagto, situacao, dtIni, dtFim, dataFinIni, dataFinFim,
                gerarCredito, idsPedidosIgnorar, idPedido, descricao, true), order, startRow, pageSize, Params(descricao));
        }

        public int GetListCount(uint? idCliente, string nomeCliente, uint idFunc, uint idFuncCad, uint idFormaPagto, int situacao, DateTime? dtIni, DateTime? dtFim,
            DateTime? dataFinIni, DateTime? dataFinFim, bool? gerarCredito, string idsPedidosIgnorar, uint idObra, uint idPedido, string descricao)
        {
            return objPersistence.ExecuteScalar(Sql(idObra, idCliente, nomeCliente, idFunc, idFuncCad, idFormaPagto, situacao, dtIni, dtFim,
                dataFinIni, dataFinFim, gerarCredito, idsPedidosIgnorar, idPedido, descricao, false), Params(descricao)).ToString().StrParaInt();
        }

        private GDAParameter[] Params(string descricao)
        {
            var lstParams = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(descricao))
                lstParams.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            return lstParams.Count > 0 ? lstParams.ToArray() : null;
        }

        #endregion

        #region Atualização do saldo

        public decimal AtualizaSaldoComTransacao(uint idObra, bool cxDiario, bool finalizarObraSeSaldoZero)
        {
            using(var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var saldo = AtualizaSaldo(transaction, null, idObra, cxDiario, finalizarObraSeSaldoZero);

                    transaction.Commit();
                    transaction.Close();
                    return saldo;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    return 0;
                }
            }
        }

        /// <summary>
        /// Atualiza o saldo de uma obra.
        /// </summary>
        public decimal AtualizaSaldo(GDASession sessao, uint idObra, bool cxDiario)
        {
            return AtualizaSaldo(sessao, null, idObra, cxDiario, true);
        }
        
        /// <summary>
        /// Atualiza o saldo de uma obra.
        /// </summary>
        public decimal AtualizaSaldo(GDASession sessao, Obra obraAtual, uint idObra, bool cxDiario, bool finalizarObraSeSaldoZero)
        {
            /* Chamado 51738.
             * Recupera ou recebe a obra por parâmetro para que o log de alteração dela seja criado corretamente. */
            obraAtual = obraAtual ?? GetElement(sessao, idObra);

            var valorObra = GetValorObra(sessao, idObra);

            var valorGasto = ExecuteScalar<decimal>(sessao, !PedidoConfig.LiberarPedido ?
                "select sum(coalesce(total, 0)) from pedido where idObra=" + idObra + " and situacao=" + (int)Pedido.SituacaoPedido.Confirmado :
                "select sum(p.valorPagamentoAntecipado) from pedido p left join pedido_espelho pe on (pe.idPedido=p.idPedido) where p.idObra=" +
                idObra + " and p.situacao in (" + (int)Pedido.SituacaoPedido.LiberadoParcialmente + "," + (int)Pedido.SituacaoPedido.Confirmado + "," +
                (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ")");

            objPersistence.ExecuteCommand(sessao, "update obra set Saldo=?saldo where idObra=" + idObra,  
                new GDAParameter("?saldo", Math.Max(valorObra - valorGasto, 0)));

            var saldo = ExecuteScalar<decimal>(sessao, "select coalesce(saldo, 0) from obra where idObra=" + idObra);
             
            if (saldo < 0)
                throw new Exception(string.Format("A obra {0} não possui saldo suficiente. Saldo da obra: {1}", idObra, saldo.ToString("C")));

            if (finalizarObraSeSaldoZero && saldo == 0 &&
                ObtemValorCampo<int>(sessao, "situacao", "idObra=" + idObra) != (int)Obra.SituacaoObra.Finalizada)
            {
                decimal temp;
                Finalizar(sessao, idObra, cxDiario, out temp);
            }
             
            /* Chamado 51738. */
            LogAlteracaoDAO.Instance.LogObra(sessao, obraAtual);

            return saldo;
        }

        #endregion

        #region Finalizar obra

        /// <summary>
        /// Finaliza uma obra (funcionário não financeiro).
        /// </summary>
        public void FinalizaFuncionario(uint idObra)
        {
            objPersistence.ExecuteCommand("update obra set situacao=" + (int)Obra.SituacaoObra.AguardandoFinanceiro + @"
                where idObra=" + idObra);
        }

        /// <summary>
        /// Finaliza uma obra.
        /// </summary>
        public void FinalizarComTransacao(uint idObra, bool cxDiario, out decimal creditoGerado)
        {
            lock (_receberObraLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        Finalizar(transaction, idObra, cxDiario, out creditoGerado);

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
        /// Finaliza uma obra.
        /// </summary>
        public void Finalizar(GDASession sessao, uint idObra, bool cxDiario, out decimal creditoGerado)
        {
            try
            {
                creditoGerado = 0;

                // Se esta obra já tiver sido finalizada, lança exceção.
                if (ExecuteScalar<bool>(sessao, "Select Count(*)>0 From obra Where IdObra=?idObra and dataFin>=?dataCad",
                    new GDAParameter("?idObra", idObra), new GDAParameter("?dataCad", DateTime.Now.AddSeconds(-10))))
                    throw new Exception("Obra já finalizada.");

                int situacao = ObtemValorCampo<int>(sessao, "situacao", "idObra=" + idObra);
                var obraCreditoCliente = ObtemValorCampo<bool>(sessao, "gerarCredito", "idObra=" + idObra);
                var dataCadObra = ObtemValorCampo<DateTime>(sessao, "dataCad", "idObra=" + idObra);

                /* Chamado 16170.
                 * Foi solicitado que a obra não pudesse ser recebida em um dia diferente de seu dia de cadastro. */
                if (obraCreditoCliente && !FinanceiroConfig.FinanceiroRec.PermitirRecebimentoObraClienteDataAnteriorDataAtual && DateTime.Now.Date > dataCadObra.Date)
                    throw new Exception("Somente obras cadastradas hoje podem ser finalizadas. Cancele esta obra e gere-a novamente.");

                if (!obraCreditoCliente && objPersistence.ExecuteSqlQueryCount(sessao, "SELECT COUNT(*) FROM pedido WHERE IdObra=" + idObra +
                    " AND situacao NOT IN (" + (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.Cancelado + ")") > 0)
                    throw new Exception(String.Format("Não é possível finalizar esta obra, existem pedidos não {0} associados à mesma.",
                        PedidoConfig.LiberarPedido ? "liberados" : "confirmados"));

                if (situacao != (int)Obra.SituacaoObra.Aberta && situacao != (int)Obra.SituacaoObra.Confirmada)
                    throw new Exception("Somente obras que estão abertas/confirmadas podem ser finalizadas.");

                decimal saldo = GetSaldo(sessao, idObra);

                // Deve gerar crédito sempre que o saldo for maior que 0, porque independente de estar gerando saldo de obra ou crédito direto
                // ao finalizar deverá gerar crédito para o cliente.
                if (saldo > 0)
                {
                    uint idCliente = ObtemValorCampo<uint>(sessao, "idCliente", "idObra=" + idObra);
                    creditoGerado = saldo;

                    ClienteDAO.Instance.CreditaCredito(sessao, idCliente, creditoGerado);

                    if (cxDiario)
                        CaixaDiarioDAO.Instance.MovCxObra(sessao, UserInfo.GetUserInfo.IdLoja, idCliente, idObra, 1, creditoGerado, 0,
                            UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado), null, null, false);
                    else
                        CaixaGeralDAO.Instance.MovCxObra(sessao, idObra, idCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado), 1,
                            creditoGerado, 0, null, false, null, null);
                }

                objPersistence.ExecuteCommand(sessao, "update obra set idFuncFin=?func, dataFin=?data, situacao=?sit where idObra=" + idObra,
                    new GDAParameter("?func", UserInfo.GetUserInfo.CodUser), new GDAParameter("?data", DateTime.Now),
                    new GDAParameter("?sit", (int)Obra.SituacaoObra.Finalizada));
            }
            catch(Exception ex)
            {
                ErroDAO.Instance.InserirFromException("GeraCrédito", ex);
                throw;
            }
        }

        #endregion

        #region Pagamento à vista

        public string PagamentoVista(Obra obra, bool cxDiario, decimal juros, bool recebimentoGerarCredito)
        {
            lock (_receberObraLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        /* Chamado 23453.
                         * A solução para este chamado é aplicar a fila de recebimento em todas as funções de recebimento,
                         * mas esta solução está em análise eu outras duas versões, enquanto não são validadas esta é uma forma
                         * paleativa de solucionar esta situação. */
                        if (objPersistence.ExecuteSqlQueryCount(transaction, 
                            string.Format("SELECT COUNT(*) FROM contas_receber WHERE IdObra={0}", obra.IdObra)) > 0)
                            throw new Exception("A conta desta obra já foi gerada.");

                        uint[] formasPagto = obra.FormasPagto;
                        uint[] tiposCartao = obra.TiposCartaoPagto;
                        uint[] contasBanco = obra.ContasBancoPagto;
                        uint[] depositoNaoIdentificado = obra.DepositoNaoIdentificado;
                        var cartaoNaoIdentificado = obra.CartaoNaoIdentificado;
                        var situacaoAtualObra = ObtemValorCampo<int>(transaction, "Situacao", string.Format("IdObra={0}", obra.IdObra));
                        var situacoesObraBloqueio = new List<Obra.SituacaoObra> { Obra.SituacaoObra.Cancelada, Obra.SituacaoObra.Finalizada };

                        /* Chamado 66151. */
                        if (situacoesObraBloqueio.Contains((Obra.SituacaoObra)situacaoAtualObra))
                            throw new Exception(string.Format("Esta obra já foi {0}.", ((Obra.SituacaoObra)situacaoAtualObra).ToString()));

                        if (UtilsFinanceiro.ContemFormaPagto(Pagto.FormaPagto.ChequeProprio, formasPagto) &&
                            String.IsNullOrEmpty(obra.ChequesPagto))
                            throw new Exception("Cadastre o(s) cheque(s) referente(s) ao pagamento da conta.");

                        UtilsFinanceiro.DadosRecebimento retorno = null;
                        uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

                        // Se não for caixa diário ou financeiro, não pode cadastrar obra
                        if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                            !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                            throw new Exception("Você não tem permissão para cadastrar obras.");

                        bool gerarCredito = obra.GerarCredito;

                        decimal totalPago = 0;

                        // Deve sempre verificar se o valor da obra bate com o valor pago, independente de estar no controle de gerar crédido ou não
                        if (obra.ValorObra > 0)
                        {
                            totalPago = obra.CreditoUtilizado;
                            foreach (decimal v in obra.ValoresPagto)
                                totalPago += v;

                            if (Math.Round(obra.ValorObra, 2) != Math.Round(totalPago, 2) && !recebimentoGerarCredito)
                                throw new Exception("O valor pago não confere com o valor a pagar. Valor pago: " +
                                                    totalPago.ToString("C") +
                                                    " Valor a pagar: " +
                                                    (!gerarCredito
                                                        ? obra.TotalProdutos.ToString("C")
                                                        : obra.ValorObra.ToString("C")));
                        }

                        string data = (obra.DataRecebimento != null ? obra.DataRecebimento.Value : DateTime.Now).ToString("dd/MM/yyyy");

                        obra.ValorCreditoAoCriar = ClienteDAO.Instance.GetCredito(transaction, obra.IdCliente);

                        // Atualiza o tipo de pagamento da obra antes de recebê-la, para que o tipo pagto não fique = 0
                        objPersistence.ExecuteCommand(transaction,
                            "Update obra set tipoPagto=" + (int) Obra.TipoPagtoObra.AVista + " Where idObra=" +
                            obra.IdObra);

                        //Caso o idloja da obra for 0 pega a loja do funcionario.
                        if(obra.IdLoja == 0 && UserInfo.GetUserInfo != null && UserInfo.GetUserInfo.IdLoja > 0)
                            obra.IdLoja = UserInfo.GetUserInfo.IdLoja;

                        retorno = UtilsFinanceiro.Receber(transaction, obra.IdLoja, null, null, null, null,
                            null, null, null, null, null, obra, null, obra.IdCliente, 0, null,
                            data, recebimentoGerarCredito ? obra.ValorObra : 0, recebimentoGerarCredito ? totalPago : 0, obra.ValoresPagto, formasPagto, contasBanco, depositoNaoIdentificado, cartaoNaoIdentificado,
                            tiposCartao, null, null, juros, false, recebimentoGerarCredito,
                            obra.CreditoUtilizado, obra.NumAutConstrucard, cxDiario, obra.ParcelasCartaoPagto,
                            obra.ChequesPagto, false,
                            UtilsFinanceiro.TipoReceb.Obra);

                        if (retorno.ex != null)
                            throw retorno.ex;

                        #region Insere as informações sobre pagamentos

                        PagtoObraDAO.Instance.DeleteByObra(transaction, obra.IdObra);

                        int numPagto = 1;
                        for (int i = 0; i < obra.ValoresPagto.Length; i++)
                        {
                            if (obra.ValoresPagto[i] == 0)
                                continue;

                            if (formasPagto[i] == (int)Data.Model.Pagto.FormaPagto.CartaoNaoIdentificado)
                            {
                                var CNIs = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(cartaoNaoIdentificado);

                                foreach (var cni in CNIs)
                                {
                                    var po = new PagtoObra
                                    {
                                        IdObra = obra.IdObra,
                                        NumFormaPagto = numPagto++,
                                        ValorPagto = cni.Valor,
                                        IdFormaPagto = formasPagto[i],
                                        IdContaBanco = (uint)cni.IdContaBanco,
                                        IdTipoCartao = (uint)cni.TipoCartao,
                                        NumAutCartao = cni.NumAutCartao
                                    };

                                    PagtoObraDAO.Instance.Insert(transaction, po);
                                }
                            }
                            else
                            {
                                var po = new PagtoObra
                                {
                                    IdObra = obra.IdObra,
                                    NumFormaPagto = numPagto++,
                                    ValorPagto = obra.ValoresPagto[i],
                                    IdFormaPagto = formasPagto[i],
                                    IdContaBanco = contasBanco[i] > 0 ? (uint?)contasBanco[i] : null,
                                    IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null,
                                    NumAutCartao = obra.NumAutCartao[i]
                                };

                                PagtoObraDAO.Instance.Insert(transaction, po);
                            }
                        }

                        // Se for pago com crédito, gera a conta recebida do credito
                        if (obra.CreditoUtilizado > 0)
                        {
                            PagtoObraDAO.Instance.Insert(transaction, new PagtoObra()
                            {
                                IdObra = obra.IdObra,
                                NumFormaPagto = numPagto,
                                ValorPagto = obra.CreditoUtilizado,
                                IdFormaPagto = (uint) Pagto.FormaPagto.Credito
                            });

                            var idContaR = ContasReceberDAO.Instance.Insert(transaction, new ContasReceber()
                            {
                                IdLoja = obra.IdLoja,
                                IdObra = obra.IdObra,
                                IdFormaPagto = null,
                                IdConta =
                                    UtilsPlanoConta.GetPlanoVista((uint) Pagto.FormaPagto.Credito),
                                Recebida = true,
                                ValorVec = obra.CreditoUtilizado,
                                ValorRec = obra.CreditoUtilizado,
                                DataVec = DateTime.Now,
                                DataRec = DateTime.Now,
                                DataCad = DateTime.Now,
                                IdCliente = obra.IdCliente,
                                UsuRec = UserInfo.GetUserInfo.CodUser,
                                Renegociada = false,
                                NumParc = 1,
                                NumParcMax = 1,
                                IdFuncComissaoRec = obra.IdCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(obra.IdCliente) : null
                        });
                            
                            #region Salva o pagamento da conta

                            var pagto = new PagtoContasReceber
                            {
                                IdContaR = idContaR,
                                IdFormaPagto = (uint) Pagto.FormaPagto.Credito,
                                ValorPagto = obra.CreditoUtilizado
                            };

                            PagtoContasReceberDAO.Instance.Insert(transaction, pagto);

                            #endregion
                        }

                        var numeroParcelaContaPagar = 0;

                        for (int i = 0; i < formasPagto.Length; i++)
                        {
                            if (formasPagto[i] == 0 || obra.ValoresPagto[i] == 0)
                                continue;

                            var idContaR = ContasReceberDAO.Instance.Insert(transaction, new ContasReceber()
                            {
                                IdLoja = obra.IdLoja,
                                IdObra = obra.IdObra,
                                IdFormaPagto = formasPagto[i],
                                IdConta = UtilsPlanoConta.GetPlanoVista(formasPagto[i]),
                                Recebida = true,
                                ValorVec = obra.ValoresPagto[i],
                                ValorRec = obra.ValoresPagto[i],
                                DataVec = DateTime.Now,
                                DataRec = DateTime.Now,
                                DataCad = DateTime.Now,
                                IdCliente = obra.IdCliente,
                                UsuRec = UserInfo.GetUserInfo.CodUser,
                                Renegociada = false,
                                NumParc = 1,
                                NumParcMax = 1,
                                IdFuncComissaoRec = obra.IdCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(obra.IdCliente) : null
                    });

                            if (formasPagto[i] == (uint)Pagto.FormaPagto.Cartao)
                                numeroParcelaContaPagar = ContasReceberDAO.Instance.AtualizarReferenciaContasCartao(transaction, retorno, obra.ParcelasCartaoPagto, numeroParcelaContaPagar, i, idContaR);

                            #region Salva o pagamento da conta

                            if (formasPagto.Length > i && formasPagto[i] == (int)Pagto.FormaPagto.CartaoNaoIdentificado)
                            {
                                var CNIs = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(cartaoNaoIdentificado);

                                foreach (var cni in CNIs)
                                {
                                    var pagto = new PagtoContasReceber
                                    {
                                        IdContaR = idContaR,
                                        IdFormaPagto = formasPagto[i],
                                        ValorPagto = cni.Valor,
                                        IdContaBanco = (uint)cni.IdContaBanco,
                                        IdTipoCartao = (uint)cni.TipoCartao,
                                        NumAutCartao = cni.NumAutCartao
                                    };

                                    PagtoContasReceberDAO.Instance.Insert(transaction, pagto);
                                }
                            }
                            else
                            {
                                var pagto = new PagtoContasReceber
                                {
                                    IdContaR = idContaR,
                                    IdFormaPagto = formasPagto[i],
                                    ValorPagto = obra.ValoresPagto[i],
                                    IdContaBanco = formasPagto[i] != (uint)Pagto.FormaPagto.Dinheiro &&
                                               contasBanco[i] > 0
                                    ? (uint?)contasBanco[i]
                                    : null,
                                    IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null,
                                    IdDepositoNaoIdentificado = depositoNaoIdentificado[i] > 0
                                    ? (uint?)depositoNaoIdentificado[i]
                                    : null,
                                    NumAutCartao = obra.NumAutCartao[i]
                                };

                                PagtoContasReceberDAO.Instance.Insert(transaction, pagto);
                            }

                            #endregion
                        }

                        #endregion

                        obra.CreditoGeradoCriar = retorno.creditoGerado;
                        obra.CreditoUtilizadoCriar = obra.CreditoUtilizado;

                        // Atualiza o IdObra nos cheques
                        foreach (Cheques c in retorno.lstChequesInseridos)
                            objPersistence.ExecuteCommand(transaction,
                                "update cheques set idObra=" + obra.IdObra + ", idCliente=" + obra.IdCliente +
                                " where idCheque=" + c.IdCheque);

                        // Atualiza a situação da Obra
                        obra.Situacao = (int) Obra.SituacaoObra.Confirmada;
                        obra.TipoPagto = (int) Obra.TipoPagtoObra.AVista;

                        base.Update(transaction, obra);

                        // O saldo precisa ser atualizado antes de finalizar a obra para que gere crédito
                        AtualizaSaldo(transaction, obra.IdObra, cxDiario);

                        // Finaliza a obra, caso seja geração de crédito
                        if (obra.GerarCredito)
                        {
                            decimal temp;
                            Finalizar(transaction, obra.IdObra, cxDiario, out temp);
                        }

                        if(recebimentoGerarCredito && retorno.creditoGerado > 0)
                        {
                            if (cxDiario)
                                CaixaDiarioDAO.Instance.MovCxObra(transaction, obra.IdLoja, obra.IdCliente, obra.IdObra, 1, retorno.creditoGerado, 0,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado), null, null, false);
                            else
                                CaixaGeralDAO.Instance.MovCxObra(transaction, obra.IdObra, obra.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado), 1,
                                    retorno.creditoGerado, 0, null, false, null, null);
                        }

                        #region Calcula o saldo devedor

                        decimal saldoDevedor;
                        decimal saldoCredito;

                        ClienteDAO.Instance.ObterSaldoDevedor(transaction, obra.IdCliente, out saldoDevedor, out saldoCredito);

                        var sqlUpdate = @"UPDATE obra SET SaldoDevedor = ?saldoDevedor, SaldoCredito = ?saldoCredito WHERE IdObra = {0}";
                        objPersistence.ExecuteCommand(transaction, string.Format(sqlUpdate, obra.IdObra), new GDAParameter("?saldoDevedor", saldoDevedor), new GDAParameter("?saldoCredito", saldoCredito));

                        #endregion

                        transaction.Commit();
                        transaction.Close();

                        return gerarCredito ? "Crédito cadastrado." : "Pagamento antecipado recebido.";
                    }
                    catch (Exceptions.LogoutException ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("PagamentoVistaObra - IdObra: {0}", obra.IdObra), ex);

                        throw new Exceptions.LogoutException(MensagemAlerta.FormatErrorMsg("Efetue o login no sistema novamente.", ex));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("PagamentoVistaObra - IdObra: {0}", obra.IdObra), ex);
                        throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cadastrar obra.", ex));
                    }
                }
            }
        }

        #endregion

        #region Pagamento à prazo

        public string PagamentoPrazo(Obra obra, bool cxDiario)
        {
            lock (_receberObraLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var situacaoAtualObra = ObtemValorCampo<int>(transaction, "Situacao", string.Format("IdObra={0}", obra.IdObra));
                        var situacoesObraBloqueio = new List<Obra.SituacaoObra> { Obra.SituacaoObra.Cancelada, Obra.SituacaoObra.Finalizada, Obra.SituacaoObra.Confirmada };

                        /* Chamado 66151. */
                        if (situacoesObraBloqueio.Contains((Obra.SituacaoObra)situacaoAtualObra))
                            throw new Exception(string.Format("Esta obra já foi {0}.", ((Obra.SituacaoObra)situacaoAtualObra).ToString()));

                        if (obra.FormasPagto != null && obra.FormasPagto[0] == 0)
                            throw new Exception("Informe a forma de pagamento da obra!");

                        #region Insere as informações sobre pagamentos

                        uint[] formasPagto = obra.FormasPagto;

                        PagtoObraDAO.Instance.DeleteByObra(transaction, obra.IdObra);

                        // Insere a forma de pagamento da Obra de acordo com os dados informados
                        var po = new PagtoObra
                        {
                            IdObra = obra.IdObra,
                            NumFormaPagto = 1,
                            ValorPagto = obra.ValorObra,
                            IdFormaPagto = formasPagto[0]
                        };

                        PagtoObraDAO.Instance.Insert(transaction, po);
                        
                        for (int i = 0; i < obra.NumParcelas; i++)
                        {
                            ContasReceber c = new ContasReceber
                            {
                                IdLoja = obra.IdLoja,
                                IdObra = obra.IdObra,
                                ValorVec = obra.ValoresParcelas[i],
                                DataVec = obra.DatasParcelas[i],
                                // Utiliza apenas a primeira forma de pagamento, porque obra recebida a prazo tem apenas uma forma de pagamento.
                                IdConta = UtilsPlanoConta.GetPlanoPrazo(formasPagto[0]),
                                IdCliente = obra.IdCliente,
                                NumParc = (i + 1),
                                NumParcMax = obra.NumParcelas,
                                IdFuncComissaoRec = obra.IdCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(obra.IdCliente) : null
                            };

                            ContasReceberDAO.Instance.Insert(transaction, c);
                        }

                        #endregion

                        // Atualiza a situação da Obra
                        obra.Situacao = (int)Obra.SituacaoObra.Confirmada;
                        obra.TipoPagto = (int)Obra.TipoPagtoObra.APrazo;
                        base.Update(transaction, obra);

                        // O saldo precisa ser atualizado antes de finalizar a obra para que gere crédito
                        AtualizaSaldo(transaction, obra.IdObra, cxDiario);

                        // Finaliza a obra, caso seja geração de crédito
                        if (obra.GerarCredito)
                        {
                            decimal temp;
                            Finalizar(transaction, obra.IdObra, cxDiario, out temp);
                        }

                        #region Calcula o saldo devedor

                        decimal saldoDevedor;
                        decimal saldoCredito;

                        ClienteDAO.Instance.ObterSaldoDevedor(transaction, obra.IdCliente, out saldoDevedor, out saldoCredito);

                        var sqlUpdate = @"UPDATE obra SET SaldoDevedor = ?saldoDevedor, SaldoCredito = ?saldoCredito WHERE IdObra = {0}";
                        objPersistence.ExecuteCommand(transaction, string.Format(sqlUpdate, obra.IdObra), new GDAParameter("?saldoDevedor", saldoDevedor), new GDAParameter("?saldoCredito", saldoCredito));

                        #endregion

                        transaction.Commit();
                        transaction.Close();

                        return "Parcelas geradas com sucesso.";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        return MensagemAlerta.FormatErrorMsg("Erro ao gerar parcelas da obra.", ex);
                    }
                }
            }
        }

        #endregion

        #region Cancela Obra

        /// <summary>
        /// Efetua o cancelamento da obra
        /// </summary>
        public void CancelaObra(uint idObra, string motivo, DateTime dataEstornoBanco, bool cancelamentoErroTef, bool gerarCredito)
        {
            lock(_receberObraLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        // A obra deve ser recuperada depois da instância da fila, para garantir que seja recuperada a obra atualizada.
                        Obra obra = GetElementByPrimaryKey(transaction, idObra);

                        if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.CancelarRecebimentos))
                            throw new Exception("Você não tem permissão para cancelar recebimentos, contacte o administrador");

                        if (obra.Situacao == (int)Obra.SituacaoObra.Cancelada)
                            throw new Exception("Esta obra já foi cancelada.");

                        /* Chamado 64461. */
                        if (ExecuteScalar<bool>(transaction, string.Format("SELECT COUNT(*)>0 FROM cheques c WHERE c.IdObra={0} AND Situacao NOT IN ({1}, {2})", idObra,
                            (int)Cheques.SituacaoCheque.Cancelado, (int)Cheques.SituacaoCheque.EmAberto)))
                            throw new Exception(@"Um ou mais cheques recebidos já foram utilizados em outras transações, cancele ou retifique as transações dos cheques antes de cancelar esta obra.");

                        // Verifica se existe algum pedido não cancelado associado à esta obra
                        if (objPersistence.ExecuteScalar(transaction, "Select count(*) From pedido Where idObra=" + idObra +
                                                                 " And situacao<>" + (int)Pedido.SituacaoPedido.Cancelado).ToString().StrParaInt() > 0)
                            throw new Exception("Cancele todos os pedidos associados à esta obra antes de cancelar a mesma.");

                        // Verifica se o cliente possui crédito para ser estornado
                        if (obra.GerarCredito && obra.Situacao != (int)Obra.SituacaoObra.Aberta && ClienteDAO.Instance.GetCredito(transaction, obra.IdCliente) < obra.ValorObra)
                            throw new Exception("O valor do crédito gerado é maior que o crédito do cliente.");

                        // Verifica se alguma parcela desta obra já foi recebida
                        if (ContasReceberDAO.Instance.ExisteRecebidaObra(transaction, idObra) && obra.TipoPagto != (int)Obra.TipoPagtoObra.AVista)
                            throw new Exception("Existe uma conta recebida associada à esta obra, cancele-a antes de cancelar a obra.");

                        // Verifica se há alguma parcela de cartão quitada para esta obra, se houver, será necessário cancelar o recebimento da mesma
                        if (ContasReceberDAO.Instance.ExisteParcCartaoRecebidaObra(transaction, idObra))
                            throw new Exception("Existem uma ou mais parcelas de cartão quitadas associadas à esta obra, cancele o recebimento antes de cancelar esta obra.");

                        // Estorna o que foi recebido da obra somente se a mesma tiver de fato recebido algo, conferindo se a situação
                        // não é nem "Aberta" e nem "Aguardando Financeiro"
                        if (obra.Situacao != (int)Obra.SituacaoObra.Aberta && obra.Situacao != (int)Obra.SituacaoObra.AguardandoFinanceiro)
                            UtilsFinanceiro.CancelaRecebimento(transaction, UtilsFinanceiro.TipoReceb.Obra, null, null, null, null, null,
                                0, obra, null, null, null, dataEstornoBanco, cancelamentoErroTef, gerarCredito);

                        // Exclui contas a receber gerada pela obra que esteja em aberto;
                        ContasReceberDAO.Instance.DeleteByObra(transaction, idObra);

                        LogCancelamentoDAO.Instance.LogObra(transaction, obra, motivo, true);

                        obra.Situacao = (int)Obra.SituacaoObra.Cancelada;
                        base.Update(transaction, obra);
                        
                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("CancelarObra - ID: {0}", idObra), ex);
                        throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cancelar obra.", ex));
                    }
                }
            }
        }

        #endregion

        #region Recupera o nome do cliente

        /// <summary>
        /// Retorna o nome do cliente da obra.
        /// </summary>
        public string GetNomeCliente(uint idObra, bool incluirCodigoCliente)
        {
            object cli = objPersistence.ExecuteScalar("select coalesce(idCliente, 0) from obra where idObra=" + idObra);
            uint idCliente = cli != null && cli.ToString() != "" ? cli.ToString().StrParaUint() : 0;
            return (incluirCodigoCliente ? idCliente + " - " : "") + ClienteDAO.Instance.GetNome(idCliente);
        }

        #endregion

        #region Recupera o total dos produtos

        /// <summary>
        /// Recupera o total dos produtos de uma obra.
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public decimal TotalProdutos(uint idObra)
        {
            decimal total = 0;
            foreach (ProdutoObra p in ProdutoObraDAO.Instance.GetForRpt(idObra))
                total += p.TotalProduto;

            return total;
        }

        #endregion

        #region Recupera informações da obra

        public decimal GetValorObra(GDASession sessao, uint idObra)
        {
            return ObtemValorCampo<decimal>(sessao, "valorObra", "idObra=" + idObra);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public decimal GetSaldo(uint idObra)
        {
            return GetSaldo(null, idObra);
        }

        public decimal GetSaldo(GDASession sessao, uint idObra)
        {
            return ObtemValorCampo<decimal>(sessao, "saldo", "idObra=" + idObra);
        }

        public bool IsGerarCredito(uint idObra)
        {
            return ObtemValorCampo<bool>("gerarCredito", "idObra=" + idObra);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public int ObtemTipoPagto(uint idObra)
        {
            return ObtemTipoPagto(null, idObra);
        }

        public int ObtemTipoPagto(GDASession sessao, uint idObra)
        {
            return ObtemValorCampo<int>(sessao, "tipoPagto", "idObra=" + idObra);
        }

        /// <summary>
        /// Obtém o saldo da obra retirando os pedidos conferidos e confirmados associados à mesma
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public decimal ObtemSaldoComPedConf(uint idObra)
        {
            return ObtemSaldoComPedConf(null, idObra);
        }

        /// <summary>
        /// Obtém o saldo da obra retirando os pedidos conferidos e confirmados associados à mesma
        /// </summary>
        public decimal ObtemSaldoComPedConf(GDASession sessao, uint idObra)
        {
            decimal saldo = ObtemValorCampo<decimal>(sessao, "saldo", "idObra=" + idObra);
            
            // O saldo da obra buscado acima é o saldo menos o valor dos pedidos confirmados, que deve ser subtraído abaixo pelo valor dos pedidos conferidos.
            saldo -= ExecuteScalar<decimal>(sessao, "Select sum(total) From pedido Where idObra=" + idObra + " And situacao In (" +
                (int)Pedido.SituacaoPedido.Conferido + ")");

            return saldo;
        }

        /// <summary>
        /// Obtem a situação da obra
        /// </summary>
        public Obra.SituacaoObra ObtemSituacao(uint idObra)
        {
            return ObtemSituacao(null, idObra);
        }

        /// <summary>
        /// Obtem a situação da obra
        /// </summary>
        public Obra.SituacaoObra ObtemSituacao(GDASession session, uint idObra)
        {
            return ObtemValorCampo<Obra.SituacaoObra>(session, "Situacao", "IdObra = " + idObra);
        }

        public uint ObtemIdFunc(GDASession session, uint idObra)
        {
            return ObtemValorCampo<uint>("idFunc", "idObra=" + idObra);
        }

        public int ObtemIdCliente(GDASession session, int idObra)
        {
            return ObtemValorCampo<int>(session, "idCliente", string.Format("idObra={0}", idObra));
        }

        #endregion

        #region Reabre a obra

        /// <summary>
        /// Reabre a obra.
        /// </summary>
        /// <param name="idObra"></param>
        public void Reabrir(uint idObra)
        {
            // Só reabre as obras que estejam na situação AguardandoFinanceiro
            objPersistence.ExecuteCommand("update obra set situacao=" + (int)Obra.SituacaoObra.Aberta + @"
                where idObra=" + idObra + " and situacao=" + (int)Obra.SituacaoObra.AguardandoFinanceiro);
        }

        #endregion

        #region Atualiza o valor da obra

        /// <summary>
        /// Atualiza o valor da obra.
        /// </summary>
        /// <param name="idObra"></param>
        public void UpdateValorObra(uint idObra)
        {
            if (!PedidoConfig.DadosPedido.UsarControleNovoObra)
                return;

            objPersistence.ExecuteCommand("update obra set valorObra=?valor where idObra=" + idObra,
                new GDAParameter("?valor", TotalProdutos(idObra)));
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(Obra objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var idObraAnterior = ObtemValorCampo<int?>(transaction, "IdObra", string.Format("GerarCredito={0} ORDER BY IdObra DESC LIMIT 1", objInsert.GerarCredito));

                    if (objInsert.GerarCredito &&
                        ClienteDAO.Instance.GetNome(transaction, objInsert.IdCliente).ToLower().Contains("consumidor"))
                        throw new Exception("Não é possível gerar crédito para consumidor final.");

                    if (ClienteDAO.Instance.GetSituacao(transaction, objInsert.IdCliente) != (int)SituacaoCliente.Ativo)
                        throw new Exception("O cliente precisa estar ativo para efetuar este procedimento.");

                    objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
                    objInsert.DataCad = DateTime.Now;
                    objInsert.IdObra = base.Insert(transaction, objInsert);

                    if (idObraAnterior.GetValueOrDefault() > 0)
                    {
                        if (idObraAnterior != null)
                        {
                            var obraAnterior = GetElementByPrimaryKey(transaction, idObraAnterior.Value);

                            /* Chamado 23374. */
                            if (obraAnterior.Usucad == objInsert.Usucad &&
                                obraAnterior.IdCliente == objInsert.IdCliente &&
                                Math.Abs((objInsert.DataCad - obraAnterior.DataCad).TotalSeconds) < 60)
                                throw new Exception("Não é possível inserir esta obra, já foi inserida uma obra com estes mesmos dados em menos de um minuto.");
                        }
                    }


                    transaction.Commit();
                    transaction.Close();

                    return objInsert.IdObra;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();


                    ErroDAO.Instance.InserirFromException("Falha ao inserir obra.", ex);
                    throw;
                }
            }
        }

        public override int Update(Obra objUpdate)
        {
            lock (_receberObraLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var obraAtual = GetElement(transaction, objUpdate.IdObra);

                        /* Chamado 54388. */
                        if (obraAtual.Situacao != (int)Obra.SituacaoObra.Aberta &&
                            (obraAtual.IdCliente != objUpdate.IdCliente || obraAtual.IdFunc != objUpdate.IdFunc || obraAtual.ValorObra != objUpdate.ValorObra))
                            throw new Exception("A obra precisa estar aberta para que os dados sejam atualizados.");

                        var retorno = base.Update(transaction, objUpdate);

                        /* Chamado 54388. */
                        LogAlteracaoDAO.Instance.LogObra(transaction, obraAtual);

                        transaction.Commit();
                        transaction.Close();

                        return retorno;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException("Falha ao atualizar obra.", ex);
                        throw ex;
                    }
                }
            }
        }

        public override int Delete(Obra objDelete)
        {
            CancelaObra(objDelete.IdObra, null, DateTime.Now, false, false);
            return 1;
        }

        #endregion
    }
}