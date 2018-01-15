using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(GeneroProdutoDAO))]
    [PersistenceClass("genero_produto")]
    public class GeneroProduto : Colosoft.Data.BaseModel, Sync.Fiscal.EFD.Entidade.IGeneroProduto
    {
        #region Propriedades

        [PersistenceProperty("IDGENEROPRODUTO", PersistenceParameterType.IdentityKey)]
        public int IdGeneroProduto { get; set; }

        [PersistenceProperty("CODIGO")]
        public string Codigo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string CodDescr
        {
            get { return Codigo + " - " + (Descricao.Length > 50 ? Descricao.Substring(0, 40) + "..." : Descricao); }
        }

        #endregion

        #region IGeneroProduto Members

        string Sync.Fiscal.EFD.Entidade.IGeneroProduto.CodigoInterno
        {
            get { return Codigo; }
        }

        #endregion
    }
}