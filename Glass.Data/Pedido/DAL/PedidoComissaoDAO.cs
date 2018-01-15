using System;
using System.Linq;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PedidoComissaoDAO : BaseDAO<PedidoComissao, PedidoComissaoDAO>
    {
        //private PedidoComissaoDAO() { }

        #region Cria/obtem pedido comissão de vários pedidos de um funcionário

        /// <summary>
        /// Obtem/cria registros da tabela pedido_comissao referentes aos pedidos e funcionário informados por parâmetro.
        /// </summary>
        public List<PedidoComissao> ObterPedidoComissaoFuncionarioPorPedidos(GDASession session, List<int> idsPedido, int idFunc)
        {
            // SQL base para recuperar os registros da tabela pedido_comissao.
            var sql = string.Format(@"SELECT pc.IdPedidoComissao, pc.IdPedido, pc.IdFunc, pc.ValorPagar, pc.ValorPago, pc.DataAlt
                FROM pedido_comissao pc WHERE pc.IdPedido IN ({0}) AND pc.IdFunc={1}", string.Join(",", idsPedido), idFunc);

            // Recupera os registros da tabela pedido_comissao.
            var pedidosComissao = objPersistence.LoadData(session, sql).ToList();

            // Caso existam mais IDs de pedido do que pedidos de comissão, significa que alguns pedidos não possuem registro na
            // tabela pedido_comissao, portanto, é necessário criar esse registro.
            if (pedidosComissao.Count < idsPedido.Count)
            {
                // Busca o ID de todos os pedidos que não existem na listagem de pedidos de comissão recuperada.
                foreach (var idPedido in idsPedido.Where(f => !pedidosComissao.Select(g => g.IdPedido).ToList().Contains((uint)f)))
                    // Cria os pedidos de comissão.
                    objPersistence.ExecuteCommand(session, string.Format("INSERT INTO pedido_comissao (IdPedido, IdFunc) VALUES ({0}, {1})", idPedido, idFunc));

                // Limpa a listagem de pedidos de comissão com os pedidos incompletos.
                pedidosComissao.Clear();

                // Recupera todos os pedidos de comissão, que já existiam e que acabaram de ser inseridos no banco.
                pedidosComissao = objPersistence.LoadData(session, sql).ToList();
            }

            return pedidosComissao;
        }

        /// <summary>
        /// Chamado 48565. Cria/atualiza registros da tabela pedido_comissao com base nos dados de comissão.
        /// </summary>
        public void CriarPedidoComissaoPorPedidosEFuncionario(GDASession sessao, IEnumerable<Pedido> pedidos, int idFunc, Pedido.TipoComissao tipoComissao)
        {
            var sql = string.Empty;
            
            // SQL base da atualização do registro da tabela pedido_comissao.
            var sqlAtualizacao = "UPDATE pedido_comissao SET ValorPagar={0}, ValorPago={1}, DataAlt=?dataAlt WHERE IdPedidoComissao={2};";
            var p = new string[3];
            var cont = 0;
            const int QTD_COMANDOS_SQL_POR_EXECUCAO = 200;

            // O método deverá prosseguir somente se o ID do funcionário for maior que zero e se o tipo da comissão for por funcionário.
            // Isso porque esses métodos foram criados para sanar a lentidão ao filtrar um funcionário na tela de comissões a serem geradas.
            // Nessa tela, somente as comissões do tipo Funcionário são criadas.
            if (idFunc > 0 && tipoComissao == Pedido.TipoComissao.Funcionario)
            {
                // Obtem as comissões dos pedidos.
                var pedidosComissao = ObterPedidoComissaoFuncionarioPorPedidos(sessao, pedidos.Select(f => (int)f.IdPedido).ToList(), idFunc);
                
                foreach (var pedidoComissao in pedidosComissao)
                {
                    // Recupera o pedido equivalente ao pedido de comissão.
                    var pedido = pedidos.FirstOrDefault(f => f.IdPedido == pedidoComissao.IdPedido);
                    
                    if (pedido == null || pedido.IdPedido == 0)
                        continue;
                    
                    if (pedido.ComissaoConfig == null)
                        pedido.ComissaoConfig = ComissaoConfigDAO.Instance.GetComissaoConfig(sessao, (uint)idFunc);

                    pedido.ComissaoFuncionario = tipoComissao;
                    
                    if (pedidoComissao.ValorPagar > 0 && pedidoComissao.ValorPagar <= pedidoComissao.ValorPago)
                        continue;
                    
                    // Recupera a data da última alteração do pedido.
                    var dataUltimaAlteracaoPedido = PedidoDAO.Instance.ObterDataUltimaAlteracao(sessao, (int)pedido.IdPedido);

                    // Caso a data da última alteração do pedido seja menor que a data da última atualização do registro na tabela
                    // pedido_comissao, não atualiza o registro novamente.
                    if (dataUltimaAlteracaoPedido.HasValue && pedidoComissao.DataAlt.HasValue && dataUltimaAlteracaoPedido < pedidoComissao.DataAlt)
                    {
                        /* Chamado 54756. */
                        var houveAlteracaoConfiguracaoComissao = objPersistence.ExecuteSqlQueryCount(sessao, string.Format(@"SELECT COUNT(*) FROM log_alteracao la
	                            INNER JOIN comissao_config cc ON (la.IdRegistroAlt=cc.IdComissaoConfig)
                            WHERE cc.IdFunc={0} AND DataAlt>?dataAlt", idFunc), new GDAParameter("?dataAlt", pedidoComissao.DataAlt)) > 0;

                        if (!houveAlteracaoConfiguracaoComissao)
                            continue;
                    }

                    // Atualiza os valores da tabela pedido_comissao.
                    p[0] = Math.Round(pedido.ValorComissaoTotal, 2).ToString().Replace(',', '.');
                    p[1] = pedido.ValorComissaoRecebida.ToString().Replace(',', '.');
                    p[2] = pedidoComissao.IdPedidoComissao.ToString().Replace(',', '.');
                    sql += string.Format(sqlAtualizacao, p);

                    cont++;

                    // Caso tenham 200 SQLs a serem executados, executa-os, caso contrário segue o loop.
                    if (cont % QTD_COMANDOS_SQL_POR_EXECUCAO == 0)
                    {
                        objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?dataAlt", DateTime.Now));
                        sql = string.Empty;
                    }
                }

                // Executa os SQLs que não foram executados no loop dos pedidos de comissão.
                if (sql.Length > 0)
                    objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?dataAlt", DateTime.Now));
            }
        }

        #endregion

        #region Métodos privados

        /// <summary>
        /// Retorna um item através do pedido e do funcionário.
        /// </summary>
        public PedidoComissao GetByPedidoFunc(GDASession session, uint idPedido, int tipoFunc, uint idFunc, bool criarSeNaoEncontrar)
        {
            string campoFunc = tipoFunc == 0 ? "idFunc" : tipoFunc == 1 ? "idComissionado" : tipoFunc == 2 ? "idInstalador" : "idGerente";
            string sql = "select idPedidoComissao from pedido_comissao where idPedido=" + idPedido +
                         " and " + (tipoFunc == 0 ? "idFunc" : tipoFunc == 1 ? "idComissionado" : tipoFunc == 2 ? "idInstalador" : "idGerente") + "=" +
                         idFunc;

            uint? idPedidoComissao = ExecuteScalar<uint?>(session, sql);

            if (idPedidoComissao == null && criarSeNaoEncontrar)
            {
                objPersistence.ExecuteCommand(session,
                    string.Format("insert into pedido_comissao (idPedido, {0}) values ({1}, {2})",
                        campoFunc, idPedido, idFunc));

                idPedidoComissao = ExecuteScalar<uint?>(session, "select @@identity");
            }

            if (idPedidoComissao == null)
                return null;

            var retorno = new PedidoComissao();
            retorno.IdPedidoComissao = idPedidoComissao.Value;
            retorno.IdPedido = idPedido;

            switch (tipoFunc)
            {
                case 0:
                    retorno.IdFunc = idFunc;
                    break;

                case 1:
                    retorno.IdComissionado = idFunc;
                    break;

                case 2:
                    retorno.IdInstalador = idFunc;
                    break;

                case 3:
                    retorno.IdGerente = idFunc;
                    break;
            }

            retorno.ValorPagar = ObtemValorCampo<decimal>(session, "valorPagar", "idPedidoComissao=" + idPedidoComissao);
            retorno.ValorPago = ObtemValorCampo<decimal>(session, "valorPago", "idPedidoComissao=" + idPedidoComissao);

            return retorno;
        }

        #endregion

        /// <summary>
        /// Cria ou altera as comissões de uma lista de pedidos.
        /// </summary>
        public void Create(IEnumerable<Pedido> pedidos, Pedido.TipoComissao tipoComissao)
        {
            Create(null, pedidos, tipoComissao);
        }

        /// <summary>
        /// Cria ou altera as comissões de uma lista de pedidos.
        /// </summary>
        public void Create(GDASession sessao, IEnumerable<Pedido> pedidos, Pedido.TipoComissao tipoComissao)
        {
            string sql = "";
            string sqlUpdate = "update pedido_comissao set ValorPagar={0}, ValorPago={1} where idPedidoComissao={2}; ";
            string[] p = new string[3];
            int cont = 0;
            const int QTD_COMANDOS_SQL_POR_EXECUCAO = 200;

            PedidoComissao pc;

            var dicFuncPedido = new Dictionary<uint, ComissaoConfig>();

            switch (tipoComissao)
            {
                case Pedido.TipoComissao.Gerente:
                    foreach (var ped in pedidos)
                    {
                        var comissaoConfigGerente = ComissaoConfigGerenteDAO.Instance.GetByIdFuncIdLoja(ped.IdLoja, 0);

                        if (comissaoConfigGerente == null)
                            continue;

                        ped.ComissaoFuncionario = tipoComissao;
                        
                        pc = GetByPedidoFunc(sessao, ped.IdPedido, 3, comissaoConfigGerente.IdFuncionario, true);

                        if (pc.ValorPagar > 0 && pc.ValorPagar <= pc.ValorPago)
                            continue;

                        var valorComissao = ComissaoConfigGerenteDAO.Instance.GetComissaoGerenteValor(comissaoConfigGerente, (uint)ped.TipoPedido, ped.ValorBaseCalcComissao); 

                        p[0] = Math.Round(valorComissao, 2).ToString().Replace(',', '.');
                        p[1] = ped.ValorComissaoRecebida.ToString().Replace(',', '.');
                        p[2] = pc.IdPedidoComissao.ToString().Replace(',', '.');
                        sql += string.Format(sqlUpdate, p);

                        cont++;

                        if (cont % QTD_COMANDOS_SQL_POR_EXECUCAO == 0)
                        {
                            objPersistence.ExecuteCommand(sessao, sql);
                            sql = string.Empty;
                        }
                    }
                    break;

                case Pedido.TipoComissao.Funcionario:
                    foreach (Pedido ped in pedidos)
                    {
                        if (ped.ComissaoConfig == null)
                        {
                            if (!dicFuncPedido.ContainsKey(ped.IdFunc))
                                dicFuncPedido.Add(ped.IdFunc, ComissaoConfigDAO.Instance.GetComissaoConfig(sessao, ped.IdFunc));

                            ped.ComissaoConfig = dicFuncPedido[ped.IdFunc];
                        }

                        ped.ComissaoFuncionario = tipoComissao;
                        pc = GetByPedidoFunc(sessao, ped.IdPedido, 0, ped.IdFunc, true);

                        if (pc.ValorPagar > 0 && pc.ValorPagar <= pc.ValorPago)
                            continue;

                        p[0] = Math.Round(ped.ValorComissaoTotal, 2).ToString().Replace(',', '.');
                        p[1] = ped.ValorComissaoRecebida.ToString().Replace(',', '.');
                        p[2] = pc.IdPedidoComissao.ToString().Replace(',', '.');
                        sql += String.Format(sqlUpdate, p);

                        cont++;

                        if (cont%QTD_COMANDOS_SQL_POR_EXECUCAO == 0)
                        {
                            objPersistence.ExecuteCommand(sessao, sql);
                            sql = String.Empty;
                        }
                    }

                    break;

                case Pedido.TipoComissao.Comissionado:
                    foreach (Pedido ped in pedidos)
                    {
                        if (ped.IdComissionado == null)
                            continue;

                        ped.ComissaoFuncionario = tipoComissao;
                        pc = GetByPedidoFunc(sessao, ped.IdPedido, 1, ped.IdComissionado.Value, true);

                        p[0] = Math.Round(ped.ValorComissaoTotal, 2).ToString().Replace(',', '.');
                        p[1] = ped.ValorComissaoRecebida.ToString().Replace(',', '.');
                        p[2] = pc.IdPedidoComissao.ToString().Replace(',', '.');
                        sql += String.Format(sqlUpdate, p);

                        cont++;

                        if (cont%QTD_COMANDOS_SQL_POR_EXECUCAO == 0)
                        {
                            objPersistence.ExecuteCommand(sessao, sql);
                            sql = String.Empty;
                        }
                    }

                    break;

                case Pedido.TipoComissao.Instalador:
                    foreach (Pedido ped in pedidos)
                    {
                        if (ped.IdInstalador == null)
                            continue;

                        ped.ComissaoFuncionario = tipoComissao;
                        pc = GetByPedidoFunc(sessao, ped.IdPedido, 2, ped.IdInstalador.Value, true);

                        p[0] = Math.Round(ped.ValorComissaoTotal, 2).ToString().Replace(',', '.');
                        p[1] = ped.ValorComissaoRecebida.ToString().Replace(',', '.');
                        p[2] = pc.IdPedidoComissao.ToString().Replace(',', '.');
                        sql += string.Format(sqlUpdate, p);

                        cont++;

                        if (cont % QTD_COMANDOS_SQL_POR_EXECUCAO == 0)
                        {
                            objPersistence.ExecuteCommand(sessao, sql);
                            sql = string.Empty;
                        }
                    }

                    break;

                case Pedido.TipoComissao.Todos:
                    Create(sessao, pedidos, Pedido.TipoComissao.Funcionario);
                    Create(sessao, pedidos, Pedido.TipoComissao.Comissionado);
                    Create(sessao, pedidos, Pedido.TipoComissao.Instalador);
                    Create(sessao, pedidos, Pedido.TipoComissao.Gerente);
                    break;
            }

            if (sql.Length > 0)
                objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Cria ou atualiza uma comissão para o pedido.
        /// </summary>
        public void Create(Pedido ped)
        {
            Create(null, ped);
        }

        /// <summary>
        /// Cria ou atualiza uma comissÃ£o para o pedido.
        /// </summary>
        public void Create(GDASession session, Pedido ped)
        {
            Create(session, new[]
            {
                ped
            }, Pedido.TipoComissao.Todos);
        }
    }
}
