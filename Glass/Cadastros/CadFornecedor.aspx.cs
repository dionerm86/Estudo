using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadFornecedor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFornecedor))
            {
                Response.Redirect("~/WebGlass/Main.aspx");
                return;
            }

            if (Request["idFornec"] != null)
            {
                hdfIdFornec.Value = Request["idFornec"];
                dtvFornecedor.ChangeMode(DetailsViewMode.Edit);
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadFornecedor));
    
            if (!ExibirUrlSistema())
            {
                dtvFornecedor.Fields[28].HeaderStyle.CssClass = "escondeUrl";
                dtvFornecedor.Fields[28].ItemStyle.CssClass = "escondeUrl";
            }
    
            if (!ExibirDataVigenciaPreco())
            {
                dtvFornecedor.Fields[29].HeaderStyle.CssClass = "escondeUrl";
                dtvFornecedor.Fields[29].ItemStyle.CssClass = "escondeUrl";
            }
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstFornecedor.aspx");
        }
    
        protected void odsFornecedor_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                /* Glass.UI.Web.Controls.ctrlParcelasUsar ctrlParcelasUsar = dtvFornecedor.FindControl("ctrlParcelasUsar1") as Glass.UI.Web.Controls.ctrlParcelasUsar;
                if (ctrlParcelasUsar != null)
                    ctrlParcelasUsar.SalvarParcelas(null, (e.ReturnValue.ToString()).StrParaInt()); */
    
                if (Request["popup"] != null)
                {
                    uint idFornec = Glass.Conversoes.StrParaUint(e.ReturnValue.ToString());
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "busca", "window.opener.setFornec(" + idFornec + ",'" + 
                        FornecedorDAO.Instance.GetNome(idFornec).Replace("'", "") + "'); closeWindow();", true);
                }
                else
                    Response.Redirect("../Listas/LstFornecedor.aspx");
            }
        }
    
        protected void odsFornecedor_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                /* Glass.UI.Web.Controls.ctrlParcelasUsar ctrlParcelasUsar = dtvFornecedor.FindControl("ctrlParcelasUsar1") as Glass.UI.Web.Controls.ctrlParcelasUsar;
                if (ctrlParcelasUsar != null)
                    ctrlParcelasUsar.SalvarParcelas(); */
    
                Response.Redirect("../Listas/LstFornecedor.aspx");
            }
        }
    
        [Ajax.AjaxMethod()]
        public string CheckIfExists(string cpfCnpj)
        {
            return FornecedorDAO.Instance.CheckIfExists(cpfCnpj).ToString().ToLower();
        }
    
        [Ajax.AjaxMethod()]
        public string ValidaCpfCnpj(string IdFornec, string CpfCnpj)
        {
            // Evita que seja possível inserir um CPF/CNPJ já cadastrado ao atualizar os dados do fornecedor.
            string idsFornec = FornecedorDAO.Instance.GetFornecedorByCPFCNPJ(CpfCnpj);
            return ((Glass.Conversoes.StrParaUint(idsFornec.Split(',')[0]) == Glass.Conversoes.StrParaUint(IdFornec)) && idsFornec.Split(',').Length == 1).ToString().ToLower();
        }
    
        protected bool ExibirUrlSistema()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuPedido.ExportarImportarPedido) &&
                PedidoConfig.ExportacaoPedido.AlterarUrlWebGlassFornec;
        }
    
        protected void UrlSistema_Load(object sender, EventArgs e)
        {
            if (!ExibirUrlSistema())
                ((Control)sender).Visible = false;
        }
    
        protected bool ExibirDataVigenciaPreco()
        {
            return FinanceiroConfig.UsarDataVigenciaPrecoFornec;
        }
    
        protected void DataVigenciaPreco_Load(object sender, EventArgs e)
        {
            if (!ExibirDataVigenciaPreco())
                ((Control)sender).Visible = false;
        }

        protected void drpSituacao_DataBound(object sender, EventArgs e)
        {
            // Esconde a situação do fornecedor caso o mesmo não tenha permissão para inativá-lo
            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.AtivarInativarFornecedor))
                ((DropDownList)sender).Style.Add("display", "none");
        }
    }
}
