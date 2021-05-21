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

        #endregion

        public FileItemDTO CombineFiles(List<FileItemEntity> fileItems)
        {
            try
            {
                using (PDFDoc doc = new PDFDoc())
                {
                    doc.InitSecurityHandler();

                    var fileNewName = new StringBuilder();

                    foreach (var fileItem in fileItems)
                    {
                        if (fileNewName.Length > 0)
                            fileNewName.Append("_");

                        fileNewName.Append(fileItem.FileName.TrimEnd(".pdf".ToCharArray()));

                        fileItem.FileFullPath = FileUtils.GetDefaultInputPath() + fileItem.FileName;

                        File.WriteAllBytes(fileItem.FileFullPath, fileItem.Bytes);

                        using (PDFDoc in_doc = new PDFDoc(fileItem.FileFullPath))
                        {
                            in_doc.InitSecurityHandler();

                            doc.InsertPages(doc.GetPageCount() + 1, in_doc, 1, in_doc.GetPageCount(), PDFDoc.InsertFlag.e_none);
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

                    var fileItemDTO = new FileItemDTO()
                    {
                        Id = new Random().Next(1, 100),
                        FileFullPath = FileUtils.GetNewFileName(fileNewName.ToString(), FileNameOptionEnum.Combine)
                    };
                    fileItemDTO.FileName = FileUtils.GetSafeFileName(fileItemDTO.FileFullPath);

                    doc.Save(fileItemDTO.FileFullPath, SDFDoc.SaveOptions.e_linearized);
                    
                    return fileItemDTO;
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

            using (PDFDoc in_doc = new PDFDoc(fileItem.FileFullPath))
            {
                Optimizer.Optimize(in_doc);

                using (PDFDoc doc = new PDFDoc())
                {
                    doc.InsertPages(doc.GetPageCount() + 1, in_doc, 1, in_doc.GetPageCount(), PDFDoc.InsertFlag.e_none);

                    in_doc.Save(fileItemDTO.FileFullPath, SDFDoc.SaveOptions.e_linearized);
                }
            }

            return fileItemDTO;
        }

        public FileItemDTO ProtectFile(FileItemEntity fileItem)
        {
            fileItem.FileFullPath = FileUtils.GetDefaultInputPath() + fileItem.FileName;

            File.WriteAllBytes(fileItem.FileFullPath, fileItem.Bytes);

            var fileItemDTO = new FileItemDTO()
            {
                Id = fileItem.Id,
                FileFullPath = FileUtils.GetNewFileName(fileItem.FileName, FileNameOptionEnum.Protect)
            };
            fileItemDTO.FileName = FileUtils.GetSafeFileName(fileItemDTO.FileFullPath);

            using (PDFDoc doc = new PDFDoc(fileItem.FileFullPath))
            {
                SecurityHandler new_handler = new SecurityHandler(SecurityHandler.AlgorithmType.e_AES_256);

                string my_password = "senha_padrao";
                new_handler.ChangeUserPassword(my_password);

                new_handler.SetPermission(SecurityHandler.Permission.e_print, true);
                new_handler.SetPermission(SecurityHandler.Permission.e_extract_content, false);

                doc.SetSecurityHandler(new_handler);

                doc.Save(fileItemDTO.FileFullPath, SDFDoc.SaveOptions.e_linearized);
            }

            return fileItemDTO;
        }

        public FileItemDTO ConvertFileToPDF(FileItemEntity fileItem)
        {
            fileItem.FileFullPath = FileUtils.GetDefaultInputPath() + fileItem.FileName;

            File.WriteAllBytes(fileItem.FileFullPath, fileItem.Bytes);

            var fileItemDTO = new FileItemDTO()
            {
                Id = fileItem.Id,
                FileFullPath = FileUtils.GetNewFileName(fileItem.FileName, FileNameOptionEnum.Convert)
            };
            fileItemDTO.FileName = FileUtils.GetSafeFileName(fileItemDTO.FileFullPath);

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

                            var htmlString = File.ReadAllText(fileItem.FileFullPath);

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

                    doc.Save(fileItemDTO.FileFullPath, SDFDoc.SaveOptions.e_linearized);

                    return fileItemDTO;
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
