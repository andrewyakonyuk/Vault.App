using System;

namespace Vault.Shared.Queries
{ 
    public class FindById : ICriterion
    {
        public FindById(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }
}