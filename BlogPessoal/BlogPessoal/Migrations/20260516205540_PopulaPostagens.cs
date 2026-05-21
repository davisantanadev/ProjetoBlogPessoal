using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogPessoal.Migrations
{   
    public partial class PopulaPostagens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO Postagens (Titulo, Texto, ImagemURL, Data, ResumoIA, TagIA, CategoriaIA, TemaID, UsuarioId)" +
                "VALUES (" +
                "'Primeiro Post', " +
                "'Conteúdo do primeiro post para popular a tabela de postagens.', " +
                "'imagem.png', " +
                "NOW(), " +
                "'Resumo gerado automaticamente', " +
                "'introdução', " +
                "'Geral', " +
                "NULL, " +
                "(SELECT `UsuarioId` FROM `Usuarios` WHERE `Email` = 'admin@admin.com' LIMIT 1)" +
                ");"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Postagens");
        }
    }
}
