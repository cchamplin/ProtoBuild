using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProtoBuild.Designer.DesignHost
{
    public class HostSurface : DesignSurface
    {

        private ISelectionService _selectionService;

        public HostSurface()
            : base()
        {
            this.AddService(typeof(IMenuCommandService), new MenuCommandService(this));
        }
        public HostSurface(IServiceProvider parentProvider)
            : base(parentProvider)
        {
            this.AddService(typeof(IMenuCommandService), new MenuCommandService(this));
        }


        internal void Initialize()
        {

            Control control = null;
            IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));

            if (host == null)
                return;

            try
            {
                // Set the backcolor
                Type hostType = host.RootComponent.GetType();
                if (hostType == typeof(Form))
                {
                    control = this.View as Control;
                    control.BackColor = Color.White;
                }
                else if (hostType == typeof(UserControl))
                {
                    control = this.View as Control;
                    control.BackColor = Color.White;
                }
                else if (hostType == typeof(Component))
                {
                    control = this.View as Control;
                    control.BackColor = Color.FloralWhite;
                }
                else
                {
                    throw new Exception("Undefined Host Type: " + hostType.ToString());
                }

                // Set SelectionService - SelectionChanged event handler
                _selectionService = (ISelectionService)(this.ServiceContainer.GetService(typeof(ISelectionService)));
                _selectionService.SelectionChanged += new EventHandler(selectionService_SelectionChanged);


                var _containerService = (IComponentChangeService)(this.ServiceContainer.GetService(typeof(IComponentChangeService)));
                _containerService.ComponentAdded += _containerService_ComponentAdded;
                
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }
        public class ToolItem {
            public Control control;
            public override string ToString()
            {
                return control.Name + " " + control.GetType().Name;
            }
        }
        void _containerService_ComponentAdded(object sender, ComponentEventArgs e)
        {
            ComboBox comboBox = (ComboBox)this.GetService(typeof(ComboBox));
            var _containerService = (IContainer)(this.ServiceContainer.GetService(typeof(IContainer)));
            comboBox.Items.Clear();
            foreach (var component in _containerService.Components)
            {
                Control ct = component as Control;
                if (ct != null)
                {
                    comboBox.Items.Add(new ToolItem() { control = ct });
                }
            }
        }
        public void ChangeSelection(IComponent[] comps)
        {
            if (_selectionService != null)
            {
                ComponentCollection cl = new ComponentCollection(comps);
                _selectionService.SetSelectedComponents(cl, SelectionTypes.Primary);
              
            }
        }
        public GenericHostLoader Loader
        {
            get;
            set;
        }

        /// <summary>
        /// When the selection changes this sets the PropertyGrid's selected component 
        /// </summary>
        private void selectionService_SelectionChanged(object sender, EventArgs e)
        {
            if (_selectionService != null)
            {
                ICollection selectedComponents = _selectionService.GetSelectedComponents();
                PropertyGrid propertyGrid = (PropertyGrid)this.GetService(typeof(PropertyGrid));


                object[] comps = new object[selectedComponents.Count];
                int i = 0;

                foreach (Object o in selectedComponents)
                {
                    comps[i] = o;
                    i++;
                }
                propertyGrid.SelectedObjects = comps;
                if (comps.Length > 0)
                {

                    var item = comps[0];
                    ComboBox comboBox = (ComboBox)this.GetService(typeof(ComboBox));
                    if (comboBox.SelectedItem != null)
                    {
                        var t = comboBox.SelectedItem as ToolItem;
                        if (t.control == item)
                        {
                            return;
                        }

                    }
                    foreach (var comboItem in comboBox.Items) {
                        var toolItem = comboItem as ToolItem;
                        if (toolItem.control == item)
                        {
                            comboBox.SelectedItem = toolItem;
                        }
                    }
                }

                
            }
        }

        public void AddService(Type type, object serviceInstance)
        {
            this.ServiceContainer.AddService(type, serviceInstance);
        }
    }
}
