using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using System.Collections;
using Glass.Data.Model.Cte;
using Glass.Data.DAL.CTe;

namespace Glass.Data.SIntegra
{
    public sealed class SIntegra : Glass.Pool.PoolableObject<SIntegra>
    {
        private SIntegra() { }

        #region Registro 10

        private Registro10 RecuperaRegistro10(Loja loja, DateTime inicio, DateTime fim, Registro10.TipoNaturezaOperacoes natureza, Registro10.TipoFinalidade finalidade)
        {
            return new Registro10(loja, inicio, fim, natureza, finalidade);
        }

        #endregion

        #region Registro 11

        private Registro11 RecuperaRegistro11(Loja loja)
        {
            return new Registro11(loja);
        }

        #endregion

        #region Registro 50

        private Registro50[] RecuperaRegistros50(IEnumerable<NotaFiscal> notasFiscais)
        {
            List<Registro50> retorno = new List<Registro50>();
                        
            foreach (NotaFiscal n in notasFiscais)
                if (n.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada)
                    retorno.Add(new Registro50(n, n.IdCfop.Value));
                else if (!n.Transporte)
                {
                    var lstProd = ProdutosNfDAO.Instance.GetProdNfByCfopAliq(n.IdNf);
                    var lstCfopAliqAdicionado = new List<string>();

                    // Se os produtos dessa nota possuir apenas um CFOP/Aliq. ICMS, inclui apenas um registro da NFe
                    if (lstProd == null || lstProd.Count == 0)
                        retorno.Add(new Registro50(n, n.IdCfop.Value));
                    else
                    {
                        uint? idCfopOriginal = n.IdCfop;
                        float? aliqIcmsOriginal = n.AliqIcms;

                        uint idCfopUtilizar = n.IdCfop.Value;

                        // Gera um registro 50 para cada cfop da nota
                        foreach (ProdutosNf pnf in lstProd) // 10,20
                        {
                            // Se o produto possuir CFOP, utiliza-o
                            if (pnf.IdCfop != null)
                                idCfopUtilizar = pnf.IdCfop.Value;

                            // Atribui o valor da Aliq. ICMS do produto na nota
                            n.AliqIcms = pnf.AliqIcms;

                            // Se um registro com determinado CFOP + AliqICMS já tiver sido adicionado, não adiciona novamente
                            if (lstCfopAliqAdicionado.Contains(idCfopUtilizar.ToString() + n.AliqIcms.Value.ToString()))
                                continue;

                            // Adiciona este CFOP + AliqICMS na variável para que não seja adicionado novamente
                            lstCfopAliqAdicionado.Add(idCfopUtilizar.ToString() + n.AliqIcms.Value.ToString());

                            // Busca o total da nota baseado no CFOP do foreach
                            if (!n.IsNfImportacao)
                            {
                                // Se a nota possuir apenas um produto, mantém os valores que já estavam na nota,
                                // o motivo disso é caso a nota venha errada do fornecedor, mantenha este valor incorreto
                                if (lstProd.Count > 1)
                                {
                                    n.TotalNota = ProdutosNfDAO.Instance.GetTotalByCfopAliq(n.IdNf, idCfopUtilizar, n.AliqIcms.Value);
                                    n.BcIcms = ProdutosNfDAO.Instance.GetBcIcmsByCfopAliq(n.IdNf, idCfopUtilizar, n.AliqIcms.Value);
                                    n.Valoricms = ProdutosNfDAO.Instance.GetValorIcmsByCfopAliq(n.IdNf, idCfopUtilizar, n.AliqIcms.Value);
                                }
                            }

                            retorno.Add(new Registro50(n, idCfopUtilizar));

                            idCfopUtilizar = idCfopOriginal.Value;
                            n.AliqIcms = aliqIcmsOriginal;
                        }
                    }
                }

            return retorno.ToArray();
        }

        #endregion

        #region Registro 51

        private Registro51[] RecuperaRegistros51(IEnumerable<NotaFiscal> notasFiscais)
        {
            List<Registro51> retorno = new List<Registro51>();

            // Só permite as notas fiscais de modelo 1 e 1A no registro 51
            // porque estava dando erro na validação do Sintegra
            var modelos = new List<string>() { "1", "01", "1A" };

            foreach (NotaFiscal n in notasFiscais)
                if (!n.Transporte && (n.Serie == null || n.Serie.ToUpper() != "U") && modelos.Contains(n.Modelo))
                    retorno.Add(new Registro51(n));

            return retorno.ToArray();
        }

        #endregion

        #region Registro 53

        private Registro53[] RecuperaRegistros53(IEnumerable<NotaFiscal> notasFiscais)
        {
            List<Registro53> retorno = new List<Registro53>();

            foreach (NotaFiscal n in notasFiscais)
                if (!n.Transporte && n.BcIcmsSt > 0)
                    retorno.Add(new Registro53(n));

            return retorno.ToArray();
        }

        #endregion

        #region Registro 54

        private Registro54[] RecuperaRegistros54(IEnumerable<NotaFiscal> notasFiscais)
        {
            List<Registro54> retorno = new List<Registro54>();

            foreach (NotaFiscal n in notasFiscais)
            {
                if (n.Transporte)
                    continue;

                ProdutosNf[] p = ProdutosNfDAO.Instance.GetByNf(n.IdNf);
                for (int i = 0; i < p.Length; i++)
                    retorno.Add(new Registro54(p[i], i + 1));
            }

            return retorno.ToArray();
        }

        #endregion

        #region Registro 61

        private Registro61[] RecuperaRegistros61(IEnumerable<NotaFiscal> notasFiscais)
        {
            List<Registro61> retorno = new List<Registro61>();

            foreach (NotaFiscal n in notasFiscais)
                if (!n.Transporte)
                    retorno.Add(new Registro61(n));

            return retorno.ToArray();
        }

        private Registro61R[] RecuperaRegistros61R(IEnumerable<NotaFiscal> notasFiscais, int mes, int ano)
        {
            List<Registro61R> retorno = new List<Registro61R>();

            var produtos = new Dictionary<uint, Produto>();

            foreach (NotaFiscal n in notasFiscais)
            {
                if (n.Transporte)
                    continue;

                ProdutosNf[] p = ProdutosNfDAO.Instance.GetByNf(n.IdNf);

                for (int i = 0; i < p.Length; i++)
                {
                    if (!produtos.ContainsKey(p[i].IdProd))
                        produtos.Add(p[i].IdProd, ProdutoDAO.Instance.GetElement(null, p[i].IdProd, (int)n.IdNf, (uint)n.IdLoja, n.IdCliente, n.IdFornec,
                            (n.TipoDocumento == (int)Glass.Data.Model.NotaFiscal.TipoDoc.Saída ||
                            (n.TipoDocumento == (int)Glass.Data.Model.NotaFiscal.TipoDoc.Entrada &&
                            CfopDAO.Instance.IsCfopDevolucao((uint)n.IdCfop)))));

                    retorno.Add(new Registro61R(produtos[p[i].IdProd], mes, ano));
                }
            }

            return retorno.ToArray();
        }

        #endregion

        #region Registro 70

        private Registro70[] RecuperaRegistros70(IEnumerable<NotaFiscal> notasFiscais, 
            IEnumerable<ConhecimentoTransporte> conhecimentosTransporte)
        {
            List<Registro70> retorno = new List<Registro70>();

            foreach (NotaFiscal n in notasFiscais)
                if (n.Transporte)
                    retorno.Add(new Registro70(n));

            foreach (ConhecimentoTransporte c in conhecimentosTransporte)
                retorno.Add(new Registro70(c));

            return retorno.ToArray();
        }

        #endregion

        #region Registro 74

        private Registro74[] RecuperaRegistros74(DateTime dataAtual, uint idLoja, ref string idsProdGerado)
        {
            List<Registro74> retorno = new List<Registro74>();

            DateTime dataInventario = DateTime.Parse("01/01/" + dataAtual.Year).AddDays(-1);
            Loja loja = LojaDAO.Instance.GetElement(idLoja);

            foreach (InventarioSintegra inv in InventarioSintegraDAO.Instance.ObtemInvetario(dataAtual, idLoja))
            {
                retorno.Add(new Registro74(dataInventario, inv.CodInterno, (float)inv.Qtd, (float)inv.Total, loja.Cnpj, loja.InscEst, loja.Uf));
                idsProdGerado += ProdutoDAO.Instance.ObtemIdProd(inv.CodInterno) + ",";
            }

            idsProdGerado = idsProdGerado.TrimEnd(',');

            return retorno.ToArray();
        }

        #endregion

        #region Registro 75

        private Registro75[] RecuperaRegistros75(IEnumerable<NotaFiscal> notasFiscais, string idsProdReg74, DateTime inicio, DateTime fim)
        {
            List<uint> idProdutosExistentes = new List<uint>();
            List<Registro75> retorno = new List<Registro75>();
            uint? idLojaNf = null;

            // Gera registros 75 para cada produtos das notas fiscais do período passado
            foreach (NotaFiscal n in notasFiscais)
            {
                if (n.Transporte)
                    continue;

                if (idLojaNf.GetValueOrDefault() == 0)
                    idLojaNf = n.IdLoja.GetValueOrDefault();

                ProdutosNf[] p = ProdutosNfDAO.Instance.GetByNf(n.IdNf);
                for (int i = 0; i < p.Length; i++)
                {
                    if (idProdutosExistentes.Contains(p[i].IdProd))
                        continue;

                    idProdutosExistentes.Add(p[i].IdProd);
                    retorno.Add(new Registro75(ProdutosNfDAO.Instance.GetElement(p[i].IdProdNf), idLojaNf.Value, inicio, fim));
                }
            }

            // Gera registros 75 para os produtos informados no registro 74
            if (!String.IsNullOrEmpty(idsProdReg74))
            {
                string[] vetIdProdReg74 = idsProdReg74.Split(',');
                foreach (string idProd in vetIdProdReg74)
                {
                    if (idProdutosExistentes.Contains(Glass.Conversoes.StrParaUint(idProd)))
                        continue;

                    idProdutosExistentes.Add(Glass.Conversoes.StrParaUint(idProd));
                    retorno.Add(new Registro75(ProdutoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idProd)), idLojaNf.Value, inicio, fim));
                }
            }

            return retorno.ToArray();
        }

        #endregion

        public string RecuperaArquivoRegistros(uint idLoja, DateTime inicio, DateTime fim, bool registro50, bool registro51,
            bool registro53, bool registro54, bool registro61, bool registro70, bool registro74, bool registro75)
        {
            StringBuilder retorno = new StringBuilder();
            List<NotaFiscal> notasFiscais = NotaFiscalDAO.Instance.GetForSintegra(idLoja, inicio.ToString("dd/MM/yyyy"), fim.ToString("dd/MM/yyyy"), false);
            List<ConhecimentoTransporte> conhecimentosTransporte = ConhecimentoTransporteDAO.Instance.GetForSintegra(idLoja, inicio.ToString("dd/MM/yyyy"), fim.ToString("dd/MM/yyyy"), false, true);

            var lojas = new Dictionary<uint, Loja>();

            if (notasFiscais.Count == 0)
                throw new Exception("Não há nenhuma nota cadastrada para a loja selecionada.");

            foreach (NotaFiscal nota in notasFiscais)
            {
                if (!lojas.ContainsKey(nota.IdLoja.Value))
                    lojas.Add(nota.IdLoja.Value, LojaDAO.Instance.GetElement(nota.IdLoja.Value));

                Loja loja = lojas[nota.IdLoja.Value];

                #region Valida dados Emitente

                    if (loja.RazaoSocial.Length > 35)
                        throw new Exception("O nome do emitente " + loja.RazaoSocial + " da nota " + nota.NumeroNFe + " deve possuir até 35 caracteres");

                    if (loja.Bairro.Length > 15)
                        throw new Exception("O bairro do emitente " + loja.RazaoSocial + " da nota " + nota.NumeroNFe + " deve possuir até 15 caracteres");

                    if (loja.Compl != null && loja.Compl.Length > 22)
                        throw new Exception("O complemento do emitente " + loja.RazaoSocial + " da nota " + nota.NumeroNFe + " deve possuir até 22 caracteres");

                    if (loja.Endereco.Length > 34)
                        throw new Exception("O endereço do emitente " + nota.NomeEmitente + " da nota " + nota.NumeroNFe + " deve possuir até 34 caracteres");

                    #endregion

                #region Valida CFOP

                //Cfop cfop = CfopDAO.Instance.GetCfop(nota.CodCfop);

                ////Se o cfop calcula ICMS 
                //if (cfop.CalcIcms && (nota.BcIcms == 0 || nota.Valoricms == 0 || (nota.AliqIcms != null && nota.AliqIcms.Value == 0)))
                //    throw new Exception("O CFOP da nota calcula ICMS, porém um ou mais valores referentes à esse imposto não está preenchido. Nota Fiscal: " + nota.NumeroNFe);
                
                //if (cfop.CalcIcmsSt && (nota.BcIcmsSt == 0 || nota.ValorIcmsSt == 0))
                //    throw new Exception("O CFOP da nota calcula ICMS-ST, porém um ou mais valores referentes à esse imposto não está preenchido. Nota Fiscal: " + nota.NumeroNFe);

                //if (cfop.CalcIpi && (nota.ValorIpi == 0))
                //    throw new Exception("O CFOP da nota calcula IPI, porém um ou mais valores referentes à esse imposto não está preenchido. Nota Fiscal: " + nota.NumeroNFe);

                ////se não calcula
                ////Se o cfop calcula ICMS 
                //if (!cfop.CalcIcms && (nota.BcIcms > 0 || nota.Valoricms > 0 || (nota.AliqIcms != null && nota.AliqIcms.Value < 1)))
                //    throw new Exception("O CFOP da nota não calcula ICMS, porém um ou mais valores referentes à esse imposto está preenchido. Nota Fiscal: " + nota.NumeroNFe);

                //if (!cfop.CalcIcmsSt && (nota.BcIcmsSt > 0 || nota.ValorIcmsSt > 0))
                //    throw new Exception("O CFOP da nota não calcula ICMS-ST, porém um ou mais valores referentes à esse imposto está preenchido. Nota Fiscal: " + nota.NumeroNFe);

                //if (!cfop.CalcIpi && (nota.ValorIpi > 0))
                //    throw new Exception("O CFOP da nota não calcula IPI, porém um ou mais valores referentes à esse imposto está preenchido. Nota Fiscal: " + nota.NumeroNFe);

                #endregion

                #region Valida IE

                if (nota.TipoDocumento == 1 && nota.IdFornec > 0)
                {
                    if (nota.IdFornec.GetValueOrDefault() > 0)
                    {
                        var fornecedor = FornecedorDAO.Instance.GetElement(nota.IdFornec.Value);

                        if (fornecedor.TipoPessoa == "J" &&
                            !Glass.Validacoes.ValidaIE(fornecedor.Uf, fornecedor.RgInscEst))
                            throw new Exception
                                ("Inscrição Estadual " + fornecedor.RgInscEst + " do fornecedor " + fornecedor.IdFornec + " - " +
                                    fornecedor.Razaosocial + " é inválida para UF " + fornecedor.Uf + ".");

                        if (!Glass.Validacoes.ValidaIE(loja.Uf, loja.InscEst))
                            throw new Exception("Inscrição Estadual do destinatário " + nota.NomeEmitente + " inválida.");
                    }
                    else if (nota.IdCliente.GetValueOrDefault() > 0)
                    {
                        var cliente = ClienteDAO.Instance.GetElement(nota.IdCliente.Value);

                        if (cliente.TipoPessoa == "J" && !Glass.Validacoes.ValidaIE(cliente.Uf, cliente.RgEscinst))
                            throw new Exception
                                ("Inscrição Estadual " + cliente.RgEscinst + " do cliente " + cliente.IdCli + " - " +
                                    cliente.Nome + " é inválida para UF " + cliente.Uf + ".");

                        if (!Glass.Validacoes.ValidaIE(loja.Uf, loja.InscEst))
                            throw new Exception("Inscrição Estadual do destinatário " + nota.NomeEmitente + " inválida.");
                    }
                }
                if (nota.TipoDocumento == 2)
                {
                    if (!Glass.Validacoes.ValidaIE(loja.Uf, loja.InscEst))
                        throw new Exception("Inscrição Estadual " + loja.InscEst + " do emitente " + nota.NomeEmitente + " inválida para UF " + loja.Uf + ".");
                }
                if (nota.TipoDocumento == 3)
                {
                    //Códigos CFOP referente à transporte
                    ArrayList cfopsTransporte = new ArrayList() { "1351", "1352", "1353", "1354", "1355", "1356", "1360", "2351", "2352", "2353", "2354", "2355", "2356", "3351", "3352", "3353", "3354", "3355", "3356", "1932", "2932" };

                    if (nota.Transporte)
                    {
                        //A nota sendo de transporte, verifica se o cfop condiz
                        if (!cfopsTransporte.Contains(nota.CodCfop))
                            throw new Exception("O CFOP " + nota.CodCfop + " da nota " + nota.NumeroNFe + " não corresponde à um CFOP de transporte.");

                        if (nota.IdTransportador == null)
                            throw new Exception("O transportador da nota " + nota.NumeroNFe + " não foi informado");
                        
                        Transportador transportador = TransportadorDAO.Instance.GetElement(nota.IdTransportador.Value);

                        if (!Glass.Validacoes.ValidaIE(transportador.Uf, transportador.InscEst))
                            throw new Exception("Inscrição Estadual do emitente " + nota.NomeEmitente + " inválida.");
                    }
                    else
                    {
                        //Não sendo de transporte, verifica o CFOP
                        if (cfopsTransporte.Contains(nota.CodCfop))
                        {
                            throw new Exception("O CFOP " + nota.CodCfop + " da nota " + nota.NumeroNFe + " corresponde à um CFOP de transporte, porém a nota não é de transporte.");
                        }
                    }

                    if (!Glass.Validacoes.ValidaIE(loja.Uf, loja.InscEst))
                        throw new Exception("Inscrição Estadual do destinatário " + nota.NomeEmitente + " inválida.");
                }

                #endregion

                #region Verifica unidade de medida de produto

                ProdutosNf[] produtos = ProdutosNfDAO.Instance.GetByNf(nota.IdNf);
                 foreach (ProdutosNf p in produtos )
                {
                    if(string.IsNullOrEmpty(p.Unidade))
                        throw new Exception("A unidade de medida do produto " + p.CodInterno + " - " + p.DescrProduto + " da nota " + nota.NumeroNFe + " não possui unidade de medida");
                }

                #endregion

                #region Verifica Modelo

                if (nota.Modelo.ToUpper().Contains("U"))
                    throw new Exception("O Modelo da nota fiscal " + nota.NumeroNFe + " não pode conter \"U\", apenas a série.");

                if (nota.Modelo.Length > 2)
                    throw new Exception("O Modelo da nota fiscal " + nota.NumeroNFe + " possui mais de 2 caracteres.");

                #endregion

                #region Aliquota de impostos

                 if (nota.AliqIcms < 1 && nota.AliqIcms > 0)
                     throw new Exception("Alíquota de ICMS não pode ser menor que 1. Nota " + nota.NumeroNFe);

                 #endregion
            }

            Registro10 registro10 = RecuperaRegistro10(lojas[idLoja], inicio, fim, Registro10.TipoNaturezaOperacoes.TodasAsOperacoes, Registro10.TipoFinalidade.Normal);
            Registro11 registro11 = RecuperaRegistro11(lojas[idLoja]);

            retorno.AppendLine(registro10.ToString());
            retorno.AppendLine(registro11.ToString());

            List<Registro> parametros = new List<Registro>();

            #region Registro 50

            if (registro50)
            {
                Registro50[] registros = RecuperaRegistros50(
                    NotaFiscalDAO.Instance.GetForSintegra(idLoja, inicio.ToString("dd/MM/yyyy"), fim.ToString("dd/MM/yyyy"), true).Where(f => !f.Consumidor).ToList());

                foreach (Registro50 r in registros)
                    retorno.AppendLine(r.ToString());

                parametros.AddRange(registros);
            }

            #endregion

            #region Registro 51

            if (registro51)
            {
                Registro51[] registros = RecuperaRegistros51(notasFiscais);
                foreach (Registro51 r in registros)
                    retorno.AppendLine(r.ToString());

                parametros.AddRange(registros);
            }

            #endregion

            #region Registro 53

            if (registro53)
            {
                Registro53[] registros = RecuperaRegistros53(notasFiscais);
                foreach (Registro53 r in registros)
                    retorno.AppendLine(r.ToString());

                parametros.AddRange(registros);
            }

            #endregion

            #region Registro 54

            if (registro54)
            {
                Registro54[] registros = RecuperaRegistros54(notasFiscais.Where(f => !f.Consumidor).ToList());
                foreach (Registro54 r in registros)
                    retorno.AppendLine(r.ToString());

                parametros.AddRange(registros);
            }

            #endregion

            #region Registro 61

            if (registro61)
            {
                //Registro61[] registros = RecuperaRegistros61(notasFiscais);
                /*Chamado 48036. */
                Registro61[] registros = RecuperaRegistros61(notasFiscais.Where(f => f.Consumidor));
                foreach (Registro61 r in registros)
                    retorno.AppendLine(r.ToString());

                parametros.AddRange(registros);

                Registro61R[] registrosR = RecuperaRegistros61R(notasFiscais.Where(f => f.Consumidor), fim.Month, fim.Year);
                foreach (Registro61R r in registrosR)
                    retorno.AppendLine(r.ToString());

                parametros.AddRange(registrosR);
            }

            #endregion

            #region Registro 70

            if (registro70)
            {
                Registro70[] registros = RecuperaRegistros70(notasFiscais, conhecimentosTransporte);
                foreach (Registro70 r in registros)
                    retorno.AppendLine(r.ToString());

                parametros.AddRange(registros);
            }

            #endregion

            #region Registro 74

            // Salva quais produtos foram gerados no registro 74 para adicioná-los no registro 75
            string idsProdReg74 = String.Empty;

            if (registro74)
            {
                Registro74[] registros = RecuperaRegistros74(fim, idLoja, ref idsProdReg74);
                foreach (Registro74 r in registros)
                    retorno.AppendLine(r.ToString());

                parametros.AddRange(registros);
            }

            #endregion

            #region Registro 75

            if (registro75)
            {
                Registro75[] registros = RecuperaRegistros75(notasFiscais, idsProdReg74, inicio, fim);
                foreach (Registro75 r in registros)
                    retorno.AppendLine(r.ToString());

                parametros.AddRange(registros);
            }

            #endregion

            Registro90 registro90 = new Registro90(lojas[idLoja], parametros.ToArray());
            retorno.AppendLine(registro90.ToString());

            return retorno.ToString().Trim();
        }

        /// <summary>
        /// Testa arquivo do sintegra
        /// </summary>
        /// <param name="registro"></param>
        /// <param name="idNf"></param>
        public void TestaArquivo(int registro, uint idNf)
        {
            switch (registro)
            {
                case 50:
                    RecuperaRegistros50(new NotaFiscal[] { NotaFiscalDAO.Instance.GetElementByPrimaryKey(idNf) })[0].ToString();
                    break;
                case 51:
                    RecuperaRegistros51(new NotaFiscal[] { NotaFiscalDAO.Instance.GetElementByPrimaryKey(idNf) })[0].ToString();
                    break;
                case 54:
                    RecuperaRegistros54(new NotaFiscal[] { NotaFiscalDAO.Instance.GetElementByPrimaryKey(idNf) })[0].ToString();
                    break;
            }
        }
    }
}