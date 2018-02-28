using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ComissaoConfigDAO))]
	[PersistenceClass("comissao_config")]
	public class ComissaoConfig
    {
        #region Propriedades

        [PersistenceProperty("IDCOMISSAOCONFIG", PersistenceParameterType.IdentityKey)]
        public uint IdComissaoConfig { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint? IdFunc { get; set; }

        [Log("Faixa 1")]
        [PersistenceProperty("FAIXAUM")]
        public decimal FaixaUm { get; set; }

        [Log("Faixa 2")]
        [PersistenceProperty("FAIXADOIS")]
        public decimal FaixaDois { get; set; }

        [Log("Faixa 3")]
        [PersistenceProperty("FAIXATRES")]
        public decimal FaixaTres { get; set; }

        [Log("Faixa 4")]
        [PersistenceProperty("FAIXAQUATRO")]
        public decimal FaixaQuatro { get; set; }

        [Log("Faixa 5")]
        [PersistenceProperty("FAIXACINCO")]
        public decimal FaixaCinco { get; set; }

        [Log("Perc. Faixa 1")]
        [PersistenceProperty("PERCFAIXAUM")]
        public float PercFaixaUm { get; set; }

        [Log("Perc. Faixa 2")]
        [PersistenceProperty("PERCFAIXADOIS")]
        public float PercFaixaDois { get; set; }

        [Log("Perc. Faixa 3")]
        [PersistenceProperty("PERCFAIXATRES")]
        public float PercFaixaTres { get; set; }

        [Log("Perc. Faixa 4")]
        [PersistenceProperty("PERCFAIXAQUATRO")]
        public float PercFaixaQuatro { get; set; }

        [Log("Perc. Faixa 5")]
        [PersistenceProperty("PERCFAIXACINCO")]
        public float PercFaixaCinco { get; set; }

        /// <summary>
        /// True - Se o vendedor vendeu até Faixa3, utilizar PercFaixa3 para todo o valor vendido.
        /// False - Se o vendedor vendeu até Faixa3, utilizar PercFaixa1 para o valor vendido até Faixa1, 
        /// PercFaixa2 para o valor vendido até Faixa2 e PercFaixa3 para o valor vendido até Faixa3
        /// </summary>
        [Log("Perc. único")]
        [PersistenceProperty("PERCUNICO")]
        public bool PercUnico { get; set; }

        [Log("Percentual Venda")]
        [PersistenceProperty("PERCENTUALVENDA")]
        public decimal PercentualVenda { get; set; }

        [Log("Percentual Revenda")]
        [PersistenceProperty("PERCENTUALREVENDA")]
        public decimal PercentualRevenda { get; set; }

        [Log("Percentual Mão Obra")]
        [PersistenceProperty("PERCENTUALMAODEOBRA")]
        public decimal PercentualMaoDeObra { get; set; }

        [Log("Percentual Mão Obra Especial")]
        [PersistenceProperty("PERCENTUALMAODEOBRAESPECIAL")]
        public decimal PercentualMaoDeObraEspecial { get; set; }

        #endregion

        #region Métodos de suporte

        /// <summary>
        /// Recupera o percentual de comissão de acordo com o tipo do pedido
        /// </summary>
        /// <param name="tipoPedido"></param>
        /// <returns></returns>
        public decimal ObterPercentualPorTipoPedido(Pedido.TipoPedidoEnum tipoPedido)
        {
            switch (tipoPedido)
            {
                case Pedido.TipoPedidoEnum.Venda:
                    return this.PercentualVenda;
                case Pedido.TipoPedidoEnum.Revenda:
                    return this.PercentualRevenda;
                case Pedido.TipoPedidoEnum.MaoDeObra:
                    return this.PercentualMaoDeObra;
                case Pedido.TipoPedidoEnum.MaoDeObraEspecial:
                    return this.PercentualMaoDeObraEspecial;
                default:
                    return 0;
            }
        }

        #endregion
    }
}