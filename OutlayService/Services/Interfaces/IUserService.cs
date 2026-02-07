using OutlayService.DTOs;
using OutlayService.Responses;

namespace OutlayService.Services.Interfaces;

/// <summary>
/// Interface for user service operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets all users
    /// </summary>
    Task<OutlayServiceResponse<IEnumerable<UserDto>>> GetAllUsersAsync();

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    Task<OutlayServiceResponse<UserDto>> GetUserByIdAsync(int id);

    /// <summary>
    /// Creates a new user
    /// </summary>
    Task<OutlayServiceResponse<UserDto>> CreateUserAsync(CreateUserDto createUserDto);

    /// <summary>
    /// Updates an existing user
    /// </summary>
    Task<OutlayServiceResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto updateUserDto);

    /// <summary>
    /// Deletes a user
    /// </summary>
    Task<OutlayServiceResponse<bool>> DeleteUserAsync(int id);

    /// <summary>
    /// Checks if a user exists by ID
    /// </summary>
    Task<bool> UserExistsAsync(int id);

    /// <summary>
    /// Checks if an email already exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
}
