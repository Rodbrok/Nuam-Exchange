using Microsoft.AspNetCore.Mvc;
using NuamExchange.Application.Classifications;
using NuamExchange.Application.Common;

namespace NuamExchange.Api.Controllers;

[ApiController]
[Route("api/v1/classifications")]
public sealed class ClassificationsController(IClassificationService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ClassificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<ClassificationDto>>> List(
        [FromQuery] ClassificationListRequest request,
        CancellationToken ct) =>
        Ok(await service.ListAsync(request, ct));

    [HttpGet("catalogs")]
    [ProducesResponseType(typeof(ClassificationCatalogsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ClassificationCatalogsDto>> Catalogs(CancellationToken ct) =>
        Ok(await service.GetCatalogsAsync(ct));

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClassificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassificationDto>> Get(string id, CancellationToken ct)
    {
        var dto = await service.GetByIdAsync(id, ct);
        SetEtag(dto.RowVersion);

        return Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClassificationWriteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClassificationWriteResponse>> Create(
        [FromBody] CreateClassificationRequest request,
        CancellationToken ct)
    {
        var response = await service.CreateAsync(request, ct);
        SetEtag(response.Classification.RowVersion);

        return CreatedAtAction(nameof(Get), new { id = response.Classification.Id }, response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ClassificationWriteResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ClassificationWriteResponse>> Update(
        string id,
        [FromBody] UpdateClassificationRequest request,
        [FromHeader(Name = "If-Match")] string? ifMatch,
        CancellationToken ct)
    {
        var response = await service.UpdateAsync(id, request, ifMatch, ct);
        SetEtag(response.Classification.RowVersion);

        return Ok(response);
    }

    [HttpPost("{id}/copy")]
    [ProducesResponseType(typeof(ClassificationWriteResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ClassificationWriteResponse>> Copy(
        string id,
        [FromBody] CopyClassificationRequest request,
        CancellationToken ct)
    {
        var response = await service.CopyAsync(id, request, ct);
        SetEtag(response.Classification.RowVersion);

        return CreatedAtAction(nameof(Get), new { id = response.Classification.Id }, response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(
        string id,
        [FromHeader(Name = "If-Match")] string? ifMatch,
        CancellationToken ct)
    {
        await service.DeleteAsync(id, ifMatch, ct);

        return NoContent();
    }

    private void SetEtag(string rowVersion) =>
        Response.Headers.ETag = $"\"{rowVersion}\"";
}
