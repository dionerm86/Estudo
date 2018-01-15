using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MoldeDAO))]
    [PersistenceClass("molde")]
    public class Molde : ModelBaseCadastro
    {
        #region Construtores

        public Molde()
        {
            Situacao = (int)SituacaoEnum.Aberto;
        }

        #endregion

        #region Enumeradores

        public enum SituacaoEnum
        {
            Aberto = 1,
            Finalizado
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDMOLDE", PersistenceParameterType.IdentityKey)]
        public uint IdMolde { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("CORTAR")]
        public bool Cortar { get; set; }

        [PersistenceProperty("BISOTAR")]
        public bool Bisotar { get; set; }

        [PersistenceProperty("TEMPERAR")]
        public bool Temperar { get; set; }

        [PersistenceProperty("JATEAR")]
        public bool Jatear { get; set; }

        [PersistenceProperty("LAQUEAR")]
        public bool Laquear { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("DATAALT")]
        public DateTime? DataAlt { get; set; }

        [PersistenceProperty("USUALT")]
        public int? UsuAlt { get; set; }

        [PersistenceProperty("PRAZOENTREGA")]
        public string PrazoEntrega { get; set; }

        [PersistenceProperty("ESPECIFICACAOMDF")]
        public string EspecificacaoMdf { get; set; }

        [PersistenceProperty("ESPECIFICACAOVIDRO")]
        public string EspecificacaoVidro { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("TELEFONECLIENTE", DirectionParameter.InputOptional)]
        public string TelefoneCliente { get; set; }

        [PersistenceProperty("ENDERECOOBRA", DirectionParameter.InputOptional)]
        public string EnderecoObra { get; set; }

        [PersistenceProperty("BAIRROOBRA", DirectionParameter.InputOptional)]
        public string BairroObra { get; set; }

        [PersistenceProperty("CIDADEOBRA", DirectionParameter.InputOptional)]
        public string CidadeObra { get; set; }

        [PersistenceProperty("TELEFONEOBRA", DirectionParameter.InputOptional)]
        public string TelefoneObra { get; set; }

        [PersistenceProperty("CELULAROBRA", DirectionParameter.InputOptional)]
        public string CelularObra { get; set; }

        [PersistenceProperty("DATAENTREGAPEDIDO", DirectionParameter.InputOptional)]
        public DateTime? DataEntregaPedido { get; set; }

        [PersistenceProperty("FUNCVEND", DirectionParameter.InputOptional)]
        public string FuncVend { get; set; }

        [PersistenceProperty("FUNCCAD", DirectionParameter.InputOptional)]
        public string FuncCad { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string EnderecoComplObra
        {
            get
            {
                return EnderecoObra + (!String.IsNullOrEmpty(BairroObra) ? " - " + BairroObra : "") +
                    (!String.IsNullOrEmpty(CidadeObra) ? " - " + CidadeObra : "");
            }
        }

        public string DescrTipoServico
        {
            get
            {
                List<string> servicos = new List<string>();
                if (Cortar) servicos.Add("Cortar");
                if (Bisotar) servicos.Add("Bisotar");
                if (Temperar) servicos.Add("Temperar");
                if (Jatear) servicos.Add("Jatear");
                if (Laquear) servicos.Add("Laquear");

                return String.Join(", ", servicos.ToArray());
            }
        }

        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case (int)SituacaoEnum.Aberto: return "Aberto";
                    case (int)SituacaoEnum.Finalizado: return "Finalizado";
                    default: return "";
                }
            }
        }

        #endregion
    }
}