using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios
{
    public interface IIestUfLojaFluxo
    {
        /// <summary>
        /// Recupera a lista de IEST cadastrado por loja
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        IList<Entidades.IestUfLoja> PesquisarIest(uint idLoja);

        /// <summary>
        /// Recupera o IEST pelo Id.
        /// </summary>
        /// <param name="idIestUfLoja"></param>
        /// <returns></returns>
        Entidades.IestUfLoja ObtemIestUfLoja(uint idIestUfLoja);

        /// <summary>
        /// Recupera o IEST pela loja e nomeUF
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="nomeUf"></param>
        /// <returns></returns>
        Entidades.IestUfLoja ObtemIestUfLoja(uint idLoja, string nomeUf);

        /// <summary>
        /// Salva os dados do IEST
        /// </summary>
        /// <param name="iestUfLoja"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarIestUfLoja(Entidades.IestUfLoja iestUfLoja);

        /// <summary>
        /// Apaga os dados do IEST
        /// </summary>
        /// <param name="iestUfLoja"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarIestUfLoja(Entidades.IestUfLoja iestUfLoja);
    }
}
