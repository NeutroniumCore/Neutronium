using System.Collections.Generic;

namespace Neutronium.Core.Test.Helper 
{
    public class TestClass
        {
            public List<TestClass> Children { get; set; } = new List<TestClass>();
            public TestClass Property1 { get; set; }
            public TestClass Property2 { get; set; }
            public TestClass Property3 { get; set; }
        }

}
