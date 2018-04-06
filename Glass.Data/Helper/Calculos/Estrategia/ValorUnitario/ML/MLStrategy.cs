﻿using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.ML
{
    class MLStrategy : BaseStrategy<MLStrategy>
    {
        private MLStrategy() { }

        protected override decimal Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, decimal total, ArredondarAluminio arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            int alturaBenef, int larguraBenef)
        {
            decimal divisor = Divisor(produto);
            return total / divisor;
        }

        private decimal Divisor(IProdutoCalculo produto)
        {
            float divisor = produto.Altura * produto.Qtde;
            return divisor > 0 ? (decimal)divisor : 1;
        }
    }
}
