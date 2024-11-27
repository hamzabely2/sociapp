import React, { useEffect, useState } from "react";
import axios from "axios";
import NavBar from "../components/NavBar";
import Cookies from "universal-cookie";
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export default function PostUser() {
  const cookies = new Cookies();
  const [posts, setPosts] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [payload, setPayload] = useState({
    title: "",
    type: "",
    downloadUrl: "defaultDownloadUrl", 
    mediaUrl: null,
  });


  useEffect(() => {
    const fetchPosts = async () => {
      try {
        const response = await axios.get(
          `${process.env.REACT_APP_URL}post/get-all-user-posts`,
          {
            headers: {
              Authorization: `Bearer ${cookies.get("token")}`,
            },
          }
        );
        setPosts(response.data);
      } catch (error) {
        console.error("Error fetching posts:", error);
      }
    };

    fetchPosts();
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setPayload((prevPayload) => ({ ...prevPayload, [name]: value }));
  };

  const handleFileChange = (e) => {

    const file = e.target.files[0];
    console.log(file)
    setPayload((prevPayload) => ({ ...prevPayload, mediaUrl: file }));
  };

  const handleCreatePost = async (e) => {
    e.preventDefault();
  
    const formData = new FormData();
    formData.append("title", payload.title);
    formData.append("type", payload.type);
    formData.append("downloadUrl", payload.downloadUrl); 
    formData.append("MediaUrl", payload.mediaUrl); 
  
    try {
      const response = await axios.post(
        `${process.env.REACT_APP_URL}post/create-post`,
        formData,
        {
          headers: {
            Authorization: `Bearer ${cookies.get("token")}`,
            "Content-Type": "multipart/form-data",
          },
        }
      );  

      if (response.status === 200) {
        toast.success(response.data.message);
      }else{
        toast.warning("L'action a échoué.");
      }
      setIsModalOpen(false);
      const updatedPosts = await axios.get(`${process.env.REACT_APP_URL}post/get-all-user-posts`, {
        headers: {
          Authorization: `Bearer ${cookies.get("token")}`,
        },
      });
      setPosts(updatedPosts.data);
    } catch (error) {
      console.error("Error creating post:", error);
    }
  };

  const HandelDeletePost = async (postId) => {
    try {
        const response = await axios.delete(
          `${process.env.REACT_APP_URL}post/delete-post/${postId}`,
          {
            headers: {
              Authorization: `Bearer ${cookies.get("token")}`,
              "Content-Type": "multipart/form-data",
            },
          }
        );  
  
        if (response.status === 200) {
          toast.success(response.data.message);
        }else{
          toast.warning("L'action a échoué.");
        }

        const updatedPosts = await axios.get(`${process.env.REACT_APP_URL}post/get-all-user-posts`, {
            headers: {
              Authorization: `Bearer ${cookies.get("token")}`,
            },
          });
          setPosts(updatedPosts.data);
      } catch (error) {
        console.error("Error creating post:", error);
      }
    }
  

  return (
    <>
      <NavBar/>
      <div className="bg-white py-24 sm:py-32">
        <div className="mx-auto max-w-7xl px-6 lg:px-8">
          <button
            type="button"
            className="mb-5 rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 m-2"
            onClick={() => setIsModalOpen(true)}
          >
            Créer un poster
          </button>
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
                <div className="mt-6 ">
                  <a
                    href={post.downloadUrl}
                    className="mr-1 rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500"
                    download
                  >
                    Télécharger
                  </a>
                  <button
                    onClick={() => HandelDeletePost(post.id)}
                    className="rounded-md bg-red-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-red-500"
                                    >
                    Supprimer
                  </button>
                </div>
              </li>
            ))}
          </ul>
        </div>
      </div>
      {isModalOpen && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
          <div className="bg-white rounded-lg p-8 shadow-lg max-w-md w-full">
            <h3 className="text-xl font-semibold text-gray-900 mb-4">Créer un poster</h3>
            <form className="space-y-4" onSubmit={handleCreatePost}>
              <input
                type="text"
                name="title"
                placeholder="Title"
                value={payload.title}
                onChange={handleInputChange}
                className="w-full border rounded-md px-4 py-2"
                required
              />
              <input
                type="file"
                onChange={handleFileChange}
                className="w-full border rounded-md px-4 py-2"
                required
              />
              <input
                type="text"
                name="type"
                placeholder="Type"
                value={payload.type}
                onChange={handleInputChange}
                className="w-full border rounded-md px-4 py-2"
                required
              />
              <div className="mt-6 flex justify-end space-x-4">
                <button
                  type="button"
                  className="bg-gray-200 px-4 py-2 rounded-md"
                  onClick={() => setIsModalOpen(false)}
                >
                  Annuler
                </button>
                <button
                  type="submit"
                  className="bg-indigo-600 text-white px-4 py-2 rounded-md"
                >
                  Créer
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
