using System;

namespace Glass.UI.Web.Utils
{
    public partial class ShowFoto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = Request["path"];
    
            if (path.ToLower().Contains("handler"))
                path = path.Replace("$", "?").Replace("@", "&").Replace("!", "\\");
    
            imgFull.ImageUrl = path;
        }
    }
}
