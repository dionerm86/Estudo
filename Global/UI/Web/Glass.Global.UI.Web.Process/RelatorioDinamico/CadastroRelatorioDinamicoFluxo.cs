using Glass.Global.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Glass.Global.UI.Web.Process.RelatorioDinamico
{
    /// <summary>
    /// Representa o fluxo do cadastro de modelo de projeto.
    /// </summary>
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class CadastroRelatorioDinamicoFluxo
    {
        #region Local Variables

        private IRelatorioDinamicoFluxo _relatorioDinamicoFluxo;
        private int? _idRelatorioDinamico;
        private Negocios.Entidades.RelatorioDinamico _relatorioDinamico;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do relatório dinâmico.
        /// </summary>
        public int? IdRelatorioDinamico
        {
            get { return _idRelatorioDinamico; }
            set
            {
                if (_idRelatorioDinamico != value)
                {
                    _idRelatorioDinamico = value;
                    _relatorioDinamico = null;
                }
            }
        }

        /// <summary>
        /// Relatório dinâmico.
        /// </summary>
        public Negocios.Entidades.RelatorioDinamico RelatorioDinamico
        {
            get
            {
                if (_relatorioDinamico == null && IdRelatorioDinamico.HasValue)
                    _relatorioDinamico = _relatorioDinamicoFluxo.ObterRelatorioDinamico(IdRelatorioDinamico.Value);

                else if (_relatorioDinamico == null && !IdRelatorioDinamico.HasValue)
                    _relatorioDinamico = _relatorioDinamicoFluxo.CriarRelatorioDinamico();

                return _relatorioDinamico;
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="relatorioDinamicoFluxo">Fluxo de negócio do relatório dinâmico.</param>
        [ImportingConstructor]
        public CadastroRelatorioDinamicoFluxo(Negocios.IRelatorioDinamicoFluxo relatorioDinamicoFluxo)
        {
            _relatorioDinamicoFluxo = relatorioDinamicoFluxo;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Cria uma nova instancia do relatório dinâmico.
        /// </summary>
        /// <returns></returns>
        public Negocios.Entidades.RelatorioDinamico CriarRelatorioDinamico()
        {
            _relatorioDinamico = _relatorioDinamicoFluxo.CriarRelatorioDinamico();
            return _relatorioDinamico;
        }

        /// <summary>
        /// Salva os dados do relatório dinâmico.
        /// </summary>
        /// <param name="relatorioDinamico"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarRelatorioDinamico(Negocios.Entidades.RelatorioDinamico relatorioDinamico)
        {
            return _relatorioDinamicoFluxo.SalvarRelatorioDinamico(relatorioDinamico);
        }

        /// <summary>
        /// Cria um filtro.
        /// </summary>
        /// <returns></returns>
        public Negocios.Entidades.RelatorioDinamicoFiltro CriarFiltro()
        {
            var filtro = _relatorioDinamicoFluxo.CriarFiltro();
            RelatorioDinamico.Filtros.Add(filtro);
            return filtro;
        }

        /// <summary>
        /// Recupera os filtros do relatório dinâmico.
        /// </summary>
        /// <returns></returns>
        public IList<Negocios.Entidades.RelatorioDinamicoFiltro> ObterFiltros()
        {
            return RelatorioDinamico.Filtros;
        }

        /// <summary>
        /// Recupera os dados do filtro associado ao relatório dinâmico.
        /// </summary>
        /// <returns></returns>
        public Negocios.Entidades.RelatorioDinamicoFiltro ObterFiltro(int idRelatorioDinamicoFiltro)
        {
            return RelatorioDinamico.Filtros.FirstOrDefault(f => f.IdRelatorioDinamicoFiltro == idRelatorioDinamicoFiltro);
        }

        /// <summary>
        /// Salva o filtro do relatório dinâmico.
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarFiltro(Negocios.Entidades.RelatorioDinamicoFiltro filtro)
        {
            if (!filtro.ExistsInStorage && !RelatorioDinamico.Filtros.Contains(filtro))
                RelatorioDinamico.Filtros.Add(filtro);

            // Salva os dados do modelo
            return SalvarRelatorioDinamico(RelatorioDinamico);
        }

        /// <summary>
        /// Apaga o filtro do relatório dinâmico.
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarFiltro(Negocios.Entidades.RelatorioDinamicoFiltro filtro)
        {
            RelatorioDinamico.Filtros.Remove(filtro);

            var resultado = _relatorioDinamicoFluxo.SalvarRelatorioDinamico(RelatorioDinamico);
            return new Colosoft.Business.DeleteResult(resultado, resultado.Message);
        }

        #region Ícone

        /// <summary>
        /// Cria um filtro.
        /// </summary>
        /// <returns></returns>
        public Negocios.Entidades.RelatorioDinamicoIcone CriarIcone()
        {
            var icone = _relatorioDinamicoFluxo.CriarIcone();
            RelatorioDinamico.Icones.Add(icone);
            return icone;
        }

        /// <summary>
        /// Recupera os ícones do relatório dinâmico.
        /// </summary>
        /// <returns></returns>
        public IList<Negocios.Entidades.RelatorioDinamicoIcone> ObterIcones()
        {
            return RelatorioDinamico.Icones;
        }

        /// <summary>
        /// Recupera os dados do ícone associado ao relatório dinâmico.
        /// </summary>
        /// <returns></returns>
        public Negocios.Entidades.RelatorioDinamicoIcone ObterIcone(int idRelatorioDinamicoIcone)
        {
            return RelatorioDinamico.Icones.FirstOrDefault(f => f.IdRelatorioDinamicoIcone == idRelatorioDinamicoIcone);
        }

        /// <summary>
        /// Salva o ícone do relatório dinâmico.
        /// </summary>
        /// <param name="icone"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarIcone(Negocios.Entidades.RelatorioDinamicoIcone icone)
        {
            if (!icone.ExistsInStorage && !RelatorioDinamico.Icones.Contains(icone))
                RelatorioDinamico.Icones.Add(icone);

            // Salva os dados do relatório
            return SalvarRelatorioDinamico(RelatorioDinamico);
        }

        /// <summary>
        /// Apaga o ícone do relatório dinâmico.
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarIcone(Negocios.Entidades.RelatorioDinamicoIcone icone)
        {
            RelatorioDinamico.Icones.Remove(icone);

            var resultado = _relatorioDinamicoFluxo.SalvarRelatorioDinamico(RelatorioDinamico);
            return new Colosoft.Business.DeleteResult(resultado, resultado.Message);
        }

        #endregion

        #endregion
    }
}
