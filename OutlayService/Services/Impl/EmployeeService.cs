using Microsoft.EntityFrameworkCore;
using OutlayService.Data;
using OutlayService.DTOs;
using OutlayService.Models;
using OutlayService.Responses;
using OutlayService.Services.Interfaces;

namespace OutlayService.Services.Impl;

/// <summary>
/// Implementation of the user service
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Initializes a new instance of the UserService class
    /// </summary>
    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets all users
    /// </summary>
    public async Task<OutlayServiceResponse<IEnumerable<UserDto>>> GetAllUsersAsync()
    {
        try
        {
            var users = await _context.Users
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedOn)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    CreatedOn = u.CreatedOn,
                    UpdatedOn = u.UpdatedOn
                })
                .ToListAsync();

            return OutlayServiceResponse<IEnumerable<UserDto>>
                .SuccessResponse(users, $"Retrieved {users.Count} users successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return OutlayServiceResponse<IEnumerable<UserDto>>
                .FailureResponse("An error occurred while retrieving users");
        }
    }

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    public async Task<OutlayServiceResponse<UserDto>> GetUserByIdAsync(int id)
    {
        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return OutlayServiceResponse<UserDto>
                    .FailureResponse($"User with ID {id} not found");
            }

            var dto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedOn = user.CreatedOn,
                UpdatedOn = user.UpdatedOn
            };

            return OutlayServiceResponse<UserDto>
                .SuccessResponse(dto, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving user with ID {id}");
            return OutlayServiceResponse<UserDto>
                .FailureResponse("An error occurred while retrieving the user");
        }
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    public async Task<OutlayServiceResponse<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        try
        {
            // Check if email already exists
            if (await EmailExistsAsync(createUserDto.Email))
            {
                return OutlayServiceResponse<UserDto>
                    .FailureResponse($"A user with email '{createUserDto.Email}' already exists");
            }

            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User created successfully with ID {user.Id}");

            var dto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedOn = user.CreatedOn,
                UpdatedOn = user.UpdatedOn
            };

            return OutlayServiceResponse<UserDto>
                .SuccessResponse(dto, "User created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return OutlayServiceResponse<UserDto>
                .FailureResponse("An error occurred while creating the user");
        }
    }

    /// <summary>
    /// Updates an existing user
    /// </summary>
    public async Task<OutlayServiceResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return OutlayServiceResponse<UserDto>
                    .FailureResponse($"User with ID {id} not found");
            }

            // Check if email is being changed and if the new email already exists
            if (!user.Email.Equals(updateUserDto.Email, StringComparison.OrdinalIgnoreCase) &&
                await EmailExistsAsync(updateUserDto.Email, id))
            {
                return OutlayServiceResponse<UserDto>
                    .FailureResponse($"A user with email '{updateUserDto.Email}' already exists");
            }

            user.Name = updateUserDto.Name;
            user.Email = updateUserDto.Email;
            user.UpdatedOn = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User with ID {id} updated successfully");

            var dto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedOn = user.CreatedOn,
                UpdatedOn = user.UpdatedOn
            };

            return OutlayServiceResponse<UserDto>
                .SuccessResponse(dto, "User updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating user with ID {id}");
            return OutlayServiceResponse<UserDto>
                .FailureResponse("An error occurred while updating the user");
        }
    }

    /// <summary>
    /// Deletes a user
    /// </summary>
    public async Task<OutlayServiceResponse<bool>> DeleteUserAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return OutlayServiceResponse<bool>
                    .FailureResponse($"User with ID {id} not found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User with ID {id} deleted successfully");

            return OutlayServiceResponse<bool>
                .SuccessResponse(true, "User deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user with ID {id}");
            return OutlayServiceResponse<bool>
                .FailureResponse("An error occurred while deleting the user");
        }
    }

    /// <summary>
    /// Checks if a user exists by ID
    /// </summary>
    public async Task<bool> UserExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    /// <summary>
    /// Checks if an email already exists
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        var query = _context.Users.Where(u => u.Email == email);

        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync();
    }
}
