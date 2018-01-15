using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Linq;

namespace Glass.Data.EFD
{
    public sealed class DataSourcesEFD : Glass.Pool.Singleton<DataSourcesEFD>
    {
        private DataSourcesEFD() { }

        #region Métodos de Suporte

        /// <summary>
        /// Recupera uma lista de GenericModel a partir dos itens de um enum.
        /// </summary>
        /// <param name="tipoEnum">O tipo do enum que será usado para retornar os dados.</param>
        /// <param name="funcaoDescricao">A função que retorna a descrição do enum. Pode ser null para retornar o próprio texto do enum.</param>
        /// <param name="ordenar">O retorno será ordenado pela descrição?</param>
        /// <returns></returns>
        public List<GenericModel> GetFromEnum(Type tipoEnum, Delegate funcaoDescricao, bool ordenar)
        {
            // Lista de retorno
            List<GenericModel> retorno = new List<GenericModel>();

            // Recupera os tipos do valor do enum e do parâmetro da função/string (se a função for null)
            Type tipoValorEnum = Enum.GetUnderlyingType(tipoEnum);
            Type tipoParametro = funcaoDescricao != null ? funcaoDescricao.Method.GetParameters()[0].ParameterType : typeof(string);

            if (funcaoDescricao != null && Nullable.GetUnderlyingType(tipoParametro) != null)
                tipoParametro = Nullable.GetUnderlyingType(tipoParametro);

            // Recupera os valores do enum
            foreach (object n in Enum.GetValues(tipoEnum))
            {
                // Converte o valor recuperado para o tipo base da enum
                object v = Conversoes.ConverteValor(tipoValorEnum, n);

                // Converte o valor para o tipo do parâmetro, se a função existir
                object p = funcaoDescricao != null ? Conversoes.ConverteValor(tipoParametro, v) : null;

                // Adiciona o valor com sua respectiva descrição à lista
                retorno.Add(new GenericModel(Conversoes.ConverteValor<uint>(v), funcaoDescricao != null ?
                    funcaoDescricao.DynamicInvoke(p).ToString() : n.ToString()));
            }

            // Ordena a lista de retorno, se desejado
            if (ordenar)
            {
                retorno.Sort(new Comparison<GenericModel>(
                    delegate(GenericModel x, GenericModel y)
                    {
                        return x.Descr.CompareTo(y.Descr);
                    }
                ));
            }

            // Retorna a lista
            return retorno;
        }

        #endregion

        #region Tipo do centro de custo

        public GenericModel[] GetTipoCentroCusto(uint idLoja)
        {
            List<uint?> permitidos = new List<uint?>();
            if (idLoja > 0)
            {
                int? tipo = LojaDAO.Instance.BuscaTipoLoja(idLoja);
                if (tipo != null)
                {
                    if (tipo == 1)
                        permitidos.AddRange(new uint?[] { 1, 2 });
                    else
                        permitidos.AddRange(new uint?[] { 3, 4, 5 });
                }
            }

            Converter<int, string> d = new Converter<int, string>(GetDescrTipoCentroCusto);
            List<GenericModel> retorno = GetFromEnum(typeof(Sync.Fiscal.Enumeracao.CentroCusto.Codigo), d, true);

            // Remove os tipos não permitidos
            for (int i = retorno.Count - 1; i >= 0; i--)
            {
                if (permitidos.Count > 0 && permitidos.Contains(retorno[i].Id))
                    continue;

                retorno.RemoveAt(i);
            }

            return retorno.ToArray();
        }

        public string GetDescrTipoCentroCusto(int codigoTipo)
        {
            return Colosoft.Translator.Translate((Sync.Fiscal.Enumeracao.CentroCusto.Codigo)codigoTipo).Format();
        }

        #endregion

        #region Tipos de bens do ativo imobilizado

        public GenericModel[] GetTiposBemAtivoImobilizado()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoBemAtivoImobilizado);
            return GetFromEnum(typeof(BemAtivoImobilizado.TipoEnum), d, true).ToArray();
        }

        public string GetDescrTipoBemAtivoImobilizado(int tipo)
        {
            switch (tipo)
            {
                case (int)BemAtivoImobilizado.TipoEnum.Bem: return "Bem";
                case (int)BemAtivoImobilizado.TipoEnum.Componente: return "Componente";
                default: return String.Empty;
            }
        }

        public enum GrupoBemAtivoImobEnum
        {
            EdificacoesEBenfeitoriasImoveisProprios = 1,
            EdificacoesEBenfeitoriasImoveisTerceiros,
            Instalacoes,
            Maquinas,
            Equipamentos,
            Veiculos,
            Outros = 99
        }

        public GenericModel[] GetGruposBemAtivoImobilizado()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrGrupoBemAtivoImobilizado);
            return GetFromEnum(typeof(GrupoBemAtivoImobEnum), d, false).ToArray();
        }

        public string GetDescrGrupoBemAtivoImobilizado(int grupoBemAtivoImob)
        {
            switch (grupoBemAtivoImob)
            {
                case (int)GrupoBemAtivoImobEnum.EdificacoesEBenfeitoriasImoveisProprios: return "Edificações e benfeitorias em imóveis próprios";
                case (int)GrupoBemAtivoImobEnum.EdificacoesEBenfeitoriasImoveisTerceiros: return "Edificações e benfeitorias em imóveis de terceiros";
                case (int)GrupoBemAtivoImobEnum.Instalacoes: return "Instalações";
                case (int)GrupoBemAtivoImobEnum.Maquinas: return "Máquinas";
                case (int)GrupoBemAtivoImobEnum.Equipamentos: return "Equipamentos";
                case (int)GrupoBemAtivoImobEnum.Veiculos: return "Veículos";
                case (int)GrupoBemAtivoImobEnum.Outros: return "Outros";
                default: return String.Empty;
            }
        }

        public enum SituacaoBemAtivoImobEnum
        {
            Ativa = 1,
            Cancelada,
            EmAndamento,
            Finalizada
        }

        public GenericModel[] GetSituacaoBemAtivoImob()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrSituacaoBemAtivoImob);
            return GetFromEnum(typeof(SituacaoBemAtivoImobEnum), d, false).ToArray();
        }

        public string GetDescrSituacaoBemAtivoImob(int situacaoBemAtivoImob)
        {
            switch (situacaoBemAtivoImob)
            {
                case (int)SituacaoBemAtivoImobEnum.Ativa: return "Ativa";
                case (int)SituacaoBemAtivoImobEnum.Cancelada: return "Cancelada";
                case (int)SituacaoBemAtivoImobEnum.EmAndamento: return "Em Andamento";
                case (int)SituacaoBemAtivoImobEnum.Finalizada: return "Finalizada";
                default: return String.Empty;
            }
        }

        #endregion

        #region Tipo de mercadoria

        /// <summary>
        /// Retorna os tipos de mercadoria disponíveis para serem usados no EFD, campo 07 no registro 200
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTipoMercadoria()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrTipoMercadoria);
            return GetFromEnum(typeof(TipoMercadoria), d, false).ToArray();
        }

        /// <summary>
        /// Retorna a descrição do tipo de mercadoria
        /// </summary>
        /// <param name="tipoMercadoria"></param>
        /// <returns></returns>
        public string GetDescrTipoMercadoria(int? tipoMercadoria)
        {
            switch (tipoMercadoria)
            {
                case (int)TipoMercadoria.MercadoriaRevenda: return Colosoft.Translator.Translate(TipoMercadoria.MercadoriaRevenda).Format();
                case (int)TipoMercadoria.MateriaPrima: return Colosoft.Translator.Translate(TipoMercadoria.MateriaPrima).Format();
                case (int)TipoMercadoria.Embalagem: return Colosoft.Translator.Translate(TipoMercadoria.Embalagem).Format();
                case (int)TipoMercadoria.ProdutoEmProcesso: return Colosoft.Translator.Translate(TipoMercadoria.ProdutoEmProcesso).Format();
                case (int)TipoMercadoria.ProdutoAcabado: return Colosoft.Translator.Translate(TipoMercadoria.ProdutoAcabado).Format();
                case (int)TipoMercadoria.Subproduto: return Colosoft.Translator.Translate(TipoMercadoria.Subproduto).Format();
                case (int)TipoMercadoria.ProdutoIntermediario: return Colosoft.Translator.Translate(TipoMercadoria.ProdutoIntermediario).Format();
                case (int)TipoMercadoria.MaterialUsoConsumo: return Colosoft.Translator.Translate(TipoMercadoria.MaterialUsoConsumo).Format();
                case (int)TipoMercadoria.AtivoImobilizado: return Colosoft.Translator.Translate(TipoMercadoria.AtivoImobilizado).Format();
                case (int)TipoMercadoria.Servicos: return Colosoft.Translator.Translate(TipoMercadoria.Servicos).Format();
                case (int)TipoMercadoria.OutrosInsumos: return Colosoft.Translator.Translate(TipoMercadoria.OutrosInsumos).Format();
                case (int)TipoMercadoria.Outras: return Colosoft.Translator.Translate(TipoMercadoria.Outras).Format();
                default: return String.Empty;
            }
        }

        #endregion

        #region Processo referenciado (nota fiscal)

        public GenericModel[] GetOrigemProcessoRef()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrOrigemProcessoRef);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.ProcessoReferenciado.Origem), d, false).ToArray();
        }

        public string GetDescrOrigemProcessoRef(int origemProcessoRef)
        {
            switch (origemProcessoRef)
            {
                case (int)Sync.Fiscal.Enumeracao.ProcessoReferenciado.Origem.SEFAZ: return "SEFAZ";
                case (int)Sync.Fiscal.Enumeracao.ProcessoReferenciado.Origem.JusticaFederal: return "Justiça Federal";
                case (int)Sync.Fiscal.Enumeracao.ProcessoReferenciado.Origem.JusticaEstadual: return "Justiça Estadual";
                case (int)Sync.Fiscal.Enumeracao.ProcessoReferenciado.Origem.SECEX_SRF: return "SECEX/SRF";
                case (int)Sync.Fiscal.Enumeracao.ProcessoReferenciado.Origem.Outros: return "Outros";
                default: return String.Empty;
            }
        }

        #endregion

        #region Documento arrecadação (processo referenciado)

        public GenericModel[] GetTipoDocumentoArrec()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoDocumentoArrec);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.DocumentoArrecadacao.TipoDocumentoArrecadacao), d, false).ToArray();
        }

        public string GetDescrTipoDocumentoArrec(int codTipo)
        {
            switch (codTipo)
            {
                case (int)Sync.Fiscal.Enumeracao.DocumentoArrecadacao.TipoDocumentoArrecadacao.DocumentoEstadualArrecadacao: return "Doc. Estadual de Arrecadação";
                case (int)Sync.Fiscal.Enumeracao.DocumentoArrecadacao.TipoDocumentoArrecadacao.GNRE: return "GNRE";
                default: return String.Empty;
            }
        }

        #endregion

        #region Documento fiscal (processo referenciado)

        public GenericModel[] GetTipoDocumentoFiscal()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoDocumentoFiscal);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.DocumentoFiscal.Tipo), d, false).ToArray();
        }

        public string GetDescrTipoDocumentoFiscal(int tipo)
        {
            switch (tipo)
            {
                case (int)Sync.Fiscal.Enumeracao.DocumentoFiscal.Tipo.Entrada: return "Entrada";
                case (int)Sync.Fiscal.Enumeracao.DocumentoFiscal.Tipo.Saida: return "Saída";
                default: return String.Empty;
            }
        }

        public GenericModel[] GetEmitenteDocumentoFiscal()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrEmitenteDocumentoFiscal);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.DocumentoFiscal.Emitente), d, false).ToArray();
        }

        public string GetDescrEmitenteDocumentoFiscal(int emitente)
        {
            switch (emitente)
            {
                case (int)Sync.Fiscal.Enumeracao.DocumentoFiscal.Emitente.EmissaoPropria: return "Emissão Própria";
                case (int)Sync.Fiscal.Enumeracao.DocumentoFiscal.Emitente.Terceiros: return "Terceiros";
                default: return String.Empty;
            }
        }

        #endregion

        #region Tipo de participante

        /// <summary>
        /// Enumeração com o tipo de participante.
        /// </summary>
        public enum TipoPartEnum
        {
            Cliente,
            Fornecedor,
            Transportador,
            Loja,
            AdministradoraCartao
        }

        public GenericModel[] GetTipoParticipante(bool exibirAdminCartao)
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoParticipante);
            var itens = GetFromEnum(typeof(TipoPartEnum), d, false);

            if (!exibirAdminCartao)
            {
                int item = itens.FindIndex(x => x.Id == (int)TipoPartEnum.AdministradoraCartao);
                itens.RemoveAt(item);
            }

            return itens.ToArray();
        }

        public string GetDescrTipoParticipante(int tipoPart)
        {
            switch (tipoPart)
            {
                case (int)TipoPartEnum.Cliente: return "Cliente";
                case (int)TipoPartEnum.Fornecedor: return "Fornecedor";
                case (int)TipoPartEnum.Loja: return "Loja";
                case (int)TipoPartEnum.Transportador: return "Transportador";
                case (int)TipoPartEnum.AdministradoraCartao: return "Admin. Cartão";
                default: return String.Empty;
            }
        }

        #endregion

        #region CST referente ao ICMS

        public enum CstIcms
        {
            TributadaIntegralmente = 0,
            TributadaComCobrancaDoIcmsPorSubstituicaoTributaria = 10,
            ComReducaoDeBaseDeCalculo = 20,
            IsentaOuNaoTributadaComCobrancaDoIcmsPorSubstituicaoTributaria = 30,
            Isenta = 40,
            NaoTributada = 41,
            Suspensão = 50,
            Diferimento = 51,
            IcmsCobradoAnteriormentePorSubstituicaoTributaria = 60,
            ComReducaoDeBaseDeCalculoECobrancaDoIcmsPorSubstituicaoTributaria = 70,
            Outras = 90
        }

        public KeyValuePair<string, string>[] GetCstIcms()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrCstIcms);
            return GetFromEnum(typeof(CstIcms), d, false).Select(x => new KeyValuePair<string, string>(x.Descr, x.Descr)).ToArray();
        }

        public string GetDescrCstIcms(int? cstIcms)
        {
            return cstIcms != null ? cstIcms.Value.ToString("00") : String.Empty;

            //switch (cstIcms)
            //{
            //    case (int)CstIcms.TributadaIntegralmente:
            //        return "Tributada integralmente";
            //    case (int)CstIcms.TributadaComCobrancaDoIcmsPorSubstituicaoTributaria:
            //        return "Tributada e com cobrança do ICMS por substituição tributária";
            //    case (int)CstIcms.ComReducaoDeBaseDeCalculo:
            //        return "Com redução de base de cálculo";
            //    case (int)CstIcms.IsentaOuNaoTributadaComCobrancaDoIcmsPorSubstituicaoTributaria:
            //        return "Isenta ou não tributada e com cobrança do ICMS por substituição tributária";
            //    case (int)CstIcms.Isenta:
            //        return "Isenta";
            //    case (int)CstIcms.NaoTributada:
            //        return "Não Tributada";
            //    case (int)CstIcms.Suspensão:
            //        return "Suspensão";
            //    case (int)CstIcms.Diferimento:
            //        return "Diferimento";
            //    case (int)CstIcms.IcmsCobradoAnteriormentePorSubstituicaoTributaria:
            //        return "ICMS cobrado anteriormente por substituição tributária";
            //    case (int)CstIcms.ComReducaoDeBaseDeCalculoECobrancaDoIcmsPorSubstituicaoTributaria:
            //        return "Com redução de base de cálculo e cobrança do ICMS por substituição tributária";
            //    case (int)CstIcms.Outras:
            //        return "Outras";
            //    default:
            //        return String.Empty;
            //}
        }

        #endregion

        #region CST referente ao IPI (Registro C170)

        public GenericModel[] GetCstIpi()
        {
            return GetCstIpi(false);
        }

        public GenericModel[] GetCstIpi(bool exibirNumeroDescricao)
        {
            var translates = Colosoft.Translator.GetTranslates(typeof(ProdutoCstIpi));

            if (exibirNumeroDescricao)
                return translates.Select(f => new GenericModel((int)f.Key, ((int)f.Key).ToString("00"))).ToArray();
            else
                return translates.Select(f => new GenericModel((int)f.Key, f.Translation)).ToArray();
        }

        #endregion

        #region Classe de consumo (nota fiscal)

        public GenericModel[] GetCodClasseConsumoNf(uint idNf)
        {
            NotaFiscal nf = new NotaFiscal();
            nf.Modelo = NotaFiscalDAO.Instance.ObtemValorCampo<string>("modelo", "idNf=" + idNf);

            Type tipo = nf.IsNfAgua ? typeof(InfoAdicionalNf.CodClasseConsumoAguaEnum) : typeof(InfoAdicionalNf.CodClasseConsumoEnergiaEnum);
            Converter<int?, string> d = nf.IsNfAgua ? new Converter<int?, string>(GetDescrCodClasseConsumoAguaNf) :
                new Converter<int?, string>(GetDescrCodClasseConsumoEletricaNf);

            return GetFromEnum(tipo, d, false).ToArray();
        }

        private string GetDescrCodClasseConsumoEletricaNf(int? codClasseConsumo)
        {
            switch (codClasseConsumo)
            {
                case (int)InfoAdicionalNf.CodClasseConsumoEnergiaEnum.Comercial: return "Comercial";
                case (int)InfoAdicionalNf.CodClasseConsumoEnergiaEnum.ConsumoProprio: return "Consumo próprio";
                case (int)InfoAdicionalNf.CodClasseConsumoEnergiaEnum.IluminacaoPublica: return "Iluminação pública";
                case (int)InfoAdicionalNf.CodClasseConsumoEnergiaEnum.Industrial: return "Industrial";
                case (int)InfoAdicionalNf.CodClasseConsumoEnergiaEnum.PoderPublico: return "Poder público";
                case (int)InfoAdicionalNf.CodClasseConsumoEnergiaEnum.Residencial: return "Residencial";
                case (int)InfoAdicionalNf.CodClasseConsumoEnergiaEnum.Rural: return "Rural";
                case (int)InfoAdicionalNf.CodClasseConsumoEnergiaEnum.ServicoPublico: return "Serviço público";
                default: return String.Empty;
            }
        }

        private string GetDescrCodClasseConsumoAguaNf(int? codClasseConsumo)
        {
            switch (codClasseConsumo)
            {
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoResAteRs50: return "Consolidado resid. até R$50,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoResAteRs100: return "Consolidado resid. de R$50,01 até R$100,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoResAteRs200: return "Consolidado resid. de R$100,01 até R$200,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoResAteRs300: return "Consolidado resid. de R$200,01 até R$300,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoResAteRs400: return "Consolidado resid. de R$300,01 até R$400,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoResAteRs500: return "Consolidado resid. de R$400,01 até R$500,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoResAteRs1000: return "Consolidado resid. de R$500,01 até R$1000,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoResAcimaRs1000: return "Consolidado resid. acima de R$1000,01";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoComIndAteRs50: return "Consolidado com./ind. até R$50,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoComIndAteRs100: return "Consolidado com./ind. de R$50,01 até R$100,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoComIndAteRs200: return "Consolidado com./ind. de R$100,01 até R$200,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoComIndAteRs300: return "Consolidado com./ind. de R$200,01 até R$300,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoComIndAteRs400: return "Consolidado com./ind. de R$300,01 até R$400,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoComIndAteRs500: return "Consolidado com./ind. de R$400,01 até R$500,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoComIndAteRs1000: return "Consolidado com./ind. de R$500,01 até R$1000,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoComIndAcimaRs1000: return "Consolidado com./ind. acima de R$1000,01";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoOrgaoPublico: return "Consolidado órgão público";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoOutrosAteRs50: return "Consolidado outros tipos até R$50,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoOutrosAteRs100: return "Consolidado outros tipos de R$50,01 até R$100,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoOutrosAteRs200: return "Consolidado outros tipos de R$100,01 até R$200,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoOutrosAteRs300: return "Consolidado outros tipos de R$200,01 até R$300,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoOutrosAteRs400: return "Consolidado outros tipos de R$300,01 até R$400,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoOutrosAteRs500: return "Consolidado outros tipos de R$400,01 até R$500,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoOutrosAteRs1000: return "Consolidado outros tipos de R$500,01 até R$1000,00";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.ConsolidadoOutrosAcimaRs1000: return "Consolidado outros tipos acima de R$1000,01";
                case (int)InfoAdicionalNf.CodClasseConsumoAguaEnum.Individual: return "Registro individual";
                default: return String.Empty;
            }
        }

        public string GetDescrCodClasseConsumoNf(bool isNfAgua, int? codClasseConsumo)
        {
            return isNfAgua ? GetDescrCodClasseConsumoAguaNf(codClasseConsumo) :
                GetDescrCodClasseConsumoEletricaNf(codClasseConsumo);
        }

        #endregion

        #region Código da ligação (nota fiscal)

        public GenericModel[] GetCodLigacaoNf()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrCodLigacaoNf);
            return GetFromEnum(typeof(InfoAdicionalNf.CodLigacaoEnum), d, false).ToArray();
        }

        public string GetDescrCodLigacaoNf(int? codLigacao)
        {
            switch (codLigacao)
            {
                case (int)InfoAdicionalNf.CodLigacaoEnum.Monofasico: return "Monofásico";
                case (int)InfoAdicionalNf.CodLigacaoEnum.Bifasico: return "Bifásico";
                case (int)InfoAdicionalNf.CodLigacaoEnum.Trifasico: return "Trifásico";
                default: return String.Empty;
            }
        }

        #endregion

        #region Código do grupo de tensão (nota fiscal)

        public GenericModel[] GetCodGrupoTensaoNf()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrCodGrupoTensaoNf);
            return GetFromEnum(typeof(InfoAdicionalNf.CodGrupoTensaoEnum), d, false).ToArray();
        }

        public string GetDescrCodGrupoTensaoNf(int? codGrupoTensao)
        {
            switch (codGrupoTensao)
            {
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.AltaTensaoA1: return "A1 - Alta Tensão (230kV ou mais)";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.AltaTensaoA2: return "A2 - Alta Tensão (88 a 138kV)";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.AltaTensaoA3: return "A3 - Alta Tensão (69kV)";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.AltaTensaoA3a: return "A3a - Alta Tensão (30kV a 44kV)";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.AltaTensaoA4: return "A4 - Alta Tensão (2,3kV a 25kV)";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.AltaTensaoSubterraneo: return "AS - Alta Tensão Subterrâneo";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.Residencial: return "B1 - Residencial";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.ResidencialBaixaRenda: return "B1 - Residencial Baixa Renda";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.Rural: return "Rural";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.CooperativaEletrificacaoRural: return "Cooperativa de Eletrificação Rural";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.ServicoPublicoIrrigacao: return "Serviço Público de Irrigação";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.DemaisClasses: return "Demais Classes";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.IluminacaoPublicaRedeDistribuicao: return "Iluminação Pública - Rede de Distribuição";
                case (int)InfoAdicionalNf.CodGrupoTensaoEnum.IluminacaoPublicaBulboLampada: return "Iluminação Pública - Bulbo de Lâmpada";
                default: return String.Empty;
            }
        }

        #endregion

        #region Tipo de CT-e (nota fiscal)

        public GenericModel[] GetTipoCte()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrTipoCte);
            return GetFromEnum(typeof(InfoAdicionalNf.TipoCteEnum), d, false).ToArray();
        }

        public string GetDescrTipoCte(int? tipoCte)
        {
            switch (tipoCte)
            {
                case (int)InfoAdicionalNf.TipoCteEnum.Normal: return "Normal";
                case (int)InfoAdicionalNf.TipoCteEnum.Complementar: return "Complemento de valores";
                case (int)InfoAdicionalNf.TipoCteEnum.AnulacaoValores: return "Anulação de valores";
                case (int)InfoAdicionalNf.TipoCteEnum.Substituto: return "Substituto";
                default: return String.Empty;
            }
        }

        #endregion

        #region Tipo de assinante (nota fiscal)

        public GenericModel[] GetTipoAssinanteNf()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrTipoAssinante);
            return GetFromEnum(typeof(InfoAdicionalNf.TipoAssinanteEnum), d, false).ToArray();
        }

        public string GetDescrTipoAssinante(int? tipoAssinante)
        {
            switch (tipoAssinante)
            {
                case (int)InfoAdicionalNf.TipoAssinanteEnum.ComercialIndustrial: return "Comercial/Industrial";
                case (int)InfoAdicionalNf.TipoAssinanteEnum.PoderPublico: return "Poder público";
                case (int)InfoAdicionalNf.TipoAssinanteEnum.ResidencialPessoaFisica: return "Residencial/Pessoa física";
                case (int)InfoAdicionalNf.TipoAssinanteEnum.Publico: return "Público";
                case (int)InfoAdicionalNf.TipoAssinanteEnum.SemiPublico: return "Semi-público";
                case (int)InfoAdicionalNf.TipoAssinanteEnum.Outros: return "Outros";
                default: return String.Empty;
            }
        }

        #endregion

        #region Natureza da Base de Cálculo de Crédito

        public enum NaturezaBcCreditoEnum
        {
            AquisicaoBensRevenda = 1,
            AquisicaoBensInsumo,
            AquisicaoServicosInsumo,
            EnergiaEletricaETermica,
            AlugueisPredios,                                    // 5
            AlugueisMaquinasEquipamentos,
            ArmazenagemMercadoriaFreteVenda,
            ContraprestacoesArrendamentoMercantil,
            BensAtivoImobilizadoDepreciacao,
            BensAtivoImobilizadoAquisicao,                      // 10
            AmortizacaoEdificacoesBenfeitoriasImoveis,
            DevolucaoVendasSujeitasIncidenciaNaoCumulativa,
            OutrasOperacoesCredito,
            TransporteCargasSubcontratacao,
            ImobiliariaCustoIncorridoUnidadeImobiliaria,        // 15
            ImobiliariaCustoOrcadoUnidadeNaoConcluida,
            PrestacaoServicosLimpezaConservacaoOuManutencao,
            EstoqueAberturaBens
        }

        public GenericModel[] GetNaturezaBcCredito()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrNaturezaBcCredito);
            return GetFromEnum(typeof(NaturezaBcCreditoEnum), d, true).ToArray();
        }

        public string GetDescrNaturezaBcCredito(int? naturezaBcCredito)
        {
            switch (naturezaBcCredito)
            {
                case (int)NaturezaBcCreditoEnum.AquisicaoBensRevenda:
                    return "Aquisição de bens para revenda";
                case (int)NaturezaBcCreditoEnum.AquisicaoBensInsumo:
                    return "Aquisição de bens utilizados como insumo";
                case (int)NaturezaBcCreditoEnum.AquisicaoServicosInsumo:
                    return "Aquisição de serviços utilizados como insumo";
                case (int)NaturezaBcCreditoEnum.EnergiaEletricaETermica:
                    return "Energia elétrica e térmica, inclusive sob a forma de vapor";
                case (int)NaturezaBcCreditoEnum.AlugueisPredios:
                    return "Aluguéis de prédios";
                case (int)NaturezaBcCreditoEnum.AlugueisMaquinasEquipamentos:
                    return "Aluguéis de máquinas e equipamentos";
                case (int)NaturezaBcCreditoEnum.ArmazenagemMercadoriaFreteVenda:
                    return "Armazenagem de mercadoria e frete na operação de venda";
                case (int)NaturezaBcCreditoEnum.ContraprestacoesArrendamentoMercantil:
                    return "Contraprestações de arrendamento mercantil";
                case (int)NaturezaBcCreditoEnum.BensAtivoImobilizadoDepreciacao:
                    return "Máquinas, equipamentos e outros bens incorporados ao ativo imobilizado (crédito sobre encargos de depreciação)";
                case (int)NaturezaBcCreditoEnum.BensAtivoImobilizadoAquisicao:
                    return "Máquinas, equipamentos e outros bens incorporados ao ativo imobilizado (crédito com base no valor de aquisição)";
                case (int)NaturezaBcCreditoEnum.AmortizacaoEdificacoesBenfeitoriasImoveis:
                    return "Amortização de edificações e benfeitorias em imóveis";
                case (int)NaturezaBcCreditoEnum.DevolucaoVendasSujeitasIncidenciaNaoCumulativa:
                    return "Devolução de Vendas Sujeitas à Incidência Não-Cumulativa";
                case (int)NaturezaBcCreditoEnum.OutrasOperacoesCredito:
                    return "Outras Operações com Direito a Crédito";
                case (int)NaturezaBcCreditoEnum.TransporteCargasSubcontratacao:
                    return "Atividade de Transporte de Cargas - Subcontratação";
                case (int)NaturezaBcCreditoEnum.ImobiliariaCustoIncorridoUnidadeImobiliaria:
                    return "Atividade Imobiliária - Custo Incorrido de Unidade Imobiliária";
                case (int)NaturezaBcCreditoEnum.ImobiliariaCustoOrcadoUnidadeNaoConcluida:
                    return "Atividade Imobiliária - Custo Orçado de unidade não concluída";
                case (int)NaturezaBcCreditoEnum.PrestacaoServicosLimpezaConservacaoOuManutencao:
                    return "Atividade de Prestação de Serviços de Limpeza, Conservação e Manutenção - vale-transporte, vale-refeição ou vale-alimentação, fardamento ou uniforme";
                case (int)NaturezaBcCreditoEnum.EstoqueAberturaBens:
                    return "Estoque de abertura de bens";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Indicador da Natureza de Frete Contratado

        public enum IndNaturezaFreteEnum
        {
            OperacoesVendasOnusSuportadoVendedor,
            OperacoesVendasOnusSuportadoAdquirente,
            OperacoesComprasGeradorasCredito,
            OperacoesComprasNaoGeradorasCredito,
            TransferenciaProdutosAcabados,
            TransferenciaProdutosEmElaboracao,
            Outras = 9
        }

        public GenericModel[] GetIndNaturezaFrete()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrIndNaturezaFrete);
            return GetFromEnum(typeof(IndNaturezaFreteEnum), d, false).ToArray();
        }

        public string GetDescrIndNaturezaFrete(int? indNaturezaFrete)
        {
            switch (indNaturezaFrete)
            {
                case (int)IndNaturezaFreteEnum.OperacoesVendasOnusSuportadoVendedor:
                    return "Operações de vendas, com ônus suportado pelo estabelecimento vendedor";
                case (int)IndNaturezaFreteEnum.OperacoesVendasOnusSuportadoAdquirente:
                    return "Operações de vendas, com ônus suportado pelo adquirente";
                case (int)IndNaturezaFreteEnum.OperacoesComprasGeradorasCredito:
                    return "Operações de compras (bens para revenda, matérias-prima e outros produtos, geradores de crédito)";
                case (int)IndNaturezaFreteEnum.OperacoesComprasNaoGeradorasCredito:
                    return "Operações de compras (bens para revenda, matérias-prima e outros produtos, não geradores de crédito)";
                case (int)IndNaturezaFreteEnum.TransferenciaProdutosAcabados:
                    return "Transferência de produtos acabados entre estabelecimentos da pessoa jurídica";
                case (int)IndNaturezaFreteEnum.TransferenciaProdutosEmElaboracao:
                    return "Transferência de produtos em elaboração entre estabelecimentos da pessoa jurídica";
                case (int)IndNaturezaFreteEnum.Outras:
                    return "Outras";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Código do Tipo de Crédito

        public enum CodCredEnum
        {
            ReceitaTributadaMercadoInternoAliquotaBasica = 101,
            ReceitaTributadaMercadoInternoAliquotasDiferenciadas = 102,
            ReceitaTributadaMercadoInternoAliquotaUnidadeProduto = 103,
            ReceitaTributadaMercadoInternoEstoqueAbertura = 104,
            ReceitaTributadaMercadoInternoAquisicaoEmbalagensRevenda = 105,
            ReceitaTributadaMercadoInternoPresumidoAgroindustria = 106,
            ReceitaTributadaMercadoInternoOutrosCreditosPresumidos = 107,
            ReceitaTributadaMercadoInternoImportacao = 108,
            ReceitaTributadaMercadoInternoAtividadeImobiliaria = 109,
            ReceitaTributadaMercadoInternoOutros = 199,
            ReceitaNaoTributadaMercadoInternoAliquotaBasica = 201,
            ReceitaNaoTributadaMercadoInternoAliquotasDiferenciadas = 202,
            ReceitaNaoTributadaMercadoInternoAliquotaUnidadeProduto = 203,
            ReceitaNaoTributadaMercadoInternoEstoqueAbertura = 204,
            ReceitaNaoTributadaMercadoInternoAquisicaoEmbalagensRevenda = 205,
            ReceitaNaoTributadaMercadoInternoPresumidoAgroindustria = 206,
            ReceitaNaoTributadaMercadoInternoOutrosCreditosPresumidos = 207,
            ReceitaNaoTributadaMercadoInternoImportacao = 208,
            ReceitaNaoTributadaMercadoInternoOutros = 299,
            ReceitaExportacaoAliquotaBasica = 301,
            ReceitaExportacaoAliquotasDiferenciadas = 302,
            ReceitaExportacaoAliquotaUnidadeProduto = 303,
            ReceitaExportacaoEstoqueAbertura = 304,
            ReceitaExportacaoAquisicaoEmbalagensRevenda = 305,
            ReceitaExportacaoPresumidoAgroindustria = 306,
            ReceitaExportacaoOutrosCreditosPresumidos = 307,
            ReceitaExportacaoImportacao = 308,
            ReceitaExportacaoOutros = 399
        }

        public GenericModel[] GetCodCred()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrCodCred);
            return GetFromEnum(typeof(CodCredEnum), d, false).ToArray();
        }

        public string GetDescrCodCred(int? codCred)
        {
            switch (codCred)
            {
                case (int)CodCredEnum.ReceitaTributadaMercadoInternoAliquotaBasica:
                    return "Crédito vinculado à receita tributada no mercado interno – Alíquota Básica";
                case (int)CodCredEnum.ReceitaTributadaMercadoInternoAliquotasDiferenciadas:
                    return "Crédito vinculado à receita tributada no mercado interno – Alíquotas Diferenciadas";
                case (int)CodCredEnum.ReceitaTributadaMercadoInternoAliquotaUnidadeProduto:
                    return "Crédito vinculado à receita tributada no mercado interno – Alíquota por Unidade de Produto";
                case (int)CodCredEnum.ReceitaTributadaMercadoInternoEstoqueAbertura:
                    return "Crédito vinculado à receita tributada no mercado interno – Estoque de Abertura";
                case (int)CodCredEnum.ReceitaTributadaMercadoInternoAquisicaoEmbalagensRevenda:
                    return "Crédito vinculado à receita tributada no mercado interno – Aquisição Embalagens para revenda";
                case (int)CodCredEnum.ReceitaTributadaMercadoInternoPresumidoAgroindustria:
                    return "Crédito vinculado à receita tributada no mercado interno – Presumido da Agroindústria";
                case (int)CodCredEnum.ReceitaTributadaMercadoInternoOutrosCreditosPresumidos:
                    return "Crédito vinculado à receita tributada no mercado interno – Outros Créditos Presumidos";
                case (int)CodCredEnum.ReceitaTributadaMercadoInternoImportacao:
                    return "Crédito vinculado à receita tributada no mercado interno – Importação";
                case (int)CodCredEnum.ReceitaTributadaMercadoInternoAtividadeImobiliaria:
                    return "Crédito vinculado à receita tributada no mercado interno – Atividade Imobiliária";
                case (int)CodCredEnum.ReceitaTributadaMercadoInternoOutros:
                    return "Crédito vinculado à receita tributada no mercado interno – Outros";
                case (int)CodCredEnum.ReceitaNaoTributadaMercadoInternoAliquotaBasica:
                    return "Crédito vinculado à receita não tributada no mercado interno – Alíquota Básica";
                case (int)CodCredEnum.ReceitaNaoTributadaMercadoInternoAliquotasDiferenciadas:
                    return "Crédito vinculado à receita não tributada no mercado interno – Alíquotas Diferenciadas";
                case (int)CodCredEnum.ReceitaNaoTributadaMercadoInternoAliquotaUnidadeProduto:
                    return "Crédito vinculado à receita não tributada no mercado interno – Alíquota por Unidade de Produto";
                case (int)CodCredEnum.ReceitaNaoTributadaMercadoInternoEstoqueAbertura:
                    return "Crédito vinculado à receita não tributada no mercado interno – Estoque de Abertura";
                case (int)CodCredEnum.ReceitaNaoTributadaMercadoInternoAquisicaoEmbalagensRevenda:
                    return "Crédito vinculado à receita não tributada no mercado interno – Aquisição Embalagens para revenda";
                case (int)CodCredEnum.ReceitaNaoTributadaMercadoInternoPresumidoAgroindustria:
                    return "Crédito vinculado à receita não tributada no mercado interno – Presumido da Agroindústria";
                case (int)CodCredEnum.ReceitaNaoTributadaMercadoInternoOutrosCreditosPresumidos:
                    return "Crédito vinculado à receita não tributada no mercado interno – Outros Créditos Presumidos";
                case (int)CodCredEnum.ReceitaNaoTributadaMercadoInternoImportacao:
                    return "Crédito vinculado à receita não tributada no mercado interno – Importação";
                case (int)CodCredEnum.ReceitaNaoTributadaMercadoInternoOutros:
                    return "Crédito vinculado à receita não tributada no mercado interno – Outros";
                case (int)CodCredEnum.ReceitaExportacaoAliquotaBasica:
                    return "Crédito vinculado à receita de exportação – Alíquota Básica";
                case (int)CodCredEnum.ReceitaExportacaoAliquotasDiferenciadas:
                    return "Crédito vinculado à receita de exportação – Alíquotas Diferenciadas";
                case (int)CodCredEnum.ReceitaExportacaoAliquotaUnidadeProduto:
                    return "Crédito vinculado à receita de exportação – Alíquota por Unidade de Produto";
                case (int)CodCredEnum.ReceitaExportacaoEstoqueAbertura:
                    return "Crédito vinculado à receita de exportação – Estoque de Abertura";
                case (int)CodCredEnum.ReceitaExportacaoAquisicaoEmbalagensRevenda:
                    return "Crédito vinculado à receita de exportação – Aquisição Embalagens para revenda";
                case (int)CodCredEnum.ReceitaExportacaoPresumidoAgroindustria:
                    return "Crédito vinculado à receita de exportação – Presumido da Agroindústria";
                case (int)CodCredEnum.ReceitaExportacaoOutrosCreditosPresumidos:
                    return "Crédito vinculado à receita de exportação – Outros Créditos Presumidos";
                case (int)CodCredEnum.ReceitaExportacaoImportacao:
                    return "Crédito vinculado à receita de exportação – Importação";
                case (int)CodCredEnum.ReceitaExportacaoOutros:
                    return "Crédito vinculado à receita de exportação – Outros";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Código da Contribuição Social

        public enum CodContEnum
        {
            ContribuicaoNaoCumulativaAliquotaBasica = 1,
            ContribuicaoNaoCumulativaAliquotasDiferenciadas = 2,
            ContribuicaoNaoCumulativaAliquotaUnidadeMedidaProduto = 3,
            ContribuicaoNaoCumulativaAtividadeImobiliaria = 4,
            ContribuicaoSubstituicaoTributaria = 31,
            ContribuicaoSubstituicaoTributariaVendasZonaFrancaManaus = 32,
            ContribuicaoCumulativaAliquotaBasica = 51,
            ContribuicaoCumulativaAliquotasDiferenciadas = 52,
            ContribuicaoCumulativaAliquotaUnidadeMedidaProduto = 53,
            ContribuicaoCumulativaAtividadeImobiliaria = 54,
            ContribuicaoSCPNaoCumulativa = 71,
            ContribuicaoSCPCumulativa = 72,
            ContribuicaoPisPasepFolhaSalarios = 99
        }

        public GenericModel[] GetCodCont()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrCodCont);
            return GetFromEnum(typeof(CodContEnum), d, false).ToArray();
        }

        public string GetDescrCodCont(int? codCont)
        {
            switch (codCont)
            {
                case (int)CodContEnum.ContribuicaoNaoCumulativaAliquotaBasica:
                    return "Contribuição não-cumulativa apurada a alíquota básica";
                case (int)CodContEnum.ContribuicaoNaoCumulativaAliquotasDiferenciadas:
                    return "Contribuição não-cumulativa apurada a alíquotas diferenciadas";
                case (int)CodContEnum.ContribuicaoNaoCumulativaAliquotaUnidadeMedidaProduto:
                    return "Contribuição não-cumulativa apurada a alíquota por unidade de medida de produto";
                case (int)CodContEnum.ContribuicaoNaoCumulativaAtividadeImobiliaria:
                    return "Contribuição não-cumulativa apurada a alíquota básica – Atividade Imobiliária";
                case (int)CodContEnum.ContribuicaoSubstituicaoTributaria:
                    return "Contribuição apurada por substituição tributária";
                case (int)CodContEnum.ContribuicaoSubstituicaoTributariaVendasZonaFrancaManaus:
                    return "Contribuição apurada por substituição tributária – Vendas à Zona Franca de Manaus";
                case (int)CodContEnum.ContribuicaoCumulativaAliquotaBasica:
                    return "Contribuição cumulativa apurada a alíquota básica";
                case (int)CodContEnum.ContribuicaoCumulativaAliquotasDiferenciadas:
                    return "Contribuição cumulativa apurada a alíquotas diferenciadas";
                case (int)CodContEnum.ContribuicaoCumulativaAliquotaUnidadeMedidaProduto:
                    return "Contribuição cumulativa apurada a alíquota por unidade de medida de produto";
                case (int)CodContEnum.ContribuicaoCumulativaAtividadeImobiliaria:
                    return "Contribuição cumulativa apurada a alíquota básica – Atividade Imobiliária";
                case (int)CodContEnum.ContribuicaoSCPNaoCumulativa:
                    return "Contribuição apurada de SCP – Incidência Não Cumulativa";
                case (int)CodContEnum.ContribuicaoSCPCumulativa:
                    return "Contribuição apurada de SCP – Incidência Cumulativa";
                case (int)CodContEnum.ContribuicaoPisPasepFolhaSalarios:
                    return "Contribuição para o PIS/Pasep – Folha de Salários";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Código do Tipo de Crédito/Contribuição Social

        public GenericModel[] GetCodCredCont(int fonteAjuste)
        {
            return fonteAjuste == (int)Sync.Fiscal.Enumeracao.AjusteContribuicao.FonteAjuste.Credito ? GetCodCred() : GetCodCont();
        }

        #endregion

        #region Fonte de Crédito/Contribuição Social

        public GenericModel[] GetFonteAjuste()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrFonteAjuste);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.AjusteContribuicao.FonteAjuste), d, false).ToArray();
        }

        public string GetDescrFonteAjuste(int fonteAjuste)
        {
            switch (fonteAjuste)
            {
                case (int)Sync.Fiscal.Enumeracao.AjusteContribuicao.FonteAjuste.Credito: return "Crédito";
                case (int)Sync.Fiscal.Enumeracao.AjusteContribuicao.FonteAjuste.Contribuicao: return "Contribuição";
                default: return String.Empty;
            }
        }

        #endregion

        #region CST PIS/COFINS

        public enum CstPisCofinsEnum
        {
            OperacaoTributavelAliquotaBasica = 1,
            OperacaoTributavelAliquotaDiferenciada,
            OperacaoTributavelAliquotaUnidadeMedidaProduto,
            OperacaoTributavelMonofasicaRevendaAliquotaZero,
            OperacaoTributavelSubstituicaoTributaria,
            OperacaoTributavelAliquotaZero,
            OperacaoIsentaContribuicao,
            OperacaoSemIncidenciaContribuicao,
            OperacaoComSuspensaoContribuicao,
            OutrasOperacoesSaida = 49,
            OperacaoDireitoCreditoReceitaTributadaMercadoInterno,
            OperacaoDireitoCreditoReceitaNaoTributadaMercadoInterno,
            OperacaoDireitoCreditoReceitaExportacao,
            OperacaoDireitoCreditoReceitaTributadaENaoTributadaMercadoInterno,
            OperacaoDireitoCreditoReceitaTributadaMercadoInternoEExportacao,
            OperacaoDireitoCreditoReceitaNaoTributadaMercadoInternoEExportacao,
            OperacaoDireitoCreditoReceitaTributadaENaoTributadaMercadoInternoEExportacao,
            CreditoPresumidoAquisicaoReceitaTributadaMercadoInterno = 60,
            CreditoPresumidoAquisicaoReceitaNaoTributadaMercadoInterno,
            CreditoPresumidoAquisicaoReceitaExportacao,
            CreditoPresumidoAquisicaoReceitaTributadaENaoTributadaMercadoInterno,
            CreditoPresumidoAquisicaoReceitaTributadaMercadoInternoEExportacao,
            CreditoPresumidoAquisicaoReceitaNaoTributadaMercadoInternoEExportacao,
            CreditoPresumidoAquisicaoReceitaTributadaENaoTributadaMercadoInternoEExportacao,
            CreditoPresumidoOutrasOperacoes,
            OperacaoAquisicaoSemDireitoACredito = 70,
            OperacaoAquisicaoComIsencao,
            OperacaoAquisicaoComSuspensao,
            OperacaoAquisicaoAliquotaZero,
            OperacaoAquisicaoSemIncidenciaContribuicao,
            OperacaoAquisicaoSubstituicaoTributaria,
            OutrasOperacoesEntrada = 98,
            OutrasOperacoes
        }

        public GenericModel[] GetCstPisCofins()
        {
            return GetCstPisCofins(false);
        }

        public GenericModel[] GetCstPisCofins(bool exibirNumeroDescricao)
        {
            Converter<int?, string> d = !exibirNumeroDescricao ? new Converter<int?, string>(GetDescrCstPisCofins) :
                new Converter<int?, string>(x => x != null ? x.Value.ToString("00") : null);

            return GetFromEnum(typeof(CstPisCofinsEnum), d, false).ToArray();
        }

        public string GetDescrCstPisCofins(int? cstPisCofins)
        {
            switch (cstPisCofins)
            {
                case (int)CstPisCofinsEnum.OperacaoTributavelAliquotaBasica:
                    return "Operação Tributável com Alíquota Básica";
                case (int)CstPisCofinsEnum.OperacaoTributavelAliquotaDiferenciada:
                    return "Operação Tributável com Alíquota Diferenciada";
                case (int)CstPisCofinsEnum.OperacaoTributavelAliquotaUnidadeMedidaProduto:
                    return "Operação Tributável com Alíquota por Unidade de Medida de Produto";
                case (int)CstPisCofinsEnum.OperacaoTributavelMonofasicaRevendaAliquotaZero:
                    return "Operação Tributável Monofásica - Revenda a Alíquota Zero";
                case (int)CstPisCofinsEnum.OperacaoTributavelSubstituicaoTributaria:
                    return "Operação Tributável por Substituição Tributária";
                case (int)CstPisCofinsEnum.OperacaoTributavelAliquotaZero:
                    return "Operação Tributável a Alíquota Zero";
                case (int)CstPisCofinsEnum.OperacaoIsentaContribuicao:
                    return "Operação Isenta da Contribuição";
                case (int)CstPisCofinsEnum.OperacaoSemIncidenciaContribuicao:
                    return "Operação sem Incidência da Contribuição";
                case (int)CstPisCofinsEnum.OperacaoComSuspensaoContribuicao:
                    return "Operação com Suspensão da Contribuição";
                case (int)CstPisCofinsEnum.OutrasOperacoesSaida:
                    return "Outras Operações de Saída";
                case (int)CstPisCofinsEnum.OperacaoDireitoCreditoReceitaTributadaMercadoInterno:
                    return "Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                case (int)CstPisCofinsEnum.OperacaoDireitoCreditoReceitaNaoTributadaMercadoInterno:
                    return "Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno";
                case (int)CstPisCofinsEnum.OperacaoDireitoCreditoReceitaExportacao:
                    return "Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação";
                case (int)CstPisCofinsEnum.OperacaoDireitoCreditoReceitaTributadaENaoTributadaMercadoInterno:
                    return "Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno";
                case (int)CstPisCofinsEnum.OperacaoDireitoCreditoReceitaTributadaMercadoInternoEExportacao:
                    return "Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                case (int)CstPisCofinsEnum.OperacaoDireitoCreditoReceitaNaoTributadaMercadoInternoEExportacao:
                    return "Operação com Direito a Crédito - Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação";
                case (int)CstPisCofinsEnum.OperacaoDireitoCreditoReceitaTributadaENaoTributadaMercadoInternoEExportacao:
                    return "Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação";
                case (int)CstPisCofinsEnum.CreditoPresumidoAquisicaoReceitaTributadaMercadoInterno:
                    return "Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                case (int)CstPisCofinsEnum.CreditoPresumidoAquisicaoReceitaNaoTributadaMercadoInterno:
                    return "Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno";
                case (int)CstPisCofinsEnum.CreditoPresumidoAquisicaoReceitaExportacao:
                    return "Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação";
                case (int)CstPisCofinsEnum.CreditoPresumidoAquisicaoReceitaTributadaENaoTributadaMercadoInterno:
                    return "Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno";
                case (int)CstPisCofinsEnum.CreditoPresumidoAquisicaoReceitaTributadaMercadoInternoEExportacao:
                    return "Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                case (int)CstPisCofinsEnum.CreditoPresumidoAquisicaoReceitaNaoTributadaMercadoInternoEExportacao:
                    return "Crédito Presumido - Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação";
                case (int)CstPisCofinsEnum.CreditoPresumidoAquisicaoReceitaTributadaENaoTributadaMercadoInternoEExportacao:
                    return "Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação";
                case (int)CstPisCofinsEnum.CreditoPresumidoOutrasOperacoes:
                    return "Crédito Presumido - Outras Operações";
                case (int)CstPisCofinsEnum.OperacaoAquisicaoSemDireitoACredito:
                    return "Operação de Aquisição sem Direito a Crédito";
                case (int)CstPisCofinsEnum.OperacaoAquisicaoComIsencao:
                    return "Operação de Aquisição com Isenção";
                case (int)CstPisCofinsEnum.OperacaoAquisicaoComSuspensao:
                    return "Operação de Aquisição com Suspensão";
                case (int)CstPisCofinsEnum.OperacaoAquisicaoAliquotaZero:
                    return "Operação de Aquisição a Alíquota Zero";
                case (int)CstPisCofinsEnum.OperacaoAquisicaoSemIncidenciaContribuicao:
                    return "Operação de Aquisição sem Incidência da Contribuição";
                case (int)CstPisCofinsEnum.OperacaoAquisicaoSubstituicaoTributaria:
                    return "Operação de Aquisição por Substituição Tributária";
                case (int)CstPisCofinsEnum.OutrasOperacoesEntrada:
                    return "Outras Operações de Entrada";
                case (int)CstPisCofinsEnum.OutrasOperacoes:
                    return "Outras Operações";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Processos Administrativos

        public GenericModel[] GetNaturezaProcessoAdministrativo()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrNaturezaProcessoAdministrativo);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.ProcessoAdministrativo.Natureza), d, true).ToArray();
        }

        public string GetDescrNaturezaProcessoAdministrativo(int natureza)
        {
            return Colosoft.Translator.Translate((Sync.Fiscal.Enumeracao.ProcessoAdministrativo.Natureza)natureza).Format();
        }

        #endregion

        #region Processos Judiciais

        public GenericModel[] GetNaturezaProcessoJudicial()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrNaturezaProcessoJudicial);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza), d, true).ToArray();
        }

        public string GetDescrNaturezaProcessoJudicial(int natureza)
        {
            switch (natureza)
            {
                case (int)Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza.DecisaoJudicialTransitadaEmJulgado:
                    return "Decisão Judicial Transitada em Julgado, a favor da Pessoa Jurídica";
                case (int)Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza.DecisaoJudicialNaoTransitadaEmJulgado:
                    return "Decisão Judicial Não Transitada em Julgado, a favor da Pessoa Jurídica";
                case (int)Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza.DecisaoJudicialLiminarMandadoSeguranca:
                    return "Decisão Judicial oriunda de Liminar em Mandado de Segurança";
                case (int)Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza.DecisaoJudicialLiminarMedidaCautelar:
                    return "Decisão Judicial oriunda de Liminar em Medida Cautelar";
                case (int)Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza.DecisaoJudicialAntecipacaoDeTutela:
                    return "Decisão Judicial oriunda de Antecipação de Tutela";
                case (int)Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza.DecisaoJudicialVinculadaDepositoAdministrativoOuJudicialMontanteIntegral:
                    return "Decisão Judicial Vinculada a Depósito Administrativo ou Judicial em Montante Integral";
                case (int)Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza.MedidaJudicialPessoaJuridicaNaoEAutor:
                    return "Medida Judicial em que a Pessoa Jurídica não é o autor";
                case (int)Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza.SumulaVinculanteAprovadaSTF:
                    return "Súmula Vinculante aprovada pelo STF";
                case (int)Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza.Outros:
                    return "Outros";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Código Indicador Incidência Tributária

        public enum CodIncTribEnum
        {
            NaoCumulativo = 1,
            Cumulativo,
            Ambos
        }

        public GenericModel[] GetCodIncTrib()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrCodIncTrib);
            return GetFromEnum(typeof(CodIncTribEnum), d, true).ToArray();
        }

        public string GetDescrCodIncTrib(int codIncTrib)
        {
            switch (codIncTrib)
            {
                case (int)CodIncTribEnum.NaoCumulativo:
                    return "Escrituração de operações com incidência exclusivamente no regime não-cumulativo";
                case (int)CodIncTribEnum.Cumulativo:
                    return "Escrituração de operações com incidência exclusivamente no regime cumulativo";
                case (int)CodIncTribEnum.Ambos:
                    return "Escrituração de operações com incidência nos regimes não-cumulativo e cumulativo";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Indicador Método Apropriação Créditos

        public enum IndAproCredEnum
        {
            ApropriacaoDireta = 1,
            RateioProporcional
        }

        public GenericModel[] GetIndAproCred()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrIndAproCred);
            return GetFromEnum(typeof(IndAproCredEnum), d, true).ToArray();
        }

        public string GetDescrIndAproCred(int indAproCred)
        {
            switch (indAproCred)
            {
                case (int)IndAproCredEnum.ApropriacaoDireta:
                    return "Método de Apropriação Direta";
                case (int)IndAproCredEnum.RateioProporcional:
                    return "Método de Rateio Proporcional (Receita Bruta)";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Tipo Contribuição Apurada Período

        public enum CodTipoContEnum
        {
            AliquotaBasica = 1,
            AliquotasEspecificas
        }

        public GenericModel[] GetCodTipoCont()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrCodTipoCont);
            return GetFromEnum(typeof(CodTipoContEnum), d, true).ToArray();
        }

        public string GetDescrCodTipoCont(int codTipoCont)
        {
            switch (codTipoCont)
            {
                case (int)CodTipoContEnum.AliquotaBasica:
                    return "Apuração da Contribuição Exclusivamente a Alíquota Básica";
                case (int)CodTipoContEnum.AliquotasEspecificas:
                    return "Apuração da Contribuição a Alíquotas Específicas (Diferenciadas e/ou por Unidade de Medida de Produto)";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Indicador Critério Escrituração Regime Cumulativo

        public GenericModel[] GetIndRegCum()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrIndRegCum);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.IndicadorRegimeCumulativo), d, true).ToArray();
        }

        public string GetDescrIndRegCum(int indRegCum)
        {
            return Colosoft.Translator.Translate((Sync.Fiscal.Enumeracao.IndicadorRegimeCumulativo)indRegCum).Format();
        }

        #endregion

        #region Código de Ajuste de Contribuição ou Crédito

        public enum CodAjusteContCredEnum
        {
            AjusteAcaoJudicial = 1,
            AjusteProcessoAdministrativo,
            AjusteLegislacaoTributaria,
            AjusteRTT,
            AjusteOutrasSituacoes,
            Estorno
        }

        public GenericModel[] GetCodAjusteContCred()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrCodAjusteContCred);
            return GetFromEnum(typeof(CodAjusteContCredEnum), d, false).ToArray();
        }

        public string GetDescrCodAjusteContCred(int? codAjusteContCred)
        {
            switch (codAjusteContCred)
            {
                case (int)CodAjusteContCredEnum.AjusteAcaoJudicial:
                    return "Ajuste Oriundo de Ação Judicial";
                case (int)CodAjusteContCredEnum.AjusteProcessoAdministrativo:
                    return "Ajuste Oriundo de Processo Administrativo";
                case (int)CodAjusteContCredEnum.AjusteLegislacaoTributaria:
                    return "Ajuste Oriundo da Legislação Tributária";
                case (int)CodAjusteContCredEnum.AjusteRTT:
                    return "Ajuste Oriundo Especificamente do RTT";
                case (int)CodAjusteContCredEnum.AjusteOutrasSituacoes:
                    return "Ajuste Oriundo de Outras Situações";
                case (int)CodAjusteContCredEnum.Estorno:
                    return "Estorno";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Enumeradores para a tabela Receitas Diversas

        #region Tipo Receita

        public GenericModel[] GetTipoReceita()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrTipoReceita);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.ReceitaDiversa.TipoReceita), d, false).ToArray();
        }

        public string GetDescrTipoReceita(int? tipoReceita)
        {
            switch (tipoReceita)
            {
                case (int)Sync.Fiscal.Enumeracao.ReceitaDiversa.TipoReceita.ReceitaFinanceira:
                    return "Receita financeira";
                case (int)Sync.Fiscal.Enumeracao.ReceitaDiversa.TipoReceita.JurosCapitalProprio:
                    return "Juros sobre o capital próprio";
                case (int)Sync.Fiscal.Enumeracao.ReceitaDiversa.TipoReceita.AluguelMoveisImoveis:
                    return "Aluguel de bens móveis e imóveis";
                case (int)Sync.Fiscal.Enumeracao.ReceitaDiversa.TipoReceita.ReceitaNaoOperacional:
                    return "Receita não operacional ";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Tipo Operação

        public GenericModel[] GetTipoOperacao()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrTipoOperacao);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.ReceitaDiversa.TipoOperacao), d, false).ToArray();
        }

        public string GetDescrTipoOperacao(int? tipoOperacao)
        {
            switch (tipoOperacao)
            {
                case (int)Sync.Fiscal.Enumeracao.ReceitaDiversa.TipoOperacao.OperRepAquisicao:
                    return "Operação Representativa de Aquisição, Custos, Despesa ou Encargos, ou Receitas, Sujeita à Incidência de Crédito de PIS/Pasep ou Cofins";
                case (int)Sync.Fiscal.Enumeracao.ReceitaDiversa.TipoOperacao.OperRepSujeitaPag:
                    return "Operação Representativa de Receita Auferida Sujeita ao Pagamento da Contribuição para o PIS/Pasep e da Cofins";
                case (int)Sync.Fiscal.Enumeracao.ReceitaDiversa.TipoOperacao.OperRepNaoSujeitaPag:
                    return "Operação Representativa de Receita Auferida Não Sujeita ao Pagamento da Contribuição para o PIS/Pasep e da Cofins";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region IndOrigemCred

        public GenericModel[] GetIndOrigemCred()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrIndOrigemCred);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.ReceitaDiversa.IndicadorOrigemCredito), d, false).ToArray();
        }

        public string GetDescrIndOrigemCred(int? indOrigemCred)
        {
            switch (indOrigemCred)
            {
                case (int)Sync.Fiscal.Enumeracao.ReceitaDiversa.IndicadorOrigemCredito.OperMercadoInterno:
                    return "Operação no Mercado Interno";
                case (int)Sync.Fiscal.Enumeracao.ReceitaDiversa.IndicadorOrigemCredito.OperImportacao:
                    return "Operação de Importação";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #endregion

        #region Enumeradores para a tabela Valores Retidos na Fonte

        #region NaturezaRetencao

        public GenericModel[] GetNaturezaRetencao()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrNaturezaRetencao);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaRetencao), d, false).ToArray();
        }

        public string GetDescrNaturezaRetencao(int? naturezaRetencao)
        {
            switch (naturezaRetencao)
            {
                case (int)Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaRetencao.RetPorOrgaos:
                    return "Retenção por Órgãos, Autarquias e Fundações Federais";
                case (int)Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaRetencao.RetOutrasEntidades:
                    return "Retenção por outras Entidades da Administração Pública Federal";
                case (int)Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaRetencao.RetPJuridica:
                    return "Retenção por Pessoas Jurídicas de Direito Privado";
                case (int)Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaRetencao.RecolPorSociedadeCoop:
                    return "Recolhimento por Sociedade Cooperativa";
                case (int)Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaRetencao.RetPorFabricanteMaqVeic:
                    return "Retenção por Fabricante de Máquinas e Veículos";
                case (int)Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaRetencao.OutrasRetencoes:
                    return "Outras Retenções";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region NaturezaReceita

        public GenericModel[] GetNaturezaReceita()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrNaturezaReceita);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaReceita), d, false).ToArray();
        }

        public string GetDescrNaturezaReceita(int? naturezaReceita)
        {
            switch (naturezaReceita)
            {
                case (int)Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaReceita.NaoCumulativa:
                    return "Receita de Natureza Não Cumulativa";
                case (int)Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaReceita.Cumulativa:
                    return "Receita de Natureza Cumulativa";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region TipoDeclarante

        public GenericModel[] GetTipoDeclarante()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrTipoDeclarante);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.ValorRetidoFonte.TipoDeclarante), d, false).ToArray();
        }

        public string GetDescrTipoDeclarante(int? tipoDeclarante)
        {
            switch (tipoDeclarante)
            {
                case (int)Sync.Fiscal.Enumeracao.ValorRetidoFonte.TipoDeclarante.BeneficiariaRetRecol:
                    return "Beneficiária da Retenção / Recolhimento";
                case (int)Sync.Fiscal.Enumeracao.ValorRetidoFonte.TipoDeclarante.ResponsavelRecol:
                    return "Responsável pelo Recolhimento";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #endregion

        #region Enumeradores para a tabela Deduções Diversas

        #region OrigemDeducao

        public GenericModel[] GetOrigemDeducao()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrOrigemDeducao);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.DeducaoDiversa.OrigemDeducao), d, false).ToArray();
        }

        public string GetDescrOrigemDeducao(int? origemDeducao)
        {
            switch (origemDeducao)
            {
                case (int)Sync.Fiscal.Enumeracao.DeducaoDiversa.OrigemDeducao.CredPreMedicamentos:
                    return "Créditos Presumidos - Medicamentos";
                case (int)Sync.Fiscal.Enumeracao.DeducaoDiversa.OrigemDeducao.CredAdmitidosRegCumulativo:
                    return "Créditos Admitidos no Regime Cumulativo – Bebidas Frias";
                case (int)Sync.Fiscal.Enumeracao.DeducaoDiversa.OrigemDeducao.ContPagaSubstTribut:
                    return "Contribuição Paga pelo Substituto Tributário - ZFM";
                case (int)Sync.Fiscal.Enumeracao.DeducaoDiversa.OrigemDeducao.SubstTributNaoOcorPre:
                    return "Substituição Tributária – Não Ocorrência do Fato Gerador Presumido";
                case (int)Sync.Fiscal.Enumeracao.DeducaoDiversa.OrigemDeducao.OutrasDeducoes:
                    return "Outras Deduções";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region NaturezaDeducao

        public GenericModel[] GetNaturezaDeducao()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrNaturezaDeducao);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.DeducaoDiversa.NaturezaDeducao), d, false).ToArray();
        }

        public string GetDescrNaturezaDeducao(int? naturezaDeducao)
        {
            switch (naturezaDeducao)
            {
                case (int)Sync.Fiscal.Enumeracao.DeducaoDiversa.NaturezaDeducao.NaoCumulativa:
                    return "Dedução de Natureza Não Cumulativa";
                case (int)Sync.Fiscal.Enumeracao.DeducaoDiversa.NaturezaDeducao.Cumulativa:
                    return "Dedução de Natureza Cumulativa";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #endregion

        #region Tipo de documento de importação

        public enum TipoDocImpEnum
        {
            DeclaracaoImportacao,
            DeclaracaoSimplificadaImportacao
        }

        public GenericModel[] GetTipoDocImp()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrTipoDocImp);
            return GetFromEnum(typeof(TipoDocImpEnum), d, false).ToArray();
        }

        public string GetDescrTipoDocImp(int? tipoDocImp)
        {
            switch (tipoDocImp)
            {
                case (int)TipoDocImpEnum.DeclaracaoImportacao:
                    return "Declaração de Importação";
                case (int)TipoDocImpEnum.DeclaracaoSimplificadaImportacao:
                    return "Declaração Simplificada de Importação";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Enumeradores para a tabela Ajuste Contribuição

        #region Tipo de imposto

        public enum TipoImpostoEnum
        {
            Pis = 1,
            Cofins,
            Icms,
            IcmsST,
            Ipi
        }

        public GenericModel[] GetTipoImposto()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoImposto);
            return GetFromEnum(typeof(TipoImpostoEnum), d, false).ToArray();
        }

        public string GetDescrTipoImposto(int tipoImposto)
        {
            switch (tipoImposto)
            {
                case (int)TipoImpostoEnum.Cofins: return "Cofins";
                case (int)TipoImpostoEnum.IcmsST: return "ICMS ST";
                default: return ((TipoImpostoEnum)tipoImposto).ToString().ToUpper();
            }
        }

        #endregion

        #region Tipo de ajuste

        public GenericModel[] GetTipoAjuste()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoAjuste);
            return GetFromEnum(typeof(Sync.Fiscal.Enumeracao.AjusteContribuicao.TipoAjuste), d, true).ToArray();
        }

        public string GetDescrTipoAjuste(int tipoAjuste)
        {
            switch (tipoAjuste)
            {
                case (int)Sync.Fiscal.Enumeracao.AjusteContribuicao.TipoAjuste.Acrescimo: return "Acréscimo";
                case (int)Sync.Fiscal.Enumeracao.AjusteContribuicao.TipoAjuste.Reducao: return "Redução";
                default: return String.Empty;
            }
        }

        #endregion

        #endregion

        #region Busca as alíquotas de imposto usadas durante um mês/ano

        /// <summary>
        /// Busca as alíquotas de imposto usadas durante um mês/ano.
        /// </summary>
        /// <param name="tipoImposto"></param>
        /// <param name="data">Uma data pertencente ao mês/ano buscados.</param>
        /// <returns></returns>
        public GenericModel[] GetAliquotasPisCofins(int tipoImposto, string data)
        {
            DateTime d = DateTime.Parse(data);

            string sql = @"select distinct pnf.aliq{0}
                from produtos_nf pnf
                    inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                where month(coalesce(if(nf.tipoDocumento<>{1}, nf.dataSaidaEnt, null), nf.dataEmissao))={2}
                    and year(coalesce(if(nf.tipoDocumento<>{1}, nf.dataSaidaEnt, null), nf.dataEmissao))={3}
                    and pnf.aliq{0} is not null";

            sql = String.Format(sql, 
                tipoImposto == (int)TipoImpostoEnum.Pis ? "Pis" : "Cofins",
                (int)NotaFiscal.TipoDoc.Saída, 
                d.Month, 
                d.Year);

            List<GenericModel> retorno = new List<GenericModel>();
            foreach (float f in ProdutosNfDAO.Instance.ExecuteMultipleScalar<float>(sql))
                retorno.Add(new GenericModel(null, f.ToString()));

            return retorno.ToArray();
        }

        #endregion

        #region Tipo de uso de crédito/contribuição

        public enum TipoUsoCredCont
        {
            Ambos,
            Entrada,
            Saída
        }

        #endregion

        #region Tipo de controle de saldo de créditos (ICMS)

        public GenericModel[] GetTipoControleSaldoCreditoIcms()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoControleSaldoCreditoIcms);
            return GetFromEnum(typeof(Sync.Fiscal.EFD.DataSources.TipoControleSaldoCreditoIcms), d, false).ToArray();
        }

        public string GetDescrTipoControleSaldoCreditoIcms(int tipoControle)
        {
            switch (tipoControle)
            {
                case (int)Sync.Fiscal.EFD.DataSources.TipoControleSaldoCreditoIcms.Nenhum: return "Nenhum";
                case (int)Sync.Fiscal.EFD.DataSources.TipoControleSaldoCreditoIcms.ContaCorrente: return "Conta Corrente";
                case (int)Sync.Fiscal.EFD.DataSources.TipoControleSaldoCreditoIcms.CreditoUtilizacaoLimitada: return "Crédito de Utilização Limitada";
                default: return "";
            }
        }

        #endregion

        #region Perfil do arquivo Fiscal

        public GenericModel[] GetPerfilArquivo()
        {
            Converter<int, string> d = GetDescrPerfilArquivo;
            return GetFromEnum(typeof(Sync.Fiscal.EFD.Configuracao.PerfilArquivo), d, false).ToArray();
        }

        public string GetDescrPerfilArquivo(int perfil)
        {
            return "Perfil " + ((Sync.Fiscal.EFD.Configuracao.PerfilArquivo)perfil).ToString();
        }

        #endregion

        #region Cód. Ajuste IPI

        public enum CodAjusteIpi
        {
            EstornoDebito = 1,
            CreditoRecebidoTransferencia,
            CreditoPresumidoIpiRessarcimentoPisCofinsLei9363 = 10,
            CreditoPresumidoIpiRessarcimentoPisCofinsLei10276,
            CreditoPresumidoIpiRegioesIncentivadas,
            CreditoPresumidoIpiFrete,
            CreditoPresumidoIpiOutros = 19,
            CreditosDecorrentesMedidaJudicial = 98,
            OutrosCreditos,
            EstornoCredito = 101,
            TransferenciaCredito,
            RessarcimentoOuCompensacaoCreditosIpi,
            OutrosDebitos = 199
        }

        public GenericModel[] GetCodAjusteIpi()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrCodAjusteIpi);
            return GetFromEnum(typeof(CodAjusteIpi), d, false).ToArray();
        }

        public string GetDescrCodAjusteIpi(int codAjusteIpi)
        {
            switch (codAjusteIpi)
            {
                case (int)CodAjusteIpi.EstornoDebito:
                    return "Valor do débito do IPI estornado";
                case (int)CodAjusteIpi.CreditoRecebidoTransferencia: 
                    return "Valor do crédito do IPI recebidos por transferência, de outro(s) estabelecimento(s) da mesma empresa";
                case (int)CodAjusteIpi.CreditoPresumidoIpiRessarcimentoPisCofinsLei9363: 
                    return "Valor do crédito presumido de IPI decorrente do ressarcimento do PIS/Pasep e da Cofins nas operações de exportação de produtos industrializados (Lei nº 9.363/1996, art. 1º)";
                case (int)CodAjusteIpi.CreditoPresumidoIpiRessarcimentoPisCofinsLei10276: 
                    return "Valor do crédito presumido de IPI decorrente do ressarcimento do PIS/Pasep e da Cofins nas operações de exportação de produtos industrializados (Lei nº 10.276/2001, art. 1º)";
                case (int)CodAjusteIpi.CreditoPresumidoIpiRegioesIncentivadas: 
                    return "Valor do crédito presumido relativo ao IPI incidente nas saídas, do estabelecimento industrial, dos produtos classificados nas posições 8702 a 8704 da Tipi (Lei nº 9.826/1999, art. 1º)";
                case (int)CodAjusteIpi.CreditoPresumidoIpiFrete: 
                    return "Valor do crédito presumido de IPI relativamente à parcela do frete cobrado pela prestação do serviço de transporte dos produtos classificados nos códigos 8433.53.00, 8433.59.1, 8701.10.00, 8701.30.00, 8701.90.00, 8702.10.00 Ex 01, 8702.90.90 Ex 01, 8703, 8704.2, 8704.3 e 87.06.00.20, da TIPI (MP nº 2.158/2001, art. 56)";
                case (int)CodAjusteIpi.CreditoPresumidoIpiOutros: 
                    return "Outros valores de crédito presumido de IPI";
                case (int)CodAjusteIpi.CreditosDecorrentesMedidaJudicial:
                    return "Valores de crédito de IPI decorrentes de medida judicial";
                case (int)CodAjusteIpi.OutrosCreditos:
                    return "Valor de outros créditos do IPI";
                case (int)CodAjusteIpi.EstornoCredito:
                    return "Valor do crédito do IPI estornado";
                case (int)CodAjusteIpi.TransferenciaCredito: 
                    return "Valor do crédito do IPI transferido no período, para outro(s) estabelecimento(s) da mesma empresa, conforme previsto na legislação tributária";
                case (int)CodAjusteIpi.RessarcimentoOuCompensacaoCreditosIpi:
                    return "Valor do crédito de IPI, solicitado junto à RFB/MF";
                case (int)CodAjusteIpi.OutrosDebitos:
                    return "Valor de outros débitos do IPI";
                default: return "";
            }
        }

        #endregion

        /// <summary>
        /// Indicador da origem do processo
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetIndProc()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel((uint)AjusteApuracaoInfoAdicional.IndProcEnum.JusticaEstadual, "Justiça Estadual"));
            lstRetorno.Add(new GenericModel((uint)AjusteApuracaoInfoAdicional.IndProcEnum.JusticaFederal, "Justiça Federal"));
            lstRetorno.Add(new GenericModel((uint)AjusteApuracaoInfoAdicional.IndProcEnum.Outros, "Outros"));
            lstRetorno.Add(new GenericModel((uint)AjusteApuracaoInfoAdicional.IndProcEnum.Sefaz, "Sefaz"));

            return lstRetorno.ToArray();
        }

        public List<TabelaSped> GetTabelaDocumentosFiscaisICMS()
        {
            List<TabelaSped> lstRetorno = new List<TabelaSped>();

            lstRetorno.Add(new TabelaSped("01", "Nota Fiscal"));
            lstRetorno.Add(new TabelaSped("1B", "Nota Fiscal Avulsa"));
            lstRetorno.Add(new TabelaSped("02", "Nota Fiscal de Venda a Consumidor"));
            lstRetorno.Add(new TabelaSped("2D", "Cupom Fiscal"));
            lstRetorno.Add(new TabelaSped("2E", "Cupom Fiscal Bilhete de Passagem"));
            lstRetorno.Add(new TabelaSped("04", "Nota Fiscal de Produtor"));
            lstRetorno.Add(new TabelaSped("06", "Nota Fiscal/Conta de Energia Elétrica"));
            lstRetorno.Add(new TabelaSped("07", "Nota Fiscal de Serviço de Transporte"));
            lstRetorno.Add(new TabelaSped("08", "Conhecimento de Transporte Rodoviário de Cargas"));
            lstRetorno.Add(new TabelaSped("8B", "Conhecimento de Transporte de Cargas Avulso"));
            lstRetorno.Add(new TabelaSped("09", "Conhecimento de Transporte Aquaviário de Cargas"));
            lstRetorno.Add(new TabelaSped("10", "Conhecimento Aéreo"));
            lstRetorno.Add(new TabelaSped("11", "Conhecimento de Transporte Ferroviário de Cargas"));
            lstRetorno.Add(new TabelaSped("13", "Bilhete de Passagem Rodoviário"));
            lstRetorno.Add(new TabelaSped("14", "Bilhete de Passagem Aquaviário"));
            lstRetorno.Add(new TabelaSped("15", "Bilhete de Passagem e Nota de Bagagem"));
            lstRetorno.Add(new TabelaSped("17", "Despacho de Transporte"));
            lstRetorno.Add(new TabelaSped("18", "Resumo de Movimento Diário"));
            lstRetorno.Add(new TabelaSped("20", "Ordem de Coleta de Cargas"));
            lstRetorno.Add(new TabelaSped("21", "Nota Fiscal de Serviço de Comunicação"));
            lstRetorno.Add(new TabelaSped("22", "Nota Fiscal de Serviço de Telecomunicação"));
            lstRetorno.Add(new TabelaSped("23", "GNRE"));
            lstRetorno.Add(new TabelaSped("24", "Autorização de Carregamento e Transporte"));
            lstRetorno.Add(new TabelaSped("25", "Manifesto de Carga"));
            lstRetorno.Add(new TabelaSped("26", "Conhecimento de Transporte Multimodal de Cargas"));
            lstRetorno.Add(new TabelaSped("27", "Nota Fiscal De Transporte Ferroviário De Carga"));
            lstRetorno.Add(new TabelaSped("28", "Nota Fiscal/Conta de Fornecimento de Gás Canalizado"));
            lstRetorno.Add(new TabelaSped("29", "Nota Fiscal/Conta De Fornecimento D'água Canalizada"));
            lstRetorno.Add(new TabelaSped("55", "Nota Fiscal Eletrônica"));
            lstRetorno.Add(new TabelaSped("57", "Conhecimento de Transporte Eletrônico - CT-e"));
            lstRetorno.Add(new TabelaSped("59", "Cupom Fiscal Eletrônico - CF-e"));

            return lstRetorno;
        }

        public List<TabelaSped> GetTabelaCodigoObrigacaoICMS()
        {
            List<TabelaSped> lstRetorno = new List<TabelaSped>();

            lstRetorno.Add(new TabelaSped("000", "ICMS a recolher"));
            lstRetorno.Add(new TabelaSped("001", "ICMS da substituição tributária pelas entradas"));
            lstRetorno.Add(new TabelaSped("002", "ICMS da substituição tributária pelas saídas para o Estado"));
            lstRetorno.Add(new TabelaSped("003", "Antecipação do diferencial de alíquotas do ICMS"));
            lstRetorno.Add(new TabelaSped("004", "Antecipação do ICMS da importação"));
            lstRetorno.Add(new TabelaSped("005", "Antecipação tributária"));
            lstRetorno.Add(new TabelaSped("006", "ICMS resultante da alíquota adicional dos itens incluídos no Fundo de Combate à Pobreza"));
            lstRetorno.Add(new TabelaSped("090", "Outras obrigações do ICMS"));
            lstRetorno.Add(new TabelaSped("999", "ICMS da substituição tributária pelas saídas para outro Estado"));

            return lstRetorno;
        }

        public GenericModel[] GetTipoImpostoSPED()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel((uint)ConfigEFD.TipoImpostoEnum.ICMS, "ICMS"));
            lstRetorno.Add(new GenericModel((uint)ConfigEFD.TipoImpostoEnum.ICMSST, "ICMS ST"));
            lstRetorno.Add(new GenericModel((uint)ConfigEFD.TipoImpostoEnum.IPI, "IPI"));

            return lstRetorno.ToArray();
        }

        public GenericModel[] GetIndicadorOrigemDocumento()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel((uint)ConfigEFD.IndicadorOrigemDocumentoEnum.Outros, "Outros."));
            lstRetorno.Add(new GenericModel((uint)ConfigEFD.IndicadorOrigemDocumentoEnum.PER_DCOMP, "PER/DCOMP"));
            lstRetorno.Add(new GenericModel((uint)ConfigEFD.IndicadorOrigemDocumentoEnum.ProcessoAdministrativo, "Processo Administrativo"));
            lstRetorno.Add(new GenericModel((uint)ConfigEFD.IndicadorOrigemDocumentoEnum.ProcessoJudicial, "Processo Judicial"));

            return lstRetorno.ToArray();
        }

        public GenericModel[] GetIndicadorTipoAjuste()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel((uint)ConfigEFD.TipoAjusteEnum.AjusteDebito, "Ajuste a Débito"));
            lstRetorno.Add(new GenericModel((uint)ConfigEFD.TipoAjusteEnum.AjusteCredito, "Ajuste a Crédito"));

            return lstRetorno.ToArray();
        }
    }
}