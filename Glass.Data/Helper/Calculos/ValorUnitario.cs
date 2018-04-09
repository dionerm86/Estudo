using GDA;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper.Calculos.Estrategia;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;
using System;

namespace Glass.Data.Helper.Calculos
{
    sealed class ValorUnitario : BaseCalculo<ValorUnitario>
    {
        private ValorUnitario() { }

        public void Calcular(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto)
        {
            var valorUnitario = RecalcularValor(sessao, container, produto, false);

            if (valorUnitario.HasValue && produto != null)
                produto.ValorUnit = valorUnitario.Value;
        }

        public decimal? RecalcularValor(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto,
            bool valorBruto)
        {
            AtualizaDadosProdutosCalculo(produto, sessao, container);

            if (!DeveExecutar(produto))
                return null;

            if (produto.Container?.IdObra > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
                return null;

            decimal total = produto.DadosProduto.ValorTabela();

            if (produto is ProdutoTrocado && produto.ValorTabelaPedido > 0)
                total = produto.ValorTabelaPedido;

            var alturaBenef = NormalizarAlturaLarguraBeneficiamento(produto.AlturaBenef, produto);
            var larguraBenef = NormalizarAlturaLarguraBeneficiamento(produto.LarguraBenef, produto);

            var compra = produto is ProdutosCompra;
            var nf = produto is ProdutosNf;

            CalcularTotal(sessao, produto);
            total = IncluirDescontoAcrescimoComissaoNoTotal(sessao, produto, valorBruto, total);

            return CalcularValor(
                sessao,
                produto,
                total,
                compra,
                nf,
                alturaBenef,
                larguraBenef
            );
        }

        public decimal? CalcularValor(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto,
            decimal baseCalculo)
        {
            AtualizaDadosProdutosCalculo(produto, sessao, container);

            if (!DeveExecutar(produto))
                return null;

            var compra = produto is ProdutosCompra;
            var nf = produto is ProdutosNf;

            var alturaBeneficiamento = NormalizarAlturaLarguraBeneficiamento(produto.AlturaBenef, produto);
            var larguraBeneficiamento = NormalizarAlturaLarguraBeneficiamento(produto.LarguraBenef, produto);

            return CalcularValor(
                sessao,
                produto,
                baseCalculo,
                compra,
                nf,
                alturaBeneficiamento,
                larguraBeneficiamento
            );
        }

        private void CalcularTotal(GDASession sessao, IProdutoCalculo produto)
        {
            float alturaOriginalProduto = produto.Altura;

            try
            {
                produto.Altura = DefinirAlturaUsar(produto);

                // Deve passar o parâmetro usarChapaVidro como true, para que caso o produto tenha sido calculado por chapa,
                // não calcule incorretamente o total do mesmo (retornado pela variável total abaixo), estava ocorrendo
                // erro ao chamar esta função a partir de ProdutosPedidoDAO.InsereAtualizaProdProj(), sendo que o produto sendo calculado
                // possuía acréscimo de 25% em caso da área do vidro ser superior à 4m²
                ValorTotal.Instance.Calcular(
                    sessao,
                    produto.Container,
                    produto,
                    ArredondarAluminio.ArredondarApenasCalculo,
                    true,
                    produto.Beneficiamentos.CountAreaMinimaSession(null),
                    true
                );
            }
            finally
            {
                produto.Altura = alturaOriginalProduto;
            }
        }

        private float DefinirAlturaUsar(IProdutoCalculo produto)
        {
            float altura = produto.AlturaCalc;

            var vidroProjeto = produto is MaterialItemProjeto
                && produto.DadosProduto.DadosGrupoSubgrupo.ProdutoEVidro();

            var aluminioML = produto.DadosProduto.DadosGrupoSubgrupo.ProdutoEAluminio()
                && produto.DadosProduto.DadosGrupoSubgrupo.TipoCalculo() == TipoCalculoGrupoProd.ML;

            if (altura == 0 || vidroProjeto || aluminioML)
            {
                altura = produto.Altura;
            }

            return altura;
        }

        private decimal IncluirDescontoAcrescimoComissaoNoTotal(GDASession sessao, IProdutoCalculo produto, bool valorBruto, decimal total)
        {
            if (valorBruto)
            {
                return total;
            }

            if (PedidoConfig.DadosPedido.AlterarValorUnitarioProduto)
            {
                ValorBruto.Instance.Calcular(sessao, produto.Container, produto);

                if (Math.Round(total, 2) != Math.Round(produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente, 2))
                {
                    int idCliente = produto.Container?.Cliente != null
                        ? (int)produto.Container.Cliente.Id
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

        private decimal? CalcularValor(GDASession sessao, IProdutoCalculo produto, decimal baseCalculo,
            bool compra, bool nf, int alturaBeneficiamento, int larguraBeneficiamento)
        {
            var estrategia = ValorUnitarioStrategyFactory.Instance.RecuperaEstrategia(produto, nf, compra);

            var valorUnitario = estrategia.Calcular(
                sessao,
                produto,
                baseCalculo,
                ArredondarAluminio.ArredondarApenasCalculo,
                true,
                nf,
                produto.Beneficiamentos.CountAreaMinimaSession(null),
                alturaBeneficiamento,
                larguraBeneficiamento
            );

            AtualizarDadosCache(produto);

            return valorUnitario;
        }
    }
}
