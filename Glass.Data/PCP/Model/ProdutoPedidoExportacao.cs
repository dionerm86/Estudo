using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoPedidoExportacaoDAO))]
    [PersistenceClass("produtos_pedido_exportacao")]
    public class ProdutoPedidoExportacao
    {
        #region Propriedades

        [PersistenceProperty("ID", PersistenceParameterType.IdentityKey)]
        public uint Id { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProduto { get; set; }

        [PersistenceProperty("IDEXPORTACAO")]
        public uint IdExportacao { get; set; }

        #endregion

        #region  Propriedades Estendidas

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("Descricao", DirectionParameter.InputOptional)]
        public string Descricao { get; set; }

        [PersistenceProperty("Altura", DirectionParameter.InputOptional)]
        public float Altura { get; set; }

        [PersistenceProperty("Largura", DirectionParameter.InputOptional)]
        public int Largura { get; set; }

        [PersistenceProperty("TotM", DirectionParameter.InputOptional)]
        public float TotM { get; set; }

        [PersistenceProperty("Total", DirectionParameter.InputOptional)]
        public decimal Total { get; set; }

        #endregion
    }
}