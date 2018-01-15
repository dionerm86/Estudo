using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public class InfoValorAgregadoDAO : BaseDAO<InfoValorAgregado, InfoValorAgregadoDAO>
    {
        //private InfoValorAgregadoDAO() { }

        private string Sql(bool selecionar)
        {
            string campos = selecionar ? @"i.*, concat(c.nomeCidade, ' / ', c.nomeUf) as nomeCidade,
                p.codInterno, p.descricao as descrProduto, c.codIbgeUf, c.codIbgeCidade" : "count(*)";

            return "select " + campos + @" from informacao_valor_agregado i
                left join cidade c on (i.idCidade=c.idCidade)
                left join produto p on (i.idProd=p.idProd)";
        }

        public IList<InfoValorAgregado> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
                return new InfoValorAgregado[] { new InfoValorAgregado() };

            return LoadDataWithSortExpression(Sql(true), sortExpression, startRow, pageSize);
        }

        public int GetCount()
        {
            int count = GetCountReal();
            return count > 0 ? count : 1;
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(false));
        }

        public IList<InfoValorAgregado> GetForEFD(DateTime inicio, DateTime termino)
        {
            string sql = Sql(true) + " where i.data >= ?ini and i.data <=?fim";
            return objPersistence.LoadData(sql, new GDAParameter("?ini", inicio),
                new GDAParameter("?fim", termino)).ToList();
        }
    }
}
