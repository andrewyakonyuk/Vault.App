using CommonDomain.Persistence;
using Vault.Activity.Commands;
using Vault.Activity.Resources;
using Vault.Shared.Commands;

namespace Vault.Activity
{
    public class ArticleCommandHandler :
        ICommand<ReadActivityCommand<ArticleResource>>,
        ICommand<LikeActivityCommand<ArticleResource>>,
        ICommand<DislikeActivityCommand<ArticleResource>>
    {
        readonly IRepository _repository;
        readonly IResourceKeyMapper _keyMapper;
        readonly ModelUpdater<Article> _modelUpdater;

        public ArticleCommandHandler(
            IRepository repository,
            IResourceKeyMapper keyMapper)
        {
            _repository = repository;
            _keyMapper = keyMapper;
            _modelUpdater = new ModelUpdater<Article>(itemKey => new Article(itemKey), repository, keyMapper);
        }

        public void Execute(DislikeActivityCommand<ArticleResource> c)
        {
            _modelUpdater.Update(c.ItemKey, article => article.Dislike(c.Published));
        }

        public void Execute(LikeActivityCommand<ArticleResource> c)
        {
            _modelUpdater.Update(c.ItemKey, article => article.Like(c.Published));
        }

        public void Execute(ReadActivityCommand<ArticleResource> c)
        {
            _modelUpdater.Update(c.ItemKey, article => article.Read(c.Resource.Uri, c.Published));
        }
    }
}