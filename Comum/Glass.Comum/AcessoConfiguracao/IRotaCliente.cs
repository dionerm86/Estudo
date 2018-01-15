namespace Glass.AcessoConfiguracao
{
    public interface IRotaCliente
    {
        /// <summary>
        /// Verifica se o cliente já foi associado a uma rota
        /// </summary>
        /// <param name="idRota"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        bool VerificaClienteAssociadoRota(uint idCliente);
    }
}
