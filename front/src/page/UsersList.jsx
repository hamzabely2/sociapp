import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Disclosure } from '@headlessui/react'
import { BellIcon } from '@heroicons/react/24/outline'
import { Link } from 'react-router-dom'

export default function UserList() {
  let user = false
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Fetch users from the API
  const fetchUsers = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await axios.get(`${process.env.REACT_APP_URL}user/get`);
      console.log(response)
      setUsers(response.data.result); // Assume API returns an array of users
    } catch (err) {
      console.error('Failed to fetch users:', err);
      setError('Échec de la récupération des utilisateurs.');
    } finally {
      setLoading(false);
    }
  };

  // Handle follow button click
  const handleFollow = async (userId) => {
    try {
      await axios.post(`${process.env.REACT_APP_URL}api/users/${userId}/follow`);
      alert(`Vous suivez maintenant l'utilisateur avec l'ID ${userId}.`);
    } catch (err) {
      console.error('Failed to follow user:', err);
      alert('Échec du suivi. Veuillez réessayer.');
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);


  return (
    <div className="min-h-full">
      <Disclosure as="nav" className="bg-gray-800">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="flex h-16 items-center justify-between">
            <div className="flex items-center">
              <div className="shrink-0">
                <Link to="/">
                  <img
                    className="h-12 w-auto"
                    src="/img/logo.png"
                    alt="pot shop"
                  />
                </Link>
              </div>
              <Link to="/user">
                <button
                  type="button"
                  className="rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 m-2"
                >
                  Utilisateur
                </button>
              </Link>
              <div className="hidden md:block">
                <div className="ml-10 flex items-baseline space-x-4">

                </div>
              </div>
            </div>
            <div className="hidden md:block">
              <div className="ml-4 flex items-center md:ml-6">
                {!user ? (
                  <>
                    <Link to="/login">
                      <button
                        type="button"
                        className="rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 m-2"
                      >
                        Login
                      </button>
                    </Link>
                    <Link to="/register">
                      <button
                        type="button"
                        className="rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
                      >
                        Register
                      </button>
                    </Link>
                  </>
                ) : (
                  <button
                    type="button"
                    className="relative rounded-full bg-gray-800 p-1 text-gray-400 hover:text-white focus:outline-none focus:ring-2 focus:ring-white focus:ring-offset-2 focus:ring-offset-gray-800"
                  >
                    <span className="absolute -inset-1.5" />
                    <span className="sr-only">View notifications</span>
                    <BellIcon aria-hidden="true" className="h-6 w-6" />
                  </button>
                )}

              </div>
            </div>
          </div>
        </div>
      </Disclosure>
      <main>
        {!error ? (<div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
          <ul role="list" className="divide-y divide-gray-100">
            {users.map((user) => (
              <li key={user.id} className="flex justify-between gap-x-6 py-5">
                <div className="flex min-w-0 gap-x-4">
                  <img
                    alt=""
                    src="https://via.placeholder.com/50" // Replace with a user image URL if available
                    className="size-12 flex-none rounded-full bg-gray-50"
                  />
                  <div className="min-w-0 flex-auto">
                    <p className="text-sm/6 font-semibold text-gray-900">{user.userName}</p>
                    <p className="mt-1 truncate text-xs/5 text-gray-500">{user.email}</p>
                  </div>
                </div>
                <div className="flex items-center gap-x-4">
                  <p className="text-sm/6 text-gray-900">User ID: {user.id}</p>
                  <button
                    onClick={() => handleFollow(user.id)}
                    className="rounded-md bg-indigo-600 px-3 py-1 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500"
                  >
                    Follow
                  </button>
                </div>
              </li>
            ))}
          </ul>
        </div>) : (
          <div>
            <p className="text-red-500">{error}</p>
          </div>
        )}
      </main>
    </div>

  );
}
