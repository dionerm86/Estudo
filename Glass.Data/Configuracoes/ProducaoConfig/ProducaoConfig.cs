using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ProducaoConfig
    {
        /// <summary>
        /// Identifica se no controle de reposição aparece apenas produtos impressos na etiqueta
        /// </summary>
        public static bool ReporApenasProduzidos
        {
            get
            {
                return PCPConfig.ControlarProducao;
            }
        }

        /// <summary>
        /// Define que as etiquetas serão criadas na produção após finalizar o PCP
        /// </summary>
        public static bool GerarEtiquetasProducaoFinalizarPCP
        {
            get
            {
                if (OrdemCargaConfig.UsarControleOrdemCarga)
                    return true;

                return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarEtiquetasProducaoFinalizarPCP);
            }
        }

        /// <summary>
        /// O motivo da perda deve ser obrigatório?
        /// </summary>
        public static bool ObrigarMotivoPerda
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ObrigarMotivoPerda); }
        }

        /// <summary>
        /// Define se na consulta de produção será buscada a data de fábrica
        /// </summary>
        public static bool BuscarDataFabricaConsultaProducao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BuscarDataFabricaConsultaProducao); }
        }

        public static bool ExpedirSomentePedidosLiberados
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExpedirSomentePedidosLiberados); }
        }

        /// <summary>
        /// Define que não será permitido ler chapa N0 com planos de corte
        /// </summary>
        public static bool BloquearLeituraPlanoCorteLoteGenerico
        {
            get
            {
                return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearLeituraPlanoCorteLoteGenerico);
            }
        }

        /// <summary>
        /// Define que não será permitido ler peças individuais na produção com a chapa N0
        /// </summary>
        public static bool BloquearLeituraPecaLoteGenerico
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearLeituraPecaLoteGenerico); }
        }

        /// <summary>
        /// Não permite que o PVB seja lido mais de uma vez
        /// </summary>
        public static bool BloquearLeituraPVBDuplicada
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearLeituraPVBDuplicada); }
        }

        public static bool ExpedirSomentePedidosLiberadosNoCarregamento
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExpedirSomentePedidosLiberados); }
        }

        /// <summary>
        /// Indica o tipo de controle de reposição que será usado.
        /// </summary>
        public static DataSources.TipoReposicaoEnum TipoControleReposicao
        {
            get { return Config.GetConfigItem<DataSources.TipoReposicaoEnum>(Config.ConfigEnum.TipoControleReposicao); }
        }

        /// <summary>
        /// A capacidade de produção será controlada por setor?
        /// </summary>
        public static bool CapacidadeProducaoPorSetor
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CapacidadeProducaoPorSetor); }
        }

        /// <summary>
        /// Raio da roldana, usado na montagem do arquivo SAG
        /// </summary>
        public static int RaioRoldana(int largura, float espessura)
        {
            switch (ControleSistema.GetSite())
            {
                case ControleSistema.ClienteSistema.BellTemper:
                case ControleSistema.ClienteSistema.Dekor:
                case ControleSistema.ClienteSistema.Diviglass:
                case ControleSistema.ClienteSistema.EstruturalVidros:
                case ControleSistema.ClienteSistema.FastTemper:
                case ControleSistema.ClienteSistema.GoldGlass:
                case ControleSistema.ClienteSistema.VidroRapido:
                case ControleSistema.ClienteSistema.Termari:
                    return 7;

                case ControleSistema.ClienteSistema.TemperGlass:
                    return espessura == 6 ? 6 : 7;

                default:
                    return 8;
            }
        }

        /// <summary>
        /// Posição X da roldana, usado na montagem do arquivo SAG
        /// </summary>
        public static int PosXRoldana(int largura)
        {
            switch (ControleSistema.GetSite())
            {
                case ControleSistema.ClienteSistema.BlueHouse:
                    return largura >= 1050 ? 100 : 50;

                case ControleSistema.ClienteSistema.TemperGlass:
                    return largura >= 700 ? 100 : 50;

                default:
                    return 50;
            }
        }

        /// <summary>
        /// Posição X da roldana, usado na montagem do arquivo SAG (0 usa a config padrão)
        /// </summary>
        public static int PosXRoldanaBoxWG
        {
            get
            {
                switch (ControleSistema.GetSite())
                {
                    case ControleSistema.ClienteSistema.Simonica:
                        return 70;

                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// Posição Y da roldana, usado na montagem do arquivo SAG
        /// </summary>
        public static int PosYRoldana
        {
            get
            {
                switch (ControleSistema.GetSite())
                {
                    case ControleSistema.ClienteSistema.Vidralia:
                        return 15;

                    default:
                        return 20;
                }
            }
        }

        /// <summary>
        /// Distância da borda do trinco na posição X
        /// </summary>
        public static int DistBordaXTrinco
        {
            get
            {
                switch (ControleSistema.GetSite())
                {
                    case ControleSistema.ClienteSistema.BlueHouse:
                        return 55;

                    default:
                        return 50;
                }
            }
        }

        /// <summary>
        /// Distância da borda do trinco em porta de correr na posição Y
        /// </summary>
        public static int DistBordaYTrincoCorrer
        {
            get
            {
                switch (ControleSistema.GetSite())
                {
                    case ControleSistema.ClienteSistema.BlueHouse:
                    case ControleSistema.ClienteSistema.CarboneAmericana:
                    case ControleSistema.ClienteSistema.CarbonyTemper:
                    case ControleSistema.ClienteSistema.MBTemper:
                    case ControleSistema.ClienteSistema.Mirandex:
                    case ControleSistema.ClienteSistema.MirandexAcre:
                    case ControleSistema.ClienteSistema.PontoVidraceiro:
                    case ControleSistema.ClienteSistema.VidroMetro:
                    case ControleSistema.ClienteSistema.VmGlobal:
                        return 45;

                    default:
                        return 40;
                }
            }
        }

        /// <summary>
        /// Distância da borda do trinco superior em porta de correr na posição Y
        /// </summary>
        public static int DistBordaYTrincoSup
        {
            get
            {
                switch (ControleSistema.GetSite())
                {
                    case ControleSistema.ClienteSistema.Mirandex:
                    case ControleSistema.ClienteSistema.MirandexAcre:
                    case ControleSistema.ClienteSistema.PontoVidraceiro:
                    case ControleSistema.ClienteSistema.VidroMetro:
                    case ControleSistema.ClienteSistema.VmGlobal:
                        return 40;

                    default:
                        return 35;
                }
            }
        }

        /// <summary>
        /// Raio do furo do puxador para BOXWG
        /// </summary>
        public static int RaioFuroPuxadorBoxWG
        {
            get
            {
                switch (ControleSistema.GetSite())
                {
                    case ControleSistema.ClienteSistema.Diviglass:
                    case ControleSistema.ClienteSistema.BellTemper:
                    case ControleSistema.ClienteSistema.FastTemper:
                    case ControleSistema.ClienteSistema.GoldGlass:
                        return 4;

                    case ControleSistema.ClienteSistema.GlobalTemper:
                        return 5;

                    default:
                        return 6;
                }
            }
        }

        /// <summary>
        /// Altura do puxador para BOXWG
        /// </summary>
        public static int AlturaPuxadorBoxWG
        {
            get
            {
                switch (ControleSistema.GetSite())
                {
                    case ControleSistema.ClienteSistema.MBTemper:
                        return 965;

                    case ControleSistema.ClienteSistema.Diviglass:
                    case ControleSistema.ClienteSistema.BellTemper:
                    case ControleSistema.ClienteSistema.FastTemper:
                    case ControleSistema.ClienteSistema.GoldGlass:
                    case ControleSistema.ClienteSistema.BlueHouse:
                    case ControleSistema.ClienteSistema.GlobalTemper:
                        return 1000;

                    case ControleSistema.ClienteSistema.VidroMetro:
                    case ControleSistema.ClienteSistema.VmGlobal:
                    case ControleSistema.ClienteSistema.PontoVidraceiro:
                        return 950;

                    default:
                        return 900;
                }
            }
        }

        /// <summary>
        /// Define que a peça vai sair de produzindo se tiver passado no setor forno
        /// </summary>
        public static bool SairDeProduzindoSePassarNoForno
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SairDeProduzindoSePassarNoForno); }
        }

        /// <summary>
        /// Define se será bloqueado ler peça em etiqueta de chapa caso tenha havido uma leitura nesta chapa algum outro dia
        /// </summary>
        public static bool BloquearLeituraPecaNaChapaDiasDiferentes
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearLeituraPecaNaChapaDiasDiferentes); }
        }

        /// <summary>
        /// Define que será adicionado 2mm na medida da peça
        /// </summary>
        public static bool Adiciona2mmNaPeca
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.Adiciona2mmNaPeca); }
        }

        /// <summary>
        /// Define se a busca de peças da produção será ordenada pelo pedido
        /// </summary>
        public static bool OrdenaPeloPedido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.OrdenaPeloPedido); }
        }

        /// <summary>
        /// Define se serão acrescentado 2mm na largura da peça ao invés de usar a aresta nas peças
        /// </summary>
        public static bool Acrescentar2mmPecaENaoUsarAresta
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.Acrescentar2mmPecaENaoUsarAresta); }
        }

        /// <summary>
        /// Na MB Temper é necessário que sejam exibidos, no painel comercial, somente os pedidos de entrega que possuem rota, chamado 9242
        /// </summary>
        public static bool ConsiderarApenasPedidosEntregaDeRotaPainelComercial
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarApenasPedidosEntregaDeRotaPainelComercial); }
        }

        /// <summary>
        /// Define se o total de etiqueta não impressa será exibido nos painéis
        /// </summary>
        public static bool ExibirTotalEtiquetaNaoImpressaPainel
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTotalEtiquetaNaoImpressaPainel); }
        }

        /// <summary>
        /// Configuração de trinco específica da Mirandex
        /// </summary>
        public static bool ConfiguracaoTrincoMirandex
        {
            get
            {
                switch (ControleSistema.GetSite())
                {
                    case ControleSistema.ClienteSistema.Mirandex:
                    case ControleSistema.ClienteSistema.MirandexAcre:
                        return true;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Define qual descrição ira ser usada para tipo no relatorio de produção por setor
        /// </summary>
        public static string DescrUsarTipoProducao
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.DescrUsarTipoProducao); }
        }
    }
}
