using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlSelCliente : BaseUserControl
    {
        #region Propriedades
    
        public uint? IdCliente
        {
            get { return Glass.Conversoes.StrParaUintNullable(ctrlSelClienteBuscar.Valor); }
            set
            {
                if (value != null && ClienteDAO.Instance.Exists(value.Value))
                {
                    ctrlSelClienteBuscar.Valor = value.Value.ToString();
                    ctrlSelClienteBuscar.Descricao = ctrlSelClienteBuscar.Valor;
                    lblNomeCliente.Text = ClienteDAO.Instance.GetNome(value.Value);
                }
                else
                {
                    ctrlSelClienteBuscar.Valor = null;
                    ctrlSelClienteBuscar.Descricao = null;
                    lblNomeCliente.Text = null;
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
            get { return ctrlSelClienteBuscar.FazerPostBackBotaoPesquisar; }
            set { ctrlSelClienteBuscar.FazerPostBackBotaoPesquisar = value; }
        }
    
        #endregion
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string GetDadosCliente(string idClienteStr)
        {
            uint idCliente = Glass.Conversoes.StrParaUint(idClienteStr);
            if (idCliente > 0 && ClienteDAO.Instance.Exists(idCliente))
                return "Ok|" + ClienteDAO.Instance.GetNome(idCliente).Replace("|", "");
            else
                return "Erro|Cliente não encontrado";
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlSelCliente));
    
            if (IsPostBack)
                if (!String.IsNullOrEmpty(ctrlSelClienteBuscar.Valor))
                {
                    uint idCli = Glass.Conversoes.StrParaUint(ctrlSelClienteBuscar.Valor);
                    IdCliente = idCli;
                }
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlSelCliente_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlSelCliente_script", @"
                    function getDescricaoSelCliente(nomeControle, id)
                    {
                        var controleOriginal = nomeControle;
                        nomeControle = nomeControle.substr(0, nomeControle.lastIndexOf('_'));
                        
                        var lblDescricao = document.getElementById(nomeControle + '_lblNomeCliente');
                        if (lblDescricao == null)
                            return;
                        
                        var resposta = id != '' ? ctrlSelCliente.GetDadosCliente(id).value.split('|') : new Array('', '');
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
