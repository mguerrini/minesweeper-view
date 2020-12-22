using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.Shared;

namespace Minesweeper.Controls
{
    public class CellClickEventArgs : EventArgs
    {
        public CellData Cell { get; internal set; }

        public bool IsRevealAction { get; set; }

        public bool IsMarkAction { get; set; }
    }
}
