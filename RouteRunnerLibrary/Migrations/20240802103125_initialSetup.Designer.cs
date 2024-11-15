﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RouteRunnerLibrary;

#nullable disable

namespace RouteRunnerLibrary.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240802103125_initialSetup")]
    partial class initialSetup
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("RouteRunnerLibrary.Models.Folder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Folders");
                });

            modelBuilder.Entity("RouteRunnerLibrary.Models.SavedRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("FolderId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HttpVerb")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FolderId");

                    b.ToTable("SavedRequests");
                });

            modelBuilder.Entity("RouteRunnerLibrary.Models.Folder", b =>
                {
                    b.HasOne("RouteRunnerLibrary.Models.Folder", "Parent")
                        .WithMany("SubFolders")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("RouteRunnerLibrary.Models.SavedRequest", b =>
                {
                    b.HasOne("RouteRunnerLibrary.Models.Folder", "Folder")
                        .WithMany("SavedRequests")
                        .HasForeignKey("FolderId");

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("RouteRunnerLibrary.Models.Folder", b =>
                {
                    b.Navigation("SavedRequests");

                    b.Navigation("SubFolders");
                });
#pragma warning restore 612, 618
        }
    }
}
