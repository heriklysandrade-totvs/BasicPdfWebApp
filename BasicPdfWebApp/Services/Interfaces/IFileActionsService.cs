using BasicPdfWebApp.Models;
using System.Collections.Generic;

namespace BasicPdfWebApp.Services
{
    public interface IFileActionsService
    {
        void CombineFiles(List<FileItemEntity> fileItems);

        FileItemDTO CompressFile(FileItemEntity fileItem);

        void ProtectFile(FileItemEntity fileItem);

        void ConvertFileToPDF(FileItemEntity fileItem);
    }
}
