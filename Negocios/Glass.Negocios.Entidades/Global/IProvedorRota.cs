namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor de rota.
    /// </summary>
    public interface IProvedorRota
    {
        /// <summary>
        /// Recupera a rota associada com o cliente
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        Rota ObtemRota(Cliente cliente);
    }
}
