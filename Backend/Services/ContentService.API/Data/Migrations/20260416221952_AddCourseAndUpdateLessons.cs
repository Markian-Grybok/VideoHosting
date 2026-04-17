using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentService.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseAndUpdateLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "course_id",
                table: "lessons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "video_file_id",
                table: "lessons",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courses", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_lessons_course_id_order",
                table: "lessons",
                columns: new[] { "course_id", "order" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_lessons_courses_course_id",
                table: "lessons",
                column: "course_id",
                principalTable: "courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lessons_courses_course_id",
                table: "lessons");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropIndex(
                name: "IX_lessons_course_id_order",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "course_id",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "order",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "video_file_id",
                table: "lessons");
        }
    }
}
