using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.MainForm
{
    internal interface IMainForm
    {
        public EventHandler OnLoaded {  get; set; }
    }
}
