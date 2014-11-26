using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace ProtoBuild.Designer.DesignHost
{
    public partial class HostPane : UserControl
    {
        private HostSurface _hostSurface;
        public HostPane(HostSurface surface)
        {
            InitializeComponent();
            try
            {
                if (surface == null)
                    return;
                _hostSurface = surface;
                Control control = _hostSurface.View as Control;

                control.Parent = this;
                control.Dock = DockStyle.Fill;
                control.Visible = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }
        public HostSurface HostSurface
        {
            get
            {
                return _hostSurface;
            }
        }
        public IDesignerHost DesignerHost
        {
            get
            {
                return (IDesignerHost)_hostSurface.GetService(typeof(IDesignerHost));
            }
        }
    }
}
