using GDA;

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

        [PersistenceProperty("PERCENTUALVENDA")]
        public decimal PercentualVenda { get; set; }

        [PersistenceProperty("PERCENTUALREVENDA")]
        public decimal PercentualRevenda { get; set; }

        [PersistenceProperty("PERCENTUALMAODEOBRA")]
        public decimal PercentualMaoDeObra { get; set; }

        [PersistenceProperty("PERCENTUALMAODEOBRAESPECIAL")]
        public decimal PercentualMaoDeObraEspecial { get; set; }
    }
}
