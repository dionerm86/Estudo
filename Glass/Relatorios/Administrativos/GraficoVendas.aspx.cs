using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Glass.Data.Model;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.RelDAL;
using Glass.Data.Helper;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.Collections;
using Sync.Controls;
using System.Linq;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class GraficoVendas : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(GraficoVendas));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Today.AddMonths(-2).ToString("01/MM/yyyy");
                
                var dataFim = DateTime.Parse(DateTime.Today.ToString("01/MM/yyyy")).AddDays(-1);
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = dataFim.ToString("dd/MM/yyyy");
            }
    
            //força as datas a serem sempre no 1o / ultimo dia do mês
            if (IsPostBack)
            {
                var dataIni = "01" + ((TextBox)ctrlDataIni.FindControl("txtData")).Text.Remove(0, 2);
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = dataIni;
    
                var dataFim = DateTime.Parse("01" + ((TextBox)ctrlDataFim.FindControl("txtData")).Text.Remove(0, 2)).AddMonths(1).AddDays(-1);
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = dataFim.ToString("dd/MM/yyyy");
            }
    
            drpAgrupar.Items[1].Text = drpTipoFunc.SelectedItem.Text;
            
            if (!IsPostBack)
                cblTipoPedido.DataBind();
            else
                GeraGrafico();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void drpTipoFunc_SelectedIndexChanged(object sender, EventArgs e)
        {
            odsVendedor.SelectParameters["funcCliente"].DefaultValue = (drpTipoFunc.SelectedValue == "1").ToString();
        }
    
        private void GeraGrafico()
        {
            #region Filtros
    
            var idLoja = drpLoja.SelectedValue.StrParaUint();
            var idVendedor = String.IsNullOrEmpty(drpVendedor.SelectedValue) ? 0 : drpVendedor.SelectedValue.StrParaUint();
            var idCliente = txtNumCli.Text.StrParaUint();
            var nomeCliente = txtNome.Text;
            var tipoFunc = drpTipoFunc.SelectedValue.StrParaInt();
            var tipoPedido = cblTipoPedido.SelectedValue;
            var idRota = drpRota.SelectedValue.StrParaUint();
            var agrupar = drpAgrupar.SelectedValue.StrParaInt();
            var dataInicio = ((TextBox)ctrlDataIni.FindControl("txtData")).Text;
            var dataFinal = ((TextBox)ctrlDataFim.FindControl("txtData")).Text;
    
            #endregion
    
            #region Declarações
    
            //Define o período
            var periodo = new List<string>();
    
            var periodoIni = DateTime.Parse(((TextBox)ctrlDataIni.FindControl("txtData")).Text);
            var periodoFim = DateTime.Parse(((TextBox)ctrlDataFim.FindControl("txtData")).Text).AddDays(1);
    
            var lstValuesX = new ArrayList();
            var lstValuesY = new ArrayList();
    
            var seriesLista = new Dictionary<string, string>();
    
            // Recupera os IDs dos filtros e preenche o hdfSeries
            var ids = new List<uint>();
            var tipoAgrupar = string.Empty;
    
            Series series = null;
    
            #endregion
    
            #region Período
    
            while (periodoIni < periodoFim)
            {
                seriesLista.Add(periodoIni.ToString("MMM-yy"), periodoIni.ToString("MMM-yy") + "|");
                periodo.Add(periodoIni.ToString("MMM-yy"));
                periodoIni = periodoIni.AddMonths(+1);
            }
    
            #endregion
    
            #region Gráfico
    
            //Cria a área do gráfico
            Chart1.Width = 1100;
            Chart1.Height = 300;
            Chart1.ChartAreas.Add("Area");
            Chart1.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.Position;
            Chart1.ChartAreas[0].Position.Height = 90;
            Chart1.ChartAreas[0].Position.Width = 77;
            Chart1.ChartAreas[0].Position.Y = 5;
            Chart1.ChartAreas[0].AxisX.IsMarginVisible = true;
            Chart1.ChartAreas[0].AxisY.IsMarginVisible = true;
            Chart1.ChartAreas[0].AxisY.LabelStyle.Format = "{C}";
            //Titulo
            Chart1.Titles.Add("Vendas");
            Chart1.Titles[0].Alignment = ContentAlignment.TopCenter;
            Chart1.Titles[0].Docking = Docking.Top;
            Chart1.Titles[0].IsDockedInsideChartArea = false;
            Chart1.Titles[0].Position.X = 13;
            Chart1.Titles[0].Position.Y = 30;
            Chart1.Titles[0].Position.Width = 63;
            Chart1.Titles[0].Position.Height = 100;
            Chart1.Titles[0].Font = new Font("Arial", 11, FontStyle.Bold);
            Chart1.ChartAreas[0].AxisX.Title = "Período";
            Chart1.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
            Chart1.ChartAreas[0].AxisX.TitleFont = new Font("Arial", 11, FontStyle.Italic);
    
            Chart1.Legends.Add("legenda");
    
            #endregion
    
            #region Series
    
            switch (agrupar)
            {
                case 0: //nenhum
                    tipoAgrupar = "nenhum";
                    ids.Add(1);
    
                    series = new Series("Empresa");
                    series.ChartType = SeriesChartType.Line;
                    //series.XValueType = ChartValueType.String;
                    series.MarkerStyle = MarkerStyle.Circle;
                    series.MarkerSize = 8;
                    series.MarkerColor = series.BorderColor;
                    series.BorderWidth = 3;
                    //Chart1.Series[i].IsValueShownAsLabel = true;
                    series.ToolTip = "#VALX" + Environment.NewLine + series.Name + " : #VALY{C}";
                    series.Legend = "legenda";
                    series.LegendText = "Empresa";
                    //series.IsVisibleInLegend = true;
                    series.LegendToolTip = series.Name;
                    Chart1.Series.Add(series);
    
                    break;
    
                case 1: //loja
                    Loja[] lojas = drpLoja.SelectedValue == "0" ? LojaDAO.Instance.GetAll() : new Loja[] { LojaDAO.Instance.GetElementByPrimaryKey(idLoja) };
                    
                    Chart1.ChartAreas[0].AxisY.Interval = 300000;
                    Chart1.ChartAreas[0].AxisY.IntervalOffset = 0;
    
                    foreach (Loja l in lojas)
                    {
                        ids.Add((uint)l.IdLoja);
    
                        if (Chart1.Series.FindByName(l.IdLoja.ToString()) == null)
                        {
                            series = new Series(l.IdLoja.ToString());
                            series.ChartType = SeriesChartType.Line;
                            //series.XValueType = ChartValueType.String;
                            series.MarkerStyle = MarkerStyle.Circle;
                            series.MarkerSize = 8;
                            series.MarkerColor = series.BorderColor;
                            series.BorderWidth = 3;
                            //Chart1.Series[i].IsValueShownAsLabel = true;
                            series.ToolTip = "#VALX" + Environment.NewLine + l.NomeFantasia + " : #VALY{C}";
                            series.Legend = "legenda";
                            series.LegendText = l.NomeFantasia;
                            //series.IsVisibleInLegend = true;
                            series.LegendToolTip = l.NomeFantasia;
                            Chart1.Series.Add(series);
                        }
                    }
    
                    tipoAgrupar = "loja";
                    break;
    
                case 2: //emissor
    
                    Chart1.ChartAreas[0].Position.Height = 90;
                    Chart1.ChartAreas[0].Position.Width = 72;
                    Chart1.ChartAreas[0].Position.Y = 5;
    
                    Funcionario[] funcs;
                    if (drpVendedor.SelectedValue == "0")
                    {
                        var list = new List<Funcionario>(FuncionarioDAO.Instance.GetVendedoresComVendas(idLoja,
                            tipoFunc == 1, dataInicio, dataFinal, 15, 0, false));
                        list.RemoveAt(0);
                        funcs = list.ToArray();
                    }
                    else
                    {
                        funcs = new [] { FuncionarioDAO.Instance.GetElementByPrimaryKey(idVendedor) };
                    }
    
                    foreach (var f in funcs)
                    {
                        ids.Add((uint)f.IdFunc);
    
                        if (Chart1.Series.FindByName(f.IdFunc.ToString()) == null)
                        {
                            series = new Series(f.IdFunc.ToString());
                            series.ChartType = SeriesChartType.Line;
                            series.XValueType = ChartValueType.String;
                            series.MarkerStyle = MarkerStyle.Circle;
                            series.MarkerSize = 8;
                            series.MarkerColor = series.BorderColor;
                            series.BorderWidth = 3;
                            series.ToolTip = "#VALX" + Environment.NewLine + f.Nome + " : #VALY{C}";
                            series.Legend = "legenda";
                            series.LegendText = f.Nome;
                            series.LegendToolTip = f.Nome;
    
                            Chart1.Series.Add(series);
                        }
                    }
    
                    tipoAgrupar = "emissor";
                    break;
    
                case 3: // cliente
                    Cliente[] cli;
                    if (idCliente == 0)
                        cli = ClienteDAO.Instance.GetClientesVendas(idLoja, dataInicio, dataFinal);
                    else
                        cli = new [] { ClienteDAO.Instance.GetElementByPrimaryKey(idCliente) };
    
                    Chart1.ChartAreas[0].Position.Height = 90;
                    Chart1.ChartAreas[0].Position.Width = 72;
                    Chart1.ChartAreas[0].Position.Y = 5;
    
                    foreach (Cliente c in cli)
                    {
                        var descNomeCliente = Configuracoes.ClienteConfig.ExibirRazaoSocialGraficoVendasCurvaABC ?
                            (!string.IsNullOrEmpty(c.Nome) ? c.Nome : c.NomeFantasia) : (!string.IsNullOrEmpty(c.NomeFantasia) ? c.NomeFantasia : c.Nome);

                        if (Chart1.Series.FindByName(c.IdCli.ToString()) == null)
                        {
                            series = new Series(c.IdCli.ToString());
                            series.ChartType = SeriesChartType.Line;
                            series.XValueType = ChartValueType.String;
                            series.MarkerStyle = MarkerStyle.Circle;
                            series.MarkerSize = 8;
                            series.MarkerColor = series.BorderColor;
                            series.BorderWidth = 3;
                            series.ToolTip = "#VALX" + Environment.NewLine + descNomeCliente + " : #VALY{C}";
                            series.Legend = "legenda";
                            series.LegendText = descNomeCliente;
                            series.LegendToolTip = descNomeCliente;
    
                            Chart1.Series.Add(series);
                        }
                        ids.Add((uint)c.IdCli);
                    }
    
                    tipoAgrupar = "cliente";
                    break;
    
                case 4: // tipoPedido
                    Chart1.ChartAreas[0].AxisY.Interval = 300000;
                    Chart1.ChartAreas[0].AxisY.IntervalOffset = 0;
                    Chart1.ChartAreas[0].Position.Height = 90;
                    Chart1.ChartAreas[0].Position.Width = 72;
                    Chart1.ChartAreas[0].Position.Y = 5;
    
                    foreach (string tp in tipoPedido.Split(','))
                    {
                        string descricaoTipoPedido = tp == "1" ? "Venda" : tp == "2" ? "Revenda" : tp == "3" ? "Mão de obra" : "Produção";
                        series = new Series(descricaoTipoPedido);
                        series.ChartType = SeriesChartType.Line;
                        series.XValueType = ChartValueType.String;
                        series.MarkerStyle = MarkerStyle.Circle;
                        series.MarkerSize = 8;
                        series.MarkerColor = series.BorderColor;
                        series.BorderWidth = 3;
                        series.ToolTip = "#VALX" + Environment.NewLine + descricaoTipoPedido + " : #VALY{C}";
                        series.Legend = "legenda";
                        series.LegendText = descricaoTipoPedido;
                        //series.IsVisibleInLegend = true;
                        series.LegendToolTip = descricaoTipoPedido;
    
                        Chart1.Series.Add(series);
    
                        ids.Add(Glass.Conversoes.StrParaUint(tp));
                    }
    
                    tipoAgrupar = "tipoPedido";
                    break;
                case 5: // rota
                    Rota[] rotas;
                    if(idRota == 0)
                    {
                        rotas = RotaDAO.Instance.ObterRotas().ToArray();
                    }
                    else
                    {
                        rotas = new[] { RotaDAO.Instance.GetElement((uint)idRota) };
                    }

                    Chart1.ChartAreas[0].Position.Height = 90;
                    Chart1.ChartAreas[0].Position.Width = 72;
                    Chart1.ChartAreas[0].Position.Y = 5;

                    foreach (var r in rotas)
                    {
                        if (Chart1.Series.FindByName(r.IdRota.ToString()) == null)
                        {
                            series = new Series(r.IdRota.ToString());
                            series.ChartType = SeriesChartType.Line;
                            series.XValueType = ChartValueType.String;
                            series.MarkerStyle = MarkerStyle.Circle;
                            series.MarkerSize = 8;
                            series.MarkerColor = series.BorderColor;
                            series.BorderWidth = 3;
                            series.ToolTip = "#VALX" + Environment.NewLine + r.Descricao + " : #VALY{C}";
                            series.Legend = "legenda";
                            series.LegendText = r.Descricao;
                            series.LegendToolTip = r.Descricao;

                            Chart1.Series.Add(series);
                        }
                        ids.Add((uint)r.IdRota);
                    }

                    tipoAgrupar = "rota";
                    break;
            }

            #endregion

            #region Dados

            var login = UserInfo.GetUserInfo;
            var cliente = login.IsCliente;
            var administrador = login.IsAdministrador;
            var emitirGarantiaReposicao = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao);
            var emitirPedidoFuncionario = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoFuncionario);

            var dados = ChartVendasDAO.Instance.GetVendasForChart(idLoja, tipoFunc, idVendedor, idCliente, nomeCliente,
                idRota, dataInicio, dataFinal, tipoPedido, agrupar, tipoAgrupar, ids,
                cliente, administrador, emitirGarantiaReposicao, emitirPedidoFuncionario);
    
            foreach (var entry in dados)
                foreach (var ch in entry.Value)
                    seriesLista[ch.Periodo] += ch.TotalVenda + "|";
    
            var listaVendas = new List<ChartVendas>();
    
            foreach (var itens in dados)
                foreach (var c in itens.Value)
                    listaVendas.Add(c);
    
            foreach (var s in Chart1.Series)
            {
                lstValuesX = new ArrayList();
                lstValuesY = new ArrayList();
    
                for (int i = 0; i < listaVendas.Count; i++)
                {
                    switch (agrupar)
                    {
                        case 0:
                            lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                            lstValuesY.Add(listaVendas[i].TotalVenda);
                            break;
                        case 1:

                            if (listaVendas[i].IdLoja.ToString() == s.Name)
                            {
                                lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                                lstValuesY.Add(listaVendas[i].TotalVenda);
                            }

                            break;
                        case 2:

                            if (listaVendas[i].IdFunc.ToString() == s.Name)
                            {
                                lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                                lstValuesY.Add(listaVendas[i].TotalVenda);
                            }

                            break;
                        case 3:

                            if (listaVendas[i].IdCliente.ToString() == s.Name)
                            {
                                lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                                lstValuesY.Add(listaVendas[i].TotalVenda);
                            }

                            break;
                        case 4:

                            string descricaoTipoPedido = listaVendas[i].TipoPedido == 1 ? "Venda" : listaVendas[i].TipoPedido == 2 ? "Revenda" :
                                listaVendas[i].TipoPedido == 3 ? "Mão de obra" : "Produção";

                            if (descricaoTipoPedido == s.Name)
                            {
                                lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                                lstValuesY.Add(listaVendas[i].TotalVenda);
                            }

                            break;
                        case 5:

                            if (listaVendas[i].IdRota.ToString() == s.Name)
                            {
                                lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                                lstValuesY.Add(listaVendas[i].TotalVenda);
                            }

                            break;
                    }
                }
    
                s.Points.DataBindXY(lstValuesX, lstValuesY);
            }
    
            #endregion
    
            var buffer = Util.Helper.ChartToByteArray(Chart1);
    
            hdfTempFile.Value = Conversoes.CodificaPara64(buffer); //String.Concat("file:///", Glass.Util.Helper.SalvaGraficoTemp(Chart1, "Vendas" + ((TextBox)ctrlDataIni.FindControl("txtData")).Text.Replace("/", "") + ((TextBox)ctrlDataFim.FindControl("txtData")).Text.Replace("/", "")));
    
            #region Apos gerar imagem
    
            foreach (var s in Chart1.Series)
            {
                lstValuesX = new ArrayList();
                lstValuesY = new ArrayList();
    
                for (var i = 0; i < listaVendas.Count; i++)
                {
                    switch (agrupar)
                    {
                        case 0:
                            lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                            lstValuesY.Add(listaVendas[i].TotalVenda);
                            break;
                        case 1:
    
                            if (listaVendas[i].IdLoja.ToString() == s.Name)
                            {
                                lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                                lstValuesY.Add(listaVendas[i].TotalVenda);
                            }
    
                            break;
    
                        case 2:
    
                            if (listaVendas[i].IdFunc.ToString() == s.Name)
                            {
                                lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                                lstValuesY.Add(listaVendas[i].TotalVenda);
                            }
    
                            break;
    
                        case 3:
    
                            if (listaVendas[i].IdCliente.ToString() == s.Name)
                            {
                                lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                                lstValuesY.Add(listaVendas[i].TotalVenda);
                            }
    
                            break;
    
                        case 4:
    
                            var descricaoTipoPedido = listaVendas[i].TipoPedido == 1 ? "Venda" : listaVendas[i].TipoPedido == 2 ? "Revenda" :
                                listaVendas[i].TipoPedido == 3 ? "Mão de obra" : "Produção";
    
                            if (descricaoTipoPedido == s.Name)
                            {
                                lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                                lstValuesY.Add(listaVendas[i].TotalVenda);
                            }
    
                            break;

                        case 5:

                            if (listaVendas[i].IdRota.ToString() == s.Name)
                            {
                                lstValuesX.Add(Convert.ToDateTime(listaVendas[i].Periodo).ToString("MMM-yy"));
                                lstValuesY.Add(listaVendas[i].TotalVenda);
                            }

                            break;
                    }
                }
    
                s.Points.DataBindXY(lstValuesX, lstValuesY);
            }
    
            #endregion
    
            #region Grid
    
            Grid(dados, seriesLista, agrupar);
    
            #endregion
        }
    
        private void Grid(Dictionary<uint, List<ChartVendas>> vendas, Dictionary<string, string> series, int agrupar)
        {
            #region DataTable -> Gridview
    
            if (vendas.Count > 0)
            {
                DataTable dt = new DataTable();
    
                #region Colunas
    
                DataColumn dcol = new DataColumn(agrupar == 0 ? "Empresa" : agrupar == 1 ? "Loja" : agrupar == 2 ? "Vendedor" : agrupar == 3 ? "Cliente" : agrupar == 4 ? "Tipo Pedido" : "Rota", typeof(System.String));
                dt.Columns.Add(dcol);
    
                Dictionary<int, List<decimal>> dictTotais = new Dictionary<int, List<decimal>>();
                int totalIndex = 0;
                foreach (KeyValuePair<string, string> entry in series)
                {
                    DataColumn dcolserie = new DataColumn(entry.Key, typeof(System.String));
                    dt.Columns.Add(dcolserie);
                    dictTotais.Add(totalIndex, new List<decimal>());
                    totalIndex++;
                }
    
                DataColumn dcoltotal = new DataColumn("Total", typeof(System.String));
                dt.Columns.Add(dcoltotal);
                dictTotais.Add(totalIndex, new List<decimal>());
    
                #endregion
    
                foreach (KeyValuePair<uint, List<ChartVendas>> entry in vendas.OrderByDescending(f => f.Value.Sum(x => x.TotalVenda)))
                {
                    DataRow dr = dt.NewRow();
                    dr[agrupar == 0 ? "Empresa" : agrupar == 1 ? "Loja" : agrupar == 2 ? "Vendedor" : agrupar == 3 ? "Cliente" : agrupar == 4 ? "Tipo Pedido" : "Rota"] =
                        agrupar == 0 ? "Empresa" : agrupar == 1 ? entry.Value[0].NomeLoja : agrupar == 2 ? entry.Value[0].NomeVendedor : agrupar == 3 ? entry.Value[0].NomeCliente : agrupar == 4 ?
                        (entry.Value[0].TipoPedido == 1 ? "Venda" : entry.Value[0].TipoPedido == 2 ? "Revenda" : entry.Value[0].TipoPedido == 3 ? "Mão de obra" : "Produção") : entry.Value[0].DescricaoRota;
    
                    decimal total = 0;
                    for (int i = 0; i < series.Count; i++)
                    {
                        total += entry.Value[i].TotalVenda;
                        dictTotais[i].Add(entry.Value[i].TotalVenda);
                        dr[entry.Value[i].Periodo] = entry.Value[i].TotalVenda.ToString("C");
                    }
    
                    dr["Total"] = total.ToString("C");
                    dictTotais[series.Count].Add(total);
    
                    dt.Rows.Add(dr);
                }

                DataRow drTotais = dt.NewRow();
                drTotais[agrupar == 0 ? "Empresa" : agrupar == 1 ? "Loja" : agrupar == 2 ? "Vendedor" : agrupar == 3 ? "Cliente" : agrupar == 4 ? "Tipo Pedido" : "Rota"] = "Total";
    
                for (var i = 0; i <= series.Count; i++)
                {
                    decimal total = 0;
                    foreach (var d in dictTotais[i])
                        total += d;
    
                    drTotais[(i + 1)] = total.ToString("C");
                }
    
                if (agrupar > 0)
                    dt.Rows.Add(drTotais);
    
                foreach (DataColumn dc in dt.Columns)
                {
                    BoundField bfield = new BoundField();
                    bfield.DataField = dc.ColumnName;
                    bfield.HeaderText = dc.ColumnName;
                    grdVendas.Columns.Add(bfield);
                }
    
                grdVendas.DataSource = dt;
                grdVendas.DataBind();
            }
    
            #endregion
        }
    
        protected void cblTipoPedido_DataBound(object sender, EventArgs e)
        {
            // Define como filtro padrão pedidos de Venda/Revenda e Mão de Obra
            foreach (ListItem li in ((CheckBoxListDropDown)sender).Items)
            {
                switch (li.Value.StrParaUint())
                {
                    case (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda:
                    case (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda:
                    case (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial:
                        li.Selected = true;
                        break;
                }
            }

            GeraGrafico();
        }
    }
}
