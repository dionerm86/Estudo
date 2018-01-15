using System.ComponentModel.Composition;

namespace Glass.Global.UI.Web.Process.RelatorioDinamico
{
    /// <summary>
    /// Implementação do repositório de arquivos do relatório dinâmico.
    /// </summary>
    [Export(typeof(Negocios.Entidades.IRelatorioDinamicoRepositorioArquivos))]
    public class RelatorioDinamicoRepositorioArquivos : Negocios.Entidades.IRelatorioDinamicoRepositorioArquivos
    {
        #region Variáveis Locais

        private static RelatorioDinamicoRepositorioArquivos _instance;

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do repositório.
        /// </summary>
        public static RelatorioDinamicoRepositorioArquivos Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RelatorioDinamicoRepositorioArquivos();

                return _instance;
            }
        }

        /// <summary>
        /// Diretório dos arquivos do relatório.
        /// </summary>
        public string DiretorioImagens
        {
            get
            {
                return System.IO.Path.Combine(Armazenamento.ArmazenamentoIsolado.DiretorioUpload, "RelatorioDinamico");
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera a Url do arquivo do relatório.
        /// </summary>
        /// <param name="idRelatorioDinamico">Identificador do relatório.</param>
        /// <returns></returns>
        public string ObterUrl(int idRelatorioDinamico)
        {
            return string.Format("~/Upload/RelatorioDinamico/{0}.rdlc", idRelatorioDinamico);
        }

        /// <summary>
        /// Recupera o caminho do arquivo do relatório.
        /// </summary>
        /// <param name="idRelatorioDinamico">Identificador do relatório.</param>
        /// <returns></returns>
        public string ObterCaminho(int idRelatorioDinamico)
        {
            return string.Format("{0}\\{1}.rdlc", DiretorioImagens, idRelatorioDinamico);
        }

        /// <summary>
        /// Verifica se o relatório possui arquivo.
        /// </summary>
        /// <param name="idRelatorioDinamico">Identificador do relatório.</param>
        /// <returns></returns>
        public bool PossuiArquivo(int idRelatorioDinamico)
        {
            return System.IO.File.Exists(ObterCaminho(idRelatorioDinamico));
        }

        /// <summary>
        /// Salva o arquivo do relatório.
        /// </summary>
        /// <param name="idRelatorioDinamico">Identificador do relatório.</param>
        /// <param name="stream">Stream contendo os dados do arquivo que será salvo.</param>
        public bool SalvarArquivo(int idRelatorioDinamico, System.IO.Stream stream)
        {
            var arquivo = ObterCaminho(idRelatorioDinamico);
            return ManipulacaoImagem.SalvarImagem(arquivo, stream);
        }

        #endregion
    }
}
