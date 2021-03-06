﻿using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PedidoExportacaoDAO : BaseDAO<PedidoExportacao, PedidoExportacaoDAO>
    {
        //private PedidoExportacaoDAO() { }

        #region Listagem Padrão

        private string Sql(uint idExportacao, int situacao, bool selecionar, out bool temFiltro,
            out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = "";

            string campos = selecionar ? "pe.*, " + ClienteDAO.Instance.GetNomeCliente("c") + " as Cliente, p.Total" : "count(*)";

            string sql = "Select " + campos + @"
                from pedido_exportacao pe
                    inner join pedido p on (pe.idPedido=p.idPedido)
                    left join cliente c on (c.Id_Cli=p.IdCli)
                Where 1 " + FILTRO_ADICIONAL;

            if (idExportacao > 0)
                filtroAdicional += " And pe.IdExportacao=" + idExportacao;

            if (situacao > 0)
                filtroAdicional += " And pe.SituacaoExportacao=" + situacao;

            return sql;
        }

        public IList<PedidoExportacao> GetList(uint idExportacao, int situacao, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression: " pe.DataSituacao Desc";

            bool temFiltro;
            string filtroAdicional;

            return LoadDataWithSortExpression(Sql(idExportacao, situacao, true, out temFiltro, out filtroAdicional), sortExpression, startRow, pageSize, temFiltro, filtroAdicional);
        }

        public int GetCount(uint idExportacao, int situacao, string dataInicial, string dataFinal)
        {
            bool temFiltro;
            string filtroAdicional;

            return GetCountWithInfoPaging(Sql(idExportacao, situacao, true, out temFiltro, out filtroAdicional), temFiltro, filtroAdicional);
        }

        public PedidoExportacao[] GetForRpt(uint idExportacao, int situacao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(idExportacao, situacao, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional);
            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        #endregion

        #region Métodos de consulta de dados

        internal string SqlSituacaoExportacao(string idPedido)
        {
            return @"select situacaoExportacao from pedido_exportacao where 
                idPedido=" + idPedido + " order by dataSituacao desc limit 1";
        }

        /// <summary>
        /// Consulta a situação da exportação de um pedido.
        /// </summary>
        public PedidoExportacao.SituacaoExportacaoEnum GetSituacaoExportacao(uint idPedido)
        {
            return GetSituacaoExportacao(null, idPedido);
        }

        /// <summary>
        /// Consulta a situação da exportação de um pedido.
        /// </summary>
        public PedidoExportacao.SituacaoExportacaoEnum GetSituacaoExportacao(GDASession session, uint idPedido)
        {
            return ExecuteScalar<PedidoExportacao.SituacaoExportacaoEnum>(session, SqlSituacaoExportacao(idPedido.ToString()));
        }

        /// <summary>
        /// Recupera o ID da última exportação de um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint ObtemIdExportacao(uint idPedido)
        {
            var sql = @"select idExportacao from pedido_exportacao where idPedido={0}
                order by dataSituacao desc limit 1";

            return ExecuteScalar<uint>(String.Format(sql, idPedido));
        }

        /// <summary>
        /// Verifica se o pedido foi exportado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoExportado(uint idPedido)
        {
            string sql = @"select Count(*) from pedido_exportacao where idPedido={0}";

            return objPersistence.ExecuteSqlQueryCount(String.Format(sql, idPedido)) > 0;
        }

        /// <summary>
        /// Recupera os pedidos da exportação passada.
        /// </summary>
        /// <returns></returns>
        public int[] PesquisarPedidosExportacao(uint idExportacao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    return this.PesquisarPedidosExportacao(sessao, idExportacao);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Recupera os pedidos da exportação passada
        /// </summary>
        public int[] PesquisarPedidosExportacao(GDASession sessao, uint idExportacao)
        {
            var sql = "SELECT IdPedido FROM pedido_exportacao WHERE IdExportacao=" + idExportacao;

            return ExecuteMultipleScalar<int>(sessao, sql).ToArray();
        }

        //Verifica se o pedido passado possui alguma exportação que não esteja cancelada
        public bool VerificarPossuiExportacao(int idPedido)
        {
            string sql = @"select Count(*) from pedido_exportacao where idPedido={0} AND SituacaoExportacao <> {1}";

            return objPersistence.ExecuteSqlQueryCount(String.Format(sql, idPedido, (int)PedidoExportacao.SituacaoExportacaoEnum.Cancelado)) > 0;
        }

        #endregion

        #region Verifica se um pedido pode ser exportado

        /// <summary>
        /// Verifica se um pedido pode ser exportado.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PodeExportar(uint idPedido)
        {
            if (objPersistence.ExecuteSqlQueryCount("select count(*) from pedido_exportacao where idPedido=" + idPedido) == 0)
                return true;

            var situacaoPedidoExportacao = GetSituacaoExportacao(idPedido);
            return situacaoPedidoExportacao == PedidoExportacao.SituacaoExportacaoEnum.Cancelado || situacaoPedidoExportacao == PedidoExportacao.SituacaoExportacaoEnum.Exportando;
        }

        #endregion

        #region Atualizações

        public void AtualizarSituacao(GDASession session, uint idPedido, int situacao)
        {
            var idExportacao = ObtemIdExportacao(idPedido);

            var sql = string.Format("UPDATE pedido_exportacao SET SituacaoExportacao={0} WHERE IdPedido={1} AND IdExportacao={2}", situacao, idPedido, idExportacao);

            objPersistence.ExecuteCommand(session, sql);
        }

        public void InserirSituacaoExportado(GDASession session, uint idPedido, int situacao)
        {
            if (situacao == (int)PedidoExportacao.SituacaoExportacaoEnum.Exportado)
                throw new Exception("Use o outro método, que contém o IdExportacao");

            InserirSituacaoExportado(session, ObtemIdExportacao(idPedido), idPedido, situacao);
        }

        public void InserirSituacaoExportado(GDASession session, uint idExportacao, uint idPedido, int situacao)
        {
            PedidoExportacao model = new PedidoExportacao();
            model.IdExportacao = idExportacao;
            model.IdPedido = idPedido;
            model.SituacaoExportacao = situacao;
            model.DataSituacao = DateTime.Now;

            PedidoExportacaoDAO.Instance.Insert(session, model);
        }

        #endregion
    }
}
