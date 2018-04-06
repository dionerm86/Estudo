using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.MLAL
{
    class MLAL6Strategy : MLALBaseStrategy<MLAL6Strategy>
    {
        private MLAL6Strategy() { }

        protected override float ValorArredondar
        {
            get { return 6f; }
        }

        protected override float? Arredondar(IProdutoCalculo produto, float decimosAltura)
        {
            if (produto.Altura < 6)
                return 6f;

            return null;
        }
    }
}
