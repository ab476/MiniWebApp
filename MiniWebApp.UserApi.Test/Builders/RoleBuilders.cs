using BuilderGenerator;
using MiniWebApp.UserApi.Contracts.Roles;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniWebApp.UserApi.Test.Builders;

[BuilderFor(typeof(CreateRoleRequest))]
public partial class CreateRoleRequestBuilder { }
