using System.Collections.Generic;
using Glass.Data.Helper;
using System.Linq;

namespace Glass.Configuracoes
{
    public partial class OrdemCargaConfig
    {
        /// <summary>
        /// Verifica se deve utilizar o controle de OC
        /// </summary>
        public static bool UsarControleOrdemCarga
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleOrdemCarga); }
        }

        public static bool UsarOrdemCargaParcial
        {
            get
            {
                if (!UsarControleOrdemCarga)
                    return false;

                return Config.GetConfigItem<bool>(Config.ConfigEnum.OrdemCargaParcial);
            }
        }

        public static bool GerarVolumeApenasDePedidosEntrega
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarVolumeApenasDePedidosEntrega); }
        }

        public static bool ExibirNomeFantasiaClienteRptCarregamento
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirNomeFantasiaClienteRptCarregamento); }
        }

        public static bool ExibirRazaoSocialCliente
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirRazaoSocialClienteVolume); }
        }

        /// <summary>
        /// Verifica quais situações do cliente não deve gerar OC
        /// </summary>
        public static List<int> SituacoesClienteNaoGerarOC
        {
            get
            {
                var config = Config.GetConfigItem<string>(Config.ConfigEnum.SituacoesClienteNaoGerarOC);

                if (string.IsNullOrEmpty(config))
                    return new List<int>() { 2, 3, 4 };

                return config.Split(',').Select(f => f.StrParaInt()).ToList();
            }
        }

        /// <summary>
        /// Verifica se no carregamento vai ser controlado os pedidos importados de outro sistema
        /// </summary>
        public static bool ControlarPedidosImportados
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ControlarPedidosImportados); }
        }

        /// <summary>
        /// Verifica se vai exibir junto com o pedido o pedido do cliente no relatorio individual de carregamento.
        /// </summary>
        public static bool ExibirPedCliRelCarregamento
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedCliRelCarregamento); }
        }

        public static bool ExibirEnderecoClienteRptOC
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirEnderecoClienteRptOC); }
        }

        /// <summary>
        /// Define que não será permitido incluir pedidos na ordem de carga que já tenham nota gerada
        /// </summary>
        public static bool BloquearInclusaoPedidoComNotaGerada
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearInclusaoPedidoComNotaGerada); }
        }

        /// <summary>
        /// Define se poderá ser perguntado ao usuário se ele deseja que seja enviado um email ao finalizar o carregamento
        /// </summary>
        public static bool PerguntarSeEnviaraEmailAoFinalizar
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PerguntarSeEnviaraEmailAoFinalizar); }
        }

        /// <summary>
        /// Define se a opção de atualizar a tela de leitura de carregamento automaticamente vem marcada por padrão
        /// </summary>
        public static bool OpcaoAtualizarAutomaticamenteMarcada
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.OpcaoAtualizarAutomaticamenteMarcada); }
        }

        /// <summary>
        /// Define uma data de entrega base para buscar somente pedidos com data de entrega maior do que a definida para gerar OC
        /// </summary>
        public static string DataEntregaBaseConsiderarPedidoParaOC
        {
            get
            {
                switch (ControleSistema.GetSite())
                {
                    case ControleSistema.ClienteSistema.MBTemper:
                        return "2016-01-01 23:59:59";

                    default:
                        return null;
                }
            }
        }
    }
}
