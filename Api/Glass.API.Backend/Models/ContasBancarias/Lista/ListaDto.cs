// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasBancarias.V1.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de contas bancárias.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="contaBancaria">A conta bancária que será retornada.</param>
        public ListaDto(Financeiro.Negocios.Entidades.ContaBancoPesquisa contaBancaria)
        {
            this.Id = contaBancaria.IdContaBanco;
            this.Nome = contaBancaria.Nome;
            this.Situacao = new IdNomeDto
            {
                Id = (int)contaBancaria.Situacao,
                Nome = Colosoft.Translator.Translate(contaBancaria.Situacao).Format(),
            };

            this.Loja = new IdNomeDto
            {
                Id = contaBancaria.IdLoja,
                Nome = contaBancaria.Loja,
            };

            this.DadosBanco = new DadosBancoDto
            {
                Banco = new IdNomeDto
                {
                    Id = contaBancaria.CodBanco,
                    Nome = this.ObterNomeBanco(contaBancaria.CodBanco),
                },

                Titular = contaBancaria.Titular,
                Agencia = contaBancaria.Agencia,
                Conta = contaBancaria.Conta,
                CodigoConvenio = contaBancaria.CodConvenio,
            };

            this.Cnab = new CnabDto
            {
                CodigoCliente = contaBancaria.CodCliente,
                Posto = contaBancaria.Posto,
            };

            this.Permissoes = new PermissoesDto
            {
                Excluir = contaBancaria.QtdeMovimentacoes == 0,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(
                    LogAlteracao.TabelaAlteracao.ContaBanco,
                    (uint)contaBancaria.IdContaBanco,
                    null),
            };
        }

        /// <summary>
        /// Obtém ou define a situação da conta bancária.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a loja da conta bancária.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public IdNomeDto Loja { get; set; }

        /// <summary>
        /// Obtém ou define dados do banco da conta bancária.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosBanco")]
        public DadosBancoDto DadosBanco { get; set; }

        /// <summary>
        /// Obtém ou define dados do CNAB da conta bancária.
        /// </summary>
        [DataMember]
        [JsonProperty("cnab")]
        public CnabDto Cnab { get; set; }

        /// <summary>
        /// Obtém ou define permissões da lista de contas bancárias.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private string ObterNomeBanco(int? codigoBanco)
        {
            if (codigoBanco == null)
            {
                return string.Empty;
            }

            var banco = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Financeiro.Negocios.IContaBancariaFluxo>()
                .ObtemBancos()
                .FirstOrDefault(f => f.Id == codigoBanco);

            return banco.Name;
        }
    }
}
