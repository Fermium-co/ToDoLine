using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoLine.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ToDoGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToDoGroups_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToDoGroupOptionsList",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Theme = table.Column<string>(nullable: true),
                    SortedBy = table.Column<int>(nullable: false),
                    HideCompletedToDoItems = table.Column<bool>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ToDoGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoGroupOptionsList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToDoGroupOptionsList_ToDoGroups_ToDoGroupId",
                        column: x => x.ToDoGroupId,
                        principalTable: "ToDoGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToDoGroupOptionsList_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToDoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    IsImportant = table.Column<bool>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    DueDate = table.Column<DateTimeOffset>(nullable: true),
                    CompletedById = table.Column<Guid>(nullable: true),
                    CreatedById = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false),
                    ToDoGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToDoItems_Users_CompletedById",
                        column: x => x.CompletedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToDoItems_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToDoItems_ToDoGroups_ToDoGroupId",
                        column: x => x.ToDoGroupId,
                        principalTable: "ToDoGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToDoItemOptionsList",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RemindOn = table.Column<DateTimeOffset>(nullable: true),
                    ShowInMyDayOn = table.Column<DateTimeOffset>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    ToDoItemId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoItemOptionsList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToDoItemOptionsList_ToDoItems_ToDoItemId",
                        column: x => x.ToDoItemId,
                        principalTable: "ToDoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToDoItemOptionsList_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToDoItemSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    ToDoItemId = table.Column<Guid>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoItemSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToDoItemSteps_ToDoItems_ToDoItemId",
                        column: x => x.ToDoItemId,
                        principalTable: "ToDoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoGroupOptionsList_ToDoGroupId",
                table: "ToDoGroupOptionsList",
                column: "ToDoGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoGroupOptionsList_UserId_ToDoGroupId",
                table: "ToDoGroupOptionsList",
                columns: new[] { "UserId", "ToDoGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToDoGroups_CreatedById",
                table: "ToDoGroups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItemOptionsList_ToDoItemId",
                table: "ToDoItemOptionsList",
                column: "ToDoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItemOptionsList_UserId_ToDoItemId",
                table: "ToDoItemOptionsList",
                columns: new[] { "UserId", "ToDoItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItems_CompletedById",
                table: "ToDoItems",
                column: "CompletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItems_CreatedById",
                table: "ToDoItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItems_ToDoGroupId",
                table: "ToDoItems",
                column: "ToDoGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItemSteps_ToDoItemId",
                table: "ToDoItemSteps",
                column: "ToDoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToDoGroupOptionsList");

            migrationBuilder.DropTable(
                name: "ToDoItemOptionsList");

            migrationBuilder.DropTable(
                name: "ToDoItemSteps");

            migrationBuilder.DropTable(
                name: "ToDoItems");

            migrationBuilder.DropTable(
                name: "ToDoGroups");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
