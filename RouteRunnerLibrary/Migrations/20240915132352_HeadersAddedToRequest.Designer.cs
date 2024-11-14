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
    [Migration("20240915132352_HeadersAddedToRequest")]
    partial class HeadersAddedToRequest
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-preview.7.24405.3");

            modelBuilder.Entity("Header", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RequestId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("Header");
                });

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

            modelBuilder.Entity("RouteRunnerLibrary.Models.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Body")
                        .HasColumnType("TEXT");

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

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("RouteRunnerLibrary.Models.RequestInHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Body")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("HttpVerb")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RequestId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("RequestsHistory");
                });

            modelBuilder.Entity("Header", b =>
                {
                    b.HasOne("RouteRunnerLibrary.Models.Request", "Request")
                        .WithMany("Headers")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Request");
                });

            modelBuilder.Entity("RouteRunnerLibrary.Models.Folder", b =>
                {
                    b.HasOne("RouteRunnerLibrary.Models.Folder", "Parent")
                        .WithMany("SubFolders")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("RouteRunnerLibrary.Models.Request", b =>
                {
                    b.HasOne("RouteRunnerLibrary.Models.Folder", "Folder")
                        .WithMany("SavedRequests")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("RouteRunnerLibrary.Models.Folder", b =>
                {
                    b.Navigation("SavedRequests");

                    b.Navigation("SubFolders");
                });

            modelBuilder.Entity("RouteRunnerLibrary.Models.Request", b =>
                {
                    b.Navigation("Headers");
                });
#pragma warning restore 612, 618
        }
    }
}
