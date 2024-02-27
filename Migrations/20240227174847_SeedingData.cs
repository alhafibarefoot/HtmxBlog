using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HtmxPost.Migrations
{
    /// <inheritdoc />
    public partial class SeedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Posts",
                columns: ["Id", "Title", "Content","postImage"],
                values: new object[,]
                {
                    { 1, "SPA", "Single Page Application","AlhafiLogo1.jpg" },
                    { 2, "HTMX", "Hyper Media Content","AlhafiLogo2.jpg" }
                }
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
