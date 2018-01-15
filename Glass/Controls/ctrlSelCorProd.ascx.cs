using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlSelCorProd : BaseUserControl
    {
        #region Campos privados
    
        private Control _controleGrupo;
    
        #endregion
    
        #region Propriedades
    
        public Control ControleGrupo
        {
            get { return _controleGrupo; }
            set { _controleGrupo = value; }
        }

        /// <summary>
        /// TODO: Propriedade usada para auxiliar na migração.
        /// </summary>
        public int? IdCorVidroInt32
        {
            get { return (int?)IdCorVidro; }
            set { IdCorVidro = (uint?)value; }
        }

        public uint? IdCorVidro
        {
            get { return TipoCor == 1 ? drpCorVidro.SelectedValue.StrParaUintNullable() : null; }
            set
            {
                if (value.HasValue)
                {
                    drpCorVidro.SelectedValue = value.ToString();
                    TipoCor = 1;
                }
            }
        }
        
        /// <summary>
        /// TODO: Propriedade usada para auxiliar na migração.
        /// </summary>
        public int? IdCorFerragemInt32
        {
            get { return (int?)IdCorFerragem; }
            set { IdCorFerragem = (uint?)value; }
        }
    
        public uint? IdCorFerragem
        {
            get { return TipoCor == 2 ? drpCorFerragem.SelectedValue.StrParaUintNullable() : null; }
            set 
            {
                if (value.HasValue)
                {
                    drpCorFerragem.SelectedValue = value.ToString();
                    TipoCor = 2;
                }
            }
        }

        /// <summary>
        /// TODO: Propriedade usada para auxiliar na migração.
        /// </summary>
        public int? IdCorAluminioInt32
        {
            get { return (int?)IdCorAluminio; }
            set { IdCorAluminio = (uint?)value; }
        }

        public uint? IdCorAluminio
        {
            get { return TipoCor == 3 ? drpCorAluminio.SelectedValue.StrParaUintNullable() : null; }
            set
            {
                if (value.HasValue)
                {
                    drpCorAluminio.SelectedValue = value.ToString();
                    TipoCor = 3;
                }
            }
        }
    
        public int TipoCor
        {
            get { return drpTipoCor.SelectedValue.StrParaInt(); }
            set { drpTipoCor.SelectedValue = value.ToString(); }
        }
    
        #endregion
    
        #region Métodos privados
    
        /// <summary>
        /// Formata um controle da página.
        /// </summary>
        /// <param name="campo">O controle da página.</param>
        private void FormatControl(Control campo)
        {
            // Garante que o campo seja válido
            if (campo == null || !(campo is WebControl))
                return;
    
            // String com o atributo que será alterado
            string atributo;
            if (campo is DropDownList)
                atributo = "OnChange";
            else if (campo is CheckBox)
                atributo = "OnClick";
            else
                atributo = "OnBlur";
    
            // String com a função que será executada
            string funcaoCalculo = "alteraGrupoProd(this.value)";
            string funcao = "";
    
            // Verifica se o controle já possui uma função atribuída ao evento OnBlur
            if (!String.IsNullOrEmpty(((WebControl)campo).Attributes[atributo]))
            {
                // Recupera a função do controle
                funcao = ((WebControl)campo).Attributes[atributo];
    
                // Verifica se a função desejada já está no controle
                if (funcao.IndexOf(funcaoCalculo) > -1)
                    return;
    
                // Coloca a função de cálculo junto à função original
                if (funcao.IndexOf("return") > -1)
                    funcao = funcao.Replace("return", funcaoCalculo + "; return");
                else
                    funcao += "; " + funcaoCalculo;
            }
    
            // Indica que apenas essa função será executada
            else
                funcao = funcaoCalculo;
    
            // Atribui a função ao controle
            if (((WebControl)campo).Attributes[atributo] == null || !((WebControl)campo).Attributes[atributo].Contains(funcao))
                ((WebControl)campo).Attributes[atributo] = funcao;
        }
    
        private string JavaScript()
        {
            return @"
                function alteraTipoCor(tipoCor)
                {
                    FindControl('drpCorVidro', 'select').style.display = tipoCor == 1 ? '' : 'none';
                    FindControl('drpCorFerragem', 'select').style.display = tipoCor == 2 ? '' : 'none';
                    FindControl('drpCorAluminio', 'select').style.display = tipoCor == 3 ? '' : 'none';
                }
    
                function alteraGrupoProd(idGrupoProd)
                {
                    var vidro = " + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @";
                    var aluminio = " + (int)Glass.Data.Model.NomeGrupoProd.Alumínio + @";
                    var ferragem = " + (int)Glass.Data.Model.NomeGrupoProd.Ferragem + @";
                    
                    var tipoCor = FindControl('drpTipoCor', 'select');
                    if (tipoCor == null)
                        return;
    
                    if (idGrupoProd == vidro)
                    {
                        tipoCor.value = 1;
                        tipoCor.style.display = 'none';
                    }
                    else if (idGrupoProd == aluminio)
                    {
                        tipoCor.value = 3;
                        tipoCor.style.display = 'none';
                    }
                    else if (idGrupoProd == ferragem)
                    {
                        tipoCor.value = 2;
                        tipoCor.style.display = 'none';
                    }
                    else
                        tipoCor.style.display = '';
    
                    alteraTipoCor(tipoCor.value);
                }";
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlSelCorProd"))
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlSelCorProd", JavaScript(), true);
    
            Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_Startup", "alteraTipoCor(document.getElementById('" + 
                drpTipoCor.ClientID + "').value);\n", true);
    
            Page.ClientScript.RegisterClientScriptBlock(GetType(), this.ClientID, 
                "var " + this.ClientID + @" = {
                    tipoCor: function() { return document.getElementById('" + drpTipoCor.ClientID + @"').value; },
                    idCorVidro: function() { return " + this.ClientID + ".tipoCor() == 1 ? document.getElementById('" + drpCorVidro.ClientID + @"').value : ''; },
                    idCorFerragem: function() { return " + this.ClientID + ".tipoCor() == 2 ? document.getElementById('" + drpCorFerragem.ClientID + @"').value : ''; },
                    idCorAluminio: function() { return " + this.ClientID + ".tipoCor() == 3 ? document.getElementById('" + drpCorAluminio.ClientID + @"').value : ''; }
                }", true);
        }
    
        protected void drpTipoCor_PreRender(object sender, EventArgs e)
        {
            FormatControl(_controleGrupo);
    
            if (_controleGrupo != null)
                Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_Grupo", 
                    "alteraGrupoProd(document.getElementById('" + _controleGrupo.ClientID + "').value);\n", true);
        }
    }
}
