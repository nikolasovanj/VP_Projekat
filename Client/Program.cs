using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<ILibrary> factory = new ChannelFactory<ILibrary>("LibraryService");

            ILibrary proxy = factory.CreateChannel();
        }
    }
}
