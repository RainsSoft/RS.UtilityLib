using System;
using System.Collections.Generic;
using System.Text;
using RS.UtilityLib.SizeOfGenericT;
namespace RS.UtilityLib.SizeOfGenericT
{
    class Class1
    {
        void test() {
            MemoryBlock mb = new MemoryBlock(1024);
            Axiom.Core.MemoryBuffer<int> a = Axiom.Core.MemoryManager.Instance.Allocate<int>(100);
            Axiom.Core.MemoryManager.Instance.Initialize(a);
        }

    }
}
