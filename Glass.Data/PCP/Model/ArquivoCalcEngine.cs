using GDA;
using System.Xml.Serialization;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(Glass.Data.DAL.ArquivoCalcEngineDAO))]
    [PersistenceClass("arquivo_calcengine")]
    public class ArquivoCalcEngine
    {
        #region Propriedades

        [PersistenceProperty("IDARQUIVOCALCENGINE", PersistenceParameterType.IdentityKey)]
        public uint IdArquivoCalcEngine { get; set; }

        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public static string TrataNome(string nome)
        {
            while (nome.Contains("  "))
                nome = nome.Replace("  ", "");

            nome = nome.Replace(" ", "").Replace(".", "").Replace("ã", "a").Replace("á", "a").Replace("â", "a")
                .Replace("é", "e").Replace("ê", "e").Replace("í", "i").Replace("ç", "c").Replace("Ã", "A").Replace("Á", "A")
                .Replace("Â", "A").Replace("É", "E").Replace("Ê", "E").Replace("Í", "I").Replace("Ç", "C");

            return nome;
        }

        [XmlIgnore]
        public int[] FlagsArqMesa { get; set; }

        #endregion
    }
}