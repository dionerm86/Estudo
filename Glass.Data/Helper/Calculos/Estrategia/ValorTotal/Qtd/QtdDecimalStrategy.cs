namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Qtd
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
