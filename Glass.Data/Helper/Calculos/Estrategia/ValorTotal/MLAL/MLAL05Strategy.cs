namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.MLAL
{
    class MLAL05Strategy : MLALBaseStrategy<MLAL05Strategy>
    {
        private MLAL05Strategy() { }

        protected override float ValorArredondar
        {
            get { return 0.5f; }
        }
    }
}
