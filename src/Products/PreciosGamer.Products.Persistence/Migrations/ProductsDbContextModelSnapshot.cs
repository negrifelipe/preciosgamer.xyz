﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;
using PreciosGamer.Products.Persistence;

#nullable disable

namespace PreciosGamer.Products.Persistence.Migrations
{
    [DbContext(typeof(ProductsDbContext))]
    partial class ProductsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PreciosGamer.Products.Persistence.Entities.ProductEntity", b =>
                {
                    b.Property<string>("SKU")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("StoreId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("CreateDate")
                        .HasColumnType("date");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(600)
                        .HasColumnType("character varying(600)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<decimal>("Price")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)")
                        .HasDefaultValue(0m);

                    b.Property<NpgsqlTsVector>("SearchVector")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("tsvector")
                        .HasAnnotation("Npgsql:TsVectorConfig", "spanish")
                        .HasAnnotation("Npgsql:TsVectorProperties", new[] { "SKU", "Name" });

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(600)
                        .HasColumnType("character varying(600)");

                    b.HasKey("SKU", "StoreId");

                    b.HasIndex("SearchVector");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("SearchVector"), "GIN");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("PreciosGamer.Products.Persistence.Entities.ProductPriceEntity", b =>
                {
                    b.Property<string>("SKU")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("StoreId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("CreateDate")
                        .HasColumnType("date");

                    b.Property<decimal>("Price")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)")
                        .HasDefaultValue(0m);

                    b.HasKey("SKU", "StoreId", "CreateDate");

                    b.ToTable("ProductPrices", (string)null);
                });

            modelBuilder.Entity("PreciosGamer.Products.Persistence.Entities.ProductPriceEntity", b =>
                {
                    b.HasOne("PreciosGamer.Products.Persistence.Entities.ProductEntity", null)
                        .WithMany()
                        .HasForeignKey("SKU", "StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
