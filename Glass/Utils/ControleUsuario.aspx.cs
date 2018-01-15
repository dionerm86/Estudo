using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using System.Linq;
using Glass.Global.Negocios.Entidades;

namespace Glass.UI.Web.Utils
{
    public partial class ControleUsuario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (drpControle.SelectedValue == "1")
                MontaTreeViewFuncionario();
            else
                MontaTreeViewTipoFunc();
        }

        protected void drpControle_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblFuncionario.Visible = drpControle.SelectedValue == "1";
            drpFuncionario.Visible = lblFuncionario.Visible;

            lblTipoFunc.Visible = drpControle.SelectedValue == "2";
            drpTipoFunc.Visible = lblTipoFunc.Visible;
        }

        #region Carrega menus e funções

        protected void MontaTreeViewFuncionario()
        {
            Funcionario funcionario;

            if (UserInfo.GetUserInfo.IsAdministrador)
            {
                btnSalvar.Visible = drpFuncionario.SelectedValue != "" && drpFuncionario.SelectedValue != "0";

                if (drpFuncionario.SelectedValue == "" || drpFuncionario.SelectedValue == "0")
                    return;

                // Recupera o funcionário
                funcionario = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>()
                    .ObtemFuncionario(int.Parse(drpFuncionario.SelectedValue));
            }
            else
            {
                btnSalvar.Visible = false;
                lblControle.Visible = false;
                drpControle.Visible = false;
                lblFuncionario.Visible = false;
                drpFuncionario.Visible = false;
                lblTipoFunc.Visible = false;
                drpTipoFunc.Visible = false;

                funcionario = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>()
                    .ObtemFuncionario((int)UserInfo.GetUserInfo.CodUser);
            }

            MontaTreeView(funcionario.IdLoja, funcionario.IdFunc, funcionario, null);
        }

        protected void MontaTreeViewTipoFunc()
        {
            btnSalvar.Visible = drpTipoFunc.SelectedValue != "" && drpTipoFunc.SelectedValue != "0";

            if (drpTipoFunc.SelectedValue == "" || drpTipoFunc.SelectedValue == "0")
                return;

            // Recupera o funcionário
            var tipoFunc = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>().ObtemTipoFuncionario(int.Parse(drpTipoFunc.SelectedValue));

            MontaTreeView((int)UserInfo.GetUserInfo.IdLoja, tipoFunc.IdTipoFuncionario, null, tipoFunc);
        }

        /// <summary>
        /// Monta a treeview com os módulos, já marcando o que o funcionário tem acesso
        /// </summary>
        protected void MontaTreeView(int idLoja, int id, Funcionario funcionario, TipoFuncionario tipoFunc)
        {
            // Quantidade de treeviews por linha
            var qtdTrvPorLinha = 4;

            // Recupera os menus do sistema
            var menus = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IMenuFluxo>().ObterMenusPorConfig(idLoja);

            // Recupera as funções do sistema
            var funcoes = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IFuncaoFluxo>().ObterFuncoes().ToList();

            var table = new Table();
            table.ID = string.Format("table_{0}", id);

            var tr = new TableRow();

            // Para cada módulo do sistema, monta uma treeview com os menus
            foreach (var menu in menus.Where(f => f.IdMenuPai.GetValueOrDefault() == 0))
            {
                // Cria a treeview com o nome do módulo
                var trv = new TreeView();
                trv.ID = string.Format("trv_{0}_{1}", menu.IdMenu, id);
                trv.EnableViewState = false;
                trv.NodeStyle.CssClass = "treeNode";
                trv.RootNodeStyle.CssClass = "rootNode";
                trv.LeafNodeStyle.CssClass = "leafNode";
                trv.Attributes.Add("onclick", "OnTreeClick(event)");

                // Monta a treeview
                trv.Nodes.Add(PreencheNo(funcionario, tipoFunc, menus.ToList(), funcoes, menu));

                // Adiciona controle em um td
                if (trv.Nodes.Count > 0)
                {
                    var td = new TableCell();
                    td.Style.Add("vertical-align", "top");
                    td.Controls.Add(trv);
                    tr.Cells.Add(td);

                    // Insere uma nova linha a cada qtdTrvPorLinha treeviews
                    if (tr.Cells.Count % qtdTrvPorLinha == 0)
                    {
                        table.Rows.Add(tr);
                        tr = new TableRow();
                    }
                }
            }

            // Insere a última linha na tabela (caso não tenha completado as quatros td's para ser inserida no foreach acima)
            if (tr.Cells.Count % qtdTrvPorLinha > 0)
                table.Controls.Add(tr);

            // Adiciona controles na tela
            divMenu.Controls.Add(table);
        }

        /// <summary>
        /// Preenche um nó da treeview
        /// </summary>
        /// <param name="nodePai"></param>
        /// <param name="funcionario"></param>
        /// <param name="menusModulo"></param>
        /// <param name="funcoes"></param>
        /// <param name="menu"></param>
        private TreeNode PreencheNo(Funcionario funcionario, TipoFuncionario tipoFunc, List<Glass.Global.Negocios.Entidades.Menu> menusModulo, List<FuncaoMenu> funcoes, Glass.Global.Negocios.Entidades.Menu menu)
        {
            var node = new TreeNode(menu.Nome);
            
            node.Value = string.Format("m|{0}", menu.IdMenu.ToString());
            node.SelectAction = TreeNodeSelectAction.Expand;
            node.Expanded = false;
            CriaFuncoes(ref node, funcionario, tipoFunc, funcoes, menu);

            // Cria os filhos deste menu
            CriaItensTreeView(ref node, funcionario, tipoFunc, menu, menusModulo, funcoes);

            if (UserInfo.GetUserInfo.IsAdministrador)
            {
                node.ImageUrl = "~/Images/Menu.png";
                node.ShowCheckBox = funcionario == null || funcionario.TipoFuncionario.IdTipoFuncionario != (int)Data.Helper.Utils.TipoFuncionario.MarcadorProducao;

                // Verifica se o usuário possui acesso à este menu
                if (funcionario != null)
                    node.Checked = funcionario.ConfigsMenuFunc.Any(f => f.IdMenu == menu.IdMenu);
                else
                    node.Checked = tipoFunc.ConfigsMenuTipoFunc.Any(f => f.IdMenu == menu.IdMenu);
            }
            else
            {
                // Verifica se o funcionário/tipo possui acesso à esta função
                if (funcionario != null)
                    node.ImageUrl += funcionario.ConfigsMenuFunc.Any(f => f.IdMenu == menu.IdMenu) ? "~/Images/validacao.gif" : "";
                else
                    node.ImageUrl += tipoFunc.ConfigsMenuTipoFunc.Any(f => f.IdMenu == menu.IdMenu) ? "~/Images/validacao.gif" : "";
            }

            return node;
        }

        /// <summary>
        /// Adiciona as funções que este menu possui como filhas do mesmo
        /// </summary>
        /// <param name="node"></param>
        /// <param name="funcionario"></param>
        /// <param name="tipoFunc"></param>
        /// <param name="funcoes"></param>
        /// <param name="menu"></param>
        private void CriaFuncoes(ref TreeNode node, Funcionario funcionario, TipoFuncionario tipoFunc, List<FuncaoMenu> funcoes, Glass.Global.Negocios.Entidades.Menu menu)
        {
            var funcoesMenu = funcoes.Where(f => f.IdMenu == menu.IdMenu);
            foreach (var funcao in funcoesMenu)
            {
                var nodeFuncao = new TreeNode(funcao.Descricao);
                nodeFuncao.Value = string.Format("f|{0}", funcao.IdFuncaoMenu.ToString());
                nodeFuncao.SelectAction = TreeNodeSelectAction.None;

                if (UserInfo.GetUserInfo.IsAdministrador)
                {
                    nodeFuncao.ShowCheckBox = true;
                    nodeFuncao.ImageUrl = "~/Images/gear.gif";

                    // Verifica se o funcionário/tipo possui acesso à esta função
                    if (funcionario != null)
                        nodeFuncao.Checked = funcionario.ConfigsFuncaoFunc.Any(f => f.IdFuncaoMenu == funcao.IdFuncaoMenu);
                    else
                        nodeFuncao.Checked = tipoFunc.ConfigsFuncaoTipoFunc.Any(f => f.IdFuncaoMenu == funcao.IdFuncaoMenu);
                }
                else
                {
                    // Verifica se o funcionário/tipo possui acesso à esta função
                    if (funcionario != null)
                        nodeFuncao.ImageUrl += funcionario.ConfigsFuncaoFunc.Any(f => f.IdFuncaoMenu == funcao.IdFuncaoMenu) ? "~/Images/validacao.gif" : "";
                    else
                        nodeFuncao.ImageUrl += tipoFunc.ConfigsFuncaoTipoFunc.Any(f => f.IdFuncaoMenu == funcao.IdFuncaoMenu) ? "~/Images/validacao.gif" : "";
                }

                node.ChildNodes.Add(nodeFuncao);
            }
        }

        /// <summary>
        /// Função recursiva que monta a treeview em vários níveis
        /// </summary>
        /// <param name="nodePai"></param>
        /// <param name="funcionario"></param>
        /// <param name="tipoFunc"></param>
        /// <param name="idMenuInicial"></param>
        /// <param name="menuPai"></param>
        /// <param name="menusModulo"></param>
        /// <param name="funcoes"></param>
        protected void CriaItensTreeView(ref TreeNode nodePai, Funcionario funcionario, TipoFuncionario tipoFunc, Glass.Global.Negocios.Entidades.Menu menuPai, List<Glass.Global.Negocios.Entidades.Menu> menusModulo, List<FuncaoMenu> funcoes)
        {
            // Caso este menu possua filhos, adiciona-os na iteração abaixo
            foreach (var menu in menusModulo.Where(f => f.IdMenuPai == menuPai.IdMenu))
                nodePai.ChildNodes.Add(PreencheNo(funcionario, tipoFunc, menusModulo, funcoes, menu));
        }

        #endregion

        #region Salva alterações

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            var idsMenu = new List<int>();
            var idsFuncaoMenu = new List<int>();

            // Percorre todas as treeviews, recuperando os menus e funções selecionados
            foreach (TableRow tr in ((Table)divMenu.Controls[0]).Rows)
                foreach (TableCell cell in tr.Cells)
                {
                    // Percorre todos os nós da treeview
                    foreach (TreeNode node in (cell.Controls[0] as TreeView).Nodes)
                        RecuperaIdsSelecionados(ref idsMenu, ref idsFuncaoMenu, node);
                }

            if (drpControle.SelectedValue == "1")
                SalvarFuncionario(idsMenu, idsFuncaoMenu);
            else
                SalvaTipoFunc(idsMenu, idsFuncaoMenu);
        }

        /// <summary>
        /// Função recursiva, que recupera os idsMenu e idsFuncaoMenu do nó passado, inclusive de seus filhos
        /// </summary>
        /// <param name="idsMenu"></param>
        /// <param name="idsFuncaoMenu"></param>
        /// <param name="node"></param>
        private static void RecuperaIdsSelecionados(ref List<int> idsMenu, ref List<int> idsFuncaoMenu, TreeNode node)
        {
            if (node.Checked)
            {
                var id = int.Parse(node.Value.Split('|')[1]);

                // Recupera o idMenu ou idFuncaoMenu
                if (node.Value.Contains("m"))
                    idsMenu.Add(id);
                else
                    idsFuncaoMenu.Add(id);
            }

            // Repete este processo para os filhos deste nós
            if (node.ChildNodes.Count > 0)
                foreach (TreeNode nodeFilho in node.ChildNodes)
                    RecuperaIdsSelecionados(ref idsMenu, ref idsFuncaoMenu, nodeFilho);
        }

        protected void SalvarFuncionario(List<int> idsMenu, List<int> idsFuncaoMenu)
        {
            var funcionarioFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

            // Recupera o funcionário
            var funcionario = funcionarioFluxo.ObtemFuncionario(int.Parse(drpFuncionario.SelectedValue));

            var menusSistema = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IMenuFluxo>().ObterMenusPorConfig(funcionario.IdLoja);

            // Limpa os menus e funções que o mesmo tem acesso (Tem que fazer desta forma ao invés de usar o .Clear() para que salve o log corretamente)
            foreach (var configMenu in funcionario.ConfigsMenuFunc.ToList())
                if (!idsMenu.Contains(configMenu.IdMenu) && menusSistema.Any(f => f.IdMenu == configMenu.IdMenu))
                    funcionario.ConfigsMenuFunc.Remove(configMenu);

            // Recupera as funções que estejam associadas aos menus que a empresa tem acesso para a partir daí verificar quais devem ser removidas do usuário
            var funcoes = funcionario.ConfigsFuncaoFunc.Where(f => menusSistema.Any(x => f.FuncaoMenu != null && x.IdMenu == f.FuncaoMenu.IdMenu)).ToList();
            foreach (var configFuncao in funcoes)
                if (!idsFuncaoMenu.Contains(configFuncao.IdFuncaoMenu))
                    funcionario.ConfigsFuncaoFunc.Remove(configFuncao);

            // Adiciona menus e funções associados
            foreach (var id in idsMenu)
                if (!funcionario.ConfigsMenuFunc.Any(f => f.IdMenu == id) && menusSistema.Any(f => f.IdMenu == id))
                    funcionario.ConfigsMenuFunc.Add(new ConfigMenuFunc() { IdFunc = funcionario.IdFunc, IdMenu = id });

            foreach (var id in idsFuncaoMenu)
                if (!funcionario.ConfigsFuncaoFunc.Any(f => f.IdFuncaoMenu == id))
                    funcionario.ConfigsFuncaoFunc.Add(new ConfigFuncaoFunc() { IdFunc = funcionario.IdFunc, IdFuncaoMenu = id });

            var resultado = funcionarioFluxo.SalvarFuncionario(funcionario);

            Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IMenuFluxo>().RemoveMenuFuncMemoria(funcionario.IdFunc);

            Config.ResetModulosUsuario(funcionario.IdFunc);

            Config.RemoveMenuUsuario(funcionario.IdFunc);

            if (!resultado)
                MensagemAlerta.ErrorMsg("Falha ao salvar permissões.", resultado);
            else
                MensagemAlerta.ShowMsg("Permissões modificadas com sucesso.", Page);
        }

        protected void SalvaTipoFunc(List<int> idsMenu, List<int> idsFuncaoMenu)
        {
            var funcionarioFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

            // Recupera o funcionário
            var tipoFunc = funcionarioFluxo.ObtemTipoFuncionario(int.Parse(drpTipoFunc.SelectedValue));

            // Limpa os menus e funções que o mesmo tem acesso (Tem que fazer desta forma ao invés de usar o .Clear() para que salve o log corretamente)
            foreach (var configMenu in tipoFunc.ConfigsMenuTipoFunc.ToList())
                if (!idsMenu.Contains(configMenu.IdMenu))
                    tipoFunc.ConfigsMenuTipoFunc.Remove(configMenu);

            foreach (var configFuncao in tipoFunc.ConfigsFuncaoTipoFunc.ToList())
                if (!idsFuncaoMenu.Contains(configFuncao.IdFuncaoMenu))
                    tipoFunc.ConfigsFuncaoTipoFunc.Remove(configFuncao);

            // Adiciona menus e funções associados
            foreach (var id in idsMenu)
                if (!tipoFunc.ConfigsMenuTipoFunc.Any(f => f.IdMenu == id))
                    tipoFunc.ConfigsMenuTipoFunc.Add(new ConfigMenuTipoFunc() { IdTipoFunc = tipoFunc.IdTipoFuncionario, IdMenu = id });

            foreach (var id in idsFuncaoMenu)
                if (!tipoFunc.ConfigsFuncaoTipoFunc.Any(f => f.IdFuncaoMenu == id))
                    tipoFunc.ConfigsFuncaoTipoFunc.Add(new ConfigFuncaoTipoFunc() { IdTipoFunc = tipoFunc.IdTipoFuncionario, IdFuncaoMenu = id });

            var resultado = funcionarioFluxo.SalvarTipoFuncionario(tipoFunc);

            if (!resultado)
                MensagemAlerta.ErrorMsg("Falha ao salvar permissões.", resultado);
            else
                MensagemAlerta.ShowMsg("Permissões modificadas com sucesso. ATENÇÃO! Estas mudanças não são refletidas para os funcionários que tiverem este tipo, o " + 
                    "funcinário terá estas permissões apenas se trocar para este tipo ou se criar um novo funcionário a partir desta alteração.", Page);
        }

        #endregion
    }
}
