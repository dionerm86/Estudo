using System.Text;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa dos preços da configuração do beneficiamento.
    /// </summary>
    public class BenefConfigPrecoPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador do preço.
        /// </summary>
        public int IdBenefConfigPreco { get; set; }

        /// <summary>
        /// Identificador da configuração do beneficiamento.
        /// </summary>
        public int IdBenefConfig { get; set; }

        /// <summary>
        /// Descrição do beneficiamento associado.
        /// </summary>
        public string DescricaoBenef { get; set; }

        /// <summary>
        /// Descrição do beneficiamento pai do beneficiamento associado;
        /// </summary>
        public string DescricaoBenefPai { get; set; }

        /// <summary>
        /// Espessura.
        /// </summary>
        public float? Espessura { get; set; }

        /// <summary>
        /// Valor atacado.
        /// </summary>
        public decimal ValorAtacado { get; set; }

        /// <summary>
        /// Valor balcão.
        /// </summary>
        public decimal ValorBalcao { get; set; }

        /// <summary>
        /// Valor obra.
        /// </summary>
        public decimal ValorObra { get; set; }

        /// <summary>
        /// Custo.
        /// </summary>
        public decimal Custo { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao
        {
            get
            {
                var descricao = new StringBuilder();

                if (!string.IsNullOrEmpty(DescricaoBenefPai))
                    descricao.Append(DescricaoBenefPai).Append(" ");

                descricao.Append(DescricaoBenef);
                if (Espessura.HasValue)
                    descricao
                        .Append(" ")
                        .Append(Espessura.Value.ToString("#00"))
                        .Append("mm");

                return descricao.ToString();
            }
        }

        #endregion
    }
}
