using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadFerragem : System.Web.UI.Page
    {

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dtvFerragem.Register("~/Listas/LstFerragem.aspx");
            odsFerragem.Register();
            odsFerragem.Selected += odsFerragem_Selected;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadFerragem));

            Glass.Projeto.Negocios.Entidades.Ferragem ferragem = null;

            var idFerragem = Conversoes.StrParaIntNullable(Request["IdFerragem"]);
            if (idFerragem != null && idFerragem.GetValueOrDefault() > 0)
            {
                dtvFerragem.ChangeMode(DetailsViewMode.Edit);

                if (!IsPostBack)
                {
                    ferragem = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Projeto.Negocios.IFerragemFluxo>()
                        .ObterFerragem(int.Parse(Request["idFerragem"]));

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.Topo))
                        ((CheckBox)dtvFerragem.FindControl("cbxTopo")).Checked = true;

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.TopoEsquerda))
                        ((CheckBox)dtvFerragem.FindControl("cbxTopoEsquerda")).Checked = true;

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.TopoCentro))
                        ((CheckBox)dtvFerragem.FindControl("cbxTopoCentro")).Checked = true;

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.TopoDireita))
                        ((CheckBox)dtvFerragem.FindControl("cbxTopoDireita")).Checked = true;

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.Esquerda))
                        ((CheckBox)dtvFerragem.FindControl("cbxEsquerda")).Checked = true;

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.CentroEsquerda))
                        ((CheckBox)dtvFerragem.FindControl("cbxCentroEsquerda")).Checked = true;

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.CentroDireita))
                        ((CheckBox)dtvFerragem.FindControl("cbxCentroDireita")).Checked = true;

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.Direita))
                        ((CheckBox)dtvFerragem.FindControl("cbxDireita")).Checked = true;

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.BaseEsquerda))
                        ((CheckBox)dtvFerragem.FindControl("cbxBaseEsquerda")).Checked = true;

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.BaseCentro))
                        ((CheckBox)dtvFerragem.FindControl("cbxBaseCentro")).Checked = true;

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.BaseDireita))
                        ((CheckBox)dtvFerragem.FindControl("cbxBaseDireita")).Checked = true;

                    if (ferragem.EstiloAncoragem.HasFlag(Data.Model.EstiloAncoragem.Base))
                        ((CheckBox)dtvFerragem.FindControl("cbxBase")).Checked = true;
                }
            };

            if (!IsPostBack && !UserInfo.GetUserInfo.IsAdminSync)
            {
                ((CheckBox)dtvFerragem.FindControl("cbxMedidasEstaticas")).Enabled = false;
                ((CheckBox)dtvFerragem.FindControl("cbxPodeRotacionar")).Enabled = false;
                ((CheckBox)dtvFerragem.FindControl("cbxPodeEspelhar")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxTopo")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxTopoEsquerda")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxTopoCentro")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxTopoDireita")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxEsquerda")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxCentroEsquerda")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxCentroDireita")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxDireita")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxBaseEsquerda")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxBaseCentro")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxBaseDireita")).Enabled = false;

                ((CheckBox)dtvFerragem.FindControl("cbxBase")).Enabled = false;
            }

            if(idFerragem > 0)
            // Carrega imagem na tela
            imgFigura.ImageUrl = Glass.Global.UI.Web.Process.Ferragem.FerragemRepositorioImagens.Instance.ObterUrl((int)idFerragem);
        }
        

        protected void odsFerragem_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var ferragem = e.InputParameters[0] as Glass.Projeto.Negocios.Entidades.Ferragem;
            ferragem.EstiloAncoragem = ObterEstiloAncoragem();

            CarregarCalcEngineFile(ferragem);
            CarregarImagem(ferragem);
        }

        private void odsFerragem_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            var ferragem = e.ReturnValue as Glass.Projeto.Negocios.Entidades.Ferragem;
        }

        protected void odsFerragem_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var ferragem = e.InputParameters[0] as Glass.Projeto.Negocios.Entidades.Ferragem;

            if (UserInfo.GetUserInfo.IsAdminSync)
                ferragem.EstiloAncoragem = ObterEstiloAncoragem();

            CarregarCalcEngineFile(ferragem);
            CarregarImagem(ferragem);
        }

        protected void odsFerragem_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
                throw e.Exception;
            else
            {
                var saveResult = e.ReturnValue as Colosoft.Business.SaveResult;
                if (!saveResult)
                    throw new Colosoft.DetailsException(saveResult.Message);
            }
        }

        protected void odsFerragem_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
                throw e.Exception;
            else
            {
                var saveResult = e.ReturnValue as Colosoft.Business.SaveResult;
                if (!saveResult)
                    throw new Colosoft.DetailsException(saveResult.Message);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstFerragem.aspx");
        }

        /// <summary>
        /// Salva a imagem carregada para a ferragem.
        /// </summary>
        /// <param name="ferragem"></param>
        public void CarregarImagem(Glass.Projeto.Negocios.Entidades.Ferragem ferragem)
        {
            // Recupera o control de upload
            var fileUpload = (FileUpload)dtvFerragem.FindControl("filImagem");

            if (fileUpload != null)
            {
                // Verifica se tem imagem e se a extenssão está correta.
                if (fileUpload.HasFile && fileUpload.FileName.EndsWith(".jpg"))
                {
                    EventHandler<Colosoft.Business.EntitySavedEventArgs> ferragemSalva = null;

                    ferragemSalva = (sender, args) =>
                    {
                        try
                        {
                            var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                                .Current.GetInstance<Glass.Projeto.Negocios.Entidades.IFerragemRepositorioImagens>();

                            // Salva a imagem da ferragem
                            repositorio.SalvarImagem(ferragem.IdFerragem, fileUpload.PostedFile.InputStream);
                        }
                        finally
                        {
                            ferragem.Saved -= ferragemSalva;
                        }
                    };

                    ferragem.Saved += ferragemSalva;

                    // Carrega imagem na tela
                    imgFigura.ImageUrl = Glass.Global.UI.Web.Process.Ferragem.FerragemRepositorioImagens.Instance.ObterUrl((int)ferragem.IdFerragem);
                }
                else if (!string.IsNullOrEmpty(fileUpload.FileName))
                {
                    throw new Exception(string.Format("O arquivo ({0}) não pode ser inserido. Verifique se a extensão da imagem é JPG e tente novamente. ", fileUpload.FileName));
                }
            }
        }

        /// <summary>
        /// Carrega o arquivo CalcEngine para a ferragem.
        /// </summary>
        /// <param name="ferragem"></param>
        public void CarregarCalcEngineFile(Glass.Projeto.Negocios.Entidades.Ferragem ferragem)
        {
            // Recupera o control de upload
            var fileUpload = (FileUpload)dtvFerragem.FindControl("filCalcPkg");

            if (fileUpload != null && fileUpload.HasFile)
            {

                if (!StringComparer.InvariantCultureIgnoreCase.Equals(System.IO.Path.GetExtension(fileUpload.FileName), ".calcpackage"))
                    throw new Exception(string.Format("O arquivo ({0}) não pode ser inserido. Verifique se a extensão da imagem é CALCPACKAGE e tente novamente. ", fileUpload.FileName));

                CalcEngine.Dxf.DxfProject project = null;

                try
                {
                    // Tenta abrir o projeto do arquivo
                    project = CalcEngine.Dxf.DxfProject.Open(fileUpload.PostedFile.InputStream);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Não foi possível carregar os dados do arquivo '{0}' informado.", fileUpload.FileName), ex);
                }

                var constantesProcessadas = new List<Glass.Projeto.Negocios.Entidades.ConstanteFerragem>();

                foreach (var variable in project.Variables.Where(f => f.GetType() == typeof(CalcEngine.Variable)))
                {
                    var constante = ferragem.Constantes.FirstOrDefault(f => f.Nome == variable.Name);

                    if (constante == null)
                    {
                        constante = new Glass.Projeto.Negocios.Entidades.ConstanteFerragem
                        {
                            Nome = variable.Name,
                            Valor = variable.Value
                        };
                    }
                    else
                        constante.Valor = variable.Value;

                    // Adiciona somente as constantes que não existirem no cadastro da ferragem no WebGlass.
                    if (!ferragem.Constantes.Contains(constante))
                        ferragem.Constantes.Add(constante);

                    constantesProcessadas.Add(constante);
                }

                // No WebGlass, remove todas as constantes que não existem no novo arquivo CalcPackage.
                foreach (var constante in ferragem.Constantes.Where(f => !constantesProcessadas.Contains(f)))
                    ferragem.Constantes.Remove(constante);

                EventHandler<Colosoft.Business.EntitySavedEventArgs> ferragemSalva = null;

                // Configura o método anonimo para ser acionado quando os dados da ferragem forem salvos
                ferragemSalva = (sender, args) =>
                {
                    try
                    {
                        var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                            .Current.GetInstance<Glass.Projeto.Negocios.Entidades.IFerragemRepositorioCalcPackage>();

                        using (var stream = new System.IO.MemoryStream())
                        {
                            // Cria o pacote para onde serão salvo os dados do CalcPackage
                            var package = CalcEngine.ProjectFilesPackage.CreatePackage(stream, false);
                            project.Save(package);
                            package.Close();

                            stream.Position = 0;

                            // Salva os dados no repositório das ferragens
                            repositorio.SalvarCalcPackage(ferragem.IdFerragem, stream);
                        }
                    }
                    finally
                    {
                        ferragem.Saved -= ferragemSalva;
                    }
                };

                ferragem.Saved += ferragemSalva;
            }
        }

        /// <summary>
        /// Recupera os estilo de ancoragem configurado nos controles da página.
        /// </summary>
        /// <returns></returns>
        private Data.Model.EstiloAncoragem ObterEstiloAncoragem()
        {
            var topo = ((CheckBox)dtvFerragem.FindControl("cbxTopo")).Checked;
            var topoEsquerda = ((CheckBox)dtvFerragem.FindControl("cbxTopoEsquerda")).Checked;
            var topoCentro = ((CheckBox)dtvFerragem.FindControl("cbxTopoCentro")).Checked;
            var topoDireita = ((CheckBox)dtvFerragem.FindControl("cbxTopoDireita")).Checked;
            var esquerda = ((CheckBox)dtvFerragem.FindControl("cbxEsquerda")).Checked;
            var centroEsquerda = ((CheckBox)dtvFerragem.FindControl("cbxCentroEsquerda")).Checked;
            var centroDireita = ((CheckBox)dtvFerragem.FindControl("cbxCentroDireita")).Checked;
            var direita = ((CheckBox)dtvFerragem.FindControl("cbxDireita")).Checked;
            var baseEsquerda = ((CheckBox)dtvFerragem.FindControl("cbxBaseEsquerda")).Checked;
            var baseCentro = ((CheckBox)dtvFerragem.FindControl("cbxBaseCentro")).Checked;
            var baseDireita = ((CheckBox)dtvFerragem.FindControl("cbxBaseDireita")).Checked;
            var basebase = ((CheckBox)dtvFerragem.FindControl("cbxBase")).Checked;

            var ancoragem = Data.Model.EstiloAncoragem.Nenhuma;

            if (topo)
                ancoragem |= Data.Model.EstiloAncoragem.Topo;

            if (topoEsquerda)
                ancoragem |= Data.Model.EstiloAncoragem.TopoEsquerda;

            if (topoCentro)
                ancoragem |= Data.Model.EstiloAncoragem.TopoCentro;

            if (topoDireita)
                ancoragem |= Data.Model.EstiloAncoragem.TopoDireita;

            if (esquerda)
                ancoragem |= Data.Model.EstiloAncoragem.Esquerda;

            if (centroEsquerda)
                ancoragem |= Data.Model.EstiloAncoragem.CentroEsquerda;

            if (centroDireita)
                ancoragem |= Data.Model.EstiloAncoragem.CentroDireita;

            if (direita)
                ancoragem |= Data.Model.EstiloAncoragem.Direita;

            if (baseEsquerda)
                ancoragem |= Data.Model.EstiloAncoragem.BaseEsquerda;

            if (baseCentro)
                ancoragem |= Data.Model.EstiloAncoragem.BaseCentro;

            if (baseDireita)
                ancoragem |= Data.Model.EstiloAncoragem.BaseDireita;

            if (basebase)
                ancoragem |= Data.Model.EstiloAncoragem.Base;

            return ancoragem;
        }

        [Ajax.AjaxMethod]
        public string IsAdminSync()
        {
            var adminSync = UserInfo.GetUserInfo.IsAdminSync.ToString().ToLower();

            return adminSync;
        }
    }
}