using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio dos cartões.
    /// </summary>
    public class CartoesFluxo : ICartoesFluxo
    {
        /// <summary>
        /// Cria novo tipo cartão de credito
        /// </summary>
        /// <returns></returns>
        public Entidades.TipoCartaoCredito CriarTipoCartaoCredito()
        {
            return SourceContext.Instance.Create<Entidades.TipoCartaoCredito>();
        }

        /// <summary>
        /// Salva o tipo de cartão de crédito
        /// </summary>
        public Colosoft.Business.SaveResult SalvarTipoCartaoCredito(Entidades.TipoCartaoCredito tipoCartaoCredito)
        {
            tipoCartaoCredito.Require("tipoCartaoCredito").NotNull();

            // Se for um novo Tipo de Cartão, cria e insere os planos de contas
            if (!tipoCartaoCredito.ExistsInStorage)
            {
                var retorno = SalvarTipoCartaoComPlanoContas(tipoCartaoCredito);
                if (!retorno)
                    return new Colosoft.Business.SaveResult(false, retorno.Message);

                // Salva os dados do Tipo cartão com os novos planos no banco.
                using (var session = SourceContext.Instance.CreateSession())
                {
                    var resultado = retorno.Salvar(session);

                    if (!resultado)
                        return resultado;

                    return session.Execute(false).ToSaveResult();
                }
            }

            // Atualiza os dados do banco.
            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = tipoCartaoCredito.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Salva o tipo de cartão de crédito com os planos de contas padrões
        /// </summary>
        private SalvarTipoCartaoComPlanoContasResultado SalvarTipoCartaoComPlanoContas(Entidades.TipoCartaoCredito tipoCartaoCredito)
        {
            var planoContasFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IPlanoContasFluxo>();

            // Recupera ou Cria os planos de contas.
            var planoContaDevolucaoPagto = planoContasFluxo.RecuperaOuCriaPlanoContas(49, "Devolução de Pagamento Cartão " +
                Data.DAL.OperadoraCartaoDAO.Instance.ObterDescricaoOperadora((uint)tipoCartaoCredito.Operadora) + " " + tipoCartaoCredito.Tipo.ToString());
            var planoContaEntrada = planoContasFluxo.RecuperaOuCriaPlanoContas(51, "Entrada Cartão " +
                Data.DAL.OperadoraCartaoDAO.Instance.ObterDescricaoOperadora((uint)tipoCartaoCredito.Operadora) + " " + tipoCartaoCredito.Tipo.ToString());
            var planoContaEstorno = planoContasFluxo.RecuperaOuCriaPlanoContas(49, "Estorno de Cartão " +
                Data.DAL.OperadoraCartaoDAO.Instance.ObterDescricaoOperadora((uint)tipoCartaoCredito.Operadora) + " " + tipoCartaoCredito.Tipo.ToString());
            var planoContaEstornoChequeDev = planoContasFluxo.RecuperaOuCriaPlanoContas(49, "Estorno Cheque Devolvido Cartão " +
                Data.DAL.OperadoraCartaoDAO.Instance.ObterDescricaoOperadora((uint)tipoCartaoCredito.Operadora) + " " + tipoCartaoCredito.Tipo.ToString());
            var planoContaEstornoDevolucaoPagto = planoContasFluxo.RecuperaOuCriaPlanoContas(49, "Estorno Devolução de Pagamento Cartão " +
                Data.DAL.OperadoraCartaoDAO.Instance.ObterDescricaoOperadora((uint)tipoCartaoCredito.Operadora) + " " + tipoCartaoCredito.Tipo.ToString());
            var planoContaEstornoEntrada = planoContasFluxo.RecuperaOuCriaPlanoContas(49, "Estorno Entrada Cartão " +
                Data.DAL.OperadoraCartaoDAO.Instance.ObterDescricaoOperadora((uint)tipoCartaoCredito.Operadora) + " " + tipoCartaoCredito.Tipo.ToString());
            var planoContaEstornoRecPrazo = planoContasFluxo.RecuperaOuCriaPlanoContas(49, "Estorno Rec. Prazo Cartão " +
                Data.DAL.OperadoraCartaoDAO.Instance.ObterDescricaoOperadora((uint)tipoCartaoCredito.Operadora) + " " + tipoCartaoCredito.Tipo.ToString());
            var planoContaFunc = planoContasFluxo.RecuperaOuCriaPlanoContas(7, "Receb. de Funcionário Cartão " +
                Data.DAL.OperadoraCartaoDAO.Instance.ObterDescricaoOperadora((uint)tipoCartaoCredito.Operadora) + " " + tipoCartaoCredito.Tipo.ToString());
            var planoContaRecChequeDev = planoContasFluxo.RecuperaOuCriaPlanoContas(48, "Recec. Cheque Devolvido Cartão " +
                Data.DAL.OperadoraCartaoDAO.Instance.ObterDescricaoOperadora((uint)tipoCartaoCredito.Operadora) + " " + tipoCartaoCredito.Tipo.ToString());
            var planoContaRecPrazo = planoContasFluxo.RecuperaOuCriaPlanoContas(50, "Recec. Prazo com Cartão " +
                Data.DAL.OperadoraCartaoDAO.Instance.ObterDescricaoOperadora((uint)tipoCartaoCredito.Operadora) + " " + tipoCartaoCredito.Tipo.ToString());
            var planoContaVista = planoContasFluxo.RecuperaOuCriaPlanoContas(51, "Receb. à Vista Cartão " +
                Data.DAL.OperadoraCartaoDAO.Instance.ObterDescricaoOperadora((uint)tipoCartaoCredito.Operadora) + " " + tipoCartaoCredito.Tipo.ToString());

            // Associa os IdsContas.
            tipoCartaoCredito.IdContaDevolucaoPagto = planoContaDevolucaoPagto.IdConta;
            tipoCartaoCredito.IdContaEntrada = planoContaEntrada.IdConta;
            tipoCartaoCredito.IdContaEstorno = planoContaEstorno.IdConta;
            tipoCartaoCredito.IdContaEstornoChequeDev = planoContaEstornoChequeDev.IdConta;
            tipoCartaoCredito.IdContaEstornoDevolucaoPagto = planoContaEstornoDevolucaoPagto.IdConta;
            tipoCartaoCredito.IdContaEstornoEntrada = planoContaEstornoEntrada.IdConta;
            tipoCartaoCredito.IdContaEstornoRecPrazo = planoContaEstornoRecPrazo.IdConta;
            tipoCartaoCredito.IdContaFunc = planoContaFunc.IdConta;
            tipoCartaoCredito.IdContaRecChequeDev = planoContaRecChequeDev.IdConta;
            tipoCartaoCredito.IdContaRecPrazo = planoContaRecPrazo.IdConta;
            tipoCartaoCredito.IdContaVista = planoContaVista.IdConta;

            var planosContas = new List<Entidades.PlanoContas>();
            planosContas.Add(planoContaDevolucaoPagto);
            planosContas.Add(planoContaEntrada);
            planosContas.Add(planoContaEstorno);
            planosContas.Add(planoContaEstornoChequeDev);
            planosContas.Add(planoContaEstornoDevolucaoPagto);
            planosContas.Add(planoContaEstornoEntrada);
            planosContas.Add(planoContaEstornoRecPrazo);
            planosContas.Add(planoContaFunc);
            planosContas.Add(planoContaRecChequeDev);
            planosContas.Add(planoContaRecPrazo);
            planosContas.Add(planoContaVista);

            // Define o IdContaGrupo para os Planos novos.
            planosContas.ForEach(pc =>
            {
                if (!pc.ExistsInStorage)
                {
                    var idContaGrupo = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.PlanoContas>()
                    .Select("MAX(IdContaGrupo), COUNT(*)")
                    .Where("IdGrupo=?idGrupo")
                    .Add("?idGrupo", pc.IdGrupo)
                    .Execute()
                    .Select(f => f.IsDBNull(0) ? 0 : f.GetInt32(0))
                    .FirstOrDefault() + 1;

                    pc.IdContaGrupo = idContaGrupo;
                }
            });

            return new SalvarTipoCartaoComPlanoContasResultado(tipoCartaoCredito, planoContaDevolucaoPagto, planoContaEntrada, planoContaEstorno, planoContaEstornoChequeDev,
                planoContaEstornoDevolucaoPagto, planoContaEstornoEntrada, planoContaEstornoRecPrazo, planoContaFunc, planoContaRecChequeDev, planoContaRecPrazo, planoContaVista);
        }

        public bool VerificarTipoCartaoCreditoUso(int idTipoCartao)
        {
            var union1 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.AssocContaBanco>("acb")
                .Where("acb.IdTipoCartao=?idTipoCartao");

            var union3 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.Pedido>("p")
                .Where("p.IdTipoCartao=?idTipoCartao");

            var union4 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.PagtoAcerto>("pa")
                .Where("pa.IdTipoCartao=?idTipoCartao");

            var union5 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.PagtoAcertoCheque>("pac")
                .Where("pac.IdTipoCartao=?idTipoCartao");

            var union6 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.PagtoAntecipacaoFornecedor>("paf")
                .Where("paf.IdTipoCartao=?idTipoCartao");

            var union7 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.PagtoContasReceber>("pcr")
                .Where("pcr.IdTipoCartao=?idTipoCartao");

            var union8 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.PagtoCreditoFornecedor>("pcf")
                .Where("pcf.IdTipoCartao=?idTipoCartao");

            var union9 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.PagtoLiberarPedido>("plp")
                .Where("plp.IdTipoCartao=?idTipoCartao");

            var union10 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.PagtoObra>("po")
                .Where("po.IdTipoCartao=?idTipoCartao");

            var union11 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.PagtoPagto>("pp")
                .Where("pp.IdTipoCartao=?idTipoCartao");

            var union12 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.PagtoSinal>("ps")
                .Where("ps.IdTipoCartao=?idTipoCartao");

            var union13 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.PagtoSinalCompra>("psc")
                .Where("psc.IdTipoCartao=?idTipoCartao");

            var union14 = SourceContext.Instance.CreateQuery()
                .Select("IdTipoCartao")
                .From<Data.Model.PagtoTrocaDevolucao>("ptd")
                .Where("ptd.IdTipoCartao=?idTipoCartao");

            var consultaPrincipal = SourceContext.Instance.CreateQuery()
                .From(union1.UnionAll(union3)
                .UnionAll(union4)
                .UnionAll(union5)
                .UnionAll(union6)
                .UnionAll(union7)
                .UnionAll(union8)
                .UnionAll(union9)
                .UnionAll(union10)
                .UnionAll(union11)
                .UnionAll(union12)
                .UnionAll(union13)
                .UnionAll(union14), "temp")
                .Where("temp.IdTipoCartao = ?idTipoCartao")
                .Add("?idTipoCartao", idTipoCartao);

            return consultaPrincipal.ExistsResult();

        }
    }
}
