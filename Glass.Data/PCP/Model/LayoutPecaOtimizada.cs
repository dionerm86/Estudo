using GDA;
using System.ComponentModel;

namespace Glass.Data.Model
{
    [PersistenceClass("layout_peca_otimizada")]
    public class LayoutPecaOtimizada : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IdLayoutPecaOtimizada", PersistenceParameterType.IdentityKey)]
        public int IdLayoutPecaOtimizada { get; set; }

        [PersistenceProperty("IdOtimizacao")]
        [PersistenceForeignKey(typeof(Otimizacao), "IdOtimizacao")]
        public int IdOtimizacao { get; set; }

        [PersistenceProperty("IdProd")]
        public int IdProd { get; set; }

        [PersistenceProperty("Qtde")]
        public int Qtde { get; set; }
    }
}
