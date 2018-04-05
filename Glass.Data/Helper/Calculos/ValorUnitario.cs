using GDA;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper.Calculos.Estrategia;
using Glass.Data.Model;
using System;

namespace Glass.Data.Helper.Calculos
{
    sealed class ValorUnitario : BaseCalculo<ValorUnitario>
    {
        private ValorUnitario() { }

        public void Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima)
        {
            var valorUnitario = RecalcularValor(sessao, produto, container, calcularAreaMinima, false);

            if (valorUnitario.HasValue && produto != null)
                produto.ValorUnit = valorUnitario.Value;
        }

        public decimal? RecalcularValor(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima, bool valorBruto)
        {
            if (!DeveExecutarParaOsItens(produto, container))
                return null;

            if (container.IdObra > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
                return null;

            decimal total = container.DadosProduto.ValorTabela(sessao, produto);

            if (produto is ProdutoTrocado && produto.ValorTabelaPedido > 0)
                total = produto.ValorTabelaPedido;

            var alturaBenef = NormalizarAlturaLarguraBeneficiamento(produto.AlturaBenef, produto);
            var larguraBenef = NormalizarAlturaLarguraBeneficiamento(produto.LarguraBenef, produto);

            var compra = produto is ProdutosCompra;
            var nf = produto is ProdutosNf;

            total = CalcularTotal(sessao, produto, container, calcularAreaMinima, total, alturaBenef, larguraBenef, compra, nf);
            total = IncluirDescontoAcrescimoComissaoNoTotal(sessao, produto, container, valorBruto, total);

            return CalcularValor(
                sessao,
                produto,
                container,
                calcularAreaMinima,
                total,
                compra,
                nf,
                alturaBenef,
                larguraBenef
            );
        }

        public decimal? CalcularValor(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima, decimal baseCalculo)
        {
            if (!DeveExecutarParaOsItens(produto, container))
                return null;

            var compra = produto is ProdutosCompra;
            var nf = produto is ProdutosNf;

            var alturaBenef = NormalizarAlturaLarguraBeneficiamento(produto.AlturaBenef, produto);
            var larguraBenef = NormalizarAlturaLarguraBeneficiamento(produto.LarguraBenef, produto);

            return CalcularValor(
                sessao,
                produto,
                container,
                calcularAreaMinima,
                baseCalculo,
                compra,
                nf,
                alturaBenef,
                larguraBenef
            );
        }

        private decimal CalcularTotal(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container, bool calcularAreaMinima,
            decimal baseCalculo, int alturaBenef, int larguraBenef, bool compra, bool nf)
        {
            float altura = DefinirAlturaUsar(sessao, produto, container);
            float totM2 = produto.TotM;
            float totM2Calc = produto.TotM2Calc;

            decimal custo = 0;
            decimal total = baseCalculo;

            uint idCliente = container.Cliente != null
                ? container.Cliente.Id
                : default(uint);

            // Deve passar o parâmetro usarChapaVidro como true, para que caso o produto tenha sido calculado por chapa,
            // não calcule incorretamente o total do mesmo (retornado pela variável total abaixo), estava ocorrendo
            // erro ao chamar esta função a partir de ProdutosPedidoDAO.InsereAtualizaProdProj(), sendo que o produto sendo calculado
            // possuía acréscimo de 25% em caso da área do vidro ser superior à 4m²
            ProdutoDAO.Instance.CalcTotaisItemProd(
                sessao,
                idCliente,
                (int)produto.IdProduto,
                produto.Largura,
                produto.Qtde,
                produto.QtdeAmbiente,
                total,
                produto.Espessura,
                produto.Redondo,
                2,
                compra,
                true,
                ref custo,
                ref altura,
                ref totM2,
                ref totM2Calc,
                ref total,
                alturaBenef,
                larguraBenef,
                nf,
                produto.Beneficiamentos.CountAreaMinimaSession(sessao),
                true,
                calcularAreaMinima
            );

            return total;
        }

        private float DefinirAlturaUsar(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container)
        {
            float altura = produto.AlturaCalc;

            var vidroProjeto = produto is MaterialItemProjeto
                && container.DadosProduto.ProdutoEVidro(sessao, produto);

            var aluminioML = container.DadosProduto.ProdutoEAluminio(sessao, produto)
                && container.DadosProduto.TipoCalculo(sessao, produto) == TipoCalculoGrupoProd.ML;

            if (altura == 0 || vidroProjeto || aluminioML)
            {
                altura = produto.Altura;
            }

            return altura;
        }

        private decimal IncluirDescontoAcrescimoComissaoNoTotal(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container, bool valorBruto, decimal total)
        {
            if (valorBruto)
            {
                return total;
            }

            if (PedidoConfig.DadosPedido.AlterarValorUnitarioProduto)
            {
                ValorBruto.Instance.Calcular(sessao, produto, container);

                if (Math.Round(total, 2) != Math.Round(produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente, 2))
                {
                    int idCliente = container.Cliente != null
                        ? (int)container.Cliente.Id
                        : default(int);

                    var produtoPossuiValorTabela = ProdutoDAO.Instance.ProdutoPossuiValorTabela(null, produto.IdProduto, produto.ValorUnitarioBruto);
                    var produtoPossuiDesconto = DescontoAcrescimoClienteDAO.Instance.ProdutoPossuiDesconto(null, idCliente, (int)produto.IdProduto);

                    if (total == 0 || !produtoPossuiValorTabela || produtoPossuiDesconto)
                        total = Math.Max(total, produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente);
                }
            }

            var desconto = PedidoConfig.RatearDescontoProdutos
                ? produto.ValorDesconto + produto.ValorDescontoProd
                : 0;

            return total
                + produto.ValorComissao
                + produto.ValorAcrescimo
                + produto.ValorAcrescimoProd
                - desconto;
        }

        private int NormalizarAlturaLarguraBeneficiamento(int? valor, IProdutoCalculo produto)
        {
            if (valor.HasValue && produto.AlturaBenef > 0 && produto.LarguraBenef > 0)
            {
                return valor.Value;
            }

            return 2;
        }

        private decimal? CalcularValor(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima, decimal baseCalculo, bool compra, bool nf, int alturaBenef, int larguraBenef)
        {
            var estrategia = ValorUnitarioStrategyFactory.Instance.RecuperaEstrategia(produto, nf, compra);

            var valorUnitario = estrategia.Calcular(
                sessao,
                produto,
                container,
                baseCalculo,
                true,
                true,
                nf,
                produto.Beneficiamentos.CountAreaMinimaSession(sessao),
                calcularAreaMinima,
                alturaBenef,
                larguraBenef
            );

            AtualizarDadosCache(produto, container);

            return valorUnitario;
        }
    }
}
