using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Owin;
using papya_api.DataProvider;
using papya_api.Models;
using papya_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Mvc.Cors;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

[assembly: OwinStartup(typeof(papya_api.Startup))]
namespace papya_api
{
    public class Startup
    {
        readonly string MyAllowSpecifyOrigins = "MyPolicy";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var connection = Configuration["ConnectionStrings:tingledb"];

            IdentityModelEventSource.ShowPII = true;

            services.AddSignalR();
            //services.AddCors();
            //services.Configure<MvcOptions>(options => {
            //options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));

            //});
            //Habilitando CORS
            //services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            //{
            //    builder.AllowAnyOrigin()
            //           .AllowAnyMethod()
            //           .AllowAnyHeader()
            //           //Adicionados para signalR
            //           //.AllowCredentials()
            //           //.WithOrigins("https://localhost:5002")
            //    ;
            //}));
            

            services.AddCors(options =>
            {
                //options.AddPolicy("MyPolicy",
                //    builder =>
                //    {
                //        builder.AllowAnyOrigin()
                //       .AllowAnyMethod()
                //       .AllowAnyHeader();
                //    });

                //options.AddPolicy("MyPolicy",
                options.AddPolicy(name: MyAllowSpecifyOrigins,
                    builder =>
                    {
                        builder.WithOrigins(
                            "https://papya.com.br",
                            "https://www.papya.com.br",
                            "https://localhost:5002"
                        //    "http://localhost:3000",
                        //    "http://192.168.5.180:3000",
                        //    "http://192.168.5.180",
                        //    "https://localhost:7215",
                        //    "https://192.168.5.179:7215"
                        )
                        //builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();


                    });
            });
            




            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            
            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);


            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

                // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;

                // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Tempo de tolerância para a expiração de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;
            });

            // Ativa o uso do token como forma de autorizar o acesso
            // a recursos deste projeto
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            //Registra a classe
            services.AddTransient<UsuarioDAO>();
            services.AddTransient<EstabelecimentoDAO>();
            services.AddTransient<FuncionarioDAO>();
            services.AddTransient<PedidoItemDAO>();
            services.AddTransient<CardapioDAO>();
            services.AddTransient<PromocoesDAO>();

            ConfigServiceDI(services);


            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddMvc();

            var swConfig = Configuration.GetSection("SwaggerURL:URL").Value;

            ConfigSwaggerDoc(services, swConfig);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            //Seta a cultura padrão
            var supportedCultures = new[]{
                new CultureInfo("en-US")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                FallBackToParentCultures = false
            });
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            //Seta a cultura padrão

            // configuracao WebApi  IApplicationBuilder
            //var config = new HttpConfiguration();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");
                //app.UseExceptionHandler("/Error");
                //app.UseHsts();
            }

            //Configuraçoes para acesso a imagens estaticas
            app.UseStaticFiles();// For the wwwroot folder


            app.UseCors(MyAllowSpecifyOrigins);


            // Configure the pipeline to use authentication
            //app.UseAuthentication();
            app.UseHttpsRedirection();

            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<PushHub>("/pushhub");
            //});

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<PushHub>("/pushhub");
            });

            //app.UseSi


            ConfigSwaggerApp(app);
            app.UseMvc();

       }

        /*private void AtivarGeracaoTokenAcesso(IAppBuilder app)
        {
            var opcoesConfiguracaoToken = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
                Provider = new ProviderDeTokensDeAcesso()
            };

            app.UseOAuthAuthorizationServer(opcoesConfiguracaoToken);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            
        }*/

        private static void ConfigServiceDI(IServiceCollection services)
        {
            //Adicionar escopo para cada classe nova
            services.AddScoped<ITipoUsuarioDataProvider, TipoUsuarioDataProvider>();
            services.AddScoped<IUsuarioDataProvider, UsuarioDataProvider>();
            services.AddScoped<IContaDataProvider, ContaDataProvider>();
            services.AddScoped<IContaDetalheDataProvider, ContaDetalheDataProvider>();
            services.AddScoped<ICardapioDataProvider, CardapioDataProvider>();
            services.AddScoped<IPedidoDataProvider, PedidoDataProvider>();
            services.AddScoped<IEmailSender, AuthMessageSender>();
            services.AddScoped<IPromocoesDataProvider, PromocoesDataProvider>();
            services.AddScoped<ICategoriaDataProvider, CategoriaDataProvider>();
            services.AddScoped<IEstabelecimentoDataProvider, EstabelecimentoDataProvider>();
            services.AddScoped<ITipoEstabelecimentoDataProvider, TipoEstabelecimentoDataProvider>();
            services.AddScoped<ITipoFuncionarioDataProvider, TipoFuncionarioDataProvider>();
            services.AddScoped<IFuncionarioDataProvider, FuncionarioDataProvider>();
            services.AddScoped<IStatusItemDataProvider, StatusItemDataProvider>();
            services.AddScoped<IAssinaturaDataProvider, AssinaturaDataProvider>();
            services.AddScoped<IFuncionarioMesaDataProvider, FuncionarioMesaDataProvider>();
            services.AddScoped<IMeioPagamentoDataProvider, MeioPagamentoDataProvider>();
            services.AddScoped<IMesaDataProvider, MesaDataProvider>();
            services.AddScoped<INotificacaoDataProvider, NotificacaoDataProvider>();
            
        }

        private static void ConfigSwaggerDoc(IServiceCollection services, string pSwConfig)
        {
            // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            
            
            //services.AddSwaggerGen();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Papya",
                    Description = "Papya - Bar digital",
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                //Configuração do Swagger
                if (pSwConfig!="")
                {
                    options.AddServer(new OpenApiServer
                    {
                        //Url = "https://www.papya.com.br",
                        Url = pSwConfig,
                        Description = "Papya Swagger",
                    }) ;
                }
                //Configuração do Swagger



                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                    });

            // c.IncludeXmlComments(xmlPath);            
        });
        }

        private static void ConfigSwaggerApp(IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("../swagger/v1/swagger.json", "papya_api._v1");
                //Essa configuração funciona publicado
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "papya_api._v1");
                //Essa configuração funciona publicado
            });

            //app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
