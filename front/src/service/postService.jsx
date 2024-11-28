import axios from "axios";
import Cookies from "universal-cookie";

const cookies = new Cookies();
const API_URL = process.env.REACT_APP_URL;

export const getFollowedPosts = async () => {
  try {
    const response = await axios.get(`${API_URL}post/get-follow-posts`, {
      headers: {
        Authorization: `Bearer ${cookies.get("token")}`,
      },
    });
    return response;
  } catch (error) {
    throw error;
  }
};


export const getAllUserPosts = async () => {
  try {
    const response = await axios.get(`${process.env.REACT_APP_URL}post/get-all-user-posts`, {
      headers: {
        Authorization: `Bearer ${cookies.get("token")}`,
      },
    });
    return response;
  } catch (error) {
    throw error;
  }
};

export const createPost = async (formData) => {
  try {
    const response = await axios.post(`${process.env.REACT_APP_URL}post/create-post`, formData, {
      headers: {
        Authorization: `Bearer ${cookies.get("token")}`,
        "Content-Type": "multipart/form-data",
      },
    });
    return response;
  } catch (error) {
    throw error;
  }
};

export const deletePost = async (postId) => {
  try {
    const response = await axios.delete(`${API_URL}post/delete-post/${postId}`, {
      headers: {
        Authorization: `Bearer ${cookies.get("token")}`,
      },
    });
    return response;
  } catch (error) {
    throw error;
  }
};
