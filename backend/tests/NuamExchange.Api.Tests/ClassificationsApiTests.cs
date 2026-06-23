using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NuamExchange.Application.Classifications;
using NuamExchange.Application.Common;
using NuamExchange.Domain.Classifications;
using NuamExchange.Infrastructure.Persistence;
using Xunit;

namespace NuamExchange.Api.Tests;

public sealed class ClassificationsApiTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    [Fact] public async Task HealthLiveReturnsOk(){using var f=CreateFactory();var r=await f.CreateClient().GetAsync("/health/live");Assert.Equal(HttpStatusCode.OK,r.StatusCode);}
    [Fact] public async Task ListReturnsOk(){using var f=CreateFactory();var r=await f.CreateClient().GetAsync("/api/v1/classifications");Assert.Equal(HttpStatusCode.OK,r.StatusCode);}
    [Fact] public async Task ListReturnsCorrectPagination(){using var f=CreateFactory();var page=await GetPage(f,"/api/v1/classifications?page=1&pageSize=10");Assert.Equal(1,page.Page);Assert.Equal(10,page.PageSize);Assert.True(page.TotalItems>=22);Assert.True(page.TotalPages>=3);}
    [Fact] public async Task ListRespectsPageAndPageSize(){using var f=CreateFactory();var page=await GetPage(f,"/api/v1/classifications?page=2&pageSize=5");Assert.Equal(2,page.Page);Assert.Equal(5,page.PageSize);Assert.Equal(5,page.Items.Count);}
    [Fact] public async Task ListFiltersByMarket(){using var f=CreateFactory();var page=await GetPage(f,"/api/v1/classifications?market=Fondos&pageSize=20");Assert.All(page.Items,x=>Assert.Equal("Fondos",x.Market));}
    [Fact] public async Task ListFiltersByFiscalYear(){using var f=CreateFactory();var page=await GetPage(f,"/api/v1/classifications?fiscalYear=2025&pageSize=20");Assert.All(page.Items,x=>Assert.Equal(2025,x.FiscalYear));}
    [Fact] public async Task ListFiltersByStatus(){using var f=CreateFactory();var page=await GetPage(f,"/api/v1/classifications?status=Vigente&pageSize=20");Assert.All(page.Items,x=>Assert.Equal(ClassificationStatus.Vigente,x.Status));}
    [Fact] public async Task ListSearchesByText(){using var f=CreateFactory();var page=await GetPage(f,"/api/v1/classifications?search=BONO&pageSize=20");Assert.Contains(page.Items,x=>x.Instrument.Contains("BONO",StringComparison.OrdinalIgnoreCase)||x.Description.Contains("bono",StringComparison.OrdinalIgnoreCase)||x.EventSequence.Contains("BONO",StringComparison.OrdinalIgnoreCase));}
    [Fact] public async Task ListSortsCorrectly(){using var f=CreateFactory();var page=await GetPage(f,"/api/v1/classifications?sortBy=amount&sortDirection=asc&pageSize=20");Assert.True(page.Items.Zip(page.Items.Skip(1),(a,b)=>a.Amount<=b.Amount).All(x=>x));}
    [Fact] public async Task CatalogsReturnsOk(){using var f=CreateFactory();var r=await f.CreateClient().GetAsync("/api/v1/classifications/catalogs");Assert.Equal(HttpStatusCode.OK,r.StatusCode);}
    [Fact] public async Task CatalogsContainsValues(){using var f=CreateFactory();var dto=await f.CreateClient().GetFromJsonAsync<ClassificationCatalogsDto>("/api/v1/classifications/catalogs",JsonOptions);Assert.NotEmpty(dto!.Markets);Assert.NotEmpty(dto.Sources);Assert.NotEmpty(dto.FiscalYears);Assert.NotEmpty(dto.Statuses);}
    [Fact] public async Task GetExistingReturnsOk(){using var f=CreateFactory();var r=await f.CreateClient().GetAsync("/api/v1/classifications/CAL-2024-001");Assert.Equal(HttpStatusCode.OK,r.StatusCode);}
    [Fact] public async Task GetExistingIncludesEtag(){using var f=CreateFactory();var r=await f.CreateClient().GetAsync("/api/v1/classifications/CAL-2024-001");Assert.True(r.Headers.Contains("ETag"));}
    [Fact] public async Task GetExistingBodyIncludesBase64RowVersion(){using var f=CreateFactory();var dto=await f.CreateClient().GetFromJsonAsync<ClassificationDto>("/api/v1/classifications/CAL-2024-001",JsonOptions);Assert.NotNull(dto);Convert.FromBase64String(dto!.RowVersion);}
    [Fact] public async Task GetMissingReturnsNotFound(){using var f=CreateFactory();var r=await f.CreateClient().GetAsync("/api/v1/classifications/missing");Assert.Equal(HttpStatusCode.NotFound,r.StatusCode);}
    [Fact] public async Task NotFoundUsesProblemJson(){using var f=CreateFactory();var r=await f.CreateClient().GetAsync("/api/v1/classifications/missing");Assert.Equal("application/problem+json",r.Content.Headers.ContentType!.MediaType);}
    [Fact] public async Task NotFoundIncludesProblemFields(){using var f=CreateFactory();using var doc=await Problem(f,"/api/v1/classifications/missing");Assert.True(doc.RootElement.TryGetProperty("title",out _));Assert.Equal(404,doc.RootElement.GetProperty("status").GetInt32());Assert.True(doc.RootElement.TryGetProperty("traceId",out _));}
    [Fact] public async Task InvalidPostReturnsBadRequest(){using var f=CreateFactory();var r=await f.CreateClient().PostAsJsonAsync("/api/v1/classifications",ValidCreate() with { FiscalYear=2023 },JsonOptions);Assert.Equal(HttpStatusCode.BadRequest,r.StatusCode);}
    [Fact] public async Task InvalidPostContainsErrors(){using var f=CreateFactory();var r=await f.CreateClient().PostAsJsonAsync("/api/v1/classifications",ValidCreate() with { FiscalYear=2023 },JsonOptions);using var json=await JsonDocument.ParseAsync(await r.Content.ReadAsStreamAsync());Assert.Equal("application/problem+json",r.Content.Headers.ContentType!.MediaType);Assert.True(json.RootElement.TryGetProperty("errors",out _));}
    [Fact] public async Task ValidPostReturnsCreated(){using var f=CreateFactory();var r=await PostValid(f);Assert.Equal(HttpStatusCode.Created,r.StatusCode);}
    [Fact] public async Task ValidPostIncludesLocation(){using var f=CreateFactory();var r=await PostValid(f);Assert.NotNull(r.Headers.Location);}
    [Fact] public async Task ValidPostIncludesEtag(){using var f=CreateFactory();var r=await PostValid(f);Assert.True(r.Headers.Contains("ETag"));}
    [Fact] public async Task ValidPostReturnsWriteResponse(){using var f=CreateFactory();var r=await PostValid(f);var dto=await r.Content.ReadFromJsonAsync<ClassificationWriteResponse>(JsonOptions);Assert.NotNull(dto);Assert.Equal("Calificación creada correctamente.",dto!.Message);Assert.NotNull(dto.Classification);}
    [Fact] public async Task DuplicatePostReturnsConflict(){using var f=CreateFactory();var r=await f.CreateClient().PostAsJsonAsync("/api/v1/classifications",ValidCreate() with { FiscalYear=2024,Market="Acciones",Instrument="NUAM-A",PaymentDate=new DateOnly(2024,1,15),EventSequence="EVT-ACC-001" },JsonOptions);Assert.Equal(HttpStatusCode.Conflict,r.StatusCode);}
    [Fact] public async Task PutMissingReturnsNotFound(){using var f=CreateFactory();var r=await f.CreateClient().PutAsJsonAsync("/api/v1/classifications/missing",ValidUpdate(),JsonOptions);Assert.Equal(HttpStatusCode.NotFound,r.StatusCode);}
    [Fact] public async Task ValidPutReturnsOkAndUpdatedEtag(){using var f=CreateFactory();var client=f.CreateClient();var r=await client.PutAsJsonAsync("/api/v1/classifications/CAL-2024-001",ValidUpdate() with { Description="Descripción actualizada" },JsonOptions);Assert.Equal(HttpStatusCode.OK,r.StatusCode);Assert.True(r.Headers.Contains("ETag"));}
    [Fact] public async Task PutInvalidIfMatchReturnsBadRequest(){using var f=CreateFactory();var req=new HttpRequestMessage(HttpMethod.Put,"/api/v1/classifications/CAL-2024-001"){Content=JsonContent.Create(ValidUpdate(),options:JsonOptions)};req.Headers.TryAddWithoutValidation("If-Match","not-base64");var r=await f.CreateClient().SendAsync(req);Assert.Equal(HttpStatusCode.BadRequest,r.StatusCode);}
    [Fact] public async Task PutMismatchedIfMatchReturnsConflict(){using var f=CreateFactory();var req=new HttpRequestMessage(HttpMethod.Put,"/api/v1/classifications/CAL-2024-001"){Content=JsonContent.Create(ValidUpdate(),options:JsonOptions)};req.Headers.TryAddWithoutValidation("If-Match","AQIDBA==");var r=await f.CreateClient().SendAsync(req);Assert.Equal(HttpStatusCode.Conflict,r.StatusCode);}
    [Fact] public async Task CopyMissingReturnsNotFound(){using var f=CreateFactory();var r=await f.CreateClient().PostAsJsonAsync("/api/v1/classifications/missing/copy",new CopyClassificationRequest(null,null,null),JsonOptions);Assert.Equal(HttpStatusCode.NotFound,r.StatusCode);}
    [Fact] public async Task CopyValidReturnsCreated(){using var f=CreateFactory();var r=await f.CreateClient().PostAsJsonAsync("/api/v1/classifications/CAL-2024-001/copy",new CopyClassificationRequest(new DateOnly(2027,1,1),"Copia válida","EVT-COPY-001"),JsonOptions);Assert.Equal(HttpStatusCode.Created,r.StatusCode);}
    [Fact] public async Task DeleteExistingReturnsNoContent(){using var f=CreateFactory();var r=await f.CreateClient().DeleteAsync("/api/v1/classifications/CAL-2024-001");Assert.Equal(HttpStatusCode.NoContent,r.StatusCode);}
    [Fact] public async Task DeleteMissingReturnsNotFound(){using var f=CreateFactory();var r=await f.CreateClient().DeleteAsync("/api/v1/classifications/missing");Assert.Equal(HttpStatusCode.NotFound,r.StatusCode);}
    [Fact] public async Task DeleteNoContentHasNoBody(){using var f=CreateFactory();var r=await f.CreateClient().DeleteAsync("/api/v1/classifications/CAL-2024-001");Assert.Equal(string.Empty,await r.Content.ReadAsStringAsync());}
    [Fact] public async Task ResponsesIncludeCorrelationId(){using var f=CreateFactory();var r=await f.CreateClient().GetAsync("/api/v1/classifications");Assert.True(r.Headers.Contains("x-correlation-id"));}
    [Fact] public async Task ValidClientCorrelationIdIsEchoed(){using var f=CreateFactory();var req=new HttpRequestMessage(HttpMethod.Get,"/api/v1/classifications");req.Headers.Add("x-correlation-id","test-correlation-001");var r=await f.CreateClient().SendAsync(req);Assert.Equal("test-correlation-001",r.Headers.GetValues("x-correlation-id").Single());}

    private static TestingWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
    private static async Task<PaginatedResponse<ClassificationDto>> GetPage(WebApplicationFactory<Program> factory, string uri) => (await factory.CreateClient().GetFromJsonAsync<PaginatedResponse<ClassificationDto>>(uri, JsonOptions))!;
    private static async Task<JsonDocument> Problem(WebApplicationFactory<Program> factory, string uri)
    {
        var response = await factory.CreateClient().GetAsync(uri);
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    }
    private static Task<HttpResponseMessage> PostValid(WebApplicationFactory<Program> factory) => factory.CreateClient().PostAsJsonAsync("/api/v1/classifications", ValidCreate(), JsonOptions);
    private static CreateClassificationRequest ValidCreate() => new("Acciones", "Manual", 2027, "NEW-API", new DateOnly(2027, 1, 1), "Pago válido API", "EVT-API-001", 1.123456m, 10.25m, ClassificationStatus.Vigente);
    private static UpdateClassificationRequest ValidUpdate() => new("Acciones", "Manual", 2026, "NUAM-A", new DateOnly(2026, 1, 15), "Dividendo válido", "EVT-001", 1.123456m, 100.25m, ClassificationStatus.Vigente);
}

internal sealed class TestingWebApplicationFactory(string databaseName) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<NuamExchangeDbContext>>();
            services.AddDbContext<NuamExchangeDbContext>(options => options.UseInMemoryDatabase(databaseName));
            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NuamExchangeDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
}
