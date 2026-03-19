import React from 'react';
import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  IconButton,
  Stack,
  useTheme,
  Skeleton,
  Button,
  Typography,
} from '@mui/material';
import {
  Delete as DeleteIcon,
  Domain as DomainIcon,
  ToggleOn as ToggleOnIcon,
  ToggleOff as ToggleOffIcon,
} from '@mui/icons-material';
import type { TenantResponse } from '../../clients/types';

// The skeleton is kept here since it's only used by the table
const TenantTableSkeleton = () => (
  <>
    {[1, 2, 3, 4, 5].map((key) => (
      <TableRow key={key}>
        <TableCell>
          <Skeleton variant="text" width="60%" height={25} />
        </TableCell>
        <TableCell>
          <Skeleton variant="text" width="40%" height={25} />
        </TableCell>
        <TableCell>
          <Skeleton variant="rounded" width={70} height={24} />
        </TableCell>
        <TableCell>
          <Skeleton variant="text" width="50%" height={25} />
        </TableCell>
        <TableCell align="right">
          <Skeleton
            variant="rectangular"
            width={120}
            height={32}
            sx={{ ml: 'auto', borderRadius: 1 }}
          />
        </TableCell>
      </TableRow>
    ))}
  </>
);

interface TenantTableProps {
  tenants: TenantResponse[];
  loading: boolean;
  canManage: boolean;
  onToggleStatus: (tenant: TenantResponse) => void;
  onDelete: (id: string) => void;
}

const TenantTable: React.FC<TenantTableProps> = ({
  tenants,
  loading,
  canManage,
  onToggleStatus,
  onDelete,
}) => {
  const theme = useTheme();

  return (
    <TableContainer
      component={Paper}
      sx={{
        borderRadius: 3,
        overflow: 'hidden',
        border: `1px solid ${theme.palette.divider}`,
      }}
    >
      <Table>
        <TableHead sx={{ bgcolor: theme.palette.action.hover }}>
          <TableRow>
            <TableCell sx={{ fontWeight: 700 }}>Company Name</TableCell>
            <TableCell sx={{ fontWeight: 700 }}>Domain</TableCell>
            <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
            <TableCell sx={{ fontWeight: 700 }}>Created On</TableCell>
            <TableCell align="right" sx={{ fontWeight: 700 }}>
              Actions
            </TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {loading ? (
            <TenantTableSkeleton />
          ) : tenants.length === 0 ? (
            <TableRow>
              <TableCell colSpan={5} align="center" sx={{ py: 8 }}>
                <Typography color="text.secondary">
                  No tenants found matching your criteria.
                </Typography>
              </TableCell>
            </TableRow>
          ) : (
            tenants.map((t) => (
              <TableRow
                key={t.id}
                hover
                sx={{
                  '&:last-child td, &:last-child th': {
                    border: 0,
                  },
                }}
              >
                <TableCell sx={{ fontWeight: 600 }}>{t.name}</TableCell>
                <TableCell>
                  <Stack direction="row" spacing={1} alignItems="center">
                    <DomainIcon fontSize="inherit" color="disabled" />
                    <Typography variant="body2">{t.domain || 'N/A'}</Typography>
                  </Stack>
                </TableCell>
                <TableCell>
                  <Chip
                    label={t.isActive ? 'Active' : 'Inactive'}
                    size="small"
                    color={t.isActive ? 'success' : 'default'}
                    sx={{
                      fontSize: '0.75rem',
                      fontWeight: 700,
                    }}
                  />
                </TableCell>
                <TableCell color="text.secondary">
                  {new Date(t.createdAt).toLocaleDateString()}
                </TableCell>

                <TableCell align="right">
                  {/* 4. Only render actions if user can manage */}
                  {canManage ? (
                    <Stack
                      direction="row"
                      spacing={1}
                      justifyContent="flex-end"
                    >
                      <Button
                        size="small"
                        variant="outlined"
                        onClick={() => onToggleStatus(t)}
                        startIcon={
                          t.isActive ? <ToggleOffIcon /> : <ToggleOnIcon />
                        }
                        sx={{
                          textTransform: 'none',
                          borderRadius: 2,
                        }}
                      >
                        {t.isActive ? 'Suspend' : 'Enable'}
                      </Button>
                      <IconButton
                        color="error"
                        size="small"
                        onClick={() => onDelete(t.id)}
                      >
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </Stack>
                  ) : (
                    <Typography variant="caption" color="text.disabled">
                      View Only
                    </Typography>
                  )}
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default TenantTable;
