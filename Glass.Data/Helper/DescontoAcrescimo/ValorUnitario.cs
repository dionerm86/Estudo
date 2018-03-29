using Glass.Configuracoes;
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
    class ValorUnitario : PoolableObject<ValorUnitario>
    {
        private ValorUnitario() { }

        public void Calcular(IProdutoDescontoAcrescimo produto, IContainerDescontoAcrescimo container,
            bool calcularAreaMinima)
        {
            if (container.IdObra > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
                return;

            var clienteRevenda = container.IdCliente.HasValue
                ? ClienteDAO.Instance.IsRevenda(container.IdCliente.Value)
                : false;

            float altura = produto.AlturaCalc, totM2 = produto.TotM, totM2Calc = produto.TotM2Calc;
            decimal custo = 0, total = ProdutoDAO.Instance.GetValorTabela(sessao, (int)produto.IdProduto, tipoEntrega, idCliente, clienteRevenda, container.Reposicao,
                produto.PercDescontoQtde, idPedido, idProjeto, idOrcamento);

            // Ao efetuar troca de produto, deve-se manter o valor vendido no pedido.
            if (produto is ProdutoTrocado && produto.ValorTabelaPedido > 0)
                total = produto.ValorTabelaPedido;

            // Alteração necessária para calcular corretamente o tubo inserido no projeto, ao recalcular orçamento
            if (altura == 0)
                altura = produto.Altura;

            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)produto.IdProduto);

            // O campo AlturaCalc no material_item_projeto considera o valor das folgas
            if (produto is MaterialItemProjeto && Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(idGrupoProd))
                altura = produto.Altura;

            // Considera o campo Altura para alumínios ML Direto
            else if (Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio(idGrupoProd) &&
                Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produto.IdProduto) == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                altura = produto.Altura;

            var alturaBenef = NormalizarAlturaLarguraBeneficiamento(produto.AlturaBenef, produto);
            var larguraBenef = NormalizarAlturaLarguraBeneficiamento(produto.LarguraBenef, produto);

            // Deve passar o parâmetro usarChapaVidro como true, para que caso o produto tenha sido calculado por chapa,
            // não calcule incorretamente o total do mesmo (retornado pela variável total abaixo), estava ocorrendo
            // erro ao chamar esta função a partir de ProdutosPedidoDAO.InsereAtualizaProdProj(), sendo que o produto sendo calculado
            // possuía acréscimo de 25% em caso da área do vidro ser superior à 4m²
            Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(sessao, idCliente.GetValueOrDefault(), (int)produto.IdProduto, produto.Largura, produto.Qtde,
                produto.QtdeAmbiente, total, produto.Espessura, produto.Redondo, 2, produto is ProdutosCompra, true, ref custo, ref altura, ref totM2, ref totM2Calc,
                ref total, alturaBenef, larguraBenef, produto is ProdutosNf, produto.Beneficiamentos.CountAreaMinimaSession(sessao), true, calcularAreaMinima);

            if (PedidoConfig.DadosPedido.AlterarValorUnitarioProduto)
            {
                ValorBruto.Instance.Calcular(produto, container);

                if (Math.Round(total, 2) != Math.Round(produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente, 2))
                {
                    var produtoPossuiValorTabela = ProdutoDAO.Instance.ProdutoPossuiValorTabela(sessao, produto.IdProduto, produto.ValorUnitarioBruto);

                    if (total == 0 || !produtoPossuiValorTabela || (produtoPossuiValorTabela && DescontoAcrescimoClienteDAO.Instance.ProdutoPossuiDesconto(sessao, (int)idCliente.GetValueOrDefault(0), (int)produto.IdProduto)))
                        total = Math.Max(total, produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente);
                }
            }

            total += produto.ValorComissao + produto.ValorAcrescimo + produto.ValorAcrescimoProd -
                (!PedidoConfig.RatearDescontoProdutos ? 0 : produto.ValorDesconto + produto.ValorDescontoProd);

            decimal valorUnit = 0;

            CalculosFluxo.CalcValorUnitItemProd(
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
                produto is ProdutosCompra,
                true,
                altura,
                totM2,
                ref valorUnit,
                produto is ProdutosNf,
                produto.Beneficiamentos.CountAreaMinimaSession(null),
                calcularAreaMinima,
                alturaBenef,
                larguraBenef);

            produto.ValorUnit = valorUnit;
        }

        internal void CalcularBruto(IProdutoDescontoAcrescimo produto, IContainerDescontoAcrescimo container)
        {
            decimal valorUnitario = 0;
            var alturaBenef = NormalizarAlturaLarguraBeneficiamento(produto.AlturaBenef, produto);
            var larguraBenef = NormalizarAlturaLarguraBeneficiamento(produto.LarguraBenef, produto);

            CalculosFluxo.CalcValorUnitItemProd(
                null,
                container.IdCliente.GetValueOrDefault(),
                (int)produto.IdProduto,
                produto.Largura,
                produto.Qtde,
                produto.QtdeAmbiente,
                produto.TotalBruto,
                produto.Espessura,
                produto.Redondo,
                1,
                false,
                !container.IsPedidoProducaoCorte,
                produto.Altura,
                produto.TotM,
                ref valorUnitario,
                produto.Beneficiamentos.CountAreaMinimaSession(null),
                alturaBenef,
                larguraBenef
            );

            produto.ValorUnitarioBruto = valorUnitario;
        }

        private int NormalizarAlturaLarguraBeneficiamento(int? valor, IProdutoDescontoAcrescimo produto)
        {
            if (valor.HasValue && produto.AlturaBenef > 0 && produto.LarguraBenef > 0)
            {
                return valor.Value;
            }

            return 2;
        }
    }
}
