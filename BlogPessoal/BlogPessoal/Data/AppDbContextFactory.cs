using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BlogPessoal.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseMySql(
            "Server=localhost;Database=BlogPessoal;Uid=root;Pwd=Davisantanadev#15",
            ServerVersion.AutoDetect("Server=localhost;Database=BlogPessoal;Uid=root;Pwd=Davisantanadev#15")
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}