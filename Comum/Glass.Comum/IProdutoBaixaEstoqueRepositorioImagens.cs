using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass
{
    public interface IProdutoBaixaEstoqueRepositorioImagens
    {
        #region Métodos Públicos

        /// <summary>
        /// Recupera a Url da imagem do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        string ObtemUrl(int idProdBaixaEst);

        /// <summary>
        /// Recupera a url da imagem do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        string ObtemCaminho(int idProdBaixaEst);

        /// <summary>
        /// Verifica se o produto possui imagem.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        bool PossuiImagem(int idProdBaixaEst);

        /// <summary>
        /// Recupera a imagem do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <param name="stream">Stream onde será grava a imagem do produto.</param>
        /// <returns>True caso a imagem tenha sido recuperada com sucesso.</returns>
        bool ObtemImagem(int idProdBaixaEst, System.IO.Stream stream);

        /// <summary>
        /// Salva a imagemd o produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <param name="stream">Stream contendo os dados da imagem que será salva.</param>
        bool SalvarImagem(int idProdBaixaEst, System.IO.Stream stream);

        #endregion
    }
}
