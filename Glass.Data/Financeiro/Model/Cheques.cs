using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ChequesDAO))]
	[PersistenceClass("cheques")]
	public class Cheques : ModelBaseCadastro
    {
        #region Enumeradores

        public enum SituacaoCheque : int
        {
            EmAberto = 1,
            Compensado,
            Devolvido,
            Quitado,
            Cancelado,
            Trocado,
            Protestado
        }

        public enum OrigemCheque : int
        {
            Entrada = 1,
            PagtoAVista,
            Parcela,
            PagtoConta,
            FinanceiroPagto,
            PagtoChequeDevolvido
        }

        public enum TipoCheque : int
        {
            Proprio = 1,
            Terceiros,
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCHEQUE", PersistenceParameterType.IdentityKey)]
        public uint IdCheque { get; set; }

        [Log("Loja", "Nome", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint? IdCliente { get; set; }

        [Log("Acerto")]
        [PersistenceProperty("IDACERTO")]
        public uint? IdAcerto { get; set; }

        [Log("Pedido")]
        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [Log("Sinal")]
        [PersistenceProperty("IDSINAL")]
        public uint? IdSinal { get; set; }

        [Log("Sinal da Compra")]
        [PersistenceProperty("IDSINALCOMPRA")]
        public uint? IdSinalCompra { get; set; }

        [Log("Antecip. Pagto. Fornecedor")]
        [PersistenceProperty("IDANTECIPFORNEC")]
        public uint? IdAntecipFornec { get; set; }

        [Log("Depósito de Cheque")]
        [PersistenceProperty("IDDEPOSITO")]
        public uint? IdDeposito { get; set; }

        [Log("Conta a Receber", "Referencia", typeof(ContasReceberDAO))]
        [PersistenceProperty("IDCONTAR")]
        public uint? IdContaR { get; set; }

        [Log("Acerto de Cheque")]
        [PersistenceProperty("IDACERTOCHEQUE")]
        public uint? IdAcertoCheque { get; set; }

        [Log("Liberação de Pedido")]
        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [Log("Troca/Devolução")]
        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public uint? IdTrocaDevolucao { get; set; }

        [Log("Obra")]
        [PersistenceProperty("IDOBRA")]
        public uint? IdObra { get; set; }

        [Log("Devolução de Pagamento")]
        [PersistenceProperty("IDDEVOLUCAOPAGTO")]
        public uint? IdDevolucaoPagto { get; set; }

        private uint? _idContaBanco;

        [Log("Conta Bancária", "Descricao", typeof(ContaBancoDAO))]
        [PersistenceProperty("IDCONTABANCO")]
        public uint? IdContaBanco
        {
            get
            {
                if (Tipo == 1 && (_idContaBanco == null || _idContaBanco == 0))
                {
                    try { _idContaBanco = ContaBancoDAO.Instance.GetIdByAgenciaConta(_agencia, _conta); }
                    catch { _idContaBanco = null; }
                }

                return _idContaBanco;
            }
            set { _idContaBanco = value; }
        }

        [Log("Número")]
        [PersistenceProperty("NUM")]
        public int Num { get; set; }

        [Log("Dígito Num. Cheque")]
        [PersistenceProperty("DIGITONUM")]
        public string DigitoNum { get; set; }

		private string _banco;

        [Log("Banco")]
		[PersistenceProperty("BANCO")]
		public string Banco
		{
            get { return _banco != null ? _banco.ToUpper() : String.Empty; }
			set { _banco = value; }
		}

		private string _agencia;

        [Log("Agência")]
		[PersistenceProperty("AGENCIA")]
		public string Agencia
		{
            get { return _agencia != null ? _agencia : String.Empty; }
			set { _agencia = value; }
		}

		private string _conta;

        [Log("Conta")]
		[PersistenceProperty("CONTA")]
		public string Conta
		{
            get { return _conta != null ? _conta : String.Empty; }
			set { _conta = value; }
		}

		private string _titular;

        [Log("Titular")]
		[PersistenceProperty("TITULAR")]
		public string Titular
		{
            get { return _titular != null ? _titular.Replace("'", "").Replace("\"", "").ToUpper() : String.Empty; }
			set { _titular = value; }
		}

        [Log("Valor")]
        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [Log("Data de vencimento")]
        [PersistenceProperty("DATAVENC")]
        public DateTime? DataVenc { get; set; }

        /// <summary>
        /// Valor recebido pelo cheque devolvido
        /// </summary>
        [Log("Valor recebido")]
        [PersistenceProperty("VALORRECEB")]
        public decimal ValorReceb { get; set; }

        /// <summary>
        /// Juros recebido pelo cheque devolvido
        /// </summary>
        [Log("Juros recebidos")]
        [PersistenceProperty("JUROSRECEB")]
        public decimal JurosReceb { get; set; }

        /// <summary>
        /// Desconto do recebimnento de acerto de cheque.
        /// </summary>
        [Log("Desconto recebido")]
        [PersistenceProperty("DESCONTORECEB")]
        public decimal DescontoReceb { get; set; }

        /// <summary>
        /// Data que o cheque devolvido/próprio foi pago
        /// </summary>
        [Log("Data de recebimento")]
        [PersistenceProperty("DATARECEB")]
        public DateTime? DataReceb { get; set; }

        /// <summary>
        /// Juros rateado pago/a ser pago de uma pagamento
        /// </summary>
        [Log("Juros pagos")]
        [PersistenceProperty("JUROSPAGTO")]
        public decimal JurosPagto { get; set; }

        /// <summary>
        /// Multa rateado pago/a ser pago de um pagamento
        /// </summary>
        [Log("Multa paga")]
        [PersistenceProperty("MULTAPAGTO")]
        public decimal MultaPagto { get; set; }

        /// <summary>
        /// 1 - Entrada
        /// 2 - Pagto a vista
        /// 3 - Parcela
        /// 4 - Pagto Conta
        /// 5 - Financeiro Pagto
        /// 6 - Pagto Cheque Devolvido
        /// </summary>
        [PersistenceProperty("ORIGEM")]
        public int Origem { get; set; }

        /// <summary>
        /// 1 - Em aberto
        /// 2 - Compensado
        /// 3 - Devolvido
        /// 4 - Quitado
        /// 5 - Cancelado
        /// 6 - Trocado
        /// 7 - Protestado
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        /// <summary>
        /// 1-Próprio
        /// 2-Terceiro
        /// </summary>
        [PersistenceProperty("TIPO")]
        public int Tipo { get; set; }

        private string _obs;

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs
        {
            get { return _obs == null ? String.Empty : _obs; }
            set { _obs = value; }
        }

        [Log("Mov. Caixa Financeiro")]
        [PersistenceProperty("MOVCAIXAFINANCEIRO")]
        public bool MovCaixaFinanceiro { get; set; }

        [Log("Mov. Banco Financeiro")]
        [PersistenceProperty("MOVBANCO")]
        public bool MovBanco { get; set; }

        [Log("Data Venc. Original")]
        [PersistenceProperty("DATAVENCORIGINAL")]
        public DateTime? DataVencOriginal { get; set; }

        [Log("Reapresentado")]
        [PersistenceProperty("REAPRESENTADO", DirectionParameter.Input)]
        public bool Reapresentado { get; set; }

        [Log("Advogado")]
        [PersistenceProperty("ADVOGADO", DirectionParameter.Input)]
        public bool Advogado { get; set; }

        [PersistenceProperty("IDDEPOSITOCANC")]
        public uint? IdDepositoCanc { get; set; }

        [Log("Cancelou Devolução")]
        [PersistenceProperty("CANCELOUDEVOLUCAO")]
        public bool CancelouDevolucao { get; set; }

        [Log("Crédito de Fornecedor")]
        [PersistenceProperty("IDCREDITOFORNECEDOR")]
        public uint? IdCreditoFornecedor { get; set; }

        [PersistenceProperty("CPFCNPJ")]
        public string CpfCnpj { get; set; }

        [Log("Mov. Caixa Diario")]
        [PersistenceProperty("MOVCAIXADIARIO")]
        public bool MovCaixaDiario { get; set; }

        [Log("CMC7")]
        [PersistenceProperty("CMC7")]
        public string Cmc7 { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("IDFORNECEDOR", DirectionParameter.InputOptional)]
        public long IdFornecedor { get; set; }

        [PersistenceProperty("NOMEFORNECEDOR", DirectionParameter.InputOptional)]
        public string NomeFornecedor { get; set; }

        /// <summary>
        /// Usado no cadastro de cheques no financeiro pagto.
        /// </summary>
        [PersistenceProperty("IDCONTA", DirectionParameter.InputOptional)]
        public uint? IdConta { get; set; }

        [PersistenceProperty("DESCRPLANOCONTA", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        [PersistenceProperty("DESCRCONTABANCO", DirectionParameter.InputOptional)]
        public string DescrContaBanco { get; set; }

        [PersistenceProperty("IDPAGTO", DirectionParameter.InputOptional)]
        public uint? IdPagto { get; set; }

        [PersistenceProperty("IDSCOMPRA", DirectionParameter.InputOptional)]
        public string IdsCompra { get; set; }

        [PersistenceProperty("IDSNF", DirectionParameter.InputOptional)]
        public string IdsNf { get; set; }

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        /// <summary>
        /// Usado no relatório de Acerto de Cheques Devolvidoa/Aberto, salva o id do acerto
        /// de cheques em que o cheque foi acertado.
        /// </summary>
        [PersistenceProperty("IDACERTOCHEQUEACERTADO", DirectionParameter.InputOptional)]
        public uint? IdAcertoChequeAcertado { get; set; }

        [PersistenceProperty("VALORACERTOCHEQUE", DirectionParameter.InputOptional)]
        public decimal ValorAcertoCheque { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NumChequeComDig
        {
            get { return Num + (!String.IsNullOrEmpty(DigitoNum) ? "-" + DigitoNum : ""); }
        }

        /// <summary>
        /// Verifia se a data de vencimento do cheque pode ser alterada
        /// </summary>
        public bool AlterarDataVenc
        {
            get
            {
                return Situacao == (int)SituacaoCheque.EmAberto;
            }
        }

        /// <summary>
        /// Atributo utilizado para marcar a data que cheque próprio foi quitado,
        /// utilizado apenas na VidroValle e na tela CadDebitarCheque.aspx
        /// </summary>
        public string DataQuitChequeProprio { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        public string DescrCheque
        {
            get
            {
                return "Num.: " + Num + " " + _banco + " Agência: " + _agencia + " Conta: " + _conta;
            }
        }

        public decimal ValorRestante
        {
            get { return Valor - ValorReceb - DescontoReceb; }
        }

        public string ValorRecebido
        {
            get
            {
                return (Situacao == (int)SituacaoCheque.Devolvido || Situacao == (int)SituacaoCheque.Protestado || Situacao == (int)SituacaoCheque.Trocado) && ValorReceb > 0 ?
                    Valor.ToString("C") + " (Rec.: " + ValorReceb.ToString("C") + (DescontoReceb > 0 ? " Desc.: " + DescontoReceb.ToString("C") : "") + ")" : Valor.ToString("C");
            }
        }

        [Log("Origem")]
        public string DescrOrigem
        {
            get
            {
                switch (Origem)
                {
                    case 1: return "Entrada";
                    case 2: return "Pagamento à Vista";
                    case 3: return "Parcela";
                    case 4: return "Pagamento de Conta";
                    case 5: return "Financeiro";
                    case 6: return "Pagamento Cheque Devolvido";
                    default: return "";
                }
            }
        }

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case 1:
                        return "Em Aberto";
                    case 2:
                        return "Compensado";
                    case 3:
                        return Reapresentado ? "Reapresentado" : Advogado ? "Advogado" : "Devolvido";
                    case 4:
                        return "Quitado";
                    case 5:
                        return "Cancelado";
                    case 6:
                        return "Trocado";
                    case 7:
                        return Advogado ? "Advogado" : "Protestado";
                    default:
                        return String.Empty;
                }
            }
        }

        [Log("Tipo")]
        public string DescrTipo
        {
            get
            {
                switch (Tipo)
                {
                    case 1: return "Próprio";
                    case 2: return "Terceiro";
                    default: return "";
                }
            }
        }

        public bool DevolvidoVisible
        {
            get { return Situacao == (int)SituacaoCheque.EmAberto; }
        }

        public bool CompensadoVisible
        {
            get { return Situacao == (int)SituacaoCheque.EmAberto; }
        }

        public bool EditDeleteVisible
        {
            get
            {
                return Situacao == (int)SituacaoCheque.EmAberto ||
                    (Situacao == (int)SituacaoCheque.Protestado && Origem == (int)OrigemCheque.FinanceiroPagto &&
                    IdDeposito == null && PagtoChequeDAO.Instance.GetPagtoByCheque(IdCheque) == 0);
            }
        }

        public bool CancelarReapresentadoVisible
        {
            get { return Situacao == (int)SituacaoCheque.Devolvido && Reapresentado; }
        }

        public bool LocalizacaoVisible
        {
            get { return Situacao != (int)SituacaoCheque.EmAberto;}
        }

        public string IdNomeCliente
        {
            get { return IdCliente > 0 ? IdCliente + " - " + NomeCliente : ""; }
        }

        public string IdNomeFornecedor
        {
            get { return IdFornecedor > 0 ? IdFornecedor + " - " + NomeFornecedor : ""; }
        }

        public string Referencia
        {
            get
            {
                string refer = String.Empty;
                string idsNfRec = Configuracoes.FinanceiroConfig.ExibirReferenciaDeNotaListaCheques ?
                    ChequesDAO.Instance.ObtemIdsNfRecebimento(IdLiberarPedido, IdPedido, IdAcerto) :
                    String.Empty;

                if (IdAcerto > 0)
                    refer += "Acerto: " + IdAcerto + " ";

                if (IdPedido > 0)
                    refer += "Pedido: " + IdPedido + " ";

                if (IdLiberarPedido > 0)
                    refer += "Liberação: " + IdLiberarPedido + " ";

                if (IdTrocaDevolucao > 0)
                    refer += "Troca/Devolução: " + IdTrocaDevolucao + " ";

                if (IdObra > 0)
                    refer += "Obra: " + IdObra + " ";

                if (IdPagto > 0)
                    refer += "Pagto: " + IdPagto + " ";

                if (IdDeposito > 0)
                    refer += "Depósito: " + IdDeposito + " ";

                if (IdDevolucaoPagto > 0)
                    refer += "Devolução de pagto: " + IdDevolucaoPagto + " ";

                if (!String.IsNullOrEmpty(IdsCompra) && IdsCompra.Length < 20)
                    refer += "Compra: " + IdsCompra + " ";

                if (IdsNf != null && !String.IsNullOrEmpty(IdsNf.Trim(',')))
                    refer += "NF-e: " + NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(IdsNf) + " ";

                if (!String.IsNullOrEmpty(idsNfRec))
                    refer += (refer.Contains("NFe: ") ? "," : "NFe: ") + NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(idsNfRec) + " ";

                if (IdSinal > 0)
                {
                    if (IdPedido == null)
                        refer += "Pedido(s): " + SinalDAO.Instance.ObtemIdsPedidos(IdSinal.Value) + " ";

                    refer += SinalDAO.Instance.GetReferencia(IdSinal.Value) + " ";
                }

                if (IdSinalCompra > 0)
                {
                    if (string.IsNullOrEmpty(IdsCompra))
                        refer += "Compras(s): " + SinalCompraDAO.Instance.ObtemIdsCompras(IdSinalCompra.Value) + " ";

                    refer += "Sinal:" + IdSinalCompra + " ";
                }

                if (IdCreditoFornecedor > 0)
                    refer += "Créd. Fornecedor: " + IdCreditoFornecedor + " ";

                if (IdAcertoCheque > 0)
                    refer += "Acerto cheque: " + IdAcertoCheque + " ";

                if (IdContaR > 0)
                {
                    var idNf = ContasReceberDAO.Instance.ObtemValorCampo<uint?>("IdNf", "IdContaR=" + IdContaR);
                    if (idNf > 0)
                        refer += "NF-e: " + NotaFiscalDAO.Instance.ObtemNumeroNf(null, idNf.Value) + " ";
                }

                return refer;
            }
        }

        public string DescrMovCaixaEBanco
        {
            get
            {
                return MovCaixaFinanceiro ? "Gerar movimentação no caixa geral" :
                    MovBanco ? "Gerar movimentação na conta bancária" : "";
            }
        }

        public bool EditarAgenciaConta
        {
            get { return Tipo == 2; }
        }

        public string DataVencLista
        {
            get
            {
                return (DataVenc != null ? DataVenc.Value.ToString("dd/MM/yyyy") : "") +
                    (DataVencOriginal != null && DataVencOriginal != DataVenc ? "<br>(Data Venc. Orig. " + DataVencOriginal.Value.ToString("dd/MM/yyyy") + ")" : "");
            }
        }

        public bool NaoMovCxGeral
        {
            get
            {
                return Origem == (int)OrigemCheque.FinanceiroPagto && !MovCaixaFinanceiro;
            }
        }

        public bool ExibirCancelarDevolucao
        {
            get
            {
                return Situacao == (int)SituacaoCheque.Devolvido && !Reapresentado &&
                    (Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento) ||
                    Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento));
            }
        }

        public bool ExibirDesmarcarProtestado
        {
            get
            {
                return Situacao == (int)SituacaoCheque.Protestado &&
                    (Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento) ||
                    Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento));
            }
        }

        [Log("CPF/CNPJ")]
        public string CpfCnpjFormatado
        {
            get { return Formatacoes.FormataCpfCnpj(CpfCnpj); }
            set { CpfCnpj = value != null ? value.Replace(".", "").Replace("-", "").Replace("/", "") : null; }
        }

        public string TitularCliente
        {
            get
            {
                string titularCliente = Titular;

                if (IdCliente > 0)
                    titularCliente += " (" + IdCliente + " - " + (!String.IsNullOrEmpty(NomeCliente) ? NomeCliente : ClienteDAO.Instance.GetNome(IdCliente.Value)) + ")";

                return titularCliente;
            }
        }

        #endregion
    }
}
