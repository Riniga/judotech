import * as React from 'react';
import {
  AppBar, Toolbar, Typography, Box, Drawer, List, ListItemButton,
  ListItemIcon, ListItemText, Divider, IconButton, Card, CardContent, Stack
} from '@mui/material';
import DashboardIcon from '@mui/icons-material/Dashboard';
import AssignmentIcon from '@mui/icons-material/Assignment';
import ReceiptLongIcon from '@mui/icons-material/ReceiptLong';
import MenuIcon from '@mui/icons-material/Menu';

const drawerWidth = 80;

export default function App() {
  const [mobileOpen, setMobileOpen] = React.useState(false);
  const toggle = () => setMobileOpen(v => !v);

  const drawer = (
    <Box sx={{ mt: 1 }}>
      <List>
        {[
          { text: 'Dashboard', icon: <DashboardIcon /> },
          { text: 'Följesedel', icon: <ReceiptLongIcon /> },
          { text: 'Rapporter', icon: <AssignmentIcon /> },
        ].map(item => (
          <ListItemButton key={item.text} sx={{ justifyContent: 'center', py: 2 }}>
            <ListItemIcon sx={{ minWidth: 0, justifyContent: 'center' }}>{item.icon}</ListItemIcon>
            <ListItemText primary={item.text} sx={{ display: 'none' }} />
          </ListItemButton>
        ))}
      </List>
      <Divider />
    </Box>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      {/* 3 */}
      <AppBar position="fixed" sx={{ zIndex: theme => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton color="inherit" edge="start" sx={{ mr: 2, display: { sm: 'none' }}} onClick={toggle}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap>Följesedel</Typography>
        </Toolbar>
      </AppBar>

      {/* Sidebar */}
      <Box component="nav" sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 }}}>
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={toggle}
          ModalProps={{ keepMounted: true }}
          sx={{ display: { xs: 'block', sm: 'none' },
                '& .MuiDrawer-paper': { width: drawerWidth } }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{ display: { xs: 'none', sm: 'block' },
                '& .MuiDrawer-paper': { width: drawerWidth, boxSizing: 'border-box' } }}
          open
        >
          {drawer}
        </Drawer>
      </Box>

      {/* Innehållsarea */}
      <Box component="main" sx={{ flexGrow: 1, p: 3, minHeight: '100vh', bgcolor: 'background.default' }}>
        <Toolbar /> {/* offset för AppBar */}
        <Stack spacing={2}>
          <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
            <Card sx={{ flex: 2 }}>
              <CardContent>
                <Typography variant="h6">Följesedelinfo</Typography>
                <Typography variant="body2" color="text.secondary">Lista med fält, ikon + label + värde…</Typography>
              </CardContent>
            </Card>
            <Card sx={{ flex: 1 }}>
              <CardContent>
                <Typography variant="h6">Kund</Typography>
                <Typography variant="body2" color="text.secondary">Kontakt, adress, orgnr…</Typography>
              </CardContent>
            </Card>
          </Stack>

          <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
            <Card sx={{ flex: 1 }}>
              <CardContent>
                <Typography variant="h6">Följesedelkommentar</Typography>
                <Typography variant="body2" color="text.secondary">Accordion / textarea etc.</Typography>
              </CardContent>
            </Card>
            <Card sx={{ flex: 1 }}>
              <CardContent>
                <Typography variant="h6">Internanteckning</Typography>
                <Typography variant="body2" color="text.secondary">Historik, tidsstämpel…</Typography>
              </CardContent>
            </Card>
          </Stack>

          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>Bränslerapport</Typography>
              <Typography variant="body2" color="text.secondary">
                Här stoppar vi in TanStack Table senare.
              </Typography>
            </CardContent>
          </Card>
        </Stack>
      </Box>
    </Box>
  );
}
