namespace Raft.Grains
{
    /// <summary>
    /// Server role.
    /// </summary>
    public enum Role
    {
        Follower = 0,
        Candidate,
        Leader
    }
}
