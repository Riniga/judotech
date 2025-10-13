import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    mode: 'light',
    primary: { main: '#2E7D32' },   // grönt som i exemplet
    secondary: { main: '#1565C0' }
  },
  shape: { borderRadius: 12 },       // rundade kort
});
export default theme;
