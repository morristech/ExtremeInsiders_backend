﻿using System.Collections.Generic;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Data
{
  public class ApplicationContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<SocialAccount> SocialAccounts { get; set; }
    public DbSet<SocialAccountProvider> SocialAccountProviders { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Culture> Cultures { get; set; }
    
    /* Entities */
    public DbSet<EntityBase> EntitiesBase { get; set; }
    public DbSet<EntityLikeable> EntitiesLikeable { get; set; }
    
    public DbSet<EntitySaleable> EntitiesSaleable { get; set; }
    public DbSet<EntitySaleablePrice> EntitySaleablePrices { get; set; }
    
    /* Sports */
    public DbSet<Sport> Sports { get; set; }
    public DbSet<SportTranslation> SportsTranslations { get; set; }
    
    /* Playlists */
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistTranslation> PlaylistsTranslations { get; set; }
    
    /* Videos and Movies */
    public DbSet<Video> Videos { get; set; }
    public DbSet<VideoTranslation> VideoTranslations { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieTranslation> MovieTranslations { get; set; }
    
    
    public DbSet<Like> Likes { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    
    public DbSet<ConfirmationCode> ConfirmationCodes { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionsPlans { get; set; }
    public DbSet<SubscriptionPlanTranslation> SubscriptionsPlansTranslations { get; set; }
    
    public DbSet<Payment> Payments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<SocialAccountProvider>().HasData(SocialAccountProvider.All);
      modelBuilder.Entity<Role>().HasData(Role.User, Role.Admin);
      modelBuilder.Entity<Culture>().HasData(Culture.Russian, Culture.English);
      
      var sport = new Sport {Id = 1 };
      var playlist = new Playlist { Id = 2, SportId = sport.Id };
      var video = new Video { Id = 3, PlaylistId = playlist.Id};
      var movie = new Movie { Id = 4, SportId = sport.Id};
      
      modelBuilder.Entity<Sport>().HasData(sport);
      modelBuilder.Entity<Playlist>().HasData(playlist);
      modelBuilder.Entity<Video>().HasData(video);
      modelBuilder.Entity<Movie>().HasData(movie);

      modelBuilder.Entity<SportTranslation>().HasKey(x => new {x.CultureId, x.BaseEntityId});
      modelBuilder.Entity<PlaylistTranslation>().HasKey(x => new {x.CultureId, x.BaseEntityId});
      modelBuilder.Entity<MovieTranslation>().HasKey(x => new {x.CultureId, x.BaseEntityId});
      modelBuilder.Entity<VideoTranslation>().HasKey(x => new {x.CultureId, x.BaseEntityId});
      
      modelBuilder.Entity<Like>().HasKey(x => new {x.UserId, x.EntityId});
      modelBuilder.Entity<EntitySaleablePrice>().HasKey(x => new {x.CurrencyId, x.EntityId});
      
      modelBuilder.Entity<Currency>().HasData(Currency.All);
    }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
      //Database.EnsureCreated();
    }
  }
}