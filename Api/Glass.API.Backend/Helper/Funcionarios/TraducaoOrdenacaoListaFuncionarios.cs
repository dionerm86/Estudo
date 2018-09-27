// <copyright file="TraducaoOrdenacaoListaFuncionarios.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Funcionarios
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de funcionários.
    /// </summary>
    internal class TraducaoOrdenacaoListaFuncionarios : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaFuncionarios"/>.
        /// </summary>
        /// <param name="ordenacao"> A ordenação realizada na tela</param>
        public TraducaoOrdenacaoListaFuncionarios(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdFunc ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdFunc";

                case "tipofunc":
                    return "TipoFuncionario";

                case "cpffunc":
                    return "cpf";

                case "rgfunc":
                    return "rg";

                case "nome":
                case "loja":
                case "telRes":
                case "telcel":
                case "telres":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
