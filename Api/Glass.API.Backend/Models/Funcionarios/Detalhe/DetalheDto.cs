// <copyright file="DetalheDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de um funcionario para a tela de edição.
    /// </summary>
    [DataContract(Name = "Detalhe")]
    public class DetalheDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="DetalheDto"/>.
        /// </summary>
        /// <param name="funcionario">A model de funcionario.</param>
        internal DetalheDto(Glass.Global.Negocios.Entidades.Funcionario funcionario)
        {
            this.Nome = funcionario.Nome;
            this.IdTipoFuncionario = funcionario.IdTipoFunc;
            this.IdsSetores = funcionario.Setores.Select(f => f.IdSetor);
            this.Endereco = new EnderecoDto
            {
                Logradouro = funcionario.Endereco,
                Bairro = funcionario.Bairro,
                Cep = funcionario.Cep,
                Complemento = funcionario.Compl,
                Cidade = new CidadeDto
                {
                    Id = 1,
                    Nome = funcionario.Cidade,
                    Uf = funcionario.Uf,
                },
            };

            this.IdLoja = funcionario.IdLoja;
            this.Contatos = new ContatosDto
            {
                TelefoneResidencial = funcionario.TelRes,
                TelefoneCelular = funcionario.TelCel,
                TelefoneContato = funcionario.TelCont,
                Email = funcionario.Email,
                Ramal = funcionario.Ramal,
            };

            this.DocumentosEDadosPessoais = new DocumentosEDadosPessoaisDto
            {
                Rg = funcionario.Rg,
                Cpf = funcionario.Cpf,
                Funcao = funcionario.Funcao,
                EstadoCivil = funcionario.EstCivil,
                DataNascimento = funcionario.DataNasc,
                DataEntrada = funcionario.DataEnt,
                DataSaida = funcionario.DataSaida,
                Salario = funcionario.Salario,
                Gratificacao = funcionario.Gratificacao,
                NumeroCTPS = funcionario.NumCarteiraTrabalho,
                AuxilioAlimentacao = funcionario.AuxAlimentacao,
                NumeroPis = funcionario.NumPis,
                Registrado = funcionario.Registrado,
                Foto = string.Empty,
            };

            this.Situacao = (int)funcionario.Situacao;
            this.Acesso = new AcessoDto
            {
                Login = funcionario.Login,
                Senha = funcionario.Senha,
            };

            this.IdsTiposPedidos = funcionario.TipoPedido.Split(',').Select(f => int.Parse(f));
            this.NumeroDiasParaAtrasarPedidos = funcionario.NumDiasAtrasarPedido;
            this.NumeroPdv = funcionario.NumeroPdv.ToString();
            this.Permissoes = new PermisoesDto
            {
                EnviarEmailPedidoFinalizado = funcionario.EnviarEmail,
                UtilizarChat = funcionario.HabilitarChat,
            };

            this.Obsercavao = funcionario.Obs;
        }

        /// <summary>
        /// Obtém ou define o nome do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do tipo do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("idTipoFuncionario")]
        public int IdTipoFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores dos setores.
        /// </summary>
        [DataMember]
        [JsonProperty("idsSetores")]
        public IEnumerable<int> IdsSetores { get; set; }

        /// <summary>
        /// Obtém ou define o endereço do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("endereco")]
        public EnderecoDto Endereco { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [DataMember]
        [JsonProperty("idLoja")]
        public int IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define os dados de contato do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("contatos")]
        public ContatosDto Contatos { get; set; }

        /// <summary>
        /// Obtém ou define os dados de documentação e pessoais do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("documentosEDadosPessoais")]
        public DocumentosEDadosPessoaisDto DocumentosEDadosPessoais { get; set; }

        /// <summary>
        /// Obtém ou define a situacao do funcionários.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public int Situacao { get; set; }

        /// <summary>
        /// Obtém ou define os dados de acesso do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("acesso")]
        public AcessoDto Acesso { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores do tipos de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idsTiposPedidos")]
        public IEnumerable<int> IdsTiposPedidos { get; set; }

        /// <summary>
        /// Obtém ou define o numero de dias para atrasar pedidos.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroDiasParaAtrasarPedidos")]
        public int NumeroDiasParaAtrasarPedidos { get; set; }

        /// <summary>
        /// Obtém ou define o numero do ponto de venda (frente de caixa).
        /// </summary>
        [DataMember]
        [JsonProperty("numeroPdv")]
        public string NumeroPdv { get; set; }

        /// <summary>
        /// Obtém ou define as permissões do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermisoesDto Permissoes { get; set; }

        /// <summary>
        /// Obtém ou define a observação do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("string")]
        public string Obsercavao { get; set; }
    }
}
