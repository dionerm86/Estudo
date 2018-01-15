namespace Glass.Projeto.Negocios.Entidades
{
    public class FerragemPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da ferragem
        /// </summary>
        public int IdFerragem { get; set; }

        /// <summary>
        /// Nome da Ferragem
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Situação da Ferragem
        /// </summary>
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Nome do fabricante da ferragem
        /// </summary>
        public string NomeFabricante { get; set; }

        /// <summary>
        /// Define o estilo de ancoragem da ferragem
        /// </summary>
        public Data.Model.EstiloAncoragem EstiloAncoragem { get; set; }

        /// <summary>
        /// Altura da Ferragem
        /// </summary>
        public double Altura { get; set; }

        /// <summary>
        /// Largura da Ferragem
        /// </summary>
        public double Largura { get; set; }

        /// <summary>
        /// Data da Alteração
        /// </summary>
        public System.DateTime DataAlteracao { get; set; }

        #endregion
    }
}
