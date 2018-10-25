using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace Glass.API.Backend.Helper.Parcelas
{
    internal class TraducaoOrdenacaoListaParcelas : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaParcelas"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaParcelas(string ordenacao)
            : base(ordenacao)
        {
        }

        protected override string OrdenacaoPadrao
        {
            get { return "IdParcela ASC"; }
        }

        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdParcela";

                case "nome":
                    return "Descricao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
