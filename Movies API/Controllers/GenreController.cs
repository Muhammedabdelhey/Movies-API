using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies_Core_Layer.Dtos;
using Movies_Core_Layer.Interfaces;
using Movies_Core_Layer.Models;
using Movies_With_Reopsitory_Pattren.Filters;

namespace Movies_With_Reopsitory_Pattren.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // to add action filter for controller
    [LogSensitiveActionAttriburte]
    // make controller need to Authenticated
    //[Authorize]
    public class GenreController : ControllerBase
    {
        private readonly IBaseRepository<Genre> _genreRepository;
        #region declare delegate and event
        public delegate void GenraChanged(Genre genre);
        public event GenraChanged OnGenraChanged;
        #endregion
        public GenreController(IBaseRepository<Genre> genreRepository)
        {
            _genreRepository = genreRepository;
        }
        [HttpGet]
        // make Action need to Authenticated
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _genreRepository.GetAllAsync());
        }

        [HttpGet]
        [Route("{id}")]
        // to add action filter for Method
        [LogSensitiveActionAttriburte]
        public async Task<IActionResult> GetBy(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
                return NotFound($"No Genre Found With ID: {id}");
            return Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> Add(GenreDto genreDto)
        {
            var genre = new Genre() { Name = genreDto.Name };
            await _genreRepository.AddAsync(genre);
            return Ok(genre);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(int id, GenreDto genreDto)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
                return NotFound($"No Genre Found With ID: {id}");

            genre.Name = genreDto.Name;
            await _genreRepository.UpdateAsync(genre);
            // call event , check handler
            OnGenraChanged?.Invoke(genre);
            return Ok(genre);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
                return NotFound($"No Genre Found With ID: {id}");

            await _genreRepository.DeleteAsync(genre);
            return Ok(genre);
        }
        //#region regster event testing only
        //var genre = new GenreController();
        //genre.OnGenraChanged += Genre_OnGenraChanged;
        //private void Genre_OnGenraChanged(Genre genre)
        //{
        //    throw new NotImplementedException();
        //}
        //#endregion
    }
}
