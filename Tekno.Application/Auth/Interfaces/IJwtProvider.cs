using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Domain.Auth;

namespace Tekno.Application.Auth.Interfaces
{
    public interface IJwtProvider
    {
        (string Token, DateTime ExpiresAt) GenerateToken(User user);
    }
}
