using System;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelDAL;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ReciboDAO))]
    public class Recibo
    {
        #region Propriedades

        /// <summary>
        /// 1-Sinal do pedido
        /// 2-Total do pedido
        /// 3-Restante
        /// 4-Parcelas
        /// 5-Outros
        /// 6-Pagamento Antecipado
        /// 7-Acerto
        /// 8-Conta Paga
        /// </summary>
        public int Tipo { get; set; }

        public uint IdOrcamento { get; set; }

        public uint IdPedido { get; set; }

        public uint IdLiberarPedido { get; set; }

        public string Cliente { get; set; }

        public string Vendedor { get; set; }

        public decimal SinalPedido { get; set; }

        public decimal Total { get; set; }

        public string NumParcelas { get; set; }

        public string Items { get; set; }

        public uint IdLoja { get; set; }

        public uint IdSinal { get; set; }

        public uint IdAcerto { get; set; }

        public int IdContaPagar { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string CidadeData
        {
            get
            {
                var idCidade = LojaDAO.Instance.ObtemValorCampo<uint>("idCidade", "idLoja=" + IdLoja);
                return CidadeDAO.Instance.GetNome(idCidade) + ", " + Formatacoes.DataExtenso(DateTime.Now);
            }
        }

        public string NomeLoja
        {
            get 
            {
                return LojaDAO.Instance.GetNome(IdLoja);
            }
        }

        public string TelefoneLoja
        {
            get { return LojaDAO.Instance.ObtemValorCampo<string>("telefone", "IdLoja=" + IdLoja); }
        }

        public string MotivoReferente { get; set; }

        public string ValorReferente { get; set; }

        public string Texto1
        {
            get
            {
                if (Tipo != 5)
                {
                    var valorRec = Tipo == 1 ? SinalPedido :
                        Tipo == 3 ? Total - SinalPedido :
                        Tipo == 2 || Tipo == 6 || Tipo == 7 || Tipo == 8 || Tipo == 9 ? Total : TotalParcelas;

                    var recebemos = Tipo == 8 ? "Pagamos ao(a) " : "Recebemos do(a) Sr(a) ";

                    var texto = recebemos + Cliente + " a importância de " +
                        valorRec.ToString("C") + " (" + Formatacoes.ValorExtenso(valorRec.ToString("C")) +
                        ") referente " + (Tipo != 8 ? "ao pagamento " : "à ") + (
                            Tipo == 1 ? "do sinal do pedido" :
                            Tipo == 2 ? "do total d" + (!PedidoConfig.LiberarPedido ? "o pedido" : "a liberação") :
                            Tipo == 3 ? "do restante do pedido" :
                            Tipo == 6 ? "antecipado" :
                            Tipo == 7 ? "do acerto" :
                            Tipo == 8 ? MotivoReferente :
                            Tipo == 9 ? "do total do orçamento" :
                            "da(s) parcela(s) d" + (!PedidoConfig.LiberarPedido ? "o pedido" : "a liberação") + ", sendo " + TextoParcelas
                        );

                    return texto;
                }
                else
                    return MotivoReferente.Replace("\\n", "\n");
            }
        }

        public string Texto2
        {
            get
            {
                string texto = Tipo == 1 ?
                    "VALOR DO SINAL.................................................................................................." + SinalPedido.ToString("C") +
                    "\nVALOR RESTANTE................................................................................................." + (Total - SinalPedido).ToString("C") :
                    Tipo == 2 ?
                        (!PedidoConfig.LiberarPedido ? "VALOR TOTAL DO PEDIDO..............................................................................." + Total.ToString("C") :
                        "VALOR TOTAL DA LIBERAÇÃO.............................................................................." + Total.ToString("C")) :
                    Tipo == 3 ?
                    "VALOR RESTANTE................................................................................................" + (Total - SinalPedido).ToString("C") +
                    "\nVALOR TOTAL DO PEDIDO.................................................................................." + Total.ToString("C") :
                    Tipo == 4 ?
                    "VALOR TOTAL DA(S) PARCELA(S)........................................................................" + TotalParcelas.ToString("C") +
                        (!PedidoConfig.LiberarPedido ? "\nVALOR TOTAL DO PEDIDO...................................................................................." + Total.ToString("C") :
                        "\nVALOR TOTAL DA LIBERAÇÃO.............................................................................." + Total.ToString("C")) :
                    Tipo == 5 ? ValorReferente.Replace("\\n", "\n") :
                    Tipo == 6 ? "VALOR TOTAL DO PAGAMENTO.............................................................................." + Total.ToString("C") :
                    Tipo == 7 ? "VALOR TOTAL DO ACERTO................................................................................." + Total.ToString("C") :
                    Tipo == 8 ? "VALOR TOTAL DO PAGAMENTO.............................................................................." + Total.ToString("C") :
                    Tipo == 9 ? "VALOR TOTAL DO ORÇAMENTO.............................................................................." + Total.ToString("C") :
                    string.Empty;

                return texto;
            }
        }

        public string TextoParcelas
        {
            get { return string.IsNullOrEmpty(NumParcelas) || NumParcelas == "0" ? ParcelasPedidoDAO.Instance.GetForRecibo(NumParcelas) : ContasReceberDAO.Instance.GetForRecibo(NumParcelas); }
        }

        public decimal TotalParcelas
        {
            get { return string.IsNullOrEmpty(NumParcelas) || NumParcelas == "0" ? ParcelasPedidoDAO.Instance.GetTotal(NumParcelas) : ContasReceberDAO.Instance.GetTotalVenc(NumParcelas); }
        }

        public string TextoVintage
        {
            get
            {
                if (IdPedido == 0)
                    return String.Empty;

                Pedido pedido = PedidoDAO.Instance.GetElementByPrimaryKey(IdPedido);
                decimal valorRec = Tipo == 1 ? SinalPedido : Total;
                string tipoPagamento = Tipo == 1 ? "do sinal" : "integral";

                string texto = "Recebemos do(a) Sr(a) " + Cliente + " a importância de " + valorRec.ToString("C") + 
                    " (" + Formatacoes.ValorExtenso(valorRec.ToString("C")) + "), referente ao pagamento " + tipoPagamento + 
                    " do seu pedido de compra n.º " + IdPedido + " de " + pedido.DataPedidoString;
                    
                if (pedido.TipoVenda <= 2) 
                {
                    texto += ", sendo " + (pedido.TipoVenda == 1 ? Total.ToString("C") + " à vista" :
                        (SinalPedido > 0 ? SinalPedido.ToString("C") + " à vista, " : "") + ParcelasPedidoDAO.Instance.GetForRecibo(IdPedido));
                }

                texto += ".\nPor ser verdade firmo o presente.";
                return texto;
            }
        }

        public string TituloId
        {
            get
            {
                var retorno = string.Empty;

                if (Tipo == 9)
                    retorno = "Orçamento Nº";
                else if (this.Tipo == 6)
                    retorno = "Pagamento Nº";
                else if (this.Tipo == 7)
                    retorno = "Acerto Nº";
                else if (this.Tipo == 8)
                    retorno = "Conta Paga Nº";
                else
                    retorno = !PedidoConfig.LiberarPedido ? "Pedido N.º" : "Liberação N.º";

                return retorno;
            }
        }

        public uint Id
        {
            get
            {
                // Recibo de orçamento.
                if (Tipo == 9)
                    return IdOrcamento;
                // Recibo de pagamento antecipado.
                else if (this.Tipo == 6)
                    return this.IdSinal;
                else if (this.Tipo == 7)
                    return this.IdAcerto;
                else if (this.Tipo == 8)
                    return (uint)this.IdContaPagar;

                return !PedidoConfig.LiberarPedido ? IdPedido : IdLiberarPedido;
            }
        }

        #endregion
    }
}