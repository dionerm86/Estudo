using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DebitoComissaoDAO))]
    [PersistenceClass("debito_comissao")]
    public class DebitoComissao
    {
        #region Propriedades

        [PersistenceProperty("IDDEBITOCOMISSAO", PersistenceParameterType.IdentityKey)]
        public uint IdDebitoComissao { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("TIPOFUNC")]
        public Pedido.TipoComissao Tipo { get; set; }

        [PersistenceProperty("VALORDEBITO")]
        public decimal ValorDebito { get; set; }

        [PersistenceProperty("IDCOMISSAO")]
        public uint? IdComissao { get; set; }

        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public uint? IdTrocaDevolucao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescricaoDebito
        {
            get { return "Débito relativo ao pagamento da comissão do pedido " + IdPedido; }
        }

        #endregion
    }
}