using System.Collections.Generic;
using System.Security.Principal;
using AElf.Sdk.CSharp;
using AElf.Sdk.CSharp.State;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Contracts.BuildersDAO
{
    public class BuildersDAO : BuildersDAOContainer.BuildersDAOBase
    {
        const string author = "Suchir";

        // Implement Initialize Smart Contract Logic
        public override Empty Initialize(Empty input)
        {
            Assert(!State.Initialized.Value, "already initialized");
            var initialProposal = new Proposal
            {
                Id = "0",
                Title = "Proposal #1",
                Description = "This is the first proposal of the DAO",
                Status = "IN PROGRESS",
                VoteThreshold = 1,
            };
            State.Proposals[initialProposal.Id] = initialProposal;
            State.NextProposalId.Value = 1;
            State.MemberCount.Value = 0;
    
            State.Initialized.Value = true;
    
            return new Empty();
        }

        // Implement Join DAO Logic
        public override Empty JoinDAO(Address input)
{
    // Based on the address, determine whether the address has joined the DAO. If it has, throw an exception
    Assert(!State.Members[input], "Member is already in the DAO");
    // If the address has not joined the DAO, then join and update the state's value to true
    State.Members[input] = true;
    // Read the value of MemberCount in the state, increment it by 1, and update it in the state
    var currentCount = State.MemberCount.Value;
    State.MemberCount.Value = currentCount + 1;
    return new Empty();
}

        // Implement Create Proposal Logic
        public override Proposal CreateProposal(CreateProposalInput input)
        {
            Assert(State.Members[input.Creator], "Only DAO members can create proposals");
            var proposalId = State.NextProposalId.Value.ToString();
            var newProposal = new Proposal
            {
                Id = proposalId,
                Title = input.Title,
                Description = input.Description,
                Status = "IN PROGRESS",
                VoteThreshold = input.VoteThreshold,
                YesVotes = { }, // Initialize as empty
                NoVotes = { }, // Initialize as empty
            };
            State.Proposals[proposalId] = newProposal;
            State.NextProposalId.Value += 1;
            return newProposal; // Ensure return
        }

        // Implement Vote on Proposal Logic
        public override Proposal VoteOnProposal(VoteInput input)
        {
            Assert(State.Members[input.Voter], "Only DAO members can vote");
            var proposal = State.Proposals[input.ProposalId]; // ?? new proposal
            Assert(proposal != null, "Proposal not found");
            Assert(
                !proposal.YesVotes.Contains(input.Voter) && !proposal.NoVotes.Contains(input.Voter),
                "Member already voted"
            );

            // Add the vote to the appropriate list
            if (input.Vote)
            {
                proposal.YesVotes.Add(input.Voter);
            }
            else
            {
                proposal.NoVotes.Add(input.Voter);
            }

            // Update the proposal in state
            State.Proposals[input.ProposalId] = proposal;

            // Check if the proposal has reached its vote threshold
            if (proposal.YesVotes.Count >= proposal.VoteThreshold)
            {
                proposal.Status = "PASSED";
            }
            else if (proposal.NoVotes.Count >= proposal.VoteThreshold)
            {
                proposal.Status = "DENIED";
            }

            return proposal;
        }

        // Implement Get All Proposals Logic
public override ProposalList GetAllProposals(Empty input)
{
    // Create a new list called ProposalList
    var proposals = new ProposalList();
    // Start iterating through Proposals from index 0 until the value of NextProposalId, read the corresponding proposal, add it to ProposalList, and finally return ProposalList
    for (var i = 0; i < State.NextProposalId.Value; i++)
    {
        var proposalCount = i.ToString();
        var proposal = State.Proposals[proposalCount];
        proposals.Proposals.Add(proposal);
    }
    return proposals;
}
        
        
        // Implement Get Proposal Logic
        public override Proposal GetProposal(StringValue input)
        {
            var proposal = State.Proposals[input.Value];
            return proposal;
        }

        // Implement Get Member Count Logic
        public override Int32Value GetMemberCount(Empty input)
        {
            var memberCount = new Int32Value
            {
                Value = State.MemberCount.Value
            };
            return memberCount;
        }

        // Implement Get Member Exist Logic
        public override BoolValue GetMemberExist(Address input)
        {
            var exist = new BoolValue
            {
                Value = State.Members[input]
            };
            return exist;
        }
    }
}

