using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Rentabilidade;
using Glass.Rentabilidade.Negocios;
using Glass.Rentabilidade.Negocios.Componentes;

namespace Glass.PCP.Negocios.Componentes
{
    /// <summary>
    /// Implementação da calculadora de rentabilidade do pedido espelho.
    /// </summary>
    public sealed class CalculadoraRentabilidadePedidoEspelho :
        Rentabilidade.Negocios.Componentes.CalculadoraRentabilidade,
        Data.ICalculadoraRentabilidade<Data.Model.PedidoEspelho>,
        Data.ICalculadoraRentabilidade<Data.Model.AmbientePedidoEspelho>,
        Data.ICalculadoraRentabilidade<Data.Model.ProdutosPedidoEspelho>,
        IProvedorItemRentabilidade<Data.Model.PedidoEspelho>,
        IProvedorItemRentabilidade<Data.Model.ProdutosPedidoEspelho>,
        IProvedorItemRentabilidade<Data.Model.AmbientePedidoEspelho>
    {
        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="provedorDescritoresRegistro"></param>
        /// <param name="provedorIndicadoresFinanceiro"></param>
        /// <param name="provedorCalculadoraRentabilidade"></param>
        public CalculadoraRentabilidadePedidoEspelho(
            IProvedorDescritorRegistroRentabilidade provedorDescritoresRegistro,
            IProvedorIndicadorFinanceiro provedorIndicadoresFinanceiro,
            IProvedorCalculadoraRentabilidade provedorCalculadoraRentabilidade)
            : base(provedorDescritoresRegistro, provedorIndicadoresFinanceiro, provedorCalculadoraRentabilidade)
        {
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

            var itemPedido = item as IItemRentabilidade<Data.Model.PedidoEspelho>;
            var itemProdutoPedido = item as IItemRentabilidade<Data.Model.ProdutosPedidoEspelho>;
            var itemAmbientePedido = item as IItemRentabilidade<Data.Model.AmbientePedidoEspelho>;

            if (itemPedido != null)
            {
                resultado = MontarResultado
                    ((IItemRentabilidadeComReferencias<Data.Model.PedidoEspelhoRentabilidade>)item,
                    (x, y) => (int)x.Tipo == y.Tipo && x.IdRegistro == y.IdRegistro,
                    (x, y) => y.Valor = x.Valor);

                var pedido = itemPedido.Proprietario;

                // Registra o evento para salvar o dados do pedido quando o resultado for salvo
                resultado.Salvando += (sender, e) =>
                {
                    var rentabilidade = pedido.RentabilidadeFinanceira = e.RentabilidadeFinanceira;
                    var percentual = pedido.PercentualRentabilidade = e.PercentualRentabilidade * 100m;
                    Data.DAL.PedidoEspelhoDAO.Instance.AtualizarRentabilidade(e.Sessao, pedido.IdPedido, percentual, rentabilidade);
                };
            }
            else if (itemProdutoPedido != null)
            {
                resultado = MontarResultado
                    ((IItemRentabilidadeComReferencias<Data.Model.ProdutoPedidoEspelhoRentabilidade>)item,
                    (x, y) => (int)x.Tipo == y.Tipo && x.IdRegistro == y.IdRegistro,
                    (x, y) => y.Valor = x.Valor);

                var produtoPedido = itemProdutoPedido.Proprietario;

                // Registra o evento para salvar o dados do pedido quando o resultado for salvo
                resultado.Salvando += (sender, e) =>
                {
                    var rentabilidade = produtoPedido.RentabilidadeFinanceira = e.RentabilidadeFinanceira;
                    var percentual = produtoPedido.PercentualRentabilidade = e.PercentualRentabilidade * 100m;
                    Data.DAL.ProdutosPedidoEspelhoDAO.Instance.AtualizarRentabilidade(e.Sessao, produtoPedido.IdProdPed, percentual, rentabilidade);
                };
            }
            else if (itemAmbientePedido != null)
            {
                resultado = MontarResultado
                    ((IItemRentabilidadeComReferencias<Data.Model.AmbientePedidoEspelhoRentabilidade>)item,
                    (x, y) => (int)x.Tipo == y.Tipo && x.IdRegistro == y.IdRegistro,
                    (x, y) => y.Valor = x.Valor);

                var ambiente = itemAmbientePedido.Proprietario;

                // Registra o evento para salvar o dados do pedido quando o resultado for salvo
                resultado.Salvando += (sender, e) =>
                {
                    var rentabilidade = ambiente.RentabilidadeFinanceira = e.RentabilidadeFinanceira;
                    var percentual = ambiente.PercentualRentabilidade = e.PercentualRentabilidade * 100m;
                    Data.DAL.AmbientePedidoEspelhoDAO.Instance.AtualizarRentabilidade(e.Sessao, ambiente.IdAmbientePedido, percentual, rentabilidade);
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
        private IItemRentabilidade<Data.Model.ProdutosPedidoEspelho> ObterItemProdutoPedido(
            GDA.GDASession sessao, Data.Model.ProdutosPedidoEspelho produtoPedido,
            int prazoMedio, IEnumerable<Data.Model.Produto> produtos,
            IEnumerable<Data.Model.ProdutosPedidoEspelho> produtosPedidoFilhos)
        {
            var registros = new Lazy<IList<Data.Model.ProdutoPedidoEspelhoRentabilidade>>(
                () => Data.DAL.ProdutoPedidoEspelhoRentabilidadeDAO.Instance.ObterPorProdutoPedido(sessao, produtoPedido.IdProdPed));

            var criarRegistro = new CriadorRegistroRentabilidade((tipo, nome, valor) =>
            {
                var idRegistro = ProvedorDescritoresRegistro.ObterRegistro(tipo, nome);
                var registro = registros.Value.FirstOrDefault(f => f.Tipo == (int)tipo && f.IdRegistro == idRegistro);

                if (registro == null)
                {
                    registro = new Data.Model.ProdutoPedidoEspelhoRentabilidade
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
            var produto = produtos.First(f => f.IdProd == produtoPedido.IdProd);

            // Calculao custo do produto
            var custoProd = Global.CalculosFluxo.CalcTotaisItemProdFast(sessao, produto.TipoCalculo, 
                produtoPedido.Altura, produtoPedido.Largura, produtoPedido.Qtde,
                produtoPedido.TotM, produto.CustoCompra, produtoPedido.AlturaBenef.GetValueOrDefault(2),
                produtoPedido.LarguraBenef.GetValueOrDefault(2));

            if (produtosPedidoFilhos.Any())
            {
                // Recupera os itens dos produtos filhos
                var itens = produtosPedidoFilhos.Select(produtoPedido1 =>
                    ObterItemProdutoPedido(sessao, produtoPedido1, prazoMedio, produtos, new Data.Model.ProdutosPedidoEspelho[0]))
                    .ToList();

                return new ItemRentabilidadeContainer<Data.Model.ProdutosPedidoEspelho, Data.Model.ProdutoPedidoEspelhoRentabilidade>(
                    ProvedorIndicadoresFinanceiro, criarRegistro, produtoPedido, itens, f => true, registros,
                    ConverterParaRegistroRentabilidade)
                {
                    Descricao = $"Produto ({produto?.CodInterno}) {produto?.Descricao}",
                    PrecoVendaSemIPI = produtoPedido.Total + produtoPedido.ValorBenef,
                    PrazoMedio = prazoMedio,
                    FatorICMSSubstituicao = 0,
                    PercentualComissao = produtoPedido.PercComissao / 100m,
                    PercentualRentabilidade = produtoPedido.PercentualRentabilidade / 100m,
                    RentabilidadeFinanceira = produtoPedido.RentabilidadeFinanceira
                };
            }
            else
                return new ItemRentabilidade<Data.Model.ProdutosPedidoEspelho, Data.Model.ProdutoPedidoEspelhoRentabilidade>(
                    ProvedorIndicadoresFinanceiro, criarRegistro, produtoPedido, registros, ConverterParaRegistroRentabilidade)
                {
                    Descricao = $"Produto ({produto.CodInterno}) {produto.Descricao}",
                    PrecoVendaSemIPI = produtoPedido.Total + produtoPedido.ValorBenef, // Na atualiza configuração do sistema o total do produto não possui o valor do IPI
                    PrecoCusto = custoProd,
                    PrazoMedio = prazoMedio,
                    PercentualICMSVenda = (decimal)produtoPedido.AliqIcms / 100m,
                    FatorICMSSubstituicao = 0,
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
        private IEnumerable<IItemRentabilidade<Data.Model.ProdutosPedidoEspelho>>
            ObterItensProdutosPedido(GDA.GDASession sessao, Data.Model.PedidoEspelho pedido, int prazoMedio)
        {
            var produtosPedido = Data.DAL.ProdutosPedidoEspelhoDAO.Instance.ObterProdutosParaRentabilidade(sessao, pedido.IdPedido);
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
                GDA.GDASession sessao, Data.Model.PedidoEspelho pedido, int prazoMedio,
                IEnumerable<IItemRentabilidade<Data.Model.ProdutosPedidoEspelho>> itensProdutoPedido)
        {
            var ambientesPedido = Data.DAL.AmbientePedidoEspelhoDAO.Instance.ObterAmbientesParaRentabilidade(sessao, pedido.IdPedido);

            foreach (var i in ambientesPedido)
                yield return ObterItemAmbientePedido(sessao, i, prazoMedio,
                    // Filtra os produtos do ambiente
                    itensProdutoPedido.Where(f => f.Proprietario.IdAmbientePedido == i.IdAmbientePedido));
        }

        /// <summary>
        /// Recupera o item da rentabilidade para o pedido informado.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        private IItemRentabilidade ObterItemPedido(GDA.GDASession sessao, Data.Model.PedidoEspelho pedido)
        {
            var registros = new Lazy<IList<Data.Model.PedidoEspelhoRentabilidade>>(
                () => Data.DAL.PedidoEspelhoRentabilidadeDAO.Instance.ObterPorPedido(sessao, pedido.IdPedido));

            var criarRegistro = new CriadorRegistroRentabilidade((tipo, nome, valor) =>
            {
                var idRegistro = ProvedorDescritoresRegistro.ObterRegistro(tipo, nome);
                var registro = registros.Value.FirstOrDefault(f => f.Tipo == (int)tipo && f.IdRegistro == idRegistro);

                if (registro == null)
                {
                    // Cria o registro da rentabilidade do pedido
                    registro = new Data.Model.PedidoEspelhoRentabilidade
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

            int prazoMedio = CalcularPrazoMedioPedido(sessao, pedido.IdPedido);

            // Recupera os itens associados com todos os produtos do pedido
            var produtos = new LazyItemRentabilidadeEnumerable(ObterItensProdutosPedido(sessao, pedido, prazoMedio));

            // Recupera os ambientes do pedido
            var ambientes = new LazyItemRentabilidadeEnumerable(ObterItensAmbientesPedido(sessao, pedido, prazoMedio, produtos.OfType<IItemRentabilidade<Data.Model.ProdutosPedidoEspelho>>()));

            var itens = produtos.Concat(ambientes);

            // Cria o filtro que recupera os itens para serem usados nos calculos
            var filtroItensParaCalculo = new Func<IItemRentabilidade, bool>((item) =>
            {
                if (item is IItemRentabilidade<Data.Model.AmbientePedidoEspelho>)
                    // Calcula a rentabilidade somente de ambientes com produtos
                    return ((IItemRentabilidadeContainer)item).Itens.Any();

                var produtoPedido = (item as IItemRentabilidade<Data.Model.ProdutosPedidoEspelho>)?.Proprietario;
                return produtoPedido != null && !produtoPedido.InvisivelFluxo && !produtoPedido.IdAmbientePedido.HasValue;
            });

            decimal percentualComissao = 0;

            if (Glass.Configuracoes.PedidoConfig.Comissao.UsarComissaoPorProduto)
            {
                decimal percComissao = 0;
                var total = pedido.Total;

                if (total > 0)
                    foreach (var item in itens.Where(filtroItensParaCalculo))
                        percComissao += ((item.PrecoVendaSemIPI * 100) / total) * (item.PercentualComissao);

                percentualComissao = percComissao / 100m;
            }
            else
                percentualComissao = (decimal)Data.DAL.PedidoDAO.Instance.ObterPercentualComissao(sessao, (int)pedido.IdPedido) / 100m;

            return new ItemRentabilidadeContainer<Data.Model.PedidoEspelho, Data.Model.PedidoEspelhoRentabilidade>(
                ProvedorIndicadoresFinanceiro, criarRegistro, pedido, itens, filtroItensParaCalculo, registros,
                ConverterParaRegistroRentabilidade)
            {
                Descricao = $"Pedido {pedido.IdPedido}",
                PrecoVendaSemIPI = (pedido.Total - pedido.ValorIpi) - pedido.ValorEntrega,
                PrazoMedio = prazoMedio,
                FatorICMSSubstituicao = 0,
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
            GDA.GDASession sessao, Data.Model.AmbientePedidoEspelho ambiente, int prazoMedio,
            IEnumerable<IItemRentabilidade<Data.Model.ProdutosPedidoEspelho>> produtos)
        {
            var registros = new Lazy<IList<Data.Model.AmbientePedidoEspelhoRentabilidade>>(
                () => Data.DAL.AmbientePedidoEspelhoRentabilidadeDAO.Instance.ObterPorAmbiente(sessao, ambiente.IdAmbientePedido));

            var criarRegistro = new CriadorRegistroRentabilidade((tipo, nome, valor) =>
            {
                var idRegistro = ProvedorDescritoresRegistro.ObterRegistro(tipo, nome);
                var registro = registros.Value.FirstOrDefault(f => f.Tipo == (int)tipo && f.IdRegistro == idRegistro);

                if (registro == null)
                {
                    // Cria o registro da rentabilidade do ambiente do pedido
                    registro = new Data.Model.AmbientePedidoEspelhoRentabilidade
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

            return new ItemRentabilidadeContainer<Data.Model.AmbientePedidoEspelho, Data.Model.AmbientePedidoEspelhoRentabilidade>(
                ProvedorIndicadoresFinanceiro, criarRegistro, ambiente, produtos, f => true, registros,
                ConverterParaRegistroRentabilidade)
            {
                Descricao = $"Ambiente {ambiente.Ambiente}",
                PrecoVendaSemIPI = total, // Na atualiza configuração do sistema o total do ambiente não possui o valor do IPI
                PrazoMedio = prazoMedio,
                FatorICMSSubstituicao = 0,
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
        private IItemRentabilidade ObterItemProdutoPedido(GDA.GDASession sessao, Data.Model.ProdutosPedidoEspelho produtoPedido)
        {
            var prazoMedio = CalcularPrazoMedioPedido(sessao, produtoPedido.IdPedido);
            var filhos = Data.DAL.ProdutosPedidoEspelhoDAO.Instance.ObterFilhosComposicao(sessao, produtoPedido.IdProdPed);

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
        private IItemRentabilidade ObterItemAmbientePedido(GDA.GDASession sessao, Data.Model.AmbientePedidoEspelho ambiente)
        {
            var prazoMedio = CalcularPrazoMedioPedido(sessao, ambiente.IdPedido);

            var produtosAmbiente = Data.DAL.ProdutosPedidoEspelhoDAO.Instance.GetByAmbiente(sessao, ambiente.IdAmbientePedido);
            var produtos = Data.DAL.ProdutoDAO.Instance.ObterProdutos(sessao, produtosAmbiente.Select(f => f.IdProd).Distinct()).ToList();

            // Recupera os itens de rentabilidade dos produtos do pedido
            var itens = new LazyItemRentabilidadeEnumerable(produtosAmbiente.Select(produtoPedido =>
            {
                // Carrega os produtos filhos
                var filhosComposicao = Data.DAL.ProdutosPedidoEspelhoDAO.Instance.ObterFilhosComposicao(sessao, produtoPedido.IdProdPed);

                if (!produtos.Any(f => f.IdProd == produtoPedido.IdProd))
                {
                    var produto = Data.DAL.ProdutoDAO.Instance.GetElementByPrimaryKey(sessao, produtoPedido.IdProd);
                    if (produto != null)
                        produtos.Add(produto);
                }

                return ObterItemProdutoPedido(sessao, produtoPedido, prazoMedio, produtos, filhosComposicao);
            }));

            return ObterItemAmbientePedido(sessao, ambiente, prazoMedio, itens.OfType<IItemRentabilidade<Data.Model.ProdutosPedidoEspelho>>());
        }

        /// <summary>
        /// Realiza a conversão dos dados de rentabilidade do pedido para um
        /// registro de rentabilidade.
        /// </summary>
        /// <param name="pedidoRentabilidade"></param>
        /// <returns></returns>
        private IRegistroRentabilidade ConverterParaRegistroRentabilidade(Data.Model.PedidoEspelhoRentabilidade pedidoRentabilidade)
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
        private IRegistroRentabilidade ConverterParaRegistroRentabilidade(Data.Model.AmbientePedidoEspelhoRentabilidade ambienteRentabilidade)
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
        private IRegistroRentabilidade ConverterParaRegistroRentabilidade(Data.Model.ProdutoPedidoEspelhoRentabilidade produtoPedidoRentabilidade)
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
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.PedidoEspelho>.ObterItem(Data.Model.PedidoEspelho referencia)
        {
            using (var sessao = new GDA.GDASession())
                return ObterItemPedido(sessao, referencia);
        }

        /// <summary>
        /// Recupera o item com base no identificador do pedido informado.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.PedidoEspelho>.ObterItem(int id)
        {
            using (var sessao = new GDA.GDASession())
            {
                var pedido = Data.DAL.PedidoEspelhoDAO.Instance.GetElementByPrimaryKey(sessao, id);
                return ObterItemPedido(sessao, pedido);
            }
        }

        /// <summary>
        /// Recupera o item com base no produto do pedido.
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.ProdutosPedidoEspelho>.ObterItem(Data.Model.ProdutosPedidoEspelho referencia)
        {
            using (var sessao = new GDA.GDASession())
                return ObterItemProdutoPedido(sessao, referencia);
        }

        /// <summary>
        /// Recupera o item com base no identificador do produto do pedido.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.ProdutosPedidoEspelho>.ObterItem(int id)
        {
            using (var sessao = new GDA.GDASession())
            {
                var produtoPedido = Data.DAL.ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(sessao, id);
                return ObterItemProdutoPedido(sessao, produtoPedido);
            }
        }

        /// <summary>
        /// Recupera o item com base no ambiente do pedido.
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.AmbientePedidoEspelho>.ObterItem(Data.Model.AmbientePedidoEspelho referencia)
        {
            using (var sessao = new GDA.GDASession())
                return ObterItemAmbientePedido(sessao, referencia);
        }

        /// <summary>
        /// Recupera o item com base no identificador do ambiente do pedido.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.AmbientePedidoEspelho>.ObterItem(int id)
        {
            using (var sessao = new GDA.GDASession())
            {
                var ambiente = Data.DAL.AmbientePedidoEspelhoDAO.Instance.GetElementByPrimaryKey(sessao, id);
                return ObterItemAmbientePedido(sessao, ambiente);
            }
        }

        #endregion

        #region Membros Data.ICalculadoraRentabilidade

        /// <summary>
        /// Executa o calculo da rentabilidade para o tipo principal da calculadora..
        /// </summary>
        /// <param name="id">Identificador da instancia principal.</param>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.PedidoEspelho>.Calcular(GDA.GDASession sessao, uint id)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var pedido = Data.DAL.PedidoEspelhoDAO.Instance.GetElementByPrimaryKey(id);
            return (this as Data.ICalculadoraRentabilidade<Data.Model.PedidoEspelho>).Calcular(sessao, pedido);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o tipo principal da calculadora..
        /// </summary>
        /// <param name="instancia">Instancia principal.</param>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.PedidoEspelho>.Calcular(GDA.GDASession sessao, Data.Model.PedidoEspelho instancia)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var item = ObterItemPedido(sessao, instancia);
            return Calcular(item);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o produto do tipo principal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="id">Identificador do produto.</param>
        /// <returns></returns>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.ProdutosPedidoEspelho>.Calcular(GDA.GDASession sessao, uint id)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var produtoPedido = Data.DAL.ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(id);
            return (this as Data.ICalculadoraRentabilidade<Data.Model.ProdutosPedidoEspelho>).Calcular(sessao, produtoPedido);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o produto do tipo principal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="instancia">Instância do produto.</param>
        /// <returns></returns>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.ProdutosPedidoEspelho>.Calcular(GDA.GDASession sessao, Data.Model.ProdutosPedidoEspelho instancia)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var item = ObterItemProdutoPedido(sessao, instancia);
            return Calcular(item);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o ambiente do tipo principal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="id">Identificador do ambiente.</param>
        /// <returns></returns>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.AmbientePedidoEspelho>.Calcular(GDA.GDASession sessao, uint id)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var ambiente = Data.DAL.AmbientePedidoEspelhoDAO.Instance.GetElementByPrimaryKey(sessao, id);
            return (this as Data.ICalculadoraRentabilidade<Data.Model.AmbientePedidoEspelho>).Calcular(sessao, ambiente);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o ambiente do tipo principal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="instancia">Instância do ambiente.</param>
        /// <returns></returns>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.AmbientePedidoEspelho>.Calcular(GDA.GDASession sessao, Data.Model.AmbientePedidoEspelho instancia)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var item = ObterItemAmbientePedido(sessao, instancia);
            return Calcular(item);
        }

        #endregion
    }
}
