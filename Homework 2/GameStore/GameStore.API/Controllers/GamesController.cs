using GameStore.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly GameStoreDbContext _context;
        public GamesController(GameStoreDbContext context)
        {
            _context = context;
        }



    }
}
