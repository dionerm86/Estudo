using System;
using GDA;
using Glass.Data.DAL;
using Sync.Utils;
using Sync.Utils.Boleto.CodigoOcorrencia;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(RegistroArquivoRemessaDAO))]
    [PersistenceClass("registro_arquivo_remessa")]
    public class RegistroArquivoRemessa
    {
        #region Propiedades

        [PersistenceProperty("IDREGISTROARQUIVOREMSSA", PersistenceParameterType.IdentityKey)]
        public uint IdRegistroArquivoRemessa { get; set; }

        [PersistenceProperty("IDARQUIVOREMESSA")]
        public uint IdArquivoRemessa { get; set; }

        [PersistenceProperty("IDCONTAR")]
        public uint IdContaR { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint IdContaBanco { get; set; }

        [PersistenceProperty("DATAOCORRENCIA")]
        public DateTime DataOcorrencia { get; set; }

        [PersistenceProperty("CODOCORRENCIA")]
        public int CodOcorrencia { get; set; }

        [PersistenceProperty("NOSSONUMERO")]
        public string NossoNumero { get; set; }

        [PersistenceProperty("NUMERODOCUMENTO")]
        public string NumeroDocumento { get; set; }

        [PersistenceProperty("USOEMPRESA")]
        public string UsoEmpresa { get; set; }

        [PersistenceProperty("VALORRECEBIDO")]
        public decimal ValorRecebido { get; set; }

        [PersistenceProperty("JUROS")]
        public decimal Juros { get; set; }

        [PersistenceProperty("MULTA")]
        public decimal Multa { get; set; }

        [PersistenceProperty("PROTESTADO")]
        public bool Protestado { get; set; }

        #endregion

        #region Propiedades estendidas

        [PersistenceProperty("CodBanco", DirectionParameter.Input)]
        public int CodBanco { get; set; }

        #endregion

        #region Propiedades de Suporte

        public string CodOcorrenciaDescricao
        {
            get
            {
                string descricao = "";

                switch (CodBanco)
                {
                    case (int)CodigoBanco.Sicredi:
                        descricao = " (" + RepositorioEnumedores.GetEnumDescricao((CodOcorrenciaSicredi)CodOcorrencia) + ")";
                        break;
                    case (int)CodigoBanco.BancoBrasil:
                        descricao = " (" + RepositorioEnumedores.GetEnumDescricao((CodigoOcorrenciaBancoBrasil)CodOcorrencia) + ")";
                        break;
                    case (int)CodigoBanco.Bradesco:
                        descricao = " (" + RepositorioEnumedores.GetEnumDescricao((CodOcorrenciaBradesco)CodOcorrencia) + ")";
                        break;
                    default:
                        break;
                }

                return CodOcorrencia + descricao;
            }
        }

        #endregion
    }
}
