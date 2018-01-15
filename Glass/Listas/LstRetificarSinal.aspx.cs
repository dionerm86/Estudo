using System;
using System.Web.UI;
using Glass.Data.DAL;
using System.Linq;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstRetificarSinal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (IsPagtoAntecipado())
                {
                    Page.Title = "Retificar Pagto. Antecipado";
                    lblSinal.Text = "Pagto. Antecipado";
                    btnRetificarSinal.Text = "Retificar Pagto. Antecipado";
                    grdPedidos.Columns[5].HeaderText = "Data Pagto. Antecipado";
    
                    selSinal.TituloTela = "Selecione o Pagto. Antecipado";
                    selSinal.TitulosColunas = "Núm. Pagto. Antecipado|Cliente|Valor|Data Pagto. Antecipado";
                   
                }
    
                hdfPagtoAntecipado.Value = IsPagtoAntecipado().ToString();
                grdPedidos.Columns[6].Visible = !IsPagtoAntecipado();
                grdPedidos.Columns[7].Visible = IsPagtoAntecipado();
            }           

            hdfIdSinal.Value = selSinal.Valor;

            AlterarControleData();
        }
    
        protected void AlterarControleData()
        {
            var pagtos = PagtoSinalDAO.Instance.GetBySinal(Conversoes.StrParaUint(hdfIdSinal.Value));

            if (!pagtos.Any() || !IsPagtoAntecipado() || pagtos.Any(f =>
                 f.IdFormaPagto != (uint)Data.Model.Pagto.FormaPagto.Deposito &&
                 f.IdFormaPagto != (uint)Data.Model.Pagto.FormaPagto.Boleto &&
                 f.IdFormaPagto != (uint)Data.Model.Pagto.FormaPagto.Cartao &&
                 f.IdFormaPagto != (uint)Data.Model.Pagto.FormaPagto.Construcard))
            {
                lblAlterarData.Visible = false;
                ctrlData.Visible = false;
                btnAterarDataMov.Visible = false;
            }
            else
            {
                lblAlterarData.Visible = true;
                ctrlData.Visible = true;
                btnAterarDataMov.Visible = true;
            }
        }

        protected bool IsPagtoAntecipado()
        {
            return Request["antecipado"] == "true";
        }
    
        protected void btnRetificarSinal_Click(object sender, EventArgs e)
        {
            try
            {
                uint idSinal = Glass.Conversoes.StrParaUint(hdfIdSinal.Value);
                SinalDAO.Instance.RetificaSinal(idSinal, hdfIdsPedidos.Value, Request["cxDiario"] == "1");
    
                Glass.MensagemAlerta.ShowMsg((IsPagtoAntecipado() ? "Pagamento antecipado " : "Sinal ") + "retificado com sucesso.", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao retificar sinal.", ex, Page);
            }
        }
    
        protected void grdPedidos_DataBound(object sender, EventArgs e)
        {
            caption.Visible = grdPedidos.Rows.Count > 0;
            btnRetificarSinal.Visible = grdPedidos.Rows.Count > 0;
        }

        protected void btnAterarDataMov_Click(object sender, EventArgs e)
        {
            var data = (TextBox)ctrlData.FindControl("txtData");

            if (string.IsNullOrEmpty(data.Text))
            {
                MensagemAlerta.ShowMsg("Digite uma data válida para redefinição.", Page);
                return;
            }

            SinalDAO.Instance.RetificarDataMovBanco(Conversoes.StrParaUint(hdfIdSinal.Value), ctrlData.Data);
            MensagemAlerta.ShowMsg("Data das movimentações alteradas com sucesso.", Page);            
        }
    }
}
