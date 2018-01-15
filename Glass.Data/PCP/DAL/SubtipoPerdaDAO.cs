using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class SubtipoPerdaDAO : BaseDAO<SubtipoPerda, SubtipoPerdaDAO>
    {
        //private SubtipoPerdaDAO() { }

        private string Sql(uint idTipoPerda, bool selecionar)
        {
            StringBuilder sql = new StringBuilder("select ");
            sql.Append(selecionar ? "s.*" : "count(*)");
            sql.Append(@" from subtipo_perda s
                where 1");

            if (idTipoPerda > 0)
                sql.AppendFormat(" and s.idTipoPerda={0}", idTipoPerda);

            return sql.ToString();
        }

        public IList<SubtipoPerda> GetList(uint idTipoPerda, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idTipoPerda) == 0)
                return new SubtipoPerda[] { new SubtipoPerda() };

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "s.descricao";
            return LoadDataWithSortExpression(Sql(idTipoPerda, true), sortExpression, 
                startRow, pageSize, null);
        }

        public int GetCount(uint idTipoPerda)
        {
            int count = GetCountReal(idTipoPerda);
            return count > 0 ? count : 1;
        }

        public int GetCountReal(uint idTipoPerda)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idTipoPerda, false));
        }

        public IList<SubtipoPerda> GetByTipoPerda(uint idTipoPerda)
        {
            return objPersistence.LoadData(Sql(idTipoPerda, true) + " order by s.descricao").ToList();
        }

        public IList<SubtipoPerda> GetListSubtipoPerda()
        {
            return objPersistence.LoadData(Sql(0, true) + " order by s.descricao").ToList();
        }

        public string GetDescricao(uint idSubtipoPerda)
        {
            return GetDescricao(null, idSubtipoPerda);
        }

        public string GetDescricao(GDASession session, uint idSubtipoPerda)
        {
            return ObtemValorCampo<string>(session, "descricao", "idSubtipoPerda=" + idSubtipoPerda);
        }

        public override int Update(SubtipoPerda objUpdate)
        {
            LogAlteracaoDAO.Instance.LogSubtipoPerda(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(SubtipoPerda objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdSubtipoPerda);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            LogAlteracaoDAO.Instance.ApagaLogSubtipoPerda(Key);
            return GDAOperations.Delete(new SubtipoPerda { IdSubtipoPerda = (int)Key });
        }
    }
}
