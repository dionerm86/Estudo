using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    [PersistenceClass("subgrupoprod_loja")]
    public class SubgrupoProdLoja : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDSUBGRUPOPROD", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(SubgrupoProd), "IdSubgrupoProd")]
        public int IdSubgrupoProd { get; set; }

        [PersistenceProperty("IDLOJA", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Loja), "IdLoja")]
        public int IdLoja { get; set; }
        
        #endregion
    }
}
