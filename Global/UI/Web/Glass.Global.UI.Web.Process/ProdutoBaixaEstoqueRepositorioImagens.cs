using Glass.Global.Negocios.Entidades;

namespace Glass.Global.UI.Web.Process
{
    /// <summary>
    /// Implementação do repositório de imagens dos produtos.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IProdutoBaixaEstoqueRepositorioImagens))]
    public class ProdutoBaixaEstoqueRepositorioImagens : IProdutoBaixaEstoqueRepositorioImagens
    {
        #region Variáveis Locais

        private static ProdutoBaixaEstoqueRepositorioImagens _instance;

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do repositório.
        /// </summary>
        public static ProdutoBaixaEstoqueRepositorioImagens Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ProdutoBaixaEstoqueRepositorioImagens();

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
                return System.IO.Path.Combine(Armazenamento.ArmazenamentoIsolado.DiretorioUpload, "ProdutosBaixaEstoque");
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera a Url da imagem do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        public string ObtemUrl(int idProdBaixaEst)
        {
            return string.Format("~/Upload/ProdutosBaixaEstoque/{0}.jpg", idProdBaixaEst);
        }

        /// <summary>
        /// Recupera o caminho da imagem do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        public string ObtemCaminho(int idProdBaixaEst)
        {
            return string.Format("{0}\\{1}.jpg", DiretorioImagens, idProdBaixaEst);
        }

        /// <summary>
        /// Verifica se o produto possui imagem.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        public bool PossuiImagem(int idProdBaixaEst)
        {
            return System.IO.File.Exists(ObtemCaminho(idProdBaixaEst));
        }

        /// <summary>
        /// Recupera a imagem do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <param name="stream">Stream onde será grava a imagem do produto.</param>
        /// <returns>True caso a imagem tenha sido recuperada com sucesso.</returns>
        public bool ObtemImagem(int idProdBaixaEst, System.IO.Stream stream)
        {
            var arquivo = ObtemCaminho(idProdBaixaEst);

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
        public bool SalvarImagem(int idProdBaixaEst, System.IO.Stream stream)
        {
            var arquivo = ObtemCaminho(idProdBaixaEst);
            return ManipulacaoImagem.SalvarImagem(arquivo, stream);
        }

        #endregion
    }
}
