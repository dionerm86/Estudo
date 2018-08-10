using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstExportarPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                txtDataFim.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDataIni.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                bool gerado = false;
                //Indica se um pedido tem algum produto selecionado.
                bool temProdutoSelecionado = false;
    
                Dictionary<uint, bool> listaPedidos = new Dictionary<uint, bool>();
    
                Dictionary<uint, List<uint>> idsProdutosPedido = new Dictionary<uint,List<uint>>();
    
                for (int i = 0; i < grdPedido.Rows.Count; i++)
                {
                    CheckBox chkMarcar = grdPedido.Rows[i].Cells[0].FindControl("chkMarcar") as CheckBox;
                    HiddenField hdfIdPedido = grdPedido.Rows[i].Cells[0].FindControl("hdfIdPedido") as HiddenField;
    
                    CheckBox chkMarcarBenef = grdPedido.Rows[i].Cells[grdPedido.Rows[i].Cells.Count - 1].FindControl("chkMarcarBenef") as CheckBox;
    
                    if (!chkMarcar.Checked)
                        continue;
    
                    gerado = true;
    
                    listaPedidos.Add(Glass.Conversoes.StrParaUint(hdfIdPedido.Value), chkMarcarBenef.Checked);
    
                    GridView grid = grdPedido.Rows[i].FindControl("grdProdutosPedido") as GridView;
    
                    if (grid.Rows.Count == 0)
                    {
                        Glass.MensagemAlerta.ShowMsg("O pedido num.: " + hdfIdPedido.Value + " não possui produtos a serem exportados.", Page);
                        return;
                    }
    
                    List<uint> idsProdutos = new List<uint>();
    
                    temProdutoSelecionado = false;
    
                    for (int j = 0; j < grid.Rows.Count; j++)
                    {
                        CheckBox chkSelProdPed = grid.Rows[j].Cells[0].FindControl("chkSelProdPed") as CheckBox;
                        HiddenField hdfIdProdPed = grid.Rows[j].Cells[0].FindControl("hdfIdProdPed") as HiddenField;
    
                        if (!chkSelProdPed.Checked)
                            continue;
    
                        idsProdutos.Add(Glass.Conversoes.StrParaUint(hdfIdProdPed.Value));
                        temProdutoSelecionado = true;
                    }
    
                    if (!temProdutoSelecionado)
                    {
                        Glass.MensagemAlerta.ShowMsg("O pedido num.: " + hdfIdPedido.Value + " não possui produtos selecionados.", Page);
                        return;
                    }
                    
                    idsProdutosPedido.Add(Glass.Conversoes.StrParaUint(hdfIdPedido.Value), idsProdutos);
                }
    
                if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.ExportarImportarPedido))
                    throw new Exception("Exportação desativada no sistema.");
                
                if (!gerado)
                    Glass.MensagemAlerta.ShowMsg("Informe os pedidos que serão exportados.", Page);
                else
                {
                    List<uint> listaIdProduto = new List<uint>();
                    Fornecedor fornecedor = FornecedorDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(ddlFornecedor.SelectedValue));
                    Loja loja = LojaDAO.Instance.GetElement(UserInfo.GetUserInfo.IdLoja);

                    foreach (KeyValuePair<uint, List<uint>> i in idsProdutosPedido)
                    {
                        foreach (uint id in i.Value)
                            listaIdProduto.Add(id);
                    }

                    uint[] idsPedidosExportados = new uint[listaPedidos.Count];
                    listaPedidos.Keys.CopyTo(idsPedidosExportados, 0);

                    byte[] buffer = UtilsExportacaoPedido.ConfigurarExportacao(listaPedidos, listaIdProduto.ToArray());
                    UtilsExportacaoPedido.CriarExportacao((uint)fornecedor.IdFornec, idsPedidosExportados, idsProdutosPedido);

                    var urlFornecedor = string.Format("{0}{1}", fornecedor.UrlSistema.ToLower().Substring(0, fornecedor.UrlSistema.ToLower().LastIndexOf("/webglass")).TrimEnd('/'),
                        "/service/wsexportacaopedido.asmx");
                    object[] parametros = new object[] { loja.Cnpj, 1, buffer };
                    object retorno = WebService.ChamarWebService(urlFornecedor, "SyncService", "EnviarPedidosFornecedor", parametros);

                    string[] dados = retorno as string[];

                    UtilsExportacaoPedido.ProcessarDadosExportacao(dados, listaPedidos);

                    Glass.MensagemAlerta.ShowMsg(dados[1], Page);

                    grdPedido.DataBind();
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("Exportação de Pedidos", ex);
                Glass.MensagemAlerta.ErrorMsg("Ocorreu um erro na exportação do(s) pedido(s).", ex, Page);
            }
        }
    
        protected void grdProdutosPedido_DataBound(object sender, EventArgs e)
        {
    
        }
        protected void grdProdutosPedido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
    
        }
        protected void grdPedido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView grid = e.Row.FindControl("grdProdutosPedido") as GridView;
    
            if (grid != null && grid.Visible)
                grid.DataBind();
        }

        protected void grdPedido_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Loja loja = LojaDAO.Instance.GetElement(UserInfo.GetUserInfo.IdLoja);

            if (e.CommandName == "Consultar")
            {
                try
                {
                    if (ddlFornecedor.SelectedValue == "0")
                        throw new Exception("Selecione o fornecedor para consulta");

                    Fornecedor fornecedor = FornecedorDAO.Instance.GetElement(Conversoes.StrParaUint(ddlFornecedor.SelectedValue));
                    Dictionary<uint, bool> listaPedidos = new Dictionary<uint, bool>();

                    listaPedidos.Add(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()), true);

                    byte[] buffer = UtilsExportacaoPedido.ConfigurarExportacao(listaPedidos, new uint[] { });
                    var urlFornecedor = string.Format("{0}{1}", fornecedor.UrlSistema.ToLower().Substring(0, fornecedor.UrlSistema.ToLower()
                        .LastIndexOf("/webglass")).TrimEnd('/'), "/service/wsexportacaopedido.asmx");

                    object[] parametros = new object[] { loja.Cnpj, 1, buffer };

                    object retorno = WebService.ChamarWebService(urlFornecedor, "SyncService", "VerificarExportacaoPedidos", parametros);

                    UtilsExportacaoPedido.AtualizarPedidosExportacao(retorno as string[]);

                    Glass.MensagemAlerta.ShowMsg("A Situação do pedido foi atualizada.", Page);

                    grdPedido.DataBind();
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao consultar situação.", ex, Page);
                }
            }
        }
    }
}
