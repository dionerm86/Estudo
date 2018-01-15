using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Colosoft;

namespace Glass.Financeiro.UI.Web.Process.Handlers
{
    public class ArquivoDominio : IHttpHandler
    {
        #region Propiedades

        [Colosoft.Web.QueryString("idContaPg")]
        public int? IdContaPg { get; set; }

        /// <summary>
        /// Identificador do Pedido.
        /// </summary>
        [Colosoft.Web.QueryString("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Identificador da Liberação
        /// </summary>
        [Colosoft.Web.QueryString("idLiberarPedido")]
        public int? IdLiberarPedido { get; set; }

        /// <summary>
        /// Identificador do Acerto
        /// </summary>
        [Colosoft.Web.QueryString("idAcerto")]
        public int? IdAcerto { get; set; }

        /// <summary>
        /// Identificador do Acerto Parcial.
        /// </summary>
        [Colosoft.Web.QueryString("idAcertoParcial")]
        public int? IdAcertoParcial { get; set; }

        /// <summary>
        /// Identificador da Troca/Devolução.
        /// </summary>
        [Colosoft.Web.QueryString("idTrocaDev")]
        public int? IdTrocaDevolucao { get; set; }

        /// <summary>
        /// Numero da Nota Fiscal.
        /// </summary>
        [Colosoft.Web.QueryString("numeroNFe")]
        public int? NumeroNfe { get; set; }

        /// <summary>
        /// Identifcador da Loja.
        /// </summary>
        [Colosoft.Web.QueryString("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Identificador do Cliente.
        /// </summary>
        [Colosoft.Web.QueryString("idCli")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Identificador do Funcionario
        /// </summary>
        [Colosoft.Web.QueryString("idFunc")]
        public int? IdFuncionario { get; set; }

        /// <summary>
        /// Identificador do Funcionario que Recebeu.
        /// </summary>
        [Colosoft.Web.QueryString("idFuncRecebido")]
        public int? IdFuncionarioRecebido { get; set; }

        /// <summary>
        /// Tipo de Entrega do Orçamento e do Pedido.
        /// </summary>
        [Colosoft.Web.QueryString("tipoEntrega")]
        public int? TipoEntrega { get; set; }

        /// <summary>
        /// Nome do Cliente.
        /// </summary>
        [Colosoft.Web.QueryString("nomeCli")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Data Inicio do Vencimento.
        /// </summary>
        [Colosoft.Web.QueryString("dtIniVenc")]
        public DateTime? DataInicioVencimento { get; set; }

        /// <summary>
        /// Data Fim do Vencimento.
        /// </summary>
        [Colosoft.Web.QueryString("dtFimVenc")]
        public DateTime? DataFimVencimento { get; set; }

        /// <summary>
        /// Data Inicio do Recebimento.
        /// </summary>
        [Colosoft.Web.QueryString("dtIniRec")]
        public DateTime? DataInicioRecebimento { get; set; }

        /// <summary>
        /// Data Fim do Recebimento.
        /// </summary>
        [Colosoft.Web.QueryString("dtFimRec")]
        public DateTime? DataFimRecebimento { get; set; }

        /// <summary>
        /// Identificador da Forma de Pagamento
        /// </summary>
        [Colosoft.Web.QueryString("idFormaPagto")]
        public int? IdFormaPagto { get; set; }

        /// <summary>
        /// Tipo do Boleto.
        /// </summary>
        [Colosoft.Web.QueryString("tipoBoleto")]
        public int? TipoBoleto { get; set; }

        /// <summary>
        /// Valor Inicial.
        /// </summary>
        [Colosoft.Web.QueryString("valorInicial")]
        public decimal? ValorInicial { get; set; }

        /// <summary>
        /// Valor Final
        /// </summary>
        [Colosoft.Web.QueryString("valorFinal")]
        public decimal? ValorFinal { get; set; }

        /// <summary>
        /// Identificador do Comissionado
        /// </summary>
        [Colosoft.Web.QueryString("idComissionado")]
        public int? IdComissionado { get; set; }

        /// <summary>
        /// Identificador da Rota.
        /// </summary>
        [Colosoft.Web.QueryString("idRota")]
        public int? IdRota { get; set; }

        /// <summary>
        /// Obsercação da Conta Recebida.
        /// </summary>
        [Colosoft.Web.QueryString("obs")]
        public string Obs { get; set; }

        /// <summary>
        /// Tipo de Conta Contabil
        /// </summary>
        [Colosoft.Web.QueryString("tipoConta")]
        public string TipoConta { get; set; }

        /// <summary>
        /// Numero do Arquivo Remessa
        /// </summary>
        [Colosoft.Web.QueryString("numArqRemessa")]
        public int? NumArqRemessa { get; set; }

        /// <summary>
        /// Referencia da Obra.
        /// </summary>
        [Colosoft.Web.QueryString("refObra")]
        public bool RefObra { get; set; }

        /// <summary>
        /// Contas Cnab
        /// </summary>
        [Colosoft.Web.QueryString("contasCnab")]
        public int? ContasCnab { get; set; }

        /// <summary>
        /// Identificador do Vendedodr Associado.
        /// </summary>
        [Colosoft.Web.QueryString("idVendedorAssociado")]
        public int? IdVendedorAssociado { get; set; }

        /// <summary>
        /// Identificador do Vendedor Obra.
        /// </summary>
        [Colosoft.Web.QueryString("idVendedorObra")]
        public int? IdVendedorObra { get; set; }

        /// <summary>
        /// Idenficador da Comissao.
        /// </summary>
        [Colosoft.Web.QueryString("idComissao")]
        public int? IdComissao { get; set; }

        /// <summary>
        /// Ordenar?
        /// </summary>
        [Colosoft.Web.QueryString("ordenar")]
        public int? Ordenar { get; set; }

        /// <summary>
        /// Exibir a Receber.
        /// </summary>
        [Colosoft.Web.QueryString("ExibirAReceber")]
        public bool ExibirAReceber { get; set; }

        /// <summary>
        /// Exibir contas renegociadas
        /// </summary>
        [Colosoft.Web.QueryString("renegociadas")]
        public bool Renegociadas { get; set; }

        /// <summary>
        /// Exibir contas renegociadas
        /// </summary>
        [Colosoft.Web.QueryString("numCte")]
        public int? NumCte { get; set; }

        /// <summary>
        /// Exibir contas renegociadas
        /// </summary>
        [Colosoft.Web.QueryString("protestadas")]
        public bool Protestadas { get; set; }

        /// <summary>
        /// Exibir contas renegociadas
        /// </summary>
        [Colosoft.Web.QueryString("contasVinculadas")]
        public bool ContasVinculadas { get; set; }

        [Colosoft.Web.QueryString("receber")]
        public bool Receber { get; set; }

        /// <summary>
        /// Id da compra
        /// </summary>
        [Colosoft.Web.QueryString("idCompra")]
        public int? IdCompra { get; set; }

        /// <summary>
        /// Nf
        /// </summary>
        [Colosoft.Web.QueryString("nf")]
        public string Nf { get; set; }

        /// <summary>
        /// Id do CustoFixo
        /// </summary>
        [Colosoft.Web.QueryString("idCustoFixo")]
        public int? IdCustoFixo { get; set; }

        /// <summary>
        /// Id do ImpostoServ
        /// </summary>
        [Colosoft.Web.QueryString("idImpostoServ")]
        public int? IdImpostoServ { get; set; }

        /// <summary>
        /// Id do fornecedor
        /// </summary>
        [Colosoft.Web.QueryString("idFornec")]
        public int? IdFornec { get; set; }

        /// <summary>
        /// Nome do fornecedor
        /// </summary>
        [Colosoft.Web.QueryString("nomeFornec")]
        public string NomeFornec { get; set; }

        /// <summary>
        /// Número Cte
        /// </summary>
        [Colosoft.Web.QueryString("numeroCte")]
        public int? NumeroCte { get; set; }

        /// <summary>
        /// forma de pagamento
        /// </summary>
        [Colosoft.Web.QueryString("formaPagto")]
        public int? FormaPagto { get; set; }

        /// <summary>
        /// Data inicio
        /// </summary>
        [Colosoft.Web.QueryString("dataIniCad")]
        public DateTime? DataInicioCadastro { get; set; }

        /// <summary>
        /// Data fim
        /// </summary>
        [Colosoft.Web.QueryString("dataFimCad")]
        public DateTime? DataFimCadastro { get; set; }

        /// <summary>
        /// Data inicio
        /// </summary>
        [Colosoft.Web.QueryString("dtIniPago")]
        public DateTime? DataIniPago { get; set; }

        /// <summary>
        /// Data fim
        /// </summary>
        [Colosoft.Web.QueryString("dtFimPago")]
        public DateTime? DataFimPago { get; set; }

        /// <summary>
        /// Data de vencimento inicio
        /// </summary>
        [Colosoft.Web.QueryString("dtIniVenc")]
        public DateTime? DataIniVenc { get; set; }

        /// <summary>
        /// Data de vencimento fim
        /// </summary>
        [Colosoft.Web.QueryString("dtFimVenc")]
        public DateTime? DataFimVenc { get; set; }

        /// <summary>
        /// Tipo
        /// </summary>
        [Colosoft.Web.QueryString("tipo")]
        public int? Tipo { get; set; }

        /// <summary>
        /// Comissão
        /// </summary>
        [Colosoft.Web.QueryString("comissao")]
        public bool? Comissao { get; set; }

        /// <summary>
        /// Plano de conta
        /// </summary>
        [Colosoft.Web.QueryString("planoConta")]
        public string PlanoConta { get; set; }

        /// <summary>
        /// Custo fixo
        /// </summary>
        [Colosoft.Web.QueryString("custoFixo")]
        public bool? CustoFixo { get; set; }

        /// <summary>
        /// Agrupar
        /// </summary>
        [Colosoft.Web.QueryString("agrupar")]
        public bool? Agrupar { get; set; }

        /// <summary>
        /// Exibir Só Previsão CustoFixo
        /// </summary>
        [Colosoft.Web.QueryString("exibirAPagar")]
        public bool? ExibirAPagar { get; set; }
 
        /// <summary>
        /// Observação.
        /// </summary>
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

                var dominioFluxo = ServiceLocator.Current.GetInstance<Negocios.IDominioFluxo>();

                var arq = new Negocios.Entidades.Dominio.Arquivo();

                if (Receber)
                {
                    var Recebida = ExibirAReceber == true ? (bool?)null : true;

                    var trocador1 = 0;

                    var tiposConta = TipoConta.Split(',').Select(f => int.TryParse(f, out trocador1)).Select(f => trocador1);

                    arq = dominioFluxo.GerarArquivoRecebidas(IdPedido, IdLiberarPedido, IdAcerto, IdAcertoParcial,
                        IdTrocaDevolucao, NumeroNfe, IdLoja, IdFuncionario, IdFuncionarioRecebido, IdCliente, TipoEntrega, NomeCliente,
                        DataInicioVencimento, DataFimVencimento, DataInicioRecebimento, DataFimRecebimento, null, null, IdFormaPagto,
                        TipoBoleto, ValorInicial, ValorFinal, Renegociadas, Recebida, IdComissionado, IdRota, Obs, Ordenar, tiposConta,
                        NumArqRemessa, RefObra, ContasCnab, IdVendedorAssociado, IdVendedorObra, IdComissao, NumCte, Protestadas, ContasVinculadas);
                }
                else
                    arq = dominioFluxo.GerarArquivoPagas(IdContaPg, IdCompra, Nf, IdLoja, IdCustoFixo, IdImpostoServ, IdFornec, NomeFornec, FormaPagto, DataInicioCadastro, DataFimCadastro,
                        DataIniPago, DataFimPago, DataIniVenc, DataFimVenc, ValorInicial, ValorFinal, Tipo, Comissao == true, Renegociadas == true, PlanoConta, CustoFixo == true,
                        ExibirAPagar == true, IdComissao, NumeroCte, Observacao);

                if (arq == null)
                    throw new Exception("Nenhuma conta encontrada.");

                var data = DateTime.Now;
                var nomeArquivo = "DOMINIO_" + data.Day + "_" + data.Month + "_" + data.Year + "_" + data.Millisecond+".txt";

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
                alert('" + MensagemAlerta.FormatErrorMsg("", ex) + @"');
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
