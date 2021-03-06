﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NewsPublish.Service;

namespace NewsPublish.Service.Migrations
{
    [DbContext(typeof(Db))]
    partial class DbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("NewsPublish.Model.Entity.Banner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddTime");

                    b.Property<string>("Image");

                    b.Property<string>("Remark");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Banner");
                });

            modelBuilder.Entity("NewsPublish.Model.Entity.News", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Contents");

                    b.Property<string>("Image");

                    b.Property<int>("NewsClassifyId");

                    b.Property<string>("PublishDate");

                    b.Property<string>("Remark");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("NewsClassifyId");

                    b.ToTable("News");
                });

            modelBuilder.Entity("NewsPublish.Model.Entity.NewsClassify", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Remark");

                    b.Property<int>("Sort");

                    b.HasKey("Id");

                    b.ToTable("NewsClassify");
                });

            modelBuilder.Entity("NewsPublish.Model.Entity.NewsComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddTime");

                    b.Property<string>("Contents");

                    b.Property<int>("NewsId");

                    b.Property<string>("Remark");

                    b.HasKey("Id");

                    b.HasIndex("NewsId");

                    b.ToTable("NewsComment");
                });

            modelBuilder.Entity("NewsPublish.Model.Entity.News", b =>
                {
                    b.HasOne("NewsPublish.Model.Entity.NewsClassify", "NewsClassify")
                        .WithMany("News")
                        .HasForeignKey("NewsClassifyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NewsPublish.Model.Entity.NewsComment", b =>
                {
                    b.HasOne("NewsPublish.Model.Entity.News", "News")
                        .WithMany("NewsComment")
                        .HasForeignKey("NewsId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
