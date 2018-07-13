using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.UI.Web.Utils
{
    public partial class SelProd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.SelProd));
    
            hdfCallback.Value = Request["callback"];
    
            // Se for seleção de produto para projeto, filtra por vidro temperado
            if (Request["proj"] != null && !IsPostBack)
            {
                ddlGrupo.DataBind();
                ddlGrupo.SelectedValue = ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString();
                ddlSubgrupo.DataBind();
                
                // Na Personalglass, por exemplo, não existe o subgrupo e produto Temperado
                // que é o subgrupo de ID igual à 2, caso o subgrupo não exista é necessário que
                // todos os subgrupos sejam buscados.
                ddlSubgrupo.SelectedValue = SubgrupoProdDAO.Instance.Exists((uint)Data.Helper.Utils.SubgrupoProduto.Temperado) ?
                    ((int)Data.Helper.Utils.SubgrupoProduto.Temperado).ToString() : "0";
            }
            
            if (String.IsNullOrEmpty(Request["idCompra"]))
                grdProduto.Columns[6].Visible = false;

            // Chamado: 10467. Esconde campo estoque fiscal, se não vier de nota fiscal,
            // caso contrário exibe o campo estoque fiscal e esconde o campo de estoque real disponível.
            if (String.IsNullOrEmpty(Request["notaFiscal"]))
                grdProduto.Columns[12].Visible = false;
            else
                grdProduto.Columns[11].Visible = false;
    
            if (Request["parent"] == "1")
            {
                Label2.Style.Add("display", "none");
                ddlGrupo.Style.Add("display", "none");
                ddlGrupo.SelectedValue = ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString();
            }

            if (Request["Parceiro"] == "1")
            {
                grdProduto.Columns[11].Visible = false;
            }

            if (!IsPostBack)
            {
                // Se tiver sido passado um idGrupoProd, filtra produtos pelo grupo passado
                if (!String.IsNullOrEmpty(Request["idGrupoProd"]) && ddlGrupo.Items.FindByValue(Request["idGrupoProd"]) != null)
                    ddlGrupo.SelectedValue = Request["IdGrupoProd"];
    
                if (!String.IsNullOrEmpty(Request["descricao"]))
                    txtDescr.Text = Request["descricao"];
    
                if (!String.IsNullOrEmpty(Request["IdPedido"]))
                    hdfPedidoProducao.Value = PedidoDAO.Instance.IsProducao(null, Glass.Conversoes.StrParaUint(Request["idPedido"])).ToString().ToLower();
                else
                    hdfPedidoProducao.Value = "false";
    
                hdfPedidoInterno.Value = (Request["pedidoInterno"] == "1").ToString().ToLower();
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProduto.PageIndex = 0;
            grdProduto.DataBind();
        }
    
        protected void ddlGrupo_DataBound(object sender, EventArgs e)
        {
            if (Request["chapa"] == "true" || Request["ambiente"] == "true" || Request["obra"] == "true" || Request["proj"] == "1")
            {
                ddlGrupo.SelectedValue = ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString();
    
                if (Request["chapa"] == "true")
                    hdfCallback.Value = "chapa";
            }
    
            else if (!String.IsNullOrEmpty(Request["IdPedido"]))
            {
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
    
                if (PedidoDAO.Instance.IsMaoDeObra(null, idPedido))
                {
                    ddlGrupo.SelectedValue = ((uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra).ToString();
                    ddlGrupo.Enabled = false;
                }
                else if (PedidoDAO.Instance.IsProducao(null, idPedido))
                {
                    ddlGrupo.SelectedValue = ((uint)Glass.Data.Model.NomeGrupoProd.Vidro).ToString();
                    ddlGrupo.Enabled = false;
                }
                else if (PedidoConfig.DadosPedido.BloqueioPedidoMaoDeObra)
                {
                    ListItem li = ddlGrupo.Items.FindByValue(((uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra).ToString());
                    if (li != null) li.Enabled = false;
                }
            }

            if (Request["produtoestoque"] == "true")
                ddlGrupo.Items.Remove(ddlGrupo.Items[0]);

        }
    
        [Ajax.AjaxMethod]
        public string IsProdChapa(string codInterno, string codIgnorar)
        {
            return ChapaVidroDAO.Instance.IsProdChapa(codInterno, codIgnorar).ToString().ToLower();
        }

        protected void ddlSubgrupo_DataBound(object sender, EventArgs e)
        {
            if (Request["produtoestoque"] == "true" && Conversoes.StrParaInt(ddlGrupo.SelectedValue) == (int)NomeGrupoProd.Vidro)
            {
                ddlSubgrupo.Items.Remove(ddlSubgrupo.Items[0]);

                for (int i = 0; i < ddlSubgrupo.Items.Count; i++)
                {
                    if (!SubgrupoProdDAO.Instance.GetElementByPrimaryKey(Conversoes.StrParaInt(ddlSubgrupo.Items[i].Value)).ProdutosEstoque)
                    {
                        ddlSubgrupo.Items.Remove(ddlSubgrupo.Items[i]);
                        i = -1;
                    }
                }
            }
        }       
    }
}
