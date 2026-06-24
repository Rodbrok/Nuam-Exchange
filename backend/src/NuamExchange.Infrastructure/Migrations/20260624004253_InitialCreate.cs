using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NuamExchange.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        private static readonly string[] SeedColumns =
        [
            "Id",
            "Amount",
            "CreatedAt",
            "Description",
            "EventSequence",
            "FiscalYear",
            "Instrument",
            "Market",
            "PaymentDate",
            "Source",
            "Status",
            "UpdateFactor",
            "UpdatedAt",
        ];

        private static readonly string[] UniqueIndexColumns =
        [
            "FiscalYear",
            "Market",
            "Instrument",
            "PaymentDate",
            "EventSequence",
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Classifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Market = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FiscalYear = table.Column<int>(type: "int", nullable: false),
                    Instrument = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EventSequence = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UpdateFactor = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classifications", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Classifications",
                columns: SeedColumns,
                values: new object[,]
                {
                    { "CAL-2024-001", 1250000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dividendo provisorio acciones serie A", "EVT-ACC-001", 2024, "NUAM-A", "Acciones", new DateOnly(2024, 1, 15), "Manual", "Vigente", 1.002345m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2024-002", 8425000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interés bono corporativo febrero", "EVT-RF-014", 2024, "BNUAM-24", "Renta Fija", new DateOnly(2024, 2, 28), "Carga X Factor", "Pendiente", 0.998761m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2024-003", 2378000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Rescate fondo liquidez local", "EVT-FON-009", 2024, "FONDO-LIQ", "Fondos", new DateOnly(2024, 3, 12), "Carga X Monto", "Observada", 1.010102m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2024-004", 980000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distribución anual de utilidades", "EVT-ACC-019", 2024, "ANDES-B", "Acciones", new DateOnly(2024, 4, 30), "Manual", "Rechazada", 1.000001m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2024-005", 11540000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Amortización parcial instrumento reajustable", "EVT-RF-022", 2024, "BCORP-27", "Renta Fija", new DateOnly(2024, 5, 18), "Carga X Monto", "Vigente", 1.034568m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2024-006", 3210000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distribución fondo crecimiento", "EVT-FON-017", 2024, "FONDO-CREC", "Fondos", new DateOnly(2024, 6, 21), "Manual", "Pendiente", 1.012m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2025-001", 4567000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dividendo adicional serie C", "EVT-ACC-031", 2025, "PACIFICO-C", "Acciones", new DateOnly(2025, 1, 10), "Carga X Factor", "Vigente", 1.045678m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2025-002", 18450000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Cupón semestral tesorería local", "EVT-RF-034", 2025, "TES-CLP-25", "Renta Fija", new DateOnly(2025, 2, 14), "Manual", "Observada", 1.007654m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2025-003", 7650000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pago beneficio fondo infraestructura", "EVT-FON-028", 2025, "FONDO-INFRA", "Fondos", new DateOnly(2025, 3, 19), "Carga X Monto", "Pendiente", 0.995432m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2025-004", 2143000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Evento societario de caja", "EVT-ACC-044", 2025, "CORDILLERA-A", "Acciones", new DateOnly(2025, 4, 23), "Manual", "Vigente", 1.000456m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2025-005", 9350000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interés instrumento bancario", "EVT-RF-041", 2025, "BANC-30", "Renta Fija", new DateOnly(2025, 5, 7), "Carga X Factor", "Rechazada", 1.023001m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2025-006", 5289000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distribución extraordinaria fondo valor", "EVT-FON-033", 2025, "FONDO-VALOR", "Fondos", new DateOnly(2025, 6, 16), "Manual", "Vigente", 1.01789m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2025-007", 1599000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pago evento especial acciones", "EVT-ACC-052", 2025, "NORTE-D", "Acciones", new DateOnly(2025, 7, 22), "Carga X Monto", "Pendiente", 1.008m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2025-008", 13200000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Liquidación cupón deuda local", "EVT-RF-058", 2025, "DEUDA-LOCAL", "Renta Fija", new DateOnly(2025, 8, 11), "Manual", "Observada", 0.991234m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2026-001", 4490000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distribución fondo balanceado", "EVT-FON-061", 2026, "FONDO-BAL", "Fondos", new DateOnly(2026, 1, 9), "Carga X Factor", "Vigente", 1.006789m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2026-002", 6780000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dividendo mínimo obligatorio", "EVT-ACC-067", 2026, "SUR-A", "Acciones", new DateOnly(2026, 2, 27), "Manual", "Pendiente", 1.011111m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2026-003", 22100000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pago cupón bono temático", "EVT-RF-071", 2026, "BONO-VERDE", "Renta Fija", new DateOnly(2026, 3, 13), "Carga X Monto", "Vigente", 1.029999m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2026-004", 3890000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Rescate programado fondo tecnología", "EVT-FON-074", 2026, "FONDO-TEC", "Fondos", new DateOnly(2026, 4, 24), "Manual", "Rechazada", 1.00321m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2026-005", 2785000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distribución complementaria", "EVT-ACC-080", 2026, "OESTE-B", "Acciones", new DateOnly(2026, 5, 15), "Carga X Factor", "Observada", 1.019876m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2026-006", 16750000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Vencimiento letra corporativa", "EVT-RF-083", 2026, "LETRA-26", "Renta Fija", new DateOnly(2026, 6, 18), "Manual", "Pendiente", 1.000999m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2026-007", 8123000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distribución fondo internacional", "EVT-FON-088", 2026, "FONDO-USD", "Fondos", new DateOnly(2026, 7, 30), "Carga X Monto", "Vigente", 0.987654m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "CAL-2026-008", 3310000m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Evento de capital con pago asociado", "EVT-ACC-095", 2026, "CENTRO-E", "Acciones", new DateOnly(2026, 8, 20), "Manual", "Observada", 1.015555m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classifications_FiscalYear",
                table: "Classifications",
                column: "FiscalYear");

            migrationBuilder.CreateIndex(
                name: "IX_Classifications_FiscalYear_Market_Instrument_PaymentDate_EventSequence",
                table: "Classifications",
                columns: UniqueIndexColumns,
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classifications_Market",
                table: "Classifications",
                column: "Market");

            migrationBuilder.CreateIndex(
                name: "IX_Classifications_PaymentDate",
                table: "Classifications",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Classifications_Status",
                table: "Classifications",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Classifications");
        }
    }
}
