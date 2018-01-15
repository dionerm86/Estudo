using System;
using System.Web.UI;
using Glass.Data.DAL;
using System.Text;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlSelGrupoSubgrupoProd : BaseUserControl
    {
        private bool _apenasVidros, _exibirSubgrupoProdutoVazio;

        #region Propriedades
    
        public bool ApenasVidros
        {
            get { return _apenasVidros; }
            set { _apenasVidros = value; }
        }
    
        public bool ExibirGrupoProdutoVazio
        {
            get { return drpGrupoProd.AppendDataBoundItems; }
            set { drpGrupoProd.AppendDataBoundItems = value; }
        }
    
        public bool ExibirSubgrupoProdutoVazio
        {
            get { return _exibirSubgrupoProdutoVazio; }
            set { _exibirSubgrupoProdutoVazio = value; }
        }

        /// <summary>
        /// TODO: Propriedade usada para auxiliar na migração.
        /// </summary>
        public int? IdGrupoProduto
        {
            get { return (int?)CodigoGrupoProduto; }
            set { CodigoGrupoProduto = (uint?)value; }
        }

        public uint? CodigoGrupoProduto
        {
            get { return Glass.Conversoes.StrParaUintNullable(drpGrupoProd.SelectedValue); }
            set { drpGrupoProd.SelectedValue = value != null ? value.ToString() : null; }
        }

        /// <summary>
        /// TODO: Propriedade usada para auxiliar na migração.
        /// </summary>
        public int? IdSubgrupoProduto
        {
            get { return (int?)CodigoSubgrupoProduto; }
            set { CodigoSubgrupoProduto = (uint?)value; }
        }

        public uint? CodigoSubgrupoProduto
        {
            get { return Glass.Conversoes.StrParaUintNullable(hdfSubgrupoProd.Value); }
            set
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "ctrlSelGrupoSubgrupoProd_subgrupo",
                    "buscaSubgrupos('" + this.ClientID + "', document.getElementById('" + drpGrupoProd.ClientID + @"'), " + ExibirSubgrupoProdutoVazio.ToString().ToLower() + @");
                    var subgrupo = document.getElementById('" + drpSubgrupoProd.ClientID + @"');
                    if (subgrupo) {
                        subgrupo.value = '" + value + @"';
                        alteraSubgrupo('" + this.ClientID + @"', subgrupo);
                    }", true);
            }
        }
    
        #endregion
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string BuscarSubgrupos(string idGrupoProd, string buscarVazio)
        {
            try
            {
                const string FORMATO = "<option value='{0}'>{1}</option>";
    
                StringBuilder options = new StringBuilder();
                if (buscarVazio.ToLower() == "true")
                    options.AppendFormat(FORMATO, "", "");
    
                foreach (var s in SubgrupoProdDAO.Instance.GetForFilter(Glass.Conversoes.StrParaInt(idGrupoProd)))
                    if (s.IdSubgrupoProd > 0)
                        options.AppendFormat(FORMATO, s.IdSubgrupoProd, (s.Descricao ?? String.Empty).Replace("|", ""));
    
                return "Ok|" + options.ToString();
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlSelGrupoSubgrupoProd));
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlSelGrupoSubgrupoProd_script"))
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlSelGrupoSubgrupoProd_script", @"
                    function alteraSubgrupo(nomeControle, drpSubgrupo)
                    {
                        var hdfSubgrupo = document.getElementById(nomeControle + '_hdfSubgrupoProd');
                        hdfSubgrupo.value = drpSubgrupo.value;
                    }
    
                    function buscaSubgrupos(nomeControle, drpGrupo, exibirVazio)
                    {
                        if (drpGrupo == null) return;
    
                        var drpSubgrupo = document.getElementById(nomeControle + '_drpSubgrupoProd');
                        var itens = drpGrupo.value == '' ? ['', ''] :
                            ctrlSelGrupoSubgrupoProd.BuscarSubgrupos(drpGrupo.value, exibirVazio).value.split('|');
    
                        if (itens[0] == 'Erro')
                        {
                            alert(itens[1]);
                            drpSubgrupo.innerHTML = '';
                        }
                        else
                            drpSubgrupo.innerHTML = itens[1];
    
                        drpSubgrupo.value = '';
                        alteraSubgrupo(nomeControle, drpSubgrupo);
                    }", true);
    
            if (!IsPostBack)
            {
                drpGrupoProd.Attributes["OnChange"] = "buscaSubgrupos('" + this.ClientID + "', this, " + ExibirSubgrupoProdutoVazio.ToString().ToLower() + ")";
                drpSubgrupoProd.Attributes["OnChange"] = "alteraSubgrupo('" + this.ClientID + "', this)";
            }
        }
    
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (ApenasVidros)
            {
                if (drpGrupoProd.Items.Count < 2)
                    drpGrupoProd.DataBind();
    
                drpGrupoProd.SelectedValue = (int)Glass.Data.Model.NomeGrupoProd.Vidro + "";
                drpGrupoProd.Enabled = false;
            }
        }
    }
}
