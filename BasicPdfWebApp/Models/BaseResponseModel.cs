using System;

namespace BasicPdfWebApp.Models
{
    public class BaseResponseModel<T> where T : class
    {
        public string message { get; set; }

        public DateTime executionDate { get; set; }

        public T data { get; set; } = null;
    }
}
