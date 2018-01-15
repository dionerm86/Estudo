using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using System.Collections.Generic;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlFormasPagtoUsar : BaseUserControl
    {
        #region Campos privados
    
        private int? _formaPagtoPadrao = null;
        private IList<Financeiro.Negocios.Entidades.FormaPagtoCliente> _formasPagto;
    
        #endregion
    
        #region Propriedades
    
        public IEnumerable<Financeiro.Negocios.Entidades.FormaPagtoCliente> FormasPagto
        {
            get { return _formasPagto; }
            set { _formasPagto = value.ToList(); }
        }
    
        public int? FormaPagtoPadrao
        {
            get { return drpFormaPagto.SelectedValue.StrParaIntNullable(); }
            set { _formaPagtoPadrao = value; }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            imgTooltip.OnClientClick = "exibirFormasPagto(this, '" + cblFormasPagto.ClientID + "'); return false;";

            if (_formasPagto == null)
                _formasPagto = new List<Glass.Financeiro.Negocios.Entidades.FormaPagtoCliente>();

            if (_formasPagto != null && IsPostBack)
            {
                foreach (ListItem item in cblFormasPagto.Items)
                {
                    var idFormaPagto = item.Value.StrParaInt();
                    var naLista = _formasPagto.FirstOrDefault(x => x.IdFormaPagto == idFormaPagto);

                    if (item.Selected)
                    {
                        if (naLista != null)
                            _formasPagto.Remove(naLista);
                    }
                    else if (naLista == null)
                        _formasPagto.Add(new Financeiro.Negocios.Entidades.FormaPagtoCliente()
                        {
                            IdFormaPagto = idFormaPagto
                        });
                }
            }
        }
    
        protected void cblFormasPagto_DataBound(object sender, EventArgs e)
        {
            var itens = _formasPagto ?? new Financeiro.Negocios.Entidades.FormaPagtoCliente[0];

            if (_formasPagto == null && FinanceiroConfig.FormaPagamento.FormaPagtoPadraoDesmarcada)
                return;

            // Marca as formas de pagamento que o cliente tem permissão
            foreach (ListItem item in cblFormasPagto.Items)
            {
                var p = itens.FirstOrDefault(x => x.IdFormaPagto == item.Value.StrParaInt());
                item.Selected = p == null;

                //Chamado 37161
                if(_formasPagto == null && item.Value.StrParaInt() == (int)Pagto.FormaPagto.Permuta)
                    item.Selected = false;
            }
        }
    
        protected void drpFormaPagto_DataBound(object sender, EventArgs e)
        {
            if (drpFormaPagto.Items.Count > 0)
                drpFormaPagto.SelectedIndex = _formaPagtoPadrao != null ? drpFormaPagto.Items.IndexOf(drpFormaPagto.Items.FindByValue(_formaPagtoPadrao.Value.ToString())) : 0;
        }
    }
}
