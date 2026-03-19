import React, { useState } from 'react';
import {
  Container,
  Typography,
  Box,
  Chip,
  Alert,
  Snackbar,
} from '@mui/material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';

import tenantClient from '../../clients/tenantClient';
import type { TenantResponse } from '../../clients/types';
import TenantForm from './TenantForm';
import TenantTable from './TenantTable';
import ConfirmDialog from '@components/ConfirmDialog';
import { usePermission } from "../../constants/usePermission";
import { AppPermissions } from '../../constants/permissions';

const TenantManager: React.FC = () => {
  const { hasPermission } = usePermission();
  const queryClient = useQueryClient();

  // 1. Check core Read access
  const canRead = hasPermission(AppPermissions.Tenants.Read);
  const canWrite = hasPermission(AppPermissions.Tenants.Write);
  const canManage = hasPermission(AppPermissions.Tenants.Manage);

  const [tenantToDelete, setTenantToDelete] = useState<string | null>(null);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  // --- Queries ---
  const {
    data: tenants = [],
    isLoading,
    error,
  } = useQuery({
    queryKey: ['tenants'],
    queryFn: ({ signal }) => tenantClient.getPaged(1, 100, signal),
    enabled: canRead,
  });

  // --- Mutations ---
  const deleteMutation = useMutation({
    mutationFn: (id: string) => tenantClient.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tenants'] });
      showSnackbar('Tenant deleted successfully', 'success');
    },
    onError: (err: any) => showSnackbar(err.message, 'error'),
  });

  const toggleMutation = useMutation({
    mutationFn: (tenant: TenantResponse) =>
      tenant.isActive
        ? tenantClient.deactivate(tenant.id, { tenantId: tenant.id })
        : tenantClient.activate(tenant.id, { tenantId: tenant.id }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['tenants'] }),
    onError: (err: any) => showSnackbar(err.message, 'error'),
  });

  // --- Handlers ---
  const showSnackbar = (message: string, severity: 'success' | 'error') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleDeleteConfirm = () => {
    if (tenantToDelete) {
      deleteMutation.mutate(tenantToDelete);
      setTenantToDelete(null);
    }
  };

  // If they can't read, show an empty state or Alert
  if (!canRead) {
    return (
      <Container sx={{ py: 4 }}>
        <Alert severity="error">
          You do not have permission to view this page.
        </Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Box
        sx={{
          mb: 4,
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
        }}
      >
        <Box>
          <Typography variant="h4" fontWeight={800} color="primary.main">
            Tenants
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Manage sub-domains and access
          </Typography>
        </Box>
        {!isLoading && (
          <Chip
            label={`${tenants.length} Total`}
            color="secondary"
            sx={{ fontWeight: 700 }}
          />
        )}
      </Box>

      {error && (
        <Alert
          severity="error"
          sx={{ mb: 3 }}
          onClose={() => queryClient.resetQueries({ queryKey: ['tenants'] })}
        >
          {(error as any).message || 'Failed to load tenants'}
        </Alert>
      )}

      {canWrite && (
        <TenantForm
          onSuccess={(msg) => showSnackbar(msg, 'success')}
          onError={(msg) => showSnackbar(msg, 'error')}
        />
      )}

      <TenantTable
        tenants={tenants}
        canManage={canManage}
        loading={isLoading}
        onToggleStatus={(t) => toggleMutation.mutate(t)}
        onDelete={setTenantToDelete}
      />

      <ConfirmDialog
        open={tenantToDelete !== null}
        title="Delete Tenant"
        message="This action cannot be undone."
        confirmText="Delete"
        onConfirm={handleDeleteConfirm}
        onCancel={() => setTenantToDelete(null)}
      />

      <Snackbar
        open={snackbar.open}
        autoHideDuration={5000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert severity={snackbar.severity} variant="filled">
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Container>
  );
};

export default TenantManager;
