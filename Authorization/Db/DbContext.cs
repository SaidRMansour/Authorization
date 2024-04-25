using System;
using Authorization.Models;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Db
{
    public class NewsSiteContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public NewsSiteContext(DbContextOptions<NewsSiteContext> options) : base(options)
        {
        }

        public void Seed()
        {
            if (!Users.Any())
            {
                Users.AddRange(
                    new User { Username = "user1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("user1password"), Role = "Editor" },
                    new User { Username = "user2", PasswordHash = BCrypt.Net.BCrypt.HashPassword("user2password"), Role = "Journalist" },
                    new User { Username = "user3", PasswordHash = BCrypt.Net.BCrypt.HashPassword("user3password"), Role = "Subscriber" }
                );
                SaveChanges();
            }

            if (!Articles.Any())
            {
                Articles.AddRange(
                    new Article { Title = "First Article", Content = "Content of the first article", AuthorId = Users.First().UserId },
                    new Article { Title = "Second Article", Content = "Content of the second article", AuthorId = Users.First().UserId }
                );
                SaveChanges();
            }

            if (!Comments.Any())
            {
                var lastUserId = Users.OrderBy(u => u.UserId).Last().UserId;
                var firstArticleId = Articles.OrderBy(a => a.ArticleId).First().ArticleId;

                Comments.AddRange(
                    new Comment { Content = "First comment", ArticleId = firstArticleId, UserId = lastUserId },
                    new Comment { Content = "Second comment", ArticleId = firstArticleId, UserId = lastUserId }
                );
                SaveChanges();
               
            }
        }


    }
}

