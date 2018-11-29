// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Projetos.MedidasProjeto;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;

namespace Glass.API.Backend.Models.Projetos.V1.MedidasProjeto.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de medidas de projeto.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaMedidasProjeto(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define a descrição da medida de projeto.
        /// </summary>
        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}
