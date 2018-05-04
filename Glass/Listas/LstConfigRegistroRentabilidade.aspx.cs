using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstConfigRegistroRentabilidade : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void grdConfigRegistroRentabilidade_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Up" || e.CommandName == "Down")
            {
                var key = (string)e.CommandArgument;
                var parts = key.Split('x');
                var tipo = 0;
                var idRegistro = 0;

                if (parts.Length > 1 && int.TryParse(parts[0], out tipo) && int.TryParse(parts[1], out idRegistro))
                {
                    var rentabilidadeFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                            .Current.GetInstance<Glass.Rentabilidade.Negocios.IRentabilidadeFluxo>();

                    var config = rentabilidadeFluxo.ObterConfigRegistroRentabilidade((byte)tipo, idRegistro);

                    if (config != null)
                    {
                        var result = new Colosoft.Business.SaveResult();

                        if ((e.CommandName == "Up" && !(result = rentabilidadeFluxo.MoverConfigRegistroRentabilidade(config, true))) ||
                            (e.CommandName == "Down" && !(result = rentabilidadeFluxo.MoverConfigRegistroRentabilidade(config, false))))
                            MensagemAlerta.ShowMsg($"Falha ao mover configuração. { result.Message }", Page);

                        grdConfigRegistroRentabilidade.DataBind();
                    }
                }
            }
        }

    }
}