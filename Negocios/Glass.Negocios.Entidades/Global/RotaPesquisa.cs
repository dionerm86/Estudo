namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa de rota.
    /// </summary>
    public class RotaPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da rota.
        /// </summary>
        public int IdRota { get; set; }

        /// <summary>
        /// Código interno.
        /// </summary>
        public string CodInterno { get; set; }

        /// <summary>
        /// Descrição da rota.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Situação da rota.
        /// </summary>
        public Glass.Situacao Situacao { get; set; }

        /// <summary>
        /// Distancia.
        /// </summary>
        public int Distancia { get; set; }

        /// <summary>
        /// Observações.
        /// </summary>
        public string Obs { get; set; }

        /// <summary>
        /// Dias da semana que a rota ocorre.
        /// </summary>
        public Data.Model.DiasSemana DiasSemana { get; set; }

        /// <summary>
        /// Número mínimo de dias para entrega na rota.
        /// </summary>
        public int NumeroMinimoDiasEntrega { get; set; }

        #endregion
    }
}
