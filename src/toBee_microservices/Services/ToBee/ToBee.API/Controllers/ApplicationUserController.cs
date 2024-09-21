using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using ToBee.API.Models;
using ToBee.API.Repositories.ApplicationUserRepository;

namespace ToBee.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ApplicationUserController : ControllerBase
	{
		private readonly IApplicationUserRepository _userRepository;

		public ApplicationUserController(IApplicationUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		/// <summary>
		/// Gets user information by user ID.
		/// </summary>
		/// <remarks>Returns the user information for the specified user ID.</remarks>
		/// <returns>The user information.</returns>
		[HttpGet]
		[SwaggerOperation(
			Summary = "Gets user information by user ID.",
			Description = "Returns the user information for the specified user ID.",
			OperationId = "GetUserById",
			Tags = new[] { "ApplicationUser" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "The user information.", typeof(ApplicationUser))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
		public async Task<ActionResult<ApplicationUser>> GetUserById()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _userRepository.GetUserByIdAsync(userId);
			if (user == null)
			{
				return NotFound();
			}
			return Ok(user);
		}

		/// <summary>
		/// Uploads the user's image.
		/// </summary>
		/// <remarks>Uploads the image for the specified user ID.</remarks>
		/// <param name="file">The image file to upload.</param>
		/// <returns>No content.</returns>
		[HttpPost("upload-image")]
		[SwaggerOperation(
			Summary = "Uploads the user's image.",
			Description = "Uploads the image for the specified user ID.",
			OperationId = "UploadUserImage",
			Tags = new[] { "ApplicationUser" }
		)]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Image uploaded successfully.")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input.")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
		public async Task<IActionResult> UploadUserImage(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("Invalid file.");
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _userRepository.GetUserByIdAsync(userId);
			if (user == null)
			{
				return NotFound();
			}

			// Save the image to the file system
			var imageUrl = await SaveImageAsync(file);

			user.UserImage = imageUrl;
			await _userRepository.UpdateUserAsync(user);

			return NoContent();
		}

		/// <summary>
		/// Updates the user's information.
		/// </summary>
		/// <remarks>Updates the information for the specified user ID.</remarks>
		/// <param name="updateUser">The user information to update.</param>
		/// <returns>No content.</returns>
		[HttpPut("update-info")]
		[SwaggerOperation(
			Summary = "Updates the user's information.",
			Description = "Updates the information for the specified user ID.",
			OperationId = "UpdateUserInfo",
			Tags = new[] { "ApplicationUser" }
		)]
		[SwaggerResponse(StatusCodes.Status204NoContent, "User information updated successfully.")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input.")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
		public async Task<IActionResult> UpdateUserInfo([FromBody] ApplicationUser updateUser)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _userRepository.GetUserByIdAsync(userId);
			if (user == null)
			{
				return NotFound();
			}

			// Update user information
			user.UserName = updateUser.UserName ?? user.UserName;
			user.Email = updateUser.Email ?? user.Email;
			user.JoinDate = updateUser.JoinDate ?? user.JoinDate;

			await _userRepository.UpdateUserAsync(user);

			return NoContent();
		}

		private async Task<string> SaveImageAsync(IFormFile file)
		{
			// Ensure the directory exists
			var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			// Save the file
			var filePath = Path.Combine(directoryPath, file.FileName);
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			// Return the URL of the saved image
			return $"/images/{file.FileName}";
		}
	}
}
