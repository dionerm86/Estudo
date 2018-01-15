using GDA;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class UsoRetalhoProducaoDAO : BaseDAO<UsoRetalhoProducao, UsoRetalhoProducaoDAO>
    {
        /// <summary>
        /// Cria a associação de uma peça ao retalho
        /// </summary>
        /// <param name="idRetalhoProducao"></param>
        /// <param name="idProdPedProducao"></param>
        /// <param name="vinculadoImpressao"></param>
        public void AssociarRetalho(uint idRetalhoProducao, uint idProdPedProducao, bool vinculadoImpressao)
        {
            AssociarRetalho(null, idRetalhoProducao, idProdPedProducao, vinculadoImpressao);
        }

        /// <summary>
        /// Cria a associação de uma peça ao retalho
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idRetalhoProducao"></param>
        /// <param name="idProdPedProducao"></param>
        /// <param name="vinculadoImpressao"></param>
        public void AssociarRetalho(GDASession sessao, uint idRetalhoProducao, uint idProdPedProducao, bool vinculadoImpressao)
        {
            Insert(sessao, new UsoRetalhoProducao()
            {
                IdRetalhoProducao = idRetalhoProducao,
                IdProdPedProducao = idProdPedProducao,
                VinculadoImpressao = vinculadoImpressao
            });
        }

        /// <summary>
        /// Remove a associação de uma peça ao retalho
        /// </summary>
        /// <param name="idRetalhoProducao"></param>
        /// <param name="idProdPedProducao"></param>
        public void RemoverAssociacao(GDASession session, uint idRetalhoProducao, uint idProdPedProducao)
        {
            if (idRetalhoProducao == 0 && idProdPedProducao == 0)
                return;

            var sql = @"
                DELETE from uso_retalho_producao 
                where 1";

            if (idRetalhoProducao > 0)
                sql += " AND IdRetalhoProducao = " + idRetalhoProducao;

            if (idProdPedProducao > 0)
                sql += " AND IdProdPedProducao = " + idProdPedProducao;

            objPersistence.ExecuteCommand(session, sql);

            if (!PossuiAssociacao(session, idRetalhoProducao, 0))
                RetalhoProducaoDAO.Instance.AlteraSituacao(session, idRetalhoProducao, RetalhoProducao.SituacaoRetalho.Disponivel);
        }

        /// <summary>
        /// Cancela a associação de um peça ao retalho
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        public void CancelarAssociacao(GDASession sessao, uint idProdPedProducao)
        {
            objPersistence.ExecuteCommand(sessao, @"
                UPDATE uso_retalho_producao
                    SET cancelado = 1
                WHERE idProdPedProducao = " + idProdPedProducao);
        }

        /// <summary>
        /// Verifica se o retalho informado possui alguma associação.
        /// </summary>
        /// <param name="idRetalhoProducao"></param>
        /// <returns></returns>
        public bool PossuiAssociacao(uint idRetalhoProducao)
        {
            return PossuiAssociacao(idRetalhoProducao, 0);
        }

        /// <summary>
        /// Verifica se uma peça esta associada a um retalho
        /// </summary>
        /// <param name="idRetalhoProducao"></param>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public bool PossuiAssociacao(uint idRetalhoProducao, uint idProdPedProducao)
        {
            return PossuiAssociacao(null, idRetalhoProducao, idProdPedProducao);
        }

        /// <summary>
        /// Verifica se uma peça esta associada a um retalho
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idRetalhoProducao"></param>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public bool PossuiAssociacao(GDASession sessao, uint idRetalhoProducao, uint idProdPedProducao)
        {
            var sql = @"
                SELECT COUNT(*) 
                FROM uso_retalho_producao 
                where 1";

            if (idRetalhoProducao > 0)
                sql += " AND IdRetalhoProducao = " + idRetalhoProducao;

            if (idProdPedProducao > 0)
                sql += " AND IdProdPedProducao = " + idProdPedProducao;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Obtem o retalho vinculado a uma peça
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public uint? ObtemIdRetalhoProducao(uint idProdPedProducao)
        {
            return ObtemIdRetalhoProducao(null, idProdPedProducao);
        }

        /// <summary>
        /// Obtem o retalho vinculado a uma peça
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public uint? ObtemIdRetalhoProducao(GDASession sessao, uint idProdPedProducao)
        {
            return ObtemValorCampo<uint?>(sessao, "idRetalhoProducao", "COALESCE(cancelado, 0) = 0 and idProdPedProducao=" + idProdPedProducao);
        }

        /// <summary>
        /// Obtem as peças vinculadas a um retalho
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idRetalhoProducao"></param>
        /// <returns></returns>
        public List<uint> ObtemIdsProdPedProducao(GDASession session, uint idRetalhoProducao)
        {
            return ExecuteMultipleScalar<uint>(session, "SELECT IdProdPedProducao FROM uso_retalho_producao WHERE IdRetalhoProducao= " + idRetalhoProducao);
        }

        /// <summary>
        /// Recupera a associação de uma peça
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public UsoRetalhoProducao ObtemAssociacao(GDASession sessao, uint idProdPedProducao)
        {
            return objPersistence.LoadOneData(sessao, "SELECT * FROM uso_retalho_producao WHERE IdProdPedProducao=" + idProdPedProducao);
        }
    }
}
