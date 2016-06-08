namespace Raft.Grains
{
    public class Log
    {
        public readonly int Term;
        public readonly int Command;

        public Log() { }

        public Log(int term, int command)
        {
            this.Term = term;
            this.Command = command;
        }
    }
}