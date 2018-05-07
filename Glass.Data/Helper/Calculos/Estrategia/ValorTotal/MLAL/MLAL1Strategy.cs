namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.MLAL
{
    class MLAL1Strategy : MLALBaseStrategy<MLAL1Strategy>
    {
        private MLAL1Strategy() { }

        protected override float ValorArredondar
        {
            get { return 1f; }
        }
    }
}
