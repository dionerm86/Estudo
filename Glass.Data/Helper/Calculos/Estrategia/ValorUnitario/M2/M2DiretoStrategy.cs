namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.M2
{
    class M2DiretoStrategy : M2BaseStrategy<M2DiretoStrategy>
    {
        private M2DiretoStrategy() { }

        protected override bool CalcularMultiploDe5
        {
            get { return false; }
        }
    }
}
