﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WordSnapWeb.Models;

public partial class WordSnapDbContext : DbContext
{
    public WordSnapDbContext()
    {
    }

    public WordSnapDbContext(DbContextOptions<WordSnapDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Cardset> Cardsets { get; set; }

    public virtual DbSet<Progress> Progresses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userscardset> Userscardsets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cards_pkey");

            entity.ToTable("cards");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CardsetRef).HasColumnName("cardset_ref");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.WordEn)
                .HasMaxLength(100)
                .HasColumnName("word_en");
            entity.Property(e => e.WordUa)
                .HasMaxLength(100)
                .HasColumnName("word_ua");

            entity.HasOne(d => d.CardsetRefNavigation).WithMany(p => p.Cards)
                .HasForeignKey(d => d.CardsetRef)
                .HasConstraintName("cards_cardset_ref_fkey");
        });

        modelBuilder.Entity<Cardset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cardsets_pkey");

            entity.ToTable("cardsets");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsPublic)
                .HasDefaultValue(false)
                .HasColumnName("is_public");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UserRef).HasColumnName("user_ref");

            entity.HasOne(d => d.UserRefNavigation).WithMany(p => p.Cardsets)
                .HasForeignKey(d => d.UserRef)
                .HasConstraintName("cardsets_user_ref_fkey");
        });

        modelBuilder.Entity<Progress>(entity =>
        {
            entity.HasKey(e => new { e.UserRef, e.CardsetRef }).HasName("progress_pkey");

            entity.ToTable("progress");

            entity.Property(e => e.UserRef).HasColumnName("user_ref");
            entity.Property(e => e.CardsetRef).HasColumnName("cardset_ref");
            entity.Property(e => e.LastAccessed)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_accessed");
            entity.Property(e => e.SuccessRate)
                .HasDefaultValueSql("0.0")
                .HasColumnName("success_rate");

            entity.HasOne(d => d.CardsetRefNavigation).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.CardsetRef)
                .HasConstraintName("progress_cardset_ref_fkey");

            entity.HasOne(d => d.UserRefNavigation).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.UserRef)
                .HasConstraintName("progress_user_ref_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.IsVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_verified");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(24)
                .IsFixedLength()
                .HasColumnName("password_salt");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Userscardset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userscardsets_pkey");

            entity.ToTable("userscardsets");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CardsetRef).HasColumnName("cardset_ref");
            entity.Property(e => e.UserRef).HasColumnName("user_ref");

            entity.HasOne(d => d.CardsetRefNavigation).WithMany(p => p.Userscardsets)
                .HasForeignKey(d => d.CardsetRef)
                .HasConstraintName("userscardsets_cardset_ref_fkey");

            entity.HasOne(d => d.UserRefNavigation).WithMany(p => p.Userscardsets)
                .HasForeignKey(d => d.UserRef)
                .HasConstraintName("userscardsets_user_ref_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
