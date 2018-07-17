using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.EFD;
using System.Reflection;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.Helper
{
    public sealed class ConfigDAO : Glass.Pool.Singleton<ConfigDAO>
    {
        private ConfigDAO() { }

        #region Recuperação/alteração de valores

        /// <summary>
        /// Retorna o valor de um item do banco de dados.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public object GetValue(Config.ConfigEnum item, uint idLoja)
        {
            Configuracao config = ConfiguracaoDAO.Instance.GetItem(item);
            ConfiguracaoLoja configLoja = ConfiguracaoLojaDAO.Instance.GetItem(item, idLoja);
            if (config == null || configLoja == null)
                return null;

            switch ((Config.TipoConfigEnum)config.Tipo)
            {
                case Config.TipoConfigEnum.Decimal: 
                    return configLoja.ValorDecimal;

                case Config.TipoConfigEnum.Inteiro:
                case Config.TipoConfigEnum.ListaMetodo:
                    return configLoja.ValorInteiro;

                case Config.TipoConfigEnum.Logico:
                    return configLoja.ValorBooleano;

                case Config.TipoConfigEnum.Texto:
                case Config.TipoConfigEnum.TextoCurto:
                case Config.TipoConfigEnum.GrupoEnumMetodo:
                    return configLoja.ValorTexto;

                case Config.TipoConfigEnum.Data:
                    return Conversoes.ConverteData(configLoja.ValorTexto);

                case Config.TipoConfigEnum.Enum:
                    var tipo = Type.GetType(config.NomeTipoEnum, false);
                    return tipo == null ? configLoja.ValorInteiro :
                        (configLoja.ValorInteiro != null ? Enum.Parse(tipo, configLoja.ValorInteiro.ToString()) : null);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Retorna a descrição do item no banco de dados.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetDescricao(Config.ConfigEnum item)
        {
            return ConfiguracaoDAO.Instance.GetItem(item).Descricao;
        }

        /// <summary>
        /// Retorna o tipo de um item no banco de dados.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Config.TipoConfigEnum GetTipo(Config.ConfigEnum item)
        {
            return (Config.TipoConfigEnum)ConfiguracaoDAO.Instance.GetItem(item).Tipo;
        }

        /// <summary>
        /// Altera o valor de uma propriedade no banco de dados.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        /// <returns>True se o valor tiver sido alterado</returns>
        public bool SetValue(Config.ConfigEnum item, uint idLoja, object value)
        {
            /* Chamado 51923. */
            if (!ControleSistema.AmbienteTeste && !UserInfo.GetUserInfo.PodeAlterarConfiguracao)
                throw new Exception("O acesso à esse sistema não foi gerado pelo SAC ou o seu tipo de usuário não possui permissão para alterar configurações " +
                    "do WebGlass, somente visualizá-las. Verifique seu usuário no SAC e logue novamente no WebGlass através do SAC, para que o token seja gerado e " +
                    "a alteração das configurações seja desbloqueada.");

            Configuracao config = ConfiguracaoDAO.Instance.GetItem(item);
            if (config == null)
                return false;

            bool valorAlterado = false;
            ConfiguracaoLoja configLoja = ConfiguracaoLojaDAO.Instance.GetItem(item, idLoja);
            
            if (configLoja == null)
            {
                configLoja = new ConfiguracaoLoja();
                configLoja.IdConfig = config.IdConfig;
            }

            if (config.UsarLoja && configLoja.IdLoja == null)
                configLoja.IdLoja = idLoja;

            switch ((Config.TipoConfigEnum)config.Tipo)
            {
                case Config.TipoConfigEnum.Decimal:
                    var valorDecimal = Glass.Conversoes.StrParaDecimal(value != null ? value.ToString() : "0");
                    valorDecimal = config.PermitirNegativo ? valorDecimal : Math.Abs(valorDecimal);
                    // Antes estava apenas fazendo um cast para "decimal?" não pode ser desta forma porque dá erro
                    valorAlterado = (configLoja.ValorDecimal == null ? 0 : configLoja.ValorDecimal.Value) != valorDecimal;
                    configLoja.ValorDecimal = value != null ? (decimal?)valorDecimal : null;
                    break;

                case Config.TipoConfigEnum.Inteiro:
                case Config.TipoConfigEnum.Enum:
                case Config.TipoConfigEnum.ListaMetodo:
                    var valorInteiro = (int?)value;
                    valorInteiro =  config.PermitirNegativo || valorInteiro == null ? valorInteiro : Math.Abs(valorInteiro.Value);
                    valorAlterado = configLoja.ValorInteiro != (int?)value;
                    configLoja.ValorInteiro = value != null ? valorInteiro : null;
                    break;

                case Config.TipoConfigEnum.Logico:
                    valorAlterado = configLoja.ValorBooleano != (bool)value;
                    configLoja.ValorBooleano = (bool)value;
                    break;

                case Config.TipoConfigEnum.Texto:
                case Config.TipoConfigEnum.TextoCurto:
                case Config.TipoConfigEnum.GrupoEnumMetodo:
                case Config.TipoConfigEnum.Data:
                    valorAlterado = configLoja.ValorTexto != value.ToString();
                    configLoja.ValorTexto = value.ToString();
                    break;
            }

            if (!valorAlterado)
                return false;

            if (ConfiguracaoLojaDAO.Instance.ExisteConfig(configLoja))
                ConfiguracaoLojaDAO.Instance.Update(configLoja);
            else
                ConfiguracaoLojaDAO.Instance.Insert(configLoja);

            Config.RemoveConfigItem(item);
            ValorAlteradoConfig(item);

            return true;
        }

        #endregion

        #region Recupera os itens para um controle do tipo Enum/ListaMetodo

        public Colosoft.WebControls.VirtualObjectDataSource GetForConfig(uint idConfig, params object[] parametros)
        {
            Config.TipoConfigEnum tipo = ConfiguracaoDAO.Instance.ObtemValorCampo<Config.TipoConfigEnum>("tipo", "idConfig=" + idConfig);
            string nomeTipoEnum = ConfiguracaoDAO.Instance.ObtemValorCampo<string>("nomeTipoEnum", "idConfig=" + idConfig);
            string nomeTipoMetodo = ConfiguracaoDAO.Instance.ObtemValorCampo<string>("nomeTipoMetodo", "idConfig=" + idConfig);

            tipo = tipo == Config.TipoConfigEnum.GrupoEnumMetodo && !String.IsNullOrEmpty(nomeTipoEnum) ? Config.TipoConfigEnum.Enum :
                tipo == Config.TipoConfigEnum.GrupoEnumMetodo && !String.IsNullOrEmpty(nomeTipoMetodo) ? Config.TipoConfigEnum.ListaMetodo : tipo;

            string nome = tipo == Config.TipoConfigEnum.Enum ? nomeTipoEnum : nomeTipoMetodo;

            var odsRetorno = new Colosoft.WebControls.VirtualObjectDataSource();
            if (tipo == Config.TipoConfigEnum.Enum)
            {
                odsRetorno.TypeName = "Glass.Data.Helper.ConfigDAO";
                odsRetorno.SelectMethod = "GetListForConfig";

                odsRetorno.SelectParameters.Add(new System.Web.UI.WebControls.Parameter()
                {
                    Name = "type",
                    DefaultValue = nome,
                    Type = TypeCode.String
                });

                odsRetorno.SelectParameters.Add(new System.Web.UI.WebControls.Parameter()
                {
                    Name = "tipoConfig",
                    DefaultValue = ((int)tipo).ToString(),
                    Type = TypeCode.Int32
                });

                if (parametros == null || parametros.Length == 0 || !typeof(System.Web.UI.WebControls.Parameter).IsAssignableFrom(parametros[0].GetType()))
                {
                    odsRetorno.SelectParameters.Add(new System.Web.UI.WebControls.Parameter()
                    {
                        Name = "parametros",
                        Type = TypeCode.Object
                    });

                    odsRetorno.Selecting += delegate(object sender, Colosoft.WebControls.VirtualObjectDataSourceSelectingEventArgs e)
                    {
                        e.InputParameters["parametros"] = parametros != null ? parametros : new object[0]; ;
                    };
                }
                else if (parametros.Length > 0)
                    foreach (System.Web.UI.WebControls.Parameter p in parametros)
                        odsRetorno.SelectParameters.Add(p);
            }
            else
            {
                odsRetorno.TypeName = nome.Split(',')[0].Trim();
                odsRetorno.SelectMethod = nome.Split(',')[1].Trim();

                if (parametros != null && parametros.Length > 0 && typeof(System.Web.UI.WebControls.Parameter).IsAssignableFrom(parametros[0].GetType()))
                    foreach (System.Web.UI.WebControls.Parameter p in parametros)
                        odsRetorno.SelectParameters.Add(p);
            }

            return odsRetorno;
        }

        public IEnumerable<GenericModel> GetListForConfig(uint idConfig, params object[] parametros)
        {
            Config.TipoConfigEnum tipo = ConfiguracaoDAO.Instance.ObtemValorCampo<Config.TipoConfigEnum>("tipo", "idConfig=" + idConfig);
            string nomeTipoEnum = ConfiguracaoDAO.Instance.ObtemValorCampo<string>("nomeTipoEnum", "idConfig=" + idConfig);
            string nomeTipoMetodo = ConfiguracaoDAO.Instance.ObtemValorCampo<string>("nomeTipoMetodo", "idConfig=" + idConfig);

            tipo = tipo == Config.TipoConfigEnum.GrupoEnumMetodo && !String.IsNullOrEmpty(nomeTipoEnum) ? Config.TipoConfigEnum.Enum : 
                tipo == Config.TipoConfigEnum.GrupoEnumMetodo && !String.IsNullOrEmpty(nomeTipoMetodo) ? Config.TipoConfigEnum.ListaMetodo : tipo;

            string nome = tipo == Config.TipoConfigEnum.Enum ? nomeTipoEnum : nomeTipoMetodo;
            return GetListForConfig(nome, (int)tipo, parametros);
        }

        public IEnumerable<GenericModel> GetListForConfig(string type, int tipoConfig, params object[] parametros)
        {
            if (tipoConfig == (int)Config.TipoConfigEnum.Enum)
            {
                Type tipo = Type.GetType(type);
                if (tipo == null)
                    return new GenericModel[0];

                return DataSourcesEFD.Instance.GetFromEnum(tipo, null, false).ToArray();
            }
            else
            {
                Type tipo = Type.GetType(type.Split(',')[0].Trim());
                if (tipo == null)
                    return new GenericModel[0];

                Type[] tiposParametros = new Type[parametros != null ? parametros.Length : 0];
                for (int i = 0; i < tiposParametros.Length; i++)
                    tiposParametros[i] = parametros[i].GetType();

                MethodInfo m = tipo.GetMethod(type.Split(',')[1].Trim(), tiposParametros);
                if (m == null)
                    throw new Exception("Não foi encontrado o método " + type.Split(',')[1].Trim() + ".");

                var tiposValidos = m.ReturnType.GetInterfaces().
                    Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>) && i.GetGenericArguments()[0] == typeof(GenericModel));

                if (tiposValidos.Count() == 0)
                    throw new Exception("O retorno do método " + m.Name + " deve ser do tipo IEnumrable<GenericModel>.");

                object inst = null;
                if (!m.IsStatic)
                {
                    PropertyInfo instance = tipo.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    if (instance != null)
                        inst = instance.GetValue(null, null);
                    else
                        inst = Activator.CreateInstance(tipo);
                }

                return m.Invoke(inst, parametros) as IEnumerable<GenericModel>;
            }
        }

        #endregion

        #region Retorno de itens para a tela de configurações

        /// <summary>
        /// Retorna a lista de itens usados na Comissão
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> GetItensComissao()
        {
            var itens = new List<Config.ConfigEnum>();
            itens.Add(Config.ConfigEnum.DescontarComissaoPerc);
            itens.Add(Config.ConfigEnum.ComissaoPedido);
            itens.Add(Config.ConfigEnum.AlterarPercComissionado);
            itens.Add(Config.ConfigEnum.UsarComissionadoCliente);
            itens.Add(Config.ConfigEnum.ComissaoAlteraValor);
            itens.Add(Config.ConfigEnum.PerComissaoPedido);

            if (PedidoConfig.Comissao.PerComissaoPedido)
            {
                itens.Add(Config.ConfigEnum.UsarComissaoPorTipoPedido);
                itens.Add(Config.ConfigEnum.UsarComissaoPorProduto);
            }

            return ConfiguracaoDAO.Instance.GetItens(itens.ToArray());
        }

        /// <summary>
        /// Retorna a lista de itens usados na Rentabilidade.
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> GetItensRentabilidade()
        {
            var itens = new List<Config.ConfigEnum>()
            {
                Config.ConfigEnum.CalcularRentabilidade,
                Config.ConfigEnum.ControlarFaixaRentabilidadeLiberacao
            };

            return ConfiguracaoDAO.Instance.GetItens(itens.ToArray());
        }

        /// <summary>
        /// Retorna a lista de itens usados no financeiro
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> GetItensFinanceiro()
        {
            var itens = new List<Config.ConfigEnum>();

            // Cartão
            itens.Add(Config.ConfigEnum.TaxaJurosConstrucard);
            itens.Add(Config.ConfigEnum.CobrarJurosCartaoCliente);
            itens.Add(Config.ConfigEnum.PedidoJurosCartao);
            itens.Add(Config.ConfigEnum.QuantidadeDiasSomarDataMovimentacaoBancariaCartaoCredito);
            itens.Add(Config.ConfigEnum.QuitarParcCartaoDebito);
            itens.Add(Config.ConfigEnum.QuantidadeDiasSomarDataMovimentacaoBancariaCartaoDebito);
            itens.Add(Config.ConfigEnum.CartaoMovimentaCxGeralDiario);
            itens.Add(Config.ConfigEnum.NumeroParcelasCompra);
            itens.Add(Config.ConfigEnum.PlanoContaBloquearEntradaSaida);
            itens.Add(Config.ConfigEnum.ControlarCreditoFornecedor);
            itens.Add(Config.ConfigEnum.NumeroParcelasRenegociar);
            itens.Add(Config.ConfigEnum.GerarCreditoFormasPagto);
            itens.Add(Config.ConfigEnum.NumeroViasNotaPromissoria);
            itens.Add(Config.ConfigEnum.SepararTiposChequesRecebimento);
            itens.Add(Config.ConfigEnum.NumeroFormasPagtoContasPagar);
            itens.Add(Config.ConfigEnum.BloquearChequesDataRetroativa);
            itens.Add(Config.ConfigEnum.BloquearChequesDigitoVerificador);
            itens.Add(Config.ConfigEnum.NumeroDiasImpedirGerarCreditoCheque);
            itens.Add(Config.ConfigEnum.PeriodoInativarClienteUltimaCompra);
            itens.Add(Config.ConfigEnum.PeriodoInativarClienteUltimaConsultaSintegra);
            itens.Add(Config.ConfigEnum.NumeroDiasContaRecAtrasada);
            itens.Add(Config.ConfigEnum.CompraCalcMult5);
            itens.Add(Config.ConfigEnum.CompraSemValores);
            itens.Add(Config.ConfigEnum.EntradaEstoqueManual);
            itens.Add(Config.ConfigEnum.UsarPgtoAntecipFornec);

            if (FinanceiroConfig.UsarPgtoAntecipFornec)
                itens.Add(Config.ConfigEnum.TipoUsoAntecipacaoFornecedor);
            
            itens.Add(Config.ConfigEnum.UsarControleCobrancaEmail);
            if (FinanceiroConfig.UsarControleCobrancaEmail)
            {
                itens.Add(Config.ConfigEnum.NumDiasAnteriorVencContaRecEnviarEmailCli);
                itens.Add(Config.ConfigEnum.NumDiasAposVencContaRecEnviarEmailCli);
            }

            if (!PedidoConfig.LiberarPedido)
                itens.Add(Config.ConfigEnum.SaidaEstoqueManual);
            
            itens.Add(Config.ConfigEnum.LimitePadraoCliente);
            itens.Add(Config.ConfigEnum.ExibirDescricaoParcelaLiberacao);
            itens.Add(Config.ConfigEnum.EmpresaConsideraChequeLimite);
            itens.Add(Config.ConfigEnum.ConsiderarChequeDepositadoVencidoNoLimite);
            itens.Add(Config.ConfigEnum.EmpresaConsideraPedidoConferidoLimite);
            itens.Add(Config.ConfigEnum.EmpresaConsideraPedidoAtivoLimite);
            itens.Add(Config.ConfigEnum.SepararValoresFiscaisEReaisContasReceber);
            itens.Add(Config.ConfigEnum.SepararValoresFiscaisEReaisContasPagar);
            itens.Add(Config.ConfigEnum.BloquearEmissaoPedidoLimiteExcedido);
            itens.Add(Config.ConfigEnum.OpcaoPagtoAntecipadoPadraoMarcada);
            itens.Add(Config.ConfigEnum.PercMinimoSinalPedidoPadrao);
            itens.Add(Config.ConfigEnum.UsarControleFinalizacaoCompra);
            itens.Add(Config.ConfigEnum.LimitarChequesPorCpfOuCnpj);
            
            itens.Add(Config.ConfigEnum.PermitirConfirmacaoPedidoPeloFinanceiro);
            if (FinanceiroConfig.PermitirConfirmacaoPedidoPeloFinanceiro)
                itens.Add(Config.ConfigEnum.PerguntarVendedorConfirmacaoFinanceiro);
            itens.Add(Config.ConfigEnum.PermitirFinalizacaoPedidoPeloFinanceiro);
            if (FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro)
                itens.Add(Config.ConfigEnum.PerguntarVendedorFinalizacaoFinanceiro);
            if (FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro || FinanceiroConfig.PermitirConfirmacaoPedidoPeloFinanceiro)
                itens.Add(Config.ConfigEnum.ClienteInativoBloqueadoEmitirPedidoComAutorizacaoFinanceiro);

            if (!FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar && !FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
                itens.Add(Config.ConfigEnum.PermitirApenasContasMesmoTipoEncontroContas);

            itens.Add(Config.ConfigEnum.EnviarEmailAdministradorDescontoMaior);

            itens.Add(Config.ConfigEnum.AdministradorEnviarEmailSmsMensagemPrecoProdutoAlterado);

            itens.Add(Config.ConfigEnum.PermitirTrocaPorPedido);
            itens.Add(Config.ConfigEnum.UsarTipoCalculoNfParaCompra);
            itens.Add(Config.ConfigEnum.NumeroDiasIgnorarClientesRecemAtivosInativarAutomaticamente);
            itens.Add(Config.ConfigEnum.ExibirCnab);            
            itens.Add(Config.ConfigEnum.CobrarPedidoReposicao);
            itens.Add(Config.ConfigEnum.ManterDescontoAdministrador);
            itens.Add(Config.ConfigEnum.BloquearClienteAoDevolverProtestarCheque);
            itens.Add(Config.ConfigEnum.UsarControleCentroCusto);
            itens.Add(Config.ConfigEnum.ComissaoPorContasRecebidas);

            itens.Add(Config.ConfigEnum.TempoAlertaFaturamentoInoperante);
            itens.Add(Config.ConfigEnum.PermitirLiberacaoPedidosLojasDiferentes);

            if (!Geral.SistemaLite)
                itens.Add(Config.ConfigEnum.ControleCaixaDiario);

            itens.Add(Config.ConfigEnum.NumeroDiasChequeVistaLiberarPedido);

            return ConfiguracaoDAO.Instance.GetItens(itens.ToArray());
        }

        /// <summary>
        /// Retorna a lista de itens usados no orçamento.
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> GetItensOrcamento()
        {
            var itens = new List<Config.ConfigEnum>();
            itens.Add(Config.ConfigEnum.DescontoMaximoOrcamento);
            itens.Add(Config.ConfigEnum.FormaPagtoOrcamento);
            itens.Add(Config.ConfigEnum.PrazoEntregaOrcamento);
            itens.Add(Config.ConfigEnum.ValidadeOrcamento);
            itens.Add(Config.ConfigEnum.SempreInserirItensProdutosOrcamento);
            itens.Add(Config.ConfigEnum.DescontoAcrescimoItensOrcamento);

            itens.Add(Config.ConfigEnum.ExibirItensProdutosRelatorio);
            itens.Add(Config.ConfigEnum.NegociarParcialmente);
            itens.Add(Config.ConfigEnum.ListaApenasOrcamentosVendedor);

            itens.Add(Config.ConfigEnum.PermitirInserirClienteInativoBloqueado);

            itens.Add(Config.ConfigEnum.PermirtirSalvarOrcamentoSemProcAplic);

            return ConfiguracaoDAO.Instance.GetItens(itens.ToArray());
        }

        /// <summary>
        /// Retorna a lista de itens usados no pedido.
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> GetItensPedido()
        {
            var itens = new List<Config.ConfigEnum>();
            
            itens.Add(Config.ConfigEnum.AmbientePedido);
            if (PedidoConfig.DadosPedido.AmbientePedido)
                itens.Add(Config.ConfigEnum.UsarAmbienteInstalacao);

            itens.Add(Config.ConfigEnum.UsarControleNovoObra);
            itens.Add(Config.ConfigEnum.DescontoMaximoPedidoAVista);
            itens.Add(Config.ConfigEnum.DescontoMaximoPedidoAPrazo);
            itens.Add(Config.ConfigEnum.DescontoMaximoPedidoAVistaGerente);
            itens.Add(Config.ConfigEnum.DescontoMaximoPedidoAPrazoGerente);
            itens.Add(Config.ConfigEnum.DescontoPorProduto);
            itens.Add(Config.ConfigEnum.CobrarMedidasExatasPedido);

            if (PedidoConfig.LiberarPedido)
                itens.Add(Config.ConfigEnum.DescontoPadraoPedidoAVista);

            itens.Add(Config.ConfigEnum.FastDelivery);
            if (PedidoConfig.Pedido_FastDelivery.FastDelivery)
            {
                itens.Add(Config.ConfigEnum.M2MaximoFastDelivery);
                itens.Add(Config.ConfigEnum.PrazoFastDelivery);
                itens.Add(Config.ConfigEnum.TaxaFastDelivery);
            }

            itens.Add(Config.ConfigEnum.AgruparBenefRelatorio);
            itens.Add(Config.ConfigEnum.ExibirM2CalcRelatorio);
            itens.Add(Config.ConfigEnum.BloqueioPedidoMaoDeObra);
            itens.Add(Config.ConfigEnum.NumeroFormasPagto);
            itens.Add(Config.ConfigEnum.NumParcelasPedido);
            itens.Add(Config.ConfigEnum.CalcularAreaMinimaApenasVidroBeneficiado);
            
            itens.Add(Config.ConfigEnum.UsarTamanhoMaximoVidro);
            if (PedidoConfig.TamanhoVidro.UsarTamanhoMaximoVidro)
            {
                itens.Add(Config.ConfigEnum.AlturaMaximaVidro);
                itens.Add(Config.ConfigEnum.LarguraMaximaVidro);
            }

            itens.Add(Config.ConfigEnum.AlturaELarguraMinimasParaPecasTemperadas);
            itens.Add(Config.ConfigEnum.AlturaELarguraMinimaParaPecasComBisote);
            itens.Add(Config.ConfigEnum.AlturaELarguraMinimaParaPecasComLapidacao);

            itens.Add(Config.ConfigEnum.NumeroDiasUteisDataEntregaPedido);
            itens.Add(Config.ConfigEnum.NumeroDiasUteisDataEntregaPedidoRevenda);
            itens.Add(Config.ConfigEnum.NumeroDiasUteisDataEntregaPedidoMaoDeObra);
            itens.Add(Config.ConfigEnum.BloquearDataEntregaPedidoVendedor);
            itens.Add(Config.ConfigEnum.DiasMinimosEntregaTipo);
            itens.Add(Config.ConfigEnum.ExibirDatasParcelasPedido);
            itens.Add(Config.ConfigEnum.BloquearDadosClientePedido);          

            itens.Add(Config.ConfigEnum.DescontoPedidoApenasAVista);
            if (PedidoConfig.Desconto.DescontoPedidoApenasAVista)
                itens.Add(Config.ConfigEnum.DescontoPedidoUmaParcela);

            itens.Add(Config.ConfigEnum.ImpedirDescontoSomativo);
            itens.Add(Config.ConfigEnum.ImpedirConfirmacaoPedidoPagamento);
            itens.Add(Config.ConfigEnum.TipoEntregaPadraoPedido);
            itens.Add(Config.ConfigEnum.AlterarValorUnitarioProduto);      
            itens.Add(Config.ConfigEnum.BloquearItensTipoPedido);

            if (PedidoConfig.DadosPedido.BloquearItensTipoPedido)
                itens.Add(Config.ConfigEnum.BloquearItensCorEspessura);

            itens.Add(Config.ConfigEnum.ExibePopupVidrosEstoque);
            itens.Add(Config.ConfigEnum.TipoNomeExibirRelatorioPedido);
            itens.Add(Config.ConfigEnum.ExibirItensProdutosPedido);
            itens.Add(Config.ConfigEnum.AlterarUrlWebGlassFornec);
            itens.Add(Config.ConfigEnum.BuscarVendedorEmitirPedido);
            itens.Add(Config.ConfigEnum.ExibirBotoesConfirmacaoPedido);
            itens.Add(Config.ConfigEnum.ObrigarInformarPedidoCliente);

            itens.Add(Config.ConfigEnum.ListaApenasPedidosVendedor);
            itens.Add(Config.ConfigEnum.CodigoClienteUsado);
            itens.Add(Config.ConfigEnum.ExibirValorProdutosInstalacao);
            itens.Add(Config.ConfigEnum.GerarInstalacaoManual);
            itens.Add(Config.ConfigEnum.GerarInstalacaoAutomaticamente);

            if (!Geral.SistemaLite)
            {
                itens.Add(Config.ConfigEnum.TipoControleReposicao);
                itens.Add(Config.ConfigEnum.ObrigarProcAplVidros);
                itens.Add(Config.ConfigEnum.NumeroDiasPedidoProntoAtrasado);
            }
            
            if (!FiscalConfig.NotaFiscalConfig.PercentualCargaTributariaParaClienteSimplesNacional.HasValue)
            {
                itens.Add(Config.ConfigEnum.CalcularIcmsPedido);
                itens.Add(Config.ConfigEnum.CalcularIpiPedido);
            }

            itens.Add(Config.ConfigEnum.InserirImagemProjetoPedido);
            itens.Add(Config.ConfigEnum.BloqEmisPedidoPorPosicaoMateriaPrima);
            itens.Add(Config.ConfigEnum.UsarM2CalcBeneficiamentos);
            itens.Add(Config.ConfigEnum.CalcularMultiplo10);
            itens.Add(Config.ConfigEnum.ManterCodInternoCampoAoInserirProduto);

            itens.Add(Config.ConfigEnum.TempoAlertaComercialInoperante);
            itens.Add(Config.ConfigEnum.GerarPedidoProducaoCorte);

            itens.Add(Config.ConfigEnum.LimiteDiarioMedicoes);
            itens.Add(Config.ConfigEnum.AdicionalVidroRedondoAte12mm);
            itens.Add(Config.ConfigEnum.AdicionalVidroRedondoAcima12mm);
            itens.Add(Config.ConfigEnum.PermitirApenasPedidosDeVendaNoEcommerce);

            return ConfiguracaoDAO.Instance.GetItens(itens.ToArray());
        }

        /// <summary>
        /// Retorna a lista de itens usados na liberação de pedido.
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> GetItensLiberarPedido()
        {
            if (!PedidoConfig.LiberarPedido)
                return new Configuracao[0];

            var itens = new List<Config.ConfigEnum>();
            
            itens.Add(Config.ConfigEnum.ApenasAdminCancelaLiberacao);
            itens.Add(Config.ConfigEnum.BloquearLiberacaoDadosPedido);
            itens.Add(Config.ConfigEnum.DescontoLiberarPedido);
            itens.Add(Config.ConfigEnum.DescontoMaximoLiberacao);
            itens.Add(Config.ConfigEnum.UsarMenorPrazoLiberarPedido);
            itens.Add(Config.ConfigEnum.UsarRelatorioLiberacao4Vias);
            itens.Add(Config.ConfigEnum.AgruparResumoLiberacaoProduto);

            if (!FiscalConfig.NotaFiscalConfig.PercentualCargaTributariaParaClienteSimplesNacional.HasValue)
            {
                itens.Add(Config.ConfigEnum.CalcularIcmsLiberacao);
                itens.Add(Config.ConfigEnum.CalcularIpiLiberacao);
            }

            itens.Add(Config.ConfigEnum.SaidaEstoqueAoLiberarPedido);

            if (Liberacao.DadosLiberacao.LiberarPedidoProdutos)
                itens.Add(Config.ConfigEnum.LiberarPedidoAtrasadoParcialmente);

            if (!OrdemCargaConfig.UsarControleOrdemCarga)
            {
                itens.Add(Config.ConfigEnum.LiberacaoParcial);
                itens.Add(Config.ConfigEnum.SaidaEstoqueBoxLiberar);
            }
            else
            {
                itens.Add(Config.ConfigEnum.SaidaEstoqueVolume);
                if (OrdemCargaConfig.UsarOrdemCargaParcial)
                    itens.Add(Config.ConfigEnum.LiberacaoParcial);
            }

            itens.Add(Config.ConfigEnum.LiberarProdutosProntos);
            itens.Add(Config.ConfigEnum.LiberarClienteRotaSemEstarPronto);

            itens.Add(Config.ConfigEnum.NumeroViasAlmoxarifeLiberacao);
            itens.Add(Config.ConfigEnum.NumeroViasExpedicaoLiberacao);
            itens.Add(Config.ConfigEnum.EnviarEmailAoLiberarPedido);

            if (Liberacao.EnviarEmailAoLiberarPedido)
                itens.Add(Config.ConfigEnum.ExibirApenasViaClienteNoEnvioEmail);

            itens.Add(Config.ConfigEnum.ExibirApenasViaExpAlmPedidosEntrega);
            itens.Add(Config.ConfigEnum.ExibirApenasViaExpAlmPedidosBalcao);

            itens.Add(Config.ConfigEnum.ImpedirLiberacaoPedidoSemPCP);
            itens.Add(Config.ConfigEnum.PrazoMaxDiaUtilRealizarTrocaDev);

            return ConfiguracaoDAO.Instance.GetItens(itens.ToArray());
        }

        /// <summary>
        /// Retorna a lista de itens usados no projeto.
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> GetItensProjeto()
        {
            var itens = new List<Config.ConfigEnum>();
            itens.Add(Config.ConfigEnum.CobrarTranspasse);
            itens.Add(Config.ConfigEnum.ProcessoCaixilho);
            itens.Add(Config.ConfigEnum.AplicacaoCaixilho);
            itens.Add(Config.ConfigEnum.ObrigatorioCorAlumFerragem);
            itens.Add(Config.ConfigEnum.ApenasVidrosPadrao);
            itens.Add(Config.ConfigEnum.UtilizarEditorImagensProjeto);
            itens.Add(Config.ConfigEnum.ValidacaoProjetoConfiguravel);
            itens.Add(Config.ConfigEnum.ExibirCADecommerce);

            if (!Glass.Configuracoes.Geral.SistemaLite)
            {
                itens.Add(Config.ConfigEnum.ProcessoInstalacao);
                itens.Add(Config.ConfigEnum.AplicacaoInstalacao);
            }

            itens.Add(Config.ConfigEnum.MedidaExataPadrao);

            return ConfiguracaoDAO.Instance.GetItens(itens.ToArray());
        }

        /// <summary>
        /// Retorna a lista de itens usados na NFe.
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> GetItensNFe()
        {
            var itens = new List<Config.ConfigEnum>();
            itens.Add(Config.ConfigEnum.NFeModoProducao);
            itens.Add(Config.ConfigEnum.AliquotaICMSSimplesNacional);
            itens.Add(Config.ConfigEnum.SIntegraRegistro50);
            itens.Add(Config.ConfigEnum.SIntegraRegistro51);
            itens.Add(Config.ConfigEnum.SIntegraRegistro53);
            itens.Add(Config.ConfigEnum.SIntegraRegistro54);
            itens.Add(Config.ConfigEnum.SIntegraRegistro61);
            itens.Add(Config.ConfigEnum.SIntegraRegistro70);
            itens.Add(Config.ConfigEnum.SIntegraRegistro74);
            itens.Add(Config.ConfigEnum.SIntegraRegistro75);
            itens.Add(Config.ConfigEnum.AgruparProdutosGerarNFe);

            if (!FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
                itens.Add(Config.ConfigEnum.GerarNotaApenasDeLiberacao);

            itens.Add(Config.ConfigEnum.InformarPedidoNaNfe);
            itens.Add(Config.ConfigEnum.InformarOrcamentoNaNFe);
            itens.Add(Config.ConfigEnum.InformarFormaPagtoNaNFe);
            itens.Add(Config.ConfigEnum.InformarPedidoClienteNaNFe);
            itens.Add(Config.ConfigEnum.InformarPedCliNoProdutoNFe);
            itens.Add(Config.ConfigEnum.NumeroParcelasNFe);
            itens.Add(Config.ConfigEnum.ContingenciaNFe);
            itens.Add(Config.ConfigEnum.JustificativaContingenciaNFe);
            itens.Add(Config.ConfigEnum.PeriodoApuracaoIpi);
            itens.Add(Config.ConfigEnum.GerarEFD);
            itens.Add(Config.ConfigEnum.CalculoAliquotaIcmsSt);
            itens.Add(Config.ConfigEnum.RatearIpiNfPedido);
            itens.Add(Config.ConfigEnum.AliquotaIcmsStRatearIpiNfPedido);
            itens.Add(Config.ConfigEnum.RatearIcmsStNfPedido);
            itens.Add(Config.ConfigEnum.IndicadorIncidenciaTributaria);
            itens.Add(Config.ConfigEnum.MetodoApropriacaoCreditos);
            itens.Add(Config.ConfigEnum.TipoContribuicaoApurada);
            itens.Add(Config.ConfigEnum.TipoContribuicaoSocialPadrao);
            itens.Add(Config.ConfigEnum.TipoNotaBuscarContribuicaoSocialPadrao);
            itens.Add(Config.ConfigEnum.TipoCreditoPadrao);
            itens.Add(Config.ConfigEnum.TipoNotaBuscarCreditoPadrao);
            itens.Add(Config.ConfigEnum.TipoControleSaldoCreditoIcms);
            itens.Add(Config.ConfigEnum.CodigoAjusteAproveitamentoCreditoIcms);

            if (FiscalConfig.TipoControleSaldoCreditoIcms == Sync.Fiscal.EFD.DataSources.TipoControleSaldoCreditoIcms.CreditoUtilizacaoLimitada)
                itens.Add(Config.ConfigEnum.PercAproveitamentoCreditoIcms);

            itens.Add(Config.ConfigEnum.PerfilArquivoEfdFiscal);
            itens.Add(Config.ConfigEnum.UtilizaFCI);

            itens.Add(Config.ConfigEnum.CTeModoProducao);
            itens.Add(Config.ConfigEnum.ContingenciaCTe);
            itens.Add(Config.ConfigEnum.JustificativaContingenciaCTe);

            itens.Add(Config.ConfigEnum.MDFeModoProducao);
            itens.Add(Config.ConfigEnum.ContingenciaMDFe);
            itens.Add(Config.ConfigEnum.JustificativaContingenciaMDFe);

            itens.Add(Config.ConfigEnum.BloquearMaisDeUmaNfeParaUmPedido);

            if (PedidoConfig.LiberarPedido)
                itens.Add(Config.ConfigEnum.BloquearEmissaoNFeApenasPedidosLiberados);
            
            itens.Add(Config.ConfigEnum.AliquotaPis);
            itens.Add(Config.ConfigEnum.AliquotaCofins);

            itens.Add(Config.ConfigEnum.UtilizaNFCe);
            itens.Add(Config.ConfigEnum.PercentualFundoPobreza);
            itens.Add(Config.ConfigEnum.ConsiderarM2CalcNotaFiscal);
            itens.Add(Config.ConfigEnum.PermitirEmitirNotaParaClienteBloqueadoOuInativo);

            return ConfiguracaoDAO.Instance.GetItens(itens.ToArray());
        }

        /// <summary>
        /// Retorna a lista de itens usados no PCP.
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> GetItensPCP()
        {
            var itens = new List<Config.ConfigEnum>();
            itens.Add(Config.ConfigEnum.UsarConferenciaFluxo);
            itens.Add(Config.ConfigEnum.GerarArquivoMesaCorte);
            
            if (PedidoConfig.LiberarPedido)
                itens.Add(Config.ConfigEnum.EnviarEmailPedidoConfirmado);
            
            if (Geral.ControlePCP)
                itens.Add(Config.ConfigEnum.EnviarEmailPedidoPronto);
            
            if (PedidoConfig.LiberarPedido || PCPConfig.ControlarProducao)
                itens.Add(Config.ConfigEnum.EnviarPedidoAnexoEmail);

            if (PCPConfig.ControlarProducao)
                itens.Add(Config.ConfigEnum.EnviarSMSPedidoPronto);

            if (PedidoConfig.LiberarPedido)
            {
                itens.Add(Config.ConfigEnum.EnviarSMSAdministrador);
                itens.Add(Config.ConfigEnum.EnviarEmailAdministrador);
            }

            itens.Add(Config.ConfigEnum.UsarPlanoCorte);
            if (PCPConfig.Etiqueta.UsarPlanoCorte)
                itens.Add(Config.ConfigEnum.TipoExportacaoEtiqueta);

            itens.Add(Config.ConfigEnum.TipoDataEtiqueta);
            itens.Add(Config.ConfigEnum.DiasDataFabrica);
            itens.Add(Config.ConfigEnum.ExibirDadosPcpListaAposConferencia);
            itens.Add(Config.ConfigEnum.ExibirImpressaoPcpListaPedidos);
            itens.Add(Config.ConfigEnum.GerarOrcamentoFerragesAluminiosPCP);
            itens.Add(Config.ConfigEnum.ObrigarLeituraSetorImpedirAvanco);
            itens.Add(Config.ConfigEnum.UtilizarSequenciaRoteiroProducao);
            itens.Add(Config.ConfigEnum.FolgaRetalho);

            if (PCPConfig.ControlarProducao)
            {
                itens.Add(Config.ConfigEnum.UsarControleChapaCorte);
                itens.Add(Config.ConfigEnum.UsarControleRetalhos);
                itens.Add(Config.ConfigEnum.ConsiderarMetaProducaoM2PecasPorDataFabrica);
                itens.Add(Config.ConfigEnum.BloquearLeituraPlanoCorteLoteGenerico);
                itens.Add(Config.ConfigEnum.BloquearLeituraPecaLoteGenerico);

                if (!PCPConfig.ConsiderarMetaProducaoM2PecasPorDataFabrica)
                    itens.Add(Config.ConfigEnum.MetaProducaoDiaria);
                else if (PCPConfig.MetaProducaoDiaria > 0)
                    ConfigDAO.Instance.SetValue(Config.ConfigEnum.MetaProducaoDiaria, 0, null);
            }
            
            itens.Add(Config.ConfigEnum.BloquearExpedicaoApenasPecasProntas);
            itens.Add(Config.ConfigEnum.UsarControleGerenciamentoProjCnc);
            itens.Add(Config.ConfigEnum.ConcatenarEspAltLargAoNumEtiqueta);
            itens.Add(Config.ConfigEnum.GerarMarcacaoPecaReposta);
            itens.Add(Config.ConfigEnum.GerarDxf);
            itens.Add(Config.ConfigEnum.GerarFml);
            itens.Add(Config.ConfigEnum.GerarSGlass);
            itens.Add(Config.ConfigEnum.ImpedirLeituraChapaComPlanoCorteVinculado);
            itens.Add(Config.ConfigEnum.ImpedirLeituraTodasPecasPedido);
            itens.Add(Config.ConfigEnum.EnviarEmailPedidoConfirmadoVendedor);
            itens.Add(Config.ConfigEnum.HabilitarFaturamentoCarregamento);
            itens.Add(Config.ConfigEnum.ControleCavalete);
            itens.Add(Config.ConfigEnum.GerenciamentoFornada);
            itens.Add(Config.ConfigEnum.HabilitarOtimizacaoAluminio);

            if (PCPConfig.HabilitarControleOtimizacaoAluminio)
            {
                itens.Add(Config.ConfigEnum.ArestaBarraAluminioOtimizacao);
                itens.Add(Config.ConfigEnum.ArestaGrau45AluminioOtimizacao);
                itens.Add(Config.ConfigEnum.ArestaGrau90AluminioOtimizacao);
                itens.Add(Config.ConfigEnum.AcrescimoBarraAluminioOtimizacaoProjetoTemperado);
            }

            return ConfiguracaoDAO.Instance.GetItens(itens.ToArray());
        }

        /// <summary>
        /// Retorna a lista de itens usados na aba Geral.
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> GetItensGeral()
        {
            var itens = new List<Config.ConfigEnum>();
            itens.Add(Config.ConfigEnum.NumeroCasasDecimaisTotM);
            itens.Add(Config.ConfigEnum.UsarBeneficiamentosTodosOsGrupos);
            itens.Add(Config.ConfigEnum.FuncVisualizaDadosApenasSuaLoja);
            itens.Add(Config.ConfigEnum.NaoExigirEnderecoConsumidorFinal);
            itens.Add(Config.ConfigEnum.ControleMedicao);
            itens.Add(Config.ConfigEnum.ControleInstalacao);

            if (!Geral.SistemaLite)
            {
                itens.Add(Config.ConfigEnum.ControlePCP);

                if (PCPConfig.ControlarProducao)
                {
                    itens.Add(Config.ConfigEnum.UsarControleOrdemCarga);

                    if (OrdemCargaConfig.UsarControleOrdemCarga)
                        itens.Add(Config.ConfigEnum.OrdemCargaParcial);
                }

                itens.Add(Config.ConfigEnum.ControlarEstoqueVidrosClientes);
                itens.Add(Config.ConfigEnum.HorariosEnvioEmailSmsAdmin);
            }

            itens.Add(Config.ConfigEnum.GerarVolumeApenasDePedidosEntrega);
            itens.Add(Config.ConfigEnum.ExibirNomeFantasiaClienteRptCarregamento);
            itens.Add(Config.ConfigEnum.ExibirRazaoSocialClienteVolume);

            return ConfiguracaoDAO.Instance.GetItens(itens.ToArray());
        }

        /// <summary>
        /// Retorna a lista de itens de configurações internas.
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> GetItensInternas()
        {
            return ConfiguracaoDAO.Instance.ObterInternas();
        }

        #endregion

        /// <summary>
        /// Função executada quando o item da configuração é salvo.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="valor"></param>
        private void ValorAlteradoConfig(Config.ConfigEnum item)
        {
            if (item == Config.ConfigEnum.ExibirM2CalcRelatorio)
                PedidoDAO.Instance.AtualizaTotM(0);

            else if (item == Config.ConfigEnum.NumeroCasasDecimaisTotM)
            {
                string sql = @"alter table produtos_orcamento change TOTM TOTM float({0},{1});
                    alter table produtos_orcamento change TOTMCALC TOTMCALC float({0},{1});
                    alter table produtos_pedido change TOTM TOTM float({0},{1});
                    alter table produtos_pedido change TOTM2CALC TOTM2CALC float({0},{1});
                    alter table produtos_pedido_espelho change TOTM TOTM float({0},{1});
                    alter table produtos_pedido_espelho change TOTM2CALC TOTM2CALC float({0},{1});
                    alter table pedido change TOTM TOTM float({0},{1});
                    alter table pedido_espelho change TOTM TOTM float({0},{1});
                    alter table material_item_projeto change TOTM TOTM float({0},{1});
                    alter table material_item_projeto change TOTM2CALC TOTM2CALC float({0},{1});
                    alter table mov_estoque change QTDEMOV QTDEMOV decimal({0},{1});
                    alter table mov_estoque_cliente change QTDEMOV QTDEMOV decimal({0},{1});
                    alter table mov_estoque_fiscal change QTDEMOV QTDEMOV decimal({0},{1});
                    alter table mov_estoque change SALDOQTDEMOV SALDOQTDEMOV decimal({0},{1});
                    alter table mov_estoque_cliente change SALDOQTDEMOV SALDOQTDEMOV decimal({0},{1});
                    alter table mov_estoque_fiscal change SALDOQTDEMOV SALDOQTDEMOV decimal({0},{1});";

                ConfiguracaoDAO.Instance.ExecuteScalar<int>(String.Format(sql, 10 + Geral.NumeroCasasDecimaisTotM, 
                    Geral.NumeroCasasDecimaisTotM));
            }
            else if(item == Config.ConfigEnum.PermitirLiberacaoPedidosLojasDiferentes)
            {
                if(FinanceiroConfig.DadosLiberacao.PermitirLiberacaoPedidosLojasDiferentes)
                    ConfiguracaoDAO.Instance.ExecuteScalar<int>("UPDATE loja SET IgnorarLiberarApenasProdutosProntos = 0");
            }
            else if (item == Config.ConfigEnum.CalcularIcmsPedido)
            {
                if (PedidoConfig.Impostos.CalcularIcmsPedido)
                    ConfiguracaoDAO.Instance.ExecuteScalar<int>("UPDATE loja SET CalcularIcmsPedido = 1");
                else
                    ConfiguracaoDAO.Instance.ExecuteScalar<int>("UPDATE loja SET CalcularIcmsPedido = 0");
            }
            else if (item == Config.ConfigEnum.CalcularIpiPedido)
            {
                if (PedidoConfig.Impostos.CalcularIpiPedido)
                    ConfiguracaoDAO.Instance.ExecuteScalar<int>("UPDATE loja SET CalcularIpiPedido = 1");
                else
                    ConfiguracaoDAO.Instance.ExecuteScalar<int>("UPDATE loja SET CalcularIpiPedido = 0");
            }
            else if (item == Config.ConfigEnum.CalcularIcmsLiberacao)
            {
                if (Liberacao.Impostos.CalcularIcmsLiberacao)
                    ConfiguracaoDAO.Instance.ExecuteScalar<int>("UPDATE loja SET CalcularIcmsLiberacao = 1");
                else
                    ConfiguracaoDAO.Instance.ExecuteScalar<int>("UPDATE loja SET CalcularIcmsLiberacao = 0");
            }
            else if (item == Config.ConfigEnum.CalcularIpiLiberacao)
            {
                if (Liberacao.Impostos.CalcularIpiLiberacao)
                    ConfiguracaoDAO.Instance.ExecuteScalar<int>("UPDATE loja SET CalcularIpiLiberacao = 1");
                else
                    ConfiguracaoDAO.Instance.ExecuteScalar<int>("UPDATE loja SET CalcularIpiLiberacao = 0");
            }
        }
    }
}