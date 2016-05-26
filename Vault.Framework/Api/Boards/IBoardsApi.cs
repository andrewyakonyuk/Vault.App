using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vault.Framework.Api.Boards
{
    public interface IBoardsApi
    {
        Task<Board> CreateBoardAsync(string name, string query);

        Task<Board> GetBoardAsync(int boardId, int offset, int count);

        Task<Board> GetBoardByQueryAsync(string query, int offset, int count);

        Task<IEnumerable<Board>> GetBoardsAsync();

        Task<bool> UpdateBoardAsync(int boardId, string name, string query);

        Task<bool> DeleteBoardAsync(int boardId);
    }
}