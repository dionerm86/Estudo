using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceClass("leitura_etiqueta_pedido_planocorte"),
    PersistenceBaseDAO(typeof(LeituraEtiquetaPedidoPlanoCorteDAO))]
    public class LeituraEtiquetaPedidoPlanoCorte : ModelBaseCadastro
    {
        [PersistenceProperty("IDLEITURAETIQUETAPEDPLANOCORTE", PersistenceParameterType.IdentityKey)]
        public uint IdLeituraEtiquetaPedPlanoCorte { get; set; }

        [PersistenceProperty("NUMETIQUETALIDA")]
        public string NumEtiquetaLida { get; set; }
    }
}
