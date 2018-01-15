using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros.Expedicao
{
    public partial class CadLeituraCarregamentoMobile : System.Web.UI.Page
    {
        #region Métodos Protegidos

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Expedicao.CadLeituraCarregamento));

            if (!IsPostBack)
            {
                // Obtém os setores que o funcionário possui acesso
                var funcSetor = FuncionarioSetorDAO.Instance.GetSetores(UserInfo.GetUserInfo.CodUser);

                if (funcSetor.Count == 0)
                    Response.Redirect("../../WebGlass/Main.aspx");

                var setor = Data.Helper.Utils.ObtemSetor((uint)funcSetor[0].IdSetor);

                // Se não for expedição de carregamento ou se não usar o controle de carregamento, sai desta tela
                if (!OrdemCargaConfig.UsarControleOrdemCarga ||
                    UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.MarcadorProducao ||
                     setor.Tipo != TipoSetor.ExpCarregamento)
                    Response.Redirect("../../WebGlass/Main.aspx");

                hdfTempoLogin.Value = setor.TempoLogin.ToString();

                UserInfo.SetActivity();

                hdfFunc.Value = UserInfo.GetUserInfo.CodUser.ToString();
            }
        }

        protected void lnkLgout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            dtvItensCarregamento.DataBind();
        }

        protected void grvProdutos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var corLinha = ((Glass.Data.Model.ItemCarregamento)e.Row.DataItem).CorLinha;
                foreach (TableCell cell in e.Row.Cells)
                    cell.ForeColor = corLinha;
            }
        }

        protected void grvProdutos_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;

            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        #endregion
    }
}