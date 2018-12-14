using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public class ConstanteFerragemDAO : BaseDAO<ConstanteFerragem, ConstanteFerragemDAO>
    {

        public List<ConstanteFerragem> ObterConstantesFerragens(int idFerragem)
        {
            string sql = @"
                SELECT cf.*
	            FROM constante_ferragem cf
	                INNER JOIN ferragem f ON (cf.IdFerragem = f.IdFerragem)
                WHERE cf.IdFerragem =" + idFerragem;

            return objPersistence.LoadData(sql).ToList();
        }
    }
}
