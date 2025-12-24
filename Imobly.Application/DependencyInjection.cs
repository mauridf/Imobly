using Imobly.Application.Interfaces;
using Imobly.Application.Mappings;
using Imobly.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Imobly.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Serviços
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IImovelService, ImovelService>();
            services.AddScoped<ILocatarioService, LocatarioService>();
            services.AddScoped<IContratoService, ContratoService>();
            services.AddScoped<IRecebimentoService, RecebimentoService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IManutencaoService, ManutencaoService>();
            services.AddScoped<ISeguroService, SeguroService>();
            services.AddScoped<IMovimentacaoFinanceiraService, MovimentacaoFinanceiraService>();
            services.AddScoped<IHistoricoReajusteService, HistoricoReajusteService>();

            // AutoMapper
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            return services;
        }
    }
}