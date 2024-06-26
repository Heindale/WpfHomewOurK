﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WpfHomewOurK;

#nullable disable

namespace WpfHomewOurK.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240516193553_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.5");

            modelBuilder.Entity("HomewOurK.Domain.Entities.Attachment", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int?>("SubjectGroupId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SubjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id", "GroupId");

                    b.HasIndex("GroupId");

                    b.HasIndex("SubjectId", "SubjectGroupId");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("HomewOurK.Domain.Entities.Group", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Grade")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GroupType")
                        .HasMaxLength(25)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("TEXT");

                    b.Property<string>("UniqGroupName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("HomewOurK.Domain.Entities.Homework", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SubjectId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CompletedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Deadline")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<bool>("Done")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Importance")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SubjectGroupId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id", "GroupId", "SubjectId");

                    b.HasIndex("GroupId");

                    b.HasIndex("SubjectId", "SubjectGroupId");

                    b.ToTable("Homeworks");
                });

            modelBuilder.Entity("HomewOurK.Domain.Entities.Subject", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("HomewOurK.Domain.Entities.Teacher", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("Teachers");
                });

            modelBuilder.Entity("SubjectTeacher", b =>
                {
                    b.Property<int>("SubjectsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SubjectsGroupId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeachersId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeachersGroupId")
                        .HasColumnType("INTEGER");

                    b.HasKey("SubjectsId", "SubjectsGroupId", "TeachersId", "TeachersGroupId");

                    b.HasIndex("TeachersId", "TeachersGroupId");

                    b.ToTable("SubjectTeacher");
                });

            modelBuilder.Entity("HomewOurK.Domain.Entities.Attachment", b =>
                {
                    b.HasOne("HomewOurK.Domain.Entities.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomewOurK.Domain.Entities.Subject", "Subject")
                        .WithMany()
                        .HasForeignKey("SubjectId", "SubjectGroupId");

                    b.Navigation("Group");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("HomewOurK.Domain.Entities.Homework", b =>
                {
                    b.HasOne("HomewOurK.Domain.Entities.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomewOurK.Domain.Entities.Subject", "Subject")
                        .WithMany()
                        .HasForeignKey("SubjectId", "SubjectGroupId");

                    b.Navigation("Group");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("HomewOurK.Domain.Entities.Subject", b =>
                {
                    b.HasOne("HomewOurK.Domain.Entities.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("HomewOurK.Domain.Entities.Teacher", b =>
                {
                    b.HasOne("HomewOurK.Domain.Entities.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("SubjectTeacher", b =>
                {
                    b.HasOne("HomewOurK.Domain.Entities.Subject", null)
                        .WithMany()
                        .HasForeignKey("SubjectsId", "SubjectsGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomewOurK.Domain.Entities.Teacher", null)
                        .WithMany()
                        .HasForeignKey("TeachersId", "TeachersGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
