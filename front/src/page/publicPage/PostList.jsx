import React, { useEffect, useState } from "react";
import NavBar from "../components/NavBar";
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { getFollowedPosts } from "../../service/postService";


export default function PostList() {
  const [posts, setPosts] = useState([]);

  useEffect(() => {
    const fetchPosts = async () => {
      try {
        const data = await getFollowedPosts();
        setPosts(data.data.response);
      } catch (error) {
        toast.error(error);
      }
    };
    fetchPosts();
  }, []);

  return (
    <>
      <NavBar />
      {posts.length === 0 ? (
        <p className="text-center text-gray-500 py-24 sm:py-32">Aucun utilisateur n'a été suivi, donc pas de post visible .</p>
      ) : (
        <div className="bg-white py-24 sm:py-32">
          <div className="mx-auto max-w-7xl px-6 lg:px-8">  
            <ul className="mx-auto mt-20 grid max-w-2xl grid-cols-1 gap-x-8 gap-y-16 sm:grid-cols-2 lg:mx-0 lg:max-w-none lg:grid-cols-3">
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
                    Posté le : {new Date(post.createDate).toLocaleDateString()}
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
      )}
    </>
  );
}
