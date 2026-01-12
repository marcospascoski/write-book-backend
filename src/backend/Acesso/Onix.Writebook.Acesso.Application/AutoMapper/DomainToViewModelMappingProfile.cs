using AutoMapper;
using Onix.Framework.Infra.Data.Implementation;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.Perfil;
using Onix.Writebook.Acesso.Application.ViewModels.PerfilPermissao;
using Onix.Writebook.Acesso.Application.ViewModels.Permissao;
using Onix.Writebook.Acesso.Application.ViewModels.Registrar;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Enums;
using Onix.Writebook.Acesso.Domain.ValueObjects;
using Onix.Writebook.Core.Application.ViewModels;

namespace Onix.Writebook.Acesso.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<SenhaValueObject, string>()
                .ConstructUsing(x => x.Valor);
            CreateMap<Usuario, UsuarioViewModel>();
            CreateMap<Usuario, RegistrarUsuarioViewModel>();

            CreateMap<Usuario, EmailRedefinicaoSenhaViewModel>()
                .ForMember(dest => dest.Destinatario, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.NomeUsuario, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.TokenRedefinicao, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items.TryGetValue("TokenRedefinicao", out var token)
                        ? token as string
                        : string.Empty));

            CreateMap<Usuario, EmailConfirmacaoViewModel>()
                .ForMember(dest => dest.Destinatario, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.NomeUsuario, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.TokenConfirmacao, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items.TryGetValue("TokenConfirmacao", out var token)
                        ? token as string
                        : string.Empty));

            CreateMap<PagedItems<Usuario>, PagedItems<UsuarioViewModel>>();

            CreateMap<Perfil, PerfilViewModel>();
            CreateMap<PagedItems<Perfil>, PagedItems<PerfilViewModel>>();

            CreateMap<PerfilPermissao, PerfilPermissaoViewModel>();

            CreateMap<Permissao, PermissaoViewModel>();
            CreateMap<PagedItems<Permissao>, PagedItems<PermissaoViewModel>>();
        }
    }
}
