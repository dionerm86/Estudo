namespace Glass.PCP.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa do tipo de perda.
    /// </summary>
    public class TipoPerdaPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador do tipo da perda.
        /// </summary>
        public int IdTipoPerda { get; set; }

        /// <summary>
        /// Identificador do setor associado.
        /// </summary>
        public int? IdSetor { get; set; }

        /// <summary>
        /// Nome do setor.
        /// </summary>
        public string Setor { get; set; }

        /// <summary>
        /// Descrição do tipo de perda.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Situação Tipo Perda
        /// </summary>
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Exibir painel de produção
        /// </summary>
        public bool ExibirPainelProducao { get; set; }

        #endregion
    }
}
