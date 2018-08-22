using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios
{
    /// <summary>
    /// Representa os possíveis tipos de item de otimização.
    /// </summary>
    public enum TipoItemOtimizacao
    {
        /// <summary>
        /// Identifica que o item é uma peça.
        /// </summary>
        Peca,
        /// <summary>
        /// Identifica que o item é um retalho.
        /// </summary>
        Retalho
    }

    /// <summary>
    /// Representa o estados de um item da otimização.
    /// </summary>
    public class ItemOtimizacao
    {
        #region Propriedades

        /// <summary>
        /// Obtém ou define o tipo do item.
        /// </summary>
        public TipoItemOtimizacao Tipo { get; set; }

        /// <summary>
        /// Identificador do produto do pedido espelho.
        /// </summary>
        public int? IdProdPed { get; set; }

        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        public int? IdPedido { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Identifica se é uma peça reposta.
        /// </summary>
        public bool PecaReposta { get; set; }

        /// <summary>
        /// Código do processo.
        /// </summary>
        public string CodProcesso { get; set; }
        
        /// <summary>
        /// Código da aplicação.
        /// </summary>
        public string CodAplicacao { get; set; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        public int Qtde { get; set; }

        /// <summary>
        /// Quantidade que foi impresso.
        /// </summary>
        public int QtdImpresso { get; set; }

        /// <summary>
        /// Quantidade a imprimir.
        /// </summary>
        public int QtdAImprimir { get; set; }

        /// <summary>
        /// Altura produção.
        /// </summary>
        public float AlturaProducao { get; set; }

        /// <summary>
        /// Largura da produção.
        /// </summary>
        public float LarguraProducao { get; set; }

        /// <summary>
        /// Observação.
        /// </summary>
        public string Obs { get; set; }

        /// <summary>
        /// Total em M2.
        /// </summary>
        public float TotM2 { get; set; }

        /// <summary>
        /// Total em M2 calculado.
        /// </summary>
        public float TotM2Calc { get; set; }

        /// <summary>
        /// Plano de corte da etiqueta.
        /// </summary>
        public string PlanoCorteEtiqueta { get; set; }

        /// <summary>
        /// Etiquetas.
        /// </summary>
        public IEnumerable<string> Etiquetas { get; set; }

        #endregion
    }
}
