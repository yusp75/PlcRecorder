using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Forms;

namespace MyPlc2
{
    public class MTreeNode
    {
        public MTreeNode(string? name, string? address, bool check)
        {
            Name = name;
            Address = address;
            Checked = check;
        }

        public string? Name { get; set; }
        public string? Address { get; set; }
        public bool Checked { get; set; }

    }

    public partial class MTreeView : TreeView
    {
        private readonly string path = System.AppDomain.CurrentDomain.BaseDirectory;
        private Dictionary<string, MTreeNode> data = new();
        private List<string> Addresses = new();
        //已激活地址
        public List<string> AddressesList { get; set; } = new();

        //委托：点击
        public delegate void ClickDelegate(List<string> addresses);
        public ClickDelegate clickDelegate;

        public delegate void DblClickDelegate(string name, string address);
        public DblClickDelegate dblClickDelegate;

        public MTreeView()
        {
            InitializeComponent();

            CheckBoxes = true;

            ReadVars();

            //拖放
            this.ItemDrag += MTreeView_ItemDrag;
        }

        //读plc变量
        public void ReadVars()
        {
            string file = path + "config\\vars.json";
            using StreamReader sr = new StreamReader(file);
            string s = sr.ReadToEnd();
            try
            {
                BeginUpdate();
                Nodes.Clear();

                List<PlcVar> vars = JsonConvert.DeserializeObject<List<PlcVar>>(s);
                foreach (PlcVar var in vars)
                {   //是否激活
                    if (var.active)
                    {
                        try
                        {
                            data.Add(var.name, new MTreeNode(var.name, var.address, false));
                            Nodes.Add(new TreeNode(var.name));

                            AddressesList.Add(var.address);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                EndUpdate();
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
        //项目拖曳
        private void MTreeView_ItemDrag(object? sender, ItemDragEventArgs e)
        {
            string name = ((TreeNode)e.Item).Text;
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(data[name].Address, DragDropEffects.Copy);
                //Debug.WriteLine("拖曳：" + data[name].Address);
            }
        }

        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            dblClickDelegate(e.Node.Text, data[e.Node.Text].Address);
        }
        //事件：勾选
        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck(e);
            if (e.Node != null)
            {
                string address = data[e.Node.Text].Address;
                bool check = e.Node.Checked;
                data[e.Node.Text].Checked = check;

                //在Addresses中添加、删除
                if (!check)
                {
                    Addresses.Remove(address);
                }
                else
                {
                    Addresses.Add(address);
                }

                //代理更新
                clickDelegate(Addresses);
            }


        }


        //
    }
}
