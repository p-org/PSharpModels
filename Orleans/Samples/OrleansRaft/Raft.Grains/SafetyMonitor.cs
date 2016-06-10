using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Orleans;
using Orleans.Concurrency;

using Raft.Interfaces;

namespace Raft.Grains
{
    /// <summary>
    /// Grain implementation class SafetyMonitor.
    /// </summary>
    [Reentrant]
    public class SafetyMonitor : Grain<int>, ISafetyMonitor
    {
        #region fields

        private HashSet<int> TermsWithLeader;

        #endregion

        #region methods

        public override async Task OnActivateAsync()
        {
            if (this.TermsWithLeader == null)
            {
                this.TermsWithLeader = new HashSet<int>();
            }

            await base.OnActivateAsync();
        }

        public Task NotifyLeaderElected(int term)
        {
            Console.WriteLine($"<RaftLog> SafetyMonitor found a leader in term {term}.");

            if (this.TermsWithLeader.Contains(term))
            {
                throw new InvalidOperationException($"Detected more than one leader in term {term}.");
            }

            this.TermsWithLeader.Add(term);

            return TaskDone.Done;
        }

        #endregion
    }
}
