using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel.Design;

namespace ProtoBuild.Designer.Tools
{
    public partial class ToolBox : UserControl, IToolboxService
    {

        private List<ToolCategory> _toolCategories;
        private ListBox _toolList;
        private Button[] _categoryPages;
        private System.Drawing.Design.ToolboxItem _pointer;
        private int _selectedIndex = 0;

        private Type[] windowsFormsToolTypes = new Type[] {
			//typeof(System.Windows.Forms.PropertyGrid), 
            typeof(System.Windows.Forms.Label), 
            typeof(System.Windows.Forms.LinkLabel), 
            typeof(System.Windows.Forms.Button), 
            typeof(System.Windows.Forms.TextBox), 
            typeof(System.Windows.Forms.CheckBox), 
            typeof(System.Windows.Forms.RadioButton), 
            typeof(System.Windows.Forms.GroupBox), 
            typeof(System.Windows.Forms.PictureBox),
            //typeof(System.Windows.Forms.Panel), 
            //typeof(System.Windows.Forms.DataGrid), 
            //typeof(System.Windows.Forms.ListBox), 
            //typeof(System.Windows.Forms.CheckedListBox), 
            typeof(System.Windows.Forms.ComboBox), 
            //typeof(System.Windows.Forms.ListView), 
            //typeof(System.Windows.Forms.TreeView), 
            //typeof(System.Windows.Forms.TabControl), 
            typeof(System.Windows.Forms.DateTimePicker), 
            //typeof(System.Windows.Forms.MonthCalendar), 
            //typeof(System.Windows.Forms.HScrollBar), 
            //typeof(System.Windows.Forms.VScrollBar), 
            //typeof(System.Windows.Forms.Timer), 
            //typeof(System.Windows.Forms.Splitter),
            //typeof(System.Windows.Forms.DomainUpDown), 
            //typeof(System.Windows.Forms.NumericUpDown), 
            //typeof(System.Windows.Forms.TrackBar), 
            //typeof(System.Windows.Forms.ProgressBar), 
            //typeof(System.Windows.Forms.RichTextBox), 
            //typeof(System.Windows.Forms.ImageList), 
            //typeof(System.Windows.Forms.HelpProvider), 
            //typeof(System.Windows.Forms.ToolTip), 
            //typeof(System.Windows.Forms.ToolBar), 
            //typeof(System.Windows.Forms.StatusBar), 
            //typeof(System.Windows.Forms.UserControl), 
            //typeof(System.Windows.Forms.NotifyIcon), 
            //typeof(System.Windows.Forms.OpenFileDialog), 
            //typeof(System.Windows.Forms.SaveFileDialog), 
            //typeof(System.Windows.Forms.FontDialog), 
            //typeof(System.Windows.Forms.ColorDialog), 
            //typeof(System.Windows.Forms.PrintDialog), 
            //typeof(System.Windows.Forms.PrintPreviewDialog), 
            //typeof(System.Windows.Forms.PrintPreviewControl), 
            //typeof(System.Windows.Forms.ErrorProvider), 
            //typeof(System.Drawing.Printing.PrintDocument), 
            //typeof(System.Windows.Forms.PageSetupDialog)
		};


        public ToolBox()
        {
            InitializeComponent();

            _pointer = new System.Drawing.Design.ToolboxItem();
            _pointer.DisplayName = "<Pointer>";
            _pointer.Bitmap = new System.Drawing.Bitmap(16, 16);


            _toolCategories = PopulateToolboxCategories();

            FillToolbox();

            _toolList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.list_KeyDown);
            _toolList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.list_MouseDown);
            _toolList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.list_DrawItem);
            //PrintToolbox();
        }
        public IDesignerHost DesignerHost { get; set; }


        private List<ToolCategory> PopulateToolboxCategories()
        {
            List<ToolCategory> toolboxCategories = new List<ToolCategory>();
            string[] categoryNames = { "Basic" };

            for (int i = 0; i < categoryNames.Length; i++)
            {
                ToolCategory toolboxTab = new ToolCategory(categoryNames[i]);

                PopulateToolboxItems(toolboxTab);
                toolboxCategories.Add(toolboxTab);
            }

            return toolboxCategories;
        }
        private void PopulateToolboxItems(ToolCategory toolCategory)
        {
            if (toolCategory == null)
                return;

            Type[] typeArray = null;

            switch (toolCategory.Name)
            {
                case "Basic":
                    typeArray = windowsFormsToolTypes;
                    break;
                default:
                    break;
            }

            List<Tool> toolItems = new List<Tool>();

            for (int i = 0; i < typeArray.Length; i++)
            {
                Tool toolItem = new Tool();

                toolItem.Type = typeArray[i];
                toolItem.Name = typeArray[i].Name;
                toolItems.Add(toolItem);
            }

            toolCategory.Tools = toolItems;
        }

        public void FillToolbox()
        {
            CreateControls();
            ConfigureControls();
            UpdateTools(this._toolCategories.Count - 1);
        }

        private void CreateControls()
        {
            this.Controls.Clear();
            _toolList = new ListBox();
            _categoryPages = new Button[this._toolCategories.Count];
        }
        private void ConfigureControls()
        {
            this.SuspendLayout();
            for (int i = this._toolCategories.Count - 1; i >= 0; i--)
            {
                // 
                // Tab Button
                // 
                Button button = new Button();

                button.Dock = System.Windows.Forms.DockStyle.Top;
                button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
                button.Location = new System.Drawing.Point(0, (i + 1) * 20);
                button.Name = this._toolCategories[i].Name;
                button.Size = new System.Drawing.Size(this.Width, 20);
                button.TabIndex = i + 1;
                button.Text = this._toolCategories[i].Name;
                button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                button.Tag = i;
                button.Click += new EventHandler(categoryClick);
                this.Controls.Add(button);
                _categoryPages[i] = button;
            }

            // 
            // toolboxTitleButton
            // 
            /*Button toolboxTitleButton = new Button();

            toolboxTitleButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
            toolboxTitleButton.Dock = System.Windows.Forms.DockStyle.Top;
            toolboxTitleButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            toolboxTitleButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            toolboxTitleButton.Location = new System.Drawing.Point(0, 0);
            toolboxTitleButton.Name = "toolboxTitleButton";
            toolboxTitleButton.Size = new System.Drawing.Size(Toolbox.Width, 20);
            toolboxTitleButton.TabIndex = 0;
            toolboxTitleButton.Text = "Toolbox";
            toolboxTitleButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            Toolbox.Controls.Add(toolboxTitleButton);*/

            // 
            // listBox
            // 
            ListBox listBox = new ListBox();

            listBox.BackColor = System.Drawing.SystemColors.ControlLight;
            listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            listBox.ItemHeight = 18;
            listBox.Location = new System.Drawing.Point(0, (this._toolCategories.Count) * 20);
            listBox.Name = "ToolsListBox";
            listBox.Size = new System.Drawing.Size(this.Width, this.Height - ((this._toolCategories.Count-1) * 20));
            listBox.TabIndex = this._toolCategories.Count + 1;

            this.Controls.Add(listBox);
            UpdateTools(this._toolCategories.Count - 1);
            this.ResumeLayout();
            _toolList = listBox;
            this.SizeChanged += new EventHandler(sizeChanged);
        }

        private void UpdateTools(int tabIndex)
        {
            _toolList.Items.Clear();
            _toolList.Items.Add(_pointer);
            if (this._toolCategories.Count <= 0)
                return;

            ToolCategory toolCategory = this._toolCategories[tabIndex];
            List<Tool> toolItems = toolCategory.Tools;

            foreach (Tool toolItem in toolItems)
            {
                Type type = toolItem.Type;
                System.Drawing.Design.ToolboxItem tbi = new System.Drawing.Design.ToolboxItem(type);
                System.Drawing.ToolboxBitmapAttribute tba = TypeDescriptor.GetAttributes(type)[typeof(System.Drawing.ToolboxBitmapAttribute)] as System.Drawing.ToolboxBitmapAttribute;

                if (tba != null)
                {
                    tbi.Bitmap = (System.Drawing.Bitmap)tba.GetImage(type);
                }

                _toolList.Items.Add(tbi);
            }
        }
        private void categoryClick(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
                return;

            int index = (int)button.Tag;

            if (button.Dock == DockStyle.Top)
            {
                for (int i = index + 1; i < _categoryPages.Length; i++)
                    _categoryPages[i].Dock = DockStyle.Bottom;
            }
            else
            {
                for (int i = 0; i <= index; i++)
                    _categoryPages[i].Dock = DockStyle.Top;
            }

            _toolList.Location = new System.Drawing.Point(0, (index + 1) * 20);
            UpdateTools(index);
        }
        private void sizeChanged(object sender, EventArgs e)
        {
            _toolList.Size = new System.Drawing.Size(this.Width, this.Height - (this._toolCategories.Count) * 20);
        }



        private void list_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            try
            {
                ListBox lbSender = sender as ListBox;
                if (lbSender == null)
                    return;

                // If this tool is the currently selected tool, draw it with a highlight.
                if (_selectedIndex == e.Index)
                {
                    e.Graphics.FillRectangle(Brushes.LightSlateGray, e.Bounds);
                }

                System.Drawing.Design.ToolboxItem tbi = lbSender.Items[e.Index] as System.Drawing.Design.ToolboxItem;
                Rectangle BitmapBounds = new Rectangle(e.Bounds.Location.X, e.Bounds.Location.Y + e.Bounds.Height / 2 - tbi.Bitmap.Height / 2, tbi.Bitmap.Width, tbi.Bitmap.Height);
                Rectangle StringBounds = new Rectangle(e.Bounds.Location.X + BitmapBounds.Width + 5, e.Bounds.Location.Y, e.Bounds.Width - BitmapBounds.Width, e.Bounds.Height);

                StringFormat format = new StringFormat();

                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Near;
                e.Graphics.DrawImage(tbi.Bitmap, BitmapBounds);
                e.Graphics.DrawString(tbi.DisplayName, new Font("Tahoma", 11, FontStyle.Regular, GraphicsUnit.World), Brushes.Black, StringBounds, format);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void list_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                ListBox lbSender = sender as ListBox;
                Rectangle lastSelectedBounds = lbSender.GetItemRectangle(0);
                try
                {
                    lastSelectedBounds = lbSender.GetItemRectangle(_selectedIndex);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                _selectedIndex = lbSender.IndexFromPoint(e.X, e.Y); // change our selection
                lbSender.SelectedIndex = _selectedIndex;
                lbSender.Invalidate(lastSelectedBounds); // clear highlight from last selection
                lbSender.Invalidate(lbSender.GetItemRectangle(_selectedIndex)); // highlight new one

                if (_selectedIndex != 0)
                {
                    if (e.Clicks == 2)
                    {
                        IDesignerHost idh = (IDesignerHost)this.DesignerHost.GetService(typeof(IDesignerHost));
                        IToolboxUser tbu = idh.GetDesigner(idh.RootComponent as IComponent) as IToolboxUser;

                        if (tbu != null)
                        {
                            tbu.ToolPicked((System.Drawing.Design.ToolboxItem)(lbSender.Items[_selectedIndex]));
                        }
                    }
                    else if (e.Clicks < 2)
                    {
                        System.Drawing.Design.ToolboxItem tbi = lbSender.Items[_selectedIndex] as System.Drawing.Design.ToolboxItem;
                        IToolboxService tbs = this;

                        // The IToolboxService serializes ToolboxItems by packaging them in DataObjects.
                        DataObject d = tbs.SerializeToolboxItem(tbi) as DataObject;

                        try
                        {
                            lbSender.DoDragDrop(d, DragDropEffects.Copy);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void list_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                ListBox lbSender = sender as ListBox;
                Rectangle lastSelectedBounds = lbSender.GetItemRectangle(0);
                try
                {
                    lastSelectedBounds = lbSender.GetItemRectangle(_selectedIndex);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                switch (e.KeyCode)
                {
                    case Keys.Up: if (_selectedIndex > 0)
                        {
                            _selectedIndex--; // change selection
                            lbSender.SelectedIndex = _selectedIndex;
                            lbSender.Invalidate(lastSelectedBounds); // clear old highlight
                            lbSender.Invalidate(lbSender.GetItemRectangle(_selectedIndex)); // add new one
                        }
                        break;

                    case Keys.Down: if (_selectedIndex + 1 < lbSender.Items.Count)
                        {
                            _selectedIndex++; // change selection
                            lbSender.SelectedIndex = _selectedIndex;
                            lbSender.Invalidate(lastSelectedBounds); // clear old highlight
                            lbSender.Invalidate(lbSender.GetItemRectangle(_selectedIndex)); // add new one
                        }
                        break;

                    case Keys.Enter:
                        if (DesignerHost == null)
                            MessageBox.Show("idh Null");

                        IToolboxUser tbu = DesignerHost.GetDesigner(DesignerHost.RootComponent as IComponent) as IToolboxUser;

                        if (tbu != null)
                        {
                            // Enter means place the tool with default location and default size.
                            tbu.ToolPicked((System.Drawing.Design.ToolboxItem)(lbSender.Items[_selectedIndex]));
                            lbSender.Invalidate(lastSelectedBounds); // clear old highlight
                            lbSender.Invalidate(lbSender.GetItemRectangle(_selectedIndex)); // add new one
                        }

                        break;

                    default:
                        {
                            Console.WriteLine("Error: Not able to add");
                            break;
                        }
                } // switch
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        public void AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host)
        {

        }

        public void AddCreator(ToolboxItemCreatorCallback creator, string format)
        {
        }

        public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, IDesignerHost host)
        {
        }

        public void AddLinkedToolboxItem(ToolboxItem toolboxItem, IDesignerHost host)
        {
        }

        public void AddToolboxItem(ToolboxItem toolboxItem, string category)
        {
        }

        public void AddToolboxItem(ToolboxItem toolboxItem)
        {
        }

        public CategoryNameCollection CategoryNames
        {
            get { return null; }
        }

        public ToolboxItem DeserializeToolboxItem(object serializedObject, IDesignerHost host)
        {
            return (System.Drawing.Design.ToolboxItem)((DataObject)serializedObject).GetData(typeof(System.Drawing.Design.ToolboxItem));
        }

        public ToolboxItem DeserializeToolboxItem(object serializedObject)
        {
            return this.DeserializeToolboxItem(serializedObject, this.DesignerHost);
        }

        public ToolboxItem GetSelectedToolboxItem(IDesignerHost host)
        {
            ListBox list = this._toolList;
            System.Drawing.Design.ToolboxItem tbi = (System.Drawing.Design.ToolboxItem)list.Items[_selectedIndex];
            if (tbi.DisplayName != "<Pointer>")
                return tbi;
            else
                return null;
        }

        public ToolboxItem GetSelectedToolboxItem()
        {
            return this.GetSelectedToolboxItem(null);
        }

        public ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host)
        {
            return null;
        }

        public ToolboxItemCollection GetToolboxItems(string category)
        {
            return null;
        }

        public ToolboxItemCollection GetToolboxItems(IDesignerHost host)
        {
            return null;
        }

        public ToolboxItemCollection GetToolboxItems()
        {
            return null;
        }

        public bool IsSupported(object serializedObject, System.Collections.ICollection filterAttributes)
        {
            return false;
        }

        public bool IsSupported(object serializedObject, IDesignerHost host)
        {
            return false;
        }

        public bool IsToolboxItem(object serializedObject, IDesignerHost host)
        {
            return false;
        }

        public bool IsToolboxItem(object serializedObject)
        {
            return false;
        }

        public void RemoveCreator(string format, IDesignerHost host)
        {
            
        }

        public void RemoveCreator(string format)
        {
            
        }

        public void RemoveToolboxItem(ToolboxItem toolboxItem, string category)
        {
            
        }

        public void RemoveToolboxItem(ToolboxItem toolboxItem)
        {
            
        }

        public string SelectedCategory
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public void SelectedToolboxItemUsed()
        {
            ListBox list = this._toolList;

            list.Invalidate(list.GetItemRectangle(_selectedIndex));
            _selectedIndex = 0;
            list.SelectedIndex = 0;
            list.Invalidate(list.GetItemRectangle(_selectedIndex));
        }

        public object SerializeToolboxItem(ToolboxItem toolboxItem)
        {
            return new DataObject(toolboxItem);
        }

        public bool SetCursor()
        {
            return false;
        }

        public void SetSelectedToolboxItem(ToolboxItem toolboxItem)
        {

        }
    }
}
