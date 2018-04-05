using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Global;

namespace Glass.Data.Helper.Calculos
{
    sealed class DiferencaCliente : BaseCalculo<DiferencaCliente>
    {
        private DiferencaCliente() { }

        public void Calcular(IProdutoCalculo produto, IContainerCalculo container)
        {
            if (!DeveExecutarParaOsItens(produto, container))
                return;

            bool revenda = ClienteDAO.Instance.IsRevenda(null, container.IdCliente);
            decimal valorTabela = ValorTabelaProduto(produto, container, revenda, null);
            decimal valorCliente = ValorTabelaProduto(produto, container, revenda, container.IdCliente);

            int tipoCalculoProduto = GrupoProdDAO.Instance.TipoCalculo(null, (int)produto.IdProduto);

            // Compara os valores
            if (valorTabela < valorCliente)
            {
                produto.ValorDescontoCliente = 0;
                produto.ValorAcrescimoCliente = CalculaValorTotal(produto, tipoCalculoProduto, valorCliente - valorTabela);
            }
            else if (valorTabela > valorCliente)
            {
                produto.ValorAcrescimoCliente = 0;
                produto.ValorDescontoCliente = CalculaValorTotal(produto, tipoCalculoProduto, valorTabela - valorCliente);
            }
            else
            {
                produto.ValorAcrescimoCliente = 0;
                produto.ValorDescontoCliente = 0;
            }

            AtualizarDadosCache(produto, container);
        }

        private decimal ValorTabelaProduto(IProdutoCalculo produto, IContainerCalculo container,
            bool revenda, uint? idCliente)
        {
            return ProdutoDAO.Instance.GetValorTabela(
                null,
                (int)produto.IdProduto,
                container.TipoEntrega,
                idCliente,
                revenda,
                container.Reposicao,
                produto.PercDescontoQtde,
                container.IdPedido(),
                container.IdProjeto(),
                container.IdOrcamento()
            );
        }

        private decimal CalculaValorTotal(IProdutoCalculo produto, int tipoCalculo, decimal baseCalculo)
        {
            return CalculosFluxo.CalcTotaisItemProdFast(
                null,
                tipoCalculo,
                produto.AlturaCalc,
                produto.Largura,
                produto.Qtde,
                produto.TotM2Calc,
                baseCalculo
            );
        }
    }
}
