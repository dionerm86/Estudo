using Glass.Data.Model;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls.MDFe
{
    public partial class ctrlRodoviario : System.Web.UI.UserControl
    {
        #region Propriedades

        private static RodoviarioMDFe _rodoviario;

        public int IdManifestoEletronico { get; set; }

        public RodoviarioMDFe Rodoviario
        {
            get
            {
                RodoviarioMDFe rod = null;

                if (IsPostBack)
                {
                    if (dtvRodoviario.CurrentMode == System.Web.UI.WebControls.DetailsViewMode.Edit)
                    {
                        rod = _rodoviario;
                        rod.PlacaTracao = ((DropDownList)dtvRodoviario.FindControl("drpVeiculoTracao")).SelectedValue;
                    }
                    else if (dtvRodoviario.CurrentMode == System.Web.UI.WebControls.DetailsViewMode.Insert)
                    {
                        rod = new RodoviarioMDFe
                        {
                            IdManifestoEletronico = IdManifestoEletronico,
                            PlacaTracao = ((DropDownList)dtvRodoviario.FindControl("drpVeiculoTracao")).SelectedValue
                        };
                    }

                    rod.CiotRodoviario = ((ctrlCIOTRodoviarioMDFe)dtvRodoviario.FindControl("ctrlCIOT")).CiotRodoviario;
                    rod.PedagioRodoviario = ((ctrlPedagioRodoviarioMDFe)dtvRodoviario.FindControl("ctrlPedagio")).PedagioRodoviario;
                    rod.CondutorVeiculo = ((ctrlCondutorVeiculoMDFe)dtvRodoviario.FindControl("ctrlCondutorVeiculo")).CondutorVeiculo;
                    rod.VeiculoRodoviario = ((ctrlVeiculoRodoviarioMDFe)dtvRodoviario.FindControl("ctrlVeiculoReboque")).VeiculoRodoviario;
                    rod.LacreRodoviario = ((ctrlLacreRodoviarioMDFe)dtvRodoviario.FindControl("ctrlLacreRodoviario")).LacreRodoviario;
                }

                return rod;
            }
            set
            {
                _rodoviario = value;
                dtvRodoviario.DataSource = new System.Collections.Generic.List<RodoviarioMDFe> { value };
                if (IdManifestoEletronico > 0)
                    dtvRodoviario.ChangeMode(System.Web.UI.WebControls.DetailsViewMode.Edit);
                dtvRodoviario.DataBind();
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    }
}