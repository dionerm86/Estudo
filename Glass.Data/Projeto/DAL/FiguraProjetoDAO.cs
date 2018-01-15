using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.IO;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class FiguraProjetoDAO : BaseDAO<FiguraProjeto, FiguraProjetoDAO>
    {
        //private FiguraProjetoDAO() { }

        private string SqlList(uint idGrupoFigProj, int situacao, bool selecionar)
        {
            string campos = selecionar ? "f.*, g.Descricao as DescrGrupoFigura" : "Count(*)";

            string sql = "Select " + campos + " From figura_projeto f " +
                "Inner Join grupo_figura_projeto g On (f.idGrupoFigProj=g.idGrupoFigProj) Where 1";

            if (idGrupoFigProj > 0)
                sql += " And g.idGrupoFigProj=" + idGrupoFigProj;

            if (situacao > 0)
                sql += " And f.situacao=" + situacao;

            return sql;
        }

        public IList<FiguraProjeto> GetList(uint idGrupoFigProj, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idGrupoFigProj) == 0)
                return new FiguraProjeto[] { new FiguraProjeto() };

            sortExpression = String.IsNullOrEmpty(sortExpression) ? "CodInterno" : sortExpression;

            return LoadDataWithSortExpression(SqlList(idGrupoFigProj, 0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCountReal(uint idGrupoFigProj)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(idGrupoFigProj, 0, false), null);
        }

        public int GetCount(uint idGrupoFigProj)
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(idGrupoFigProj, 0, false), null);

            return count == 0 ? 1 : count;
        }

        public IList<FiguraProjeto> GetOrdered(uint idGrupoFigProj)
        {
            return objPersistence.LoadData(SqlList(idGrupoFigProj, 1, true)).ToList();
        }

        public uint? ObtemIdFiguraProjeto(string descrGrupoFigura, string codInterno)
        {
            string sql = @"select idFiguraProjeto from figura_projeto fp 
                inner join grupo_figura_projeto gfp on (fp.idGrupoFigProj=gfp.idGrupoFigProj)
                where fp.codInterno=?cod and gfp.descricao=?descr";

            return ExecuteScalar<uint?>(sql, new GDAParameter("?cod", codInterno), new GDAParameter("?descr", descrGrupoFigura));
        }

        public string ObtemCodInterno(uint idFiguraProjeto)
        {
            return ObtemValorCampo<string>("codInterno", "idFiguraProjeto=" + idFiguraProjeto);
        }

        #region Métodos sobrescritos

        public override int Update(FiguraProjeto objUpdate)
        {
            return base.Update(objUpdate);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Verifica se este grupo está sendo usado em alguma figura de projeto
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From figura_peca_item_projeto Where idFiguraProjeto=" + Key) > 0)
                throw new Exception("Esta figura não pode ser excluída pois existem projetos utilizando-a.");

            string fotoPath = Utils.GetFigurasProjetoPath + Key + ".jpg";

            if (File.Exists(fotoPath))
                File.Delete(fotoPath);

            return GDAOperations.Delete(new FiguraProjeto { IdFiguraProjeto = Key });
        }

        public override int Delete(FiguraProjeto objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdFiguraProjeto);
        }

        #endregion
    }
}
