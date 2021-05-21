using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicPdfWebApp.Models;
using Microsoft.AspNetCore.Http;
using System;
using BasicPdfWebApp.Utils;

namespace BasicPdfWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : InternalBaseController<FileItemEntity, FileItemDTO>
    {
        private readonly FileContext _context;

        public FileController(FileContext context)
        {
            _context = context;
        }

        // GET: api/File
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileItemDTO>>> GetFileItems()
        {
            return await _context.FileItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        // GET: api/File/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FileItemDTO>> GetFileItem(int id)
        {
            var fileItem = await _context.FileItems.FindAsync(id);

            if (fileItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(fileItem);
        }

        // PUT: api/File/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFileItem(int id, FileItemDTO fileItemDTO)
        {
            if (id != fileItemDTO.Id)
            {
                return BadRequest();
            }

            var fileItem = await _context.FileItems.FindAsync(id);
            if (fileItem == null)
            {
                return NotFound();
            }

            fileItem.Id = fileItemDTO.Id;
            fileItem.FileFullPath = fileItemDTO.FileFullPath;

            _context.Entry(fileItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!FileItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/File
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FileItemDTO>> PostFileItem(FileItemDTO fileItemDTO)
        {
            var fileItem = new FileItemEntity();
            iMapper.Map(fileItemDTO, fileItem);

            _context.FileItems.Add(fileItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetFileItem),
                new { id = fileItem.Id },
                ItemToDTO(fileItem));
        }

        [HttpPost("new")]
        public FileItemDTO PostFileItem([FromForm] IFormFile inputFormFile)
        {
            var formFile = Request.Form.Files[0];

            var fileItemEntity = new FileItemEntity()
            {
                Id = new Random().Next(0, 200),
                Guid = new Guid(),
                Bytes = formFile.GetBytesFromFormFile(),
                FileName = formFile.FileName
            };

            var fileItemDTO = new FileItemDTO();
            iMapper. Map(fileItemEntity, fileItemDTO);

            return fileItemDTO;
        }

        // DELETE: api/File/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFileItem(int id)
        {
            var fileItem = await _context.FileItems.FindAsync(id);
            if (fileItem == null)
            {
                return NotFound();
            }

            _context.FileItems.Remove(fileItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FileItemExists(int id)
        {
            return _context.FileItems.Any(e => e.Id == id);
        }

        private static FileItemDTO ItemToDTO(FileItemEntity fileItem) =>
            new FileItemDTO
            {
                Id = fileItem.Id,
                FileFullPath = fileItem.FileFullPath
            };
    }
}
