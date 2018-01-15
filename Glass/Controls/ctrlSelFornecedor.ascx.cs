using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlSelFornecedor : BaseUserControl
    {
        #region Propriedades
    
        public uint? IdFornec
        {
            get { return Glass.Conversoes.StrParaUintNullable(ctrlSelFornecBuscar.Valor); }
            set
            {
                if (value != null && FornecedorDAO.Instance.Exists(value.Value))
                {
                    ctrlSelFornecBuscar.Valor = value.Value.ToString();
                    ctrlSelFornecBuscar.Descricao = ctrlSelFornecBuscar.Valor;
                    lblNomeFornec.Text = FornecedorDAO.Instance.GetNome(value.Value);
                }
                else
                {
                    ctrlSelFornecBuscar.Valor = null;
                    ctrlSelFornecBuscar.Descricao = null;
                    lblNomeFornec.Text = null;
                }
            }
        }
    
        public string Callback
        {
            get { return hdfCallback.Value; }
            set { hdfCallback.Value = value; }
        }
    
        public bool FazerPostBackBotaoPesquisar
        {
            get { return ctrlSelFornecBuscar.FazerPostBackBotaoPesquisar; }
            set { ctrlSelFornecBuscar.FazerPostBackBotaoPesquisar = value; }
        }
    
        #endregion
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string GetDadosFornecedor(string idFornecStr)
        {
            uint idFornec = Glass.Conversoes.StrParaUint(idFornecStr);
            if (idFornec > 0 && FornecedorDAO.Instance.Exists(idFornec))
                return "Ok|" + FornecedorDAO.Instance.GetNome(idFornec).Replace("|", "");
            else
                return "Erro|Fornecedor não encontrado";
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlSelFornecedor));
    
            if (IsPostBack)
                if (!String.IsNullOrEmpty(ctrlSelFornecBuscar.Valor))
                {
                    uint idFornec = Glass.Conversoes.StrParaUint(ctrlSelFornecBuscar.Valor);
                    IdFornec = idFornec;
                }
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlSelFornecedor_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlSelFornecedor_script", @"
                    function getDescricaoSelFornecedor(nomeControle, id)
                    {
                        var controleOriginal = nomeControle;
                        nomeControle = nomeControle.substr(0, nomeControle.lastIndexOf('_'));
                        
                        var lblDescricao = document.getElementById(nomeControle + '_lblNomeFornec');
                        if (lblDescricao == null)
                            return;
                        
                        var resposta = id != '' ? ctrlSelFornecedor.GetDadosFornecedor(id).value.split('|') : new Array('', '');
                        if (resposta[0] == 'Erro')
                            alert(resposta[1]);
                        else
                            lblDescricao.innerHTML = resposta[1];
                        
                        var callback = document.getElementById(nomeControle + '_hdfCallback').value;
                        if (callback != '')
                            eval(callback + '(\'' + controleOriginal + '\', \'' + id + '\')');
                    }", true);
            }
        }
    }
}
