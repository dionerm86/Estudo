using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Log;
using System.Linq;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(LiberarPedidoDAO))]
	[PersistenceClass("liberarpedido")]
	public class LiberarPedido
    {
        #region Enumeradores

        public enum SituacaoLiberarPedido
        {
            Liberado = 1,
            Cancelado,
            Processando
        }

        public enum TipoPagtoEnum
        {
            AVista = 1,
            APrazo,
            Garantia,
            Reposicao,
            Funcionario = 6
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDLIBERARPEDIDO", PersistenceParameterType.IdentityKey)]
        public uint IdLiberarPedido { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("IDPARCELA")]
        public uint? IdParcela { get; set; }

        /// <summary>
        /// 1-À Vista
        /// 2-À Prazo
        /// 3-Garantia
        /// 4-Reposição
        /// </summary>
        [PersistenceProperty("TIPOPAGTO")]
        public int TipoPagto { get; set; }

        [Log("Número Parcelas")]
        [PersistenceProperty("NUMPARC")]
        public int NumParc { get; set; }

        [Log("Núm. Aut. Construcard")]
        [PersistenceProperty("NUMAUTCONSTRUCARD")]
        public string NumAutConstrucard { get; set; }

        [PersistenceProperty("CREDITOUTILIZADO")]
        public decimal CreditoUtilizado { get; set; }

        [PersistenceProperty("CREDITOGERADO")]
        public decimal CreditoGerado { get; set; }

        [Log("Data da Liberação")]
        [PersistenceProperty("DATALIBERACAO")]
        public DateTime DataLiberacao { get; set; }

        [Log("Total")]
        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("TIPODESCONTO")]
        public int TipoDesconto { get; set; }

        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        [PersistenceProperty("TIPOACRESCIMO")]
        public int TipoAcrescimo { get; set; }

        [PersistenceProperty("ACRESCIMO")]
        public decimal Acrescimo { get; set; }

        [Log("Funcionário Venda", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNCVENDA")]
        public uint? IdFuncVenda { get; set; }

        [PersistenceProperty("VALORCREDITOAOLIBERAR")]
        public decimal? ValorCreditoAoLiberar { get; set; }

        [PersistenceProperty("IDFUNCCANC")]
        public uint? IdFuncCanc { get; set; }

        [PersistenceProperty("OBSCANC")]
        public string ObsCanc { get; set; }

        [PersistenceProperty("DATACANC")]
        public DateTime? DataCanc { get; set; }

        [PersistenceProperty("IDLOJARECEBIMENTO")]
        public int? IdLojaRecebimento { get; set; }

        /// <summary>
        /// Valor total da liberação a ser pago.
        /// </summary>
        [PersistenceProperty("TOTALPAGAR")]
        public decimal? TotalPagar { get; set; }

        /// <summary>
        /// Valor total pago da liberação.
        /// </summary>
        [PersistenceProperty("TOTALPAGO")]
        public decimal? TotalPago { get; set; }

        /// <summary>
        /// Indica se o valor da comissão deve ser descontado do comissionado.
        /// </summary>
        [PersistenceProperty("DESCONTARCOMISSAO")]
        public bool? DescontarComissao { get; set; }

        /// <summary>
        /// Indica se o recebimento deve ser feito no caixa diário.
        /// </summary>
        [PersistenceProperty("RECEBIMENTOCAIXADIARIO")]
        public bool? RecebimentoCaixaDiario { get; set; }

        /// <summary>
        /// Indica se o recebimento deve gerar crédito.
        /// </summary>
        [PersistenceProperty("RECEBIMENTOGERARCREDITO")]
        public bool? RecebimentoGerarCredito { get; set; }

        /// <summary>
        /// Saldo devedor ao criar a liberação
        /// </summary>
        [PersistenceProperty("SaldoDevedor")]
        public decimal SaldoDevedor { get; set; }

        /// <summary>
        /// Saldo de crédito ao criar a liberação
        /// </summary>
        [PersistenceProperty("SaldoCredito")]
        public decimal SaldoCredito { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        private string _nomeCliente;

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente
        {
            get { return _nomeCliente != null ? _nomeCliente.ToUpper() : String.Empty; }
            set { _nomeCliente = value; }
        }

        private string _nomeClienteFantasia;

        [PersistenceProperty("NOMECLIENTEFANTASIA", DirectionParameter.InputOptional)]
        public string NomeClienteFantasia
        {
            get { return _nomeClienteFantasia != null ? _nomeClienteFantasia.ToUpper() : String.Empty; }
            set { _nomeClienteFantasia = value; }
        }

        private string _nomeFunc;

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeFunc); }
            set { _nomeFunc = value; }
        }

        [PersistenceProperty("DESCRFORMAPAGTO", DirectionParameter.InputOptional)]
        public string DescrFormaPagto { get; set; }

        [PersistenceProperty("VALORICMS", DirectionParameter.InputOptional)]
        public decimal ValorIcms { get; set; }

        [PersistenceProperty("ISLIBERACAOPARCIAL", DirectionParameter.InputOptional)]
        public bool IsLiberacaoParcial { get; set; }

        [PersistenceProperty("ISCLIENTEROTA", DirectionParameter.InputOptional)]
        public bool IsClienteRota { get; set; }

        [PersistenceProperty("Endereco", DirectionParameter.InputOptional)]
        public string Endereco { get; set; }

        [PersistenceProperty("Numero", DirectionParameter.InputOptional)]
        public string Numero { get; set; }

        [PersistenceProperty("Bairro", DirectionParameter.InputOptional)]
        public string Bairro { get; set; }

        [PersistenceProperty("IdCidade", DirectionParameter.InputOptional)]
        public uint? IdCidade { get; set; }

        [PersistenceProperty("Cep", DirectionParameter.InputOptional)]
        public string Cep { get; set; }

        [PersistenceProperty("Compl", DirectionParameter.InputOptional)]
        public string Compl { get; set; }

        [PersistenceProperty("Telefone", DirectionParameter.InputOptional)]
        public string Telefone { get; set; }

        #endregion

        #region Propriedades de Suporte

        public byte[] BarCodeImage
        {
            get
            {
                return Utils.GetBarCode(IdLiberarPedido.ToString());
            }
        }

        public string DescrPagto
        {
            get 
            { 
                return DescrTipoPagto + (!String.IsNullOrEmpty(DescrFormaPagto) ? " - " + DescrFormaPagto : String.Empty) +
                    (CreditoUtilizado > 0 ? ", Crédito" : "");
            }
        }

        [Log("Tipo Pagto.")]
        public string DescrTipoPagto
        {
            get 
            { 
                return TipoPagto == 1 ? "À Vista" : TipoPagto == 2 ? "À Prazo" : 
                    TipoPagto == 3 ? "Garantia" : TipoPagto == 4 ? "Reposição" : 
                    TipoPagto == 6 ? "Funcionário" : "N/D";
            }
        }

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                switch ((SituacaoLiberarPedido)Situacao)
                {
                    case SituacaoLiberarPedido.Cancelado: return "Cancelado";
                    case SituacaoLiberarPedido.Liberado: return "Liberado";
                    case SituacaoLiberarPedido.Processando: return "Processando";
                    default: return string.Empty;
                }
            }
        }

        public string DescricaoPagto { get; set; }

        public bool CancelarVisible
        {
            get 
            { 
                return Situacao != 2 && (
                    Liberacao.ApenasAdminCancelaLiberacao ? 
                        UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Administrador : true
                    ); 
            }
        }

        public bool BoletoVisivel
        {
            get
            {
                var contasR = ContasReceberDAO.Instance.GetByPedidoLiberacao(0, IdLiberarPedido, null);
                return contasR.Any(f=> f.BoletoVisivel == true);
            }
        }

        public bool ExibirNotaPromissoria
        {
            get { return LiberarPedidoDAO.Instance.ExibirNotaPromissoria(TipoPagto, Situacao); }
        }

        public bool ExibirGerarNf
        {
            get
            {
                return true;
            }
        }

        public bool ExibirNfeGerada
        {
            get
            {
                return LiberarPedidoDAO.Instance.TemNfe(IdLiberarPedido) && Situacao != (int)LiberarPedido.SituacaoLiberarPedido.Cancelado &&
                    PedidoConfig.LiberarPedido;
            }
        }

        public string IdNomeCliente
        {
            get { return IdCliente + " - " + _nomeCliente; }
        }

        [Log("Desconto")]
        public string TextoDesconto
        {
            get { return TipoDesconto == 1 ? Desconto + "%" : Desconto.ToString("C"); }
        }

        [Log("Acréscimo")]
        public string TextoAcrescimo
        {
            get { return TipoAcrescimo == 1 ? Acrescimo + "%" : Acrescimo.ToString("C"); }
        }

        public decimal TotalSemIcms
        {
            get { return Total - (decimal)ValorIcms; }
        }

        /// <summary>
        /// Propriedade usada na impressão do relatório.
        /// </summary>
        public int NumeroVia { get; set; }

        /// <summary>
        /// Propriedade usada na impressão do relatório.
        /// </summary>
        public int NumeroResumo { get; set; }

        /// <summary>
        /// Propriedade usada na impressão do relatório.
        /// </summary>
        public bool ResumoAlmoxarife { get; set; }

        public string IdsPedidos { get; set; }

        private decimal? _valorCreditoEntrada = null;

        public decimal ValorCreditoEntrada
        {
            get 
            {
                if (_valorCreditoEntrada == null && TipoPagto == (int)TipoPagtoEnum.APrazo)
                {
                    if (CreditoUtilizado > 0)
                        _valorCreditoEntrada = CreditoUtilizado;
                    else
                        _valorCreditoEntrada = CaixaDiarioDAO.Instance.GetCreditoLiberarPedido(IdLiberarPedido, true) +
                            CaixaGeralDAO.Instance.GetCreditoLiberarPedido(IdLiberarPedido, true);
                }

                return _valorCreditoEntrada != null ? _valorCreditoEntrada.Value : 0;
            }
        }

        [Log("Movimentação Crédito")]
        public string MovimentacaoCredito
        {
            get
            {
                decimal utilizado = CreditoUtilizado;
                decimal gerado = CreditoGerado;
                
                if (ValorCreditoAoLiberar == null || (ValorCreditoAoLiberar == 0 && (utilizado + gerado) == 0))
                    return "";

                return "Crédito inicial: " + ValorCreditoAoLiberar.Value.ToString("C") + "    " +
                    (utilizado > 0 ? "Crédito utilizado: " + utilizado.ToString("C") + "    " : "") +
                    (gerado > 0 ? "Crédito gerado: " + gerado.ToString("C") + "    " : "") +
                    "Saldo de crédito: " + (ValorCreditoAoLiberar.Value - (utilizado < 0 ? 0 : utilizado) + gerado).ToString("C");
            }
        }

        public string DadosCancelamento
        {
            get
            {
                if (Situacao != (int)SituacaoLiberarPedido.Cancelado || IdFuncCanc == null || IdFuncCanc == 0)
                    return "";

                return "Cancelado por " + FuncionarioDAO.Instance.GetNome(IdFuncCanc.Value) + " no dia " + DataCanc.Value.ToString("dd/MM/yyyy") +
                    " às " + DataCanc.Value.ToString("HH:mm:ss") + ". " + ObsCanc;
            }
        }

        public string NotasFiscaisGeradas
        {
            get
            {
                return NotaFiscalDAO.Instance.ObtemNumNfePedidoLiberacao(IdLiberarPedido, null, false);
            }
        }

        public bool ExibirReenvioEmail
        {
            get
            {
                bool pedidoEntrega = LiberarPedidoDAO.Instance.ExecuteScalar<int>(@"select count(*) from pedido 
                    where idPedido in (select idPedido from produtos_liberar_pedido where idLiberarPedido=" + IdLiberarPedido +
                    ") and tipoEntrega=" + DataSources.Instance.GetTipoEntregaEntrega().GetValueOrDefault()) > 0;

                return Liberacao.EnviarEmailAoLiberarPedido && pedidoEntrega;
            }
        }

        public bool CanceladoSistema
        {
            get
            {
                return Situacao == (int)SituacaoLiberarPedido.Cancelado && IdFuncCanc == null;
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

        public string Cidade
        {
            get
            {
                return CidadeDAO.Instance.GetNome(IdCidade);
            }
        }

        #endregion
    }
}