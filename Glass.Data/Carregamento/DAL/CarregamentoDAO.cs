using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class CarregamentoDAO : BaseCadastroDAO<Carregamento, CarregamentoDAO>
    {
        #region Recupera uma lista de carregamento

        private string Sql(uint idCarregamento, uint idOC, uint idPedido, int idRota, uint idMotorista, string placa, string situacao,
            string dtSaidaIni, string dtSaidaFim, uint idLoja, bool selecionar)
        {
            string campos = selecionar ? @"c.*, f.nome as NomeMotorista, group_concat(Distinct(r.CODINTERNO) SEPARATOR ', ') as DescrRotas,
                CONCAT(v.placa, ' ', v.modelo, ' ', v.Cor, ' ', v.AnoFab) as Veiculo,
                l.nomeFantasia as nomeLoja, '$$$' as criterio" : "COUNT(*)";

            string criterio = "";

            string sql =
                string.Format(@"
                    SELECT {0}
                    FROM carregamento c
                    INNER JOIN loja l ON (c.idLoja = l.idLoja)
                    LEFT JOIN funcionario f ON (c.idMotorista = f.idFunc)
                    LEFT JOIN veiculo v ON (c.placa = v.placa)
                    LEFT JOIN ordem_carga oc ON (c.IDCARREGAMENTO = oc.IDCARREGAMENTO)
                    LEFT JOIN rota r ON (oc.IDROTA = r.IDROTA)
                    WHERE 1", campos);

            if (idCarregamento > 0)
            {
                sql += " AND c.idCarregamento=" + idCarregamento;
                criterio += "Carregamento: " + idCarregamento + "     ";
            }

            if (idRota > 0)
            {
                sql += string.Format($" AND r.idRota={idRota}");
                criterio += string.Format("Rota: {0}     ", RotaDAO.Instance.ObtemCodRotas(idRota.ToString()));
            }

            if (idMotorista > 0)
            {
                sql += " AND c.idMotorista=" + idMotorista;
                criterio += "Motorista: " + FuncionarioDAO.Instance.GetNome(idMotorista) + "     ";
            }

            if (!string.IsNullOrEmpty(placa) && placa != "0")
            {
                sql += " AND c.placa=?placa";
                criterio += "Veículo: " + VeiculoDAO.Instance.ObtemValorCampo<string>("CONCAT(placa, ' ', modelo, ' ', Cor, ' ', AnoFab)",
                    "placa=?placa", new GDAParameter("?placa", placa)) + "     ";
            }

            if (!string.IsNullOrEmpty(situacao) && situacao != "0")
            {
                sql += " AND c.situacao IN(?situacao)";
            }

            if (!string.IsNullOrEmpty(dtSaidaIni))
            {
                sql += " AND c.dataPrevistaSaida>=?dtSaidaIni";
                criterio += "Data Prev. Saída Inicial: " + dtSaidaIni + "     ";
            }

            if (!string.IsNullOrEmpty(dtSaidaFim))
            {
                sql += " AND c.dataPrevistaSaida<=?dtSaidaFim";
                criterio += "Data Prev. Saída Final: " + dtSaidaFim + "     ";
            }

            if (idLoja > 0)
            {
                sql += " AND c.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "     ";
            }

            if (idOC > 0)
            {
                sql += $" AND oc.IdOrdemCarga={idOC}";
                criterio += "Ordem de Carga: " + idOC + "     ";
            }

            if (idPedido > 0)
            {
                sql += @" AND EXISTS (
                    SELECT *
                    FROM pedido_ordem_carga poc
                    INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.idOrdemCarga)
                    WHERE oc.idCarregamento = c.idCarregamento AND poc.idPedido = " + idPedido + @"
                    )";
            }

            sql += " GROUP BY c.idCarregamento ORDER BY c.idCarregamento DESC";

            var seleciona = selecionar ? "temp.*" : "Count(*)";

            return $" Select {seleciona} From ({sql.Replace("$$$", criterio)}) AS temp";
        }

        private GDAParameter[] GetParam(string placa, string situacao, string dtSaidaIni, string dtSaidaFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(placa))
                lstParam.Add(new GDAParameter("?placa", placa));

            if (!String.IsNullOrEmpty(situacao))
                lstParam.Add(new GDAParameter("?situacao", situacao));

            if (!String.IsNullOrEmpty(dtSaidaIni))
                lstParam.Add(new GDAParameter("?dtSaidaIni", DateTime.Parse(dtSaidaIni)));

            if (!String.IsNullOrEmpty(dtSaidaFim))
                lstParam.Add(new GDAParameter("?dtSaidaFim", DateTime.Parse(dtSaidaFim)));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        /// <summary>
        /// Recupera uma lista de carregamentos
        /// </summary>
        public IList<Carregamento> GetListWithExpression(uint idCarregamento, uint idOC, uint idPedido, int idRota, uint idMotorista, string placa, string situacao,
            string dtSaidaIni, string dtSaidaFim, uint idLoja, string sortExpression, int startRow, int pageSize)
        {
            string sql = Sql(idCarregamento, idOC, idPedido, idRota, idMotorista, placa, situacao, dtSaidaIni, dtSaidaFim, idLoja, true);
            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, GetParam(placa, situacao, dtSaidaIni, dtSaidaFim));
        }

        public int GetListWithExpressionCount(uint idCarregamento, uint idOC, uint idPedido, int idRota, uint idMotorista, string placa, string situacao,
            string dtSaidaIni, string dtSaidaFim, uint idLoja)
        {
            string sql = Sql(idCarregamento, idOC, idPedido, idRota, idMotorista, placa, situacao, dtSaidaIni, dtSaidaFim, idLoja, false);
            return objPersistence.ExecuteSqlQueryCount(sql, GetParam(placa, situacao, dtSaidaIni, dtSaidaFim));
        }

        /// <summary>
        /// Recupera uma lista de carregamentos para o relatório
        /// </summary>
        public IList<Carregamento> GetListForRpt(uint idCarregamento, uint idOC, uint idPedido, int idRota, uint idMotorista, string placa, string situacao,
            string dtSaidaIni, string dtSaidaFim, uint idLoja)
        {
            string sql = Sql(idCarregamento, idOC, idPedido, idRota, idMotorista, placa, situacao, dtSaidaIni, dtSaidaFim, idLoja, true);
            return objPersistence.LoadData(sql, GetParam(placa, situacao, dtSaidaIni, dtSaidaFim)).ToList();
        }

        /// <summary>
        /// Recupera um carregamento
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public Carregamento GetElement(uint idCarregamento)
        {
            return objPersistence.LoadOneData(Sql(idCarregamento, 0, 0, 0, 0, null, null, null, null, 0, true));
        }

        #endregion

        #region Obtem dados do carregamento

        /// <summary>
        /// Recupera a situação do carregamento
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public Carregamento.SituacaoCarregamentoEnum ObtemSituacao(uint idCarregamento)
        {
            return ObtemValorCampo<Carregamento.SituacaoCarregamentoEnum>("situacao", "idCarregamento=" + idCarregamento);
        }

        /// <summary>
        /// Verifica quantas OC um carregamento possiu
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public int ObtemQtdeOCsCarregamento(GDASession sessao, uint idCarregamento)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM ordem_carga
                WHERE idCarregamento=" + idCarregamento;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql);
        }

        /// <summary>
        /// Verifica quantas OC um carregamento possiu
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public int ObtemQtdeOCsCarregamento(uint idCarregamento)
        {
            return ObtemQtdeOCsCarregamento(null, idCarregamento);
        }

        /// <summary>
        /// Obtem a placa do veículo usado no carregamento.
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public string ObtemPlaca(uint idCarregamento)
        {
            return ObtemPlaca(null, idCarregamento);
        }

        /// <summary>
        /// Obtem a placa do veículo usado no carregamento.
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public string ObtemPlaca(GDASession sessao, uint idCarregamento)
        {
            return ObtemValorCampo<string>(sessao ,"placa", "idCarregamento=" + idCarregamento);
        }

        #endregion

        #region Verifica se um carregamento foi todo carregado

        /// <summary>
        /// Verifica se um carregamento foi completamente carregado
        /// </summary>
        public void AtualizaCarregamentoCarregado(GDASession sessao, uint idCarregamento, string etiqueta)
        {
            var carregamentoAtual = GetElementByPrimaryKey(sessao, idCarregamento);
            var situacao = 0;

            var sql = string.Format(@"SELECT COUNT(*)
                FROM item_carregamento ic
                WHERE (ic.Carregado IS NULL OR ic.Carregado = 0) AND ic.IdCarregamento={0}", idCarregamento);

            if (objPersistence.ExecuteSqlQueryCount(sessao, sql) == 0)
                situacao = (int)Carregamento.SituacaoCarregamentoEnum.Carregado;
            else
                situacao = (int)Carregamento.SituacaoCarregamentoEnum.PendenteCarregamento;

            objPersistence.ExecuteCommand(sessao, string.Format("UPDATE carregamento SET Situacao={0} WHERE IdCarregamento={1}", situacao, idCarregamento));
            
            foreach (var idOC in OrdemCargaDAO.Instance.GetIdsOCsByCarregamento(sessao, idCarregamento))
                OrdemCargaDAO.Instance.VerificaOCCarregada(sessao, idCarregamento, idOC, etiqueta);

            var carregamentoNovo = GetElementByPrimaryKey(sessao, idCarregamento);

            LogAlteracaoDAO.Instance.LogCarregamento(sessao, carregamentoAtual, carregamentoNovo);
        }

        #endregion

        #region Verifica se o carregamento tem alguma peça carregada

        /// <summary>
        /// Verifica se o carregamento tem alguma peça carregada
        /// </summary>
        public bool CarregamentoTemItemCarregado(GDASession session, uint idCarregamento)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM item_carregamento ic
                WHERE ic.Carregado = TRUE AND ic.idCarregamento=" + idCarregamento;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Obtem as Cidades do carregamento

        public IList<CidadesCarregamento> ObtemCidadesCarregamento(List<OrdemCarga> ocs)
        {
            var cidadesCarregamento = new List<CidadesCarregamento>();

            foreach (var oc in ocs.Where(f => ClienteDAO.Instance.ClienteImportacao(f.IdCliente)))
            {
                var cidadesExternas = oc.Pedidos.Where(f => !string.IsNullOrEmpty(f.CidadeClienteExterno) && !string.IsNullOrEmpty(f.UfClienteExterno))
                    .GroupBy(f => (f.CidadeClienteExterno + " - " + f.UfClienteExterno))
                    .Select(f => new { NomeCidade = f.Key, QtdeClientes = f.Count() });

                foreach (var c in cidadesExternas)
                {
                    if (cidadesCarregamento.Where(x => x.NomeCidade == c.NomeCidade).Count() == 0)
                        cidadesCarregamento.Add(new CidadesCarregamento() { NomeCidade = c.NomeCidade, QtdeClientes = c.QtdeClientes });
                    else
                        cidadesCarregamento.ForEach(x => x.QtdeClientes += (x.NomeCidade == c.NomeCidade ? c.QtdeClientes : 0));
                }
            }

            var cidades = ocs.Where(f => !ClienteDAO.Instance.ClienteImportacao(f.IdCliente))
                .GroupBy(f => f.CidadeCliente)
                .Select(f => new { NomeCidade = f.Key, QtdeClientes = f.Count() });

            foreach (var c in cidades)
            {
                if (cidadesCarregamento.Where(x => x.NomeCidade == c.NomeCidade).Count() == 0)
                    cidadesCarregamento.Add(new CidadesCarregamento() { NomeCidade = c.NomeCidade, QtdeClientes = c.QtdeClientes });
                else

                    cidadesCarregamento.ForEach(x => x.QtdeClientes += (x.NomeCidade == c.NomeCidade ? c.QtdeClientes : 0));
            }

            return cidadesCarregamento.OrderBy(f => f.NomeCidade).ToList();
        }

        #endregion

        #region Métodos Sobrescritos

        /// <summary>
        /// Atualiza um carregamento
        /// </summary>
        /// <param name="objUpdate"></param>
        /// <returns></returns>
        public override int Update(Carregamento objUpdate)
        {
            var carregamento = GetElementByPrimaryKey(objUpdate.IdCarregamento);

            objUpdate.Situacao = carregamento.Situacao;
            objUpdate.IdLoja = carregamento.IdLoja;

            LogAlteracaoDAO.Instance.LogCarregamento(null, carregamento, objUpdate);

            return base.Update(objUpdate);
        }

        #endregion

        #region Faturamento       

        /// <summary>
        /// Busca os pedidos pendentes para finalização do faturamento
        /// </summary>
        public string BuscarPendenciasFaturamento(uint idCarregamento)
        {
            var idsPedido = PedidoDAO.Instance.GetIdsPedidosByCarregamento(null, idCarregamento);
            var retorno = string.Empty;

            var sql = string.Format(@"
                SELECT p.IdPedido 
                FROM pedido p 
                WHERE p.IdPedido IN ({0}) AND p.Situacao NOT IN ({1})", string.Join(", ", idsPedido), (int)Pedido.SituacaoPedido.Confirmado);

            var pedidosNaoLiberados = ExecuteMultipleScalar<uint>(sql);
            if(pedidosNaoLiberados.Any())
                retorno += string.Format("Os pedidos ({0}) não estão liberados.", string.Join(", ", pedidosNaoLiberados));

            sql = string.Format(@"
                SELECT pnf.IdPedido 
                FROM pedidos_nota_fiscal pnf
                LEFT JOIN nota_fiscal nf ON (pnf.IdNf = nf.IdNf)
                WHERE pnf.IdPedido IN ({0}) AND nf.Situacao IN ({1})",
                string.Join(", ", idsPedido), ((int)NotaFiscal.SituacaoEnum.Autorizada + "," + (int)NotaFiscal.SituacaoEnum.ProcessoEmissao));

            foreach (var item in ExecuteMultipleScalar<uint>(sql))
                idsPedido.Remove(item);
            
            if(idsPedido.Any())
                retorno += string.Format("Os pedidos ({0}) não possuem notas fiscais autorizadas.", string.Join(", ", idsPedido));

            return retorno;
        }

        /// <summary>
        /// Altera a situação do faturamento dos carregamentos dos pedidos passados
        /// </summary>
        public void AlterarSituacaoFaturamentoCarregamentos(GDASession sessao, IEnumerable<uint> idsPedido)
        {            
            string sql =
                string.Format(@"
                    SELECT *
                    FROM carregamento c
                    INNER JOIN loja l ON (c.idLoja = l.idLoja)
                    LEFT JOIN funcionario f ON (c.idMotorista = f.idFunc)
                    LEFT JOIN veiculo v ON (c.placa = v.placa)
                    WHERE EXISTS (
                        SELECT *
                        FROM pedido_ordem_carga poc
                        INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.idOrdemCarga)
                        WHERE oc.idCarregamento = c.idCarregamento AND poc.idPedido IN (" + string.Join(",", idsPedido) + @"))
                    ORDER BY c.idCarregamento DESC");

            var carregamentos = LoadDataWithSortExpression(sessao, sql, string.Empty, 0, 10, null);

            foreach (var carregamento in carregamentos)
            {
                var idsPedidoCarregamento = PedidoDAO.Instance.GetIdsPedidosByCarregamento(sessao, carregamento.IdCarregamento);

                if (!idsPedidoCarregamento.Any())
                    continue;

                sql = string.Format(@"SELECT IF((Situacao={2} AND NotaAutorizada), true, false) AS Faturado FROM (
                SELECT p.Situacao, (COUNT(nf.IdNf) > 0) AS NotaAutorizada
                FROM pedido p
                LEFT JOIN pedidos_nota_fiscal pnf ON (p.IdPedido=pnf.IdPedido)
                LEFT JOIN nota_fiscal nf ON (pnf.IdNf=nf.IdNf)
                WHERE p.IdPedido IN ({0}) AND nf.Situacao IN ({1})) tmp",
                    string.Join(", ", idsPedidoCarregamento), ((int)NotaFiscal.SituacaoEnum.Autorizada + "," + (int)NotaFiscal.SituacaoEnum.ProcessoEmissao), (uint)Pedido.SituacaoPedido.Confirmado);

                var retornoPedidos = ExecuteMultipleScalar<bool>(sessao, sql);

                if (!(retornoPedidos.Any(f => f == false)))
                    carregamento.SituacaoFaturamento = SituacaoFaturamentoEnum.Faturado;
                else if (retornoPedidos.Any(f => f == true))
                    carregamento.SituacaoFaturamento = SituacaoFaturamentoEnum.FaturadoParcialmente;
                else 
                    carregamento.SituacaoFaturamento = SituacaoFaturamentoEnum.NaoFaturado;

                Instance.Update(sessao, carregamento);
            }
        }

        /// <summary>
        /// Atualiza as ocs do carregamento que forem parciais ao liberar
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idsProdutosPedido"></param>
        public void AtualizaCarregamentoParcial(GDASession session, uint[] idsProdutosPedido)
        {
            if (!OrdemCargaConfig.UsarOrdemCargaParcial)
                return;

            var idsUsados = new List<uint>();

            //Percorrer as peças liberadas removendo as que não foram liberadas do item carregamento 
            for (var i = 0; i < idsProdutosPedido.Length; i++)
            {
                var idPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(session, idsProdutosPedido[i]);
                if (!PedidoDAO.Instance.ObtemOrdemCargaParcial(session, idPedido))
                    continue;

                ItemCarregamentoDAO.Instance.DeleteByIdProdPed(session, idsProdutosPedido[i]);
                idsUsados.Add(idsProdutosPedido[i]);
            }

            //Marca as ocs como carregada parcialmente
            foreach (var idCarregamento in ItemCarregamentoDAO.Instance.ObterIdsCarregamento(session, idsUsados))
                Instance.AtualizaCarregamentoCarregado(session, idCarregamento, null);
        }

        #endregion
    }
}
