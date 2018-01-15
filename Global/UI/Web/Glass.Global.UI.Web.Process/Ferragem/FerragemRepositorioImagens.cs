using System.ComponentModel.Composition;
using System.IO;

namespace Glass.Global.UI.Web.Process.Ferragem
{
    /// <summary>
    /// Implementação do repositório de imagens da ferragem.
    /// </summary>
    [Export(typeof(Projeto.Negocios.Entidades.IFerragemRepositorioImagens))]
    public class FerragemRepositorioImagens : Projeto.Negocios.Entidades.IFerragemRepositorioImagens
    {
        #region Variáveis Locais

        private static FerragemRepositorioImagens _instance;

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do repositório.
        /// </summary>
        public static FerragemRepositorioImagens Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FerragemRepositorioImagens();

                return _instance;
            }
        }

        /// <summary>
        /// Diretório das imagens dos ferragems.
        /// </summary>
        public string DiretorioImagens
        {
            get
            {
                return System.IO.Path.Combine(Armazenamento.ArmazenamentoIsolado.DiretorioUpload, "Ferragem\\Imagem");
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera a Url da imagem da ferragem.
        /// </summary>
        /// <param name="idFerragem">Identificador do ferragem.</param>
        public string ObterUrl(int idFerragem)
        {
            return string.Format("~/Upload/Ferragem/Imagem/{0}.jpg", idFerragem);
        }

        /// <summary>
        /// Recupera o caminho da imagem da ferragem.
        /// </summary>
        /// <param name="idFerragem">Identificador do ferragem.</param>
        public string ObterCaminho(int idFerragem)
        {
            return string.Format("{0}\\{1}.jpg", DiretorioImagens, idFerragem);
        }

        /// <summary>
        /// Verifica se o ferragem possui imagem.
        /// </summary>
        /// <param name="idFerragem">Identificador da ferragem.</param>
        /// <returns></returns>
        public bool PossuiImagem(int idFerragem)
        {
            return System.IO.File.Exists(ObterCaminho(idFerragem));
        }

        /// <summary>
        /// Salva a imagem da ferragem.
        /// </summary>
        /// <param name="idFerragem">Identificador da ferragem.</param>
        /// <param name="stream">Stream contendo os dados da imagem que será salva.</param>
        public bool SalvarImagem(int idFerragem, System.IO.Stream stream)
        {
            var arquivo = ObterCaminho(idFerragem);

            // Caso a ferragem já possua imagem, o arquivo deve ser removido, para que seja salva a nova imagem.
            if (File.Exists(arquivo))
                File.Delete(arquivo);

            return ManipulacaoImagem.SalvarImagem(arquivo, stream);
        }

        #endregion
    }
}
