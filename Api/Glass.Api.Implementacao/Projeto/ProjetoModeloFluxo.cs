using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    /// <summary>
    /// Entidade de negocio projeto modelo.
    /// </summary>
    public class ProjetoModelo : Glass.Api.Projeto.IProjetoModelo
    {
        #region Propriedades

        /// <summary>
        /// Identificador.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Código.
        /// </summary>
        public string Codigo { get; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao { get; }

        /// <summary>
        /// Caminho da imagem do projeto modelo.
        /// </summary>
        public string ModeloPath { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da entidade de negocio projeto modelo.
        /// </summary>
        /// <param name="modelo"></param>
        public ProjetoModelo(Glass.Data.Model.ProjetoModelo modelo)
        {
            Id = modelo.IdProjetoModelo;
            Codigo = modelo.Codigo;
            Descricao = modelo.Descricao;
            ModeloPath = modelo.ModeloPath.Replace("../../", ServiceLocator.Current.GetInstance<Api.IConfiguracao>().EnderecoServicoImagem);
        }

        #endregion
    }

    /// <summary>
    /// Fluxo de negocio do projeto modelo.
    /// </summary>
    public class ProjetoModeloFluxo : Glass.Api.Projeto.IProjetoModeloFluxo
    {
        /// <summary>
        /// Recupera os projetos modelo mais utilizados.
        /// </summary>
        /// <param name="idCliente">Identificador do cliente.</param>
        /// <param name="qtd">Quantidade de modelos.</param>
        /// <returns></returns>
        public IList<Glass.Api.Projeto.IProjetoModelo> ObterMaisUsados(int idCliente = 0, int qtd = 21)
        {
            // TODO: utilizar método com cliente.
            return Glass.Data.DAL.ProjetoModeloDAO.Instance.ObtemMaisUsados(21)
                .Select(f => new ProjetoModelo(f)).ToList<Glass.Api.Projeto.IProjetoModelo>();
        }

        /// <summary>
        /// Recupera o projeto modelo.
        /// </summary>
        /// <param name="idGrupoModelo">Identificador do grupo modelo.</param>
        /// <param name="codigo">Código do projeto modelo.</param>
        /// <returns></returns>
        public IList<Glass.Api.Projeto.IProjetoModelo> ObterProjetoModelo(int idGrupoModelo, string codigo)
        {
            return Glass.Data.DAL.ProjetoModeloDAO.Instance.GetList(codigo, null, (uint)idGrupoModelo)
                .Select(f => new ProjetoModelo(f)).ToList<Glass.Api.Projeto.IProjetoModelo>();
        }
    }
}
