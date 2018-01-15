using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class BoletoImpressoDAO : BaseCadastroDAO<BoletoImpresso, BoletoImpressoDAO>
    {
        //private BoletoImpressoDAO() { }

        public bool BoletoFoiImpresso(int? idContaR, int? idNf)
        {
            var sql = "SELECT COUNT(*) FROM boleto_impresso WHERE 1";

            if (idContaR > 0)
                sql += string.Format(" AND IdContaR={0}", idContaR);

            if (idNf > 0)
                sql += string.Format(" AND IdNf={0}", idNf);

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }
    }
}
