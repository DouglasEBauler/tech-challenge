using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TechChallenge.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EMPLOYEE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FIRST_NAME = table.Column<string>(type: "text", nullable: false),
                    LAST_NAME = table.Column<string>(type: "text", nullable: false),
                    EMAIL = table.Column<string>(type: "text", nullable: false),
                    DOCUMENT_NUMBER = table.Column<string>(type: "text", nullable: false),
                    DOCUMENT_NUMBER_INDEX = table.Column<string>(type: "text", nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "text", nullable: false),
                    PASSWORD_SALT = table.Column<string>(type: "text", nullable: false),
                    BIRTH_DATE = table.Column<DateTime>(type: "DATE", nullable: false),
                    MANAGER_ID = table.Column<int>(type: "integer", nullable: true),
                    ROLE = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_EMPLOYEE_MANAGER_ID",
                        column: x => x.MANAGER_ID,
                        principalTable: "EMPLOYEE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_PHONE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NUMBER = table.Column<string>(type: "text", nullable: false),
                    TYPE = table.Column<int>(type: "integer", nullable: false),
                    EMPLOYEE_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_PHONE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_PHONE_EMPLOYEE_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalTable: "EMPLOYEE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_MANAGER_ID",
                table: "EMPLOYEE",
                column: "MANAGER_ID");

            migrationBuilder.CreateIndex(
                name: "UX_EMPLOYEE_DOCUMENT_NUMBER_INDEX",
                table: "EMPLOYEE",
                column: "DOCUMENT_NUMBER_INDEX",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_EMPLOYEE_EMAIL_INDEX",
                table: "EMPLOYEE",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_PHONE_EMPLOYEE_ID",
                table: "EMPLOYEE_PHONE",
                column: "EMPLOYEE_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EMPLOYEE_PHONE");

            migrationBuilder.DropTable(
                name: "EMPLOYEE");
        }
    }
}
