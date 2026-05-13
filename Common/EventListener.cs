using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class EventListener
    {
        public void HandleEvent(object sender, EventArgs e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
