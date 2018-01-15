using System;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlSelCidade : BaseUserControl
    {
        #region Propriedades
    
        public uint? IdCidade
        {
            get { return Glass.Conversoes.StrParaUintNullable(ctrlSelCidade1.Valor); }
            set
            {
                if (value != null)
                {
                    Cidade c = CidadeDAO.Instance.GetElementByPrimaryKey(value.Value);
                    if (drpUf.Items.Count == 0) drpUf.DataBind();
    
                    ctrlSelCidade1.Valor = c.IdCidade.ToString();
                    drpUf.SelectedValue = c.NomeUf;
                }
                else
                {
                    ctrlSelCidade1.Valor = "";
                    ctrlSelCidade1.Descricao = "";
                    drpUf.SelectedValue = "";
                }
            }
        }
    
        public bool PermitirVazio
        {
            get { return ctrlSelCidade1.PermitirVazio; }
            set { ctrlSelCidade1.PermitirVazio = value; }
        }
    
        public bool FazerPostBackBotaoPesquisar
        {
            get { return ctrlSelCidade1.FazerPostBackBotaoPesquisar; }
            set { ctrlSelCidade1.FazerPostBackBotaoPesquisar = value; }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                if (!String.IsNullOrEmpty(ctrlSelCidade1.Valor))
                {
                    uint idCidade = Glass.Conversoes.StrParaUint(ctrlSelCidade1.Valor);
                    IdCidade = idCidade;
                }
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlSelCidade_script"))
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlSelCidade_script", @"
                    function limpar(drpUf)
                    {
                        var tabela = drpUf;
                        while (tabela.nodeName.toLowerCase() != 'table')
                            tabela = tabela.parentNode;
                        
                        var hdfValor = FindControl('hdfValor', 'input', tabela);
                        var txtDescr = FindControl('txtDescr', 'input', tabela);
                        
                        hdfValor.value = '';
                        txtDescr.value = '';
                    }", true);
        }
    }
}
