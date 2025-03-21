﻿using DepuChef.Application.Models.User;
using System.Linq.Expressions;

namespace DepuChef.Application.Services;

public interface IUserService
{
    Task<User?> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken);
    Task<User?> GetUser(Expression<Func<User, bool>> expression, CancellationToken cancellationToken);
    Task UpdateUser(User user, CancellationToken cancellationToken);
}
