import React, { useState } from 'react';
import axios from 'axios';
import { Link, useNavigate } from 'react-router-dom';
import { setCookie } from './Token';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    const data = { email, password };

    try {
      const response = await axios.post(
        `${process.env.REACT_APP_URL}user/login`,
        data
      );
      console.log(response, "respose")
      if (response.status === 200) {
        setCookie(response.data.response);
        toast.success(response.data.message);
        navigate('/');
      } else {
        toast.warning("L'action a échoué.");
      }
      toast.warning("L'action a échoué.");
    } catch (error) {
      toast.error(error.message !== "Network Error" ? error.response.data.message : error.message )
    }
  };

  return (
    <div className="flex min-h-full flex-1 flex-col justify-center px-6 py-12 lg:px-8">
      <ToastContainer />
      <div className="sm:mx-auto sm:w-full sm:max-w-sm">
          <img
            className="mx-auto h-[180px] w-auto"
            src="/img/logo.png"
            alt="Pot Shop"
          />
        <h2 className="mt-10 text-center text-2xl font-bold tracking-tight text-gray-900">
          Se connecter
        </h2>
      </div>

      <div className="mt-10 sm:mx-auto sm:w-full sm:max-w-sm">
        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-900">
              Email
            </label>
            <div className="mt-2">
              <input
                id="email"
                name="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                autoComplete="email"
                className="p-1 block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-indigo-600"
              />
            </div>
          </div>

          <div>
            <label htmlFor="password" className="block text-sm font-medium text-gray-900">
              Password
            </label>
            <div className="mt-2">
              <input
                id="password"
                name="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                autoComplete="current-password"
                className="p-1 block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-indigo-600"
              />
            </div>
          </div>
          <div>
            <Link to="/register">
            <p className='mb-2 text-indigo-500'>inscrivez-vous maintenant</p>
            </Link>
            <button
              type="submit"
              className="flex w-full justify-center rounded-md bg-indigo-600 px-3 py-1.5 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
            >
              Valider
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}