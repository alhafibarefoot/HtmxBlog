using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HtmxPost.Migrations
{
    /// <inheritdoc />
    public partial class SeedingPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Posts",
                columns: ["Id", "Title", "Content"],
                values: new object[,]
                {
                    { 1, "SPA", "Single Page Application", },
                    { 2, "HTMX", "Hyper Media Content" }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
