﻿using System;
using System.Collections.Generic;

namespace Echo.IntegrationTests.Subject
{
    public class ProvisioningResponse
    {
        public IEnumerable<ServiceType> ProvisionedServices { get; set; }
    }

    public class ProvisioningFailureException : Exception
    {

    }
}
