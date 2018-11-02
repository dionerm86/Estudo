using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class GrupoModeloDAO : BaseDAO<GrupoModelo, GrupoModeloDAO>
	{
        //private GrupoModeloDAO() { }

        public List<GrupoModelo> GetOrdered()
        {
            string sql = "Select * From grupo_modelo where situacao=" + (int)GrupoModelo.SituacaoGrupoModelo.Ativo + " Order By case idGrupoModelo " +
                "when " + (int)UtilsProjeto.GrupoModelo.Correr08mm + " then 1 " +
                "when " + (int)UtilsProjeto.GrupoModelo.Correr10mm + " then 2 " +
                "when " + (int)UtilsProjeto.GrupoModelo.CorrerComKit08mm + " then 3 " +
                "when " + (int)UtilsProjeto.GrupoModelo.CorrerComKit10mm + " then 4 " +
                "else 99 end, descricao";

            return objPersistence.LoadData(sql);
        }

        private string SqlList(bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From grupo_modelo ";

            return sql;
        }

        public IList<GrupoModelo> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
                return new GrupoModelo[] { new GrupoModelo() };

            sortExpression = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList(true), sortExpression, startRow, pageSize, null);
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(false), null);
        }

        public int GetCount()
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(false), null);

            return count == 0 ? 1 : count;
        }

        public int GetCountProjetosModelos(uint idGrupoModelo)
        {
            string sql = "select count(*) from projeto_modelo where idGrupoModelo=" + idGrupoModelo;
            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        public void AlterarSituacao(uint idGrupoModelo, GrupoModelo.SituacaoGrupoModelo situacao)
        {
            objPersistence.ExecuteCommand("update grupo_modelo set situacao=" + (int)situacao + " where idGrupoModelo=" + idGrupoModelo);
        }

        public uint? FindByDescricao(GDASession session, uint idGrupoModelo, string descricao)
        {
            var p = new GDAParameter("?descricao", descricao);
            var sql = "select count(*) from grupo_modelo where idGrupoModelo=" + idGrupoModelo + " and descricao=?descricao";

            if (objPersistence.ExecuteSqlQueryCount(session, sql, p) > 0)
                return idGrupoModelo;

            sql = "select {0} from grupo_modelo where descricao=?descricao";
            if (objPersistence.ExecuteSqlQueryCount(session, string.Format(sql, "count(*)"), p) > 0)
            {
                object retorno = objPersistence.ExecuteScalar(session, string.Format(sql, "idGrupoModelo"), p);
                return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (uint?)Glass.Conversoes.StrParaUint(retorno.ToString()) : null;
            }

            return null;
        }

        public string ObtemDescricao(uint idGrupoModelo)
        {
            return ObtemDescricao(null, idGrupoModelo);
        }

        public string ObtemDescricao(GDASession session, uint idGrupoModelo)
        {
            return ObtemValorCampo<string>(session, "descricao", "idGrupoModelo=" + idGrupoModelo);
        }

        #region Métodos sobrescritos

        public override int Update(GrupoModelo objUpdate)
        {
            LogAlteracaoDAO.Instance.LogGrupoModelo(objUpdate);
            return base.Update(objUpdate);
        }

        public override int DeleteByPrimaryKey(GDASession sessao, int key)
        {
            if (ExecuteScalar<bool>(sessao, "Select Count(*)>0 From projeto_modelo Where idGrupoModelo=" + key))
            {
                throw new InvalidOperationException("Este grupo não pode ser excluído pois existem projetos cadastrados no mesmo.");
            }

            return base.DeleteByPrimaryKey(sessao, key);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return DeleteByPrimaryKey(null, (int)Key);
        }

        public override int Delete(GrupoModelo objDelete)
        {
            return DeleteByPrimaryKey(null, (int)objDelete.IdGrupoModelo);
        }

        #endregion
    }
}