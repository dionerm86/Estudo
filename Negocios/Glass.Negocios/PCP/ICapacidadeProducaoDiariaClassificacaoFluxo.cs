using System;
using System.Collections.Generic;

namespace Glass.PCP.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio da capacidade diária da classificacao
    /// </summary>
    public interface ICapacidadeProducaoDiariaClassificacaoFluxo
    {
        /// <summary>
        /// Obtem a capacidade pela data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IList<Entidades.CapacidadeDiariaProducaoClassificacao> ObtemPelaData(DateTime data);

        /// <summary>
        /// Salva a capacidade Diaria.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="capacidades"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult Salvar(DateTime data, IEnumerable<Entidades.CapacidadeDiariaProducaoClassificacao> capacidades);

        /// <summary>
        /// Obtem a capacidade pela data
        /// </summary>
        /// <param name="idClassificacao"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        int ObtemPelaData(int idClassificacao, DateTime data);
    }
}
