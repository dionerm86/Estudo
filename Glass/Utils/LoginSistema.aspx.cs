using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class LoginSistema : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grdLogin.Columns[3].Visible = Data.Helper.UserInfo.GetUserInfo.IsAdminSync;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    }
}
