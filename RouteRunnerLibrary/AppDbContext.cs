﻿using Microsoft.EntityFrameworkCore;
using RouteRunnerLibrary.Models;

namespace RouteRunnerLibrary;

public class AppDbContext : DbContext
{
	public DbSet<Folder> Folders { get; set; }
	public DbSet<SavedRequest> SavedRequests { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)  
	{
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		string routeRunnerFolderPath = Path.Combine(documentsPath, "RouteRunner");

		if (!Directory.Exists(routeRunnerFolderPath))
		{
			Directory.CreateDirectory(routeRunnerFolderPath);
		}
		string dbPath = Path.Combine(routeRunnerFolderPath, "route-runner-app.db");
		optionsBuilder.UseSqlite($"Data Source={dbPath}");

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		
	}
}
