using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProtoBuild.Designer.DesignHost
{
    public class HostSurfaceManager : DesignSurfaceManager
    {

        public enum LoaderType
        {
            BasicDesignerLoader = 1,
            NoLoader = 2
        }

        public HostSurfaceManager()
            : base()
        {
            this.AddService(typeof(INameCreationService), new NameCreationService());
            this.ActiveDesignSurfaceChanged += new ActiveDesignSurfaceChangedEventHandler(HostSurfaceManager_ActiveDesignSurfaceChanged);
        }

        protected override DesignSurface CreateDesignSurfaceCore(IServiceProvider parentProvider)
        {
            return new HostSurface(parentProvider);
        }

        private HostPane GetNewHost(Type rootComponentType)
        {
            HostSurface hostSurface = (HostSurface)this.CreateDesignSurface(this.ServiceContainer);

            if (rootComponentType == typeof(Form))
                hostSurface.BeginLoad(typeof(Form));
            else if (rootComponentType == typeof(UserControl))
                hostSurface.BeginLoad(typeof(UserControl));
            else if (rootComponentType == typeof(Component))
                hostSurface.BeginLoad(typeof(Component));
            else
                throw new Exception("Undefined Host Type: " + rootComponentType.ToString());

            hostSurface.Initialize();
            this.ActiveDesignSurface = hostSurface;
            return new HostPane(hostSurface);
        }


        public HostPane GetNewHost(Type rootComponentType, LoaderType loaderType)
        {
            if (loaderType == LoaderType.NoLoader)
                return GetNewHost(rootComponentType);

            HostSurface hostSurface = (HostSurface)this.CreateDesignSurface(this.ServiceContainer);
            IDesignerHost host = (IDesignerHost)hostSurface.GetService(typeof(IDesignerHost));

            switch (loaderType)
            {
                case LoaderType.BasicDesignerLoader:
                    GenericHostLoader basicHostLoader = new GenericHostLoader(rootComponentType);
                    hostSurface.BeginLoad(basicHostLoader);
                    hostSurface.Loader = basicHostLoader;
                    break;

                default:
                    throw new Exception("Loader is not defined: " + loaderType.ToString());
            }

            hostSurface.Initialize();
            return new HostPane(hostSurface);
        }


        public HostPane GetNewHost(string fileName)
        {
            if (fileName == null || !File.Exists(fileName))
                MessageBox.Show("FileName is incorrect: " + fileName);

            LoaderType loaderType = LoaderType.NoLoader;

            if (fileName.EndsWith("xml"))
                loaderType = LoaderType.BasicDesignerLoader;

            if (loaderType == LoaderType.NoLoader)
            {
                throw new Exception("File cannot be opened. Please check the type or extension of the file. Supported format is Xml");
            }

            HostSurface hostSurface = (HostSurface)this.CreateDesignSurface(this.ServiceContainer);
            IDesignerHost host = (IDesignerHost)hostSurface.GetService(typeof(IDesignerHost));

            GenericHostLoader basicHostLoader = new GenericHostLoader(fileName);
            hostSurface.BeginLoad(basicHostLoader);
            hostSurface.Loader = basicHostLoader;
            hostSurface.Initialize();
            return new HostPane(hostSurface);
        }

        public void AddService(Type type, object serviceInstance)
        {
            this.ServiceContainer.AddService(type, serviceInstance);
        }

        void HostSurfaceManager_ActiveDesignSurfaceChanged(object sender, ActiveDesignSurfaceChangedEventArgs e)
        {
            //ToolWindows.OutputWindow o = this.GetService(typeof(ToolWindows.OutputWindow)) as ToolWindows.OutputWindow;
           // o.RichTextBox.Text += "New host added.\n";
            Trace.WriteLine("New host added");
        }
    }
}
