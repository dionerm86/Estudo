using System;
using GDA;
using Glass.Data.DAL;
using System.Collections.Generic;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CaixaDiarioDAO))]
	[PersistenceClass("caixa_diario")]
	public class CaixaDiario : ModelBaseCadastro
    {
        #region Enumeradores
        
        public enum FormaSaidaEnum : int
        {
            Dinheiro=1,
            Cheque
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCAIXADIARIO", PersistenceParameterType.IdentityKey)]
        public uint IdCaixaDiario { get; set; }

        [PersistenceProperty("IDCLIENTE")]
        public uint? IdCliente { get; set; }

        [PersistenceProperty("IDFORNEC")]
        public uint? IdFornec { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDACERTO")]
        public uint? IdAcerto { get; set; }

        [PersistenceProperty("IDSINAL")]
        public uint? IdSinal { get; set; }

        [PersistenceProperty("IDCONTAR")]
        public uint? IdContaR { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("IDOBRA")]
        public uint? IdObra { get; set; }

        [PersistenceProperty("IDCONTA")]
        public uint IdConta { get; set; }

        [PersistenceProperty("IDCHEQUE")]
        public uint? IdCheque { get; set; }

        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public uint? IdTrocaDevolucao { get; set; }

        [PersistenceProperty("IdAcertoCheque")]
        public uint? IdAcertoCheque { get; set; }

        [PersistenceProperty("IdDevolucaoPagto")]
        public uint? IdDevolucaoPagto { get; set; }

        [PersistenceProperty("IDCAIXADIARIOESTORNO")]
        public int? IdCaixaDiarioEstorno { get; set; }

        /// <summary>
        /// 1-Dinheiro
        /// 2-Cheque
        /// </summary>
        [PersistenceProperty("FORMASAIDA")]
        public int? FormaSaida { get; set; }

        /// <summary>
        /// 1-Entrada, 2-Saída
        /// </summary>
        [PersistenceProperty("TIPOMOV")]
        public int TipoMov { get; set; }

        [PersistenceProperty("SALDO")]
        public decimal Saldo { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("JUROS")]
        public decimal Juros { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("NUMAUTCONSTRUCARD")]
        public string NumAutConstrucard { get; set; }

        [PersistenceProperty("MUDARSALDO")]
        public bool? MudarSaldo { get; set; }      

        [PersistenceProperty("IDCARTAONAOIDENTIFICADO")]
        public uint? IdCartaoNaoIdentificado { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _nomeLoja;

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja
        {
            get { return _nomeLoja != null ? "Fechamento diário Caixa Rotativo - " + _nomeLoja : " "; }
            set { _nomeLoja = value; }
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

        [PersistenceProperty("DescrPlanoConta", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string ClienteFornecedor
        {
            get
            {
                return IdCliente > 0 ? IdCliente + " - " + _nomeCliente :
                    IdFornec > 0 ? IdFornec + " - " + _nomeFornecedor : String.Empty;
            }
        }

        public string RptData
        {
            get { return DateTime.Now.ToString("dd/MM/yyyy HH:mm"); }
        }

        public decimal TotalCheques { get; set; }

        public decimal TotalDinheiro { get; set; }

        public decimal TotalCartao { get; set; }

        public decimal TotalDeposito { get; set; }

        public decimal TotalBoleto { get; set; }

        public decimal TotalBoletoBancoBrasil { get; set; }

        public decimal TotalBoletoLumen { get; set; }

        public decimal TotalBoletoOutros { get; set; }

        public decimal TotalBoletoSantander { get; set; }

        public decimal TotalConstrucard { get; set; }

        public decimal TotalPermuta { get; set; }

        public decimal TotalMasterCredito { get; set; }

        public decimal TotalMasterDebito { get; set; }

        public decimal TotalVisaCredito { get; set; }

        public decimal TotalVisaDebito { get; set; }

        public decimal TotalOutrosCredito { get; set; }

        public decimal TotalOutrosDebito { get; set; }

        public decimal CreditoGerado { get; set; }

        public decimal CreditoRecebido { get; set; }

        public decimal TotalSaidaDinheiro { get; set; }

        public decimal TotalSaidaCheque { get; set; }

        public decimal TotalRecDinheiro { get; set; }

        public decimal TotalTransfCxGeralDinheiro { get; set; }

        public decimal TotalEstornoDinheiro { get; set; }

        public bool CreditoVisible { get; set; }

        public string Referencia
        {
            get
            {
                string refer = String.Empty;

                if (IdAcerto > 0)
                    refer += string.Format("Acerto: {0} ", IdAcerto);

                if (IdAcertoCheque > 0)
                    refer += "Acerto Cheque: " + IdAcertoCheque + " ";

                if (IdCheque > 0)
                    refer += "Cheque: " + ChequesDAO.Instance.ObtemNumCheque(IdCheque.Value) + " ";

                if (IdPedido > 0)
                    refer += "Pedido: " + IdPedido + " ";

                if (IdLiberarPedido > 0)
                    refer += string.Format("Liberação: {0} ", IdLiberarPedido);

                if (IdObra > 0)
                    refer += "Obra: " + IdObra + " ";

                if (IdTrocaDevolucao > 0)
                    refer += "Troca/Devolução: " + IdTrocaDevolucao + " ";

                if (IdDevolucaoPagto > 0)
                    refer += "Devolução de pagto.: " + IdDevolucaoPagto + " ";

                if (IdSinal > 0)
                    refer += SinalDAO.Instance.GetReferencia(IdSinal.Value) + " ";

                if (IdSinal > 0 && Configuracoes.FinanceiroConfig.CaixaDiario.ExibirPedidosDoSinal)
                    refer += string.Format("Pedido(s): {0} ", SinalDAO.Instance.ObtemIdsPedidos(IdSinal.Value));
                else if (IdAcerto > 0 && Configuracoes.FinanceiroConfig.CaixaDiario.ExibirPedidosDoAcerto)
                {
                    var idsPedido = AcertoDAO.Instance.ObterIdsPedido(null, (int)IdAcerto.Value);

                    if (string.IsNullOrEmpty(idsPedido))
                    {
                        var idsLiberarPedido = AcertoDAO.Instance.ObterIdsLiberarPedido(null, (int)IdAcerto.Value);

                        if (!string.IsNullOrEmpty(idsLiberarPedido))
                            idsPedido += LiberarPedidoDAO.Instance.IdsPedidos(null, idsLiberarPedido);
                    }

                    if (!string.IsNullOrEmpty(idsPedido))
                        refer += string.Format("Pedido(s): {0} ", idsPedido);
                }
                else if (IdLiberarPedido > 0 && Configuracoes.FinanceiroConfig.CaixaDiario.ExibirPedidosDaLiberacao)
                    refer += string.Format("Pedido(s): {0} ",
                        LiberarPedidoDAO.Instance.IdsPedidos(null, IdLiberarPedido.Value.ToString()));

                if(IdCartaoNaoIdentificado > 0)
                    refer += "Cartão não Identificado.: " + IdCartaoNaoIdentificado + " ";

                if (String.IsNullOrEmpty(refer) && IdContaR > 0)
                    refer = ContasReceberDAO.Instance.GetReferencia(IdContaR.Value);                               

                return refer;
            }
        }

        public IList<TipoCartaoCredito> Cartoes { get; set; }

        #endregion
    }
}