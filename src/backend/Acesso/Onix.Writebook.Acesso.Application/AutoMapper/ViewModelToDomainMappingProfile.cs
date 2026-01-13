using AutoMapper;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.Perfil;
using Onix.Writebook.Acesso.Application.ViewModels.PerfilPermissao;
using Onix.Writebook.Acesso.Application.ViewModels.Permissao;
using Onix.Writebook.Acesso.Application.ViewModels.Registrar;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.ValueObjects;

namespace Onix.Writebook.Acesso.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<string, SenhaValueObject>()
                .ConstructUsing(x => new SenhaValueObject(x));

            CreateMap<UsuarioViewModel, Usuario>();

            CreateMap<RegistrarUsuarioViewModel, Usuario>()
                .ForMember(dest => dest.Senha, opt => opt.MapFrom(src => src.Senha));

            CreateMap<PerfilViewModel, Perfil>();

            CreateMap<PermissaoViewModel, Permissao>();

            CreateMap<PerfilPermissaoViewModel, PerfilPermissao>();

            CreateMap<RefreshTokenViewModel, RefreshToken>();
        }
    }
}
