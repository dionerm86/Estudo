// <copyright file="ValidacaoFiltroPedidoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Produtos.Filtro;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Produtos.Estrategias.Filtro
{
    /// <summary>
    /// Classe com as estratégias de validação de filtro para o tipo 'Pedido'.
    /// </summary>
    internal class ValidacaoFiltroPedidoStrategy : IValidacaoFiltro
    {
        private readonly ApiController apiController;
        private readonly bool produtoComposicao;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ValidacaoFiltroPedidoStrategy"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="produtoComposicao">Indica se a validação será feita para um produto de composição.</param>
        public ValidacaoFiltroPedidoStrategy(ApiController apiController, bool produtoComposicao)
        {
            this.apiController = apiController;
            this.produtoComposicao = produtoComposicao;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarAntesBusca(GDASession sessao, string codigoInterno, string dadosAdicionaisValidacao)
        {
            try
            {
                var adicionais = JsonConvert.DeserializeObject<DadosAdicionaisFiltroPedidoDto>(dadosAdicionaisValidacao);

                if (adicionais == null)
                {
                    throw new ArgumentException("dadosAdicionaisValidacao");
                }
            }
            catch
            {
                string formato = JsonConvert.SerializeObject(new DadosAdicionaisFiltroPedidoDto());
                return this.apiController.ErroValidacao("Os dados adicionais não foram informados corretamente. "
                    + $"É esperado um objeto no seguinte formato: {formato}");
            }

            return null;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarDepoisBusca(GDASession sessao, string codigoInterno, string dadosAdicionaisValidacao, Produto produto)
        {
            var dadosAdicionais = JsonConvert.DeserializeObject<DadosAdicionaisFiltroPedidoDto>(dadosAdicionaisValidacao);

            var validacoes = new Func<IHttpActionResult>[]
            {
                () => this.ValidarDadosBasicos(produto),
                () => this.ValidarPedidoMaoDeObra(produto, dadosAdicionais),
                () => this.ValidarPedidoProducao(sessao, produto, dadosAdicionais),
                () => this.ValidarPedidoMaoDeObraEspecial(sessao, produto, dadosAdicionais),
                () => this.ValidarPedidoVenda(sessao, produto, dadosAdicionais),
                () => this.ValidarPedidoRevenda(sessao, produto, dadosAdicionais),
                () => this.ValidarPedidoObra(sessao, produto, dadosAdicionais),
            };

            foreach (var validacao in validacoes)
            {
                var resultado = validacao();

                if (resultado != null)
                {
                    return resultado;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public ProdutoDto ObterProduto(GDASession sessao, string dadosAdicionaisValidacao, Produto produto)
        {
            var dadosAdicionais = JsonConvert.DeserializeObject<DadosAdicionaisFiltroPedidoDto>(dadosAdicionaisValidacao);

            var produtoDto = new ProdutoDto(sessao, produto);

            if (!this.AlterarProdutoParaObraPedido(sessao, produtoDto, dadosAdicionais))
            {
                produtoDto.PodeEditarValorUnitario = PedidoConfig.DadosPedido.AlterarValorUnitarioProduto;
                produtoDto.ValorMinimo = this.ObterValorMinimoProduto(sessao, dadosAdicionais, produto);
                produtoDto.ValorUnitario = this.ObterValorTabelaProduto(sessao, dadosAdicionais, produto);
            }

            produtoDto.DescontoAcrescimoCliente = this.ObterDadosDescontoAcrescimoCliente(sessao, dadosAdicionais, produto);
            produtoDto.TamanhoMaximoObra = this.ObterTamanhoMaximoObra(sessao, dadosAdicionais, produto);

            return produtoDto;
        }

        private IHttpActionResult ValidarDadosBasicos(Produto produto)
        {
            if (produto.Situacao == Situacao.Inativo)
            {
                var observacao = string.IsNullOrWhiteSpace(produto.Obs)
                    ? string.Empty
                    : " Obs.: " + produto.Obs;

                return this.apiController.ErroValidacao("Produto inativo." + observacao);
            }

            if (produto.Compra)
            {
                return this.apiController.ErroValidacao("Produto utilizado apenas na compra.");
            }

            return null;
        }

        private IHttpActionResult ValidarPedidoMaoDeObra(Produto produto, DadosAdicionaisFiltroPedidoDto dadosAdicionais)
        {
            if (dadosAdicionais.TipoPedido == Data.Model.Pedido.TipoPedidoEnum.MaoDeObra)
            {
                if (dadosAdicionais.ProdutoAmbiente && produto.IdGrupoProd != (uint)NomeGrupoProd.Vidro)
                {
                    return this.apiController.ErroValidacao("Apenas produtos do grupo 'Vidro' podem ser usados como peça de vidro.");
                }
                else if (!dadosAdicionais.ProdutoAmbiente && produto.IdGrupoProd != (uint)NomeGrupoProd.MaoDeObra)
                {
                    return this.apiController.ErroValidacao("Apenas produtos do grupo 'Mão de Obra Beneficiamento' podem ser incluídos nesse pedido.");
                }
            }

            return null;
        }

        private IHttpActionResult ValidarPedidoProducao(GDASession sessao, Produto produto, DadosAdicionaisFiltroPedidoDto dadosAdicionais)
        {
            if (dadosAdicionais.TipoPedido == Data.Model.Pedido.TipoPedidoEnum.Producao
                && !ProdutoPedidoProducaoDAO.Instance.PedidoProducaoGeradoPorPedidoRevenda(sessao, (uint)dadosAdicionais.IdPedido))
            {
                if (produto.IdGrupoProd != (uint)NomeGrupoProd.Vidro
                    || !SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, produto.IdGrupoProd, produto.IdSubgrupoProd))
                {
                    return this.apiController.ErroValidacao("Apenas produtos do grupo 'Vidro' marcados como 'Produtos para Estoque' podem ser incluídos nesse pedido.");
                }

                if (!ProdutoBaixaEstoqueDAO.Instance.TemProdutoBaixa(sessao, (uint)produto.IdProd))
                {
                    return this.apiController.ErroValidacao("Esse produto ainda não possui um produto associado. Para usá-lo aqui é preciso que você altere o cadastro desse produto e associe o produto final.");
                }
            }

            return null;
        }

        private IHttpActionResult ValidarPedidoMaoDeObraEspecial(GDASession sessao, Produto produto, DadosAdicionaisFiltroPedidoDto dadosAdicionais)
        {
            if (dadosAdicionais.TipoPedido == Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial)
            {
                if (produto.IdGrupoProd != (uint)NomeGrupoProd.Vidro
                    || SubgrupoProdDAO.Instance.IsSubgrupoProducao(produto.IdGrupoProd, produto.IdSubgrupoProd))
                {
                    return this.apiController.ErroValidacao("Apenas produtos do grupo 'Vidro', e que não são marcados como 'Produtos para Estoque', podem ser utilizados nesse pedido.");
                }

                if (PedidoConfig.DadosPedido.BloquearItensCorEspessura
                    && !LojaDAO.Instance.GetIgnorarBloquearItensCorEspessura(sessao, (uint)dadosAdicionais.IdLoja)
                    && produto.IdCorVidro > 0
                    && produto.Espessura > 0
                    && !ProdutosPedidoDAO.Instance.ValidarSeTodosOsVidrosSaoDaMesmaCorEEspessura(sessao, dadosAdicionais.IdPedido, produto.IdCorVidro, produto.Espessura))
                {
                    return this.apiController.ErroValidacao("Todos os produtos devem ter a mesma cor e espessura.");
                }
            }

            return null;
        }

        private IHttpActionResult ValidarPedidoVenda(GDASession sessao, Produto produto, DadosAdicionaisFiltroPedidoDto dadosAdicionais)
        {
            if (dadosAdicionais.TipoPedido == Data.Model.Pedido.TipoPedidoEnum.Venda)
            {
                if (produto.IdGrupoProd == (uint)NomeGrupoProd.MaoDeObra)
                {
                    return this.apiController.ErroValidacao("Produtos do grupo 'Mão de Obra Beneficiamento' estão bloqueados para pedidos comuns.");
                }

                bool subgrupoPermiteItemRevendaNaVenda = produto.IdSubgrupoProd.HasValue
                    && SubgrupoProdDAO.Instance.ObtemPermitirItemRevendaNaVenda(produto.IdSubgrupoProd.Value);

                if (!this.produtoComposicao
                    && PedidoConfig.DadosPedido.BloquearItensTipoPedido
                    && !subgrupoPermiteItemRevendaNaVenda)
                {
                    if ((produto.IdGrupoProd != (uint)NomeGrupoProd.Vidro
                        || (produto.IdGrupoProd == (uint)NomeGrupoProd.Vidro
                            && SubgrupoProdDAO.Instance.IsSubgrupoProducao(produto.IdGrupoProd, produto.IdSubgrupoProd)))
                        && produto.IdGrupoProd != (uint)NomeGrupoProd.MaoDeObra)
                    {
                        return this.apiController.ErroValidacao("Produtos de revenda não podem ser incluídos em um pedido de venda.");
                    }

                    if (PedidoConfig.DadosPedido.BloquearItensCorEspessura
                        && !LojaDAO.Instance.GetIgnorarBloquearItensCorEspessura(sessao, (uint)dadosAdicionais.IdLoja)
                        && produto.IdCorVidro > 0
                        && produto.Espessura > 0
                        && !ProdutosPedidoDAO.Instance.ValidarSeTodosOsVidrosSaoDaMesmaCorEEspessura(sessao, dadosAdicionais.IdPedido, produto.IdCorVidro, produto.Espessura))
                    {
                        return this.apiController.ErroValidacao("Todos os produtos devem ter a mesma cor e espessura.");
                    }

                    if (produto.IdGrupoProd == (uint)NomeGrupoProd.Vidro
                        && !PedidoConfig.TelaCadastro.PermitirInserirVidroComumComComposicao)
                    {
                        foreach (var prodped in ProdutosPedidoDAO.Instance.GetByPedido(sessao, (uint)dadosAdicionais.IdPedido))
                        {
                            var itemRevendaNaVenda = prodped.IdSubgrupoProd > 0
                                && SubgrupoProdDAO.Instance.ObtemPermitirItemRevendaNaVenda((int)prodped.IdSubgrupoProd);

                            var tipo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(null, (int)prodped.IdProd);
                            var tipoProd = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(null, (int)produto.IdProd);

                            if (((tipo != TipoSubgrupoProd.VidroDuplo && tipo != TipoSubgrupoProd.VidroLaminado && (tipoProd == TipoSubgrupoProd.VidroDuplo || tipoProd == TipoSubgrupoProd.VidroLaminado)) ||
                                ((tipo == TipoSubgrupoProd.VidroDuplo || tipo == TipoSubgrupoProd.VidroLaminado) && tipoProd != TipoSubgrupoProd.VidroDuplo && tipoProd != TipoSubgrupoProd.VidroLaminado)) &&
                                !itemRevendaNaVenda)
                            {
                                return this.apiController.ErroValidacao("Não é possivel inserir produtos do tipo de subgrupo vidro duplo ou laminado junto com produtos comuns e temperados.");
                            }
                        }
                    }
                }
            }

            return null;
        }

        private IHttpActionResult ValidarPedidoRevenda(GDASession sessao, Produto produto, DadosAdicionaisFiltroPedidoDto dadosAdicionais)
        {
            if (dadosAdicionais.TipoPedido == Data.Model.Pedido.TipoPedidoEnum.Revenda)
            {
                if (produto.IdGrupoProd == (uint)NomeGrupoProd.MaoDeObra)
                {
                    return this.apiController.ErroValidacao("Produtos do grupo 'Mão de Obra Beneficiamento' estão bloqueados para pedidos comuns.");
                }

                bool subgrupoPermiteItemRevendaNaVenda = produto.IdSubgrupoProd.HasValue
                    && SubgrupoProdDAO.Instance.ObtemPermitirItemRevendaNaVenda(produto.IdSubgrupoProd.Value);

                if (!this.produtoComposicao
                    && PedidoConfig.DadosPedido.BloquearItensTipoPedido
                    && !subgrupoPermiteItemRevendaNaVenda
                    && ((produto.IdGrupoProd == (uint)NomeGrupoProd.Vidro
                        && !SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, produto.IdGrupoProd, produto.IdSubgrupoProd))
                        || produto.IdGrupoProd == (uint)NomeGrupoProd.MaoDeObra))
                {
                    return this.apiController.ErroValidacao("Produtos de venda não podem ser incluídos em um pedido de revenda.");
                }
            }

            return null;
        }

        private IHttpActionResult ValidarPedidoObra(GDASession sessao, Produto produto, DadosAdicionaisFiltroPedidoDto dadosAdicionais)
        {
            if (dadosAdicionais.IdObra > 0)
            {
                var tipoSubgrupo = produto.IdSubgrupoProd.HasValue
                    ? SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, produto.IdProd)
                    : (TipoSubgrupoProd?)null;

                var tiposSubgrupoValidar = new[]
                {
                    TipoSubgrupoProd.VidroLaminado,
                    TipoSubgrupoProd.VidroDuplo,
                };

                if (!ProdutoBaixaEstoqueDAO.Instance.TemProdutoBaixa((uint)produto.IdProd)
                    && tipoSubgrupo.HasValue
                    && tiposSubgrupoValidar.Contains(tipoSubgrupo.Value))
                {
                    return this.apiController.ErroValidacao("Não é possível inserir produtos do tipo de subgrupo vidro duplo ou laminado sem produto de composição em seu cadastro.");
                }

                if (PedidoConfig.DadosPedido.UsarControleNovoObra)
                {
                    var produtoObra = ProdutoObraDAO.Instance.GetByCodInterno(
                        (uint)dadosAdicionais.IdObra.Value,
                        produto.CodInterno);

                    if (produtoObra == null)
                    {
                        return this.apiController.ErroValidacao("Esse produto não está cadastrado no pagamento antecipado.");
                    }
                }
            }

            return null;
        }

        private decimal ObterValorMinimoProduto(GDASession sessao, DadosAdicionaisFiltroPedidoDto dadosAdicionais, Produto produto)
        {
            return ProdutoDAO.Instance.GetValorMinimo(
                sessao,
                produto.IdProd,
                (int)dadosAdicionais.TipoEntrega,
                (uint?)dadosAdicionais.Cliente?.Id,
                dadosAdicionais.Cliente?.Revenda ?? false,
                dadosAdicionais.TipoVenda == Data.Model.Pedido.TipoVendaPedido.Reposição,
                (float)dadosAdicionais.PercentualDescontoPorQuantidade,
                dadosAdicionais.IdPedido,
                null,
                null);
        }

        private decimal ObterValorTabelaProduto(GDASession sessao, DadosAdicionaisFiltroPedidoDto dadosAdicionais, Produto produto)
        {
            return ProdutoDAO.Instance.GetValorTabela(
                sessao,
                produto.IdProd,
                (int)dadosAdicionais.TipoEntrega,
                (uint?)dadosAdicionais.Cliente?.Id,
                dadosAdicionais.Cliente?.Revenda ?? false,
                dadosAdicionais.TipoVenda == Data.Model.Pedido.TipoVendaPedido.Reposição,
                (float)dadosAdicionais.PercentualDescontoPorQuantidade,
                dadosAdicionais.IdPedido,
                null,
                null);
        }

        private bool AlterarProdutoParaObraPedido(GDASession sessao, ProdutoDto produto, DadosAdicionaisFiltroPedidoDto dadosAdicionais)
        {
            if (dadosAdicionais.IdObra > 0)
            {
                var produtoObra = ProdutoObraDAO.Instance.IsProdutoObra(
                    sessao,
                    (uint)dadosAdicionais.IdObra.Value,
                    (uint)produto.Id);

                if (produtoObra.ProdutoValido)
                {
                    produto.PodeEditarValorUnitario = produtoObra.AlterarValorUnitario;
                    produto.ValorUnitario = produtoObra.ValorUnitProduto;
                    produto.ValorMinimo = produtoObra.ValorUnitProduto;

                    return true;
                }
            }

            return false;
        }

        private DescontoAcrescimoClienteDto ObterDadosDescontoAcrescimoCliente(GDASession sessao, DadosAdicionaisFiltroPedidoDto dadosAdicionais, Produto produto)
        {
            var descontoAcrescimoCliente = DescontoAcrescimoClienteDAO.Instance.GetDescontoAcrescimo(
                sessao,
                (uint)dadosAdicionais.Cliente.Id,
                produto.IdGrupoProd,
                produto.IdSubgrupoProd,
                produto.IdProd,
                null,
                null);

            return new DescontoAcrescimoClienteDto
            {
                Percentual = (double)(descontoAcrescimoCliente?.PercMultiplicar ?? 1),
                UsarNosBeneficiamentos = descontoAcrescimoCliente?.AplicarBeneficiamentos ?? false,
            };
        }

        private double? ObterTamanhoMaximoObra(GDASession sessao, DadosAdicionaisFiltroPedidoDto dadosAdicionais, Produto produto)
        {
            if (dadosAdicionais.IdObra > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
            {
                var tamanhoProduto = dadosAdicionais.AreaEmM2DesconsiderarObra ?? 0;

                var tamanhoProdutos = ProdutosPedidoDAO.Instance.TotalMedidasObra(
                    sessao,
                    (uint)dadosAdicionais.IdObra.Value,
                    produto.CodInterno,
                    null)
                    - tamanhoProduto;

                var produtoObra = ProdutoObraDAO.Instance.GetByCodInterno(
                    sessao,
                    (uint)dadosAdicionais.IdObra.Value,
                    produto.CodInterno);

                var tamanhoMaximoRestante = produtoObra.TamanhoMaximo
                    - tamanhoProdutos
                    + tamanhoProduto;

                if (produtoObra.TamanhoMaximo > 0 && tamanhoMaximoRestante == 0)
                {
                    tamanhoMaximoRestante = 0.01f;
                }

                return tamanhoMaximoRestante;
            }

            return null;
        }
    }
}
