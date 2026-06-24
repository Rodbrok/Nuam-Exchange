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

    [Fact]
    public async Task HealthLiveReturnsOk()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ListReturnsOk()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().GetAsync("/api/v1/classifications");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ListReturnsCorrectPagination()
    {
        using var factory = CreateFactory();
        var page = await GetPage(factory, "/api/v1/classifications?page=1&pageSize=10");

        Assert.Equal(1, page.Page);
        Assert.Equal(10, page.PageSize);
        Assert.True(page.TotalItems >= 22);
        Assert.True(page.TotalPages >= 3);
    }

    [Fact]
    public async Task ListRespectsPageAndPageSize()
    {
        using var factory = CreateFactory();
        var page = await GetPage(factory, "/api/v1/classifications?page=2&pageSize=5");

        Assert.Equal(2, page.Page);
        Assert.Equal(5, page.PageSize);
        Assert.Equal(5, page.Items.Count);
    }

    [Fact]
    public async Task ListFiltersByMarket()
    {
        using var factory = CreateFactory();
        var page = await GetPage(factory, "/api/v1/classifications?market=Fondos&pageSize=20");

        Assert.All(page.Items, classification => Assert.Equal("Fondos", classification.Market));
    }

    [Fact]
    public async Task ListFiltersByFiscalYear()
    {
        using var factory = CreateFactory();
        var page = await GetPage(factory, "/api/v1/classifications?fiscalYear=2025&pageSize=20");

        Assert.All(page.Items, classification => Assert.Equal(2025, classification.FiscalYear));
    }

    [Fact]
    public async Task ListFiltersByStatus()
    {
        using var factory = CreateFactory();
        var page = await GetPage(factory, "/api/v1/classifications?status=Vigente&pageSize=20");

        Assert.All(page.Items, classification => Assert.Equal(ClassificationStatus.Vigente, classification.Status));
    }

    [Fact]
    public async Task ListSearchesByText()
    {
        using var factory = CreateFactory();
        var page = await GetPage(factory, "/api/v1/classifications?search=BONO&pageSize=20");

        Assert.Contains(
            page.Items,
            classification => classification.Instrument.Contains("BONO", StringComparison.OrdinalIgnoreCase)
                || classification.Description.Contains("bono", StringComparison.OrdinalIgnoreCase)
                || classification.EventSequence.Contains("BONO", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ListSortsCorrectly()
    {
        using var factory = CreateFactory();
        var page = await GetPage(factory, "/api/v1/classifications?sortBy=amount&sortDirection=asc&pageSize=20");

        Assert.True(page.Items.Zip(page.Items.Skip(1), (previous, current) => previous.Amount <= current.Amount).All(result => result));
    }

    [Fact]
    public async Task CatalogsReturnsOk()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().GetAsync("/api/v1/classifications/catalogs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CatalogsContainsValues()
    {
        using var factory = CreateFactory();
        var catalogs = await factory.CreateClient().GetFromJsonAsync<ClassificationCatalogsDto>(
            "/api/v1/classifications/catalogs",
            JsonOptions);

        Assert.NotNull(catalogs);
        Assert.NotEmpty(catalogs.Markets);
        Assert.NotEmpty(catalogs.Sources);
        Assert.NotEmpty(catalogs.FiscalYears);
        Assert.NotEmpty(catalogs.Statuses);
    }

    [Fact]
    public async Task GetExistingReturnsOk()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().GetAsync("/api/v1/classifications/CAL-2024-001");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetExistingIncludesEtag()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().GetAsync("/api/v1/classifications/CAL-2024-001");

        Assert.True(response.Headers.Contains("ETag"));
    }

    [Fact]
    public async Task GetExistingBodyIncludesBase64RowVersion()
    {
        using var factory = CreateFactory();
        var classification = await factory.CreateClient().GetFromJsonAsync<ClassificationDto>(
            "/api/v1/classifications/CAL-2024-001",
            JsonOptions);

        Assert.NotNull(classification);
        Convert.FromBase64String(classification.RowVersion);
    }

    [Fact]
    public async Task GetMissingReturnsNotFound()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().GetAsync("/api/v1/classifications/missing");

        await AssertProblemJsonAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task NotFoundUsesProblemJson()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().GetAsync("/api/v1/classifications/missing");

        await AssertProblemJsonAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task NotFoundIncludesProblemFields()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().GetAsync("/api/v1/classifications/missing");

        await AssertProblemJsonAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task InvalidPostReturnsBadRequest()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/v1/classifications",
            ValidCreate() with { FiscalYear = 2023 },
            JsonOptions);

        await AssertProblemJsonAsync(response, HttpStatusCode.BadRequest, expectErrors: true);
    }

    [Fact]
    public async Task InvalidPostContainsErrors()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/v1/classifications",
            ValidCreate() with { FiscalYear = 2023 },
            JsonOptions);

        await AssertProblemJsonAsync(response, HttpStatusCode.BadRequest, expectErrors: true);
    }

    [Fact]
    public async Task ValidPostReturnsCreated()
    {
        using var factory = CreateFactory();
        var response = await PostValid(factory);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task ValidPostIncludesLocation()
    {
        using var factory = CreateFactory();
        var response = await PostValid(factory);

        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task ValidPostIncludesEtag()
    {
        using var factory = CreateFactory();
        var response = await PostValid(factory);

        Assert.True(response.Headers.Contains("ETag"));
    }

    [Fact]
    public async Task ValidPostReturnsWriteResponse()
    {
        using var factory = CreateFactory();
        var response = await PostValid(factory);
        var responseDto = await response.Content.ReadFromJsonAsync<ClassificationWriteResponse>(JsonOptions);

        Assert.NotNull(responseDto);
        Assert.Equal("Calificación creada correctamente.", responseDto.Message);
        Assert.NotNull(responseDto.Classification);
    }

    [Fact]
    public async Task DuplicatePostReturnsConflict()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/v1/classifications",
            ValidCreate() with
            {
                FiscalYear = 2024,
                Market = "Acciones",
                Instrument = "NUAM-A",
                PaymentDate = new DateOnly(2024, 1, 15),
                EventSequence = "EVT-ACC-001",
            },
            JsonOptions);

        await AssertProblemJsonAsync(response, HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PutMissingReturnsNotFound()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().PutAsJsonAsync(
            "/api/v1/classifications/missing",
            ValidUpdate(),
            JsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ValidPutReturnsOkAndUpdatedEtag()
    {
        using var factory = CreateFactory();
        var client = factory.CreateClient();
        var response = await client.PutAsJsonAsync(
            "/api/v1/classifications/CAL-2024-001",
            ValidUpdate() with { Description = "Descripción actualizada" },
            JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("ETag"));
    }

    [Fact]
    public async Task PutInvalidIfMatchReturnsBadRequest()
    {
        using var factory = CreateFactory();
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/classifications/CAL-2024-001")
        {
            Content = JsonContent.Create(ValidUpdate(), options: JsonOptions),
        };
        request.Headers.TryAddWithoutValidation("If-Match", "not-base64");

        var response = await factory.CreateClient().SendAsync(request);

        await AssertProblemJsonAsync(response, HttpStatusCode.BadRequest, expectErrors: true);
    }

    [Fact]
    public async Task PutMismatchedIfMatchReturnsConflict()
    {
        using var factory = CreateFactory();
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/classifications/CAL-2024-001")
        {
            Content = JsonContent.Create(ValidUpdate(), options: JsonOptions),
        };
        request.Headers.TryAddWithoutValidation("If-Match", "AQIDBA==");

        var response = await factory.CreateClient().SendAsync(request);

        await AssertProblemJsonAsync(response, HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CopyMissingReturnsNotFound()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/v1/classifications/missing/copy",
            new CopyClassificationRequest(null, null, null),
            JsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CopyValidReturnsCreated()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/v1/classifications/CAL-2024-001/copy",
            new CopyClassificationRequest(new DateOnly(2027, 1, 1), "Copia válida", "EVT-COPY-001"),
            JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task DeleteExistingReturnsNoContent()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().DeleteAsync("/api/v1/classifications/CAL-2024-001");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMissingReturnsNotFound()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().DeleteAsync("/api/v1/classifications/missing");

        await AssertProblemJsonAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteNoContentHasNoBody()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().DeleteAsync("/api/v1/classifications/CAL-2024-001");

        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ResponsesIncludeCorrelationId()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().GetAsync("/api/v1/classifications");

        Assert.True(response.Headers.Contains("x-correlation-id"));
    }

    [Fact]
    public async Task ValidClientCorrelationIdIsEchoed()
    {
        using var factory = CreateFactory();
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/classifications");
        request.Headers.Add("x-correlation-id", "test-correlation-001");

        var response = await factory.CreateClient().SendAsync(request);

        Assert.Equal("test-correlation-001", response.Headers.GetValues("x-correlation-id").Single());
    }

    private static TestingWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));

    private static async Task<PaginatedResponse<ClassificationDto>> GetPage(
        WebApplicationFactory<Program> factory,
        string uri) =>
        (await factory.CreateClient().GetFromJsonAsync<PaginatedResponse<ClassificationDto>>(uri, JsonOptions))!;

    private static async Task AssertProblemJsonAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        bool expectErrors = false)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        Assert.True(document.RootElement.TryGetProperty("title", out _));
        Assert.Equal((int)expectedStatus, document.RootElement.GetProperty("status").GetInt32());
        Assert.True(document.RootElement.TryGetProperty("traceId", out _));

        if (expectErrors)
        {
            Assert.True(document.RootElement.TryGetProperty("errors", out _));
        }
    }

    private static Task<HttpResponseMessage> PostValid(WebApplicationFactory<Program> factory) =>
        factory.CreateClient().PostAsJsonAsync("/api/v1/classifications", ValidCreate(), JsonOptions);

    private static CreateClassificationRequest ValidCreate() =>
        new(
            "Acciones",
            "Manual",
            2027,
            "NEW-API",
            new DateOnly(2027, 1, 1),
            "Pago válido API",
            "EVT-API-001",
            1.123456m,
            10.25m,
            ClassificationStatus.Vigente);

    private static UpdateClassificationRequest ValidUpdate() =>
        new(
            "Acciones",
            "Manual",
            2026,
            "NUAM-A",
            new DateOnly(2026, 1, 15),
            "Dividendo válido",
            "EVT-001",
            1.123456m,
            100.25m,
            ClassificationStatus.Vigente);
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
