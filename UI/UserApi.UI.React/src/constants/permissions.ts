export const AppPermissions = {
  Tenants: {
    Read: "tenants.read",
    Write: "tenants.write",
    Manage: "tenants.manage",
    get All(): string[] {
      return [this.Read, this.Write, this.Manage];
    }
  },

  Permissions: {
    Read: "permissions.read",
    get All(): string[] {
      return [this.Read];
    }
  },

  Roles: {
    Read: "roles.read",
    Write: "roles.write",
    Manage: "roles.manage",
    get All(): string[] {
      return [this.Read, this.Write, this.Manage];
    }
  },

  Users: {
    Read: "users.read",
    Write: "users.write",
    Manage: "users.manage",
    get All(): string[] {
      return [this.Read, this.Write, this.Manage];
    }
  },

  // Global All using the spread operator
  get All(): string[] {
    return [
      ...this.Tenants.All,
      ...this.Roles.All,
      ...this.Users.All,
      ...this.Permissions.All
    ];
  }
} as const;

// Optional: Create a type for your permissions
export type AppPermission = typeof AppPermissions.All[number];