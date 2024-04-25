using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization.Db;
using Authorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Authorization.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly NewsSiteContext _context;

        public ArticlesController(NewsSiteContext context)
        {
            _context = context;
        }

        private int GetUserIdFromClaims()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }
            }
            return -1;
        }


        [Authorize(Policy = "CanReadArticle")]
        [HttpGet]
        public IActionResult GetArticles()
        {
            var articles = _context.Articles.ToList();
            return Ok(articles);
        }

        
        [Authorize(Policy = "CanEditArticle")]
        [HttpPut("edit/{articleId}")]
        public IActionResult EditArticle(int articleId, [FromBody] Article article)
        {
            var existingArticle = _context.Articles.FirstOrDefault(a => a.ArticleId == articleId);
            if (existingArticle == null)
            {
                return NotFound();
            }
            if (existingArticle.AuthorId != GetUserIdFromClaims() && !User.IsInRole("Editor"))
            {
                return Forbid();
            }

            existingArticle.Title = article.Title;
            existingArticle.Content = article.Content;
            _context.SaveChanges();
            return Ok("Article updated successfully.");
        }

        [Authorize(Policy = "CanCreateArticle")]
        [HttpPost("create")]
        public IActionResult CreateArticle([FromBody] Article article)
        {
            int authorId = GetUserIdFromClaims();
            if (authorId == -1)
            {
                return Unauthorized("No valid user identified");
            }

            var newArticle = new Article
            {
                Title = article.Title,
                Content = article.Content,
                AuthorId = authorId,
                CreatedAt = DateTime.Now
            };
            _context.Articles.Add(newArticle);
            _context.SaveChanges();
            return Ok(newArticle);
        }

        [Authorize(Policy = "CanDeleteArticle")]
        [HttpDelete("delete/{articleId}")]
        public IActionResult DeleteArticle(int articleId)
        {
            var findingArticle = _context.Articles.FirstOrDefault(a => a.ArticleId == articleId);
            if (findingArticle == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("Editor"))
            {
                return Forbid();
            }

            _context.Articles.Remove(findingArticle);
            _context.SaveChanges();
            return Ok("Article deleted successfully.");
        }

        [Authorize(Policy = "CanReadArticle")]
        [HttpGet("comments")]
        public IActionResult GetComment()
        {
            var comments = _context.Comments.ToList();
            return Ok(comments);
        }

        [Authorize(Policy = "CanComment")]
        [HttpPost("createComment/{articleId}")]
        public IActionResult CreateComment(int articleId, [FromBody] Comment comment)
        {

            var findingArticle = _context.Articles.FirstOrDefault(a => a.ArticleId == articleId);
            if (findingArticle == null)
            {
                return NotFound();
            }

            int userId = GetUserIdFromClaims();
            if (userId == -1)
            {
                return Unauthorized("No valid user identified");
            }

           
            var newComment = new Comment
            {
                Content = comment.Content,
                ArticleId = findingArticle.ArticleId,
                UserId = userId
            };

            _context.Comments.Add(newComment);
            _context.SaveChanges();
            return Ok(newComment);
        }

        

        [Authorize(Policy = "CanEditComment")]
        [HttpPut("editComment/{commentId}")]
        public IActionResult EditComment(int commentId, [FromBody] Comment comment)
        {
            var exisitingComment = _context.Comments.FirstOrDefault(a => a.CommentId == commentId);
            if (exisitingComment == null)
            {
                return NotFound();
            }
            if (exisitingComment.UserId != GetUserIdFromClaims() && !User.IsInRole("Editor"))
            {
                return Forbid();
            }

            exisitingComment.Content = comment.Content;
            _context.SaveChanges();
            return Ok("comment updated successfully.");
        }

        [Authorize(Policy = "CanDeleteComment")]
        [HttpDelete("deleteComment/{commentId}")]
        public IActionResult DeleteComment(int commentId)
        {
            var findingComment = _context.Comments.FirstOrDefault(a => a.CommentId == commentId);
            if (findingComment == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("Editor"))
            {
                return Forbid();
            }

            _context.Comments.Remove(findingComment);
            _context.SaveChanges();
            return Ok("Comment deleted successfully.");
        }
    }
}

