using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadItensVolume : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadItensVolume));
    
            if (!IsPostBack)
            {
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                uint idVolume = Glass.Conversoes.StrParaUint(Request["idVolume"]);
    
                //Se a situa��o do volume n�o for aberto volta para a listagem.
                if (idVolume > 0 && VolumeDAO.Instance.GetSituacao(idVolume) != Glass.Data.Model.Volume.SituacaoVolume.Aberto)
                    Response.Redirect("~/Listas/LstVolumes.aspx");
            }
        }
    
        protected void grdVolumeProdutosPedido_DataBound(object sender, EventArgs e)
        {
            GridView grdVolumeProdutosPedido = (GridView)sender;
            if (grdVolumeProdutosPedido.Rows.Count != 1)
                return;
    
            uint idVolume = Glass.Conversoes.StrParaUint(Request["idVolume"]);
            if (VolumeProdutosPedidoDAO.Instance.GetListCountReal(idVolume) == 0)
                grdVolumeProdutosPedido.Rows[0].Visible = false;
        }
    
        protected void grdProdutosPedido_DataBound(object sender, EventArgs e)
        {
            GridView grdProdutosPedido = (GridView)sender;
            if (grdProdutosPedido.Rows.Count != 1)
                return;
    
            var idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
            if (idPedido == 0)
                return;
    
            if (ProdutosPedidoDAO.Instance.GetForGerarVolumeCountReal(idPedido) == 0)
                grdProdutosPedido.Rows[0].Visible = false;
        }
    
        private void AtualizaPagina(uint idPedido, uint idVolume)
        {
            Response.Redirect("CadItensVolume.aspx?popup=true&exibirCabecalhoPopup=true&" +
                "idPedido=" + idPedido + "&idVolume=" + idVolume);
        }
    
        protected void imbAddPeca_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //Busca o id do volume e do pedido
                uint idVolume = Glass.Conversoes.StrParaUint(Request["idVolume"]);
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
    
                //Cria o dicionario para armazenas as pe�as escolhidas
                Dictionary<uint, float> pecas = new Dictionary<uint, float>();
    
                foreach (GridViewRow row in grdProdutosPedido.Rows)
                {
                    if (row.RowType != DataControlRowType.DataRow)
                        continue;
    
                    if (((CheckBox)row.FindControl("chkSelProd")).Checked)
                    {
                        var idProdPed = row.FindControl("hdfIdProdPed");
                        if (idProdPed == null)
                            throw new Exception("Pe�a n�o encontrada.");
    
                        var qtde = row.FindControl("txtQtdePecas");
                        if (qtde == null)
                            throw new Exception("Qtde pe�as n�o encontrada.");
    
                        //Preenche o dicionario com as pe�as selecionadas
                        pecas.Add(Glass.Conversoes.StrParaUint(((HiddenField)idProdPed).Value), Glass.Conversoes.StrParaFloat(((TextBox)qtde).Text));
                    }
                }
    
                //Adiciona a as pe�as ao volume
                idVolume = WebGlass.Business.OrdemCarga.Fluxo.VolumeFluxo.Instance.AddItem(idPedido, idVolume, pecas);
    
                AtualizaPagina(idPedido, idVolume);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao adicionar pe�as ao volume.", ex, Page);
            }
        }
    
        protected void imbRemovePeca_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //Busca o id do pedido
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                if (idPedido == 0)
                    throw new Exception("Pedido n�o encontrado.");
    
                //Busca o id do volume
                uint idVolume = Glass.Conversoes.StrParaUint(Request["idVolume"]);
                if (idVolume == 0)
                    throw new Exception("Volume n�o encontrado.");
    
                //Cria o dicionario para armazenas as pe�as escolhidas
                Dictionary<uint, float> pecas = new Dictionary<uint, float>();
    
                foreach (GridViewRow row in grdVolumeProdutosPedido.Rows)
                {
                    if (row.RowType != DataControlRowType.DataRow)
                        continue;
    
                    if (((CheckBox)row.FindControl("chkSelProd")).Checked)
                    {
                        var idProdPed = row.FindControl("hdfIdProdPed");
                        if (idProdPed == null)
                            throw new Exception("Pe�a n�o encontrada.");
    
                        var qtde = row.FindControl("txtQtdePecas");
                        if (qtde == null)
                            throw new Exception("Qtde pe�as n�o encontrada.");
    
                        //Preenche o dicionario com as pe�as selecionadas
                        pecas.Add(Glass.Conversoes.StrParaUint(((HiddenField)idProdPed).Value), Glass.Conversoes.StrParaFloat(((TextBox)qtde).Text));
                    }
                }
    
                //Remove as pe�as do volume
                WebGlass.Business.OrdemCarga.Fluxo.VolumeFluxo.Instance.RemoveItem(idVolume, pecas);
    
                AtualizaPagina(idPedido, idVolume);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao remover pe�as do volume.", ex, Page);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string FecharVolume(string idVolume)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.VolumeFluxo.Ajax.FecharVolume(idVolume);
        }
    }
}
