using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Google.Recaptcha
{
    public interface IReCaptchaGateway
    {
        Task<bool> ValidateReCaptcha(string validationToken);
    }
}
