namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.MLAL
{
    class MLAL0Strategy : MLALBaseStrategy<MLAL0Strategy>
    {
        private MLAL0Strategy() { }

        protected override float ValorArredondar
        {
            get { return 0; }
        }
    }
}
