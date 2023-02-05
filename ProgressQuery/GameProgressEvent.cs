using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressQuery
{
    public delegate void OnGameProgressHandler(OnGameProgressEventArgs e);
    public class OnGameProgressEventArgs:EventArgs
    {
        public string Name { get; set; }

        public bool code { get; set; }
    }
}
