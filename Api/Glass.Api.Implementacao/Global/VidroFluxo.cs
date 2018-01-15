using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Global
{
    /// <summary>
    /// Entidade de negocio cor de vidro.
    /// </summary>
    public class CorVidro : Glass.Api.Global.ICorVidro
    {
        #region Propriedades

        /// <summary>
        /// Identificador.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public CorVidro()
        {

        }

        /// <summary>
        /// Construtor da entidade de negocio cor de vidro.
        /// </summary>
        /// <param name="modelo"></param>
        public CorVidro(Glass.Data.Model.CorVidro modelo)
        {
            Id = modelo.IdCorVidro;
            Descricao = modelo.Descricao;
        }

        #endregion
    }

    public class EspessuraVidro : Api.Global.IEspessuraVidro
    {
        #region Propriedades

        /// <summary>
        /// Valor.
        /// </summary>
        public int Valor { get; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da entidade de negocio espessura do vidro.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="descricao"></param>
        public EspessuraVidro(int valor, string descricao)
        {
            Valor = valor;
            Descricao = descricao;
        }

        #endregion
    }

    /// <summary>
    /// Fluxo de negocio do vidro.
    /// </summary>
    public class VidroFluxo : Glass.Api.Global.IVidroFluxo
    {
        /// <summary>
        /// Recupera as epessuras do vidro.
        /// </summary>
        /// <returns></returns>
        public IList<Glass.Api.Global.IEspessuraVidro> ObterEspessuras()
        {
            var retorno = new List<Glass.Api.Global.IEspessuraVidro>();
            retorno.Add(new EspessuraVidro(06, "06MM"));
            retorno.Add(new EspessuraVidro(08, "08MM"));
            retorno.Add(new EspessuraVidro(10, "10MM"));
            retorno.Add(new EspessuraVidro(12, "12MM"));

            return retorno;
        }

        /// <summary>
        /// Recupera as cores do vidro.
        /// </summary>
        /// <returns></returns>
        public IList<Glass.Api.Global.ICorVidro> ObterCores()
        {
            var retorno = Glass.Data.DAL.CorVidroDAO.Instance.GetForProjeto()
                .Select(f => new CorVidro(f)).ToList<Glass.Api.Global.ICorVidro>();

            //retorno.Insert(0, new CorVidro());
            return retorno;
        }
    }
}
