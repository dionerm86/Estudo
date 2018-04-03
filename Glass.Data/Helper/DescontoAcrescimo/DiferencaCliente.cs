using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Global;
using Glass.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Helper.DescontoAcrescimo
{
    public class DiferencaCliente : PoolableObject<DiferencaCliente>
    {
        private DiferencaCliente() { }

        public void Calcular(IProdutoDescontoAcrescimo produto, IContainerDescontoAcrescimo container)
        {
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
        }

        private decimal ValorTabelaProduto(IProdutoDescontoAcrescimo produto, IContainerDescontoAcrescimo container,
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

        private decimal CalculaValorTotal(IProdutoDescontoAcrescimo produto, int tipoCalculo, decimal baseCalculo)
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
