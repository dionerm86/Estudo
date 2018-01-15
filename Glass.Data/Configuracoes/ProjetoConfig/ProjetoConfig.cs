using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class ProjetoConfig
    {
        /// <summary>
        /// Verifica se ao gerar materiais das peças de vidro a medida será calculada com base na medida do vão
        /// </summary>
        public static bool UsarMedidaExataProjeto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CobrarMedidasExatasPedido); }
        }        

        /// <summary>
        /// Define se a prancheta de edição de peça deve ser exibida no e-commerce
        /// </summary>
        public static bool ExibirCADecommerce
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirCADecommerce); }
        }

        /// <summary>
        /// Define se a empresa permite que a imagem individual do projeto seja desenhada no pedido original
        /// </summary>
        public static bool InserirImagemProjetoPedido
        {
            get
            {
                return (Config.GetConfigItem<bool>(Config.ConfigEnum.InserirImagemProjetoPedido) && (UserInfo.GetUserInfo.IsCliente ? ExibirCADecommerce : true)) ||
                       Glass.Configuracoes.Geral.SistemaLite;
            }
        }

        /// <summary>
        /// Define se o plano de contas será exibido "Plano Conta - Grupo Conta" ao invés de "Grupo Conta - Plano Conta"
        /// </summary>
        public static bool InverterExibicaoPlanoConta
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.InverterExibicaoPlanoConta); }
        }

        /// <summary>
        /// Ao cadastrar um novo projeto no pedido, põe o ped cli do pedido no ambiente
        /// </summary>
        public static bool BuscarPedCliAoInserirProjeto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BuscarPedCliAoInserirProjeto); }
        }
 
        /// <summary>
        /// Define se a imagem editada (e consequentemente salva no diretório) do projeto será mantida após o projeto ser re-confirmado
        /// </summary>
        public static bool ManterImagensEditadasAoConfirmarProjeto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ManterImagensEditadasAoConfirmarProjeto); }
        }

        /// <summary>
        /// Define se a imagem editada (e consequentemente salva no diretório) do projeto será mantida após o projeto ser re-confirmado, mesmo que as medidas das peças tenham sido alteradas.
        /// </summary>
        public static bool GerarPecasComMedidasIncoerentesDaImagemEditada
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarPecasComMedidasIncoerentesDaImagemEditada); }
        }

        public static bool UtilizarEditorImagensProjeto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UtilizarEditorImagensProjeto); }
        }

        /// <summary>
        /// Define se sera utilizado o editor CAD Project
        /// </summary>
        public static bool UtilizarEditorCADImagensProjeto
        {
            get
            {
                return (_urlCadProject != null && !string.IsNullOrEmpty(_urlCadProject.ToString()));
            }
        }

        /// <summary>
        /// URL do servidor do CadProject
        /// </summary>
        private static object _urlCadProject = System.Configuration.ConfigurationManager.AppSettings["UrlCadProject"];

        /// <summary>
        /// Url do editor CAD Project
        /// </summary>
        public static string UrlServicoCadProject
        {
            get
            {
                // Se for informado URL do servidor do CadProject, retorna o mesmo.
                if (_urlCadProject != null && !string.IsNullOrEmpty(_urlCadProject.ToString()))
                    return _urlCadProject.ToString();

                return null;
            }
        }

        /// <summary>
        /// Informa, no web.config, se o CadProject está instalado no mesmo servidor do sistema.
        /// </summary>
        private static object _cadProjectInstaladoNoMesmoLocalSistema = System.Configuration.ConfigurationManager.AppSettings["CadProjectInstaladoNoMesmoLocalSistema"];

        /// <summary>
        /// Informa se o CadProject está instalado no mesmo servidor do sistema.
        /// </summary>
        public static bool CadProjectInstaladoNoMesmoLocalSistema
        {
            get
            {
                return _cadProjectInstaladoNoMesmoLocalSistema == null || _cadProjectInstaladoNoMesmoLocalSistema.ToString().ToLower() == "true";
            }
        }

        /// <summary>
        /// Verifica se o transpasse, normalmente de 50mm será cobrado no projeto
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public static bool CobrarTranspasse
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CobrarTranspasse); }
        }

        /// <summary>
        /// Retorna o caminho que o arquivo CalcEngine deve ser salvo.
        /// </summary>
        public static string CaminhoSalvarCalcEngine
        {
            get { return Utils.GetArquivoCalcEnginePath; }
        }

        /// <summary>
        /// Retorna a altura padrão para projetos de box de 6mm
        /// </summary>
        public static int AlturaPadraoProjetoBox6mm
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.AlturaPadraoProjetoBox6mm); }
        }

        /// <summary>
        /// Retorna a altura padrão para projetos de box acima de 6mm
        /// </summary>
        public static int AlturaPadraoProjetoBoxAcima6mm
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.AlturaPadraoProjetoBoxAcima6mm); }
        }

        /// <summary>
        /// Define se a empresa poderá trabalhar com seleção de espessura do vidro ao calcular o projeto.
        /// </summary>
        public static bool SelecionarEspessuraAoCalcularProjeto
        {
            get
            {
                if (ControleSistema.CalcularProjetosPorCorEspessura)
                    return true;

                return Config.GetConfigItem<bool>(Config.ConfigEnum.SelecionarEspessuraAoCalcularProjeto);
            }
        }

        public static int ValidacaoProjetoConfiguravel
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.ValidacaoProjetoConfiguravel); }
        }

        /// <summary>
        /// Define se as peças do projeto poderão ser editadas no PCP mesmo se o cálculo do projeto seja de vão ao invés de medidas exatas.
        /// </summary>
        public static bool PermitirAlterarMedidasPecasProjetoCalculoVaoPCP
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirAlterarMedidasPecasProjetoCalculoVaoPCP); }
        }

        public static bool FMLBasicoSalvarMaiorMedidaNoCampoAltura
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FMLBasicoSalvarMaiorMedidaNoCampoAltura); }
        }

        /// <summary>
        /// Define se ira exibir o menu de folga de projeto no e-commerce.
        /// </summary>
        public static bool ExibirFolgaProjetoEcommerce
        {
            get
            {
                return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirFolgaProjetoEcommerce);
            }
        }
    }
}
