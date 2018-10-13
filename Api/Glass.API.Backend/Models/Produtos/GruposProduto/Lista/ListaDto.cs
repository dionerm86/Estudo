// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.GruposProduto.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de grupos de produto.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="grupoProduto">O grupo de produto que será retornado.</param>
        public ListaDto(Global.Negocios.Entidades.GrupoProd grupoProduto)
        {
            this.Id = grupoProduto.IdGrupoProd;
            this.Nome = grupoProduto.Descricao;
            this.Tipo = new IdNomeDto()
            {
                Id = (int)grupoProduto.TipoGrupo,
                Nome = Colosoft.Translator.Translate(grupoProduto.TipoGrupo).Format(),
            };

            this.TiposCalculo = new TiposCalculoDto()
            {
                Pedido = new IdNomeDto()
                {
                    Id = (int?)grupoProduto.TipoCalculo,
                    Nome = Colosoft.Translator.Translate(grupoProduto.TipoCalculo).Format(),
                },
                NotaFiscal = new IdNomeDto()
                {
                    Id = (int?)grupoProduto.TipoCalculoNf,
                    Nome = Colosoft.Translator.Translate(grupoProduto.TipoCalculoNf).Format(),
                },
            };

            this.Estoque = new EstoqueDto()
            {
                BloquearEstoque = grupoProduto.BloquearEstoque,
                AlterarEstoque = grupoProduto.AlterarEstoque,
                AlterarEstoqueFiscal = grupoProduto.AlterarEstoqueFiscal,
                ExibirMensagemEstoque = grupoProduto.ExibirMensagemEstoque,
                GeraVolume = grupoProduto.GeraVolume,
            };

            this.Permissoes = new PermissoesDto()
            {
                Excluir = !grupoProduto.GrupoSistema,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(Data.Model.LogAlteracao.TabelaAlteracao.GrupoProduto, (uint)grupoProduto.IdGrupoProd, null),
            };
        }

        /// <summary>
        /// Obtém ou define o tipo do grupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public IdNomeDto Tipo { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de cálculo do grupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposCalculo")]
        public TiposCalculoDto TiposCalculo { get; set; }

        /// <summary>
        /// Obtém ou define dados de estoque do grupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("estoque")]
        public EstoqueDto Estoque { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões do item.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
