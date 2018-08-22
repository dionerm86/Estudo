using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor dos planos de corte.
    /// </summary>
    public interface IProvedorPlanoCorte
    {
        /// <summary>
        /// Obtém o número de etiqueta para o plano de corte.
        /// </summary>
        /// <param name="planoOtimizacao">Nome do plano de otimização pai.</param>
        /// <param name="posicaoPlanoCorte">Posição do plano de corte.</param>
        /// <param name="quantidadePlanosCorte">Quantidade de planos de corte no plano de otimização.</param>
        /// <returns></returns>
        string ObterNumeroEtiqueta(string planoOtimizacao, int posicaoPlanoCorte, int quantidadePlanosCorte);

        /// <summary>
        /// Obtém os produtos de impressão associados do o plano de corte.
        /// </summary>
        /// <param name="planoCorte"></param>
        /// <returns></returns>
        IEnumerable<Data.Model.ProdutoImpressao> ObterProdutosImpressao(PlanoCorte planoCorte);
    }
}
