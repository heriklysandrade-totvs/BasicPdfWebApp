using AutoMapper;
using BasicPdfWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicPdfWebApp.Controllers
{
    public class InternalBaseController<E, D> : ControllerBase
        where E : BaseEntity
        where D : BaseDTO
    {
        protected IMapper iMapper
        {
            get
            {
                if (_iMapper == null)
                    this._iMapper = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<D, E>();
                        cfg.CreateMap<E, D>();
                    }).CreateMapper();

                return this._iMapper;
            }
        }
        private IMapper _iMapper = null;
    }
}
