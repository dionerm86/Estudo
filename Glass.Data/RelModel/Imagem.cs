using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ImagemDAO))]
    [PersistenceClass("imagem")]
    public class Imagem
    {
        #region Propriedades

        [PersistenceProperty("CHAVE")]
        public uint Chave { get; set; }

        [PersistenceProperty("IMAGEM1")]
        public byte[] Imagem1 { get; set; }

        [PersistenceProperty("IMAGEM2")]
        public byte[] Imagem2 { get; set; }

        [PersistenceProperty("IMAGEM3")]
        public byte[] Imagem3 { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool EsconderImagem1
        {
            get { return Imagem1 == null || Imagem1.Length == 0; }
        }

        public bool EsconderImagem2
        {
            get { return Imagem2 == null || Imagem2.Length == 0; }
        }

        public bool EsconderImagem3
        {
            get { return Imagem3 == null || Imagem3.Length == 0; }
        }

        public string UrlImagem1 { get; set; }

        public string UrlImagem2 { get; set; }

        public string UrlImagem3 { get; set; }

        public string DescImagem1 { get; set; }

        public string DescImagem2 { get; set; }

        public string DescImagem3 { get; set; }

        public string CodImagem1 { get; set; }

        public string CodImagem2 { get; set; }

        public string CodImagem3 { get; set; }

        public string Criterio { get; set; }

        #endregion
    }
}