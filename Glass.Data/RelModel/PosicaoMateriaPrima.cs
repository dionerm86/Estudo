using System.Collections.Generic;
using GDA;
using Glass.Data.RelDAL;
using System.Linq;
using Glass.Data.DAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(PosicaoMateriaPrimaDAO))]
    [PersistenceClass("posicao_materia_prima")]
    public class PosicaoMateriaPrima
    {
        #region Propriedades

        [PersistenceProperty("ESPESSURA")]
        public float Espessura { get; set; }

        [PersistenceProperty("IDCORVIDRO")]
        public uint IdCorVidro { get; set; }

        [PersistenceProperty("DESCRCORVIDRO")]
        public string DescrCorVidro { get; set; }

        [PersistenceProperty("QTDE")]
        public decimal Qtde { get; set; }

        [PersistenceProperty("TOTM2")]
        public decimal TotM2 { get; set; }

        [PersistenceProperty("TOTM2COMETIQUETA")]
        public decimal TotM2ComEtiqueta { get; set; }

        [PersistenceProperty("TOTM2SEMETIQUETA")]
        public decimal TotM2SemEtiqueta { get; set; }

        [PersistenceProperty("TOTM2PRODUCAO")]
        public decimal TotM2Producao { get; set; }

        [PersistenceProperty("TOTM2VENDA")]
        public decimal TotM2Venda { get; set; }
        
        #endregion

        #region Propriedades de Suporte

        public string EspessuraString { get { return Espessura.ToString() + " MM"; } }

        public IList<PosicaoMateriaPrimaChapa> Chapas { get; set; }

        public decimal TotM2Estoque
        {
            get
            {
                return Chapas.Sum(f => f.TotalM2Chapa);
            }
        }

        public decimal TotM2Disponivel
        {
            get
            {
                return TotM2Estoque - TotM2;
            }
        }

        public string TotM2DisponivelNovo
        {
            get
            {
                var TotM2DisponivelSalvo = 0m;
                var adminSync = Helper.UserInfo.GetUserInfo.IsAdminSync;

                if (adminSync)
                    TotM2DisponivelSalvo = MovMateriaPrimaDAO.Instance.ObterSaldo(null, (int)IdCorVidro, (decimal)Espessura);

                return TotM2Disponivel + (adminSync ? " (" + TotM2DisponivelSalvo + ")" : "");
            }
        }

        #endregion
    }
}