using System;
using System.Data;
using Vault.Shared.Domain;

namespace Vault.Shared.Search
{
    public class DefaultReportUnitOfWorkFactory : IReportUnitOfWorkFactory
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DefaultReportUnitOfWorkFactory(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public IUnitOfWork Create()
        {
            return _unitOfWorkFactory.Create();
        }

        public IUnitOfWork Create(IsolationLevel isolationLevel)
        {
            return _unitOfWorkFactory.Create(isolationLevel);
        }
    }
}