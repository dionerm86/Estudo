using System;
using Glass.Data.Helper;
using System.Web;
using System.IO;

namespace Glass.Configuracoes
{
    public static partial class PCPConfig
    {
        /// <summary>
        /// Verifica se a empresa usa os produtos da conferência do pedido no fluxo do sistema.
        /// </summary>
        public static bool UsarConferenciaFluxo
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarConferenciaFluxo); }
        }

        public static bool BuscarProdutoPedidoAssociadoAoIdLojaFuncionarioAoBuscarProdutos
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BuscarProdutoPedidoAssociadoAoIdLojaFuncionarioAoBuscarProdutos); }
        }

        /// <summary>
        /// Verifica se a empressa possui controle de produção (Consulta de produção, etc.)
        /// </summary>
        public static bool ControlarProducao
        {
            get
            {
                return !Geral.SistemaLite &&
                    Config.GetConfigItem<bool>(Config.ConfigEnum.ControlePCP);
            }
        }

        /// <summary>
        /// Habilitar faturamento carregamento
        /// </summary>
        public static bool HabilitarFaturamentoCarregamento
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.HabilitarFaturamentoCarregamento); }
        }

        /// <summary>
        /// Habilitar controle de cavalete?
        /// </summary>
        public static bool ControleCavalete
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ControleCavalete); }
        }

        /// <summary>
        /// Exibe os dados da conferência na lista de pedidos.
        /// </summary>
        public static bool ExibirDadosPcpListaAposConferencia
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirDadosPcpListaAposConferencia); }
        }

        /// <summary>
        /// Define se a impressão do pedido PCP com valores deve aparecer na lista de pedidos.
        /// </summary>
        public static bool ExibirImpressaoPcpListaPedidos
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirImpressaoPcpListaPedidos); }
        }

        /// <summary>
        /// Define se será gerado um orçamento com ferragens e alumínios a partir do PCP finalizado.
        /// </summary>
        public static bool GerarOrcamentoFerragensAluminiosPCP
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarOrcamentoFerragesAluminiosPCP); }
        }

        /// <summary>
        /// Define que será obrigatório realizar leitura nos setores marcados com "Impedir Avanço",
        /// ao tentar efetuar leitura em setores posteriores
        /// </summary>
        public static bool ObrigarLeituraSetorImpedirAvanco
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ObrigarLeituraSetorImpedirAvanco); }
        }

        /// <summary>
        /// Define que a sequência dos setores deverá seguir a sequência do roteiro de produção
        /// do produto que está sendo lido. (Na prática, funciona como se todos os setores possuíssem a
        /// opção "Impedir avanço" marcada).
        /// </summary>
        public static bool UtilizarSequenciaRoteiroProducao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UtilizarSequenciaRoteiroProducao); }
        }

        /// <summary>
        /// Meta de produção diária em M2.
        /// </summary>
        public static double MetaProducaoDiaria
        {
            get { return Config.GetConfigItem<double>(Config.ConfigEnum.MetaProducaoDiaria); }
        }

        /// <summary>
        /// Considerar como meta de produção o total de metro quadrado das peças impressas que devem ser produzidas no dia conforme a data de fábrica.
        /// </summary>
        public static bool ConsiderarMetaProducaoM2PecasPorDataFabrica
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarMetaProducaoM2PecasPorDataFabrica); }
        }

        /// <summary>
        /// Define se para ser liberado o pedido de venda deve ser conferido no PCP.
        /// </summary>
        public static bool ImpedirLiberacaoPedidoSemPCP
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ImpedirLiberacaoPedidoSemPCP); }
        }

        /// <summary>
        /// Define se a empresa exige que os projetos sejam confirmados no PCP
        /// </summary>
        public static bool ExigirConferenciaPCP
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExigirConferenciaPCP); }
        }

        /// <summary>
        /// Define que apenas administrador pode reabrir pedidos no PCP
        /// </summary>
        public static bool ReabrirPCPSomenteAdmin
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ReabrirPCPSomenteAdmin); }
        }

        /// <summary>
        /// Não permite que peças sejam excluídas no PCP
        /// </summary>
        public static bool BloquearExclusaoProdutosPCP
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearExclusaoProdutosPCP); }
        }

        /// <summary>
        /// As peças canceladas (mão-de-obra) devem ser exibidas por padrão?
        /// </summary>
        public static bool ExibirPecasCancMaoObraPadrao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPecasCancMaoObraPadrao); }
        }

        /// <summary>
        /// As peças canceladas devem ser exibidas na liberação?
        /// </summary>
        public static bool ExibirPecasCancLiberacao
        {
            get
            {
                if (!PedidoConfig.LiberarPedido)
                    return false;

                return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPecasCancLiberacao);
            }
        }

        /// <summary>
        /// Verifica se a empresa controla se os projetos cnc ja foram criados.
        /// </summary>
        public static bool UsarControleGerenciamentoProjCnc
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleGerenciamentoProjCnc); }
        }

        /// <summary>
        /// Define que caso seja para gerar forma inexistente no arquivo de exportação do optyway, ao invés de preencher XXXXXX, preenche a forma que estiver no cadastro do produto
        /// </summary>
        public static bool UsarFormaProdutoSeForFormaInexistente
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarFormaProdutoSeForFormaInexistente); }
        }

        /// <summary>
        /// Define que será concatenado no início da etiqueta a espessura, largura e altura no formato EELLLLAAAA,
        /// a largura será exibida primeiro apenas se for maior que a altura, e vice-versa
        /// </summary>
        public static bool ConcatenarEspAltLargAoNumEtiqueta
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConcatenarEspAltLargAoNumEtiqueta); }
        }
        
        /// <summary>
        /// Define se serão criados clones
        /// </summary>
        public static bool CriarClone
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CriarClone); }
        }

        /// <summary>
        /// Define se valores de custo e de venda serão exibidos nos relatórios de produção
        /// </summary>
        public static bool ExibirCustoVendaRelatoriosProducao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirCustoVendaRelatoriosProducao); }
        }

        /// <summary>
        /// Bloqueia a expedição de peças para permitir apenas peças prontas.
        /// (Não é válido para o carregamento, já que este possui uma configuração independente)
        /// </summary>
        public static bool BloquearExpedicaoApenasPecasProntas
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearExpedicaoApenasPecasProntas); }
        }

        /// <summary>
        /// Verifica se deve exportar as informações da etiqueta no arquivo .asc do OptyWay
        /// Usando quando a mesa de corte imprime as etiquetas
        /// </summary>
        public static bool ExportarInfoEtiquetaOptyWay
        {
            get
            {
                var config = System.Configuration.ConfigurationManager.AppSettings["ExportarInfoEtiquetaOptyWay"];
                return config != null && config.ToLower() == "true";
            }
        }

        /// <summary>
        /// Verifica se vai ser utilizado o novo controle de expedição balcão
        /// </summary>
        public static bool UsarNovoControleExpBalcao
        {
            get
            {
                //Se a empresa não utiliza Ordem de carga porem quer utilizar o novo controle de expedição
                if (Config.GetConfigItem<bool>(Config.ConfigEnum.UsarNovoControleExpBalcaoSemCarregamento))
                    return true;

                return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarNovoControleExpBalcao) && OrdemCargaConfig.UsarControleOrdemCarga;
            }
        }

        /// <summary>
        /// Verifica se a empresa permite que uma etiqueta de box seja lida em pedidos diferentes desde que a mesma ja tenha sido lida em uma oc de transferencia
        /// </summary>
        public static bool PermirtirLerEtqBoxDuasVezesSeTransferencia
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermirtirLerEtqBoxDuasVezesSeTransferencia); }
        }

        /* Chamado 16665. */
        /// <summary>
        /// Define se a empresa irá gerar ou não marcação de peças repostas.
        /// </summary>
        public static bool GerarMarcacaoPecaReposta
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarMarcacaoPecaReposta); }
        }

        /// <summary>
        /// Define que a situação cnc dos pedidos pcp ficarão como Não Projetado caso seja pedido de importação
        /// </summary>
        public static bool GerarPCPNaoProjetadoPedidosImportados
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarPCPNaoProjetadoPedidosImportados); }
        }

        /// <summary>
        /// Verifica se os relatórios “Compra de Mercadoria” e “Produtos não comprados” serão exibidos
        /// </summary>
        public static bool ExibirRelatoriosCompras
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirRelatoriosCompras); }
        }

        /// <summary>
        /// Obtem a configuração da aresta
        /// </summary>
        public static string ObtemArestaConfig
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.ConfigAresta); }
        }

        #region Configurações FML

        private static object _caminhoFml = System.Configuration.ConfigurationManager.AppSettings["CaminhoSalvarFml"];

        /// <summary>
        /// Retorna o caminho que o arquivo FML deve ser salvo.
        /// </summary>
        public static string CaminhoSalvarFml
        {
            get
            {
                if (_caminhoFml != null && !String.IsNullOrEmpty(_caminhoFml.ToString()))
                    return _caminhoFml.ToString();

                switch (ControleSistema.GetSite())
                {
                    default:
                        return Utils.GetArquivoFmlGeradoPath;
                }
            }
        }

        /// <summary>
        /// Indica se a empresa utiliza a geração do arquivo FML.
        /// </summary>
        public static bool EmpresaGeraArquivoFml
        {
            get
            {
                if (Config.GetConfigItem<bool>(Config.ConfigEnum.GerarFml))
                    return true;

                return _caminhoFml != null && !String.IsNullOrEmpty(_caminhoFml.ToString());
            }
        }

        #endregion

        #region Configurações DXF

        private static object _caminhoDxf = System.Configuration.ConfigurationManager.AppSettings["CaminhoSalvarDxf"];

        /// <summary>
        /// Retorna o caminho que o arquivo DXF deve ser salvo.
        /// </summary>
        public static string CaminhoSalvarDxf
        {
            get
            {
                if (_caminhoDxf != null && !String.IsNullOrEmpty(_caminhoDxf.ToString()))
                    return _caminhoDxf.ToString();

                return Utils.GetArquivoDxfGeradoPath;
            }
        }

        /// <summary>
        /// Indica se a empresa utiliza a geração do arquivo DXF.
        /// </summary>
        public static bool EmpresaGeraArquivoDxf
        {
            get
            {
                if (Config.GetConfigItem<bool>(Config.ConfigEnum.GerarDxf))
                    return true;

                return _caminhoDxf != null && !String.IsNullOrEmpty(_caminhoDxf.ToString());
            }
        }

        /// <summary>
        /// Retorna o endereço virtual da pasta onde deverão ser salvas as imagens das peças anexadas/editas dos projetos
        /// </summary>
        public static string GetPecaProjetoVirtualPath
        {
            get
            {
                string caminho = "~/Upload/PecaProjeto/";

                if (!Directory.Exists(HttpContext.Current.Server.MapPath(caminho)))
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(caminho));

                return caminho;
            }
        }

        /// <summary>
        /// Caminho utilizado para salvar os arquivos do CadProject
        /// </summary>
        /// <param name="pcp"></param>
        /// <returns></returns>
        public static string CaminhoSalvarCadProject(bool pcp)
        {
            return PCPConfig.GetArquivoCadProjectPath(false, pcp); 
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos CadProject.
        /// </summary>
        public static string GetArquivoCadProjectPath(bool criadoPeloProjeto, bool pcp)
        {
            var caminho = HttpContext.Current.Server.MapPath(GetArquivoCadProjectVirtualPath(criadoPeloProjeto, pcp));
            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            return caminho;
        }

        /// <summary>
        /// Retorna o endereço virtual dos arquivos do CadProject.
        /// </summary>
        public static string GetArquivoCadProjectVirtualPath(bool criadoPeloProjeto, bool pcp)
        {
            return "~/Upload/CadProject/" + (criadoPeloProjeto ? "Projeto" : (pcp ? "PCP" : "Comercial")) + "/";
        }

        /// <summary>
        /// Caminho utilizado para salvar os arquivos do CadProject (se gerado pelo processo)
        /// </summary>
        public static string CaminhoSalvarCadProjectProjeto()
        {
            return PCPConfig.GetArquivoCadProjectPath(true, false);
        }

        #endregion

        #region Configurações SGLASS

        private static object _caminhoHardwareSGlass = System.Configuration.ConfigurationManager.AppSettings["CaminhoSalvarSGlassHardware"];
        private static object _caminhoProgramSGlass = System.Configuration.ConfigurationManager.AppSettings["CaminhoSalvarProgramSGlass"];

        /// <summary>
        /// Retorna o caminho que o arquivo SGlass hardware deve ser salvo.
        /// </summary>
        public static string CaminhoSalvarSGlassHardware
        {
            get
            {
                if (_caminhoHardwareSGlass != null && !string.IsNullOrWhiteSpace(_caminhoHardwareSGlass.ToString()))
                    return _caminhoHardwareSGlass.ToString();

                return Utils.GetArquivoSGlassHardwareGeradoPath;
            }
        }

        /// <summary>
        /// Retorna o caminho que o arquivo SGlass Program deve ser salvo.
        /// </summary>
        public static string CaminhoSalvarProgramSGlass
        {
            get
            {
                if (_caminhoProgramSGlass != null && !string.IsNullOrWhiteSpace(_caminhoProgramSGlass.ToString()))
                    return _caminhoProgramSGlass.ToString();

                return Utils.GetArquivoSGlassProgramGeradoPath;
            }
        }

        /// <summary>
        /// Indica se a empresa utiliza a geração do arquivo SGlass.
        /// </summary>
        public static bool EmpresaGeraArquivoSGlass
        {
            get
            {
                if (Config.GetConfigItem<bool>(Config.ConfigEnum.GerarSGlass))
                    return true;

                return _caminhoHardwareSGlass != null && !string.IsNullOrWhiteSpace(_caminhoHardwareSGlass.ToString())
                    && _caminhoProgramSGlass != null && !string.IsNullOrWhiteSpace(_caminhoProgramSGlass.ToString());
            }
        }

        #endregion

        #region Configurações Intermac

        private static object _caminhoIntermac = System.Configuration.ConfigurationManager.AppSettings["CaminhoSalvarIntermac"];

        /// <summary>
        /// Retorna o caminho que o arquivo Intermac deve ser salvo.
        /// </summary>
        public static string CaminhoSalvarIntermac
        {
            get
            {
                if (_caminhoIntermac != null && !string.IsNullOrWhiteSpace(_caminhoIntermac.ToString()))
                    return _caminhoIntermac.ToString();

                return Utils.GetArquivoIntermacGeradoPath;
            }
        }

        /// <summary>
        /// Indica se a empresa utiliza a geração do arquivo Intermac.
        /// </summary>
        public static bool EmpresaGeraArquivoIntermac
        {
            get
            {
                if (Config.GetConfigItem<bool>(Config.ConfigEnum.GerarArquivoIntermac))
                    return true;

                return _caminhoIntermac != null && !string.IsNullOrWhiteSpace(_caminhoIntermac.ToString());
            }
        }

        #endregion

        /// <summary>
        /// Retorna a situação padrão do retalho ao criá-lo.
        /// </summary>
        public static Data.Model.SituacaoRetalhoProducao SituacaoRetalhoAoCriar
        {
            get { return (Data.Model.SituacaoRetalhoProducao)Enum.Parse(typeof(Data.Model.SituacaoRetalhoProducao), Config.GetConfigItem<string>(Config.ConfigEnum.SituacaoRetalhoAoCriar)); }
        }

        /// <summary>
        /// Veirifica se deve usar o novo controle de aresta
        /// </summary>
        public static bool UsarNovoControleAresta
        {
            get
            {
                var config = System.Configuration.ConfigurationManager.AppSettings["NovoControleAresta"];
                return config != null && config.ToLower() == "true";
            }
        }

        /// <summary>
        /// Verifica se a empresa permite a criação de retalho para peças repostas que não foram cortadas. Num cliente específico eles precisam gerar retalho de chapas laminadas que ainda não passaram no corte.
        /// </summary>
        public static bool ImpedirCriarRetalhoPecaRepostaNaoCortada
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ImpedirCriarRetalhoPecaRepostaNaoCortada); }
        }

        /// <summary>
        /// Define se serão exportados na notas 1,2 e 3 do arquivo do optyway o processo, observação da peça e rota
        /// </summary>
        public static bool ExportarProcessoObsRotaOptyway
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExportarProcessoObsRotaOptyway); }
        }

        /// <summary>
        /// Define se o link para exportar somente peças não exportadas para o optyay ficará visível
        /// </summary>
        public static bool ExibirOpcaoExportarApenasNaoExportadasOptyway
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirOpcaoExportarApenasNaoExportadasOptyway); }
        }

        /// <summary>
        /// Define se serão exportados na notas 1, 2 e 4 do arquivo do optyway o processo, aplicação e se a peça possui Serigrafia ou Pintura
        /// </summary>
        public static bool ExportarSerigrafiaPinturaOptyway
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExportarSerigrafiaPinturaOptyway); }
        }

        /// <summary>
        /// Define que o arquivo SAG será criado com números decimais e não hexadecimais
        /// </summary>
        public static bool CriarArquivoSAGComNumDecimal
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CriarArquivoSAGComNumDecimal); }
        }

        /// <summary>
        /// Define a quantidade de dígitos que o arquivo de mesa pode ter
        /// </summary>
        public static int QtdDigitosNomeArquivoMesa
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.QtdDigitosNomeArquivoMesa); }
        }

        /// <summary>
        /// Define o que será preenchido no campo forma caso a peça não tenha forma definida
        /// </summary>
        public static string PreenchimentoFormaNaoExistente
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.PreenchimentoFormaNaoExistente); }
        }

        /// <summary>
        /// Define que não irá gerar shapeId no arquivo de exportação para o optyway caso a peça seja de reposição e não tenha sido definida forma para a etiqueta
        /// </summary>
        public static bool GerarShapeIdVazioSePecaRepostaEEtiquetaSemForma
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarShapeIdVazioSePecaRepostaEEtiquetaSemForma); }
        }

        /// <summary>
        /// Define que a peça irá com prioridade máxima (999) para o optyway caso seja fast delivery ou peça reposta
        /// </summary>
        public static bool PrioridadeMaximaArquivoOptywaySeFastDeliveryOuPecaReposta
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PrioridadeMaximaArquivoOptywaySeFastDeliveryOuPecaReposta); }
        }

        /// <summary>
        /// Define que a peça irá com prioridade máxima (999) para o optyway caso seja fast delivery ou peça reposta
        /// </summary>
        public static string TipoArquivoMesaPadrao
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.TipoArquivoMesaPadrao); }
        }

        /// <summary>
        /// Define que será preenchido "REPOSIÇÃO" e "GARANTIA" no campo "Forma" da etiqueta, caso o pedido seja reposição ou garantia
        /// </summary>
        public static bool PreencherReposicaoGarantiaCampoForma
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PreencherReposicaoGarantiaCampoForma); }
        }

        /// <summary>
        /// Define que será preenchido "REDONDO" no campo "Forma" da etiqueta, caso a peça tenha o beneficiamento redondo
        /// </summary>
        public static bool PreencherRedondoCampoForma
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PreencherRedondoCampoForma); }
        }

        /// <summary>
        /// Define se o filtro padrão na tela de reposição de peças será o dia atual e não o período de hoje até um mês atrás
        /// </summary>
        public static bool FiltroPadraoDiaAtualTelaReposicao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FiltroPadraoDiaAtualTelaReposicao); }
        }

        /// <summary>
        /// Define se no nome de arquivo de mesa deverá ser trocada a "/" por ";"
        /// </summary>
        public static bool NomeArquivoMesaBarraPorPontoVirgula
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NomeArquivoMesaBarraPorPontoVirgula); }
        }

        /// <summary>
        /// Define se no nome de arquivo de mesa deverá ser trocar o caractere "/" pelo caractere "ç".
        /// </summary>
        public static bool NomeArquivoMesaBarraPorCeCedilha
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NomeArquivoMesaBarraPorCeCedilha); }
        }

        /// <summary>
        /// Define se o nome de arquivo de mesa deverá ser recriado
        /// </summary>
        public static bool NomeArquivoMesaRecriado
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NomeArquivoMesaRecriado); }
        }

        /// <summary>
        /// Define se o nome de arquivo de mesa deverá substituir hífen por aspas simples e barra por O craseado.
        /// </summary>
        public static bool NomeArquivoMesaComHifenEOCraseado
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NomeArquivoMesaComHifenEOCraseado); }
        }

        /// <summary>
        /// Define se o nome de arquivo DXF deverá substituir hífen por aspas simples e barra por cedilha(ç).
        /// </summary>
        public static bool NomeArquivoDxfComAspasECedilha
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NomeArquivoDxfComAspasECedilha); }
        }

        /// <summary>
        /// Define que será enviado para o optyway o valor "N" no campo rotação, caso o codInterno do produto seja "CANELFUME" ou "CANELADO"
        /// </summary>
        public static bool EnviarOptywayRotacaoNSeCanelado
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EnviarOptywayRotacaoNSeCanelado); }
        }

        /// <summary>
        /// Define que será enviado para o corte certo Largura X Altura
        /// </summary>
        public static bool ExibirLarguraAlturaCorteCerto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirLarguraAlturaCorteCerto); }
        }

        /// <summary>
        /// Define se os arquivos básicos serão salvos ao finalizar o PCP, no caminho onde o DXF ou FML são salvos.
        /// </summary>
        public static bool SalvarArquivoBasicoAoFinalizarPCP
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SalvarArquivoBasicoAoFinalizarPCP); }
        }

        /// <summary>
        /// Define se o número da etiqueta deverá ser exibido acima da imagem da peça na tela de marcação de produção, caso a configuração seja false, o número da etiqueta será exibido abaixo da imagem da peça.
        /// </summary>
        public static bool ExibirNumeroEtiquetaAcimaImagemPecaTelaMarcacao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirNumeroEtiquetaAcimaImagemPecaTelaMarcacao); }
        }

        /// <summary>
        /// Define se o controle de otmização de barras de alumínio sera utilizado
        /// </summary>
        public static bool HabilitarControleOtimizacaoAluminio
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.HabilitarOtimizacaoAluminio); }
        }

        /// <summary>
        /// Aresta utilizada na otimização da barra de alumínio.
        /// </summary>
        public static decimal ArestaBarraAluminioOtimizacao
        {
            get { return Config.GetConfigItem<decimal>(Config.ConfigEnum.ArestaBarraAluminioOtimizacao); }
        }

        /// <summary>
        /// Aresta utilizada na otimização de alumínio com corte 45°.
        /// </summary>
        public static decimal ArestaGrau45AluminioOtimizacao
        {
            get { return Config.GetConfigItem<decimal>(Config.ConfigEnum.ArestaGrau45AluminioOtimizacao); }
        }

        /// <summary>
        /// Aresta utilizada na otimização de alumínio com corte 90°.
        /// </summary>
        public static decimal ArestaGrau90AluminioOtimizacao
        {
            get { return Config.GetConfigItem<decimal>(Config.ConfigEnum.ArestaGrau90AluminioOtimizacao); }
        }

        /// <summary>
        /// Acrecimo por peça na otimização de aluminios em projetos de temperados
        /// </summary>
        public static decimal AcrescimoBarraAluminioOtimizacaoProjetoTemperado
        {
            get { return Config.GetConfigItem<decimal>(Config.ConfigEnum.AcrescimoBarraAluminioOtimizacaoProjetoTemperado); }
        }

        /// <summary>
        /// Indica se a empresa trabalha com gerenciamento de fornada
        /// </summary>
        public static bool GerenciamentoFornada
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerenciamentoFornada); }
        }

        /// <summary>
        /// Define se no relatório de retalhos de produção será exibido tabela de M² por Cor e Espessura.
        /// </summary>
        public static bool ExibirTotalM2RetalhoCorEspessura
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTotalM2RetalhoCorEspessura); }
        }

        /// <summary>
        /// Define se será permitido gerar conferencia de pedido de revenda
        /// </summary>
        public static bool PermitirGerarConferenciaPedidoRevenda
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirGerarConferenciaPedidoRevenda); }
        }

        /// <summary>
        /// Verifica se ira dar a opção de gerar o arquivo de otimização sem SAG
        /// </summary>
        public static bool PermitirGerarArqOtimizacaoSemSag
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirGerarArqOtimizacaoSemSag); }
        }

        /// <summary>
        /// Define a versão do arquivo de exportação para o Optyway que será usada (4 ou 7)
        /// </summary>
        public static int VersaoArquivoOptyway
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.VersaoArquivoOptyway); }
        }

        /// <summary>
        /// Define a versão do arquivo de exportação para o Optyway que será usada (4 ou 7)
        /// </summary>
        public static bool PermitirImpressaoDePedidosImportadosApenasConferidos
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirImpressaoDePedidosImportadosApenasConferidos); }
        }
    }
}