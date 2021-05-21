using BasicPdfWebApp.Models;
using BasicPdfWebApp.Utils;
using pdftron.Common;
using pdftron.PDF;
using pdftron.SDF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BasicPdfWebApp.Services
{
    public class FileActionsService : IFileActionsService
    {
        #region Auxiliares

        private void SaveFile(PDFDoc doc, string fileName, FileNameOptionEnum fileNameOption)
        {
            doc.Save(
                FileUtils.GetNewFileName(
                    FileUtils.GetFilePath(string.Empty) + fileName,
                    fileNameOption),
                SDFDoc.SaveOptions.e_linearized);
        }

        #endregion

        public void CombineFiles(List<FileItemEntity> fileItems)
        {
            try
            {
                using (PDFDoc new_doc = new PDFDoc())
                {
                    new_doc.InitSecurityHandler();

                    var fileNewName = new StringBuilder();

                    foreach (var item in fileItems)
                    {
                        if (fileNewName.Length > 0)
                            fileNewName.Append("_");

                        fileNewName.Append(item.FileName.TrimEnd(".pdf".ToCharArray()));

                        using (PDFDoc in_doc = new PDFDoc(item.FileFullPath))
                        {
                            in_doc.InitSecurityHandler();

                            new_doc.InsertPages(new_doc.GetPageCount() + 1, in_doc, 1, in_doc.GetPageCount(), PDFDoc.InsertFlag.e_none);
                        }
                    }

                    #region Teste
                    //Era pra funcionar, mas lança uma exceção no método "new_doc.ImportPages"

                    //ArrayList copy_pages = new ArrayList();

                    //for (PageIterator itr = in_doc.GetPageIterator(); itr.HasNext(); itr.Next())
                    //{
                    //    copy_pages.Add(itr.Current());
                    //}

                    //var imported_pages = new_doc.ImportPages(copy_pages, false);

                    //for (int i = 0; i != imported_pages.Count; ++i)
                    //{
                    //    new_doc.PagePushBack((Page)imported_pages[i]);
                    //}
                    #endregion

                    SaveFile(new_doc, fileNewName.ToString(), FileNameOptionEnum.Convert);
                }
            }
            catch (PDFNetException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public FileItemDTO CompressFile(FileItemEntity fileItem)
        {
            fileItem.FileFullPath = FileUtils.GetDefaultInputPath() + fileItem.FileName;

            File.WriteAllBytes(fileItem.FileFullPath, fileItem.Bytes);

            var fileItemDTO = new FileItemDTO()
            {
                Id = fileItem.Id,
                FileFullPath = FileUtils.GetNewFileName(fileItem.FileName, FileNameOptionEnum.Compress)
            };
            fileItemDTO.FileName = FileUtils.GetSafeFileName(fileItemDTO.FileFullPath);

            using (PDFDoc doc = new PDFDoc(fileItem.FileFullPath))
            {
                Optimizer.Optimize(doc);

                using (PDFDoc newDoc = new PDFDoc())
                {
                    newDoc.InsertPages(newDoc.GetPageCount() + 1, doc, 1, doc.GetPageCount(), PDFDoc.InsertFlag.e_none);

                    SaveFile(newDoc, fileItemDTO.FileFullPath, FileNameOptionEnum.Compress);
                }
            }

            return fileItemDTO;
        }

        public void ProtectFile(FileItemEntity fileItem)
        {
            using (PDFDoc doc = new PDFDoc(fileItem.FileFullPath))
            {
                SecurityHandler new_handler = new SecurityHandler(SecurityHandler.AlgorithmType.e_AES_256);

                string my_password = "senha_padrao";
                new_handler.ChangeUserPassword(my_password);

                new_handler.SetPermission(SecurityHandler.Permission.e_print, true);
                new_handler.SetPermission(SecurityHandler.Permission.e_extract_content, false);

                doc.SetSecurityHandler(new_handler);

                SaveFile(doc, fileItem.FileFullPath, FileNameOptionEnum.Encrypt);
            }
        }

        public void ConvertFileToPDF(FileItemEntity fileItem)
        {
            using (PDFDoc doc = new PDFDoc())
            {
                try
                {
                    switch (FileUtils.GetFileType(fileItem.FileFullPath))
                    {
                        case FileType.MSOffice:
                            pdftron.PDF.Convert.OfficeToPDF(doc, fileItem.FileFullPath, null);
                            break;

                        case FileType.HTML:
                            HTML2PDF converter = new HTML2PDF();

                            var htmlString = System.IO.File.ReadAllText(fileItem.FileFullPath);

                            converter.InsertFromHtmlString(htmlString);

                            converter.Convert(doc);
                            break;

                        case FileType.Other:
                            pdftron.PDF.Convert.ToPdf(doc, fileItem.FileFullPath);
                            break;

                        case FileType.NotMapped:
                            throw new ArgumentException($"Extensão {FileUtils.GetFileExtension(fileItem.FileFullPath)} não mapeada");

                        default:
                            break;
                    }

                    SaveFile(doc, fileItem.FileFullPath, FileNameOptionEnum.Convert);
                }
                catch (PDFNetException ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
