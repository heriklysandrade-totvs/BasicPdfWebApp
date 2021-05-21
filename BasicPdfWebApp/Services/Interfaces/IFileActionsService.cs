using BasicPdfWebApp.Models;
using System.Collections.Generic;

namespace BasicPdfWebApp.Services
{
    public interface IFileActionsService
    {
        FileItemDTO CombineFiles(List<FileItemEntity> fileItems);

        FileItemDTO CompressFile(FileItemEntity fileItem);

        FileItemDTO ProtectFile(FileItemEntity fileItem);

        FileItemDTO ConvertFileToPDF(FileItemEntity fileItem);
    }
}
