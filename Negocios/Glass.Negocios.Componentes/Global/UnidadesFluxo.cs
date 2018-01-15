using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de unidades do sistema.
    /// </summary>
    public class UnidadesFluxo : IUnidadesFluxo, Entidades.IValidadorUnidadeMedida
    {
        #region Unidade Medida

        /// <summary>
        /// Pesquisa as unidades de medida.
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="descricao"></param>
        /// <returns></returns>
        public IList<Entidades.UnidadeMedida> PesquisarUnidadesMedida(string codigo, string descricao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.UnidadeMedida>()
                .OrderBy("Codigo");

            if (!string.IsNullOrEmpty(codigo))
                consulta.WhereClause
                    .And("Codigo LIKE ?codigo")
                    .Add("?codigo", string.Format("%{0}%", codigo));

            if (!string.IsNullOrEmpty(descricao))
                consulta.WhereClause
                    .And("Descricao LIKE ?descricao")
                    .Add("?descricao", string.Format("%{0}%", descricao));

            return consulta.ToVirtualResult<Entidades.UnidadeMedida>();
        }

        /// <summary>
        /// Recupera os descritores das unidades de medida do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemUnidadesMedida()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.UnidadeMedida>()
                .OrderBy("Codigo")
                .ProcessResultDescriptor<Entidades.UnidadeMedida>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados da unidade de medida.
        /// </summary>
        /// <param name="idUnidadeMedida"></param>
        /// <returns></returns>
        public Entidades.UnidadeMedida ObtemUnidadeMedida(int idUnidadeMedida)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.UnidadeMedida>()
                .Where("IdUnidadeMedida=?id")
                .Add("?id", idUnidadeMedida)
                .ProcessResult<Entidades.UnidadeMedida>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da unidade de medida.
        /// </summary>
        /// <param name="unidadeMedida"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarUnidadeMedida(Entidades.UnidadeMedida unidadeMedida)
        {
            unidadeMedida.Require("unidadeMedida").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = unidadeMedida.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da unidade de medida.
        /// </summary>
        /// <param name="unidadeMedida"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarUnidadeMedida(Entidades.UnidadeMedida unidadeMedida)
        {
            unidadeMedida.Require("unidadeMedida").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = unidadeMedida.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Membros de IValidadorUnidadeMedida

        /// <summary>
        /// Valida a existencia da unidade de medida.
        /// </summary>
        /// <param name="unidadeMedida"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorUnidadeMedida.ValidaExistencia
            (Entidades.UnidadeMedida unidadeMedida)
        {
            // Verifica se esta unidade de medida está sendo usada em algum produto
            if (SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>()
                    .Where("IdUnidadeMedida=?id OR IdUnidadeMedidaTrib=?id")
                    .Add("?id", unidadeMedida.IdUnidadeMedida)
                    .ExistsResult())
            {
                return new IMessageFormattable[]
                {
                    "Esta unidade de medida está sendo usada, portanto não pode ser excluída.".GetFormatter()
                };
            }

            return new IMessageFormattable[0];
        }

        /// <summary>
        /// Valida a atualização da unidade de medida.
        /// </summary>
        /// <param name="unidadeMedida"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorUnidadeMedida.ValidaAtualizacao
            (Entidades.UnidadeMedida unidadeMedida)
        {
            var mensagens = new List<IMessageFormattable>();
            var consultas = SourceContext.Instance.CreateMultiQuery();

            // Verifica se é uma nova unidade de medida
            if (!unidadeMedida.ExistsInStorage)
                // Verifica se já existe uma unidade de medida cadastrada com o código passado.
                consultas.Add(
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.UnidadeMedida>()
                        .Where("Codigo=?codigo").Add("?codigo", unidadeMedida.Codigo)
                        .Count(),
                        (s, q, r) =>
                        {
                            if (r.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                                mensagens.Add("Já existe uma unidade de medida cadastrada com este código.".GetFormatter());

                        });

            else
                consultas.Add(
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.UnidadeMedida>()
                        .Where("IdUnidadeMedida<>?id AND Codigo=?codigo")
                        .Add("?id", unidadeMedida.IdUnidadeMedida)
                        .Add("?codigo", unidadeMedida.Codigo)
                        .Count(),
                        (s, q, r) =>
                        {
                            if (r.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                                mensagens.Add("Já existe uma unidade de medida cadastrada com este código.".GetFormatter());
                        });

            consultas.Execute();

            return mensagens.ToArray();
        }

        #endregion
    }
}
