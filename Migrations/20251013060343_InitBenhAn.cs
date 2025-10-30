using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APP.Migrations
{
    public partial class InitBenhAn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BenhAn",
                columns: table => new
                {
                    makcb = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    maubenhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sobenhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bacsy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ngaylam = table.Column<DateTime>(type: "datetime2", nullable: false),
                    manv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    daky = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    manvtongket = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ngaytongket = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhAn", x => x.makcb);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BenhAn");
        }
    }
}
