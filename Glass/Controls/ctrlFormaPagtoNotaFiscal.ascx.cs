using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlFormaPagtoNotaFiscal : BaseUserControl
    {
        #region Variáveis Locais

        #endregion

        #region Propriedades

        /// <summary>
        /// Pagamentos
        /// </summary>
        public IList<Data.Model.PagtoNotaFiscal> PagtoNotaFiscal { get; set; }

        #endregion

        #region Métodos Protegidos

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlFormaPagtoNotaFiscal_script"))
            {
                Page.ClientScript.RegisterClientScriptInclude("ctrlFormaPagtoNotaFiscal_script", ResolveUrl("~/Scripts/ctrlFormaPagtoNotaFiscal.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
                Page.ClientScript.RegisterStartupScript(typeof(string), this.ClientID, "<script>var " + this.ID + " = new ctrlFormaPagtoNotaFiscal('" + this.ClientID + "');</script>");
            }

            if(string.IsNullOrEmpty(imgAdicionar.OnClientClick))
                imgAdicionar.OnClientClick = this.ID + ".adicionarLinha(); return false;";

            if (string.IsNullOrEmpty(imgRemover.OnClientClick))
                imgRemover.OnClientClick = this.ID + ".removerLinha(); return false;";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (PagtoNotaFiscal == null || !PagtoNotaFiscal.Any())
                return;

            hdfFormaPagto.Value = string.Join(";", PagtoNotaFiscal.Select(f => f.FormaPagto.ToString()).ToArray());
            hdfValoreReceb.Value = string.Join(";", PagtoNotaFiscal.Select(f => f.Valor.ToString()).ToArray());
            hdfBandeira.Value = string.Join(";", PagtoNotaFiscal.Select(f => f.Bandeira.ToString()).ToArray());
            hdfCnpj.Value = string.Join(";", PagtoNotaFiscal.Select(f => f.CnpjCredenciadora).ToArray());
            hdfNumAut.Value = string.Join(";", PagtoNotaFiscal.Select(f => f.NumAut).ToArray());

            Page.ClientScript.RegisterStartupScript(GetType(), "iniciar" + new Random().Next(0, 1000), this.ID + ".carregaPagamentos();", true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var formasPagto = hdfFormaPagto.Value.Split(';');
            var valorReceb = hdfValoreReceb.Value.Split(';');
            var bandeira = hdfBandeira.Value.Split(';');
            var cnpj = hdfCnpj.Value.Split(';');
            var numAut = hdfNumAut.Value.Split(';');

            var index = 0;
            PagtoNotaFiscal = formasPagto.Select(f => new Data.Model.PagtoNotaFiscal()
            {
                FormaPagto = formasPagto[index].StrParaInt(),
                Valor = valorReceb[index].StrParaDecimal(),
                Bandeira = bandeira[index].StrParaIntNullable(),
                CnpjCredenciadora = cnpj[index],
                NumAut = numAut[index++]
            })
            .ToList();
        }

            #endregion
        }
}