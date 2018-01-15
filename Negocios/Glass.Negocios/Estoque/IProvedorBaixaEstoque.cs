using GDA;
using Glass.Estoque.Negocios.Entidades;
using System.Collections.Generic;

namespace Glass.Estoque.Negocios
{
    public interface IProvedorBaixaEstoque
    {
        /// <summary>
        /// Realiza a baixa no estoque de volumes e chapas de vidro
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="dadosBaixarEstoque"></param>
        /// <param name="idVolume"></param>
        /// <param name="idProdImpressaoChapa"></param>
        /// <param name="manual"></param>
        /// <param name="observacao"></param>
        void BaixarEstoque(GDASession sessao, uint idLoja, IEnumerable<DetalhesBaixaEstoque> dadosBaixarEstoque,
            uint? idVolume, uint? idProdImpressaoChapa, bool manual, string observacao = null);

        void EstornaBaixaEstoque(GDASession sessao, uint idLoja, IEnumerable<DetalhesBaixaEstoque> dadosEstornoEstoque, uint? idVolume, uint? idProdImpressaoChapa);
    }
}
