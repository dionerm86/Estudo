using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ArquivoOtimizacaoDAO))]
    [PersistenceClass("arquivo_otimizacao")]
    public class ArquivoOtimizacao
    {
        #region Enumeradores

        public enum DirecaoEnum
        {
            Exportar = 1,
            Importar
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDARQUIVOOTIMIZ", PersistenceParameterType.IdentityKey)]
        public uint IdArquivoOtimizacao { get; set; }

        [PersistenceProperty("DIRECAO")]
        public int Direcao { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("EXTENSAOARQUIVO")]
        public string ExtensaoArquivo { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrDirecao
        {
            get
            {
                switch (Direcao)
                {
                    case (int)DirecaoEnum.Exportar: return "Exportação";
                    case (int)DirecaoEnum.Importar: return "Importação";
                    default: return "";
                }
            }
        }

        public string NomeArquivo
        {
            get
            {
                return (Direcao == (int)DirecaoEnum.Exportar ? "Exp_" + IdArquivoOtimizacao.ToString().PadLeft(10, '0') :
                    DataCad.ToString("dd-MM-yyyy hh_mm")) + ExtensaoArquivo;
            }
        }

        public string CaminhoArquivo
        {
            get { return Utils.GetArquivoOtimizacaoVirtualPath + NomeArquivo; }
        }

        #endregion
    }
}