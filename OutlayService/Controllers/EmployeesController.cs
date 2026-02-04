using Microsoft.AspNetCore.Mvc;
using OutlayService.DTOs;
using OutlayService.Responses;
using OutlayService.Services.Interfaces;

namespace OutlayService.Controllers;

/// <summary>
/// API controller for user operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    /// <summary>
    /// Initializes a new instance of the UsersController class
    /// </summary>
    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all users
    /// </summary>
    /// <returns>List of all users</returns>
    /// <response code="200">Returns the list of users</response>
    /// <response code="500">If an error occurs while retrieving users</response>
    [HttpGet]
    public async Task<ActionResult<OutlayServiceResponse<IEnumerable<UserDto>>>> GetAllUsers()
    {
        _logger.LogInformation("Getting all users");
        var response = await _userService.GetAllUsersAsync();
        return Ok(response);
    }

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="id">The user ID</param>
    /// <returns>The user with the specified ID</returns>
    /// <response code="200">Returns the user</response>
    /// <response code="404">If the user is not found</response>
    /// <response code="500">If an error occurs</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<OutlayServiceResponse<UserDto>>> GetUserById(int id)
    {
        _logger.LogInformation($"Getting user with ID {id}");
        var response = await _userService.GetUserByIdAsync(id);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="createUserDto">The user data</param>
    /// <returns>The created user</returns>
    /// <response code="201">User created successfully</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="409">If email already exists</response>
    /// <response code="500">If an error occurs</response>
    [HttpPost]
    public async Task<ActionResult<OutlayServiceResponse<UserDto>>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"Creating new user with email {createUserDto.Email}");
        var response = await _userService.CreateUserAsync(createUserDto);

        if (!response.Success)
        {
            return Conflict(response);
        }

        return CreatedAtAction(nameof(GetUserById), new { id = response.Result?.Id }, response);
    }

    /// <summary>
    /// Updates an existing user
    /// </summary>
    /// <param name="id">The user ID</param>
    /// <param name="updateUserDto">The updated user data</param>
    /// <returns>The updated user</returns>
    /// <response code="200">User updated successfully</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="404">If the user is not found</response>
    /// <response code="409">If email already exists</response>
    /// <response code="500">If an error occurs</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<OutlayServiceResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"Updating user with ID {id}");
        var response = await _userService.UpdateUserAsync(id, updateUserDto);

        if (!response.Success)
        {
            if (response.Message.Contains("not found"))
            {
                return NotFound(response);
            }
            return Conflict(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="id">The user ID</param>
    /// <returns>Success message</returns>
    /// <response code="200">User deleted successfully</response>
    /// <response code="404">If the user is not found</response>
    /// <response code="500">If an error occurs</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult<OutlayServiceResponse<bool>>> DeleteUser(int id)
    {
        _logger.LogInformation($"Deleting user with ID {id}");
        var response = await _userService.DeleteUserAsync(id);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}
