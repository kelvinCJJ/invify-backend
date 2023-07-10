using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invify.Infrastructure.Migrations
{
    public partial class updateProductNullinPurchase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "3c54f767-53fa-476d-9578-4b4742c5089e",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "c5d7e836-3ca4-4d8b-a0e2-aa8fb43f0124", "430bdd5c-8eab-4480-8d58-c86d808f7017" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "3c54f767-53fa-476d-9578-4b4742c5089e",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "21f9e016-4c6f-4ae8-bf3a-e137f8ff11c7", "83ebe167-dd3b-4ab3-8430-184bd6ced133" });
        }
    }
}
