﻿using Vault.Shared.Domain;

namespace Vault.Shared.Search
{
    public interface IIndexUnitOfWorkFactory
    {
        IIndexUnitOfWork CreateUnitOfWork();
    }
}