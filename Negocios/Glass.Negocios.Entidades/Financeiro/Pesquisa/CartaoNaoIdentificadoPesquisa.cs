using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// CLasse de pesquisa para recuperação de informações do cartão não identificado para exibição na listagem
    /// </summary>
    public class CartaoNaoIdentificadoPesquisa : ICartaoNaoIdentificado
    {
        /// <summary>
        /// Código do cartão não identificado
        /// </summary>
        public int IdCartaoNaoIdentificado { get; set; }

        /// <summary>
        /// Nome banco
        /// </summary>
        public string NomeBanco { get; set; }

        /// <summary>
        /// Agência bancária
        /// </summary>
        public string Agencia { get; set; }

        /// <summary>
        /// Conta bancária
        /// </summary>
        public string Conta { get; set; }

        /// <summary>
        /// Data de cadastro do Cartão não Identificado
        /// </summary>
        public DateTime DataCad { get; set; }

        /// <summary>
        /// Usuário que cadastrou o CNI
        /// </summary>
        public string FuncionarioCadastro { get; set; }

        /// <summary>
        /// Valor do CNI
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Tipo Cartão
        /// </summary>
        public int TipoCartao { get; set; }

        /// <summary>
        /// Situação do CNI
        /// </summary>
        public Data.Model.SituacaoCartaoNaoIdentificado Situacao { get; set; }

        /// <summary>
        /// Observação do CNI
        /// </summary>
        public string Observacao { get; set; }

        /// <summary>
        /// Id do acerto
        /// </summary>
        public int? IdAcerto { get; set; }

        /// <summary>
        /// Id da conta a receber
        /// </summary>
        public int? IdContaR { get; set; }

        /// <summary>
        /// Id da devolução do pagamento
        /// </summary>
        public int? IdDevolucaoPagto { get; set; }

        /// <summary>
        /// Id da liberação do pedido
        /// </summary>
        public int? IdLiberarPedido { get; set; }

        /// <summary>
        /// Id da obra
        /// </summary>
        public int? IdObra { get; set; }

        /// <summary>
        /// Id do pedido
        /// </summary>
        public int? IdPedido { get; set; }

        /// <summary>
        /// Id do sinal 
        /// </summary>
        public int? IdSinal { get; set; }

        /// <summary>
        /// Id da troca/Devolução
        /// </summary>
        public int? IdTrocaDevolucao { get; set; }

        /// <summary>
        /// Id do acerto do cheque
        /// </summary>
        public int? IdAcertoCheque { get; set; }

        /// <summary>
        /// Número de autorização de cartão
        /// </summary>
        public string NumAutCartao { get; set; }

        /// <summary>
        /// Data de recebimento do CNI
        /// </summary>
        public DateTime DataVenda { get; set; }

        /// <summary>
        /// Número estabelecimento
        /// </summary>
        public string NumeroEstabelecimento { get; set; }

        /// <summary>
        /// Últimos dígitos do cartão
        /// </summary>
        public string UltimosDigitosCartao { get; set; }

        /// <summary>
        /// O CNI foi importado?
        /// </summary>
        public bool Importado { get; set; }

        /// <summary>
        /// Número de parcelas geradas
        /// </summary>
        public int NumeroParcelas { get; set; }

        /// <summary>
        /// Id do arquivo que gerou o CNI
        /// </summary>
        public int? IdArquivoCartaoNaoIdentificado { get; set; }

        /// <summary>
        /// Referência
        /// </summary>
        public string Referencia
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator
                  .Current.GetInstance<IProvedorCartaoNaoIdentificado>().ObterReferencia(this);
            }
        }

        /// <summary>
        /// Indicador se o registro pode ser alterado
        /// </summary>
        public bool PodeEditar
        {
            get { return Situacao == Data.Model.SituacaoCartaoNaoIdentificado.Ativo && !Importado; }
        }

        /// <summary>
        /// Indicador se o registro pode ser cancelado
        /// </summary>
        public bool podeCancelar
        {
            get
            {
                return Situacao == Data.Model.SituacaoCartaoNaoIdentificado.Ativo &&
                    Data.Helper.Config.PossuiPermissao(Data.Helper.Config.FuncaoMenuCadastro.PermitirCancelarCni);
            }
        }

        /// <summary>
        /// Conta bancária
        /// </summary>
        public string ContaBancaria
        {
            get
            {
                return NomeBanco + " Agência: " + Agencia + " Conta: " + Conta;
            }
        }

        /// <summary>
        /// Descrição da situação
        /// </summary>
        public string DescrStuacao
        {
            get { return Situacao.ToString(); }
        }

        /// <summary>
        /// Descrição do tipo do cartão
        /// </summary>
        public string TipoCartaoStr
        {
            get { return DescOperadora + " " + DescBandeira + " " + Tipo.Translate().Format(); }
        }

        /// <summary>
        /// Operadora do cartão
        /// </summary>
        public uint Operadora { get; set; }

        /// <summary>
        /// Bandeira do cartão
        /// </summary>
        public uint Bandeira { get; set; }

        /// <summary>
        /// Tipo do cartão
        /// </summary>
        public Glass.Data.Model.TipoCartaoEnum Tipo { get; set; }

        /// <summary>
        /// Descrição da Bandeira
        /// </summary>
        public string DescBandeira { get; set; }

        /// <summary>
        /// Descrição da Operadora
        /// </summary>
        public string DescOperadora { get; set; }
    }
}
