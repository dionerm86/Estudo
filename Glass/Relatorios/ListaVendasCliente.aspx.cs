using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.RelDAL;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaVendasCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.ListaVendasCliente));
    
            if (!IsPostBack)
            {
                DateTime dataInicio = DateTime.Now;
                DateTime dataFim = DateTime.Now;
    
                drpInicio.SelectedIndex = dataInicio.Month - 1;
                txtInicio.Text = dataInicio.Year.ToString();
    
                drpFim.SelectedIndex = dataFim.Month - 1;
                txtFim.Text = dataFim.Year.ToString();
    
                if (!RotaDAO.Instance.ExisteRota())
                {
                    lblRota.Style.Add("display", "none");
                    drpRota.Style.Add("display", "none");
                    imgPesqRota.Style.Add("display", "none");
                }
            }
        }
    
        #region Métodos AJAX
    
        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (String.IsNullOrEmpty(idCli) || !ClienteDAO.Instance.Exists(Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Conversoes.StrParaUint(idCli));
        }
    
        [Ajax.AjaxMethod()]
        public string GetFunc(string idFunc)
        {
            if (String.IsNullOrEmpty(idFunc) || !FuncionarioDAO.Instance.Exists(Conversoes.StrParaUint(idFunc)))
                return "Erro;Funcionário não encontrado.";
            else
                return "Ok;" + FuncionarioDAO.Instance.GetNome(Conversoes.StrParaUint(idFunc));
        }
    
        #endregion
    
        protected void odsVendas_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            var retorno = e.ReturnValue as List<Vendas>;
    
            if (retorno == null || retorno.Count == 0)
                return;
    
            uint idCliente = !String.IsNullOrEmpty(txtNumCli.Text) ? Conversoes.StrParaUint(txtNumCli.Text) : 0;
            uint idRota = Conversoes.StrParaUint(drpRota.SelectedValue);
            string idsFunc = cblFuncionario.SelectedValue;
            bool revenda = chkRevenda.Checked;
            int mesInicio = Conversoes.StrParaInt(drpInicio.SelectedValue);
            int anoInicio = Conversoes.StrParaInt(txtInicio.Text);
            int mesFim = Conversoes.StrParaInt(drpFim.SelectedValue);
            int anoFim = Conversoes.StrParaInt(txtFim.Text);
            string tipoMedia = cblTipoMedia.SelectedValue;
            uint idLoja = Conversoes.StrParaUint(drpLoja.SelectedValue);
            bool lojaCliente = chkLojaCliente.Checked;
            string tipoCliente = ddlGrupoCliente.SelectedValue;
            int situacaoCliente = drpSituacaoCli.SelectedValue.StrParaInt();

            string[] mesVenda = VendasDAO.Instance.GetMesesVenda(idCliente, txtNome.Text, idRota.ToString(), revenda, 0, null, mesInicio, anoInicio, 
                mesFim, anoFim, tipoMedia, 0, idsFunc, null, idLoja, lojaCliente, tipoCliente, situacaoCliente);
    
            if (mesVenda.Length + 7 != grdVendas.Columns.Count)
            {
                DataControlField cod = grdVendas.Columns[0];
                DataControlField nomeCliente = grdVendas.Columns[1];
                DataControlField vendedor = grdVendas.Columns[2];
                DataControlField media = grdVendas.Columns[3];
                DataControlField total = grdVendas.Columns[grdVendas.Columns.Count - 3];
                DataControlField totM2 = grdVendas.Columns[grdVendas.Columns.Count - 2];
                DataControlField totalItens = grdVendas.Columns[grdVendas.Columns.Count - 1];
                grdVendas.Columns.Clear();
                grdVendas.Columns.Add(cod);
                grdVendas.Columns.Add(nomeCliente);
                grdVendas.Columns.Add(vendedor);
                grdVendas.Columns.Add(media);
                grdVendas.Columns.Add(total);
                grdVendas.Columns.Add(totM2);
                grdVendas.Columns.Add(totalItens);
    
                foreach (string mesAno in mesVenda)
                {
                    int index = grdVendas.Columns.Count - 3;
                    grdVendas.Columns.Insert(index, new TemplateField());
                    grdVendas.Columns[index].HeaderText = mesAno;
                    grdVendas.Columns[index].ItemStyle.Wrap = false;
                }
            }
        }
    
        protected void grdVendas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            Vendas item = e.Row.DataItem as Vendas;
            if (item == null)
                return;
    
            for (int i = 4; i < grdVendas.Columns.Count - 3; i++)
            {
                e.Row.Cells[i].Text = "";
    
                for (int j = 0; j < item.MesVenda.Length; j++)
                {
                    if (item.MesVenda[j] == grdVendas.Columns[i].HeaderText)
                    {
                        e.Row.Cells[i].Text = item.ValorVenda[j].ToString("C");
                        break;
                    }
                }
    
                if (string.IsNullOrEmpty(e.Row.Cells[i].Text))
                    e.Row.Cells[i].Text = 0.ToString("C");
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdVendas.DataBind();
            grdVendas.PageIndex = 0;
        }        
    }
}
