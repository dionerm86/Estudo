using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class TurnoDAO : BaseDAO<Turno, TurnoDAO>
    {
        //private TurnoDAO() { }

        #region Listagem padrão

        private string Sql(uint idTurno, string descricao, bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From turnos Where 1 ";

            if (idTurno > 0)
                sql += " And IdTurno=" + idTurno;

            if (!string.IsNullOrEmpty(descricao))
                sql += " And Descricao Like '%" + descricao + "%'"; 

            return sql;
        }

        public Turno GetElement(uint idTurno)
        {
            return objPersistence.LoadOneData(Sql(idTurno, null, true));
        }

        public IList<Turno> GetList(string sortExpression, int startRow, int pageSize)
        {
            return objPersistence.LoadDataWithSortExpression(Sql(0, null, true), new InfoSortExpression(sortExpression), new InfoPaging(startRow, pageSize), null).ToList();
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0,null, false), null);
        }

        public List<Turno> GetList()
        {
            List<Turno> turnos = objPersistence.LoadData(Sql(0, null, true));

            turnos.Sort(delegate(Turno t1, Turno t2) { return t1.IdTurno.CompareTo(t2.IdTurno); });

            return turnos;
        }

        #endregion
    }
}
