namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do repositório de imagens dos clientes.
    /// </summary>
    public interface IClienteRepositorioImagens
    {
        /// <summary>
        /// Recupera a Url da imagem do cliente.
        /// </summary>
        string ObterUrl(int idCliente);

        /// <summary>
        /// Salva a imagem do cliente.
        /// </summary>
        bool SalvarImagem(int idCliente, System.IO.Stream stream);
    }
}
