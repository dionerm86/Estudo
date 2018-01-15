using System;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(CreditoDAO))]
    public class Credito
    {
        #region Propriedades

        internal uint IdCaixaDiario { get; set; }

        internal uint IdCaixaGeral { get; set; }

        public uint? IdPedido { get; set; }

        public uint? IdAcerto { get; set; }

        public uint? IdLiberarPedido { get; set; }

        public uint? IdObra { get; set; }

        public uint? IdAntecipFornec { get; set; }

        public uint? IdContaR { get; set; }

        public uint? IdTrocaDevolucao { get; set; }

        public uint? IdSinal { get; set; }

        public uint? IdCheque { get; set; }

        public uint? IdPagto { get; set; }

        public uint? IdDevolucaoPagto { get; set; }

        public uint? IdDeposito { get; set; }

        public uint? IdContaPg { get; set; }

        public uint? IdCompra { get; set; }

        public uint? IdAcertoCheque { get; set; }

        public uint IdConta { get; set; }

        public decimal Valor { get; set; }

        public decimal Saldo { get; set; }

        /// <summary>
        /// 1-Gerado
        /// 2-Estorno Utilizado
        /// 3-Utilizado
        /// 4-Estorno Gerado
        /// </summary>
        public int TipoMov { get; set; }

        public DateTime Data { get; set; }

        public uint? IdCreditoFornecedor { get; set; }

        #endregion

        #region Propriedades de Suporte

        private string _descrPlanoConta;

        public string DescrPlanoConta
        {
            get
            {
                if (String.IsNullOrEmpty(_descrPlanoConta))
                    _descrPlanoConta = Glass.Data.DAL.PlanoContasDAO.Instance.ObtemValorCampo<string>("descricao", "idConta=" + IdConta);

                return _descrPlanoConta;
            }
        }

        public string Referencia
        {
            get
            {
                if (IdContaR > 0)
                    return ContasReceberDAO.Instance.GetReferencia(IdContaR.Value);

                if (IdContaPg > 0)
                    return ContasPagarDAO.Instance.GetReferencia(IdContaPg.Value);

                string refer = String.Empty;

                if (IdAcerto > 0)
                    refer += "Acerto: " + IdAcerto + " ";

                if (IdPedido > 0)
                    refer += "Pedido: " + IdPedido + " ";

                if (IdLiberarPedido > 0)
                {
                    refer += "Liberação: " + IdLiberarPedido + " ";

                    if (Configuracoes.FinanceiroConfig.FinanceiroRec.ExibirPedidosDaLiberacaoMovCredito)
                        refer += " (Pedidos: " + ProdutosLiberarPedidoDAO.Instance.GetIdsPedidoByLiberacaoString(IdLiberarPedido.Value).Replace(",", ", ") + ")";
                }

                if (IdObra > 0)
                    refer += "Obra: " + IdObra + " ";

                if (IdAntecipFornec > 0)
                    refer += "Antecip. Fornecedor: " + IdAntecipFornec + " ";

                if (IdTrocaDevolucao > 0)
                    refer += "Troca/Devolução: " + IdTrocaDevolucao + " ";

                if (IdSinal > 0)
                    refer += SinalDAO.Instance.GetReferencia(IdSinal.Value) + " ";

                if (IdCheque > 0)
                    refer += "Cheque: " + ChequesDAO.Instance.GetElementByPrimaryKey(IdCheque.Value).Num + " ";

                if (IdPagto > 0)
                    refer += "Pagto: " + IdPagto + " ";

                if (IdDevolucaoPagto > 0)
                    refer += "Devolução Pagto: " + IdDevolucaoPagto + " ";

                if (IdDeposito > 0)
                    refer += "Depósito: " + IdDeposito + " ";

                if (IdCompra > 0)
                    refer += "Compra: " + IdCompra + " ";

                if (IdAcertoCheque > 0)
                    refer += "Acerto Cheque: " + IdAcertoCheque + " ";

                if (IdCreditoFornecedor > 0)
                    refer += "Créd. Fornecedor: " + IdCreditoFornecedor + " ";

                return refer;
            }
        }

        #endregion
    }
}