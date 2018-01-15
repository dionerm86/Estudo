using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class DescontoQtdeDAO : BaseDAO<DescontoQtde, DescontoQtdeDAO>
    {
        //private DescontoQtdeDAO() { }

        private string Sql(uint idProd, bool selecionar)
        {
            string campos = selecionar ? "*" : "count(*)";
            string sql = "select " + campos + " from desconto_qtde where 1";

            if (idProd > 0)
                sql += " and idProd=" + idProd;

            sql += " order by qtde desc";
            return sql;
        }

        public IList<DescontoQtde> GetByProd(uint idProd)
        {
            if (GetCountByProd(idProd) == 0)
            {
                var temp = new DescontoQtde[1];
                temp[0] = new DescontoQtde();
                temp[0].IdProd = idProd;
                
                return temp;
            }

            return objPersistence.LoadData(Sql(idProd, true)).ToList();
        }

        public int GetCountByProd(uint idProd)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idProd, false));
        }

        public float GetPercDescontoByProd(uint idProd, int qtde)
        {
            if (UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Administrador)
                return 100;

            var desc = GetByProd(idProd);

            foreach (DescontoQtde d in desc)
                if (qtde >= d.Qtde)
                    return d.PercDescontoMax;

            return 0;
        }
    }
}
