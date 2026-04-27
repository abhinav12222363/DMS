import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { api } from '../../services/api';

interface AuthState {
  token: string | null;
  username: string | null;
  role: string | null;
  loading: boolean;
  error: string | null;
}

const initialState: AuthState = { token: localStorage.getItem('token'), username: null, role: null, loading: false, error: null };

export const login = createAsyncThunk('auth/login', async (payload: { username: string; password: string; captchaToken: string }) => {
  const { data } = await api.post('/auth/login', payload);
  localStorage.setItem('token', data.token);
  return data as { token: string; username: string; role: string };
});

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    logout: (state) => {
      state.token = null;
      state.username = null;
      state.role = null;
      localStorage.removeItem('token');
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (s) => {
        s.loading = true;
        s.error = null;
      })
      .addCase(login.fulfilled, (s, a) => {
        s.loading = false;
        s.token = a.payload.token;
        s.username = a.payload.username;
        s.role = a.payload.role;
      })
      .addCase(login.rejected, (s) => {
        s.loading = false;
        s.error = 'Authentication failed';
      });
  }
});

export const { logout } = authSlice.actions;
export default authSlice.reducer;
