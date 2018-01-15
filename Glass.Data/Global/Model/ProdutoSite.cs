using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoSiteDAO))]
    [PersistenceClass("produto_site")]
    public class ProdutoSite
    {
        #region Propriedades

        [PersistenceProperty("CodProduto", PersistenceParameterType.IdentityKey)]
        public uint CodProduto { get; set; }

        /// <summary>
        /// 1-Vidro, 2-Serviço, 3-Fixador
        /// </summary>
        [PersistenceProperty("TipoProduto")]
        public int TipoProduto { get; set; }

        [PersistenceProperty("Descricao")]
        public string Descricao { get; set; }

        [PersistenceProperty("Foto")]
        public string Foto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrTipo
        {
            get
            {
                switch (TipoProduto)
                {
                    case 1:
                        return "Vidro";
                    case 2:
                        return "Serviço";
                    case 3:
                        return "Fixador";
                    default:
                        return "";
                }
            }
        }

        #endregion
    }
}