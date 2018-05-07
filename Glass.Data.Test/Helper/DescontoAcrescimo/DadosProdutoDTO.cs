using Glass.Data.Model;
using System;

namespace Glass.Data.Test.Helper.DescontoAcrescimo
{
    class DadosProdutoDTO : IDadosProduto
    {
        public IDadosBaixaEstoque DadosBaixaEstoque
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDadosChapaVidro DadosChapaVidro
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDadosGrupoSubgrupo DadosGrupoSubgrupo
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int? AlturaProduto()
        {
            throw new NotImplementedException();
        }

        public float AreaMinima()
        {
            throw new NotImplementedException();
        }

        public bool CalcularAreaMinima(int numeroBeneficiamentos)
        {
            throw new NotImplementedException();
        }

        public decimal CustoCompra()
        {
            throw new NotImplementedException();
        }

        public string Descricao()
        {
            throw new NotImplementedException();
        }

        public int? LarguraProduto()
        {
            throw new NotImplementedException();
        }

        public decimal ValorTabela(bool usarCliente = true)
        {
            throw new NotImplementedException();
        }
    }
}
