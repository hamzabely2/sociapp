import React, { useEffect, useState } from 'react';
import NavBar from '../components/NavBar';
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { getAllUsers,  } from '../../service/userService';
import {  followUser } from '../../service/followService';

export default function UserList() {
  const [users, setUsers] = useState([]);

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const data = await getAllUsers(); 
        setUsers(data.data.response);
      } catch (err) {
        toast.error(err);
      }
    };
    fetchUsers();
  }, []);

  const handleFollow = async (userIdFollowed) => {
    try {
      const data = await followUser(userIdFollowed); 
      console.log(data)
      if(data.status === 200){
        toast.success(data.data.message);
      }else{
        toast.error(data.response.data.message);
      }
    } catch (err) {
      toast.error(err);
    }
  };

  return (
    <div className="min-h-full">
      <NavBar />
      <main>
          <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
            <ul role="list" className="divide-y divide-gray-100">
              {users.map((user) => (
                <li key={user.id} className="flex justify-between gap-x-6 py-5">
                  <div className="flex min-w-0 gap-x-4">
                    <img
                      alt=""
                      src={user.imageUrl || "https://via.placeholder.com/50"}
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
          </div>   
      </main>
    </div>
  );
}
