using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HtmxPost.Migrations
{
    /// <inheritdoc />
    public partial class ImageField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "postImage",
                table: "Posts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "postImage",
                table: "Posts");
        }
    }
}
