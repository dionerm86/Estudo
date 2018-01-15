using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ComissaoDAO : BaseDAO<Comissao, ComissaoDAO>
    {
        //private ComissaoDAO() { }

        #region Busca Padrão

        private string Sql(uint idComissao, string idsComissoes, Pedido.TipoComissao tipoFunc, uint idFuncComissionado,
            uint idPedido, string dataIni, string dataFim, bool selecionar)
        {
            return Sql(null, idComissao, idsComissoes, tipoFunc, idFuncComissionado, idPedido, dataIni, dataFim, selecionar);
        }

        private string Sql(GDASession session, uint idComissao, string idsComissoes, Pedido.TipoComissao tipoFunc, uint idFuncComissionado,
            uint idPedido, string dataIni, string dataFim, bool selecionar)
        {
            string campos = selecionar ? @"c.*, f.Nome as NomeFuncionario, com.Nome as NomeComissionado, i.Nome as NomeInstalador, 
                '$$$' as criterio" : "Count(*)";
            string criterio = "";

            string sql = @"
                Select " + campos + @" 
                From comissao c 
                    Left Join comissionado com On (c.idComissionado=com.idComissionado) 
                    Left Join funcionario f On (c.idFunc=f.idFunc) 
                    Left Join funcionario i On (c.idInstalador=i.idFunc) 
                    LEFT JOIN comissao_contas_receber ccr ON (ccr.IdComissao = c.IdComissao)
                Where ccr.IdComissaoContasReceber IS NULL";

            if (idComissao > 0)
                sql += " And c.idComissao=" + idComissao;
            else if (!String.IsNullOrEmpty(idsComissoes))
                sql += " And c.idComissao in (" + idsComissoes + ")";

            if (tipoFunc == Pedido.TipoComissao.Funcionario)
            {
                sql += " And c.idFunc is not null";
                criterio += "Funcionários    ";
            }
            else if (tipoFunc == Pedido.TipoComissao.Comissionado)
            {
                sql += " And c.idComissionado is not null";
                criterio += "Comissionados    ";
            }
            else if (tipoFunc == Pedido.TipoComissao.Instalador)
            {
                sql += " And c.idInstalador is not null";
                criterio += "Instaladores    ";
            }

            else if (tipoFunc == Pedido.TipoComissao.Gerente)
            {
                sql += " And c.idGerente is not null";
                criterio += "Gerentes    ";
            }

            if (idPedido > 0)
            {
                sql += " and c.idComissao in (select idComissao from comissao_pedido where idPedido=" + idPedido + ")";
                criterio += "Pedido: " + idPedido + "    ";
            }
            else
            {
                if (idFuncComissionado > 0)
                {
                    if (tipoFunc == Pedido.TipoComissao.Funcionario)
                    {
                        sql += " And c.idFunc=" + idFuncComissionado;
                        criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(session, idFuncComissionado) + "    ";
                    }
                    else if (tipoFunc == Pedido.TipoComissao.Comissionado)
                    {
                        sql += " And c.idComissionado=" + idFuncComissionado;
                        criterio += "Comissionado: " + ComissionadoDAO.Instance.GetNome(session, idFuncComissionado) + "    ";
                    }
                    else if (tipoFunc == Pedido.TipoComissao.Instalador)
                    {
                        sql += " And c.idInstalador=" + idFuncComissionado;
                        criterio += "Instalador: " + FuncionarioDAO.Instance.GetNome(session, idFuncComissionado) + "    ";
                    }
                }

                if (!String.IsNullOrEmpty(dataIni))
                {
                    sql += " And c.dataCad>=?dataIni";
                    criterio = "Data Início: " + dataIni + "    ";
                }

                if (!String.IsNullOrEmpty(dataFim))
                {
                    sql += " And c.dataCad<=?dataFim";
                    criterio = "Data Fim: " + dataFim + "    ";
                }
            }

            return sql.Replace("$$$", criterio);
        }

        public Comissao GetElement(uint idComissao)
        {
            return objPersistence.LoadOneData(Sql(idComissao, null, Pedido.TipoComissao.Todos, 0, 0, null, null, true));
        }

        public IList<Comissao> GetForRpt(uint tipoFunc, uint idFuncComissionado, uint idPedido, string dataIni, string dataFim)
        {
            return objPersistence.LoadData(Sql(0, null, (Pedido.TipoComissao)tipoFunc, idFuncComissionado, idPedido, dataIni, dataFim, true) + " Order By dataCad desc", GetParam(dataIni, dataFim)).ToList();
        }

        public uint[] GetForRptRecibos(uint tipoFunc, uint idFuncComissionado, uint idPedido, string dataIni, string dataFim)
        {
            return objPersistence.LoadResult(Sql(0, null, (Pedido.TipoComissao)tipoFunc, idFuncComissionado, idPedido, dataIni, dataFim, true), GetParam(dataIni, dataFim)).Select(f => f.GetUInt32(0))
                       .ToList().ToArray();
        }

        public IList<Comissao> GetList(uint tipoFunc, uint idFuncComissionado, uint idPedido, string dataIni, string dataFim,
            string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "dataCad desc" : sortExpression;

            return LoadDataWithSortExpression(Sql(0, null, (Pedido.TipoComissao)tipoFunc, idFuncComissionado, idPedido, dataIni, dataFim, true), sort, startRow, pageSize, GetParam(dataIni, dataFim));
        }

        public int GetCount(uint tipoFunc, uint idFuncComissionado, uint idPedido, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, null, (Pedido.TipoComissao)tipoFunc, idFuncComissionado, idPedido, dataIni, dataFim,
                false), GetParam(dataIni, dataFim));
        }

        public IList<Comissao> GetForRetificar(uint tipoFunc, uint idFuncComissionado)
        {
            string sql = Sql(0, null, (Pedido.TipoComissao)tipoFunc, idFuncComissionado, 0, null, null, true) + " and c.idComissao in (select distinct idComissao from contas_pagar where paga=false or paga is null) Order By c.dataCad desc";
            return objPersistence.LoadData(sql).ToList();
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca por string

        /// <summary>
        /// Retorna uma lista de comissões de uma string.
        /// </summary>
        public IList<Comissao> GetByString(string idsComissoes)
        {
            return GetByString(null, idsComissoes);
        }

        /// <summary>
        /// Retorna uma lista de comissões de uma string.
        /// </summary>
        public IList<Comissao> GetByString(GDASession session, string idsComissoes)
        {
            return objPersistence.LoadData(session, Sql(session, 0, idsComissoes, Pedido.TipoComissao.Todos, 0, 0, null, null, true)).ToList();
        }

        #endregion

        #region Gera comissão

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Gera comissão para o funcionário/comissionado referente aos pedidos passados
        /// </summary>
        public decimal GerarComissao(Pedido.TipoComissao tipoComissao, uint idFuncComissionado, string idsPedido, string dataRefIni, string dataRefFim,
            decimal valorCalculadoPagina, string dataContaPagar)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = GerarComissao(transaction, tipoComissao, idFuncComissionado, idsPedido, dataRefIni, dataRefFim, valorCalculadoPagina, dataContaPagar);

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

        /// <summary>        
        /// Gera comissão para o funcionário/comissionado referente aos pedidos passados
        /// </summary>
        /// <param name="tipoComissao">0-Funcionário, 1-Comissionado, 2-Instalador</param>
        /// <param name="idFuncComissionado">idFunc ou idComissionado</param>
        /// <param name="idsPedido"></param>
        /// <param name="dataRefIni"></param>
        /// <param name="dataRefFim"></param>
        /// <param name="valorCalculadoPagina"></param>
        /// <param name="dataContaPagar"></param>
        /// <returns></returns>
        public decimal GerarComissao(GDASession sessao, Pedido.TipoComissao tipoComissao, uint idFuncComissionado, string idsPedido, string dataRefIni, string dataRefFim,
            decimal valorCalculadoPagina, string dataContaPagar)
        {
            // Apenas administrador, financeiro geral e financeiro pagto podem gerar comissões
            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                throw new Exception("Você não tem permissão para gerar comissões");

            uint idComissao = 0;
            decimal valorComissao = 0;
            decimal percAcrescimo = 1;

            // Calcula o valor a pagar de comissão para todos os pedidos selecionados
            Pedido[] pedidos = PedidoDAO.Instance.GetByString(idsPedido, idFuncComissionado, tipoComissao, dataRefIni, dataRefFim);

            if (tipoComissao == Pedido.TipoComissao.Gerente)
            {
                valorComissao = ComissaoConfigGerenteDAO.Instance.GetValorComissaoPedidos(pedidos, idFuncComissionado, dataRefIni, dataRefFim);
            }
            else if (Configuracoes.ComissaoConfig.DescontarComissaoPerc || tipoComissao == Pedido.TipoComissao.Comissionado)
            {
                foreach (Pedido p in pedidos)
                    valorComissao += p.ValorComissaoPagar;
            }
            else
            {
                valorComissao = Math.Round(ComissaoConfigDAO.Instance.GetComissaoValor(tipoComissao, idFuncComissionado,
                    dataRefIni, dataRefFim, idsPedido), 2);
            }

            // Subtrai os débitos de comissão que o funcionário possa ter
            KeyValuePair<string, decimal> debitos = DebitoComissaoDAO.Instance.GetDebitos(idFuncComissionado, tipoComissao);
            valorComissao -= debitos.Value;

            // Ajusta o valor calculado ao valor exibido na página de comissão, se necessário
            if (valorCalculadoPagina > 0)
                percAcrescimo = Math.Round(Math.Round(valorComissao, 2) / valorCalculadoPagina);

            if (valorComissao <= 0)
                throw new Exception("Comissão para o(os) pedido(os) já foi gerada, ou os pedidos não possuem valores a serem gerados.");

            // Cria uma nova comissao
            Comissao comissao = new Comissao();
            comissao.Total = valorComissao;
            comissao.DataRefIni = DateTime.Parse(dataRefIni);
            comissao.DataRefFim = DateTime.Parse(dataRefFim);
            comissao.DataCad = DateTime.Now;

            // Define o tipo de funcionário para a nova comissão
            switch (tipoComissao)
            {
                case Pedido.TipoComissao.Funcionario:
                    comissao.IdFunc = idFuncComissionado;
                    break;
                case Pedido.TipoComissao.Comissionado:
                    comissao.IdComissionado = idFuncComissionado;
                    break;
                case Pedido.TipoComissao.Instalador:
                    comissao.IdInstalador = idFuncComissionado;
                    break;
                case Pedido.TipoComissao.Gerente:
                    comissao.IdGerente = idFuncComissionado;
                    break;
            }

            try
            {
                idComissao = Insert(sessao, comissao);

                // Associa os pedidos 
                string sqlInsert = String.Empty;

                // Salva o valor pago por pedido
                foreach (Pedido p in pedidos)
                {                                       
                    var valorComissaoPedido = p.ValorComissaoPagar * percAcrescimo;
                    decimal baseCalcRecebido = Convert.ToDecimal(ComissaoPedidoDAO.Instance.GetTotalBaseCalcComissaoPedido(p.IdPedido, (int)tipoComissao, idFuncComissionado));
                    decimal baseCalcAtual = p.ValorBaseCalcComissao - (PedidoConfig.LiberarPedido ? 0 : baseCalcRecebido);

                    string baseCalc = baseCalcAtual.ToString().Replace(',', '.');

                    /* Chamado 58585. */
                    if (valorComissaoPedido <= 0)
                        throw new Exception("Comissão para o(os) pedido(os) já foi gerada, ou os pedidos não possuem valores a serem gerados.");

                    sqlInsert += "Insert Into comissao_pedido (idPedido, idComissao, valor, basecalccomissao)" +
                        " values (" + p.IdPedido + ", " + idComissao + ", " + valorComissaoPedido.ToString().Replace(',', '.') + ", " + baseCalc + ");";
                    
                    if (tipoComissao == Pedido.TipoComissao.Gerente)
                        p.ValorComissaoGerentePago = decimal.Round(valorComissaoPedido, 2);
                }

                objPersistence.ExecuteCommand(sessao, sqlInsert);

                // Atualiza a tabela de comissão por pedido
                if(tipoComissao != Pedido.TipoComissao.Gerente)
                    pedidos = PedidoDAO.Instance.GetByString(idsPedido, idFuncComissionado, tipoComissao, dataRefIni, dataRefFim);

                if (tipoComissao == Pedido.TipoComissao.Gerente)
                    PedidoComissaoDAO.Instance.Create(sessao, pedidos, tipoComissao, (int?)idFuncComissionado);

                else
                    PedidoComissaoDAO.Instance.Create(sessao, pedidos, tipoComissao);

                // Marca que os débitos foram quitados
                DebitoComissaoDAO.Instance.MarcaComissao(sessao, debitos.Key, idComissao, tipoComissao);

                #region Gera a conta a pagar para a comissão

                DateTime dataPagar;
                if (!DateTime.TryParse(dataContaPagar, out dataPagar))
                    dataPagar = DateTime.Now.AddMonths(1);

                ContasPagar cp = new ContasPagar();
                cp.IdComissao = idComissao;
                cp.IdConta = FinanceiroConfig.PlanoContaComissao;
                cp.DataVenc = dataPagar;
                cp.ValorVenc = valorComissao;
                cp.IdLoja = UserInfo.GetUserInfo.IdLoja;
                cp.NumParc = 1;
                cp.NumParcMax = 1;
                cp.Usucad = UserInfo.GetUserInfo.CodUser;

                ContasPagarDAO.Instance.Insert(sessao, cp);

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return valorComissao;
        }

        #endregion

        #region Retifica comissão

        /// <summary>
        /// Retifica comissão para o funcionário/comissionado referente aos pedidos passados
        /// </summary>
        /// <param name="tipoComissao">0-Funcionário, 1-Comissionado, 2-Instalador</param>
        /// <param name="idFuncComissionado">idFunc ou idComissionado</param>
        /// <param name="idsPedidoRetirar">Os ids dos pedidos que serão retirados da comissão.</param>
        public void RetificarComissao(uint idComissao, Pedido.TipoComissao tipoComissao, uint idFuncComissionado, string idsPedidoRetirar,
            decimal valorComissao, string dataContaPagar)
        {
            // Apenas financeiro pagto podem retificar comissões
            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                throw new Exception("Você não tem permissão para retificar comissões");

            decimal valorComissaoRetirar = 0;

            // Define os pedidos que serão retirados da comissão
            List<uint> idPedidoRetirar = new List<uint>();

            if (!String.IsNullOrEmpty(idsPedidoRetirar))
                idPedidoRetirar.AddRange(Array.ConvertAll<string, uint>(idsPedidoRetirar.Split(','),
                     new Converter<string, uint>(
                         delegate(string x)
                         {
                             return Glass.Conversoes.StrParaUint(x);
                         }
                     )));

            // Busca os pedidos da comissão
            var pedidos = PedidoDAO.Instance.GetPedidosByComissao(idComissao, tipoComissao, idFuncComissionado);

            // Calcula o valor que será removido da comissão
            foreach (Pedido p in pedidos)
                if (idPedidoRetirar.Contains(p.IdPedido))
                {
                    if (!DebitoComissaoDAO.Instance.VerificaCancelarPedido(idComissao, p.IdPedido, tipoComissao))
                        throw new Exception("O pedido " + p.IdPedido + " já foi cancelado e tem um débito de comissão já quitado.");

                    valorComissaoRetirar += (decimal)p.ValorPagoComissao;
                }

            // Remove os pedidos da lista de débitos de comissão
            foreach (Pedido p in pedidos)
                if (idPedidoRetirar.Contains(p.IdPedido))
                    DebitoComissaoDAO.Instance.CancelaPedido(p.IdPedido, tipoComissao);

            // Recupera as datas de início e fim da comissão paga
            string dataIni = ObtemValorCampo<string>("dataRefIni", "idComissao=" + idComissao);
            string dataFim = ObtemValorCampo<string>("dataRefFim", "idComissao=" + idComissao);

            if (!String.IsNullOrEmpty(dataIni))
                dataIni = dataIni.Split(' ')[0];

            if (!String.IsNullOrEmpty(dataFim))
                dataFim = dataFim.Split(' ')[0];

            // Altera a comissao
            objPersistence.ExecuteCommand("update comissao set total=total-" + valorComissaoRetirar.ToString().Replace(",", ".") + " where idComissao=" + idComissao);
            if (!String.IsNullOrEmpty(idsPedidoRetirar))
                objPersistence.ExecuteCommand("delete from comissao_pedido where idComissao=" + idComissao + " and idPedido in (" + idsPedidoRetirar + ")");

            if (tipoComissao == Pedido.TipoComissao.Gerente)
            {
                if (!string.IsNullOrEmpty(idsPedidoRetirar))
                    objPersistence.ExecuteCommand("update pedido_comissao set valorpago = 0 where IdGerente = " + idFuncComissionado + " AND IdPedido IN (" + idsPedidoRetirar + ")");
            }
            else
            {
                // Atualiza a tabela de comissão por pedido
                pedidos = PedidoDAO.Instance.GetByString(idsPedidoRetirar, idFuncComissionado, tipoComissao, dataIni, dataFim);
                PedidoComissaoDAO.Instance.Create(pedidos, tipoComissao);
            }

            #region Gera a conta a pagar para a comissão

            ContasPagarDAO.Instance.DeleteByComissao(idComissao, "Retificação");

            DateTime dataPagar;
            if (!DateTime.TryParse(dataContaPagar, out dataPagar))
                dataPagar = DateTime.Now.AddMonths(1);

            ContasPagar cp = new ContasPagar();
            cp.IdComissao = idComissao;
            cp.IdConta = FinanceiroConfig.PlanoContaComissao;
            cp.DataVenc = dataPagar;
            cp.ValorVenc = valorComissao;
            cp.IdLoja = UserInfo.GetUserInfo.IdLoja;
            cp.NumParc = 1;
            cp.NumParcMax = 1;

            ContasPagarDAO.Instance.Insert(cp);

            #endregion
        }

        #endregion

        #region Métodos sobrescritos

        private Pedido.TipoComissao GetTipoFunc(uint idComissao)
        {
            if (ObtemValorCampo<uint?>("idFunc", "idComissao=" + idComissao) > 0)
                return Pedido.TipoComissao.Funcionario;
            else if (ObtemValorCampo<uint?>("idComissionado", "idComissao=" + idComissao) > 0)
                return Pedido.TipoComissao.Comissionado;
            else if (ObtemValorCampo<uint?>("idInstalador", "idComissao=" + idComissao) > 0)
                return Pedido.TipoComissao.Instalador;
            else if (ObtemValorCampo<uint?>("idGerente", "idComissao=" + idComissao) > 0)
                return Pedido.TipoComissao.Gerente;

            return Pedido.TipoComissao.Todos;
        }

        public override int Delete(Comissao objDelete)
        {
            // Verifica a conta a pagar gerada por esta comissão foi paga
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From contas_pagar Where paga=true And idComissao=" +
                objDelete.IdComissao) > 0)
                throw new Exception("Cancele o pagamento da conta gerada por esta comissão.");

            // Recupera os IDs dos pedidos dessa comissão
            var idsPedidos = ComissaoPedidoDAO.Instance.GetIdsPedidosByComissao(objDelete.IdComissao);

            // Verifica se há débitos pagos para os pedidos da comissão
            var tipoFunc = GetTipoFunc(objDelete.IdComissao);
            foreach (uint idPedido in idsPedidos)
            {
                if (!DebitoComissaoDAO.Instance.VerificaCancelarPedido(objDelete.IdComissao, idPedido, tipoFunc))
                    throw new Exception("O pedido " + idPedido + " já foi cancelado e tem um débito de comissão já quitado.");
            }

            // Concatena os ids em uma string separada por vírgula
            string ids = String.Join(",", Array.ConvertAll<uint, string>(idsPedidos, x => x.ToString()));

            // Apaga a conta a pagar da comissão
            ContasPagarDAO.Instance.DeleteByComissao(objDelete.IdComissao, "Cancelamento");

            // Limpa os débitos de comissão
            DebitoComissaoDAO.Instance.CancelaComissao(objDelete.IdComissao);

            // Remove de pedido comissão o valor pago nessa comissão
            if (tipoFunc == Pedido.TipoComissao.Funcionario)
            {
                foreach (uint idPedido in idsPedidos)
                {   
                   var valorPago = ExecuteScalar<decimal>(
                        string.Format("Select valor From comissao_pedido Where idPedido={0} And idComissao={1}", idPedido, objDelete.IdComissao));

                    objPersistence.ExecuteCommand(
                        string.Format("Update pedido_comissao Set ValorPago=ValorPago-?valorPago Where idFunc={0} And idPedido={1}",
                        ObtemValorCampo<uint>("IdFunc", "IdComissao=" + objDelete.IdComissao), idPedido), new GDAParameter("?valorPago", valorPago));
                }
            }     

            if(tipoFunc == Pedido.TipoComissao.Gerente)
            {
                foreach (uint idPedido in idsPedidos)
                {
                    var valorPago = ExecuteScalar<decimal>(
                         string.Format("Select valor From comissao_pedido Where idPedido={0} And idComissao={1}", idPedido, objDelete.IdComissao));

                    objPersistence.ExecuteCommand(
                        string.Format("Update pedido_comissao Set ValorPago=ValorPago-?valorPago Where IdGerente={0} And idPedido={1}",
                        ObtemValorCampo<uint>("IdGerente", "IdComissao=" + objDelete.IdComissao), idPedido), new GDAParameter("?valorPago", valorPago));
                }
            }

            // Exclui as associações dos pedidos com esta comissão
            ComissaoPedidoDAO.Instance.DeleteByComissao(objDelete.IdComissao);

            // Recalcula as comissões pagas para os pedidos
            string dataIni = ObtemValorCampo<string>("dataRefIni", "idComissao=" + objDelete.IdComissao).Split(' ')[0];
            string dataFim = ObtemValorCampo<string>("dataRefFim", "idComissao=" + objDelete.IdComissao).Split(' ')[0];
            PedidoComissaoDAO.Instance.Create(PedidoDAO.Instance.GetByString(ids, 0, Pedido.TipoComissao.Todos, dataIni, dataFim), Pedido.TipoComissao.Todos);

            return base.Delete(objDelete);
        }

        #endregion

        #region Comissao Contas Recebidas

        private string SqlContasRecebidas(uint idComissao, uint idFunc, uint idLoja, string dataIni, string dataFim, ref string filtroAdicional, bool selecionar)
        {
            var campos = @"c.*, f.Nome as NomeFuncionario";

            var sql = @"
                SELECT " + campos + @"
                FROM comissao c
                    INNER JOIN funcionario f ON (c.IdFunc = f.IdFunc)
                    INNER JOIN comissao_contas_receber ccr ON (ccr.IdComissao = c.IdComissao)
                    INNER JOIN contas_receber cr ON (ccr.IdContaR = cr.IdContaR)
                WHERE 1 {0}";

            if (idComissao > 0)
            {
                sql += " AND c.IdComissao = " + idComissao;
            }

            if (idFunc > 0)
            {
                sql += " AND f.idFunc = " + idFunc;
            }

            if (idLoja > 0)
            {
                sql += " AND cr.IdLoja = " + idLoja;
            }

            if (!string.IsNullOrEmpty(dataIni))
            {
                sql += " AND cr.DataCad >= ?dataIni";
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                sql += " AND cr.DataCad <= ?dataFim";
            }

            sql += " GROUP BY c.IdComissao";

            if (!selecionar)
                sql = "SELECT COUNT(*) FROM (" + sql + ") as tmp";

            return string.Format(sql, filtroAdicional);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Comissao> GetListContasRecebidas(uint idComissao, uint idFunc, uint idLoja, string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional = "";
            var sql = SqlContasRecebidas(idComissao, idFunc, idLoja, dataIni, dataFim, ref filtroAdicional, true) + " ORDER BY c.IdComissao DESC";

            return LoadDataWithSortExpression(sql,sortExpression, startRow,pageSize, GetParam(dataIni, dataFim)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public int GetListContasRecebidasCount(uint idComissao, uint idFunc, uint idLoja, string dataIni, string dataFim)
        {
            string filtroAdicional = "";
            return objPersistence.ExecuteSqlQueryCount(SqlContasRecebidas(idComissao, idFunc, idLoja, dataIni, dataFim, ref filtroAdicional, false), GetParam(dataIni, dataFim));
        }


        /// <summary>
        /// Gera a comissão das contas recebidas
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idsContasR"></param>
        /// <param name="dataContaPg"></param>
        /// <param name="dataCadIni"></param>
        /// <param name="dataCadFim"></param>
        /// <param name="dataRecIni"></param>
        /// <param name="dataRecFim"></param>
        public void GerarComissaoContasRecebidas(uint idFunc, string idsContasR, string dataContaPg, string dataCadIni, string dataCadFim, string dataRecIni, string dataRecFim)
        {
            using (var trans = new GDA.GDATransaction())
            {
                trans.BeginTransaction();

                idsContasR = idsContasR.Trim(',');

                if (idFunc == 0)
                    throw new Exception("O funcionário não foi informado.");

                if (String.IsNullOrEmpty(idsContasR))
                    throw new Exception("Nenhuma conta foi informada.");

                var ids = idsContasR.Trim(',').Split(',').Select(x => x.StrParaUint()).ToList();

                if (ids.Count == 0)
                    throw new Exception("Nenhuma conta foi informada.");

                var contas = ContasReceberDAO.Instance.ObtemContasRecebidasParaComissao(trans, idFunc, idsContasR);

                var contasFora = contas.Where(f => !ids.Contains(f.IdContaR)).Select(f => f.IdContaR).ToList();
                contasFora.AddRange(ids.Where(f => !contas.Select(x => x.IdContaR).ToList().Contains(f)).Select(f => f).ToList());

                if (contasFora.Count > 0)
                    throw new Exception("As contas: " + string.Join(",", contasFora.Select(f => f.ToString()).ToArray()) + " não estão na listagem");

                var valorComissao = contas.Sum(f => f.ValorComissao);

                if (valorComissao <= 0)
                    throw new Exception("Não há valor para gerar comissão.");

                try
                {
                    // Cria uma nova comissao
                    Comissao comissao = new Comissao();
                    comissao.Total = valorComissao;
                    comissao.DataRefIni = dataCadIni.StrParaDate().GetValueOrDefault();
                    comissao.DataRefFim = dataCadFim.StrParaDate().GetValueOrDefault();
                    comissao.DataRecIni = dataRecIni.StrParaDate();
                    comissao.DataRecFim = dataRecFim.StrParaDate();
                    comissao.DataCad = DateTime.Now;
                    comissao.IdFunc = idFunc;

                    var idComissao = Insert(trans, comissao);

                    #region Gera a conta a pagar para a comissão

                    DateTime dataPagar;
                    if (!DateTime.TryParse(dataContaPg, out dataPagar))
                        dataPagar = DateTime.Now.AddMonths(1);

                    ContasPagar cp = new ContasPagar();
                    cp.IdComissao = idComissao;
                    cp.IdConta = FinanceiroConfig.PlanoContaComissao;
                    cp.DataVenc = dataPagar;
                    cp.ValorVenc = valorComissao;
                    cp.IdLoja = UserInfo.GetUserInfo.IdLoja;
                    cp.NumParc = 1;
                    cp.NumParcMax = 1;
                    cp.Usucad = UserInfo.GetUserInfo.CodUser;

                    ContasPagarDAO.Instance.Insert(trans, cp);

                    #endregion

                    #region Vincula as contas marcadas para gerar comissão

                    foreach (var c in contas)
                    {
                        ComissaoContasReceberDAO.Instance.Insert(trans, new ComissaoContasReceber()
                        {
                            IdComissao = (int)idComissao,
                            IdContaR = (int)c.IdContaR,
                            Valor = c.ValorComissao
                        });
                    }

                    #endregion

                    trans.Commit();
                    trans.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    trans.Close();

                    ErroDAO.Instance.InserirFromException("Comissão de contas recebidas", ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Buscas as comissões de um vendedor para retificar
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public IList<Comissao> GetForRetificarContasRecebidas(uint idFunc)
        {
            string filtroAdicional = " and c.idComissao in (select distinct idComissao from contas_pagar where COALESCE(paga, 0) = 0)";
            string sql = SqlContasRecebidas(0, idFunc, 0, null, null, ref filtroAdicional, true) + " Order By c.dataCad desc";
            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retifica uma comissão
        /// </summary>
        /// <param name="idComissao"></param>
        /// <param name="idFunc"></param>
        /// <param name="idsContasRemover"></param>
        /// <param name="valorComissao"></param>
        /// <param name="dataContaPagar"></param>
        public void RetificarComissaoContasReceber(uint idComissao, uint idFunc, string idsContasRemover, decimal valorComissao, string dataContaPagar)
        {
            using (var trans = new GDA.GDATransaction())
            {
                trans.BeginTransaction();

                // Apenas financeiro pagto pode retificar comissões
                uint tipoUsuario = UserInfo.GetUserInfo.TipoUsuario;
                if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                    throw new Exception("Você não tem permissão para retificar comissões");

                var contaPagar = ContasPagarDAO.Instance.GetByComissao(trans, idComissao);
                var comissao = GetElementByPrimaryKey(trans, idComissao);

                if (comissao == null)
                    throw new Exception("Comissão não encontrada.");

                if (contaPagar == null || contaPagar.Paga)
                    throw new Exception("Não é possível retificar essa conta, pois ela já foi paga.");

                try
                {
                    //Remove a associação das contas
                    ComissaoContasReceberDAO.Instance.DeleteByContasRecebidas(trans, idsContasRemover);

                    //Verifica a data da conta a pagar
                    DateTime dataPagar;
                    if (!DateTime.TryParse(dataContaPagar, out dataPagar))
                        dataPagar = DateTime.Now.AddMonths(1);

                    //Atualiza a conta a pagar
                    contaPagar.DataVenc = dataPagar;
                    contaPagar.ValorVenc = valorComissao;
                    ContasPagarDAO.Instance.Update(trans, contaPagar);

                    //Atualiza a comissao
                    comissao.Total = valorComissao;
                    Update(trans, comissao);

                    trans.Commit();
                    trans.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    trans.Close();

                    ErroDAO.Instance.InserirFromException("Retificar comissão de contas recebidas", ex);
                    throw ex;
                }
            }

        }

        public void RemoveComissaoContasRecebidas(Comissao objDelete)
        {
            // Verifica a conta a pagar gerada por esta comissão foi paga
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From contas_pagar Where paga=true And idComissao=" +
                objDelete.IdComissao) > 0)
                throw new Exception("Cancele o pagamento da conta gerada por esta comissão.");

            using (var trans = new GDA.GDATransaction())
            {
                try
                {
                    trans.BeginTransaction();

                    //remove o vinculo das contas recebidas com a comissão
                    ComissaoContasReceberDAO.Instance.DeleteByComissao(trans, objDelete.IdComissao);

                    // Apaga a conta a pagar da comissão
                    ContasPagarDAO.Instance.DeleteByComissao(trans, objDelete.IdComissao, "Cancelamento");

                    //Apaga a comissão
                    DeleteByPrimaryKey(objDelete.IdComissao);

                    trans.Commit();
                    trans.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    trans.Close();

                    ErroDAO.Instance.InserirFromException("Cancelar comissão de contas recebidas", ex);
                    
                    throw ex;
                }
                


            }
        }

        public IList<Comissao> GetForRptContasRecebidas(uint idComissao, uint idFunc, uint idLoja, string dataIni, string dataFim)
        {
            string filtroAdicional = "";
            var sql = SqlContasRecebidas(idComissao, idFunc, idLoja, dataIni, dataFim, ref filtroAdicional, true);
            return objPersistence.LoadData(sql + " Order By dataCad desc", GetParam(dataIni, dataFim)).ToList();
        }

        public uint ObtemIdLoja(uint idComissao)
        {
            var sql = @"
                SELECT cr.IdLoja
                FROM comissao_contas_receber ccr
                    INNER JOIN contas_receber cr ON (ccr.IdContaR = cr.IdContaR)
                WHERE ccr.IdComissao = " + idComissao + @"
                LIMIT 1";

            return ExecuteScalar<uint>(sql);
        }

        #endregion

        #region Obtem dados da comissão

        public string ObtemPeriodo(uint idComissao)
        {
            var dtIni = ObtemValorCampo<DateTime>("dataRefIni", "IdComissao = " + idComissao);
            var dtFim = ObtemValorCampo<DateTime>("dataRefFim", "IdComissao = " + idComissao);

            if (dtIni.ToShortDateString() == "01/01/0001" && dtFim.ToShortDateString() == "01/01/0001")
                return "";

            return dtIni.ToShortDateString() + " à " + dtFim.ToShortDateString();
        }

        public string ObtemPeriodoRec(uint idComissao)
        {
            var dtIni = ObtemValorCampo<DateTime>("dataRecIni", "IdComissao = " + idComissao);
            var dtFim = ObtemValorCampo<DateTime>("dataRecFim", "IdComissao = " + idComissao);

            if (dtIni.ToShortDateString() == "01/01/0001" && dtFim.ToShortDateString() == "01/01/0001")
                return "";

            return dtIni.ToShortDateString() + " à " + dtFim.ToShortDateString();
        }

        #endregion
    }
}