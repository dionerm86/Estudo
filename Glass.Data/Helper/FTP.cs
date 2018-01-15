using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.Helper
{
    public static class FTP
    {
        /// <summary>
        /// Método utilizado para salvar anexo utilizando configurações do servidor,
        /// salvando o anexo imediatamente.
        /// </summary>
        /// <param name="dominioFtp">Domínio FTP</param>
        /// <param name="usuario">Usuário do domínio FTP</param>
        /// <param name="senha">Senha do domínio FTP</param>
        /// <param name="nomeArquivo">Nome completo do arquivo com a extensão</param>
        /// <param name="arquivo">Bytes do arquivo</param>
        /// <param name="caminhoFisicoArq">Caminho físico do arquivo</param>
        public static void SalvarArquivo(string dominioFtp, string usuario, string senha, string nomeArquivo, byte[] arquivo, string caminhoFisicoArq)
        {
            // Recupera os bytes do arquivo
            byte[] conteudoArquivo = !String.IsNullOrEmpty(caminhoFisicoArq) ? Utils.GetBytesArquivo(caminhoFisicoArq) : arquivo;

            if (conteudoArquivo != null)
                // Instancia a classe WebClient para que seja possível gravar dados no FTP
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    // Seta as credenciais do FTP onde o arquivo será salvo
                    client.Credentials = new System.Net.NetworkCredential(usuario, senha);
                    // Define em qual pasta será criado o arquivo e define o nome do mesmo
                    client.UploadData(dominioFtp + nomeArquivo, "STOR", conteudoArquivo);
                }
            else
                throw new Exception("Arquivo inválido.");
        }

        /// <summary>
        /// Método utilizado para salvar anexo utilizando configurações do servidor, salvando o anexo imediatamente,
        /// o nome do arquivo deve ser nulo somente se o método for utilizado para salvar as imagens de projeto da Temper Mat
        /// </summary>
        /// <param name="dominioFtp">Domínio FTP</param>
        /// <param name="usuario">Usuário do domínio FTP</param>
        /// <param name="senha">Senha do domínio FTP</param>
        /// <param name="nomeArquivo">Nome completo do arquivo com a extensão</param>
        /// <param name="idPedidoEspelho">Id do pedido espelho</param>
        public static void SalvarImagensProjeto(string dominioFtp, string usuario, string senha, string nomeArquivo, uint idPedidoEspelho)
        {
            SalvarImagensProjeto(null, dominioFtp, usuario, senha, nomeArquivo, idPedidoEspelho);
        }

        /// <summary>
        /// Método utilizado para salvar anexo utilizando configurações do servidor, salvando o anexo imediatamente,
        /// o nome do arquivo deve ser nulo somente se o método for utilizado para salvar as imagens de projeto da Temper Mat
        /// </summary>
        /// <param name="dominioFtp">Domínio FTP</param>
        /// <param name="usuario">Usuário do domínio FTP</param>
        /// <param name="senha">Senha do domínio FTP</param>
        /// <param name="nomeArquivo">Nome completo do arquivo com a extensão</param>
        /// <param name="idPedidoEspelho">Id do pedido espelho</param>
        public static void SalvarImagensProjeto(GDASession sessao, string dominioFtp, string usuario, string senha, string nomeArquivo, uint idPedidoEspelho)
        {
            // Recupera todos os idsItemProjeto que possuem referência do idPedidoEspelho passado como parametro
            string[] ids = ItemProjetoDAO.Instance.GetValoresCampo(sessao, "Select idItemProjeto From item_projeto Where idPedidoEspelho=" + idPedidoEspelho, "idItemProjeto").Split(',');
            
            // Salva todas as imagens de cada projeto, cada um com seu índice
            for (int i = 0; i < ids.Length; i++)
            {
                // Recupera os bytes do relatório
                byte[] contArquivo = GetBytesImagensProjeto(sessao, Glass.Conversoes.StrParaUint(ids[i]));
                
                // Define o nome do arquivo
                nomeArquivo = nomeArquivo == null ? DateTime.Now.ToString("yyyyMMddHHmmss") + "-P" + (i + 1).ToString("0##") + "-WEB.PDF" : nomeArquivo;
                
                // Salva o arquivo
                SalvarArquivo(dominioFtp, usuario, senha, nomeArquivo, contArquivo, null);
            }
        }

        /// <summary>
        /// Método utilizado para recuperar os bytes das imagens de um determinado projeto calculado.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns>Retorna os bytes das imagens de um determinado projeto calculado.</returns>
        private static byte[] GetBytesImagensProjeto(uint idItemProjeto)
        {
            return GetBytesImagensProjeto(null, idItemProjeto);
        }

        /// <summary>
        /// Método utilizado para recuperar os bytes das imagens de um determinado projeto calculado.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns>Retorna os bytes das imagens de um determinado projeto calculado.</returns>
        private static byte[] GetBytesImagensProjeto(GDASession sessao, uint idItemProjeto)
        {
            // Variáveis necessárias para renderizar o relatório
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            // Instancia a classe LocalReport para que sejam setados os dados do Rpt
            var report = new LocalReport();
            report.ReportPath = "Relatorios/Projeto/rptImagemProjetoFTP.rdlc";
            var lstParam = new List<ReportParameter>();

            // Instancia a classe ItemProjeto para que as imagens de projeto e suas informações sejam recuperadas
            var itemProjeto = ItemProjetoDAO.Instance.GetByString(sessao, idItemProjeto.ToString());
            var lstImagens = new List<Imagem>();
            var modelo = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(itemProjeto[0].IdProjetoModelo);       
            // Pega a imagem do projeto com as medidas já desenhadas e o modelo da imagem
            if (modelo.IdGrupoModelo != (uint)UtilsProjeto.GrupoModelo.Outros)
            {
                itemProjeto[0].ImagemProjeto = Utils.GetImageFromRequest(UtilsProjeto.GetFiguraAssociadaUrl(sessao, itemProjeto[0].IdItemProjeto, modelo));
                itemProjeto[0].ImagemProjetoModelo = Utils.GetImageFromRequest("../../Handlers/LoadImage.ashx?path=" + Utils.ModelosProjetoPath(HttpContext.Current) +
                    modelo.NomeFigura + "&altura=" + modelo.AlturaFigura + "&largura=" + modelo.LarguraFigura);
            }
            // Recupera as imagens individuais do projeto com as alterações efetuadas no PCP
            lstImagens.AddRange(ImagemDAO.Instance.GetPecasAlteradas(sessao, itemProjeto[0].IdItemProjeto, 0.75f));
            // Adiciona o id do item projeto à lista de parametros
            lstParam.Add(new ReportParameter("IdItemProjeto", itemProjeto[0].IdItemProjeto.ToString()));
            // Popula os modelos que foram setados no Rpt
            report.DataSources.Add(new ReportDataSource("ItemProjeto", itemProjeto));
            report.DataSources.Add(new ReportDataSource("Imagem", lstImagens.ToArray()));          
            // Seta os parametros definidos no Rpt
            report.SetParameters(lstParam);
            // Renderiza o relatório em PDF e retorna os bytes
            return report.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
        }   
    }
}