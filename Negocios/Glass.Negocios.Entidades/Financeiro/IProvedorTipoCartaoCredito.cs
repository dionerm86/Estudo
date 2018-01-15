namespace Glass.Financeiro.Negocios.Entidades
{
    public interface IProvedorTipoCartaoCredito
    {
        /// <summary>
        /// Verifica se o tipo cartão crétido está em uso
        /// </summary>
        /// <param name="idTipoCartao"></param>
        /// <returns></returns>
        bool VerificarTipoCartaoCreditoUso(int idTipoCartao);
    }
}
