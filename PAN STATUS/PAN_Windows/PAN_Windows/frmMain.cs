using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PAN_Windows
{
    public partial class frmMain : Form
    {
        string user_name = "";
        int UserId = 0;
        int Usergroupid = 0;
        string Password = "";
        public frmMain()
        {
            InitializeComponent();
        }
        public frmMain(int compid, string user_code)
        {
            InitializeComponent();
            UserId = compid;
            user_name = user_code;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                Main();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Main()
        {
            try
            {
                GetPassword frmprintinv = new GetPassword();
                //frmprintinv.MdiParent = this;
                foreach (Form f in Application.OpenForms)
                {
                    if (f is GetPassword)
                    {
                        f.Focus();
                        return;
                    }
                }
                frmprintinv.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void fc_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Refresh();
        }     

        private void mnuExit_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure to exit ?", "PAN STATUS", MessageBoxButtons.YesNo))
            {
                Application.Exit();
            }
        }

        private void panInquiry_Click(object sender, EventArgs e)
        {
            try
            {
                frmGetPanInquiry frmprintinv = new frmGetPanInquiry();
                //frmprintinv.MdiParent = this;
                foreach (Form f in Application.OpenForms)
                {
                    if (f is frmGetPanInquiry)
                    {
                        f.Focus();
                        return;
                    }
                }
                frmprintinv.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panDetailFetchAll_Click(object sender, EventArgs e)
        {
            try
            {
                frmGetPanDetailsAll frmprintinv = new frmGetPanDetailsAll();
                //frmprintinv.MdiParent = this;
                foreach (Form f in Application.OpenForms)
                {
                    if (f is frmGetPanDetailsAll)
                    {
                        f.Focus();
                        return;
                    }
                }
                frmprintinv.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
    }
}
