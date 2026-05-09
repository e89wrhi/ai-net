using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImageGen.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "image_gen_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_model_id = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    last_generated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    Quality = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    Style = table.Column<int>(type: "integer", nullable: false),
                    TotalCost = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalTokens = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_image_gen_sessions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "image_gen_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    prompt = table.Column<string>(type: "text", nullable: false),
                    image = table.Column<string>(type: "text", nullable: false),
                    size = table.Column<int>(type: "integer", nullable: false),
                    style = table.Column<int>(type: "integer", nullable: false),
                    generated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    TokenUsed = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_image_gen_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_image_gen_results_image_gen_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "image_gen_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_image_gen_results_session_id",
                table: "image_gen_results",
                column: "session_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "image_gen_results");

            migrationBuilder.DropTable(
                name: "image_gen_sessions");
        }
    }
}
