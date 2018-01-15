using System.ComponentModel.Composition;

namespace Glass.Global.UI.Web.Process.Cliente
{
    /// <summary>
    /// Implementação do repositório de imagens do cliente.
    /// </summary>
    [Export(typeof(Negocios.Entidades.IClienteRepositorioImagens))]
    public class ClienteRepositorioImagens : Negocios.Entidades.IClienteRepositorioImagens
    {
        #region Variáveis Locais

        private static ClienteRepositorioImagens _instance;

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do repositório.
        /// </summary>
        public static ClienteRepositorioImagens Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ClienteRepositorioImagens();

                return _instance;
            }
        }

        /// <summary>
        /// Diretório das imagens dos clientes.
        /// </summary>
        public string DiretorioImagens
        {
            get
            {
                return System.IO.Path.Combine(Armazenamento.ArmazenamentoIsolado.DiretorioUpload, "Clientes");
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera a Url da imagem do cliente.
        /// </summary>
        public string ObterUrl(int idCliente)
        {
            return string.Format("~/Upload/Clientes/Logo{0}.png", idCliente);
        }

        /// <summary>
        /// Recupera o caminho da imagem do cliente.
        /// </summary>
        public string ObterCaminho(int idCliente)
        {
            return string.Format("{0}\\Logo{1}.png", DiretorioImagens, idCliente);
        }

        /// <summary>
        /// Verifica se o cliente possui imagem.
        /// </summary>
        public bool PossuiImagem(int idCliente)
        {
            return System.IO.File.Exists(ObterCaminho(idCliente));
        }

        /// <summary>
        /// Salva a imagem do cliente.
        /// </summary>
        /// <param name="idCliente">Identificador do cliente.</param>
        /// <param name="stream">Stream contendo os dados da imagem que será salva.</param>
        public bool SalvarImagem(int idCliente, System.IO.Stream stream)
        {
            var arquivo = ObterCaminho(idCliente);
            return ManipulacaoImagem.SalvarImagem(arquivo, stream);
        }

        #endregion
    }
}
