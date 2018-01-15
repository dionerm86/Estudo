using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaDefinicaoCargaRota : System.Web.UI.Page
    {
        private static List<DefinicaoCargaRota> dataSource;

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.ListaDefinicaoCargaRota));

            if (!IsPostBack)
            {
                txtDataIni.DataString = DateTime.Now.ToString("dd/MM/yyyy");
                txtDataFim.DataString = DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy");

                drpRota.DataBind();
                drpRota.SelectedIndex = 1;

                dataSource = DefinicaoCargaRotaDAO.Instance.ObterDados(Glass.Conversoes.StrParaInt(drpRota.SelectedValue), txtDataIni.DataString, txtDataFim.DataString);
                grdDados.DataSource = dataSource;
                grdDados.DataBind();

                if (dataSource.Count > 0)
                    ObterIndices();
            }

        }
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            dataSource = DefinicaoCargaRotaDAO.Instance.ObterDados(Glass.Conversoes.StrParaInt(drpRota.SelectedValue), txtDataIni.DataString, txtDataFim.DataString);
            grdDados.DataSource = dataSource;
            grdDados.DataBind();

            ObterIndices();
        }
        protected void grdDados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Up")
            {
                int indice = Glass.Conversoes.StrParaInt(e.CommandArgument.ToString());

                DefinicaoCargaRota down = dataSource.Find(new Predicate<DefinicaoCargaRota>(
                delegate (DefinicaoCargaRota x)
                {
                    return x.Indice == indice - 1;
                }));

                DefinicaoCargaRota up = dataSource.Find(new Predicate<DefinicaoCargaRota>(
                delegate (DefinicaoCargaRota x)
                {
                    return x.Indice == indice;
                }));

                up.Indice = up.Indice - 1;
                down.Indice = down.Indice + 1;

                dataSource.Sort(delegate (DefinicaoCargaRota p1, DefinicaoCargaRota p2)
                {
                    return p1.Indice.CompareTo(p2.Indice);
                });

                ObterIndices();

                grdDados.DataSource = dataSource;
                grdDados.DataBind();
            }
            if (e.CommandName == "Down")
            {
                int indice = Glass.Conversoes.StrParaInt(e.CommandArgument.ToString());

                DefinicaoCargaRota down = dataSource.Find(new Predicate<DefinicaoCargaRota>(
                delegate (DefinicaoCargaRota x)
                {
                    return x.Indice == indice;
                }));

                DefinicaoCargaRota up = dataSource.Find(new Predicate<DefinicaoCargaRota>(
                delegate (DefinicaoCargaRota x)
                {
                    return x.Indice == indice + 1;
                }));

                up.Indice = up.Indice - 1;
                down.Indice = down.Indice + 1;

                dataSource.Sort(delegate (DefinicaoCargaRota p1, DefinicaoCargaRota p2)
                {
                    return p1.Indice.CompareTo(p2.Indice);
                });

                ObterIndices();

                grdDados.DataSource = dataSource;
                grdDados.DataBind();
            }
        }

        private void ObterIndices()
        {
            string indices = "";

            foreach (DefinicaoCargaRota d in dataSource)
                indices += d.IdCliente + ",";

            hdfIndice.Value = indices.TrimEnd(',');
        }

        decimal totalM = 0;
        decimal totalPeso = 0;
        decimal entregue = 0;
        decimal pronto = 0;
        decimal pendente = 0;
        decimal etiq = 0;

        protected void grdDados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                totalM += Convert.ToDecimal(((Label)e.Row.Cells[1].FindControl("lblTotalM")).Text);
                totalPeso += Convert.ToDecimal(((Label)e.Row.Cells[1].FindControl("lblTotalPeso")).Text);
                pronto += Convert.ToDecimal(((Label)e.Row.Cells[1].FindControl("lblTotalPronto")).Text);
                entregue += Convert.ToDecimal(((Label)e.Row.Cells[1].FindControl("lblTotalEntregue")).Text);
                pendente += Convert.ToDecimal(((Label)e.Row.Cells[1].FindControl("lblTotalPendente")).Text);
                etiq += Convert.ToDecimal(((Label)e.Row.Cells[1].FindControl("lblTotalEtiq")).Text);
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                Label somaTotal = e.Row.FindControl("lblSomaTotalM") as Label;
                somaTotal.Text = totalM.ToString();

                Label somaTotalPeso = e.Row.FindControl("lblSomaTotalPeso") as Label;
                somaTotalPeso.Text = totalPeso.ToString();

                Label somaTotalPronto = e.Row.FindControl("lblSomaPronto") as Label;
                somaTotalPronto.Text = pronto.ToString();

                Label somaTotalPendente = e.Row.FindControl("lblSomaPendente") as Label;
                somaTotalPendente.Text = pendente.ToString();

                Label somaTotalEntregue = e.Row.FindControl("lblSomaEntregue") as Label;
                somaTotalEntregue.Text = entregue.ToString();

                Label somaTotalEtiq = e.Row.FindControl("lblSomaEtiq") as Label;
                somaTotalEtiq.Text = etiq.ToString();
            }
        }
    }
}
