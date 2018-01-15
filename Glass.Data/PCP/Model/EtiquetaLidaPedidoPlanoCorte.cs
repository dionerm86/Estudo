using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceClass("etiqueta_lida_pedido_planocorte"),
    PersistenceBaseDAO(typeof(EtiquetaLidaPedidoPlanoCorteDAO))]
    public class EtiquetaLidaPedidoPlanoCorte
    {
        [PersistenceProperty("IDLEITURAETIQUETAPEDPLANOCORTE", PersistenceParameterType.Key)]
        public uint IdLeituraEtiquetaPedPlanoCorte { get; set; }

        [PersistenceProperty("NUMETIQUETAREAL", PersistenceParameterType.Key)]
        public string NumEtiquetaReal { get; set; }
    }
}
