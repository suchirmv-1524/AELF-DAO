using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AElf.Sdk.CSharp.State;
using AElf.Types;

namespace AElf.Contracts.BuildersDAO
{
    // The state class is access the blockchain state
    public class BuildersDAOState : ContractState
    {
        public BoolState Initialized { get; set; }
        public MappedState<Address, bool> Members { get; set; }
        public MappedState<string, Proposal> Proposals { get; set; }
        public Int32State MemberCount { get; set; }
        public Int32State NextProposalId { get; set; }
    }
}