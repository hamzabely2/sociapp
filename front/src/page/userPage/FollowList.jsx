import React, { useEffect, useState } from "react";
import NavBar from "../components/NavBar";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { getFollowedUsers, unfollowUser } from "../../service/followService";
import { getUser } from "../../service/userService";

export default function FollowList() {
  const [users, setUsers] = useState([]);
  const [user, setUser] = useState([]);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const data = await getUser(); 
        setUser(data.data.response);
      } catch (error) {
        toast.error("Erreur lors de la récupération de l'utilisateur.");
      }
    };

    fetchUser();
  }, []);


  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const data = await getFollowedUsers();
        console.log(data)
        setUsers(data.data.response);
      } catch (error) {
      }
    };
    fetchUsers();
  }, []);

  const handleUnFollow = async (userIdFollowed) => {
    try {
      const data = await unfollowUser(userIdFollowed,user.Id);
      toast.success(data.data.message);
      setUsers((prevUsers) => prevUsers.filter((user) => user.id !== userIdFollowed));
    } catch (error) {
      toast.error(error);
    }
    try {
      const data = await getFollowedUsers();
      console.log(data)
      setUsers(data.data.response);
    } catch (error) {
      toast.warning(error)
    }
  };
 

  return (
    <div className="min-h-full">
      <NavBar />
      <main>
      {users.length === 0 ? (
        <p className="text-center text-gray-500 py-24 sm:py-32">Aucun utilisateur n'a été suivi</p>
      ) : (
          <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
            <ul role="list" className="divide-y divide-gray-100">
              {users.map((user) => (
                <li key={user.id} className="flex justify-between gap-x-6 py-5">
                  <div className="flex min-w-0 gap-x-4">
                    <img
                      alt=""
                      src="https://via.placeholder.com/50" // Remplacez par une URL d'image utilisateur si disponible
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
          </div>
      )}
      </main>
    </div>
  );
}
