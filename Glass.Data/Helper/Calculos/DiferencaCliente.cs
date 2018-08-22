using GDA;
using Glass.Data.Model;
using Glass.Global;
using Glass.Pool;

namespace Glass.Data.Helper.Calculos
{
    sealed class DiferencaCliente : Singleton<DiferencaCliente>
    {
        private DiferencaCliente() { }

        public void Calcular(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto)
        {
            produto.InicializarParaCalculo(sessao, container);

            decimal valorTabela = produto.DadosProduto.ValorTabela(false);
            decimal valorCliente = produto.DadosProduto.ValorTabela(true);

            int tipoCalculoProduto = (int)produto.DadosProduto.DadosGrupoSubgrupo.TipoCalculo();

            if (valorTabela < valorCliente)
            {
                produto.ValorDescontoCliente = 0;
                produto.ValorAcrescimoCliente = CalculaValorTotal(sessao, produto, tipoCalculoProduto,
                    valorCliente - valorTabela);
            }
            else if (valorTabela > valorCliente)
            {
                produto.ValorAcrescimoCliente = 0;
                produto.ValorDescontoCliente = CalculaValorTotal(sessao, produto, tipoCalculoProduto,
                    valorTabela - valorCliente);
            }
            else
            {
                produto.ValorAcrescimoCliente = 0;
                produto.ValorDescontoCliente = 0;
            }
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
