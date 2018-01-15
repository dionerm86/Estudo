using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceClass("flag_arq_mesa_produto")]
    public class FlagArqMesaProduto : Colosoft.Data.BaseModel
    {
        #region Propiedades

        [PersistenceProperty("IdProduto", PersistenceParameterType.Key)]
        public int IdProduto{ get; set; }

        [PersistenceProperty("IdFlagArqMesa", PersistenceParameterType.Key)]
        public int IdFlagArqMesa { get; set; }

        #endregion

        #region Propriedades de Suporte

        public static string TrataDescricao(string descricao)
        {
            while (descricao.Contains("  "))
                descricao = descricao.Replace("  ", "");

            descricao = descricao.Replace(" ", "").Replace(".", "").Replace("ã", "a").Replace("á", "a").Replace("â", "a")
                .Replace("é", "e").Replace("ê", "e").Replace("í", "i").Replace("ç", "c").Replace("Ã", "A").Replace("Á", "A")
                .Replace("Â", "A").Replace("É", "E").Replace("Ê", "E").Replace("Í", "I").Replace("Ç", "C");

            return descricao;
        }

        #endregion
    }
}
