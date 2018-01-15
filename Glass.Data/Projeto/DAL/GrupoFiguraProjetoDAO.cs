using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class GrupoFiguraProjetoDAO : BaseDAO<GrupoFiguraProjeto, GrupoFiguraProjetoDAO>
    {
        //private GrupoFiguraProjetoDAO() { }

        private string SqlList(bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From grupo_figura_projeto";

            return sql;
        }
        
        public IList<GrupoFiguraProjeto> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
                return new GrupoFiguraProjeto[] { new GrupoFiguraProjeto() };

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

        public IList<GrupoFiguraProjeto> GetOrdered()
        {
            return objPersistence.LoadData("Select * From grupo_figura_projeto Where situacao=" + (int)GrupoFiguraProjeto.SituacaoGrupo.Ativo + " Order By Descricao").ToList();
        }

        public void AlterarSituacao(uint idGrupoFigProj, GrupoFiguraProjeto.SituacaoGrupo situacao)
        {
            objPersistence.ExecuteCommand("update grupo_figura_projeto set situacao=" + (int)situacao + " where idGrupoFigProj=" + idGrupoFigProj);
        }

        #region Métodos sobrescritos

        public override int Update(GrupoFiguraProjeto objUpdate)
        {
            return base.Update(objUpdate);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Verifica se este grupo está sendo usado em alguma figura de projeto
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From figura_projeto Where idGrupoFigProj=" + Key) > 0)
                throw new Exception("Este grupo não pode ser excluído pois existem projetos cadastrados no mesmo.");

            return GDAOperations.Delete(new GrupoFiguraProjeto { IdGrupoFigProj = Key });
        }

        public override int Delete(GrupoFiguraProjeto objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdGrupoFigProj);
        }

        #endregion
    }
}
