namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor das formas de pagamento do cliente.
    /// </summary>
    public interface IProvedorFormaPagtoCliente
    {
        /// <summary>
        /// Recupera a identificação da forma de pagamento do cliente.
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        string ObtemIdentificacao(int idFormaPagto);
    }
}
