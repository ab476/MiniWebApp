import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Paper from "@mui/material/Paper";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import tenantClient from "@clients/tenantClient";


interface TenantFormProps {
  onSuccess: (message: string) => void;
  onError: (message: string) => void;
}

const TenantForm: React.FC<TenantFormProps> = ({ onSuccess, onError }) => {
  const queryClient = useQueryClient();
  const [formData, setFormData] = useState({ name: '', domain: '' });

  const mutation = useMutation({
    mutationFn: (data: typeof formData) => tenantClient.create(data),
    onSuccess: () => {
      setFormData({ name: '', domain: '' });
      queryClient.invalidateQueries({ queryKey: ['tenants'] });
      onSuccess('New tenant provisioned successfully!');
    },
    onError: (err: any) => onError(err.message),
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    mutation.mutate(formData);
  };

  return (
    <Paper component="form" onSubmit={handleSubmit} sx={{ p: 3, mb: 4, borderRadius: 3 }}>
      <Typography variant="h6" sx={{ mb: 2, fontSize: '1rem', fontWeight: 700 }}>Quick Add</Typography>
      <Box sx={{ display: 'flex', flexDirection: { xs: 'column', sm: 'row' }, gap: 2 }}>
        <TextField
          fullWidth
          label="Tenant Name"
          name="name" // Linked to handleChange
          size="small"
          required
          value={formData.name}
          onChange={handleChange}
        />
        <TextField
          fullWidth
          label="Custom Domain"
          name="domain" // Linked to handleChange
          placeholder="acme.yourtool.com"
          size="small"
          value={formData.domain}
          onChange={handleChange}
          inputProps={{ pattern: "^([a-z0-9]+(-[a-z0-9]+)*\\.)+[a-z]{2,}$" }} // Basic URL validation
        />
        <Button 
          variant="contained" 
          type="submit" 
          disabled={mutation.isPending}
          sx={{ minWidth: 150 }}
        >
          {mutation.isPending ? 'Creating...' : 'Create Tenant'}
        </Button>
      </Box>
    </Paper>
  );
};
export default TenantForm;