using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Global
{
    /// <summary>
    /// Implementação da entidade cor do aluminio.
    /// </summary>
    public class CorAluminio : Glass.Api.Global.ICorAluminio
    {
        /// <summary>
        /// Identificador.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao { get; }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="model"></param>
        public CorAluminio(Data.Model.CorAluminio model)
        {
            Id = model.IdCorAluminio;
            Descricao = model.Descricao;
        }
    }

    /// <summary>
    /// Implementação do fluxo de negocio da cor do alumínio.
    /// </summary>
    public class AluminioFluxo : Api.Global.IAluminioFluxo
    {

        /// <summary>
        /// Recupera as cores dos aluminio.
        /// </summary>
        /// <returns></returns>
        public IList<Api.Global.ICorAluminio> ObterCores()
        {
            var retorno = new List<Glass.Api.Global.ICorAluminio>();

            retorno.AddRange(Glass.Data.DAL.CorAluminioDAO.Instance.GetAll().Select(f => new CorAluminio(f)));

            return retorno;
        }
    }
}
