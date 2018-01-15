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
    
                //Se a situação do volume não for aberto volta para a listagem.
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
    
                //Cria o dicionario para armazenas as peças escolhidas
                Dictionary<uint, float> pecas = new Dictionary<uint, float>();
    
                foreach (GridViewRow row in grdProdutosPedido.Rows)
                {
                    if (row.RowType != DataControlRowType.DataRow)
                        continue;
    
                    if (((CheckBox)row.FindControl("chkSelProd")).Checked)
                    {
                        var idProdPed = row.FindControl("hdfIdProdPed");
                        if (idProdPed == null)
                            throw new Exception("Peça não encontrada.");
    
                        var qtde = row.FindControl("txtQtdePecas");
                        if (qtde == null)
                            throw new Exception("Qtde peças não encontrada.");
    
                        //Preenche o dicionario com as peças selecionadas
                        pecas.Add(Glass.Conversoes.StrParaUint(((HiddenField)idProdPed).Value), Glass.Conversoes.StrParaFloat(((TextBox)qtde).Text));
                    }
                }
    
                //Adiciona a as peças ao volume
                idVolume = WebGlass.Business.OrdemCarga.Fluxo.VolumeFluxo.Instance.AddItem(idPedido, idVolume, pecas);
    
                AtualizaPagina(idPedido, idVolume);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao adicionar peças ao volume.", ex, Page);
            }
        }
    
        protected void imbRemovePeca_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //Busca o id do pedido
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                if (idPedido == 0)
                    throw new Exception("Pedido não encontrado.");
    
                //Busca o id do volume
                uint idVolume = Glass.Conversoes.StrParaUint(Request["idVolume"]);
                if (idVolume == 0)
                    throw new Exception("Volume não encontrado.");
    
                //Cria o dicionario para armazenas as peças escolhidas
                Dictionary<uint, float> pecas = new Dictionary<uint, float>();
    
                foreach (GridViewRow row in grdVolumeProdutosPedido.Rows)
                {
                    if (row.RowType != DataControlRowType.DataRow)
                        continue;
    
                    if (((CheckBox)row.FindControl("chkSelProd")).Checked)
                    {
                        var idProdPed = row.FindControl("hdfIdProdPed");
                        if (idProdPed == null)
                            throw new Exception("Peça não encontrada.");
    
                        var qtde = row.FindControl("txtQtdePecas");
                        if (qtde == null)
                            throw new Exception("Qtde peças não encontrada.");
    
                        //Preenche o dicionario com as peças selecionadas
                        pecas.Add(Glass.Conversoes.StrParaUint(((HiddenField)idProdPed).Value), Glass.Conversoes.StrParaFloat(((TextBox)qtde).Text));
                    }
                }
    
                //Remove as peças do volume
                WebGlass.Business.OrdemCarga.Fluxo.VolumeFluxo.Instance.RemoveItem(idVolume, pecas);
    
                AtualizaPagina(idPedido, idVolume);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao remover peças do volume.", ex, Page);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string FecharVolume(string idVolume)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.VolumeFluxo.Ajax.FecharVolume(idVolume);
        }
    }
}
