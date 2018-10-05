using Glass.Data.DAL;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class CondutoresDAO : BaseDAO<Condutores, CondutoresDAO>
    {
        /// <summary>
        /// Recupera a listagem de condutores.
        /// </summary>
        public IList<Condutores> GetList()
        {
            return objPersistence.LoadData("select * from condutores").ToList();
        }
    }
}
