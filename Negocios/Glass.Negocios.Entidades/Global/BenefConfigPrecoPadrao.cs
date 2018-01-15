using System.Collections.Generic;
using System.Text;
using Colosoft;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados do preço padrão da configuração do beneficiamento.
    /// </summary>
    public class BenefConfigPrecoPadrao
    {
        #region Variáveis Locais

        private List<BenefConfigPrecoPadrao> _precos = new List<BenefConfigPrecoPadrao>();

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do preço.
        /// </summary>
        public int IdBenefConfigPreco { get; set; }

        /// <summary>
        /// Identificador da configuração do beneficiamento associado.
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
        /// Tipo de calculo do beneficiamento associado.
        /// </summary>
        public Glass.Data.Model.TipoCalculoBenef TipoCalculo { get; set; }

        /// <summary>
        /// Descrição com o tipo do calculo.
        /// </summary>
        public string DescricaoComTipoCalculo
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

                return string.Format("{0} ({1})", descricao.ToString(), TipoCalculo.Translate());
            }
        }

        /// <summary>
        /// Identificador do subgrupo de produtos.
        /// </summary>
        public int? IdSubgrupoProd { get; set; }

        /// <summary>
        /// Descrição do subgrupo de produtos.
        /// </summary>
        public string SubgrupoProd { get; set; }

        /// <summary>
        /// Identificador da cor do vidro.
        /// </summary>
        public int? IdCorVidro { get; set; }

        /// <summary>
        /// Descricao da cor do vidro.
        /// </summary>
        public string CorVidro { get; set; }

        /// <summary>
        /// Espessura do vidro.
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
        /// Relação dos preços não padrão associados.
        /// </summary>
        public IList<BenefConfigPrecoPadrao> Precos 
        {
            get { return _precos; }
        }

        #endregion
    }
}
