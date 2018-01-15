using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class AmbienteOrcamentoDAO : BaseDAO<AmbienteOrcamento, AmbienteOrcamentoDAO>
    {
        //private AmbienteOrcamentoDAO() { }

        private string SqlList(uint idOrca, bool selecionar)
        {
            string campos = selecionar ? "*, (select cast(coalesce(sum(coalesce(total, 0)), 0) as decimal(12,2)) from produtos_orcamento where " +
                "idAmbienteOrca=ao.idAmbienteOrca and idProdParent is null) as ValorProdutos" : "count(*)";

            return "select " + campos + " from ambiente_orcamento ao where idOrcamento=" + idOrca;
        }

        public IList<AmbienteOrcamento> GetByOrcamento(uint idOrca)
        {
            return GetByOrcamento(null, idOrca);
        }

        public IList<AmbienteOrcamento> GetByOrcamento(GDASession session, uint idOrca)
        {
            return objPersistence.LoadData(session, SqlList(idOrca, true) + " Order By idAmbienteOrca").ToList();
        }

        public IList<AmbienteOrcamento> GetList(uint idOrca, string sortExpression, int startRow, int pageSize)
        {
            string sort = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "idAmbienteOrca";

            var retorno = LoadDataWithSortExpression(SqlList(idOrca, true), sort, startRow, pageSize, null);

            if (retorno == null || retorno.Count == 0)
            {
                retorno = new AmbienteOrcamento[1];
                retorno[0] = new AmbienteOrcamento();
            }
            
            return retorno;
        }

        public int GetListCount(uint idOrca)
        {
            int retorno = objPersistence.ExecuteSqlQueryCount(SqlList(idOrca, false));
            return (retorno == 0) ? 1 : retorno;
        }

        public bool PossuiProdutos(uint idAmbienteOrcamento)
        {
            string sql = "select count(*) from produtos_orcamento where idAmbienteOrca=" + idAmbienteOrcamento;
            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Retorna o id do ambiente de um orçamento relacionado a um item de projeto.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public uint? GetIdByItemProjeto(uint idItemProjeto)
        {
            string sql = "select idAmbienteOrca from produtos_orcamento where idItemProjeto=" + idItemProjeto;
            return ExecuteScalar<uint?>(sql);
        }

        /// <summary>
        /// Verifica se o idAmbienteOrca existe no banco.
        /// </summary>
        public bool AmbienteOrcamentoExiste(GDASession session, uint idAmbienteOrca)
        {
            var idAmbOrca = ObtemValorCampo<uint>(session, "idAmbienteOrca", "idAmbienteOrca=" + idAmbienteOrca);
            return idAmbOrca == idAmbienteOrca;
        }

        #region Obtém dados do ambiente

        public string ObtemAmbiente(uint idAmbienteOrca)
        {
            return ObtemValorCampo<string>("ambiente", "idAmbienteOrca=" + idAmbienteOrca);
        }

        #endregion

        #region Métodos sobrescritos

        public override int Delete(AmbienteOrcamento objDelete)
        {
            if (PossuiProdutos(objDelete.IdAmbienteOrca))
                throw new Exception("Esse ambiente possui produtos. Exclua-os antes de excluir o ambiente.");

            return base.Delete(objDelete);
        }

        public override int Update(AmbienteOrcamento objUpdate)
        {
            int retorno = base.Update(objUpdate);
            objPersistence.ExecuteCommand("update produtos_orcamento set Ambiente=?ambiente where idAmbienteOrca=" + objUpdate.IdAmbienteOrca, 
                new GDAParameter("?ambiente", objUpdate.Ambiente));

            return retorno;
        }

        #endregion
    }
}
