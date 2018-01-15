using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ParcelasNaoUsarDAO))]
    [PersistenceClass("parcelas_nao_usar")]
    public class ParcelasNaoUsar : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDPARCELA", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Parcelas), "IdParcela")]
        public int IdParcela { get; set; }

        [PersistenceProperty("IDCLIENTE", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Cliente), "IdCli")]
        public int? IdCliente { get; set; }

        [PersistenceProperty("IDFORNEC", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Fornecedor), "IdFornec")]
        public int? IdFornecedor { get; set; }

        #endregion
    }
}