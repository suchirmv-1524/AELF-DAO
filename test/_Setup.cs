using AElf.Cryptography.ECDSA;
using AElf.Testing.TestBase;
using AElf.Contracts.BuildersDAO; // Ensure this namespace matches your project structure

namespace AElf.Contracts.BuildersDAO
{
    public class Module : ContractTestModule<BuildersDAOTests>
    {
        // Implementation specific to your test module setup
    } 
  
    public class TestBase : ContractTestBase<Module>
    {
        internal readonly BuildersDAOContainer.BuildersDAOStub BuildersDAOStub;

        private ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;

        public TestBase()
        {
            BuildersDAOStub = GetBuildersDAOContractStub(DefaultKeyPair);
        }

        private BuildersDAOContainer.BuildersDAOStub GetBuildersDAOContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<BuildersDAOContainer.BuildersDAOStub>(ContractAddress, senderKeyPair);
        }
    }
}
