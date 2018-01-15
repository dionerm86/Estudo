using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class ShowMsg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = Request["title"];
            txtMsg.Text = FormataMsg(Request["msg"]);
        }
    
        /// <summary>
        /// Formata mensagem com os seguinte códigos:
        ///     %bl% = \r\n
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected string FormataMsg(string msg)
        {
            if (String.IsNullOrEmpty(msg))
                return String.Empty;
    
            msg = msg.Replace("%bl%", "\r\n");
    
            return msg;
        }
    }
}
