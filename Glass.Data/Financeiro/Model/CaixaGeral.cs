using System;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CaixaGeralDAO))]
	[PersistenceClass("caixa_geral")]
	public class CaixaGeral : ModelBaseCadastro
    {
        #region Enumeradores

        public enum FormaSaidaEnum
        {
            Dinheiro=1,
            Cheque
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCAIXAGERAL", PersistenceParameterType.IdentityKey)]
        public uint IdCaixaGeral { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint? IdCliente { get; set; }

        [Log("Fornecedor", "Nomefantasia", typeof(FornecedorDAO))]
        [PersistenceProperty("IDFORNEC")]
        public uint? IdFornec { get; set; }

        [PersistenceProperty("idPedido")]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDSINAL")]
        public uint? IdSinal { get; set; }

        [PersistenceProperty("IDSINALCOMPRA")]
        public uint? IdSinalCompra { get; set; }

        [PersistenceProperty("idCompra")]
        public uint? IdCompra { get; set; }

        [PersistenceProperty("IDACERTO")]
        public uint? IdAcerto { get; set; }

        [PersistenceProperty("IDCONTAPG")]
        public uint? IdContaPg { get; set; }

        [PersistenceProperty("IDCONTAR")]
        public uint? IdContaR { get; set; }

        /// <summary>
        /// Campo utilizado para identificar qual cheque devolvido esta sendo pago com esta movimentação
        /// </summary>
        [PersistenceProperty("IDCHEQUE")]
        public uint? IdCheque { get; set; }

        [PersistenceProperty("IDPAGTO")]
        public uint? IdPagto { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("IDOBRA")]
        public uint? IdObra { get; set; }

        [PersistenceProperty("IDANTECIPFORNEC")]
        public uint? IdAntecipFornec { get; set; }

        [PersistenceProperty("IDACERTOCHEQUE")]
        public uint? IdAcertoCheque { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint? IdContaBanco { get; set; }

        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public uint? IdTrocaDevolucao { get; set; }

        [PersistenceProperty("IDDEPOSITO")]
        public uint? IdDeposito { get; set; }     

        private uint? _idLoja;

        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        public uint? IdLoja
        {
            get 
            { 
                if (_idLoja == null)
                    _idLoja = FuncionarioDAO.Instance.ObtemIdLoja((uint)Usucad);

                return _idLoja;
            }
            set { _idLoja = value; }
        }

        [Log("Plano Conta", "Descricao", typeof(PlanoContasDAO))]
        [PersistenceProperty("IDCONTA")]
        public uint IdConta { get; set; }

        [PersistenceProperty("IDDEVOLUCAOPAGTO")]
        public uint? IdDevolucaoPagto { get; set; }

        /// <summary>
        /// 1-Entrada, 2-Saída
        /// </summary>
        [PersistenceProperty("TIPOMOV")]
        public int TipoMov { get; set; }

        /// <summary>
        /// 1-Dinheiro
        /// 2-Cheque
        /// </summary>
        [PersistenceProperty("FORMASAIDA")]
        public int FormaSaida { get; set; }

        [Log("Valor")]
        [PersistenceProperty("VALORMOV")]
        public decimal ValorMov { get; set; }

        [Log("Juros")]
        [PersistenceProperty("JUROS")]
        public decimal Juros { get; set; }

        [Log("Data")]
        [PersistenceProperty("DATAMOV")]
        public DateTime DataMov { get; set; }

        [PersistenceProperty("DATAMOVBANCO")]
        public DateTime? DataMovBanco { get; set; }

        [Log("Saldo")]
        [PersistenceProperty("SALDO")]
        public decimal Saldo { get; set; }

        [Log("Núm. Aut. Construcard")]
        [PersistenceProperty("NUMAUTCONSTRUCARD")]
        public string NumAutConstrucard { get; set; }

        [PersistenceProperty("LANCMANUAL")]
        public bool LancManual { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("IDCREDFORNEC")]
        public uint? IdCreditoFornecedor { get; set; }

        [PersistenceProperty("IDCARTAONAOIDENTIFICADO")]
        public uint? IdCartaoNaoIdentificado { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("DescrGrupoConta", DirectionParameter.InputOptional)]
        public string DescrGrupoConta { get; set; }

        private string _descrPlanoConta;

        [PersistenceProperty("DescrPlanoConta", DirectionParameter.InputOptional)]
        public string DescrPlanoConta
        {
            get 
            { 
                return !String.IsNullOrEmpty(_descrPlanoConta) ? 
                    _descrPlanoConta + (
                        FormaSaida == 1 && !_descrPlanoConta.ToLower().Contains("dinheiro") ? " (Dinheiro)" :
                        FormaSaida == 2 && !_descrPlanoConta.ToLower().Contains("cheque") ? " (Cheque)" : ""
                        ) : 
                    _descrPlanoConta;
            }
            set { _descrPlanoConta = value; }
        }

        private string _nomeFornecedor;

        [PersistenceProperty("NomeFornecedor", DirectionParameter.InputOptional)]
        public string NomeFornecedor
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFornecedor); }
            set { _nomeFornecedor = value; }
        }

        private string _nomeCliente;

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente
        {
            get { return BibliotecaTexto.GetThreeFirstWords(_nomeCliente); }
            set { _nomeCliente = value; }
        }

        [PersistenceProperty("DescrUsuCad", DirectionParameter.InputOptional)]
        public override string DescrUsuCad
        {
            get { return BibliotecaTexto.GetFirstName(base.DescrUsuCad); }
            set { base.DescrUsuCad = value; }
        }

        [PersistenceProperty("DESCRICAOCONTARECEBERCONTABIL", DirectionParameter.InputOptional)]
        public string DescricaoContaReceberContabil { get; set; }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Usado para esconder este valor caso seja SALDO ANTERIOR
        /// </summary>
        public uint? CodMov
        {
            get { return IdCaixaGeral > 0 ? (uint?)IdCaixaGeral : null; }
        }

        /// <summary>
        /// Usado para esconder este valor caso seja SALDO ANTERIOR
        /// </summary>
        public decimal? ValorString
        {
            get { return ValorMov != 0 ? (decimal?)ValorMov : null; }
        }

        /// <summary>
        /// Usado para esconder este valor caso seja SALDO ANTERIOR
        /// </summary>
        public decimal? JurosString
        {
            get { return Juros > 0 ? (decimal?)Juros : null; }
        }

        /// <summary>
        /// Usado para esconder este valor caso seja SALDO ANTERIOR
        /// </summary>
        public DateTime? DataMovString
        {
            get { return DataMov.Year > 1 ? (DateTime?)DataMov : null; }
        }

        [Log("Tipo Mov.")]
        public string DescrTipoMov
        {
            get { return TipoMov == 1 ? "Entrada" : "Saída"; }
        }

        [Log("Forma Saída")]
        public string DescrFormaSaida
        {
            get { return FormaSaida == 1 ? "Dinheiro" : FormaSaida == 2 ? "Cheque" : ""; }
        }

        [Log("Referência")]
        public string Referencia
        {
            get
            {
                if (Obs == "SALDO ANTERIOR" || Obs == "SALDO ATUAL")
                    return String.Empty;

                string refer = String.Empty;

                if (IdAcerto > 0)
                    refer += "Acerto: " + IdAcerto + " ";

                if (IdAcertoCheque > 0)
                    refer += "Acerto Cheque: " + IdAcertoCheque + " ";

                if (IdDeposito > 0)
                    refer += "Depósito: " + IdDeposito + " ";

                if (IdCheque > 0)
                    refer += "Cheque: " + ChequesDAO.Instance.ObtemNumCheque(IdCheque.Value) + " ";

                if (IdCompra > 0)
                    refer += "Compra: " + IdCompra + " ";

                if (IdPedido > 0)
                    refer += "Pedido: " + IdPedido + " ";

                if (IdLiberarPedido > 0)
                    refer += "Liberação: " + IdLiberarPedido + " ";

                if (IdPagto > 0)
                    refer += "Pagto: " + IdPagto + " ";

                if (IdObra > 0)
                    refer += "Obra: " + IdObra + " ";

                if (IdAntecipFornec > 0)
                    refer += "Antecipação de fornecedor: " + IdAntecipFornec + " ";

                if (IdTrocaDevolucao > 0)
                    refer += "Troca/Devolução: " + IdTrocaDevolucao + " ";

                if (IdDevolucaoPagto > 0)
                    refer += "Devolução de pagto.: " + IdDevolucaoPagto + " ";

                if (IdSinal > 0)
                    refer += SinalDAO.Instance.GetReferencia(IdSinal.Value) + " ";

                if (IdSinalCompra > 0)
                    refer += "Sinal da Compra: " + IdSinalCompra + " ";

                if (IdCreditoFornecedor > 0)
                    refer += "Créd. Fornecedor: " + IdCreditoFornecedor + " ";

                if (IdCartaoNaoIdentificado > 0)
                    refer += "Cartão não Identificado: " + IdCartaoNaoIdentificado + " ";

                if (IdContaR > 0)
                    refer += ContasReceberDAO.Instance.GetReferencia(IdContaR.Value) +
                        (String.IsNullOrEmpty(DescricaoContaReceberContabil) ? "" :
                        String.Format(" ({0})", DescricaoContaReceberContabil)) + " ";                

                return refer;
            }
        }

        public string PlanoContaObs
        {
            get 
            {
                if (Obs == "SALDO ANTERIOR" || Obs == "SALDO ATUAL")
                    return Obs;

                string descrPlanoConta = DescrPlanoConta;

                if (IdContaBanco > 0)
                    descrPlanoConta += "(" + ContaBancoDAO.Instance.GetDescricao(IdContaBanco.Value) + ")";

                if (!String.IsNullOrEmpty(Obs))
                    descrPlanoConta += ". Obs: " + Obs;

                return descrPlanoConta; 
            }
        }

        public string RptData
        {
            get { return DateTime.Now.ToString("dd/MM/yyyy HH:mm"); }
        }

        /// <summary>
        /// Controla se o total de dinheiro, cheque e cheques de terc em aberto aparecerá
        /// </summary>
        public bool MostrarTotalGeral { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        public decimal TotalCheque { get; set; }

        public decimal TotalChequeDevolvido { get; set; }

        public decimal TotalChequeReapresentado { get; set; }

        public decimal TotalDinheiro { get; set; }

        public decimal TotalChequeTerc { get; set; }

        public decimal TotalEntradaDinheiro { get; set; }

        public decimal TotalEntradaCheque { get; set; }

        public decimal TotalEntradaCartao { get; set; }

        public decimal TotalEntradaConstrucard { get; set; }

        public decimal TotalEntradaPermuta { get; set; }

        public decimal TotalEntradaDeposito { get; set; }

        public decimal TotalEntradaBoleto { get; set; }

        public decimal TotalEntradas
        {
            get
            {
                return TotalEntradaDinheiro + TotalEntradaCheque + TotalEntradaCartao +
                    TotalEntradaConstrucard + TotalEntradaDeposito + TotalEntradaBoleto + TotalEntradaPermuta;
            }
        }

        public bool ExibirTotalEntrada
        {
            get { return false; }
        }

        public decimal TotalCreditoRecebido { get; set; }

        public decimal TotalSaidaDinheiro { get; set; }

        public decimal TotalSaidaCheque { get; set; }

        public decimal TotalSaidaCartao { get; set; }

        public decimal TotalSaidaConstrucard { get; set; }

        public decimal TotalSaidaPermuta { get; set; }

        public decimal TotalSaidaDeposito { get; set; }

        public decimal TotalSaidaBoleto { get; set; }

        public decimal TotalCreditoGerado { get; set; }

        public decimal TotalContasRecebidasContabeis { get; set; }

        public decimal TotalContasRecebidasNaoContabeis { get; set; }

        public decimal SaldoDinheiro { get; set; }

        public decimal SaldoCheque { get; set; }

        public decimal SaldoCartao { get; set; }

        public decimal SaldoConstrucard { get; set; }

        public decimal SaldoPermuta { get; set; }

        public decimal SaldoDeposito { get; set; }

        public decimal SaldoBoleto { get; set; }

        public decimal ContasReceberGeradas { get; set; }

        public string ClienteFornecedor
        {
            get 
            {
                var clienteFornecedor = IdCliente > 0 ? IdCliente + " - " + _nomeCliente : 
                    IdFornec > 0 ? IdFornec + " - " + _nomeFornecedor : string.Empty;

                if (string.IsNullOrEmpty(clienteFornecedor) && IdCheque > 0)
                {
                    var idCliente = ChequesDAO.Instance.ObtemIdCliente(IdCheque.Value);
                    if (idCliente > 0)
                        clienteFornecedor = string.Format("{0} - {1}", idCliente, ClienteDAO.Instance.GetNome(idCliente));
                }

                return clienteFornecedor;
            }
        }

        public DateTime DataMovExibir
        {
            get { return DataMovBanco != null ? DataMovBanco.Value : DataMov; }
        }

        #endregion
    }
}