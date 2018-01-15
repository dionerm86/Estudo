using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstBandeiraCartao : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdBandeiraCartao.Register(true, true);
            odsBandeiraCartao.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imbInserir_Click(object sender, ImageClickEventArgs e)
        {
            var descricao = ((TextBox)grdBandeiraCartao.FooterRow.FindControl("txtDescricao")).Text;

            BandeiraCartaoDAO.Instance.Insert(new BandeiraCartao
            {
                Descricao = descricao
            });

            grdBandeiraCartao.DataBind();
        }
    }
}