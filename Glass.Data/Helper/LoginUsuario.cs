using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Helper
{
    public class LoginUsuario
    {
        #region Propriedades

        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint CodUser { get; set; }

        [PersistenceProperty("IDCLIENTE")]
        public uint? IdCliente { get; set; }

        [PersistenceProperty("IDTIPOFUNC")]
        public uint TipoUsuario { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        #endregion

        #region Propriedades Estendidas

        public string DescrTipoFunc { get; set; }

        public string NomeLoja { get; set; }

        public string UfLoja { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string UsuarioSync { get; set; }

        /// <summary>
        /// Propriedade utilizada em usuário Sync, verifica se o usuário possui permissão para alterar a configuração do cliente.
        /// </summary>
        public bool PodeAlterarConfiguracao { get; set; }

        public DateTime UltimaAtividade { get; set; }

        private bool? _isAdminSync;

        public bool IsAdminSync
        {
            get
            {
                if (!IsAdministrador)
                    return false;

                if (_isAdminSync == null)
                    _isAdminSync = FuncionarioDAO.Instance.IsAdminSync(CodUser);

                return _isAdminSync.GetValueOrDefault();
            }
        }

        public bool IsCaixaDiario
        {
            get { return Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario); }
        }

        public bool IsFinanceiroReceb
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
            }
        }

        public bool IsFinanceiroPagto
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento);
            }
        }

        public bool IsFinanceiroGeral
        {
            get { return Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) && Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento); }
        }

        public bool IsAdministrador
        {
            get { return TipoUsuario == (uint)Utils.TipoFuncionario.Administrador; }
        }

        public bool IsCliente
        {
            get { return IdCliente > 0; }
        }

        #endregion
    }
}
