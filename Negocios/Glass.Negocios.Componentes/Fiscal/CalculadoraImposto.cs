using Glass.Configuracoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;

namespace Glass.Fiscal.Negocios.Componentes
{
    /// <summary>
    /// Implementação base da calculadora de imposto.
    /// </summary>
    public class CalculadoraImposto : ICalculadoraImposto
    {
        #region Variáveis Locais

        private IItemImpostoContainer _container;
        private decimal _totalDescontoAplicado = 0;

        #endregion

        #region Propriedades

        /// <summary>
        /// Container que está sendo processado.
        /// </summary>
        protected IItemImpostoContainer Container => _container;

        /// <summary>
        /// Provedor dos ICMS de produto por estado.
        /// </summary>
        protected Entidades.IProvedorIcmsProdutoUf ProvedorIcmsProdutoUf { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="provedorIcmsProdutoUf"></param>
        public CalculadoraImposto(Entidades.IProvedorIcmsProdutoUf provedorIcmsProdutoUf)
        {
            if (provedorIcmsProdutoUf == null)
                throw new ArgumentNullException(nameof(provedorIcmsProdutoUf));

            ProvedorIcmsProdutoUf = provedorIcmsProdutoUf;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Carrega a aliquota o ICMS.
        /// </summary>
        /// <param name="item"></param>
        private void CarregarAliquotaIcms(ItemImpostoResultado item)
        {
            item.AliqIcms = ProvedorIcmsProdutoUf
                .ObterIcmsPorProduto(item.Produto, Container.Loja, Container.Fornecedor, Container.Cliente);
        }

        /// <summary>
        /// Carrega a Aliquota do ICMS ST interna.
        /// </summary>
        /// <param name="item"></param>
        private void CarregarAliquotaIcmsStInterna(ItemImpostoResultado item)
        {
            if (item.CalcularAliquotaIcmsSt && Container.Loja != null)
                item.AliqIcmsSt = ProvedorIcmsProdutoUf
                    .ObterAliquotaIcmsSt(item.Produto, Container.Loja, Container.Fornecedor, Container.Cliente);
        }

        /// <summary>
        /// Carrega a aliquota do FCP.
        /// </summary>
        /// <param name="item"></param>
        private void CarregarAliquotaFcp(ItemImpostoResultado item)
        {
            item.AliqFcp = ProvedorIcmsProdutoUf
                .ObterFCPPorProduto(item.Produto, Container.Loja, Container.Fornecedor, Container.Cliente);
        }

        /// <summary>
        /// Carrega a aliquota do FCP ST.
        /// </summary>
        /// <param name="item"></param>
        private void CarregarAliquotaFcpSt(ItemImpostoResultado item)
        {
            item.AliqFcpSt = ProvedorIcmsProdutoUf
                .ObterAliquotaFCPSTPorProduto(item.Produto, Container.Loja, Container.Fornecedor, Container.Cliente);
        }

        /// <summary>
        /// Carrega a aliquota do PIS.
        /// </summary>
        /// <param name="item"></param>
        private void CarregarAliquotaPis(ItemImpostoResultado item)
        {
            item.AliqPis = Data.NFeUtils.ConfigNFe.AliqPis((uint)Container.Loja.IdLoja);
            item.BcPis = item.Total;
            item.ValorPis = item.BcPis * (decimal)item.AliqPis / 100m;
        }

        /// <summary>
        /// Carrega a aliquota do COFINS.
        /// </summary>
        /// <param name="item"></param>
        private void CarregarAliquotaCofins(ItemImpostoResultado item)
        {
            item.AliqCofins = Data.NFeUtils.ConfigNFe.AliqCofins((uint)Container.Loja.IdLoja);
            item.BcCofins = item.Total;
            item.ValorCofins = item.BcCofins * (decimal)item.AliqCofins / 100m;
        }

        /// <summary>
        /// Calcula o valor do IPI.
        /// </summary>
        /// <param name="item"></param>
        private void CalcularIpi(ItemImpostoResultado item)
        {
            if ((item.NaturezaOperacao?.CalcIpi ?? false) && item.AliqIpi > 0)
            {
                var bcIpi = item.Total;

                if (item.NaturezaOperacao.FreteIntegraBcIpi)
                    bcIpi += item.ValorFrete;

                // É necessário colocar arredondamento pois na nota será arredondado em duas casas decimais,
                // para que o somatório de ipi dos itens fique igual ao total de ipi da nota é necessário 
                // fazer esse arredondamento.
                item.ValorIpi = Math.Round(bcIpi * (decimal)(item.AliqIpi / 100), 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                item.ValorIpi = 0m;
                item.AliqIpi = 0;
            }
        }

        /// <summary>
        /// Calcula o valor do ICMS.
        /// </summary>
        /// <param name="item"></param>
        private void CalcularIcms(ItemImpostoResultado item, decimal percentualDesconto)
        {
            if (!item.CalcularAliquotaIcmsSt)
            {
                item.ValorIcms = 0m;
                item.BcIcms = 0m;
                return;
            }

            // Se o CFOP selecionado estiver marcado para calcular ICMS
            if (item.NaturezaOperacao != null &&
                (item.NaturezaOperacao.CalcIcms || 
                 item.NaturezaOperacao.CalcIcmsSt))
            {
                // Não integra o valor do campo outras despesas na BC ICMS se for nota de devolução
                var naoIncluirOutrasDespBCIcms = (int)Container.FinalidadeEmissao == (int)Data.Model.NotaFiscal.FinalidadeEmissaoEnum.Devolucao;

                /* Chamado 50313. */
                if (!item.Referencia.NaturezaOperacao.CalcIcms && item.Referencia.NaturezaOperacao.CalcIcmsSt)
                    item.AliqIcms = ProvedorIcmsProdutoUf.ObterIcmsPorProduto(item.Produto, Container.Loja, Container.Fornecedor, Container.Cliente);

                //Se for NF de entrada e a natureza estiver marcada para calcular o icms de energia elétrica.
                if (Container.TipoDocumento == Sync.Fiscal.Enumeracao.NFe.TipoDocumento.Entrada && item.NaturezaOperacao.CalcEnergiaEletrica)
                {
                    item.BcIcms = item.Total / (decimal)(1 - item.AliqIcms / 100);
                    item.ValorIcms = item.BcIcms * (decimal)item.AliqIcms / 100;
                }
                else if (Container.Loja.Crt == Data.Model.CrtLoja.LucroReal || 
                         Container.Loja.Crt == Data.Model.CrtLoja.LucroPresumido || Container.TipoDocumento == Sync.Fiscal.Enumeracao.NFe.TipoDocumento.EntradaTerceiros) // Se o emitente for Regime Normal
                {
                    if (item.AliqIcms > 0)
                    {
                        item.BcIcms = item.Total +
                            (Container.ModalidadeFrete == Data.Model.ModalidadeFrete.ContaDoRemetente ? item.ValorFrete : 0)
                            + (naoIncluirOutrasDespBCIcms ? 0 : item.ValorOutrasDespesas)
                            + item.ValorIof + item.ValorDespesaAduaneira - item.ValorDesconto;

                        if (Container.NotaFiscalImportadaSistema)
                            item.BcIcms = item.BcIcms / (decimal)(1 - (item.AliqIcms / 100));

                        // Soma o IPI à base de cálculo, se CFOP estiver marcado para calcular desta forma
                        if (item.NaturezaOperacao.IpiIntegraBcIcms) item.BcIcms += item.ValorIpi;

                        // Se for CST 20: Com redução na BC ICMS
                        // Se for CST 70: Com redução na BC ICMS, considerando o código do valor fiscal = 1
                        if ((item.Cst == Sync.Fiscal.Enumeracao.Cst.CstIcms.ComReducaoDeBaseDeCalculo && item.PercRedBcIcms > 0) || 
                            (item.Cst == Sync.Fiscal.Enumeracao.Cst.CstIcms.ComReducaoDeBaseDeCalculoECobrancaDoIcmsPorSubstituicaoTributaria && 
                             item.PercRedBcIcms > 0 && item.CodValorFiscal == 1))
                            item.BcIcms = item.BcIcms * (decimal)(1 - (item.PercRedBcIcms / 100));

                        // Criado para resolver os chamados 12720, 14223, 14370 e 14646, junto com outra alteração feita logo acima
                        item.BcIcms = Math.Round(item.BcIcms, 2, MidpointRounding.AwayFromZero);

                        // É necessário colocar arredondamento pois na nota será arredondado em duas casas decimais,
                        // para que o somatório de icms dos itens fique igual ao total de icms da nota é necessário 
                        // fazer esse arredondamento.
                        item.ValorIcms = Math.Round(item.BcIcms * (decimal)(item.AliqIcms / 100), 2, MidpointRounding.AwayFromZero);

                        // Se for CST 51: ICMS com diferimento
                        if (item.Cst == Sync.Fiscal.Enumeracao.Cst.CstIcms.Diferimento)
                            item.ValorIcms = Math.Round(item.ValorIcms - (item.ValorIcms * ((decimal)item.PercDiferimento / 100)), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        item.BcIcms = 0;
                        item.ValorIcms = 0;
                    }
                }
                else // Se o emitente for Simples Nacional
                {
                    if (item.AliqIcms > 0)
                    {
                        item.BcIcms = (item.Total +
                            (Container.ModalidadeFrete == Data.Model.ModalidadeFrete.ContaDoRemetente ? item.ValorFrete : 0)
                            + (naoIncluirOutrasDespBCIcms ? 0 : item.ValorOutrasDespesas) + item.ValorIof + item.ValorDespesaAduaneira - (percentualDesconto * item.Total));
                        if (item.NaturezaOperacao.IpiIntegraBcIcms) item.BcIcms += item.ValorIpi;
                        // No Simples Nacional não existe CST e sim CSOSN, necessário verificar qual CSOSN possui redução na BCICMS e ajustar a lógica
                        // Se CST igual a 20 ou 70, calcula redução da BC ICMS.
                        //if ((prodNf.Cst == "20" || prodNf.Cst == "70") && prodNf.PercRedBcIcms > 0)
                        //    prodNf.BcIcms = prodNf.BcIcms * (decimal)(1 - (prodNf.PercRedBcIcms / 100));

                        // É necessário colocar arredondamento pois na nota será arredondado em duas casas decimais,
                        // para que o somatório de icms dos itens fique igual ao total de icms da nota é necessário 
                        // fazer esse arredondamento.
                        item.ValorIcms = Math.Round(item.BcIcms * (decimal)(item.AliqIcms / 100), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        item.BcIcms = 0;
                        item.ValorIcms = 0;
                    }
                }
            }
            else
            {
                item.BcIcms = 0;
                item.ValorIcms = 0;
                item.AliqIcms = 0;
            }
        }

        /// <summary>
        /// Calcula o fundo de combate a pobresa.
        /// </summary>
        /// <param name="item"></param>
        private void CalcularFcp(ItemImpostoResultado item)
        {
            // Se o CFOP selecionado estiver marcado para calcular ICMS
            if (item.NaturezaOperacao != null &&
                (item.NaturezaOperacao.CalcIcms|| item.NaturezaOperacao.CalcIcmsSt))
            {
                if (!item.NaturezaOperacao.CalcIcms && item.NaturezaOperacao.CalcIcmsSt)
                    item.AliqFcp = ProvedorIcmsProdutoUf.ObterFCPPorProduto(item.Produto, Container.Loja, Container.Fornecedor, Container.Cliente);

                if (item.AliqFcp > 0)
                {
                    item.BcFcp = item.BcIcms;
                    item.ValorFcp = Math.Round(item.BcFcp * (decimal)(item.AliqFcp / 100), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    item.BcFcp = 0;
                    item.ValorFcp = 0;
                }
            }
            else
            {
                item.BcFcp = 0;
                item.ValorFcp = 0;
                item.AliqFcp = 0;
            }
        }

        /// <summary>
        /// Calcula o ICMS ST
        /// </summary>
        /// <param name="item"></param>
        private void CalcularIcmsSt(ItemImpostoResultado item)
        {
            if (item.NaturezaOperacao != null && item.NaturezaOperacao.CalcIcmsSt)
            {
                // Só calcula o ST se a alíquota de icms st tiver sido informada
                if (((Container.TipoDocumento == Sync.Fiscal.Enumeracao.NFe.TipoDocumento.EntradaTerceiros || 
                      Container.TipoDocumento == Sync.Fiscal.Enumeracao.NFe.TipoDocumento.Cliente) && item.AliqIcmsSt > 0) || 
                    (Container.TipoDocumento != Sync.Fiscal.Enumeracao.NFe.TipoDocumento.EntradaTerceiros && 
                     Container.TipoDocumento != Sync.Fiscal.Enumeracao.NFe.TipoDocumento.Cliente))
                {
                    using (var sessao = new GDA.GDASession())
                    {
                        // Cria uma instancia do calculo de ICMS ST.
                        var calcIcmsSt = Data.Helper.CalculoIcmsStFactory.ObtemInstancia(sessao, Container.Loja?.IdLoja ?? 0, Container.Cliente?.IdCli,
                            Container.Fornecedor?.IdFornec, item.NaturezaOperacao.IdCfop, ((int?)item.Cst)?.ToString("00"), Container.IdNf, (item.NaturezaOperacao?.CalcIpi ?? false) && item.AliqIpi > 0);
                        // Verifica se a Nota é de saída.
                        bool saida = Container.TipoDocumento == Sync.Fiscal.Enumeracao.NFe.TipoDocumento.Saida ||
                            /* Chamado 32984 e 39660. */
                            (Container.TipoDocumento == Sync.Fiscal.Enumeracao.NFe.TipoDocumento.Entrada &&
                            (item.NaturezaOperacao.Cfop?.VerificaCfopDevolucao() ?? false));

                        // Chamado 15452: Coloquei o round para resolver um problema ao autorizar a nota
                        item.BcIcmsSt = Math.Round(calcIcmsSt.ObtemBaseCalculoIcmsSt(item, saida), 2, MidpointRounding.AwayFromZero);
                        item.AliqIcmsSt = calcIcmsSt.ObtemAliquotaIcmsSt(item, saida);
                        item.ValorIcmsSt = Math.Round(calcIcmsSt.ObtemValorIcmsSt(item, saida), 2, MidpointRounding.AwayFromZero);
                    }
                }

                /* Chamado 50313. */
                if (!item.NaturezaOperacao.CalcIcms)
                {
                    item.BcIcms = 0;
                    item.ValorIcms = 0;
                    item.AliqIcms = 0;
                }
            }
            else
            {
                item.BcIcmsSt = 0;
                item.AliqIcmsSt = 0;
                item.ValorIcmsSt = 0;
            }
        }

        /// <summary>
        /// Calcula o FCP ST.
        /// </summary>
        /// <param name="item"></param>
        private void CalcularFcpSt(ItemImpostoResultado item)
        {
            // Se o CFOP selecionado estiver marcado para calcular ICMS ST
            if (item.NaturezaOperacao != null && item.NaturezaOperacao.CalcIcmsSt)
            {
                if (item.AliqFcpSt > 0)
                {
                    item.BcFcpSt = item.BcIcmsSt;
                    item.ValorFcpSt = Math.Round((item.BcFcpSt * (decimal)(item.AliqFcpSt / 100)), 2, MidpointRounding.AwayFromZero) - item.ValorFcp;
                }
                else
                {
                    item.BcFcpSt = 0;
                    item.ValorFcpSt = 0;
                }

                if (!item.NaturezaOperacao.CalcIcms)
                {
                    item.BcFcp = 0;
                    item.ValorFcp = 0;
                    item.AliqFcp = 0;
                }
            }
            else
            {
                item.BcFcpSt = 0;
                item.ValorFcpSt = 0;
                item.AliqFcpSt = 0;
            }
        }

        /// <summary>
        /// Calcula o PIS.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ipiIntegraBcPISCOFINS"></param>
        private void CalcularPis(ItemImpostoResultado item, bool ipiIntegraBcPISCOFINS)
        {
            if (item.NaturezaOperacao != null && item.NaturezaOperacao.CalcPis)
            {
                if (Container.FinalidadeEmissao != Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao.Complementar && 
                    ((int?)(item.CstPis) < 3 || (int?)(item.CstPis) > 9))
                {
                    item.BcPis = Math.Round(item.Total + (ipiIntegraBcPISCOFINS ? item.ValorIpi : 0) - item.ValorDesconto, 2, MidpointRounding.AwayFromZero);
                    item.AliqPis = item.AliqPis > 0 ? item.AliqPis : Glass.Data.NFeUtils.ConfigNFe.AliqPis((uint)(Container.Loja?.IdLoja ?? 0));
                    item.ValorPis = Math.Round(item.BcPis * ((decimal)item.AliqPis / 100), 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                item.AliqPis = 0;
                item.BcPis = 0;
                item.ValorPis = 0;
            }
        }

        /// <summary>
        /// Calcula o Cofins.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ipiIntegraBcPISCOFINS"></param>
        private void CalcularConfis(ItemImpostoResultado item, bool ipiIntegraBcPISCOFINS)
        {
            if (item.NaturezaOperacao != null && item.NaturezaOperacao.CalcCofins)
            {
                if (Container.FinalidadeEmissao != Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao.Complementar && 
                    ((int?)(item.CstCofins) < 3 || (int?)(item.CstCofins) > 9))
                {
                    item.BcCofins = Math.Round(item.Total + (ipiIntegraBcPISCOFINS ? item.ValorIpi : 0) - item.ValorDesconto, 2, MidpointRounding.AwayFromZero);
                    item.AliqCofins = item.AliqCofins > 0 ? item.AliqCofins : Glass.Data.NFeUtils.ConfigNFe.AliqCofins((uint)Container.Loja.IdLoja);
                    item.ValorCofins = Math.Round(item.BcCofins * ((decimal)item.AliqCofins / 100), 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                item.AliqCofins = 0;
                item.BcCofins = 0;
                item.ValorCofins = 0;
            }
        }
        
        /// <summary>
        /// Calcula os impostos para o item informado.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="percentualDesconto"></param>
        private void Calcular(ItemImpostoResultado item, decimal percentualDesconto)
        {
            CarregarAliquotaIcms(item);
            CarregarAliquotaIcmsStInterna(item);
            CarregarAliquotaFcp(item);
            CarregarAliquotaFcpSt(item);
            CarregarAliquotaPis(item);
            CarregarAliquotaCofins(item);

            // IPI integra BC do Pis se o CST for diferente de 0 e nota de entrada (segundo Julielberty), porém segundo o Higor,
            // deve integrar somente se o ipi não gerar crédito e a empresa destinatária ser do lucro presumido e gerar crédito PIS/COFINS,
            // portanto, a opção foi alterada para ficar assim somente para a Vipal
            var ipiIntegraBcPISCOFINS = 
                FiscalConfig.NotaFiscalConfig.IpiIntegraBcPISCOFINS &&
                _container.TipoDocumento != Sync.Fiscal.Enumeracao.NFe.TipoDocumento.Saida && item.Referencia.CstIpi != 0;

            var qtdItens = Container.Itens.Count();

            item.ValorFrete = Container.ValorFrete / qtdItens;
            item.ValorSeguro = Container.ValorSeguro / qtdItens;
            item.ValorOutrasDespesas = Container.OutrasDespesas / qtdItens;

            // Criado para resolver os chamados 12720, 14223, 14370 e 14646, soma o desconto distribuído entre os produtos, caso sobre um valor de desconto,
            // ajusta no último produto
            //prodNf.ValorDesconto = Math.Round(percDesconto * Math.Round(prodNf.Total, 2), 2);
            /* Chamado 36827.
             * O valor de desconto rateado pelos produtos estava sendo arredondado com duas casas decimais
             * causando diferença entre a base de cálculo do ICMS da nota com a soma da base de cálculo do ICMS dos produtos.
             * Alterei o arredondamento para 6 casas decimais e o valor da base de cálculo ficou correto. */
            //prodNf.ValorDesconto = Math.Round(percDesconto * Math.Round(prodNf.Total, 6), 6);
            /* Chamado 47780. */
            item.ValorDesconto = Math.Round(percentualDesconto * item.Total, 2, MidpointRounding.AwayFromZero);

            _totalDescontoAplicado += item.ValorDesconto;

            if (item.Referencia == Container.Itens.Last() && Math.Abs(Container.ValorDesconto - _totalDescontoAplicado) <= (decimal)0.3)
                item.ValorDesconto += (Container.ValorDesconto - _totalDescontoAplicado);

            // Realiza o calculo dos impostos se a Nota NÃO for de Importação, ou, se for Nota de Ajuste com mais de um produto.
            if (!Container.NotaFiscalImportadaSistema || Container.FinalidadeEmissao == Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao.Ajuste)
            {
                CalcularIpi(item);
                CalcularIcms(item, percentualDesconto);
                CalcularFcp(item);
                CalcularIcmsSt(item);
                CalcularFcpSt(item);
                CalcularPis(item, ipiIntegraBcPISCOFINS);
                CalcularConfis(item, ipiIntegraBcPISCOFINS);
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Realiza o calculo do impostao para o container de itens informado.
        /// </summary>
        /// <param name="container"></param>
        public ICalculoImpostoResultado Calcular(IItemImpostoContainer container)
        {
            _container = container;

            // Calcula o percentual de desconto, considerando todas as casas decimais (TotalProd na nota fiscal salva arredondado)
            // O totalProd foi alterado para recuperar da lista de produtos para ao inserir um produto ele ser considerado.
            var totalProd = container.Itens.Sum(f => f.Total);
            decimal percDesconto = (container.ValorDesconto / (totalProd > 0 ? totalProd : 1));

            var itens = container.Itens.Select(f => new ItemImpostoResultado(f, container.CalcularAliquotaIcmsSt)).ToList();

            _totalDescontoAplicado = 0m;

            foreach (var item in itens)
            {
                // Calcula os impostos do item
                Calcular(item, percDesconto);
            }

            return new Resultado(container, itens);
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Implementação do resultado do calculo.
        /// </summary>
        class Resultado : ICalculoImpostoResultado
        {
            #region Propriedades

            /// <summary>
            /// Container dos itens usados no cálculo.
            /// </summary>
            public IItemImpostoContainer Container { get; }

            /// <summary>
            /// Itens do resultado.
            /// </summary>
            public IEnumerable<IItemCalculoImpostoResultado> Itens { get; }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="container"></param>
            /// <param name="itens"></param>
            public Resultado(IItemImpostoContainer container, IEnumerable<IItemCalculoImpostoResultado> itens)
            {
                Container = container;
                Itens = itens;
            }

            #endregion
        }

        #endregion
    }
}
