import React, { useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    const data = {
      email,
      password,
    };

    try {
      const response = await axios.post(
        `${process.env.REACT_APP_URL}user/login`,
        data
      );

      // Extrait le token de la réponse
      const { token } = response.data;

      // Vérifie si un token a été renvoyé
      if (token) {
        // Définit le token dans un cookie avec une durée de 7 jours
        document.cookie = `authToken=${token}; path=/; max-age=${7 * 24 * 60 * 60}; secure; HttpOnly`;

        console.log('Token saved in cookies.');
      } else {
        console.error('No token received from the server.');
      }
    } catch (error) {
      console.error('Login failed:', error);
      setError('Échec de la connexion. Veuillez vérifier vos informations.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex min-h-full flex-1 flex-col justify-center px-6 py-12 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-sm">
        <Link to="/">
        <img
          className="mx-auto h-[180px] w-auto"
          src="/img/logo.png"
          alt="Pot Shop"
        />
        </Link>
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

          {error && <p className="text-red-500 text-sm">{error}</p>}

          <div>
            <button
              type="submit"
              className={`flex w-full justify-center rounded-md bg-indigo-600 px-3 py-1.5 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 ${
                loading ? 'opacity-50' : ''
              }`}
              disabled={loading}
            >
              {loading ? 'Chargement...' : 'Valider'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
