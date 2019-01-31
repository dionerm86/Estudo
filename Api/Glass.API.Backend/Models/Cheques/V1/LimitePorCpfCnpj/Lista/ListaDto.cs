// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using WebGlass.Business.Cheque.Entidade;

namespace Glass.API.Backend.Models.Cheques.V1.LimitePorCpfCnpj.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um limite de cheque por Cpf ou Cnpj para a tela de listagem.
    /// </summary>
    [DataContract(Name = "LimiteChequePorCpfCnpj")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="limiteCheque">A model de limite de cheque.</param>
        internal ListaDto(LimiteCheque limiteCheque)
        {
            this.Id = (int)limiteCheque.Codigo;
            this.CpfCnpj = limiteCheque.CpfCnpj;
            this.Limite = new LimiteDto
            {
                Total = limiteCheque.Limite,
                Utilizado = limiteCheque.ValorUtilizado,
                Restante = limiteCheque.ValorRestante,
            };

            this.Observacao = limiteCheque.Observacao;
            this.CorLinha = this.ObterCorLinha(limiteCheque);
            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.LimiteChequeCpfCnpj, limiteCheque.Codigo, null),
            };
        }

        /// <summary>
        /// Obtém ou define o cpf/cnpj associado ao cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes ao limite do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("limite")]
        public LimiteDto Limite { get; set; }

        /// <summary>
        /// Obtém ou define a observação do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        /// <summary>
        /// Obtém ou define as permissões concebidas ao limite de cheque por cpf/cnpj.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private string ObterCorLinha(LimiteCheque limiteCheque)
        {
            if (limiteCheque.ValorRestante < 0)
            {
                return "Red";
            }
            else if (limiteCheque.ValorRestante > 0)
            {
                return "Green";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}