using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;
using System.ComponentModel;
using Glass.Log;

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis situações dos clientes do sistema.
    /// </summary>
    public enum SituacaoCliente
    {
        /// <summary>
        /// Ativo.
        /// </summary>
        [Description("Ativo")]
        Ativo = 1,
        /// <summary>
        /// Inativo.
        /// </summary>
        [Description("Inativo")]
        Inativo,
        /// <summary>
        /// Cancelado.
        /// </summary>
        [Description("Cancelado")]
        Cancelado,
        /// <summary>
        /// Bloqueado
        /// </summary>
        [Description("Bloqueado")]
        Bloqueado
    }

    /// <summary>
    /// Possíveis tipos fiscais de cliente no sistema.
    /// </summary>
    public enum TipoFiscalCliente
    {
        /// <summary>
        /// Consumidor Final.
        /// </summary>
        [Description("Consumidor Final")]
        ConsumidorFinal = 1,
        /// <summary>
        /// Revenda.
        /// </summary>
        [Description("Revenda")]
        Revenda
    }

    /// <summary>
    /// Possíveis tipos de CRT (regime) do cliente no sistema.
    /// </summary>
    public enum CrtCliente
    {
        /// <summary>
        /// Regime Normal.
        /// </summary>
        [Description("Regime Normal")]
        RegimeNormal = 1,
        /// <summary>
        /// Simples Nacional.
        /// </summary>
        [Description("Simples Nacional")]
        SimplesNacional
    }

    /// <summary>
    /// Possíveis tipos de pessoa no sistema.
    /// </summary>
    public enum TipoPessoa : byte
    {
        /// <summary>
        /// Física.
        /// </summary>
        [Description("Física")]
        Fisica = (byte)'F',
        /// <summary>
        /// Jurídica.
        /// </summary>
        [Description("Jurídica")]
        Juridica = (byte)'J'
    }

    [PersistenceBaseDAO(typeof(ClienteDAO))]
	[PersistenceClass("cliente")]
	public class Cliente : ModelBaseCadastro, Sync.Fiscal.EFD.Entidade.ICliente
    {
        #region Propriedades

        [PersistenceProperty("ID_CLI", PersistenceParameterType.IdentityKey)]
        public int IdCli { get; set; }

        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("ID_LOJA")]
        [PersistenceForeignKey(typeof(Loja), "IdLoja")]
        public int? IdLoja { get; set; }

        [Log("Cidade", true, "NomeCidadeUf", typeof(CidadeDAO))]
        [PersistenceProperty("IDCIDADE")]
        [PersistenceForeignKey(typeof(Cidade), "IdCidade")]
        public int? IdCidade { get; set; }

        [Log("Comissionado", "Nome", typeof(ComissionadoDAO))]
        [PersistenceProperty("IDCOMISSIONADO")]
        [PersistenceForeignKey(typeof(Comissionado), "IdComissionado")]
        public int? IdComissionado { get; set; }

        [Log("Vendedor", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int? IdFunc { get; set; }

        [Log("Forma Pagto. Padrão", "Descricao", typeof(FormaPagtoDAO))]
        [PersistenceProperty("IDFORMAPAGTO")]
        [PersistenceForeignKey(typeof(FormaPagto), "IdFormaPagto")]
        public int? IdFormaPagto { get; set; }

        [Log("Tipo de Cliente", "Descricao", typeof(TipoClienteDAO))]
        [PersistenceProperty("IDTIPOCLIENTE")]
        [PersistenceForeignKey(typeof(TipoCliente), "IdTipoCliente")]
        public int? IdTipoCliente { get; set; }

        [Log("Plano de Conta Contábil", "Descricao", typeof(PlanoContaContabilDAO))]
        [PersistenceProperty("IDCONTACONTABIL")]
        [PersistenceForeignKey(typeof(PlanoContaContabil), "IdContaContabil")]
        public int? IdContaContabil { get; set; }

        /// <summary>
        /// 1-Consumidor Final
        /// 2-Revenda
        /// </summary>
        [Log("Tipo Fiscal")]
        [PersistenceProperty("TIPOFISCAL")]
        public int? TipoFiscal { get; set; }

        [Log("Indicador da IE do Destinatário")]
        [PersistenceProperty("INDICADORIEDESTINATARIO")]
        public IndicadorIEDestinatario IndicadorIEDestinatario { get; set; }

        [Log("Tabela Desconto/Acréscimo", "Descricao", typeof(TabelaDescontoAcrescimoClienteDAO))]
        [PersistenceProperty("IDTABELADESCONTO")]
        [PersistenceForeignKey(typeof(TabelaDescontoAcrescimoCliente), "IdTabelaDesconto")]
        public int? IdTabelaDesconto { get; set; }

        [Log("Transportador", "Nome", typeof(TransportadorDAO))]
        [PersistenceProperty("IDTRANSPORTADOR")]
        [PersistenceForeignKey(typeof(Transportador), "IdTransportador")]
        public int? IdTransportador { get; set; }

        [Log("Conta Bancária", "Descricao", typeof(ContaBancoDAO))]
        [PersistenceProperty("IdContaBanco")]
        [PersistenceForeignKey(typeof(ContaBanco), "IdContaBanco")]
        public int? IdContaBanco { get; set; }

        /// <summary>
        /// F - Física
        /// J - Jurídica
        /// </summary>
        [Log("Tipo de Pessoa")]
        [PersistenceProperty("TIPO_PESSOA")]
        public string TipoPessoa { get; set; }

		private string _nome;

        [Log("Nome/Razão Social", true)]
		[PersistenceProperty("NOME")]
		public string Nome
		{
			get { return _nome == null ? String.Empty : _nome.ToUpper(); }
			set { _nome = value; }
		}

        [Log("Apelido/Nome Fantasia")]
        [PersistenceProperty("NOMEFANTASIA")]
        public string NomeFantasia { get; set; }

		private string _endereco;

        [Log("Endereço", true)]
		[PersistenceProperty("ENDERECO")]
		public string Endereco
		{
			get { return _endereco == null ? String.Empty : _endereco.ToUpper(); }
			set { _endereco = value; }
		}

        private string _numero;

        [Log("Número", true)]
        [PersistenceProperty("NUMERO")]
        public string Numero
        {
            get { return _numero == null ? String.Empty : _numero; }
            set { _numero = value; }
        }

		private string _compl;

        [Log("Complemento", true)]
		[PersistenceProperty("COMPL")]
		public string Compl
		{
			get { return _compl == null ? String.Empty : _compl.ToUpper(); }
			set { _compl = value; }
		}

		private string _bairro;

        [Log("Bairro", true)]
		[PersistenceProperty("BAIRRO")]
		public string Bairro
		{
            get { return _bairro == null ? String.Empty : _bairro.ToUpper(); }
			set { _bairro = value; }
		}

		private string _cep;

        [Log("CEP")]
		[PersistenceProperty("CEP")]
		public string Cep
		{
			get { return _cep == null ? String.Empty : _cep; }
			set { _cep = value; }
		}

        [Log("País", true, "NomePais", typeof(PaisDAO))]
        [PersistenceProperty("IdPais")]
        [PersistenceForeignKey(typeof(Pais), "IdPais")]
        public int IdPais { get; set; }

        [Log("CPF/CNPJ", true)]
        [PersistenceProperty("CPF_CNPJ")]
        public string CpfCnpj { get; set; }

        [Log("RG/Inscrição Estadual", true)]
        [PersistenceProperty("RG_ESCINST")]
        public string RgEscinst { get; set; }

        [Log("Id. Estrangeiro.", true)]
        [PersistenceProperty("NumEstrangeiro")]
        public string NumEstrangeiro { get; set; }

        [Log("Suframa", true)]
        [PersistenceProperty("SUFRAMA")]
        public string Suframa { get; set; }

        [Log("Data de Nascimento")]
        [PersistenceProperty("DATA_NASC")]
        public DateTime? DataNasc { get; set; }

        [Log("Data da Última Compra")]
        [PersistenceProperty("DT_ULT_COMPRA", DirectionParameter.Input)]
        public DateTime? DtUltCompra { get; set; }

        [PersistenceProperty("TotalComprado", DirectionParameter.Input)]
        public decimal TotalComprado { get; set; }

		private string _telRes;

        [Log("Telefone Residencial")]
		[PersistenceProperty("TEL_RES")]
		public string TelRes
		{
			get { return _telRes == null ? String.Empty : _telRes; }
			set { _telRes = value; }
		}

        [Log("Telefone de Contato")]
        [PersistenceProperty("TEL_CONT")]
        public string TelCont { get; set; }

		private string _telCel;

        [Log("Telefone Celular")]
		[PersistenceProperty("TEL_CEL")]
		public string TelCel
		{
			get { return _telCel == null ? String.Empty : _telCel; }
			set { _telCel = value; }
		}

        [Log("Fax")]
        [PersistenceProperty("FAX")]
        public string Fax { get; set; }

        private string _contato;

        [Log("Contato")]
        [PersistenceProperty("CONTATO")]
        public string Contato
        {
            get { return _contato != null ? _contato.ToUpper() : _contato; }
            set { _contato = value; }
        }

        [Log("EmailContato")]
        [PersistenceProperty("EMAILCONTATO")]
        public string EmailContato { get; set; }

        [Log("SetorContato")]
        [PersistenceProperty("SETORCONTATO")]
        public string SetorContato { get; set; }

		private string _obs;

        [Log("Observação")]
		[PersistenceProperty("OBS")]
		public string Obs
		{
			get { return _obs != null ? _obs : String.Empty; }
			set { _obs = value; }
		}

        [Log("Histórico")]
        [PersistenceProperty("HISTORICO")]
        public string Historico { get; set; }

        [Log("Crédito")]
        [PersistenceProperty("CREDITO")]
        public decimal Credito { get; set; }

        [Log("Limite")]
        [PersistenceProperty("LIMITE")]
        public decimal Limite { get; set; }

        [Log("Limite Cheques por CPF/CNPJ")]
        [PersistenceProperty("LIMITECHEQUES")]
        public decimal LimiteCheques { get; set; }

        [Log("Valor Média Inicial")]
        [PersistenceProperty("VALORMEDIAINI")]
        public decimal ValorMediaIni { get; set; }

        [Log("Valor Média Final")]
        [PersistenceProperty("VALORMEDIAFIM")]
        public decimal ValorMediaFim { get; set; }

        [Log("E-mail Comercial")]
        [PersistenceProperty("EMAIL")]
        public string Email { get; set; }

        [Log("E-mail Fiscal")]
        [PersistenceProperty("EMAILFISCAL")]
        public string EmailFiscal { get; set; }

        [Log("E-mail Cobrança")]
        [PersistenceProperty("EMAILCOBRANCA")]
        public string EmailCobranca { get; set; }

        [PersistenceProperty("LOGIN")]
        public string Login { get; set; }

        [PersistenceProperty("SENHA")]
        public string Senha { get; set; }

        /// <summary>
        /// 1-Ativo
        /// 2-Inativo
        /// 3-Cancelado
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [Log("Revenda")]
        [PersistenceProperty("REVENDA")]
        public bool Revenda { get; set; }

        [Log("Contato 1")]
        [PersistenceProperty("CONTATO1")]
        public string Contato1 { get; set; }

        [Log("Telefone Celular Contato 1")]
        [PersistenceProperty("CELCONTATO1")]
        public string CelContato1 { get; set; }

        [Log("Ramal Contato 1")]
        [PersistenceProperty("RAMALCONTATO1")]
        public string RamalContato1 { get; set; }

        [Log("E-mail Contato 1")]
        [PersistenceProperty("EMAILCONTATO1")]
        public string EmailContato1 { get; set; }

        [Log("Contato 2")]
        [PersistenceProperty("CONTATO2")]
        public string Contato2 { get; set; }

        [Log("Telefone Celular Contato 2")]
        [PersistenceProperty("CELCONTATO2")]
        public string CelContato2 { get; set; }

        [Log("Ramal Contato 2")]
        [PersistenceProperty("RAMALCONTATO2")]
        public string RamalContato2 { get; set; }

        [Log("E-mail Contato 2")]
        [PersistenceProperty("EMAILCONTATO2")]
        public string EmailContato2 { get; set; }

        [PersistenceProperty("DATAALT")]
        public DateTime? DataAlt { get; set; }

        [PersistenceProperty("USUALT")]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int? UsuAlt { get; set; }

        [Log("Forma de Pagamento", "Descricao", typeof(ParcelasDAO))]
        [PersistenceProperty("TIPOPAGTO")]
        [PersistenceForeignKey(typeof(Parcelas), "IdParcela")]
        public int? TipoPagto { get; set; }

        [Log("Cobrar ICMS ST")]
        [PersistenceProperty("COBRARICMSST")]
        public bool CobrarIcmsSt { get; set; }

        [Log("Cobrar IPI")]
        [PersistenceProperty("COBRARIPI")]
        public bool CobrarIpi { get; set; }

        [Log("Percentual Desconto")]
        [PersistenceProperty("PERCREDUCAONFE")]
        public float PercReducaoNFe { get; set; }

        [Log("Percentual Desconto de Revenda")]
        [PersistenceProperty("PERCREDUCAONFEREVENDA")]
        public float PercReducaoNFeRevenda { get; set; }

        [Log("Percentual Mínimo Sinal Ped.")]
        [PersistenceProperty("PERCSINALMIN")]
        public float? PercSinalMinimo { get; set; }

        [Log("Percentual Comissão")]
        [PersistenceProperty("PERCENTUALCOMISSAO")]
        public float PercentualComissao { get; set; }

        [Log("Pagar antes da produção")]
        [PersistenceProperty("PAGAMENTOANTESPRODUCAO")]
        public bool PagamentoAntesProducao { get; set; }

        [Log("Ignorar bloq. ped não entr.")]
        [PersistenceProperty("IGNORARBLOQUEIOPEDPRONTO")]
        public bool IgnorarBloqueioPedPronto { get; set; }

        [Log("Bloquear receb. cheque limite")]
        [PersistenceProperty("BLOQUEARRECEBCHEQUELIMITE")]
        public bool BloquearRecebChequeLimite { get; set; }

        [Log("Bloquear receb. cheque próprio limite")]
        [PersistenceProperty("BLOQUEARRECEBCHEQUEPROPRIOLIMITE")]
        public bool BloquearRecebChequeProprioLimite { get; set; }

        private string _enderecoCobranca;

        [Log("Endereço Cobrança", true)]
        [PersistenceProperty("ENDERECOCOBRANCA")]
        public string EnderecoCobranca
        {
            get { return _enderecoCobranca == null ? String.Empty : _enderecoCobranca.ToUpper(); }
            set { _enderecoCobranca = value; }
        }

        private string _numeroCobranca;

        [Log("Número Cobrança", true)]
        [PersistenceProperty("NUMEROCOBRANCA")]
        public string NumeroCobranca
        {
            get { return _numeroCobranca == null ? String.Empty : _numeroCobranca; }
            set { _numeroCobranca = value; }
        }

        private string _complCobranca;

        [Log("Complemento Cobrança", true)]
        [PersistenceProperty("COMPLCOBRANCA")]
        public string ComplCobranca
        {
            get { return _complCobranca == null ? String.Empty : _complCobranca.ToUpper(); }
            set { _complCobranca = value; }
        }

        private string _bairroCobranca;

        [Log("Bairro Cobrança", true)]
        [PersistenceProperty("BAIRROCOBRANCA")]
        public string BairroCobranca
        {
            get { return _bairroCobranca == null ? String.Empty : _bairroCobranca.ToUpper(); }
            set { _bairroCobranca = value; }
        }

        private string _cepCobranca;

        [Log("CEP Cobrança")]
        [PersistenceProperty("CEPCOBRANCA")]
        public string CepCobranca
        {
            get { return _cepCobranca == null ? String.Empty : _cepCobranca; }
            set { _cepCobranca = value; }
        }

        [Log("Cidade Cobrança", true, "NomeCidadeUf", typeof(CidadeDAO))]
        [PersistenceProperty("IDCIDADECOBRANCA")]
        [PersistenceForeignKey(typeof(Cidade), "IdCidade")]
        public int? IdCidadeCobranca { get; set; }

        private string _cepEntrega;

        [Log("CEP Entrega")]
        [PersistenceProperty("CEPENTREGA")]
        public string CepEntrega
        {
          get { return _cepEntrega == null ? String.Empty : _cepEntrega; }
          set { _cepEntrega = value; }
        }

        private string _enderecoEntrega;

        [Log("Endereco Entrega", true)]
        [PersistenceProperty("ENDERECOENTREGA")]
        public string EnderecoEntrega
        {
          get { return _enderecoEntrega == null ? String.Empty : _enderecoEntrega; }
          set { _enderecoEntrega = value; }
        }

        private string _numeroEntrega;

        [Log("Numero Entrega", true)]
        [PersistenceProperty("NUMEROENTREGA")]
        public string NumeroEntrega
        {
          get { return _numeroEntrega == null ? String.Empty : _numeroEntrega; }
          set { _numeroEntrega = value; }
        }

        private string _complEntrega;

        [Log("Complemento Entrega", true)]
        [PersistenceProperty("COMPLENTREGA")]
        public string ComplEntrega
        {
          get { return _complEntrega == null ? String.Empty : _complEntrega; }
          set { _complEntrega = value; }
        }

        private string _bairroEntrega;

        [Log("Bairro Entrega", true)]
        [PersistenceProperty("BAIRROENTREGA")]
        public string BairroEntrega
        {
          get { return _bairroEntrega == null ? String.Empty : _bairroEntrega; }
          set { _bairroEntrega = value; }
        }

        [Log("Cidade Entrega", true, "NomeCidadeUf", typeof(CidadeDAO))]
        [PersistenceProperty("IDCIDADEENTREGA")]
        [PersistenceForeignKey(typeof(Cidade), "IdCidade")]
        public int? IdCidadeEntrega { get; set; }

        private string _cidadeEntrega;

        [PersistenceProperty("NOMECIDADEENTREGA", DirectionParameter.InputOptional)]
        public string CidadeEntrega
        {
            get { return _cidadeEntrega == null ? String.Empty : _cidadeEntrega.ToUpper(); }
            set { _cidadeEntrega = value; }
        }

        [Log("Não receb. e-mail ped. pronto")]
        [PersistenceProperty("NAORECEBEEMAILPEDPRONTO")]
        public bool NaoReceberEmailPedPronto { get; set; }

        [Log("Não receb. e-mail ped. PCP")]
        [PersistenceProperty("NAORECEBEEMAILPEDPCP")]
        public bool NaoReceberEmailPedPcp { get; set; }

        [Log("Não recebe e-mail fiscal")]
        [PersistenceProperty("NAORECEBEEMAILFISCAL")]
        public bool NaoReceberEmailFiscal { get; set; }

        [Log("Não recebe SMS")]
        [PersistenceProperty("NAORECEBESMS")]
        public bool NaoReceberSms { get; set; }

        [Log("Não recebe e-mail de cobrança vencida")]
        [PersistenceProperty("NAORECEBEREMAILCOBRANCAVENCIDA")]
        public bool NaoReceberEmailCobrancaVencida { get; set; }

        [Log("Não recebe e-mail de cobrança a vencer")]
        [PersistenceProperty("NAORECEBEREMAILCOBRANCAVENCER")]
        public bool NaoReceberEmailCobrancaVencer { get; set; }

        [Log("Perc. Comissão Vendedor")]
        [PersistenceProperty("PERCCOMISSAOFUNC")]
        public float PercComissaoFunc { get; set; }

        /// <summary>
        /// Define se o cliente será bloqueado caso possua alguma conta a receber vencida
        /// </summary>
        [Log("Bloquear pedido se houver conta vencida")]
        [PersistenceProperty("BLOQUEARPEDIDOCONTAVENCIDA")]
        public bool BloquearPedidoContaVencida { get; set; }

        /// <summary>
        /// Define o endereço do web service do cliente
        /// </summary>
        [Log("URL Sistema WebGlass")]
        [PersistenceProperty("URLSISTEMA")]
        public string UrlSistema { get; set; }

        [Log("Gerar Orçamento pelo PCP")]
        [PersistenceProperty("GERARORCAMENTOPCP")]
        public bool GerarOrcamentoPcp { get; set; }

        [Log("Produtor Rural")]
        [PersistenceProperty("PRODUTORRURAL")]
        public bool ProdutorRural { get; set; }

        [Log("Data da Última Consulta no Sintegra")]
        [PersistenceProperty("DTULTCONSINTEGRA")]
        public DateTime? DtUltConSintegra { get; set; }

        [Log("Usuário da Última Consulta no Sintegra")]
        [PersistenceProperty("USUARIOULTCONSINTEGRA")]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int? UsuUltConSintegra { get; set; }

        [PersistenceProperty("CRT")]
        public int Crt { get; set; }

        [Log("Controlar Estoque de Vidros do Cliente")]
        [PersistenceProperty("CONTROLARESTOQUEVIDROS")]
        public bool ControlarEstoqueVidros { get; set; }

        [Log("Observação para NF-e")]
        [PersistenceProperty("OBSNFE")]
        public string ObsNfe { get; set; }

        [Log("Não enviar e-mail ao liberar pedido")]
        [PersistenceProperty("NAOENVIAREMAILLIBERACAO")]
        public bool NaoEnviarEmailLiberacao { get; set; }

        /// <summary>
        /// Indica se o cliente pode gerar somente OC de transferencia
        /// </summary>
        [PersistenceProperty("SOMENTEOCTRANSFERENCIA")]
        public bool SomenteOcTransferencia { get; set; }

        [Log("Obs. Liberação")]
        [PersistenceProperty("OBSLIBERACAO")]
        public string ObsLiberacao { get; set; }

        [Log("Data Limite do Cad.")]
        [PersistenceProperty("DATALIMITECAD")]
        public DateTime? DataLimiteCad { get; set; }

        [Log("CNAE")]
        [PersistenceProperty("CNAE")]
        public string Cnae { get; set; }

        [PersistenceProperty("UsoLimite")]
        public decimal UsoLimite { get; set; }

        [PersistenceProperty("DATAATUALIZACAOUSOLIMITE")]
        public DateTime? DataAtualizacaoUsoLimite { get; set; }

        [PersistenceProperty("Importacao")]
        public bool Importacao { get; set; }

        [Log("Atendente", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNCATENDENTE")]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public uint? IdFuncAtendente { get; set; }

        [PersistenceProperty("HabilitarEditorCad")]
        public bool HabilitarEditorCad { get; set; }

        [PersistenceProperty("IdsSubgrupoProd")]
        public string IdsSubgrupoProd { get; set; }

        [PersistenceProperty("DescontoEcommerce")]
        public float? DescontoEcommerce { get; set; }

        /// <summary>
        /// Define se os pedidos emitidos para esse cliente
        /// serão contabilizados no SMS de resumo diário
        /// </summary>
        [Log("IgnorarNoSmsResumoDiario")]
        [PersistenceProperty("IgnorarNoSmsResumoDiario")]
        public bool IgnorarNoSmsResumoDiario { get; set; }

        [Log("Grupo do Cliente", "Descricao", typeof(GrupoClienteDAO))]
        [PersistenceProperty("IDGRUPOCLIENTE")]
        [PersistenceForeignKey(typeof(GrupoCliente), "IdGrupoCliente")]
        public int? IdGrupoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o Percentual da bonificação do cliente.
        /// </summary>
        [Log("Percentual de Bonificação", true)]
        [PersistenceProperty("PERCENTUALBONIFICACAO")]
        public decimal? PercentualBonificacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _cidade;

        [PersistenceProperty("NOMECIDADE", DirectionParameter.InputOptional)]
        public string Cidade
        {
            get { return _cidade == null ? String.Empty : _cidade.ToUpper(); }
            set { _cidade = value; }
        }

        private string _cidadeCobranca;

        [PersistenceProperty("NOMECIDADECOBRANCA", DirectionParameter.InputOptional)]
        public string CidadeCobranca
        {
            get { return _cidadeCobranca == null ? String.Empty : _cidadeCobranca.ToUpper(); }
            set { _cidadeCobranca = value; }
        }

        private string _uf;

        [PersistenceProperty("UF", DirectionParameter.InputOptional)]
        public string Uf
        {
            get { return _uf == null ? String.Empty : _uf.ToUpper(); }
            set { _uf = value; }
        }

        private string _ufCobranca;

        [PersistenceProperty("UFCOBRANCA", DirectionParameter.InputOptional)]
        public string UfCobranca
        {
            get { return _ufCobranca == null ? String.Empty : _ufCobranca.ToUpper(); }
            set { _ufCobranca = value; }
        }

        private string _ufEntrega;

        [PersistenceProperty("UFENTREGA", DirectionParameter.InputOptional)]
        public string UfEntrega
        {
            get { return _ufEntrega == null ? String.Empty : _ufEntrega.ToUpper(); }
            set { _ufEntrega = value; }
        }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("TotalCusto", DirectionParameter.InputOptional)]
        public decimal TotalCusto { get; set; }

        private string _descrUsuAlt;

        [PersistenceProperty("DescrUsuAlt", DirectionParameter.InputOptional)]
        public string DescrUsuAlt
        {
            get
            {
                try
                {
                    return !String.IsNullOrEmpty(_descrUsuAlt) ? BibliotecaTexto.GetTwoFirstNames(_descrUsuAlt) : _descrUsuAlt;
                }
                catch
                {
                    return _descrUsuAlt;
                }
            }
            set { _descrUsuAlt = value; }
        }

        [PersistenceProperty("NomeComissionado", DirectionParameter.InputOptional)]
        public string NomeComissionado { get; set; }

        [PersistenceProperty("CODIBGECIDADE", DirectionParameter.InputOptional)]
        public string CodIbgeCidade { get; set; }

        [PersistenceProperty("CODIBGEUF", DirectionParameter.InputOptional)]
        public string CodIbgeUf { get; set; }

        [PersistenceProperty("CODIBGECIDADECOBRANCA", DirectionParameter.InputOptional)]
        public string CodIbgeCidadeCobranca { get; set; }

        [PersistenceProperty("CODIBGEUFCOBRANCA", DirectionParameter.InputOptional)]
        public string CodIbgeUfCobranca { get; set; }

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("DescrTipoCliente", DirectionParameter.InputOptional)]
        public string DescrTipoCliente { get; set; }

        [PersistenceProperty("FORMAPAGAMENTO", DirectionParameter.InputOptional)]
        public string FormaPagamento { get; set; }

        [PersistenceProperty("TIPOCLIENTE", DirectionParameter.InputOptional)]
        public string TipoCliente { get; set; }

        [PersistenceProperty("LOJA", DirectionParameter.InputOptional)]
        public string Loja { get; set; }

        [PersistenceProperty("ROTA", DirectionParameter.InputOptional)]
        public string Rota { get; set; }

        [PersistenceProperty("CODIGOROTA", DirectionParameter.InputOptional)]
        public string CodigoRota { get; set; }

        [PersistenceProperty("PARCELA", DirectionParameter.InputOptional)]
        public string Parcela { get; set; }

        [PersistenceProperty("DescontoAcrescimo", DirectionParameter.InputOptional)]
        public string TabelaDescontoAcrescimo { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal LimiteUtilizado { get; set; }

        [Log("CRT")]
        public string DescrCrt
        {
            get
            {
                switch (Crt)
                {
                    case (int)CrtCliente.RegimeNormal: return "Regime Normal";
                    case (int)CrtCliente.SimplesNacional: return "Simples Nacional";
                    default: return String.Empty;
                }
            }
        }

        /// <summary>
        /// Usado para inserir/editar rota do cliente direto do cadastro de clientes
        /// </summary>
        [Log("Rota", true, "CodInterno", typeof(RotaDAO))]
        [PersistenceProperty("IDROTA", DirectionParameter.InputOptional)]
        public int? IdRota { get; set; }

        public decimal Lucro
        {
            get { return TotalComprado - (decimal)TotalCusto; }
        }

        public string DataNascString
        {
            get { return Conversoes.ConverteData(DataNasc, false); }
            set { DataNasc = Conversoes.ConverteData(value); }
        }

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case (int)SituacaoCliente.Ativo: return "Ativo";
                    case (int)SituacaoCliente.Inativo: return "Inativo";
                    case (int)SituacaoCliente.Cancelado: return "Cancelado";
                    case (int)SituacaoCliente.Bloqueado: return "Bloqueado";
                    default: return "";
                }
            }
        }

        [Log("Tipo de Pessoa")]
        public string DescrTipoPessoa
        {
            get { return TipoPessoa == "F" ? "PF" : "PJ"; }
        }

        public string EnderecoCompleto
        {
            get
            {
                string compl = String.IsNullOrEmpty(_compl) ? " " : " (" + _compl + ") ";

                return _endereco + (!String.IsNullOrEmpty(_numero) ? ", " + _numero : String.Empty) + compl + _bairro + " - " + _cidade + "/" + _uf;
            }
        }

        public string EnderecoCompletoCobranca
        {
            get
            {
                string complCobranca = String.IsNullOrEmpty(_complCobranca) ? " " : " (" + _complCobranca + ") ";

                return _enderecoCobranca + (!String.IsNullOrEmpty(_numeroCobranca) ? ", " + _numeroCobranca : String.Empty) + complCobranca + _bairroCobranca + " - " + _cidadeCobranca + "/" + _ufCobranca;
            }
        }

        public string EnderecoCompletoEntrega
        {
            get
            {
                string complEntrega = String.IsNullOrEmpty(_complEntrega) ? " " : " (" + _complEntrega + ") ";

                return _enderecoEntrega + (!String.IsNullOrEmpty(_numeroEntrega) ? ", " + _numeroEntrega : String.Empty) + complEntrega + _bairroEntrega + " - " + _cidadeEntrega + "/" + _ufEntrega;
            }
        }

        public string CodMunicipio
        {
            get { return CodIbgeUf + CodIbgeCidade; }
        }

        public string CodMunicipioCobranca
        {
            get { return CodIbgeUfCobranca + CodIbgeCidadeCobranca; }
        }

        public string IdNome
        {
            get { return IdCli.ToString() + " - " + _nome; }
        }

        public bool SituacaoEnabled
        {
            get
            {
                uint tipoFunc = Glass.Data.Helper.UserInfo.GetUserInfo.TipoUsuario;

                return Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) ||
                    Config.PossuiPermissao(Config.FuncaoMenuCadastro.AtivarInativarCliente);
            }
        }

        public string Telefone
        {
            get
            {
                if (!String.IsNullOrEmpty(TelCont))
                    return TelCont;
                else if (!String.IsNullOrEmpty(_telRes))
                    return _telRes;
                else if (!String.IsNullOrEmpty(_telCel))
                    return _telCel;
                else
                    return String.Empty;
            }
        }

        // Apenas administrador/usuário liberado pode excluir cliente
        public bool ExcluirVisible
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarCliente);
            }
        }

        // Apenas administrador/usuário liberado pode ver / inserir sugestões
        public bool SugestoesVisible
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes);
            }
        }

        // Apenas administrador pode dar desconto à cliente
        public bool DescontoVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                return Config.PossuiPermissao(Config.FuncaoMenuCadastro.DescontoAcrescimoProdutoCliente) && IdTabelaDesconto.GetValueOrDefault() == 0;
            }
        }

        // Apenas administrador pode inativar cliente pela lista
        public bool InativarVisible
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuCadastro.AtivarInativarCliente) && Situacao != (int)SituacaoCliente.Cancelado;
            }
        }

        public bool ExisteRotaVisible
        {
            get { return RotaDAO.Instance.GetCount() > 0; }
        }

        public bool ExistePrecoTabelaVisible
        {
            get { return TabelaDescontoAcrescimoClienteDAO.Instance.GetCountReal() > 0; }
        }

        private decimal? _debitos = null;

        public decimal Debitos
        {
            get
            {
                if (_debitos == null)
                    _debitos = ContasReceberDAO.Instance.GetDebitos((uint)IdCli, null);

                return _debitos.GetValueOrDefault(0);
            }
        }

        public string LimiteDisp
        {
            get
            {
                if (Limite == 0)
                    return "-";

                return (Limite - Debitos).ToString("C");
            }
        }

        /// <summary>
        /// Verifica se o usuário logado atualmente é administrador
        /// </summary>
        public bool IsAdministrador
        {
            get { return UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Administrador; }
        }

        /// <summary>
        /// Usado para exibir/esconder PercReducaoNfe no cadastro de cliente.
        /// </summary>
        public bool PercReducaoNfeVisible
        {
            get { return IsAdministrador || Config.PossuiPermissao(Config.FuncaoMenuCadastro.AlterarPercRedNfe); }
        }

        public bool FotosVisible
        {
            get { return Config.PossuiPermissao(Config.FuncaoMenuCadastro.AnexarArquivosCliente); }
        }

        public string SituacaoString
        {
            get { return Enum.GetName(typeof(SituacaoCliente), Situacao); }
        }

        [Log("Tipo Fiscal")]
        public string TipoFiscalString
        {
            get
            {
                return TipoFiscal == (int)TipoFiscalCliente.ConsumidorFinal ? "Consumidor Final" :
                    TipoFiscal == (int)TipoFiscalCliente.Revenda ? "Revenda" : String.Empty;
            }
        }

        public string FormasPagamento
        {
            get
            {
                var formasPagto = FormaPagtoDAO.Instance.GetByCliente((uint)IdCli).Select(f => f.Descricao);
                return string.Join(",", formasPagto.Select(i => i.ToString()).ToArray());
            }
        }

        #endregion

        #region IParticipanteNFe Members

        int Sync.Fiscal.EFD.Entidade.IParticipanteNFe.Codigo
        {
            get { return IdCli; }
        }

        int Sync.Fiscal.EFD.Entidade.IParticipanteNFe.CodigoCidade
        {
            get { return IdCidade ?? 0; }
        }

        Sync.Fiscal.Enumeracao.TipoPessoa Sync.Fiscal.EFD.Entidade.IParticipanteNFe.TipoPessoa
        {
            get { return TipoPessoa == "F" ? Sync.Fiscal.Enumeracao.TipoPessoa.Fisica : Sync.Fiscal.Enumeracao.TipoPessoa.Juridica; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.CpfCnpj
        {
            get { return CpfCnpj; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.InscricaoEstadual
        {
            get { return RgEscinst; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.Complemento
        {
            get { return Compl; }
        }

        #endregion
    }
}
