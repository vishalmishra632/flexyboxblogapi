using BlogAPI.Core.Common;
using BlogAPI.Core.DTOs;
using BlogAPI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BlogAPI.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogPostsController> _logger;

        public BlogPostsController(IBlogService blogService, ILogger<BlogPostsController> logger)
        {
            _blogService = blogService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all blog posts
        /// </summary>
        /// <returns>A list of all blog posts</returns>
        /// <response code="200">Returns the list of blog posts</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(Result<List<BlogPostDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Result<List<BlogPostDto>>>> GetAllPosts()
        {
            _logger.LogInformation("Getting all blog posts");
            var result = await _blogService.GetAllPostsAsync();
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific blog post by id
        /// </summary>
        /// <param name="id">The ID of the blog post to retrieve</param>
        /// <returns>The requested blog post</returns>
        /// <response code="200">Returns the requested blog post</response>
        /// <response code="404">If the blog post is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Result<BlogPostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<BlogPostDto>>> GetPost(int id)
        {
            var result = await _blogService.GetPostByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new blog post
        /// </summary>
        /// <param name="dto">The blog post data</param>
        /// <returns>The created blog post</returns>
        /// <response code="201">Returns the newly created blog post</response>
        /// <response code="400">If the request is invalid</response>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Result<BlogPostDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<BlogPostDto>>> CreatePost([FromBody] CreateBlogPostDto dto)
        {
            var result = await _blogService.CreatePostAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetPost), new { id = result.Data.Id, version = "1.0" }, result);
        }

        /// <summary>
        /// Updates an existing blog post
        /// </summary>
        /// <param name="id">The ID of the blog post to update</param>
        /// <param name="dto">The updated blog post data</param>
        /// <returns>The updated blog post</returns>
        /// <response code="200">Returns the updated blog post</response>
        /// <response code="404">If the blog post is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<BlogPostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<BlogPostDto>>> UpdatePost(int id, [FromBody] UpdateBlogPostDto dto)
        {
            var result = await _blogService.UpdatePostAsync(id, dto);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a specific blog post
        /// </summary>
        /// <param name="id">The ID of the blog post to delete</param>
        /// <returns>A success indicator</returns>
        /// <response code="200">If the blog post was successfully deleted</response>
        /// <response code="404">If the blog post is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<bool>>> DeletePost(int id)
        {
            var result = await _blogService.DeletePostAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
    }
}