using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstFerragem : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdFerragem.Register(true, true);
            odsFerragem.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdFerragem.PageIndex = 0;
        }

        protected void lnkInserirFerragem_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/Projeto/CadFerragem.aspx");
        }

        protected void lnkInserirFerragem_Load(object sender, EventArgs e)
        {
            var adminSync = Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync;

            lnkInserirFerragem.Visible = adminSync;
        }
    }
}