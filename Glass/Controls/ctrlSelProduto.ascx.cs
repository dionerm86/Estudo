using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlSelProduto : BaseUserControl
    {
        private string _callback;
        private bool? _compra;
        private bool? _nf;

        #region Propriedades

        /// <summary>
        /// Identificador do produto para Int32.
        /// </summary>
        public int? IdProdInt32
        {
            get { return (int?)IdProd; }
            set { IdProd = (uint?)value; }
        }

        public uint? IdProd
        {
            get { return Glass.Conversoes.StrParaUintNullable(ctrlSelProdBuscar.Valor); }
            set
            {
                if (value != null && ProdutoDAO.Instance.Exists(value.Value))
                {
                    ctrlSelProdBuscar.Valor = value.Value.ToString();
                    ctrlSelProdBuscar.Descricao = ProdutoDAO.Instance.GetCodInterno((int)value.Value);
                    lblDescricaoProd.Text = ProdutoDAO.Instance.ObtemDescricao((int)value.Value);
                    hdfTipoCalculo.Value = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, (int)value.Value, Nf.GetValueOrDefault(false)).ToString();
    
                    Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_setProd",
                        this.ClientID + ".CallbackSelecao(" + this.ClientID + "," + value + ");\n", true);
                }
                else
                {
                    ctrlSelProdBuscar.Valor = null;
                    ctrlSelProdBuscar.Descricao = null;
                    lblDescricaoProd.Text = null;
                    hdfTipoCalculo.Value = null;
                }
            }
        }
    
        public bool PermitirVazio
        {
            get { return ctrlSelProdBuscar.PermitirVazio; }
            set { ctrlSelProdBuscar.PermitirVazio = value; }
        }
    
        public string Callback
        {
            get { return _callback ?? ""; }
            set { _callback = value; }
        }
    
        public bool FazerPostBackBotaoPesquisar
        {
            get { return ctrlSelProdBuscar.FazerPostBackBotaoPesquisar; }
            set { ctrlSelProdBuscar.FazerPostBackBotaoPesquisar = value; }
        }
    
        public bool? Compra
        {
            get { return _compra; }
            set { _compra = value; }
        }
    
        public bool? Nf
        {
            get { return _nf; }
            set { _nf = value; }
        }

        public string DescricaoItemGenerico
        {
            get { return txtDescricaoItemGenerico.Text; }
            set { txtDescricaoItemGenerico.Text = value; }
        }
    
        public CustomValidator Validador
        {
            get { return ctrlSelProdBuscar.Validador; }
        }
    
        #endregion
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string GetDadosProduto(string idProdStr, string isCompraStr, string isNfStr)
        {
            uint idProd = Glass.Conversoes.StrParaUint(idProdStr);
            bool isCompra = false;
            bool isNf = !String.IsNullOrEmpty(isNfStr) && bool.Parse(isNfStr);
    
            if (idProd > 0 && ProdutoDAO.Instance.Exists(idProd))
            {
                if (!String.IsNullOrEmpty(isCompraStr) && !(isCompra = bool.Parse(isCompraStr)))
                {
                    if (ProdutoDAO.Instance.ObtemCompra(idProd))
                        return "Erro|Produto apenas para compra.";
                }

                return "Ok|" + ProdutoDAO.Instance.ObtemDescricao((int)idProd).Replace("|", "") + "|" +
                    Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, (int)idProd, isNf ||
                    (isCompra && CompraConfig.UsarTipoCalculoNfParaCompra)) + "|" +
                    ProdutoDAO.Instance.ObtemItemGenerico(idProd).ToString().ToLower();
            }
            else
                return "Erro|Produto não encontrado.";
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlSelProduto));
    
            // Registra este script no Page_Load para que as funções inseridas abaixo no Page_PreRender funcionem
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlSelProduto_script"))
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlSelProduto_script", this.ResolveClientUrl("~/Scripts/ctrlSelProduto.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
        }
    
        protected void Page_PreRender(object sender, EventArgs e)
        {
            string dadosControle = @"
                Callback: '{0}',
                Compra: {1},
                Nf: {2}
            ";
    
            ctrlSelProdBuscar.CallbackSelecao = this.ClientID + ".ObtemDadosProduto";
    
            dadosControle = String.Format(dadosControle,
                Callback,
                Compra != null ? Compra.Value.ToString().ToLower() : "null",
                Nf != null ? Nf.Value.ToString().ToLower() : "null"
            );
    
            Page.ClientScript.RegisterClientScriptBlock(GetType(), this.ClientID,
                String.Format("var {0} = new SelProdutoType('{0}', {1});\n", this.ClientID, "{" + dadosControle + "}"), true);
    
            // A atribuição de idProd no IdProd deve ser feita aqui, pois o Set do IdProd registra um script na página que depende do registro feito acima.
            if (IsPostBack)
                if (!String.IsNullOrEmpty(ctrlSelProdBuscar.Valor))
                {
                    uint idProd = Glass.Conversoes.StrParaUint(ctrlSelProdBuscar.Valor);
                    IdProd = idProd;
                }
        }
    }
}
