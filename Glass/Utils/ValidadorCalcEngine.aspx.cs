using System;
using System.Web.UI;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class ValidadorCalcEngine : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(ValidadorCalcEngine));
        }

        [Ajax.AjaxMethod]
        public string GetListCalcEngine()
        {
            var arquivos = Glass.Data.DAL.ArquivoCalcEngineDAO.Instance.GetListCalcEngine(null, null, null, 0, 0);

            return string.Join("|", arquivos.Select(f => f.Nome));
        }
    }
}
