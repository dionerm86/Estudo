namespace Glass.Negocios.Global
{
    /// <summary>
    /// Representa o resutlado da operação de salvar cliente
    /// </summary>
    public class SalvarClienteResultado : Colosoft.Business.OperationResult
    {
        #region Propriedades

        /// <summary>
        /// Id do cliente
        /// </summary>
        public int IdCliente { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        public SalvarClienteResultado(bool success, Colosoft.IMessageFormattable message)
            : base(success, message)
        {
        }

        /// <summary>
        /// Cria a instancia 
        /// </summary>
        /// <param name="movEstoque"></param>
        public SalvarClienteResultado(int idCliente)
            : base(true, null)
        {
            IdCliente = idCliente;
        }

        #endregion
    }
}
