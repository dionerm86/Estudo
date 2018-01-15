using Glass.Data.Helper;
using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ResumoCorteDAO))]
    public class ResumoCorte
    {
        #region Propriedades

        public uint IdPedido { get; set; }

        public uint IdProd { get; set; }

        public uint IdGrupoProd { get; set; }

        public string CodInterno { get; set; }

        public string DescrProd { get; set; }

        public string DescrGrupoProd { get; set; }

        public double Altura { get; set; }

        public double TotM2 { get; set; }

        public double TotM2Calc { get; set; }

        public decimal Total { get; set; }

        public float Qtde { get; set; }

        public float Espessura { get; set; }

        public bool IsVidro { get; set; }

        public int Largura { get; set; }

        public string CodAplicacao { get; set; }

        public string CodProcesso { get; set; }

        public double Peso
        {
            get { return Utils.CalcPeso((int)IdProd, Espessura, (float)TotM2, (float)Qtde, (float)Altura, false); }
        }

        public int NumeroVia { get; set; }

        #endregion
    }
}