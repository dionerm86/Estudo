using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Global
{
    /// <summary>
    /// Implementação da entidade cor de ferragem.
    /// </summary>
    public class CorFerragem : Api.Global.ICorFerragem
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
        public CorFerragem(Data.Model.CorFerragem model)
        {
            Id = model.IdCorFerragem;
            Descricao = model.Descricao;
        }
    }

    /// <summary>
    /// Implementação do fluxo de negocio da cor da ferragem.
    /// </summary>
    public class FerragemFluxo : Api.Global.IFerragemFluxo
    {

        /// <summary>
        /// Recupera as cores dos ferragens.
        /// </summary>
        /// <returns></returns>
        public IList<Api.Global.ICorFerragem> ObterCores()
        {
            var retorno = new List<Glass.Api.Global.ICorFerragem>();

            retorno.AddRange(Glass.Data.DAL.CorFerragemDAO.Instance.GetAll().Select(f => new CorFerragem(f)));

            return retorno;
        }
    }
}
