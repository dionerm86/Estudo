using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Seguranca;

namespace Glass.Data.Model
{
    /// <summary>
    /// Armazena as informações do usuário.
    /// </summary>
    public class InfoUsuario : Seguranca.IInfoUsuario
    {
        public InfoUsuario() { }

        #region Variavies Locais

        private ModuloIndividual[] _permissoes;
        private static Dictionary<uint, uint[]> _modulosUsuario = new Dictionary<uint, uint[]>();

        #endregion

        #region Propriedades

        /// <summary>
        /// Codigo do usuario.
        /// </summary>
        public uint CodUser { get; private set; }

        /// <summary>
        /// Nome do usuário
        /// </summary>
        public string Nome { get; private set; }

        /// <summary>
        /// Tipo do usuário.
        /// </summary>
        public Seguranca.TipoFuncionario TipoUsuario { get; private set; }

        /// <summary>
        /// Identificador do cliente associado.
        /// </summary>
        public uint? IdCliente { get; private set; }

        /// <summary>
        /// Identificador da loja associada.
        /// </summary>
        public uint? IdLoja { get; private set; }

        /// <summary>
        /// Define é uma usuário adminstrador da Sync
        /// </summary>
        public bool IsAdminSync { get; private set; }

        /// <summary>
        /// Nome da loja associada.
        /// </summary>
        public string NomeLoja { get; private set; }

        /// <summary>
        /// UF da loja.
        /// </summary>
        public string UfLoja { get; set; }

        public bool IsCaixaDiario
        {
            get
            {
                return TipoUsuario == Glass.Seguranca.TipoFuncionario.CaixaDiario || VerificarPermissao(Glass.Seguranca.ModuloIndividual.CaixaDiario);
            }
        }

        public bool IsFinanceiroReceb
        {
            get
            {
                return TipoUsuario == Glass.Seguranca.TipoFuncionario.Financeiro ||
                       TipoUsuario == Glass.Seguranca.TipoFuncionario.FinanceiroGeral ||
                       VerificarPermissao(Glass.Seguranca.ModuloIndividual.FinanceiroRecebimento);
            }
        }

        public bool IsFinanceiroPagto
        {
            get
            {
                return TipoUsuario == Glass.Seguranca.TipoFuncionario.FinanceiroPagto ||
                       TipoUsuario == Glass.Seguranca.TipoFuncionario.FinanceiroGeral ||
                       VerificarPermissao(Glass.Seguranca.ModuloIndividual.FinanceiroPagamento);
            }
        }

        /// <summary>
        /// Identifica se o usuário é do financeiro geral.
        /// </summary>
        public bool IsFinanceiroGeral
        {
            get { return TipoUsuario == Glass.Seguranca.TipoFuncionario.FinanceiroGeral; }
        }

        /// <summary>
        /// Identifica se o usuário é um adminstrador.
        /// </summary>
        public bool IsAdministrador
        {
            get { return TipoUsuario == Glass.Seguranca.TipoFuncionario.Administrador; }
        }

        /// <summary>
        /// Identifica se o usuário é um cliente.
        /// </summary>
        public bool IsCliente
        {
            get { return IdCliente > 0; }
        }

        #endregion

        #region Contrutores

        /// <summary>
        /// Construtor padrao.
        /// </summary>
        /// <param name="codUser"></param>
        /// <param name="nome"></param>
        /// <param name="tipoUsuario"></param>
        /// <param name="idCliente"></param>
        /// <param name="idLoja"></param>
        /// <param name="nomeLoja"></param>
        /// <param name="ufLoja"></param>
        /// <param name="isAdminSync"></param>
        /// <param name="ultimaAtividade"></param>
        /// <param name="permissoes"></param>
        public InfoUsuario(
            uint codUser, string nome, Seguranca.TipoFuncionario tipoUsuario,
            uint? idCliente, uint? idLoja, string nomeLoja, string ufLoja,
            bool isAdminSync, DateTime ultimaAtividade)
        {
            this.CodUser = codUser;
            this.Nome = nome;
            this.TipoUsuario = tipoUsuario;
            this.IdCliente = idCliente;
            this.IdLoja = idLoja;
            this.NomeLoja = nomeLoja;
            this.UfLoja = ufLoja;
            this.IsAdminSync = isAdminSync;
        }

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Configura o identificador do cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        public void ConfiguraCliente(uint idCliente)
        {
            IdCliente = idCliente;
        }

        /// <summary>
        /// Configura as permissões do usuário.
        /// </summary>
        public void ConfiguraPermissoes(IEnumerable<ModuloIndividual> permissoes)
        {
            this._permissoes = permissoes.ToArray();
        }

        /// <summary>
        /// Verifica se o usuário tem a permissão informada.
        /// </summary>
        /// <param name="modulo">Modulo que será verificado.</param>
        /// <returns></returns>
        public bool VerificarPermissao(ModuloIndividual modulo)
        {
            return _permissoes != null && _permissoes.Any(f => f == modulo);
        }

        #endregion
    }
}
