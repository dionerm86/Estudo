using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class TipoBoletoDAO : BaseDAO<TipoBoleto, TipoBoletoDAO>
    {
        //private TipoBoletoDAO() { }

        public IList<TipoBoleto> GetOrdered()
        {
            return objPersistence.LoadData("select * from tipo_boleto order by Descricao asc").ToList();
        }
    }
}
