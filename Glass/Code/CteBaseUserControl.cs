using System;
using System.Collections.Generic;
using System.Linq;
using WebGlass.Business.ConhecimentoTransporte.Entidade;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Glass.UI.Web
{
    public abstract class CteBaseUserControl : BaseUserControl
    {
        private WebGlass.Business.ConhecimentoTransporte.Entidade.Cte.TipoDocumentoCteEnum _tipoDocumentoCte;
        private List<BaseValidator> Validadores, Obrigatorios;
    
        #region Eventos
    
        public event EventHandler AlterouTipoDocumentoCte;
    
        #endregion
    
        #region Propriedades
    
        public virtual Cte.TipoDocumentoCteEnum TipoDocumentoCte
        {
            get
            {
                if (_tipoDocumentoCte == 0)
                {
                    _tipoDocumentoCte = (Cte.TipoDocumentoCteEnum)Conversoes.StrParaInt(Request["tipo"]);

                    if (_tipoDocumentoCte == 0)
                        _tipoDocumentoCte = (Cte.TipoDocumentoCteEnum)WebGlass.Business.ConhecimentoTransporte.Fluxo.BuscarCte.
                            Instance.GetCte(Conversoes.StrParaUint(Request["idCte"])).TipoDocumentoCte;
                }

                return _tipoDocumentoCte;
            }
            set
            {
                if (value == 0)
                {
                    _tipoDocumentoCte = (Cte.TipoDocumentoCteEnum)Conversoes.StrParaInt(Request["tipo"]);

                    if (_tipoDocumentoCte == 0)
                        _tipoDocumentoCte = (Cte.TipoDocumentoCteEnum)WebGlass.Business.ConhecimentoTransporte.Fluxo.BuscarCte.
                            Instance.GetCte(Conversoes.StrParaUint(Request["idCte"])).TipoDocumentoCte;
                }
                else
                    _tipoDocumentoCte = value;
    
                if (AlterouTipoDocumentoCte != null)
                    AlterouTipoDocumentoCte(this, EventArgs.Empty);
            }
        }
    
        public abstract IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada { get; }
    
        #endregion
    
        #region Métodos de suporte
    
        protected string ObtemTextoCampoObrigatorio(params BaseValidator[] validadores)
        {
            bool obrigatorio = false;
            foreach (var v in validadores)
                if (IsValidadorObrigatorio(v))
                {
                    obrigatorio = true;
                    break;
                }
    
            return obrigatorio ? " *" : String.Empty;
        }
    
        private void RecuperaValidadores()
        {
            if (Validadores.Count > 0)
                return;
    
            Queue<Control> controles = new Queue<Control>();
    
            foreach (Control c in this.Controls)
                controles.Enqueue(c);
    
            while (controles.Count > 0)
            {
                var c = controles.Dequeue();
    
                if (c is BaseValidator)
                    Validadores.Add(c as BaseValidator);
    
                else if (c.Controls.Count > 0)
                    foreach (Control c1 in c.Controls)
                        controles.Enqueue(c1);
            }
        }
    
        private void RecuperaValidadoresObrigatorios()
        {
            RecuperaValidadores();
    
            if (Validadores.Count == 0 || Obrigatorios.Count > 0)
                return;
    
            Obrigatorios.AddRange(Validadores.ToArray());
    
            if (TipoDocumentoCte == Cte.TipoDocumentoCteEnum.EntradaTerceiros)
                Obrigatorios = Obrigatorios.Intersect(ValidadoresObrigatoriosEntrada ?? new BaseValidator[0]).ToList();
    
            Obrigatorios = Obrigatorios.Where(x => x.Enabled && x.Visible).ToList();
        }
    
        private bool IsValidadorObrigatorio(BaseValidator validador)
        {
            RecuperaValidadoresObrigatorios();
            return Obrigatorios.Count(x => x.ClientID == validador.ClientID) > 0;
        }
    
        #endregion
    
        public CteBaseUserControl()
        {
            Validadores = new List<BaseValidator>();
            Obrigatorios = new List<BaseValidator>();
    
            this.PreRender += new EventHandler(CteBaseUserControl_PreRender);
        }
    
        private void CteBaseUserControl_PreRender(object sender, EventArgs e)
        {
            RecuperaValidadores();
    
            foreach (var v in Validadores)
                if (!IsValidadorObrigatorio(v))
                {
                    v.Enabled = false;
                    v.Visible = false;
                }
        }
    }
}
