using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Global.Negocios.Entidades
{
    public interface ILojaRepositorioImagens
    {
        /// <summary>
        /// Recupera a Url da imagem da Loja.
        /// </summary>
        /// <param name="idFunc">Identificador do funcionário.</param>
        /// <returns></returns>
        string ObtemUrl(int IdLoja, bool cor);

        /// <summary>
        /// Salva a imagem da Loja.
        /// </summary>
        bool SalvarImagem(int IdLoja, bool cor, System.IO.Stream stream);
    }
}
