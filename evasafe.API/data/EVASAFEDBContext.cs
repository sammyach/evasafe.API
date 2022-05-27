﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace evasafe.API.Data
{
    public partial class EVASAFEDBContext : DbContext
    {
        public EVASAFEDBContext()
        {
        }

        public EVASAFEDBContext(DbContextOptions<EVASAFEDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EvAccountActionsType> EvAccountActionsTypes { get; set; } = null!;
        public virtual DbSet<EvAppUser> EvAppUsers { get; set; } = null!;
        public virtual DbSet<EvUserAccountActionToken> EvUserAccountActionTokens { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) { 

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EvAccountActionsType>(entity =>
            {
                entity.ToTable("ev_account_actions_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionType)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("action_type");
            });

            modelBuilder.Entity<EvAppUser>(entity =>
            {
                entity.HasKey(e => e.Email)
                    .HasName("PK_app_users");

                entity.ToTable("ev_app_users");

                entity.Property(e => e.Email)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("date_created");

                entity.Property(e => e.DateDeleted)
                    .HasColumnType("datetime")
                    .HasColumnName("date_deleted");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.Enabled).HasColumnName("enabled");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.HashString)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("hash_string");

                entity.Property(e => e.JobTitle)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("job_title");

                entity.Property(e => e.LastLoginDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_login_date");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date");

                entity.Property(e => e.LastName)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("last_name");

                entity.Property(e => e.Nacl)
                    .HasMaxLength(256)
                    .HasColumnName("nacl");

                entity.Property(e => e.Organization)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("organization");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(256)
                    .HasColumnName("password_hash");

                entity.Property(e => e.Phone)
                    .HasMaxLength(13)
                    .IsUnicode(false)
                    .HasColumnName("phone");
            });

            modelBuilder.Entity<EvUserAccountActionToken>(entity =>
            {
                entity.ToTable("ev_user_account_action_tokens");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Action).HasColumnName("action");

                entity.Property(e => e.Token)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("token");

                entity.Property(e => e.TokenDate)
                    .HasColumnType("datetime")
                    .HasColumnName("token_date");

                entity.Property(e => e.User)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("user");

                entity.HasOne(d => d.ActionNavigation)
                    .WithMany(p => p.EvUserAccountActionTokens)
                    .HasForeignKey(d => d.Action)
                    .HasConstraintName("FK__user_acco__actio__32E0915F");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.EvUserAccountActionTokens)
                    .HasForeignKey(d => d.User)
                    .HasConstraintName("FK__user_accou__user__31EC6D26");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
