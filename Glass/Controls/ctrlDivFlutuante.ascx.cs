using System;
using System.Web.UI;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlDivFlutuante : BaseUserControl
    {
        public enum PosicaoFloatEnum
        {
            Left,
            Right
        }
    
        public ctrlDivFlutuante()
        {
            PosicaoFloat = PosicaoFloatEnum.Right;
        }
    
        #region Propriedades
    
        public bool ExibirSombra
        {
            get { return panFlutuante.CssClass.Contains("sombra"); }
            set { panFlutuante.CssClass = "boxFlutuante" + (value ? " sombra" : String.Empty); }
        }
    
        public PosicaoFloatEnum PosicaoFloat { get; set; }
    
        public ITemplate ItemTemplate { get; set; }
        
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlDivFlutuante_css"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlDivFlutuante_css", CssRegistrar(), false);
                Page.ClientScript.RegisterStartupScript(GetType(), "ctrlDivFlutuante_js", JavaScriptRegistrar(), true);
            }
        }
    
        private string JavaScriptRegistrar()
        {
            return @"
                $(document).ready(function() {
                    $(window).scroll(function() {
                        var posTop = $(document).scrollTop() + 'px';
                        $('#" + panFlutuante.ClientID + @"').animate({ top: posTop }, { duration: 500, queue: false });
                    });
                });";
        }
    
        private string CssRegistrar()
        {
            return @"
                <style type=""text/css"">
                    .boxFlutuante
                    {
                        margin: 5px;
                        position: relative;
                        background-color: #fff;
                        border: 1px solid #C0C0C0;
                        text-align: left;
                        -webkit-border-radius: 5px;
                        -moz-border-radius: 5px;
                        border-radius: 5px;
                        padding: 5px;
                    }
    
                    .sombra
                    {
                        -webkit-box-shadow: 2px 2px 3px rgba(50, 50, 50, 0.3);
                        -moz-box-shadow: 2px 2px 3px rgba(50, 50, 50, 0.3);
                        box-shadow: 2px 2px 3px rgba(50, 50, 50, 0.3);
                    }
                </style>";
        }
    
        protected void panFlutuante_PreRender(object sender, EventArgs e)
        {
            ItemTemplate.InstantiateIn(panFlutuante);
            panFlutuante.Style["float"] = PosicaoFloat.ToString().ToLower();
        }
    
        public override void DataBind()
        {
            panFlutuante.DataBind();
        }
    }
}
