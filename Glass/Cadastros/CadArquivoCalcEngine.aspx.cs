using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Collections.Generic;

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
                    dtvArquivoCalcEngine.ChangeMode(DetailsViewMode.Insert);
            }
        }

        protected void odsArquivoCalcEngine_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                // Salva o arquivo que ser� subido.
                var fluArquivoCalcEngine = ((FileUpload)dtvArquivoCalcEngine.FindControl("fluArquivoCalcEngine"));
                // Recupera o id do �ltimo arquivo CalcEngine inserido no sistema.
                var idArquivoCalcEngine = (uint)ArquivoCalcEngineDAO.Instance.ObtemUltimoIdArquivoCalcEngine();

                try
                {
                    // Vari�vel criada para recuperar os arquivos do .calcpackage.
                    CalcEngine.ProjectFilesPackage pacote = null;
                    // Vari�vel criada para ler a configura��o do projeto.
                    CalcEngine.Dxf.DxfProject projeto = null;
                    // Lista criada para setar as vari�veis do CalcEngine.
                    var lstVariaveisCalcEngine = new List<ArquivoCalcEngineVariavel>();

                    using (System.IO.Stream pacoteStream = fluArquivoCalcEngine.FileContent)
                    {
                        // Esse m�todo deserializa os dados do pacote que est�o contidos na Stream a recupera a inst�ncia do pacote de configura��o.
                        pacote = CalcEngine.ProjectFilesPackage.LoadPackage(pacoteStream);
                    }

                    // L� a configura��o do projeto.
                    projeto = CalcEngine.Dxf.DxfProject.LoadFromPackage(pacote);

                    // Seta as vari�veis do CalcEngine em uma lista.
                    foreach (var variavel in projeto.Variables)
                    {
                        if (variavel is CalcEngine.Constant || variavel is CalcEngine.Function)
                            continue;

                        // Cria uma nova vari�vel CalcEngine.
                        var variavelCalcEngine = new ArquivoCalcEngineVariavel();
                        variavelCalcEngine.VariavelCalcEngine = variavel.Name;
                        // Salva o valor padr�o da vari�vel somente se n�o forem medidas de altura ou largura.
                        variavelCalcEngine.ValorPadrao = (decimal)(variavel.Name.ToLower() == "altura" || variavel.Name.ToLower() == "largura" ||
                            variavel.Name.ToLower() == "alturabase" || variavel.Name.ToLower() == "largurabase" ? 0 : variavel.Value);
                        if (variavel.Name.ToLower() == "altura" || variavel.Name.ToLower() == "largura" ||
                            variavel.Name.ToLower() == "alturabase" || variavel.Name.ToLower() == "largurabase")
                            variavelCalcEngine.VariavelSistema = variavelCalcEngine.VariavelCalcEngine;

                        // Seta a vari�vel CalcEngine na lista.
                        lstVariaveisCalcEngine.Add(variavelCalcEngine);
                    }

                    // Salva as vari�veis do CalcEngine.
                    foreach (var variavel in lstVariaveisCalcEngine)
                    {
                        variavel.IdArquivoCalcEngine = idArquivoCalcEngine;
                        // Insere a vari�vel do arquivo CalcEngine e associa-a ao arquivo.
                        ArquivoCalcEngineVariavelDAO.Instance.Insert(variavel);
                    }
                }
                catch (Exception ex)
                {
                    // Caso o arquivo tenha sido salvo, ent�o deve ser exclu�do.
                    if (System.IO.File.Exists(ProjetoConfig.CaminhoSalvarCalcEngine + fluArquivoCalcEngine.FileName))
                        System.IO.File.Delete(ProjetoConfig.CaminhoSalvarCalcEngine + fluArquivoCalcEngine.FileName);

                    // Deleta as vari�veis associadas ao arquivo CalcEngine.
                    ArquivoCalcEngineVariavelDAO.Instance.DeletaPeloIdArquivoCalcEngine(idArquivoCalcEngine);
                    ArquivoCalcEngineDAO.Instance.DeleteByPrimaryKey(idArquivoCalcEngine);

                    // Retorna uma mensagem informando o erro ocorrido ao salvar o arquivo.
                    throw ex;
                }

                Response.Redirect("CadArquivoCalcEngine.aspx?IdArquivoCalcEngine=" + idArquivoCalcEngine);
            }
            else
            {
                // Deleta as vari�veis associadas ao arquivo CalcEngine.
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

            // Salva o arquivo que ser� subido.
            var fluArquivoCalcEngine = ((FileUpload)dtvArquivoCalcEngine.FindControl("fluArquivoCalcEngine"));

            if (fluArquivoCalcEngine != null && !String.IsNullOrEmpty(fluArquivoCalcEngine.FileName))
            {
                // Salva o nome do arquivo CalcEngine.
                var nomeArquivo = fluArquivoCalcEngine.FileName;

                try
                {
                    // Vari�vel criada para recuperar os arquivos do .calcpackage.
                    CalcEngine.ProjectFilesPackage pacote = null;
                    // Vari�vel criada para ler a configura��o do projeto.
                    CalcEngine.Dxf.DxfProject projeto = null;
                    // Lista criada para setar as vari�veis do CalcEngine.
                    var lstVariaveisCalcEngine = new List<ArquivoCalcEngineVariavel>();

                    using (System.IO.Stream pacoteStream = fluArquivoCalcEngine.FileContent)
                    {
                        // Esse m�todo deserializa os dados do pacote que est�o contidos na Stream a recupera a inst�ncia do pacote de configura��o.
                        pacote = CalcEngine.ProjectFilesPackage.LoadPackage(pacoteStream);
                    }

                    // L� a configura��o do projeto.
                    projeto = CalcEngine.Dxf.DxfProject.LoadFromPackage(pacote);

                    // Seta as vari�veis do CalcEngine em uma lista.
                    foreach (var variavel in projeto.Variables)
                    {
                        if (variavel is CalcEngine.Constant || variavel is CalcEngine.Function)
                            continue;

                        // Cria uma nova vari�vel CalcEngine.
                        var variavelCalcEngine = new ArquivoCalcEngineVariavel();
                        variavelCalcEngine.VariavelCalcEngine = variavel.Name;
                        // Salva o valor padr�o da vari�vel somente se n�o forem medidas de altura ou largura.
                        variavelCalcEngine.ValorPadrao = (decimal)(variavel.Name.ToLower() == "altura" || variavel.Name.ToLower() == "largura" ||
                            variavel.Name.ToLower() == "alturabase" || variavel.Name.ToLower() == "largurabase" ? 0 : variavel.Value);
                        if (variavel.Name.ToLower() == "altura" || variavel.Name.ToLower() == "largura" ||
                            variavel.Name.ToLower() == "alturabase" || variavel.Name.ToLower() == "largurabase")
                            variavelCalcEngine.VariavelSistema = variavelCalcEngine.VariavelCalcEngine;

                        // Seta a vari�vel CalcEngine na lista.
                        lstVariaveisCalcEngine.Add(variavelCalcEngine);
                    }

                    ArquivoCalcEngineVariavelDAO.Instance.DeletaPeloIdArquivoCalcEngine(arquivoCalcEngine.IdArquivoCalcEngine);

                    // Salva as vari�veis do CalcEngine.
                    foreach (var variavel in lstVariaveisCalcEngine)
                    {
                        variavel.IdArquivoCalcEngine = arquivoCalcEngine.IdArquivoCalcEngine;
                        // Insere a vari�vel do arquivo CalcEngine e associa-a ao arquivo.
                        ArquivoCalcEngineVariavelDAO.Instance.Insert(variavel);
                    }

                    // Atualiza o nome do arquivo.
                    arquivoCalcEngine.Nome = nomeArquivo.ToUpper().Replace(".CALCPACKAGE", "");

                    // Apaga o arquivo CalcEngine antigo.
                    if (System.IO.File.Exists(ProjetoConfig.CaminhoSalvarCalcEngine + arquivoCalcEngine.Nome + ".calcpackage"))
                        System.IO.File.Delete(ProjetoConfig.CaminhoSalvarCalcEngine + arquivoCalcEngine.Nome + ".calcpackage");
                    // Salva o arquivo CalcEngine.
                    fluArquivoCalcEngine.SaveAs(ProjetoConfig.CaminhoSalvarCalcEngine + arquivoCalcEngine.Nome + ".calcpackage");
                }
                catch (Exception ex)
                {
                    // Deleta as vari�veis associadas ao arquivo CalcEngine.
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
            // Salva o arquivo que ser� subido.
            var fluArquivoCalcEngine = ((FileUpload)dtvArquivoCalcEngine.FindControl("fluArquivoCalcEngine"));
            // Salva o nome do arquivo CalcEngine.
            arquivoCalcEngine.Nome = fluArquivoCalcEngine.FileName.ToUpper().Replace(".CALCPACKAGE", "");

            // Valida o nome do arquivo a ser inclu�do no sistema.
            if (!ArquivoCalcEngineDAO.Instance.ValidaNomeArquivoNovo(arquivoCalcEngine.Nome))
            {
                throw new Exception("J� existem registros na tabela arquivo_calcengine ou existe arquivo na " +
                    "pasta ArquivosCalcEngine com o nome informado. Verifique estas informa��es e tente salvar o arquivo novamente.");
            }

            // Salva o arquivo CalcEngine.
            fluArquivoCalcEngine.SaveAs(ProjetoConfig.CaminhoSalvarCalcEngine + fluArquivoCalcEngine.FileName);
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstArquivoCalcEngine.aspx");
        }
    }
}
