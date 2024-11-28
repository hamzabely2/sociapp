import axios from "axios";
import Cookies from "universal-cookie";
const cookies = new Cookies();

export const getFollowedUsers = async () => {
  try {
    const response = await axios.get(`${process.env.REACT_APP_URL}follow/get-list-followed-users`, {
      headers: {
        Authorization: `Bearer ${cookies.get("token")}`,
      },
    });
    return response;
  } catch (error) {
    throw error;
  }
};

export const followUser = async (userIdFollowed) => {
    try {
      const response = await axios.post(
        `${process.env.REACT_APP_URL}follow/followe-user/${userIdFollowed}`,{
          headers: {
            Authorization: `Bearer ${cookies.get("token")}`,
          },
        }
      );
      return response; 
    } catch (error) {
      throw error; 
    }
  };

export const unfollowUser = async (userIdFollowed) => {
  try {
    const response = await axios.delete(
      `${process.env.REACT_APP_URL}follow/unfollowe-user/${userIdFollowed}`, {
        headers: {
          Authorization: `Bearer ${cookies.get("token")}`,
        },
      }
    );
    return response;
  } catch (error) {
    throw error;
  }
};
