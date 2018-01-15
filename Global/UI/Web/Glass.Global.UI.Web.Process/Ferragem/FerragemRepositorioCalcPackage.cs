using System.ComponentModel.Composition;
using System.IO;

namespace Glass.Global.UI.Web.Process.Ferragem
{
    /// <summary>
    /// Implementação do repositório de CalcPackage da ferragem.
    /// </summary>
    [Export(typeof(Projeto.Negocios.Entidades.IFerragemRepositorioCalcPackage))]
    public class FerragemRepositorioCalcPackage : Projeto.Negocios.Entidades.IFerragemRepositorioCalcPackage
    {
        #region Variáveis Locais

        private static FerragemRepositorioCalcPackage _instance;

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do repositório.
        /// </summary>
        public static FerragemRepositorioCalcPackage Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FerragemRepositorioCalcPackage();

                return _instance;
            }
        }

        /// <summary>
        /// Diretório dos CalcPackage dos ferragems.
        /// </summary>
        public string DiretorioCalcPackage
        {
            get
            {
                return System.IO.Path.Combine(Armazenamento.ArmazenamentoIsolado.DiretorioUpload, "Ferragem\\CalcPackage");
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera a Url do CalcPackage da ferragem.
        /// </summary>
        /// <param name="idFerragem">Identificador do ferragem.</param>
        public string ObterUrl(int idFerragem)
        {
            return string.Format("~/Upload/Ferragem/CalcPackage/{0}.calcpackage", idFerragem);
        }

        /// <summary>
        /// Recupera o caminho do CalcPackage da ferragem.
        /// </summary>
        /// <param name="idFerragem">Identificador do ferragem.</param>
        public string ObterCaminho(int idFerragem)
        {
            return string.Format("{0}\\{1}.calcpackage", DiretorioCalcPackage, idFerragem);
        }

        /// <summary>
        /// Verifica se a ferragem possui CalcPackage.
        /// </summary>
        /// <param name="idFerragem">Identificador da ferragem.</param>
        /// <returns></returns>
        public bool PossuiCalcPackage(int idFerragem)
        {
            return System.IO.File.Exists(ObterCaminho(idFerragem));
        }

        /// <summary>
        /// Salva o CalcPackage da ferragem.
        /// </summary>
        /// <param name="idFerragem">Identificador da ferragem.</param>
        /// <param name="stream">Stream contendo os dados do CalcPackage que será salva.</param>
        public bool SalvarCalcPackage(int idFerragem, System.IO.Stream stream)
        {
            var arquivo = ObterCaminho(idFerragem);

            // Caso o a ferragem já possua CalcPackage, o arquivo deve ser removido, para que o novo CalcPackage seja inserido.
            if (File.Exists(arquivo))
                File.Delete(arquivo);

            return ManipulacaoImagem.SalvarImagem(arquivo, stream);
        }

        #endregion
    }
}
