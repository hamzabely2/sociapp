import React, { useEffect, useState } from "react";
import axios from "axios";
import Cookies from "universal-cookie";
import NavBar from "../components/NavBar";
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export default function PostList() {
  const [posts, setPosts] = useState([]);
  const cookies = new Cookies();

  useEffect(() => {
    const fetchPosts = async () => {
      try {
        const response = await axios.get(
          `${process.env.REACT_APP_URL}post/get-follow-posts`,
          {
            headers: {
              Authorization: `Bearer ${cookies.get("token")}`,
            },
          }
        );
        if (response.status === 200) {
          setPosts(response.data);
          toast.success(response.data.message);
        }else{
          toast.warning("L'action a échoué.");
        }
      } catch (error) {
        toast.warning(error);
      }
    };
    fetchPosts();
  }, []);

  return (
    <>
      <NavBar />
      <div className="bg-white py-24 sm:py-32">
        <div className="mx-auto max-w-7xl px-6 lg:px-8">  
          <ul
            role="list"
            className="mx-auto mt-20 grid max-w-2xl grid-cols-1 gap-x-8 gap-y-16 sm:grid-cols-2 lg:mx-0 lg:max-w-none lg:grid-cols-3"
          >
            {posts.map((post) => (
              <li key={post.id} className="flex flex-col">
                <img
                  alt=""
                  src={post.downloadUrl}
                  className="aspect-[3/2] w-full rounded-2xl object-cover"
                />
                <h3 className="mt-6 text-lg font-semibold tracking-tight text-gray-900">
                  {post.title}
                </h3>
                <p className="text-base text-gray-600">Type: {post.type}</p>
                <p className="text-sm text-gray-500">
                  Posted on: {new Date(post.createDate).toLocaleDateString()}
                </p>
                <div className="mt-6">
                  <a
                    href={post.downloadUrl}
                    className="rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500"
                    download
                  >
                    Télécharger
                  </a>
                </div>
              </li>
            ))}
          </ul>
        </div>
      </div>
    </>
  );
}
