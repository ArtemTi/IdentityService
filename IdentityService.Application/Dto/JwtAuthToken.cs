using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Dto
{
    public class JwtAuthToken
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
