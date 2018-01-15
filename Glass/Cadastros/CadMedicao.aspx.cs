using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Web.UI.HtmlControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadMedicao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadMedicao));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (Request["idMedicao"] != null)
                dtvMedicao.ChangeMode(DetailsViewMode.Edit);
    
            if (!IsPostBack && dtvMedicao.CurrentMode == DetailsViewMode.Insert)
            {
                LoginUsuario login = UserInfo.GetUserInfo;
    
                if (login.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Vendedor &&
                    dtvMedicao.FindControl("drpVendedorIns") != null)
                {
                    ((DropDownList)dtvMedicao.FindControl("drpVendedorIns")).SelectedValue = login.CodUser.ToString();
                    ((DropDownList)dtvMedicao.FindControl("drpVendedorIns")).Enabled = false;
                }
            }
    
            // Esconde campos valor, forma pagto e data instalação
            dtvMedicao.Fields[19].Visible = false;
            dtvMedicao.Fields[20].Visible = false;
            dtvMedicao.Fields[21].Visible = false;
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstMedicao.aspx");
        }
    
        protected void odsMedicao_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Medição", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("../Listas/LstMedicao.aspx?idMedicao=" + e.ReturnValue);
        }
    
        protected void odsMedicao_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar Medição", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("../Listas/LstMedicao.aspx");
        }
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string VerificarLimite(string idMedicao, string data)
        {
            return WebGlass.Business.Medicao.Fluxo.BuscarEValidar.Ajax.VerificarLimite(idMedicao, data);
        }
    
        [Ajax.AjaxMethod]
        public string NumMedicoesDia(string data)
        {
            return WebGlass.Business.Medicao.Fluxo.BuscarEValidar.Ajax.NumMedicoesDia(data);
        }

        [Ajax.AjaxMethod]
        public string VerificarPodeAssociarOrcamento(string idOrcamento)
        {
            return WebGlass.Business.Medicao.Fluxo.BuscarEValidar.Ajax.VerificarPodeAssociarOrcamento(idOrcamento);
        }

        [Ajax.AjaxMethod]
        public string VerificarPodeAssociarPedido(string idPedido)
        {
            return WebGlass.Business.Medicao.Fluxo.BuscarEValidar.Ajax.VerificarPodeAssociarPedido(idPedido);
        }

        /// <summary>
        /// Verifica se a medição informada é a medição definitiva de algum orçamento.
        /// </summary>
        [Ajax.AjaxMethod]
        public string VerificarMedicaoDefinitivaOrcamento(string idMedicao)
        {
            return WebGlass.Business.Medicao.Fluxo.BuscarEValidar.Ajax.VerificarMedicaoDefinitivaOrcamento(idMedicao);
        }

        [Ajax.AjaxMethod]
        public string GetCli(string idCli)
        {
            return WebGlass.Business.Orcamento.Fluxo.BuscarEValidar.Ajax.GetCli(idCli);
        }

        [Ajax.AjaxMethod]
        public string GetDadosOrcamento(string idOrcamento)
        {
            return WebGlass.Business.Orcamento.Fluxo.BuscarEValidar.Ajax.GetDadosOrcamento(idOrcamento);
        }

        #endregion

        protected void txtFormaPagto_Load(object sender, EventArgs e)
        {
            // Carrega o texto da forma de pagamento do orçamento para a medição no cadastro
            //((TextBox)sender).Text = OrcamentoConfig.DadosOrcamento.FormaPagtoOrcamento;
        }
    
        protected void drpVendedor_DataBinding(object sender, EventArgs e)
        {
            uint? idFunc = ((Medicao)dtvMedicao.DataItem).IdFunc;
            if (idFunc == null)
                return;
    
            DropDownList d = (DropDownList)sender;
            if (d.Items.FindByValue(idFunc.Value.ToString()) == null)
            {
                string nomeFunc = FuncionarioDAO.Instance.GetNome(idFunc.Value);
                d.Items.Insert(1, new ListItem(nomeFunc, idFunc.Value.ToString()));
            }
        }
    }
}
