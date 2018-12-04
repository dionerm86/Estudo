// <copyright file="TraducaoOrdenacaoListaVolumes.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Volumes
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de volumes.
    /// </summary>
    internal class TraducaoOrdenacaoListaVolumes : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaVolumes"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaVolumes(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdVolume Desc"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdVolume";

                case "funcionariofinalizacao":
                    return "NomeFuncFinalizacao";

                case "datafinalizacao":
                    return "DataFechamento";

                case "situacao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
