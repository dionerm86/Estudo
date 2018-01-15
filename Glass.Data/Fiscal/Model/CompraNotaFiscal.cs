using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CompraNotaFiscalDAO)),
    PersistenceClass("compra_nota_fiscal")]
    public class CompraNotaFiscal
    {
        [PersistenceProperty("IDNF", PersistenceParameterType.Key)]
        public uint IdNf { get; set; }

        [PersistenceProperty("IDCOMPRA", PersistenceParameterType.Key)]
        public uint IdCompra { get; set; }
    }
}
