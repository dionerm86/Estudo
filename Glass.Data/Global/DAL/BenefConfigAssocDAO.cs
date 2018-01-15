using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class BenefConfigAssocDAO : BaseDAO<BenefConfigAssoc, BenefConfigAssocDAO>
    {
        //private BenefConfigAssocDAO() { }

        private string Sql(uint idBenefConfig, bool selecionar)
        {
            string campos = selecionar ? "bca.*, bc.idParent as IdParentAssoc, coalesce(bcPai.tipoControle, bc.tipoControle) as TipoControleAssoc" : "count(*)";
            string sql = @"
                select " + campos + @"
                from benef_config_assoc bca 
                    left join benef_config bc on (bca.idBenefConfigAssoc=bc.idBenefConfig) 
                    left join benef_config bcPai on (bc.idParent=bcPai.idBenefConfig)
                where 1"; 
            
            if (idBenefConfig > 0)
                sql += " and idBenefConfig=" + idBenefConfig;

            return sql;
        }

        public new IList<BenefConfigAssoc> GetAll()
        {
            return objPersistence.LoadData(Sql(0, true)).ToList();
        }

        /// <summary>
        /// Retorna os beneficiamentos associados a um beneficiamento.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        public IList<BenefConfigAssoc> GetByIdBenefConfig(uint idBenefConfig)
        {
            return objPersistence.LoadData(Sql(idBenefConfig, true)).ToList();
        }
    }
}
