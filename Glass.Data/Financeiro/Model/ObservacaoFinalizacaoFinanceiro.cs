using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ObservacaoFinalizacaoFinanceiroDAO))]
    [PersistenceClass("observacao_finalizacao_financeiro")]
    public class ObservacaoFinalizacaoFinanceiro
    {
        #region Enumerações

        public enum MotivoEnum
        {
            Aberto,
            Finalizacao,
            NegacaoFinalizar,
            Confirmacao,
            NegacaoConfirmar
        }

        public enum TipoObs
        {
            Finalizacao = 1,
            Confirmacao
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDOBSFINANC", PersistenceParameterType.IdentityKey)]
        public uint IdObsFinanc { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("OBSERVACAO")]
        public string Observacao { get; set; }

        [PersistenceProperty("NUMSEQ")]
        public int NumSeq { get; set; }

        [PersistenceProperty("MOTIVO")]
        public MotivoEnum Motivo { get; set; }

        [PersistenceProperty("IDFUNCCAD")]
        public uint IdFuncCad { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("MOTIVOERROFINALIZARFINANC")]
        public string MotivoErroFinalizarFinanc { get; set; }

        [PersistenceProperty("MOTIVOERROCONFIRMARFINANC")]
        public string MotivoErroConfirmarFinanc { get; set; }

        #endregion

        #region Propriedades extendidas

        [PersistenceProperty("NOMEFUNCCAD", DirectionParameter.InputOptional)]
        public string NomeFuncCad { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de suporte

        public string DescrMotivo
        {
            get
            {
                switch (Motivo)
                {
                    case MotivoEnum.Aberto:
                        return "Aberto";

                    case MotivoEnum.Finalizacao:
                        return "Finalização";

                    case MotivoEnum.NegacaoFinalizar:
                        return "Negação ao Finalizar";

                    case MotivoEnum.Confirmacao:
                        return "Confirmação";

                    case MotivoEnum.NegacaoConfirmar:
                        return "Negação ao Confirmação";

                    default:
                        return String.Empty;
                }
            }
        }

        public string MotivoFinanceiro
        {
            get
            {
                return !string.IsNullOrEmpty(MotivoErroFinalizarFinanc) ? MotivoErroFinalizarFinanc :
                    !string.IsNullOrEmpty(MotivoErroConfirmarFinanc) ? MotivoErroConfirmarFinanc : null;
            }
        }

        #endregion
    }
}
