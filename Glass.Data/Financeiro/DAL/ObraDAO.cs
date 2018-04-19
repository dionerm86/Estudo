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
        public decimal AtualizaSaldo(GDASession sessao, uint idObra, bool cxDiario, bool finalizarObraSeSaldoZero)
        {
            return AtualizaSaldo(sessao, null, idObra, cxDiario, finalizarObraSeSaldoZero);
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

        /// <summary>
        /// Finaliza uma obra (funcionário não financeiro).
        /// </summary>
        public void FinalizaFuncionarioDescontinuado(uint idObra)
        {
            objPersistence.ExecuteCommand("update obra set situacao=" + (int)Obra.SituacaoObra.AguardandoFinanceiro + @"
                where idObra=" + idObra);
        }

        /// <summary>
        /// Finaliza uma obra.
        /// </summary>
        public void FinalizarComTransacaoDescontinuado(uint idObra, bool cxDiario, out decimal creditoGerado)
        {
            lock (_receberObraLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        FinalizarDescontinuado(transaction, idObra, cxDiario, out creditoGerado);

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
        public void FinalizarDescontinuado(GDASession sessao, uint idObra, bool cxDiario, out decimal creditoGerado)
        {
            try
            {
                #region Declaração de variáveis

                creditoGerado = 0;
                var obra = GetElement(sessao, idObra);
                var usuarioLogado = UserInfo.GetUserInfo;

                #endregion

                #region Validação da finalização da obra

                ValidarFinalizarObra(sessao, obra);

                #endregion

                #region Geração de crédito para o cliente

                // Deve gerar crédito sempre que o saldo for maior que 0, porque independente de estar gerando saldo de obra ou crédito direto ao finalizar deverá gerar crédito para o cliente.
                if (obra.Saldo > 0)
                {
                    creditoGerado = obra.Saldo;

                    ClienteDAO.Instance.CreditaCredito(sessao, obra.IdCliente, creditoGerado);

                    if (cxDiario)
                    {
                        CaixaDiarioDAO.Instance.MovCxObra(sessao, usuarioLogado.IdLoja, obra.IdCliente, idObra, 1, creditoGerado, 0,
                            UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado), null, null, false);
                    }
                    else
                    {
                        CaixaGeralDAO.Instance.MovCxObra(sessao, idObra, obra.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado), 1, creditoGerado, 0,
                            null, false, null, null);
                    }
                }

                #endregion

                #region Atualização dos dados de finalização da obra

                objPersistence.ExecuteCommand(sessao, string.Format("UPDATE OBRA SET IdFuncFin=?idFuncionarioFinalizacao, DataFin=?dataFinalizacao, Situacao=?situacao WHERE IdObra={0}", idObra),
                    new GDAParameter("?idFuncionarioFinalizacao", usuarioLogado.CodUser),
                    new GDAParameter("?dataFinalizacao", DateTime.Now),
                    new GDAParameter("?situacao", (int)Obra.SituacaoObra.Finalizada));

                #endregion
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("FinalizarObra", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Valida a finalização da obra.
        /// </summary>
        public void ValidarFinalizarObra(GDASession session, Obra obra)
        {
            #region Validações dos dados da obra

            // Se esta obra já tiver sido finalizada, lança exceção.
            if (obra.DataFin >= DateTime.Now.AddSeconds(-10))
            {
                throw new Exception("Obra já finalizada.");
            }

            /* Chamado 16170.
             * Foi solicitado que a obra não pudesse ser recebida em um dia diferente de seu dia de cadastro. */
            if (obra.GerarCredito && !FinanceiroConfig.FinanceiroRec.PermitirRecebimentoObraClienteDataAnteriorDataAtual && DateTime.Now.Date > obra.DataCad.Date)
            {
                throw new Exception("Somente obras cadastradas hoje podem ser finalizadas. Cancele esta obra e gere-a novamente.");
            }

            if (obra.Situacao != (int)Obra.SituacaoObra.Aberta && obra.Situacao != (int)Obra.SituacaoObra.Confirmada)
            {
                throw new Exception("Somente obras que estão abertas/confirmadas podem ser finalizadas.");
            }

            #endregion

            #region Validações dos pedidos da obra

            if (!obra.GerarCredito && objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) FROM pedido WHERE IdObra={0} AND Situacao NOT IN ({1},{2})",
                obra.IdObra, (int)Pedido.SituacaoPedido.Confirmado, (int)Pedido.SituacaoPedido.Cancelado)) > 0)
            {
                throw new Exception(string.Format("Não é possível finalizar esta obra, existem pedidos não {0} associados à mesma.", PedidoConfig.LiberarPedido ? "liberados" : "confirmados"));
            }

            #endregion
        }

        #endregion

        #region Pagamento à vista

        /// <summary>
        /// Efetua o pagamento à vista da obra.
        /// </summary>
        public string PagamentoVista(bool caixaDiario, decimal juros, Obra obra, bool recebimentoGerarCredito)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    CriarPrePagamentoVista(transaction, caixaDiario, juros, obra, recebimentoGerarCredito);

                    FinalizarPrePagamentoVista(transaction, (int)obra.IdObra);

                    transaction.Commit();
                    transaction.Close();

                    return obra.GerarCredito ? "Crédito cadastrado." : "Pagamento antecipado recebido.";
                }
                catch (Exceptions.LogoutException ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("PagamentoVista - IdObra: {0}", obra.IdObra), ex);
                    throw new Exceptions.LogoutException(MensagemAlerta.FormatErrorMsg("Efetue o login no sistema novamente.", ex));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("PagamentoVista - IdObra: {0}", obra.IdObra), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao efetuar o pagamento da obra.", ex));
                }
            }
        }

        /// <summary>
        /// Cria o pré pagamento à vista da obra.
        /// </summary>
        public string CriarPrePagamentoVistaComTransacao(bool caixaDiario, decimal juros, Obra obra, bool recebimentoGerarCredito)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    CriarPrePagamentoVista(transaction, caixaDiario, juros, obra, recebimentoGerarCredito);

                    TransacaoCapptaTefDAO.Instance.Insert(transaction, new TransacaoCapptaTef()
                    {
                        IdReferencia = (int)obra.IdObra,
                        TipoRecebimento = UtilsFinanceiro.TipoReceb.Obra
                    });

                    transaction.Commit();
                    transaction.Close();

                    return obra.GerarCredito ? "Crédito cadastrado." : "Pagamento antecipado recebido.";
                }
                catch (Exceptions.LogoutException ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("PagamentoVista - IdObra: {0}", obra.IdObra), ex);
                    throw new Exceptions.LogoutException(MensagemAlerta.FormatErrorMsg("Efetue o login no sistema novamente.", ex));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CriarPrePagamentoVistaComTransacao - ID obra: {0}.", obra.IdObra), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao efetuar o pagamento da obra.", ex));
                }
            }
        }

        /// <summary>
        /// Cria o pré pagamento à vista da obra.
        /// </summary>
        public void CriarPrePagamentoVista(GDASession session, bool caixaDiario, decimal juros, Obra obra, bool recebimentoGerarCredito)
        {
            #region Declaração de variáveis

            var usuarioLogado = UserInfo.GetUserInfo;
            var contadorPagamento = 1;
            var idLoja = (int)usuarioLogado.IdLoja;
            var totalPagar = obra.ValorObra;
            decimal totalPago = 0;

            #endregion

            #region Cálculo dos totais da obra

            // Deve sempre verificar se o valor da obra bate com o valor pago, independente de estar no controle de gerar crédido ou não.
            if (obra.ValorObra > 0)
            {
                // Calcula o valor total pago da obra.
                totalPago = obra.CreditoUtilizado + obra.ValoresPagto.Sum(f => f);
            }

            #endregion

            #region Atualização de dados da obra

            obra.TotalPagar = totalPagar;
            obra.TotalPago = totalPago;
            obra.IdLojaRecebimento = idLoja;
            obra.JurosRecebimento = juros;
            obra.RecebimentoCaixaDiario = caixaDiario;
            obra.RecebimentoGerarCredito = recebimentoGerarCredito;

            #endregion

            #region Validação do recebimento e finalização da obra

            ValidarPagamentoVista(session, obra);

            #endregion

            #region Inserção das informações sobre pagamentos

            // Cadastro dos cheques utilizados no pagamento da obra.
            ChequesObraDAO.Instance.InserirPelaString(session, obra, obra.ChequesPagto?.Split('|').ToList() ?? new List<string>());
            // Garante que não haverá chaves duplicadas para esta obra.
            PagtoObraDAO.Instance.DeleteByObra(session, obra.IdObra);

            for (var i = 0; i < obra.ValoresPagto.Length; i++)
            {
                if (obra.FormasPagto[i] == 0 || obra.ValoresPagto[i] == 0)
                {
                    continue;
                }

                if (obra.FormasPagto[i] == (int)Pagto.FormaPagto.CartaoNaoIdentificado)
                {
                    var cartoesNaoIdentificado = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(session, obra.CartaoNaoIdentificado);

                    foreach (var cartaoNaoIdentificado in cartoesNaoIdentificado)
                    {
                        var pagamentoObra = new PagtoObra
                        {
                            IdObra = obra.IdObra,
                            NumFormaPagto = contadorPagamento++,
                            ValorPagto = cartaoNaoIdentificado.Valor,
                            IdFormaPagto = obra.FormasPagto[i],
                            IdContaBanco = (uint)cartaoNaoIdentificado.IdContaBanco,
                            IdCartaoNaoIdentificado = cartaoNaoIdentificado.IdCartaoNaoIdentificado,
                            IdTipoCartao = (uint)cartaoNaoIdentificado.TipoCartao,
                            NumAutCartao = cartaoNaoIdentificado.NumAutCartao
                        };

                        PagtoObraDAO.Instance.Insert(session, pagamentoObra);
                    }
                }
                else
                {
                    var pagamentoObra = new PagtoObra
                    {
                        IdObra = obra.IdObra,
                        NumFormaPagto = contadorPagamento++,
                        ValorPagto = obra.ValoresPagto[i],
                        IdFormaPagto = obra.FormasPagto[i],
                        IdContaBanco = obra.ContasBancoPagto[i] > 0 ? (uint?)obra.ContasBancoPagto[i] : null,
                        IdDepositoNaoIdentificado = obra.DepositoNaoIdentificado.ElementAtOrDefault(i) > 0 ? (int?)obra.DepositoNaoIdentificado[i] : null,
                        IdTipoCartao = obra.TiposCartaoPagto.ElementAtOrDefault(i) > 0 ? (uint?)obra.TiposCartaoPagto[i] : null,
                        QuantidadeParcelaCartao = obra.ParcelasCartaoPagto.ElementAtOrDefault(i) > 0 ? (int?)obra.ParcelasCartaoPagto[i] : null,
                        NumAutCartao = obra.NumAutCartao[i]
                    };

                    PagtoObraDAO.Instance.Insert(session, pagamentoObra);
                }
            }

            // Se for pago com crédito, gera a conta recebida do credito
            if (obra.CreditoUtilizado > 0)
            {
                PagtoObraDAO.Instance.Insert(session, new PagtoObra()
                {
                    IdObra = obra.IdObra,
                    NumFormaPagto = contadorPagamento,
                    ValorPagto = obra.CreditoUtilizado,
                    IdFormaPagto = (uint)Pagto.FormaPagto.Credito
                });
            }

            #endregion

            #region Atualização dos dados da obra

            obra.CreditoUtilizadoCriar = obra.CreditoUtilizado;
            obra.ValorCreditoAoCriar = ClienteDAO.Instance.GetCredito(session, obra.IdCliente);
            // Atualiza a situação da Obra.
            obra.Situacao = (int)Obra.SituacaoObra.Processando;
            obra.TipoPagto = (int)Obra.TipoPagtoObra.AVista;
            // O valor total a pagar e valor total pago devem ser zerados, após a validação, caso o recebimento deva gerar crédito.
            obra.TotalPagar = recebimentoGerarCredito ? totalPagar : 0;
            obra.TotalPago = recebimentoGerarCredito ? totalPago : 0;

            base.Update(session, obra);

            // O saldo precisa ser atualizado antes de finalizar a obra para que gere crédito.
            AtualizaSaldo(session, obra, obra.IdObra, caixaDiario, false);

            #endregion
        }

        /// <summary>
        /// Valida o pagamento à vista da obra.
        /// </summary>
        private void ValidarPagamentoVista(GDASession session, Obra obra)
        {
            #region Declaração de variáveis

            var situacaoAtualObra = ObtemValorCampo<int>(session, "Situacao", string.Format("IdObra={0}", obra.IdObra));
            var situacoesObraBloqueio = new List<Obra.SituacaoObra> { Obra.SituacaoObra.Cancelada, Obra.SituacaoObra.Finalizada };
            var totalPagar = obra.TotalPagar.GetValueOrDefault();
            var totalPago = obra.TotalPago.GetValueOrDefault();
            var idLojaRecebimento = obra.IdLojaRecebimento.GetValueOrDefault();
            var jurosRecebimento = obra.JurosRecebimento.GetValueOrDefault();
            var recebimentoCaixaDiario = obra.RecebimentoCaixaDiario.GetValueOrDefault();
            var recebimentoGerarCredito = obra.RecebimentoGerarCredito.GetValueOrDefault();
            var valorGastoObra = ObterValorGasto(session, (int)obra.IdObra);
            var saldoObra = Math.Max(obra.ValorObra - valorGastoObra, 0);

            #endregion

            #region Validações de permissão

            // Se não for caixa diário ou financeiro, não pode cadastrar obra
            if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
            {
                throw new Exception("Você não tem permissão para cadastrar obras.");
            }

            #endregion

            #region Validações dos dados da obra

            /* Chamado 66151. */
            if (situacoesObraBloqueio.Contains((Obra.SituacaoObra)situacaoAtualObra))
            {
                throw new Exception(string.Format("Esta obra já foi {0}.", ((Obra.SituacaoObra)situacaoAtualObra).ToString()));
            }

            /* Chamado 23453.
             * A solução para este chamado é aplicar a fila de recebimento em todas as funções de recebimento,
             * mas esta solução está em análise eu outras duas versões, enquanto não são validadas esta é uma forma
             * paleativa de solucionar esta situação. */
            if (objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) FROM contas_receber WHERE IdObra={0};", obra.IdObra)) > 0)
            {
                throw new Exception("A conta desta obra já foi gerada.");
            }

            #endregion

            #region Validações dos dados do recebimento

            if (Math.Round(obra.ValorObra, 2) != Math.Round(totalPago, 2) && !recebimentoGerarCredito)
            {
                throw new Exception(string.Format("O valor pago não confere com o valor a pagar. Valor pago: {0} Valor a pagar: {1}.",
                    totalPago.ToString("C"), !obra.GerarCredito ? obra.TotalProdutos.ToString("C") : obra.ValorObra.ToString("C")));
            }

            if (UtilsFinanceiro.ContemFormaPagto(Pagto.FormaPagto.ChequeProprio, obra.FormasPagto) && string.IsNullOrEmpty(obra.ChequesPagto))
            {
                throw new Exception("Cadastre o(s) cheque(s) referente(s) ao pagamento da conta.");
            }

            UtilsFinanceiro.ValidarRecebimento(session, recebimentoCaixaDiario, (int)obra.IdCliente, idLojaRecebimento,
                obra.CartaoNaoIdentificado?.Select(f => ((int?)f).GetValueOrDefault()) ?? new List<int>(), obra.ContasBancoPagto?.Select(f => ((int?)f).GetValueOrDefault()) ?? new List<int>(),
                obra.FormasPagto?.Select(f => ((int?)f).GetValueOrDefault()) ?? new List<int>(), recebimentoGerarCredito, jurosRecebimento, false, UtilsFinanceiro.TipoReceb.Obra, totalPago, totalPagar);

            #endregion

            #region Validação da finalização da obra

            // Finaliza a obra, caso seja geração de crédito ou caso o saldo seja 0, pois, nesse último caso, a obra será finalizada após a atualização do saldo.
            if (obra.GerarCredito || saldoObra == 0)
            {
                ValidarFinalizarObra(session, obra);
            }

            #endregion
        }

        /// <summary>
        /// Finaliza o pré pagamento à vista da obra.
        /// </summary>
        public void FinalizarPrePagamentoVistaComTransacao(int idObra)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    FinalizarPrePagamentoVista(transaction, idObra);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("FinalizarPrePagamentoVistaComTransacao - ID obra: {0}.", idObra), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao finalizar o pré recebimento da obra.", ex));
                }
            }
        }

        /// <summary>
        /// Finaliza o pré pagamento à vista da obra.
        /// </summary>
        public void FinalizarPrePagamentoVista(GDASession session, int idObra)
        {
            #region Declaração de variáveis

            UtilsFinanceiro.DadosRecebimento retorno = null;
            var usuarioLogado = UserInfo.GetUserInfo;
            var obra = GetElement(session, (uint)idObra);
            var dataRecebimento = obra.DataRecebimento.GetValueOrDefault(DateTime.Now).ToString("dd/MM/yyyy");
            var totalPago = obra.TotalPago.GetValueOrDefault();
            var totalPagar = obra.TotalPagar.GetValueOrDefault();
            var idLojaRecebimento = obra.IdLojaRecebimento.GetValueOrDefault();
            var jurosRecebimento = obra.JurosRecebimento.GetValueOrDefault();
            var recebimentoCaixaDiario = obra.RecebimentoCaixaDiario.GetValueOrDefault();
            var recebimentoGerarCredito = obra.RecebimentoGerarCredito.GetValueOrDefault();
            decimal creditoUtilizado = 0;
            decimal saldoDevedor = 0;
            decimal saldoCredito = 0;
            var sqlAtualizacaoSaldoDevedorCliente = string.Empty;
            // Recupera os cheques que foram selecionados no momento do recebimento da obra.
            var chequesRecebimento = ChequesObraDAO.Instance.ObterStringChequesPelaObra(session, idObra);
            var pagamentosObra = PagtoObraDAO.Instance.GetByObra(session, (uint)idObra);
            // Variáveis criadas para recuperar os dados do pagamento da obra.
            var idsCartaoNaoIdentificado = new List<int?>();
            var idsContaBanco = new List<int?>();
            var idsDepositoNaoIdentificado = new List<int?>();
            var idsFormaPagamento = new List<int>();
            var numerosAutorizacaoCartao = new List<string>();
            var quantidadesParcelaCartao = new List<int?>();
            var tiposCartao = new List<int?>();
            var valoresPagos = new List<decimal>();
            var numeroParcelaContaReceber = 0;

            #endregion

            #region Recuperação dos dados de pagamento da obra

            if (pagamentosObra.Any(f => f.IdFormaPagto != (uint)Pagto.FormaPagto.Credito && f.IdFormaPagto != (uint)Pagto.FormaPagto.Obra))
            {
                foreach (var pagamentoObra in pagamentosObra.Where(f => f.IdFormaPagto != (uint)Pagto.FormaPagto.Credito && f.IdFormaPagto != (uint)Pagto.FormaPagto.Obra)
                    .OrderBy(f => f.NumFormaPagto))
                {
                    idsCartaoNaoIdentificado.Add(pagamentoObra.IdCartaoNaoIdentificado.GetValueOrDefault());
                    idsContaBanco.Add((int)pagamentoObra.IdContaBanco.GetValueOrDefault());
                    idsDepositoNaoIdentificado.Add(pagamentoObra.IdDepositoNaoIdentificado.GetValueOrDefault());
                    idsFormaPagamento.Add((int)pagamentoObra.IdFormaPagto);
                    numerosAutorizacaoCartao.Add(pagamentoObra.NumAutCartao);
                    quantidadesParcelaCartao.Add(pagamentoObra.QuantidadeParcelaCartao.GetValueOrDefault());
                    tiposCartao.Add(((int?)pagamentoObra.IdTipoCartao).GetValueOrDefault());
                    valoresPagos.Add(pagamentoObra.ValorPagto);
                }
            }

            if (pagamentosObra.Any(f => f.IdFormaPagto == (uint)Pagto.FormaPagto.Credito))
            {
                creditoUtilizado = (pagamentosObra.FirstOrDefault(f => f.IdFormaPagto == (uint)Pagto.FormaPagto.Credito)?.ValorPagto).GetValueOrDefault();
            }

            #endregion

            #region Recebimento da obra

            retorno = UtilsFinanceiro.Receber(session, (uint)obra.IdLojaRecebimento, null, null, null, null, null, null, null, null, null, obra, null, obra.IdCliente, 0, null, dataRecebimento,
                totalPagar, totalPago, valoresPagos.Select(f => f).ToArray(), idsFormaPagamento.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                idsContaBanco.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), idsDepositoNaoIdentificado.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                idsCartaoNaoIdentificado.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), tiposCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), null, null,
                jurosRecebimento, false, recebimentoGerarCredito, creditoUtilizado, obra.NumAutConstrucard, recebimentoCaixaDiario,
                quantidadesParcelaCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), chequesRecebimento, false, UtilsFinanceiro.TipoReceb.Obra);

            if (retorno.ex != null)
            {
                throw retorno.ex;
            }

            #endregion

            #region Inserção dos dados das contas recebidas

            for (var i = 0; i < idsFormaPagamento.Count(); i++)
            {
                if (idsFormaPagamento.ElementAtOrDefault(i) == 0 || valoresPagos.ElementAtOrDefault(i) == 0)
                {
                    continue;
                }

                var idContaR = ContasReceberDAO.Instance.Insert(session, new ContasReceber()
                {
                    IdLoja = (uint)obra.IdLojaRecebimento,
                    IdObra = obra.IdObra,
                    IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i),
                    IdConta = UtilsPlanoConta.GetPlanoVista((uint)idsFormaPagamento.ElementAt(i)),
                    Recebida = true,
                    ValorVec = valoresPagos.ElementAt(i),
                    ValorRec = valoresPagos.ElementAt(i),
                    DataVec = DateTime.Now,
                    DataRec = DateTime.Now,
                    DataCad = DateTime.Now,
                    IdCliente = obra.IdCliente,
                    UsuRec = usuarioLogado.CodUser,
                    Renegociada = false,
                    NumParc = 1,
                    NumParcMax = 1,
                    IdFuncComissaoRec = obra.IdCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(session, obra.IdCliente) : null
                });

                if (idsFormaPagamento.ElementAt(i) == (uint)Pagto.FormaPagto.Cartao)
                {
                    numeroParcelaContaReceber = ContasReceberDAO.Instance.AtualizarReferenciaContasCartao(session, retorno, quantidadesParcelaCartao.Select(f => f.GetValueOrDefault()),
                        numeroParcelaContaReceber, i, idContaR);
                }

                #region Salva o pagamento da conta

                if (idsFormaPagamento.Count() > i && idsFormaPagamento.ElementAtOrDefault(i) == (int)Pagto.FormaPagto.CartaoNaoIdentificado)
                {
                    var cartoesNaoIdentificado = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(session, idsCartaoNaoIdentificado.Select(f => ((uint?)f).GetValueOrDefault()).ToArray());

                    foreach (var cartaoNaoIdentificado in cartoesNaoIdentificado)
                    {
                        var pagamentoContaReceber = new PagtoContasReceber
                        {
                            IdContaR = idContaR,
                            IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i),
                            ValorPagto = cartaoNaoIdentificado.Valor,
                            IdContaBanco = (uint)cartaoNaoIdentificado.IdContaBanco,
                            IdTipoCartao = (uint)cartaoNaoIdentificado.TipoCartao,
                            NumAutCartao = cartaoNaoIdentificado.NumAutCartao
                        };

                        PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);
                    }
                }
                else
                {
                    var pagamentoContaReceber = new PagtoContasReceber
                    {
                        IdContaR = idContaR,
                        IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i),
                        ValorPagto = valoresPagos.ElementAt(i),
                        IdContaBanco = idsFormaPagamento.ElementAt(i) != (uint)Pagto.FormaPagto.Dinheiro && idsContaBanco.ElementAtOrDefault(i) > 0 ? (uint?)idsContaBanco.ElementAt(i) : null,
                        IdTipoCartao = tiposCartao.ElementAtOrDefault(i) > 0 ? (uint?)tiposCartao.ElementAt(i) : null,
                        IdDepositoNaoIdentificado = idsDepositoNaoIdentificado.ElementAtOrDefault(i) > 0 ? (uint?)idsDepositoNaoIdentificado.ElementAt(i) : null,
                        NumAutCartao = !string.IsNullOrWhiteSpace(numerosAutorizacaoCartao.ElementAt(i)) ? numerosAutorizacaoCartao.ElementAt(i) : null
                    };

                    PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);
                }

                #endregion
            }

            #region Recebimento com crédito

            // Se for pago com crédito, gera a conta recebida do credito
            if (obra.CreditoUtilizado > 0)
            {
                var idContaR = ContasReceberDAO.Instance.Insert(session, new ContasReceber()
                {
                    IdLoja = (uint)obra.IdLojaRecebimento,
                    IdObra = obra.IdObra,
                    IdFormaPagto = null,
                    IdConta = UtilsPlanoConta.GetPlanoVista((uint)Pagto.FormaPagto.Credito),
                    Recebida = true,
                    ValorVec = obra.CreditoUtilizado,
                    ValorRec = obra.CreditoUtilizado,
                    DataVec = DateTime.Now,
                    DataRec = DateTime.Now,
                    DataCad = DateTime.Now,
                    IdCliente = obra.IdCliente,
                    UsuRec = usuarioLogado.CodUser,
                    Renegociada = false,
                    NumParc = 1,
                    NumParcMax = 1
                });

                #region Salva o pagamento da conta

                var pagamentoContaReceber = new PagtoContasReceber
                {
                    IdContaR = idContaR,
                    IdFormaPagto = (uint)Pagto.FormaPagto.Credito,
                    ValorPagto = obra.CreditoUtilizado
                };

                PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);

                #endregion
            }

            #endregion

            #endregion

            #region Referenciação dos cheques da obra

            // Associa a obra ao cheques utilizados no pagamento.
            if (retorno.lstChequesInseridos?.Any(f => f.IdCheque > 0) ?? false)
            {
                objPersistence.ExecuteCommand(session, string.Format("UPDATE cheques SET IdObra={0}, IdCliente={1} WHERE IdCheque IN ({2});", obra.IdObra, obra.IdCliente,
                    string.Join(",", retorno.lstChequesInseridos.Where(f => f.IdCheque > 0).Select(f => f.IdCheque))));
            }

            #endregion

            #region Atualização dos dados da obra

            obra.CreditoGeradoCriar = retorno.creditoGerado;
            obra.CreditoUtilizadoCriar = creditoUtilizado;
            // Atualiza a situação da Obra.
            obra.Situacao = (int)Obra.SituacaoObra.Confirmada;

            base.Update(session, obra);

            // O saldo precisa ser atualizado antes de finalizar a obra para que gere crédito
            AtualizaSaldo(session, obra.IdObra, obra.RecebimentoCaixaDiario.GetValueOrDefault());

            #endregion

            #region Finalização da obra

            // Finaliza a obra, caso seja geração de crédito
            if (obra.GerarCredito)
            {
                decimal temp;
                Finalizar(session, obra.IdObra, recebimentoCaixaDiario, out temp);
            }

            #endregion

            #region Geração das movimentações do caixa

            if (recebimentoGerarCredito && retorno.creditoGerado > 0)
            {
                if (recebimentoCaixaDiario)
                {
                    CaixaDiarioDAO.Instance.MovCxObra(session, (uint)idLojaRecebimento, obra.IdCliente, obra.IdObra, 1, retorno.creditoGerado, 0,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado), null, null, false);
                }
                else
                {
                    CaixaGeralDAO.Instance.MovCxObra(session, obra.IdObra, obra.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado), 1,
                        retorno.creditoGerado, 0, null, false, null, null);
                }
            }

            #endregion

            #region Cálculo do saldo devedor do cliente

            ClienteDAO.Instance.ObterSaldoDevedor(session, obra.IdCliente, out saldoDevedor, out saldoCredito);

            objPersistence.ExecuteCommand(session, string.Format("UPDATE obra SET SaldoDevedor=?saldoDevedor, SaldoCredito=?saldoCredito WHERE IdObra={0};", obra.IdObra),
                new GDAParameter("?saldoDevedor", saldoDevedor), new GDAParameter("?saldoCredito", saldoCredito));

            #endregion
        }

        /// <summary>
        /// Cancela o pré pagamento à vista da obra.
        /// </summary>
        public void CancelarPrePagamentoVistaComTransacao(int idObra, string motivo)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    CancelarPrePagamentoVista(transaction, idObra, motivo);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CancelarPrePagamentoVista - ID obra: {0}.", idObra), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cancelar a obra.", ex));
                }
            }
        }

        /// <summary>
        /// Cancela o pré pagamento à vista da obra.
        /// </summary>
        public void CancelarPrePagamentoVista(GDASession session, int idObra, string motivo)
        {
            #region Declaração de variáveis

            var obra = GetElement(session, (uint)idObra);
            var recebimentoCaixaDiario = obra.RecebimentoCaixaDiario.GetValueOrDefault();

            #endregion

            #region Validações

            // Verifica se existem cheques associados à esta obra depositados.
            if (ExecuteScalar<bool>(string.Format("SELECT COUNT(*) > 0 FROM cheques WHERE IdDeposito > 0 AND IdObra={0};", obra.IdObra)))
            {
                throw new Exception("Esta obra possui cheques que já foram depositados, cancele ou retifique o depósito antes de cancelá-la.");
            }

            #endregion

            #region Remoção dos dados do pagamento

            // Garante que não haverá chaves duplicadas para esta obra.
            PagtoObraDAO.Instance.DeleteByObra(session, obra.IdObra);
            // Exclui os cheques utilizados no recebimento, para que ao receber a obra novamente, somente os cheques do último recebimentos estejam no banco de dados.
            ChequesObraDAO.Instance.ExcluirPelaObra(session, (int)obra.IdObra);

            #endregion

            #region Atualização dos dados da obra

            obra.CreditoUtilizadoCriar = 0;
            obra.ValorCreditoAoCriar = 0;
            // Atualiza a situação da Obra.
            obra.Situacao = (int)Obra.SituacaoObra.Aberta;
            obra.TipoPagto = (int)Obra.TipoPagtoObra.AVista;
            obra.JurosRecebimento = null;
            obra.RecebimentoCaixaDiario = false;
            obra.RecebimentoGerarCredito = false;

            Update(session, obra);

            // O saldo precisa ser atualizado antes de finalizar a obra para que gere crédito.
            AtualizaSaldo(session, obra, obra.IdObra, recebimentoCaixaDiario, false);

            #endregion

            LogCancelamentoDAO.Instance.LogObra(session, obra, motivo, true);
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

        /// <summary>
        /// Obtém o valor gasto da obra.
        /// </summary>
        public decimal ObterValorGasto(GDASession session, int idObra)
        {
            var sqlConfirmacao = string.Format(@"SELECT SUM(COALESCE(Total, 0)) FROM pedido
                WHERE IdObra={0} AND Situacao={1}", idObra, (int)Pedido.SituacaoPedido.Confirmado);
            var sqlLiberacao = string.Format(@"SELECT SUM(p.ValorPagamentoAntecipado) FROM pedido p
                    LEFT JOIN pedido_espelho pe ON (pe.IdPedido=p.IdPedido)
                WHERE p.IdObra={0} AND p.Situacao IN ({1},{2},{3})",
                idObra, (int)Pedido.SituacaoPedido.LiberadoParcialmente, (int)Pedido.SituacaoPedido.Confirmado, (int)Pedido.SituacaoPedido.ConfirmadoLiberacao);

            return ExecuteScalar<decimal>(session, !PedidoConfig.LiberarPedido ? sqlConfirmacao : sqlLiberacao);
        }

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