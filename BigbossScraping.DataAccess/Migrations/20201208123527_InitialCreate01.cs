using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BigbossScraping.DataAccess.Migrations
{
    public partial class InitialCreate01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "BigBoss");

            migrationBuilder.CreateTable(
                name: "ChannelCategory",
                schema: "BigBoss",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Url = table.Column<string>(maxLength: 1000, nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    LastModified = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramMetadata",
                schema: "BigBoss",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProgramCategoryId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 250, nullable: true),
                    Url = table.Column<string>(maxLength: 1000, nullable: true),
                    Image = table.Column<string>(maxLength: 1000, nullable: true),
                    ImageAlternative = table.Column<string>(maxLength: 1000, nullable: true),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    LastModified = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramMetadata_ChannelCategory_ProgramCategoryId",
                        column: x => x.ProgramCategoryId,
                        principalSchema: "BigBoss",
                        principalTable: "ChannelCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramDetails",
                schema: "BigBoss",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 250, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    VideoUrl = table.Column<string>(maxLength: 750, nullable: true),
                    VideoBanner = table.Column<string>(maxLength: 750, nullable: true),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    LastModified = table.Column<DateTimeOffset>(nullable: false),
                    ProgramId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramDetails_ProgramMetadata_ProgramId",
                        column: x => x.ProgramId,
                        principalSchema: "BigBoss",
                        principalTable: "ProgramMetadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramDetails_ProgramId",
                schema: "BigBoss",
                table: "ProgramDetails",
                column: "ProgramId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramMetadata_ProgramCategoryId",
                schema: "BigBoss",
                table: "ProgramMetadata",
                column: "ProgramCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramDetails",
                schema: "BigBoss");

            migrationBuilder.DropTable(
                name: "ProgramMetadata",
                schema: "BigBoss");

            migrationBuilder.DropTable(
                name: "ChannelCategory",
                schema: "BigBoss");
        }
    }
}
