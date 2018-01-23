using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vault.WebHost.Models.Navigation;
using Vault.WebHost.Services.Boards;

namespace Vault.WebHost.Components
{
    [ViewComponent(Name = "Navigation")]
    public class NavigationComponent : ViewComponent
    {
        readonly IBoardsApi _boardsApi;

        public NavigationComponent(IBoardsApi boardsApi)
        {
            _boardsApi = boardsApi;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await CreateModel();

            return View(model);
        }

        async Task<NavigationModel> CreateModel()
        {
            var model = new NavigationModel();

            foreach (var item in await _boardsApi.GetBoardsAsync())
            {
                model.Boards.Add(new NavigationItem
                {
                    Url = Url.RouteUrl("board-detail", new { boardId = item.Id, title = item.Name ?? string.Empty }),
                    Text = item.Name ?? item.Id + ""
                });
            }

            return model;
        }
    }
}