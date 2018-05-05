namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.MLAL
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
