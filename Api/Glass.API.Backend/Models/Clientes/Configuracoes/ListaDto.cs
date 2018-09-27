// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de clientes.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.CadastrarCliente = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarCliente);
            this.ExibirTotalComprado = Config.PossuiPermissao(Config.FuncaoMenuCadastro.ExibirTotalCompradoCliente);
            this.Imprimir = Config.PossuiPermissao(Config.FuncaoMenuCadastro.ExportarImprimirDadosClientes);
            this.AnexarImagens = Config.PossuiPermissao(Config.FuncaoMenuCadastro.AnexarArquivosCliente);
            this.CadastrarSugestoes = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes);
            this.AtivarClientes = UserInfo.GetUserInfo.IsAdministrador;
            this.AlterarVendedor = UserInfo.GetUserInfo.IsAdministrador;
            this.AlterarRota = UserInfo.GetUserInfo.IsAdministrador;
            this.ConsultarPrecoTabela = this.ExibirPrecoTabela();
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário tem permissão de cadastrar clientes.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarCliente")]
        public bool CadastrarCliente { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário pode visualizar o total comprado por cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirTotalComprado")]
        public bool ExibirTotalComprado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverão ser exibidos links de impressão da lista de clientes.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimir")]
        public bool Imprimir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido anexar imagens ao cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("anexarImagens")]
        public bool AnexarImagens { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido cadastrar sugestões para o cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarSugestoes")]
        public bool CadastrarSugestoes { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido consultar o preço de tabela do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("consultarPrecoTabela")]
        public bool ConsultarPrecoTabela { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido ativar os clientes buscados com o filtro da tela.
        /// </summary>
        [DataMember]
        [JsonProperty("ativarClientes")]
        public bool AtivarClientes { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido alterar o vendedor dos clientes buscados com o filtro da tela.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarVendedor")]
        public bool AlterarVendedor { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido alterar a rota dos clientes buscados com o filtro da tela.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarRota")]
        public bool AlterarRota { get; set; }

        private bool ExibirPrecoTabela()
        {
            // Recupera o funcionário
            var funcionario = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>().ObtemFuncionario((int)UserInfo.GetUserInfo.CodUser);

            // Verificar se ele possui acesso ao menu de preço de tabela
            var menusFunc = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                .GetInstance<Glass.Global.Negocios.IMenuFluxo>()
                .ObterMenusPorFuncionario(funcionario);

            return menusFunc != null && menusFunc.Any(f => !string.IsNullOrEmpty(f.Url) && f.Url.ToLower().Contains("listaprecotabcliente"));
        }
    }
}
