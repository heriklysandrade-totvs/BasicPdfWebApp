using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace BasicPdfWebApp.Utils
{
    public static class FileUtils
    {
        public const float INCH = 72f;

        public const string SHORT_DEFAULT_OUTPUT_PATH = "\\Watch_Folders\\Output\\";

        public const string SHORT_DEFAULT_INPUT_PATH = "\\Watch_Folders\\Input\\";

        public const string _MSOfficeFileExtensions = ".odt;.ods;.odp;.odg;.odf;.ott;.emf;.fods;.fodt;.stw;.lwp;.wps;.xml;.xls;.xlsx;.xlsm;.xlt;.csv;" +
                                                       ".doc;.docx;.docm;.dotm;.dot;.dotx;.eml;.msg;.rtf;.ppt;.pptx;.pptm;.pps;.ppsx;.pot;.potx;.potm";

        public const string _HtmlFileExtensions = ".html;.htm;.mht;.mhtml";

        public const string _OtherFileExtensions = ".bmp;.jpg;.jpeg;.cdr;.dcx;.ico;.gif;.pct;.pcx;.pic;.png;.rgb;.sam;.tif;.tiff;.tga;.wpg;.txt";

        public static string GetFilePath(string fullFileName, bool useDefaultPath = true)
        {
            if (useDefaultPath)
                return GetDefaultOutputPath();

            return fullFileName.TrimEnd(fullFileName.Split("\\").Last().ToCharArray());
        }

        public static string GetNewFileName(string fullFileName, FileNameOptionEnum nameOptionEnum, bool useDefaultPath = true)
        {
            switch (nameOptionEnum)
            {
                case FileNameOptionEnum.Edit:
                    return GetFilePath(fullFileName, useDefaultPath) + $"{fullFileName.Split("\\").Last().TrimEnd(".pdf".ToCharArray())}_edit_{new Random().Next(0, 200)}.pdf";

                case FileNameOptionEnum.Copy:
                    return GetFilePath(fullFileName, useDefaultPath) + $"{fullFileName.Split("\\").Last().TrimEnd(".pdf".ToCharArray())}_copy_{new Random().Next(0, 200)}.pdf";

                case FileNameOptionEnum.Merge:
                    return GetFilePath(fullFileName, useDefaultPath) + $"{fullFileName.Split("\\").Last().TrimEnd(".pdf".ToCharArray())}_merge_{new Random().Next(0, 200)}.pdf";

                case FileNameOptionEnum.Encrypt:
                    return GetFilePath(fullFileName, useDefaultPath) + $"{fullFileName.Split("\\").Last().TrimEnd(".pdf".ToCharArray())}_encrypt_{new Random().Next(0, 200)}.pdf";

                case FileNameOptionEnum.Compress:
                    return GetFilePath(fullFileName, useDefaultPath) + $"{fullFileName.Split("\\").Last().TrimEnd(".pdf".ToCharArray())}_compress_{new Random().Next(0, 200)}.pdf";

                case FileNameOptionEnum.Convert:
                    return GetFilePath(fullFileName, useDefaultPath) + $"{fullFileName.Split("\\").Last().Split('.').First()}_convert_{new Random().Next(0, 200)}.pdf";

                default:
                    return string.Empty;
            }
        }

        public static string GetSafeFileName(string fullFileName)
        {
            return fullFileName.Split("\\").Last();
        }

        public static FileType GetFileType(string path)
        {
            var extension = GetFileExtension(path);

            if (_MSOfficeFileExtensions.Split(";").Contains(extension))
            {
                return FileType.MSOffice;
            }
            else if (_HtmlFileExtensions.Split(";").Contains(extension))
            {
                return FileType.HTML;
            }
            else if (_OtherFileExtensions.Split(";").Contains(extension))
            {
                return FileType.Other;
            }
            else
            {
                return FileType.NotMapped;
            }
        }

        public static string GetFileExtension(string path)
        {
            return ("." + path.Split("\\").Last().Split(".").Last()).ToLower();
        }

        public static void CheckDefaultPaths()
        {
            if (!Directory.Exists(GetDefaultOutputPath()))
                Directory.CreateDirectory(GetDefaultOutputPath());

            if (!Directory.Exists(GetDefaultInputPath()))
                Directory.CreateDirectory(GetDefaultInputPath());
        }

        public static string GetDefaultOutputPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + FileUtils.SHORT_DEFAULT_OUTPUT_PATH;
        }

        public static string GetDefaultInputPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + FileUtils.SHORT_DEFAULT_INPUT_PATH;
        }

        public static byte[] GetBytesFromFormFile(this IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    public enum FileNameOptionEnum : int
    {
        Edit = 1,
        Copy = 2,
        Merge = 3,
        Encrypt = 4,
        Compress = 5,
        Convert = 6
    }

    public enum FileType
    {
        MSOffice = 1,
        HTML = 2,
        Other = 3,
        NotMapped = 99
    }
}
