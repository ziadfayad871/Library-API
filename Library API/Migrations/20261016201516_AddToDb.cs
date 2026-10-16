using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Library_API.Migrations
{
    /// <inheritdoc />
    public partial class AddToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Pages = table.Column<int>(type: "int", nullable: false),
                    PublishedYear = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookCopies",
                columns: table => new
                {
                    CopyNo = table.Column<int>(type: "int", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    ShelfLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCopies", x => x.CopyNo);
                    table.ForeignKey(
                        name: "FK_BookCopies_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "CreatedAt", "Description", "Name", "Pages", "Price", "PublishedYear" },
                values: new object[,]
                {
                    { 1, "F. Scott Fitzgerald", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A classic American novel", "The Great Gatsby", 180, 12.99m, new DateTime(1925, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Harper Lee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A novel about racial injustice in the Deep South", "To Kill a Mockingbird", 281, 14.99m, new DateTime(1960, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "George Orwell", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A dystopian novel about a totalitarian regime", "1984", 328, 9.99m, new DateTime(1949, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "Jane Austen", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A romantic novel about the manners and customs of the British gentry", "Pride and Prejudice", 279, 11.99m, new DateTime(1813, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_BookId",
                table: "BookCopies",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Name",
                table: "Books",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookCopies");

            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
