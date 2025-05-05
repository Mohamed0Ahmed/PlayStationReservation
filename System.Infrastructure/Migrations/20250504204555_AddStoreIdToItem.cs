using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace System.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreIdToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_MenuCategories_MenuCategoryId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_MenuCategoryId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_Name_MenuCategoryId",
                table: "MenuItems");

            migrationBuilder.RenameColumn(
                name: "MenuCategoryId",
                table: "MenuItems",
                newName: "StoreId");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "MenuItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CategoryId",
                table: "MenuItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_Name_CategoryId",
                table: "MenuItems",
                columns: new[] { "Name", "CategoryId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_MenuCategories_CategoryId",
                table: "MenuItems",
                column: "CategoryId",
                principalTable: "MenuCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_MenuCategories_CategoryId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_CategoryId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_Name_CategoryId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "MenuItems");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "MenuItems",
                newName: "MenuCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_MenuCategoryId",
                table: "MenuItems",
                column: "MenuCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_Name_MenuCategoryId",
                table: "MenuItems",
                columns: new[] { "Name", "MenuCategoryId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_MenuCategories_MenuCategoryId",
                table: "MenuItems",
                column: "MenuCategoryId",
                principalTable: "MenuCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
