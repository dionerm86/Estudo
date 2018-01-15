using System;
using System.Collections.Generic;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa do cliente.
    /// </summary>
    public class ClientePesquisa
    {
        #region Propriedades

        public int IdCli { get; set; }

        public string Nome { get; set; }

        public string IdNome
        {
            get { return IdCli + " - " + NomeExibir.ToUpper(); }
        }

        public string NomeExibir
        {
            get
            {
                return Configuracoes.Liberacao.RelatorioLiberacaoPedido.TipoNomeExibirRelatorioPedido ==
                 Data.Helper.DataSources.TipoNomeExibirRelatorioPedido.NomeFantasia ?
                 NomeFantasia ?? Nome :
                 Nome ?? NomeFantasia;
            }
        }

        public string NomeFantasia { get; set; }

        public string CpfCnpj { get; set; }

        public string Endereco { get; set; }

        public IList<Financeiro.Negocios.Entidades.FormaPagtoCliente> FormasPagto { get; set; }

        public string Numero { get; set; }

        public string Compl { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Uf { get; set; }

        public string EnderecoCompleto
        {
            get
            {
                string compl = String.IsNullOrEmpty(Compl) ? " " : " (" + Compl + ") ";
                return Endereco + (!String.IsNullOrEmpty(Numero) ? ", " + Numero : String.Empty) + compl + Bairro + " - " + Cidade + "/" + Uf;
            }
        }

        public string EnderecoEntrega { get; set; }

        public string NumeroEntrega { get; set; }

        public string ComplEntrega { get; set; }

        public string BairroEntrega { get; set; }

        public string TelCont { get; set; }

        public string TelRes { get; set; }

        public string TelCel { get; set; }

        public string Telefone
        {
            get
            {
                if (!String.IsNullOrEmpty(TelCont))
                    return TelCont;
                else if (!String.IsNullOrEmpty(TelRes))
                    return TelRes;
                else if (!String.IsNullOrEmpty(TelCel))
                    return TelCel;
                else
                    return String.Empty;
            }
        }

        public Data.Model.SituacaoCliente Situacao { get; set; }

        public string Email { get; set; }

        public DateTime? DtUltCompra { get; set; }

        public decimal TotalComprado { get; set; }

        public string FormaPagamento { get; set; }

        public string Parcela { get; set; }

        public DateTime DataCad { get; set; }

        public string DescrUsuCad { get; set; }

        public DateTime? DataAlt { get; set; }

        public string DescrUsuAlt { get; set; }

        public bool Revenda { get; set; }

        public int? IdTabelaDesconto { get; set; }

        public string Historico { get; set; }

        public int? IdFunc { get; set; }

        public string NomeFunc { get; set; }
 
        public decimal Limite { get; set; }

        public decimal UsoLimite { get; set; }

        public string CidadeUf
        {
            get
            {
                return Cidade + "/" + Uf;
            }
        }

        public int IdFuncAtendente { get; set; }

        public string NomeAtendente { get; set; }

        #endregion
    }
}