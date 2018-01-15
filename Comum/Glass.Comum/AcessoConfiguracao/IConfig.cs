namespace Glass.AcessoConfiguracao
{
    /// <summary>
    /// Assinatura do contexto das configurações do sistema.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Retorna o valor de um item do banco de dados.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        object GetValue(uint item, uint idLoja);
    }
}
