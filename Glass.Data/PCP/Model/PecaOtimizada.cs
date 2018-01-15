using GDA;
using System.ComponentModel;

namespace Glass.Data.Model
{
    [PersistenceClass("peca_otimizada")]
    public class PecaOtimizada : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IdPecaOtimizada", PersistenceParameterType.IdentityKey)]
        public int IdPecaOtimizada { get; set; }

        [PersistenceProperty("IdLayoutPecaOtimizada")]
        [PersistenceForeignKey(typeof(LayoutPecaOtimizada), "IdLayoutPecaOtimizada")]
        public int IdLayoutPecaOtimizada { get; set; }

        [PersistenceProperty("IdProdPed")]
        public int? IdProdPed { get; set; }

        [PersistenceProperty("GrauCorte")]
        public GrauCorteEnum GrauCorte { get; set; }

        [PersistenceProperty("Sobra")]
        public bool Sobra { get; set; }

        [PersistenceProperty("PosicaoX")]
        public decimal PosicaoX { get; set; }

        [PersistenceProperty("Comprimento")]
        public decimal Comprimento { get; set; }
    }
}
