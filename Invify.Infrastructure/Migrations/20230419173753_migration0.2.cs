using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invify.Infrastructure.Migrations
{
    public partial class migration02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Purchase_PurchaseId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_PurchaseId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "PurchaseId",
                table: "Product");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "3c54f767-53fa-476d-9578-4b4742c5089e",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "c8bf411e-00db-4743-be21-abdad764c8b3", "2a6dbfda-689b-4602-8542-41845ad5dbee" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_ProductId",
                table: "Purchase",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Product_ProductId",
                table: "Purchase",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Product_ProductId",
                table: "Purchase");

            migrationBuilder.DropIndex(
                name: "IX_Purchase_ProductId",
                table: "Purchase");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "3c54f767-53fa-476d-9578-4b4742c5089e",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "f5f231b4-79f3-48ca-a954-3fa61fb1fdf8", "aac34d69-1210-45eb-96eb-e18f21172bd9" });

            migrationBuilder.CreateIndex(
                name: "IX_Product_PurchaseId",
                table: "Product",
                column: "PurchaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Purchase_PurchaseId",
                table: "Product",
                column: "PurchaseId",
                principalTable: "Purchase",
                principalColumn: "Id");
        }
    }
}
