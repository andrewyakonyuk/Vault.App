using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Vault.Framework.Api.Boards;
using Vault.Web.Models.Navigation;

namespace Vault.Web.Components
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
                    Url = Url.Board(item.Id, item.Name ?? string.Empty, HttpContext.User.Identity.Name),
                    Text = item.Name ?? item.Id + ""
                });
            }

            return model;
        }
    }
}