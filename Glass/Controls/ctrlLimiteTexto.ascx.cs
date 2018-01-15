using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public enum Posicionamento
    {
        Flutuante = 1,
        TamanhoFixo = 2
    }

    public partial class ctrlLimiteTexto : BaseUserControl
    {
        #region Propriedades

        private string _idToControlValidade;
        public string IdControlToValidate
        {
            get { return this._idToControlValidade; }
            set { this._idToControlValidade = value; }
        }

        private string _width;
        public string Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private string _height;
        public string Height
        {
            get { return _height; }
            set { _height = value; }
        }

        private Posicionamento _position;
        public Posicionamento Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private int _maxCaractere
        {
            get
            {
                Control objControl = this.LocalizarControle();

                if (objControl != null)
                    return ((TextBox)objControl).MaxLength;
                else
                    return 0;
            }
        }

        private string _script
        {
            get
            {
                string script = "<script type=\"text/javascript\">"
                              + "function contarCaracteres(max, controle, lblResult)"
                              + "{"
                    //+ "var controle = document.getElementById(controle);"
                    //+ "var lblMensagem = document.getElementById(lblResult);"
                              + "var controle = FindControl(controle, \"textarea\");"
                              + "var lblMensagem = FindControl(lblResult, \"span\");"

                              + "if(controle != null && controle != \"undefined\")"
                              + "{"
                              + "var tamanhoTexto = controle.value.length;"

                              + "if(max - tamanhoTexto >= 0)"
                              + "{"
                              + "lblMensagem.value = max - tamanhoTexto + \" caracteres restantes\";"
                              + "lblMensagem.innerText = max - tamanhoTexto + \" caracteres restantes\";"
                              + "}"

                              + "if(tamanhoTexto > max)"
                              + "controle.value = controle.value.substring(0, max);"
                              + "}"
                              + "}"
                              + "</script>";

                return script;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            string attrFormat = "";

            if (this.Position == Posicionamento.Flutuante)
                attrFormat += "position: relative; float: left; ";
            if (!String.IsNullOrEmpty(this.Width))
                attrFormat += "Width: " + this.Width + "; ";
            if (!String.IsNullOrEmpty(this.Height))
                attrFormat += "Height: " + this.Height + "; ";

            if (!String.IsNullOrEmpty(attrFormat))
                this.divControl.Attributes.Add("style", attrFormat);

            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlLimitador"))
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ctrlLimitador", this._script);

            this.setarKeyUp();
        }

        protected override void OnPreRender(EventArgs e)
        {
            Label objLabel = new Label();
            objLabel.ID = "lbl" + this.ID;
            objLabel.Text = this._maxCaractere.ToString() + " caracteres restantes.";
            this.pchLabelControle.Controls.Add(objLabel);
        }

        private void setarKeyUp()
        {
            Control objControl = this.LocalizarControle();

            if (objControl != null)
                ((TextBox)objControl).Attributes.Add("onKeyUp", "contarCaracteres(" + this._maxCaractere.ToString() + ", \"" + this.IdControlToValidate + "\", \"lbl" + this.ID + "\"" + ");");
        }

        private Control LocalizarControle()
        {
            return this.NamingContainer.FindControl(this.IdControlToValidate);
        }
    }
}