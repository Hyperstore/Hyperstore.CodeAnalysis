using Hyperstore.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Run().Wait();
        }

        private async Task Run()
        {
            var domain = await StoreBuilder.CreateDomain<MyModelDefinition>("x");
            using (var session = domain.Store.BeginSession())
            {
                var lib = new Library(domain);
                session.AcceptChanges();
            }
        }
    }
}
