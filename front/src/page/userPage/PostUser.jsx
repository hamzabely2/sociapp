import React, { useEffect, useState } from "react";
import NavBar from "../components/NavBar";
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { getAllUserPosts, createPost, deletePost } from "../../service/postService";

export default function PostUser() {
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
        const data = await getAllUserPosts();
        console.log(data)
        setPosts(data.data.respose);
      } catch (error) {
        toast.error(error);
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
      const data = await createPost(formData);
      toast.success(data.data.message);
      setIsModalOpen(false);
      const updatedPosts = await getAllUserPosts();
      setPosts(updatedPosts.data.respose);
    } catch (error) {
      toast.error(error);
    }
  };

  const handleDeletePost = async (postId) => {
    try {
      const data = await deletePost(postId);
      toast.success(data.data.message);
      const updatedPosts = await getAllUserPosts();
      setPosts(updatedPosts.data.respose);
    } catch (error) {
      toast.error(error);
    }
  };

  return (
    <>
      <NavBar />
      <div className="bg-white py-24 sm:py-32">
        <div className="mx-auto max-w-7xl px-6 lg:px-8">
          <button
            type="button"
            className="mb-5 rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 m-2"
            onClick={() => setIsModalOpen(true)}
          >
            Créer un post
          </button>
          <ul
            role="list"
            className="mx-auto mt-20 grid max-w-2xl grid-cols-1 gap-x-8 gap-y-16 sm:grid-cols-2 lg:mx-0 lg:max-w-none lg:grid-cols-3"
          >
            {posts.map((post) => (
              <li key={post.id} className="flex flex-col">
                {post.downloadUrl ? (
  post.downloadUrl.endsWith(".mp4") || 
  post.downloadUrl.endsWith(".webm") || 
  post.downloadUrl.endsWith(".ogg") ? (
    <video
      controls
      className="aspect-[3/2] w-full rounded-2xl object-cover"
    >
      <source src={post.downloadUrl} type="video/mp4" />
      Votre navigateur ne supporte pas les vidéos.
    </video>
  ) : (
    <img
      alt=""
      src={post.downloadUrl}
      className="aspect-[3/2] w-full rounded-2xl object-cover"
    />
  )
) : (
  <img
    alt="Contenu indisponible"
    src="URL_DE_L_IMAGE_PAR_DÉFAUT"
    className="aspect-[3/2] w-full rounded-2xl object-cover"
  />
)}
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
                    className="mr-1 rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500"
                    download
                  >
                    Télécharger
                  </a>
                  <button
                    onClick={() => handleDeletePost(post.id)}
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
            <h3 className="text-xl font-semibold text-gray-900 mb-4">Créer un post</h3>
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
