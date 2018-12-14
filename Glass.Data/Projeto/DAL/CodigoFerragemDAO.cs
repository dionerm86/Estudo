using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    class CodigoFerragemDAO : BaseDAO<CodigoFerragem, CodigoFerragemDAO>
    {
        public List<CodigoFerragem> ObterCodigoFerragens(int idFerragem)
        {
            string sql = @"
                SELECT cf.*
	            FROM codigo_ferragem cf
	                INNER JOIN ferragem f ON (cf.IdFerragem = f.IdFerragem)
                WHERE cf.IdFerragem =" + idFerragem;

            return objPersistence.LoadData(sql).ToList();
        }
    }
}
