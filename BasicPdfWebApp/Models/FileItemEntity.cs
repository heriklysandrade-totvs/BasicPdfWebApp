using System;
using System.Linq;

namespace BasicPdfWebApp.Models
{
    public class FileItemEntity : BaseEntity
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public byte[] Bytes { get; set; }

        public string FileFullPath { get; set; }

        public string FileName { get; set; }

        //public string SafeFileName
        //{
        //    get
        //    {
        //        return FileFullPath.Split("\\").Last();
        //    }
        //}
    }
}
