using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet
{
    public static class Messagebox
    {
        public static void show(string content)
        {
            frmAlert frm = new frmAlert();
            frm.content = content;
            frm.ShowDialog();
        }
    }
}
