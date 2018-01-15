using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    /// <summary>
    /// Implemetação do grupo modelo.
    /// </summary>
    public class GrupoModelo : Glass.Api.Projeto.IGrupoModelo
    {
        /// <summary>
        /// Identificador do grupo.
        /// </summary>
        public uint Id { get;}

        /// <summary>
        /// Nome do grupo.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Construtor da entidade de negocio para grupo modelo.
        /// </summary>
        /// <param name="modelo"></param>
        public GrupoModelo(Glass.Data.Model.GrupoModelo modelo)
        {
            Id = modelo.IdGrupoModelo;
            Nome = modelo.Descricao;
        }
    }

    /// <summary>
    /// Implementação do fuxo de negocio do grupo modelo.
    /// </summary>
    public class GrupoModeloFluxo : Glass.Api.Projeto.IGrupoModeloFluxo
    {
        /// <summary>
        /// Recupera os grupos dos modelos do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Glass.Api.Projeto.IGrupoModelo> ObterGruposModelo()
        {
            return Glass.Data.DAL.GrupoModeloDAO.Instance.GetOrdered()
                .Select(f => new GrupoModelo(f)).ToList<Glass.Api.Projeto.IGrupoModelo>();            
        }
    }
}
