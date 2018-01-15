using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TabelaDescontoAcrescimoClienteDAO))]
    [PersistenceClass("tabela_desconto_acrescimo_cliente")]
    public class TabelaDescontoAcrescimoCliente : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDTABELADESCONTO", PersistenceParameterType.IdentityKey)]
        public int IdTabelaDesconto { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion
    }
}