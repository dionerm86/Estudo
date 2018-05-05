namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.M2
{
    class M2DiretoStrategy : M2BaseStrategy<M2DiretoStrategy>
    {
        private M2DiretoStrategy() { }

        protected override bool DeveCalcularMultiploDe5
        {
            get { return false; }
        }
    }
}
