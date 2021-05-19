using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicPdfWebApp.Models
{
    public class FileItem
    {
        public int Id { get; set; }
        
        //public Guid Guid { get; set; }

        public string FileFullPath { get; set; }

        public string Secret { get; set; }
    }
}
