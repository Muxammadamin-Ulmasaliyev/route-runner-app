using Microsoft.EntityFrameworkCore;
using RouteRunnerLibrary.Models;

namespace RouteRunnerLibrary;

public class AppDbContext : DbContext
{
	public DbSet<Folder> Folders { get; set; }
	public DbSet<SavedRequest> SavedRequests { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		/*string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		string routeRunnerFolderPath = Path.Combine(documentsPath, "RouteRunner");

		if (!Directory.Exists(routeRunnerFolderPath))
		{
			Directory.CreateDirectory(routeRunnerFolderPath);
		}

		string dbPath = Path.Combine(routeRunnerFolderPath, "route-runner-app.db");

		optionsBuilder.UseSqlite($"Data Source={dbPath}");*/


		string dbPath = Path.Combine(AppContext.BaseDirectory, "route-runner-app.db");
		optionsBuilder.UseSqlite($"Data Source={dbPath}");


	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Folder>()
		  .HasOne(f => f.Parent)
		  .WithMany(f => f.SubFolders)
		  .HasForeignKey(f => f.ParentId)
		  .OnDelete(DeleteBehavior.Cascade);

		// Configure one-to-many relationship between Folder and Request with cascade delete
		modelBuilder.Entity<Folder>()
			.HasMany(f => f.SavedRequests)
			.WithOne(r => r.Folder)
			.HasForeignKey(r => r.FolderId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
