using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Global
{
    /// <summary>
    /// Opções para cálculo de projeto.
    /// </summary>
    public class CalcProjetoOptions
    {
        #region Properties

        /// <summary>
        /// Cores de vidro.
        /// </summary>
        public IList<Api.Global.ICorVidro> CoresVidro { get; }

        /// <summary>
        /// Espessuras do vidro.
        /// </summary>
        public IList<Api.Global.IEspessuraVidro> EspessurasVidro { get; }

        /// <summary>
        /// Cor aluminio.
        /// </summary>
        public IList<Api.Global.ICorAluminio> CoresAluminio { get; }

        /// <summary>
        /// Cor ferragem.
        /// </summary>
        public IList<Api.Global.ICorFerragem> CoresFerragem { get; }

        /// <summary>
        /// Grupos de modelo.
        /// </summary>
        public IList<Api.Projeto.IGrupoModelo> GruposModelo { get; }

        /// <summary>
        /// Tipos de entrega.
        /// </summary>
        public IList<Api.Global.ITipoEntrega> TiposEntrega { get; }

        /// <summary>
        /// Projetos mais usados.
        /// </summary>
        public IList<Api.Projeto.IProjetoModelo> MaisUsados { get; }

        /// <summary>
        /// Tipo de entrega.
        /// </summary>
        public int TipoEntrega { get; }

        /// <summary>
        /// Identifica se o usuário pode mudar Cor e Espessura dentro de um projeto.
        /// </summary>
        public bool ManterCorEspessura { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="coresVidro"></param>
        /// <param name="espessurasVidro"></param>
        /// <param name="coresAluminio"></param>
        /// <param name="coresFerragem"></param>
        /// <param name="gruposModelo"></param>
        /// <param name="tiposEntrega"></param>
        /// <param name="maisUsados"></param>
        /// <param name="tipoEntrega"></param>
        public CalcProjetoOptions(IList<Api.Global.ICorVidro> coresVidro, IList<Api.Global.IEspessuraVidro> espessurasVidro, IList<Api.Global.ICorAluminio> coresAluminio, IList<Api.Global.ICorFerragem> coresFerragem, 
            IList<Api.Projeto.IGrupoModelo> gruposModelo, IList<Api.Global.ITipoEntrega> tiposEntrega, IList<Api.Projeto.IProjetoModelo> maisUsados, int tipoEntrega)
        {
            CoresVidro = coresVidro;
            EspessurasVidro = espessurasVidro;
            CoresAluminio = coresAluminio;
            CoresFerragem = coresFerragem;
            GruposModelo = gruposModelo;
            TiposEntrega = tiposEntrega;
            MaisUsados = maisUsados;
            TipoEntrega = tipoEntrega;
            ManterCorEspessura = System.Configuration.ConfigurationManager.AppSettings["ManterCorEspessura"].ToLower() == "true";
        }

        #endregion
    }
}
