
export const usePermission = () => {
  // Replace this with your actual auth logic/context
  const userPermissions: string[] = ['tenants.read', 'tenants.write']; 

  const hasPermission = (permission: string) => {
    return userPermissions.includes(permission);
  };

  return { hasPermission };
};