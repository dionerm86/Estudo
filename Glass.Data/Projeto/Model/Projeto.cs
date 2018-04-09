using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProjetoDAO))]
	[PersistenceClass("projeto")]
	public class Projeto
    {
        #region Enumeradores

        public enum SituacaoProjeto : int
        {
            Aberto,
            Finalizado,
            Cancelado
        }

        public enum TipoEntregaProjeto : int
        {
            Balcao = 1,
            Comum,
            Temperado,
            Entrega,
            ManutencaoTemperado,
            Esquadria
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPROJETO", PersistenceParameterType.IdentityKey)]
        public uint IdProjeto { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IDCLIENTE")]
        public uint? IdCliente { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("IDORCAMENTO")]
        public uint? IdOrcamento { get; set; }

        [PersistenceProperty("TIPOENTREGA")]
        public int TipoEntrega { get; set; }

        [PersistenceProperty("NOMECLIENTE")]
        public string NomeCliente { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("DATAFIN")]
        public DateTime? DataFin { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("APENASVIDRO")]
        public bool ApenasVidro { get; set; }

        [PersistenceProperty("TAXAPRAZO")]
        public Single TaxaPrazo { get; set; }

        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("VALORICMS")]
        public decimal ValorIcms { get; set; }

        [PersistenceProperty("CUSTOTOTAL")]
        public decimal CustoTotal { get; set; }

        [PersistenceProperty("PEDCLI")]
        public string PedCli { get; set; }

        [PersistenceProperty("TIPOVENDA")]
        public int TipoVenda { get; set; }

        [PersistenceProperty("FastDelivery")]
        public bool FastDelivery { get; set; }

        [PersistenceProperty("OBSLIBERACAO")]
        public string ObsLiberacao { get; set; }

        [PersistenceProperty("IDTRANSPORTADOR")]
        public int? IdTransportador { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _nomeFunc;

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc
        {
            get { return _nomeFunc != null ? _nomeFunc.ToUpper() : String.Empty; }
            set { _nomeFunc = value; }
        }

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("CliRevenda", DirectionParameter.InputOptional)]
        public bool CliRevenda { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string IdNomeCliente
        {
            get { return IdCliente > 0 ? IdCliente + " - " + NomeCliente : NomeCliente; }
        }

        public uint IdPedido { get; set; }

        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case (int)SituacaoProjeto.Aberto:
                        return "Aberto";
                    case (int)SituacaoProjeto.Cancelado:
                        return "Cancelado";
                    case (int)SituacaoProjeto.Finalizado:
                        return "Finalizado";
                    default:
                        return String.Empty;
                }
            }
        }

        public string DescrTipoEntrega
        {
            get
            {
                switch (TipoEntrega)
                {
                    case 1:
                        return "Balcão";
                    case 2:
                        return "Colocação Comum";
                    case 3:
                        return "Colocação Temperado";
                    case 4:
                        return "Entrega";
                    case 5:
                        return "Manutenção Temperado";
                    case 6:
                        return "Colocação Esquadria";
                    default:
                        return String.Empty;
                }
            }
        }

        // Controla visibilidade da opcao editar na grid
        public bool EditVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;
                bool flagSituacao;
                bool flagVendedor = true;
                bool flagAuxAdm = true;

                // Apenas Ativo e Conferido podem ser editados, mas se estiver Ativo/Em Conferência ou Em Conferencia
                // e não tiver ido para conferência, pode editar.
                flagSituacao = Situacao == (int)SituacaoProjeto.Aberto;

                // Se não for Gerente/Auxiliar verifica se o pedido é do usuário logado
                if (login.TipoUsuario != (uint)Utils.TipoFuncionario.Gerente &&
                    login.TipoUsuario != (uint)Utils.TipoFuncionario.AuxAdministrativo &&
                    login.TipoUsuario != (uint)Utils.TipoFuncionario.AuxEtiqueta &&
                    login.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador)
                    flagVendedor = IdFunc == login.CodUser;

                // Se for Auxiliar Adm., só pode alterar pedidos da loja dele
                if (login.TipoUsuario == (uint)Utils.TipoFuncionario.AuxAdministrativo ||
                    login.TipoUsuario == (uint)Utils.TipoFuncionario.AuxEtiqueta)
                    flagAuxAdm = IdLoja == login.IdLoja;

                return flagSituacao && flagVendedor && flagAuxAdm;
            }
        }

        /// <summary>
        /// Apenas gerentes podem excluir projeto
        /// </summary>
        public bool DeleteVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                return (login.TipoUsuario == (uint)Utils.TipoFuncionario.Administrador ||
                    login.TipoUsuario == (uint)Utils.TipoFuncionario.Gerente || IdFunc == UserInfo.GetUserInfo.CodUser) &&
                    Situacao != (int)SituacaoProjeto.Cancelado;
            }
        }

        public int NumeroItensProjeto
        {
            get { return ItemProjetoDAO.Instance.GetCount(IdProjeto); }
        }

        public string DescricaoDescontoEcommerce
        {
            get
            {
                var descontoEcommerce = ClienteDAO.Instance.ObterPorcentagemDescontoEcommerce(null, (int)IdCliente);

                if (descontoEcommerce > 0)
                {
                    return string.Format("AO GERAR O PEDIDO VOCÊ TERÁ UM DESCONTO DE: {0}%", descontoEcommerce);
                }

                return "";
            }
        }

        #endregion
    }
}