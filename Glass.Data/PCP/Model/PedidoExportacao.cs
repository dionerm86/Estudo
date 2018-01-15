using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidoExportacaoDAO))]
    [PersistenceClass("pedido_exportacao")]
    public class PedidoExportacao
    {
        #region Enumeradores

        public enum SituacaoExportacaoEnum
        {
            Exportado = 1,
            Pronto,
            Chegou,
            Cancelado
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IdPedidoExportacao", PersistenceParameterType.IdentityKey)]
        public uint IdPedidoExportacao { get; set; }

        [PersistenceProperty("IDEXPORTACAO")]
        public uint IdExportacao { get; set; }

        [PersistenceProperty("IdPedido")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("SituacaoExportacao")]
        public int SituacaoExportacao { get; set; }

        [PersistenceProperty("DataSituacao")]
        public DateTime? DataSituacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("Cliente", DirectionParameter.InputOptional)]
        public string Cliente { get; set; }

        [PersistenceProperty("Total", DirectionParameter.InputOptional)]
        public decimal Total { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string SituacaoExportacaoDescr
        {
            get
            {
                string retorno = "";

                switch (SituacaoExportacao)
                {
                    case (int)SituacaoExportacaoEnum.Exportado: retorno = "Exportado"; break;
                    case (int)SituacaoExportacaoEnum.Chegou: retorno = "Chegou"; break;
                    case (int)SituacaoExportacaoEnum.Pronto: retorno = "Pronto"; break;
                    case (int)SituacaoExportacaoEnum.Cancelado: retorno = "Cancelado"; break;
                }

                return retorno;
            }
        }

        public bool VisibleChegou
        {
            get
            {
                return SituacaoExportacao == (uint)SituacaoExportacaoEnum.Pronto;
            }
        }

        #endregion
    }
}