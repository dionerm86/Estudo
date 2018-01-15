using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstPedidosEspelho : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstPedidosEspelho));
    
            if (!IsPostBack)
            {
                if (!Geral.ControlePCP || !Config.PossuiPermissao(Config.FuncaoMenuPCP.VisualizarPedidosEmConferencia))
                {
                    Response.Redirect("~/webglass/main.aspx");
                    return;
                }

                if (!PCPConfig.UsarControleGerenciamentoProjCnc)
                {
                    grdPedido.Columns[14].Visible = false;
                    tbSituacaoCnc.Style["display"] = "none";
                    lnkArquivoCnc.Visible = false;
                    imbArquivoCnc.Visible = false;
                }

                if (!PCPConfig.EmpresaGeraArquivoDxf)
                {
                    imbArquivoDxf.Visible = false;
                    lnkArquivoDxf.Visible = false;
                }

                if (!PCPConfig.EmpresaGeraArquivoFml)
                {
                    imbArquivoFml.Visible = false;
                    lnkArquivoFml.Visible = false;
                }

                if (!PCPConfig.EmpresaGeraArquivoSGlass)
                {
                    imbArquivoSglass.Visible = false;
                    lnkArquivoSglass.Visible = false;
                }

                if (!PCPConfig.EmpresaGeraArquivoIntermac)
                {
                    imgArquivoIntermac.Visible = false;
                    lnkArquivoIntermac.Visible = false;
                }
            }
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadPedidoEspelho.aspx");
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    
        protected void grdPedido_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CancelarEspelho")
            {
                try
                {
                    PedidoEspelhoDAO.Instance.CancelarEspelhoComTransacao(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
    
                    grdPedido.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar conferência.", ex, Page);
                }
            }
            else if (e.CommandName == "Reabrir")
            {
                try
                {
                    PedidoEspelhoDAO.Instance.ReabrirPedidoComTransacao(e.CommandArgument.ToString().StrParaUint());
    
                    grdPedido.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao reabrir pedido.", ex, Page);
                }
            }
            else if (e.CommandName == "SituacaoCnc")
            {
                try
                {
                    var idPedido = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    var pedEsp = PedidoEspelhoDAO.Instance.GetElement(idPedido);
    
                    if (pedEsp == null)
                        throw new Exception("Pedido não encontrado.");
    
                    if (pedEsp.SituacaoCnc == (int)PedidoEspelho.SituacaoCncEnum.SemNecessidadeNaoConferido)
                        throw new Exception("Não há necessidade de um projeto cnc para esse pedido.");
    
                    if (pedEsp.SituacaoCnc == (int)PedidoEspelho.SituacaoCncEnum.NaoProjetado)
                        pedEsp.SituacaoCnc = (int)PedidoEspelho.SituacaoCncEnum.Projetado;
                    else
                        pedEsp.SituacaoCnc = (int)PedidoEspelho.SituacaoCncEnum.NaoProjetado;
    
                    pedEsp.DataProjetoCnc = DateTime.Now;
                    pedEsp.UsuProjetoCnc = UserInfo.GetUserInfo.CodUser;
    
                    LogAlteracaoDAO.Instance.LogPedidoEspelho(pedEsp, LogAlteracaoDAO.SequenciaObjeto.Novo);
                    PedidoEspelhoDAO.Instance.Update(pedEsp);
    
                    grdPedido.DataBind();
    
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao alterar situação proj. cnc.", ex, Page);
                }
            }
            else if (e.CommandName == "SituacaoCncConferencia")
            {
                try
                {
                    var idPedido = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    var pedEsp = PedidoEspelhoDAO.Instance.GetElement(idPedido);

                    if (pedEsp == null)
                        throw new Exception("Pedido não encontrado.");

                    if (pedEsp.SituacaoCnc == (int)PedidoEspelho.SituacaoCncEnum.SemNecessidadeNaoConferido)
                        pedEsp.SituacaoCnc = (int)PedidoEspelho.SituacaoCncEnum.SemNecessidadeConferido;
                    else
                        pedEsp.SituacaoCnc = (int)PedidoEspelho.SituacaoCncEnum.SemNecessidadeNaoConferido;

                    pedEsp.DataProjetoCnc = DateTime.Now;
                    pedEsp.UsuProjetoCnc = UserInfo.GetUserInfo.CodUser;

                    LogAlteracaoDAO.Instance.LogPedidoEspelho(pedEsp, LogAlteracaoDAO.SequenciaObjeto.Novo);
                    PedidoEspelhoDAO.Instance.Update(pedEsp);

                    grdPedido.DataBind();

                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao alterar situação proj. cnc.", ex, Page);
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
            if (String.IsNullOrEmpty(idCli) || !ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }

        #endregion

        protected void grdPedido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var item = e.Row.DataItem as PedidoEspelho;
            if (item == null)
                return;

            if (item.CorLinhaLista != System.Drawing.Color.Black)
                foreach (TableCell c in e.Row.Cells)
                {
                    c.ForeColor = item.CorLinhaLista;

                    foreach (Control c1 in c.Controls)
                        if (c1 is WebControl)
                            ((WebControl)c1).ForeColor = c.ForeColor;
                }
        }
    }
}