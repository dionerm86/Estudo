namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.M2
{
    class M2Strategy : M2BaseStrategy<M2Strategy>
    {
        private M2Strategy() { }

        protected override bool DeveCalcularMultiploDe5
        {
            get { return true; }
        }
    }
}
