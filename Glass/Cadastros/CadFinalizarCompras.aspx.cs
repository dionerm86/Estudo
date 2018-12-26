using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadFinalizarCompras : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddDays(-15).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFabrica.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCompra.PageIndex = 0;
        }
    
        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            string comprasOk = "";
    
            try
            {
                DateTime[] datas = ctrlParcelas1.Datas;
                List<uint> idsCompras = new List<uint>();
                var boletoChegou = chkBoletoChegou.Checked;
                var idFormaPagto = Conversoes.StrParaUint(drpFormaPagto.SelectedValue);
                var dataFabrica = ctrlDataFabrica.Data;
                var nf = txtNf.Text;
                var numParc = Conversoes.StrParaInt(drpNumParc.SelectedValue);

                for (int i = 0; i < grdCompra.Rows.Count; i++)
                {
                    CheckBox chkFinalizar = (CheckBox)grdCompra.Rows[i].FindControl("chkFinalizar");
                    if (!chkFinalizar.Checked)
                        continue;
    
                    uint idCompra;
                    if (!uint.TryParse(grdCompra.Rows[i].Cells[1].Text, out idCompra))
                        continue;
    
                    idsCompras.Add(idCompra);
                }
    
                comprasOk = WebGlass.Business.Compra.Fluxo.FinalizarCompra.Instance.FinalizarVarias(idsCompras,
                    datas, numParc, nf, dataFabrica, idFormaPagto, boletoChegou);
    
                Glass.MensagemAlerta.ShowMsg("Compras finalizadas com sucesso!", Page);
            }
            catch (Exception ex)
            {
                string mensagem = comprasOk == "" ? "Falha ao finalizar compras" :
                    "Falha ao finalizar algumas compras. (Compras finalizadas: " + comprasOk.TrimEnd(',', ' ') + ")";
    
                Glass.MensagemAlerta.ErrorMsg(mensagem, ex, Page);
            }
    
            grdCompra.DataBind();
        }

        protected void drpNumParc_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                drpNumParc.Items.Clear();

                for (var i=1; i<= Configuracoes.FinanceiroConfig.Compra.NumeroParcelasCompra; i++)
                    drpNumParc.Items.Add(i.ToString());
            }
        }

        protected void ctrlParcelas1_Load(object sender, EventArgs e)
        {
            ctrlParcelas1.CampoExibirParcelas = hdfExibirParcelas;            ctrlParcelas1.CampoParcelasVisiveis = drpNumParc;
            ctrlParcelas1.NumParcelas = drpNumParc.Items.Count;
        }
    
        protected void ctrlParcelas1_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime[] datas = new DateTime[ctrlParcelas1.NumParcelas];
                for (int i = 0; i < datas.Length; i++)
                    datas[i] = DateTime.Now.AddDays(30 * i);
    
                ctrlParcelas1.Datas = datas;
            }
        }
    }
}
