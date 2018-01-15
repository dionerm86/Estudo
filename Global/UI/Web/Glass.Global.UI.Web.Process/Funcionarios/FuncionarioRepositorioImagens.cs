using System.ComponentModel.Composition;

namespace Glass.Global.UI.Web.Process.Funcionarios
{
    /// <summary>
    /// Implementação do repositório de imagens do funcionário.
    /// </summary>
    [Export(typeof(Negocios.Entidades.IFuncionarioRepositorioImagens))]
    public class FuncionarioRepositorioImagens : Negocios.Entidades.IFuncionarioRepositorioImagens
    {
        #region Variáveis Locais

        private static FuncionarioRepositorioImagens _instance;

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do repositório.
        /// </summary>
        public static FuncionarioRepositorioImagens Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FuncionarioRepositorioImagens();

                return _instance;
            }
        }

        /// <summary>
        /// Diretório das imagens dos funcionários.
        /// </summary>
        public string DiretorioImagens
        {
            get
            {
                return System.IO.Path.Combine(Armazenamento.ArmazenamentoIsolado.DiretorioUpload, "Funcionarios");
            }
        }

        #endregion

        #region Métodos Públicos

        // <summary>
        /// Recupera a Url da imagem do funcionário.
        /// </summary>
        /// <param name="idFunc">Identificador do funcionário.</param>
        /// <returns></returns>
        public string ObtemUrl(int idFunc)
        {
            return string.Format("~/Upload/Funcionarios/{0}.jpg", idFunc);
        }

        /// <summary>
        /// Recupera o caminho da imagem do funcionário.
        /// </summary>
        /// <param name="idFunc">Identificador do funcionário.</param>
        /// <returns></returns>
        public string ObtemCaminho(int idFunc)
        {
            return string.Format("{0}\\{1}.jpg", DiretorioImagens, idFunc);
        }

        /// <summary>
        /// Verifica se o funcionário possui imagem.
        /// </summary>
        /// <param name="idProd">Identificador do funcionario.</param>
        /// <returns></returns>
        public bool PossuiImagem(int idFunc)
        {
            return System.IO.File.Exists(ObtemCaminho(idFunc));
        }

        /// <summary>
        /// Salva a imagem do funcionário.
        /// </summary>
        /// <param name="idFuncionario">Identificador do funcionário.</param>
        /// <param name="stream">Stream contendo os dados da imagem que será salva.</param>
        public bool SalvarImagem(int idFuncionario, System.IO.Stream stream)
        {
            var arquivo = ObtemCaminho(idFuncionario);
            return ManipulacaoImagem.SalvarImagem(arquivo, stream);
        }

        #endregion
    }
}
