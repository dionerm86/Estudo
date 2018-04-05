using GDA;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Global;

namespace Glass.Data.Helper.Calculos
{
    sealed class DiferencaCliente : BaseCalculo<DiferencaCliente>
    {
        private DiferencaCliente() { }

        public void Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container)
        {
            if (!DeveExecutarParaOsItens(produto, container))
                return;
            
            decimal valorTabela = container.DadosProduto.ValorTabela(sessao, produto, false);
            decimal valorCliente = container.DadosProduto.ValorTabela(sessao, produto, true);

            int tipoCalculoProduto = (int)container.DadosProduto.TipoCalculo(sessao, produto);
            
            if (valorTabela < valorCliente)
            {
                produto.ValorDescontoCliente = 0;
                produto.ValorAcrescimoCliente = CalculaValorTotal(sessao, produto, tipoCalculoProduto, valorCliente - valorTabela);
            }
            else if (valorTabela > valorCliente)
            {
                produto.ValorAcrescimoCliente = 0;
                produto.ValorDescontoCliente = CalculaValorTotal(sessao, produto, tipoCalculoProduto, valorTabela - valorCliente);
            }
            else
            {
                produto.ValorAcrescimoCliente = 0;
                produto.ValorDescontoCliente = 0;
            }

            AtualizarDadosCache(produto, container);
        }

        private decimal CalculaValorTotal(GDASession sessao, IProdutoCalculo produto, int tipoCalculo, decimal baseCalculo)
        {
            return CalculosFluxo.CalcTotaisItemProdFast(
                sessao,
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
