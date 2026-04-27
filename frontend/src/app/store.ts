import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../features/auth/authSlice';
import distributorReducer from '../features/distributor/distributorSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    distributor: distributorReducer
  }
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
