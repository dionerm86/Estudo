using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(TitulosIFDDAO))]
    [PersistenceClass("titulos_ifd")]
    public class TitulosIFD
    {
        #region Enumeradores

        public enum OcorrenciaTitulos
        {
            SaldoAnterior,
            Entradas,
            Saidas,
            Vencidos,
            Vencer
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("Ocorrencia")]
        public long Ocorrencia { get; set; }

        [PersistenceProperty("IdConta")]
        public uint IdConta { get; set; }

        [PersistenceProperty("Valor")]
        public decimal Valor { get; set; }

        [PersistenceProperty("Receber")]
        public bool Receber { get; set; }

        #endregion

        #region Propriedades de Suporte

        public int Tipo
        {
            get 
            {
                switch ((OcorrenciaTitulos)Ocorrencia)
                {
                    case OcorrenciaTitulos.SaldoAnterior:
                    case OcorrenciaTitulos.Entradas:
                    case OcorrenciaTitulos.Saidas:
                        return 0;
                        
                    case OcorrenciaTitulos.Vencidos:
                    case OcorrenciaTitulos.Vencer:
                        return 1;

                    default: return -1;
                }
            }
        }

        public string DescrOcorrencia
        {
            get
            {
                switch (Ocorrencia)
                {
                    case (int)OcorrenciaTitulos.SaldoAnterior: return "Saldo Anterior *";
                    case (int)OcorrenciaTitulos.Entradas: return "Entradas";
                    case (int)OcorrenciaTitulos.Saidas: return "Saídas";
                    case (int)OcorrenciaTitulos.Vencidos: return "Vencidos";
                    case (int)OcorrenciaTitulos.Vencer: return "A Vencer";
                    default: return String.Empty;
                }
            }
        }

        public string DescrPagto
        {
            get 
            {
                return Receber ? UtilsPlanoConta.GetDescrFormaPagtoByIdConta(IdConta) :
                    IdConta > 0 ? CategoriaContaDAO.Instance.ObtemDescricao(IdConta) : String.Empty;
            }
        }

        #endregion
    }
}