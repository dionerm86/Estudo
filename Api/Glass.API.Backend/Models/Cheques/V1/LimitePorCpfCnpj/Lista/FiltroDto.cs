// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Cheques.LimitePorCpfCnpj;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cheques.V1.LimitePorCpfCnpj.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de limites de cheques por cpf/cnpj.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaLimiteChequeCpfCnpj(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o cpf/cnpj associado aos cheques.
        /// </summary>
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }
    }
}