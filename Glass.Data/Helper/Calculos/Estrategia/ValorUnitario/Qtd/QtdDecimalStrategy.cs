namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.Qtd
{
    class QtdDecimalStrategy : QtdBaseStrategy<QtdDecimalStrategy>
    {
        private QtdDecimalStrategy() { }

        protected override bool ValidarQuantidadeDecimal
        {
            get { return false; }
        }
    }
}
