using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Glass.Configuracoes;
using GDA;
using System.ComponentModel;

namespace Glass.Data.Helper
{
    public static class Utils
    {
        //-----------------------------------------------------

        #region ExecScript (temporário - apagar depois)

        /// <summary>
        /// Calcula os valores brutos do produto.
        /// </summary>
        /// <param name="produto"></param>
        public static void CalculaValorBruto(object produto)
        {
            if (produto is IProdutoDescontoAcrescimo)
                DescontoAcrescimo.Calcular.Instance.CalculaValorBruto(produto as IProdutoDescontoAcrescimo);
        }

        public static void CorrigeMovEstoque()
        {
            string tipoCalc = "coalesce({0}s.tipoCalculo, g.tipoCalculo)";
            
            string sqlProd = @"select p.idProd from produto p
                inner join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                left join subgrupo_prod s on (s.idSubgrupoProd=p.idSubgrupoProd)
                where {0} in (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 + "," +
                    (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + @")
                    and exists (select * from mov_estoque{1} where idProd=p.idProd)";

            string sqlUpdate = @"update mov_estoque{0} m
                inner join produto p on (m.idProd=p.idProd)
                set m.valorMov={1}/6*m.qtdeMov
                where lancManual and m.idProd in ({2})";
                
            string sqlLoja = "select idLoja from mov_estoque{0} where idProd=";

            string sqlIdMovEstoque = @"select idMovEstoque{0} from mov_estoque{1}
                where idProd={2} and idLoja={3} order by dataMov asc, idMovEstoque{0} asc limit 1";
            
            string idsProd, idsLojas;
            uint idMovEstoque;

            #region Mov_Estoque
            
            idsProd = ProdutoDAO.Instance.GetValoresCampo(String.Format(sqlProd,
                String.Format(tipoCalc, ""), ""), "idProd");

            MovEstoqueDAO.Instance.ExecuteScalar<int>(String.Format(sqlUpdate, "", "p.custoCompra", idsProd));

            foreach (uint p in Array.ConvertAll(idsProd.Split(','), x => Glass.Conversoes.StrParaUint(x)))
            {
                idsLojas = MovEstoqueDAO.Instance.GetValoresCampo(String.Format(sqlLoja, "") + p, "idLoja");
                
                foreach (uint l in Array.ConvertAll(idsLojas.Split(','), x => Glass.Conversoes.StrParaUint(x)))
                {
                    idMovEstoque = MovEstoqueDAO.Instance.ExecuteScalar<uint>(
                        String.Format(sqlIdMovEstoque, "", "", p, l));

                    if (idMovEstoque > 0)
                        MovEstoqueDAO.Instance.AtualizaSaldo(idMovEstoque);
                }
            }

            #endregion

            #region Mov_Estoque_Fiscal

            idsProd = ProdutoDAO.Instance.GetValoresCampo(String.Format(sqlProd,
                String.Format(tipoCalc, "s.tipoCalculoNf, g.tipoCalculoNf, "), "_fiscal"), "idProd");

            MovEstoqueFiscalDAO.Instance.ExecuteScalar<int>(String.Format(sqlUpdate, "_fiscal",
                "if(p.valorFiscal>0, p.valorFiscal, p.custoCompra)", idsProd));

            foreach (uint p in Array.ConvertAll(idsProd.Split(','), x => Glass.Conversoes.StrParaUint(x)))
            {
                idsLojas = MovEstoqueFiscalDAO.Instance.GetValoresCampo(String.Format(sqlLoja, "_fiscal") + p, "idLoja");

                foreach (uint l in Array.ConvertAll(idsLojas.Split(','), x => Glass.Conversoes.StrParaUint(x)))
                {
                    idMovEstoque = MovEstoqueFiscalDAO.Instance.ExecuteScalar<uint>(
                        String.Format(sqlIdMovEstoque, "Fiscal", "_fiscal", p, l));

                    if (idMovEstoque > 0)
                        MovEstoqueFiscalDAO.Instance.AtualizaSaldo(null, idMovEstoque);
                }
            }

            #endregion
        }

        #endregion

        //-----------------------------------------------------

        #region Enumeradores

        public enum TipoBoleto : uint
        {
            Lumen = 1,
            Santander,
            BancoBrasil,
            Outros
        }     

        public enum TipoFuncionario : ushort
        {
            Gerente = 1,
            Vendedor,
            AuxAdministrativo,
            SupervisorProducao=14,
            MotoristaMedidor,               // 15
            MotoristaInstalador,
            InstaladorComum,
            InstaladorTemperado,
            Administrador,
            AuxAlmoxarifado=30,
            MarcadorProducao=196,
            AuxEtiqueta=201,
            Fiscal=202
        }        

        public enum SubgrupoProduto : int
        {
            Comum = 1,
            Temperado = 2,
            LevesDefeitos = 3,
            RetalhosProducao = 4,
            BoxPadrao = 5,
            Espelho = 6,
            SobrasDeVidro = 8
        }        

        /// <summary>
        /// Tipo de perda do vidro.
        /// </summary>
        public enum TipoPerda : uint
        {
            ErroCliente = 1,
            ErroVenda,
            ErroImpressao,
            FalhaComunicacao,
            ErroCorte,
            FalhaOperacional,
            FalhaMecanica,
            ErroMedidas,
            DefeitoFabrica,
            VidroManchado,
            VidroArranhado,
            QuebraManusear,
            FalhaArmazenamento,
            QuebraCaminhao,
            QuebraTempera,
            VidroDesaparecido,
            Outros,
            QuebraMaquina
        }

        #endregion        

        #region Descrição do tipo de perda

        ///// <summary>
        ///// Retorna a descrição para o tipo de perda.
        ///// </summary>
        ///// <param name="_tipoPerda"></param>
        ///// <returns></returns>
        //public static string GetDescrTipoPerda(TipoPerda tipoPerda)
        //{
        //    switch (tipoPerda)
        //    {
        //        case TipoPerda.ErroCliente: return "Erro do cliente";
        //        case TipoPerda.ErroVenda: return "Erro de venda";
        //        case TipoPerda.ErroImpressao: return "Erro de impressão";
        //        case TipoPerda.FalhaComunicacao: return "Falha de comunicação";
        //        case TipoPerda.ErroCorte: return "Erro de corte";
        //        case TipoPerda.FalhaOperacional: return "Falha operacional";
        //        case TipoPerda.FalhaMecanica: return "Falha mecânica";
        //        case TipoPerda.ErroMedidas: return "Erro de medidas";
        //        case TipoPerda.DefeitoFabrica: return "Defeito de fábrica";
        //        case TipoPerda.VidroManchado: return "Vidro manchado";
        //        case TipoPerda.VidroArranhado: return "Vidro arranhado";
        //        case TipoPerda.QuebraManusear: return "Quebra ao manusear";
        //        case TipoPerda.FalhaArmazenamento: return "Falha de armazenamento";
        //        case TipoPerda.QuebraCaminhao: return "Quebra no caminhão";
        //        case TipoPerda.QuebraTempera: return "Quebra na têmpera";
        //        case TipoPerda.QuebraMaquina: return "Quebra na máquina";
        //        case TipoPerda.VidroDesaparecido: return "Vidro desaparecido";
        //        case TipoPerda.Outros: return "Outros";
        //        default: return null;
        //    }
        //}

        #endregion
              
        #region Grava mensagem de erro em arquivo de log

        public static void LogError(Exception ex)
        {
            string log = HttpContext.Current.Server.MapPath("~/logErro.txt");

            if (!System.IO.File.Exists(log))
                System.IO.File.Create(log);

            string erro = DateTime.Now + ": " + ex.Message + " Fonte: " + ex.Source;

            if (ex.InnerException != null && ex.InnerException.Message != null)
                erro += "\r\nInner Exception: " + ex.InnerException.Message;

            erro += " \r\nStackTrace: " + ex.StackTrace + "\r\n";

            System.IO.FileInfo fi = new System.IO.FileInfo(log);
            System.IO.StreamWriter sw = fi.AppendText();
            sw.WriteLine(erro);
            sw.Close();
        }

        #endregion        
       
        #region Converte imagem em byte[]

        /// <summary>
        /// Obtém a URL do site
        /// </summary>
        /// <param name="urlSite"></param>
        /// <returns></returns>
        internal static string GetUrlSite(HttpContext context)
        {
            // Se o site possuir /glass, deve fazer um tratamento diferenciado
            string urlSite = context.Request.Url.AbsolutePath;

            if (context.Request.Url.Host.ToLower() == "localhost" &&
                (context.Request.Url.OriginalString.ToLower().Contains("/glass") ||
                context.Request.Url.OriginalString.ToLower().Contains("/webglass")))
            {
                // Remove o nome da pasta do IIS local (Glass, WebGlass...) da URL
                // buscada, mantendo-a na URL do site
                List<string> dadosUrl = new List<string>(urlSite.Split('/'));
                dadosUrl.RemoveAt(1);

                urlSite = String.Join("/", dadosUrl.ToArray());
            }

            var url = context.Request.Url.OriginalString.Remove(context.Request.Url.OriginalString.IndexOf(urlSite));

            // Chamado 10302. Caso o tamanho da constante ApplicationPath seja igual à 1 quer dizer que o sistema não
            // está rodando através de uma aplicação mas sim de um site, sendo assim o application path não deve
            // ser adicionado à url retornada.
            url += (context.Request.ApplicationPath.Length > 1 && url.IndexOf(context.Request.ApplicationPath) == -1 ? context.Request.ApplicationPath : "");

            return url;
        }

        /// <summary>
        /// Retorna a url completa da url virtual passada
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetFullUrl(HttpContext context, string url)
        {
            url = url.Replace("~/", "").Replace("../", "");
            url = GetUrlSite(context) + "/" + url;

            return url;
        }

        /// <summary>
        /// Verifica se uma requisição foi local.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsLocalUrl(HttpContext context)
        {
            return /*context.Request.IsLocal && context.Server.MachineName.ToLower().Contains("sync") &&*/
                Utils.GetFullUrl(context, context.Request.Url.ToString()).ToLower().Contains("localhost");
        }

        /// <summary>
        /// Retorna conversão de imagem de requisição http em byte[]
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] GetImageFromRequest(string url)
        {
            return GetImageFromRequest(HttpContext.Current, url);
        }

        /// <summary>
        /// Retorna conversão de imagem de requisição http em byte[]
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static byte[] GetImageFromRequest(HttpContext context, string url)
        {
            try
            {
                url = GetFullUrl(context, url);
                if (url.Contains(GetUrlSite(context)) && !url.ToLower().Contains("/handlers/") && !url.ToLower().Contains("/relatorios/"))
                {
                    url = "~/" + url.Substring(GetUrlSite(context).Length + 1);
                    return GetImageFromFile(context.Server.MapPath(url));
                }
                
                var uri = new Uri(url);

                if ((!url.ToLower().Contains("/relatorios/") &&
                    System.Configuration.ConfigurationManager.AppSettings["TrocarPorta"] == "true") ||
                    System.Configuration.ConfigurationManager.AppSettings["TrocarPortaRelatorio"] == "true")
                    uri = new Uri(string.Format("{0}://localhost:{1}{2}", uri.Scheme, uri.Port, uri.PathAndQuery));

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.CookieContainer = new CookieContainer();
                for (int i = 0; i < context.Request.Cookies.Count; i++)
                {
                    HttpCookie c = context.Request.Cookies[i];
                    request.CookieContainer.Add(uri, new Cookie(c.Name, c.Value, c.Path));
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (BinaryReader reader = new BinaryReader(response.GetResponseStream()))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] buffer;
                            byte[] file;

                            buffer = reader.ReadBytes(1024);
                            while (buffer.Length > 0)
                            {
                                ms.Write(buffer, 0, buffer.Length);
                                buffer = reader.ReadBytes(1024);
                            }

                            file = new byte[(int)ms.Length];
                            ms.Position = 0;
                            ms.Read(file, 0, file.Length);

                            return file;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("Falha ao gerar anexo de email. URL: " + url, ex);

                return new byte[0];
            }
        }

        /// <summary>
        /// Retorna uma imagem de um arquivo.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] GetImageFromFile(string path)
        {
            List<byte> retorno = new List<byte>();
            
            if(File.Exists(path))
            {
                using (FileStream f = File.OpenRead(path))
                {
                    byte[] temp = new byte[1024];
                    int tamanho = 0;
                    while ((tamanho = f.Read(temp, 0, temp.Length)) > 0)
                    {
                        if (tamanho < temp.Length)
                        {
                            byte[] copiar = new byte[tamanho];
                            for (int i = 0; i < tamanho; i++)
                                copiar[i] = temp[i];

                            retorno.AddRange(copiar);
                        }
                        else
                            retorno.AddRange(temp);
                    }
                }
            }

            return retorno.ToArray();
        }

        #endregion

        #region Salva um byte[] em um arquivo

        /// <summary>
        /// Recupera os bytes de um arquivo
        /// </summary>
        /// <param name="caminhoFisicoArq">URL do arquivo</param>
        /// <returns>Retorna os bytes do arquivo</returns>
        public static byte[] GetBytesArquivo(string caminhoFisicoArq)
        {
            // Verifica se o arquivo existe
            if (File.Exists(caminhoFisicoArq))
                // Instancia a classe FileStream para abrir o arquivo
                using (FileStream fs = File.Open(caminhoFisicoArq, FileMode.Open))
                    // Instancia a classe StreamReader para ler os dados do arquivo
                    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                        // Retorna os bytes do arquivo
                        return Encoding.Default.GetBytes(sr.ReadToEnd());
            else
                // Caso o arquivo não exista é lançada uma nova exceção
                throw new Exception("Arquivo não exite.");
        }

        #endregion

        #region Caminhos de pastas/arquivos

        /// <summary>
        /// Representa o diretório de upload.
        /// </summary>
        private static string DiretorioUpload
        {
            get
            {
                var diretorio = System.Configuration.ConfigurationManager.AppSettings["diretorioUpload"];

                if (string.IsNullOrEmpty(diretorio))
                    return "~/Upload";

                return diretorio;
            }
        }

        /// <summary>
        /// Monta o caminha a partir do diretório de upload.
        /// </summary>
        /// <param name="complemento"></param>
        /// <returns></returns>
        private static string MontarDiretorioUpload(params string[] complemento)
        {
            var diretorio = DiretorioUpload;

            if (complemento != null)
                foreach(var i in complemento)
                    diretorio = System.IO.Path.Combine(diretorio, i);

            if (diretorio.StartsWith("~"))
                return HttpContext.Current.Server.MapPath(diretorio.Replace('\\', '/'));

            return diretorio;
        }

        /// <summary>
        /// Retorna o endereço físico da pasta onde deverão ser salvas as imagens das peças anexadas/editas dos produtos do pedido espelho
        /// </summary>
        public static string GetPecaProducaoPath
        {
            get
            {
                var caminho = HttpContext.Current.Server.MapPath(GetPecaProducaoVirtualPath);

                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço virtual da pasta onde deverão ser salvas as imagens das peças anexadas/editas dos produtos do pedido espelho
        /// </summary>
        public static string GetPecaProducaoVirtualPath
        {
            get { return "~/Upload/PecaProducao/"; }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverão ser salvos os arquivos de CNI
        /// </summary>
        public static string GetArquivosCNIPath
        {
            get
            {
                var caminhoFisico = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload/ArquivosCNI");

                if (!Directory.Exists(caminhoFisico))
                    Directory.CreateDirectory(caminhoFisico);

                return caminhoFisico;
            }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverão ser salvos os arquivos de EDI quitar parcela de cartão automatico
        /// </summary>
        public static string GetArquivoQuitacaoParcelaCartaoPath
        {
            get
            {
                var caminhoFisico = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload/ArquivoQuitacaoParcelaCartao");

                if (!Directory.Exists(caminhoFisico))
                    Directory.CreateDirectory(caminhoFisico);

                return caminhoFisico;
            }
        }

        /// <summary>
        /// Retorna o endereço físico da pasta onde deverão ser salvas as imagens das peças anexadas/editas dos produtos do pedido 
        /// </summary>
        public static string GetPecaComercialPath
        {
            get 
            { 
                string caminho = HttpContext.Current.Server.MapPath("~/Upload/PecaComercial/");
                
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço físico da pasta onde deverão ser salvas as imagens das peças anexadas/editas dos produtos do projeto 
        /// </summary>
        public static string GetPecaProjetoPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath("~/Upload/PecaProjeto/");

                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }


        /// <summary>
        /// Retorna o endereço virtual da pasta onde deverão ser salvas as imagens das peças anexadas/editas dos produtos do pedido 
        /// </summary>
        public static string GetPecaComercialVirtualPath
        {
            get 
            {
                string caminho = "~/Upload/PecaComercial/";

                if (!Directory.Exists(HttpContext.Current.Server.MapPath(caminho)))
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(caminho));

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salvo os XMLs das cartas de correção
        /// </summary>
        public static string GetCartaCorrecaoXmlPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetCartaCorrecaoPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        public static string GetCartaCorrecaoPath
        {
            get { return "~/Upload/CartaCorrecao/"; }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salvo os XMLs da NF-e
        /// </summary>
        public static string GetNfeXmlPath
        {
            get { return GetNfeXmlPathInternal(HttpContext.Current); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salvo os XMLs da NF-e
        /// </summary>
        internal static string GetNfeXmlPathInternal(HttpContext context)
        {
            var caminho = context.Server.MapPath("~/Upload/NFe/");

            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            return caminho;
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salvo os XMLs do CT-e
        /// </summary>
        public static string GetCteXmlPath
        {
            get { return GetCteXmlPathInternal(HttpContext.Current); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salvo os XMLs do CT-e
        /// </summary>
        internal static string GetCteXmlPathInternal(HttpContext context)
        {
            var caminho = context.Server.MapPath("~/Upload/CTe/");

            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            return caminho;
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salvo os XMLs do MDF-e
        /// </summary>
        public static string GetMDFeXmlPath
        {
            get { return GetMDFeXmlPathInternal(HttpContext.Current); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salvo os XMLs do MDF-e
        /// </summary>
        internal static string GetMDFeXmlPathInternal(HttpContext context)
        {
            var caminho = context.Server.MapPath("~/Upload/MDFe/");

            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            return caminho;
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salvo os Certificados Digitais
        /// </summary>
        public static string GetCertPath
        {
            get
            {
                var caminho = HttpContext.Current.Server.MapPath("~/Upload/Cert/");

                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde está salvo os schemas de validação da NFe
        /// </summary>
        public static string GetSchemasPath
        {
            get
            {
                var caminho = HttpContext.Current.Server.MapPath("~/Schemas/");

                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde está salvo os schemas de validação do MDFe
        /// </summary>
        public static string GetMDFeSchemasPath
        {
            get
            {
                var caminho = HttpContext.Current.Server.MapPath("~/Schemas/MDFe/");

                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosMedicaoPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/Medicoes"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosCompraPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/Compras"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosPedidoPath
        {
            get { return MontarDiretorioUpload("Pedidos"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosLiberacaoPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/Liberacoes"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosOrcamentoPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/Orcamentos"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosClientePath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/Clientes"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosFornecedorPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/Fornecedores"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosDevolucaoPagtoPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/DevolucaoPagto"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosImpostoServPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/ImpostoServ"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosTrocaDevolucaoPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/TrocaDevolucao"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosConciliacaoBancariaPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/ConciliacaoBancaria"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto
        /// </summary>
        public static string GetFotosPagtoPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/Pagto"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto.
        /// </summary>
        public static string GetFotosChequesPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/Cheque"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto.
        /// </summary>
        public static string GetFotosAcertoPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/Acerto"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto.
        /// </summary>
        public static string GetFotosPagtoAntecipadoPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/PagtoAntecipado"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto.
        /// </summary>
        public static string GetFotosObraPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/Obra"); }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto.
        /// </summary>
        public static string GetFotosSugestaoPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/Sugestao"); }
        }
      
        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salva a foto.
        /// </summary>
        public static string GetFotosPedidoInternoPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Upload/PedidoInterno"); }
        }

        /// <summary>
        /// Retorna o endereço físico onde são salvos os pedidos a serem importados.
        /// </summary>
        public static string GetImportacaoPedidoPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetImportacaoPedidoVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço físico onde é salvo o XML de cada pedido junto com a imagem a ser anexada.
        /// </summary>
        public static string GetImportacaoPedidoVirtualPath
        {
            get { return "~/Upload/ImportacaoPedido/"; }
        }

        /// <summary>
        /// Retorna o endereço físico onde deverão ser salvos os pedidos importados via leitura de XML.
        /// </summary>
        public static string GetPedidoImportadoPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetPedidoImportadoVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço físico onde é salvo o XML de cada pedido junto com a imagem a ser anexada.
        /// </summary>
        public static string GetPedidoImportadoVirtualPath
        {
            get { return "~/Upload/ImportacaoPedido/Importado/"; }
        }

        /// <summary>
        /// Retorna o endereço físico onde é salvo o XML de cada pedido que não foi importado junto com a imagem a ser anexada.
        /// </summary>
        public static string GetPedidoNaoImportadoVirtualPath
        {
            get { return "~/Upload/ImportacaoPedido/NaoImportado/"; }
        }

        /// <summary>
        /// Retorna o endereço físico onde deverão ser salvos os pedidos não importados via leitura de XML.
        /// </summary>
        public static string GetPedidoNaoImportadoPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetPedidoNaoImportadoVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço virtual dos arquivos CalcEngine.
        /// </summary>
        public static string GetArquivoCalcEngineVirtualPath
        {
            get { return "~/Upload/ArquivoCalcEngine/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos CalcEngine.
        /// </summary>
        public static string GetArquivoCalcEnginePath
        {
            get
            {
                var caminho = HttpContext.Current.Server.MapPath(GetArquivoCalcEngineVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço virtual dos arquivos DXF.
        /// </summary>
        public static string GetArquivoDxfVirtualPath
        {
            get { return "~/Upload/ArquivoDXF/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos DXF.
        /// </summary>
        public static string GetArquivoDxfPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetArquivoDxfVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }
 
         /// <summary>
        /// Retorna o endereço virtual dos arquivos DXF gerados.
        /// </summary>
        public static string GetArquivoDxfGeradoVirtualPath
        {
            get { return "~/Upload/ArquivoDXFGerado/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos DXF gerados.
        /// </summary>
        public static string GetArquivoDxfGeradoPath
        {
            get
            {
                var caminho = HttpContext.Current.Server.MapPath(GetArquivoDxfGeradoVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        #region SGlass

        /// <summary>
        /// Retorna o endereço virtual dos arquivos SGlass Hardware gerados.
        /// </summary>
        public static string GetArquivoSGlassHardwareGeradoVirtualPath
        {
            get { return "~/Upload/ArquivoSGLASSGerado/Hardware/"; }
        }

        /// <summary>
        /// Retorna o endereço virtual dos arquivos SGlass Program gerados.
        /// </summary>
        public static string GetArquivoSGlassProgramGeradoVirtualPath
        {
            get { return "~/Upload/ArquivoSGLASSGerado/Program/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos SGlass Hardware gerados.
        /// </summary>
        public static string GetArquivoSGlassHardwareGeradoPath
        {
            get
            {
                var caminho = HttpContext.Current.Server.MapPath(GetArquivoSGlassHardwareGeradoVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos SGlass Program gerados.
        /// </summary>
        public static string GetArquivoSGlassProgramGeradoPath
        {
            get
            {
                var caminho = HttpContext.Current.Server.MapPath(GetArquivoSGlassProgramGeradoVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        #endregion

        #region Intermac

        /// <summary>
        /// Retorna o endereço virtual dos arquivos Intermac gerados.
        /// </summary>
        public static string GetArquivoIntermacGeradoVirtualPath
        {
            get { return "~/Upload/ArquivoIntermacGerado/"; }
        }


        /// <summary>
        /// Retorna o endereço físico dos arquivos Intermac gerados.
        /// </summary>
        public static string GetArquivoIntermacGeradoPath
        {
            get
            {
                var caminho = HttpContext.Current.Server.MapPath(GetArquivoIntermacGeradoVirtualPath);

                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço virtual dos arquivos de configuração da Intermac.
        /// </summary>
        public static string GetArqConfigIntermacVirtualPath
        {
            get { return "~/Upload/Intermac/bsolid/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos de configuração da Intermac.
        /// </summary>
        public static string ArqConfigIntermacPath(HttpContext context)
        {
            var caminho = context.Server.MapPath(GetArqConfigIntermacVirtualPath);

            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            return caminho;
        }

        #endregion

        /// <summary>
        /// Retorna o endereço virtual dos arquivos FML.
        /// </summary>
        public static string GetArquivoFmlVirtualPath
        {
            get { return "~/Upload/ArquivoFML/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos FML.
        /// </summary>
        public static string GetArquivoFmlPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetArquivoFmlVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço virtual dos arquivos FML gerados.
        /// </summary>
        public static string GetArquivoFmlGeradoVirtualPath
        {
            get { return "~/Upload/ArquivoFMLGerado/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos FML gerados.
        /// </summary>
        public static string GetArquivoFmlGeradoPath
        {
            get
            {
                var caminho = HttpContext.Current.Server.MapPath(GetArquivoFmlGeradoVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço virtual dos arquivos de configuração da Forvet.
        /// </summary>
        public static string GetArqConfigForvetVirtualPath
        {
            get { return "~/Upload/Forvet/" + System.Configuration.ConfigurationManager.AppSettings["sistema"] + "/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos de configuração da Forvet.
        /// </summary>
        public static string GetArqConfigForvetPath
        {
            get { return ArqConfigForvetPath(HttpContext.Current); }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos de configuração da Forvet.
        /// </summary>
        public static string ArqConfigForvetPath(HttpContext context)
        {
            string caminho = context.Server.MapPath(GetArqConfigForvetVirtualPath);
            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            return caminho;
        }

        /// <summary>
        /// Retorna o endereço físico dos modelos de projetos
        /// </summary>
        public static string GetModelosProjetoPath
        {
            get { return ModelosProjetoPath(HttpContext.Current); }
        }

        /// <summary>
        /// Retorna o endereço físico dos modelos de projetos
        /// </summary>
        public static string ModelosProjetoPath(HttpContext context)
        {
            return context.Server.MapPath("~/Upload/ModelosProjeto/");
        }

        /// <summary>
        /// Retorna o endereço físico dos modelos de projetos
        /// </summary>
        public static string GetFigurasProjetoPath
        {
            get 
            {
                string caminho = HttpContext.Current.Server.MapPath(GetFigurasProjetoVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço físico dos modelos de projetos
        /// </summary>
        public static string GetArquivoOtimizacaoVirtualPath
        {
            get { return "~/Upload/ArqOtimiz/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos de otimização de chapa
        /// </summary>
        public static string GetArquivoOtimizacaoPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetArquivoOtimizacaoVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        public static string GetArquivoFCIVirtualPath
        {
            get { return "~/Upload/ArqFCI/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos de otimização de chapa
        /// </summary>
        public static string GetArquivoFCIPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetArquivoFCIVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço físico dos funcionários
        /// </summary>
        public static string GetFuncionariosPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetFuncionariosVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço físico dos funcionários
        /// </summary>
        public static string GetFuncionariosVirtualPath
        {
            get { return "~/Upload/Funcionarios/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos produtos
        /// </summary>
        public static string GetProdutosPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetProdutosVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }
        
        /// <summary>
        /// Retorna o endereço físico dos produtos
        /// </summary>
        public static string GetProdutosVirtualPath
        {
            get { return "~/Upload/Produtos/"; }
        }

        public static string GetProdutosOrcamentoVirtualPath
        {
            get { return "~/Upload/ProdutosOrca/"; }
        }

        /// <summary>
        /// Retorna o endereço físico das imagens dos produtos do orçamento.
        /// </summary>
        public static string GetProdutosOrcamentoPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetProdutosOrcamentoVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        public static string GetArquivoRemessaVirtualPath
        {
            get { return "~/Upload/ArquivoRemessa"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos de remessa.
        /// </summary>
        public static string GetArquivoRemessaPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetArquivoRemessaVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço virtual dos arquivos de mesa de corte dos pedidos importados.
        /// </summary>
        public static string GetArquivoMesaCorteImpVirtualPath
        {
            get { return "~/Upload/ArquivoMesaCorteImp/"; }
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos de mesa de corte dos pedidos importados.
        /// </summary>
        public static string GetArquivoMesaCorteImpPath
        {
            get
            {
                string caminho = HttpContext.Current.Server.MapPath(GetArquivoMesaCorteImpVirtualPath);
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                return caminho;
            }
        }

        /// <summary>
        /// Retorna o endereço virtual dos modelos de projetos
        /// </summary>
        public static string GetModelosProjetoVirtualPath
        {
            get { return "~/Upload/ModelosProjeto/"; }
        }

        /// <summary>
        /// Retorna o endereço físico das figuras de projeto
        /// </summary>
        public static string GetFigurasProjetoVirtualPath
        {
            get { return "~/Upload/FigurasProjeto/"; }
        }

        /// <summary>
        /// Retorna o endereço físico do servidor onde deverá ser salvo o bkp do bd temporariamente
        /// </summary>
        public static string GetBackupPath
        {
            get
            {
                var caminhoFisico = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload/BkpBD");

                if (!Directory.Exists(caminhoFisico))
                    Directory.CreateDirectory(caminhoFisico);

                return caminhoFisico;
            }
        }

        /// <summary>
        /// Retorna o endereço virtual dos arquivos do CadProject.
        /// </summary>
        public static string GetArquivoCadProjectVirtualPath(bool pcp)
        {
            return "~/Upload/CadProject/" + (pcp ? "PCP" : "Comercial") + "/";
        }

        /// <summary>
        /// Retorna o endereço físico dos arquivos CadProject.
        /// </summary>
        public static string GetArquivoCadProjectPath(bool pcp)
        {
            var caminho = HttpContext.Current.Server.MapPath(GetArquivoCadProjectVirtualPath(pcp));
            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            return caminho;
        }

        #endregion

        #region Tipo Entrega

        /// <summary>
        /// Retorna a descrição do tipo de entrega
        /// </summary>
        /// <param name="tipoEntrega"></param>
        /// <returns></returns>
        public static string GetDescrTipoEntrega(int? tipoEntrega)
        {
            if (tipoEntrega == null)
                return String.Empty;

            foreach (GenericModel m in DataSources.Instance.GetTipoEntrega())
                if ((int)m.Id == (int)tipoEntrega)
                    return m.Descr;

            return String.Empty;
        }

        #endregion        

        #region Código de barras

        public enum GirarImagem
        {
            Normal,
            Girar90Graus,
            Girar180Graus,
            Girar270Graus
        }

        /// <summary>
        /// Retorna um vetor de bytes com a imagem do código de barras.
        /// </summary>
        /// <param name="code">O texto que será usado para gerar o código de barras.</param>
        /// <returns></returns>
        public static byte[] GetBarCode(string code)
        {
            return GetBarCode(code, GirarImagem.Normal);
        }

        /// <summary>
        /// Retorna um vetor de bytes com a imagem do código de barras.
        /// </summary>
        /// <param name="code">O texto que será usado para gerar o código de barras.</param>
        /// <returns></returns>
        public static byte[] GetBarCode(string code, GirarImagem girar)
        {
            iTextSharp.text.pdf.Barcode barCode = null;
            
            barCode = new iTextSharp.text.pdf.Barcode128();
            barCode.CodeType = iTextSharp.text.pdf.Barcode.CODE128;
            barCode.ChecksumText = true;
            barCode.GenerateChecksum = true;
            barCode.StartStopText = true;
            barCode.Code = code;

            System.Drawing.Image image = barCode.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White);

            switch (girar)
            {
                case GirarImagem.Girar90Graus:
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;

                case GirarImagem.Girar180Graus:
                    image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;

                case GirarImagem.Girar270Graus:
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        #endregion

        #region QR Code

        /// <summary>
        /// Retorna um vetor de bytes com a imagem do QR Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static byte[] ObterQrCode(string code)
        {
            var encoder = new Gma.QrCodeNet.Encoding.QrEncoder(Gma.QrCodeNet.Encoding.ErrorCorrectionLevel.M);

            var qrCode = encoder.Encode(code);

            var escala = 5;

            var matrix = qrCode.Matrix;
            var brush = new SolidBrush(Color.White);
            var image = new Bitmap((matrix.Width * escala) + 1, (matrix.Height * escala) + 1);
            var g = Graphics.FromImage(image);

            g.FillRectangle(brush, new Rectangle(0, 0, image.Width, image.Height));
            brush.Color = Color.Black;

            for (int i = 0; i < matrix.Width; i++)
                for (int j = 0; j < matrix.Height; j++)
                    if (matrix[j, i])
                        g.FillRectangle(brush, j * escala, i * escala, escala, escala);

            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        #endregion

        #region Calcula o peso do produto

        internal enum TipoCalcPeso
        {
            ProdutoOrcamento,
            ProdutoPedido,
            ProdutoPedidoEspelho
        }

        /// <summary>
        /// Retorna o SQL para o cálculo do peso dos produtos de um pedido.
        /// </summary>
        /// <returns></returns>
        internal static string SqlCalcPeso(TipoCalcPeso tipoCalcPeso, uint? idParent, bool filtrarVisibilidade, bool pcp, bool nf)
        {
            string tipoCalc = "coalesce(s_calc.tipoCalculo, g_calc.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + ")";
            if (nf) tipoCalc = tipoCalc.Replace("coalesce(", "coalesce(s_calc.tipoCalculoNf, g_calc.tipoCalculoNf, ");

            string campoIdProd = tipoCalcPeso != TipoCalcPeso.ProdutoOrcamento ? "pp_calc.idProd" : "pp_calc.idProduto";

            string campo = tipoCalcPeso == TipoCalcPeso.ProdutoOrcamento ? "pp_calc.idProd" :
                tipoCalcPeso == TipoCalcPeso.ProdutoPedido ? "pp_calc.idProdPed" :
                tipoCalcPeso == TipoCalcPeso.ProdutoPedidoEspelho ? "pp_calc.idProdPed" : "null";

            string tabela = tipoCalcPeso == TipoCalcPeso.ProdutoOrcamento ? "produtos_orcamento" :
                tipoCalcPeso == TipoCalcPeso.ProdutoPedido ? "produtos_pedido" :
                tipoCalcPeso == TipoCalcPeso.ProdutoPedidoEspelho ? "produtos_pedido_espelho" : "null";

            string sql = @"
                select " + campo + " as id, coalesce(if(p_calc.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @", if(pp_calc.espessura>0, pp_calc.espessura, coalesce(p_calc.espessura, 0)) * pp_calc.totM * 2.5,
                    if(" + tipoCalc + " in (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.Perimetro + @"), p_calc.peso * pp_calc.totM,
                    if(" + tipoCalc + " in (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.ML + @"), p_calc.peso * pp_calc.qtde * pp_calc.altura,
                    if(" + tipoCalc + "=" + (int)Glass.Data.Model.TipoCalculoGrupoProd.QtdDecimal + @" and lower(um.codigo)='kg', pp_calc.qtde,
                    p_calc.peso * pp_calc.qtde)))),0) as peso
                from " + tabela + @" as pp_calc
                    inner join produto p_calc on (" + campoIdProd + @"=p_calc.idProd)
                    Left Join unidade_medida um On (p_calc.idUnidadeMedida=um.idUnidadeMedida) 
                    inner join grupo_prod g_calc on (p_calc.idGrupoProd=g_calc.idGrupoProd)
                    left join subgrupo_prod s_calc on (p_calc.idSubgrupoProd=s_calc.idSubgrupoProd)
                where 1";

            if (idParent > 0)
            {
                sql += String.Format(" and pp_calc.{1}={0}", idParent.Value,
                    tipoCalcPeso == TipoCalcPeso.ProdutoOrcamento ? "idOrcamento" :
                    tipoCalcPeso == TipoCalcPeso.ProdutoPedido ? "idPedido" :
                    tipoCalcPeso == TipoCalcPeso.ProdutoPedidoEspelho ? "idPedido" : "null");
            }

            if (filtrarVisibilidade)
                sql += String.Format(" and !coalesce(pp_calc.invisivel{0}, false)", pcp ? "Fluxo" : "Pedido");

            return sql;
        }

        /// <summary>
        /// Calcula o peso do produto
        /// </summary>
        public static float CalcPeso(int idProd, float espessura, float totM, float qtde, float altura, bool nf)
        {
            return CalcPeso(null, idProd, espessura, totM, qtde, altura, nf);
        }

        /// <summary>
        /// Calcula o peso do produto
        /// </summary>
        public static float CalcPeso(GDASession session, int idProd, float espessura, float totM, float qtde, float altura, bool nf)
        {
            float peso = ProdutoDAO.Instance.ObtemValorCampo<float>(session, "peso", "idProd=" + idProd);
            int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, idProd, nf);

            if (espessura == 0)
                espessura = ProdutoDAO.Instance.ObtemEspessura(session, idProd);

            if (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(ProdutoDAO.Instance.ObtemIdGrupoProd(session, idProd)))
                return (float)(2.5 * espessura * totM);
            else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.Perimetro)
                return (float)(peso * totM);
            else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalc ==
                (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                return (float)(peso * qtde * altura);
            else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.QtdDecimal && ProdutoDAO.Instance.ObtemUnidadeMedida(session, idProd).ToLower() == "kg")
                return qtde;
            else
                return (float)(peso * qtde);
        }

        #endregion

        #region Produção

        private static Setor[] _getSetores;

        public static Setor[] GetSetores
        {
            get 
            {
                if (_getSetores == null || _getSetores.Length <= 1)
                    _getSetores = SetorDAO.Instance.GetOrdered();

                return Utils._getSetores;
            }
            set { Utils._getSetores = value; }
        }

        /// <summary>
        /// Obtém setor pelo seu id
        /// </summary>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public static Setor ObtemSetor(uint idSetor)
        {
            foreach (Setor s in GetSetores)
                if (s.IdSetor == idSetor)
                    return s;

            return null;
        }

        /// <summary>
        /// Obtém o primeiro setor de entrega da lista
        /// </summary>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public static uint? ObtemIdSetorEntregue()
        {
            foreach (Setor s in GetSetores)
                if (s.Tipo == TipoSetor.Entregue)
                    return (uint)s.IdSetor;

            return null;
        }

        /// <summary>
        /// Retorna a cor do setor passado
        /// </summary>
        /// <param name="idSetor"></param>
        public static string ObtemCorSetor(uint idSetor)
        {
            foreach (Setor s in GetSetores)
                if (s.IdSetor == idSetor)
                    return s.DescrCorSystem;

            return "Black";
        }

        #endregion

        /// <summary>
        /// Retorna o caminho do relatório padrão com máscara para usar no format (Ex.: "Relatorios/rptPedidoInstalacao{0}.rdlc")
        /// </summary>
        /// <param name="caminhoRelatorioPadraoComMascara"></param>
        /// <returns></returns>
        public static string CaminhoRelatorio(string caminhoRelatorioPadraoComMascara)
        {
            var caminhoRelatorio = string.Format(caminhoRelatorioPadraoComMascara, ControleSistema.GetSite().ToString());

            if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                return caminhoRelatorio;

            return string.Format(caminhoRelatorioPadraoComMascara, "");
        }

        public static string MontaDescrLapBis(int bisAlt, int bisLarg, int lapAlt, int lapLarg, float espBisote, string complAltura,
            string complLargura, bool compra)
        {
            if (complAltura == null)
                complAltura = String.Empty;

            if (complLargura == null)
                complLargura = String.Empty;
            
            return
                (bisAlt > 0 || bisLarg > 0 ? " " + (PedidoConfig.EmpresaTrabalhaAlturaLargura ? bisAlt + complAltura + "x" + bisLarg + complLargura : bisLarg + complLargura + "x" + bisAlt + complAltura) : "") +
                (espBisote > 0 ? " " + espBisote + "mm" : String.Empty) +
                (lapAlt > 0 || lapLarg > 0 ? " " + (PedidoConfig.EmpresaTrabalhaAlturaLargura ? lapAlt + complAltura + "x" + lapLarg + complLargura : lapLarg + complLargura + "x" + lapAlt + complAltura) : "");
        }

        public static bool ArquivoExiste(string arquivo)
        {
            if (arquivo.Contains("/"))
            {
                byte[] bytes = GetImageFromRequest(arquivo);
                return bytes.Length > 0;
            }
            else
            {
                arquivo = HttpContext.Current.Server.MapPath(arquivo);
                return File.Exists(arquivo);
            }
        }                               
        
        /// <summary>
        /// Retorna uma coluna da grid com base no seu HeaderText
        /// </summary>
        /// <param name="headerText"></param>
        /// <param name="gridView"></param>
        /// <returns></returns>
        public static System.Web.UI.WebControls.DataControlField BuscarColunaGrid(string headerText, System.Web.UI.WebControls.GridView gridView)
        {
            foreach (System.Web.UI.WebControls.DataControlField coluna in gridView.Columns)
                if (coluna.HeaderText == headerText)
                    return gridView.Columns[gridView.Columns.IndexOf(coluna)];

            return null;
        }

        public static System.Web.UI.WebControls.DetailsViewRow BuscarColunaDetails(string headerText, System.Web.UI.WebControls.DetailsView detailsView)
        {
            foreach (System.Web.UI.WebControls.DataControlField coluna in detailsView.Fields)
                if (coluna.HeaderText == headerText)
                    return detailsView.Rows[detailsView.Fields.IndexOf(coluna)];

            return null;
        }

        //public static Control FindControlRecursivo(string controlName, Control container)
        //{
        //    object ctrl = container.FindControl(controlName);

        //    if (ctrl == null)
        //    {
        //        foreach (Control item in container.Controls)
        //        {
        //            ctrl = FindControlRecursivo(controlName, item);
        //            if (ctrl != null)
        //                break;
        //        }
        //    }

        //    if (ctrl == null)
        //        return null;

        //    return (Control)ctrl;
        //}        

    }
}