using GDA;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DAL.ComissaoConfigGerenteDAO))]
    [PersistenceClass("comissao_config_gerente")]
    public class ComissaoConfigGerente 
    {
        [PersistenceProperty("IDCOMISSAOCONFIGGERENTE", PersistenceParameterType.IdentityKey)]
        public uint IdComissaoConfigGerente { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IDFUNCIONARIO")]
        public uint IdFuncionario { get; set; }

        [Log("Percentual Venda")]
        [PersistenceProperty("PERCENTUALVENDA")]
        public decimal PercentualVenda { get; set; }

        [Log("Percentual Revenda")]
        [PersistenceProperty("PERCENTUALREVENDA")]
        public decimal PercentualRevenda { get; set; }

        [Log("Percentual Mão Obra")]
        [PersistenceProperty("PERCENTUALMAODEOBRA")]
        public decimal PercentualMaoDeObra { get; set; }

        [Log("Percentual Mão Obra Especial")]
        [PersistenceProperty("PERCENTUALMAODEOBRAESPECIAL")]
        public decimal PercentualMaoDeObraEspecial { get; set; }
    }
}
