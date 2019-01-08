using Microsoft.Practices.ServiceLocation;
using System;
using System.Web;
using Colosoft;

namespace Glass.Financeiro.UI.Web.Process.Handlers
{
    public class ArquivoGCon : IHttpHandler
    {
        #region Propiedades

        [Colosoft.Web.QueryString("idContaPg")]
        public int? IdContaPg { get; set; }

        [Colosoft.Web.QueryString("idPedido")]
        public int? IdPedido { get; set; }

        [Colosoft.Web.QueryString("idLiberarPedido")]
        public int? IdLiberarPeidido { get; set; }

        [Colosoft.Web.QueryString("idAcerto")]
        public int? IdAcerto { get; set; }

        [Colosoft.Web.QueryString("idAcertoParcial")]
        public int? IdAcertoParcial { get; set; }

        [Colosoft.Web.QueryString("idTrocaDev")]
        public int? IdTrocaDev { get; set; }

        [Colosoft.Web.QueryString("numeroNFe")]
        public int? NumeroNfe { get; set; }

        [Colosoft.Web.QueryString("idLoja")]
        public int? IdLoja { get; set; }

        [Colosoft.Web.QueryString("idCli")]
        public int? IdCliente { get; set; }

        [Colosoft.Web.QueryString("idFunc")]
        public int? IdFunc { get; set; }

        [Colosoft.Web.QueryString("idFuncRecebido")]
        public int? IdFuncRecebido { get; set; }

        [Colosoft.Web.QueryString("tipoEntrega")]
        public int? TipoEntrega { get; set; }

        [Colosoft.Web.QueryString("nomeCli")]
        public string NomeCli { get; set; }

        [Colosoft.Web.QueryString("dataIniCad")]
        public DateTime? DataIniCad { get; set; }

        [Colosoft.Web.QueryString("dataFimCad")]
        public DateTime? DataFimCad { get; set; }

        [Colosoft.Web.QueryString("dtIniVenc")]
        public DateTime? DtInivenc { get; set; }

        [Colosoft.Web.QueryString("dtFimVenc")]
        public DateTime? DtFimVenc { get; set; }

        [Colosoft.Web.QueryString("dtIniRec")]
        public DateTime? DtIniRec { get; set; }

        [Colosoft.Web.QueryString("dtFimRec")]
        public DateTime? DtFimRec { get; set; }

        [Colosoft.Web.QueryString("idFormaPagto")]
        public int? IdFormaPagto { get; set; }

        [Colosoft.Web.QueryString("tipoBoleto")]
        public int? TipoBoleto { get; set; }

        [Colosoft.Web.QueryString("valorInicial")]
        public decimal? ValorInicial { get; set; }

        [Colosoft.Web.QueryString("valorFinal")]
        public decimal? ValorFinal { get; set; }

        [Colosoft.Web.QueryString("idContaBancoRecebimento")]
        public int? IdContaBancoRecebimento { get; set; }

        [Colosoft.Web.QueryString("renegociadas")]
        public bool Renegociadas { get; set; }

        [Colosoft.Web.QueryString("idComissionado")]
        public int? IdComissionado { get; set; }

        [Colosoft.Web.QueryString("idRota")]
        public int? IdRota { get; set; }

        [Colosoft.Web.QueryString("obs")]
        public string Obs { get; set; }

        [Colosoft.Web.QueryString("numArqRemessa")]
        public int? NumArqRemessa { get; set; }

        [Colosoft.Web.QueryString("refObra")]
        public bool RefObra { get; set; }

        [Colosoft.Web.QueryString("contasCnab")]
        public int? ContasCnab { get; set; }

        [Colosoft.Web.QueryString("receber")]
        public bool Receber { get; set; }

        [Colosoft.Web.QueryString("idCompra")]
        public int? IdCompra { get; set; }

        [Colosoft.Web.QueryString("idCustoFixo")]
        public int? IdCustoFixo { get; set; }

        [Colosoft.Web.QueryString("idImpServ")]
        public int? IdImpServ { get; set; }

        [Colosoft.Web.QueryString("idComissao")]
        public int? IdComissao { get; set; }

        [Colosoft.Web.QueryString("idFornec")]
        public int? IdFornec { get; set; }

        [Colosoft.Web.QueryString("nomeFornec")]
        public string NomeFornec { get; set; }

        [Colosoft.Web.QueryString("idConta")]
        public int? IdConta { get; set; }

        [Colosoft.Web.QueryString("custoFixo")]
        public bool CustoFixo { get; set; }

        [Colosoft.Web.QueryString("comissao")]
        public bool Comissao { get; set; }

        [Colosoft.Web.QueryString("contasVinculadas")]
        public bool ContasVinculadas { get; set; }

        [Colosoft.Web.QueryString("jurosMulta")]
        public bool JurosMulta { get; set; }

        [Colosoft.Web.QueryString("idVendedorObra")]
        public int? IdVendedorObra { get; set; }
 
        [Colosoft.Web.QueryString("observacao")]
        public string Observacao { get; set; }

        #endregion

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                this.RefreshFromParameters(context.Request, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));

                var gConFluxo = ServiceLocator.Current.GetInstance<Glass.Financeiro.Negocios.IGConFluxo>();

                var arq = new Glass.Financeiro.Negocios.Entidades.GCon.Arquivo();

                if (Receber)
                    arq = gConFluxo.GerarArquivoRecebidas(IdPedido, IdLiberarPeidido, IdAcerto, IdAcertoParcial, IdTrocaDev, NumeroNfe,
                        IdLoja, IdCliente, IdFunc, IdFuncRecebido, TipoEntrega, NomeCli, DtInivenc, DtFimVenc, DtIniRec, DtFimRec,
                        DataIniCad, DataFimCad, IdFormaPagto, TipoBoleto, ValorInicial, ValorFinal, IdContaBancoRecebimento, Renegociadas, IdComissionado, IdRota,
                        Obs, NumArqRemessa, IdVendedorObra, RefObra, ContasCnab, ContasVinculadas);
                else
                    arq = gConFluxo.GerarArquivoPagas(IdContaPg, IdCompra, NumeroNfe, IdCustoFixo, IdImpServ, IdComissao, ValorInicial, ValorFinal, DataIniCad, DataFimCad, DtInivenc, DtFimVenc,
                        DtIniRec, DtFimRec, IdLoja, IdFornec, NomeFornec, IdFormaPagto, IdConta, JurosMulta, Observacao);

                if (arq == null)
                    throw new Exception("Nenhuma conta encontrada.");

                var data = DateTime.Now;
                var nomeArquivo = "GCON_" + data.Day + "_" + data.Month + "_" + data.Year + "_" + data.Millisecond+".txt";

                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + nomeArquivo + "\"");

                arq.Salvar(context.Response.OutputStream);
            }
            catch (Exception ex)
            {
                // Devolve o erro
                context.Response.ContentType = "text/html";
                context.Response.Write(GetErrorResponse(ex));
            }
        }

        private string GetErrorResponse(Exception ex)
        {
            bool debug = false;

            string html = debug ? ex.ToString().Replace("\n", "<br>").Replace("\r", "").Replace(" ", "&nbsp;") : @"
            <script type='text/javascript'>
                alert('" + Glass.MensagemAlerta.FormatErrorMsg("", ex) + @"');
                window.history.go(-1);
            </script>";

            return @"
            <html>
                <body>
                    " + html + @"
                </body>
            </html>";
        }
    }
}
