namespace Vault.WebHost.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Boards;
    using Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Services.Boards;

    [Authorize]
    public class BoardsController : Controller
    {
        readonly IWorkContextAccessor _workContextAccessor;
        readonly IBoardsApi _boardsApi;

        public BoardsController(
            IWorkContextAccessor workContextAccessor,
            IBoardsApi boardsApi)
        {
            _workContextAccessor = workContextAccessor;
            _boardsApi = boardsApi;
        }

        public WorkContext WorkContext { get { return _workContextAccessor.WorkContext; } }

        [HttpGet]
        public async Task<IActionResult> Index(int offset = 0, int count = 10)
        {
            var board = await _boardsApi.GetBoardByQueryAsync(null, offset, count);
            return View(board);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoard(CreateBoard model)
        {
            if (ModelState.IsValid)
            {
                var board = await _boardsApi.CreateBoardAsync(model.Name, model.Query);
                if (board != null)
                {
                    return RedirectToAction(nameof(Detail), new { boardId = board.Id, title = board.Name });
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int boardId, int offset = 0, int count = 20)
        {
            var board = await _boardsApi.GetBoardAsync(boardId, offset, count);
            if (board == null)
                return new NotFoundResult();

            if (Request.IsPjaxRequest())
                return PartialView("Index", board);

            return View("Index", board);
        }

        public async Task<IActionResult> Search(string q, int offset = 0, int count = 20)
        {
            if (string.Equals(HttpContext.Request.Method, "post", System.StringComparison.OrdinalIgnoreCase))
                return RedirectToRoute("board-search", new { q });

            var board = await _boardsApi.GetBoardByQueryAsync(q, offset, count);

            if (Request.IsPjaxRequest())
                return PartialView("Index", board);

            return View("Index", board);
        }

        [HttpPost]
        public async Task<IActionResult> Update([Required]int boardId, [Required] string boardName, string q)
        {
            await _boardsApi.UpdateBoardAsync(boardId, boardName, q);

            var board = await _boardsApi.GetBoardAsync(boardId, 0, 20);
            if (board == null)
                return new NotFoundResult();

            return RedirectToAction(nameof(Detail), new { boardId = board.Id, title = board.Name });
        }

        public async Task<IActionResult> Delete([Required] int boardId)
        {
            await _boardsApi.DeleteBoardAsync(boardId);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Suggestions(string term)
        {
            var board = await _boardsApi.GetBoardByQueryAsync(term, 0, 10);

            var suggestions = board.Cards.Select(t => new SuggestionModel
            {
                Card = t,
                Type = t.GetType().Name,
                Value = t.Name
            }).ToList();
            return Json(suggestions, new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                ContractResolver = new LowerCaseContractResolver()
            });
        }
    }

    public class LowerCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            var resolvedPropertyName = base.ResolvePropertyName(propertyName);
            if (string.IsNullOrEmpty(resolvedPropertyName))
                return resolvedPropertyName;
            return char.ToLowerInvariant(resolvedPropertyName[0]) + resolvedPropertyName.Substring(1);
        }
    }
}