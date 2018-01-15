using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(InstalacaoDAO))]
	[PersistenceClass("instalacao")]
	public class Instalacao : ModelBaseCadastro
    {
        #region Enumeradores

        public enum SituacaoInst : int
        {
            Aberta=1,
            EmAndamento,
            Finalizada,
            Cancelada,
            Continuada,
            Agendar,
            Colagem,
            DeptoTecnico
        }

        public enum TipoInst
        {
            Comum=1,
            Temperado,
            SistemaReikiEstrutura,
            Entrega,
            SistemaReikiVidros,
            EuroGlass
        }

        #endregion

        #region Propriedades

        [Log("Id. da Instalação.")]
        [PersistenceProperty("IDINSTALACAO", PersistenceParameterType.IdentityKey)]
        public uint IdInstalacao { get; set; }

        [Log("Id. do Pedido")]
        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDORDEMINSTALACAO")]
        public uint? IdOrdemInstalacao { get; set; }

        [Log("Data de Entrega")]
        [PersistenceProperty("DATAENTREGA")]
        public DateTime DataEntrega { get; set; }

        [Log("Data de Instalação")]
        [PersistenceProperty("DATAINSTALACAO")]
        public DateTime? DataInstalacao { get; set; }

        /// <summary>
        /// 1-Comum
        /// 2-Temperado
        /// 3-Sistema Reiki (Estrutura)
        /// 4-Entrega
        /// 5-Sistema Reiki (Vidros)
        /// 6-Euro Glass
        /// 7-Serralheiria
        /// 8-Mão de Obra
        /// 9-Espelho
        /// 10-Box de acrílico
        /// 11-Vidro comum (Tampo de mesa)
        /// 12-Produtos de terceiros
        /// 13-Vidro comum reforma
        /// </summary>
        [Log("Tipo da Instalação")]
        [PersistenceProperty("TIPOINSTALACAO")]
        public int TipoInstalacao { get; set; }

        [PersistenceProperty("LIBERARTEMPERADO")]
        public bool LiberarTemperado { get; set; }

        /// <summary>
        /// 1-Aberta
        /// 2-Em Andamento
        /// 3-Finalizada
        /// 4-Cancelada
        /// 5-Continuada
        /// 6-A agendar
        /// </summary>
        [Log("Situação")]
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [Log("Usu. Final")]
        [PersistenceProperty("USUFINAL")]
        public uint? UsuFinal { get; set; }

        [Log("Data Final")]
        [PersistenceProperty("DATAFINAL")]
        public DateTime? DataFinal { get; set; }

        [PersistenceProperty("CONFIRMADA")]
        public bool Confirmada { get; set; }

        [PersistenceProperty("DATACONFIRMADA")]
        public DateTime? DataConfirmada { get; set; }

        [PersistenceProperty("LATITUDE")]
        public decimal? Latitude { get; set; }

        [PersistenceProperty("LONGITUDE")]
        public decimal? Longitude { get; set; }

        [Log("Obs.")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Data ordem de instalação")]
        [PersistenceProperty("DATAORDEMINSTALACAO")]
        public DateTime? DataOrdemInstalacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _nomesEquipes;

        [PersistenceProperty("NOMESEQUIPES", DirectionParameter.InputOptional)]
        public string NomesEquipes
        {
            get { return _nomesEquipes != null ? _nomesEquipes : String.Empty; }
            set { _nomesEquipes = value; }
        }

        private string _nomeLoja;

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja
        {
            get { return _nomeLoja != null ? _nomeLoja : String.Empty; }
            set { _nomeLoja = value; }
        }

        [PersistenceProperty("IDCLIENTE", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }

        private string _nomeCliente;

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente
        {
            get { return BibliotecaTexto.GetThreeFirstWords(_nomeCliente != null ? _nomeCliente.ToUpper() : String.Empty); }
            set { _nomeCliente = value; }
        }

        private string _localObra;

        [PersistenceProperty("LocalObra", DirectionParameter.InputOptional)]
        public string LocalObra
        {
            get { return _localObra != null ? _localObra.ToUpper() : String.Empty; }
            set { _localObra = value; }
        }

        [PersistenceProperty("IDORCAMENTO", DirectionParameter.InputOptional)]
        public uint? IdOrcamento { get; set; }

        [PersistenceProperty("DATACONFPEDIDO", DirectionParameter.InputOptional)]
        public DateTime? DataConfPedido { get; set; }

        [PersistenceProperty("DATAENTREGAPEDIDO", DirectionParameter.InputOptional)]
        public DateTime? DataEntregaPedido { get; set; }

        [PersistenceProperty("TEMPOINST", DirectionParameter.InputOptional)]
        public string TempoInst { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("VALORPEDIDO", DirectionParameter.InputOptional)]
        public decimal ValorPedido { get; set; }

        [PersistenceProperty("VALORPRODUTOSINSTALADOS", DirectionParameter.InputOptional)]
        public decimal ValorProdutosInstalados { get; set; }

        #endregion

        #region Propriedades de Suporte

        #region Propriedades utilizadas na tela de finalizar/continuar/cancelar

        public bool FinalizarVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                return Situacao == (int)SituacaoInst.EmAndamento && !PedidoConfig.Instalacao.UsarAmbienteInstalacao &&
                    (Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) ||
                    Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado));
            }
        }

        public bool ContinuarVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                return Situacao == 2 && !PedidoConfig.Instalacao.UsarAmbienteInstalacao &&
                    (Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) ||
                    Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado));
            }
        }

        public bool ContinuarConfirmadaVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                return (Situacao == 3 || (Situacao == 2 && PedidoConfig.Instalacao.UsarAmbienteInstalacao)) &&
                    (Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) ||
                    Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado));
            }
        }

        public string ContinuarConfirmadaText
        {
            get { return Situacao == 3 ? "Continuar" : "Continuar/Finalizar"; }
        }

        public bool CancelarVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                return (Situacao == (int)SituacaoInst.Aberta || Situacao == (int)SituacaoInst.EmAndamento) &&
                    (Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) ||
                    Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado));
            }
        }

        public bool CancelarFinalizadaVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                return (Situacao == (int)SituacaoInst.Finalizada) &&
                    (Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) ||
                    Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado));
            }
        }

        #endregion

        public string DescrTipoInstalacao
        {
            get
            {
                foreach (GenericModel g in DataSources.Instance.GetTipoInstalacao())
                    if ((int)g.Id == TipoInstalacao)
                        return g.Descr;

                return "N/D";
            }
        }

        public string DescrSituacao
        {
            get
            {
                string sitObs = 
                    Situacao == 1 ? "Aberta" :
                    Situacao == 2 ? "Em Andamento" :
                    Situacao == 3 ? "Finalizada" :
                    Situacao == 4 ? "Cancelada" :
                    Situacao == 5 ? "Continuada " :
                    Situacao == 6 ? "A agendar" :
                    Situacao == 7 ? "Colagem" :
                    Situacao == 8 ? "Depto. Técnico" :
                    String.Empty;

                return !String.IsNullOrEmpty(Obs) ? sitObs + " (" + Obs + ")" : sitObs;
            }
        }

        public bool ImprimirOrdemInstVisible
        {
            get
            {
                return Situacao != (int)SituacaoInst.Aberta && Situacao != (int)SituacaoInst.Agendar &&
                    Situacao != (int)SituacaoInst.Colagem && Situacao != (int)SituacaoInst.DeptoTecnico && IdOrdemInstalacao > 0;
            }
        }

        public bool ExibirImpressaoPcp
        {
            get { return Glass.Data.DAL.PedidoEspelhoDAO.Instance.ExisteEspelho(IdPedido); }
        }

        public bool PodeAlterarSituacao
        {
            get
            {
                return InstalacaoConfig.TelaListagem.PermitirAlterarSituacaoManualmente && 
                    (Situacao == (int)SituacaoInst.Aberta || Situacao == (int)SituacaoInst.Agendar || Situacao == (int)SituacaoInst.Colagem || 
                    Situacao == (int)SituacaoInst.DeptoTecnico);
            }
        }

        #endregion
    }
}