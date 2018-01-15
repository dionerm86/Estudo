using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Data.Helper;

namespace Glass.UI.Web.Listas
{
    public partial class LstOrcamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                grdOrcamento.Columns[3].Visible = PCPConfig.GerarOrcamentoFerragensAluminiosPCP;

                if (OrcamentoConfig.FiltroPadraoAtivoListagem)
                    drpSituacao.SelectedValue = "1";

                lnkOrçamento.Visible = Config.PossuiPermissao(Config.FuncaoMenuOrcamento.EmitirOrcamento);
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void lnkOrçamento_Click(object sender, EventArgs e)
        {
            Response.Redirect("../cadastros/cadOrcamento.aspx");
        }
    
        protected void lnkPesquisar_Click(object sender, EventArgs e)
        {
            grdOrcamento.PageIndex = 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdOrcamento.PageIndex = 0;
        }
    
        protected void drpSituacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdOrcamento.PageIndex = 0;
        }

        protected bool SugestoesVisible()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes);
        }


        protected void grdOrcamento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "GerarPedido")
            {
                try
                {
                    uint idOrcamento = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    var orca = OrcamentoDAO.Instance.GetElement(idOrcamento);
    
                    LinkButton lnkGerarPedido = (LinkButton)e.CommandSource;
                    HiddenField hdfIdCliente = (HiddenField)lnkGerarPedido.Parent.FindControl("hdfIdCliente");
    
                    if (orca.IdCliente == null)
                    {
                        Cliente cli = ClienteDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(hdfIdCliente.Value));
                        
                        orca.IdCliente = (uint)cli.IdCli;
                        orca.NomeCliente = cli.Nome;
    
                        if (String.IsNullOrEmpty(orca.Bairro))
                            orca.Bairro = cli.Bairro;
                        if (String.IsNullOrEmpty(orca.Cidade))
                            orca.Cidade = CidadeDAO.Instance.GetNome((uint?)cli.IdCidade);
                        if (String.IsNullOrEmpty(orca.Endereco))
                            orca.Endereco = cli.Endereco + (!String.IsNullOrEmpty(cli.Numero) ? ", " + cli.Numero : String.Empty);
                        if (String.IsNullOrEmpty(orca.TelCliente))
                            orca.TelCliente = !String.IsNullOrEmpty(cli.TelCont) ? cli.TelCont : cli.TelRes;
                        if (String.IsNullOrEmpty(orca.CelCliente))
                            orca.CelCliente = cli.TelCel;
                        if (String.IsNullOrEmpty(orca.Email))
                            orca.Email = (cli.Email != null ? cli.Email.Split(',')[0] : null);
    
                        OrcamentoDAO.Instance.Update(orca);
                    }
    
                    uint idPedido = PedidoDAO.Instance.GerarPedido(idOrcamento);
                    Response.Redirect("../Cadastros/CadPedido.aspx?idPedido=" + idPedido);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao gerar pedido.", ex, Page);
                }            
            }
            else if (e.CommandName == "EnviarEmail")
            {
                var idOrcamento = Convert.ToUInt32(e.CommandArgument);

                // Envia o e-mail
                Email.EnviaEmailOrcamento(null, idOrcamento);

                LogAlteracaoDAO.Instance.LogEnvioEmailOrcamento(idOrcamento);

                MensagemAlerta.ShowMsg("O e-mail foi adicionado na fila para ser enviado.", Page);
            }
        }
    
        protected void lnkGerarPedido_Load(object sender, EventArgs e)
        {
            LinkButton lnkGerarPedido = (LinkButton)sender;
            HiddenField hdfIdCliente = (HiddenField)lnkGerarPedido.Parent.FindControl("hdfIdCliente");
    
            lnkGerarPedido.OnClientClick = String.Format(lnkGerarPedido.OnClientClick, hdfIdCliente.ClientID);
        }
    }
}
