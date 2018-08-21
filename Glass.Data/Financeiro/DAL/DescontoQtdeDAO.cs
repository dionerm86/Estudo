using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;
using System.Collections.Generic;

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
            return this.GetByProd(null, idProd);
        }

        public IList<DescontoQtde> GetByProd(GDASession sessao, uint idProd)
        {
            if (this.GetCountByProd(sessao, idProd) == 0)
            {
                var temp = new DescontoQtde[1];
                temp[0] = new DescontoQtde();
                temp[0].IdProd = idProd;

                return temp;
            }

            return this.objPersistence.LoadData(sessao, this.Sql(idProd, true)).ToList();
        }

        public int GetCountByProd(uint idProd)
        {
            return this.GetCountByProd(null, idProd);
        }

        public int GetCountByProd(GDASession sessao, uint idProd)
        {
            return this.objPersistence.ExecuteSqlQueryCount(sessao, Sql(idProd, false));
        }

        public float GetPercDescontoByProd(uint idProd, int qtde)
        {
            return this.GetPercDescontoByProd(null, idProd, qtde);
        }

        public float GetPercDescontoByProd(GDASession sessao, uint idProd, int qtde)
        {
            if (UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Administrador)
            {
                return 100;
            }

            var desc = this.GetByProd(sessao, idProd);

            foreach (DescontoQtde d in desc)
            {
                if (qtde >= d.Qtde)
                {
                    return d.PercDescontoMax;
                }
            }

            return 0;
        }
    }
}
