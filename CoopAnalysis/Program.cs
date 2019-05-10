using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoopAnalysis
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Engine e = new Engine();
            e.Run(args);
        }

    
    }
}
