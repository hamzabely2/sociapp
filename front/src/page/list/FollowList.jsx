import React, { useEffect, useState } from 'react';
import axios from 'axios';
import NavBar from '../components/NavBar'
import Cookies from "universal-cookie";

export default function FollowList() {
  const [users, setUsers] = useState([]);
  const [error, setError] = useState('');
  const cookies = new Cookies();

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await axios.get(
          `${process.env.REACT_APP_URL}follow/get-list-followed-users`,
          {
            headers: {
              Authorization: `Bearer ${cookies.get("token")}`,
            },
          }
        );
        setUsers(response.data);
      } catch (error) {
        console.error("Error fetching posts:", error);
      }
    };

    fetchUsers();
  }, []);

  const handleUnFollow = async (userId) => {
    try {
      await axios.post(`${process.env.REACT_APP_URL}api/users/${userId}/follow`);
      alert(`Vous suivez maintenant l'utilisateur avec l'ID ${userId}.`);
    } catch (err) {
      console.error('Failed to follow user:', err);
      alert('Échec du suivi. Veuillez réessayer.');
    }
  };

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
                    onClick={() => handleUnFollow(user.id)}
                    className="rounded-md bg-red-600 px-3 py-1 text-sm font-semibold text-white shadow-sm hover:bg-red-500"
                  >
                    Ne plus suivre
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
