using System;
using Glass.Data.Helper;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Configuracoes
{
    public static class Geral
    {
        #region Define a versão do WebGlass a ser usada

        /// <summary>
        /// Define se a empresa não vende vidro
        /// </summary>
        public static bool NaoVendeVidro()
        {
            return Config.GetConfigItem<bool>(Config.ConfigEnum.NaoVendeVidro);
        }

        #endregion

        /// <summary>
        /// Retorna o número de casas decimais ao calcular TotM.
        /// </summary>
        public static int NumeroCasasDecimaisTotM
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroCasasDecimaisTotM); }
        }

        /// <summary>
        /// Define se os beneficiamentos serão usados para todos os grupos de produtos
        /// (ao invés de serem apenas para vidros).
        /// </summary>
        public static bool UsarBeneficiamentosTodosOsGrupos
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarBeneficiamentosTodosOsGrupos); }
        }

        /// <summary>
        /// Indica se os clientes do tipo Consumidor Final deverão informar os dados de endereço ou não.
        /// </summary>
        public static bool NaoExigirEnderecoConsumidorFinal
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NaoExigirEnderecoConsumidorFinal); }
        }

        /// <summary>
        /// Indica se sera mostrado apenas os dados da loja assiciada ao cadastro do funcionario
        /// </summary>
        public static bool FuncVisualizaDadosApenasSuaLoja
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FuncVisualizaDadosApenasSuaLoja); }
        }

        #region Manter desconto dado por administrador

        /// <summary>
        /// Manter desconto dado por administrador?
        /// </summary>
        public static bool ManterDescontoAdministrador
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ManterDescontoAdministrador); }
        }

        #endregion

        #region Adicional aplicado quando vidro é redondo

        /// <summary>
        /// Adicional que será aplicado no vidro na largura e na altura caso seja redondo
        /// </summary>
        public static int AdicionalVidroRedondoAte12mm
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.AdicionalVidroRedondoAte12mm); }
        }

        /// <summary>
        /// Adicional que será aplicado no vidro na largura e na altura caso seja redondo
        /// </summary>
        public static int AdicionalVidroRedondoAcima12mm
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.AdicionalVidroRedondoAcima12mm); }
        }

        #endregion

        #region box

        /// <summary>
        /// Define que a empresa somente revende box, para que ao dar saída de estoque, os boxes sejam buscados.
        /// </summary>
        public static bool EmpresaSomenteRevendeBox
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EmpresaSomenteRevendeBox); }
        }

        #endregion

        private static string _versao;

        /// <summary>
        /// Retorna a versão atual do sistema
        /// </summary>
        /// <returns></returns>
        public static string ObtemVersao()
        {
            return ObtemVersao(false);
        }

        /// <summary>
        /// Retorna a versão atual do sistema
        /// </summary>
        /// <returns></returns>
        public static string ObtemVersao(bool paraJavaScript)
        {
            /* Chamado 52225. */
            if (!string.IsNullOrEmpty(_versao) && _versao.Count(f => f == '.') >= 2)
            {
                // Caso a versão já tenha sido recuperada, possua a revisão informada, seja uma solicitação de página
                // ou o funcionário seja Admin Sync, retorna a versão com revisão.
                if (_versao.Count(f => f == '.') == 3 && _versao.Split('.')[3] != "0" &&
                    (paraJavaScript || (UserInfo.GetUserInfo != null && UserInfo.GetUserInfo.IsAdminSync)))
                    return _versao;
                // Caso a revisão tenha sido informada e o usuário não seja Admin Sync, retorna a versão sem a revisão.
                else if (!paraJavaScript && UserInfo.GetUserInfo != null && !UserInfo.GetUserInfo.IsAdminSync)
                    return string.Join(".", new string[] { _versao.Split('.')[0], _versao.Split('.')[1], _versao.Split('.')[2] });
            }

            Assembly assembly = Assembly.GetAssembly(typeof(Utils));
            string versao = assembly.ToString();
            versao = versao.Substring(versao.IndexOf("Version="));
            versao = versao.Substring(8, versao.IndexOf(",") - 8);

            string[] dadosVersao = versao.Split('.');
            int posVersao = dadosVersao.Length - 1;

            // Só exibe o último dígito se for administrador Sync
            if (posVersao == 3 && !paraJavaScript && (UserInfo.GetUserInfo == null || !UserInfo.GetUserInfo.IsAdminSync))
                dadosVersao[3] = "0";

            // Se os últimos dígitos da versão for 0, não os exibe
            while (posVersao > 1)
                if (dadosVersao[posVersao].StrParaInt() == 0)
                    dadosVersao[posVersao--] = "";
                else
                    break;

            _versao = string.Join(".", dadosVersao).TrimEnd('.');

            return _versao;
        }

        /// <summary>
        /// Texto impresso no rodape do relatório
        /// </summary>
        public static string TextoRodapeRelatorio(string nomeFuncionario)
        {
            return TextoRodapeRelatorio(nomeFuncionario, true);
        }

        /// <summary>
        /// Texto impresso no rodape do relatório
        /// </summary>
        public static string TextoRodapeRelatorio(string nomeFuncionario, bool incluirData)
        {
            return
                string.Format("WebGlass v{0} - Relatório impresso por {1}{2}",
                    ObtemVersao(), BibliotecaTexto.GetTwoFirstNames(nomeFuncionario),
                    incluirData ? string.Format(" em {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm")) : string.Empty);
        }

        public static string GetFormatTotM()
        {
            return Geral.NumeroCasasDecimaisTotM == 0 ? "0" :
                "0.".PadRight(Geral.NumeroCasasDecimaisTotM + 2, '#');
        }

        public static bool ExibirAlertasAdministrador
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirAlertasAdministrador); }
        }

        /// <summary>
        /// Verifica se vai gravar em log de arquivo os relatorios que foram abertos no sistema.
        /// </summary>
        public static bool GravarLogAberturaRelatorio
        {
            get
            {
                return false;
                //var config = System.Configuration.ConfigurationManager.AppSettings["GravarLogAberturaRelatorio"];
                //return config != null && config.ToLower() == "true";
            }
        }

        /// <summary>
        /// Verifica se a versão usada do sistema é lite
        /// </summary>
        public static bool SistemaLite
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SistemaLite); }
        }

        /// <summary>
        /// Verifica se a empresa possui controle de medição
        /// </summary>
        public static bool ControleMedicao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ControleMedicao); }
        }

        /// <summary>
        /// Verifica se a empresa possui controle de instalação
        /// </summary>
        public static bool ControleInstalacao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ControleInstalacao); }
        }

        /// <summary>
        /// Verifica se a empresa possui controle de conferência (antigo)
        /// </summary>
        public static bool ControleConferencia
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ControleConferencia); }
        }

        /// <summary>
        /// Verifica se a empresa possui controle de PCP
        /// </summary>
        public static bool ControlePCP
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ControlePCP); }
        }

        /// <summary>
        /// Verifica se a empresa possui controle de caixa diário
        /// </summary>
        public static bool ControleCaixaDiario
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ControleCaixaDiario); }
        }

        /// <summary>
        /// Define se a loja do cliente deverá ser considerada em orçamentos, pedidos, liberações etc ao invés da loja do funcionário.
        /// </summary>
        public static bool ConsiderarLojaClientePedidoFluxoSistema
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarLojaClientePedidoFluxoSistema); }
        }

        /// <summary>
        /// Define um texto que será exibido ao enviar o sms diário
        /// </summary>
        public static string TextoAdicionalSMS
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.TextoAdicionalSMS); }
        }

        /// <summary>
        /// Define um texto que será exibido ao enviar o sms diário
        /// </summary>
        public static string TextoSMSPedidoPronto
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.TextoSMSPedidoPronto); }
        }

        /// <summary>
        /// Define a data início para considerar pedidos nos cálculos de produção e faturamento, enviados por SMS e Email para os administradores
        /// </summary>
        public static DateTime DataInicioEnvioSMSEmailAdministradores
        {
            get
            {
                var config = Config.GetConfigItem<string>(Config.ConfigEnum.DataInicioEnvioSMSEmailAdministradores);

                if (string.IsNullOrWhiteSpace(config))
                    return DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00:00"));

                // Pega informações sobre como o SMS deve ser enviado
                var dadosConfig = config.Split('_');

                if (dadosConfig[0] == "HOJE")
                    return DateTime.Parse(DateTime.Now.ToString(string.Format("dd/MM/yyyy {0}", dadosConfig[1])));

                if (dadosConfig[0] == "ONTEM")
                    return DateTime.Parse(DateTime.Now.ToString(string.Format("dd/MM/yyyy {0}", dadosConfig[1]))).AddDays(-1);

                if (dadosConfig[0] == "HOJEEONTEM")
                {
                    if (DateTime.Now.Hour == 7)
                        return DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 18:30:00")).AddDays(-1);
                    else if (DateTime.Now.Hour == 18 && DateTime.Now.Minute >= 30)
                        return DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 07:00:00"));
                    else // Põe um período bem curto para não enviar
                        return DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00:00"));
                }

                return DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00:00"));
            }
        }

        /// <summary>
        /// Define a data fim para considerar pedidos nos cálculos de produção e faturamento, enviados por SMS e Email para os administradores
        /// </summary>
        public static DateTime DataFimEnvioSMSEmailAdministradores
        {
            get
            {
                var config = Config.GetConfigItem<string>(Config.ConfigEnum.DataFimEnvioSMSEmailAdministradores);

                if (string.IsNullOrWhiteSpace(config))
                    return DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 23:59:59"));

                // Pega informações sobre como o SMS deve ser enviado
                var dadosConfig = config.Split('_');

                if (dadosConfig[0] == "HOJE")
                    return DateTime.Parse(DateTime.Now.ToString(string.Format("dd/MM/yyyy {0}", dadosConfig[1])));

                if (dadosConfig[0] == "ONTEM")
                    return DateTime.Parse(DateTime.Now.ToString(string.Format("dd/MM/yyyy {0}", dadosConfig[1]))).AddDays(-1);

                if (dadosConfig[0] == "HOJEEONTEM")
                {
                    if (DateTime.Now.Hour == 7)
                        return DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 07:00:00"));
                    else if (DateTime.Now.Hour == 18 && DateTime.Now.Minute >= 30)
                        return DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 18:30:00"));
                    else // Põe um período bem curto para não enviar
                        return DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00:01"));
                }

                return DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 23:59:59"));
            }
        }

        /// <summary>
        /// Define que box para estoque terão sua altura arrendondada para 1900 para cálculo do m²
        /// </summary>
        public static bool ArredondarBoxPara1900
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ArredondarBoxPara1900); }
        }

        /// <summary>
        /// Define que produtos do subgrupo "BOX PADRÃO" terão sua altura arrendondada para 1900 para cálculo do m²
        /// </summary>
        public static bool ArredondarBoxPara1900SubgrupoBoxPadrao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ArredondarBoxPara1900SubgrupoBoxPadrao); }
        }

        /// <summary>
        /// Define o múltiplo que será usado para cálculo do vidro Aramado
        /// </summary>
        public static int MultiploParaCalculoDeAramado
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.MultiploParaCalculoDeAramado); }
        }

        /// <summary>
        /// Define se o sábado será considerado dia útil nos cálculos de data do sistema
        /// </summary>
        public static bool ConsiderarSabadoDiaUtil
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarSabadoDiaUtil); }
        }

        /// <summary>
        /// Define se o Domingo será considerado dia útil nos cálculos de data do sistema
        /// </summary>
        public static bool ConsiderarDomingoDiaUtil
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarDomingoDiaUtil); }
        }

        /// <summary>
        /// Define o horário inicial que se pode fazer login no sistema.
        /// </summary>
        public static string HorarioInicioLogin
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.HorarioInicioLogin); }
        }

        /// <summary>
        /// Define o horário final que se pode fazer login no sistema.
        /// </summary>
        public static string HorarioFimLogin
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.HorarioFimLogin); }
        }

        /// <summary>
        /// Define que será exibida a razão social na tela de sugestão de clientes
        /// </summary>
        public static bool ExibirRazaoSocialTelaSugestao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirRazaoSocialTelaSugestao); }
        }

        /// <summary>
        /// Define se deve bloquear o carregamento acima da capacidade (KG) do veículo, ou se apenas será exibido mensagem de alerta.
        /// </summary>
        public static bool BloquearGerarCarregamentoAcimaCapacidadeVeiculo
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearGerarCarregamentoAcimaCapacidadeVeiculo); }
        }

        /// <summary>
        /// Define quais cliente não devem ter SMS enviados para clientes se o pedido for importado
        /// </summary>
        public static IEnumerable<uint> NaoEnviarEmailPedidoProntoPedidoImportado
        {
            get
            {
                var config = Config.GetConfigItem<string>(Config.ConfigEnum.NaoEnviarEmailPedidoProntoPedidoImportado);

                if (string.IsNullOrWhiteSpace(config))
                    return new List<uint>();

                return config.Split(',').Select(f => f.StrParaUint());
            }
        }

        /// <summary>
        /// Define se a informação do desconto por quantidade do produto deverá ser concatenada à sua descrição nos relatórios.
        /// </summary>
        public static bool ConcatenarDescontoPorQuantidadeNaDescricaoDoProduto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConcatenarDescontoPorQuantidadeNaDescricaoDoProduto); }
        }

        /// <summary>
        /// Obtém a informação que será exibida no e-mail enviado para administradores.
        /// </summary>
        public static string TextoEmailAdministradores
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.TextoEmailAdministradores); }
        }
    }
}
