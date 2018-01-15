using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MovBancoDAO))]
	[PersistenceClass("mov_banco")]
	public class MovBanco : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDMOVBANCO", PersistenceParameterType.IdentityKey)]
        public uint IdMovBanco { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint? IdCliente { get; set; }

        [Log("Fornecedor", "Nome", typeof(FornecedorDAO))]
        [PersistenceProperty("IDFORNEC")]
        public uint? IdFornecedor { get; set; }

        [PersistenceProperty("IDLOJA")]
        public int IdLoja { get; set; }

        [Log("Conta bancária", "Nome", typeof(ContaBancoDAO))]
        [PersistenceProperty("IDCONTABANCO")]
        public uint IdContaBanco { get; set; }

        [Log("Sinal")]
        [PersistenceProperty("IDSINAL")]
        public uint? IdSinal { get; set; }

        [Log("Sinal da Compra")]
        [PersistenceProperty("IDSINALCOMPRA")]
        public uint? IdSinalCompra { get; set; }

        [Log("Conta bancária Dest.", "Nome", typeof(ContaBancoDAO))]
        [PersistenceProperty("IDCONTABANCODEST")]
        public uint? IdContaBancoDest { get; set; }

        [Log("Plano conta", "Descricao", typeof(PlanoContasDAO))]
        [PersistenceProperty("IDCONTA")]
        public uint IdConta { get; set; }

        [Log("Conta a receber")]
        [PersistenceProperty("IDCONTAR")]
        public uint? IdContaR { get; set; }

        [Log("Pedido")]
        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [Log("Acerto")]
        [PersistenceProperty("IDACERTO")]
        public uint? IdAcerto { get; set; }

        [Log("Depósito de cheque")]
        [PersistenceProperty("IDDEPOSITO")]
        public uint? IdDeposito { get; set; }

        [Log("Pagamento")]
        [PersistenceProperty("IDPAGTO")]
        public uint? IdPagto { get; set; }

        [Log("Conta a pagar")]
        [PersistenceProperty("IDCONTAPG")]
        public uint? IdContaPg { get; set; }

        /// <summary>
        /// Campo utilizado para identificar qual cheque devolvido esta sendo pago com esta movimentação
        /// </summary>
        [Log("Cheque")]
        [PersistenceProperty("IDCHEQUE")]
        public uint? IdCheque { get; set; }

        [Log("Liberação de pedido")]
        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [Log("Obra")]
        [PersistenceProperty("IDOBRA")]
        public uint? IdObra { get; set; }

        [Log("Antecip. Pagto. Fornecedor")]
        [PersistenceProperty("IDANTECIPFORNEC")]
        public uint? IdAntecipFornec { get; set; }

        [Log("Acerto de cheque")]
        [PersistenceProperty("IDACERTOCHEQUE")]
        public uint? IdAcertoCheque { get; set; }

        [Log("Antecipação de boleto")]
        [PersistenceProperty("IDANTECIPCONTAREC")]
        public uint? IdAntecipContaRec { get; set; }

        [Log("Troca/devolução")]
        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public uint? IdTrocaDevolucao { get; set; }

        [Log("Devolução de pagamento")]
        [PersistenceProperty("IDDEVOLUCAOPAGTO")]
        public uint? IdDevolucaoPagto { get; set; }

        [Log("Deposito não identificado")]
        [PersistenceProperty("IDDEPOSITONAOIDENTIFICADO")]
        public uint? IdDepositoNaoIdentificado { get; set; }        

        [Log("Deposito não identificado")]
        [PersistenceProperty("IdArquivoRemessa")]
        public uint? IdArquivoRemessa { get; set; }

        [Log("Valor")]
        [PersistenceProperty("VALORMOV")]
        public decimal ValorMov { get; set; }

        [Log("Juros")]
        [PersistenceProperty("JUROS")]
        public decimal Juros { get; set; }

        [Log("Data")]
        [PersistenceProperty("DATAMOV")]
        public DateTime DataMov { get; set; }

        [Log("Data Original")]
        [PersistenceProperty("DATAORIGINAL")]
        public DateTime? DataOriginal { get; set; }

        /// <summary>
        /// 1-Entrada
        /// 2-Saída
        /// </summary>
        [PersistenceProperty("TIPOMOV")]
        public int TipoMov { get; set; }

        [PersistenceProperty("SALDO")]
        public decimal Saldo { get; set; }

        [Log("Lançamento manual")]
        [PersistenceProperty("LANCMANUAL")]
        public bool LancManual { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Crédito de Fornecedor")]
        [PersistenceProperty("IDCREDITOFORNECEDOR")]
        public uint? IdCreditoFornecedor { get; set; }

        [PersistenceProperty("IDCARTAONAOIDENTIFICADO")]
        public uint? IdCartaoNaoIdentificado { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRCONTABANCO", DirectionParameter.InputOptional)]
        public string DescrContaBanco { get; set; }

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("DescrPlanoConta", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("NomeFornec", DirectionParameter.InputOptional)]
        public string NomeFornecedor { get; set; }

        [PersistenceProperty("DataUltimaConciliacao", DirectionParameter.InputOptional)]
        public DateTime? DataUltimaConciliacao { get; set; }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Usado para esconder este valor caso seja SALDO ANTERIOR
        /// </summary>
        public float? CodMov
        {
            get { return IdMovBanco > 0 ? (float?)IdMovBanco : null; }
        }

        /// <summary>
        /// Usado para esconder este valor caso seja SALDO ANTERIOR
        /// </summary>
        public decimal? ValorString
        {
            get { return ValorMov > 0 ? (decimal?)ValorMov : null; }
        }

        /// <summary>
        /// Usado para esconder este valor caso seja SALDO ANTERIOR
        /// </summary>
        public DateTime? DataMovString
        {
            get 
            { 
                return DataMov.Year > 1 ? (DateTime?)DataMov : null; 
            }
        }

        public DateTime? DataCadString
        {
            get { return base.DataCad.Year > 1 ? (DateTime?)base.DataCad : null; }
        }

        [Log("Tipo de movimentação")]
        public string DescrTipoMov
        {
            get { return TipoMov == 1 ? "C" : TipoMov == 2 ? "D" : String.Empty; }
        }

        public string Referencia
        {
            get
            {
                string refer = String.Empty;

                if (IdAcerto > 0)
                    refer += "Acerto: " + IdAcerto + " ";

                if (IdAcertoCheque > 0)
                    refer += "Acerto Cheque: " + IdAcertoCheque + " ";

                if (IdAntecipContaRec > 0)
                    refer += "Antecipação: " + IdAntecipContaRec + " ";

                if (IdCheque > 0)
                    refer += "Cheque: " + ChequesDAO.Instance.ObtemNumCheque(IdCheque.Value) + " ";

                if (IdPedido > 0)
                    refer += "Pedido: " + IdPedido + " ";

                if (IdLiberarPedido > 0)
                    refer += "Liberação: " + IdLiberarPedido + " ";

                if (IdTrocaDevolucao > 0)
                    refer += "Troca/Devolução: " + IdTrocaDevolucao + " ";

                if (IdPagto > 0)
                    refer += "Pagto: " + IdPagto + " ";

                if (IdDeposito > 0)
                    refer += "Depósito: " + IdDeposito + " ";

                if (IdObra > 0)
                    refer += "Obra: " + IdObra + " ";

                if (IdTrocaDevolucao > 0)
                    refer += "Troca/Devolução: " + IdTrocaDevolucao + " ";

                if (IdDevolucaoPagto > 0)
                    refer += "Devolução de pagto.: " + IdDevolucaoPagto + " ";

                if (IdSinal > 0)
                    refer += SinalDAO.Instance.GetReferencia(IdSinal.Value) + " ";

                if (IdSinalCompra > 0)
                    refer += "Sinal da Compra: " + IdSinalCompra + " ";

                if (IdCreditoFornecedor > 0)
                    refer += "Créd. Fornecedor: " + IdCreditoFornecedor + " ";

                if (IdDepositoNaoIdentificado > 0)
                {
                    refer += "Depósito Não Identificado: " + IdDepositoNaoIdentificado + "  ";
                    var dep = DepositoNaoIdentificadoDAO.Instance.GetElementByPrimaryKey(IdDepositoNaoIdentificado.Value);
                    if (dep != null)
                        refer += dep.Referencia + " ";
                }           
                
                if(IdCartaoNaoIdentificado > 0)
                    refer += "Cartão não Identificado: " + IdCartaoNaoIdentificado + " ";

                if (string.IsNullOrEmpty(refer) && IdContaR > 0)
                    refer = ContasReceberDAO.Instance.GetReferencia(IdContaR.Value);

                if (DataOriginal != null && DataOriginal.Value.ToShortDateString() != DataMov.ToShortDateString())
                    refer += "Data Original: " + DataOriginal.Value.ToString("dd/MM/yyyy");

                return refer;
            }
        }

        public string NomeClienteFornecedor
        {
            get
            {
                string nomeCliente = !String.IsNullOrEmpty(NomeCliente) ? IdCliente + " - " + NomeCliente : "";
                string nomeFornecedor = !String.IsNullOrEmpty(NomeFornecedor) ? IdFornecedor + " - " + NomeFornecedor : "";
                string separador = !String.IsNullOrEmpty(NomeCliente) && !String.IsNullOrEmpty(NomeFornecedor) ? " / " : "";

                return nomeCliente + separador + nomeFornecedor;
            }
        }

        public bool EditVisible
        {
            get { return IdMovBanco > 0; }
        }

        public bool UpDownVisible
        {
            get { return IdMovBanco > 0; }
        }

        public bool ExcluirVisible
        {
            get
            {
                return IdMovBanco > 0 &&
                    IdConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCxGeralParaContaBancariaDinheiro) &&
                    IdConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfContaBancariaParaCxGeralDinheiro) &&
                    ((IdAcerto == null && IdAcertoCheque == null && IdAntecipContaRec == null && IdSinal == null &&
                    IdCheque == null && IdContaBancoDest == null && IdContaPg == null && IdContaR == null && IdDeposito == null &&
                    IdLiberarPedido == null && IdObra == null && IdPagto == null && IdPedido == null && IdDepositoNaoIdentificado == null &&
                    IdCartaoNaoIdentificado == null && IdArquivoRemessa == null) || IdContaBancoDest != null);
            }
        }

        #endregion
    }
}