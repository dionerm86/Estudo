// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.ArquivosOtimizacao;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.ArquivosOtimizacao.V1
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de arquivos de otimização.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaArquivosOtimizacao(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do funcionário.
        /// </summary>
        [JsonProperty("idFuncionario")]
        public int? IdFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define o período do início do cadastro.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define o período do fim do cadastro.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da ação.
        /// </summary>
        [JsonProperty("direcao")]
        public int? Direcao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta.
        /// </summary>
        [JsonProperty("codigoEtiqueta")]
        public int? CodigoEtiqueta { get; set; }
    }
}
