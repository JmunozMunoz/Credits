using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    public class ClientCompromise
    {

        public string FullName { get; set; }

        public List<DetailedCreditCompromise> detailedCreditCompromises { get; set; }


    }
}
