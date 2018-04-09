﻿using System;
using System.Collections.Generic;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using Glass.Pool;
using GDA;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo
{
    class SemAlteracaoStrategy : Singleton<SemAlteracaoStrategy>, IDescontoAcrescimoStrategy
    {
        private SemAlteracaoStrategy() { }

        public bool Aplicar(GDASession sessao, TipoValor tipo, decimal valorAplicar, IEnumerable<IProdutoCalculo> produtos)
        {
            return false;
        }

        public bool Remover(GDASession sessao, IEnumerable<IProdutoCalculo> produtos)
        {
            return false;
        }
    }
}
