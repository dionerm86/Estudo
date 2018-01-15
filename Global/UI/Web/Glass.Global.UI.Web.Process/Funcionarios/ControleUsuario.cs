using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Global.UI.Web.Process.Funcionarios
{
    /// <summary>
    /// Clase para auxilar na parte do controle de usuários.
    /// </summary>
    public class ControleUsuario
    {
        #region Tipos Aninhados

        /// <summary>
        /// Armazena os dados do grupo de módulos.
        /// </summary>
        public class GrupoModulo
        {
            #region Variáveis Locais

            private string _descricao;
            private IEnumerable<Negocios.Entidades.FuncModuloPesquisa> _modulos;

            #endregion

            #region Propriedades

            /// <summary>
            /// Descrição do grupo
            /// </summary>
            public string Descricao
            {
                get { return _descricao; }
            }

            /// <summary>
            /// Módulos associados.
            /// </summary>
            public IEnumerable<Negocios.Entidades.FuncModuloPesquisa> Modulos
            {
                get { return _modulos; }
            }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="descricao"></param>
            /// <param name="modulos"></param>
            public GrupoModulo(string descricao, IEnumerable<Negocios.Entidades.FuncModuloPesquisa> modulos)
            {
                _descricao = descricao;
                _modulos = modulos;
            }

            #endregion
        }

        #endregion

        #region Variáveis Locais

        private Negocios.IFuncionarioFluxo _funcionarioFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Fluxo do funcionário.
        /// </summary>
        protected Negocios.IFuncionarioFluxo FuncionarioFluxo
        {
            get
            {
                if (_funcionarioFluxo == null)
                    _funcionarioFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Negocios.IFuncionarioFluxo>();

                return _funcionarioFluxo;
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera os grupos de módulos do funcionário.
        /// </summary>
        /// <param name="idFunc">Identificador do funcionário.</param>
        /// <returns></returns>
        public IEnumerable<GrupoModulo> ObtemGruposModulos(int idFunc)
        {
            if (idFunc > 0)
                return FuncionarioFluxo.ObtemModulosFuncionario(idFunc)
                    .GroupBy(f => f.GrupoModulo ?? string.Empty, StringComparer.InvariantCultureIgnoreCase)
                    .Select(f => new GrupoModulo(f.Key, f));
            else
                return new GrupoModulo[0];
        }

        /// <summary>
        /// Altera a permissão.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idModulo"></param>
        /// <param name="permitir"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult AlterarPermissao(int idFunc, int idModulo, bool permitir)
        {
            var funcionario = FuncionarioFluxo.ObtemFuncionario(idFunc);

            if (funcionario == null)
                return new Colosoft.Business.SaveResult(false, "Dados do funcionário não foram encontrados".GetFormatter());

            var modulo = funcionario.FuncionarioModulos.FirstOrDefault(f => f.IdModulo == idModulo);
            if (modulo == null)
            {
                modulo = new Negocios.Entidades.FuncModulo
                {
                    IdModulo = idModulo
                };
                funcionario.FuncionarioModulos.Add(modulo);
            }

            modulo.Permitir = permitir;

            // Salva os dados do funcionário
            return FuncionarioFluxo.SalvarFuncionario(funcionario);
        }
        
        #endregion
    }
}
