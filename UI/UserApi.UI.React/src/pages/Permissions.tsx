import React, { useEffect, useState, useCallback } from 'react';
import { 
  Typography, Button, Box, Paper, IconButton, 
  Dialog, DialogTitle, DialogContent, TextField, DialogActions 
} from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { Delete as DeleteIcon, Edit as EditIcon, Add as AddIcon } from '@mui/icons-material';
import type { PermissionResponse, PagedRequest } from '../types/permission';

const PermissionsPage: React.FC = () => {
  // State Management
  const [rows, setRows] = useState<PermissionResponse[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [paginationModel, setPaginationModel] = useState({ page: 0, pageSize: 10 });
  
  // Dialog State
  const [open, setOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState({ name: '', description: '' });

  // 1. Fetch Paged Data (Matches IPermissionQueries.GetPagedAsync)
  const fetchPermissions = useCallback(async () => {
    setLoading(true);
    try {
      // Replace with your actual API call logic
      // const response = await api.getPermissions({ pageNumber: paginationModel.page + 1, pageSize: paginationModel.pageSize });
      // setRows(response.items);
      // setTotalCount(response.totalCount);
      console.log("Fetching page:", paginationModel.page);
    } finally {
      setLoading(false);
    }
  }, [paginationModel]);

  useEffect(() => { fetchPermissions(); }, [fetchPermissions]);

  // 2. Handle Create/Update (Matches IPermissionRepository Create/Update)
  const handleSave = async () => {
    if (editingId) {
      console.log("Updating:", editingId, formData);
    } else {
      console.log("Creating:", formData);
    }
    handleClose();
    fetchPermissions();
  };

  // 3. Handle Delete (Matches IPermissionRepository.DeleteAsync)
  const handleDelete = async (id: string) => {
    if (window.confirm("Are you sure?")) {
      console.log("Deleting:", id);
      fetchPermissions();
    }
  };

  const handleOpen = (permission?: PermissionResponse) => {
    if (permission) {
      setEditingId(permission.id);
      setFormData({ name: permission.name, description: permission.description });
    } else {
      setEditingId(null);
      setFormData({ name: '', description: '' });
    }
    setOpen(true);
  };

  const handleClose = () => setOpen(false);

  const columns: GridColDef[] = [
    { field: 'name', headerName: 'Permission Name', flex: 1 },
    { field: 'description', headerName: 'Description', flex: 2 },
    {
      field: 'actions',
      headerName: 'Actions',
      width: 120,
      renderCell: (params) => (
        <Box>
          <IconButton onClick={() => handleOpen(params.row)} color="primary"><EditIcon /></IconButton>
          <IconButton onClick={() => handleDelete(params.row.id)} color="error"><DeleteIcon /></IconButton>
        </Box>
      ),
    },
  ];

  return (
    <Box sx={{ p: 3 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
        <Typography variant="h4">Permissions</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => handleOpen()}>
          Add Permission
        </Button>
      </Box>

      <Paper sx={{ height: 400, width: '100%' }}>
        <DataGrid
          rows={rows}
          columns={columns}
          rowCount={totalCount}
          loading={loading}
          pageSizeOptions={[5, 10, 20]}
          paginationModel={paginationModel}
          onPaginationModelChange={setPaginationModel}
          paginationMode="server"
        />
      </Paper>

      {/* Create/Edit Dialog */}
      <Dialog open={open} onClose={handleClose} fullWidth>
        <DialogTitle>{editingId ? 'Edit Permission' : 'New Permission'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <TextField 
            label="Name" 
            fullWidth 
            value={formData.name} 
            onChange={(e) => setFormData({...formData, name: e.target.value})} 
          />
          <TextField 
            label="Description" 
            fullWidth 
            multiline 
            rows={3} 
            value={formData.description} 
            onChange={(e) => setFormData({...formData, description: e.target.value})} 
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleSave} variant="contained">Save</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default PermissionsPage;