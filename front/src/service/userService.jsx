import axios from 'axios';
import Cookies from "universal-cookie";

const cookies = new Cookies();

export const getUser = async () => {
  try {
    const response = await axios.get(`${process.env.REACT_APP_URL}user/get-user`, {
      headers: {
        Authorization: `Bearer ${cookies.get("token")}`,
      },
    });
    return response;
  } catch (error) {
    throw new Error("Erreur");
  }
};

export const getAllUsers = async () => {
  try {
    const response = await axios.get(`${process.env.REACT_APP_URL}user/get-all-users`);
    return response; 
  } catch (error) {
    throw new Error("Erreur");
  }
}

export const updateUserProfile = async (profilePrivacy, userId) => {
  try {
    const response = await axios.put(
      `${process.env.REACT_APP_URL}user/update-user/${userId}`, 
      {profilePrivacy}, 
      {
        headers: {
          Authorization: `Bearer ${cookies.get("token")}`, 
          "Content-Type": "application/json"
        },
      }
    );
    return response;
  } catch (error) {
    throw new Error("Erreur"); 
  }
};



