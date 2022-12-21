using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleSTest.Services {
    public class TestService : ITestService {

        public void Update()
        {
            System.Diagnostics.Debug.Print("doubt you see this");
        }
    }
}
