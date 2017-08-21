using EPMTools.RBSResources.EPMRepository;
using EPMTools.RBSResources.EPMRepository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPMTools.RBSResources.WindowsApp
{
    public partial class RBSTree : Form
    {
        public static List<User> Users = new List<User>();
        public static List<Node> nodes = new List<Node>();
        public Utilities pwaUtilities = null;
        public bool isConnected = false;

        public RBSTree()
        {
            InitializeComponent();
        }

        private void AddNodes(IList<Node> nodes, TreeNodeCollection collection)
        {
            foreach (var node in nodes)
            {
                TreeNode collnode = new TreeNode();
                collnode.Tag = node.ID;
                collnode.Text = node.Text;
                collnode.Name = node.FullText;

                if (node.SubNodes.Count > 0)
                    AddNodes(node.SubNodes, collnode.Nodes);

                collection.Add(collnode);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = nodes.Where(n => n.ID == (Guid)e.Node.Tag).SingleOrDefault();
            if (node != null)
            {
                dataGridView1.DataSource = Users.Where(u => u.RBSInternalName == node.Internalname).ToList();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            Loading loadingWindow = new Loading();
            loadingWindow.Show(this);

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, args) =>
            {
                this.Invoke(new MethodInvoker(() => this.Enabled = false));
                try
                {
                    RBSTree.Users = pwaUtilities.GetResources();
                    RBSTree.nodes = pwaUtilities.GetNodes();

                    foreach (var item in RBSTree.nodes)
                    {
                        item.SubNodes = RBSTree.nodes.Where(n => n.ParentID == item.ID).ToList();
                        item.Users = RBSTree.Users.Where(u => u.RBSInternalName == item.Internalname).ToList();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            bw.RunWorkerCompleted += (s, args) =>
            {
                loadingWindow.Close();
                AddNodes(nodes.Where(n => n.ParentID == Guid.Empty).ToList(), treeView1.Nodes);
                this.Invoke(new MethodInvoker(() => { this.Enabled = true; }));

            };
            bw.Disposed += (s, args) =>
            {
                

            };

            bw.RunWorkerAsync();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var user = ((sender as DataGridView).DataSource as List<EPMTools.RBSResources.EPMRepository.Entities.User>).ElementAtOrDefault(e.RowIndex);
            if (user != null)
            {
                Loading loadingWindow = new Loading();
                loadingWindow.Show(this);

                UserMembership userMembershipWindow = new UserMembership();
                
                var groups = pwaUtilities.GetUserSharepointGroupGroups(user.ID);
                userMembershipWindow.dgvUserGroups.DataSource = groups;

                var PSGroups = pwaUtilities.GetProjectServerGroups(user.ID);
                userMembershipWindow.dgvPSGroups.DataSource = PSGroups;

                userMembershipWindow.ShowDialog();

                loadingWindow.Close();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Loading loadingWindow = new Loading();
            loadingWindow.Show(this);

            if (!isConnected)
            {
                pwaUtilities = new Utilities(txtPwaUrl.Text.Trim(), txtUsername.Text.Trim(), txtPassword.Text.Trim(), txtDomain.Text.Trim());
                pwaUtilities.Connect();
                isConnected = true;

                txtDomain.Enabled = txtPassword.Enabled = txtPwaUrl.Enabled = txtUsername.Enabled = false;
            }
            else
            {
                pwaUtilities = null;
                isConnected = false;

                txtDomain.Enabled = txtPassword.Enabled = txtPwaUrl.Enabled = txtUsername.Enabled = true;
            }

            loadingWindow.Close();
        }
    }
}
