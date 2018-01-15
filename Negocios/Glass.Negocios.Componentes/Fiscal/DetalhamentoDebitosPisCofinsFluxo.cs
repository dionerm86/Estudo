using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Fiscal.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio de débitos de PIS/Cofins.
    /// </summary>
    public class DetalhamentoDebitosPisCofinsFluxo : IDetalhamentoDebitosPisCofinsFluxo
    {
        /// <summary>
        /// Recupera os dados de débitos de PIS/Cofins.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.DetalhamentoDebitosPisCofins> ObtemDebitosPisCofins()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.DetalhamentoDebitosPisCofins>()
                .OrderBy("DataPagamento DESC")
                .ToVirtualResultLazy<Entidades.DetalhamentoDebitosPisCofins>();
        }

        /// <summary>
        /// Cria um item para cadastro.
        /// </summary>
        /// <returns></returns>
        public Entidades.DetalhamentoDebitosPisCofins CriarDebitoPisCofins()
        {
            return SourceContext.Instance.Create<Entidades.DetalhamentoDebitosPisCofins>();
        }

        /// <summary>
        /// Recupera um item de débitos de PIS/Cofins.
        /// </summary>
        /// <param name="IdDetalhamentoPisCofins"></param>
        /// <returns></returns>
        public Entidades.DetalhamentoDebitosPisCofins ObtemDebitoPisCofins(int IdDetalhamentoPisCofins)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.DetalhamentoDebitosPisCofins>()
                .Where("IdDetalhamentoPisCofins=?id")
                    .Add("?id", IdDetalhamentoPisCofins)
                .ProcessLazyResult<Entidades.DetalhamentoDebitosPisCofins>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do débito de PIS/Cofins.
        /// </summary>
        /// <param name="debitoPisCofins"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarDebitoPisCofins(Entidades.DetalhamentoDebitosPisCofins debitoPisCofins)
        {
            debitoPisCofins.Require("debitoPisCofins").NotNull();

            using (var session = SourceContext.Instance.CreateSession()) {
                var resultado = debitoPisCofins.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados de PIS/Cofins.
        /// </summary>
        /// <param name="debitoPisCofins"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarDebitoPisCofins(Entidades.DetalhamentoDebitosPisCofins debitoPisCofins)
        {
            debitoPisCofins.Require("debitoPisCofins").NotNull();

            using (var session = SourceContext.Instance.CreateSession()) {
                var resultado = debitoPisCofins.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }
    }
}
