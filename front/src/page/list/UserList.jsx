import React, { useEffect, useState } from 'react';
import axios from 'axios';
import NavBar from '../components/NavBar'

export default function UserList() {
  const [users, setUsers] = useState([]);
  const [error, setError] = useState('');

  // Fetch users from the API
  const fetchUsers = async () => {
    setError('');
    try {
      const response = await axios.get(`${process.env.REACT_APP_URL}user/get-all-users`);
      console.log(response)
      setUsers(response.data.result); // Assume API returns an array of users
    } catch (err) {
      console.error('Failed to fetch users:', err);
      setError('Échec de la récupération des utilisateurs.');
    } 
  };

  const handleFollow = async (userId) => {
    try {
      await axios.post(`${process.env.REACT_APP_URL}api/follow/followe-user`);
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
    <div classNa
    me="min-h-full">
      <NavBar />
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
                    Suivre
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
