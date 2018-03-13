using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class VersaoDLLs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var dlls = AppDomain.CurrentDomain.GetAssemblies()
                .Where(f => f.FullName.StartsWith("Calc") || f.FullName.StartsWith("Colosoft") || f.FullName.StartsWith("Glass") || f.FullName.StartsWith("CadProject"))
                        .Select(f =>
                            {
                                var dll = f.GetName();
                                return new
                                    {
                                        Nome = dll.Name,
                                        Versao = dll.Version
                                    };
                            }).OrderBy(f => f.Nome);

            grdDLL.DataSource = dlls;
            grdDLL.DataBind();
        }      
    }  
}
