using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ComissaoPedidoDAO))]
	[PersistenceClass("comissao_pedido")]
	public class ComissaoPedido
    {
        #region Propriedades

        [PersistenceProperty("IDCOMISSAOPEDIDO", PersistenceParameterType.IdentityKey)]
        public uint IdComissaoPedido { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDCOMISSAO")]
        public uint IdComissao { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("BASECALCCOMISSAO")]
        public decimal BaseCalcComissao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODCLIENTE", DirectionParameter.InputOptional)]
        public string CodCliente { get; set; }

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("DATACONFPEDIDO", DirectionParameter.InputOptional)]
        public DateTime DataConfPedido { get; set; }

        [PersistenceProperty("DATALIBERACAO", DirectionParameter.InputOptional)]
        public DateTime? DataLiberacao { get; set; }

        [PersistenceProperty("DATAREFINI", DirectionParameter.InputOptional)]
        public DateTime? DataRefIni { get; set; }

        [PersistenceProperty("DATAREFFIM", DirectionParameter.InputOptional)]
        public DateTime? DataRefFim { get; set; }

        [PersistenceProperty("TOTALPEDIDO", DirectionParameter.InputOptional)]
        public decimal TotalPedido { get; set; }

        [PersistenceProperty("TOTALDEBITOCOMISSAO", DirectionParameter.InputOptional)]
        public decimal TotalDebitoComissao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal TotalComissao { get; set; }

        public string NomeFuncCom { get; set; }

        public uint IdFunc { get; set; }

        public int TipoFunc { get; set; }

        #endregion
    }
}