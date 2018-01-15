using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidoInternoDAO))]
    [PersistenceClass("pedido_interno")]
    public class PedidoInterno
    {
        #region Enumeradores

        public enum SituacaoPedidoInt
        {
            Aberto = 1,
            Cancelado,
            Finalizado,
            Confirmado,
            ConfirmadoParcialmente,
            Autorizado
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPEDIDOINTERNO", PersistenceParameterType.IdentityKey)]
        public uint IdPedidoInterno { get; set; }

        [PersistenceProperty("IDFUNCCAD")]
        public uint IdFuncCad { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IdCentroCusto")]
        public int? IdCentroCusto { get; set; }

        private int _situacao = (int)SituacaoPedidoInt.Aberto;

        [PersistenceProperty("SITUACAO")]
        public int Situacao
        {
            get { return _situacao; }
            set { _situacao = value; }
        }

        [PersistenceProperty("DATAPEDIDO")]
        public DateTime DataPedido { get; set; }

        [PersistenceProperty("IDFUNCAUT")]
        public uint? IdFuncAut { get; set; }

        [PersistenceProperty("DATAAUT")]
        public DateTime? DataAut { get; set; }

        [PersistenceProperty("USUCONF")]
        public uint? UsuConf { get; set; }

        [PersistenceProperty("DATACONF")]
        public DateTime? DataConf { get; set; }

        [PersistenceProperty("OBS")]
        public string Observacao { get; set; }

        #endregion 

        #region Propriedades Estendidas

        private string _nomeFuncAut;

        [PersistenceProperty("NOMEFUNCAUT", DirectionParameter.InputOptional)]
        public string NomeFuncAut
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeFuncAut); }
            set { _nomeFuncAut = value; }
        }

        private string _nomeFuncCad;

        [PersistenceProperty("NOMEFUNCCAD", DirectionParameter.InputOptional)]
        public string NomeFuncCad
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeFuncCad); }
            set { _nomeFuncCad = value; }
        }

        private string _nomeFuncConf;

        [PersistenceProperty("NOMEFUNCCONF", DirectionParameter.InputOptional)]
        public string NomeFuncConf
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeFuncConf); }
            set { _nomeFuncConf = value; }
        }

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrSituacao
        {
            get
            {
                switch ((SituacaoPedidoInt)_situacao)
                {
                    case SituacaoPedidoInt.Aberto: return "Aberto";
                    case SituacaoPedidoInt.Cancelado: return "Cancelado";
                    case SituacaoPedidoInt.Finalizado: return "Finalizado";
                    case SituacaoPedidoInt.Confirmado: return "Confirmado";
                    case SituacaoPedidoInt.ConfirmadoParcialmente: return "Confirmado Parcialmente";
                    case SituacaoPedidoInt.Autorizado: return "Autorizado";
                    default: return "";
                }
            }
        }

        public bool EditVisible
        {
            get { return _situacao == (int)SituacaoPedidoInt.Aberto; }
        }

        public bool DeleteVisible
        {
            get
            {
                if (!UserInfo.GetUserInfo.IsAdministrador && IdFuncCad != UserInfo.GetUserInfo.CodUser)
                    return false;

                return _situacao != (int)SituacaoPedidoInt.Cancelado && 
                    _situacao != (int)SituacaoPedidoInt.Confirmado && 
                    _situacao != (int)SituacaoPedidoInt.ConfirmadoParcialmente;
            }
        }

        public bool ReabrirVisible
        {
            get 
            {
                return _situacao == (int)SituacaoPedidoInt.Finalizado ||
                    _situacao == (int)SituacaoPedidoInt.Autorizado;
            }
        }


        public string DescrCentroCusto
        {
            get
            {
                return IdCentroCusto.GetValueOrDefault(0) > 0 ? CentroCustoDAO.Instance.ObtemDescricao(IdCentroCusto.Value) : "";
            }
        }

        #endregion
    }
}