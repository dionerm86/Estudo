using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Glass.Api.Projeto
{
    /// <summary>
    /// Representa um peça do item de projeto.
    /// </summary>
    public class PecaItemProjeto : IPecaItemProjeto
    {
        #region Variaveis locais

        private List<PosicaoPecaIndividual> _posicoesPecaIndividual = new List<PosicaoPecaIndividual>();

        #endregion

        #region Contrutores

        public PecaItemProjeto()
        {

        }

        public PecaItemProjeto(IPecaItemProjeto pip)
        {
            IdPecaItemProj = Guid.NewGuid();
            IdPecaProjMod = pip.IdPecaProjMod;
            IdProd = (int?)pip.IdProd;
            Altura = pip.Altura;
            Largura = pip.Largura;
            Qtde = pip.Qtde;
            Tipo = pip.Tipo;
            Item = pip.Item;
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Posições das medidas da peça do projeto
        /// </summary>
        public List<PosicaoPecaIndividual> PosicoesPeca
        {
            get { return _posicoesPecaIndividual; }
        }

        /// <summary>
        /// Identificador da peça.
        /// </summary>
        public Guid IdPecaItemProj { get; set; }

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        public int? IdProd { get; set; }

        /// <summary>
        /// Altura da peça.
        /// </summary>
        public int Altura { get; set; }

        /// <summary>
        /// Largura da peça.
        /// </summary>
        public int Largura { get; set; }

        /// <summary>
        /// Tipo da peça.
        /// </summary>
        public int Tipo { get; set; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        public int Qtde { get; set; }

        /// <summary>
        /// Identifica se é uma peça redonda.
        /// </summary>
        public bool Redondo { get; set; }

        /// <summary>
        /// Observação.
        /// </summary>
        public string Obs { get; set; }

        /// <summary>
        /// Item.
        /// </summary>
        public string Item { get; set; }

        #endregion

        #region IPecaItemProjeto Members

        /// <summary>
        /// Identificador da peça do modelo de projeto.
        /// </summary>
        public uint IdPecaProjMod { get; set; }

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        uint? IPecaItemProjeto.IdProd
        {
            get
            {
                return (uint?)IdProd;
            }
        }

        IEnumerable<IPosicaoPecaIndividual> IPecaItemProjeto.PosicoesPeca
        {
            get
            {
                return PosicoesPeca;
            }
        }

        #endregion
    }
}