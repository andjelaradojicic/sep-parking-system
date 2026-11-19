using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Psp.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "merchants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    merchant_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    api_key_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    success_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    failed_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    error_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_merchants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_methods",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_methods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "merchant_payment_methods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    merchant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_method_id = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_merchant_payment_methods", x => x.id);
                    table.ForeignKey(
                        name: "fk_merchant_payment_methods_merchants_merchant_id",
                        column: x => x.merchant_id,
                        principalTable: "merchants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_merchant_payment_methods_payment_methods_payment_method_id",
                        column: x => x.payment_method_id,
                        principalTable: "payment_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "payment_methods",
                columns: new[] { "id", "code", "is_available", "name" },
                values: new object[,]
                {
                    { 1, "CARD", true, "Platna kartica" },
                    { 2, "QR", true, "IPS QR kod" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_merchant_payment_methods_merchant_id_payment_method_id",
                table: "merchant_payment_methods",
                columns: new[] { "merchant_id", "payment_method_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_merchant_payment_methods_payment_method_id",
                table: "merchant_payment_methods",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "ix_merchants_email",
                table: "merchants",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_merchants_merchant_id",
                table: "merchants",
                column: "merchant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_payment_methods_code",
                table: "payment_methods",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "merchant_payment_methods");

            migrationBuilder.DropTable(
                name: "merchants");

            migrationBuilder.DropTable(
                name: "payment_methods");
        }
    }
}
