using System;

namespace Vault.Shared.Queries
{
    public class Paging : ICriterion
    {
        public Paging(int offset, int count)
        {
            Offset = offset;
            Count = count;
        }

        public Paging()
        {
            Count = 100;
        }

        public int Offset { get; set; }

        public int Count { get; set; }
    }
}