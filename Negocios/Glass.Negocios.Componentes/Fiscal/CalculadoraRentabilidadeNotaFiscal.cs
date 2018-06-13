using Glass.Rentabilidade;
using Glass.Rentabilidade.Negocios;
using Glass.Rentabilidade.Negocios.Componentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Componentes
{
    /// <summary>
    /// Implementação da calculadora de rentabilidade da nota fiscal.
    /// </summary>
    public class CalculadoraRentabilidadeNotaFiscal :
        Rentabilidade.Negocios.Componentes.CalculadoraRentabilidade,
        Data.ICalculadoraRentabilidade<Data.Model.NotaFiscal>,
        Data.ICalculadoraRentabilidade<Data.Model.ProdutosNf>,
        IProvedorItemRentabilidade<Data.Model.NotaFiscal>,
        IProvedorItemRentabilidade<Data.Model.ProdutosNf>
    {
        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="provedorDescritoresRegistro"></param>
        /// <param name="provedorIndicadoresFinanceiro"></param>
        /// <param name="provedorCalculadoraRentabilidade"></param>
        public CalculadoraRentabilidadeNotaFiscal(
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

            var itemNotaFiscal = item as IItemRentabilidade<Data.Model.NotaFiscal>;
            var itemProdutoNf = item as IItemRentabilidade<Data.Model.ProdutosNf>;
            var itemProdutoNfCusto = item as IItemRentabilidade<Data.Model.ProdutoNfCusto>;

            if (itemNotaFiscal != null)
            {
                resultado = MontarResultado
                    ((IItemRentabilidadeComReferencias<Data.Model.NotaFiscalRentabilidade>)item,
                    (x, y) => (int)x.Tipo == y.Tipo && x.IdRegistro == y.IdRegistro,
                    (x, y) => y.Valor = x.Valor);

                var noteFiscal = itemNotaFiscal.Proprietario;

                // Registra o evento para salvar o dados da nota fiscal quando o resultado for salvo
                resultado.Salvando += (sender, e) =>
                {
                    var rentabilidade = noteFiscal.RentabilidadeFinanceira = e.RentabilidadeFinanceira;
                    var percentual = noteFiscal.PercentualRentabilidade = e.PercentualRentabilidade * 100m;
                    Data.DAL.NotaFiscalDAO.Instance.AtualizarRentabilidade(e.Sessao, noteFiscal.IdNf, percentual, rentabilidade);
                };
            }
            else if (itemProdutoNf != null)
            {
                resultado = MontarResultado
                    ((IItemRentabilidadeComReferencias<Data.Model.ProdutoNfRentabilidade>)item,
                    (x, y) => (int)x.Tipo == y.Tipo && x.IdRegistro == y.IdRegistro,
                    (x, y) => y.Valor = x.Valor);

                var produtoNf = itemProdutoNf.Proprietario;

                // Registra o evento para salvar o dados do produto da nota fiscal quando o resultado for salvo
                resultado.Salvando += (sender, e) =>
                {
                    var rentabilidade = produtoNf.RentabilidadeFinanceira = e.RentabilidadeFinanceira;
                    var percentual = produtoNf.PercentualRentabilidade = e.PercentualRentabilidade * 100m;
                    Data.DAL.ProdutosNfDAO.Instance.AtualizarRentabilidade(e.Sessao, produtoNf.IdProdNf, percentual, rentabilidade);
                };
            }
            else if (itemProdutoNfCusto != null)
            {
                resultado = MontarResultado
                    ((IItemRentabilidadeComReferencias<Data.Model.ProdutoNfCustoRentabilidade>)item,
                    (x, y) => (int)x.Tipo == y.Tipo && x.IdRegistro == y.IdRegistro,
                    (x, y) => y.Valor = x.Valor);

                var produtoNfCusto = itemProdutoNfCusto.Proprietario;

                // Registra o evento para salvar o dados do custo do produto da nota fiscal quando o resultado for salvo
                resultado.Salvando += (sender, e) =>
                {
                    var rentabilidade = produtoNfCusto.RentabilidadeFinanceira = e.RentabilidadeFinanceira;
                    var percentual = produtoNfCusto.PercentualRentabilidade = e.PercentualRentabilidade * 100m;
                    Data.DAL.ProdutoNfCustoDAO.Instance.AtualizarRentabilidade(e.Sessao, produtoNfCusto.IdProdNfCusto, percentual, rentabilidade);
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
        /// Calcula o prazo médida da nota fiscal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNf"></param>
        /// <returns></returns>
        private int CalcularPrazoMedioNotaFiscal(GDA.GDASession sessao, uint idNf)
        {
            return Data.DAL.NotaFiscalDAO.Instance.CalcularPrazoMedio(sessao, idNf);
        }

        /// <summary>
        /// Recupera o item da rentabilidade para o custo do produto da nota fiscal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="produtoNf"></param>
        /// <param name="produtoNfCusto"></param>
        /// <param name="prazoMedio"></param>
        /// <param name="produto"></param>
        /// <returns></returns>
        private IItemRentabilidade<Data.Model.ProdutoNfCusto> ObterItemProdutoNfCusto(
            GDA.GDASession sessao, Data.Model.ProdutosNf produtoNf,
            Data.Model.ProdutoNfCusto produtoNfCusto, int prazoMedio, Data.Model.Produto produto)
        {
            var registros = new Lazy<IList<Data.Model.ProdutoNfCustoRentabilidade>>(
                    () => Data.DAL.ProdutoNfCustoRentabilidadeDAO.Instance.ObterPorProdutoNf(sessao, produtoNfCusto.IdProdNfCusto));

            var criarRegistro = new CriadorRegistroRentabilidade(
                (tipo, nome, valor) =>
                {
                    var idRegistro = ProvedorDescritoresRegistro.ObterRegistro(tipo, nome);
                    var registro = registros.Value.FirstOrDefault(f => f.Tipo == (int)tipo && f.IdRegistro == idRegistro);

                    if (registro == null)
                    {
                        registro = new Data.Model.ProdutoNfCustoRentabilidade
                        {
                            IdProdNfCusto = produtoNfCusto.IdProdNfCusto,
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

            // Calcula o custo do produto
            var custoProd = Glass.Global.CalculosFluxo.CalcTotaisItemProdFast(sessao, produto.TipoCalculo,
                produtoNf.Altura, produtoNf.Largura, produtoNf.Qtde,
                produtoNf.TotM, produtoNfCusto.CustoCompra,
                produtoNf.AlturaBenef.GetValueOrDefault(2),
                produtoNf.LarguraBenef.GetValueOrDefault(2));

            var preceoVendaSemIPIUnitario = (produtoNf.Total + produtoNf.ValorIcmsSt) / (decimal)produtoNf.Qtde;

            return new ItemRentabilidade<Data.Model.ProdutoNfCusto, Data.Model.ProdutoNfCustoRentabilidade>(
                    ProvedorIndicadoresFinanceiro, criarRegistro, produtoNfCusto, registros, ConverterParaRegistroRentabilidade)
            {
                Descricao = $"Produto [{produtoNfCusto.Qtde}] ({produto?.CodInterno}) {produto?.Descricao}",
                PrecoVendaSemIPI = preceoVendaSemIPIUnitario * produtoNfCusto.Qtde,
                PrecoCusto = custoProd,
                PrazoMedio = prazoMedio,
                PercentualICMSVenda = (decimal)produtoNf.AliqIcms / 100m,
                FatorICMSSubstituicao = 0,
                PercentualIPICompra = (decimal)(produto?.AliqIPI ?? 0) / 100m,
                PercentualIPIVenda = (decimal)produtoNf.AliqIpi / 100m,
                PercentualComissao = produtoNf.PercComissao / 100m,
                CustosExtras = 0m,
                PercentualRentabilidade = produtoNfCusto.PercentualRentabilidade / 100m,
                RentabilidadeFinanceira = produtoNfCusto.RentabilidadeFinanceira
            };
        }

        /// <summary>
        /// Recupera um item de rentabilidade para o produto pedido informado.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="produtoNf"></param>
        /// <param name="produtoNfCustos">Custos associados.</param>
        /// <param name="prazoMedio"></param>
        /// <param name="produtos"></param>
        /// <returns></returns>
        private IItemRentabilidade<Data.Model.ProdutosNf> ObterItemProdutoNf(
            GDA.GDASession sessao, Data.Model.ProdutosNf produtoNf, IEnumerable<Data.Model.ProdutoNfCusto> produtoNfCustos,
            int prazoMedio, IEnumerable<Data.Model.Produto> produtos)
        {
            var registros = new Lazy<IList<Data.Model.ProdutoNfRentabilidade>>(
                    () => Data.DAL.ProdutoNfRentabilidadeDAO.Instance.ObterPorProdutoNf(sessao, produtoNf.IdProdNf));

            var criarRegistro = new CriadorRegistroRentabilidade(
                (tipo, nome, valor) =>
                {
                    var idRegistro = ProvedorDescritoresRegistro.ObterRegistro(tipo, nome);
                    var registro = registros.Value.FirstOrDefault(f => f.Tipo == (int)tipo && f.IdRegistro == idRegistro);

                    if (registro == null)
                    {
                        registro = new Data.Model.ProdutoNfRentabilidade
                        {
                            IdProdNf = (int)produtoNf.IdProdNf,
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
            var produto = produtos.FirstOrDefault(f => f.IdProd == produtoNf.IdProd);

            if (!produtoNfCustos.Any())
                produtoNfCustos = new[]
                {
                    new Data.Model.ProdutoNfCusto
                    {
                       IdProdNf = produtoNf.IdProdNf,
                       IdProdNfEntrada = 0,
                       Qtde = (int)produtoNf.Qtde,
                       CustoCompra = produto.CustoCompra
                    }
                };

            var qtdeProdutoNfCustos = produtoNfCustos.Count();

            if (qtdeProdutoNfCustos > 1)
            {
                var itens = produtoNfCustos.Select(produtoNfCusto => ObterItemProdutoNfCusto(sessao, produtoNf, produtoNfCusto, prazoMedio, produto));

                return new ItemRentabilidadeContainer<Data.Model.ProdutosNf, Data.Model.ProdutoNfRentabilidade>(
                        ProvedorIndicadoresFinanceiro, criarRegistro,
                        produtoNf, itens, f => true, registros,
                        ConverterParaRegistroRentabilidade)
                {
                    Descricao = $"Produto ({produto?.CodInterno}) {produto?.Descricao}",
                    PrecoVendaSemIPI = produtoNf.Total + produtoNf.ValorIcmsSt,
                    PrazoMedio = prazoMedio,
                    FatorICMSSubstituicao = 0,
                    PercentualComissao = produtoNf.PercComissao / 100m,
                    PercentualRentabilidade = produtoNf.PercentualRentabilidade / 100m,
                    RentabilidadeFinanceira = produtoNf.RentabilidadeFinanceira
                };
            }
            else
            {
                var custoCompra = qtdeProdutoNfCustos == 1 ? produtoNfCustos.First().CustoCompra : produto.CustoCompra;

                // Calcula o custo do produto
                var custoProd = Glass.Global.CalculosFluxo.CalcTotaisItemProdFast(sessao, produto.TipoCalculo,
                    produtoNf.Altura, produtoNf.Largura, produtoNf.Qtde,
                    produtoNf.TotM, custoCompra,
                    produtoNf.AlturaBenef.GetValueOrDefault(2),
                    produtoNf.LarguraBenef.GetValueOrDefault(2));

                return new ItemRentabilidade<Data.Model.ProdutosNf, Data.Model.ProdutoNfRentabilidade>(
                    ProvedorIndicadoresFinanceiro, criarRegistro, produtoNf, registros, ConverterParaRegistroRentabilidade)
                {
                    Descricao = $"Produto ({produto?.CodInterno}) {produto?.Descricao}",
                    PrecoVendaSemIPI = produtoNf.Total + produtoNf.ValorIcmsSt, // Não atualizar a configuração do sistema o total do produto não possui o valor do IPI
                    PrecoCusto = custoProd,
                    PrazoMedio = prazoMedio,
                    PercentualICMSVenda = (decimal)produtoNf.AliqIcms / 100m,
                    FatorICMSSubstituicao = 0,
                    PercentualIPICompra = (decimal)(produto?.AliqIPI ?? 0) / 100m,
                    PercentualIPIVenda = (decimal)produtoNf.AliqIpi / 100m,
                    PercentualComissao = produtoNf.PercComissao / 100m,
                    CustosExtras = 0m,
                    PercentualRentabilidade = produtoNf.PercentualRentabilidade / 100m,
                    RentabilidadeFinanceira = produtoNf.RentabilidadeFinanceira
                };
            }
            
        }

        /// <summary>
        /// Recupera os itens de rentabilidade dos produtos da nota fiscal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="notaFiscal">Pedido.</param>
        /// <param name="prazoMedio">Prazo médio de faturamenteo do pedido.</param>
        /// <returns></returns>
        private IEnumerable<IItemRentabilidade<Data.Model.ProdutosNf>>
            ObterItensProdutosNf(GDA.GDASession sessao, Data.Model.NotaFiscal notaFiscal, int prazoMedio)
        {
            var produtosNf = Data.DAL.ProdutosNfDAO.Instance.ObterProdutosParaRentabilidade(sessao, notaFiscal.IdNf);
            var produtosNfCusto = Data.DAL.ProdutoNfCustoDAO.Instance.ObterCustosPorNotaFiscal(sessao, notaFiscal.IdNf);
            var produtos = Data.DAL.ProdutoDAO.Instance.ObterProdutos(sessao, produtosNf.Select(f => f.IdProd).Distinct()).ToList();

            foreach (var i in produtosNf)
                yield return ObterItemProdutoNf(sessao, i, produtosNfCusto.Where(f => f.IdProdNf == i.IdProdNf), prazoMedio, produtos);
        }

        /// <summary>
        /// Recupera o item da rentabilidade para a nota fiscal informado.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="notaFiscal"></param>
        /// <returns></returns>
        private IItemRentabilidade ObterItemNotaFiscal(GDA.GDASession sessao, Data.Model.NotaFiscal notaFiscal)
        {
            var registros = new Lazy<IList<Data.Model.NotaFiscalRentabilidade>>(
                () => Data.DAL.NotaFiscalRentabilidadeDAO.Instance.ObterPorPedido(sessao, notaFiscal.IdNf));

            var criarRegistro = new CriadorRegistroRentabilidade((tipo, nome, valor) =>
            {
                var idRegistro = ProvedorDescritoresRegistro.ObterRegistro(tipo, nome);
                var registro = registros.Value.FirstOrDefault(f => f.Tipo == (int)tipo && f.IdRegistro == idRegistro);

                if (registro == null)
                {
                    // Cria o registro da rentabilidade da nota fiscal
                    registro = new Data.Model.NotaFiscalRentabilidade
                    {
                        IdNf = (int)notaFiscal.IdNf,
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

            var prazoMedio = CalcularPrazoMedioNotaFiscal(sessao, notaFiscal.IdNf);

            // Recupera os itens associados com todos os produtos da nota
            var itens = new LazyItemRentabilidadeEnumerable(ObterItensProdutosNf(sessao, notaFiscal, prazoMedio));

            decimal percentualComissao = 0;

            if (Glass.Configuracoes.PedidoConfig.Comissao.UsarComissaoPorProduto)
            {
                decimal percComissao = 0;
                var total = notaFiscal.TotalNota - notaFiscal.ValorIpi;

                if (total > 0)
                    foreach (var item in itens)
                        percComissao += ((item.PrecoVendaSemIPI * 100) / total) * (item.PercentualComissao);

                percentualComissao = percComissao / 100m;
            }

            return new ItemRentabilidadeContainer<Data.Model.NotaFiscal, Data.Model.NotaFiscalRentabilidade>(
                ProvedorIndicadoresFinanceiro, criarRegistro, notaFiscal, itens, f => true, registros,
                ConverterParaRegistroRentabilidade)
            {
                Descricao = $"Nota Fiscal {notaFiscal.IdNf}",
                PrecoVendaSemIPI = notaFiscal.TotalNota - notaFiscal.ValorIpi,
                PrazoMedio = prazoMedio,
                FatorICMSSubstituicao = 0,
                PercentualComissao = percentualComissao,
                CustosExtras = notaFiscal.OutrasDespesas + notaFiscal.ValorSeguro + notaFiscal.ValorFrete,
                PercentualRentabilidade = notaFiscal.PercentualRentabilidade / 100m,
                RentabilidadeFinanceira = notaFiscal.RentabilidadeFinanceira
            };
        }

        /// <summary>
        /// Recupera o item de rentabilidade para o produto da nota fiscal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="produtoNf"></param>
        /// <returns></returns>
        private IItemRentabilidade ObterItemProdutoNf(GDA.GDASession sessao, Data.Model.ProdutosNf produtoNf)
        {
            var prazoMedio = CalcularPrazoMedioNotaFiscal(sessao, produtoNf.IdNf);
            var produto = Data.DAL.ProdutoDAO.Instance.GetElementByPrimaryKey(produtoNf.IdProd);
            var custos = Data.DAL.ProdutoNfCustoDAO.Instance.ObterCustosPorProdutoNotaFiscal(sessao, produtoNf.IdProdNf);

            return ObterItemProdutoNf(sessao, produtoNf, custos, prazoMedio, new[] { produto });
        }

        /// <summary>
        /// Realiza a conversão dos dados de rentabilidade da nota fiscal para um
        /// registro de rentabilidade.
        /// </summary>
        /// <param name="notaFiscalRentabilidade"></param>
        /// <returns></returns>
        private IRegistroRentabilidade ConverterParaRegistroRentabilidade(Data.Model.NotaFiscalRentabilidade notaFiscalRentabilidade)
        {
            var tipo = (TipoRegistroRentabilidade)notaFiscalRentabilidade.Tipo;

            return new RegistroRentabilidade(notaFiscalRentabilidade.IdRegistro,
                ProvedorDescritoresRegistro.ObterDescritor(tipo, notaFiscalRentabilidade.IdRegistro), tipo, notaFiscalRentabilidade.Valor);
        }

        /// <summary>
        /// Realiza a conversão dos dados de rentabilidade do produto da nota para um
        /// registro de rentabilidade.
        /// </summary>
        /// <param name="produtoNfRentabilidade"></param>
        /// <returns></returns>
        private IRegistroRentabilidade ConverterParaRegistroRentabilidade(Data.Model.ProdutoNfRentabilidade produtoNfRentabilidade)
        {
            var tipo = (TipoRegistroRentabilidade)produtoNfRentabilidade.Tipo;

            return new RegistroRentabilidade(produtoNfRentabilidade.IdRegistro,
                ProvedorDescritoresRegistro.ObterDescritor(tipo, produtoNfRentabilidade.IdRegistro), tipo, produtoNfRentabilidade.Valor);
        }

        /// <summary>
        /// Realiza a conversão dos dados de rentabilidade do custo do produto da nota 
        /// para um registro da rentabilidade.
        /// </summary>
        /// <param name="produtoNfCustoRentabilidade"></param>
        /// <returns></returns>
        private IRegistroRentabilidade ConverterParaRegistroRentabilidade(Data.Model.ProdutoNfCustoRentabilidade produtoNfCustoRentabilidade)
        {
            var tipo = (TipoRegistroRentabilidade)produtoNfCustoRentabilidade.Tipo;

            return new RegistroRentabilidade(produtoNfCustoRentabilidade.IdRegistro,
                ProvedorDescritoresRegistro.ObterDescritor(tipo, produtoNfCustoRentabilidade.IdRegistro), tipo, produtoNfCustoRentabilidade.Valor);
        }

        #endregion

        #region Membros IProvedorItemRentabilidade

        /// <summary>
        /// Recupera o item com base na nota fiscal informada.
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.NotaFiscal>.ObterItem(Data.Model.NotaFiscal referencia)
        {
            using (var sessao = new GDA.GDASession())
                return ObterItemNotaFiscal(sessao, referencia);
        }

        /// <summary>
        /// Recupera o item com base no identificador da nota fiscal informada.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.NotaFiscal>.ObterItem(int id)
        {
            using (var sessao = new GDA.GDASession())
            {
                var pedido = Data.DAL.NotaFiscalDAO.Instance.GetElementByPrimaryKey(sessao, id);
                return ObterItemNotaFiscal(sessao, pedido);
            }
        }

        /// <summary>
        /// Recupera o item com base no produto do pedido.
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.ProdutosNf>.ObterItem(Data.Model.ProdutosNf referencia)
        {
            using (var sessao = new GDA.GDASession())
                return ObterItemProdutoNf(sessao, referencia);
        }

        /// <summary>
        /// Recupera o item com base no identificador do produto do pedido.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade IProvedorItemRentabilidade<Data.Model.ProdutosNf>.ObterItem(int id)
        {
            using (var sessao = new GDA.GDASession())
            {
                var produtoPedido = Data.DAL.ProdutosNfDAO.Instance.GetElementByPrimaryKey(sessao, id);
                return ObterItemProdutoNf(sessao, produtoPedido);
            }
        }

        #endregion

        #region Membros Data.ICalculadoraRentabilidade

        /// <summary>
        /// Executa o calculo da rentabilidade para o tipo principal da calculadora.
        /// </summary>
        /// <param name="id">Identificador da instancia principal.</param>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.NotaFiscal>.Calcular(GDA.GDASession sessao, uint id)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var notaFiscal = Data.DAL.NotaFiscalDAO.Instance.GetElementByPrimaryKey(id);
            return (this as Data.ICalculadoraRentabilidade<Data.Model.NotaFiscal>).Calcular(sessao, notaFiscal);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o tipo principal da calculadora.
        /// </summary>
        /// <param name="instancia">Instancia principal.</param>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.NotaFiscal>.Calcular(GDA.GDASession sessao, Data.Model.NotaFiscal instancia)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var item = ObterItemNotaFiscal(sessao, instancia);
            return Calcular(item);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o produto do tipo principal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="id">Identificador do produto.</param>
        /// <returns></returns>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.ProdutosNf>.Calcular(GDA.GDASession sessao, uint id)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var produtoNf = Data.DAL.ProdutosNfDAO.Instance.GetElementByPrimaryKey(id);
            return (this as Data.ICalculadoraRentabilidade<Data.Model.ProdutosNf>).Calcular(sessao, produtoNf);
        }

        /// <summary>
        /// Executa o calculo da rentabilidade para o produto do tipo principal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="instancia">Instância do produto.</param>
        /// <returns></returns>
        Data.ICalculoRentabilidadeResultado Data.ICalculadoraRentabilidade<Data.Model.ProdutosNf>.Calcular(GDA.GDASession sessao, Data.Model.ProdutosNf instancia)
        {
            if (!CalculoHabilitado)
                return CriarResultadoNaoExecutado();

            var item = ObterItemProdutoNf(sessao, instancia);
            return Calcular(item);
        }

        #endregion
    }
}
