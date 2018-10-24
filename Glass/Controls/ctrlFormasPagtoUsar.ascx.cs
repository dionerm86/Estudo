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

        protected void CblFormasPagto_DataBound(object sender, EventArgs e)
        {
            var itens = this._formasPagto ?? new Financeiro.Negocios.Entidades.FormaPagtoCliente[0];

            if (this._formasPagto == null && FinanceiroConfig.FormaPagamento.FormaPagtoPadraoDesmarcada)
            {
                return;
            }

            // Marca as formas de pagamento que o cliente tem permissão
            foreach (ListItem item in this.cblFormasPagto.Items)
            {
                var p = itens.FirstOrDefault(x => x.IdFormaPagto == item.Value.StrParaInt());
                item.Selected = p == null;

                //Chamado 37161
                if (this._formasPagto == null && item.Value.StrParaInt() == (int)Pagto.FormaPagto.Permuta)
                {
                    item.Selected = false;
                }
            }
        }

        protected void DrpFormaPagto_DataBound(object sender, EventArgs e)
        {
            var itens = this._formasPagto ?? new Financeiro.Negocios.Entidades.FormaPagtoCliente[0];

            // Marca as formas de pagamento que o cliente tem permissão
            foreach (ListItem item in this.cblFormasPagto.Items)
            {
                var p = itens.FirstOrDefault(x => x.IdFormaPagto == item.Value.StrParaInt());

                if ((p != null) && (this._formaPagtoPadrao != item.Value.StrParaInt()))
                {
                    var itemRemover = new ListItem(item.Text, item.Value);
                    this.drpFormaPagto.Items.Remove(itemRemover);
                }
            }

            if (this.drpFormaPagto.Items.Count > 0)
            {
                this.drpFormaPagto.SelectedIndex = this._formaPagtoPadrao != null ?
                    this.drpFormaPagto.Items.IndexOf(this.drpFormaPagto.Items.FindByValue(this._formaPagtoPadrao.Value.ToString())) : 0;
            }
        }

        protected void CblFormasPagto_PreRender(object sender, EventArgs e)
        {
            for (int i = 0; i < this.cblFormasPagto.Items.Count; i++)
            {
                this.cblFormasPagto.Items[i].Attributes.Add(
                    "onclick",
                    $"alterarFormasPagtoPadrao(this.checked, {this.cblFormasPagto.Items[i].Value}, '{this.cblFormasPagto.Items[i].Text}', {this._formaPagtoPadrao})");
            }
        }
    }
}
