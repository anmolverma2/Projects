using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FM.Services.CoupenAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedCouponsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MinAmount",
                table: "Coupens",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Coupens",
                columns: new[] { "CouponId", "CoupenCode", "DiscountAmount", "MinAmount" },
                values: new object[,]
                {
                    { 1, "10OFF", 10.0, 20 },
                    { 2, "20OFF", 20.0, 40 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupens",
                keyColumn: "CouponId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Coupens",
                keyColumn: "CouponId",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "MinAmount",
                table: "Coupens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
