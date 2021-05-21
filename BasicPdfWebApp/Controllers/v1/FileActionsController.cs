using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BasicPdfWebApp.Models;
using BasicPdfWebApp.Utils;
using System.Collections.Generic;
using BasicPdfWebApp.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;

namespace BasicPdfWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class FileActionsController : InternalBaseController<FileItemEntity, FileItemDTO>
    {
        private readonly FileContext _context;

        private readonly IFileActionsService _fileActionsService;

 

        public FileActionsController(FileContext context, IFileActionsService fileActionsService)
        {
            _context = context;
            _fileActionsService = fileActionsService;

            FileUtils.CheckDefaultPaths();
        }

        [HttpPost("combine")]
        public BaseResponseModel<FileItemDTO> CombineFiles(IFormFileCollection formFileCollection)
        {
            var fileItemEntities = new List<FileItemEntity>();

            foreach (var formFile in Request.Form.Files)
            {
                fileItemEntities.Add(new FileItemEntity()
                {
                    Id = new Random().Next(0, 200),
                    Guid = new Guid(),
                    Bytes = formFile.GetBytesFromFormFile(),
                    FileName = formFile.FileName
                });
            }

            var fileItemDTO = _fileActionsService.CombineFiles(fileItemEntities);

            return new BaseResponseModel<FileItemDTO>() { message = "sucesso", executionDate = DateTime.Now, data = fileItemDTO };
        }

        [HttpPost("compress")]
        public BaseResponseModel<FileItemDTO> CompressFile([FromForm] IFormFile inputFormFile)
        {
            var formFile = Request.Form.Files[0];

            var fileItemEntity = new FileItemEntity()
            {
                Id = new Random().Next(0, 200),
                Guid = new Guid(),
                Bytes = formFile.GetBytesFromFormFile(),
                FileName = formFile.FileName
            };

            var fileItemDTO = _fileActionsService.CompressFile(fileItemEntity);

            return new BaseResponseModel<FileItemDTO>() { message = "sucesso", executionDate = DateTime.Now, data = fileItemDTO };
        }

        [HttpPost("protect")]
        public BaseResponseModel<FileItemDTO> ProtectFile([FromForm] IFormFile inputFormFile)
        {
            var formFile = Request.Form.Files[0];

            var fileItemEntity = new FileItemEntity()
            {
                Id = new Random().Next(0, 200),
                Guid = new Guid(),
                Bytes = formFile.GetBytesFromFormFile(),
                FileName = formFile.FileName
            };

            var fileItemDTO = _fileActionsService.ProtectFile(fileItemEntity);

            return new BaseResponseModel<FileItemDTO>() { message = "sucesso", executionDate = DateTime.Now, data = fileItemDTO };
        }

        [HttpPost("convert/topdf")]
        public BaseResponseModel<FileItemDTO> ConvertFileToPDF([FromForm] IFormFile inputFormFile)
        {
            var formFile = Request.Form.Files[0];

            var fileItemEntity = new FileItemEntity()
            {
                Id = new Random().Next(0, 200),
                Guid = new Guid(),
                Bytes = formFile.GetBytesFromFormFile(),
                FileName = formFile.FileName
            };

            var fileItemDTO = _fileActionsService.ConvertFileToPDF(fileItemEntity);

            return new BaseResponseModel<FileItemDTO>() { message = "sucesso", executionDate = DateTime.Now, data = fileItemDTO };
        }
    }
}
