// <copyright file="TraducaoOrdenacaoListaSetores.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Producao.Setores
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de setores.
    /// </summary>
    internal class TraducaoOrdenacaoListaSetores : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaSetores"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaSetores(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "NumeroSequencia ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "nome":
                    return "Descricao";

                case "sequencia":
                    return "NumeroSequencia";

                case "alturamaxima":
                    return "Altura";

                case "larguramaxima":
                    return "Largura";

                case "corsetor":
                    return "Cor";

                case "exibirsetoresleiturapeca":
                    return "ExibirSetores";

                case "exibirnalistaerelatorio":
                    return "ExibirRelatorio";

                case "consultarantesdaleitura":
                    return "ConsultarAntes";

                case "situacao":
                case "capacidadediaria":
                case "ignorarcapacidadediaria":
                case "cortela":
                case "exibirpainelcomercial":
                case "exibirpainelproducao":
                case "exibirimagemcompleta":
                case "tipo":
                case "corte":
                case "forno":
                case "laminado":
                case "entradaestoque":
                case "gerenciarfornada":
                case "desafioperda":
                case "metaperda":
                case "impediravanco":
                case "informarrota":
                case "informarcavalete":
                case "permitirleituraforaroteiro":
                case "tempologin":
                case "tempoalertainatividade":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
