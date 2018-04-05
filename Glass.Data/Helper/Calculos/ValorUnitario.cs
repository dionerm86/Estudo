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

        public void Calcular(IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima)
        {
            var valorUnitario = RecalcularValor(produto, container, calcularAreaMinima, false);

            if (valorUnitario.HasValue && produto != null)
                produto.ValorUnit = valorUnitario.Value;
        }

        public decimal? RecalcularValor(IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima, bool valorBruto)
        {
            if (!DeveExecutarParaOsItens(produto, container))
                return null;

            if (container.IdObra > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
                return null;

            var clienteRevenda = container.Cliente != null && container.Cliente.Revenda;

            float altura = produto.AlturaCalc, totM2 = produto.TotM, totM2Calc = produto.TotM2Calc;
            decimal custo = 0;
            decimal total = ProdutoDAO.Instance.GetValorTabela(produto, container, clienteRevenda);

            if (produto is ProdutoTrocado && produto.ValorTabelaPedido > 0)
                total = produto.ValorTabelaPedido;

            if (altura == 0)
                altura = produto.Altura;

            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(null, (int)produto.IdProduto);

            if (produto is MaterialItemProjeto && GrupoProdDAO.Instance.IsVidro(idGrupoProd))
                altura = produto.Altura;
            else if (GrupoProdDAO.Instance.IsAluminio(idGrupoProd) &&
                GrupoProdDAO.Instance.TipoCalculo(null, (int)produto.IdProduto) == (int)TipoCalculoGrupoProd.ML)
                altura = produto.Altura;

            var alturaBenef = NormalizarAlturaLarguraBeneficiamento(produto.AlturaBenef, produto);
            var larguraBenef = NormalizarAlturaLarguraBeneficiamento(produto.LarguraBenef, produto);

            var compra = produto is ProdutosCompra;
            var nf = produto is ProdutosNf;
            
            // Deve passar o parâmetro usarChapaVidro como true, para que caso o produto tenha sido calculado por chapa,
            // não calcule incorretamente o total do mesmo (retornado pela variável total abaixo), estava ocorrendo
            // erro ao chamar esta função a partir de ProdutosPedidoDAO.InsereAtualizaProdProj(), sendo que o produto sendo calculado
            // possuía acréscimo de 25% em caso da área do vidro ser superior à 4m²
            ProdutoDAO.Instance.CalcTotaisItemProd(
                null,
                container.IdCliente.GetValueOrDefault(),
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
                produto.Beneficiamentos.CountAreaMinimaSession(null),
                true,
                calcularAreaMinima
            );

            total = RecalcularTotal(produto, container, valorBruto, total);

            return CalcularValor(
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

        public decimal? CalcularValor(IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima, decimal baseCalculo)
        {
            if (!DeveExecutarParaOsItens(produto, container))
                return null;

            var compra = produto is ProdutosCompra;
            var nf = produto is ProdutosNf;

            var alturaBenef = NormalizarAlturaLarguraBeneficiamento(produto.AlturaBenef, produto);
            var larguraBenef = NormalizarAlturaLarguraBeneficiamento(produto.LarguraBenef, produto);

            return CalcularValor(
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

        private decimal? CalcularValor(IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima, decimal baseCalculo, bool compra, bool nf, int alturaBenef, int larguraBenef)
        {
            var estrategia = ValorUnitarioStrategyFactory.Instance.RecuperaEstrategia(produto, nf, compra);

            var valorUnitario = estrategia.Calcular(
                produto,
                container,
                baseCalculo,
                true,
                true,
                nf,
                produto.Beneficiamentos.CountAreaMinimaSession(null),
                calcularAreaMinima,
                alturaBenef,
                larguraBenef
            );

            AtualizarDadosCache(produto, container);

            return valorUnitario;
        }

        private decimal RecalcularTotal(IProdutoCalculo produto, IContainerCalculo container, bool valorBruto, decimal total)
        {
            if (!valorBruto)
            {
                if (PedidoConfig.DadosPedido.AlterarValorUnitarioProduto)
                {
                    ValorBruto.Instance.Calcular(produto, container);

                    if (Math.Round(total, 2) != Math.Round(produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente, 2))
                    {
                        var produtoPossuiValorTabela = ProdutoDAO.Instance.ProdutoPossuiValorTabela(null, produto.IdProduto, produto.ValorUnitarioBruto);
                        var produtoPossuiDesconto = DescontoAcrescimoClienteDAO.Instance.ProdutoPossuiDesconto(null, (int)container.IdCliente.GetValueOrDefault(0), (int)produto.IdProduto);

                        if (total == 0 || !produtoPossuiValorTabela || produtoPossuiDesconto)
                            total = Math.Max(total, produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente);
                    }
                }

                var desconto = PedidoConfig.RatearDescontoProdutos
                    ? produto.ValorDesconto + produto.ValorDescontoProd
                    : 0;

                total = total
                    + produto.ValorComissao
                    + produto.ValorAcrescimo
                    + produto.ValorAcrescimoProd
                    - desconto;
            }

            return total;
        }

        private int NormalizarAlturaLarguraBeneficiamento(int? valor, IProdutoCalculo produto)
        {
            if (valor.HasValue && produto.AlturaBenef > 0 && produto.LarguraBenef > 0)
            {
                return valor.Value;
            }

            return 2;
        }
    }
}
