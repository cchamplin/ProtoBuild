using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProtoBuild.Designer.DesignHost;

namespace ProtoBuild.Designer
{
    public partial class Designer : Form
    {
        private HostSurfaceManager _hostSurfaceManager = null;
        private int _formCount = 0;
        public Designer()
        {
            InitializeComponent();
            _hostSurfaceManager = new HostSurfaceManager();
            _hostSurfaceManager.AddService(typeof(IToolboxService), this.toolBox);
            _hostSurfaceManager.AddService(typeof(System.Windows.Forms.PropertyGrid), this.propertyGrid1);
            _hostSurfaceManager.AddService(typeof(System.Windows.Forms.ComboBox), this.comboBox1);
            this.tabControl1.SelectedIndexChanged += new EventHandler(tabControl1_SelectedIndexChanged);

            _formCount++;
            HostPane pane = _hostSurfaceManager.GetNewHost(typeof(Form), HostSurfaceManager.LoaderType.BasicDesignerLoader);
            AddTabForNewHost("Form" + _formCount.ToString() + " - " + "Design", pane);

        }

        private void AddTabForNewHost(string tabText, HostPane pane)
        {
            this.toolBox.DesignerHost = pane.DesignerHost;
            TabPage tabpage = new TabPage(tabText);
            //tabpage.Tag = CurrentMenuSelectionLoaderType;
            pane.Parent = tabpage;
            pane.Dock = DockStyle.Fill;
            this.tabControl1.TabPages.Add(tabpage);
            this.tabControl1.SelectedIndex = this.tabControl1.TabPages.Count - 1;
            _hostSurfaceManager.ActiveDesignSurface = pane.HostSurface;

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _formCount++;
            HostPane pane = _hostSurfaceManager.GetNewHost(typeof(Form), HostSurfaceManager.LoaderType.BasicDesignerLoader);
            AddTabForNewHost("Form" + _formCount.ToString() + " - " + "Design", pane);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = comboBox1.SelectedItem as HostSurface.ToolItem;
            if (item != null)
            {
                var surface = _hostSurfaceManager.ActiveDesignSurface as HostSurface;
                surface.ChangeSelection(new IComponent[] { item.control as IComponent });
                
                
            }
        }
        

    }
}
