using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DepositoNaoIdentificadoDAO))]
    [PersistenceClass("deposito_nao_identificado")]
    public class DepositoNaoIdentificado
    {
        #region Enumeradores

        public enum SituacaoEnum
        {
            Ativo = 1,
            Cancelado,
            EmUso
        }

        #endregion

        #region Propriedades

        [Log("Depósito não Identificado")]
        [PersistenceProperty("IDDEPOSITONAOIDENTIFICADO", PersistenceParameterType.IdentityKey)]
        public uint IdDepositoNaoIdentificado { get; set; }

        [Log("Conta Bancária")]
        [PersistenceProperty("IDCONTABANCO")]
        public uint IdContaBanco { get; set; }

        [Log("Valor Mov.")]
        [PersistenceProperty("VALORMOV")]
        public decimal ValorMov { get; set; }

        [Log("Data Mov.")]
        [PersistenceProperty("DATAMOV")]
        public DateTime DataMov { get; set; }

        [Log("Obs")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("IDACERTO")]
        public uint? IdAcerto { get; set; }

        [PersistenceProperty("IDCONTAR")]
        public uint? IdContaR { get; set; }

        [PersistenceProperty("IDOBRA")]
        public uint? IdObra { get; set; }

        [PersistenceProperty("IDSINAL")]
        public uint? IdSinal { get; set; }

        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public uint? IdTrocaDevolucao { get; set; }

        [PersistenceProperty("IDDEVOLUCAOPAGTO")]
        public uint? IdDevolucaoPagto { get; set; }

        [PersistenceProperty("IDACERTOCHEQUE")]
        public uint? IdAcertoCheque { get; set; }

        [PersistenceProperty("SITUACAO")]
        public SituacaoEnum Situacao { get; set; }

        [PersistenceProperty("IDFUNCCAD", DirectionParameter.OutputOnlyInsert)]
        public uint IdFuncCad { get; set; }

        [Log("Data de Cadastro")]
        [PersistenceProperty("DATACAD", DirectionParameter.OutputOnlyInsert)]
        public DateTime DataCad { get; set; }

        #endregion

        #region Propriedades extendidas

        [Log("Conta bancária")]
        [PersistenceProperty("DESCRCONTABANCO", DirectionParameter.InputOptional)]
        public string DescrContaBanco { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de suporte

        public string Descricao
        {
            get
            {
                return String.Format("{0} - {1}, Valor: {2}, Data: {3}", 
                    IdDepositoNaoIdentificado, 
                    DescrContaBanco, 
                    ValorMov.ToString("C"), 
                    DataMov);
            }
        }

        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case SituacaoEnum.EmUso: return "Em uso";
                    default: return Situacao.ToString();
                }
            }
        }

        [Log("Referência")]
        public string Referencia
        {
            get
            {
                string referencia = "";

                if (IdAcerto > 0)
                    referencia += ", Acerto: " + IdAcerto;

                if (IdContaR > 0)
                    referencia += ", " + ContasReceberDAO.Instance.GetReferencia(IdContaR.Value);

                if (IdDevolucaoPagto > 0)
                    referencia += ", Devolução pagto.: " + IdDevolucaoPagto;

                if (IdLiberarPedido > 0)
                    referencia += ", Liberação pedido: " + IdLiberarPedido;

                if (IdObra > 0)
                    referencia += ", Obra: " + IdObra;

                if (IdPedido > 0)
                    referencia += ", Pedido: " + IdPedido;

                if (IdSinal > 0)
                    referencia += ", " + SinalDAO.Instance.GetReferencia(IdSinal.Value);

                if (IdTrocaDevolucao > 0)
                    referencia += ", " + (TrocaDevolucaoDAO.Instance.IsTroca(IdTrocaDevolucao.Value) ? 
                        "Troca: " : "Devolução: ") + IdTrocaDevolucao;
                if (IdAcertoCheque > 0)
                    referencia += ", Acerto Cheque: " + IdAcertoCheque;

                return referencia.TrimStart(',', ' ');
            }
        }

        [Log("Data Mov.")]
        public string DataMovString
        {
            get { return Conversoes.ConverteData(DataMov, true); }
            set { DataMov = Conversoes.ConverteData(value).Value; }
        }

        public bool EditarVisible { get { return Situacao == SituacaoEnum.Ativo; } }

        public bool CancelarVisible { get { return Situacao == SituacaoEnum.Ativo; } }

        public string ValorMovString
        {
            get { return ValorMov.ToString(); }
            set { ValorMov = Conversoes.StrParaDecimal(value); }
        }

        #endregion
    }
}
