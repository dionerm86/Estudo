using System;
using GDA;
using Glass.Data.RelDAL;
using Glass.Data.DAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(RecebimentoDAO))]
    [PersistenceClass("recebimento")]
    public class Recebimento
    {
        #region Propriedades

        [PersistenceProperty("GRUPO")]
        public long Grupo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion

        #region PropriedadesExtendidas

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("IdAcerto", DirectionParameter.InputOptional)]
        public uint? IdAcerto { get; set; }

        [PersistenceProperty("IdAcertoCheque", DirectionParameter.InputOptional)]
        public int? IdAcertoCheque { get; set; }

        [PersistenceProperty("IdAntecipContaRec", DirectionParameter.InputOptional)]        
        public int? IdAntecipContaRec { get; set; }

        [PersistenceProperty("IdCheque", DirectionParameter.InputOptional)]        
        public uint? IdCheque { get; set; }

        [PersistenceProperty("IdPedido", DirectionParameter.InputOptional)]        
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IdLiberarPedido", DirectionParameter.InputOptional)]        
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("IdTrocaDevolucao", DirectionParameter.InputOptional)]        
        public uint? IdTrocaDevolucao { get; set; }

        [PersistenceProperty("IdPagto", DirectionParameter.InputOptional)]        
        public int? IdPagto { get; set; }

        [PersistenceProperty("IdDeposito", DirectionParameter.InputOptional)]        
        public int? IdDeposito { get; set; }

        [PersistenceProperty("IdObra", DirectionParameter.InputOptional)]        
        public uint? IdObra { get; set; }        

        [PersistenceProperty("IdDevolucaoPagto", DirectionParameter.InputOptional)]        
        public int? IdDevolucaoPagto { get; set; }

        [PersistenceProperty("IdSinal", DirectionParameter.InputOptional)]
        public uint? IdSinal { get; set; }

        [PersistenceProperty("IdSinalCompra", DirectionParameter.InputOptional)]
        public int? IdSinalCompra { get; set; }

        [PersistenceProperty("IdCreditoFornecedor", DirectionParameter.InputOptional)]
        public int? IdCreditoFornecedor { get; set; }

        [PersistenceProperty("IdDepositoNaoIdentificado", DirectionParameter.InputOptional)]
        public int? IdDepositoNaoIdentificado { get; set; }

        [PersistenceProperty("IdAntecipFornec", DirectionParameter.InputOptional)]
        public int? IdAntecipFornec { get; set; }

        [PersistenceProperty("IdCompra", DirectionParameter.InputOptional)]
        public int? IdCompra { get; set; }

        [PersistenceProperty("IdContaR", DirectionParameter.InputOptional)]
        public uint? IdContaR { get; set; }

        [PersistenceProperty("DescricaoPlanoConta", DirectionParameter.InputOptional)]
        public string DescricaoPlanoConta { get; set; }

        [PersistenceProperty("DataMovimentacao", DirectionParameter.InputOptional)]
        public DateTime DataMovimentacao { get; set; }
        //[PersistenceProperty("DESCRICAOCONTARECEBERCONTABIL", DirectionParameter.InputOptional)]
        //public string DescricaoContaReceberContabil { get; set; }

        #endregion

        #region Propriedades de Suporte

        private bool _isTotal = false;

        public bool IsTotal
        {
            get { return _isTotal; }
            set { _isTotal = value; }
        }

        public string DescrGrupo
        {
            get
            {
                switch (Grupo)
                {
                    case 1: return "Valor Vendido";
                    case 2: return "Valor Recebido";
                    case 3: return "Valor a Receber";
                    default: return String.Empty;
                }
            }
        }

        public string DescricaoGrafico
        {
            get { return Formatacoes.TrataStringDocFiscal(Descricao); }
        }

        public string DescricaoGrafico1
        {
            get { return Descricao + ", " + String.Format("{0:C}", Valor); }
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

                if (IdDeposito > 0)
                    refer += "Depósito: " + IdDeposito + " ";

                if (IdCheque > 0)
                    refer += "Cheque: " + ChequesDAO.Instance.ObtemNumCheque(IdCheque.Value) + " ";

                if (IdCompra > 0)
                    refer += "Compra: " + IdCompra + " ";                            

                if (IdPedido > 0)
                    refer += "Pedido: " + IdPedido + " ";

                if (IdLiberarPedido > 0)
                    refer += "Liberação: " + IdLiberarPedido + " ";

                if (IdPagto > 0)
                    refer += "Pagto: " + IdPagto + " ";

                if (IdObra > 0)
                    refer += "Obra: " + IdObra + " ";

                if (IdAntecipFornec > 0)
                    refer += "Antecipação de fornecedor: " + IdAntecipFornec + " ";

                if (IdTrocaDevolucao > 0)
                    refer += "Troca/Devolução: " + IdTrocaDevolucao + " ";

                if (IdDevolucaoPagto > 0)
                    refer += "Devolução de pagto.: " + IdDevolucaoPagto + " ";

                if (IdSinal > 0)
                {
                    refer += SinalDAO.Instance.GetReferencia(IdSinal.Value) + " ";
                    refer += "Pedido(s): " + SinalDAO.Instance.ObtemIdsPedidos(IdSinal.Value) + " ";
                }

                if (IdSinalCompra > 0)
                    refer += "Sinal da Compra: " + IdSinalCompra + " ";

                if (IdCreditoFornecedor > 0)
                    refer += "Créd. Fornecedor: " + IdCreditoFornecedor + " ";

                if (IdAntecipContaRec > 0)
                    refer += "Antecipação: " + IdAntecipContaRec + " ";                                    

                if (IdDepositoNaoIdentificado > 0)
                {
                    refer += "Depósito Não Identificado: " + IdDepositoNaoIdentificado + "  ";
                    var dep = DepositoNaoIdentificadoDAO.Instance.GetElementByPrimaryKey((uint)IdDepositoNaoIdentificado.Value);
                    if (dep != null)
                        refer += dep.Referencia + " ";
                }

                //if (IdContaR > 0)
                //    refer += ContasReceberDAO.Instance.GetReferencia(IdContaR.Value) +
                //        (String.IsNullOrEmpty(DescricaoContaReceberContabil) ? "" :
                //        String.Format(" ({0})", DescricaoContaReceberContabil)) + " ";

                return refer;
            }
        }

        public decimal TotalRecebido { get; set; }
        
        public decimal TotalEstornado { get; set; }

        public decimal Total
        {
            get
            {
                return TotalRecebido - TotalEstornado;
            }
        }

        #endregion
    }

    public class RecebimentoImagem
    {
        public byte[] Buffer { get; set; }
    }
}