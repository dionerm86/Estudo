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
        /// Obtém ou define a data inicial do cadastro da exportação/importação do arquivo.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do cadastro da exportação/importação do arquivo.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de direção, importação/exportação.
        /// </summary>
        [JsonProperty("direcao")]
        public int? Direcao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public uint? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta do arquivo.
        /// </summary>
        [JsonProperty("codigoEtiqueta")]
        public string CodigoEtiqueta { get; set; }
    }
}
