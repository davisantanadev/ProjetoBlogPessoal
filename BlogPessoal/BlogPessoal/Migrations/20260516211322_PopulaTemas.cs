using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogPessoal.Migrations
{
    public partial class PopulaTemas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO `Temas` (`Descricao`) VALUES ('Geral');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Temas");
        }
    }
}
