using System;
using System.Collections.Generic;
using System.Text;

namespace MiniWebApp.AppHost.Options;

public sealed class AppHostOptions
{
    public InfrastructureOptions Infrastructure { get; set; } = new();
    public ProjectsOptions Projects { get; set; } = new();
}

public sealed class InfrastructureOptions
{
    public bool Redis { get; set; }
    public bool Postgres { get; set; }
    public bool MySql { get; set; }
    public bool LocalStack { get; set; }
    public bool Kafka { get; set; }
}

public sealed class ProjectsOptions
{
    public bool UserApi { get; set; }
    public bool ApiService { get; set; }
    public bool WebFrontend { get; set; }
    public bool TaskApi { get; set; }
    public bool ReactApp { get; set; }
    public bool KakfaApi { get; set; }
}

