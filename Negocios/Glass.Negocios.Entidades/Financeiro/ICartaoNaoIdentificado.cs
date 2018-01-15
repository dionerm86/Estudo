
namespace Glass.Financeiro.Negocios.Entidades
{
    public interface ICartaoNaoIdentificado
    {        
        /// <summary>
        /// Identificador do acerto associado.
        /// </summary>
        int? IdAcerto { get; }

        /// <summary>
        /// Identificador da conta a receber associada.
        /// </summary>
        int? IdContaR { get; }

        /// <summary>
        /// Identificador da devolução do pagamento.
        /// </summary>
        int? IdDevolucaoPagto { get; }

        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        int? IdLiberarPedido { get; }

        /// <summary>
        /// Identificador da obra associada.
        /// </summary>
        int? IdObra { get; }

        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        int? IdPedido { get; }

        /// <summary>
        /// Identificador do sinal associado.
        /// </summary>
        int? IdSinal { get; }

        /// <summary>
        /// Identificador da troca/devolução associada.
        /// </summary>
        int? IdTrocaDevolucao { get; }

        /// <summary>
        /// Identificador do acerto de cheque associado.
        /// </summary>
        int? IdAcertoCheque { get; }
    }
}
