using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Glass.Configuracoes;
using Glass.Global.Negocios.Entidades;
using Glass.Data.DAL;

namespace Glass.Global.Negocios.Componentes
{
    public class MenuFluxo : IMenuFluxo, IProvedorConfigMenuFunc, IProvedorConfigMenuTipoFunc, IProvedorConfigFuncaoFunc, IProvedorConfigFuncaoTipoFunc
    {
        /// <summary>
        /// Lista com os menus do sistema, já filtrado pelas configurações da empresa
        /// </summary>
        private Dictionary<int, List<Entidades.Menu>> _lstMenu = new Dictionary<int, List<Entidades.Menu>>();

        /// <summary>
        /// Lista com os menus do sistema, já filtrado pelas configurações da empresa
        /// </summary>
        private Dictionary<int, IEnumerable<Entidades.Menu>> _dicMenuFunc = new Dictionary<int, IEnumerable<Menu>>();

        /// <summary>
        /// Obtém a identificação do menu
        /// </summary>
        /// <param name="idMenu"></param>
        /// <returns></returns>
        public string ObtemIdentificacaoMenu(int idMenu)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Menu>()
                .Where("IdMenu=?id")
                .Add("?id", idMenu)
                .Select("Nome")
                .Execute()
                .Select(f => f.GetString(0))
                .FirstOrDefault();
        }

        /// <summary>
        /// Obtém a identificação da função de menu
        /// </summary>
        /// <param name="idFuncaoMenu"></param>
        /// <returns></returns>
        public string ObtemIdentificacaoFuncaoMenu(int idFuncaoMenu)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.FuncaoMenu>()
                .Where("IdFuncaoMenu=?id")
                .Add("?id", idFuncaoMenu)
                .Select("Descricao")
                .Execute()
                .Select(f => f.GetString(0))
                .FirstOrDefault();
        }

        /// <summary>
        /// Busca os menus de acordo com as configurações da loja
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public IList<Entidades.Menu> ObterMenusPorConfig(int idLoja)
        {
            if (_lstMenu.Count() == 0 || !_lstMenu.ContainsKey(idLoja))
            {
                if (idLoja == 0)
                    return new List<Entidades.Menu>();

                // Define quais menus não deverão ser buscados de acordo com configurações internas do sistema
                var idsMenuNaoBuscar = string.Empty;

                if (!MenuConfig.ExibirCompraCaixa)
                    idsMenuNaoBuscar += "149,";

                if (!PedidoConfig.LiberarPedido)
                    idsMenuNaoBuscar += "129,193,194,195,197,198,199,233,435,524,";
                else
                    idsMenuNaoBuscar += "174,200,";

                if (ProducaoConfig.TipoControleReposicao != Data.Helper.DataSources.TipoReposicaoEnum.Pedido)
                    idsMenuNaoBuscar += "467,103,";

                if (ProducaoConfig.TipoControleReposicao != Data.Helper.DataSources.TipoReposicaoEnum.Peca)
                    idsMenuNaoBuscar += "468,102,";

                // Gerencial dinâmico
                idsMenuNaoBuscar += "390,";

                if (!FinanceiroConfig.UsarControleLiberarFinanc)
                    idsMenuNaoBuscar += "259,";

                // Define se a opção de emitir nota em contingência SCAN deverá aparecer
                if (FiscalConfig.NotaFiscalConfig.ContingenciaNFe != Data.Helper.DataSources.TipoContingenciaNFe.SCAN)
                    idsMenuNaoBuscar += "328,";

                if (!MenuConfig.ExibirCartaoNaoIdentificado)
                    idsMenuNaoBuscar += "600,";

                if (!FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
                    idsMenuNaoBuscar += "605,";

                if (Geral.SistemaLite && !Data.Helper.UserInfo.GetUserInfo.IsAdminSync)
                    idsMenuNaoBuscar += "82,83,84,";

                idsMenuNaoBuscar += ObterIdsMenuComConfigDesabilitada(idLoja);

                if (ComissaoDAO.Instance.VerificarComissaoContasRecebidas() && idsMenuNaoBuscar.Split(',').Contains("317"))
                {
                    idsMenuNaoBuscar = idsMenuNaoBuscar.Replace(",317,", ",").Replace(",318,", ",").Replace(",319,", ",").Replace(",320,", ",");
                }

                // Busca os menus de acordo com as configurações da empresa
                var retorno = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Menu>("m")
                    .Where(string.Format("m.IdMenu Not In ({0})", idsMenuNaoBuscar.TrimEnd(',')));

                // Se for sistema lite, esconde menus que não devem aparecer
                if (Geral.SistemaLite)
                    retorno.WhereClause
                        .And("ExibirLite=1");

                var menus = retorno.ProcessLazyResult<Menu>().ToList();

                if (!_lstMenu.ContainsKey(idLoja))
                    _lstMenu.Add(idLoja, menus.OrderBy(f => f.IdModulo).ThenBy(f => f.NumSeq).ToList());
            }

            // Remove menus de flag se não for admin sync
            if (!Data.Helper.UserInfo.GetUserInfo.IsAdminSync)
                _lstMenu[idLoja].RemoveAll(f => f.IdMenu == 78 || f.IdMenu == 79);

            return _lstMenu[idLoja];
        }

        /// <summary>
        /// Retorna os ids de menu que não deverão ser buscados devido às configurações associadas à eles estarem desabilitadas
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idsMenuNaoBuscar"></param>
        /// <returns></returns>
        private static string ObterIdsMenuComConfigDesabilitada(int idLoja)
        {
            // Busca todos os configs que possua idLoja
            var idsConfigComLoja = string.Join(",",
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ConfiguracaoLoja>()
                    .Where("IdLoja is not null")
                    .Select("IdConfig")
                    .Execute()
                    .Select(f => f.GetInt32(0)));

            if (string.IsNullOrEmpty(idsConfigComLoja))
                idsConfigComLoja = "0";

            // Adiciona na lista menus que estão associados às configs desabilitadas
            var idsMenuNaoBuscar = string.Join(",",
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ConfigMenu>("cm")
                    .InnerJoin<Data.Model.ConfiguracaoLoja>("cm.IdConfig=cl.IdConfig", "cl")
                    .Where(string.Format("(cl.IdLoja=?idLoja And ISNULL(cl.ValorBooleano, 0)=0) Or (cl.IdLoja is null And ISNULL(cl.ValorBooleano, 0)=0 And cl.IdConfig Not In ({0}))", idsConfigComLoja))
                        .Add("?idLoja", idLoja)
                    .Select("IdMenu")
                    .Execute()
                    .Select(f => f.GetInt32(0)));

            if (string.IsNullOrEmpty(idsMenuNaoBuscar))
                idsMenuNaoBuscar = "0";

            return idsMenuNaoBuscar;
        }

        /// <summary>
        /// Obtém os menus que o funcionário tem acesso
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        public IEnumerable<Entidades.Menu> ObterMenusPorFuncionario(Entidades.Funcionario funcionario)
        {
            _dicMenuFunc= _dicMenuFunc == null ? new Dictionary<int, IEnumerable<Menu>>() : _dicMenuFunc;

            // Carrega os menus do funcionário, caso o mesmo não esteja no dicionário
            if (!_dicMenuFunc.ContainsKey(funcionario.IdFunc))
            {
                // Menus que a empresa pode usar
                var menus = ObterMenusPorConfig(funcionario.IdLoja);

                // Menus liberados para o usuário
                var menusFunc = funcionario.ConfigsMenuFunc.Select(f => f.IdMenu);

                if (!_dicMenuFunc.ContainsKey(funcionario.IdFunc))
                    _dicMenuFunc.Add(funcionario.IdFunc, menus.Where(f => menusFunc.Contains(f.IdMenu)));
            }

            return _dicMenuFunc[funcionario.IdFunc];
        }

        /// <summary>
        /// Remove da memória os menus do funcionário passado
        /// </summary>
        /// <param name="idFunc"></param>
        public void RemoveMenuFuncMemoria(int idFunc)
        {
            if (_dicMenuFunc.ContainsKey(idFunc))
                _dicMenuFunc.Remove(idFunc);

            Data.Helper.Config.RemoveMenuUsuario(idFunc);
        }

        /// <summary>
        /// Remove o menu da memóra
        /// </summary>
        public void RemoveMenuMemoria(int[] idsConfig)
        {
            // Verifica se o config passado está associado à algum menu
            var possuiAssociacao =
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ConfigMenu>()
                    .Where(string.Format("IdConfig in ({0})", string.Join(",", idsConfig)))
                    .Select("IdConfig")
                    .Execute()
                    .Count() > 0;

            if (possuiAssociacao || idsConfig.Contains((int)Data.Helper.Config.ConfigEnum.TipoControleReposicao))
            {
                _lstMenu.Clear();
                _dicMenuFunc.Clear();
                Data.Helper.Config.LimpaMenuUsuario();
            }
        }
    }
}
