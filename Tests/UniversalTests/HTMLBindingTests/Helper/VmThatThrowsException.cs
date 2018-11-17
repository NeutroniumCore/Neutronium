using System;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class VmThatThrowsException
    {
        internal VmThatThrowsException()
        {
            Int = 5;
        }
        public int Int { get; set; }
        public int Explosive => throw new Exception();
    }
}
