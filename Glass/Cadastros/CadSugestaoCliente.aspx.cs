using Glass.Data.Helper;
using Glass.Global.Negocios.Entidades;
using Microsoft.Practices.ServiceLocation;
using System;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadSugestaoCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes))
            {
                Response.Redirect("~/WebGlass/Main.aspx");
                return;
            }

            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            
            if (!string.IsNullOrEmpty(Request["idCliente"]))
            {
                lblCliente.Text = Data.DAL.ClienteDAO.Instance.GetNome(Conversoes.StrParaUint(Request["idCliente"]));
                hdfCliente.Value = Request["idCliente"];
    
                lblInfoCli.Visible = false;
                txtNomeCliente.Visible = false;
                Label3.Visible = false;
                txtNumCli.Visible = false;
                lnkSelCliente.Visible = false;
            }
            else if (!string.IsNullOrEmpty(Request["idPedido"]))
            {
                var idCliente = Data.DAL.PedidoDAO.Instance.GetIdCliente(null, Request["idPedido"].StrParaUint());
                lblCliente.Text = Data.DAL.ClienteDAO.Instance.GetNome(idCliente);
                hdfCliente.Value = Request["idCliente"];

                lblInfoCli.Visible = false;
                txtNomeCliente.Visible = false;
                Label3.Visible = false;
                txtNumCli.Visible = false;
                lnkSelCliente.Visible = false;
            }
            else if (!string.IsNullOrEmpty(Request["idOrcamento"]))
            {
                var idCliente = Data.DAL.OrcamentoDAO.Instance.ObtemIdCliente(Request["idOrcamento"].StrParaUint());

                if (idCliente.GetValueOrDefault() == 0)
                    lblCliente.Visible = lblInfoNomeCli.Visible = false;
                else
                {
                    lblCliente.Text = Data.DAL.ClienteDAO.Instance.GetNome(idCliente.Value);
                    hdfCliente.Value = Request["idCliente"];

                    lblInfoCli.Visible = false;
                    txtNomeCliente.Visible = false;
                    Label3.Visible = false;
                    txtNumCli.Visible = false;
                    lnkSelCliente.Visible = false;
                }
            }
            else
                lblCliente.Visible = lblInfoNomeCli.Visible = false;
        }
    
        protected void btnInserir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDescricao.Text))
            {
                lblStatus.Text = "É necessário digitar uma descrição para cadastrar a sugestão.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }
    
            string descr = txtDescricao.Text;

            int idCli = 0;
            int? idPedido = null;
            uint? idOrcamento = null;

            if (!string.IsNullOrEmpty(Request["idCliente"]))
            {
                idCli = Request["idCliente"].StrParaInt();
            }
            else if (!string.IsNullOrEmpty(Request["idPedido"]))
            {
                idPedido = Request["idPedido"].StrParaInt();
                idCli = (int)Data.DAL.PedidoDAO.Instance.GetIdCliente(null, Request["idPedido"].StrParaUint());
            }
            else if (!string.IsNullOrEmpty(Request["idOrcamento"]))
            {
                idOrcamento = Request["idOrcamento"].StrParaUint();
                idCli = (int)Data.DAL.OrcamentoDAO.Instance.ObtemIdCliente(Request["idOrcamento"].StrParaUint()).GetValueOrDefault();
            }
            else
            {
                idCli = hdfCliente.Value.StrParaInt();
            }
    
            if (idCli == 0)
                return;
    
            Data.Model.TipoSugestao tipo = Data.Model.TipoSugestao.Negociacao;
            
            Enum.TryParse<Glass.Data.Model.TipoSugestao>(drpTipo.SelectedValue, out tipo);

            var sugestaoFluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.ISugestaoFluxo>();

            var sc = sugestaoFluxo.CriarSugestaoCliente();
            sc.IdCliente = idCli;
            sc.IdPedido = idPedido;
            sc.IdOrcamento = idOrcamento;
            sc.Descricao = descr;
            sc.TipoSugestao = tipo;

            try
            {
                var resultado = sugestaoFluxo.SalvarSugestao(sc);
                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar.", resultado);
                else
                {
                    txtDescricao.Text = "";
                    lblStatus.Text = "Sugestão cadastrada com sucesso.";
                    lblStatus.ForeColor = System.Drawing.Color.Blue;
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar.", ex, Page);
            }
        }
    
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["idCliente"]))
                Response.Redirect("../Listas/LstSugestaoCliente.aspx?idCliente=" + Request["idCliente"]);
            else if (!string.IsNullOrEmpty(Request["idPedido"]))
                Response.Redirect("../Listas/LstSugestaoCliente.aspx?idPedido=" + Request["idPedido"]);
            else if (!string.IsNullOrEmpty(Request["idOrcamento"]))
                Response.Redirect("../Listas/LstSugestaoCliente.aspx?idOrcamento=" + Request["idOrcamento"]);
        }
    }
}
