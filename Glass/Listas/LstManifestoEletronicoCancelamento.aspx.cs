using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstManifestoEletronicoCancelamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdManifestoEletronico.PageIndex = 0;
        }

        protected void grdManifestoEletronico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var situacao = ((Glass.Data.Model.ManifestoEletronico)e.Row.DataItem).Situacao;

            var cor = situacao == Data.Model.SituacaoEnum.FalhaCancelar || situacao == Data.Model.SituacaoEnum.FalhaEmitir ||
                    situacao == Data.Model.SituacaoEnum.FalhaEncerrar ? System.Drawing.Color.Red :
                situacao == Data.Model.SituacaoEnum.Autorizado ||
                    situacao == Data.Model.SituacaoEnum.Encerrado ? System.Drawing.Color.Blue :
                System.Drawing.Color.Black;

            if (cor != System.Drawing.Color.Black)
                ((Label)e.Row.FindControl("lblSituacao")).ForeColor = cor;
        }
    }
}