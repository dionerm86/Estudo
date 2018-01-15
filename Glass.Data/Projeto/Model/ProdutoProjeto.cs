using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoProjetoDAO))]
	[PersistenceClass("produto_projeto")]
	public class ProdutoProjeto
    {
        #region Enumeradores

        public enum TipoProduto : int
        {
            Aluminio=1,
            Ferragem,
            Outros,
            Vidro
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPRODPROJ", PersistenceParameterType.IdentityKey)]
        public uint IdProdProj { get; set; }

        [Log("Produto", "Descricao", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPROD")]
        public uint? IdProd { get; set; }

        /// <summary>
        /// 1-Alumínio
        /// 2-Ferragem
        /// 3-Outros
        /// 4-Vidro
        /// </summary>
        [PersistenceProperty("TIPO")]
        public int Tipo { get; set; }

        [Log("Código")]
        [PersistenceProperty("CODINTERNO")]
        public string CodInterno { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }
 
        [Log("Produto Padrão")]
        [PersistenceProperty("PRODUTOPADRAO")]
        public bool ProdutoPadrao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("PRODUTOASSOCIADO", DirectionParameter.InputOptional)]
        public string ProdutoAssociado { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool ProdConfigVisible
        {
            get { return Tipo != (int)TipoProduto.Outros; }
        }

        [Log("Tipo")]
        public string DescrTipo
        {
            get 
            {
                return Tipo == 1 ? "Alumínio" : Tipo == 2 ? "Ferragem" : Tipo == 3 ? "Outros" : Tipo == 4 ? "Vidro" : String.Empty;
            }
        }

        #endregion
    }
}