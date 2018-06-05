using Glass.Global.Negocios.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor de ICMS de produto por UF.
    /// </summary>
    public interface IProvedorIcmsProdutoUf
    {
        /// <summary>
        /// Busca o valor do ICMS por produto e UF.
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="loja">Loja que será usada na pesquisa.</param>
        /// <param name="fornecedor">Fornecedor que será usado na pesquisa.</param>
        /// <param name="cliente">Cliente que será usado na pesquisa</param>
        /// <returns></returns>
        float ObterIcmsPorProduto(Produto produto, Loja loja, Fornecedor fornecedor, Cliente cliente);

        /// <summary>
        /// Obtém a alíquota de ICMS ST que será utilizada.
        /// Será o valor configurado no produto, se for maior que zero;
        /// caso contrário, será a alíquota de ICMS intraestadual.
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="loja"></param>
        /// <param name="fornecedor"></param>
        /// <param name="cliente"></param>
        /// <returns></returns>
        float ObterAliquotaIcmsSt(Produto produto, Loja loja, Fornecedor fornecedor, Cliente cliente);

        /// <summary>
        /// Busca o valor do FCP por produto e UF.
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="loja"></param>
        /// <param name="fornecedor"></param>
        /// <param name="cliente"></param>
        /// <returns></returns>
        float ObterFCPPorProduto(Produto produto, Loja loja, Fornecedor fornecedor, Cliente cliente);

        /// <summary>
        /// Busca o valor da Aliquota FCP por produto e UF.
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="loja"></param>
        /// <param name="fornecedor"></param>
        /// <param name="cliente"></param>
        /// <returns></returns>
        float ObterAliquotaFCPSTPorProduto(Produto produto, Loja loja, Fornecedor fornecedor, Cliente cliente);
    }
}
