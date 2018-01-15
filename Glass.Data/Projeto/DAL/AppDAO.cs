using Glass.Data.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Glass.Data.DAL
{
    /// <summary>
    /// DAO usado para carrega os dados para o aplicativo.
    /// </summary>
    public class AppDAO
    {
        #region Métodos Privados

        /// <summary>
        /// Recupera os identificadores das configurações compatíveis com o aplicativo.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<int> ObterIdsConfiguracoes()
        {
            return new[] { 229 };
        }

        /// <summary>
        /// Recupera o valor da configuração.
        /// </summary>
        /// <param name="configuracao"></param>
        /// <param name="configuracaoLoja"></param>
        /// <returns></returns>
        private static object ObterValor(Data.Model.Configuracao configuracao, ConfiguracaoLoja configuracaoLoja)
        {
            switch ((Helper.Config.TipoConfigEnum)configuracao.Tipo)
            {
                case Helper.Config.TipoConfigEnum.Decimal:
                    return configuracaoLoja.ValorDecimal;

                case Helper.Config.TipoConfigEnum.Inteiro:
                case Helper.Config.TipoConfigEnum.ListaMetodo:
                    return configuracaoLoja.ValorInteiro;

                case Helper.Config.TipoConfigEnum.Logico:
                    return configuracaoLoja.ValorBooleano;

                case Helper.Config.TipoConfigEnum.Texto:
                case Helper.Config.TipoConfigEnum.TextoCurto:
                case Helper.Config.TipoConfigEnum.GrupoEnumMetodo:
                    return configuracaoLoja.ValorTexto;

                case Helper.Config.TipoConfigEnum.Data:
                    return Conversoes.ConverteData(configuracaoLoja.ValorTexto);

                case Helper.Config.TipoConfigEnum.Enum:
                    return configuracaoLoja.ValorInteiro;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Recupera as configurações.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Configuracao> ObterConfiguracoes()
        {
            var idsConfiguracoes = ObterIdsConfiguracoes().Select(f => (Helper.Config.ConfigEnum)f).ToArray();

            // Recupera as configurações
            var configuracoes = ConfiguracaoDAO.Instance.GetItens(idsConfiguracoes);

            var userInfo = Helper.UserInfo.GetUserInfo;

            var configuracoesLoja = ConfiguracaoLojaDAO.Instance.GetItens(userInfo?.IdLoja ?? 0, idsConfiguracoes);

            foreach(var config in configuracoes)
            {
                var configuracaoLoja = configuracoesLoja.FirstOrDefault(f => f.IdConfig == config.IdConfig);

                yield return new Configuracao
                {
                    IdConfig = (int)config.IdConfig,
                    Tipo = config.Tipo,
                    Valor = ObterValor(config, configuracaoLoja)
                };
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera os dados para a conta do aplicativo.
        /// </summary>
        /// <returns></returns>
        public ContaAppDados ObterDadosConta()
        {
            using (var session = new GDA.GDASession())
            {
                var resultado = new ContaAppDados();

                resultado.CoresVidro = CorVidroDAO.Instance.GetAll(session);
                resultado.EspessurasVidro = new[]
                {
                    new EspessuraVidro(6),
                    new EspessuraVidro(8),
                    new EspessuraVidro(10),
                    new EspessuraVidro(12),
                };
                resultado.TiposEntrega = TipoEntregaDAO.Instance.GetAll(session);
                resultado.UnidadesMedida = UnidadeMedidaDAO.Instance.GetAll(session);
                resultado.GruposProduto = GrupoProdDAO.Instance.GetAll(session);
                resultado.SubgruposProduto = SubgrupoProdDAO.Instance.GetAll(session);
                resultado.EtiquetasAplicacao = EtiquetaAplicacaoDAO.Instance.GetAll(session);
                resultado.EtiquetasProcesso = EtiquetaProcessoDAO.Instance.GetAll(session);
                resultado.Produtos = ProdutoDAO.Instance.GetAll(session);
                resultado.GruposModelo = GrupoModeloDAO.Instance.GetAll(session);
                resultado.MedidasProjeto = MedidaProjetoDAO.Instance.GetAll(session);
                resultado.ProjetoModelos = ProjetoModeloDAO.Instance.GetAll(session);
                resultado.PosicoesPecaModelo = PosicaoPecaModeloDAO.Instance.GetAll(session);
                resultado.MateriasProjetoModelo = MaterialProjetoModeloDAO.Instance.GetAll(session);
                resultado.MedidasProjetoModelo = MedidaProjetoModeloDAO.Instance.GetAll(session);
                resultado.PecasProjetoModelo = PecaProjetoModeloDAO.Instance.GetAll(session);
                resultado.Configuracoes = ObterConfiguracoes().ToArray();

                return resultado;
            }
        }

        /// <summary>
        /// Recupera os totalizadores dos dados da conta.
        /// </summary>
        public ContaAppTotalizadores ObterTotalizadoresConta()
        {
            using (var session = new GDA.GDASession())
            {
                var resultado = new ContaAppTotalizadores();

                resultado.CoresVidro = (int)CorVidroDAO.Instance.Count(session);
                resultado.EspessurasVidro = 4;
                resultado.TiposEntrega = (int)TipoEntregaDAO.Instance.Count(session);
                resultado.GruposModelo = (int)GrupoModeloDAO.Instance.Count(session);
                resultado.MedidasProjeto = (int)MedidaProjetoDAO.Instance.Count(session);
                resultado.ProjetoModelos = (int)ProjetoModeloDAO.Instance.Count(session);
                resultado.PosicoesPecaModelo = (int)PosicaoPecaModeloDAO.Instance.Count(session);
                resultado.MateriaisProjetoModelo = (int)MaterialProjetoModeloDAO.Instance.Count(session);
                resultado.MedidasProjetoModelo = (int)MedidaProjetoModeloDAO.Instance.Count(session);
                resultado.PecasProjetoModelo = (int)PecaProjetoModeloDAO.Instance.Count(session);
                resultado.Configuracoes = ObterIdsConfiguracoes().Count();

                return resultado;
            }
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Representa a espessura do vidro.
        /// </summary>
        public class EspessuraVidro
        {
            #region Propriedades

            /// <summary>
            /// Valor da espessura.
            /// </summary>
            public int Valor { get; set; }

            /// <summary>
            /// Descrição da espessura.
            /// </summary>
            public string Descricao { get; set; }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor geral
            /// </summary>
            public EspessuraVidro()
            {
            }

            /// <summary>
            /// Cria a instancia com os valores iniciais.
            /// </summary>
            /// <param name="valor"></param>
            public EspessuraVidro(int valor)
            {
                Valor = valor;
                Descricao = string.Format("{0:00}MM", valor);
            }

            #endregion
        }

        /// <summary>
        /// Representa uma configuração da conta.
        /// </summary>
        public class Configuracao
        {
            #region Properties

            /// <summary>
            /// Identificador do configuração.
            /// </summary>
            public int IdConfig { get; set; }

            /// <summary>
            /// Tipo da configuração.
            /// </summary>
            public int Tipo { get; set; }

            /// <summary>
            /// Valor.
            /// </summary>
            public object Valor { get; set; }

            #endregion
        }

        /// <summary>
        /// Dados gerais da conta do aplicativo.
        /// </summary>
        public class ContaAppDados
        {
            #region Propriedades

            /// <summary>
            /// Cores de vidro.
            /// </summary>
            public IEnumerable<CorVidro> CoresVidro { get; set; }

            /// <summary>
            /// Espessuras de vidro.
            /// </summary>
            public IEnumerable<EspessuraVidro> EspessurasVidro { get; set; }

            /// <summary>
            /// Tipos de entrega.
            /// </summary>
            public IEnumerable<TipoEntrega> TiposEntrega { get; set; }

            /// <summary>
            /// Unidades de medida.
            /// </summary>
            public IEnumerable<UnidadeMedida> UnidadesMedida { get; set; }

            /// <summary>
            /// Grupos de produto.
            /// </summary>
            public IEnumerable<GrupoProd> GruposProduto { get; set; }

            /// <summary>
            /// Subgrupo de produtos.
            /// </summary>
            public IEnumerable<SubgrupoProd> SubgruposProduto { get; set; }

            /// <summary>
            /// Etiquetas de aplicação.
            /// </summary>
            public IEnumerable<EtiquetaAplicacao> EtiquetasAplicacao { get; set; }

            /// <summary>
            /// Etiquetas de processo.
            /// </summary>
            public IEnumerable<EtiquetaProcesso> EtiquetasProcesso { get; set; }

            /// <summary>
            /// Produtos.
            /// </summary>
            public IEnumerable<Produto> Produtos { get; set; }

            /// <summary>
            /// Grupos de modelo.
            /// </summary>
            public IEnumerable<GrupoModelo> GruposModelo { get; set; }

            /// <summary>
            /// Modelos de projeto.
            /// </summary>
            public IEnumerable<ProjetoModelo> ProjetoModelos { get; set; }

            /// <summary>
            /// Medidas de projeto.
            /// </summary>
            public IEnumerable<MedidaProjeto> MedidasProjeto { get; set; }

            /// <summary>
            /// Posições das peças do modelo de projeto.
            /// </summary>
            public IEnumerable<PosicaoPecaModelo> PosicoesPecaModelo { get; set; }

            /// <summary>
            /// Materiais dos modelos de projeto.
            /// </summary>
            public IEnumerable<MaterialProjetoModelo> MateriasProjetoModelo { get; set; }

            /// <summary>
            /// Medidas do modelo de projeto.
            /// </summary>
            public IEnumerable<MedidaProjetoModelo> MedidasProjetoModelo { get; set; }

            /// <summary>
            /// Peças do modelo de projeto.
            /// </summary>
            public IEnumerable<PecaProjetoModelo> PecasProjetoModelo { get; set; }

            /// <summary>
            /// Configurações.
            /// </summary>
            public IEnumerable<Configuracao> Configuracoes { get; set; }

            #endregion
        }

        /// <summary>
        /// Armazena os totalizadores dos registro da conta.
        /// </summary>
        public class ContaAppTotalizadores
        {
            #region Propriedades

            /// <summary>
            /// Total de cores de vidro.
            /// </summary>
            public int CoresVidro { get; set; }

            /// <summary>
            /// Total de espessuras de vidro.
            /// </summary>
            public int EspessurasVidro { get; set; }

            /// <summary>
            /// Total de tipos de entrega.
            /// </summary>
            public int TiposEntrega { get; set; }

            /// <summary>
            /// Total de grupos de modelos.
            /// </summary>
            public int GruposModelo { get; set; }

            /// <summary>
            /// Total de modelos de projeto.
            /// </summary>
            public int ProjetoModelos { get; set; }

            /// <summary>
            /// Total de medidas de projeto.
            /// </summary>
            public int MedidasProjeto { get; set; }

            /// <summary>
            /// Total de posições de peça dos modelos.
            /// </summary>
            public int PosicoesPecaModelo { get; set; }

            /// <summary>
            /// Total de materias dos modelos.
            /// </summary>
            public int MateriaisProjetoModelo { get; set; }

            /// <summary>
            /// Total de medidas dos modelos.
            /// </summary>
            public int MedidasProjetoModelo { get; set; }

            /// <summary>
            /// Total de peças do modelo de projeto.
            /// </summary>
            public int PecasProjetoModelo { get; set; }

            /// <summary>
            /// Configurações.
            /// </summary>
            public int Configuracoes { get; set; }

            #endregion
        }

        #endregion
    }
}
