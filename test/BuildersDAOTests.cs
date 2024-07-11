using System;
using System.Threading.Tasks;
using AElf.Contracts.BuildersDAO;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
//using src.Protobuf.reference.acs12.proto;
using Shouldly;
using Xunit;

namespace AElf.Contracts.BuildersDAO.Tests
{
    public class BuildersDAOTests : TestBase
    {
        [Fact]
        public async Task InitializeTest_Success()
        {
            await BuildersDAOStub.Initialize.SendAsync(new Empty());
            var proposal = await BuildersDAOStub.GetProposal.CallAsync(new StringValue { Value = "0" });
            proposal.Title.ShouldBe("Proposal #1");
        }

        [Fact]
        public async Task InitializeTest_Duplicate()
        {
            await BuildersDAOStub.Initialize.SendAsync(new Empty());
            var executionResult = await BuildersDAOStub.Initialize.SendWithExceptionAsync(new Empty());
            executionResult.TransactionResult.Error.ShouldContain("Already initialized.");
        }

        [Fact]
        public async Task JoinDAOTest_Success()
        {
            await BuildersDAOStub.Initialize.SendAsync(new Empty());
            await BuildersDAOStub.JoinDAO.SendAsync(Accounts[1].Address);
            await BuildersDAOStub.JoinDAO.SendAsync(Accounts[2].Address);
            var exist1 = await BuildersDAOStub.GetMemberExist.CallAsync(Accounts[1].Address);
            var exist2 = await BuildersDAOStub.GetMemberExist.CallAsync(Accounts[2].Address);
            exist1.Value.ShouldBe(true);
            exist2.Value.ShouldBe(true);
        }

        [Fact]
        public async Task JoinDAOTest_Duplicate()
        {
            await BuildersDAOStub.Initialize.SendAsync(new Empty());
            await BuildersDAOStub.JoinDAO.SendAsync(Accounts[1].Address);
            var executionResult = await BuildersDAOStub.JoinDAO.SendWithExceptionAsync(Accounts[1].Address);
            executionResult.TransactionResult.Error.ShouldContain("Member is already in the DAO");
        }

        [Fact]
        public async Task CreateProposalTest_Success()
        {
            await JoinDAOTest_Success();
            var proposal = await CreateMockProposal(Accounts[1].Address);
            proposal.Title.ShouldBe("mock_proposal");
            proposal.Id.ShouldBe("1");
        }

        private async Task<Proposal> CreateMockProposal(Address creator)
        {
            var createProposalInput = new CreateProposalInput
            {
                Creator = creator,
                Description = "mock_proposal_desc",
                Title = "mock_proposal",
                VoteThreshold = 1
            };
            var proposal = await BuildersDAOStub.CreateProposal.SendAsync(createProposalInput);
            return proposal.Output;
        }

        [Fact]
        public async Task CreateProposalTest_NoPermission()
        {
            await JoinDAOTest_Success();
            try
            {
                await CreateMockProposal(Accounts[2].Address);
            }
            catch (Exception e)
            {
                e.Message.ShouldContain("Only DAO members can create proposals");
            }
        }

        [Fact]
        public async Task VoteOnProposalTest_Success()
        {
            await JoinDAOTest_Success();
            var proposal = await CreateMockProposal(Accounts[1].Address);
            var voteInput1 = new VoteInput
            {
                ProposalId = proposal.Id,
                Vote = true,
                Voter = Accounts[1].Address
            };
            var voteInput2 = new VoteInput
            {
                ProposalId = proposal.Id,
                Vote = false,
                Voter = Accounts[2].Address
            };
            await BuildersDAOStub.VoteOnProposal.SendAsync(voteInput1);
            await BuildersDAOStub.VoteOnProposal.SendAsync(voteInput2);
            var proposalResult = await BuildersDAOStub.GetProposal.CallAsync(new StringValue { Value = proposal.Id });
            proposalResult.YesVotes.ShouldContain(Accounts[1].Address);
            proposalResult.NoVotes.ShouldContain(Accounts[2].Address);
        }

        [Fact]
        public async Task VoteOnProposalTest_NoPermission()
        {
            await JoinDAOTest_Success();
            var proposal = await CreateMockProposal(Accounts[1].Address);
            var voteInput1 = new VoteInput
            {
                ProposalId = proposal.Id,
                Vote = true,
                Voter = Accounts[3].Address
            };
            var executionResult = await BuildersDAOStub.VoteOnProposal.SendWithExceptionAsync(voteInput1);
            executionResult.TransactionResult.Error.ShouldContain("Only DAO members can vote");
        }

        [Fact]
        public async Task VoteOnProposalTest_NoProposal()
        {
            await JoinDAOTest_Success();
            var proposal = await CreateMockProposal(Accounts[1].Address);
            var voteInput1 = new VoteInput
            {
                ProposalId = "123",
                Vote = true,
                Voter = Accounts[1].Address
            };
            var executionResult = await BuildersDAOStub.VoteOnProposal.SendWithExceptionAsync(voteInput1);
            executionResult.TransactionResult.Error.ShouldContain("Proposal not found");
        }
    }
}