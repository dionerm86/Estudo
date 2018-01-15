namespace Glass.Data.Model
{
    public class OrcamentoRapido
    {
        #region Propriedades

        public string Descricao { get; set; }

        public int Qtde { get; set; }

        public float? Altura { get; set; }

        public int? Largura { get; set; }

        public double TotM2 { get; set; }

        public double TotM2Calc { get; set; }

        public string Servicos { get; set; }
 
        public decimal ValorUnitario { get; set; }

        public decimal Total { get; set; }

        #endregion
    }
}