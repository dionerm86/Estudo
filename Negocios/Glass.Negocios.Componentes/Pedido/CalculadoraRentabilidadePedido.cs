﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data;
using Glass.Rentabilidade;
using Glass.Rentabilidade.Negocios;
using Glass.Rentabilidade.Negocios.Componentes;

namespace Glass.Pedido.Negocios.Componentes
{
    /// <summary>
    /// Implementação da calculadora de rentabilidade do pedido.
    /// </summary>
    public sealed class CalculadoraRentabilidadePedido : 
        Rentabilidade.Negocios.Componentes.CalculadoraRentabilidade, 
        Data.ICalculadoraRentabilidade<Data.Model.Pedido>,
        Data.ICalculadoraRentabilidade<Data.Model.AmbientePedido>,
        Data.ICalculadoraRentabilidade<Data.Model.ProdutosPedido>,
        IProvedorItemRentabilidade<Data.Model.Pedido>,
        IProvedorItemRentabilidade<Data.Model.ProdutosPedido>,
        IProvedorItemRentabilidade<Data.Model.AmbientePedido>
    {
        #region Propriedades

        /// <summary>
        /// Calculadora da comissão para a rentabilidade.
        /// </summary>
        private ICalculadoraComissaoRentabilidade CalculadoraComissaoRentabilidade { get; }

        /// <summary>
        /// Obtém ou define o identificador do funcionário que está
        /// vinculado com o cálculo.
        /// </summary>
        private int IdFunc { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja que está
        /// vinculada com o cálculo.
        /// </summary>
        private int IdLoja { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="provedorDescritoresRegistro"></param>
        /// <param name="provedorIndicadoresFinanceiro"></param>
        /// <param name="provedorCalculadoraRentabilidade"></param>
        /// <param name="calculadoraComissaoRentabilidade"></param>
        public CalculadoraRentabilidadePedido(
            IProvedorDescritorRegistroRentabilidade provedorDescritoresRegistro,
            IProvedorIndicadorFinanceiro provedorIndicadoresFinanceiro,
            IProvedorCalculadoraRentabilidade provedorCalculadoraRentabilidade,
            ICalculadoraComissaoRentabilidade calculadoraComissaoRentabilidade)
            : base(provedorDescritoresRegistro, provedorIndicadoresFinanceiro, provedorCalculadoraRentabilidade)
        {
            CalculadoraComissaoRentabilidade = calculadoraComissaoRentabilidade;
        }

        #endregion

        #region Métodos Protegidos

        /// <summary>
        /// Cria o resultado do calculo da rentabilidade para o item informado.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="resultadoCalculo"></param>
        /// <param name="subResultados">Sub resultados processados.</param>
        /// <returns></returns>
        protected override Data.ICalculoRentabilidadeResultado CriaResultado(
            IItemRentabilidade item, ResultadoRentabilidade resultadoCalculo,
            IEnumerable<Data.ICalculoRentabilidadeResultado> subResultados)
        {
            Data.ICalculoRentabilidadeResultado resultado;

            var itemPedido = item as IItemRentabilidade<Data.Model.Pedido>;
            var itemProdutoPedido = item as IItemRentabilidade<Data.Model.ProdutosPedido>;
            var itemAmbientePedido = item as IItemRentabilidade<Data.Model.AmbientePedido>;

            if (itemPedido != null)
            {
                resultado = MontarResultado
                    ((IItemRentabilidadeComReferencias<Data.Model.PedidoRentabilidade>)item,
                    (x, y) => (int)x.Tipo == y.Tipo && x.IdRegistro == y.IdRegistro,
                    (x, y) => y.Valor = x.Valor);

                var pedido = itemPedido.Proprietario;

                // Registra o evento para salvar o dados do pedido quando o resultado for salvo
                resultado.Salvando += (sender, e) =>
                {
                    var rentabilidade = pedido.RentabilidadeFinanceira = e.RentabilidadeFinanceira;
                    var percentual = pedido.PercentualRentabilidade = e.PercentualRentabilidade * 100m;

                    var percentualComissao = item.PercentualComissao * 100m;
                    pedido.PercentualComissao = (float)percentualComissao;

                    Data.DAL.PedidoDAO.Instance.AtualizarRentabilidade(e.Sessao, 
                        pedido.IdPedido, percentual, rentabilidade, percentualComissao);
                };
            }
            else if (itemProdutoPedido != null)
            {
                resultado = MontarResultado
                    ((IItemRentabilidadeComReferencias<Data.Model.ProdutoPedidoRentabilidade>)item,
                    (x, y) => (int)x.Tipo == y.Tipo && x.IdRegistro == y.IdRegistro,
                    (x, y) => y.Valor = x.Valor);

                var produtoPedido = itemProdutoPedido.Proprietario;

                // Registra o evento para salvar o dados do pedido quando o resultado for salvo
                resultado.Salvando += (sender, e) =>
                {
                    var rentabilidade = produtoPedido.RentabilidadeFinanceira = e.RentabilidadeFinanceira;
                    var percentual = produtoPedido.PercentualRentabilidade = e.PercentualRentabilidade * 100m;

                    var percentualComissao = item.PercentualComissao * 100m;
                    produtoPedido.PercComissao = percentualComissao;

                    Data.DAL.ProdutosPedidoDAO.Instance.AtualizarRentabilidade(e.Sessao, 
                        produtoPedido.IdProdPed, percentual, rentabilidade, percentualComissao);
                };
            }
            else if (itemAmbientePedido != null)
            {
                resultado = MontarResultado
                    ((IItemRentabilidadeComReferencias<Data.Model.AmbientePedidoRentabilidade>)item,
                    (x, y) => (int)x.Tipo == y.Tipo && x.IdRegistro == y.IdRegistro,
                    (x, y) => y.Valor = x.Valor);

                var ambiente = itemAmbientePedido.Proprietario;

                // Registra o evento para salvar o dados do pedido quando o resultado for salvo
                resultado.Salvando += (sender, e) =>
                {
                    var rentabilidade = ambiente.RentabilidadeFinanceira = e.RentabilidadeFinanceira;
                    var percentual = ambiente.PercentualRentabilidade = e.PercentualRentabilidade * 100m;
                    Data.DAL.AmbientePedidoDAO.Instance.AtualizarRentabilidade(e.Sessao, ambiente.IdAmbientePedido, percentual, rentabilidade);
                };
            }
            else
                throw new NotSupportedException("Tipo do item de rentabilidade não suportado.");

            var resultadosContainer = resultado as ICalculoRentabilidadeResultadoContainer;
            if (resultadosContainer != null)
                foreach (var i in subResultados)
                    resultadosContainer.Adicionar(i);

            return resultado;
        }

        /// <summary>
        /// Executa o cálculo para o item informado.
        /// </summary>
        /// <param name="item">Item sobre o qual será calculada a rentabilidade.</param>
        protected override ICalculoRentabilidadeResultado Calcular(IItemRentabilidade item)
        {
            var possuiaRegistroRentabilidade = item.RegistrosRentabilidade.Any();

            var resultado = base.Calcular(item);
            var itemProdutoPedido = item as IItemRentabilidade<Data.Model.ProdutosPedido>;

            if (itemProdutoPedido != null && resultado.Executado)
            {
                var tentativas = 20;

                var calcularComissaoBase = !possuiaRegistroRentabilidade;

                while (tentativas > 0)
                {
                    var resultadoComissao = CalculadoraComissaoRentabilidade.Calcular(item, IdLoja, IdFunc, calcularComissaoBase);

                    if (!resultadoComissao.Valido || calcularComissaoBase) 
                    {
                        ((ItemRentabilidade)item).PercentualComissao = resultadoComissao.PercentualComissao;
                        itemProdutoPedido.Proprietario.PercComissao = resultadoComissao.PercentualComissao * 100m;
                    }

                    if (resultadoComissao.Valido)
                        break;

                    // Recalcula a rentabilidade
                    resultado = base.Calcular(item);

                    if (!resultado.Executado)
                        break;

                    tentativas--;
                }
            }

            return resultado;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Calcula o prazo médio do pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        private int CalcularPrazoMedioPedido(GDA.GDASession sessao, uint idPedido)
        {
            var tipoVenda = Data.DAL.PedidoDAO.Instance.ObtemTipoVenda(sessao, idPedido);

            if (tipoVenda == (int)Data.Model.Pedido.TipoVendaPedido.APrazo)
            {
                var idParcela = Data.DAL.PedidoDAO.Instance.ObtemIdParcela(sessao, idPedido);
                if (idParcela.HasValue)
                {
                    // Recupera a parcela associada com o pedido
                    var parcela = Data.DAL.ParcelasDAO.Instance.GetElementByPrimaryKey(sessao, idParcela.Value);

                    return CalcularPrazoMedio(parcela);
                }
            }

            return 0;
        }

        /// <summary>
        /// Recupera um item de rentabilidade para o produto pedido informado.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="produtoPedido"></param>
        /// <param name="prazoMedio"></param>
        /// <param name="produtos"></param>
        /// <param name="produtosPedidoFilhos">Relação dos produtos do pedidos filhos do produto informado.</param>
        /// <returns></returns>
        private IItemRentabilidade<Data.Model.ProdutosPedido> ObterItemProdutoPedido(
            GDA.GDASession sessao, Data.Model.ProdutosPedido produtoPedido,
            int prazoMedio, IEnumerable<Data.Model.Produto> produtos,
            IEnumerable<Data.Model.ProdutosPedido> produtosPedidoFilhos)
        {
            var registros = new Lazy<IList<Data.Model.ProdutoPedidoRentabilidade>>(
                () => Data.DAL.ProdutoPedidoRentabilidadeDAO.Instance.ObterPorProdutoPedido(sessao, produtoPedido.IdProdPed));

            var criarRegistro = new CriadorRegistroRentabilidade((tipo, nome, valor) =>
            {
                var idRegistro = ProvedorDescritoresRegistro.ObterRegistro(tipo, nome);
                var registro = registros.Value.FirstOrDefault(f => f.Tipo == (int)tipo && f.IdRegistro == idRegistro);

                if (registro == null)
                {
                    registro = new Data.Model.ProdutoPedidoRentabilidade
                    {
                        IdProdPed = (int)produtoPedido.IdProdPed,
                        IdRegistro = idRegistro,
                        Tipo = (int)tipo,
                        Valor = valor
                    };
                    registros.Value.Add(registro);
                }
                else
                    registro.Valor = valor;

                return ConverterParaRegistroRentabilidade(registro);
            });

            // Carrega o produto associado
            var produto = produtos.FirstOrDefault(f => f.IdProd == produtoPedido.IdProd);

            // Calculao custo do produto
            var custoProd = Global.CalculosFluxo.CalcTotaisItemProdFast(sessao, produto.TipoCalculo,
                produtoPedido.Altura, produtoPedido.Largura, produtoPedido.Qtde,
                produtoPedido.TotM, produto.CustoCompra, produtoPedido.AlturaBenef.GetValueOrDefault(2),
                produtoPedido.LarguraBenef.GetValueOrDefault(2));

            if (produtosPedidoFilhos.Any())
            {
                // Recupera os itens dos produtos filhos
                var itens = produtosPedidoFilhos.Select(produtoPedido1 =>
                    ObterItemProdutoPedido(sessao, produtoPedido1, prazoMedio, produtos, new Data.Model.ProdutosPedido[0]))
                    .ToList();

                return new ItemRentabilidadeContainer<Data.Model.ProdutosPedido, Data.Model.ProdutoPedidoRentabilidade>(
                    ProvedorIndicadoresFinanceiro, criarRegistro, produtoPedido, itens, f => true, registros,
                    ConverterParaRegistroRentabilidade)
                {
                    Descricao = $"Produto ({produto?.CodInterno}) {produto?.Descricao}",
                    PrecoVendaSemIPI = produtoPedido.Total + produtoPedido.ValorBenef,
                    PrazoMedio = prazoMedio,
                    PercentualComissao = produtoPedido.PercComissao / 100m,
                    PercentualRentabilidade = produtoPedido.PercentualRentabilidade / 100m,
                    RentabilidadeFinanceira = produtoPedido.RentabilidadeFinanceira
                };
            }
            else
                return new ItemRentabilidade<Data.Model.ProdutosPedido, Data.Model.ProdutoPedidoRentabilidade>(
                    ProvedorIndicadoresFinanceiro, criarRegistro, produtoPedido, registros, ConverterParaRegistroRentabilidade)
                {
                    Descricao = $"Produto ({produto?.CodInterno}) {produto?.Descricao}",
                    PrecoVendaSemIPI = produtoPedido.Total + produtoPedido.ValorBenef, // Não atualizar a configuração do sistema o total do produto não possui o valor do IPI
                    PrecoCusto = custoProd,
                    PrazoMedio = prazoMedio,
                    PercentualICMSVenda = (decimal)produtoPedido.AliqIcms / 100m,
                    FatorICMSSubstituicao = 0m,
                    PercentualIPICompra = 0m, //(decimal)(produto?.AliqIPI ?? 0) / 100m,
                    PercentualIPIVenda = (decimal)produtoPedido.AliqIpi / 100m,
                    PercentualComissao = produtoPedido.PercComissao / 100m,
                    CustosExtras = 0M,
                    PercentualRentabilidade = produtoPedido.PercentualRentabilidade / 100m,
                    RentabilidadeFinanceira = produtoPedido.RentabilidadeFinanceira
                };
        }

        /// <summary>
        /// Recupera os itens de rentabilidade dos produtos do pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="pedido">Pedido.</param>
        /// <param name="prazoMedio">Prazo médio de faturamenteo do pedido.</param>
        /// <returns></returns>
        private IEnumerable<IItemRentabilidade<Data.Model.ProdutosPedido>> 
            ObterItensProdutosPedido(GDA.GDASession sessao, Data.Model.Pedido pedido, int prazoMedio)
        {
            var produtosPedido = Data.DAL.ProdutosPedidoDAO.Instance.ObterProdutosParaRentabilidade(sessao, pedido.IdPedido);
            var produtos = Data.DAL.ProdutoDAO.Instance.ObterProdutos(sessao, produtosPedido.Select(f => f.IdProd).Distinct()).ToList();

            // Ignora produtos do pedido com pai
            foreach (var i in produtosPedido.Where(f => !f.IdProdPedParent.HasValue))
                yield return ObterItemProdutoPedido(
                    sessao, i, prazoMedio, produtos,
                    produtosPedido.Where(f => f.IdProdPedParent == i.IdProdPed));
        }

        /// <summary>
        /// Recupera os itens de ambientes do pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="pedido"></param>
        /// <param name="prazoMedio"></param>
        /// <param name="itensProdutoPedido"></param>
        /// <returns></returns>
        private IEnumerable<IItemRentabilidade>
            ObterItensAmbientesPedido(
                GDA.GDASession sessao, Data.Model.Pedido pedido, int prazoMedio, 
                IEnumerable<IItemRentabilidade<Data.Model.ProdutosPedido>> itensProdutoPedido)
        {
            var ambientesPedido = Data.DAL.AmbientePedidoDAO.Instance.ObterAmbientesParaRentabilidade(sessao, pedido.IdPedido);

            foreach (var i in ambientesPedido)
                yield return ObterItemAmbientePedido(sessao, i, prazoMedio, 
                    // Filtra os produtos do ambiente
                    itensProdutoPedido.Where(f => f.Proprietario.IdAmbientePedido == i.IdAmbientePedido));
        }

        /// <summary>
        /// Recupera o item da rentabilidade para o pedido informado.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="pedido"></param>
        /// <returns></returns>
        private IItemRentabilidade ObterItemPedido(GDA.GDASession sessao, Data.Model.Pedido pedido)
        {
            var registros = new Lazy<IList<Data.Model.PedidoRentabilidade>>(
                () => Data.DAL.PedidoRentabilidadeDAO.Instance.ObterPorPedido(sessao, pedido.IdPedido));

            var criarRegistro = new CriadorRegistroRentabilidade((tipo, nome, valor) =>
            {
                var idRegistro = ProvedorDescritoresRegistro.ObterRegistro(tipo, nome);
                var registro = registros.Value.FirstOrDefault(f => f.Tipo == (int)tipo && f.IdRegistro == idRegistro);

                if (registro == null)
                {
                    // Cria o registro da rentabilidade do pedido
                    registro = new Data.Model.PedidoRentabilidade
                    {
                        IdPedido = (int)pedido.IdPedido,
                        IdRegistro = idRegistro,
                        Tipo = (int)tipo,
                        Valor = valor
                    };
                    registros.Value.Add(registro);
                }
                else
                    registro.Valor = valor;

                return ConverterParaRegistroRentabilidade(registro);
            });

            int prazoMedio;

            // Verifica se é venda à prazo
            if (pedido.TipoVenda.GetValueOrDefault() == (int)Data.Model.Pedido.TipoVendaPedido.APrazo && pedido.IdParcela.HasValue)
                prazoMedio = CalcularPrazoMedio(Data.DAL.ParcelasDAO.Instance.GetElementByPrimaryKey(sessao, pedido.IdParcela.Value));
            else
                prazoMedio = 0;

            // Recupera os itens associados com todos os produtos do pedido
            var produtos = new LazyItemRentabilidadeEnumerable(ObterItensProdutosPedido(sessao, pedido, prazoMedio));

            // Recupera os ambientes do pedido
            var ambientes = new LazyItemRentabilidadeEnumerable(ObterItensAmbientesPedido(sessao, pedido, prazoMedio, produtos.OfType<IItemRentabilidade<Data.Model.ProdutosPedido>>()));

            var itens = produtos.Concat(ambientes);

            // Cria o filtro que recupera os itens para serem usados nos calculos
            var filtroItensParaCalculo = new Func<IItemRentabilidade, bool>((item) =>
            {
                if (item is IItemRentabilidade<Data.Model.AmbientePedido>)
                    // Calcula a rentabilidade somente de ambientes com produtos
                    return ((IItemRentabilidadeContainer)item).Itens.Any();

                var produtoPedido = (item as IItemRentabilidade<Data.Model.ProdutosPedido>)?.Proprietario;
                return produtoPedido != null && 
                       !produtoPedido.InvisivelPedido && !produtoPedido.IdAmbientePedido.HasValue;
            });

            decimal percentualComissao = 0;

            if (Glass.Configuracoes.PedidoConfig.Comissao.UsarComissaoPorProduto)
            {
                decimal percComissao = 0;
                var total = (pedido.Total - pedido.ValorIpi - pedido.ValorIcms) - pedido.ValorEntrega;

                if (total > 0)
                    foreach (var item in itens.Where(filtroItensParaCalculo))
                        percComissao += ((item.PrecoVendaSemIPI * 100) / total) * (item.PercentualComissao);

                percentualComissao = percComissao / 100m;
            }
            else
                percentualComissao = (decimal)pedido.PercentualComissao / 100m;

            return new ItemRentabilidadeContainer<Data.Model.Pedido, Data.Model.PedidoRentabilidade>(
                ProvedorIndicadoresFinanceiro, criarRegistro, pedido, itens, filtroItensParaCalculo, registros, 
                ConverterParaRegistroRentabilidade)
            {
                Descricao = $"Pedido {pedido.IdPedido}",
                PrecoVendaSemIPI = (pedido.Total - pedido.ValorIpi - pedido.ValorIcms) - pedido.ValorEntrega,
                PrazoMedio = prazoMedio,
                PercentualComissao = percentualComissao,
                CustosExtras = pedido.ValorEntrega,
                PercentualRentabilidade = pedido.PercentualRentabilidade / 100m,
                RentabilidadeFinanceira = pedido.RentabilidadeFinanceira
            };
        }

        /// <summary>
        /// Recupera o item de rentabilidade para o ambiente do pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="ambiente"></param>
        /// <returns></returns>
        private IItemRentabilidade ObterItemAmbientePedido(
            GDA.GDASession sessao, Data.Model.AmbientePedido ambiente, int prazoMedio, 
            IEnumerable<IItemRentabilidade<Data.Model.ProdutosPedido>> produtos)
        {
            var registros = new Lazy<IList<Data.Model.AmbientePedidoRentabilidade>>(
                () => Data.DAL.AmbientePedidoRentabilidadeDAO.Instance.ObterPorAmbiente(sessao, ambiente.IdAmbientePedido));

            var criarRegistro = new CriadorRegistroRentabilidade((tipo, nome, valor) =>
            {
                var idRegistro = ProvedorDescritoresRegistro.ObterRegistro(tipo, nome);
                var registro = registros.Value.FirstOrDefault(f => f.Tipo == (int)tipo && f.IdRegistro == idRegistro);

                if (registro == null)
                {
                    // Cria o registro da rentabilidade do ambiente do pedido
                    registro = new Data.Model.AmbientePedidoRentabilidade
                    {
                        IdAmbientePedido = (int)ambiente.IdAmbientePedido,
                        IdRegistro = idRegistro,
                        Tipo = (int)tipo,
                        Valor = valor
                    };
                    registros.Value.Add(registro);
                }
                else
                    registro.Valor = valor;

                return ConverterParaRegistroRentabilidade(registro);
            });

            var produtosAmbiente = produtos.Select(f => f.Proprietario);

            // Calcula os valores dos produtos do ambiente
            var valorProdutos = produtosAmbiente.Sum(f => f.Total + f.ValorBenef);
            var valorDescontoAtual = produtosAmbiente.Sum(f => f.ValorDescontoProd);
            
            // Calcula o total do ambiente
            var total = valorProdutos - (!Glass.Configuracoes.PedidoConfig.RatearDescontoProdutos ? valorDescontoAtual : 0);

            var percentualComissao = 0m;

            if (Glass.Configuracoes.PedidoConfig.Comissao.UsarComissaoPorProduto)
            {
                decimal percComissao = 0;

                if (total > 0)
                    foreach (var prod in produtosAmbiente)
                        percComissao += ((prod.Total * 100) / total) * (prod.PercComissao / 100);

                percentualComissao = percComissao / 100m;
            }

            return new ItemRentabilidadeContainer<Data.Model.AmbientePedido, Data.Model.AmbientePedidoRentabilidade>(
                ProvedorIndicadoresFinanceiro, criarRegistro, ambiente, produtos, f => true, registros, 
                ConverterParaRegistroRentabilidade)
            {
                Descricao = $"Ambiente {ambiente.Ambiente}",
                PrecoVendaSemIPI = total, // Não atualizar a configuração do sistema o total do produto não possui o valor do IPI
                PrazoMedio = prazoMedio,
                PercentualComissao = percentualComissao,
                PercentualRentabilidade = ambiente.PercentualRentabilidade / 100m,
                RentabilidadeFinanceira = ambiente.RentabilidadeFinanceira
            };
        }

        /// <summary>
        /// Recupera o item de rentabilidade para o produto do pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="produtoPedido"></param>
        /// <returns></returns>
        private IItemRentabilidade ObterItemProdutoPedido(GDA.GDASession sessao, Data.Model.ProdutosPedido produtoPedido)
        {
            var prazoMedio = CalcularPrazoMedioPedido(sessao, produtoPedido.IdPedido);
            var filhos = Data.DAL.ProdutosPedidoDAO.Instance.ObterFilhosComposicao(sessao, (int)produtoPedido.IdProdPed);

            var produtos = Data.DAL.ProdutoDAO.Instance.ObterProdutos(sessao,
                filhos.Select(f => f.IdProd).Concat(new uint[] { produtoPedido.IdProd }).Distinct());
           
            return ObterItemProdutoPedido(sessao, produtoPedido, prazoMedio, produtos, filhos);
        }

        /// <summary>
        /// Recupera o item de rentabilidade para o ambiente do pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="ambiente"></param>
        /// <returns></returns>
        private IItemRentabilidade ObterItemAmbientePedido(GDA.GDASession sessao, Data.Model.AmbientePedido ambiente)
        {
            var prazoMedio = CalcularPrazoMedioPedido(sessao, ambiente.IdPedido);

            var produtosAmbiente = Data.DAL.ProdutosPedidoDAO.Instance.GetByAmbiente(sessao, ambiente.IdAmbientePedido);
            var produtos = Data.DAL.ProdutoDAO.Instance.ObterProdutos(sessao, produtosAmbiente.Select(f => f.IdProd).Distinct()).ToList();
            
            // Recupera os itens de rentabilidade dos produtos do pedido
            var itens = new LazyItemRentabilidadeEnumerable(produtosAmbiente.Select(produtoPedido =>
            {
                // Carrega os produtos filhos
                var filhosComposicao = Data.DAL.ProdutosPedidoDAO.Instance.ObterFilhosComposicao(sessao, (int)produtoPedido.IdProdPed);

                if (!produtos.Any(f => f.IdProd == produtoPedido.IdProd))
                {
                    var produto = Data.DAL.ProdutoDAO.Instance.GetElementByPrimaryKey(sessao, produtoPedido.IdProd);
                    if (produto != null)
                        produtos.Add(produto);
                }

                return ObterItemProdutoPedido(sessao, produtoPedido, prazoMedio, produtos, filhosComposicao);
            }));

            return ObterItemAmbientePedido(sessao, ambiente, prazoMedio, itens.OfType<IItemRentabilidade<Data.Model.ProdutosPedido>>());
        }

        /// <summary>
        /// Realiza a conversão dos dados de rentabilidade do pedido para um
        /// registro de rentabilidade.
        /// </summary>
        /// <param name="pedidoRentabilidade"></param>
        /// <returns></returns>
        private IRegistroRentabilidade ConverterParaRegistroRentabilidade(Data.Model.PedidoRentabilidade pedidoRentabilidade)
        {
            var tipo = (TipoRegistroRentabilidade)pedidoRentabilidade.Tipo;

            return new RegistroRentabilidade(pedidoRentabilidade.IdRegistro,
                ProvedorDescritoresRegistro.ObterDescritor(tipo, pedidoRentabilidade.IdRegistro), tipo, pedidoRentabilidade.Valor);
        }

        /// <summary>
        /// Realiza a conversão dos dados de rentabilidade do ambiente para um 
        /// registro de rentabilidade.
        /// </summary>
        /// <param name="ambienteRentabilidade"></param>
        /// <returns></returns>
        private IRegistroRentabilidade ConverterParaRegistroRentabilidade(Data.Model.AmbientePedidoRentabilidade ambienteRentabilidade)
        {
            var tipo = (TipoRegistroRentabilidade)ambienteRentabilidade.Tipo;

            return new RegistroRentabilidade(ambienteRentabilidade.IdRegistro,
                ProvedorDescritoresRegistro.ObterDescritor(tipo, ambienteRentabilidade.IdRegistro), tipo, ambienteRentabilidade.Valor);
        }

        /// <summary>
        /// Realiza a conversão dos dados de rentabilidade do ambiente para um
        /// registro de rentabilidade.
        /// </summary>
        /// <param name="produtoPedidoRentabilidade"></param>
        /// <returns></returns>
        private IRegistroRentabilidade ConverterParaRegistroRentabilidade(Data.Model.ProdutoPedidoRentabilidade produtoPedidoRentabilidade)
        {
            var tipo = (TipoRegistroRentabilidade)produtoPedidoRentabilidade.Tipo;

            return new RegistroRentabilidade(produtoPedidoRentabilidade.IdRegistro,
                ProvedorDescritoresRegistro.ObterDescritor(tipo, produtoPedidoRentabilidade.IdRegistro), tipo, produtoPedidoRentabilidade.Valor);
        }

        #endregion

        #region Membros IProvedorItemRentabilidade

        /// <summary>
        /// Recupera o item com base no pedido informado.
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.Pedido>.ObterItem(Data.Model.Pedido referencia)
        {
            using (var sessao = new GDA.GDASession())
                return ObterItemPedido(sessao, referencia);
        }

        /// <summary>
        /// Recupera o item com base no pedido informado.
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.Pedido>.ObterItem(GDA.GDASession sessao, Data.Model.Pedido referencia)
        {
            return ObterItemPedido(sessao, referencia);
        }

        /// <summary>
        /// Recupera o item com base no identificador do pedido informado.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.Pedido>.ObterItem(int id)
        {
            using (var sessao = new GDA.GDASession())
                return ((IProvedorItemRentabilidade<Data.Model.Pedido>)this).ObterItem(sessao, id);
        }

        /// <summary>
        /// Recupera o item com base no identificador do pedido informado.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.Pedido>.ObterItem(GDA.GDASession sessao, int id)
        {
            var pedido = Data.DAL.PedidoDAO.Instance.GetElementByPrimaryKey(sessao, id);
            return ObterItemPedido(sessao, pedido);
        }

        /// <summary>
        /// Recupera o item com base no produto do pedido.
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.ProdutosPedido>.ObterItem(Data.Model.ProdutosPedido referencia)
        {
            using (var sessao = new GDA.GDASession())
                return ObterItemProdutoPedido(sessao, referencia);
        }

        /// <summary>
        /// Recupera o item com base no produto do pedido.
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.ProdutosPedido>.ObterItem(GDA.GDASession sessao, Data.Model.ProdutosPedido referencia)
        {
            return ObterItemProdutoPedido(sessao, referencia);
        }

        /// <summary>
        /// Recupera o item com base no identificador do produto do pedido.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.ProdutosPedido>.ObterItem(int id)
        {
            using (var sessao = new GDA.GDASession())
                return ((IProvedorItemRentabilidade<Data.Model.ProdutosPedido>)this).ObterItem(sessao, id);
        }

        /// <summary>
        /// Recupera o item com base no identificador do produto do pedido.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.ProdutosPedido>.ObterItem(GDA.GDASession sessao, int id)
        {
            var produtoPedido = Data.DAL.ProdutosPedidoDAO.Instance.GetElementByPrimaryKey(sessao, id);
            return ObterItemProdutoPedido(sessao, produtoPedido);
        }

        /// <summary>
        /// Recupera o item com base no ambiente do pedido.
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.AmbientePedido>.ObterItem(Data.Model.AmbientePedido referencia)
        {
            using (var sessao = new GDA.GDASession())
                return ObterItemAmbientePedido(sessao, referencia);
        }

        /// <summary>
        /// Recupera o item com base no ambiente do pedido.
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.AmbientePedido>.ObterItem(GDA.GDASession sessao, Data.Model.AmbientePedido referencia)
        {
            return ObterItemAmbientePedido(sessao, referencia);
        }

        /// <summary>
        /// Recupera o item com base no identificador do ambiente do pedido.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.AmbientePedido>.ObterItem(int id)
        {
            using (var sessao = new GDA.GDASession())
                return ((IProvedorItemRentabilidade<Data.Model.AmbientePedido>)this).ObterItem(sessao, id);
        }

        /// <summary>
        /// Recupera o item com base no identificador do ambiente do pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.AmbientePedido>.ObterItem(GDA.GDASession sessao, int id)
        {
            var ambiente = Data.DAL.AmbientePedidoDAO.Instance.GetElementByPrimaryKey(sessao, id);
            return ObterItemAmbientePedido(sessao, ambiente);
        }

        #endregion

        #region Membros Data.ICalculadoraRentabilidade

        /// <summary>
        /// Executa o calculo da rentabilidade para o tipo principal da calculadora..
        /// </summary>
        /// <param name="id">Identificador da instancia principal.</param>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.Pedido>.Calcular(GDA.GDASession sessao, uint id)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var pedido = Data.DAL.PedidoDAO.Instance.GetElementByPrimaryKey(id);
            return (this as Data.ICalculadoraRentabilidade<Data.Model.Pedido>).Calcular(sessao, pedido);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o tipo principal da calculadora..
        /// </summary>
        /// <param name="instancia">Instancia principal.</param>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.Pedido>.Calcular(GDA.GDASession sessao, Data.Model.Pedido instancia)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            IdFunc = (int)instancia.IdFunc;
            IdLoja = (int)instancia.IdLoja;
            var item = ObterItemPedido(sessao, instancia);
            return Calcular(item);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o produto do tipo principal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="id">Identificador do produto.</param>
        /// <returns></returns>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.ProdutosPedido>.Calcular(GDA.GDASession sessao, uint id)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var produtoPedido = Data.DAL.ProdutosPedidoDAO.Instance.GetElementByPrimaryKey(sessao, id);
            return (this as Data.ICalculadoraRentabilidade<Data.Model.ProdutosPedido>).Calcular(sessao, produtoPedido);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o produto do tipo principal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="instancia">Instância do produto.</param>
        /// <returns></returns>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.ProdutosPedido>.Calcular(GDA.GDASession sessao, Data.Model.ProdutosPedido instancia)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var pedido = Data.DAL.PedidoDAO.Instance.GetElementByPrimaryKey(sessao, instancia.IdPedido);
            IdFunc = (int)pedido.IdFunc;
            IdLoja = (int)pedido.IdLoja;

            var item = ObterItemProdutoPedido(sessao, instancia);
            return Calcular(item);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o ambiente do tipo principal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="id">Identificador do ambiente.</param>
        /// <returns></returns>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.AmbientePedido>.Calcular(GDA.GDASession sessao, uint id)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var ambiente = Data.DAL.AmbientePedidoDAO.Instance.GetElementByPrimaryKey(sessao, id);
            return (this as Data.ICalculadoraRentabilidade<Data.Model.AmbientePedido>).Calcular(sessao, ambiente);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o ambiente do tipo principal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="instancia">Instância do ambiente.</param>
        /// <returns></returns>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.AmbientePedido>.Calcular(GDA.GDASession sessao, Data.Model.AmbientePedido instancia)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var pedido = Data.DAL.PedidoDAO.Instance.GetElementByPrimaryKey(sessao, instancia.IdPedido);
            IdFunc = (int)pedido.IdFunc;
            IdLoja = (int)pedido.IdLoja;

            var item = ObterItemAmbientePedido(sessao, instancia);
            return Calcular(item);
        }

        #endregion
    }
}
