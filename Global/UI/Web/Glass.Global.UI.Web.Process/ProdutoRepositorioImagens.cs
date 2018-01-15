using Glass.Global.Negocios.Entidades;

namespace Glass.Global.UI.Web.Process
{
    /// <summary>
    /// Implementação do repositório de imagens dos produtos.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IProdutoRepositorioImagens))]
    public class ProdutoRepositorioImagens : IProdutoRepositorioImagens
    {
        #region Variáveis Locais

        private static ProdutoRepositorioImagens _instance;

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do repositório.
        /// </summary>
        public static ProdutoRepositorioImagens Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ProdutoRepositorioImagens();

                return _instance;
            }
        }

        /// <summary>
        /// Diretório das imagens dos produtos.
        /// </summary>
        public string DiretorioImagens
        {
            get
            {
                return System.IO.Path.Combine(Armazenamento.ArmazenamentoIsolado.DiretorioUpload, "Produtos");
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera a Url da imagem do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        public string ObtemUrl(int idProd)
        {
            return string.Format("~/Upload/Produtos/{0}.jpg", idProd);
        }

        /// <summary>
        /// Recupera o caminho da imagem do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        public string ObtemCaminho(int idProd)
        {
            return string.Format("{0}\\{1}.jpg", DiretorioImagens, idProd);
        }

        /// <summary>
        /// Verifica se o produto possui imagem.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        public bool PossuiImagem(int idProd)
        {
            return System.IO.File.Exists(ObtemCaminho(idProd));
        }

        /// <summary>
        /// Recupera a imagem do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <param name="stream">Stream onde será grava a imagem do produto.</param>
        /// <returns>True caso a imagem tenha sido recuperada com sucesso.</returns>
        public bool ObtemImagem(int idProd, System.IO.Stream stream)
        {
            var arquivo = ObtemCaminho(idProd);

            if (System.IO.File.Exists(arquivo))
            {
                using (var fs = System.IO.File.OpenRead(arquivo))
                {
                    int read = 0;
                    byte[] buffer = new byte[1024];

                    while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                        stream.Write(buffer, 0, read);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Salva a imagemd o produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <param name="stream">Stream contendo os dados da imagem que será salva.</param>
        public bool SalvarImagem(int idProd, System.IO.Stream stream)
        {
            var arquivo = ObtemCaminho(idProd);
            return ManipulacaoImagem.SalvarImagem(arquivo, stream);   
        }

        #endregion
    }
}
