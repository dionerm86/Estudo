using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace WebGlass.Business.FCI.Fluxo
{
    public class ArquivoFCIFluxo : BaseFluxo<ArquivoFCIFluxo>
    {
        /// <summary>
        /// Retorna a lista de arquivos FCI gerados pelo sistema
        /// </summary>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<Glass.Data.Model.ArquivoFCI> ObterLista(string sortExpression, int startRow, int pageSize)
        {
            return Glass.Data.DAL.ArquivoFCIDAO.Instance.ObterLista(sortExpression, startRow, pageSize);
        }

        /// <summary>
        /// Retorna o numero de arquivos FCI gerados pelo sistema
        /// </summary>
        /// <returns></returns>
        public int ObterListaCount()
        {
            return Glass.Data.DAL.ArquivoFCIDAO.Instance.ObterListaCount();
        }

        #region Inserir

        /// <summary>
        /// Gera um novo arquivo FCI e seu produtos
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        public uint GerarArquivoFci(string dados)
        {
            uint idArquivoFci = 0;
            var lstProdArquivoFci = new List<uint>();

            try
            {
                idArquivoFci = ArquivoFCIDAO.Instance.Insert(new Glass.Data.Model.ArquivoFCI()
                {
                    UsuCad = UserInfo.GetUserInfo.CodUser,
                    DataCad = DateTime.Now
                });

                foreach (var item in dados.Split('$'))
                {
                    var d = item.Split(';');

                    if (d.Length < 4)
                        continue;

                    var idProd = Glass.Conversoes.StrParaUint(d[0]);
                    var parcelaImportada = Glass.Conversoes.StrParaDecimal(d[1]);
                    var saidaInterestadual = Glass.Conversoes.StrParaDecimal(d[2]);
                    var conteudoImportacao = Glass.Conversoes.StrParaDecimal(d[3]);
                    var idProdNf = Glass.Conversoes.StrParaUint(d[4]);

                    lstProdArquivoFci.Add(ProdutosArquivoFCIDAO.Instance.Insert(new Glass.Data.Model.ProdutosArquivoFCI()
                    {
                        IdArquivoFCI = idArquivoFci,
                        IdProd = idProd,
                        ParcelaImportada = parcelaImportada,
                        SaidaInterestadual = saidaInterestadual,
                        ConteudoImportacao = conteudoImportacao,
                        IdProdNf = idProdNf > 0 ? idProdNf : (uint?)null
                    }));
                }

                return idArquivoFci;
            }
            catch (Exception ex)
            {
                //TODO apagar se der erro.//kadu
                throw ex;
            }
        }

        #endregion

        #region Deletar arquivo FCI

        /// <summary>
        /// Deleta uma OC
        /// </summary>
        /// <param name="objDelete"></param>
        /// <returns></returns>
        public int Delete(Glass.Data.Model.ArquivoFCI objDelete)
        {
            if (objDelete.IdArquivoFCI == 0)
                throw new Exception("Nenhum arquivo FCI foi informado.");

            //Deleta os produtos do arquivo da FCI.
            ProdutosArquivoFCIDAO.Instance.DeleteByArquivoFCI(objDelete.IdArquivoFCI);

            return ArquivoFCIDAO.Instance.DeleteByPrimaryKey(objDelete.IdArquivoFCI);
        }

        #endregion

        #region Importar arquivo FCI

        /// <summary>
        /// Importa arquivo de retorno da FCI
        /// </summary>
        /// <param name="idArquivoFci"></param>
        /// <param name="arqRetorno"></param>
        public void ImportaArqRetorno(uint idArquivoFci, byte[] arqRetorno)
        {
            //Valida se o fci foi informado
            if (idArquivoFci == 0)
                throw new Exception("Informe o FCI.");

            //Valida se o arquivo de retorno foi informado
            if (arqRetorno == null || arqRetorno.Length == 0)
                throw new Exception("Informe o arquivo de retorno.");

            try
            {
                var enc = new System.Text.UTF8Encoding();
                var arqRetornoStr = enc.GetString(arqRetorno);
                var arrArqRetorno = arqRetornoStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                if (arrArqRetorno.Length == 0 || arrArqRetorno[0].Split('|').Length < 6)
                    throw new Exception("Arquivo invalido.");

                //Busca o numero de protocolo de envio no arquivo de retorno
                uint numProtocolo = Glass.Conversoes.StrParaUint(arrArqRetorno[0].Split('|')[6]);

                //Verifica se encontrou o protocolo
                if (numProtocolo == 0)
                    throw new Exception("Arquivo invalido. O número de protocolo não foi encontrado.");

                //Busca os produtos no arquivo de retorno
                var prodsArqRetorno = (from ar in arrArqRetorno
                                       where ar.Split('|').Length == 11 && ar.Split('|')[0].Equals("5020")
                                       select new
                                       {
                                           CodProd = ar.Split('|')[3],
                                           ConteudoImportacao = ar.Split('|')[8],
                                           NumControleFci = ar.Split('|')[9],
                                       }).ToList();

                //verifica se tem produtos no retorno
                if (prodsArqRetorno.Count == 0)
                    throw new Exception("Arquivo invalido. Nenhum produto foi encontrado.");

                //Busca o arquivo da FCI
                var arqFci = ArquivoFCIDAO.Instance.GetElement(idArquivoFci);

                //Busca os produtos da FCI
                var lstProd = ProdutosArquivoFCIDAO.Instance.GetByIdArquivoFci(arqFci.IdArquivoFCI);

                //Percorre os produdos do arquivo de retorno
                foreach (var p in prodsArqRetorno)
                {
                    //Id do produto do arquivo de retorno
                    var idProd = ProdutoDAO.Instance.ObtemIdProd(p.CodProd);

                    //Conteúdo de importação do arquivo de retorno
                    var conteudoImportacao = Glass.Conversoes.StrParaDecimal(p.ConteudoImportacao);

                    #region Atualiza numero de controle da FCI do produto

                    var prod = (from pr in lstProd
                                where pr.IdProd == idProd &&
                                pr.ConteudoImportacao == conteudoImportacao
                                select pr).FirstOrDefault();

                    if (prod == null)
                        throw new Exception("O Produto " + ProdutoDAO.Instance.ObtemDescricao(idProd) +
                            " do arquivo de retorno não foi encontrado no arquivo da FCI gerado.");

                    ProdutosArquivoFCIDAO.Instance.AtualizaNumControleFci(prod.IdProdArquivoFCI, p.NumControleFci);

                    //Se o produto da fci tiver sido gerado de uma nf atualiza o produto da mesma.
                    if (prod.IdProdNf.GetValueOrDefault(0) > 0)
                    {
                        ProdutosNfDAO.Instance.AtualizaDadosFCIProdutoNf(prod.IdProdNf.Value, prod.ParcelaImportada, prod.SaidaInterestadual,
                            prod.ConteudoImportacao, p.NumControleFci);
                    }

                    #endregion
                }

                //Atualizao arquivo da FCI
                arqFci.Protocolo = numProtocolo;
                arqFci.UsuImportacao = UserInfo.GetUserInfo.CodUser;
                arqFci.DataImportacao = DateTime.Now;
                ArquivoFCIDAO.Instance.Update(arqFci);

                //Salva o arquivo
                using (var f = System.IO.File.Create(Utils.GetArquivoFCIPath + idArquivoFci + "_retorno.txt"))
                {
                    using (System.IO.StreamWriter w = new System.IO.StreamWriter(f))
                        w.Write(arqRetornoStr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
