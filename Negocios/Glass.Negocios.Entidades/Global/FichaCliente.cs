using System;

namespace Glass.Global.Negocios.Entidades
{
    public class FichaCliente
    {
        public int IdCli { get; set; }

        public string Nome { get; set; }

        public string NomeFantasia { get; set; }

        public string Email { get; set; }

        public Data.Model.TipoPessoa TipoPessoa { get; set; }

        public bool ProdutorRural { get; set; }

        public DateTime? DataNasc { get; set; }

        public string CpfCnpj { get; set; }

        public string Suframa { get; set; }

        public string RgInscEst { get; set; }

        public string Fax { get; set; }

        public string TelRes { get; set; }

        public string TelCel { get; set; }

        public string Contato { get; set; }

        public string TelCont { get; set; }

        public string Endereco { get; set; }

        public string Numero { get; set; }

        public string Compl { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Uf { get; set; }

        public string Cep { get; set; }

        public string EnderecoCobranca { get; set; }

        public string NumeroCobranca { get; set; }

        public string ComplCobranca { get; set; }

        public string BairroCobranca { get; set; }

        public string CidadeCobranca { get; set; }

        public string UfCobranca { get; set; }

        public string CepCobranca { get; set; }

        public decimal Limite { get; set; }

        public decimal ValorMediaIni { get; set; }

        public decimal ValorMediaFim { get; set; }

        public decimal Credito { get; set; }

        public float PercSinalMinimo { get; set; }

        public bool Revenda { get; set; }

        public string FormasPagamento { get; set; }
        
        public string Parcelas { get; set; }

        public string Parcela { get; set; }

        public bool PagamentoAntesProducao { get; set; }

        public string TabelaDescontoAcrescimo { get; set; }

        public bool CobrarIcmsSt { get; set; }

        public bool CobrarIpi { get; set; }

        public string Loja { get; set; }

        public string NomeFunc { get; set; }

        public string NomeComissionado { get; set; }

        public float PercComissaoFunc { get; set; }

        public string Rota { get; set; }

        public string Login { get; set; }

        public Data.Model.SituacaoCliente Situacao { get; set; }

        public string TipoCliente { get; set; }

        public bool BloquearPedidoContaVencida { get; set; }

        public bool IgnorarBloqueioPedPronto { get; set; }

        public string Obs { get; set; }

        public string Contato1 { get; set; }

        public string CelContato1 { get; set; }

        public string EmailContato1 { get; set; }

        public string RamalContato1 { get; set; }

        public string Contato2 { get; set; }

        public string CelContato2 { get; set; }

        public string EmailContato2 { get; set; }

        public string RamalContato2 { get; set; }

        public string Contato3 { get; set; }

        public string CelContato3 { get; set; }

        public string EmailContato3 { get; set; }

        public string RamalContato3 { get; set; }

        public string Historico { get; set; }

        public int IdLoja { get; set; }

        public bool CalcularIcmsPedido { get; set; }

        public bool CalcularIpiPedido { get; set; }

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
    }
}
