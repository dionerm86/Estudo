using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceClass("pecas_excluidas_sistema"),
    PersistenceBaseDAO(typeof(PecasExcluidasSistemaDAO))]
    public class PecasExcluidasSistema : ModelBaseCadastro
    {
        [PersistenceProperty("IDPECAEXCLUIDASISTEMA", PersistenceParameterType.IdentityKey)]
        public uint IdPecaExcluidaSistema { get; set; }

        [PersistenceProperty("NUMETIQUETA")]
        public string NumEtiqueta { get; set; }

        [PersistenceProperty("IDPRODPED")]
        public uint IdProdPed { get; set; }

        [PersistenceProperty("IDSETOR")]
        public uint IdSetor { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("TRACE")]
        public string Trace { get; set; }
    }
}
