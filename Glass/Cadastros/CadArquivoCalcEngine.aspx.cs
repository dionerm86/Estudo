using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Collections.Generic;
using System.Linq;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadArquivoCalcEngine : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                if (Request["IdArquivoCalcEngine"] != null)
                    dtvArquivoCalcEngine.ChangeMode(DetailsViewMode.Edit);
                else
                {
                    dtvArquivoCalcEngine.ChangeMode(DetailsViewMode.Insert);
                    ((Button)dtvArquivoCalcEngine.FindControl("btnDownload")).Visible = false;
                }
                    
            }
        }

        protected void odsArquivoCalcEngine_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                // Salva o arquivo que será subido.
                var fluArquivoCalcEngine = ((FileUpload)dtvArquivoCalcEngine.FindControl("fluArquivoCalcEngine"));
                // Recupera o id do último arquivo CalcEngine inserido no sistema.
                var idArquivoCalcEngine = (uint)ArquivoCalcEngineDAO.Instance.ObtemUltimoIdArquivoCalcEngine();

                try
                {
                    // Variável criada para recuperar os arquivos do .calcpackage.
                    CalcEngine.ProjectFilesPackage pacote = null;
                    // Variável criada para ler a configuração do projeto.
                    CalcEngine.Dxf.DxfProject projeto = null;
                    // Lista criada para setar as variáveis do CalcEngine.
                    var lstVariaveisCalcEngine = new List<ArquivoCalcEngineVariavel>();

                    using (System.IO.Stream pacoteStream = fluArquivoCalcEngine.FileContent)
                    {
                        // Esse método deserializa os dados do pacote que estão contidos na Stream a recupera a instância do pacote de configuração.
                        pacote = CalcEngine.ProjectFilesPackage.LoadPackage(pacoteStream);
                    }

                    // Lê a configuração do projeto.
                    projeto = CalcEngine.Dxf.DxfProject.LoadFromPackage(pacote);

                    // Seta as variáveis do CalcEngine em uma lista.
                    foreach (var variavel in projeto.Variables.Where(f => f.GetType() == typeof(CalcEngine.Variable)))
                    {
                        // Cria uma nova variável CalcEngine.
                        var variavelCalcEngine = new ArquivoCalcEngineVariavel();
                        variavelCalcEngine.VariavelCalcEngine = variavel.Name;
                        // Salva o valor padrão da variável somente se não forem medidas de altura ou largura.
                        variavelCalcEngine.ValorPadrao = (decimal)(variavel.Name.ToLower() == "altura" || variavel.Name.ToLower() == "largura" ||
                            variavel.Name.ToLower() == "alturabase" || variavel.Name.ToLower() == "largurabase" ? 0 : variavel.Value);
                        if (variavel.Name.ToLower() == "altura" || variavel.Name.ToLower() == "largura" ||
                            variavel.Name.ToLower() == "alturabase" || variavel.Name.ToLower() == "largurabase")
                            variavelCalcEngine.VariavelSistema = variavelCalcEngine.VariavelCalcEngine;

                        // Seta a variável CalcEngine na lista.
                        lstVariaveisCalcEngine.Add(variavelCalcEngine);
                    }

                    // Salva as variáveis do CalcEngine.
                    foreach (var variavel in lstVariaveisCalcEngine)
                    {
                        variavel.IdArquivoCalcEngine = idArquivoCalcEngine;
                        // Insere a variável do arquivo CalcEngine e associa-a ao arquivo.
                        ArquivoCalcEngineVariavelDAO.Instance.Insert(variavel);
                    }
                }
                catch (Exception ex)
                {
                    // Caso o arquivo tenha sido salvo, então deve ser excluído.
                    if (System.IO.File.Exists(ProjetoConfig.CaminhoSalvarCalcEngine + fluArquivoCalcEngine.FileName))
                        System.IO.File.Delete(ProjetoConfig.CaminhoSalvarCalcEngine + fluArquivoCalcEngine.FileName);

                    // Deleta as variáveis associadas ao arquivo CalcEngine.
                    ArquivoCalcEngineVariavelDAO.Instance.DeletaPeloIdArquivoCalcEngine(idArquivoCalcEngine);
                    ArquivoCalcEngineDAO.Instance.DeleteByPrimaryKey(idArquivoCalcEngine);

                    // Retorna uma mensagem informando o erro ocorrido ao salvar o arquivo.
                    throw ex;
                }

                Response.Redirect("CadArquivoCalcEngine.aspx?IdArquivoCalcEngine=" + idArquivoCalcEngine);
            }
            else
            {
                // Deleta as variáveis associadas ao arquivo CalcEngine.
                ArquivoCalcEngineVariavelDAO.Instance.DeletaPeloIdArquivoCalcEngine((uint)(ArquivoCalcEngineDAO.Instance.ObtemUltimoIdArquivoCalcEngine() + 1));

                Glass.MensagemAlerta.ErrorMsg("Erro ao inserir o arquivo.", e.Exception.InnerException, this);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsArquivoCalcEngine_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                Response.Redirect("CadArquivoCalcEngine.aspx?IdArquivoCalcEngine=" + Request["IdArquivoCalcEngine"]);
            }
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Erro ao atualizar o arquivo.", e.Exception.InnerException, this);
                e.ExceptionHandled = true;
            }
        }

        /// <summary>
        /// Atualiza o arquivo selecionado na pasta dos arquivos CalcEngine.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void odsArquivoCalcEngine_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var arquivoCalcEngine = e.InputParameters[0] as Glass.Data.Model.ArquivoCalcEngine;

            // Salva o arquivo que será subido.
            var fluArquivoCalcEngine = ((FileUpload)dtvArquivoCalcEngine.FindControl("fluArquivoCalcEngine"));


            if (fluArquivoCalcEngine != null && !String.IsNullOrEmpty(fluArquivoCalcEngine.FileName))
            {
                // Salva o nome do arquivo CalcEngine.
                var nomeArquivo = fluArquivoCalcEngine.FileName;                

                /* Chamado 62033. */
                if (arquivoCalcEngine.Nome != nomeArquivo.ToUpper().Replace(".CALCPACKAGE", string.Empty))
                    throw new Exception("Não é possível atualizar inserir um arquivo com o nome diferente do arquivo atualizado.");

                // Variável criada para recuperar os arquivos do .calcpackage.
                CalcEngine.ProjectFilesPackage pacote = null;
                // Variável criada para ler a configuração do projeto.
                CalcEngine.Dxf.DxfProject projeto = null;
                // Lista criada para setar as variáveis do CalcEngine.
                var lstVariaveisCalcEngine = new List<ArquivoCalcEngineVariavel>();

                // Apaga o arquivo CalcEngine antigo.
                if (System.IO.File.Exists(ProjetoConfig.CaminhoSalvarCalcEngine + arquivoCalcEngine.Nome + ".calcpackage"))
                    System.IO.File.Delete(ProjetoConfig.CaminhoSalvarCalcEngine + arquivoCalcEngine.Nome + ".calcpackage");
                
                // Salva o arquivo CalcEngine.
                using (var m = new System.IO.MemoryStream(fluArquivoCalcEngine.FileBytes))
                {
                    //if (!ArquivoMesaCorteDAO.Instance.ValidarCadastroCalcEngine(m))
                    //    throw new Exception("O arquivo inserido está com falhas de validação");

                    //m.Position = 0;

                    var buffer = new byte[1024];
                    var read = 0;

                    using (var file = System.IO.File.Create(ProjetoConfig.CaminhoSalvarCalcEngine + fluArquivoCalcEngine.FileName))
                    {
                        while ((read = m.Read(buffer, 0, buffer.Length)) > 0)
                            file.Write(buffer, 0, read);

                        file.Flush();
                    }
                }

                using (System.IO.Stream pacoteStream = fluArquivoCalcEngine.FileContent)
                {
                    // Esse método deserializa os dados do pacote que estão contidos na Stream a recupera a instância do pacote de configuração.
                    pacote = CalcEngine.ProjectFilesPackage.LoadPackage(pacoteStream);
                }

                // Lê a configuração do projeto.
                projeto = CalcEngine.Dxf.DxfProject.LoadFromPackage(pacote);

                try
                {
                    // Seta as variáveis do CalcEngine em uma lista.
                    foreach (var variavel in projeto.Variables.Where(f => f.GetType() == typeof(CalcEngine.Variable)))
                    {
                        // Cria uma nova variável CalcEngine.
                        var variavelCalcEngine = new ArquivoCalcEngineVariavel();
                        variavelCalcEngine.VariavelCalcEngine = variavel.Name;
                        // Salva o valor padrão da variável somente se não forem medidas de altura ou largura.
                        variavelCalcEngine.ValorPadrao = (decimal)(variavel.Name.ToLower() == "altura" || variavel.Name.ToLower() == "largura" ||
                            variavel.Name.ToLower() == "alturabase" || variavel.Name.ToLower() == "largurabase" ? 0 : variavel.Value);
                        if (variavel.Name.ToLower() == "altura" || variavel.Name.ToLower() == "largura" ||
                            variavel.Name.ToLower() == "alturabase" || variavel.Name.ToLower() == "largurabase")
                            variavelCalcEngine.VariavelSistema = variavelCalcEngine.VariavelCalcEngine;

                        // Seta a variável CalcEngine na lista.
                        lstVariaveisCalcEngine.Add(variavelCalcEngine);
                    }

                    ArquivoCalcEngineVariavelDAO.Instance.DeletaPeloIdArquivoCalcEngine(arquivoCalcEngine.IdArquivoCalcEngine);

                    // Salva as variáveis do CalcEngine.
                    foreach (var variavel in lstVariaveisCalcEngine)
                    {
                        variavel.IdArquivoCalcEngine = arquivoCalcEngine.IdArquivoCalcEngine;
                        // Insere a variável do arquivo CalcEngine e associa-a ao arquivo.
                        ArquivoCalcEngineVariavelDAO.Instance.Insert(variavel);
                    }
                }
                catch (Exception ex)
                {
                    // Deleta as variáveis associadas ao arquivo CalcEngine.
                    ArquivoCalcEngineVariavelDAO.Instance.DeletaPeloIdArquivoCalcEngine(arquivoCalcEngine.IdArquivoCalcEngine);

                    // Retorna uma mensagem informando o erro ocorrido ao salvar o arquivo.
                    throw ex;
                }
            }
        }
        
        /// <summary>
        /// Insere o arquivo selecionado na pasta dos arquivos CalcEngine.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void odsArquivoCalcEngine_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var arquivoCalcEngine = e.InputParameters[0] as Glass.Data.Model.ArquivoCalcEngine;
            // Salva o arquivo que será subido.
            var fluArquivoCalcEngine = ((FileUpload)dtvArquivoCalcEngine.FindControl("fluArquivoCalcEngine"));
            // Salva o nome do arquivo CalcEngine.
            arquivoCalcEngine.Nome = fluArquivoCalcEngine.FileName.ToUpper().Replace(".CALCPACKAGE", "");

            // Valida o nome do arquivo a ser incluído no sistema.
            if (!ArquivoCalcEngineDAO.Instance.ValidaNomeArquivoNovo(arquivoCalcEngine.Nome))
            {
                throw new Exception("Já existem registros na tabela arquivo_calcengine ou existe arquivo na " +
                    "pasta ArquivosCalcEngine com o nome informado. Verifique estas informações e tente salvar o arquivo novamente.");
            }

            using (var m = new System.IO.MemoryStream(fluArquivoCalcEngine.FileBytes))
            {
                //if (!ArquivoMesaCorteDAO.Instance.ValidarCadastroCalcEngine(m))
                //    throw new Exception("O arquivo inserido está com falhas de validação");

                //m.Position = 0;

                var buffer = new byte[1024];
                var read = 0;

                using (var file = System.IO.File.Create(ProjetoConfig.CaminhoSalvarCalcEngine + fluArquivoCalcEngine.FileName))
                {
                    while ((read = m.Read(buffer, 0, buffer.Length)) > 0)
                        file.Write(buffer, 0, read);

                    file.Flush();
                }
            }
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstArquivoCalcEngine.aspx");
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            if (Request["IdArquivoCalcEngine"] != null)
            {
                var nomeArquivo = ArquivoCalcEngineDAO.Instance.ObtemNomeArquivo(null, Glass.Conversoes.StrParaUint(Request["IdArquivoCalcEngine"]));
                if (System.IO.File.Exists(ProjetoConfig.CaminhoSalvarCalcEngine + string.Format("\\{0}.calcpackage", nomeArquivo)))
                { 
                    Response.Redirect("~/Handlers/Download.ashx?filePath=" + 
                        (ProjetoConfig.CaminhoSalvarCalcEngine + string.Format("\\{0}.calcpackage", nomeArquivo)) + " &fileName=" + nomeArquivo + ".calcpackage");
                }
                else
                {
                    Glass.MensagemAlerta.ShowMsg("Nenhum arquivo encontrado.", this);
                }
            }

        }
    }
}
