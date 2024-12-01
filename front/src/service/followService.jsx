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
    throw new Error("Erreur");
  }
};

export const followUser = async (userIdFollowed, userId) => {
  try {
    const response = await axios.post(
      `${process.env.REACT_APP_URL}follow/followe-user/${userIdFollowed}`,
      userId, 
      {
        headers: {
          Authorization: `Bearer ${cookies.get("token")}`,
          "Content-Type": "application/json",
        },
      }
    );
    return response;
  } catch (error) {
    return error;
  }
};


export const unfollowUser = async (userIdFollowed,userId) => {
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
    throw new Error("Erreur");
  }
};
