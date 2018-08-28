using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;
using Glass.Data.DAL.CTe;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class RegraNaturezaOperacaoDAO : BaseCadastroDAO<RegraNaturezaOperacao, RegraNaturezaOperacaoDAO>
    {
        //private RegraNaturezaOperacaoDAO() { }

        public enum TipoBuscaNaturezaOperacao
        {
            NotaFiscal,
            ProdutoNotaFiscal,
            ConhecimentoTransporte
        }

        #region Busca padrão

        private string Sql(uint idRegraNaturezaOperacao, uint idLoja, uint idTipoCliente, uint idGrupoProd, uint idSubgrupoProd, 
            uint idCorVidro, uint idCorFerragem, uint idCorAluminio, float espessura, uint idNaturezaOperacao, bool selecionar, out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select ");

            sql.Append(selecionar ? "rno.*" : "count(*)");

            sql.AppendFormat(@"
                from regra_natureza_operacao rno
                where 1 {0}", FILTRO_ADICIONAL);

            StringBuilder fa = new StringBuilder();

            if (idRegraNaturezaOperacao > 0)
                fa.AppendFormat(" and rno.idRegraNaturezaOperacao={0}", idRegraNaturezaOperacao);

            if (idLoja > 0)
                fa.AppendFormat(" and rno.idLoja={0}", idLoja);

            if (idTipoCliente > 0)
                fa.AppendFormat(" and rno.idTipoCliente={0}", idTipoCliente);

            if (idGrupoProd > 0)
                fa.AppendFormat(" and rno.idGrupoProd={0}", idGrupoProd);

            if (idSubgrupoProd > 0)
                fa.AppendFormat(" and rno.idSubgrupoProd={0}", idSubgrupoProd);

            if (idCorVidro > 0)
                fa.AppendFormat(" and rno.idCorVidro={0}", idCorVidro);

            if (idCorFerragem > 0)
                fa.AppendFormat(" and rno.idCorFerragem={0}", idCorFerragem);

            if (idCorAluminio > 0)
                fa.AppendFormat(" and rno.idCorAluminio={0}", idCorAluminio);

            if (espessura > 0)
                fa.AppendFormat(" and rno.espessura={0}", espessura.ToString().Replace(",", "."));

            if (idNaturezaOperacao > 0)
                fa.AppendFormat(@" and (rno.idNaturezaOperacaoProdIntra={0} or rno.idNaturezaOperacaoProdInter={0}
                    or rno.idNaturezaOperacaoRevIntra={0} or rno.idNaturezaOperacaoRevInter={0}
                    or rno.idNaturezaOperacaoProdStIntra={0} or rno.idNaturezaOperacaoProdStInter={0}
                    or rno.idNaturezaOperacaoRevStIntra={0} or rno.idNaturezaOperacaoRevStInter={0})", idNaturezaOperacao);

            filtroAdicional = fa.ToString();
            return sql.ToString();
        }

        public IList<RegraNaturezaOperacao> ObtemLista(uint idLoja, uint idTipoCliente, uint idGrupoProd, uint idSubgrupoProd,
            uint idCorVidro, uint idCorFerragem, uint idCorAluminio, float espessura, uint idNaturezaOperacao, 
            string sortExpression, int startRow, int pageSize)
        {
            if (String.IsNullOrEmpty(sortExpression))
                sortExpression = "rno.idLoja, rno.idTipoCliente, rno.idGrupoProd, rno.idSubgrupoProd";

            string filtro;
            return LoadDataWithSortExpression(Sql(0, idLoja, idTipoCliente, idGrupoProd, idSubgrupoProd, idCorVidro, idCorFerragem,
                idCorAluminio, espessura, idNaturezaOperacao, true, out filtro), sortExpression, startRow, pageSize, false, filtro);
        }

        public int ObtemNumeroRegistros(uint idLoja, uint idTipoCliente, uint idGrupoProd, uint idSubgrupoProd,
            uint idCorVidro, uint idCorFerragem, uint idCorAluminio, float espessura, uint idNaturezaOperacao)
        {
            string filtro;
            return GetCountWithInfoPaging(Sql(0, idLoja, idTipoCliente, idGrupoProd, idSubgrupoProd, idCorVidro, idCorFerragem,
                idCorAluminio, espessura, idNaturezaOperacao, true, out filtro), false, filtro);
        }

        public RegraNaturezaOperacao ObtemItem(uint idRegraNaturezaOperacao)
        {
            string filtro;
            return objPersistence.LoadOneData(Sql(idRegraNaturezaOperacao, 0, 0, 0, 0, 0, 0, 0, 0, 0, true, 
                out filtro).Replace(FILTRO_ADICIONAL, filtro));
        }

        #endregion

        #region Obtém dados da regra de natureza de operação

        /// <summary>
        /// Obtém a descrição da regra de natureza de operação.
        /// </summary>
        public string ObtemDescricao(uint idRegraNaturezaOperacao)
        {
            return ObtemDescricao(null, idRegraNaturezaOperacao);
        }

        /// <summary>
        /// Obtém a descrição da regra de natureza de operação.
        /// </summary>
        public string ObtemDescricao(GDA.GDASession session, uint idRegraNaturezaOperacao)
        {
            StringBuilder desc = new StringBuilder();
            string where = "idRegraNaturezaOperacao=" + idRegraNaturezaOperacao;

            uint? idLoja = ObtemValorCampo<uint?>(session, "idLoja", where);
            uint? idTipoCliente = ObtemValorCampo<uint?>(session, "idTipoCliente", where);
            uint? idGrupoProd = ObtemValorCampo<uint?>(session, "idGrupoProd", where);
            uint? idSubgrupoProd = ObtemValorCampo<uint?>(session, "idSubgrupoProd", where);

            if (idLoja > 0)
                desc.AppendFormat("Loja: {0} / ", LojaDAO.Instance.GetNome(session, idLoja.Value));

            if (idTipoCliente > 0)
                desc.AppendFormat("Tipo Cliente: {0} / ", TipoClienteDAO.Instance.GetNome(session, idTipoCliente.Value));

            if (idGrupoProd > 0)
                desc.AppendFormat("Grupo: {0} / ", GrupoProdDAO.Instance.GetDescricao(session, (int)idGrupoProd.Value));

            if (idSubgrupoProd > 0)
                desc.AppendFormat("Subgrupo: {0} / ", SubgrupoProdDAO.Instance.GetDescricao(session, (int)idSubgrupoProd.Value));

            return desc.ToString().TrimEnd('/', ' ');
        }

        #endregion

        #region Busca a regra de natureza de operação

        private RegraNaturezaOperacao BuscaRegra(GDA.GDASession session, uint? idNf, NotaFiscal.TipoDoc? tipoDocumentoNotaFiscal, uint? idLoja, uint? idTipoCliente,
            uint? idGrupoProd, uint? idSubgrupoProd, uint? idCorVidro, uint? idCorAluminio, uint? idCorFerragem, float? espessura, bool gerandoNfSaida, string ufDestino)
        {
            // Só busca a regra de natureza de operação para notas fiscais de saída
            if (!gerandoNfSaida &&
                ((tipoDocumentoNotaFiscal == null &&
                (idNf.GetValueOrDefault() == 0 || NotaFiscalDAO.Instance.GetTipoDocumento(session, idNf.GetValueOrDefault()) != (int)NotaFiscal.TipoDoc.Saída)) ||
                tipoDocumentoNotaFiscal != NotaFiscal.TipoDoc.Saída))
                return null;

            // Faz a ordenação decrescente para considerar o item que tiver o valor preenchido
            // nos campos ser retornado (no lugar de um item mais geral - com menos campos preenchidos)
            StringBuilder sql = new StringBuilder(@"
                select * from regra_natureza_operacao
                where (UfDest LIKE ?ufDestino OR UfDest IS NULL) {0}
                order by espessura desc, idCorFerragem desc, idCorAluminio desc, idCorVidro desc, 
                    idSubgrupoProd desc, idGrupoProd desc, idTipoCliente desc, idLoja desc
                limit 1");

            // Considera os filtros para busca
            StringBuilder where = new StringBuilder();

            if (idLoja > 0)
                where.AppendFormat(" and coalesce(idLoja, {0})={0}", idLoja);

            if (idTipoCliente > 0)
                where.AppendFormat(" and coalesce(idTipoCliente, {0})={0}", idTipoCliente);

            if (idGrupoProd > 0)
                where.AppendFormat(" and coalesce(idGrupoProd, {0})={0}", idGrupoProd);

            if (idSubgrupoProd > 0)
                where.AppendFormat(" and coalesce(idSubgrupoProd, {0})={0}", idSubgrupoProd);
            // Chamado 13565.
            // Havia uma regra cadastrada no sistema com e sem subgrupo, a regra sem subgrupo estava considerando
            // a regra com subgrupo, incluímos esta condição no sql (caso não tenha código do subgrupo) para que
            // não seja buscada a regra com subgrupo.
            else
                where.AppendFormat(" AND COALESCE(idSubgrupoProd, 0)=0");

            if (idCorVidro > 0)
                where.AppendFormat(" and coalesce(idCorVidro, {0})={0}", idCorVidro);

            if (idCorAluminio > 0)
                where.AppendFormat(" and coalesce(idCorAluminio, {0})={0}", idCorAluminio);

            if (idCorFerragem > 0)
                where.AppendFormat(" and coalesce(idCorFerragem, {0})={0}", idCorFerragem);

            if (espessura > 0)
                where.AppendFormat(" and coalesce(espessura, {0})={0}", espessura.ToString().Replace(",", "."));

            // Retorna apenas o primeiro item do retorno da consulta, se houver
            var itens = objPersistence.LoadData(session, string.Format(sql.ToString(), where.ToString()), new GDAParameter("?ufDestino", "%" + ufDestino + "%")).ToList();
            return itens.Count > 0 ? itens[0] : null;
        }

        public uint? BuscaNaturezaOperacao(uint idItem, TipoBuscaNaturezaOperacao tipoBusca)
        {
            // Restringe a busca apenas para produtos de nota fiscal
            // (o método já funciona com os outros tipos também, mas
            // ainda não será liberado para funcionamento por conta de
            // regras de negócio para atribuição da regra à NF-e ou CT-e)
            if (tipoBusca != TipoBuscaNaturezaOperacao.ProdutoNotaFiscal)
                return null;

            uint idNf = 0;
            uint? idLoja, idCliente;
            int? idProd = null;
            idLoja = idCliente = null;

            #region Busca os dados para recuperar a regra

            switch (tipoBusca)
            {
                case TipoBuscaNaturezaOperacao.NotaFiscal:
                    idNf = idItem;
                    idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(idItem);
                    idCliente = NotaFiscalDAO.Instance.ObtemIdCliente(idItem);
                    break;

                case TipoBuscaNaturezaOperacao.ProdutoNotaFiscal:
                    idNf = ProdutosNfDAO.Instance.ObtemIdNf(idItem);
                    idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(idNf);
                    idCliente = NotaFiscalDAO.Instance.ObtemIdCliente(idNf);
                    idProd = ProdutosNfDAO.Instance.ObtemValorCampo<int>("idProd", "idProdNf=" + idItem);
                    break;

                case TipoBuscaNaturezaOperacao.ConhecimentoTransporte:
                    idLoja = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(idItem, (int)ParticipanteCte.TipoParticipanteEnum.Emitente).IdLoja;
                    idCliente = ParticipanteCteDAO.Instance.GetParticipanteByIdCte(idItem).FirstOrDefault(x => x.IdCliente > 0).IdCliente;
                    break;
            }

            #endregion

            return BuscaNaturezaOperacao(idNf, idLoja, idCliente, idProd);
        }

        public uint? BuscaNaturezaOperacao(uint idNf, uint? idLoja, uint? idCliente, int? idProd)
        {
            return BuscaNaturezaOperacao(null, idNf, idLoja, idCliente, idProd);
        }

        public uint? BuscaNaturezaOperacao(GDA.GDASession session, uint? idLoja, uint? idCliente, int? idProd)
        {
            return BuscaNaturezaOperacao(session, null, null, idLoja, idCliente, idProd, true);
        }

        public uint? BuscaNaturezaOperacao(GDA.GDASession session, uint idNf, uint? idLoja, uint? idCliente, int? idProd)
        {
            return BuscaNaturezaOperacao(session, idNf, null, idLoja, idCliente, idProd, false);
        }

        public uint? BuscaNaturezaOperacao(NotaFiscal.TipoDoc tipoDocumentoNotaFiscal, uint? idLoja, uint? idCliente, int? idProd)
        {
            return BuscaNaturezaOperacao(null, tipoDocumentoNotaFiscal, idLoja, idCliente, idProd);
        }

        public uint? BuscaNaturezaOperacao(GDA.GDASession session, NotaFiscal.TipoDoc tipoDocumentoNotaFiscal, uint? idLoja, uint? idCliente, int? idProd)
        {
            return BuscaNaturezaOperacao(session, null, tipoDocumentoNotaFiscal, idLoja, idCliente, idProd, false);
        }

        public uint? BuscaNaturezaOperacao(uint idNf, NotaFiscal.TipoDoc tipoDocumentoNotaFiscal, uint idLoja, uint? idCliente,
            int? idProd)
        {
            return BuscaNaturezaOperacao(null, idNf, tipoDocumentoNotaFiscal, idLoja, idCliente, idProd);
        }

        public uint? BuscaNaturezaOperacao(GDA.GDASession session, uint idNf, NotaFiscal.TipoDoc tipoDocumentoNotaFiscal, uint idLoja, uint? idCliente,
            int? idProd)
        {
            return BuscaNaturezaOperacao(session, idNf, tipoDocumentoNotaFiscal, idLoja, idCliente, idProd, false);
        }

        public uint? BuscaNaturezaOperacao(uint? idNf, NotaFiscal.TipoDoc? tipoDocumentoNotaFiscal, uint? idLoja, uint? idCliente,
            int? idProd, bool gerandoNfSaida)
        {
            return BuscaNaturezaOperacao(null, idNf, tipoDocumentoNotaFiscal, idLoja, idCliente, idProd, gerandoNfSaida);
        }

        public uint? BuscaNaturezaOperacao(GDA.GDASession session, uint? idNf, NotaFiscal.TipoDoc? tipoDocumentoNotaFiscal, uint? idLoja, uint? idCliente,
            int? idProd, bool gerandoNfSaida)
        {
            // Preenche as variáveis que são utilizadas nos métodos abaixo
            uint? idTipoCliente = idCliente > 0 ?
                ClienteDAO.Instance.ObtemValorCampo<uint?>(session, "idTipoCliente", "id_Cli=" + idCliente) :
                null;

            uint? idGrupoProd = idProd > 0 ?
                (uint?)ProdutoDAO.Instance.ObtemIdGrupoProd(session, idProd.Value) :
                null;

            uint? idSubgrupoProd = (uint?)(idProd > 0 ?
                ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, idProd.Value) :
                null);

            uint? idCorVidro = (uint?)(idProd > 0 ?
                ProdutoDAO.Instance.ObtemIdCorVidro(session, idProd.Value) :
                null);

            uint? idCorAluminio = (idProd > 0 ?
                ProdutoDAO.Instance.ObtemIdCorAluminio(session, idProd.Value) :
                null);

            uint? idCorFerragem = (idProd > 0 ?
                ProdutoDAO.Instance.ObtemIdCorFerragem(session, idProd.Value) :
                null);

            float? espessura = idProd > 0 ?
                ProdutoDAO.Instance.ObtemEspessura(session, idProd.Value) :
                (float?)null;

            var dados = MvaProdutoUfDAO.Instance.ObterDadosParaBuscar(session, idLoja.Value, null, idCliente, true);

            // Busca a regra de natureza de operação que será aplicada
            var regra = BuscaRegra(session, idNf, tipoDocumentoNotaFiscal, idLoja, idTipoCliente, idGrupoProd, idSubgrupoProd, idCorVidro,
                idCorAluminio, idCorFerragem, espessura, gerandoNfSaida, dados.UfDestino);

            // Indica que a regra não foi encontrada
            if (regra == null)
                return null;
            
            bool prodRevenda = !ProdutoDAO.Instance.IsProdutoVenda(session, idProd.Value);
            var mva = idProd > 0 ? MvaProdutoUfDAO.Instance.ObterMvaPorProduto(session, idProd.Value, idLoja.Value, dados.UfOrigem, 
                dados.UfDestino, dados.Simples, dados.TipoCliente, true) : 0;

            // O produto consulta as regras de natureza de operação de ST apenas se
            // o MVA relativo à movimentação entre as UF 
            bool possuiSt = mva > 0;

            // Verifica se as UFs são iguais (origem e destino)
            if (string.Equals(dados.UfOrigem, dados.UfDestino, StringComparison.InvariantCultureIgnoreCase))
            {
                return (uint?)(!possuiSt ?
                    (prodRevenda ? regra.IdNaturezaOperacaoRevIntra : regra.IdNaturezaOperacaoProdIntra) :
                    (prodRevenda ? regra.IdNaturezaOperacaoRevStIntra : regra.IdNaturezaOperacaoProdStIntra));
            }
            else
            {
                return (uint?)(!possuiSt ?
                    (prodRevenda ? regra.IdNaturezaOperacaoRevInter : regra.IdNaturezaOperacaoProdInter) :
                    (prodRevenda ? regra.IdNaturezaOperacaoRevStInter : regra.IdNaturezaOperacaoProdStInter));
            }
        }

        #endregion

        #region Métodos sobrescritos

        public override int Update(RegraNaturezaOperacao objUpdate)
        {
            LogAlteracaoDAO.Instance.LogRegraNaturezaOperacao(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(RegraNaturezaOperacao objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdRegraNaturezaOperacao);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return Excluir(Key, String.Empty, false);
        }

        public int Excluir(uint idRegraNaturezaOperacao, string motivo, bool manual)
        {
            var item = GetElementByPrimaryKey(idRegraNaturezaOperacao);

            LogCancelamentoDAO.Instance.LogRegraNaturezaOperacao(item, motivo, manual);
            return base.DeleteByPrimaryKey(idRegraNaturezaOperacao);
        }

        #endregion
    }
}
