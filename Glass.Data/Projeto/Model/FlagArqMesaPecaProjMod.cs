using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FlagArqMesaPecaProjModDAO))]
    [PersistenceClass("flag_arq_mesa_peca_projeto_modelo")]
    public class FlagArqMesaPecaProjMod
    {
        #region Propiedades

        [PersistenceProperty("IdPecaProjMod", PersistenceParameterType.Key)]
        public int IdPecaProjMod { get; set; }

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
