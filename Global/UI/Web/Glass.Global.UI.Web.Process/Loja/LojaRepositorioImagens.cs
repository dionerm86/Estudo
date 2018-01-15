using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;

namespace Glass.Global.UI.Web.Process.Loja
{
    /// <summary>
    /// Implementação do repositório de imagens do funcionário.
    /// </summary>
    [Export(typeof(Negocios.Entidades.ILojaRepositorioImagens))]
    public class LojaRepositorioImagens : Negocios.Entidades.ILojaRepositorioImagens
    {
        #region Variáveis Locais

        private static LojaRepositorioImagens _instance;

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do repositório.
        /// </summary>
        public static LojaRepositorioImagens Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LojaRepositorioImagens();

                return _instance;
            }
        }

        /// <summary>
        /// Diretório das imagens.
        /// </summary>
        public string DiretorioImagens
        {
            get
            {
                return Armazenamento.ArmazenamentoIsolado.DiretorioImages;
            }
        }

        #endregion

        #region Métodos Públicos

        // <summary>
        /// Recupera a Url da imagem da loja.
        /// </summary>
        /// <param name="idFunc">Identificador do funcionário.</param>
        /// <returns></returns>
        public string ObtemUrl(int idLoja, bool cor)
        {
            var nomeArquivo = string.Format("logo{0}{1}{2}", 
                Data.Helper.ControleSistema.GetSite(), 
                (cor ? "Color" : string.Empty), idLoja);
            return string.Format("~/Images/{0}.png", nomeArquivo);
        }

        public string ObtemCaminho(int idLoja, bool cor)
        {
            var nomeArquivo = string.Format("logo{0}{1}{2}",
                Data.Helper.ControleSistema.GetSite(),
                (cor ? "Color" : string.Empty), idLoja);

            return string.Format("{0}\\{1}.png", DiretorioImagens, nomeArquivo);
        }

        /// <summary>
        /// Verifica se o funcionário possui imagem.
        /// </summary>
        /// <param name="idProd">Identificador do funcionario.</param>
        /// <returns></returns>
        public bool PossuiImagem(int idLoja, bool cor)
        {
            return File.Exists(ObtemCaminho(idLoja,cor));
        }

        /// <summary>
        /// Salva a imagem do funcionário.
        /// </summary>
        /// <param name="idFuncionario">Identificador do funcionário.</param>
        /// <param name="stream">Stream contendo os dados da imagem que será salva.</param>
        public bool SalvarImagem(int idLoja, bool cor, Stream stream)
        {
            var arquivo = ObtemCaminho(idLoja, cor);
            return ManipulacaoImagem.SalvarImagem(arquivo, stream);
        }

        #endregion 
    }
}