using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ObraDAO))]
	[PersistenceClass("obra")]
	public class Obra : ModelBaseCadastro
    {
        #region Enumeradores

        public enum SituacaoObra
        {
            Aberta = 1,
            Cancelada,
            Finalizada,
            Confirmada,
            AguardandoFinanceiro
        }

        public enum TipoPagtoObra
        {
            AVista = 1,
            APrazo
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDOBRA", PersistenceParameterType.IdentityKey)]
        public uint IdObra { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }
        
        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [PersistenceProperty("TIPOPAGTO")]
        public int TipoPagto { get; set; }

        [Log("Número Parcelas")]
        [PersistenceProperty("NUMPARCELAS")]
        public int NumParcelas { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [Log("Funcionário Finalização", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNCFIN")]
        public uint? IdFuncFin { get; set; }

        /// <summary>
        /// 1 - Aberta
        /// 2 - Cancelada
        /// 3 - Finalizada
        /// 4 - Confirmada
        /// 5 - AguardandoFinanceiro
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

		private string _descricao;

        [Log("Descrição")]
		[PersistenceProperty("DESCRICAO")]
		public string Descricao
		{
            get { return _descricao != null ? _descricao.Replace("\r", "").Replace("\n", "") : String.Empty; }
			set { _descricao = value; }
		}

        [Log("Valor da Obra")]
        [PersistenceProperty("VALOROBRA")]
        public decimal ValorObra { get; set; }

        [Log("Saldo")]
        [PersistenceProperty("SALDO")]
        public decimal Saldo { get; set; }

        [Log("Núm. Aut. Construcard")]
        [PersistenceProperty("NUMAUTCONSTRUCARD", 30)]
        public string NumAutConstrucard { get; set; }

        [Log("Data de Finalização")]
        [PersistenceProperty("DATAFIN")]
        public DateTime? DataFin { get; set; }

        [Log("Gerar Crédito")]
        [PersistenceProperty("GERARCREDITO")]
        public bool GerarCredito { get; set; }

        [PersistenceProperty("VALORCREDITOAOCRIAR")]
        public decimal? ValorCreditoAoCriar { get; set; }

        [PersistenceProperty("CREDITOGERADOCRIAR")]
        public decimal? CreditoGeradoCriar { get; set; }

        [PersistenceProperty("CREDITOUTILIZADOCRIAR")]
        public decimal? CreditoUtilizadoCriar { get; set; }

        /// <summary>
        /// Saldo devedor ao criar a obra
        /// </summary>
        [PersistenceProperty("SaldoDevedor")]
        public decimal SaldoDevedor { get; set; }

        /// <summary>
        /// Saldo de crédito ao criar a obra
        /// </summary>
        [PersistenceProperty("SaldoCredito")]
        public decimal SaldoCredito { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("NOMEFUNCCAD", DirectionParameter.InputOptional)]
        public string NomeFuncCad { get; set; }

        [PersistenceProperty("NOMEFUNCFIN", DirectionParameter.InputOptional)]
        public string NomeFuncFin { get; set; }

        [PersistenceProperty("CREDITOCLIENTE", DirectionParameter.InputOptional)]
        public decimal CreditoCliente { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [Log("Pedidos")]
        [PersistenceProperty("IdsPedidos", DirectionParameter.InputOptional)]
        public string IdsPedidos { get; set; }

        [Log("Total Pedidos")]
        [PersistenceProperty("TotalPedidos", DirectionParameter.InputOptional)]
        public decimal TotalPedidos { get; set; }

        [Log("Pedidos Ativos")]
        [PersistenceProperty("IdsPedidosAbertos", DirectionParameter.InputOptional)]
        public string IdsPedidosAbertos { get; set; }

        [Log("Total Pedidos Ativos")]
        [PersistenceProperty("TotalPedidosAbertos", DirectionParameter.InputOptional)]
        public decimal TotalPedidosAbertos { get; set; }

        [Log("Pedidos Conferidos")]
        [PersistenceProperty("IdsPedidosConferidos", DirectionParameter.InputOptional)]
        public string IdsPedidosConferidos { get; set; }

        [Log("Total Pedidos Conferidos")]
        [PersistenceProperty("TotalPedidosConferidos", DirectionParameter.InputOptional)]
        public decimal TotalPedidosConferidos { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string IdNomeCliente
        {
            get { return IdCliente + " - " + NomeCliente; }
        }

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case (int)Obra.SituacaoObra.Aberta: return "Aberta";
                    case (int)Obra.SituacaoObra.Cancelada: return "Cancelada";
                    case (int)Obra.SituacaoObra.Finalizada: return "Finalizada";
                    case (int)Obra.SituacaoObra.Confirmada: return "Confirmada";
                    case (int)Obra.SituacaoObra.AguardandoFinanceiro: return "Aguardando Financeiro";
                    default: return "";
                }
            }
        }

        #region Parcelas

        public DateTime DataPrimParc { get; set; }

        public DateTime DataSegParc { get; set; }

        public DateTime DataTerParc { get; set; }

        public DateTime DataQuarParc { get; set; }

        public DateTime DataQuinParc { get; set; }

        public DateTime DataSexParc { get; set; }

        public DateTime DataSetParc { get; set; }

        public DateTime DataOitParc { get; set; }

        public DateTime DataNonaParc { get; set; }

        public DateTime DataDecParc { get; set; }

        public DateTime DataDecPrimParc { get; set; }

        public DateTime DataDecSegParc { get; set; }

        public DateTime DataDecTerParc { get; set; }

        public DateTime DataDecQuarParc { get; set; }

        public DateTime DataDecQuinParc { get; set; }

        public DateTime DataDecSexParc { get; set; }

        public DateTime DataDecSetParc { get; set; }

        public DateTime DataDecOitParc { get; set; }

        public DateTime DataDecNonaParc { get; set; }

        public DateTime DataVigParc { get; set; }

        public DateTime DataVigPrimParc { get; set; }

        public DateTime DataVigSegParc { get; set; }

        public DateTime DataVigTerParc { get; set; }

        public DateTime DataVigQuarParc { get; set; }

        public DateTime DataVigQuinParc { get; set; }

        public DateTime[] DatasParcelas
        {
            get
            {
                DateTime[] retorno = new DateTime[25];
                retorno[0] = DataPrimParc;
                retorno[1] = DataSegParc;
                retorno[2] = DataTerParc;
                retorno[3] = DataQuarParc;
                retorno[4] = DataQuinParc;
                retorno[5] = DataSexParc;
                retorno[6] = DataSetParc;
                retorno[7] = DataOitParc;
                retorno[8] = DataNonaParc;
                retorno[9] = DataDecParc;
                retorno[10] = DataDecPrimParc;
                retorno[11] = DataDecSegParc;
                retorno[12] = DataDecTerParc;
                retorno[13] = DataDecQuarParc;
                retorno[14] = DataDecQuinParc;
                retorno[15] = DataDecSexParc;
                retorno[16] = DataDecSetParc;
                retorno[17] = DataDecOitParc;
                retorno[18] = DataDecNonaParc;
                retorno[19] = DataVigParc;
                retorno[20] = DataVigPrimParc;
                retorno[21] = DataVigSegParc;
                retorno[22] = DataVigTerParc;
                retorno[23] = DataVigQuarParc;
                retorno[24] = DataVigQuinParc;

                return retorno;
            }
            set
            {
                if (value.Length > 0)
                    DataPrimParc = value[0];
                if (value.Length > 1)
                    DataSegParc = value[1];
                if (value.Length > 2)
                    DataTerParc = value[2];
                if (value.Length > 3)
                    DataQuarParc = value[3];
                if (value.Length > 4)
                    DataQuinParc = value[4];
                if (value.Length > 5)
                    DataSexParc = value[5];
                if (value.Length > 6)
                    DataSetParc = value[6];
                if (value.Length > 7)
                    DataOitParc = value[7];
                if (value.Length > 8)
                    DataNonaParc = value[8];
                if (value.Length > 9)
                    DataDecParc = value[9];
                if (value.Length > 10)
                    DataDecPrimParc = value[10];
                if (value.Length > 11)
                    DataDecSegParc = value[11];
                if (value.Length > 12)
                    DataDecTerParc = value[12];
                if (value.Length > 13)
                    DataDecQuarParc = value[13];
                if (value.Length > 14)
                    DataDecQuinParc = value[14];
                if (value.Length > 15)
                    DataDecSexParc = value[15];
                if (value.Length > 16)
                    DataDecSetParc = value[16];
                if (value.Length > 17)
                    DataDecOitParc = value[17];
                if (value.Length > 18)
                    DataDecNonaParc = value[18];
                if (value.Length > 19)
                    DataVigParc = value[19];
                if (value.Length > 20)
                    DataVigPrimParc = value[20];
                if (value.Length > 21)
                    DataVigSegParc = value[21];
                if (value.Length > 22)
                    DataVigTerParc = value[22];
                if (value.Length > 23)
                    DataVigQuarParc = value[23];
                if (value.Length > 24)
                    DataVigQuinParc = value[24];
            }
        }

        public decimal ValorPrimParc { get; set; }

        public decimal ValorSegParc { get; set; }

        public decimal ValorTerParc { get; set; }

        public decimal ValorQuarParc { get; set; }

        public decimal ValorQuinParc { get; set; }

        public decimal ValorSexParc { get; set; }

        public decimal ValorSetParc { get; set; }

        public decimal ValorOitParc { get; set; }

        public decimal ValorNonaParc { get; set; }

        public decimal ValorDecParc { get; set; }

        public decimal ValorDecPrimParc { get; set; }

        public decimal ValorDecSegParc { get; set; }

        public decimal ValorDecTerParc { get; set; }

        public decimal ValorDecQuarParc { get; set; }

        public decimal ValorDecQuinParc { get; set; }

        public decimal ValorDecSexParc { get; set; }

        public decimal ValorDecSetParc { get; set; }

        public decimal ValorDecOitParc { get; set; }

        public decimal ValorDecNonaParc { get; set; }

        public decimal ValorVigParc { get; set; }

        public decimal ValorVigPrimParc { get; set; }

        public decimal ValorVigSegParc { get; set; }

        public decimal ValorVigTerParc { get; set; }

        public decimal ValorVigQuarParc { get; set; }

        public decimal ValorVigQuinParc { get; set; }

        public decimal[] ValoresParcelas
        {
            get 
            {
                decimal[] retorno = new decimal[25];
                retorno[0] = ValorPrimParc;
                retorno[1] = ValorSegParc;
                retorno[2] = ValorTerParc;
                retorno[3] = ValorQuarParc;
                retorno[4] = ValorQuinParc;
                retorno[5] = ValorSexParc;
                retorno[6] = ValorSetParc;
                retorno[7] = ValorOitParc;
                retorno[8] = ValorNonaParc;
                retorno[9] = ValorDecParc;
                retorno[10] = ValorDecPrimParc;
                retorno[11] = ValorDecSegParc;
                retorno[12] = ValorDecTerParc;
                retorno[13] = ValorDecQuarParc;
                retorno[14] = ValorDecQuinParc;
                retorno[15] = ValorDecSexParc;
                retorno[16] = ValorDecSetParc;
                retorno[17] = ValorDecOitParc;
                retorno[18] = ValorDecNonaParc;
                retorno[19] = ValorVigParc;
                retorno[20] = ValorVigPrimParc;
                retorno[21] = ValorVigSegParc;
                retorno[22] = ValorVigTerParc;
                retorno[23] = ValorVigQuarParc;
                retorno[24] = ValorVigQuinParc;

                return retorno;
            }
            set
            {
                if (value.Length > 0)
                    ValorPrimParc = value[0];
                if (value.Length > 1)
                    ValorSegParc = value[1];
                if (value.Length > 2)
                    ValorTerParc = value[2];
                if (value.Length > 3)
                    ValorQuarParc = value[3];
                if (value.Length > 4)
                    ValorQuinParc = value[4];
                if (value.Length > 5)
                    ValorSexParc = value[5];
                if (value.Length > 6)
                    ValorSetParc = value[6];
                if (value.Length > 7)
                    ValorOitParc = value[7];
                if (value.Length > 8)
                    ValorNonaParc = value[8];
                if (value.Length > 9)
                    ValorDecParc = value[9];
                if (value.Length > 10)
                    ValorDecPrimParc = value[10];
                if (value.Length > 11)
                    ValorDecSegParc = value[11];
                if (value.Length > 12)
                    ValorDecTerParc = value[12];
                if (value.Length > 13)
                    ValorDecQuarParc = value[13];
                if (value.Length > 14)
                    ValorDecQuinParc = value[14];
                if (value.Length > 15)
                    ValorDecSexParc = value[15];
                if (value.Length > 16)
                    ValorDecSetParc = value[16];
                if (value.Length > 17)
                    ValorDecOitParc = value[17];
                if (value.Length > 18)
                    ValorDecNonaParc = value[18];
                if (value.Length > 19)
                    ValorVigParc = value[19];
                if (value.Length > 20)
                    ValorVigPrimParc = value[20];
                if (value.Length > 21)
                    ValorVigSegParc = value[21];
                if (value.Length > 22)
                    ValorVigTerParc = value[22];
                if (value.Length > 23)
                    ValorVigQuarParc = value[23];
                if (value.Length > 24)
                    ValorVigQuinParc = value[24];
            }
        }

        #endregion

        #region Formas de pagamento

        public decimal[] ValoresPagto { get; set; }

        public uint[] FormasPagto { get; set; }

        public uint[] TiposCartaoPagto { get; set; }

        public uint[] ParcelasCartaoPagto { get; set; }

        public uint[] ContasBancoPagto { get; set; }

        public uint[] DepositoNaoIdentificado { get; set; }

        public uint[] CartaoNaoIdentificado { get; set; }

        public string ChequesPagto { get; set; }

        public decimal CreditoUtilizado { get; set; }

        public DateTime? DataRecebimento { get; set; }

        public string[] NumAutCartao { get; set; }

        #endregion

        private bool ObraFinanceiro
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
            }
        }

        private bool ObraFunc
        {
            get { return ObraFinanceiro || IdFunc == UserInfo.GetUserInfo.CodUser; }
        }

        public bool EditVisible
        {
            get { return Situacao == (int)SituacaoObra.Aberta && ObraFunc; }
        }

        public bool CancelVisible
        {
            get { return Situacao != (int)SituacaoObra.Cancelada && ObraFunc &&
                    (!GerarCredito || Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.GerarCreditoAvulsoCliente) ||
                    Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.GerarCreditoAvulsoCliente)); }
        }

        public bool FinalizarVisible
        {
            get { return Situacao == (int)SituacaoObra.Confirmada && ObraFinanceiro; }
        }

        public bool ReabrirVisible
        {
            get { return Situacao == (int)SituacaoObra.AguardandoFinanceiro && ObraFunc; }
        }

        public decimal TotalProdutos
        {
            get { return ObraDAO.Instance.TotalProdutos(IdObra); }
        }

        [Log("Movimentação Crédito")]
        public string MovimentacaoCredito
        {
            get
            {
                decimal utilizado = CreditoUtilizadoCriar != null ? CreditoUtilizadoCriar.Value : 0;
                decimal gerado = CreditoGeradoCriar != null ? CreditoGeradoCriar.Value : 0;

                if (ValorCreditoAoCriar == null || (ValorCreditoAoCriar == 0 && (utilizado + gerado) == 0))
                    return "";

                return "Crédito inicial: " + ValorCreditoAoCriar.Value.ToString("C") + "    " +
                    (utilizado > 0 ? "Crédito utilizado: " + utilizado.ToString("C") + "    " : "") +
                    (gerado > 0 ? "Crédito gerado: " + gerado.ToString("C") + "    " : "") +
                    "Saldo de crédito: " + (ValorCreditoAoCriar.Value - utilizado + (!GerarCredito ? gerado : ValorObra)).ToString("C");
            }
        }

        public string DescrFormaPagto
        {
            get
            {
                string retorno = "";
                var totalParcelas = Glass.Data.DAL.ContasReceberDAO.Instance.ObterNumParcMaxObra(null, IdObra);
                foreach (PagtoObra po in PagtoObraDAO.Instance.GetByObra(IdObra))
                    retorno += po.DescrFormaPagto + (po.IdFormaPagto == 5 && totalParcelas > 0 ? " " + totalParcelas + " parcela(s)" : "") + ": " + po.ValorPagto.ToString("C") +
                        (po.IdContaBanco > 0 ? " (" + ContaBancoDAO.Instance.GetDescricao(po.IdContaBanco.Value) + ")" : String.Empty) + "\n";
                
                if (string.IsNullOrEmpty(retorno))
                    retorno = "À prazo";

                return retorno.TrimEnd(',', ' ');
            }
        }

        public string DescrSaldoPedidos
        {
            get
            {
                string retorno = "Saldo da Obra: {0:c}\nSaldo Pedidos Abertos: {1:c}\nSaldo Atual: {2:c}";
                return String.Format(retorno, Saldo, TotalPedidosAbertos, Saldo - TotalPedidosAbertos);
            }
        }

        public string ValorObraExtenso
        {
            get
            {
                return Formatacoes.ValorExtenso(ValorObra.ToString("0.00"));
            }
        }

        /// <summary>
        /// Saldo total do cliente. 
        /// Créditos - debítos.
        /// </summary>
        public decimal SaldoTotal
        {
            get
            {
                return SaldoCredito - SaldoDevedor;
            }
        }

        #endregion
    }
}