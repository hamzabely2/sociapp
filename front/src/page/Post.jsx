import React from 'react';
import  { useEffect, useState } from 'react';
import axios from 'axios';

export default function Post() {
  const [posts, setPosts] = useState([]);

  useEffect(() => {
    const fetchPosts = async () => {
      try {
        const response = await axios.get(process.env.REACT_APP_URL + 'post/get'); 
        setPosts(response.data);
      } catch (error) {
        console.error('Error fetching posts:', error);
      }
    };
    fetchPosts();
  }, []);

  if (!posts.length) {
    return <p>Loading posts...</p>;
  }

  return (
    <div className="bg-white py-24 sm:py-32">
      <div className="mx-auto max-w-7xl px-6 lg:px-8">
        <div className="mx-auto max-w-2xl lg:mx-0">
          <h2 className="text-pretty text-4xl font-semibold tracking-tight text-gray-900 sm:text-5xl">
            Posts
          </h2>
          <p className="mt-6 text-lg/8 text-gray-600">
            Explore our latest posts, tutorials, and articles written by our team.
          </p>
        </div>

        {/* List of posts */}
        <ul
          role="list"
          className="mx-auto mt-20 grid max-w-2xl grid-cols-1 gap-x-8 gap-y-16 sm:grid-cols-2 lg:mx-0 lg:max-w-none lg:grid-cols-3"
        >
          {posts.map((post) => (
            <li key={post.id} className="flex flex-col">
              <img
                alt=""
                src={post.mediaUrl}
                className="aspect-[3/2] w-full rounded-2xl object-cover"
              />
              <h3 className="mt-6 text-lg/8 font-semibold tracking-tight text-gray-900">
                {post.title}
              </h3>
              <p className="text-base/7 text-gray-600">Type: {post.type}</p>
              <p className="text-sm text-gray-500">
                Posted on: {post.createDate}
              </p>
              <div className="mt-6">
                <a
                  href={post.downloadUrl}
                  className="rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500"
                >
                  Download
                </a>
              </div>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}
