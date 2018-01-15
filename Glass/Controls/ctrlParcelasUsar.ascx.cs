using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlParcelasUsar : BaseUserControl
    {
        #region Campos privados
    
        private int? _formaPagtoPadrao = null;
        private IList<Financeiro.Negocios.Entidades.ParcelasNaoUsar> _parcelasNaoUsar;
        private int? _id;
        private int? _idCliente;

        #endregion

        #region Propriedades

        public IEnumerable<Financeiro.Negocios.Entidades.ParcelasNaoUsar> ParcelasNaoUsar
        {
            get { return _parcelasNaoUsar; }
            set { _parcelasNaoUsar = value.ToList(); }
        }

        public int? FormaPagtoPadrao
        {
            get { return drpTipoPagto.SelectedValue.StrParaIntNullable(); }
            set { _formaPagtoPadrao = value; }
        }
    
        public bool BloquearPagto
        {
            get { return chkBloquearPagto.Checked; }
            set { chkBloquearPagto.Checked = value; }
        }

        public int? IdFornec
        {
            get { return _id; }
            set { _id = value; }
        }

        public int? IdCliente
        {
            get { return _idCliente; }
            set { _idCliente = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            imgTooltip.OnClientClick = "exibirParcelas(this, '" + cblParcelas.ClientID + "'); return false;";

            if (_parcelasNaoUsar == null)
                _parcelasNaoUsar = new List<Financeiro.Negocios.Entidades.ParcelasNaoUsar>();

            if (_parcelasNaoUsar != null && IsPostBack)
            {
                foreach (ListItem item in cblParcelas.Items)
                {
                    var idParcela = item.Value.StrParaInt();
                    var naLista = _parcelasNaoUsar.FirstOrDefault(x => x.IdParcela == idParcela);

                    if (item.Selected)
                    {
                        if (naLista != null)
                            _parcelasNaoUsar.Remove(naLista);
                    }
                    else if (naLista == null)
                        _parcelasNaoUsar.Add(new Financeiro.Negocios.Entidades.ParcelasNaoUsar()
                        {
                            IdParcela = idParcela
                        });
                }
            }
        }
    
        protected void cblParcelas_DataBound(object sender, EventArgs e)
        {
            var parcelas = odsParcelas.Select().Cast<Colosoft.Business.BusinessEntityDescriptor>();

            if (IdFornec > 0)
            {
                foreach (ListItem item in cblParcelas.Items)
                {
                    item.Attributes.Add("OnClick", "habilitar(this, " + item.Value + ", document.getElementById('" + drpTipoPagto.ClientID + "'))");

                    uint idParcela = item.Value.StrParaUint();
                    var p = _parcelasNaoUsar.FirstOrDefault(x => x.IdParcela == idParcela);
                    item.Selected = p == null;
                }
            }
            else
            {
                foreach (ListItem item in cblParcelas.Items)
                {
                    item.Attributes.Add("OnClick", "habilitar(this, " + item.Value + ", document.getElementById('" + drpTipoPagto.ClientID + "'))");

                    uint idParcela = item.Value.StrParaUint();

                    // Se for atualização de cliente e a lista de "parcelas não usar" estiver vazia, quer dizer que o cliente tem acesso à todas as parcelas
                    if (IdCliente > 0 && _parcelasNaoUsar != null && _parcelasNaoUsar.Count() == 0)
                    {
                        item.Selected = true;
                    }
                    else if (_parcelasNaoUsar != null && _parcelasNaoUsar.Count() > 0)
                    {
                        var p = _parcelasNaoUsar.FirstOrDefault(x => x.IdParcela == idParcela);
                        item.Selected = p == null;
                    }
                    else
                    {
                        var parcela = parcelas.FirstOrDefault(x => x.Id == idParcela);
                        if (parcela == null)
                            continue;

                        var parcelaFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                            .Current.GetInstance<Financeiro.Negocios.IParcelasFluxo>();

                        item.Selected = parcelaFluxo.ObtemParcela(parcela.Id).ParcelaPadrao;
                    }
                }
            }
        }
    
        protected void drpTipoPagto_DataBound(object sender, EventArgs e)
        {
            if (drpTipoPagto.Items.Count > 0)
                drpTipoPagto.SelectedIndex = _formaPagtoPadrao != null ? drpTipoPagto.Items.IndexOf(drpTipoPagto.Items.FindByValue(_formaPagtoPadrao.Value.ToString())) : 0;
        }
    }
}
