using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketStressTestTools
{
    public partial class F_Main : Form
    {
		private SynchronizationContext sync = null;

        public F_Main()
        {
            InitializeComponent();
        }

        private void F_Main_Load( object sender, EventArgs e )
        {
			this.sync = SynchronizationContext.Current;
        }

        private void bt_Start_Click( object sender, EventArgs e )
        {
			SocketStressTest.SocketClientManager cm = new SocketStressTest.SocketClientManager();
			cm.Init("192.168.3.1", 80, 1000, 10, 10);
			cm.ConnectedCountChange += ( a, b ) =>
			{
				this.sync.Post(f=>
				{
					this.textBox1.Text = b.ToString();
				}, b );
			};

			try
			{
				cm.StartAsync().ContinueWith(f=>
				{
					this.sync.Post(k=>
					{
						MessageBox.Show((( TaskStatus)k).ToString());
					},f.Status );
				} );
			}
			catch ( Exception ex )
			{
				MessageBox.Show(ex.ToString());
			}
        }

        private void bt_End_Click( object sender, EventArgs e )
        {

        }

        private void tb_Port_KeyPress( object sender, KeyPressEventArgs e )
        {

        }
    }
}
