using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuamExchange.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    private static readonly string[] UniqueClassificationIndexColumns =
    [
        "FiscalYear",
        "Market",
        "Instrument",
        "PaymentDate",
        "EventSequence",
    ];

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
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Classifications", classification => classification.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Classifications_FiscalYear",
            table: "Classifications",
            column: "FiscalYear");

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

        migrationBuilder.CreateIndex(
            name: "IX_Classifications_FiscalYear_Market_Instrument_PaymentDate_EventSequence",
            table: "Classifications",
            columns: UniqueClassificationIndexColumns,
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("Classifications");
    }
}
