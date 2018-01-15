using Glass.Data.Helper;
using Glass.Global.Negocios;
using Glass.Global.Negocios.Entidades;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios.Dinamicos
{
    public partial class ListaDinamico : System.Web.UI.Page
    {
        #region Variáveis Locais

        private IRelatorioDinamicoFluxo _relatorioDinamicoFluxo;
        private RelatorioDinamico _relatorio;

        #endregion

        #region Métodos Públicos / Protegidos

        protected void Page_Load(object sender, EventArgs e)
        {
            // Recupera a instância do fluxo do relatório dinâmico
            _relatorioDinamicoFluxo = ServiceLocator.Current.GetInstance<IRelatorioDinamicoFluxo>();
            _relatorio = _relatorioDinamicoFluxo.ObterRelatorioDinamico(IdRelatorioDinamico());

            //Se não achar o relatorio volta pra pagina inicial
            if (_relatorio == null)
                Response.Redirect("~/webglass/Main.aspx");

            // Informa o título da página
            Page.Title = _relatorio.NomeRelatorio;

            //Define o pageSize
            if (_relatorio.QuantidadeRegistrosPorPagina > 0)
                grdDinamico.PageSize = _relatorio.QuantidadeRegistrosPorPagina;

            //Monta os filtros
            MontarFiltros();

            //Monta o link de inserção
            MontaLinkInsercao();

            if (!IsPostBack)
            {
                //Monta a grid com os registros
                PopulaGrid();

                // Esconde opção de imprimir se o relatório não tiver rdlc
                if (!System.IO.File.Exists(Server.MapPath(string.Format("~/Upload/RelatorioDinamico/{0}.rdlc", IdRelatorioDinamico()))))
                {
                    lnkImprimir.Visible = false;
                    lnkExportarExcel.Visible = false;
                }
            }
        }

        protected void lnkImprimir_Click(object sender, EventArgs e)
        {
            ImprimirExportarExcel(false);
        }

        protected void lnkExportarExcel_Click(object sender, EventArgs e)
        {
            ImprimirExportarExcel(true);
        }

        protected void grdDinamico_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdDinamico.PageIndex = e.NewPageIndex;
            PopulaGrid();
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Monta a grid com o sql cadastrado
        /// </summary>
        private void PopulaGrid()
        {
            // Executa o comando sql salvo e recupera os registros
            var lista = _relatorioDinamicoFluxo.PesquisarListaDinamica(IdRelatorioDinamico(), RecuperarValorFiltros(), grdDinamico.PageIndex, grdDinamico.PageSize);

            DataTable dt = new DataTable();

            if (lista.Count > 0)
            {
                // Adiciona uma coluna vazia para inserir os ícones
                dt.Columns.Add(" ");

                // Cria as colunas com valores do sql na grid
                foreach (var campo in lista.First().Keys)
                    dt.Columns.Add(campo, typeof(string));

                foreach (var item in lista)
                {
                    var dr = dt.NewRow();

                    foreach (var campo in item)
                        dr[campo.Key] = campo.Value;

                    dt.Rows.Add(dr);
                }

                // Adiciona uma coluna vazia para inserir os ícones
                dt.Columns.Add("  ");
            }

            dt.AcceptChanges();
            grdDinamico.DataSource = dt;
            grdDinamico.RowDataBound += GrdDinamico_RowDataBound;
            grdDinamico.DataBind();
        }

        private void GrdDinamico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex == -1)
                return;

            // Busca os ícones do relatório
            var icones = _relatorio.Icones.OrderBy(f => f.NumSeq);

            // Insere ícones
            foreach (var item in icones)
            {
                var imgBtn = new ImageButton();
                imgBtn.ImageUrl = item.ImagemIcone;

                // Substitui os valores do javascript e do método de visibilidade pelos valores desta linha
                var funcaoJavascript = item.FuncaoJavaScript;
                var metodoVisibilidade = item.MetodoVisibilidade ?? "";

                foreach (TableCell campo in e.Row.Cells)
                {
                    // Ignora a primeira coluna, pois é a coluna onde irão aparecer os ícones
                    if (e.Row.Cells.GetCellIndex(campo) == 0 || e.Row.Cells.GetCellIndex(campo) == e.Row.Cells.Count -1)
                        continue;

                    // Pega a descrição da coluna
                    var descricaoColuna = grdDinamico.HeaderRow.Cells[e.Row.Cells.GetCellIndex(campo)].Text;

                    // Substitui o valor que possa ter no javascript do ícone
                    funcaoJavascript = funcaoJavascript.Replace(string.Format("[{0}]", descricaoColuna), campo.Text);
                    metodoVisibilidade = metodoVisibilidade.Replace(string.Format("[{0}]", descricaoColuna), campo.Text);
                }

                //Substitui as variaveis
                funcaoJavascript = SubistituiVariaveis(funcaoJavascript);
                metodoVisibilidade = SubistituiVariaveis(metodoVisibilidade);

                imgBtn.ToolTip = item.NomeIcone;
                imgBtn.OnClientClick = funcaoJavascript;

                //Se tiver sido informado uma método de visibilidade do icone executa e seta a visibilidade
                if (!string.IsNullOrWhiteSpace(metodoVisibilidade))
                    imgBtn.Visible = ExecutaMetodoVisibilidade(metodoVisibilidade);

                if (item.MostrarFinalGrid)
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(imgBtn);
                else
                    e.Row.Cells[0].Controls.Add(imgBtn);
            }
        }

        /// <summary>
        /// Cria as opções que são preenchidas na lista de seleção e/ou no controle de múltipla seleção
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private ListItem[] CriarOpcoes(string filtro)
        {
            // Se for sql, executa no banco e recupera os resultados
            if (filtro.Length > 4 && filtro.Substring(0, 4).ToLower() == "sql:")
            {
                return _relatorioDinamicoFluxo.ObterFiltros(filtro.Remove(0, 4))
                    .Select(f => new ListItem(f.Item1, f.Item2)).ToArray();
            }
            // Se for opções fixas, apenas monta uma lista e retorna
            else
            {
                var listaRetorno = new List<ListItem>();

                var opcoes = filtro.Split('|');
                foreach (var item in opcoes)
                {
                    var opcao = item.Split(',');
                    listaRetorno.Add(new ListItem(opcao[0], opcao[1]));
                }

                return listaRetorno.ToArray();
            }
        }

        /// <summary>
        /// Monta os filtros na tela
        /// </summary>
        private void MontarFiltros()
        {
            var listaTr = new List<TableRow>();
            var listaTd = new List<TableCell>();

            foreach (var filtro in _relatorio.Filtros.OrderBy(f => f.NumSeq))
            {
                // Cria o label com a descrição do filtro
                var tdLabel = new TableCell();
                var lbl = new Label();
                lbl.ID = string.Format("lbl{0}", filtro.NomeColunaSql);
                lbl.ForeColor = System.Drawing.ColorTranslator.FromHtml("#0066FF");
                lbl.Text = filtro.NomeFiltro;
                tdLabel.Controls.Add(lbl);
                listaTd.Add(tdLabel);

                // Cria o controle
                switch (filtro.TipoControle)
                {
                    case Data.Model.TipoControle.Numero:
                        {
                            var tdControl = new TableCell();
                            var txt = new TextBox();
                            txt.ID = string.Format("txt{0}", filtro.NomeColunaSql);
                            txt.Attributes.Add("onkeypress", "return soNumeros(event, true, true);");

                            // Define valor padrão
                            if (!IsPostBack && !string.IsNullOrEmpty(filtro.ValorPadrao))
                                txt.Text = filtro.ValorPadrao;

                            tdControl.Controls.Add(txt);
                            listaTd.Add(tdControl);
                            break;
                        }
                    case Data.Model.TipoControle.Texto:
                        {
                            var tdControl = new TableCell();
                            var txt = new TextBox();
                            txt.ID = string.Format("txt{0}", filtro.NomeColunaSql);

                            // Define valor padrão
                            if (!IsPostBack && !string.IsNullOrEmpty(filtro.ValorPadrao))
                                txt.Text = filtro.ValorPadrao;

                            tdControl.Controls.Add(txt);
                            listaTd.Add(tdControl);
                            break;
                        }
                    case Data.Model.TipoControle.ListaDeSelecao:
                    case Data.Model.TipoControle.Ordenacaoo:
                    case Data.Model.TipoControle.Agrupamento:
                        {
                            var tdControl = new TableCell();
                            var drp = new DropDownList();
                            drp.ID = string.Format("drp{0}", filtro.NomeColunaSql);
                            drp.Items.Add(new ListItem("", ""));
                            drp.Items.AddRange(CriarOpcoes(filtro.Opcoes));

                            // Define valor padrão
                            if (!IsPostBack && !string.IsNullOrEmpty(filtro.ValorPadrao))
                                drp.SelectedValue = filtro.ValorPadrao;

                            tdControl.Controls.Add(drp);
                            listaTd.Add(tdControl);
                            break;
                        }
                    case Data.Model.TipoControle.PeriodoDeData:
                    case Data.Model.TipoControle.PeriodoDeDataHora:
                    case Data.Model.TipoControle.Data:
                    case Data.Model.TipoControle.DataHora:
                        {
                            var tdControl = new TableCell();

                            #region Data inicial

                            var ctrlDataIni = (Controls.ctrlData)Page.LoadControl("~/Controls/ctrlData.ascx");
                            ctrlDataIni.ID = string.Format("ctd{0}Ini", filtro.NomeColunaSql.Replace(".",""));
                            ctrlDataIni.ReadOnly = Web.Controls.ctrlData.ReadOnlyEnum.ReadWrite;
                            ctrlDataIni.Enabled = true;

                            //Define se ira mostrar a hora ou não
                            if (filtro.TipoControle == Data.Model.TipoControle.PeriodoDeDataHora || filtro.TipoControle == Data.Model.TipoControle.DataHora)
                                ctrlDataIni.ExibirHoras = true;

                            // Define valor padrão
                            if (!IsPostBack && !string.IsNullOrWhiteSpace(filtro.ValorPadrao) && !string.IsNullOrWhiteSpace(filtro.ValorPadrao.Split(',')[0]))
                                ctrlDataIni.Data = CalculaExpressaoData(filtro.ValorPadrao.Split(',')[0]);

                            tdControl.Controls.Add(ctrlDataIni);

                            #endregion

                            #region Data final

                            if (filtro.TipoControle == Data.Model.TipoControle.PeriodoDeData || filtro.TipoControle == Data.Model.TipoControle.PeriodoDeDataHora)
                            {
                                var ctrlDataFim = (Controls.ctrlData)Page.LoadControl("~/Controls/ctrlData.ascx");
                                ctrlDataFim.ID = string.Format("ctd{0}Fim", filtro.NomeColunaSql.Replace(".", ""));
                                ctrlDataFim.ReadOnly = Web.Controls.ctrlData.ReadOnlyEnum.ReadWrite;
                                ctrlDataFim.Enabled = true;

                                // Define se ira mostrar a hora ou não
                                if (filtro.TipoControle == Data.Model.TipoControle.PeriodoDeDataHora)
                                    ctrlDataFim.ExibirHoras = true;

                                // Define valor padrão
                                if (!IsPostBack && !string.IsNullOrEmpty(filtro.ValorPadrao) && filtro.ValorPadrao.Split(',').Length > 1 && !string.IsNullOrWhiteSpace(filtro.ValorPadrao.Split(',')[1]))
                                    ctrlDataFim.Data = CalculaExpressaoData(filtro.ValorPadrao.Split(',')[1]).AddHours(23).AddMinutes(59);

                                tdControl.Controls.Add(ctrlDataFim);
                            }

                            #endregion

                            listaTd.Add(tdControl);
                            break;
                        }
                    case Data.Model.TipoControle.CaixaDeSelecao:
                        {
                            var tdControl = new TableCell();
                            var chk = new CheckBox();
                            chk.ID = string.Format("chk{0}", filtro.NomeColunaSql);

                            // Define valor padrão
                            if (!IsPostBack && !string.IsNullOrEmpty(filtro.ValorPadrao))
                                chk.Checked = bool.Parse(filtro.ValorPadrao);

                            tdControl.Controls.Add(chk);
                            listaTd.Add(tdControl);
                            break;
                        }
                    case Data.Model.TipoControle.MultiplaSelecao:
                        {
                            var tdControl = new TableCell();
                            var cbl = new Sync.Controls.CheckBoxListDropDown();
                            cbl.ID = string.Format("cbl{0}", filtro.NomeColunaSql.Replace(".", ""));
                            cbl.Items.AddRange(CriarOpcoes(filtro.Opcoes));

                            // Define valor padrão
                            if (!IsPostBack && !string.IsNullOrEmpty(filtro.ValorPadrao))
                                cbl.SelectedValues = filtro.ValorPadrao.Split(',').Select(f => int.Parse(f)).ToArray();

                            tdControl.Controls.Add(cbl);
                            listaTd.Add(tdControl);
                            break;
                        }
                    case Data.Model.TipoControle.Cliente:
                        {
                            var tdControl = new TableCell();
                            var ctrlPesqCli = (Controls.ctrlPesquisaCliente)Page.LoadControl("~/Controls/ctrlPesquisaCliente.ascx");
                            ctrlPesqCli.ID = string.Format("ctrlPesqCli{0}", filtro.NomeColunaSql.Replace(".", "").Replace(",",""));

                            tdControl.Controls.Add(ctrlPesqCli);
                            listaTd.Add(tdControl);
                            break;
                        }
                }

                #region Lupa de pesquisa

                // Cria a lupa para pesquisar
                var tdPesq = new TableCell();
                var imb = new ImageButton();
                imb.Click += Pesq_Click;
                imb.ImageUrl = "~/Images/Pesquisar.gif";
                imb.ID = string.Format("lnk{0}", filtro.NomeColunaSql);
                tdPesq.Controls.Add(imb);
                listaTd.Add(tdPesq);

                #endregion

                // A cada 4 células de filtro insere uma linha na tabela de filtros
                if (listaTd.Count >= 4 * 3)
                {
                    // Cria uma nova linha e insere as células criadas até então
                    var tr = new TableRow();
                    tr.Cells.AddRange(listaTd.ToArray());
                    listaTr.Add(tr);

                    // Limpa a lista de células
                    listaTd.Clear();
                }
            }
            
            // Insere o restante das células no filtro
            if (listaTd.Count > 0)
            {
                // Cria uma nova linha e insere as células restantes
                var tr = new TableRow();
                tr.Cells.AddRange(listaTd.ToArray());
                listaTr.Add(tr);
            }

            tbFiltros.Rows.AddRange(listaTr.ToArray());
        }

        /// <summary>
        /// Recupera os filtros na tela com seus respectivos valores
        /// </summary>
        /// <returns></returns>
        private List<Tuple<RelatorioDinamicoFiltro, string>> RecuperarValorFiltros()
        {
            var filtros = new List<Tuple<RelatorioDinamicoFiltro, string>>();

            // Pega o valor dos filtros na tela 
            foreach (var filtro in _relatorio.Filtros)
            {
                var valorFiltro = string.Empty;

                switch (filtro.TipoControle)
                {
                    case Data.Model.TipoControle.Numero:
                    case Data.Model.TipoControle.Texto:
                        {
                            valorFiltro = ((TextBox)tbFiltros.FindControl(string.Format("txt{0}", filtro.NomeColunaSql))).Text;
                            break;
                        }

                    case Data.Model.TipoControle.ListaDeSelecao:
                    case Data.Model.TipoControle.Agrupamento:
                    case Data.Model.TipoControle.Ordenacaoo:
                        {
                            valorFiltro = ((DropDownList)tbFiltros.FindControl(string.Format("drp{0}", filtro.NomeColunaSql))).SelectedValue;
                            break;
                        }

                    case Data.Model.TipoControle.PeriodoDeData:
                    case Data.Model.TipoControle.PeriodoDeDataHora:
                    case Data.Model.TipoControle.Data:
                    case Data.Model.TipoControle.DataHora:
                        {
                            var dataIni = ((Controls.ctrlData)tbFiltros.FindControl(string.Format("ctd{0}Ini", filtro.NomeColunaSql.Replace(".","")))).DataString;
                            var dataFim = "";

                            if (filtro.TipoControle == Data.Model.TipoControle.PeriodoDeData || filtro.TipoControle == Data.Model.TipoControle.PeriodoDeDataHora)
                                dataFim = ((Controls.ctrlData)tbFiltros.FindControl(string.Format("ctd{0}Fim", filtro.NomeColunaSql.Replace(".", "")))).DataString;

                            valorFiltro = string.Format("{0}|{1}", dataIni, dataFim);
                            break;
                        }

                    case Data.Model.TipoControle.CaixaDeSelecao:
                        {
                            var selecionado = ((CheckBox)tbFiltros.FindControl(string.Format("chk{0}", filtro.NomeColunaSql))).Checked;

                            if (selecionado) valorFiltro = "1";
                            break;
                        }

                    case Data.Model.TipoControle.MultiplaSelecao:
                        {
                            foreach (var item in ((Sync.Controls.CheckBoxListDropDown)tbFiltros.FindControl(string.Format("cbl{0}", filtro.NomeColunaSql.Replace(".", "")))).Items)
                                if (((ListItem)item).Selected)
                                    valorFiltro += ((ListItem)item).Value + "|";

                            valorFiltro = valorFiltro.TrimEnd('|');
                            break;
                        }

                    case Data.Model.TipoControle.Cliente:
                        {
                            var ctrlPesqCli = ((Controls.ctrlPesquisaCliente)tbFiltros.FindControl(string.Format("ctrlPesqCli{0}", filtro.NomeColunaSql.Replace(".", "").Replace(",", ""))));

                            valorFiltro = string.Format("{0}|{1}", ctrlPesqCli.IdCliente, ctrlPesqCli.NomeCliente);

                            break;
                        }
                }

                filtros.Add(new Tuple<RelatorioDinamicoFiltro, string>(filtro, valorFiltro));
            }

            return filtros;
        }

        private void Pesq_Click(object sender, ImageClickEventArgs e)
        {
            grdDinamico.PageIndex = 0;
            PopulaGrid();
        }

        private void ImprimirExportarExcel(bool exportarExcel)
        {
            // Recupera o valor preenchido nos filtros
            var filtros = RecuperarValorFiltros();

            // Variáveis para controlar as variáveis a serem passadas por queryString
            var queryString = string.Empty;

            // Monta uma queryString com todos os filtros realizados
            foreach (var filtro in filtros)
                queryString += string.Format("&Filtro_{0}={1}", filtro.Item1.IdRelatorioDinamicoFiltro, filtro.Item2);

            // Abre o relatório passando os filtros selecionados
            Page.ClientScript.RegisterClientScriptBlock(typeof(ListaDinamico), "rpt",
                string.Format("openWindow(600, 800, 'RelDinamico.aspx?id={0}&exportarExcel={1}{2}'); ", IdRelatorioDinamico(), exportarExcel.ToString().ToLower(), queryString), true);
        }

        private int IdRelatorioDinamico()
        {
            return Request["id"].StrParaInt();
        }

        private void MontaLinkInsercao()
        {
            if(!string.IsNullOrWhiteSpace(_relatorio.LinkInsercaoNome) && !string.IsNullOrWhiteSpace(_relatorio.LinkInsersaoUrl))
            {
                lnkInsercao.Text = _relatorio.LinkInsercaoNome;
                lnkInsercao.Click += LnkInsercao_Click;
            }
        }

        private void LnkInsercao_Click(object sender, EventArgs e)
        {
            Response.Redirect(_relatorio.LinkInsersaoUrl);
        }

        private bool ExecutaMetodoVisibilidade(string metodoVisibilidade)
        {
            var mtdVisArr = metodoVisibilidade.Split(';');
            var nsInterface = string.Format("{0}, {1}", mtdVisArr[0].Trim(), mtdVisArr[1].Trim());
            var nomeMetodo = mtdVisArr[2].Split('(')[0];
            var parametrosValores = mtdVisArr[2].Split('(')[1].Trim(')').Split(',');

            //busca o tipo da interface informada.
            var iType = Type.GetType(nsInterface);

            //cria a instância para chamar o método
            var instance = ServiceLocator.Current.GetInstance(iType);

            //Busca o método
            var mtd = iType.GetMethod(nomeMetodo);

            //Busca os parametros do método
            var mtdParametros = mtd.GetParameters();

            //Cria o array de parametros para ser informado na execução do método
            var parametros = new object[mtdParametros.Length];

            //Converte os valores dos parametros informados
            for (int i = 0; i < mtdParametros.Length; i++)
                parametros[i] = Convert.ChangeType(parametrosValores[i], mtdParametros[i].ParameterType);

            //Executa o método e retorna a visibilidade
            return (bool)mtd.Invoke(instance, parametros);
        }

        private DateTime CalculaExpressaoData(string expressao)
        {
            expressao = expressao.ToLower().Trim();
            var data = DateTime.Now.Date;

            if (expressao.Equals("primeirodiames"))
                return data.ObtemPrimeiroDiaMesAtual();
            else if (expressao.Equals("ultimodiames"))
                return data.ObtemUltimoDiaMesAtual();

            var valor = expressao.Substring(0, expressao.Length - 1).StrParaInt();
            var periodo = expressao[expressao.Length - 1];

            switch (periodo)
            {
                case 'd':
                    return data.AddDays(valor);

                case 's':
                    return data.AddDays(valor * 7);

                case 'm':
                    return data.AddMonths(valor);

                case 'a':
                    return data.AddYears(valor);

                default:
                    return data;
            }
        }

        private string SubistituiVariaveis(string expressao)
        {
            var matchVariaveis = Regex.Match(expressao, @"{@\w+}");

            while (matchVariaveis.Success)
            {
                var variavel = matchVariaveis.Value;

                var matchNomeVariavel = Regex.Match(matchVariaveis.Value, @"\w+");

                if (!matchNomeVariavel.Success)
                    continue;

                switch (matchNomeVariavel.Value.ToLower())
                {
                    case "idreldinamico":
                        {
                            expressao = expressao.Replace(variavel, IdRelatorioDinamico().ToString());
                            break;
                        }

                    case "idfunc":
                        {
                            expressao = expressao.Replace(variavel, UserInfo.GetUserInfo.CodUser.ToString());
                            break;
                        }

                    default:
                        break;
                }

                matchVariaveis = matchVariaveis.NextMatch();
            }

            return expressao;
        }

        #endregion
    }
}