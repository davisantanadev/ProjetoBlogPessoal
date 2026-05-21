using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogPessoal.Migrations
{

    public partial class PopulaUsuario : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO `Usuarios` (`Nome`, `Email`, `Senha`, `FotoURL`) VALUES ('Davi', 'davi@gmail.com', 'senha123', 'foto.png')");
        }
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DELETE FROM Usuarios");
        }
    }
}
