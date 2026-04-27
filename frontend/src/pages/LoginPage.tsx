import { useState } from 'react';
import { Eye, EyeOff } from 'lucide-react';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import { login } from '../features/auth/authSlice';
import { useNavigate } from 'react-router-dom';

export function LoginPage() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const loading = useAppSelector((s) => s.auth.loading);
  const [showPassword, setShowPassword] = useState(false);
  const [form, setForm] = useState({ username: '', password: '', captchaToken: '' });

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await dispatch(login(form));
    navigate('/select-distributor');
  };

  return (
    <div className="min-h-screen grid place-items-center bg-gradient-to-br from-slate-100 to-blue-50">
      <form className="bg-white p-6 rounded-xl shadow-lg w-full max-w-md space-y-4" onSubmit={onSubmit}>
        <h1 className="text-2xl font-bold">DMS Login</h1>
        <input className="border rounded px-3 py-2 w-full" placeholder="Username" onChange={(e) => setForm({ ...form, username: e.target.value })} />
        <div className="relative">
          <input type={showPassword ? 'text' : 'password'} className="border rounded px-3 py-2 w-full" placeholder="Password" onChange={(e) => setForm({ ...form, password: e.target.value })} />
          <button type="button" className="absolute right-3 top-3" onClick={() => setShowPassword((v) => !v)}>{showPassword ? <EyeOff size={16} /> : <Eye size={16} />}</button>
        </div>
        <input className="border rounded px-3 py-2 w-full" placeholder="CAPTCHA token" onChange={(e) => setForm({ ...form, captchaToken: e.target.value })} />
        <button disabled={loading} className="bg-primary text-white w-full py-2 rounded disabled:opacity-50">{loading ? 'Signing in...' : 'Login'}</button>
        <button type="button" className="text-primary text-sm">Forgot Password?</button>
      </form>
    </div>
  );
}
