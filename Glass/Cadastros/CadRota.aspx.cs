using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Glass.Global.Negocios.Entidades;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRota : System.Web.UI.Page
    {
        #region Variáveis Locais

        /// <summary>
        /// Instancia da rota do cliente que está sendo inserida.
        /// </summary>
        private Rota _rota;

        #endregion

        #region Métodos Protegidos

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Request["idRota"] == null)
                dtvRota.ChangeMode(DetailsViewMode.Insert);
            else
                dtvRota.ChangeMode(DetailsViewMode.Edit);

            odsRota.Register();
            dtvRota.Register();
            odsRotaCliente.Register();
            grdClientes.Register();

            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadRota));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarRota))
            {
                Response.Redirect("~/WebGlass/Main.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (Request["idRota"] == null)
                {
                    lblSubtitle.Visible = false;
                    lnkAssociarCliente.Visible = false;
                    grdClientes.Visible = false;
                }
            }
        }
    
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstRota.aspx");
        }
    
        protected void odsRota_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Rota.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect(Request.Url + "?idRota=" + _rota.IdRota);
        }
    
        protected void odsRota_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar Rota.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                if (e.ReturnValue is Colosoft.Business.SaveResult && !(Colosoft.Business.SaveResult)e.ReturnValue)
                {
                    e.ExceptionHandled = true;
                    return;
                }

                Page.ClientScript.RegisterClientScriptBlock(GetType(), "rotaAtualizada", "alert('Rota atualizada com sucesso!'); " +
                    "redirectUrl('../Listas/LstRota.aspx');", true);
                MensagemAlerta.ShowMsg("Rota atualizada com sucesso.", Page);
            }
        }
    
        protected void grdClientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Excluir")
            {
                try
                {
                    var fluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IRotaFluxo>();

                    var rotaCliente = fluxo.ObtemRotaCliente(Request["idRota"].StrParaInt(), e.CommandArgument.ToString().StrParaInt());

                    if (rotaCliente != null)
                    {
                        var resultado = fluxo.ApagarRotaCliente(rotaCliente);
                        if (!resultado)
                        {
                            Glass.MensagemAlerta.ErrorMsg("Falha ao desassociar cliente.", resultado);
                            return;
                        }
                    }

                    grdClientes.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao desassociar cliente.", ex, Page);
                }
            }
            else if (e.CommandName == "Up" || e.CommandName == "Down")
            {
                try
                {
                    string[] ids = e.CommandArgument.ToString().Split(',');
                    var fluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IRotaFluxo>();
                    var resultado = fluxo.AlterarPosicao(ids[0].StrParaInt(), ids[1].StrParaInt(), e.CommandName == "Up");

                    if (!resultado)
                        Glass.MensagemAlerta.ErrorMsg("Falha ao mudar posição do cliente.", resultado);
                    else
                        grdClientes.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao mudar posição do cliente.", ex, Page);
                }
            }
        }
    
        protected void odsRota_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            Data.Model.DiasSemana diasSemanaEnum = 0;
    
            var diasSemana = ((Sync.Controls.CheckBoxListDropDown)dtvRota.FindControl("cblDiaSemana"))
                .SelectedValue.Split(',').Select(f => (Data.Model.DiasSemana)Glass.Conversoes.StrParaUint(f)).ToList();
    
            diasSemana.ForEach(x => diasSemanaEnum |= x);

            _rota = (Rota)e.InputParameters[0];
            _rota.DiasSemana = diasSemanaEnum;
        }
    
        protected void odsRota_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            Data.Model.DiasSemana diasSemanaEnum = 0;
    
            var diasSemana = ((Sync.Controls.CheckBoxListDropDown)dtvRota.FindControl("cblDiaSemana"))
                .SelectedValue.Split(',').Select(f => (Data.Model.DiasSemana)Glass.Conversoes.StrParaUint(f)).ToList();
    
            diasSemana.ForEach(x => diasSemanaEnum |= x);

            _rota = (Rota)e.InputParameters[0];
            _rota.DiasSemana = diasSemanaEnum;
        }
    
        protected void dtvRota_DataBound(object sender, EventArgs e)
        {
            var dtvRota = (DetailsView)sender;
    
            if (dtvRota.DataItem == null)
                return;
    
            var rota = (Rota)dtvRota.DataItem;

            if (rota.DiasSemana == Data.Model.DiasSemana.Nenhum)
                return;
    
            var diasSemana = string.Join(",", rota.DiasSemana.ToString().Split(',')
                .Select(f => ((int)(Data.Model.DiasSemana)Enum.Parse(typeof(Data.Model.DiasSemana), f)).ToString()).ToArray());
    
            if (dtvRota.CurrentMode != DetailsViewMode.ReadOnly)
                ((Sync.Controls.CheckBoxListDropDown)dtvRota.FindControl("cblDiaSemana")).SelectedValue = diasSemana;
        }

        protected void dtvRota_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            e.KeepInEditMode = true;
        }


        #endregion

        #region Métodos Ajax

        [Ajax.AjaxMethod()]
        public string AssociaCliente(string idRota, string idCliente)
        {
            try
            {
                var rotaCliente = new RotaCliente
                {
                    IdRota = idRota.StrParaInt(),
                    IdCliente = idCliente.StrParaInt()
                };

                var fluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IRotaFluxo>();
                var resultado = fluxo.SalvarRotaCliente(rotaCliente);
                if (!resultado)
                    return string.Format("Erro|{0}", resultado.Message.Format());

                return "Ok";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro|Falha ao associar cliente.", ex);
            }
        }

        #endregion
    }
}
