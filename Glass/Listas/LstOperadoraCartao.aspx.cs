using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstOperadoraCartao : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdOperadoraCartao.Register(true, true);
            odsOperadoraCartao.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imbInserir_Click(object sender, ImageClickEventArgs e)
        {
            var descricao = ((TextBox)grdOperadoraCartao.FooterRow.FindControl("txtDescricao")).Text;

            OperadoraCartaoDAO.Instance.Insert(new OperadoraCartao
            {
                Descricao = descricao
            });

            grdOperadoraCartao.DataBind();
        }
    }
}