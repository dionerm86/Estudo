using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class GrupoMedidaProjetoDAO : BaseDAO<GrupoMedidaProjeto, GrupoMedidaProjetoDAO>
    {
        //private GrupoMedidaProjetoDAO() { }

        /// <summary>
        /// Obtem todos os grupos de medida de projeto ordenados pela descrição.
        /// </summary>
        public GrupoMedidaProjeto[] ObtemOrdenado()
        {
            return objPersistence.LoadData("SELECT gmp.* FROM grupo_medida_projeto gmp ORDER BY gmp.descricao").ToList().ToArray();
        }

        #region Busca Padrão

        private string SqlList(bool selecionar)
        {
            string campos = selecionar ? "gmp.*" : "Count(*)";

            string sql = "Select " + campos + " From grupo_medida_projeto gmp ";

            return sql;
        }

        public IList<GrupoMedidaProjeto> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
                return new GrupoMedidaProjeto[] { new GrupoMedidaProjeto() };

            sortExpression = String.IsNullOrEmpty(sortExpression) ? "gmp.Descricao" : sortExpression;

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

        #endregion

        public uint? FindByDescricao(uint idGrupoMedidaProjeto, string descricao)
        {
            return FindByDescricao(null, idGrupoMedidaProjeto, descricao);
        }

        public uint? FindByDescricao(GDASession session, uint idGrupoMedidaProjeto, string descricao)
        {
            var p = new GDAParameter("?descricao", descricao);
            var sql = "select count(*) from grupo_medida_projeto where idGrupoMedProj=" + idGrupoMedidaProjeto + " and descricao=?descricao";

            if (idGrupoMedidaProjeto > 0)
                if (objPersistence.ExecuteSqlQueryCount(session, sql, p) > 0)
                    return idGrupoMedidaProjeto;

            sql = "select {0} from grupo_medida_projeto where descricao=?descricao";
            if (objPersistence.ExecuteSqlQueryCount(session, string.Format(sql, "count(*)"), p) > 0)
            {
                object retorno = objPersistence.ExecuteScalar(session, string.Format(sql, "idGrupoMedProj"), p);
                return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (uint?)Glass.Conversoes.StrParaUint(retorno.ToString()) : null;
            }

            return null;
        }

        public string ObtemDescricao(uint idGrupoMedidaProjeto)
        {
            return ObtemValorCampo<string>("descricao", "idGrupoMedProj=" + idGrupoMedidaProjeto);
        }

        #region Métodos sobrescritos

        public override uint Insert(GrupoMedidaProjeto objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, GrupoMedidaProjeto objInsert)
        {
            if (FindByDescricao(session, objInsert.IdGrupoMedProj, objInsert.Descricao) > 0)
                throw new Exception("Já existe um Grupo de Medida de Projeto com a descrição informada.");

            return base.Insert(session, objInsert);
        }

        public override int Update(GrupoMedidaProjeto objUpdate)
        {
            // Verifica se este grupo medida projeto está associado a alguma medida de projeto
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From medida_projeto Where idGrupoMedProj=" + objUpdate.IdGrupoMedProj) > 0)
                throw new Exception("Esse grupo de medidas não pode ser atualizado pois existem medidas associadas a ele.");

            //Verifica se este grupo medida projeto está associado a algum arquivo calcengine variável
            GDAParameter p = new GDAParameter("?variavelsistema", ObtemDescricao(objUpdate.IdGrupoMedProj));
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From arquivo_calcengine_variavel Where variavelsistema=?variavelsistema", p) > 0)
                throw new Exception("Esse grupo de medidas não pode ser atualizado pois existem arquivos calcengine variáveis associadas a ele.");

            LogAlteracaoDAO.Instance.LogGrupoMedidaProjeto(objUpdate);
            return base.Update(objUpdate);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Verifica se este grupo medida projeto está associado a alguma medida de projeto
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From medida_projeto Where idGrupoMedProj=" + Key) > 0)
                throw new Exception("Esse grupo de medidas não pode ser excluído pois existem medidas associadas a ele.");

            //Verifica se este grupo medida projeto está associado a algum arquivo calcengine variável
            GDAParameter p = new GDAParameter("?variavelsistema", ObtemDescricao(Key));
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From arquivo_calcengine_variavel Where variavelsistema=?variavelsistema", p) > 0)
                throw new Exception("Esse grupo de medidas não pode ser excluído pois existem arquivos calcengine variáveis associadas a ele.");

            LogAlteracaoDAO.Instance.ApagaLogGrupoMedidaProjeto(Key);
            return base.DeleteByPrimaryKey(Key);
        }

        public override int Delete(GrupoMedidaProjeto objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdGrupoMedProj);
        }

        #endregion
    }
}