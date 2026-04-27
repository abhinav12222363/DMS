import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { api } from '../../services/api';

export interface Distributor { id: string; code: string; name: string; zone: string }

interface DistributorState {
  available: Distributor[];
  selected: Distributor | null;
}

const initialState: DistributorState = { available: [], selected: null };

export const fetchDistributors = createAsyncThunk('distributor/fetch', async () => {
  const { data } = await api.get('/distributors/my');
  return data as Distributor[];
});

const slice = createSlice({
  name: 'distributor',
  initialState,
  reducers: {
    selectDistributor: (state, action) => {
      state.selected = action.payload;
    }
  },
  extraReducers: (builder) => {
    builder.addCase(fetchDistributors.fulfilled, (state, action) => {
      state.available = action.payload;
    });
  }
});

export const { selectDistributor } = slice.actions;
export default slice.reducer;
