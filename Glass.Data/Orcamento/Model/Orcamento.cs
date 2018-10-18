using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Log;
using System.Xml.Serialization;
using Glass.Data.Model.Calculos;
using System.Collections.Generic;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(OrcamentoDAO))]
	[PersistenceClass("orcamento")]
	public class Orcamento : ModelBaseCadastro, IContainerCalculo
    {
        #region Construtores

        public Orcamento()
        {
            // Define valores padrão para as variáveis
            _tipoAcrescimo = 2;
            _tipoDesconto = 2;
        }

        #endregion

        #region Enumeradores

        public enum SituacaoOrcamento
        {
            EmAberto=1,
            Negociado,
            NaoNegociado,
            EmNegociacao,
            NegociadoParcialmente
        }

        public enum TipoEntregaOrcamento : int
        {
            Balcao = 1,
            Comum,
            Temperado,
            Entrega,
            ManutencaoTemperado,
            Esquadria
        }

        public enum TipoOrcamentoEnum
        {
            Venda = 1,
            Revenda
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDORCAMENTO", PersistenceParameterType.IdentityKey)]
        public uint IdOrcamento { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint? IdCliente { get; set; }

        [Log("Projeto")]
        [PersistenceProperty("IDPROJETO")]
        public uint? IdProjeto { get; set; }

        [Log("Funcionario", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        public uint? IdFuncionario { get; set; }

        [Log("Pedido Gerado")]
        [PersistenceProperty("IDPEDIDOGERADO")]
        public uint? IdPedidoGerado { get; set; }

        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("IdLoja")]
        public uint? IdLoja { get; set; }

        [Log("Comissionado", "Nome", typeof(ComissionadoDAO))]
        [PersistenceProperty("IDCOMISSIONADO")]
        public uint? IdComissionado { get; set; }

        [PersistenceProperty("IDORCAMENTOORIGINAL", DirectionParameter.OutputOnlyInsert)]
        public uint? IdOrcamentoOriginal { get; set; }

        [PersistenceProperty("IDPEDIDOESPELHO")]
        public uint? IdPedidoEspelho { get; set; }
        
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("DATASITUACAO")]
        public DateTime? DataSituacao { get; set; }

        [Log("A/C")]
        [PersistenceProperty("AOSCUIDADOS")]
        public string AosCuidados { get; set; }

		private string _nomeCliente;

        [Log("Nome Cliente")]
        [PersistenceProperty("NOMECLIENTE")]
		public string NomeCliente
		{
            get { return _nomeCliente != null ? _nomeCliente.ToUpper() : String.Empty; }
            set { _nomeCliente = value; }
		}

		private string _telCliente;

        [Log("Tel. Cliente")]
		[PersistenceProperty("TELCLIENTE")]
		public string TelCliente
		{
            get { return _telCliente != null ? _telCliente : ""; }
            set { _telCliente = value; }
		}

		private string _celCliente;

        [Log("Cel. Cliente")]
		[PersistenceProperty("CELCLIENTE")]
		public string CelCliente
		{
            get { return _celCliente != null ? _celCliente : String.Empty; }
            set { _celCliente = value; }
		}

        [Log("FAX")]
        [PersistenceProperty("FAX")]
        public string Fax { get; set; }

        [Log("Obs")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Obs Interna")]
        [PersistenceProperty("OBSINTERNA")]
        public string ObsInterna { get; set; }

		private string _formaPagto;

        [Log("Forma Pagto.")]
		[PersistenceProperty("FORMAPAGTO")]
		public string FormaPagto
		{
            get { return _formaPagto != null ? _formaPagto : ""; }
            set { _formaPagto = value; }
		}

        private string _email;

        [Log("Email")]
        [PersistenceProperty("Email")]
        public string Email
        {
            get { return _email != null ? _email : ""; }
            set { _email = value; }
        }

        private string _prazoEntrega;

        [Log("Prazo Entrega")]
        [PersistenceProperty("PrazoEntrega")]
        public string PrazoEntrega
        {
            get { return _prazoEntrega != null ? _prazoEntrega : ""; }
            set { _prazoEntrega = value; }
        }

        private string _obra;

        [Log("Obra")]
        [PersistenceProperty("Obra")]
        public string Obra
        {
            get { return _obra != null ? _obra : ""; }
            set { _obra = value; }
        }

        private string _contato;

        [Log("Contato")]
        [PersistenceProperty("Contato")]
        public string Contato
        {
            get { return _contato != null ? _contato : ""; }
            set { _contato = value; }
        }

        private string _Endereco;

        [Log("Endereço")]
        [PersistenceProperty("Endereco")]
        public string Endereco
        {
            get { return _Endereco != null ? _Endereco : ""; }
            set { _Endereco = value; }
        }

        private string _Bairro;

        [Log("Bairro")]
        [PersistenceProperty("Bairro")]
        public string Bairro
        {
            get { return _Bairro != null ? _Bairro : ""; }
            set { _Bairro = value; }
        }

        private string _Cidade;

        [Log("Cidade")]
        [PersistenceProperty("Cidade")]
        public string Cidade
        {
            get { return _Cidade != null ? _Cidade : ""; }
            set { _Cidade = value; }
        }

        private string _Cep;

        [Log("CEP")]
        [PersistenceProperty("Cep")]
        public string Cep
        {
            get { return _Cep != null ? _Cep : ""; }
            set { _Cep = value; }
        }

        [Log("Validade")]
        [PersistenceProperty("Validade")]
        public string Validade { get; set; }

        [PersistenceProperty("TOTAL", DirectionParameter.OutputOnlyInsert)]
        public decimal Total { get; set; }

        [PersistenceProperty("CUSTO")]
        public decimal Custo { get; set; }

        [Log("Perc. Comissão")]
        [PersistenceProperty("PERCCOMISSAO")]
        public float PercComissao { get; set; }

        [Log("Valor Comissão")]
        [PersistenceProperty("VALORCOMISSAO")]
        public decimal ValorComissao { get; set; }

        [PersistenceProperty("MOSTRARTOTAL")]
        public bool MostrarTotal { get; set; }

        [PersistenceProperty("MOSTRARTOTALPROD")]
        public bool MostrarTotalProd { get; set; }

        [PersistenceProperty("IMPRIMIRPRODUTOSORCAMENTO")]
        public bool ImprimirProdutosOrcamento { get; set; }

        [Log("Taxa Prazo")]
        [PersistenceProperty("TAXAPRAZO")]
        public float TaxaPrazo { get; set; }

        [Log("Desconto")]
        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        private int _tipoDesconto;

        [Log("Tipo Desconto")]
        [PersistenceProperty("TIPODESCONTO")]
        public int TipoDesconto
        {
            get
            {
                if (_tipoDesconto == 0) _tipoDesconto = 2;
                return _tipoDesconto;
            }
            set { _tipoDesconto = value; }
        }

        [PersistenceProperty("IDFUNCDESC", DirectionParameter.Input)]
        public uint? IdFuncDesc { get; set; }

        [Log("Acréscimo")]
        [PersistenceProperty("ACRESCIMO")]
        public decimal Acrescimo { get; set; }

        private int _tipoAcrescimo;

        [Log("Tipo Acréscimo")]
        [PersistenceProperty("TIPOACRESCIMO")]
        public int TipoAcrescimo
        {
            get
            {
                if (_tipoAcrescimo == 0) _tipoAcrescimo = 2;
                return _tipoAcrescimo;
            }
            set { _tipoAcrescimo = value; }
        }

        [Log("Tipo Entrega")]
        [PersistenceProperty("TIPOENTREGA")]
        public int? TipoEntrega { get; set; }

        [PersistenceProperty("ENDERECOOBRA")]
        public string EnderecoObra { get; set; }

        [PersistenceProperty("BAIRROOBRA")]
        public string BairroObra { get; set; }

        [PersistenceProperty("CIDADEOBRA")]
        public string CidadeObra { get; set; }

        [PersistenceProperty("CEPOBRA")]
        public string CepObra { get; set; }

        [PersistenceProperty("DATACAD")]
        public override DateTime DataCad { get; set; }

        [PersistenceProperty("ALIQUOTAICMS")]
        public float AliquotaIcms { get; set; }

        [PersistenceProperty("VALORICMS")]
        public decimal ValorIcms { get; set; }

        [PersistenceProperty("ALIQUOTAIPI")]
        public float AliquotaIpi { get; set; }

        [PersistenceProperty("VALORIPI")]
        public decimal ValorIpi { get; set; }

        [Log("Data Entrega")]
        [PersistenceProperty("DATAENTREGA")]
        public DateTime? DataEntrega { get; set; }

        [Log("Tipo Orçamento")]
        [PersistenceProperty("TIPOORCAMENTO")]
        public int? TipoOrcamento { get; set; }

        [Log("Valor do Frete")]
        [PersistenceProperty("ValorEntrega")]
        public decimal ValorEntrega { get; set; }

        [PersistenceProperty("NUMEROPARCELAS", DirectionParameter.Input)]
        public int NumeroParcelas { get; set; }

        [PersistenceProperty("DATARECALCULAR", DirectionParameter.Input)]
        public DateTime? DataRecalcular { get; set; }

        [PersistenceProperty("IDFUNCRECALCULAR", DirectionParameter.Input)]
        public uint? IdFuncRecalcular { get; set; }

        [PersistenceProperty("DATAALT", DirectionParameter.Input)]
        public DateTime? DataAlt { get; set; }

        [PersistenceProperty("USUALT", DirectionParameter.Input)]
        public uint? UsuAlt { get; set; }

        [PersistenceProperty("PESO", DirectionParameter.Input)]
        public float Peso { get; set; }

        /// <summary>
        /// 1-À Vista
        /// 2-À Prazo
        /// 3-Reposição
        /// 4-Garantia
        /// 5-Obra
        /// </summary>
        [PersistenceProperty("TIPOVENDA")]
        public int? TipoVenda { get; set; }

        [Log("Parcela", "Descricao", typeof(ParcelasDAO))]
        [PersistenceProperty("IDPARCELA")]
        public uint? IdParcela { get; set; }

        private int _numParc = 1;

        [Log("Núm. Parcelas")]
        [PersistenceProperty("NUMPARC")]
        public int NumParc
        {
            get { return _numParc; }
            set { _numParc = value; }
        }

        /// <summary>
        /// Percentual da rentabilidade.
        /// </summary>
        [PersistenceProperty("PERCENTUALRENTABILIDADE")]
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Valor da rentabilidade financeira.
        /// </summary>
        [PersistenceProperty("RENTABILIDADEFINANCEIRA")]
        public decimal RentabilidadeFinanceira { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CliRevenda", DirectionParameter.InputOptional)]
        public bool CliRevenda { get; set; }

        private string _nomeFuncionario;

        [PersistenceProperty("NOMEFUNCIONARIO",DirectionParameter.InputOptional)]
        public string NomeFuncionario
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeFuncionario); }
            set { _nomeFuncionario = value; }
        }

        [PersistenceProperty("EMAILFUNCIONARIO", DirectionParameter.InputOptional)]
        public string EmailFuncionario { get; set; }

        [PersistenceProperty("RAMALFUNC", DirectionParameter.InputOptional)]
        public string RamalFunc { get; set; }

        private string _nomeMedidor;

        [PersistenceProperty("NOMEMEDIDOR", DirectionParameter.InputOptional)]
        public string NomeMedidor
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeMedidor); }
            set { _nomeMedidor = value; }
        }

        private string _nomeComissionado;

        [PersistenceProperty("NOMECOMISSIONADO", DirectionParameter.InputOptional)]
        public string NomeComissionado
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeComissionado); }
            set { _nomeComissionado = value; }
        }

        private string _descrUsuAlt;

        [PersistenceProperty("DESCRUSUALT", DirectionParameter.InputOptional)]
        public string DescrUsuAlt
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_descrUsuAlt); }
            set { _descrUsuAlt = value; }
        }

        [PersistenceProperty("IdItensProjeto", DirectionParameter.InputOptional)]
        public string IdItensProjeto { get; set; }

        [PersistenceProperty("TotM", DirectionParameter.InputOptional)]
        public decimal TotM { get; set; }

        [PersistenceProperty("IdMedicaoDefinitiva", DirectionParameter.InputOptional)]
        public uint? IdMedicaoDefinitiva
        {
            get
            {
                return MedicaoDAO.Instance.ObterMedicaoDefinitivaPeloIdOrcamento((int)IdOrcamento);
            }
        }
            #region Loja

            [PersistenceProperty("CnpjLoja", DirectionParameter.InputOptional)]
        public string Cnpj { get; set; }

        [PersistenceProperty("InscEstLoja", DirectionParameter.InputOptional)]
        public string InscEst { get; set; }

        private string _nomeLoja;

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja
        {
            get { return _nomeLoja != null ? _nomeLoja : ""; }
            set { _nomeLoja = value; }
        }

        [PersistenceProperty("EmailLoja", DirectionParameter.InputOptional)]
        public string EmailLoja { get; set; }

        private string _telefoneLoja;

        [PersistenceProperty("TelefoneLoja", DirectionParameter.InputOptional)]
        public string TelefoneLoja
        {
            get { return _telefoneLoja != null ? _telefoneLoja : ""; }
            set { _telefoneLoja = value; }
        }

        private string _faxLoja;

        [PersistenceProperty("FaxLoja", DirectionParameter.InputOptional)]
        public string FaxLoja
        {
            get { return _faxLoja != null ? _faxLoja : ""; }
            set { _faxLoja = value; }
        }

        private string _rptEnderecoLoja;

        [PersistenceProperty("LogradouroLoja", DirectionParameter.InputOptional)]
        public string RptEnderecoLoja
        {
            get { return _rptEnderecoLoja != null ? _rptEnderecoLoja : String.Empty; }
            set { _rptEnderecoLoja = value; }
        }

        private string _rptComplLoja;

        [PersistenceProperty("ComplLoja", DirectionParameter.InputOptional)]
        public string RptComplLoja
        {
            get { return _rptComplLoja != null ? _rptComplLoja : String.Empty; }
            set { _rptComplLoja = value; }
        }

        private string _rptBairroLoja;

        [PersistenceProperty("BairroLoja", DirectionParameter.InputOptional)]
        public string RptBairroLoja
        {
            get { return _rptBairroLoja != null ? _rptBairroLoja : String.Empty; }
            set { _rptBairroLoja = value; }
        }

        private string _rptCidadeLoja;

        [PersistenceProperty("CidadeLoja", DirectionParameter.InputOptional)]
        public string RptCidadeLoja
        {
            get { return _rptCidadeLoja != null ? _rptCidadeLoja : String.Empty; }
            set { _rptCidadeLoja = value; }
        }

        [PersistenceProperty("UfLoja", DirectionParameter.InputOptional)]
        public string RptUfLoja { get; set; }

        [PersistenceProperty("CepLoja", DirectionParameter.InputOptional)]
        public string RptCepLoja { get; set; }

        private string _numeroLoja;

        [PersistenceProperty("NUMEROLOJA", DirectionParameter.InputOptional)]
        public string NumeroLoja
        {
            get { return _numeroLoja != null ? _numeroLoja : String.Empty; }
            set { _numeroLoja = value; }
        }

        #endregion

        #region Cliente

        [PersistenceProperty("CpfCnpjCliente", DirectionParameter.InputOptional)]
        public string CpfCnpjCliente { get; set; }

        [PersistenceProperty("InscEstCliente", DirectionParameter.InputOptional)]
        public string InscEstCliente { get; set; }
               
        [PersistenceProperty("ObsNfe", DirectionParameter.InputOptional)]
        public string ObsNfe { get; set; }

        #endregion

        [XmlIgnore]
        [PersistenceProperty("DescrObra", DirectionParameter.InputOptional)]
        public string DescrObra { get; set; }

        #endregion

        #region Propriedades de Suporte

        #region Métodos estáticos internos

        internal static decimal GetValorPerc(int tipoRetorno, int tipo, decimal valor, decimal total)
        {
            if (tipoRetorno == 1)
                return tipo == 1 ? valor : valor / total * 100;
            else
                return tipo == 1 ? total * (valor / 100) : valor;
        }

        internal static string GetTextoPerc(int tipo, decimal valor, decimal total)
        {
            return GetTextoPerc(tipo, valor, total, 0);
        }

        internal static string GetTextoPerc(int tipo, decimal valor, decimal total, decimal acrescimoDescontoOrcamento)
        {
            if (valor == 0)
                return 0.ToString("C") + " (0%)";

            decimal valorPorc =
                acrescimoDescontoOrcamento > 0 ?
                    acrescimoDescontoOrcamento : GetValorPerc(1, tipo, valor, total);
            decimal valorReal = GetValorPerc(2, tipo, valor, total);

            return valorReal.ToString("C") + " (" + valorPorc.ToString("0.#####") + "%)";
        }

        internal static string GetDescrSituacao(int situacao)
        {
            switch (situacao)
            {
                case 1:
                    return "Em Aberto";
                case 2:
                    return "Negociado";
                case 3:
                    return "Não Negociado";
                case 4:
                    return "Em Negociação";
                case 5:
                    return "Negociado parcialmente";
                default:
                    return String.Empty;
            }
        }

        #endregion

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }
        
        [Log("Situação")]
        public string DescrSituacao
        {
            get { return GetDescrSituacao(Situacao); }
        }

        // Apenas pedidos ativos podem ser editados
        public bool EditVisible
        {
            get
            {
                return OrcamentoDAO.Instance.VerificarPodeEditarOrcamento(IdLoja, IdFuncionario, IdPedidoGerado, Situacao);
            }
        }

        /// <summary>
        /// Define a visibilidade da impressão do orçamento
        /// </summary>
        public bool ExibirImpressao
        {
            get { return !OrcamentoConfig.RelatorioOrcamento.ApenasAdminImprimeOrcamento || UserInfo.GetUserInfo.IsAdministrador; }
        }

        public bool ExibirRelatorioCalculo
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuPedido.VisualizarMemoriaCalculo);
            }
        }

        public bool GerarPedidoVisible
        {
            get
            {
                return EditVisible && Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedido) &&
                    ((Situacao == (int)SituacaoOrcamento.Negociado && 
                    (IdPedidoGerado == null || IdPedidoGerado <= 0)) ||
                    Situacao == (int)SituacaoOrcamento.NegociadoParcialmente);
            }
        }

        public bool ExibirMedicaoDefinitiva
        {
            get
            {
                return OrcamentoConfig.OrcamentoGeraMedicaoDefinitiva && IdMedicaoDefinitiva == null &&
                    Geral.ControleMedicao && 
                    EditVisible && Situacao == (int)SituacaoOrcamento.Negociado;
            }
        }

        public bool ExibirLimparComissionado
        {
            get { return !PedidoConfig.Comissao.UsarComissionadoCliente; }
        }

        public string TelefoneRpt
        {
            get { return _telCliente + "   " + (!String.IsNullOrEmpty(_celCliente) ? _celCliente : String.Empty); }
        }

        public string TelVendedor
        {
            get
            {
                var retorno = "";

                if(IdFuncionario != null && IdFuncionario > 0)
                {
                    retorno = FuncionarioDAO.Instance.ObtemTelCel(IdFuncionario.Value);
                }

                return retorno;
            }
        }

        public string FoneFaxLoja
        {
            get { return "Fone: " + _telefoneLoja + (!String.IsNullOrEmpty(_faxLoja) ? " - Fax: " + _faxLoja : String.Empty); }
        }

        public string DataLocal { get; set; }

        public string DataOrcamento
        {
            get { return DataCad.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string NomeFuncAbrv
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeFuncionario); }
        }

        public string ContatoRel
        {
            get { return "Contato: " + _contato; }
        }

        public string ObsRel
        {
            get { return "Observações: " + Obs; }
        }

        public string ObservacaoConcatenada
        {
            get { return Obs + (!String.IsNullOrEmpty(Obs) ? " - " + ObsInterna : ObsInterna); }
        }

        public string NomeClienteLista
        {
            get { return IdCliente > 0 ? IdCliente.Value + " - " + _nomeCliente : _nomeCliente; }
        }

        public string TextoDesconto
        {
            get { return _tipoDesconto == 1 ? Desconto + "%" : Desconto.ToString("C"); }
        }

        private bool _somarDescontoProdutosTotal = true;

        public bool SomarDescontoProdutosTotal
        {
            get { return _somarDescontoProdutosTotal; }
            set { _somarDescontoProdutosTotal = value; }
        }

        public decimal DescontoTotal
        {
            get
            {
                return OrcamentoDAO.Instance.ObterDescontoOrcamento(null, (int)IdOrcamento) +
                    (_somarDescontoProdutosTotal ? OrcamentoDAO.Instance.ObterDescontoProdutos(null, (int)IdOrcamento) : 0);
            }
        }

        public string TextoDescontoTotal
        {
            get { return DescontoTotal.ToString("C"); }
        }

        public string TextoDescontoPerc
        {
            get { return GetTextoPerc(_tipoDesconto, Desconto, TotalSemDesconto); }
        }

        public string TextoDescontoTotalPerc
        {
            get
            {
                return GetTextoPerc(2, DescontoTotal, TotalSemDesconto, 0);
            }
        }

        public string TextoAcrescimo
        {
            get { return _tipoAcrescimo == 1 ? Acrescimo + "%" : Acrescimo.ToString("C"); }
        }

        public string TextoAcrescimoPerc
        {
            get { return GetTextoPerc(_tipoAcrescimo, Acrescimo, TotalSemAcrescimo); }
        }

        public string TextoAcrescimoTotalPerc
        {
            get
            {
                return GetTextoPerc(2,
                    OrcamentoDAO.Instance.ObterAcrescimoOrcamento(null, (int)IdOrcamento) +
                    OrcamentoDAO.Instance.ObterAcrescimoProdutos(null, (int)IdOrcamento), TotalSemAcrescimo, 0);
            }
        }

        public decimal TotalSemDesconto
        {
            get { return OrcamentoDAO.Instance.ObterTotalSemDesconto(null, (int)IdOrcamento, Total); }
        }

        public decimal TotalSemAcrescimo
        {
            get { return OrcamentoDAO.Instance.ObterTotalSemAcrescimo(null, (int)IdOrcamento, Total); }
        }

        public decimal TotalSemDescontoETaxaPrazo
        {
            get { return OrcamentoDAO.Instance.ObterTotalSemDescontoETaxaPrazo(null, (int)IdOrcamento, Total); }
        }

        public decimal TotalBruto
        {
            get { return OrcamentoDAO.Instance.ObterTotalBruto(null, (int)IdOrcamento); }
        }

        public string EnderecoLoja
        {
            get 
            { 
                return RptEnderecoLoja + " n.º " + NumeroLoja + ", " + RptBairroLoja + " - " + RptCidadeLoja;
            }
        }

        public string DadosLoja
        {
            get { return EnderecoLoja + " - " + FoneFaxLoja + " - " + EmailLoja; }
        }

        public string LocalObra
        {
            get 
            {
                string local = EnderecoObra + ", " + BairroObra + " - " + CidadeObra;

                return String.IsNullOrEmpty(local) ? String.Empty : local;
            }
        }

        public string DescrMostrarTotal
        {
            get { return MostrarTotal ? "Mostrar total na impressão" : ""; }
        }

        public string DescrMostrarTotalProd
        {
            get { return MostrarTotalProd ? "Mostrar total por produto" : ""; }
        }

        public bool EditEnabled
        {
            get { return OrcamentoDAO.Instance.EditEnabled(IdOrcamento); }
        }

        public bool ExibirImpressaoProjeto
        {
            get { return !String.IsNullOrEmpty(IdItensProjeto); }
        }

        public string NomeCompletoFuncionario
        {
            get { return _nomeFuncionario; }
        }

        public string DescrTipoOrcamento
        {
            get
            {
                return TipoOrcamento == (int)TipoOrcamentoEnum.Venda ? "Venda" :
                    TipoOrcamento == (int)TipoOrcamentoEnum.Revenda ? "Revenda" : "";
            }
        }

        public string CpfCnpj
        {
            get
            {
                return (IdCliente == null || IdCliente == 0) ? "" : ClienteDAO.Instance.GetElement((uint)IdCliente).CpfCnpj;
            }
        }

        public string IdsMedicao
        {
            get { return MedicaoDAO.Instance.ObterIdsMedicaoPeloIdOrcamento(null, (int)IdOrcamento); }
        }

        #region Tipo Entrega

        public string DescrTipoEntrega
        {
            get { return Utils.GetDescrTipoEntrega(TipoEntrega); }
        }

        #endregion

        [PersistenceProperty("QTDEPECAS", DirectionParameter.InputOptional)]
        public long QtdePecas { get; set; }

        public string ObsCliente
        {
            get
            {
                if (IdCliente.GetValueOrDefault() == 0)
                    return string.Empty;

                var obs = ClienteDAO.Instance.ObterObsPedido(IdCliente.GetValueOrDefault());

                if (obs.Split(';')[0] == "Erro")
                    return obs.Split(';')[1];

                return obs;
            }
        }

        #endregion

        #region IContainerCalculo

        uint IContainerCalculo.Id
        {
            get { return IdOrcamento; }
        }

        private IDadosCliente cliente;

        IDadosCliente IContainerCalculo.Cliente
        {
            get
            {
                if (cliente == null)
                {
                    cliente = new ClienteDTO(() => IdCliente ?? 0);
                }

                return cliente;
            }
        }

        private IDadosAmbiente ambientes;

        IDadosAmbiente IContainerCalculo.Ambientes
        {
            get
            {
                if (ambientes == null)
                {
                    ambientes = new DadosAmbienteDTO(
                        this,
                        () => ProdutosOrcamentoDAO.Instance.ObterProdutosAmbienteOrcamento(null, (int)IdOrcamento)
                    );
                }

                return ambientes;
            }
        }

        uint? IContainerCalculo.IdObra
        {
            get { return null; }
        }

        int? IContainerCalculo.TipoEntrega
        {
            get { return TipoEntrega; }
        }

        int? IContainerCalculo.TipoVenda
        {
            get { return TipoVenda; }
        }

        bool IContainerCalculo.Reposicao
        {
            get { return TipoVenda == (int)Pedido.TipoVendaPedido.Reposição; }
        }

        bool IContainerCalculo.MaoDeObra
        {
            get { return false; }
        }

        bool IContainerCalculo.IsPedidoProducaoCorte
        {
            get { return false; }
        }

        uint? IContainerCalculo.IdParcela
        {
            get { return IdParcela; }
        }

        #endregion
    }
}