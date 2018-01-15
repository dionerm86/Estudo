using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MedicaoDAO))]
	[PersistenceClass("medicao")]
	public class Medicao : ModelBaseCadastro
    {
        #region Enumeradores

        public enum SituacaoMedicao
        {
            Aberta = 1,
            EmAndamento,
            Finalizada,
            Remarcada,
            Cancelada
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDMEDICAO", PersistenceParameterType.IdentityKey)]
        public uint IdMedicao { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public int? IdPedido { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint? IdFunc { get; set; }

        [PersistenceProperty("IDFUNCCONF")]
        public uint? IdFuncConf { get; set; }

        [PersistenceProperty("IDFUNCMED")]
        public uint? IdFuncMed { get; set; }

        [PersistenceProperty("IDORCAMENTO")]
        public uint? IdOrcamento { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

		private string _nomeCliente;

		[PersistenceProperty("NOMECLIENTE")]
		public string NomeCliente
		{
            get { return _nomeCliente != null ? _nomeCliente.ToUpper() : String.Empty; }
			set { _nomeCliente = value; }
		}

        [PersistenceProperty("TELCLIENTE")]
        public string TelCliente { get; set; }

        [PersistenceProperty("CELCLIENTE")]
        public string CelCliente { get; set; }

        [PersistenceProperty("EMAILCLIENTE")]
        public string EmailCliente { get; set; }

		private string _endereco;

		[PersistenceProperty("ENDERECO")]
		public string Endereco
		{
			get { return _endereco != null ? _endereco.ToUpper() : String.Empty; }
			set { _endereco = value; }
		}

		private string _cidade;

		[PersistenceProperty("CIDADE")]
		public string Cidade
		{
            get { return _cidade != null ? _cidade.ToUpper() : String.Empty; }
			set { _cidade = value; }
		}

		private string _bairro;

		[PersistenceProperty("BAIRRO")]
		public string Bairro
		{
            get { return _bairro != null ? _bairro.ToUpper() : String.Empty; }
			set { _bairro = value; }
		}

		private string _compl;

		[PersistenceProperty("COMPL")]
		public string Compl
		{
            get { return _compl != null ? _compl.ToUpper() : String.Empty; }
			set { _compl = value; }
		}

        [PersistenceProperty("CEP")]
        public string Cep { get; set; }

        private string _contatoObra;

        [PersistenceProperty("CONTATOOBRA")]
        public string ContatoObra
        {
            get { return _contatoObra != null ? _contatoObra.ToUpper() : String.Empty; }
            set { _contatoObra = value; }
        }

        [PersistenceProperty("MEDICAODEFINITIVA")]
        public bool MedicaoDefinitiva { get; set; }

        [PersistenceProperty("OBSMEDICAO")]
        public string ObsMedicao { get; set; }

        [PersistenceProperty("DATAMEDICAO")]
        public DateTime? DataMedicao { get; set; }

        /// <summary>
        /// 1-Manhã
        /// 2-Tarde
        /// 3-1º Turno
        /// 4-Horário Comercial
        /// </summary>
        [PersistenceProperty("TURNO")]
        public int Turno { get; set; }

        [PersistenceProperty("HORA")]
        public string Hora { get; set; }

		private int _situacao = 1;

        /// <summary>
        /// 1-Aberta
        /// 2-Em andamento
        /// 3-Finalizada
        /// 4-Remarcada
        /// 5-Cancelada
        /// </summary>
		[PersistenceProperty("SITUACAO")]
		public int Situacao
		{
			get { return _situacao; }
			set { _situacao = value; }
		}

        [PersistenceProperty("DATACONF")]
        public DateTime? DataConf { get; set; }

        [PersistenceProperty("OBSCONF")]
        public string ObsConf { get; set; }

        [PersistenceProperty("LATITUDE")]
        public decimal? Latitude { get; set; }

        [PersistenceProperty("LONGITUDE")]
        public decimal? Longitude { get; set; }

        [PersistenceProperty("DATAEFETUAR")]
        public DateTime? DataEfetuar { get; set; }

        [PersistenceProperty("NUMSEQ")]
        public int NumSeq { get; set; }

        [PersistenceProperty("REFERENCIA")]
        public string Referencia { get; set; }

        [PersistenceProperty("FORMAPAGTO")]
        public string FormaPagto { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("DATAINSTALACAO")]
        public DateTime? DataInstalacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("ENDERECOLOJA", DirectionParameter.InputOptional)]
        public string EnderecoLoja { get; set; }

        [PersistenceProperty("NUMEROLOJA", DirectionParameter.InputOptional)]
        public string NumeroLoja { get; set; }

        [PersistenceProperty("BAIRROLOJA", DirectionParameter.InputOptional)]
        public string BairroLoja { get; set; }

        [PersistenceProperty("CIDADELOJA", DirectionParameter.InputOptional)]
        public string CidadeLoja { get; set; }

        [PersistenceProperty("TELLOJA", DirectionParameter.InputOptional)]
        public string TelLoja { get; set; }

        [PersistenceProperty("NomeMedidor", DirectionParameter.InputOptional)]
        public string NomeMedidor { get; set; }

        private string _nomeVendedor;

        [PersistenceProperty("NomeVendedor", DirectionParameter.InputOptional)]
        public string NomeVendedor
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeVendedor); }
            set { _nomeVendedor = value; }
        }

        [PersistenceProperty("IsMedicaoDefinitivaOrcamento", DirectionParameter.InputOptional)]
        public bool IsMedicaoDefinitivaOrcamento { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrSituacao
        {
            get 
            {
                switch (_situacao)
                {
                    case 1: 
                        return "Aberta";
                    case 2:
                        return "Em andamento";
                    case 3:
                        return "Finalizada";
                    case 4:
                        return "Remarcada";
                    case 5:
                        return "Cancelada";
                    default:
                        return "";
                }
            }
        }
        
        public string DescrTurno
        {
            get { return Turno == 1 ? "Manhã" : Turno == 2 ? "Tarde" : Turno == 3 ? "1.º Horário" : Turno == 4 ? "Horário Comercial" : String.Empty; }
        }

        public string EnderecoCompleto
        {
            get { return Endereco + " " + Compl + ", " + Bairro + " - " + Cidade; }
        }

        public string DadosLoja
        {
            get 
            {
                return EnderecoLoja + (!String.IsNullOrEmpty(NumeroLoja) ? " nº " + NumeroLoja : String.Empty) +
                    " - " + BairroLoja + " - " + CidadeLoja + " - Fone: " + TelLoja;
            }
        }

        [PersistenceProperty("criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        /// <summary>
        /// Controla a visibilidade da drop de situação ao editar uma medição
        /// </summary>
        public bool SituacaoEnabled
        {
            get
            {
                // Exibe apenas se o usuário logado for um Aux. Escritório Medição
                return Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao);
            }
        }

        /// <summary>
        /// Controla a visibilidade do botão de fotos na grid
        /// </summary>
        public bool FotosVisible
        {
            get
            {
                // Exibe apenas se a medição estiver "Aberta" ou se a medição não estiver finalizada/cancelada e o 
                // usuário logado for um Aux. Escritório Medição
                return _situacao == 1 || Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao);
            }
        }

        /// <summary>
        /// Controla se a drop do vendedor estará disponível para edição
        /// </summary>
        public bool DropVendedorEnabled
        {
            get 
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                bool flagFunc =
                    // Se o funcionário for vendedor, só edita o que for dele
                    (login.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor && IdFunc == null) ||

                    // Se o funcionário não for vendedor, edita se tiver permissão
                    (login.TipoUsuario != (uint)Utils.TipoFuncionario.Vendedor &&
                    Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao));

                return flagFunc;
            }
        }

        /// <summary>
        /// Controla a visibilidade do botão editar na grid
        /// </summary>
        public bool EditVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                bool flagFunc =
                    // Se o funcionário for vendedor, só edita o que for dele
                    (login.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor &&
                    Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao) &&
                    (IdFunc == login.CodUser || IdFunc == null || MedicaoConfig.MedicaoPermissaoAlterarTodos)) ||

                    // Se o funcionário não for vendedor, edita se tiver permissão
                    (login.TipoUsuario != (uint)Utils.TipoFuncionario.Vendedor &&
                    Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao));

                // Exibe apenas se a medição estiver "Aberta" ou se a medição não estiver finalizada/cancelada e o 
                // usuário logado for um Aux. Escritório Medição
                return (_situacao == 1 && flagFunc) || (_situacao != 3 && _situacao != 5 && flagFunc);
            }
        }

        /// <summary>
        /// Controla a visibilidade do botão excluir na grid
        /// </summary>
        public bool DeleteVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                bool flagFunc =
                    // Se o funcionário for vendedor, só edita o que for dele, a não ser que no config diga o contrário
                    (login.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor &&
                    Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao) &&
                    (IdFunc == login.CodUser || IdFunc == null || MedicaoConfig.MedicaoPermissaoAlterarTodos)) ||

                    // Se o funcionário não for vendedor, edita se tiver permissão
                    (login.TipoUsuario != (uint)Utils.TipoFuncionario.Vendedor &&
                    Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao));

                // Exibe apenas se a medição estiver "Aberta" ou se a medição não estiver finalizada e o 
                // usuário logado for um Aux. Escritório Medição
                return (_situacao == 1 && flagFunc) || (_situacao != 3 && _situacao != 5 && flagFunc);
            }
        }

        /// <summary>
        /// Controla a visibilidade do botão finalizar na grid
        /// </summary>
        public bool FinalizarVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                bool flagFunc =
                    // Se o funcionário for vendedor, só edita o que for dele
                    (login.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor &&
                    Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao) &&
                    (IdFunc == login.CodUser || IdFunc == null || MedicaoConfig.MedicaoPermissaoAlterarTodos)) ||

                    // Se o funcionário não for vendedor, edita se tiver permissão
                    (login.TipoUsuario != (uint)Utils.TipoFuncionario.Vendedor &&
                    Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao));

                // Exibe apenas se a medição estiver "Em andamento" ou "Remarcada" e se o funcionário for
                // Aux. Escritorio Medição ou Gerente ou Administrador
                return (_situacao == 2 || _situacao == 4) && flagFunc;
            }
        }

        #endregion
    }
}