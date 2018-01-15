using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class SetorBenefDAO : BaseDAO<SetorBenef, SetorBenefDAO>
    {
        //private SetorBenefDAO() { }

        public IList<SetorBenef> GetBySetor(int idSetor)
        {
            return objPersistence.LoadData("select * from setor_benef where idSetor=" + idSetor).ToList();
        }

        public void DeleteBySetor(int idSetor)
        {
            objPersistence.ExecuteCommand("delete from setor_benef where idSetor=" + idSetor);
        }
    }
}
