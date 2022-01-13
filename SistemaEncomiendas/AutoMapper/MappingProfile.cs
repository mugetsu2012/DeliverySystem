using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SistemaEncomiendas.Core.DTO;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.AutoMapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CrearEditarUsuarioDto, Usuario>().ReverseMap();

            CreateMap<CrearEditarPaqueteDto, Paquete>().ReverseMap();
        }
    }
}
